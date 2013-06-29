using BrendanGrant.Helpers.FileAssociation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IPATools.PlistEditor.Association
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
            Run();
        }

        public static void Run()
        {
            FileAssociationInfo associate = new FileAssociationInfo(".plist");
            if (!associate.Exists)
            {
                associate.Create();
            }
            associate.ContentType = "Application/PList";
            associate.ProgID = "ProperityList";
            ////if (associate.OpenWithList == null)
            //{
            //    associate.OpenWithList = new string[]{
            //        args
            //    };
            //}
            //else if(!associate.OpenWithList.Contains("args"))
            //{
            //    List<string> list = new List<string>();
            //    list.Add(args);
            //    list.AddRange(associate.OpenWithList);
            //    associate.OpenWithList = list.ToArray();
            //}
            string args = Path.GetFullPath(Path.Combine(Application.StartupPath, "IPATools.PlistEditor.exe")) + " \"%1\"";
            string ico = Path.Combine(Application.StartupPath, "file.ico");
            ProgramVerb open = new ProgramVerb("Open", args);
            ProgramAssociationInfo pai = new ProgramAssociationInfo(associate.ProgID);
            if (!pai.Exists)
            {
                pai.Create("ProperityList", open);
            }
            else
            {
                for (int i = 0; i < pai.Verbs.Length; i++)
                {
                    if (pai.Verbs[i].Name.Equals("open", StringComparison.OrdinalIgnoreCase))
                    {
                        pai.RemoveVerb(pai.Verbs[i]);
                        pai.AddVerb(open);
                        break;
                    }
                }
            }
            pai.DefaultIcon = new ProgramIcon(ico);

        }
    }
}
