using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    /// <summary>
    /// A summary message.
    /// </summary>
    /// <remarks>
    /// {"summary":{"pressure_trend":"steady","strike_count_1h":6,"strike_count_3h":6,"precip_total_1h":0.0,"strike_last_dist":37,"strike_last_epoch":1597160656,"precip_accum_local_yesterday":0.0,"precip_accum_local_yesterday_final":0.0,"precip_analysis_type_yesterday":1,"feels_like":26.2,"heat_index":26.2,"wind_chill":26.2},"serial_number":"ST-00012575","hub_sn":"HB-00028109","type":"obs_st","source":"mqtt","obs":[[1597160716,0.49,0.98,1.43,40,3,993.6,26.2,84,49795,3.57,415,0.0,0,0,0,2.61,1,2.307425,null,null,0]],"device_id":79424,"firmware_revision":134}
    /// </remarks>
    class SummaryMessage
    {
        /// <summary>
        /// The summary.
        /// </summary>
        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }

        /// <summary>
        /// The device serial number.
        /// </summary>
        [JsonPropertyName("serial_number")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// The hub serial number.
        /// </summary>
        [JsonPropertyName("hub_sn")]
        public string HubSerialNumber { get; set; }

        /// <summary>
        /// The message type.
        /// </summary>
        /// <remarks>
        /// i.e. "type":"obs_st"
        /// </remarks>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// The source.
        /// </summary>
        /// <remarks>
        /// i.e. "source":"mqtt"
        /// </remarks>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// A list of observations.
        /// </summary>
        /// <remarks>
        /// i.e. "obs":[[1597160776,1.03,1.48,1.92,27,3,993.6,26.3,84,50991,3.63,425,0.0,0,35,1,2.61,1,2.307425,null,null,0]]
        /// </remarks>
        [JsonPropertyName("obs")]
        public List<List<double?>> Observations { get; set; }

        /// <summary>
        /// The device id.
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }

        /// <summary>
        /// The firmware revision.
        /// </summary>
        [JsonPropertyName("firmware_revision")]
        public int FirmwareRevision { get; set; }

        /// <summary>
        /// The first observation in observations.
        /// </summary>
        public Observation FirstObservation => new Observation(Observations[0]);
    }
}
