namespace TNMStaging_StagingViewerApp
{
    partial class dlgValuePicker
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.webBrPickerNotes = new System.Windows.Forms.WebBrowser();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridViewPicker = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPicker)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.webBrPickerNotes);
            this.panel1.Location = new System.Drawing.Point(10, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(673, 144);
            this.panel1.TabIndex = 1;
            // 
            // webBrPickerNotes
            // 
            this.webBrPickerNotes.AllowWebBrowserDrop = false;
            this.webBrPickerNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrPickerNotes.IsWebBrowserContextMenuEnabled = false;
            this.webBrPickerNotes.Location = new System.Drawing.Point(0, 0);
            this.webBrPickerNotes.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrPickerNotes.Name = "webBrPickerNotes";
            this.webBrPickerNotes.Size = new System.Drawing.Size(671, 142);
            this.webBrPickerNotes.TabIndex = 2;
            this.webBrPickerNotes.WebBrowserShortcutsEnabled = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 456);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(184, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "** Double click on a value to select it.";
            // 
            // dataGridViewPicker
            // 
            this.dataGridViewPicker.AllowUserToAddRows = false;
            this.dataGridViewPicker.AllowUserToDeleteRows = false;
            this.dataGridViewPicker.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewPicker.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewPicker.Location = new System.Drawing.Point(10, 174);
            this.dataGridViewPicker.MultiSelect = false;
            this.dataGridViewPicker.Name = "dataGridViewPicker";
            this.dataGridViewPicker.ReadOnly = true;
            this.dataGridViewPicker.RowHeadersVisible = false;
            this.dataGridViewPicker.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewPicker.Size = new System.Drawing.Size(673, 275);
            this.dataGridViewPicker.TabIndex = 3;
            this.dataGridViewPicker.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPicker_CellContentDoubleClick);
            this.dataGridViewPicker.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewPicker_CellDoubleClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Notes:";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(608, 456);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dlgValuePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(696, 494);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridViewPicker);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgValuePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select a Variable Value";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewPicker)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.WebBrowser webBrPickerNotes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridViewPicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClose;
    }
}