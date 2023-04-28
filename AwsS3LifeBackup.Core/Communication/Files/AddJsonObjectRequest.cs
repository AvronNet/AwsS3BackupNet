using System.Text.Json.Serialization;

namespace AwsS3LifeBackup.Core.Communication.Files
{
    public class AddJsonObjectRequest
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("timesent")]
        public DateTime TimeSent { get; set; }
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
