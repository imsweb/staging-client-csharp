using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.CS;
using TNMStagingCSharp.Src.Staging.TNM;
using TNMStagingCSharp.Src.Staging.EOD;
using TNMStagingCSharp.Src.Staging.Pediatric;

namespace TNMStaging_StagingViewerApp
{
    public partial class FrmMain : Form
    {
        private ExternalStagingFileDataProvider mProvider = null;
        private Staging mStaging = null;
        private List<String> mlstSchemaIds = new List<String>();

        private String msCurrentStagingSchemaID = "";
        private String msCurrentDescriminatorKey = "";
        private String msCurrentDescriminatorTableId = "";


        private enum inputVarCols { NAME = 0, TABLE_ID, DEFAULT, USED_FOR_STAGING, NAACCR_NUM, REQUIRED_BY, DESCRIPTION };
        private enum outputVarCols { NAME = 0, DEFAULT, NAACCR_NUM, DESCRIPTION };

        private enum stageVarInputCols { INPUT_NAME = 0, TABLE_ID, SELECTED_VALUE, VALUE_DESCRIPTION, FIELD_KEY };
        private enum stageVarOutputCols { OUTPUT_NAME = 0, OUTPUT_VALUE };


        public FrmMain()
        {
            InitializeComponent();

            String sCaption = "Staging Algorithm Viewer - Version ";
            sCaption += Globals.PROGRAM_VERSION;
            Text = sCaption;

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

        private void ClearSchemaTab()
        {
            lblSchemaTitle.Text = "";
            lblSchemaSubtitle.Text = "";
            wbSchemaDescr.DocumentText = "";
            wbSchemaNotes.DocumentText = "";
            dataGridStagingInputs.Rows.Clear();
            dataGridOutputs.Rows.Clear();
            cmbSchemaSelect.Items.Clear();
        }

        private void SetYearDx()
        {
            txtYearDx.Text = DateTime.Now.Year.ToString();
        }

        private void cmbAlgorithms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAlgorithms.SelectedIndex >= 0)
            {
                // Clear any previous algorithm selection. This involves clearing all the tabs
                ClearSchemaTab();
                ClearTNMStageTesterTab();
                SetYearDx();

                // clear the schema count
                lblNumSchemas.Text = "0";

                // unselect listbox selections
                cmbSchemaSelect.SelectedItem = -1;
                cmbSchemaSelect.SelectedValue = "";
                cmbSchemaSelect.Items.Clear();
                cmbSchemaSelect.SelectedText = "";
                //panel1.Refresh();

                Refresh();

                mStaging = null;

                string sName = (string)cmbAlgorithms.Items[cmbAlgorithms.SelectedIndex];
                LoadAlgorithm(sName);
            }
        }

        //
        // Clear all controls on the TNM staging tab
        //
        void ClearTNMStageTesterTab()
        {
            // Clear the schema selection controls
            ClearSchemaSelection();

            // This is not part of the ClearSchemaSelection() function.
            // Remove all items from the site and hist lists!!
            cmbxSite.Items.Clear();
            cmbxHist.Items.Clear();

            // data input
            dataGridVariables.Rows.Clear();

            //staging output
            ClearStagingResults();
        }

        void ClearSchemaSelection()
        {
            // clear the schema controls
            cmbxSite.SelectedIndex = -1;
            cmbxSite.Text = "";
            cmbxHist.SelectedIndex = -1;
            cmbxHist.Text = "";
            lstSchemas.Items.Clear();
            cmbDescriminator.Items.Clear();
            cmbDescriminator.Text = "";
            cmbDescriminator.Visible = false;
            lblDescriminator.Visible = false;

        }

        void ClearStagingResults()
        {
            dataGridResults.Rows.Clear();
            richTxErrorMessage.Text = "";
            txtStageResult.Text = "";
        }

        public async void LoadAlgorithm(string sFilename)
        {
            bool bFailure = false;
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

                FileStream SourceStream = null;

                try
                {
                    SourceStream = File.Open(sFullFilename, FileMode.Open);
                    await Task.Delay(WAIT_AMT);
                    mProvider = new ExternalStagingFileDataProvider(SourceStream);
                    await Task.Delay(WAIT_AMT);
                    mStaging = Staging.getInstance(mProvider);
                }
                catch (Exception)
                {
                    bFailure = true;
                    MessageBox.Show("Unable to load the algorithm " + sFilename + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tabControl.Enabled = false;
                    pbLoad.Visible = false;
                    tmrLoad.Enabled = false;
                }

                if (!bFailure)
                {
                    UpdateProgress(100);
                    await Task.Delay(WAIT_AMT);
                }

                SourceStream.Close();
                if (!bFailure)
                {
                    UpdateAlgorithmHeaderValues();
                    LoadSchemas();
                    SetupStageTesterTab();
                }
            }
            if (!bFailure)
            {
                pbLoad.Visible = false;
                tmrLoad.Enabled = false;
                UpdateProgress(100);
                await Task.Delay(WAIT_AMT);


                tabControl.Enabled = true;

                // unselect listbox selections
                cmbSchemaSelect.SelectedItem = -1;
                cmbSchemaSelect.SelectedText = "";
                Refresh();
            }
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
            catch (Exception)
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
            Schema thisSchema = null;
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
                Schema thisSchema = mStaging.getSchema(sSchemaID);
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


                    List<IInput> lstInputs = thisSchema.getInputs();
                    IInput thisStagingSchemaInput = null;
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
                    List<IOutput> lstOutputs = thisSchema.getOutputs();
                    IOutput thisStagingSchemaOutput = null;
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
            Schema thisSchema = null;
            ITable thisTable = null;
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

            // Get the selected row in the dataGridStagingInputs grid
            // If a row is selected, then get the table name and open up the dialog to view it's contents
            int columnIndex = dataGridStagingInputs.CurrentCell.ColumnIndex;
            int rowIndex = dataGridStagingInputs.CurrentCell.RowIndex;

            if (rowIndex >= 0)
            {
                if (cmbSchemaSelect.SelectedIndex >= 0)
                {
                    string sSchemaID = mlstSchemaIds[cmbSchemaSelect.SelectedIndex];
                    Schema thisSchema = mStaging.getSchema(sSchemaID);
                    if (thisSchema != null) sSchemaName = thisSchema.getName();
                }

                sTableId = dataGridStagingInputs[(int)inputVarCols.TABLE_ID, rowIndex].Value.ToString();
                dlgTable dlg = new TNMStaging_StagingViewerApp.dlgTable(mStaging, sSchemaName, sTableId);
                dlg.ShowDialog();
            }
        }

