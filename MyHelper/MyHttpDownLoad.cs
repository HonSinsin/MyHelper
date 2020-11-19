using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace MyHelper4Web
{
    /// <summary> 
    /// 下载文件 且进度条展示
    /// </summary> 
    public class MyHttpDownLoad
    {
        private static long fileLength;
        private static long downLength;//已经下载文件大小，外面想用就改成公共属性 
        private static bool stopDown;
        public MyHttpDownLoad()
        {
            fileLength = 0;
            downLength = 0;
            stopDown = false;
        }

        /// <summary> 
        /// 文件下载 
        /// </summary> 
        /// <param name="url">连接</param> 
        /// <param name="fileName">本地保存文件名</param> 
        /// <param name="progressBar">进度条</param> 
        /// <param name="label">返回已经下载的百分比</param> 
        public static void HttpDownFile(string url, string fileName, System.Windows.Forms.ProgressBar progressBar, Label label)
        {
            stopDown = false;
            Stream str = null, fs = null;
            try
            {
                //获取下载文件长度 
                fileLength = GetDownLength(url);
                downLength = 0;
                if (fileLength > 0)
                {
                    WebClient DownFile = new WebClient();
                    str = DownFile.OpenRead(url);
                    //判断并建立文件 
                    if (CreateFile(fileName))
                    {
                        byte[] mbyte = new byte[1024];
                        int readL = str.Read(mbyte, 0, 1024);
                        fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                        //读取流 
                        while (readL != 0)
                        {
                            if (stopDown)
                                break;
                            downLength += readL;//已经下载大小 
                            fs.Write(mbyte, 0, readL);//写文件 
                            readL = str.Read(mbyte, 0, 1024);//读流 
                            progressBar.Value = (int)(downLength * 100 / fileLength);
                            label.Text = progressBar.Value.ToString() + "%";
                            System.Windows.Forms.Application.DoEvents();
                        }
                        str.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                if (str != null)
                    str.Close();
                if (fs != null)
                    fs.Close();
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary> 
        /// 文件下载 
        /// </summary> 
        ///<param   name="url">连接</param> 
        ///<param name="fileName">本地保存文件名</param>
        public static void HttpDownFile(string url, string fileName)
        {
            try
            {
                WebClient DownFile = new WebClient();
                DownFile.DownloadFile(url, fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary> 
        /// 获取下载文件大小 
        /// </summary> 
        /// <param   name="url">连接</param> 
        /// <returns>文件长度</returns> 
        private static long GetDownLength(string url)
        {
            try
            {
                WebRequest wrq = WebRequest.Create(url);
                WebResponse wrp = (WebResponse)wrq.GetResponse();
                wrp.Close();
                return wrp.ContentLength;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        /// <summary> 
        /// 建立文件(文件如已经存在，删除重建) 
        /// </summary> 
        /// <param   name="fileName">文件全名(包括保存目录)</param> 
        /// <returns></returns> 
        private static bool CreateFile(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                Stream s = File.Create(fileName);
                s.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public void downClose()
        {
            stopDown = true;
        }
    }
}
