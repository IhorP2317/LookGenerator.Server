namespace LookGenerator.Application.Settings ;

    public class IdentityHttpClientSettings
    {
        public string ClientName { get; set; } = string.Empty;
        public string BaseAddress { get; set; } = string.Empty;
        public string RegisterEndpoint { get; set; } = string.Empty;
        public string RegisterAdminEndpoint { get; set; } = string.Empty;
        public string ConfirmEmailEndpoint { get; set; } = string.Empty;
    }