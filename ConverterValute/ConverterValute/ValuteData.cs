using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ConverterValute
{
    class ValuteData
    {
        [JsonProperty("Date")]
        public DateTime Date { get; set; }

        [JsonProperty("PreviousDate")]
        public DateTime PreviousDate { get; set; }

        [JsonProperty("PreviousURL")]
        public string PreviousURL { get; set; }

        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }

        [JsonProperty("Valute")]
        public Dictionary<string, Valute> Valute { get; set; }
    }
}
