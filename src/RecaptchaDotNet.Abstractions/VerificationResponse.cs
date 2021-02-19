namespace RecaptchaDotNet.Abstractions
{
    public class VerificationResponse
    {
        public bool Success { get; set; }
        public string HostName { get; set; }
    }
}