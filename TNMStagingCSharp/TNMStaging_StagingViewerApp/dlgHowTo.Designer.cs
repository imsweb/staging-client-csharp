namespace TNMStaging_StagingViewerApp
{
    partial class dlgHowTo
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
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDisp1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblDisp2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(216, 387);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 27);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(172, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "1. Select a Staging Algorithm";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(274, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "2. Display Schema Information or Stage a Case";
            // 
            // lblStep1
            // 
            this.lblStep1.Location = new System.Drawing.Point(31, 35);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size(486, 68);
            this.lblStep1.TabIndex = 1;
            this.lblStep1.Text = "XXXXXX";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(31, 151);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(486, 26);
            this.label4.TabIndex = 3;
            this.label4.Text = "These two tabs appear just below the \"Select Staging Algorithm\" control. Their de" +
    "scription follows:";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(31, 177);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(486, 26);
            this.label3.TabIndex = 4;
            this.label3.Text = "Display Schema Information";
            // 
            // lblDisp1
            // 
            this.lblDisp1.Location = new System.Drawing.Point(31, 203);
            this.lblDisp1.Name = "lblDisp1";
            this.lblDisp1.Size = new System.Drawing.Size(486, 31);
            this.lblDisp1.TabIndex = 5;
            this.lblDisp1.Text = "XXXX";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(31, 245);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(486, 26);
            this.label6.TabIndex = 6;
            this.label6.Text = "Stage a Case";
            // 
            // lblDisp2
            // 
            this.lblDisp2.Location = new System.Drawing.Point(31, 271);
            this.lblDisp2.Name = "lblDisp2";
            this.lblDisp2.Size = new System.Drawing.Size(486, 96);
            this.lblDisp2.TabIndex = 7;
            this.lblDisp2.Text = "XXXX";
            // 
            // dlgHowTo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 426);
            this.Controls.Add(this.lblDisp2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblDisp1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblStep1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "dlgHowTo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Steps to use the Staging Algorithm Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblStep1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDisp1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblDisp2;
    }
}