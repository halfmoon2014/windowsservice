using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;
namespace LLWebService
{
    [XmlRoot("Root")]
    public class AccountingInfo
    {
        [XmlAttribute("rq")]
        private DateTime rq;

        public DateTime Rq
        {
            get { return rq; }
            set { rq = value; }
        }
        [XmlAttribute("fph")]
        private String fph;

        public String Fph
        {
            get { return fph; }
            set { fph = value; }
        }
        [XmlAttribute("je")]
        private decimal je;

        public decimal Je
        {
            get { return je; }
            set { je = value; }
        }

        [XmlAttribute("ssgs")]
        private String ssgs;

        public String Ssgs
        {
            get { return ssgs; }
            set { ssgs = value; }
        }
        [XmlAttribute("dutyFee")]
        private decimal dutyFee;

        public decimal DutyFee
        {
            get { return dutyFee; }
            set { dutyFee = value; }
        }

        [XmlAttribute("amount")]
        private decimal amount;

        public decimal Amount
        {
            get { return amount; }
            set { amount = value; }
        }
        [XmlAttribute("status")]
        private Int16 status;
        /// <summary>
        /// 单据状态
        /// 1 正常填开
        /// 0 未开票
        /// 2 作废
        /// </summary>
        public Int16 Status
        {
            get { return status; }
            set { status = value; }
        }
    }
}