
using Google.Cloud.Storage.V1;
using System.Text.Json;
using System.Text;
using Google.Apis.Auth.OAuth2;
using System.Net.Http.Headers;

namespace webapi.Services;

public class UploadService
{

    private readonly StorageClient _storageService;
    private readonly string bucketName = "receiptify";

    public UploadService()
    {
        _storageService = StorageClient.Create();

    }

    public async Task<string> UploadImage(IFormFile file)
    {
        var fileName = "images/" + Guid.NewGuid() + Path.GetExtension(file.FileName);

        using var stream = file.OpenReadStream();

        var uploadResult = await _storageService.UploadObjectAsync(bucketName, fileName, file.ContentType, stream, new UploadObjectOptions { PredefinedAcl = PredefinedObjectAcl.PublicRead });

        return uploadResult.Name;
    }

    public async Task<JsonDocument> TriggerWorkflow(string token, string filename)
    {
        var url = "https://workflowexecutions.googleapis.com/v1/projects/receiptify-438806/locations/us-central1/workflows/receipt-upload/executions";

        var payload = new
        {
            argument = JsonSerializer.Serialize(new
            {
                token,
                filename
            })
        };

        var jsonPayload = JsonSerializer.Serialize(payload);
        var client = new HttpClient();
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        // ***** credentials stuff

        GoogleCredential credential = await GoogleCredential
           .GetApplicationDefaultAsync();

        if (credential.IsCreateScopedRequired)
        {
            credential = credential.CreateScoped(new[]
            {
                "https://www.googleapis.com/auth/cloud-platform"
            });
        }

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // *****

        // TODO: REMOVE POLLING & REPLACE WITH PUB/SUB, cloud functions, and SignalR event driven pipeline


        var response = await client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var json = JsonDocument.Parse(result);
        var executionName = json.RootElement.GetProperty("name").GetString();

        if (executionName != null && executionName.Length != 0)
        {
            try
            {
                // poll endpoint for results
                var output = await GetExecutionResultAsync(executionName, accessToken);

                var workflowResults = output.RootElement.GetProperty("result").GetString();

                if (workflowResults != null && workflowResults.Length > 0)
                {
                    return JsonDocument.Parse(workflowResults);
                }
                else
                {
                    // workflow results are empty
                    throw new Exception("Workflow Error");
                }



            }
            catch
            {
                // workflow failed or cancelled
                throw new Exception("Failed to poll results.");
            }

        }
        else
        {
            // failed to start workflow
            throw new Exception("Failed to trigger workflow.");
        }
    }

    public static async Task<JsonDocument> GetExecutionResultAsync(string executionName, string accessToken)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        // poll endpoint until state = "SUCCEEDED"
        while (true)
        {
            var response = await httpClient.GetAsync($"https://workflowexecutions.googleapis.com/v1/{executionName}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var state = json.RootElement.GetProperty("state").GetString();

            if (state == "SUCCEEDED")
            {
                // Return the result string
                return json;
            }
            else if (state == "FAILED" || state == "CANCELLED")
            {
                throw new Exception($"Workflow execution ended with state: {state}");
            }
            else
            {
                // Still running - wait and poll again
                await Task.Delay(1000);
            }
        }
    }
}