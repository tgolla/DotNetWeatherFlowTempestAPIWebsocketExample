using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    /// <summary>
    /// Lightning strike event response message. [type = evt_strike]
    /// </summary>
    /// <remarks>
    /// Unfortunately the WeatherFlow Smart Weather API WebSocket reference (https://weatherflow.github.io/SmartWeather/api/ws.html)
    /// was out of date when putting this class together and as such the class was built using the message returned 
    /// and https://json2csharp.com/.
    /// </remarks>
    public class LightningStrikeEvent
    {
        /// <summary>
        /// The device serial number.
        /// </summary>
        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The message type.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The hub serial number.
        /// </summary>
        [JsonPropertyName("hub_sn")]
        public string HubSerialNumber { get; set; }

        /// <summary>
        /// The event data. 
        /// </summary>
        /// <remarks>
        /// i.e. "evt": [1597165492, 42, -1714, 1] - I believe these are as follows.
        /// Time - Epoch in seconds. (32bit)
        /// Distance
        /// Energy
        /// Unknown
        /// </remarks>
        [JsonPropertyName("evt")]
        public List<int> Event { get; set; }

        /// <summary>
        /// The source.
        /// </summary>
        /// <remarks>
        /// i.e. "source": "enhanced" - not exactly sure what this is.
        /// </remarks>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// The device id.
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// The time at which the lightning strike event occured at.
        /// </summary>
        public DateTime OccuredAt => DateTimeOffset.FromUnixTimeSeconds(Event[0]).DateTime;

        /// <summary>
        /// The distance of the lightning strike.
        /// </summary>
        public int Distance => Event[1];

        /// <summary>
        /// The energy of the lightning strike.
        /// </summary>
        public int Energy => Event[2];

        /// <summary>
        /// An unknown event value.
        /// </summary>
        public int Unknown => Event[3];
    }
}
