using BrendanGrant.Helpers.FileAssociation;
using FarsiLibrary.Win;
using ICSharpCode.TextEditor;
using IPAParser;
using IPATools.PlistEditor.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPATools.PlistEditor
{
    public class MainController
    {
        public DropFileControl Drop { get; protected set; }

        Form main;
       
        Dictionary<FATabStripItem, PlistEditControl> controls;
        PlistEditControl curEditor;

        public MainController(Form frm)
        {
            main = frm;
            controls = new Dictionary<FATabStripItem, PlistEditControl>();
            Drop = new DropFileControl(frm);
            Drop.DropNotice+=drop_DropNotice;
        }

        public void CreateNew(FarsiLibrary.Win.FATabStrip tabStrip)
        {
            string title = Res.NewPListTitle;
            PlistEditControl c = CreateTab(tabStrip, title);
            c.Editor.Text = Res.NewPListContent;
        }

        private PlistEditControl CreateTab(FarsiLibrary.Win.FATabStrip tabStrip, string title, PlistInfo p = null)
        {
            PlistEditControl c = new PlistEditControl(tabStrip, title, p);
            if (controls.Count == 0 || p!=null)
            {
                curEditor = c;
                tabStrip.SelectedItem = c.Tab;
            }
            c.Tab.ContextMenuStrip = ((FormMain)main).contextMenuStripTab;
            controls.Add(c.Tab, c);
            Drop.Bind(c.Tab);
            return c;
        }

        void drop_DropNotice(object sender, DropFileEventArgs e)
        {
            foreach (var item in e.Files)
            {
                this.Open(
                ((FormMain)main).faTabStripMain, item);
                //this.Open( , item);
            }
        }



        public void Open(FarsiLibrary.Win.FATabStrip faTabStripMain, string path="")
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (string.IsNullOrEmpty(path))
                {
                    ofd.Filter = "PList(*.plist)|*.plist|All files|*.*";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        path = ofd.FileName;
                    }
                }
                if (!string.IsNullOrEmpty(path))
                {
                    try
                    {
                        PlistInfo p = new PlistInfo(path);
                        bool suc = p.Read();
                        CreateTab(faTabStripMain, p.Info.Name, p);

                        if (!suc)
                        {
                            MessageBox.Show("Xml format error");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        Debug.WriteLine(ex);
                    }
                }
            }
        }

        internal void Save(PlistEditControl edit =null, bool saveAs = false, bool isBinaryFormat =false)
        {
            if (edit == null)
            {
                edit = curEditor;
            }
            if (edit == null)
            {
                return;
            }
            //正常打开的文件
            if (!saveAs && !edit.Pinfo.IsTemp)
            {
                edit.Save();
            }
            else
            {
                //新建或者另存为的文件
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "PList(*.plist)|*.plist|All files|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        edit.Save(isBinaryFormat, sfd.FileName);
                    }

                }
            }
            
        }

        internal void SetActivate(FATabStripItem tab)
        {
            if (controls.ContainsKey(tab))
            {
                curEditor = controls[tab];
            }
        }

        internal bool Close(TabStripItemClosingEventArgs e=null)
        {
            bool canClose = true;
            PlistEditControl edit = null;
            if (e == null)
            {
                edit = curEditor;
            }
            else
            {
                if (controls.ContainsKey(e.Item))
                {
                    edit = controls[e.Item];
                }
                else
                {
                    Debug.WriteLine("Err");
                    return false;
                }
            }


            if (!edit.Saved)
            {
                DialogResult dr = MessageBox.Show("Save [" + edit.Tab.Title + "]", "Confirm Close", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dr)
                {
                    case DialogResult.Cancel:
                        if(e!=null)
                        {
                            e.Cancel = true;
                        }
                        canClose = false;
                        break;

                    case DialogResult.No:
                        CloseTab(edit.Tab, e==null);
                        break;

                    default:
                        //Default Yes = Save first and close
                        Save(edit);
                        CloseTab(edit.Tab, e == null);
                        break;
                }

            }
            else
            {
                CloseTab(edit.Tab, e == null);
            }
            return canClose;
        }

        private void CloseTab(FATabStripItem tab, bool goClose)
        {
            PlistEditControl edit = controls[tab];
            edit.Pinfo.Dispose();
            controls.Remove(tab);
            if (goClose)
            {
                ((FATabStrip)tab.Parent).RemoveTab(tab);
            }
        }

        internal void FormatXml()
        {
            if (curEditor != null)
            {
                curEditor.FormatXml();
            }
        }



        internal void EditUndo()
        {
            if (curEditor != null)
            {
                curEditor.Editor.Undo();
            }
        }

        internal void EditRedo()
        {
            if (curEditor != null)
            {
                curEditor.Editor.Redo();
            }
        }

        internal void BindSystem()
        {
            if (MessageBox.Show("Associate .plist with this editor?", "Confirm  File Association",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (File.Exists(Res.IPATools_PlistEditor_Association))
                {
                    try
                    {
                        Process.Start(Res.IPATools_PlistEditor_Association);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Missing: " + Res.IPATools_PlistEditor_Association);
                }

            }
           
        }

        internal void Exit(FormClosingEventArgs e =null)
        {
            bool allSaved = true;
            foreach (var item in controls.Values)
            {
                if (!item.Saved)
                {
                    allSaved = false;
                    break;
                }
            }
            if (!allSaved)
            {
                DialogResult dr = MessageBox.Show("Save your documents before exit?", "Confirm Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                switch (dr)
                { 
                    case DialogResult.Cancel:
                        if (e != null)
                        {
                            e.Cancel = true;
                        }
                        break;
                    case DialogResult.No:
                        main.Hide();
                        
                        break;
                    default:
                        //Yes Save
                        foreach (var item in controls.Values)
                        {
                            if (!item.Saved)
                            {
                                Save(item);
                            }
                        }
                        main.Hide();
                        Application.Exit();
                        break;
                }
            }
        }

        internal void OpenPath()
        {
            if (curEditor != null)
            {
                try
                {
                    Process.Start(curEditor.Pinfo.Info.Directory.FullName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
