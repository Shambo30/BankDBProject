using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLib
{
    public class ActivityLog
    {
        public int LogId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
        public string Timestamp { get; set; }
    }
}
