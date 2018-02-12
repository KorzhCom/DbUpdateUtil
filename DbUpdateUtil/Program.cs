using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

using DbUpdateUtil.DateUpdate;


namespace DbUpdateUtil
{
    class Program {
        static void Main(string[] args) {

            Console.WriteLine("update utility version 1.0. Copyright (c) 2018 Korzh.com");
            Console.WriteLine("Current dir: " + Directory.GetCurrentDirectory());

            string configFileName = getConfigFileName(args);

            if (!string.IsNullOrEmpty(configFileName)) {
                if (File.Exists(configFileName)) {
                    var dateUpdaterInfoList = JsonConvert.DeserializeObject<List<DateUpdaterInfo>>(File.ReadAllText(configFileName));

                    foreach (var dateUpdaterInfo in dateUpdaterInfoList) {
                        try {
                            var type = dateUpdaterInfo.UpdaterType;
                            Console.WriteLine("Updating " + dateUpdaterInfo.Path + " ...");
                            DateUpdater dateUpdater = null;
                            if (type.ToLower() == "xml") {
                                dateUpdater = new XmlDateUpdater(
                                    dateUpdaterInfo.Path.Split(";"),
                                    dateUpdaterInfo.AddInfo?.Split(";"),
                                    dateUpdaterInfo.DateFormats?.Split(";"),
                                    dateUpdaterInfo.OutputFromat
                                );
                            }
                            else {
                                if (type.ToLower() == "sql") {
                                    dateUpdater = new SqlDateUpdater(
                                        dateUpdaterInfo.Path.Split(";"),
                                        dateUpdaterInfo.AddInfo,
                                        dateUpdaterInfo.DateFormats?.Split(";"),
                                        dateUpdaterInfo.OutputFromat
                                    );
                                }
                            }

                            Update(dateUpdater);
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message);
                        }
                        
                    }

                }
                else {
                    Console.WriteLine("File not found: " + configFileName);
                }
            }

#if DEBUG 
            Console.WriteLine("Done. Press enter");
            Console.ReadKey();
#endif
        }

        static void Update(IDateUpdate dateUpdate) {

            var biggestDate = dateUpdate.FindBiggestDate();

            dateUpdate.UpdateAllDates(biggestDate);

        }

        static string getConfigFileName(string[] args) {

            string helpLine = "udpate <path to config file>\n";

            if (args.Length == 1) {
                string param = args[0];
                if (param == "--help" || param == "\\h" || param == "-h") {
                    Console.WriteLine(helpLine);
                    return string.Empty;
                } else {
                    return param;
                }
            }
            else {
                Console.WriteLine(helpLine);
                return string.Empty;
            }

        }

    }
}
