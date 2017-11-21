using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Data.SqlClient;
using nrWebClass;
using System.Data;
using System.Xml.Serialization;
using LiLanzModel;
using System.Xml;
using System.IO;
using System.Text;
namespace LLWebService
{
    /// <summary>
    /// WebServiceForYpPicUpload 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class WebServiceForYpPicUpload : System.Web.Services.WebService
    {
        LiLanzDAL dal = new LiLanzDAL();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string YphhList(string kfbh, int splbid)
        {
            List<ListYFYYSP> yyList = new List<ListYFYYSP>();
            using (IDataReader dr = dal.ExecuteReader(String.Format(@"select t2.ypbh,t2.ypzlbh,T4.DM+'.'+t4.mc LB,T1.lsdj1 lsdj,t3.zlmxid,T3.MYPIC from yf_t_cpkfjh as t1
inner join yf_t_cpkfzlb as t2 on t1.id=t2.id 
INNER JOIN yf_t_cpkfsjtg as t3 on t2.zlmxid=t3.zlmxid and t3.tplx='sjtg' 
INNER JOIN YX_T_Splb AS T4 ON T1.SPLBID=T4.ID
where  t1.kfbh='{0}' and t1.splbid={1} ", kfbh, splbid)))
            {
                while (dr.Read())
                {
                    ListYFYYSP yy = new ListYFYYSP();
                    yy.Yphh = dr[0].ToString();
                    yy.Ypzlbh = dr[1].ToString();
                    yy.Splbmc = dr[2].ToString();
                    yy.Lsdj = dr[3].ToString();
                    yy.Zlmxid = int.Parse(dr[4].ToString());
                    yyList.Add(yy);
                }

            }
            return ToString<List<ListYFYYSP>>(yyList);
        }
        [WebMethod]
        public string YphhListV2(string kfbh, int splbid, string bq)
        {
            string sql = String.Format(@"select t2.ypbh,t2.ypzlbh,T4.DM+'.'+t4.mc LB,T1.lsdj1 lsdj,t3.zlmxid,T3.MYPIC,isnull(t5.bq,'') bq,isnull(t6.num,0) photoCount,isnull(t5.psbs,0) psbs
from yf_t_cpkfjh as t1
inner join yf_t_cpkfzlb as t2 on t1.id=t2.id 
INNER JOIN yf_t_cpkfsjtg as t3 on t2.zlmxid=t3.zlmxid and t3.tplx='sjtg' 
INNER JOIN YX_T_Splb AS T4 ON T1.SPLBID=T4.ID
LEFT JOIN yx_t_ypdmb AS t5 ON  t2.ypbh=t5.yphh
LEFT JOIN (SELECT TableID,COUNT(1) num FROM t_uploadfile 
                WHERE groupid='1003' AND TableID <> 0 group by TableID) as t6 ON t2.zlmxid = t6.TableID
where  t1.kfbh='{0}' ", kfbh);

            if (splbid != 0 )
                sql += String.Format("and t1.splbid={0} ", splbid);

            if(bq != "")
                sql += String.Format("AND t5.bq = '{0}' ", bq);

            List<ListYFYYSP> yyList = new List<ListYFYYSP>();
            try
            {
                using (IDataReader dr = dal.ExecuteReader(sql))
                {
                    while (dr.Read())
                    {
                        ListYFYYSP yy = new ListYFYYSP();
                        yy.Yphh = dr[0].ToString();
                        yy.Ypzlbh = dr[1].ToString();
                        yy.Splbmc = dr[2].ToString();
                        yy.Lsdj = dr[3].ToString();
                        yy.Zlmxid = int.Parse(dr[4].ToString());
                        yy.PicCount = int.Parse(dr[7].ToString());
                        yy.Bq = dr[6].ToString();
                        yy.Psbs = dr[7].ToString();
                        yyList.Add(yy);
                    }

                }
                return ToString<List<ListYFYYSP>>(yyList);
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        [WebMethod]
        public string Yplb(string kfbh)
        {
            List<ItemCat> _list = new List<ItemCat>();
            using (IDataReader dr = dal.ExecuteReader(String.Format(@"select DISTINCT T4.id,T4.dm + '.' + t4.mc from yf_t_cpkfjh as t1
INNER JOIN YX_T_Splb  AS T4 ON T1.SPLBID=T4.ID
WHERE kfbh='{0}'", kfbh)))
            {
                while (dr.Read())
                {
                    ItemCat cat = new ItemCat();
                    cat.Id = int.Parse( dr[0].ToString());
                    cat.Cname = dr[1].ToString();
                    _list.Add(cat);
                }

            }
            return ToString<List<ItemCat>>(_list);
        }
        [WebMethod]
        public string KFBH()
        {
            List<ItemKFBH> KfList = new List<ItemKFBH>();
            using (IDataReader dr = dal.ExecuteReader(@"select top 10 dm,mc from YF_T_Kfbh order by id desc"))
            {
                while (dr.Read())
                {
                    ItemKFBH item = new ItemKFBH();
                    item.Dm = dr[0].ToString();
                    item.Mc = dr[1].ToString();
                    KfList.Add(item);
                }

            }
            return ToString<List<ItemKFBH>>(KfList);
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
        protected string GetFileName()
        {
            Random rd = new Random();
            StringBuilder serial = new StringBuilder();
            serial.Append(DateTime.Now.ToString("yyyyMMddHHmmssff"));
            serial.Append(rd.Next(100000, 999999).ToString());
            return serial.ToString();
        }
        [WebMethod]
        public string UploadFile(byte[] photo, int zlmxid, int type)
        {
            string fileName = GetFileName() + ".jpg";
            //设置文件保存虚拟路径
            string urlPath = "../MyUpload/" + DateTime.Now.ToString("yyyyMM") + "/";
            //设置文件保存物理路径
            string filePath = Server.MapPath("") + "\\MyUpload\\" + DateTime.Now.ToString("yyyyMM");
            //检查是否有该路径  没有就创建
            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
            writeByteArrayToFile(Path.Combine(filePath, fileName), photo);
            string comm = String.Format(@"INSERT INTO t_uploadfile 
(tableid,groupName,groupid,urladdress,CREATEdate,filename)
select {0},'{2}',1003,'{1}',GETDATE(),yphh from yf_t_cpkfsjtg where tplx='sjtg' and zlmxid={0}",
             zlmxid, urlPath + fileName, ArrayFileType[type]);
            dal.ExecuteNonQuery(comm);
            return "done";
        }
        public void writeByteArrayToFile(string fileName, byte[] buffer)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
            {
                fs.Write(buffer, 0, (int)buffer.Length);
            }
        }
        [WebMethod]
        public void InitUpload(int zlmxid)
        {
            string comm = @"select zlmxid from yf_t_cpkfsjtg where tplx='cyzp'
            and zlmxid={0}";
            comm = String.Format(comm, zlmxid);
            if (dal.ExecuteScalar(comm) == null)
            {
                comm = @"INSERT INTO yf_t_cpkfsjtg(zlmxid, mc, zdr, zdrq, tplx, yphh, mypic) 
            select zlmxid ,mc,'{1}',getdate(),'cyzp',yphh,'已上传' from yf_t_cpkfsjtg a 
            where a.zlmxid='{0}' and a.tplx='sjtg';";
                comm = String.Format(comm, zlmxid, "");
                dal.ExecuteNonQuery(comm);
            }
        }
        [WebMethod]
        public string GetFileList(int zlmxid)
        {
            List<AttachmentList> _lists = new List<AttachmentList>();
            using (IDataReader dr = dal.ExecuteReader(String.Format(@"select id,urladdress,filename from t_uploadfile
 where groupid='1003' and TableID={0}", zlmxid)))
            {
                while (dr.Read())
                {
                    AttachmentList item = new AttachmentList();
                    item.FileName = dr[2].ToString();
                    item.FileID = dr[0].ToString();
                    item.Url = dr[1].ToString();
                    _lists.Add(item);
                }

            }
            return ToString<List<AttachmentList>>(_lists);
        }
        enum FileType
        {
            front = 0, 
            back =1 ,
            inside =2
        }
        String[] ArrayFileType = {"正面",
        "背面",
        "里布"};
        ///
        /// 写日志(用于跟踪)
        ///
        static public void WriteLog(string strMemo)
        {
            string path = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            string filename = path + @"/logs/log.txt";
            if (!Directory.Exists(path + @"/logs/"))
                Directory.CreateDirectory(path + @"/logs/");
            StreamWriter sr = null;
            try
            {
                if (!File.Exists(filename))
                {
                    sr = File.CreateText(filename);
                }
                else
                {
                    sr = File.AppendText(filename);
                }
                sr.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH-mm-ss] "));
                sr.WriteLine(strMemo);
            }
            catch
            {
            }
            finally
            {
                if (sr != null)
                    sr.Close();
            }

        }
        [WebMethod]
        public bool FileDel(int id)
        {
            List<AttachmentList> _lists = new List<AttachmentList>();
            using (IDataReader dr = 
                dal.ExecuteReader(String.Format(@"select urladdress from t_uploadfile where id={0}", id)))
            {
                if (dr.Read())
                {
                    string FilePath = Server.MapPath("./") + dr[0].ToString().Remove(0,3);
                    if (File.Exists(FilePath))
                        File.Delete(FilePath);
                }
            }
            dal.ExecuteNonQuery(String.Format(@"DELETE from t_uploadfile where id={0}", id));
            return true;
        }
    }
}
