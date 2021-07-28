using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public class FtpHelper
    {
        private static string ftpServerIP;
        private static string ftpUserID;
        private static string ftpPassword;
        private static string ftpURI;

        /// <summary>
        /// 连接FTP
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        //private FtpHelper(string FtpServerIP, string FtpUserID, string FtpPassword)
        //{
        //    ftpServerIP = FtpServerIP;
        //    ftpUserID = FtpUserID;
        //    ftpPassword = FtpPassword;
        //    ftpURI = $"ftp://{ftpServerIP}/";
        //}
        private FtpHelper() { }

        public static FtpHelper Instance { get; } = new FtpHelper();

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="FtpServerIP">FTP连接地址</param>
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>
        /// <param name="FtpUserID">用户名</param>
        /// <param name="FtpPassword">密码</param>
        public static void SetSetting(string FtpServerIP, string FtpUserID, string FtpPassword)
        {
            ftpServerIP = FtpServerIP;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = $"ftp://{ftpServerIP}/";
        }

        #region 暂时删除上传方法
        ////上传文件
        //public string UploadFile(string[] filePaths)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (filePaths != null && filePaths.Length > 0)
        //    {
        //        foreach (var file in filePaths)
        //        {
        //            sb.Append(Upload(file));

        //        }
        //    }
        //    return sb.ToString();
        //}

        ///// <summary>
        ///// 上传文件
        ///// </summary>
        ///// <param name="filename"></param>
        //private string Upload(string filename)
        //{
        //    FileInfo fileInf = new FileInfo(filename);
        //    if (!fileInf.Exists)
        //    {
        //        return filename + " 不存在!\n";
        //    }

        //    string uri = ftpURI + fileInf.Name;
        //    FtpWebRequest reqFTP= (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

        //    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        //    reqFTP.KeepAlive = false;
        //    reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
        //    reqFTP.UseBinary = true;
        //    reqFTP.UsePassive = false;  //选择主动还是被动模式
        //    //Entering Passive Mode
        //    reqFTP.ContentLength = fileInf.Length;

        //    using ( FileStream fs = fileInf.OpenRead())
        //    using (Stream strm = reqFTP.GetRequestStream())
        //    {
        //        fs.CopyTo(strm);

        //    }
        //    using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
        //    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        //    {
        //        return reader.ReadToEnd();
        //    }
        //} 
        #endregion


        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        public void Download(string dirPath, string saveFileDir, string fileName)
        {

            var saveFilePath = $"{saveFileDir}\\{fileName}";

            //文件远程地址
            var remoteFilePath = $"{ftpURI}{dirPath.Trim('/')}/{fileName}";

            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(remoteFilePath));
            reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);

            using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
            using (Stream ftpStream = response.GetResponseStream())
            using (FileStream outputStream = new FileStream(saveFilePath, FileMode.Create))
            {
                ftpStream.CopyTo(outputStream);
            }



        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        public string Delete(string filePath)
        {
            string uri = $"{ftpURI}{filePath.Trim('/')}";
            FtpWebRequest reqFTP;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

            string result = String.Empty;
            using (FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse())
            using (Stream datastream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(datastream))
            {
                long size = response.ContentLength;
                result = sr.ReadToEnd();
                return result;
            }
        }

        /// <summary>
        /// 获取当前目录下明细(包含文件和文件夹)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFilesDetailList(string dirPath)
        {
            List<string> retFiles = new List<string>();
            StringBuilder result = new StringBuilder();
            FtpWebRequest ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + dirPath.Trim('/')));
            ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;

            using (WebResponse response = ftp.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    //防止文件过多，需要的时候再取
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }

        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFileList(string dirPath, string mask)
        {
            List<string> downloadFiles = new List<string>();
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri($"{ftpURI}{dirPath.Trim('/')}"));
            reqFTP.UseBinary = true;
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
            using (WebResponse response = reqFTP.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (string.IsNullOrEmpty(mask) == false && mask.Trim() != "*.*")
                    {
                        string mask_ = mask.Substring(0, mask.IndexOf("*"));
                        if (line.Substring(0, mask_.Length) == mask_)
                        {
                            yield return line;
                        }
                    }
                    else
                    {
                        yield return line;
                    }
                    line = reader.ReadLine();
                }
            }



        }

        /// <summary>
        /// 获取当前目录下所有的文件夹列表(仅文件夹)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDirectoryList(string dirpath)
        {
            var drectory = GetFilesDetailList(dirpath);
            foreach (string str in drectory)
            {
                if (str.Trim().Substring(0, 1).ToUpper() == "D")
                {
                    yield return str;
                }
            }

        }

        /// <summary>
        /// 判断当前目录下指定的子目录是否存在
        /// </summary>
        /// <param name="RemoteDirectoryName">指定的目录名</param>
        public bool DirectoryExist(string dirPath, string RemoteDirectoryName)
        {
            var dirList = GetDirectoryList(dirPath);
            foreach (string str in dirList)
            {
                if (str.Trim() == RemoteDirectoryName.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断当前目录下指定的文件是否存在
        /// </summary>
        /// <param name="RemoteFileName">远程文件名</param>
        public bool FileExist(string dirPath, string RemoteFileName)
        {
            var fileList = GetFileList(dirPath, "*.*");
            foreach (string str in fileList)
            {
                if (str.Trim() == RemoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
