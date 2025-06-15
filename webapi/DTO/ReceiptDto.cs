namespace webapi.DTO;

public class CreateReceiptDto


{

    public DateTime? UploadDate { get; set; }

    public string ReceiptUrl { get; set; } = null!;

    public string? UploadedBy { get; set; }


}