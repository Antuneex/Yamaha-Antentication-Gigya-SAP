using System.Text.Json.Serialization;

namespace IntegrationHubApi.Domain.Extensions
{
    public class OrderFilter
    {
        public string Field { get; set; }

        public OrderTypeEnum OrderType { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public enum OrderTypeEnum
        {
            asc,
            desc
        }
    }
}