using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveCourseClient.Models
{
    public class TableTask
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("InputsNumber")]
        public int InputsNumber { get; set; }

        [JsonProperty("ExpectedOutput")]
        public string ExpectedOutput { get; set; }

        [JsonProperty("SchemeImage")]
        public byte[] SchemeImage { get; set; }
    }
}
