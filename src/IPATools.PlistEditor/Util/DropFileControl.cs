using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IPATools.PlistEditor.Util
{
    public class DropFileEventArgs : EventArgs
    {
        public IEnumerable<string> Files { get; protected set; }

        public DropFileEventArgs(IEnumerable<string> files)
        {
            if (files == null)
            {
                files = new string[0];
            }
            Files = files;
        }
    }


    public class DropFileControl
    {
        public event EventHandler<DropFileEventArgs> DropNotice;

        public DropFileControl(Control control)
        {
            Bind(control);
        }

        public DropFileControl(Form form)
        {
            Bind(form);
        }

        public void Bind(Control control)
        {
            control.AllowDrop = true;
            control.DragEnter += control_DragEnter;
            control.DragDrop += control_DragDrop;
        }

        public void Bind(Form form)
        {
            form.AllowDrop = true;
            form.DragEnter += control_DragEnter;
            form.DragDrop += control_DragDrop;
        }

        void control_DragDrop(object sender, DragEventArgs e)
        {
            System.Array ar = e.Data.GetData(DataFormats.FileDrop) as System.Array;
            if (ar != null && this.DropNotice!=null)
            {
                DropFileEventArgs dfe = new DropFileEventArgs(ar.Cast<string>());
                DropNotice(sender, dfe);
            }
        }

        void control_DragEnter(object sender, DragEventArgs e)
        {
            Debug.WriteLine("c");
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None; 
        }

    }
}
