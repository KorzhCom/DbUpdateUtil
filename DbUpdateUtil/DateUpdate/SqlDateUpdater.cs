using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace DbUpdateUtil.DateUpdate
{
 
    /// <summary>
    /// This class contains functions to update all date values in sql script files
    /// </summary>
    public class SqlDateUpdater : DateUpdater {

        /// <summary>
        /// States, which we can get, when parse sql-script
        /// </summary>
        enum State {
            ReadChar,
            ReadDate,
        }

        /// <summary>
        ///  
        /// </summary>
        private char _quotes;

        /// <summary>
        /// Initialize an instance of SqlDateUpdater
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="dbFromat"></param>
        public SqlDateUpdater(string[] fileNames, string dbFromat, string outputFormat = null) : base (fileNames, outputFormat) {


            switch (dbFromat.ToLower()) {
                case "msserver":
                    _quotes = '\'';
                    break;
                case "mysql":
                    _quotes = '\'';
                    break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(outputFormat)) {
                _outputFormat = "yyyy-MM-dd HH:mm:ss.000";
            }
           
            _dateFormats.Add(new Regex(@"[\d]{4}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}:[\d]{2}.[\d]{3}"));
        }

        /// <summary>
        /// Initialize an instance of SqlDateUpdater
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="dbFromat"></param>
        /// <param name="dateFormas"></param>
        public SqlDateUpdater(string[] fileNames, string dbFormat, string[] dateFormats, string outputFormat = null) : base (fileNames, outputFormat) {

            switch (dbFormat.ToLower()) {
                case "msserver":
                    _quotes = '\'';
                    break;
                case "mysql":
                    _quotes = '\'';
                    break;
                default:
                    _quotes = '\'';
                    break;
            }

            _dateFormats = new List<Regex>();
            _dateFormats.Add(new Regex(@"[\d]{4}-[\d]{2}-[\d]{2} [\d]{2}:[\d]{2}:[\d]{2}.[\d]{3}"));
            if (dateFormats != null) {
                foreach (var format in dateFormats) {
                    _dateFormats.Add(new Regex(format));
                }
            } 

            if (string.IsNullOrEmpty(outputFormat)) {
                _outputFormat = "yyyy-MM-dd HH:mm:ss.000";
            }
           
 
        }


        /// <summary>
        /// Parse sql scripts and finds the biggest date in them
        /// </summary>
        /// <returns></returns>
        public override DateTime FindBiggestDate() {
            var maxDate = DateTime.MinValue;
            foreach (var fileName in _fileNames) {
                string sqlScript = File.ReadAllText(fileName);
                State state = State.ReadChar;

                string date = "";
                foreach (var symb in sqlScript) {
                    switch (state){
                        case State.ReadChar:
                            if (symb == _quotes) {
                                state = State.ReadDate;
                            }
                            break;
                        case State.ReadDate:
                            if (symb == _quotes) {
                                state = State.ReadChar;
                               
                                if (RightFormat(date)) {
                                    DateTime currentDate = DateTime.MinValue;
                                    var result = DateTime.TryParse(date, out currentDate);

                                    if (result) {
                                       
                                        if (currentDate.Year > maxDate.Year) {
                                            maxDate = currentDate;
                                        }
                                    }
                                }
                                date = "";
                            }
                            else {
                                date += symb;
                            }
                            break;
                    }
                        

                }

            }

            return maxDate;
        }

        /// <summary>
        /// Finds the delta of years for passed date the current date.
        /// Shift all dates in the current files by this delta.
        /// </summary>
        /// <param name="date"></param>
        public override void UpdateAllDates(DateTime date) {
            var now = DateTime.Now;
            var deltaYear = now.Year - date.Year;

            foreach (var fileName in _fileNames) {
                string sqlScript = File.ReadAllText(fileName);
                State state = State.ReadChar;

                string dateString = "";

                foreach (var symb in sqlScript) {
                    switch (state) {
                        case State.ReadChar:
                            if (symb == _quotes) {
                                state = State.ReadDate;
                            }
                            break;
                        case State.ReadDate:
                            if (symb == _quotes) {
                                state = State.ReadChar;

                                if (RightFormat(dateString)) {
                                    DateTime currentDate = DateTime.MinValue;
                                    var result = DateTime.TryParse(dateString, out currentDate);

                                    if (result) {

                                        string newDateString = currentDate.AddYears(deltaYear).ToString(_outputFormat);
                                        sqlScript = sqlScript.Replace(dateString, newDateString);

                                    //    Console.WriteLine(dateString + "->" + newDateString);
                                    }
                                }
                                dateString = "";
                            }
                            else {
                                dateString += symb;
                            }
                            break;
                    }
                }

                File.WriteAllText("new-" + fileName, sqlScript);
            }

        }
    }
}
