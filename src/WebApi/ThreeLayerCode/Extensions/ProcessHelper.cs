using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System.Diagnostics
{
    public class ProcessHelper
    {
        public static Process Create(string programName)
        {
            Process p = new Process();
            //设置要启动的应用程序
            p.StartInfo.FileName = programName;
            //是否使用操作系统shell启动
            p.StartInfo.UseShellExecute = false;
            // 接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardInput = true;
            //输出信息
            p.StartInfo.RedirectStandardOutput = true;
            // 输出错误
            p.StartInfo.RedirectStandardError = true;
            //不显示程序窗口
            p.StartInfo.CreateNoWindow = true;
            //启动程序
            p.Start();
            return p;
        }

        public static void Push(Process p, string strInput)
        {
            p.StandardInput.WriteLine(strInput);
        }
        public static string GetResult(Process p)
        {

            p.StandardInput.AutoFlush = true;
            //获取输出信息
            string strOuput = p.StandardOutput.ReadToEnd();
            //等待程序执行完退出进程
            p.WaitForExit();
            Console.WriteLine(strOuput);
            p.Close();
            return strOuput;
        }

    }
}
