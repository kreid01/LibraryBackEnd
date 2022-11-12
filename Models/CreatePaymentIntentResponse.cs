using Newtonsoft.Json;

namespace LibrayBackEnd.Models
{
    public class CreatePaymentIntentResponse
    {
        [JsonProperty("clientSecret")]
        public string ClientSecret { get; set; }
    }
}
