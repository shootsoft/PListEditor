using IPAParser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IPATools.PlistEditor
{
    public class PlistInfo : IDisposable
    {
        public FileInfo Info { get; protected set; }
        public string TempXml { get; protected set; }
        public string Content { get; set; }
        public bool IsBinary { get; protected set; }
        public bool IsTemp { get; protected set; }
        private FileStream fs;
        private plistType pt;

        public PlistInfo(string file = "")
        {
            file = Init(file);
        }

        private string Init(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".plist");
                this.IsTemp = true;
            }
            Info = new FileInfo(file);
            Content = string.Empty;
            return file;
        }

        public bool Read()
        {
            bool b = false;
            if (Info.Exists)
            {
                fs = new FileStream(Info.FullName, FileMode.Open, FileAccess.ReadWrite);
                pt = Plist.getPlistType(fs);
                IsBinary = pt == plistType.Binary;
                fs.Position = 0;
                try
                {
                    object obj = Plist.readPlist(fs, pt);
                    Content = Plist.writeXml(obj);
                    b = true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    try
                    {
                        if (fs != null && pt == plistType.Xml)
                        {
                            fs.Position = 0;
                            byte[] bytes = new byte[(int)fs.Length];
                            if (fs.Read(bytes, 0, bytes.Length)>0)
                            {
                                Content = Encoding.UTF8.GetString(bytes);
                            }                      
                        }
                    }
                    catch (Exception exx)
                    {
                        Debug.WriteLine(exx);
                    }
                }

            }
            return b;
        }

        internal bool Save(string content, bool isBinaryFormat = false, string path = "")
        {
            bool returnValue = true;
            object obj = null;
            try
            {
                obj = Plist.readPlist(Encoding.UTF8.GetBytes(content));
                //需要重新写文件
                if (!string.IsNullOrEmpty(path))
                {
                    if (isBinaryFormat)
                    {
                        pt = plistType.Binary;
                    }
                    else
                    {
                        pt = plistType.Xml;
                    }
                    IsBinary = isBinaryFormat;
                }
                else
                {
                    path = Info.FullName;
                }
                returnValue = CreateFileStream(path);

                if (returnValue)
                {
                    if (pt == plistType.Binary)
                    {
                        Plist.writeBinary(obj, fs);
                    }
                    else
                    {
                        Plist.writeXml(obj, fs);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                returnValue = false;
            }
            return returnValue;
        }

        private bool CreateFileStream(string path)
        {
            bool returnValue = false;
            try
            {
                Info = new FileInfo(path);
                if (fs != null)
                {
                    fs.Close();
                }
                if (Info.Exists)
                {
                    Info.Delete();
                }
                fs = new FileStream(Info.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                returnValue = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return returnValue;
        }

        public void Dispose()
        {
            fs.Dispose();
        }
    }
}
