using System.Text.Json.Serialization;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    /// <summary>
    /// Acknowledgement response message. [type = ack]
    /// </summary>
    class Acknowledgement
    {
        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The id.
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
