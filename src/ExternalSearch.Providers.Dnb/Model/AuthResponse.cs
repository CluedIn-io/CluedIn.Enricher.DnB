using Newtonsoft.Json;

namespace CluedIn.ExternalSearch.Providers.DnB.Model
{
    public class AuthResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expiresIn")]
        public int ExpiresIn { get; set; }
    }
}
