using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace PumaCode.SvnDotNet.UnitTests.SubversionSharp {

    /// <summary>
    /// Constructor is public in order to be called by Serializer but shouldn't be
    /// called directly by unit tests; use GetTestLog instead.
    /// </summary>
    [XmlRoot("log")]
    public class SvnTestLog {

        private LogEntry[] _logEntries;
        private int _headRevision;

        public static SvnTestLog GetTestLog(string xmlPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SvnTestLog));
            SvnTestLog result;

            using(StreamReader reader = new StreamReader(xmlPath)) {
                result = (SvnTestLog) serializer.Deserialize(reader);
            }

            return result;
        }

        /// <summary>
        /// Gets the head revision number of the test repository.
        /// </summary>
        public int HeadRevision
        {
            get { return _headRevision; }
        }

        /// <summary>
        /// Gets the head log entry, or null if LogEntries is not defined
        /// </summary>
        public LogEntry HeadLogEntry
        {
            get
            {
                if(_logEntries == null)
                    return null;

                return GetLogEntry(_headRevision);
            }
        }

        /// <summary>
        /// Gets the specified LogEntry, or null if not found
        /// </summary>
        public LogEntry GetLogEntry(int revision)
        {
            // This is pretty inefficient but should serve for unit testing purposes as long as
            // the test repo stays pretty small
            if(_logEntries == null)
                return null;

            foreach(LogEntry logEntry in _logEntries) {
                if(logEntry.Revision == revision)
                    return logEntry;
            }

            return null;
        }

        [XmlElement("logentry")]
        public LogEntry[] LogEntries
        {
            get { return _logEntries; }

            set
            {
                _logEntries = value;

                _headRevision = 0;

                foreach(LogEntry logEntry in _logEntries) {
                    if(logEntry.Revision > _headRevision)
                        _headRevision = logEntry.Revision;
                }
            }
        }
    }

    public class LogEntry {
        private int _revision;
        private string _author;
        private string _dateString;
        private string _message;

        [XmlAttribute("revision")]
        public int Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }

        [XmlElement("author")]
        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        [XmlElement("date")]
        public string DateString
        {
            get { return _dateString; }
            set { _dateString = value; }
        }

        [XmlElement("msg")]
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
    }
}
