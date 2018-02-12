using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DbUpdateUtil.DateUpdate
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DateUpdater : IDateUpdate {

        /// <summary>
        /// Contains names of files 
        /// </summary>
        protected string[] _fileNames;

        /// <summary>
        /// Date fromats in regular expresion type
        /// </summary>
        protected List<Regex> _dateFormats = new List<Regex>();

        /// <summary>
        /// Output fromat for dates
        /// </summary>
        protected string _outputFormat;

        public DateUpdater(string[] fileNames, string outputFormat = null) {
            _fileNames = fileNames;
            _outputFormat = outputFormat;
            _dateFormats = new List<Regex>();
        }

        protected bool RightFormat(string dateString) {
            foreach (var format in _dateFormats) {
                if (format.IsMatch(dateString)) {
                    return true;
                }
            }

            return false;
        }

        // <summary>
        /// Parse information and finds the biggest date
        /// </summary>
        /// <returns></returns>
        public abstract DateTime FindBiggestDate();

        /// <summary>
        /// Finds the delta of years for passed date the current date.
        /// Shift all dates in the current infromation by this delta.
        /// </summary>
        public abstract void UpdateAllDates(DateTime date);
    }
}
