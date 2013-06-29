using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IPAParser
{
    public class Parser
    {
        public static byte[] ParsePlist(string ipaPath)
        {
            byte[] buff = new byte[0];   
            if (File.Exists(ipaPath))
            {
                try
                {
                    using (ZipInputStream zip = new ZipInputStream(new FileStream(ipaPath, FileMode.Open)))
                    {
                        ZipEntry file = zip.GetNextEntry();
                        while (file != null)
                        {
                            if (file.Name.ToLower().EndsWith("/info.plist"))
                            {
                                string plist = ipaPath + ".info.plist";
                                buff = new byte[(int)file.Size];
                                zip.Read(buff, 0, (int)file.Size);
                                break;
                            }
                            file = zip.GetNextEntry();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            return buff;
        }

        public static bool ExtractPlist(string ipaPath, string path)
        {
            byte[] buff = ParsePlist(ipaPath);
            if (buff.Length > 0)
            {
                File.WriteAllBytes(path, buff);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Convert2XmlPlist(string plistPath, string xmlPath)
        {
            object p = Plist.readPlist(plistPath);
            Plist.writeXml(p, xmlPath);
        }

        public static void Convert2BinPlist(string plistPath, string xmlPath)
        {
            object p = Plist.readPlist(plistPath);
            Plist.writeBinary(p, xmlPath);
        }
        
        public static Dictionary<string, object> ParseInfo(string ipaPath, params string[] keys)
        {
            return ParseInfo(ipaPath, string.Empty, keys);
        }

        public static Dictionary<string, object> ParseInfo(string ipaPath, string plistPath,  params string[] keys)
        {
            Dictionary<string, object> list = new Dictionary<string, object>();
            HashSet<string> HashKeys = new HashSet<string>(keys);
            byte[] buff = ParsePlist(ipaPath);
            if (!string.IsNullOrEmpty(plistPath))
            {
                File.WriteAllBytes(plistPath, buff);
            }
            Dictionary<string, object> p = Plist.readPlist(buff) as Dictionary<string, object>;
            if (HashKeys.Count == 0)
            {
                return p;
            }
            else
            {
                foreach (var item in p)
                {
                    if (HashKeys.Contains(item.Key))
                    {
                        list.Add(item.Key, item.Value);
                    }
                }
            }
            return list;
        }
    }
}
