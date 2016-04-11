using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace MYERPCopyer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //Copyer C = new Copyer();
            //C.Entry();
            frmInfo f;
            if (args != null && args.Count()>0)
            {
                f = new frmInfo(args[0]);
            }
            else
            {
                f = new frmInfo();
            }
            Application.Run(f);
        }

        private class Copyer
        {
            private Thread _MainTD;
            private string _OutputWord;
            private Icon ico = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            private NotifyIcon nficon = new NotifyIcon();

            public static void KillMyself()
            {
                string bat = @"@echo off
:tryagain1
del %2
if exist %2 goto tryagain1
:tryagain
del %1
if exist %1 goto tryagain
del %0";
                File.WriteAllText("killme.bat", bat);//写bat文件
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "killme.bat";
                psi.Arguments = string.Format("\"{0}\" \"{1}\"", Application.ExecutablePath, string.Format("{0}\\UpdateLog.txt", Application.StartupPath));
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                Thread.Sleep(2000);
                Process.Start(psi);
            }

            public static DriveInfo lastDrive()
            {
                DriveInfo[] x = DriveInfo.GetDrives();
                for (int i = x.Length - 1; i >= 0; i--)
                {
                    if (x[i].DriveType == DriveType.Fixed)
                        return x[i];
                }
                return null;
            }

            public void Entry()
            {
                FileInfo logFile = new FileInfo(string.Format("{0}\\UpdateLog.txt", Application.StartupPath));
                if (logFile.Exists) logFile.Delete();
                FileStream logStream = logFile.Create(); logStream.Flush();
                StreamWriter logWriter = new StreamWriter(logStream, System.Text.Encoding.Unicode);
                string longcharter = new string(Convert.ToChar("-"), 20);
                logWriter.WriteLine("{0}开始{0}", longcharter);
                Form1 f = new Form1();
                f.Show();
                f.setVersion("正在检查", "正在检查");
                f.TopMost = true;
                Application.DoEvents();
                Thread.Sleep(300);
                try
                {
                    logWriter.WriteLine("开始更新程序");
                    nficon.Icon = ico; logWriter.WriteLine("设置图标OK。");
                    nficon.Visible = true; logWriter.WriteLine("设置托盘OK。");
                    nficon.ShowBalloonTip(20000, "明扬管理系统", "系统更新和初始化。", ToolTipIcon.Info);
                    logWriter.Write("准备开始进入更新过程。等1秒...");
                    Application.DoEvents();
                    Thread.Sleep(1000);
                    logWriter.WriteLine("开始。");
                    string localPath = Application.StartupPath;
                    if (lastDrive() == null) return;
                    string dir = string.Format("{0}MYERP-NT", lastDrive().RootDirectory.FullName); logWriter.WriteLine("安装位置：{0}", dir);
                    string rFileName = string.Format("{0}\\{1}.exe", dir, System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
                    logWriter.WriteLine("运行位置：{0}", rFileName);
                    string showword = string.Format("本地路径：{0}\r\n运行路径：{1}\r\n{2}", dir, localPath, localPath != dir ? "不一致，需要重新安装。" : "一致，检查更新。");
                    nficon.ShowBalloonTip(20000, "明扬管理系统", showword, ToolTipIcon.Info);
                    logWriter.WriteLine("{0}\r\n{1}\r\n{0}", new string(Convert.ToChar("-"), 50), showword);
                    if (localPath != dir)
                    {
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        nficon.ShowBalloonTip(50000, "明扬管理系统", "拷贝更新程序到正确位置，稍等。\n正在等待上一个操作结束。", ToolTipIcon.Info);
                        logWriter.WriteLine("{2}\r\n准备拷贝。\r\n从：{0}\r\n到：{1}", Application.ExecutablePath, rFileName, longcharter);
                        DateTime starttime1 = DateTime.Now;
                        int u = 0;
                        do
                        {
                            try
                            {
                                int tsen = Convert.ToInt32(Math.Floor((DateTime.Now - starttime1).TotalSeconds));
                                if (tsen % 2 == 0)
                                {
                                    u++;
                                    if (u > 10)
                                    {
                                        u = 100;
                                        logWriter.WriteLine("超时退出");
                                        break;
                                    }
                                    logWriter.Write("第{0}次尝试，耗时：{1}，", u, tsen);
                                    File.Copy(Application.ExecutablePath, rFileName, true);
                                    break;
                                }
                            }
                            catch (Exception exx)
                            {
                                logWriter.WriteLine("出错：{0}", exx.Message);
                                Thread.Sleep(1000);
                            }
                        } while (true);

                        if (u == 100) throw new Exception("无法将更新程序拷贝到正确位置。");

                        nficon.ShowBalloonTip(20000, "明扬管理系统", string.Format("拷贝到：{0}\r\n等候5秒重启。", rFileName), ToolTipIcon.Info);

                        logWriter.WriteLine("拷贝完成，耗时：{0:0.0000}\r\n{1}", (DateTime.Now - starttime1).TotalSeconds, longcharter);
                        Thread.Sleep(5000);
                        if (File.Exists(rFileName))
                        {
                            Process.Start(rFileName);
                        }
                        nficon.ShowBalloonTip(20000, "明扬管理系统", "重启了，2秒后自动退出。", ToolTipIcon.Info);
                        logWriter.WriteLine("已经调用了{0}，2秒后自动删除自身。", rFileName);
                        Thread.Sleep(2000);
                        nficon.Dispose();
                        logWriter.WriteLine("显示资源已经回收。", rFileName);
                        KillMyself();
                        logWriter.WriteLine("自杀程序运行了。", rFileName);
                        return;
                    }
                    logWriter.WriteLine("{0}\r\n{1}", new string(Convert.ToChar("-"), 50), "开启更新线程。");
                    _MainTD = new Thread(CopyFile);
                    _MainTD.Start();
                    logWriter.WriteLine("{1}\r\n{0}", new string(Convert.ToChar("-"), 50), "线程已经开启。");
                    DateTime dBegin = DateTime.Now;
                    int i = 0;

                    do
                    {
                        string sayword;
                        double t = (DateTime.Now - dBegin).TotalSeconds;
                        lock (this)
                        {
                            sayword = _OutputWord;
                            _OutputWord = "";
                        }
                        try
                        {
                            if (sayword != null)
                            {
                                if (sayword.Length > 0)
                                {
                                    i++;
                                    string tt = string.Format("{0}.{1} 耗时：{2:0.0000}秒", i, sayword, t);
                                    nficon.ShowBalloonTip(20000, "明扬管理系统", tt, ToolTipIcon.Info);
                                    if (sayword.Substring(0, 2) == "EE")
                                    {
                                        string lvv = Regex.Match(sayword, @"(?<=LV\=)(.+)(?=\,)").Value;
                                        string rvv = Regex.Match(sayword, @"(?<=\,RV\=)(.+)$").Value;
                                        f.setVersion(rvv, lvv);
                                        Thread.Sleep(100);
                                    }
                                    else
                                    {
                                        f.setinfo(tt);
                                        Thread.Sleep(100);
                                    }
                                    logWriter.WriteLine("{0}", tt);
                                    Application.DoEvents();
                                    dBegin = DateTime.Now;
                                }
                            }
                        }
                        catch (Exception exp)
                        {
                            string errword1 = string.Format("错误：{0}\r\n{1}\r\n", exp.Message, exp.StackTrace);
                            nficon.ShowBalloonTip(20000, "明扬管理系统", errword1, ToolTipIcon.Info);
                            logWriter.WriteLine("{0}", errword1);
                            continue;
                        }
                        if (sayword == "OK") return;
                    } while (true);
                }
                catch (Exception eax)
                {
                    string errword1 = string.Format("错误：{0}\r\n{1}\r\n", eax.Message, eax.StackTrace);
                    nficon.ShowBalloonTip(20000, "明扬管理系统", errword1, ToolTipIcon.Info);
                    logWriter.WriteLine("{0}", errword1);
                }
                finally
                {
                    f.Close();
                    nficon.Dispose();
                    logWriter.WriteLine("{0}结束{0}", longcharter);
                    logWriter.Flush();
                    logWriter.Close();
                    logStream.Flush();
                    logStream.Close();
                    Application.Exit();
                }
            }

            private void CopyFile()
            {
                string LocalDisk = @"L:";
                string hgword = new string(Convert.ToChar("-"), 50);
                try
                {
                    Thread.Sleep(500);
                    bool makeshortcut = true, startmainsystem = true;
                    string localPath = Application.StartupPath;
                    string remotePath = @"\\192.168.1.6\123\Release", UID = "erpupdate", Password = "abcde_12345";
                    lock (this) { _OutputWord = "正准备连接远程更新位置。"; }
                    Thread.Sleep(50);
                    try
                    {
                        var xu = NetworkNeighborhood.NetDiskConnection.Connect(remotePath, LocalDisk, UID, Password);
                        lock (this) { _OutputWord = string.Format("连接远程更新位置{2}：“{0}”\r\n  到本地：“{1}”，\r\n  准备读取文件进行核对。", remotePath, LocalDisk, xu); }
                        Thread.Sleep(50);
                        DriveInfo a = new DriveInfo(LocalDisk);
                        FileInfo[] fs = a.RootDirectory.GetFiles();
                        lock (this) { _OutputWord = "获取了远程更新位置的文件，检查更新程序完整性。"; }
                        Thread.Sleep(50);
                        var f = from af in fs
                                where af.Name == "MYERPCopyer.exe"
                                select af;
                        FileInfo thisexe = new FileInfo(Application.ExecutablePath);
                        foreach (FileInfo aa in f)
                        {
                            string word2 = string.Format("远程更新程序的修改时间：{0}\r\n  本地更新程序的修改时间：{1}\r\n  {2}", aa.LastWriteTime, thisexe.LastWriteTime, (aa.LastWriteTime - thisexe.LastWriteTime).Minutes != 0 ? "需要更新更新程序。" : "不需要更新更新程序。");
                            lock (this) { _OutputWord = word2; }
                            Thread.Sleep(50);
                            if ((aa.LastWriteTime - thisexe.LastWriteTime).Minutes != 0)
                            {
                                lock (this) { _OutputWord = "更新程序需要更新，拷贝更新程序到桌面。"; }
                                Thread.Sleep(50);
                                string deskexe = string.Format("{0}\\{1}.exe", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Application.ProductName);
                                aa.CopyTo(deskexe, true);
                                lock (this) { _OutputWord = "拷贝完成，2秒后在桌面重新启动更新程序。"; }
                                Thread.Sleep(2000);
                                if (File.Exists(deskexe))
                                {
                                    Process.Start(deskexe);
                                }
                                lock (this) { _OutputWord = "新更新程序已经在桌面启动，自动退出。"; }
                                Thread.Sleep(50);
                                makeshortcut = false;
                                startmainsystem = false;
                                return;
                            }
                            else
                            {
                                lock (this) { _OutputWord = "更新程序不需要更新。"; }
                                Thread.Sleep(50);
                                makeshortcut = true;
                                startmainsystem = true;
                            }
                        }
                        lock (this) { _OutputWord = string.Format("检查版本号。startmainsystem={0}", startmainsystem); }
                        Thread.Sleep(50);
                        if (startmainsystem)
                        {
                            var xf = from bf in fs
                                     where bf.Name == "UpdateConfig.xml"
                                     select bf;
                            bool renew = false;
                            foreach (FileInfo bb in xf)
                            {
                                try
                                {
                                    XmlDocument xcDocRemote = new XmlDocument();
                                    xcDocRemote.Load(bb.Open(FileMode.Open));
                                    string rv = ((XmlElement)xcDocRemote.SelectSingleNode("Config/Version")).GetAttribute("Value");
                                    XmlDocument xcDocLocal = new XmlDocument();
                                    string localxmlfile = string.Format("{0}\\UpdateConfig.xml", Application.StartupPath);
                                    if (File.Exists(localxmlfile))
                                    {
                                        xcDocLocal.Load(localxmlfile);
                                        XmlElement lvitem = (XmlElement)xcDocLocal.SelectSingleNode("Config/Version");
                                        string lv = lvitem.GetAttribute("Value");
                                        lock (this) { _OutputWord = string.Format("EE,LV={0},RV={1}", lv, rv); }
                                        Thread.Sleep(500);
                                        lock (this) { _OutputWord = string.Format("远端版本号：{0}\r\n  本地版本号：{1}\r\n  {2}", rv, lv, rv == lv ? "不需要更新。" : "需要更新。"); }
                                        Thread.Sleep(50);
                                        if (rv == lv)
                                            renew = false;
                                        else
                                        {
                                            lock (this) { _OutputWord = "需要更新，正在写入配置。"; }
                                            Thread.Sleep(50);
                                            renew = true;
                                            lvitem.SetAttribute("Value", rv);
                                            xcDocLocal.Save(localxmlfile);
                                        }
                                    }
                                    else
                                    {
                                        lock (this) { _OutputWord = "生成本地版本配置文件。稍候更新系统。"; }
                                        Thread.Sleep(50);
                                        XmlElement xmlc = xcDocLocal.CreateElement("Config");
                                        XmlElement xmlv = xcDocLocal.CreateElement("Version");
                                        xmlv.SetAttribute("Value", rv);
                                        xmlc.AppendChild(xmlv);
                                        xcDocLocal.AppendChild(xmlc);
                                        xcDocLocal.Save(localxmlfile);
                                        renew = true;
                                    }
                                }
                                catch (Exception eee)
                                {
                                    lock (this) { _OutputWord = string.Format("{2}\r\n  出错:{0}\r\n  {1}\r\n  {2}", eee.Message, eee.StackTrace, hgword); }
                                    Thread.Sleep(50);
                                    renew = true;
                                }
                                break;
                            }
                            lock (this) { _OutputWord = string.Format("准备更新，renew={0}", renew); }
                            Thread.Sleep(50);
                            if (renew)
                            {
                                foreach (FileInfo fi in fs)
                                {
                                    try
                                    {
                                        bool Cp = true;
                                        if (fi.Name.ToUpper() == "UpdateConfig.xml".ToUpper()) continue;
                                        if (fi.Name.ToUpper() == "MYERPCopyer.exe".ToUpper()) continue;
                                        string LocalFileName = string.Format("{0}\\{1}", localPath, fi.Name);
                                        if (File.Exists(LocalFileName))
                                        {
                                            FileInfo rf = new FileInfo(LocalFileName);
                                            Cp = (rf.LastWriteTime != fi.LastWriteTime);
                                        }
                                        if (Cp) fi.CopyTo(LocalFileName, true);
                                        if (Cp)
                                        {
                                            lock (this) { _OutputWord = string.Format("{0}复制完成。", fi.Name); }
                                        }
                                        else
                                        {
                                            lock (this) { _OutputWord = string.Format("{0}不需要更新。", fi.Name); }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        lock (this) { _OutputWord = string.Format("{2}\r\n  出错:{0}\r\n  {1}\r\n  {2}", ex.Message, ex.StackTrace, hgword); }
                                        continue;
                                    }
                                    finally
                                    {
                                        Thread.Sleep(50);
                                    }
                                }
                            }
                            else
                            {
                                lock (this) { _OutputWord = string.Format("不需要更新。"); }
                                Thread.Sleep(50);
                                makeshortcut = false;
                                startmainsystem = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (this) { _OutputWord = string.Format("{2}\r\n  出错:{0}\r\n  {1}\r\n  {2}", ex.Message, ex.StackTrace, hgword); }
                        Thread.Sleep(100);
                    }
                    lock (this) { _OutputWord = string.Format("准备生产快捷方式：makeshortcut={0},startmainsystem={1}", makeshortcut, startmainsystem); }
                    Thread.Sleep(50);
                    if (makeshortcut)
                    {
                        try
                        {
                            lock (this) { _OutputWord = "需要生成快捷方式。显示桌面后等1秒。"; }
                            ShowDesktop();
                            Thread.Sleep(500);
                            string desktopDir = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                            string lnkFile = string.Format("{0}\\明揚ERP系統.lnk", desktopDir);
                            if (File.Exists(lnkFile)) File.Delete(lnkFile);
                            WindowsShortcut.Shortcut WC = new WindowsShortcut.Shortcut();
                            WC.Description = "蘇州明揚新ERP系統";
                            WC.Path = string.Format("{0}\\{1}", localPath, "MYERP.exe");
                            WC.WorkingDirectory = string.Format("{0}", localPath);
                            WC.Save(lnkFile);
                            lock (this) { _OutputWord = string.Format("{0}生成快捷方式到桌面{1}。", WC.Path, lnkFile); }
                            Thread.Sleep(50);
                        }
                        catch (Exception exx)
                        {
                            lock (this) { _OutputWord = string.Format("{2}\r\n  出错:{0}\r\n  {1}\r\n  {2}", exx.Message, exx.StackTrace, hgword); }
                            Thread.Sleep(500);
                        }
                    }
                    if (startmainsystem)
                    {
                        if (!makeshortcut) ShowDesktop();
                        lock (this) { _OutputWord = "1秒后开启ERP系统。"; }
                        Thread.Sleep(1000);
                        Process.Start(string.Format("{0}\\{1}", localPath, "MYERP.exe"));
                        lock (this) { _OutputWord = "ERP系统已经启动，准备退出引导程序。"; }
                        Thread.Sleep(50);
                    }
                }
                catch (Exception eexx)
                {
                    lock (this) { _OutputWord = string.Format("{2}\r\n出错:{0}\r\n{1}\r\n{2}", eexx.Message, eexx.StackTrace, hgword); }
                    Thread.Sleep(500);
                }
                finally
                {
                    lock (this) { _OutputWord = "撤销远程服务器的映射。"; }
                    NetworkNeighborhood.NetDiskConnection.Disconnect(LocalDisk);
                    lock (this) { _OutputWord = "撤销完成。退出。"; }
                    Thread.Sleep(90);
                    lock (this) { _OutputWord = "OK"; }
                    Thread.Sleep(90);
                }
            }

            private void ShowDesktop()
            {
                Type shellType = Type.GetTypeFromProgID("Shell.Application");
                object shellObject = System.Activator.CreateInstance(shellType);
                shellType.InvokeMember("ToggleDesktop", System.Reflection.BindingFlags.InvokeMethod,
                    null, shellObject, null);
            }
        }
    }
}