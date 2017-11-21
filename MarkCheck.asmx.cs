using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Data;
using LiLanzModel;
namespace LLWebService
{
    /// <summary>
    /// MarkCheck 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class MarkCheck : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            
            return "Hello World";
        }
        [WebMethod]
        public string MarkInfo(string barcode)
        {
            string sql = string.Format("select cmdm,sphh from yx_t_tmb where tzid=1 and tmlx=1 and tm='{0}'", barcode);
            SortedDictionary<string, string> row = get_row(sql);
            MarkInfo m = new MarkInfo();
            m.Sphh = row["sphh"];
            
            string cmdm = row["cmdm"];

            sql = string.Format("select tml,splbid,lsdj from yx_t_spdmb where sphh='{0}'", m.Sphh);
            row = get_row(sql);

            m.Lsdj = row["lsdj"];
            string lbid = row["splbid"];
            string tml = row["tml"];

            sql = string.Format("select top 1 box from yx_v_dddjcmmx where djlx=905 and sphh='{0}'", m.Sphh);
            row = get_row(sql);
            string box = row["box"];

            sql = string.Format("select hx,gg from yx_T_spggb where splbid={0} and lx='{1}' and cmdm='{2}'", lbid, box, cmdm);
            row = get_row(sql);
            m.Hx = row["hx"];
            m.Gg = row["gg"];

            return nrWebClass.xmlHelper.ToString<MarkInfo>(m);
        }
        private SortedDictionary<string, string> get_row(string sql) 
        {
            SortedDictionary<string, string> row = new SortedDictionary<string,string>();
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();

            IDataReader reader = dal.ExecuteReader(sql);
            if (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                    row.Add(reader.GetName(i), reader[i].ToString());
            }
            else 
            {
                return null;
            }
            reader.Dispose();
            return row;
                        
        }
    }
}
