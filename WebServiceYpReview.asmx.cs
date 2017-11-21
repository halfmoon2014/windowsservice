using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Xml.Serialization;
using LiLanzModel;
using System.IO;
using System.Xml;
using System.Text;
using nrWebClass;
using System.Data;

namespace LLWebService
{
    /// <summary>
    /// WebServiceYpReview 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class WebServiceYpReview : System.Web.Services.WebService
    {
        LiLanzDAL dal = new LiLanzDAL();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string ReviewList(string code) 
        {
            List<YpReview> list = new List<YpReview>();
            //WHERE kfbh = '20153'
            if (code.Length == 5 || code.Length == 4)
                code = cache.SysConfig.GetValue("CheckViewPrefix") + code;
            string sql = string.Format(@"select TOP 10 id,sphh,yphh,lsdj,mlcf,ypmc,ysbs psbs,ysr psr,ysrq psrq,ztbs,ppbs from yx_T_ypdmb
            WHERE kfbh LIKE '{1}' and yphh like '%{0}%' OR bq='{0}'", code, cache.SysConfig.GetValue("CheckViewYearSeason"));
            using (IDataReader dr = dal.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    YpReview  yy = new YpReview();
                    yy.Id = int.Parse(dr[0].ToString());
                    yy.Sphh = dr[1].ToString();
                    yy.Yphh = dr[2].ToString();
                    yy.Lsdj = decimal.Parse(dr[3].ToString());
                    yy.Mlcf = dr[4].ToString();
                    yy.Ypmc = dr[5].ToString();
                    yy.Psbs = dr[6].ToString();
                    yy.Psr = dr[7].ToString();
                    yy.Psrq = dr[8].ToString();
                    yy.Iszt = dr[9].ToString();
                    yy.Ispp = dr[10].ToString();
                    yy.IsEnable = true;
                    list.Add(yy);
                }

            }
            return ToString<List<YpReview>>(list);
        }
        [WebMethod]
        public string ReviewListByName(string cname)
        {
            List<YpReview> list = new List<YpReview>();
            //WHERE kfbh = '20153'
            string sql = string.Format(@"select t1.id,sphh,yphh,lsdj,mlcf,ypmc,ysbs psbs,ysr psr,ysrq psrq,ztbs,ppbs,t2.dm+'.'+t2.mc lbmc from yx_t_ypdmb t1
inner join YX_T_Splb t2 on t1.splbid=t2.id
            WHERE kfbh LIKE '{1}' and ysr='{0}' and ysbs=1 ORDER BY psrq DESC", cname, cache.SysConfig.GetValue("CheckViewYearSeason"));
            using (IDataReader dr = dal.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    YpReview yy = new YpReview();
                    //yy.Id = int.Parse(dr[0].ToString());
                    //yy.Sphh = dr[1].ToString();
                    yy.Yphh = dr[2].ToString();
                    yy.Lsdj = decimal.Parse(dr[3].ToString());
                    //yy.Mlcf = dr[4].ToString();
                    yy.Ypmc = dr[5].ToString();
                    //yy.Psr = dr[7].ToString();
                    //yy.Psrq = dr[8].ToString();
                    yy.Iszt = dr[9].ToString();
                    yy.Ispp = dr[10].ToString();
                    yy.Splb = dr[11].ToString();
                    //yy.IsEnable = true;
                    list.Add(yy);
                }

            }
            return ToString<List<YpReview>>(list);
        }
        [WebMethod]
        public string ProductList(string code)
        {
            List<YpReview> list = new List<YpReview>();

            string sql = string.Format(@"select TOP 10 id,sphh,yphh,lsdj,mlcf,ypmc,lcpsbs psbs,lcpsr psr,lcpsrq psrq,ztbs,ppbs from yx_T_ypdmb
            WHERE kfbh LIKE '{1}' and (yphh like '%{0}%' OR bq='{0}')", code, cache.SysConfig.GetValue("CheckViewYearSeason"));
            using (IDataReader dr = dal.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    YpReview yy = new YpReview();
                    yy.Id = int.Parse(dr[0].ToString());
                    yy.Sphh = dr[1].ToString();
                    yy.Yphh = dr[2].ToString();
                    yy.Lsdj = decimal.Parse(dr[3].ToString());
                    yy.Mlcf = dr[4].ToString();
                    yy.Ypmc = dr[5].ToString();
                    yy.Psbs = dr[6].ToString();
                    yy.Psr = dr[7].ToString();
                    yy.Psrq = dr[8].ToString();
                    yy.Iszt = dr[9].ToString();
                    yy.Ispp = dr[10].ToString();
                    yy.IsEnable = true;
                    list.Add(yy);
                }

            }
            return ToString<List<YpReview>>(list);
        }
        [WebMethod]
        public string ProductListAll(string code)
        {
            List<YpReview> list = new List<YpReview>();

            string sql = string.Format(@"select sphh,yphh,lsdj,mlcf,ypmc,lcpsbs psbs,lcpsr psr,lcpsrq psrq from yx_T_ypdmb
            WHERE kfbh LIKE '{1}' and (yphh like '%{0}%' OR bq='{0}')", code, cache.SysConfig.GetValue("CheckViewYearSeason"));
            using (IDataReader dr = dal.ExecuteReader(sql))
            {
                while (dr.Read())
                {
                    YpReview yy = new YpReview();
                    yy.Id = int.Parse(dr[0].ToString());
                    yy.Sphh = dr[1].ToString();
                    yy.Yphh = dr[2].ToString();
                    yy.Lsdj = decimal.Parse(dr[3].ToString());
                    yy.Mlcf = dr[4].ToString();
                    yy.Ypmc = dr[5].ToString();
                    yy.Psbs = dr[6].ToString();
                    yy.Psr = dr[7].ToString();
                    yy.Psrq = dr[8].ToString();
                    yy.IsEnable = true;
                    list.Add(yy);
                }

            }
            return ToString<List<YpReview>>(list);
        }
        /// <summary>
        /// 预评审更新
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="psr">评审人</param>
        /// <param name="iszt">主推标识</param>
        /// <param name="ispp">拍片标识</param>
        /// <param name="kxsp">是否轻商务</param>
        /// <returns></returns>
        [WebMethod]
        public int review(string ids, string psr = "", string iszt = "0", string ispp = "0", int kxsp = 0)
        {
            string sql = string.Format(@"update yx_T_ypdmb 
set ysbs=1, ysrq=getdate(),ysr='{1}',ztbs={2},ppbs={3},kxsp={4}
where id IN ({0})", ids, psr, iszt, ispp, kxsp);
            WriteLog(sql);
            return dal.ExecuteNonQuery(sql);
        }
        [WebMethod]
        public int ProductPass(string ids, string psr)
        {
            string sql = string.Format("update yx_T_ypdmb set lcpsbs=1, lcpsrq=getdate(),lcpsr='{1}' where id IN ({0}) and lcpsbs<>1", ids, psr);
            WriteLog(sql);
            return dal.ExecuteNonQuery(sql);
        }
        /// <summary>
        /// 量产评审
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="psr"></param>
        /// <param name="iszt">主推</param>
        /// <param name="ispp">拍片</param>
        /// <param name="kxsp">是否轻商务</param>
        /// <returns></returns>
        [WebMethod]
        public int ProductPass2(string ids, string psr,string iszt="0", string ispp="0", int kxsp = 0)
        {
            string sql = string.Format(@"update yx_T_ypdmb set lcpsbs=1, lcpsrq=getdate(),lcpsr='{1}',ztbs={2},ppbs={3}, kxsp = {4}
where id IN ({0})", ids, psr, iszt, ispp, kxsp);
            WriteLog(sql);
            return dal.ExecuteNonQuery(sql);
        }
        protected string ToString<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.Indent = true;
            xws.OmitXmlDeclaration = true;
            XmlWriter textWriter = XmlWriter.Create(ms, xws);
            XmlSerializerNamespaces _namespaces = new XmlSerializerNamespaces(
                        new XmlQualifiedName[] {
                        new XmlQualifiedName(null, null)  
                 });
            serializer.Serialize(textWriter, obj);
            return Encoding.UTF8.GetString(ms.ToArray());
        }
        static public void WriteLog(string strMemo)
        {
            LogHelper.Info(strMemo);
            //string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            //string filename = path + @"/logs/log.txt";
            //if (!Directory.Exists(path + @"/logs/"))
            //    Directory.CreateDirectory(path + @"/logs/");
            //StreamWriter sr = null;
            //try
            //{
            //    if (!File.Exists(filename))
            //    {
            //        sr = File.CreateText(filename);
            //    }
            //    else
            //    {
            //        sr = File.AppendText(filename);
            //    }
            //    sr.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH-mm-ss] "));
            //    sr.WriteLine(strMemo);
            //}
            //catch
            //{
            //}
            //finally
            //{
            //    if (sr != null)
            //        sr.Close();
            //}

        }
        [WebMethod]
        public string LoginIn(string name, string pwd)
        {
            StringBuilder sb = new StringBuilder();
            LoginBLL login = new LoginBLL();
            LiLanzModel.User u = login.LoginCheck(name, pwd);
            if (u.Userid > 0)
            {
                return u.Cname;
            }
            else
                return "";

        }
    }
}
