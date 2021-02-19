using System.Threading.Tasks;

namespace RecaptchaDotNet.Abstractions
{
    public interface ISiteVerifier
    {
        Task<VerificationResponse> VerifyAsync(string response);
    }
}