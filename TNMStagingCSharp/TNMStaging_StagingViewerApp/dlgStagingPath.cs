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
    public partial class dlgStagingPath : Form
    {
        public dlgStagingPath(String sStagingPath)
        {
            InitializeComponent();

            richTextBoxPath.Text = sStagingPath;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
