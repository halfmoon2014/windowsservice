using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using nrWebClass;
using System.Data;
using LiLanzModel;

namespace LLWebService
{
    /// <summary>
    /// LabelPrint 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class LabelPrint : System.Web.Services.WebService
    {
        //public CustomerSoapHeader header;

        LiLanzDAL dal = new LiLanzDAL();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string list(DateTime datestart, DateTime dateend) 
        {
            /*
             *  khid='16434' 强兴
             *   --and t2.xmzjybs=1 厂家签收标识
             */
            string sql = String.Format(@"select t1.rq, t1.zdr, t1.djh, t1.bz, t1.id,t2.chdm,t2.xmzjybs  from YX_T_dddjb t1
                inner join cl_T_sygzb t2 on t1.id=t2.lymxid
                and gzlx = 1011
                and t2.khid='16434'
                and t1.djlx=905          
                and t1.djzt=1 
                AND t1.rq>='{0:yyyy-MM-dd}'
                and t1.rq<'{1:yyyy-MM-dd}'  ", datestart, dateend.AddDays(1));
            List<OrderList> list = new List<OrderList>();
            using (IDataReader reader = dal.ExecuteReader(sql)) 
            {
                while (reader.Read())
                {
                    OrderList order = new OrderList();

                    order.Id = reader["id"].ToString();
                    order.Ordersn = reader["djh"].ToString();
                    order.Odate = reader["rq"].ToString();
                    order.Note = reader["bz"].ToString();
                    order.Chdm = reader["chdm"].ToString();
                    order.Isqs = reader["xmzjybs"].ToString();
                    order.Creater = reader["zdr"].ToString();

                    list.Add(order);
                }
            }
            return xmlHelper.ToString<List<OrderList>>(list);
        }
        /// <summary>
        /// 打印明细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public string orderDetail(int id)
        {
            string sql = String.Format(@"SELECT t4.gg, dbo.f_EBPwd(spid) qrcode,spid,tm,t2.sphh,t2.tm1,t2.tm2,t4.hx,t3.lsdj,dbo.f_36(t1.lsh-100000) sn,t4.hx2,t2.zbyid 
   FROM yx_T_spidb t1
 	    INNER JOIN  (
            select distinct t1.sphh,t2.tm tm1,t3.tm tm2,t2.cmdm,t1.zbyid from yx_v_dddjcmmx t1
	        INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=1) t2 on t1.cmdm=t2.cmdm and t1.sphh=t2.sphh 
	        INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=2) t3 on t1.cmdm=t3.cmdm and t1.sphh=t3.sphh 
	        where id={0} ) t2 ON t1.tm=t2.tm2
   INNER JOIN YX_T_Spdmb t3 ON t2.sphh=t3.sphh AND t3.tzid=1
   left join yx_v_sphxggb t4 on t3.yphh=t4.yphh and t2.cmdm=t4.cmdm
   where lydjid={0} ORDER BY t2.sphh,t2.cmdm,spid", id);
            List<OrderLabelDetail> list = new List<OrderLabelDetail>();
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    OrderLabelDetail detail = new OrderLabelDetail();
                    detail.Spid = reader["spid"].ToString();
                    detail.Qrcode = reader["qrcode"].ToString();
                    detail.Sphh = reader["sphh"].ToString();
                    detail.Tm = reader["tm1"].ToString();

                    if (!reader.IsDBNull(0))
                    {
                        if (reader["hx2"].ToString().Trim() == "")
                            detail.Hx = reader["hx"].ToString();
                        else
                            detail.Hx = String.Format("上衣:{0} 裤子:{1}", reader["hx"].ToString().TrimEnd(), reader["hx2"].ToString().TrimEnd());

                        detail.Gg = reader["gg"].ToString();
                    }
                    
                    detail.Sn = reader["sn"].ToString();
                    detail.Lsdj = reader["lsdj"].ToString();
                    
                    list.Add(detail);
                }
            }
            return xmlHelper.ToString<List<OrderLabelDetail>>(list);
        }
        /// <summary>
        /// 打印明细，按货号生成
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public string orderDetailItem(int id,string sphh)
        {
            string sql = String.Format(@"SELECT t4.gg, dbo.f_EBPwd(spid) qrcode,spid,tm,t2.sphh,t2.tm1,t2.tm2,t4.hx,t3.lsdj,dbo.f_36(t1.lsh-100000) sn,t4.hx2,t2.zbyid,t2.cmdm,t3.tml  
   FROM yx_T_spidb t1
 	    INNER JOIN  (
            select distinct t1.sphh,t2.tm tm1,t3.tm tm2,t2.cmdm,t1.zbyid from yx_v_dddjcmmx t1
	        INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=1) t2 on t1.cmdm=t2.cmdm and t1.sphh=t2.sphh 
	        INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=2) t3 on t1.cmdm=t3.cmdm and t1.sphh=t3.sphh 
	        where id={0} ) t2 ON t1.tm=t2.tm2
   INNER JOIN YX_T_Spdmb t3 ON t2.sphh=t3.sphh
   left join yx_v_sphxggb t4 on t3.yphh=t4.yphh and t2.cmdm=t4.cmdm
   where lydjid={0} AND t3.sphh='{1}' ORDER BY t2.sphh,t2.cmdm,spid", id, sphh);
            List<OrderLabelDetail> list = new List<OrderLabelDetail>();

            Dictionary<string, decimal> filler = ItemFillter(sphh);

            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    OrderLabelDetail detail = new OrderLabelDetail();
                    detail.Spid = reader["spid"].ToString();
                    detail.Qrcode = reader["qrcode"].ToString();
                    detail.Sphh = reader["sphh"].ToString();
                    detail.Tm = reader["tm1"].ToString();
                    detail.Filler = 0;//默认为0嗯

                    if (!reader.IsDBNull(0))
                    {
                        if (reader["hx2"].ToString().Trim() == "")
                            detail.Hx = reader["hx"].ToString();
                        else
                            detail.Hx = String.Format("上衣:{0} 裤子:{1}", reader["hx"].ToString().TrimEnd(), reader["hx2"].ToString().TrimEnd());

                        detail.Gg = reader["gg"].ToString();
                    }
                    string cmdm = reader["cmdm"].ToString().Remove(0, 2);
                  
                    if (filler.ContainsKey(cmdm))
                        detail.Filler = filler[cmdm];
                    detail.Sn = reader["sn"].ToString();
                    //detail.Lsdj = reader["lsdj"].ToString();
                    list.Add(detail);
                }
            }
            return xmlHelper.ToString<List<OrderLabelDetail>>(list);
        }
        /// <summary>
        /// 货号列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public string orderItem(int id)
        {
            List<OrderLabelItem> list = new List<OrderLabelItem>();
            string sql = String.Format(@"SELECT t1.sphh,t1.sl,t2.lsdj, t2.spmc, t1.mxid,t4.xh,t4.title FROM YX_T_dddjmx as t1
inner join YX_T_Spdmb as t2 on t1.sphh=t2.sphh 
INNER JOIN yf_T_bjdlb t3 ON t1.lymxid=t3.id
inner join yf_v_rinsingtemplate t4 on t4.id=t3.lydjid
WHERE t1.id={0}", id);
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    OrderLabelItem item = new OrderLabelItem();
                    item.Mxid = reader["mxid"].ToString();
                    item.Sphh = reader["sphh"].ToString();
                    item.Sl = reader["sl"].ToString();
                    item.Spmc = reader["spmc"].ToString();
                    item.Temp = reader["xh"].ToString() + reader["title"].ToString();
                    list.Add(item);
                }
            }
            return xmlHelper.ToString<List<OrderLabelItem>>(list);
        }
        /// <summary>
        /// 吊牌项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public string ItemInfo(int id) 
        {
            try
            {
                List<OrderLabelInfo> list = new List<OrderLabelInfo>();
                string sql = String.Format(@"select T2.sphh,a.bz as dj,a.id as zbid,a.mc as pm,b.mc as zxbz ,T3.mc,t2.lymxid,t4.lsdj,t5.xh,t4.spmc
from Yf_T_bjdbjzb a
INNER JOIN Yf_T_bjdbjzb b ON  a.ssid=b.id 
inner join yf_T_bjdlb T ON a.id=T.tplx
INNER JOIN YX_T_dddjmx T2 ON T.id=t2.lymxid
INNER JOIN Yf_T_bjdbjzb T3 on T3.lx=905 and T.sylx=T3.id 
INNER JOIN YX_T_Spdmb t4 on t2.sphh=t4.sphh
inner join yf_v_rinsingtemplate t5 on t5.id=T.lydjid
where a.lx=903 and T2.id={0}", id);
                using (IDataReader reader = dal.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        OrderLabelInfo item = new OrderLabelInfo();
                        item.Sphh = reader["sphh"].ToString();
                        item.Dj = reader["dj"].ToString(); //等级
                        item.Pm = reader["pm"].ToString();
                        item.Zxbz = reader["zxbz"].ToString();
                        item.Aqlb = reader["mc"].ToString();
                        item.Mxid = reader["lymxid"].ToString();
                        item.Lsdj = decimal.Parse(reader["lsdj"].ToString());//零售单价
                        item.Template = reader["xh"].ToString();
                        item.Iname = reader["spmc"].ToString();
                        item.ProDate = GetProductionDate(item.Sphh);
                        
                        list.Add(item);
                    }
                }
                foreach (OrderLabelInfo info in list)
                {
                    sql = string.Format("select pdjg,sz from yf_T_bjdmxb where mxid={0} AND bzzid<=3 and bzzid>=0 ", info.Mxid);
                    List<MaterialInfo> mlist = new List<MaterialInfo>();
                    using (IDataReader reader = dal.ExecuteReader(sql))
                    {
                        while (reader.Read())
                        {
                            MaterialInfo m = new MaterialInfo();
                            m.Title = reader["pdjg"].ToString();
                            m.Value = reader["sz"].ToString();
                            mlist.Add(m);
                        }
                    }
                    info.Material = mlist;
                }

                return xmlHelper.ToString<List<OrderLabelInfo>>(list);
            }
            catch (Exception ex) 
            {
                return ex.ToString();
            }
        }
        [WebMethod]
        public string BarCodeOrderId(String barcode)
        {
            String sql = string.Format("SELECT lydjid FROM YX_T_Spidb where spid='{0}'", barcode);
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                if (reader.Read())               
                    return ItemInfo(int.Parse(reader[0].ToString()));                
            }         
            return "";
        }
        /// <summary>
        /// 补单个条码
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        [WebMethod]
        public string BarCodeDetail(String barcode)
        {
            OrderLabelDetail detail = new OrderLabelDetail();
            String sql = String.Format(@"SELECT t1.spid,dbo.f_EBPwd(t1.spid) barcode,t1.tm,t1.lydjid,t2.yid,t3.cmdm,t3.sphh, dbo.f_36(t1.lsh-100000) sn
FROM YX_T_Spidb t1
inner join yx_t_dddjb t2 on t1.lydjid=t2.id 
INNER JOIN yx_t_tmb t3 ON t1.tm=t3.tm 
and t3.tzid=1 
and t3.tmlx=2 
and t1.spid='{0}'",  barcode);
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                if (reader.Read())
                {
                    detail.Spid = reader[0].ToString();
                    detail.Qrcode = reader[1].ToString();
                    detail.Sphh = reader["sphh"].ToString();
                    detail.Cmdm = reader["cmdm"].ToString();
                    detail.HxKind = int.Parse(reader["yid"].ToString());
                    detail.Sn = reader["sn"].ToString();
                }
            }
            sql = String.Format(@"select tm,t2.yphh,t2.lsdj from yx_t_tmb t1
inner join YX_T_Spdmb t2 on t1.sphh=t2.sphh
where t1.tzid=1 and tmlx=1 and cmdm='{0}' and t1.sphh='{1}'", detail.Cmdm, detail.Sphh);
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                if (reader.Read())
                {
                    detail.Tm = reader[0].ToString();
                    detail.Yphh = reader[1].ToString();
                    detail.Lsdj = reader[2].ToString();
                }
            }
            sql = String.Format("SELECT hx,hx2,gg from yx_v_sphxggb where yphh='{0}' and cmdm='{1}'",
                detail.Yphh, detail.Cmdm);
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                if (reader.Read())
                {
                    detail.Hx = reader[0].ToString();
                    if (reader[1].ToString().Trim() != "")
                        detail.Hx = String.Format("上衣:{0} 裤子:{1}", 
                            reader[0].ToString().Trim(), reader[1].ToString().Trim());
                    detail.Gg = reader[2].ToString();
                }
            }
            Dictionary<string, decimal> filler = ItemFillter(detail.Sphh);

            string cmdm = detail.Cmdm.ToString().Remove(0, 2);

            if (filler.ContainsKey(cmdm))
                detail.Filler = filler[cmdm];

            return xmlHelper.ToString<OrderLabelDetail>(detail); 
        }
        /// <summary>
        /// 促销品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public string orderDetail40mm(int id)
        {
            string sql = string.Format(@"select dbo.f_EBPwd(t3.tm) qrcode,t1.sphh,t2.tm tm1,t3.tm tm2,t2.cmdm,t1.zbyid,t1.lymxid,t5.yphh,t1.sl0,t4.hx,t4.gg from yx_v_dddjcmmx t1
	INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=1) t2 on t1.cmdm=t2.cmdm and t1.sphh=t2.sphh 
	INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=2) t3 on t1.cmdm=t3.cmdm and t1.sphh=t3.sphh 
  INNER JOIN YX_T_Spdmb t5 ON t1.sphh=t5.sphh
  LEFT join yx_v_sphxggb t4 on t5.yphh=t4.yphh and t2.cmdm=t4.cmdm
