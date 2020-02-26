using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheCustomAnalyzeService.Helpers
{
    public class JsonSuccessMessage<T>
    {
        public string message { get; set; }
        public IEnumerable<T> data { get; set; }
    }
}
