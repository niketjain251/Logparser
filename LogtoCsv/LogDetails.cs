using System;

namespace LogtoCsv {
    class LogDetails {
        public class data {
            public string LogDate { get; set; }
            public string LogTime { get; set; }
            public string LogLevel { get; set; }
            public string LogMessage { get; set; }
            public override string ToString () {
                return this.LogLevel + "," + this.LogDate + "," + this.LogTime + "," + this.LogMessage;
            }
        }
    }
}