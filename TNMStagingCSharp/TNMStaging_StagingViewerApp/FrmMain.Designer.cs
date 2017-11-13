namespace TNMStaging_StagingViewerApp
{
    partial class FrmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpHowToUse = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSchemas = new System.Windows.Forms.TabPage();
            this.btnSchemaViewTable = new System.Windows.Forms.Button();
            this.btnSchemaViewSelection = new System.Windows.Forms.Button();
            this.dataGridOutputs = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridStagingInputs = new System.Windows.Forms.DataGridView();
            this.pnlSchemaDescr = new System.Windows.Forms.Panel();
            this.wbSchemaDescr = new System.Windows.Forms.WebBrowser();
            this.pnlSchemaNotes = new System.Windows.Forms.Panel();
            this.wbSchemaNotes = new System.Windows.Forms.WebBrowser();
            this.lblSchemaSubtitle = new System.Windows.Forms.Label();
            this.lblSchemaTitle = new System.Windows.Forms.Label();
            this.cmbSchemaSelect = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabStage = new System.Windows.Forms.TabPage();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pbLoad = new System.Windows.Forms.ProgressBar();
            this.lblAlgorithmName = new System.Windows.Forms.Label();
            this.lblNumSchemas = new System.Windows.Forms.Label();
            this.lblDLLVersion = new System.Windows.Forms.Label();
            this.lblDLLDate = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbAlgorithms = new System.Windows.Forms.ComboBox();
            this.tmrLoad = new System.Windows.Forms.Timer(this.components);
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RequiredBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabSchemas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOutputs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridStagingInputs)).BeginInit();
            this.pnlSchemaDescr.SuspendLayout();
            this.pnlSchemaNotes.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpHowToUse,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpHowToUse
            // 
            this.mnuHelpHowToUse.Name = "mnuHelpHowToUse";
            this.mnuHelpHowToUse.Size = new System.Drawing.Size(138, 22);
            this.mnuHelpHowToUse.Text = "&How To Use";
            this.mnuHelpHowToUse.Click += new System.EventHandler(this.mnuHelpHowToUse_Click);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(138, 22);
            this.mnuHelpAbout.Text = "&About";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSchemas);
            this.tabControl.Controls.Add(this.tabStage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 75);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1008, 619);
            this.tabControl.TabIndex = 9;
            // 
            // tabSchemas
            // 
            this.tabSchemas.Controls.Add(this.btnSchemaViewTable);
            this.tabSchemas.Controls.Add(this.btnSchemaViewSelection);
            this.tabSchemas.Controls.Add(this.dataGridOutputs);
            this.tabSchemas.Controls.Add(this.dataGridStagingInputs);
            this.tabSchemas.Controls.Add(this.pnlSchemaDescr);
            this.tabSchemas.Controls.Add(this.pnlSchemaNotes);
            this.tabSchemas.Controls.Add(this.lblSchemaSubtitle);
            this.tabSchemas.Controls.Add(this.lblSchemaTitle);
            this.tabSchemas.Controls.Add(this.cmbSchemaSelect);
            this.tabSchemas.Controls.Add(this.label11);
            this.tabSchemas.Controls.Add(this.label10);
            this.tabSchemas.Controls.Add(this.label9);
            this.tabSchemas.Controls.Add(this.label8);
            this.tabSchemas.Controls.Add(this.label7);
            this.tabSchemas.Controls.Add(this.label6);
            this.tabSchemas.Controls.Add(this.label5);
            this.tabSchemas.Location = new System.Drawing.Point(4, 22);
            this.tabSchemas.Name = "tabSchemas";
            this.tabSchemas.Padding = new System.Windows.Forms.Padding(3);
            this.tabSchemas.Size = new System.Drawing.Size(1000, 593);
            this.tabSchemas.TabIndex = 0;
            this.tabSchemas.Text = "Schemas";
            this.tabSchemas.UseVisualStyleBackColor = true;
            // 
            // btnSchemaViewTable
            // 
            this.btnSchemaViewTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSchemaViewTable.Location = new System.Drawing.Point(11, 248);
            this.btnSchemaViewTable.Name = "btnSchemaViewTable";
            this.btnSchemaViewTable.Size = new System.Drawing.Size(83, 23);
            this.btnSchemaViewTable.TabIndex = 90;
            this.btnSchemaViewTable.Text = "&View Table...";
            this.btnSchemaViewTable.UseVisualStyleBackColor = true;
            this.btnSchemaViewTable.Click += new System.EventHandler(this.btnSchemaViewTable_Click);
            // 
            // btnSchemaViewSelection
            // 
            this.btnSchemaViewSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSchemaViewSelection.Location = new System.Drawing.Point(806, 10);
            this.btnSchemaViewSelection.Name = "btnSchemaViewSelection";
            this.btnSchemaViewSelection.Size = new System.Drawing.Size(180, 23);
            this.btnSchemaViewSelection.TabIndex = 89;
            this.btnSchemaViewSelection.Text = "&View Schema Selection Criteria...";
            this.btnSchemaViewSelection.UseVisualStyleBackColor = true;
            this.btnSchemaViewSelection.Click += new System.EventHandler(this.btnSchemaViewSelection_Click);
            // 
            // dataGridOutputs
            // 
            this.dataGridOutputs.AllowUserToAddRows = false;
            this.dataGridOutputs.AllowUserToDeleteRows = false;
            this.dataGridOutputs.AllowUserToResizeRows = false;
            this.dataGridOutputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridOutputs.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridOutputs.CausesValidation = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridOutputs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridOutputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridOutputs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
            this.dataGridOutputs.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridOutputs.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridOutputs.Location = new System.Drawing.Point(131, 435);
            this.dataGridOutputs.MultiSelect = false;
            this.dataGridOutputs.Name = "dataGridOutputs";
            this.dataGridOutputs.ReadOnly = true;
            this.dataGridOutputs.RowHeadersVisible = false;
            this.dataGridOutputs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridOutputs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridOutputs.ShowCellErrors = false;
            this.dataGridOutputs.ShowRowErrors = false;
            this.dataGridOutputs.Size = new System.Drawing.Size(855, 144);
            this.dataGridOutputs.TabIndex = 84;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 300;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewTextBoxColumn3.HeaderText = "Default";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn3.Width = 70;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn4.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn4.HeaderText = "NAACCR #";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            this.dataGridViewTextBoxColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn4.Width = 70;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn5.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn5.HeaderText = "Description";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            this.dataGridViewTextBoxColumn5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn5.Width = 450;
            // 
            // dataGridStagingInputs
            // 
            this.dataGridStagingInputs.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.dataGridStagingInputs.AllowUserToAddRows = false;
            this.dataGridStagingInputs.AllowUserToDeleteRows = false;
            this.dataGridStagingInputs.AllowUserToResizeRows = false;
            this.dataGridStagingInputs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridStagingInputs.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridStagingInputs.CausesValidation = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.ControlLight;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridStagingInputs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridStagingInputs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridStagingInputs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn12,
            this.dataGridViewTextBoxColumn14,
            this.dataGridViewTextBoxColumn15,
            this.Type,
            this.dataGridViewTextBoxColumn16,
            this.RequiredBy,
            this.dataGridViewTextBoxColumn17});
            this.dataGridStagingInputs.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridStagingInputs.GridColor = System.Drawing.SystemColors.Control;
            this.dataGridStagingInputs.Location = new System.Drawing.Point(131, 232);
            this.dataGridStagingInputs.MultiSelect = false;
            this.dataGridStagingInputs.Name = "dataGridStagingInputs";
            this.dataGridStagingInputs.ReadOnly = true;
            this.dataGridStagingInputs.RowHeadersVisible = false;
            this.dataGridStagingInputs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridStagingInputs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridStagingInputs.ShowCellErrors = false;
            this.dataGridStagingInputs.ShowRowErrors = false;
            this.dataGridStagingInputs.Size = new System.Drawing.Size(854, 190);
            this.dataGridStagingInputs.TabIndex = 80;
            // 
            // pnlSchemaDescr
            // 
            this.pnlSchemaDescr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSchemaDescr.Controls.Add(this.wbSchemaDescr);
            this.pnlSchemaDescr.Location = new System.Drawing.Point(606, 91);
            this.pnlSchemaDescr.Name = "pnlSchemaDescr";
            this.pnlSchemaDescr.Size = new System.Drawing.Size(380, 128);
            this.pnlSchemaDescr.TabIndex = 23;
            // 
            // wbSchemaDescr
            // 
            this.wbSchemaDescr.AllowWebBrowserDrop = false;
            this.wbSchemaDescr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbSchemaDescr.Location = new System.Drawing.Point(0, 0);
            this.wbSchemaDescr.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbSchemaDescr.Name = "wbSchemaDescr";
            this.wbSchemaDescr.Size = new System.Drawing.Size(378, 126);
            this.wbSchemaDescr.TabIndex = 22;
            this.wbSchemaDescr.WebBrowserShortcutsEnabled = false;
            // 
            // pnlSchemaNotes
            // 
            this.pnlSchemaNotes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSchemaNotes.Controls.Add(this.wbSchemaNotes);
            this.pnlSchemaNotes.Location = new System.Drawing.Point(130, 90);
            this.pnlSchemaNotes.Name = "pnlSchemaNotes";
            this.pnlSchemaNotes.Size = new System.Drawing.Size(380, 128);
            this.pnlSchemaNotes.TabIndex = 22;
            // 
            // wbSchemaNotes
            // 
            this.wbSchemaNotes.AllowWebBrowserDrop = false;
            this.wbSchemaNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbSchemaNotes.Location = new System.Drawing.Point(0, 0);
            this.wbSchemaNotes.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbSchemaNotes.Name = "wbSchemaNotes";
            this.wbSchemaNotes.Size = new System.Drawing.Size(378, 126);
            this.wbSchemaNotes.TabIndex = 22;
            this.wbSchemaNotes.WebBrowserShortcutsEnabled = false;
            // 
            // lblSchemaSubtitle
            // 
            this.lblSchemaSubtitle.AutoSize = true;
            this.lblSchemaSubtitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSchemaSubtitle.Location = new System.Drawing.Point(127, 67);
            this.lblSchemaSubtitle.Name = "lblSchemaSubtitle";
            this.lblSchemaSubtitle.Size = new System.Drawing.Size(91, 13);
            this.lblSchemaSubtitle.TabIndex = 20;
            this.lblSchemaSubtitle.Text = "lblSchemaSubtitle";
            // 
            // lblSchemaTitle
            // 
            this.lblSchemaTitle.AutoSize = true;
            this.lblSchemaTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSchemaTitle.Location = new System.Drawing.Point(127, 45);
            this.lblSchemaTitle.Name = "lblSchemaTitle";
            this.lblSchemaTitle.Size = new System.Drawing.Size(76, 13);
            this.lblSchemaTitle.TabIndex = 19;
            this.lblSchemaTitle.Text = "lblSchemaTitle";
            // 
            // cmbSchemaSelect
            // 
            this.cmbSchemaSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSchemaSelect.FormattingEnabled = true;
            this.cmbSchemaSelect.Location = new System.Drawing.Point(130, 12);
            this.cmbSchemaSelect.Name = "cmbSchemaSelect";
            this.cmbSchemaSelect.Size = new System.Drawing.Size(297, 21);
            this.cmbSchemaSelect.TabIndex = 18;
            this.cmbSchemaSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSchemaSelect_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 435);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Outputs:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(8, 232);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Inputs:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(525, 90);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Description:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(8, 90);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Notes:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 67);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(54, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Subtitle:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Title:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Select a Schema:";
            // 
            // tabStage
            // 
            this.tabStage.Location = new System.Drawing.Point(4, 22);
            this.tabStage.Name = "tabStage";
            this.tabStage.Padding = new System.Windows.Forms.Padding(3);
            this.tabStage.Size = new System.Drawing.Size(1000, 593);
            this.tabStage.TabIndex = 1;
            this.tabStage.Text = "Stage a Case";
            this.tabStage.UseVisualStyleBackColor = true;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnClose);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 694);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(1008, 36);
            this.pnlBottom.TabIndex = 10;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(894, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 27);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.pbLoad);
            this.pnlTop.Controls.Add(this.lblAlgorithmName);
            this.pnlTop.Controls.Add(this.lblNumSchemas);
            this.pnlTop.Controls.Add(this.lblDLLVersion);
            this.pnlTop.Controls.Add(this.lblDLLDate);
            this.pnlTop.Controls.Add(this.label4);
            this.pnlTop.Controls.Add(this.label3);
            this.pnlTop.Controls.Add(this.label2);
            this.pnlTop.Controls.Add(this.label1);
            this.pnlTop.Controls.Add(this.cmbAlgorithms);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 24);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1008, 51);
            this.pnlTop.TabIndex = 11;
            // 
            // pbLoad
            // 
            this.pbLoad.Location = new System.Drawing.Point(172, 29);
            this.pbLoad.Name = "pbLoad";
            this.pbLoad.Size = new System.Drawing.Size(243, 19);
            this.pbLoad.TabIndex = 18;
            this.pbLoad.Visible = false;
            // 
            // lblAlgorithmName
            // 
            this.lblAlgorithmName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlgorithmName.Location = new System.Drawing.Point(172, 29);
            this.lblAlgorithmName.Name = "lblAlgorithmName";
            this.lblAlgorithmName.Size = new System.Drawing.Size(243, 13);
            this.lblAlgorithmName.TabIndex = 17;
            // 
            // lblNumSchemas
            // 
            this.lblNumSchemas.AutoSize = true;
            this.lblNumSchemas.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumSchemas.Location = new System.Drawing.Point(625, 6);
            this.lblNumSchemas.Name = "lblNumSchemas";
            this.lblNumSchemas.Size = new System.Drawing.Size(83, 13);
            this.lblNumSchemas.TabIndex = 16;
            this.lblNumSchemas.Text = "lblNumSchemas";
            // 
            // lblDLLVersion
            // 
            this.lblDLLVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDLLVersion.AutoSize = true;
            this.lblDLLVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDLLVersion.Location = new System.Drawing.Point(912, 6);
            this.lblDLLVersion.Name = "lblDLLVersion";
            this.lblDLLVersion.Size = new System.Drawing.Size(72, 13);
            this.lblDLLVersion.TabIndex = 15;
            this.lblDLLVersion.Text = "lblDLLVersion";
            // 
            // lblDLLDate
            // 
            this.lblDLLDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDLLDate.AutoSize = true;
            this.lblDLLDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDLLDate.Location = new System.Drawing.Point(912, 29);
            this.lblDLLDate.Name = "lblDLLDate";
            this.lblDLLDate.Size = new System.Drawing.Size(60, 13);
            this.lblDLLDate.TabIndex = 14;
            this.lblDLLDate.Text = "lblDLLDate";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(769, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Date";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(769, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Staging DLL Version";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(433, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Number of Available Schemas";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Select Staging Algorithm";
            // 
            // cmbAlgorithms
            // 
            this.cmbAlgorithms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlgorithms.FormattingEnabled = true;
            this.cmbAlgorithms.Location = new System.Drawing.Point(172, 3);
            this.cmbAlgorithms.Name = "cmbAlgorithms";
            this.cmbAlgorithms.Size = new System.Drawing.Size(243, 21);
            this.cmbAlgorithms.TabIndex = 9;
            this.cmbAlgorithms.SelectedIndexChanged += new System.EventHandler(this.cmbAlgorithms_SelectedIndexChanged);
            // 
            // tmrLoad
            // 
            this.tmrLoad.Tick += new System.EventHandler(this.tmrLoad_Tick);
            // 
            // dataGridViewTextBoxColumn12
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn12.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn12.HeaderText = "Name";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.ReadOnly = true;
            this.dataGridViewTextBoxColumn12.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn12.Width = 300;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.HeaderText = "Table ID";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.ReadOnly = true;
            this.dataGridViewTextBoxColumn14.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn14.Visible = false;
            this.dataGridViewTextBoxColumn14.Width = 140;
            // 
            // dataGridViewTextBoxColumn15
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn15.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn15.HeaderText = "Default";
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.ReadOnly = true;
            this.dataGridViewTextBoxColumn15.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn15.Width = 70;
            // 
            // Type
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Type.DefaultCellStyle = dataGridViewCellStyle9;
            this.Type.HeaderText = "Used for Staging";
            this.Type.Name = "Type";
            this.Type.ReadOnly = true;
            this.Type.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Type.Width = 98;
            // 
            // dataGridViewTextBoxColumn16
            // 
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn16.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataGridViewTextBoxColumn16.HeaderText = "NAACCR #";
            this.dataGridViewTextBoxColumn16.Name = "dataGridViewTextBoxColumn16";
            this.dataGridViewTextBoxColumn16.ReadOnly = true;
            this.dataGridViewTextBoxColumn16.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn16.Width = 70;
            // 
            // RequiredBy
            // 
            this.RequiredBy.FillWeight = 300F;
            this.RequiredBy.HeaderText = "Required By";
            this.RequiredBy.Name = "RequiredBy";
            this.RequiredBy.ReadOnly = true;
            this.RequiredBy.Visible = false;
            // 
            // dataGridViewTextBoxColumn17
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn17.DefaultCellStyle = dataGridViewCellStyle11;
            this.dataGridViewTextBoxColumn17.HeaderText = "Description";
            this.dataGridViewTextBoxColumn17.Name = "dataGridViewTextBoxColumn17";
            this.dataGridViewTextBoxColumn17.ReadOnly = true;
            this.dataGridViewTextBoxColumn17.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn17.Width = 313;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlTop);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pnlBottom);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1024, 768);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Staging Algorithm Viewer";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabSchemas.ResumeLayout(false);
            this.tabSchemas.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridOutputs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridStagingInputs)).EndInit();
            this.pnlSchemaDescr.ResumeLayout(false);
            this.pnlSchemaNotes.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpHowToUse;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSchemas;
        private System.Windows.Forms.TabPage tabStage;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblNumSchemas;
        private System.Windows.Forms.Label lblDLLVersion;
        private System.Windows.Forms.Label lblDLLDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbAlgorithms;
        private System.Windows.Forms.Label lblAlgorithmName;
        private System.Windows.Forms.ProgressBar pbLoad;
        private System.Windows.Forms.Timer tmrLoad;
        private System.Windows.Forms.Button btnSchemaViewTable;
        private System.Windows.Forms.Button btnSchemaViewSelection;
        private System.Windows.Forms.DataGridView dataGridOutputs;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridView dataGridStagingInputs;
        private System.Windows.Forms.Panel pnlSchemaDescr;
        private System.Windows.Forms.WebBrowser wbSchemaDescr;
        private System.Windows.Forms.Panel pnlSchemaNotes;
        private System.Windows.Forms.WebBrowser wbSchemaNotes;
        private System.Windows.Forms.Label lblSchemaSubtitle;
        private System.Windows.Forms.Label lblSchemaTitle;
        private System.Windows.Forms.ComboBox cmbSchemaSelect;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn16;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequiredBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn17;
    }
}

