using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;

namespace MYERPCopyer
{
    public partial class frmInfo : Form
    {
        private string args {get;set;}
        public frmInfo()
        {
            this.args = string.Empty;
            InitializeComponent();
        }

        public frmInfo (string args)
        {
            this.args = args;
            InitializeComponent();
        }

        private void frmInfo_Load(object sender, EventArgs e)
        {
            lblTitle.Text = string.Format("ERP更新工具");
            lblDescription.Text = "正在檢查更新設置。";
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            lbList.Items.Clear();
            pbProcess.Value = pbProcess.Maximum;
            UpdateLoader();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x84) //WM_NCHITTEST
            {
                m.Result = (IntPtr)2; //HTCAPTION
            }
        }

        private void UpdateLoader()
        {
            Thread t = new Thread(new ThreadStart(UpdateSystem));
            t.IsBackground = true;
            t.Start();
        }

        private int ShowInList(string Word, int xIndex, bool Cover)
        {
            try
            {
                if (xIndex > 0)
                {
                    string rWord = InfoDescription;
                    Invoke((MethodInvoker)delegate
                    {
                        if (Cover)
                            rWord = Word;
                        else
                            rWord = string.Format("{0}{1}", rWord, Word);
                        lbList.Items[xIndex] = rWord;
                        lbList.SelectedIndex = xIndex;
                        Application.DoEvents();
                    });
                    InfoDescription = rWord;
                    return xIndex;
                }
                else
                {
                    int x = 0;
                    Invoke((MethodInvoker)delegate
                    {
                        x = lbList.Items.Add(Word);
                        lbList.SelectedIndex = x;
                        Application.DoEvents();
                    });
                    InfoDescription = Word;
                    return x;
                }
            }
            catch
            {
                return 0;
            }
        }

        private int ShowInList(string Word)
        {
            return ShowInList(Word, 0);
        }

        private int ShowInList(string Word, int xIndex)
        {
            return ShowInList(Word, xIndex, false);
        }

        string _InfoTitle = string.Empty;
        private string InfoTitle
        {
            get
            {
                return _InfoTitle;
            }
            set
            {
                _InfoTitle = value;
                Invoke((MethodInvoker)delegate
                {
                    lblTitle.Text = _InfoTitle;
                    Application.DoEvents();
                });
            }
        }

        string _InfoDescription = string.Empty;
        private string InfoDescription
        {
            get
            {
                return _InfoDescription;
            }
            set
            {
                _InfoDescription = value;
                Invoke((MethodInvoker)delegate
                {
                    lblDescription.Text = _InfoDescription;
                    Application.DoEvents();
                });
            }
        }

