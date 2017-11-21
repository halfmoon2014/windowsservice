using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using LiLanzModel;
using nrWebClass;
using System.Data;
using System.IO;

namespace LLWebService
{
    /// <summary>
    /// PLMCoreDraw 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class PLMCoreDraw : System.Web.Services.WebService
    {
        nrWebClass.LiLanzDAL dal = new nrWebClass.LiLanzDAL();
        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        /// <summary>
        /// 查找指令单
        /// </summary>
        /// <param name="zlbh"></param>
        /// <returns></returns>
        [WebMethod]
        public string InstuctList(string zlbh, string itemCode)
        {
            List<PLMInstruct> list = new List<PLMInstruct>();
            string sql = @" SELECT top (100) zlmxid,ypbh,ypzlbh,case when a.shbs=0 then '未审' when a.shbs=1 then '已审' else '审核中' end as shzt,g.dm+'.'+g.mc  from yf_t_cpkfzlb a  
 inner join YF_T_Cpkfjh jh on a.id=jh.id
 inner join yx_v_splb g on jh.splbid=g.id 
where ypzlbh like '{0}%' ";
            sql = string.Format(sql, zlbh);

            if (itemCode.Trim().Length > 0)
                sql += string.Format(" AND ypbh like '{0}%'", itemCode);

            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    PLMInstruct pstruct = new PLMInstruct();
                    pstruct.Zlmxid = reader.GetInt32(0);
                    pstruct.Yphh = reader.GetString(1);
                    pstruct.Zlbh = reader.GetString(2);
                    pstruct.CheckStatus = reader.GetString(3); 
                    pstruct.Cname = reader.GetString(4); 
                    list.Add(pstruct);
                }
            }
            return xmlHelper.ToString<List<PLMInstruct>>(list); 
        }
        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
         [WebMethod]
        public string ComponentType()
        {
            List<PLMComponentCat> list = new List<PLMComponentCat>();
             String sql = @"SELECT DISTINCT lbjcdm,lbjcmc from yx_t_splb where jb=3 and ccid like '-61-1166-%'
and lbjcdm <> ''";
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read()) 
                {
                    PLMComponentCat cat = new PLMComponentCat();
                    //cat.Id = reader.GetInt32(2);
                    cat.Code = reader.GetString(0);
                    cat.Cname = reader.GetString(1);

                    list.Add(cat);
                }
            }
            return xmlHelper.ToString<List<PLMComponentCat>>(list); 
        }
        /// <summary>
        /// 获取组件列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
         public string ComponentList(int seasonID, int ComponentType, string ccid, string code, string name)
        {
            try
            {
                string _ccid = "-1-3-";
                if (ComponentType == 1)
                    _ccid = "-1-2-";
                List<ComponentInfo> list = new List<ComponentInfo>();
                string sql = String.Format(@"SELECT t1.comcode,t1.ComName,t1.ComWorkHour,t1.ComWorkPrice,t1.Remarks,t1.Creater,t1.ClickCounts,t1.id from 
yf_t_ComponentBasics t1 
inner join yf_t_ComponentClass t2 ON t1.ComType = t2.id
where t1.tzid=1 
and t1.IsActive = 1
AND t2.ClassCCID LIKE '{0}%'", _ccid);
                if (seasonID > 0)
                    sql += String.Format(" AND comseason={0} ", seasonID);

                if (ccid != "")
                    sql += String.Format(" AND t2.ClassCCID + '-' LIKE '{0}-%'", ccid);

                if(name.Trim() != "")
                    sql += String.Format(" AND t1.ComName LIKE '%{0}%' ", name.Trim());

                if (code.Trim() != "")
                    sql += String.Format(" AND t1.comcode LIKE '%{0}%' ", code.Trim());

                using (IDataReader reader = dal.ExecuteReader(sql))
                {
                    while (reader.Read())
                    {
                        ComponentInfo info = new ComponentInfo();
                        info.ComCode = reader.GetString(0);
                        info.ComName = reader.GetString(1);
                        info.ComWorkHour = reader.GetDecimal(2);
                        info.ComWorkPrice = reader.GetDecimal(3);
                        info.Remarks = reader.GetString(4);
                        info.Creater = reader.GetString(5);
                        info.Clicks = reader.GetInt32(6);
                        info.ThumbPath = "/MyUpload/nofind.jpg";
                        info.Id = reader.GetInt32(7);
                        list.Add(info);
                    }
                }
                /*
               651 --缩略图
               652 --实物图
               653 --CDR设计文件  groupid=651 AND
               */
                foreach (ComponentInfo info in list)
                {
                    sql = String.Format(@"select URLAddress,groupid from t_uploadfile where TableID={0}", info.Id);
                    using (IDataReader reader = dal.ExecuteReader(sql))
                    {
                        while (reader.Read()) 
                        { 
                            switch (reader.GetInt32(1))
                            {
                                case 651:
                                    info.ThumbPath = reader.GetString(0);
                                    if (!File.Exists(Server.MapPath(info.ThumbPath)))
                                        info.ThumbPath = "/MyUpload/nofind.jpg";
                                    break;
                                case 653:
                                    info.CDRPath = reader.GetString(0);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                return xmlHelper.ToString<List<ComponentInfo>>(list); 
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            
        }
        /// <summary>
        /// 季节列表
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public string SeasonList()
        {
            List<PLMseason> list = new List<PLMseason>();
            string sql = @"select top(15) dm,mc from yf_t_kfbh order by id desc";
            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    PLMseason season = new PLMseason();
                    season.Id =  int.Parse(reader.GetString(0));
                    season.Code = reader.GetString(0);
                    season.Name = reader.GetString(1);

                    list.Add(season);
                }
            }
            return xmlHelper.ToString<List<PLMseason>>(list); 
        }
        /// <summary>
        /// 部件分类
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [WebMethod]
        public string ComponentCat(string ccid)
        {
            int id = 0;
            string sql = String.Format(@"SELECT id from yf_t_ComponentClass where ClassCCID = '{0}' and isactive = 1", ccid);
            Object rel = dal.ExecuteScalar(sql);
            if (rel != null)
                int.TryParse(rel.ToString(), out id);

            List<PLMComponentCat> list = new List<PLMComponentCat>();
            sql = String.Format(@"SELECT classCode,className,id,ClassCCID
 from yf_t_ComponentClass where ClassParentID={0} and isactive = 1;", id);

            using (IDataReader reader = dal.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    PLMComponentCat cat = new PLMComponentCat();
                    cat.Id = reader.GetInt32(2);
                    cat.Code = reader.GetString(0);
                    cat.Cname = reader.GetString(1);
                    cat.Ccid = reader.GetString(3);
                    list.Add(cat);
                }
            }
            return xmlHelper.ToString<List<PLMComponentCat>>(list);
        }
        /// <summary>
        /// 指令单最新的cdr文件
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [WebMethod]
        public string InstructCdrFile(int id)
        {
            string url = "";
            string sql = String.Format(@"select isnull(b.urlPath,b.wjm) url FROM yf_t_cpkfsjtg a 
INNER JOIN  yf_t_cpkfsjtg_fj b  ON a.id=b.ssid WHERE a.zlmxid={0} AND a.tplx='sgtp' order by b.id desc", id);
            Object rel = dal.ExecuteScalar(sql);
            if (rel != null)
                url = rel.ToString();
            return url;
        }
    }
}
