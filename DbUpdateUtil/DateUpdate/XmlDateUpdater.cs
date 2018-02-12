using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DbUpdateUtil.DateUpdate
{
    /// <summary>
    /// This class contains functions to update all date values in xml files
    /// </summary>
    public class XmlDateUpdater : DateUpdater {

        /// <summary>
        /// Contains name of nodes, which has DateTime type
        /// </summary>
        private List<string> _dateTimeNodesNames;


        /// <summary>
        /// Initialize an instance of XmlDateUpdater
        /// </summary>
        /// <param name="fileNames">vector of xml-file names</param>
        public XmlDateUpdater(string[] fileNames, string outputFormat = null) : base(fileNames, outputFormat) {
            _dateTimeNodesNames = new List<string>();
            _dateFormats.Add(new Regex(@"[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}"));
        }

        /// <summary>
        /// Initialize an instance of XmlDateUpdate 
        /// </summary>
        /// <param name="fileNames">vector of xml-file names</param>
        /// <param name="dateTimeNodeNames">contains node names with datetime type</param>
        public XmlDateUpdater(string[] fileNames, IEnumerable<string> dateTimeNodeNames, string outputFormat = null) : base (fileNames, outputFormat) {
            _dateTimeNodesNames = new List<string>();
            //normailze node names

            if (dateTimeNodeNames != null) {
                foreach (var dateTimeFieldName in dateTimeNodeNames) {
                    _dateTimeNodesNames.Add(dateTimeFieldName.ToLower());
                }
            }
           
            _dateFormats.Add(new Regex(@"[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}"));
        }

        /// <summary
        /// Initialize an instance of XmlDateUpdater 
        /// </summary>
        /// <param name="fileNames"></param>
        /// <param name="dateTimeNodeNames"></param>
        /// <param name="formats"></param>
        public XmlDateUpdater(string[] fileNames, IEnumerable<string> dateTimeNodeNames, string[] formats, string outputFormat = null) : base (fileNames, outputFormat) {
            _dateTimeNodesNames = new List<string>();
            //normailze node names

            if (dateTimeNodeNames != null) {
                foreach (var dateTimeFieldName in dateTimeNodeNames) {
                    _dateTimeNodesNames.Add(dateTimeFieldName.ToLower());
                }
            }

            _dateFormats.Add(new Regex(@"[\d]{4}-[\d]{2}-[\d]{2}T[\d]{2}:[\d]{2}:[\d]{2}"));

            if (formats != null) {
                foreach (var format in formats) {
                    _dateFormats.Add(new Regex(format));
                }
            }
            
        }

        /// <summary>
        /// Parse xml documents and finds the biggest date in them
        /// </summary>
        /// <returns>The biggest date</returns>
        public override DateTime FindBiggestDate() {
            DateTime maxDate = DateTime.MinValue;

            if (_dateTimeNodesNames.Count > 0) {

                foreach (var fileName in _fileNames) {
                    var xmlDoc = XDocument.Load(fileName);
                    var rootElem = xmlDoc.Root;

                    foreach (var mainNode in rootElem.Elements()) {
                        foreach (var node in mainNode.Elements()) {
                            string nodeName = node.Name.LocalName.ToLower();
                            if (_dateTimeNodesNames.Contains(nodeName)) {

                                if (RightFormat(node.Value)) {
                                    DateTime currentDate = DateTime.MinValue;
                                    var result = DateTime.TryParse(node.Value, out currentDate);

                                    if (result) {
                                        
                                        if (currentDate.Year > maxDate.Year) {
                                            maxDate = currentDate;
                                        }

                                    }
                                    else {
                                        _dateTimeNodesNames.Remove(nodeName);
                                    }
                                }
                                else {
                                    _dateTimeNodesNames.Remove(nodeName);
                                }
                            }
                        }
                    }

                  
                }
            }
            else {
                foreach (var fileName in _fileNames) {
                    var xmlDoc = XDocument.Load(fileName);
                    var rootElem = xmlDoc.Root;

                    foreach (var mainNode in rootElem.Elements()) {
                        foreach (var node in mainNode.Elements()) {

                            if (RightFormat(node.Value)) {
                                DateTime currentDate = DateTime.MinValue;
                                var result = DateTime.TryParse(node.Value, out currentDate);

                                if (result) {
                                    
                                    if (currentDate.Year > maxDate.Year) {
                                        maxDate = currentDate;
                                    }

                                    string nodeName = node.Name.LocalName.ToLower();
                                    if (!_dateTimeNodesNames.Contains(nodeName)) {
                                        _dateTimeNodesNames.Add(nodeName);
                                    }
                                }
                            }
                           
                        }
                    }
                    
                }

            }

            return maxDate;
        }

        /// <summary>
        /// Finds the delta of years for passed date andthe current date.
        /// Shift all dates in the current files by this delta.
        /// </summary>
        /// <param name="date"></param>
        public override void UpdateAllDates(DateTime date) {
            var now = DateTime.Now;
            var deltaYear = now.Year - date.Year;

            if (_dateTimeNodesNames.Count > 0) {

                foreach (var fileName in _fileNames) {
                    var xmlDoc = XDocument.Load(fileName);
                    var rootElem = xmlDoc.Root;

                    foreach (var mainNode in rootElem.Elements()) {
                        foreach (var node in mainNode.Elements()) {
                            if (_dateTimeNodesNames.Contains(node.Name.LocalName.ToLower())) {
                                DateTime currentDate;
                                var result = DateTime.TryParse(node.Value, out currentDate);

                                if (result) {
                                    var newDate = currentDate.AddYears(deltaYear);

                                    //Console.WriteLine(currentDate.ToString() + "->" + newDate.ToString());

                                    if (_outputFormat != null) {
                                        node.SetValue(newDate.ToString(_outputFormat));
                                    }
                                    else {
                                        node.SetValue(newDate);
                                    }
                                }

                            }
                        }
                    }

       
                    xmlDoc.Save("New-" + fileName);
                }

            }

        }
    }
}
