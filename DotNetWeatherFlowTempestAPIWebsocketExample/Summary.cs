using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    public class Summary
    {
        /// <summary>
        /// Pressure trend.
        /// </summary>
        /// <remarks>
        /// i.e. "pressure_trend":"steady"
        /// </remarks>
        [JsonPropertyName("pressure_trend")]
        public string PressureTrend { get; set; }

        /// <summary>
        /// One hour lightning strike count.
        /// </summary>
        [JsonPropertyName("strike_count_1h")]
        public int OneHourLightningStrikeCount { get; set; }

        /// <summary>
        /// Three hour lightning strike count.
        /// </summary>
        [JsonPropertyName("strike_count_3h")]
        public int ThreeHourLightningStrikeCount { get; set; }

        /// <summary>
        /// One hour precipitation total.
        /// </summary>
        [JsonPropertyName("precip_total_1h")]
        public double OneHourPrecipitationTotal { get; set; }

        /// <summary>
        /// Last lightning strike distance.
        /// </summary>
        [JsonPropertyName("strike_last_dist")]
        public int LastLightningStrikeDistance { get; set; }

        /// <summary>
        /// Last lightning strike Epoch.
        /// </summary>
        [JsonPropertyName("strike_last_epoch")]
        public int LastLightningStrikeEpoch { get; set; }

        /// <summary>
        /// Local precipitation accumulation for yesterday.
        /// </summary>
        [JsonPropertyName("precip_accum_local_yesterday")]
        public double LocalPrecipitationAccumulationForYesterday { get; set; }

        /// <summary>
        /// Final local precipitation accumulation for yesterday.
        /// </summary>
        [JsonPropertyName("precip_accum_local_yesterday_final")]
        public double FinalLocalPrecipitationAccumulationForYesterday { get; set; }

        /// <summary>
        /// Precipitation analysis type for yesterday.
        /// </summary>
        [JsonPropertyName("precip_analysis_type_yesterday")]
        public int PrecipitationAnalysisTypeForYesterday { get; set; }

        /// <summary>
        /// It feels like.
        /// </summary>
        [JsonPropertyName("feels_like")]
        public double FeelsLike { get; set; }

        /// <summary>
        /// The heat index.
        /// </summary>
        [JsonPropertyName("heat_index")]
        public double HeatIndex { get; set; }

        /// <summary>
        /// The wind chill.
        /// </summary>
        [JsonPropertyName("wind_chill")]
        public double WindChill { get; set; }
    }
}
