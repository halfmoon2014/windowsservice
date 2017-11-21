using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using System.Data;
using nrWebClass;

namespace LLWebService
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class wmsSoBoxPrint : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string SoBox(string boxsn)
        {
            SerializableDictionary<string, string> res = new SerializableDictionary<string, string>();
            nrWebClass.LiLanzDAL dbhelper = new nrWebClass.LiLanzDAL();
            Int32 id = 0, khid = 0;
            IDataReader dr = dbhelper.ExecuteReader(String.Format("SELECT top 1 id from yx_t_kcdjspid where zxxh='{0}'", boxsn));
            if (dr.Read())
                id = dr.GetInt32(0);
            else
            {
                res.Add("status", "200");
                res.Add("msg", "找不到装箱信息。");
                return JsonConvert.SerializeObject(res);
            }
            dr = dbhelper.ExecuteReader(String.Format("SELECT khid from YX_T_kcdjb where id={0}", id));
            if (dr.Read())
                khid = dr.GetInt32(0);
            else
            {
                res.Add("status", "200");
                res.Add("msg", "找不到出库单。");
                return JsonConvert.SerializeObject(res);
            }

            dr = dbhelper.ExecuteReader(String.Format("select khmc from yx_T_khb where khid={0}", khid));
            if (dr.Read())
                res.Add("cname", dr.GetString(0));

            dr = dbhelper.ExecuteReader(String.Format("select mdd,ckdz,lxdh,shr from yx_T_khb_hyxx where khid={0}", khid));
            if (dr.Read()) 
            {
                res.Add("addr", dr.GetString(0));
                res.Add("AddrDetail", dr.GetString(1));
                res.Add("phone", dr.GetString(2));
                res.Add("contact", dr.GetString(3));
            }
            return JsonConvert.SerializeObject(res);
        }
    }
}
