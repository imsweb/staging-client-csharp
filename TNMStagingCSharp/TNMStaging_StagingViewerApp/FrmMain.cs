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
using System.Diagnostics;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;

namespace TNMStaging_StagingViewerApp
{
    public partial class FrmMain : Form
    {
        private Staging mStaging = null;
        private List<String> mlstSchemaIds = new List<String>();

        private enum inputVarCols { NAME = 0, TABLE_ID, DEFAULT, USED_FOR_STAGING, NAACCR_NUM, REQUIRED_BY, DESCRIPTION };
        private enum outputVarCols { NAME = 0, DEFAULT, NAACCR_NUM, DESCRIPTION };

        public FrmMain()
        {
            InitializeComponent();


            lblSchemaTitle.Text = "";
            lblSchemaSubtitle.Text = "";
            wbSchemaNotes.DocumentText = "";
            wbSchemaDescr.DocumentText = "";


            LoadAlgorithms();
            UpdateAlgorithmHeaderValues();
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

        public void UpdateAlgorithmHeaderValues()
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
                LoadAlgorithm(sName);
            }
        }

        public async void LoadAlgorithm(string sFilename)
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
                UpdateAlgorithmHeaderValues();
                LoadSchemas();
            }
            pbLoad.Visible = false;
            tmrLoad.Enabled = false;
            UpdateProgress(100);
            await Task.Delay(WAIT_AMT);
        }

        private void UpdateProgress(int iNewValue)
        {
            try
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
            catch (Exception e)
            {

            }
        }

        private void tmrLoad_Tick(object sender, EventArgs e)
        {
            UpdateProgress(pbLoad.Value + 10);
        }

        public void LoadSchemas()
        {
            cmbSchemaSelect.Items.Clear();
            mlstSchemaIds.Clear();

            HashSet<String> hsSchemaIds = mStaging.getSchemaIds();
            StagingSchema thisSchema = null;
            foreach (String sID in hsSchemaIds)
            {
                mlstSchemaIds.Add(sID);
                thisSchema = mStaging.getSchema(sID);
                cmbSchemaSelect.Items.Add(thisSchema.getName());
            }
            if (cmbSchemaSelect.Items.Count > 0)
            {
                cmbSchemaSelect.SelectedIndex = 0;
            }


        }

        private void cmbSchemaSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSchemaSelect.SelectedIndex >= 0)
            {
                String sSchemaID = mlstSchemaIds[cmbSchemaSelect.SelectedIndex];
                LoadSchemaInformation(sSchemaID);
            }
        }

        public void LoadSchemaInformation(String sSchemaID)
        {
            try
            {
                StagingSchema thisSchema = mStaging.getSchema(sSchemaID);
                dataGridStagingInputs.RowCount = 0;
                dataGridOutputs.RowCount = 0;
                if (thisSchema != null)
                {
                    lblSchemaTitle.Text = thisSchema.getTitle();
                    lblSchemaSubtitle.Text = thisSchema.getSubtitle();

                    String result = "";
                    String DescrResult = "";
                    String sNotes = thisSchema.getNotes();
                    if (sNotes != null)
                    {
                        CommonMark.CommonMarkSettings settings = CommonMark.CommonMarkSettings.Default.Clone();
                        settings.RenderSoftLineBreaksAsLineBreaks = true;
                        wbSchemaNotes.Document.Body.Style = "font-family=Microsoft Sans Serif;font-size=11px";
                        result = CommonMark.CommonMarkConverter.Convert(sNotes, settings);
                        result = result.Replace("<p>", "<p style=\"font-family=Microsoft Sans Serif;font-size:11px\">");
                    }
                    wbSchemaNotes.DocumentText = result;

                    String sDescription = thisSchema.getDescription();
                    if (sDescription != null)
                    {
                        CommonMark.CommonMarkSettings DescrSettings = CommonMark.CommonMarkSettings.Default.Clone();
                        DescrSettings.RenderSoftLineBreaksAsLineBreaks = true;
                        DescrResult = CommonMark.CommonMarkConverter.Convert(sDescription, DescrSettings);
                        DescrResult = DescrResult.Replace("<p>", "<p style=\"font-family=Microsoft Sans Serif;font-size:11px\">");
                    }
                    wbSchemaDescr.DocumentText = DescrResult;


                    List<StagingSchemaInput> lstInputs = thisSchema.getInputs();
                    StagingSchemaInput thisStagingSchemaInput = null;
                    bool bSummaryStageField = false;
                    String sKey = "";
                    String sInputName = "";
                    int iRowIndex = -1;
                    for (int i = 0; i < lstInputs.Count; i++)
                    {
                        thisStagingSchemaInput = lstInputs[i];
                        if (thisStagingSchemaInput != null)
                        {
                            // For TNM Version 1.0, summary stage information is in the mappings, but that version
                            // does not produce summary stage.  Do not display those specific fields. 
                            //
                            sKey = thisStagingSchemaInput.getKey();
                            bSummaryStageField = ((sKey == "seer_mets") || (sKey == "seer_nodes") || (sKey == "seer_primary_tumor"));
                            if (!bSummaryStageField)
                            {
                                // Add SSF number to InputName if it is an SSF
                                sInputName = thisStagingSchemaInput.getName();
                                if (sKey.StartsWith("ssf")) sInputName = sKey.ToUpper() + ": " + sInputName;

                                dataGridStagingInputs.RowCount++;
                                iRowIndex = dataGridStagingInputs.RowCount - 1;
                                dataGridStagingInputs[(int)inputVarCols.NAME, iRowIndex].Value = sInputName;
                                dataGridStagingInputs[(int)inputVarCols.TABLE_ID, iRowIndex].Value = thisStagingSchemaInput.getTable();
                                dataGridStagingInputs[(int)inputVarCols.DEFAULT, iRowIndex].Value = thisStagingSchemaInput.getDefault();
                                dataGridStagingInputs[(int)inputVarCols.USED_FOR_STAGING, iRowIndex].Value = (thisStagingSchemaInput.getUsedForStaging() ? "Yes" : "No");
                                dataGridStagingInputs[(int)inputVarCols.NAACCR_NUM, iRowIndex].Value = thisStagingSchemaInput.getNaaccrItem();
                                dataGridStagingInputs[(int)inputVarCols.REQUIRED_BY, iRowIndex].Value = "";
                                dataGridStagingInputs[(int)inputVarCols.DESCRIPTION, iRowIndex].Value = thisStagingSchemaInput.getDescription();
                                //dataGridStagingInputs.Rows[iRowIndex].Height = 30;
                                dataGridStagingInputs.AutoResizeRow(iRowIndex, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);
                            }
                        }
                    }


                    // Display the staging outputs
                    List<StagingSchemaOutput> lstOutputs = thisSchema.getOutputs();
                    StagingSchemaOutput thisStagingSchemaOutput = null;
                    for (int i = 0; i < lstOutputs.Count; i++)
		            {
                        thisStagingSchemaOutput = lstOutputs[i];
                        if (thisStagingSchemaOutput != null)
                        {
                            dataGridOutputs.RowCount++;
                            iRowIndex = dataGridOutputs.RowCount - 1;
                            dataGridOutputs[(int)outputVarCols.NAME, iRowIndex].Value = thisStagingSchemaOutput.getName();
                            dataGridOutputs[(int)outputVarCols.DEFAULT, iRowIndex].Value = thisStagingSchemaOutput.getDefault();
                            if (thisStagingSchemaOutput.getNaaccrItem() == -1)  // No NAACCR number exists for this variable.
                                dataGridOutputs[(int)outputVarCols.NAACCR_NUM, iRowIndex].Value = "";
                            else
                                dataGridOutputs[(int)outputVarCols.NAACCR_NUM, iRowIndex].Value = thisStagingSchemaOutput.getNaaccrItem();
                            dataGridOutputs[(int)outputVarCols.DESCRIPTION, iRowIndex].Value = thisStagingSchemaOutput.getDescription();
                            dataGridOutputs.AutoResizeRow(iRowIndex, DataGridViewAutoSizeRowMode.AllCellsExceptHeader);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("Exception: " + e.Message);
            }
        }


        private void btnSchemaViewSelection_Click(object sender, EventArgs e)
        {
            StagingSchema thisSchema = null;
            StagingTable thisTable = null;
            String sSchemaID = "";
            String sSchemaName = "";
            String sTableId = "";

            if (cmbSchemaSelect.SelectedIndex >= 0)
            {
                sSchemaID = mlstSchemaIds[cmbSchemaSelect.SelectedIndex];
                thisSchema = mStaging.getSchema(sSchemaID);
            }
            if (thisSchema != null)
            {
                sSchemaName = thisSchema.getName();
                sTableId = thisSchema.getSchemaSelectionTable();
                thisTable = mStaging.getTable(sTableId);
            }
            if (thisTable != null)
            {
                dlgTable dlg = new TNMStaging_StagingViewerApp.dlgTable(mStaging, sSchemaName, sTableId);
                dlg.ShowDialog();
            }
        }

        private void btnSchemaViewTable_Click(object sender, EventArgs e)
        {
            String sSchemaName = "";
            String sTableId = "";

            dlgTable dlg = new TNMStaging_StagingViewerApp.dlgTable(mStaging, sSchemaName, sTableId);
            dlg.ShowDialog();
        }
    }
}
