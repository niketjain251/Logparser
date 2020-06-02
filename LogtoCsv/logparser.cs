using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using static LogtoCsv.LogDetails;

namespace LogtoCsv {
    class LogParser {
        static void Main (string[] args) {
            List<string> file_list = new List<string> ();

            List<string> Word = new List<string> ();

            List<data> logs = new List<data> ();

            string[] csv_path = new string[1];

            if (args.Length == 0) {
                string helpinfo = "Usage: logParser --log-dir <dir> --log-level <level> --csv <out> \n --log-dir   Directory to parse recursively for .log files \n --csv Out file-path (absolute/relative)";
                Console.WriteLine (helpinfo);
            } else {
                FileLogLevelData (args, file_list, Word, csv_path);

                foreach (var file_path in file_list) {
                    ExtractLogData (logs, file_path, Word);

                }

                if (csv_path[0] != null)
                    AddLogDataToCsv (logs, csv_path);
                else
                    Console.WriteLine ("Csv Path No Defined");
            }

        }

        private static void FileLogLevelData (string[] args, List<string> file_list, List<string> Word, string[] csv_path) {
            for (int i = 0; i < args.Count (); i++) {
                if (args[i] == "--log-dir") {
                    string DirPath = args[i + 1];
                    if (Directory.Exists (DirPath))
                        Directory.GetFiles (DirPath).ToList ().ForEach (s => file_list.Add (s));
                    else
                        Console.WriteLine ("Log Directory File Path not found");
                } else if (args[i] == "--log-level") {
                    string[] levelInfo = args[i + 1].Split ("|");
                    foreach (var level in levelInfo) {
                        Word.Add (level.ToUpper ());
                    }

                } else if (args[i] == "--csv") {
                    var csv_file_name = args[i + 1];
                    var foldername = @"\CsvFiles\";
                    if (!Directory.Exists (Directory.GetCurrentDirectory () + foldername)) {
                        Directory.CreateDirectory (Directory.GetCurrentDirectory () + foldername);
                        using (FileStream fs = File.Create (Directory.GetCurrentDirectory () + foldername + csv_file_name + ".csv")) {
                            csv_path[0] = fs.Name;
                        }

                    } else {
                        using (FileStream fsc = File.Create (Directory.GetCurrentDirectory () + foldername + csv_file_name + ".csv")) {
                            csv_path[0] = fsc.Name;
                        }
                    }

                }
            }
        }

        private static void ExtractLogData (List<data> logs, string path, List<string> Word) {
            using (StreamReader sr = new StreamReader (path)) {
                string line;

                while ((line = sr.ReadLine ()) != null) {

                    bool logLevel = Word.Any (line.Contains);

                    if (logLevel) {
                        var log = Regex.Split (line, " ");
                        String[] Messagedata = line.Split (":.");
                        string datestr = log[0] + " " + log[1];
                        string level = log[2].Split (":") [0];
                        DateTime LogDateTime = DateTime.ParseExact (datestr, "MM/dd H:mm:ss", CultureInfo.CurrentCulture);
                        string logTime = LogDateTime.ToShortTimeString () + " " + LogDateTime.ToString ("tt");
                        string LogMsgData = Messagedata[1].Trim ().Replace (",", "");
                        logs.Add (new data { LogDate = LogDateTime.ToLongDateString (), LogTime = logTime, LogLevel = level, LogMessage = LogMsgData });
                    }

                }
            }
        }

        private static void AddLogDataToCsv (List<data> logs, string[] path) {
            System.Text.StringBuilder sb = new System.Text.StringBuilder ();
            sb.AppendLine ("Level,Date,Time,Text");
            foreach (var record in logs) {
                sb.AppendLine (record.ToString ());
            }
            System.IO.File.WriteAllText (path[0], sb.ToString ());
        }

    }
}