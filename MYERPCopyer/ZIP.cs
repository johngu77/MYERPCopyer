﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using SevenZip.Sdk;
using SevenZip;

namespace MYERPCopyer
{
    /// <summary> 
    /// 适用与ZIP压缩 
    /// </summary> 
    public class ZipHelper
    {
        public delegate void Unziping(int Percent, string fileName);
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="forComposeDirectory">文件夹路径</param>
        /// <param name="ZipFileName">形成的zip文件</param>
        public static void CreateZipFile(string forComposeDirectory, string ZipFileName)
        {

            if (!Directory.Exists(forComposeDirectory)) return;
            try
            {
                string[] filenames = Directory.GetFiles(forComposeDirectory);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(ZipFileName)))
                {

                    s.SetLevel(9); // 压缩级别 0-9
                    //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames)
                    {
                        ZipEntry entry = new ZipEntry(Path.GetFileName(file));
                        entry.DateTime = DateTime.Now;
                        s.PutNextEntry(entry);
                        using (FileStream fs = File.OpenRead(file))
                        {
                            int sourceBytes;
                            do
                            {
                                sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                s.Write(buffer, 0, sourceBytes);
                            } while (sourceBytes > 0);
                        }
                    }
                    s.Finish();
                    s.Close();
                }
            }
            catch 
            {
            }
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="ZipFileName">解压缩zip文件</param>
        public static void UnZipFile(string ZipFileName, string ExtractToPath, Unziping cZiping)
        {
            if (!File.Exists(ZipFileName)) return;
            int ESum = 0, EIndex = 0;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(ZipFileName)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    ESum++;
                }
            }
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(ZipFileName)))
            {
                ZipEntry theEntry;
                if (Directory.Exists(ExtractToPath)) Directory.Delete(ExtractToPath, true);
                Directory.CreateDirectory(ExtractToPath);
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fName = Path.GetFileName(theEntry.Name);
                    string fileName = string.Format("{0}\\{1}", ExtractToPath, fName);
                    if (fileName != String.Empty)
                    {
                        if (cZiping != null)
                        {
                            EIndex++;
                            int p = (int)Math.Floor(((double)EIndex * 100) / (double)ESum);
                            cZiping(p, fName);
                        }
                        using (FileStream streamWriter = File.Create(fileName))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }


        public static void UnzipDirectory(string ZipFileName,string ExtractToPath,Unziping cZiping)
        {
            FastZipEvents fe = new FastZipEvents();
            int ESum = 0, EIndex = 0;
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(ZipFileName)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    ESum++;
                }
            }
            ESum += 1;
            fe.CompletedFile = (object o, ScanEventArgs arg) =>
            {
                if (cZiping != null)
                {
                    EIndex++;
                    int p = (int)Math.Floor(((double)EIndex * 100) / (double)ESum);
                    cZiping(p, arg.Name);
                }
            };

            FastZip f = new FastZip(fe);
            if (Directory.Exists(ExtractToPath)) Directory.Delete(ExtractToPath, true);
            Directory.CreateDirectory(ExtractToPath);
            f.CreateEmptyDirectories = true;
            f.RestoreDateTimeOnExtract = true;
            f.ExtractZip(ZipFileName, ExtractToPath, string.Empty);
        }


        public static void Create7Zip(string forComposeDirectory, string ZipFileName, Unziping Processing)
        {
            string startuppath;
            if (IntPtr.Size == 4)
                startuppath = string.Format(@"{0}\{1}", System.IO.Directory.GetCurrentDirectory(), "7za-32.dll");
            else
                startuppath = string.Format(@"{0}\{1}", System.IO.Directory.GetCurrentDirectory(), "7za-64.dll");
            SevenZipBase.SetLibraryPath(startuppath);
            SevenZipCompressor fe = new SevenZipCompressor();
            fe.FileCompressionStarted += (object sender, FileNameEventArgs e) =>
            {
                if (Processing != null) Processing((int)e.PercentDone, Path.GetFileName(e.FileName));
            };
            fe.CompressionLevel = CompressionLevel.High;
            fe.CompressionMode = CompressionMode.Create;
            fe.CompressDirectory(forComposeDirectory, ZipFileName);
        }

        public static void Extra7Zip(string ZipFileName, string ExtractToPath, Unziping Processing)
        {
            string startuppath;
            if (IntPtr.Size == 4)
                startuppath = string.Format(@"{0}\{1}", System.IO.Directory.GetCurrentDirectory(), "7za-32.dll");
            else
                startuppath = string.Format(@"{0}\{1}", System.IO.Directory.GetCurrentDirectory(), "7za-64.dll");
            SevenZipBase.SetLibraryPath(startuppath);
            SevenZipExtractor fe = new SevenZipExtractor(ZipFileName);
            fe.FileExtractionStarted += (object sender, FileInfoEventArgs e) =>
            {
                if (Processing != null) Processing((int)e.PercentDone, Path.GetFileName(e.FileInfo.FileName));
            };
            fe.ExtractArchive(ExtractToPath);
        }
    }
}
