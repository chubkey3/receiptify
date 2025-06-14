using FirebaseAdmin.Auth;

namespace webapi.Services;

public class FirebaseAuthService
{
    public async Task<FirebaseToken> VerifyTokenAsync(string idToken)
    {
        return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    }
}