where t1.id={0} ORDER BY t1.sphh,t2.cmdm", id);
            List<OrderLabelDetail> list = new List<OrderLabelDetail>();
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    OrderLabelDetail detail = new OrderLabelDetail();
                    detail.Spid = reader["tm2"].ToString();
                    detail.Qrcode = reader["qrcode"].ToString();
                    detail.Sphh = reader["sphh"].ToString();
                    detail.Tm = reader["tm1"].ToString();
                    detail.Hx = reader["hx"].ToString();
                    detail.Gg = reader["gg"].ToString();
                    detail.Sn = "";
                    //detail.Lsdj = reader["lsdj"].ToString();
                    detail.Num = int.Parse(reader["sl0"].ToString());
                    list.Add(detail);
                }
            }
            return xmlHelper.ToString<List<OrderLabelDetail>>(list);
        }
        /// <summary>
        /// 促销品吊牌补打
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public string orderDetailByBarcod40mm(string barcode)
        {
            string sql = string.Format(@"select dbo.f_EBPwd(t3.tm) qrcode,t1.sphh,t2.tm tm1,t3.tm tm2,t2.cmdm,t1.zbyid,t1.lymxid,t5.yphh,t1.sl0,t4.hx from yx_v_dddjcmmx t1
	INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=1) t2 on t1.cmdm=t2.cmdm and t1.sphh=t2.sphh 
	INNER JOIN (select sphh,cmdm,tm from yx_t_tmb where tzid=1 and tmlx=2) t3 on t1.cmdm=t3.cmdm and t1.sphh=t3.sphh 
  INNER JOIN YX_T_Spdmb t5 ON t1.sphh=t5.sphh
  inner join yx_v_sphxggb t4 on t5.yphh=t4.yphh and t2.cmdm=t4.cmdm
where  t3.tm = '{0}' AND T1.DJLX=905 AND t1.djzt=1 ", barcode);
            List<OrderLabelDetail> list = new List<OrderLabelDetail>();
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    OrderLabelDetail detail = new OrderLabelDetail();
                    detail.Spid = reader["tm2"].ToString();
                    detail.Qrcode = reader["qrcode"].ToString();
                    detail.Sphh = reader["sphh"].ToString();
                    detail.Tm = reader["tm1"].ToString();
                    detail.Hx = reader["hx"].ToString();
                    detail.Sn = "";
                    //detail.Lsdj = reader["lsdj"].ToString();
                    detail.Num = int.Parse(reader["sl0"].ToString());
                    list.Add(detail);
                }
            }
            return xmlHelper.ToString<List<OrderLabelDetail>>(list);
        }
        /// <summary>
        /// 获取生产日期
        /// </summary>
        /// <param name="sphh"></param>
        /// <returns></returns>
        public DateTime GetProductionDate(string sphh)
        {
            DateTime val = DateTime.Parse("1900-01-01");
            string sql = string.Format(@"SELECT cpksrq FROM zw_t_htscddmx where sphh = '{0}' 
union all SELECT jhksrq from zw_t_httpddmx where sphh = '{0}'", sphh);
      
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    if (reader.IsDBNull(0))
                        continue;
                    if (DateTime.Compare(val, reader.GetDateTime(0)) < 0)
                        val = reader.GetDateTime(0);
                }
            }
            return val;
        }
        /// <summary>
        /// 货号尺码充绒量
        /// </summary>
        /// <param name="ItemCode"></param>
        /// <returns></returns>
        public Dictionary<string,decimal> ItemFillter(String ItemCode)
        {
            Dictionary<string, decimal> dict = new Dictionary<string, decimal>();
            string sql = @"SELECT t4.cmdm,(t4.hsz+t4.bzsh) * 1000 val FROM dbo.YF_T_Bom t1
INNER JOIN cl_v_chdmb_all t2 ON t1.chdm=t2.chdm
INNER JOIN yf_T_bjdlb t3 ON t2.bjid=t3.id AND t3.kzx1 = 297
INNER JOIN YF_T_Bomcmmx t4 ON t1.id=t4.id
WHERE t1.sphh='{0}'";
            sql = string.Format(sql, ItemCode);
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                    dict.Add(reader.GetString(0), reader.GetDecimal(1));     
            }
            return dict;
        }
        
    }
}
