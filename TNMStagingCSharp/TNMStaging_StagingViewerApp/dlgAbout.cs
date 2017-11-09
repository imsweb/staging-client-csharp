using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNMStaging_StagingViewerApp
{
    public partial class dlgAbout : Form
    {
        public dlgAbout()
        {
            InitializeComponent();
            linkLabel1.Links.Add(0, 100, linkLabel1.Text);
            linkLabel2.Links.Add(0, 100, linkLabel2.Text);
            linkLabel3.Links.Add(0, 100, linkLabel3.Text);
            linkLabel4.Links.Add(0, 100, linkLabel4.Text);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String sAddr = (string)e.Link.LinkData; 
            System.Diagnostics.Process.Start(sAddr);
        }

    }
}
