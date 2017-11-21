using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using nrWebClass;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
namespace LLWebService
{
    /// <summary>
    /// Service1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod(EnableSession = true)]
        public string LoginIn(string name, string pwd)
        {
            StringBuilder sb = new StringBuilder();
            LoginBLL login = new LoginBLL();
            LiLanzModel.User u = login.LoginCheck(name, pwd);
            if (u.Userid > 0)
            {
                sb.Append("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                sb.Append(String.Format("<usr u_id=\"{0}\" u_name=\"{1}\" u_cname=\"{2}\">", u.Userid, u.Name, u.Cname));
                sb.Append("</usr>");
           
                return sb.ToString();
            }
            else
                return "";
           
        }
        [WebMethod]
        public string Customers(int userid)
        {
            LoginBLL login = new LoginBLL();
            return login.getCustomers(userid);
        }
        [WebMethod]
        public string Stocks(string tzid)
        {
            return nrWebClass.ERP.Base.GetStockXml(tzid);
        }
        [WebMethod]
        public string billUpload(string xml)
        {
            kcdj _kcdj = new kcdj("");
            return _kcdj.billSave(xml);
        }
        /// <summary>
        /// 会员信息
        /// </summary>
        /// <param name="CardSn"></param>
        /// <returns></returns>
        [WebMethod]
        public string PersonerInfo(string CardSn)
        {
            StringBuilder sb = new StringBuilder();
            try
            {               
                if(ConfigurationManager.AppSettings["CofeePosCardType"] == "1")
                    CardSn = (100000000 + DeCode1(CardSn)).ToString().Remove(0,1);
                string ConnString = ConfigurationManager.AppSettings["cfsf"];
                nrWebClass.DAL.SqlDbHelper dal = new nrWebClass.DAL.SqlDbHelper(ConnString);
                SqlParameter[] paramters = new SqlParameter[]{
                  new SqlParameter("@CardSn", int.Parse(CardSn))
                };
                LiLanzDAL dal2 = new LiLanzDAL();
                
                using (IDataReader reader = dal.ExecuteReader(@"select t2.DeptName,CardSnr,CustomerName,
t1.CustomerNo,t2.DeptNo,t1.AccountNo,isnull(t1.AccStat,0),isnull(t1.CardStat,0),t1.sex
from tb_Customer_coffee as t1
inner join tb_Department as t2 on t1.DeptNo=t2.DeptNo where CardSnr=@CardSn", CommandType.Text, paramters))
                {
                   
                    if (reader.Read())
                    {
                        if (reader[6].ToString() == "0" && reader[7].ToString() == "0")
                        {
                            int isLeave = 0;
                            if (IsLeave(reader[5].ToString()))
                                isLeave = 1;

                            sb.Append("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                            sb.Append(String.Format("<CardInfo Dept=\"{0}\" CardNo=\"{1}\" Cname=\"{2}\" PersonSn=\"{3}\" DeptNo=\"{4}\" AccountNo=\"{5}\" Sex=\"{6}\" isLeave=\"{7}\">",
                                reader[0], CardSnFill(reader[1].ToString()), reader[2], reader[3], reader[4], reader[5], reader["sex"], isLeave));
                            sb.Append("</CardInfo>");
                        }
                        //SqlParameter[] paramters2 = new SqlParameter[]{
                        //  new SqlParameter("@AccountNo", int.Parse(reader[5].ToString()))
                        //};
                        //if (dal2.ExecuteScalar("select 1 from cy_t_coffeeStopSign where AccountNo=@AccountNo",
                        //    CommandType.Text, paramters2) != null)
                        //    sb.Length = 0;
                        
                    }
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                sb.Append(ex.ToString());
            }
            return sb.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CardSn"></param>
        /// <returns></returns>
        [WebMethod]
        public string PersonerInfoNoEncrypt(string CardSn)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                string ConnString = ConfigurationManager.AppSettings["cfsf"];
                nrWebClass.DAL.SqlDbHelper dal = new nrWebClass.DAL.SqlDbHelper(ConnString);
                SqlParameter[] paramters = new SqlParameter[]{
                  new SqlParameter("@CardSn", int.Parse(CardSn))
                };
                IDataReader reader = dal.ExecuteReader(@"select t2.DeptName,CardSnr,CustomerName,
t1.CustomerNo,t2.DeptNo,t1.AccountNo,isnull(t1.AccStat,0) AccStat,isnull(t1.CardStat,0) CardStat,t1.sex
from tb_Customer_coffee as t1
inner join tb_Department as t2 on t1.DeptNo=t2.DeptNo where CardSnr=@CardSn", CommandType.Text, paramters);
                if (reader.Read())
                {
                    if (reader[6].ToString() == "0" && reader[7].ToString() == "0")
                    {
                        int isLeave = 0;
                        if (IsLeave(reader[5].ToString()))
                            isLeave = 1;

                        //Log.Info(IsLeave(reader[5].ToString()).ToString());

                        sb.Append("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                        sb.Append(String.Format("<CardInfo Dept=\"{0}\" CardNo=\"{1}\" Cname=\"{2}\" PersonSn=\"{3}\" DeptNo=\"{4}\" AccountNo=\"{5}\" Sex=\"{6}\" isLeave=\"{7}\">",
                            reader[0], CardSnFill(reader[1].ToString()), reader[2], reader[3], reader[4], reader[5], reader["sex"], isLeave));
                        sb.Append("</CardInfo>");
                    }
                }
                reader.Dispose();
            }
            catch (Exception ex)
            {
                sb.Append(ex.ToString());
            }
            finally
            {

            }
            return sb.ToString();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="opwd"></param>
        /// <param name="npwd"></param>
        /// <returns></returns>
        [WebMethod]
        public string ChangePassword(int id, string opwd, string npwd)
        {
            LoginBLL login = new LoginBLL();
            string result = login.ChangePW(id, opwd, npwd);
            if (result == "ok")
                return "Successed";
            else
                return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CardID"></param>
        /// <returns></returns>
        private int DeCode1(string CardID)
        {
            try
            {
                if (CardID.Length != 8)
                    return 0;
                else
                {
                    string First3 = CardID.Substring(0, 3);
                    string Last5 = CardID.Substring(3, 5);

                    string First3Base16 = Convert.ToString(Convert.ToInt32(First3), 16);
                    string Last5Base16 = Convert.ToString(Convert.ToInt32(Last5), 16);

                    Last5Base16 = "0000".Remove(0, Last5Base16.Length) + Last5Base16;

                    string NewCardIDBase16 = First3Base16 + Last5Base16;
                    int NewCardID = Convert.ToInt32(NewCardIDBase16, 16);

                    return NewCardID;
                }
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// 补位
        /// </summary>
        /// <param name="sn"></param>
        /// <returns></returns>
        private string CardSnFill(string sn)
        {
            return "00000000".Remove(0, sn.Length) + sn;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AccountNo"></param>
        /// <returns></returns>
        private bool IsLeave(string AccountNo) 
        {
            String sql = String.Format("SELECT 1 FROM cy_t_coffeeStopSign WHERE AccountNo = '{0}'", AccountNo);
            //Log.Info(sql);
            nrWebClass.LiLanzDAL dal = new LiLanzDAL();
            Object rel = dal.ExecuteScalar(sql);
            //Log.Info(rel.GetType().ToString());
            if (rel == null)
                return false;
            return true;
        }
    }
}