using System;
using System.Collections.Generic;
using System.Text;

namespace DbUpdateUtil.DateUpdate
{
    /// <summary>
    /// Interface, which contains methods for updating dates
    /// </summary>
    interface IDateUpdate {

        /// <summary>
        /// Parse information and finds the biggest date 
        /// </summary>
        /// <returns></returns>
        DateTime FindBiggestDate();

        /// <summary>
        /// Finds the delta of years for passed date and the current date.
        /// Shift all dates in the current files by this delta.
        /// </summary>
        /// <param name="date"></param>
        void UpdateAllDates(DateTime date);
    }
}
