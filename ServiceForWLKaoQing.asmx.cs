using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using ZKDataUpLoad;
using System.Xml;
using System.Data.SqlClient;
using nrWebClass;
using System.Data;
using System.Configuration;
namespace LLWebService
{
    /// <summary>
    /// ServiceForWLKaoQing 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class ServiceForWLKaoQing : System.Web.Services.WebService
    {

        [WebMethod]
        public string UpdateLoadData(string xml, string dayStart, string dayEnd)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<CheckTime>));
            // A FileStream is needed to read the XML document.   
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(xml));
            List<CheckTime> AddressList = (List<CheckTime>)serializer.Deserialize(reader);
            int rowCount = 0;
            LiLanzDAL dal = new LiLanzDAL();
            try
            {
                string sql = "DELETE FROM dbo.kq_T_rydkmx WHERE ChecktimeStart>=@start and ChecktimeStart<@end";
                SqlParameter[] paramters = new SqlParameter[]{
                    new SqlParameter("@start", DateTime.Parse(dayStart)),
                    new SqlParameter("@end", DateTime.Parse(dayEnd).AddDays(1))
                };
                dal.ExecuteNonQuery(sql, CommandType.Text, paramters);
                foreach (CheckTime ch in AddressList)
                {
                    if(ch.BadgeNumber.Length < 6)
                        ch.BadgeNumber = "000000".Substring(0, 6 - ch.BadgeNumber.Length) + ch.BadgeNumber;

                    sql = @" INSERT INTO dbo.kq_T_rydkmx
                (BadgeNumber, days, statu, ChecktimeStart, ChecktimeEnd, LateMinutes,EarlyMinutes) VALUES  
                (@BadgeNumber, @days, @statu, @ChecktimeStart, @ChecktimeEnd, @LateMinutes, @EarlyMinutes);";
                    paramters = new SqlParameter[]{
                  new SqlParameter("@BadgeNumber", ch.BadgeNumber),
                  new SqlParameter("@days", Decimal.Parse(ch.days)),
                  new SqlParameter("@statu", int.Parse(ch.statu)),
                  new SqlParameter("@ChecktimeStart", DateTime.Parse(ch.ChecktimeStart)),
                  new SqlParameter("@ChecktimeEnd", DateTime.Parse(ch.ChecktimeEnd)),
                  new SqlParameter("@LateMinutes", int.Parse(ch.LateMinutes)),
                  new SqlParameter("@EarlyMinutes", int.Parse(ch.EarlyMinutes))
                };
                    
                    if (dal.ExecuteNonQuery(sql, CommandType.Text, paramters) > 0)
                        rowCount++;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return rowCount.ToString();
        }
        [WebMethod]
        public string UpdateLoadDataForPerson(string xml, string dayStart, string dayEnd, string BadgeNumber)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<CheckTime>));
            // A FileStream is needed to read the XML document.   
            XmlReader reader = XmlReader.Create(new System.IO.StringReader(xml));
            List<CheckTime> AddressList = (List<CheckTime>)serializer.Deserialize(reader);
            int rowCount = 0;
            LiLanzDAL dal = new LiLanzDAL();
            try
            {
                string sql = @"DELETE FROM dbo.kq_T_rydkmx WHERE ChecktimeStart>=@start 
and ChecktimeStart<@end and BadgeNumber=@BadgeNumber";
                SqlParameter[] paramters = new SqlParameter[]{
                    new SqlParameter("@start", DateTime.Parse(dayStart)),
                    new SqlParameter("@end", DateTime.Parse(dayEnd).AddDays(1)),
                    new SqlParameter("@BadgeNumber", "000000".Substring(0, 6 - BadgeNumber.Length) + BadgeNumber)
                };
                dal.ExecuteNonQuery(sql, CommandType.Text, paramters);
                foreach (CheckTime ch in AddressList)
                {
                    ch.BadgeNumber = "000000".Substring(0, 6 - ch.BadgeNumber.Length) + ch.BadgeNumber;
                    sql = @" INSERT INTO dbo.kq_T_rydkmx
                (BadgeNumber, days, statu, ChecktimeStart, ChecktimeEnd, LateMinutes,EarlyMinutes) VALUES  
                (@BadgeNumber, @days, @statu, @ChecktimeStart, @ChecktimeEnd, @LateMinutes, @EarlyMinutes);";
                    paramters = new SqlParameter[]{
                  new SqlParameter("@BadgeNumber", ch.BadgeNumber),
                  new SqlParameter("@days", Decimal.Parse(ch.days)),
                  new SqlParameter("@statu", int.Parse(ch.statu)),
                  new SqlParameter("@ChecktimeStart", DateTime.Parse(ch.ChecktimeStart)),
                  new SqlParameter("@ChecktimeEnd", DateTime.Parse(ch.ChecktimeEnd)),
                  new SqlParameter("@LateMinutes", int.Parse(ch.LateMinutes)),
                  new SqlParameter("@EarlyMinutes", int.Parse(ch.EarlyMinutes))
                };

                    if (dal.ExecuteNonQuery(sql, CommandType.Text, paramters) > 0)
                        rowCount++;
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

            return rowCount.ToString();
        }
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
    }
}
