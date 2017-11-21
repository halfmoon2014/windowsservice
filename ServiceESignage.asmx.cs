using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using nrWebClass;
using System.Data;

namespace LLWebService
{
    /// <summary>
    /// ServiceESignage 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class ServiceESignage : System.Web.Services.WebService
    {
        LiLanzDAL dal = new LiLanzDAL();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string CutInfo()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            string sql = @"SELECT  a.id,a.rq,a.djh,a.sphh,a.chdm,b.pc AS ch ,b.mksl ,b.mkid ,b.cs AS lbcs ,b.jhyl, b.bfsq, b.sl , b.ccbfsl sjsl,d.mc bmmc,e.dw,f.mc CatName,e.chmc,b.bfsqrq,b.ddzt,b.ccblr,b.cjllr,a.zdr
FROM    sc_T_sccjb a
        INNER JOIN sc_T_sccjmx b ON b.id = a.id
		INNER JOIN sc_T_scbmb d on a.cjid=d.id 
		inner join cl_t_chdmb e on a.chdm=e.chdm 
		INNER JOIN dbo.CL_T_Chlb f ON e.chlbid=f.id
WHERE   a.tzid = 11360
        AND b.bfsqrq >= '{0:yyyy-MM-dd}'
        AND b.bfsq >= 1 and b.bfsq<>3";//3领料确认 =4通知领料
            sql = string.Format(sql, DateTime.Now);
            using (IDataReader dr = dal.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    Dictionary<string, object> info = new Dictionary<string, object>();
                    info.Add("sphh", dr["sphh"]);
                    info.Add("ch", dr["ch"]);
                    info.Add("sl", dr["sl"]);
                    info.Add("sjsl", dr["sjsl"]);
                    info.Add("bmmc", dr["bmmc"]);
                    info.Add("dw", dr["dw"]);
                    info.Add("CatName", dr["CatName"]);
                    info.Add("chmc", dr["chmc"]);
                    info.Add("chdm", dr["chdm"]);
                    info.Add("bfsqrq", dr["bfsqrq"]);
                    info.Add("ddzt", dr["bfsq"]);
                    info.Add("zdr", dr["zdr"]);
                    list.Add(info);
                }
            }
            return JsonConvert.SerializeObject(list);
        }
    }
}
