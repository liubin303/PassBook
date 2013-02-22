using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace passbook
{
    public class WriteLog
    {
        private static string _filepath = AppDomain.CurrentDomain.BaseDirectory + "//" + System.Configuration.ConfigurationManager.AppSettings["WrongLogPath"];
        public WriteLog()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 日志文件存储路径
        /// </summary>
        public static string filepath
        {
            set { _filepath = value; }
            get { return _filepath; }
        }

        public static void tradeLog(string FilePath, string Message)
        {
            //Page page = (Page)HttpContext.Current.CurrentHandler;
            //FilePath = page.Server.MapPath(FilePath);


            if (!System.IO.File.Exists(FilePath))
            {
                System.IO.FileStream f = System.IO.File.Create(FilePath);
                f.Close();

            }
            StreamWriter fs = new StreamWriter(FilePath, true);
            fs.WriteLine(DateTime.Now.ToString() + ": " + Message);
            fs.Close();

        }

        public static void tradeCurLog(string FilePath, string Message)
        {
            StreamWriter fs = new StreamWriter(FilePath, true);
            fs.WriteLine(DateTime.Now.ToString() + ": " + Message);
            fs.Close();
        }

        /// <summary>
        /// 添加日记
        /// </summary>
        /// <param name="Message"></param>
        public static void tradeLog(string Message)
        {
            StreamWriter fs = new StreamWriter(_filepath, true);
            fs.WriteLine(DateTime.Now.ToString() + ": " + Message);
            fs.Close();
        }

        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="path"></param>
        public static void SetTradeLog(string Message)
        {

            string FilePath = _filepath;

            FilePath += DateTime.Now.Year;


            // +"\\" + DateTime.Now.Year;
            if (!Directory.Exists(FilePath))//若文件夹不存在则新建文件夹   
            {
                Directory.CreateDirectory(FilePath); //新建文件夹   
            }
            FilePath = FilePath + "\\" + DateTime.Now.Month;
            if (!Directory.Exists(FilePath))//若文件夹不存在则新建文件夹   
            {
                Directory.CreateDirectory(FilePath); //新建文件夹   
            }


            FilePath += "\\" + DateTime.Now.Day + ".txt";
            FilePathConfirm(FilePath);
            StreamWriter fs = new StreamWriter(FilePath, true);
            fs.WriteLine(DateTime.Now.ToString() + ": " + Message);
            fs.Close();
        }

        //判断路径文件
        public static void FilePathConfirm(string filepath)
        {
            //filepath = filepath.Replace("/", "\\");
            //string path = filepath.Substring(0, filepath.LastIndexOf("\\"));


            //if (!File.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            if (!File.Exists(filepath))
            {
                StreamWriter sr = File.CreateText(filepath);
                sr.WriteLine();
                sr.Close();

            }
            if (File.GetAttributes(filepath).ToString().IndexOf("ReadOnly") != -1)
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
            }
        }
    }
}
