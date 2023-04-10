using AdaptiveCourseClient.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveCourseClient.Models
{
    public class CheckTable
    {
        public string Result { get; set; }

        public int Id { get; set; }

        public CheckTable(string result, int id)
        {
            Result = result;
            Id = id;
        }
    }
}
