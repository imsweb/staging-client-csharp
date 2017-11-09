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
    public partial class dlgHowTo : Form
    {
        public dlgHowTo()
        {
            InitializeComponent();

            lblStep1.Text = "The first step in using the Algorithm Viewer is to select one of the available algorithms. ";
            lblStep1.Text += "You will need to have zip files containing the algorithm files in the same directory as the Viewer. ";
            lblStep1.Text += "The algorithm selection can be found in the upper left-hand corner of the application. ";
            lblStep1.Text += "Once you select a staging algorithm, the application will take a few moments to load the algorithm. ";
            lblStep1.Text += "Once the algorithm is loaded into memory, the program will display information such as the number of schemas available. ";

            lblDisp1.Text = "This tab is used to select specific schemas for view (from those involved in the selected staging algorithm). ";
            lblDisp1.Text += "Once a schema is selected, information about that schema will be displayed on the screen. ";

            lblDisp2.Text = "This tab is used to stage cases for the selected algorithm. First you must select a site and histology. ";
            lblDisp2.Text += "If a schema can be identified from those two values, then the schema name will be displayed and a list of inputs will be shown. ";
            lblDisp2.Text += "If a discriminator is required, then the correct variable and associated list of values will be displayed. ";
            lblDisp2.Text += "Once the input list is shown, you can enter input values directly in the \"Value\" column of the Input Variables list. ";
            lblDisp2.Text += "If you double-click on any variable name, a window will appear showing the pick-list for hte variable. ";
            lblDisp2.Text += "If you double-click on any entry in that window, the corresponding value will be placed in the Input Variables list.";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
