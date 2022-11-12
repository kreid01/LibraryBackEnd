using Newtonsoft.Json;

namespace LibrayBackEnd.Models
{
    public class CreatePaymentIntentRequest
    {
        [JsonProperty("price")]
        public long Price { get; set; }
    }
}
