using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    /// <summary>
    /// A status message.
    /// </summary>
    /// <remarks>
    /// {"status":{"status_code":0,"status_message":"SUCCESS"},"device_id":79424,"type":"obs_st","source":"cache","summary":{"pressure_trend":"steady","strike_count_1h":6,"strike_count_3h":6,"precip_total_1h":0.0,"strike_last_dist":37,"strike_last_epoch":1597160656,"precip_accum_local_yesterday":0.0,"precip_accum_local_yesterday_final":0.0,"precip_analysis_type_yesterday":1,"feels_like":26.2,"heat_index":26.2,"wind_chill":26.2},"obs":[[1597160656,0.54,1.07,1.61,32,3,993.6,26.2,84,49154,3.54,410,0,0,37,1,2.61,1,2.307425,null,null,0]]}
    /// </remarks>
    class StatusMessage
    {
        /// <summary>
        /// The status.
        /// </summary>
        [JsonPropertyName("status")]
        public Status Status { get; set; }

        /// <summary>
        /// The device id.
        /// </summary>
        [JsonPropertyName("device_id")]
        public int DeviceId { get; set; }


        /// <summary>
        /// The message tpe.
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
        /// i.e. "source":"cache"
        /// </remarks>
        [JsonPropertyName("source")]
        public string Source { get; set; }

        /// <summary>
        /// The summary.
        /// </summary>
        [JsonPropertyName("summary")]
        public Summary Summary { get; set; }

        /// <summary>
        /// An observation. 
        /// </summary>
        /// <remarks>
        /// i.e. "obs":[[1597160656,0.54,1.07,1.61,32,3,993.6,26.2,84,49154,3.54,410,0,0,37,1,2.61,1,2.307425,null,null,0]]
        /// </remarks>
        [JsonPropertyName("obs")]
        public List<List<double?>> Observations { get; set; }

        /// <summary>
        /// The first observation in observations.
        /// </summary>
        public Observation FirstObservation => new Observation(Observations[0]);
    }
}
