using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BOM.Models
{
    public class Status
    {
        public int ReturnCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    public class InputRequest
    {
        public string merchantcode { get; set; }
        public string data { get; set; }
    }
}