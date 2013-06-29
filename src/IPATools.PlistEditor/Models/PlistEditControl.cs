using FarsiLibrary.Win;
using ICSharpCode.TextEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPATools.PlistEditor
{
    public class PlistEditControl
    {
        public FATabStripItem Tab { get; protected set; }
        public TextEditorControl Editor { get; protected set; }
        public PlistInfo Pinfo { get; protected set; }
        public bool Saved { get; protected set; }


        public PlistEditControl(FATabStrip tabStrip, string title, PlistInfo p = null)
        {
            Editor = new TextEditorControl();
            Tab = new FATabStripItem();
            Tab.Title = title;
            Tab.Controls.Add(Editor);
            Editor.Dock = DockStyle.Fill;
            if (p == null)
            {
                p = new PlistInfo();
            }
            Pinfo = p;
            Editor.Text = p.Content;
            Editor.SetHighlighting("XML");
            Editor.TextChanged += Editor_TextChanged;
            
            tabStrip.AddTab(Tab);
            Saved = true;
        }

        void Editor_TextChanged(object sender, EventArgs e)
        {
            if (Saved && (Editor.Text.Length != Pinfo.Content.Length || Editor.Text!=Pinfo.Content))
            {
                Tab.Title = "* " + Tab.Title;
                Saved = false;
            } 
        }

        public void Save(bool isBinaryFormat = false, string path = "")
        {
            Saved = Pinfo.Save(Editor.Text, isBinaryFormat, path);
            if (!Saved)
            {
                MessageBox.Show("Xml format error or the file cannot be written.");
            }
            else
            {
                Tab.Title = Pinfo.Info.Name;
            }
        }

        internal void FormatXml()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(Editor.Text);

            System.IO.StringWriter sw = new System.IO.StringWriter();
            using (System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(sw))
            {
                writer.Indentation = 2;  // the Indentation
                writer.Formatting = System.Xml.Formatting.Indented;
                doc.WriteContentTo(writer);
                writer.Close();
            }
            Editor.Text = sw.ToString();
        }
    }
}
