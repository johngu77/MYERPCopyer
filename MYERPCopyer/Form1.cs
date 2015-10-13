using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MYERPCopyer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            listBox1.Items.Clear();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BringToFront();
        }

        public void setTitle(string Company)
        {
            label1.Text = string.Format("{0}ERP版本更新", Company);
        }

        public void setVersion(string rv, string lv)
        {
            label2.Text = string.Format("当前版本：{0} 最新版本：{1}", lv, rv);
            BringToFront();
        }

        public void setinfo(string info)
        {
            int x = listBox1.Items.Add(info);
            listBox1.SelectedIndex = x;
            BringToFront();
            Select();
        }
    }
}