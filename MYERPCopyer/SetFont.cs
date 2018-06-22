using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace MYERPCopyer
{

    class SetFont
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);
        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, // handle to destination window
                                             uint Msg, // message
                                             int wParam, // first message parameter
                                             int lParam // second message parameter
                                             );

        [DllImport("gdi32")]
        public static extern int AddFontResource(string lpFileName);



        public static bool InstallFont(string sFontFileName, string sFontName)
        {
            string _sTargetFontPath = string.Format(@"{0}\fonts\{1}", System.Environment.GetEnvironmentVariable("WINDIR"), Path.GetFileName(sFontFileName));//系统FONT目录
            string _sResourceFontPath = sFontFileName;//需要安装的FONT目录
            try
            {
                if (!File.Exists(_sTargetFontPath) && File.Exists(_sResourceFontPath))
                {
                    int _nRet;
                    File.Copy(_sResourceFontPath, _sTargetFontPath);
                    _nRet = AddFontResource(_sTargetFontPath);
                    _nRet = WriteProfileString("fonts", sFontName + "(TrueType)", Path.GetFileName(sFontFileName));
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
