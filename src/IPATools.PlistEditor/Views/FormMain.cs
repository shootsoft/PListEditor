using FarsiLibrary.Win;
using ICSharpCode.TextEditor;
using IPATools.PlistEditor.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace IPATools.PlistEditor
{
    public partial class FormMain : Form
    {
        MainController controller;

        public FormMain(params string[] args)
        {
            InitializeComponent();
            this.Text = Program.TITLE;
            controller = new MainController(this);
            if (args != null && args.Length > 0)
            {
                foreach (var item in args)
                {
                    controller.Open(this.faTabStripMain, item);
                }
            }

        
        }


        protected override void DefWndProc(ref Message m) 
        {
            switch (m.Msg)
            {
                //接收自定义消息MYMESSAGE，并显示其参数
                case 0X400:
                    string path = File.ReadAllText(Path.Combine(Application.StartupPath, "Open.tmp"), Encoding.UTF8);
                    //string str = Marshal.PtrToStringUni(m.LParam);
                    //object obj =m.GetLParam(typeof(string));
                    //string path = obj.ToString();
                    controller.Open(this.faTabStripMain, path);
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        #region Menu

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.CreateNew(faTabStripMain);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Save();
        }

        private void saveBinaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Save(null, true, true);
        }

        private void savetextFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Save(null, true, false);
        }


        private void addExplorerMenuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.BindSystem();
        }


        #endregion

        #region  Button

        private void buttonNew_Click(object sender, EventArgs e)
        {
            controller.CreateNew(faTabStripMain);
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            controller.Open(faTabStripMain);
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            controller.Save();
        }


        #endregion

        private void faTabStripMain_TabStripItemSelectionChanged(TabStripItemChangedEventArgs e)
        {
            controller.SetActivate(e.Item);
        }

        private void faTabStripMain_TabStripItemClosing(TabStripItemClosingEventArgs e)
        {
            controller.Close(e);
        }

        private void formatXmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.FormatXml();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.EditUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.EditRedo();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Exit(null);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            controller.Exit(e);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void tabSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Save();
        }

        private void tabOpenPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.OpenPath();
        }

        private void tabCloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            controller.Close();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controller.Close();
        }

        private void denoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/shootsoft/PListEditor");
        }

       
    }
}
