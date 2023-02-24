using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveCourseClient.Models
{
    public class SchemeTask
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("InputsNumber")]
        public int InputsNumber { get; set; }

        [JsonProperty("ContactsNumberMax")]
        public int ContactsNumberMax { get; set; }

        [JsonProperty("OrNumber")]
        public int OrNumber { get; set; }

        [JsonProperty("AndNumber")]
        public int AndNumber { get; set; }
    }
}