        private void dataGridStagingInputs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnSchemaViewTable_Click(sender, e);
        }

        //=======================================================================================================================================
        // Staging Tab
        //=======================================================================================================================================
        void SetupStageTesterTab()
        {
            String sSiteCode = "";
            String sSiteLabel = "";

            // Populate site list - with the site codes from the current algorithm
            ITable thisTable = mStaging.getTable(StagingDataProvider.PRIMARY_SITE_TABLE);
            int iRows = 0;
            cmbxSite.Items.Clear();
            if (thisTable != null)
            {
                iRows = thisTable.getRawRows().Count;
                for (int i = 0; i < iRows; i++)
                {
                    sSiteCode = thisTable.getRawRows()[i][0];
                    sSiteLabel = thisTable.getRawRows()[i][1];
                    cmbxSite.Items.Add(sSiteCode + " - " + sSiteLabel);
                }
            }

            // Populate hist list - with the site codes from the current algorithm
            HashSet<String> lstHist = mProvider.getValidHistologies();
            cmbxHist.Items.Clear();
            foreach (String s in lstHist)
            {
                cmbxHist.Items.Add(s);
            }
        }


        private void cmbxSite_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDescriminator.SelectedIndex = -1;
            LocateSchema();
        }

        private void cmbxHist_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbDescriminator.SelectedIndex = -1;
            LocateSchema();
        }

        private void cmbDescriminator_SelectedIndexChanged(object sender, EventArgs e)
        {
            LocateSchema();
        }

        void LocateSchema()
        {
            GetSchemas(Get_Site(), Get_Histology(), Get_Discriminator());
        }

        String Get_Site()
        {
            String site = "";
            if (cmbxSite.SelectedIndex >= 0)
                site = cmbxSite.SelectedItem.ToString();
            else
                site = cmbxSite.Text.ToString();
            site.Trim();
            if (site.Length > 0)
            {
                //Locate the "-" and get the code before it.
                if (site.IndexOf("-") >= 0)
                {
                    site = site.Substring(0, site.IndexOf("-"));
                    site = site.Trim();
                }
            }
            return site;
        }

        String Get_Histology()
        {
            String hist = "";
            if (cmbxHist.SelectedIndex >= 0)
                hist = cmbxHist.SelectedItem.ToString();
            else
                hist = cmbxHist.Text.ToString();
            hist.Trim();
            if (hist.Length > 0)
            {
                //Locate the "-" and get the code before it.
                if (hist.IndexOf("-") >= 0)
                {
                    hist = hist.Substring(0, hist.IndexOf("-") + 1);
                    hist = hist.Trim();
                }
            }
            return hist;
        }

        String Get_Discriminator()
        {
            // Is a descriminator control present?
            String discrim = "";
            if (cmbDescriminator.Visible)
            {
                if (cmbDescriminator.SelectedIndex >= 0)
                    discrim = cmbDescriminator.SelectedItem.ToString();
                else
                    discrim = cmbDescriminator.Text.ToString();
                discrim.Trim();
                if (discrim.Length > 0)
                {
                    //Locate the "-" and get the code before it.
                    if (discrim.IndexOf("::") > 0)
                    {
                        discrim = discrim.Substring(0, discrim.IndexOf("::"));
                        discrim = discrim.Trim();
                    }
                }
            }
            return discrim;
        }

        void EnableStaging()
        {
            dataGridVariables.Enabled = true;
            btnStageCase.Enabled = true;
            btnTablePath.Enabled = true;
            btnClearInputs.Enabled = true;
            btnSetDefaults.Enabled = true;
            btnClearResults.Enabled = true;
            dataGridResults.Enabled = true;
            // Do not need to do anything with txtStageResult
            // Do not need to do anything with richTxErrorMessage
            lblSummaryStageWarning.Visible = false;
        }

        private void DisableStaging()
        {
            dataGridVariables.Enabled = false;
            btnStageCase.Enabled = false;
            btnTablePath.Enabled = false;
            btnClearInputs.Enabled = false;
            btnSetDefaults.Enabled = false;
            btnClearResults.Enabled = false;
            dataGridResults.Enabled = false;
            dataGridResults.Rows.Clear();
            txtStageResult.Text = "";
            richTxErrorMessage.Text = "";

            lblSummaryStageWarning.Visible = true;
        }


        void GetSchemas(String site, String hist, String discrim)
        {
            //String sDescriminatorKey = "";
            //String sDescriminatorTableId = "";
            //String sNewSchema = "";

            //int iNumSchemas = 0;
            //int iNumCols = 0;
            //int iNumRows = 0;
            //String sFirstSchemaId = "";


            bool bNoSchemasFound = true;
            Schema thisStagingSchema = null;

            // Any time this function is called, enable the staging controls.  The controls could have been
            // disabled due to the current histology being only summary staged.
            EnableStaging();

            if ((site == null) || (site == "") || (hist == null) || (hist == ""))
                return;

            SchemaLookup lookup = new SchemaLookup(site, hist);
            List<Schema> lstSchemas = mStaging.lookupSchema(lookup);
            if (lstSchemas != null)
            {
                if (lstSchemas.Count == 1)
                {
                    thisStagingSchema = lstSchemas[0];
                    bNoSchemasFound = false;
                }
                else if (lstSchemas.Count > 1)
                {
                    if (discrim.Length > 0)
                    {
                        // Add the discriminator key to the lookup.
                        SchemaLookup newlookup = new SchemaLookup(site, hist);
                        newlookup.setInput(msCurrentDescriminatorKey, discrim);
                        lstSchemas = mStaging.lookupSchema(newlookup);
                        if (lstSchemas.Count == 1)
                        {
                            thisStagingSchema = lstSchemas[0];
                            bNoSchemasFound = false;
                        }
                        else
                        {
                            // Load and show the Descriminator selection. 
                            lstSchemas = mStaging.lookupSchema(lookup);
                            LoadAndShowDiscriminator(lstSchemas);
                            bNoSchemasFound = false;
                        }
                    }
                    else
                    {
                        // Load and show the Descriminator selection. 
                        LoadAndShowDiscriminator(lstSchemas);
                        bNoSchemasFound = false;
                    }
                }
            }

            if (bNoSchemasFound)
            {
                // no schema found.
                // Clear all controls - just in case they had valid info displayed.
                //
                msCurrentStagingSchemaID = "";
                msCurrentDescriminatorKey = "";
                msCurrentDescriminatorTableId = "";
                this.lstSchemas.Items.Clear();
                dataGridVariables.Rows.Clear();
                ClearStagingResults();
                cmbDescriminator.Text = "";
                cmbDescriminator.Visible = false;
                lblDescriminator.Text = "Descriminator:";
                lblDescriminator.Visible = false;
                cmbDescriminator.Items.Clear();
            }
            else if (thisStagingSchema != null)
            {
                msCurrentStagingSchemaID = thisStagingSchema.getId();

                dataGridVariables.Rows.Clear();
                ClearStagingResults();

                this.lstSchemas.Items.Clear();
                this.lstSchemas.Items.Add(thisStagingSchema.getName());

                if (discrim.Length == 0)
                {
                    msCurrentDescriminatorKey = "";
                    msCurrentDescriminatorTableId = "";

                    cmbDescriminator.Text = "";
                    cmbDescriminator.Visible = false;
                    lblDescriminator.Text = "Descriminator:";
                    lblDescriminator.Visible = false;
                    cmbDescriminator.Items.Clear();
                }

                PopulateVariablesList(msCurrentStagingSchemaID);
            }

            /*
            SchemaLookup lookup = new SchemaLookup(site, hist);
            List<StagingSchema> lstSchemas = mStaging.lookupSchema(lookup);
            StagingSchema thisSchema = null;
            HashSet<String> setDiscrim = null;
            List<StagingSchemaInput> lstInputs = null;
            StagingSchemaInput thisStagingSchemaInput = null;

            iNumSchemas = 0;
            if (lstSchemas != null) iNumSchemas = lstSchemas.Count;

            if (iNumSchemas > 1)
            {
                String sDiscrimKeyID = "";
                for (int i=0; (i < lstSchemas.Count) && (sDiscrimKeyID == ""); i++)
                {
                    thisSchema = lstSchemas[i];
                    if (thisSchema != null)
                    {
                        setDiscrim = thisSchema.getSchemaDiscriminators();
                        if (setDiscrim.Count > 0)
                        {
                            sDiscrimKeyID = setDiscrim.First();
                        }

                        lstInputs = thisSchema.getInputs();
                        if (lstInputs.Count > 0)
                        {
                            thisStagingSchemaInput = lstInputs.First();
                            if (thisStagingSchemaInput != null)
                            {
                                sDescriminatorTableId = thisStagingSchemaInput.getTable();
                            }
                        }
                    }
                }

                if (descrim != "")
                {
                    // Using the site, hist, and descrim value see if the schema id has changed
                    sNewSchema = gcnew String(TNMStage_get_schema_id(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSite, sHist, sDescriminator));
                    char* sSchemaId = (char*)Marshal::StringToHGlobalAnsi(sNewSchema).ToPointer();
                    // if sNewSchema is empty, then the site, hist, descrim did not go a schema or went to multiple schemas (SSF-25, 981 for stomach/esoph ge junc
                    if (sNewSchema == "")
                    {
                        // need to treat this as a multiple schema - 
                        msCurrentDescriminatorTableId = "";
                    }
                }

                // If there is a descriminator list being displayed, then see if it the same table id.
                // We need to do this because the descriminator key could be SSF-25 and thus apply to multiple tables
                if (msCurrentDescriminatorTableId == sDescriminatorTableId)
                {
                    // if the descriminator is blank, then there is nothing to do
                    if (descrim != "")
                    {
                        // Using the site, hist, and descrim value see if the schema id has changed
                        char* sDescriminator = (char*)Marshal::StringToHGlobalAnsi(descrim).ToPointer();
                        sNewSchema = gcnew String(TNMStage_get_schema_id(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSite, sHist, sDescriminator));
                        char* sSchemaId = (char*)Marshal::StringToHGlobalAnsi(sNewSchema).ToPointer();

                        // If the schema id has not changed, then they changed site, hist, or the descriminator but the schema did NOT change - nothing to do for this occurance
                        // But if it is a new schema!!
                        if (sNewSchema != msCurrentSchemaID)
                        {
                            msCurrentSchemaID = sNewSchema;

                            lstbxSchemas.Items.Clear();
                            lstbxSchemas.Items.Add(gcnew String(TNMStage_get_schema_name(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSchemaId)));

                            // clear other controls
                            dataGridVariables.Rows.Clear();
                            ClearStagingResults();
                            cmbDescriminator.Text = "";
                            cmbDescriminator.Visible = false;
                            lblDescriminator.Text = "Descriminator:";
                            lblDescriminator.Visible = false;
                            cmbDescriminator.Items.Clear();
                        }
                        else // clear inputs and re-populate them
                        {
                            // clear other controls
                            dataGridVariables.Rows.Clear();
                            ClearStagingResults();

                            PopulateVariablesList(sSchemaId);
                        }
                    }
                }
                else // it's a different descriminator table, so re-populate the descriminator list and clear the input and output tables.
                {
                    msCurrentSchemaID = "";
                    msCurrentDescriminatorKey = sDescriminatorKey;
                    msCurrentDescriminatorTableId = sDescriminatorTableId;

                    // clear other controls
                    dataGridVariables.Rows.Clear();
                    ClearStagingResults();
                    cmbDescriminator.Text = "";
                    cmbDescriminator.Visible = true;
                    cmbDescriminator.Items.Clear();

                    // Populate the list box with the schema names
                    std::string sSchemaID;
                    lstbxSchemas.Items.Clear();
                    for (int i = 0; i < iNumSchemas; i++)
                    {
                        // Get the next applicable schema and put it in the list box
                        sSchemaID = TNMStage_get_app_schema_ids(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSite, sHist, i);
                        // Now using the schema ID, get the displayable name
                        lstbxSchemas.Items.Add(gcnew String(TNMStage_get_schema_name(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSchemaID.c_str())));

                        if (i == 0)
                            sFirstSchemaId = sSchemaID;
                    }

                    //*******************************************************************************
                    // Get the descriminator variable name and populate the descriminator value list
                    //*******************************************************************************
                    char* sTableId = (char*)Marshal::StringToHGlobalAnsi(sDescriminatorTableId).ToPointer();

                    // Get the label from table pointed to by tableid
                    std::string sFieldName = TNMStage_get_table_title(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sTableId);
                    String ^ sLabel = gcnew String(sFieldName.c_str());
                    lblDescriminator.Text = sLabel;
                    lblDescriminator.Visible = true;

                    // Populate the variable value selection list			
                    iNumCols = TNMStage_get_table_num_columns(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sTableId);
                    iNumRows = TNMStage_get_table_num_rows(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sTableId);

                    for (int n = 0; n < iNumRows; n++)
                    {
                        String ^ sValue = gcnew String(TNMStage_get_table_cell_value(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sTableId, n, 0));
                        String ^ sLabel = gcnew String(TNMStage_get_table_cell_value(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sTableId, n, 1));
                        cmbDescriminator.Items.Add(sValue + " - " + sLabel);
                    }
                }
            }
            else if (iNumSchemas == 1)
            {
                // Get the schema ID
                sNewSchema = gcnew String(TNMStage_get_schema_id(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSite, sHist, ""));

                // Because a histology change within a schema can change the input variable list (those required to stage), 
                // we need to either always refresh the input variable list (thus whiping out anything the user entered), 
                // OR we need to check the new input list and see if it is identical to the current list.  This only happens when the 
                // site, hist, or discriminator change takes us to the SAME schema that is displayed.
                // FOR NOW, ALWAYS REFRESH THE LIST.
                //
                // See if the new site-hist combo goes to the currently displayed schema.  If so, then no changes necessary.
                // If they go to a different schema, the change the input, output, etc. - clear what is currently there.
                String sSchemaID = "";

                msCurrentSchemaID = sNewSchema;
                msCurrentDescriminatorKey = "";
                msCurrentDescriminatorTableId = "";

                dataGridVariables.Rows.Clear();
                ClearStagingResults();

                lstbxSchemas.Items.Clear();
                lstbxSchemas.Items.Add(gcnew String(TNMStage_get_schema_name(gpsLoadedAlgorithmName.c_str(), gpsLoadedAlgorithmVersion.c_str(), sSchemaID)));

                cmbDescriminator.Text = "";
                cmbDescriminator.Visible = false;
                lblDescriminator.Text = "Descriminator:";
                lblDescriminator.Visible = false;
                cmbDescriminator.Items.Clear();

                PopulateVariablesList(sSchemaID);
            }
            else // Just in case there are no schemas from the selection.
            {
                // no schema found.
                // Clear all controls - just in case they had valid info displayed.
                //
                msCurrentSchemaID = "";
                msCurrentDescriminatorKey = "";
                msCurrentDescriminatorTableId = "";
                lstbxSchemas.Items.Clear();
                dataGridVariables.Rows.Clear();
                ClearStagingResults();
                cmbDescriminator.Text = "";
                cmbDescriminator.Visible = false;
                lblDescriminator.Text = "Descriminator:";
                lblDescriminator.Visible = false;
                cmbDescriminator.Items.Clear();
            }
            */
        }


        private void LoadAndShowDiscriminator(List<Schema> lstSchemas)
        {
            String sDiscrimKeyID = "";
            String sDescrimTableId = "";
            Schema thisSchema = null;
            HashSet<String> setDiscrim = null;
            List<IInput> lstInputs = null;
            IInput thisStagingSchemaInput = null;

            for (int i = 0; (i < lstSchemas.Count) && (sDiscrimKeyID == ""); i++)
            {
                thisSchema = lstSchemas[i];
                if (thisSchema != null)
                {
                    setDiscrim = thisSchema.getSchemaDiscriminators();
                    if (setDiscrim.Count > 0)
                    {
                        sDiscrimKeyID = setDiscrim.First();
                        lstInputs = thisSchema.getInputs();
                        for (int inp = 0; inp < lstInputs.Count; inp++)
                        {
                            thisStagingSchemaInput = lstInputs[inp];
                            if (thisStagingSchemaInput.getKey() == sDiscrimKeyID)
                            {
                                sDescrimTableId = thisStagingSchemaInput.getTable();
                            }
                        }
                    }
                }
            }

            msCurrentStagingSchemaID = "";
            msCurrentDescriminatorKey = "";
            msCurrentDescriminatorTableId = "";

            if ((sDiscrimKeyID != "") && (sDescrimTableId != ""))
            {
                msCurrentDescriminatorKey = sDiscrimKeyID;
                msCurrentDescriminatorTableId = sDescrimTableId;

                // clear other controls
                dataGridVariables.Rows.Clear();
                ClearStagingResults();
                cmbDescriminator.Text = "";
                cmbDescriminator.Visible = true;
                cmbDescriminator.Items.Clear();

                // Populate the list box with the schema names
                this.lstSchemas.Items.Clear();
                for (int i = 0; i < lstSchemas.Count; i++)
                {
                    thisSchema = lstSchemas[i];
                    this.lstSchemas.Items.Add(thisSchema.getName());
                }

                //*******************************************************************************
                // Get the descriminator variable name and populate the descriminator value list
                //*******************************************************************************

                // Get the label from table pointed to by tableid
                ITable thisTable = mStaging.getTable(sDescrimTableId);
                if (thisTable != null)
                {
                    lblDescriminator.Text = thisTable.getTitle();
                    lblDescriminator.Visible = true;
                    cmbDescriminator.Left = 109;
                    cmbDescriminator.Width = 802;
                    lblDescriminator.Refresh();
                    if (lblDescriminator.Left + lblDescriminator.Width > cmbDescriminator.Left)
                    {
                        cmbDescriminator.Left = lblDescriminator.Left + lblDescriminator.Width + 20;
                        cmbDescriminator.Width = 802 - (cmbDescriminator.Left - 109);
                    }
                    // Populate the variable value selection list			
                    int iNumRows = thisTable.getRawRows().Count;
                    String sValue = "";
                    String sLabel = "";
                    for (int n = 0; n < iNumRows; n++)
                    {
                        sValue = string.Empty;
                        sLabel = string.Empty;
                        if (thisTable.getRawRows()[n].Count > 0)
                        {
                            sValue = thisTable.getRawRows()[n][0];
                        }
                        if (thisTable.getRawRows()[n].Count > 1)
                        {
                            sLabel = thisTable.getRawRows()[n][1];
                        }
                        cmbDescriminator.Items.Add(sValue + " :: " + sLabel);
                    }
                }
            }
        }


        private void btnClearAll_Click(object sender, EventArgs e)
        {
            // Enable the staging controls - just in case they are currently disabled 
            // due to a summary stage only histology
            EnableStaging();

            // Clear all displays and get ready for new information to be shown!!
            msCurrentStagingSchemaID = "";
            msCurrentDescriminatorKey = "";
            msCurrentDescriminatorTableId = "";

            // Clear the schema selection controls - but don't clear the site or hist lists!!!!
            ClearSchemaSelection();

            // clear the remaining controls
            dataGridVariables.Rows.Clear();

            // clear the staging results
            ClearStagingResults();
        }

        private void txbxYearDx_Leave(object sender, EventArgs e)
        {
            ClearStagingResults();
        }

        private void btnStageCase_Click(object sender, EventArgs e)
        {
            if (lstSchemas.Items.Count == 1)
            {
                StageCase();
            }
        }

        private void btnClearInputs_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridVariables.Rows.Count; i++)
            {
                dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, i].Value = "";
                dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, i].Value = "";

                // must reset the cell color.  if the cell is currently red (bad value), then
                // we must set it back to its original color since the emtpy cell is not invalid.
                DataGridViewCell cell = dataGridVariables.Rows[i].Cells[(int)stageVarInputCols.SELECTED_VALUE];
                DataGridViewCell cell_other = dataGridVariables.Rows[i].Cells[(int)stageVarInputCols.INPUT_NAME];
                cell.Style.BackColor = cell_other.Style.BackColor;
            }
        }

        private void btnSetDefaults_Click(object sender, EventArgs e)
        {
            String sKey = "";
            String sDefault = "";
            String sValueDescription = "";
            String sTableID = "";
            Schema thisSchema = null;
            IInput input = null;
            for (int i = 0; i < dataGridVariables.Rows.Count; i++)
            {
                // get the variable key and see if it has a default value 
                thisSchema = mStaging.getSchema(msCurrentStagingSchemaID);
                if (thisSchema != null)
                {
                    sKey = dataGridVariables[(int)stageVarInputCols.FIELD_KEY, i].Value.ToString();
                    thisSchema.getInputMap().TryGetValue(sKey, out input);
                    if (input != null)
                    {
                        sDefault = input.getDefault();
                        if (sDefault != null)
                        {
                            dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, i].Value = sDefault;
                            sValueDescription = "";
                            sTableID = input.getTable();
                            if (sDefault.Length > 0) sValueDescription = GetLabelOfSelectedValue(sTableID, sDefault);
                            dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, i].Value = sValueDescription;
                        }
                    }
                }
            }
        }

        private void btnClearResults_Click(object sender, EventArgs e)
        {
            ClearStagingResults();
        }

        // For TNM Version 1.0, summary stage information is in the mappings, but that version
        // does not produce summary stage.  So, we need to hard-code in a check to make sure we
        // catch the histologies (for a given schema) that only summary stage.  
        bool OnlySummaryStageInputs(HashSet<String> setStagingInputKeys)
        {
            bool bOnlySummaryStageInputs = false;
            HashSet<String> setNotAllowedStagingInputKeys = new HashSet<string>();

            // Remove the keys that will accompany a Summary Stage Only Histology
            // The setStagingInputKeys is a copy of the original, so we can manipulate it.
            //
            setStagingInputKeys.Remove("site");
            setStagingInputKeys.Remove("hist");
            setStagingInputKeys.Remove("sex");
            setStagingInputKeys.Remove("ssf25");

            setNotAllowedStagingInputKeys.Add("seer_mets");
            setNotAllowedStagingInputKeys.Add("seer_nodes");
            setNotAllowedStagingInputKeys.Add("seer_primary_tumor");
            // If the input list consists of SEER Mets, SEER Nodes, and SEER Primary Tumor then it only
            // summary stages.
            bOnlySummaryStageInputs = (setStagingInputKeys.SetEquals(setNotAllowedStagingInputKeys));
            if (!bOnlySummaryStageInputs)
            {
                setNotAllowedStagingInputKeys.Add("sex");
                bOnlySummaryStageInputs = (setStagingInputKeys == setNotAllowedStagingInputKeys);
            }
            return bOnlySummaryStageInputs;
        }


        private void PopulateVariablesList(string sStagingSchemaID)
        {
            String site = "";
            String histology = "";
            String discriminator = "";
            //String sVarTableId = "";

            HashSet<String> setStagingInputKeys = new HashSet<String>();
            int iRowIndex;
            //int iNumInputs;

            System.Drawing.Color cNonStagingRowColor = System.Drawing.Color.White;
            System.Drawing.Color cStagingRowColor = System.Drawing.Color.PaleTurquoise;

            site = Get_Site();
            histology = Get_Histology();
            discriminator = Get_Discriminator();

            // First clear the list of the old inputs
            dataGridVariables.Rows.Clear();

            //*********************************************************************
            // First get all the keys of the staging input fields and place them
            // in a set - for quick searches.
            //*********************************************************************
            IInput thisInput = null;
            List<IInput> lstStagingInputs = null;
            Schema thisSchema = mStaging.getSchema(sStagingSchemaID);
            if (thisSchema != null)
            {
                lstStagingInputs = thisSchema.getInputs();

                for (int i = 0; i < lstStagingInputs.Count; i++)
                {
                    if (lstStagingInputs[i].getUsedForStaging())
                    {
                        setStagingInputKeys.Add(lstStagingInputs[i].getKey());
                    }
                }
            }

            //*********************************************************************
            // Now loop through all the input fields for the schema and if you
            // find the input key in the vsStagingInputKeys vector, display it.
            // 
            // The keys placed in the vsStagingInputKeys vector are from a std.set
            // and thus are sorted.  We need to display them in the order that they
            // appear in the schema. Thus we loop through the schema input fields 
            // and if they are in the vsStagingInputKeys vector, we display them.
            //*********************************************************************

            // This is a special check for TNM version 1.2
            // See if the dataGridVariables only contains the following variables:
            //  1.  SEER Primary Tumor
            //  2.  SEER Regional Nodes
            //  3.  SEER Mets		
            if (OnlySummaryStageInputs(setStagingInputKeys))
            {
                dataGridVariables.Rows.Clear();

                // Now disable the ability to stage the case
                DisableStaging();
            }
            else // go ahead and display the fields - skip those SS fields
            {
                String sKey = "";
                String sInputName = "";
                String sTableID = "";
                String sDefault = "";
                String sDescription = "";
                String sVariableName = "";
                String sValueDescription = "";
                for (int i = 0; i < lstStagingInputs.Count; i++)
                {
                    thisInput = lstStagingInputs[i];
                    sKey = thisInput.getKey();

                    if (setStagingInputKeys.Contains(sKey))
                    {
                        // Get the algorithm type and version
                        sInputName = thisInput.getName();
                        sTableID = thisInput.getTable();
                        sDefault = thisInput.getDefault();
                        sDescription = thisInput.getDescription();

                        if (sInputName == null) sInputName = "";
                        if (sTableID == null) sTableID = "";
                        if (sDefault == null) sDefault = "";
                        if (sDescription == null) sDescription = "";

                        // Using the table name, get the name of the table - this will be the base variable name. i.e. "SSF1", "Nodes Pos", etc..
                        sVariableName = mStaging.getTable(sTableID).getName();

                        // If a default value was supplied, then get it's description for the associated table.
                        if (sDefault.Length > 0) sValueDescription = GetLabelOfSelectedValue(sTableID, sDefault);

                        // No need to display Site, Histology, or Year of Diagnosis in the list - they are handled special at the top of the form.
                        // Always use the variable key since it is unique and consistent over algorithm versions.
                        if ((sKey != "site") && (sKey != "hist") && (sKey != "year_dx") && (sKey != "ssf25") && (sKey != "sex") &&
                            (sKey != "seer_mets") && (sKey != "seer_nodes") && (sKey != "seer_primary_tumor"))
                        {
                            if (sKey.StartsWith("ssf")) sInputName = sKey.ToUpper() + ": " + sInputName;

                            dataGridVariables.RowCount++;
                            iRowIndex = dataGridVariables.RowCount - 1;
                            dataGridVariables[(int)stageVarInputCols.INPUT_NAME, iRowIndex].Value = sInputName;
                            dataGridVariables[(int)stageVarInputCols.TABLE_ID, iRowIndex].Value = sTableID;
                            dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, iRowIndex].Value = sDefault;
                            dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, iRowIndex].Value = sValueDescription;
                            dataGridVariables[(int)stageVarInputCols.FIELD_KEY, iRowIndex].Value = sKey;
                        }
                    }
                }
            }
        }

        private String GetLabelOfSelectedValue(String sTableId, String sDefault)
        {
            int iRowIndex, iInputColumn = -1, iDescriptionColumn = -1;
            String sValueDescription = null;
            List<int> lstDescriptionColumns = new List<int>();
            String sInputCellValue;

            // Locate the table
            ITable thisTable = mStaging.getTable(sTableId);
            List<IColumnDefinition> lstColDefs = null;
            // Locate the TNM_COLUMN_INPUT and the first TNM_COLUMN_DESCRIPTION column
            if (thisTable != null)
            {
                lstColDefs = thisTable.getColumnDefinitions();
                ColumnType cType;
                for (int i = 0; i < lstColDefs.Count; i++)
                {
                    cType = lstColDefs[i].getType();
                    if ((cType == ColumnType.INPUT) && (iInputColumn == -1))
                        iInputColumn = i;
                    else if (cType == ColumnType.DESCRIPTION)
                        lstDescriptionColumns.Add(i); // keep track of descrption columns
                }
            }

            // See if there are two or more description fields.  If so, see if we can find one
            // that has a label of "Description"
            if (lstDescriptionColumns.Count > 1)
            {
                String sColumnLabel = "";
                for (int i = 0; i < lstDescriptionColumns.Count; i++)
                {
                    sColumnLabel = ((IColumnDefinition)lstColDefs[lstDescriptionColumns[i]]).getName();
                    if (sColumnLabel == "Description")
                        iDescriptionColumn = lstDescriptionColumns[i];
                }
                if (iDescriptionColumn == -1)
                    iDescriptionColumn = lstDescriptionColumns[0];
            }
            else if (lstDescriptionColumns.Count == 0)
                iDescriptionColumn = 0;  // for tables that do not have a description.  point to the code
            else
                iDescriptionColumn = lstDescriptionColumns[0];


            // Only continue if you have both an input column and a description column
            if ((iInputColumn >= 0) && (iDescriptionColumn >= 0))
            {
                int iNumRows = thisTable.getRawRows().Count;
                List<String> thisRow = null;
                // Now loop through the table rows and add each cell to the table
                for (iRowIndex = 0; (iRowIndex < iNumRows) && (sValueDescription == null); iRowIndex++)
                {
                    thisRow = thisTable.getRawRows()[iRowIndex];
                    sInputCellValue = thisRow[iInputColumn].Trim();
                    if (sInputCellValue == sDefault)
                        sValueDescription = thisRow[iDescriptionColumn];
                    else if (sInputCellValue.Contains("-") && (sInputCellValue.IndexOf("-") != (sInputCellValue.Length - 1))) // cell contains a range
                    {
                        String[] ranges = sInputCellValue.Split(",".ToCharArray());
                        for (int range = 0; range < ranges.Length; range++)
                        {
                            String[] indivRange = ranges[range].Split("-".ToCharArray());
                            if (indivRange.Length == 1)
                            {
                                //if just one entry, then do a string compare
                                if (indivRange[0] == sDefault)
                                    sValueDescription = thisRow[iDescriptionColumn];
                            }
                            else if (indivRange.Length == 2)
                            {
                                int iDefault = 0;
                                int iLow = 0;
                                int iHigh = 0;
                                int.TryParse(sDefault, out iDefault);
                                int.TryParse(indivRange[0], out iLow);
                                int.TryParse(indivRange[1], out iHigh);
                                if ((iLow <= iDefault) && (iDefault <= iHigh))
                                    sValueDescription = thisRow[iDescriptionColumn];
                            }
                        }
                    }
                }
            }
            return sValueDescription;
        }

        private void dataGridVariables_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ValidateVariableValue();
        }

        private void dataGridVariables_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ProcessSelectedCell();
            Refresh();
        }

        private void dataGridVariables_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ProcessSelectedCell();
            Refresh();
        }

        private void ValidateVariableValue()
        {
            bool bIsValueValid;
            String sCellValue = "";
            String sFieldKey = "";
            String sTableID = "";

            System.Drawing.Color cNonStagingRowColor = System.Drawing.Color.White;
            System.Drawing.Color cStagingRowColor = System.Drawing.Color.PaleTurquoise;
            System.Drawing.Color cInvalidValueColor = System.Drawing.Color.Red;

            // Once this function is called, then you know a user has adjusted the input values. 
            // So, we need to clear the staging results or they can be misleading.
            ClearStagingResults();

            int columnIndex = (int)stageVarInputCols.SELECTED_VALUE;
            int rowIndex = dataGridVariables.CurrentCell.RowIndex;

            // edit cell content - column 3 contains the variable value 
            if (rowIndex >= 0)
            {
                // get schema id from the special edit box
                if (msCurrentStagingSchemaID != "")
                {
                    sCellValue = "";
                    if (dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, rowIndex].Value != null)
                    {
                        if (!String.IsNullOrEmpty(dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, rowIndex].Value.ToString()))
					        sCellValue = dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, rowIndex].Value.ToString();
                    }

                    sTableID = dataGridVariables[(int)stageVarInputCols.TABLE_ID, rowIndex].Value.ToString();
                    sFieldKey = dataGridVariables[(int)stageVarInputCols.FIELD_KEY, rowIndex].Value.ToString();
                    bIsValueValid = mStaging.isCodeValid(msCurrentStagingSchemaID, sFieldKey, sCellValue);

                    String sValueDescription = "";
                    // Color the cell red if the value is invalid
                    if (!bIsValueValid)
                    {
                        DataGridViewCell cell = dataGridVariables.Rows[rowIndex].Cells[columnIndex];
                        cell.Style.BackColor = System.Drawing.Color.Red;
                        //dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, rowIndex].Value = sValueDescription;
                    }
                    else // change it back to the original color - they might have corrected a value
                    {
                        DataGridViewCell cell = dataGridVariables.Rows[rowIndex].Cells[columnIndex];
                        int iOtherCol = ((columnIndex > 0) ? (columnIndex - 1) : (columnIndex + 1));
                        DataGridViewCell cell_other = dataGridVariables.Rows[rowIndex].Cells[iOtherCol];
                        cell.Style.BackColor = cell_other.Style.BackColor;

                        // If a default value was supplied, then get it's description for the associated table.					
                        if (sCellValue.Length <= 0)
                        {
                            //dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, rowIndex].Value = sValueDescription;
                            sValueDescription = GetLabelOfSelectedValue(sTableID, "");
                            dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, rowIndex].Value = sValueDescription;
                        }
                        else
                        {
                            sValueDescription = GetLabelOfSelectedValue(sTableID, sCellValue);
                            dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, rowIndex].Value = sValueDescription;
                        }
                    }
                }
            }
        }


        private void ProcessSelectedCell()
        {
            String  sSelectedValue;
            String  sSelectedValueLabel;

            // Get the selected row in the dataGridStagingInputs grid
            // If a row is selected, then get the table name and open up the dialog to view it's contents

            int columnIndex = dataGridVariables.CurrentCell.ColumnIndex;
            int rowIndex = dataGridVariables.CurrentCell.RowIndex;

            if (rowIndex >= 0)
            {
                // If the cell is being edited, don't open the dialog on a double-click
                String sInputName = dataGridVariables[(int)stageVarInputCols.INPUT_NAME, rowIndex].Value.ToString();
                String sTableId = dataGridVariables[(int)stageVarInputCols.TABLE_ID, rowIndex].Value.ToString();

                dlgValuePicker dlg = new TNMStaging_StagingViewerApp.dlgValuePicker(mStaging, sTableId, sInputName);
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    sSelectedValue = dlg.GetSelectedValue();
                    sSelectedValueLabel = dlg.GetSelectedValueLabel();

                    dataGridVariables.EndEdit();

                    // Place the selected value in the table cell.
                    dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, rowIndex].Value = sSelectedValue;
                    dataGridVariables[(int)stageVarInputCols.VALUE_DESCRIPTION, rowIndex].Value = sSelectedValueLabel;
                    dataGridVariables.Refresh();
                    dlg.Close();

                    // Need to edit the cell.  If the cell was "invalid", then it 
                    // needs to change to be "valid" color.
                    ValidateVariableValue();
                }
            }
        }

        private void btnTablePath_Click(object sender, EventArgs e)
        {
            dlgStagingPath dlg = new TNMStaging_StagingViewerApp.dlgStagingPath(richTxStagingPath.Text);
            dlg.ShowDialog();
        }

        private void StageCase()
        {
            // First, clear the list of results
            dataGridResults.Rows.Clear();
            richTxErrorMessage.Text = "";
            richTxStagingPath.Text = "";

            // Set the inputs.
            StagingData data = null;
            bool bCSStage = (mProvider.getAlgorithm().IndexOf("cs") >= 0);
            bool bTNMStage = (mProvider.getAlgorithm().IndexOf("tnm") >= 0);
            bool bEODStage = (mProvider.getAlgorithm().IndexOf("eod") >= 0);
            bool bPediatricStage = (mProvider.getAlgorithm().IndexOf("pediatric") >= 0);

            if (bCSStage)           data = new CsStagingData();
            else if (bTNMStage)     data = new TnmStagingData();
            else if (bEODStage)     data = new EodStagingData();
            else if (bPediatricStage) data = new PediatricStagingData();


            String sSite = cmbxSite.Text;
            if (sSite.IndexOf("-") > 0)
            {
                sSite = sSite.Substring(0, sSite.IndexOf("-") - 1);
                sSite = sSite.Trim();
            }
            data.setInput("site", sSite);
            data.setInput("hist", cmbxHist.Text);
            data.setInput("year_dx", txtYearDx.Text);

            if (cmbDescriminator.Visible && (msCurrentDescriminatorKey != ""))
            {
                String sDiscrimVal = cmbDescriminator.Text;
                if (sDiscrimVal.IndexOf("::") > 0)
                {
                    sDiscrimVal = sDiscrimVal.Substring(0, sDiscrimVal.IndexOf("::") - 1);
                    sDiscrimVal = sDiscrimVal.Trim();
                }
                data.setInput(msCurrentDescriminatorKey, sDiscrimVal);
            }

            String sFieldKey = "";
            String sFieldValue = "";
            for (int i = 0; i < dataGridVariables.Rows.Count; i++)
            {
                // get the variable 'key' - last column in the grid
                sFieldKey = dataGridVariables[(int)stageVarInputCols.FIELD_KEY, i].Value.ToString();
                sFieldValue = "";
                if (dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, i].Value != null)
                    sFieldValue = dataGridVariables[(int)stageVarInputCols.SELECTED_VALUE, i].Value.ToString();
                data.setInput(sFieldKey, sFieldValue);
            }

            // Perform the staging
            mStaging.stage(data);

            
            if (data.getResult() == StagingData.Result.STAGED) txtStageResult.Text = "Staged";
            else if (data.getResult() == StagingData.Result.FAILED_MISSING_SITE_OR_HISTOLOGY) txtStageResult.Text = "Missing Site or Histology";
            else if (data.getResult() == StagingData.Result.FAILED_NO_MATCHING_SCHEMA) txtStageResult.Text = "No Matching Schema";
            else if (data.getResult() == StagingData.Result.FAILED_MULITPLE_MATCHING_SCHEMAS) txtStageResult.Text = "Multiple Matching Schemas";
            else if (data.getResult() == StagingData.Result.FAILED_INVALID_YEAR_DX) txtStageResult.Text = "Invalid Year Dx";
            else if (data.getResult() == StagingData.Result.FAILED_INVALID_INPUT) txtStageResult.Text = "Invalid Input";

            List<Error> lstErrors = data.getErrors();
            for (int i=0; i < lstErrors.Count; i++)
            {
                richTxErrorMessage.Text += lstErrors[i].getMessage() + "\r\n";
            }
            List<string> lstPath = data.getPath();
            for (int i = 0; i < lstPath.Count; i++)
            {
                richTxStagingPath.Text += lstPath[i] + "\r\n";
            }

            // Display the Outputs
            Dictionary<string, string> dictOut = data.getOutput();
            int iRow = 0;
            String sFieldName = "";
            foreach (KeyValuePair<string, string> entry in dictOut)
            {
                if ((entry.Key != "schema_number") && (entry.Key != "csver_derived"))
                {
                    // Put the output variables in the output list
                    iRow = dataGridResults.RowCount;
                    dataGridResults.RowCount++;
                    sFieldName = GetSchemaOutputName(data.getSchemaId(), entry.Key);
                   
                    dataGridResults[(int)stageVarOutputCols.OUTPUT_NAME, iRow].Value = sFieldName;
                    dataGridResults[(int)stageVarOutputCols.OUTPUT_VALUE, iRow].Value = entry.Value;
                }
            }
        }

        private String GetSchemaOutputName(String sSchemaId, String sKey)
        {
            String sRetval = sKey;
            Schema thisSchema = mStaging.getSchema(sSchemaId);
            List<IOutput> lstOutputs = null;
            if (thisSchema != null)
            {
                lstOutputs = thisSchema.getOutputs();
            }
            if (lstOutputs != null)
            {
                for (int i=0; i < lstOutputs.Count; i++)
                {
                    if (lstOutputs[i].getKey() == sKey)
                    {
                        sRetval = lstOutputs[i].getName();
                    }
                }
            }

            return sRetval;
        }

    }
}