        private void UpdateSystem()
        {
            Thread.Sleep(600);
            int x = ShowInList("更新開始....");
            if (args.Length >0)
            {
                x = ShowInList("由系统调用，等待2秒后启动...");

                Application.DoEvents();
                Thread.Sleep(2000);
            }
            x = ShowInList("檢查本機IP:");
            Thread.Sleep(600);
            string LocalIP = MYERPCopyer.NetworkNeighborhood.NetDiskConnection.GetLocalIp();
            x = ShowInList(LocalIP, x);
            string RemoteIP = "192.168.1.6", RemoteFileUrl, RemoteConfigUrl, RemoteCopyerUrl, CompanyTitleName;
            Regex rgx = new Regex(@"(\d+)(?=\.)");
            int IPSect = int.Parse(rgx.Matches(LocalIP)[2].Value);
            Thread.Sleep(600);
            switch (IPSect)
            {
                case 0:
                    RemoteIP = "192.168.1.6";
                    RemoteConfigUrl = string.Format(@"http://{0}/Config/MY/Start.xml", RemoteIP);
                    RemoteFileUrl = string.Format(@"http://{0}/Config/MY/Setup.zip", RemoteIP);
                    RemoteCopyerUrl = string.Format(@"http://{0}/Config/MY/MYERPCopyer.zip", RemoteIP);
                    CompanyTitleName = "明扬";
                    break;
                case 16:   //明泰
                    RemoteIP = "192.168.16.41";
                    RemoteConfigUrl = string.Format(@"http://{0}/Config/MT/Start.xml", RemoteIP);
                    RemoteFileUrl = string.Format(@"http://{0}/Config/MT/Setup.zip", RemoteIP);
                    RemoteCopyerUrl = string.Format(@"http://{0}/Config/MT/MYERPCopyer.zip", RemoteIP);
                    CompanyTitleName = "明泰";
                    break;
                case 10:
                    RemoteIP = "192.168.16.41";
                    RemoteConfigUrl = string.Format(@"http://{0}/Config/MT/Start.xml", RemoteIP);
                    RemoteFileUrl = string.Format(@"http://{0}/Config/MT/Setup.zip", RemoteIP);
                    RemoteCopyerUrl = string.Format(@"http://{0}/Config/MT/MYERPCopyer.zip", RemoteIP);
                    CompanyTitleName = "明翔";
                    break;
                default:
                    RemoteIP = "192.168.1.6";
                    RemoteConfigUrl = string.Format(@"http://{0}/Config/MY/Start.xml", RemoteIP);
                    RemoteFileUrl = string.Format(@"http://{0}/Config/MY/Setup.zip", RemoteIP);
                    RemoteCopyerUrl = string.Format(@"http://{0}/Config/MY/MYERPCopyer.zip", RemoteIP);
                    CompanyTitleName = "明扬";
                    break;
            }
            x = ShowInList(string.Format(",設置服務器:{0}", RemoteIP), x);

            string StartPath = System.Environment.CurrentDirectory;
            string SetupPath = @"C:\MYERP-NT";
            ///啓動不是在正確的安裝位置，自動重新生成安裝位置，并且自動重新安裝。
            if (StartPath.ToLower() != SetupPath.ToLower())
            {
                string LocalUpdateTempCopyer = string.Format("{0}{1}", Path.GetTempPath(), "MYERPCopyer.zip"),
                       LocalUpdateTempCopyerTemp = string.Format("{0}{1}", Path.GetTempPath(), "MYERPCopyerTemp"),
                       LocalUpdateTempCopyerTempFile = string.Format("{0}{1}", Path.GetTempPath(), "MYERPCopyerTemp\\MYERPCopyer.exe");
                try
                {
                    if (Directory.Exists(LocalUpdateTempCopyerTemp)) Directory.Delete(LocalUpdateTempCopyerTemp, true);
                    if (File.Exists(LocalUpdateTempCopyer)) File.Delete(LocalUpdateTempCopyer);
                }
                catch
                {
                }
                WebClient wc = new WebClient();
                InfoTitle = "正在更新安裝";
                x = ShowInList("下載安裝必要文件...");
                wc.DownloadFile(RemoteCopyerUrl, LocalUpdateTempCopyer);
                x = ShowInList("解壓縮...");
                ZipHelper.UnZipFile(LocalUpdateTempCopyer, LocalUpdateTempCopyerTemp, null);
                x = ShowInList(@"清理安裝文件夾，安裝到C:\MYERP-NT");
                if (Directory.Exists(SetupPath)) Directory.Delete(SetupPath, true);
                Directory.CreateDirectory(SetupPath);
                string SetupCopyerFileName = string.Format("{0}\\{1}", SetupPath, "MYERPCopyer.exe");
                File.Copy(LocalUpdateTempCopyerTempFile, SetupCopyerFileName);
                x = ShowInList(@"安裝工具已經更新，1秒鐘后開始安裝。");
                try
                {
                    Directory.Delete(LocalUpdateTempCopyerTemp, true);
                    File.Delete(LocalUpdateTempCopyer);
                }
                catch
                {
                }
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = SetupCopyerFileName;
                psi.WorkingDirectory = @"C:\MYERP-NT";
                Process.Start(psi);
                Invoke((MethodInvoker)delegate
                {
                    Hide();
                    Thread t = new Thread(new ThreadStart(KillMyself));
                    t.Start();
                    Thread.Sleep(1000);
                    Close();
                    Application.Exit();
                });
                return;
            }

            string LocalUpdateTempPath = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "UpdateTEMP");
            ///检查是否含有更新文件夹，如果没有就直接重新下载。
            if (!(Directory.Exists(LocalUpdateTempPath) && Directory.GetFiles(LocalUpdateTempPath).Count() > 0))
            {
                string LocalUpdateTempZIP = string.Format("{0}\\{1}", System.Environment.CurrentDirectory, "Setup.zip");
                WebClient wc = new WebClient();
                InfoTitle = "获取更新文件。";
                x = ShowInList("下载安装包，重新安装...");
                bool ProcessComplate = false;
                wc.DownloadProgressChanged += (object sender, DownloadProgressChangedEventArgs e) =>
                {
                    Invoke((MethodInvoker)delegate
                    {
                        pbProcess.Maximum = 100;
                        x = ShowInList(string.Format("正在下载安装包.....{0}%", e.ProgressPercentage), x, true);
                        pbProcess.Value = e.ProgressPercentage;
                        Application.DoEvents();
                    });
                };
                wc.DownloadFileCompleted += (object sender, AsyncCompletedEventArgs e) =>
                {
                    ProcessComplate = true;
                };
                wc.DownloadFileAsync(new Uri(RemoteFileUrl), LocalUpdateTempZIP);
                while (!ProcessComplate)
                {
                    Application.DoEvents();
                    Thread.Sleep(200);
                }
                x = ShowInList("下载完成", x, true);
                if (File.Exists(LocalUpdateTempZIP))
                {
                    x = ShowInList("解压缩。");
                    x = ShowInList("正在解压。");
                    Application.DoEvents();
                    Thread.Sleep(1000);
                    if (Directory.Exists(LocalUpdateTempPath)) Directory.Delete(LocalUpdateTempPath, true);
                    ZipHelper.UnZipFile(LocalUpdateTempZIP, LocalUpdateTempPath,
                        (int Percent, string fileName) =>
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                pbProcess.Maximum = 100;
                                InfoTitle = string.Format("解压缩  {0}%", Percent);
                                x = ShowInList(string.Format("正在解压缩.....{0}%", Percent), x, true);
                                pbProcess.Value = Percent;
                                Application.DoEvents();
                            });
                            this.InfoDescription = fileName;
                        }
                        );
                    if (File.Exists(LocalUpdateTempZIP)) File.Delete(LocalUpdateTempZIP);
                }
            }
            ///找到更新文件夹，直接复制后打开。
            if (Directory.Exists(LocalUpdateTempPath) && Directory.GetFiles(LocalUpdateTempPath).Count() > 0)
            {
                x = ShowInList("更新。準備複製文件....");
                InfoTitle = "複製檔案...";
                Process processes = Process.GetCurrentProcess();
                string thisexename = processes.ProcessName;
                string[] files = Directory.GetFiles(LocalUpdateTempPath);
                Invoke((MethodInvoker)delegate
                {
                    pbProcess.Maximum = files.Length + 2;
                    pbProcess.Minimum = 0;
                    pbProcess.Value = 1;
                });
                foreach (var item in files)
                {
                    if (Path.GetFileName(item).ToLower() == thisexename.ToLower()) continue;
                    string UpdateTmpFile = string.Format("{0}\\{1}", LocalUpdateTempPath, Path.GetFileName(item)),
                           RealFile = string.Format("C:\\MYERP-NT\\{0}", Path.GetFileName(item));
                    InfoTitle = string.Format("複製檔案... {0:0%}", (double)pbProcess.Value / (double)pbProcess.Maximum);
                    InfoDescription = string.Format("正在Copy {0}...", Path.GetFileName(item));
                    Invoke((MethodInvoker)delegate
                    {
                        pbProcess.Value += 1;
                        Application.DoEvents();
                    });
                    try
                    {
                        if ((!File.Exists(RealFile)) || !IsFileInUse(RealFile))
                        {
                            File.Copy(UpdateTmpFile, RealFile, true);
                            x = ShowInList("Done.", x);
                        }
                        else
                        {
                            x = ShowInList("占用.", x);
                        }
                    }
                    catch
                    {
                        x = ShowInList("Error.", x);
                    }
                    x = ShowInList(string.Format("複製檔案... {0:0%}", (double)pbProcess.Value / (double)pbProcess.Maximum), x, true);
                }
                x = ShowInList("複製完成。", x, true);
                Directory.Delete(LocalUpdateTempPath, true);
            }

            try
            {
                ///开始生成快捷方式。和重新待开
                string desktopDir = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                string lnkFile = string.Format("{0}\\{1}ERP系統.lnk", desktopDir, CompanyTitleName);
                if (File.Exists(lnkFile)) File.Delete(lnkFile);
                WindowsShortcut.Shortcut WC = new WindowsShortcut.Shortcut();
                WC.Description = string.Format("{0}新ERP系統", CompanyTitleName);
                WC.Path = string.Format("C:\\MYERP-NT\\{0}", "MYERP.exe");
                WC.WorkingDirectory = "C:\\MYERP-NT\\";
                WC.Save(lnkFile);
            }
            catch
            {

            }

            XDocument xdoc = XDocument.Load(RemoteConfigUrl);
            var vXml = from a in xdoc.Element("Root").Element("Host").Elements()
                       select a;
            var vvXml = from a in vXml.Elements()
                        where a.Name == "Set" && a.Attribute("Key").Value == "Version"
                        select a;
            string VersionValue = vvXml.FirstOrDefault().Attribute("Name").Value;
            //获取最新版本号，写入UpdateConfig.xml
            XmlDocument xUpdateConfig = new XmlDocument();
            XmlDeclaration xUpdateConfig_Decl = xUpdateConfig.CreateXmlDeclaration("1.0", "utf-8", "yes");
            xUpdateConfig.AppendChild(xUpdateConfig_Decl);
            XmlElement xUpdateConfig_Config = xUpdateConfig.CreateElement("Config");
            XmlElement xUpdateConfig_Version = xUpdateConfig.CreateElement("Version");
            xUpdateConfig_Version.SetAttribute("Value", VersionValue);
            xUpdateConfig_Config.AppendChild(xUpdateConfig_Version);
            xUpdateConfig.AppendChild(xUpdateConfig_Config);
            string UpdateConfigFile = @"C:\MYERP-NT\UpdateConfig.xml";
            if (File.Exists(UpdateConfigFile)) File.Delete(UpdateConfigFile);
            xUpdateConfig.Save(@"C:\MYERP-NT\UpdateConfig.xml");

            try
            {
                //重新開啓ERP
                ProcessStartInfo PSMain = new ProcessStartInfo();
                PSMain.FileName = "C:\\MYERP-NT\\MYERP.exe";
                PSMain.WorkingDirectory = @"C:\MYERP-NT";
                Process.Start(PSMain);
            }
            catch
            {

            }
            Invoke((MethodInvoker)delegate
            {
                Thread.Sleep(300);
                Close();
                Application.Exit();
            });
        }

        public static bool IsFileInUse(string fileName)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                fs.Close();
                return false;
            }
            catch
            {
                return true;//true表示正在使用,false没有使用
            }
        }

        public static void KillMyself()
        {
            string bat = @"
@echo off
:tryagain1
del %1
if exist %1 goto tryagain1
del %0
";
            File.WriteAllText("killme.bat", bat);//写bat文件
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "killme.bat";
            psi.Arguments = string.Format("\"{0}\"", Path.GetFullPath(Application.ExecutablePath));
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            Thread.Sleep(1000);
            Process.Start(psi);
        }

    }
}
