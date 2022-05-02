using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    /// <summary>
    /// The status.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// The status code.
        /// </summary>
        /// <remarks>
        /// i.e. "status_code":0
        /// </remarks>
        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }

        /// <summary>
        /// The status message.
        /// </summary>
        /// <remarks>
        /// i.e. "status_message":"SUCCESS" 
        /// </remarks>
        [JsonPropertyName("status_message")]
        public string StatusMessage { get; set; }
    }
}
