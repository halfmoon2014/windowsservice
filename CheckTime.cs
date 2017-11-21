using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
namespace ZKDataUpLoad
{
    [XmlRoot("Root")]
    public class CheckTime
    {
        private string _badgeNumber;
        private string _days;
        private string _statu;
        private string _checktimeStart;
        private string _checktimeEnd;
        private string _lateMinutes;
        private string _earlyMinutes;
        [XmlAttribute("bgNum")]
        public string BadgeNumber
        {
            get { return _badgeNumber; }
            set { _badgeNumber = value; }
        }
        [XmlAttribute("dy")]
        public string days
        {
            get{
                if (_days == null)
                    _days = "0";
                return _days;
            }
            set { 
                _days = value; 
            }
        }
        [XmlAttribute("Stat")]
        public string statu
        {
            get
            {
                if (_statu == null)
                    _statu = "0";
                return _statu;
            }
            set { _statu = value; }
        }
        [XmlAttribute("Ch1")]
        public string ChecktimeStart
        {
            get
            {
                if (_checktimeStart == null)
                    _checktimeStart = "1900-01-01";
                return _checktimeStart;
            }
            set { _checktimeStart = value; }
        }
        [XmlAttribute("Ch2")]
        public string ChecktimeEnd
        {
            get
            {
                if (_checktimeEnd == null)
                    _checktimeEnd = "1900-01-01";
                return _checktimeEnd;
            }
            set { _checktimeEnd = value; }
        }
        [XmlAttribute("Min1")]
        public string LateMinutes
        {
            get
            {
                if (_lateMinutes == null)
                    _lateMinutes = "0";
                return _lateMinutes;
            }
            set { _lateMinutes = value; }
        }
        [XmlAttribute("Min2")]
        public string EarlyMinutes
        {
            get
            {
                if (_earlyMinutes == null)
                    _earlyMinutes = "0";
                return _earlyMinutes;
            }
            set { _earlyMinutes = value; }
        }
    }
}
