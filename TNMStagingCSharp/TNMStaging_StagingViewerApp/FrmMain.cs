using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using TNMStagingCSharp.Src.Staging;
using System.Diagnostics;

namespace TNMStaging_StagingViewerApp
{
    public partial class FrmMain : Form
    {
        private Staging mStaging = null;


        public FrmMain()
        {
            InitializeComponent();
            LoadAlgorithms();
            UpdateSchemaHeaderValues();
        }

        public void LoadAlgorithms()
        {
            cmbAlgorithms.Items.Clear();

            // Look for zip files in this directory. 
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.zip");
            string sFinalFilename = "";
            foreach (string sFilename in files)
            {
                sFinalFilename = Path.GetFileName(sFilename).Replace(".zip", "");
                cmbAlgorithms.Items.Add(sFinalFilename);
            }
            files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.gz");
            foreach (string sFilename in files)
            {
                sFinalFilename = Path.GetFileName(sFilename).Replace(".gz", "");
                cmbAlgorithms.Items.Add(sFinalFilename);
            }
        }

        public void UpdateSchemaHeaderValues()
        {
            int iNumSchema = 0;
            string sAlgName = "";
            if (mStaging != null)
            {
                //sAlgName = mStaging.getAlgorithm() + " - " + mStaging.getVersion();
                iNumSchema = mStaging.getSchemaIds().Count;
            }
            lblNumSchemas.Text = iNumSchema.ToString();
            lblAlgorithmName.Text = sAlgName;

            lblDLLVersion.Text = typeof(TNMStagingCSharp.Src.Staging.Staging).Assembly.GetName().Version.ToString();

            var entryAssembly = typeof(TNMStagingCSharp.Src.Staging.Staging).Assembly;
            var fileInfo = new FileInfo(entryAssembly.Location);
            var buildDate = fileInfo.LastWriteTime;
            lblDLLDate.Text = buildDate.ToShortDateString();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuHelpHowToUse_Click(object sender, EventArgs e)
        {
            dlgHowTo dlg = new TNMStaging_StagingViewerApp.dlgHowTo();
            dlg.ShowDialog();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            dlgAbout dlg = new TNMStaging_StagingViewerApp.dlgAbout();
            dlg.ShowDialog();
        }

        private void cmbAlgorithms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAlgorithms.SelectedIndex >= 0)
            {
                string sName = (string)cmbAlgorithms.Items[cmbAlgorithms.SelectedIndex];
                LoadSchema(sName);
            }
        }

        public async void LoadSchema(string sFilename)
        {
            const int WAIT_AMT = 300;
            UpdateProgress(0);
            pbLoad.Visible = true;
            tmrLoad.Enabled = true;
            await Task.Delay(WAIT_AMT);

            // Find the file to load. 
            string sFullFilename = Directory.GetCurrentDirectory() + "\\" + sFilename + ".zip";
            if (!File.Exists(sFullFilename))
            {
                sFullFilename = Directory.GetCurrentDirectory() + sFilename + ".gz";
            }
            if (File.Exists(sFullFilename))
            {
                await Task.Delay(WAIT_AMT);

                FileStream SourceStream = File.Open(sFullFilename, FileMode.Open);
                await Task.Delay(WAIT_AMT);
                ExternalStagingFileDataProvider provider = new ExternalStagingFileDataProvider(SourceStream);
                await Task.Delay(WAIT_AMT);
                mStaging = Staging.getInstance(provider);

                UpdateProgress(100);
                await Task.Delay(WAIT_AMT);

                SourceStream.Close();
                UpdateSchemaHeaderValues();
            }
            pbLoad.Visible = false;
            tmrLoad.Enabled = false;
            UpdateProgress(100);
            await Task.Delay(WAIT_AMT);
        }

        private void UpdateProgress(int iNewValue)
        {
            if (iNewValue > 100) iNewValue = 100;
            pbLoad.Value = iNewValue;
            //Debug.WriteLine("Value: " + pbLoad.Value);
            pbLoad.Invalidate();
            pbLoad.Update();
            pbLoad.Refresh();
            Application.DoEvents();

            //System.Threading.Thread.Sleep(100);
        }

        private void tmrLoad_Tick(object sender, EventArgs e)
        {
            UpdateProgress(pbLoad.Value + 10);
        }
    }
}
