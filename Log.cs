using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
namespace LLWebService
{
    public class Log
    {
        static public void Info(string strMemo)
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
    }
}