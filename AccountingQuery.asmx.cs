using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using System.Data;
using System.IO;
using LiLanzModel;

namespace LLWebService
{
    /// <summary>
    /// AccountingQuery 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class AccountingQuery : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string AccountingForQuery(String StartFph, int Number,string filter)
        {
            List<AccountingInfo> accList = new List<AccountingInfo>();
            string comm = @"select top {0} a.kprq,a.pjh,a.hsje,a.sje,a.hsje-a.sje as bhsje,a.khmc,
            case when a.djbs=1 and djzt=0 then '未开票' when a.djbs=1 and djzt=1 then '正常填开' when a.djbs=0 then '作废' end as fpzt,
            case a.pjlx when 11 then '普通发票' when 12 then  '增值税专用发票' end as pjlx,djbs,djzt
            from zw_t_pjxxb a 
            inner join zw_t_pjkcd b 
            on a.rkid=b.id 
            {1}
            where b.tzid=1 
            order by pjh";
            string SqlFilter = "";
            if (StartFph != "")
                SqlFilter = String.Format(@" and a.pjh>={0} ", StartFph);
            if (filter != "")
                SqlFilter += filter;
            comm = String.Format(comm, Number, SqlFilter);
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            using (IDataReader reader = dal.ExecuteReader(comm))
            {
                while (reader.Read())
                {

                    AccountingInfo acc = new AccountingInfo();
                    acc.Rq = DateTime.Parse(reader[0].ToString());
                    acc.Fph = reader[1].ToString();
                    acc.Je = Decimal.Parse(reader[2].ToString());
                    acc.Amount = Decimal.Parse(reader[4].ToString());
                    acc.DutyFee = Decimal.Parse(reader[3].ToString());
                    if (reader["djbs"].ToString() == "1")
                    {
                        if (reader["djzt"].ToString() == "1")
                            acc.Status = 1;
                        else
                            acc.Status = 0;
                    }
                    else
                        acc.Status = 2;
                    accList.Add(acc);
                }
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<AccountingInfo>));
            StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, accList);
            return textWriter.ToString();
        }
        [WebMethod]
        public string AccountingForQueryGYD(String StartFph, int Number)
        {
            List<AccountingInfo> accList = new List<AccountingInfo>();
            string comm = @"SELECT top {1} kprq,fph,( SELECT    SUM(kpje)
                                    FROM      yx_t_ddfpgxb
                                    WHERE     fpid = a.id
                                ) AS je  FROM yx_t_fpxxb AS a 
                            WHERE FPH>='{0}'
                            ORDER BY fph";
            comm = String.Format(comm, StartFph, Number);
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL("14204");
            using (IDataReader reader = dal.ExecuteReader(comm))
            {
                while (reader.Read())
                {
                    AccountingInfo acc = new AccountingInfo();
                    acc.Rq = DateTime.Parse(reader[0].ToString());
                    acc.Fph = reader[1].ToString();
                    acc.Je = Decimal.Parse(reader[2].ToString());
                    accList.Add(acc);
                }
            }
            XmlSerializer serializer = new XmlSerializer(typeof(List<AccountingInfo>));
            StringWriter textWriter = new StringWriter();
            serializer.Serialize(textWriter, accList);
            return textWriter.ToString();
        }
        [WebMethod]
        public string InvioceCompanyList()
        {
            List<InvioceCompany> list = new List<InvioceCompany>();
            string comm = @"select id,dm,mc,txdz,lxdh,qydd,qymc from yx_t_khmx where khid=1";
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            using (IDataReader reader = dal.ExecuteReader(comm))
            {
                while (reader.Read())
                {
                    InvioceCompany comp = new InvioceCompany();
                    comp.Id = int.Parse(reader[0].ToString());
                    comp.Dm = reader[1].ToString();
                    comp.Mc = reader[2].ToString();
                    list.Add(comp);
                }
            }
            return nrWebClass.xmlHelper.ToString<List<InvioceCompany>>(list);
        }
        [WebMethod]
        public string InvioceList(DateTime dayStart, DateTime dayEnd, int companyId, string cat)
        {
            List<Invioce> list = new List<Invioce>();
            string comm = String.Format(@"select a.id,shbs=case when a.shbs=1 then '已审' else '未审' end
,a.id,a.rq,a.djh,mx.je as je,isnull(a.shbs,0) as shbs,
 a.zdr,a.kprq,a.fph,a.fpzdr,d.mc as sskh,f.zhmc as khmc,
h.dm+'.'+h.mc as fplx,ly.rq as lyrq,ly.djh as lydjh,a.jsrq,a.qsrq ,a.fpdm,k3.khdm+'.'+k3.khmc k3khmc ,
xt.mc as khfl,f.swdjh,f.yhzh,lxdz + ' ' + f.lxdh addr,d.Invoicer,a.fph, a.fpdm, a.kprq, f.khyh,mx.se as se,mx.bhsje,mx.sl as sl
,CASE WHEN mx.ls>8 THEN '是' ELSE '否' END AS  isqd,isnull(a.fhr,'') as fhr,isnull(a.skr,'') as skr 
 from yx_t_fpxxb a inner join yx_t_khmx d on a.sskh=d.id 
  inner join (select id,sum(round(je,2)-ROUND(je/1.17,2)) as se,sum(je) as je,sum(ROUND(je/1.17,2)) as bhsje,sum(sl) as sl ,COUNT(*) AS ls
             from yx_T_fpxxmxb group by id) as mx on a.id=mx.id
 inner join zw_t_yhzlb  f on a.khid=f.id 
 left outer join  zw_t_k3khdy k3 on a.k3khid=k3.id 
 left outer join t_xtdm h on a.fplx=h.dm and h.ssid=7735 
 left join t_xtdm xt on k3.khfl=xt.dm and xt.ssid=8813
 left outer join yx_t_hxdjb ly on a.lydjid=ly.id and a.lydjlx=ly.djlx and ly.tzid=1
 where a.tzid=1 
 and a.djbs=1
 and a.djlx=137  
 and a.rq>='{0:yyyy-MM-dd}' 
 and a.rq<'{1:yyyy-MM-dd}'
and a.sskh = {2}
and  a.fplx = '{3}'", dayStart, dayEnd.AddDays(1), companyId, cat);
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            using (IDataReader reader = dal.ExecuteReader(comm))
            {
                while (reader.Read())
                {
                    Invioce invoice = new Invioce();
                    invoice.Id = int.Parse(reader[0].ToString());
                    invoice.CName = reader["khmc"].ToString();
                    invoice.InvoiceDate = DateTime.Parse(reader["kprq"].ToString());
                    invoice.Cdate = DateTime.Parse(reader["rq"].ToString());
                    invoice.Sn = reader["djh"].ToString() + "|" + reader["isqd"].ToString();
                    invoice.AmountTotal = Decimal.Parse(reader["je"].ToString());
                    invoice.CtaxCode = reader["swdjh"].ToString();
                    invoice.Type = reader["fplx"].ToString();
                    invoice.CAddrPhone = reader["addr"].ToString();
                    invoice.CbankAccount =reader["khyh"].ToString() + " " + reader["yhzh"].ToString();
                    invoice.Invoicer = reader["Invoicer"].ToString();
                    invoice.TaxCode = reader["fpdm"].ToString();
                    invoice.TaxDate = DateTime.Parse(reader["kprq"].ToString());
                    invoice.TaxNumber = reader["fph"].ToString();
                    invoice.NumberTotal = Decimal.Parse(reader["sl"].ToString());
                    invoice.AmountAddTax = Decimal.Parse(reader["bhsje"].ToString());//不含税金额
                    invoice.Checker = reader["fhr"].ToString();//复核人
                    invoice.Cashier = reader["skr"].ToString();//收款人
                    
                    list.Add(invoice);
                }
            }
            return nrWebClass.xmlHelper.ToString<List<Invioce>>(list);
        }
        [WebMethod]
        public string InvioceDetail(int id)
        {          
            try
            {
                List<InvioceDetail> list = new List<InvioceDetail>();
                string comm = String.Format(@"select a.id,b.mxid,b.sphh as cwhh,c.xz,c.splbdm,c.cwspmc as spmc,b.sl,b.dj,b.je,
b.je-cast(b.je/1.17 as numeric(12,2)) se,cast(b.je/1.17 as numeric(12,2)) bhsje,
b.bm,b.cmmc,C.dw
from yx_t_fpxxb a 
inner join yx_t_fpxxmxb b on a.id=b.id 
inner join yx_v_cwspdmb c on b.sphh=c.cwhh where a.id = {0}", id);
                nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
                using (IDataReader reader = dal.ExecuteReader(comm))
                {
                    while (reader.Read())
                    {
                        InvioceDetail detail = new InvioceDetail();
                        if (reader["bm"].ToString() != "")
                        {
                            detail.GoodsName = reader["bm"].ToString();
                        }
                        else {
                            detail.GoodsName = reader["spmc"].ToString();
                        }

                        detail.Standard = reader["cmmc"].ToString();
                        detail.Number = Decimal.Parse(reader["sl"].ToString());
                        detail.Amount = Decimal.Parse(reader["je"].ToString());
                        detail.Unit = reader["dw"].ToString();
                        detail.PriceKind = 1;

                        list.Add(detail);
                    }
                }
                return nrWebClass.xmlHelper.ToString<List<InvioceDetail>>(list);
            }
            catch (Exception ex)
            {
                return ex.ToString();
                
            }        
        }
        //更新
        [WebMethod]
        public int InvioceUpdate(int id, DateTime invioceDate, String sn, String invoiceCode, string printer,double Amount,double TaxAmount)
        {
            String sql = String.Format(@"declare @hsje decimal(10,2),@sje decimal(10,2),@khmc varchar(100);
                                         select @hsje=sum(b.je),@sje=sum(round(b.je,2)-ROUND(b.je/1.17,2)),@khmc=max(f.zhmc) 
                                        FROM dbo.yx_t_fpxxb a INNER JOIN dbo.yx_t_fpxxmxb b ON a.id=b.id
                                        inner join zw_t_yhzlb f on a.khid=f.id   WHERE a.id='{4}';
                                        UPDATE yx_t_fpxxb SET kprq='{0}', fph='{1}', fpdm='{2}',fpzdr='{3}' where id={4};
                                         update a set djbs=1,djzt=1,hsje={5},sbl=0.17,sje={6},khmc=@khmc,kprq='{0}'  
                                         from zw_t_pjxxb a inner join zw_t_pjkcd b on a.rkid=b.id and b.shbs=1 where a.pjh='{1}' and a.fpdm='{2}' ",
                invioceDate, sn, invoiceCode, printer, id,Amount,TaxAmount);
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            int i = dal.ExecuteNonQuery(sql);
            return i;
        }
        //查询拓力的发票库存
        [WebMethod]
        public int InvioceSelect(String invoiceNumber, String invoiceCode)
        {
            String sql = String.Format(@" select a.id from dbo.zw_t_pjkcd a
                                        INNER JOIN dbo.zw_t_pjxxb b ON a.id=b.rkid
                                        WHERE b.fpdm='{0}' AND b.pjh='{1}'",
                invoiceNumber, invoiceCode);
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            int i = dal.ExecuteNonQuery(sql);
            return i;
        }
        [WebMethod]
        public int InvioceVoid(int id, String invoiceNumber, String invoiceCode)
        {
            String sql = String.Format(@"UPDATE yx_t_fpxxb SET shbs=0,shr='',shrq=getdate(),fph='',fpdm='',fpzdr='' where shbs=1 and  id={2};
                                         update a set djbs=0,djzt=1  
                                         from zw_t_pjxxb a inner join zw_t_pjkcd b on a.rkid=b.id and b.shbs=1 where a.pjh='{0}' and a.fpdm='{1}' ",
                invoiceNumber, invoiceCode, id);
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            int i = dal.ExecuteNonQuery(sql);
            return i;
        }
        [WebMethod]
        public string cat()
        {
            List<InvioceCat> list = new List<InvioceCat>();
            string comm = "SELECT dm,mc from t_xtdm where ssid=7735";
            nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
            using (IDataReader reader = dal.ExecuteReader(comm))
            {
                while (reader.Read())
                {
                    InvioceCat icat = new InvioceCat();
                    icat.Cat = reader[0].ToString();
                    icat.Cname = reader[1].ToString();
                    list.Add(icat);
                }
            }      
            return nrWebClass.xmlHelper.ToString<List<InvioceCat>>(list);
        }
    }
}
