using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_StagingViewerApp
{
    public partial class dlgValuePicker : Form
    {
        private Staging mStaging;

        private String msSelectedValue;
        private String msGetSelectedValueLabel;
        private String msInputVar;
        private String msTableId;
        private int miInputCol;
        private int miDescriptionColumn;


        public dlgValuePicker(Staging pStaging, String sTableId, String sInputVar)
        {
            InitializeComponent();

            mStaging = pStaging;

            msTableId = sTableId;
            msInputVar = sInputVar;
            miInputCol = -1;
            miDescriptionColumn = -1;

            Setup();

        }

        public string GetSelectedValue()
        {
            return msSelectedValue;
        }
        public string GetSelectedValueLabel()
        {
            return msGetSelectedValueLabel;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void dataGridViewPicker_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CellSelectedInGrid();
        }

        private void dataGridViewPicker_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            CellSelectedInGrid();
        }

        private void Setup()
        {
            int iNumColumnsUsed = 0;
            List<int> lstDescriptionColumns = new List<int>();
            String sColumnName = "";
            String sColumnLabel = "";

            StagingTable thisTable = mStaging.getTable(msTableId);

            //String sVariableTitle = thisTable.getTitle();
            //String sVariableSubtitle = thisTable.getSubtitle();

            Text = msInputVar;

            // Convert the notes, which are in MarkDown, to HTML
            String sNotes = thisTable.getNotes();
            if (sNotes == null) sNotes = ""; CommonMark.CommonMarkSettings settings = CommonMark.CommonMarkSettings.Default.Clone();
            settings.RenderSoftLineBreaksAsLineBreaks = true;
            String result = CommonMark.CommonMarkConverter.Convert(sNotes, settings);
            result = result.Replace("<p>", "<p style=\"font-family=Microsoft Sans Serif;font-size:11px\">");
            webBrPickerNotes.DocumentText = result;


            // Set the rows to auto size to view the contents of larger text cells
            dataGridViewPicker.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Obtain the number of columns and rows for the table
            List<IColumnDefinition> lstColDefs = thisTable.getColumnDefinitions();
            StagingColumnDefinition thisColDef = null;
            int iNumColumns = 0;
            int iNumRows = 0;
            ColumnType cType = 0;
            if (thisTable.getRawRows() != null) iNumRows = thisTable.getRawRows().Count;
            if (lstColDefs != null)
            {
                iNumColumns = lstColDefs.Count;
                for (int iColIndex = 0; iColIndex < lstColDefs.Count; iColIndex++)
                {
                    thisColDef = (StagingColumnDefinition)lstColDefs[iColIndex];

                    // Get the column type.  If the type is "INPUT" OR "DESCRIPTION", then display the column.  Otherwise skip it.
                    cType = thisColDef.getType();
                    if ((cType == ColumnType.INPUT) || (cType == ColumnType.DESCRIPTION))
                    {
                        // Add a new column to the table data view
                        sColumnName = "ColumnName";
                        sColumnName += iColIndex;
                        sColumnLabel = thisColDef.getName();
                        dataGridViewPicker.Columns.Add(sColumnName, sColumnLabel);
                        iNumColumnsUsed++;

                        // keep track of descrption columns
                        if (cType == ColumnType.DESCRIPTION)
                            lstDescriptionColumns.Add(iColIndex);

                        // Need to keep track of the first INPUT column.  That column contains the information we pass back if a user
                        // selects the row or cell.
                        if ((cType == ColumnType.INPUT) && (miInputCol == -1))
                            miInputCol = iNumColumnsUsed - 1;

                        dataGridViewPicker.Columns[iNumColumnsUsed - 1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        dataGridViewPicker.Columns[iNumColumnsUsed - 1].SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
            }


            // Set the widths of the columns in the datagridview.  Set the input column to be small and the description columns to be large
            //  We do this so that if we only have an input and description will fill the entire grid view (no veritical scrolling).
            //  If we have 2 or more desription fields, then we need to fit them all in.
            // 
            int iWidthDivisor = 0;
            int iViewableWidth = dataGridViewPicker.Width - 50; // remove size of input column
            if (iNumColumnsUsed > 1)
                iWidthDivisor = (int)((double)iViewableWidth / (double)(iNumColumnsUsed - 1));
            else
                iWidthDivisor = 300;
            for (int i = 0; i < dataGridViewPicker.Columns.Count; i++)
            {
                if (i == miInputCol)
                {
                    if (iNumColumnsUsed == 1)
                        dataGridViewPicker.Columns[i].Width = 300;
                    else
                        dataGridViewPicker.Columns[i].Width = 50;
                }
                else
                    dataGridViewPicker.Columns[i].Width = iWidthDivisor;
            }

            // Now loop through the table rows and add each cell to the table
            String sCellValue = "";
            for (int iRowIndex = 0; iRowIndex < iNumRows; iRowIndex++)
            {
                dataGridViewPicker.RowCount++;
                dataGridViewPicker.Rows[iRowIndex].Height = 40;

                for (int iColIndex = 0; iColIndex < iNumColumns; iColIndex++)
                {
                    thisColDef = (StagingColumnDefinition)lstColDefs[iColIndex];
                    cType = thisColDef.getType();

                    if ((cType == ColumnType.INPUT) || (cType == ColumnType.DESCRIPTION))
                    {
                        sCellValue = thisTable.getRawRows()[iRowIndex][iColIndex];
                        dataGridViewPicker[iColIndex, iRowIndex].Value = sCellValue;
                    }
                }
            }

            // See if there are two or more description fields.  If so, see if we can find one
            // that has a label of "Description"
            miDescriptionColumn = -1;
            if (lstDescriptionColumns.Count > 1)
            {
                sColumnLabel = "";
                for (int iColIndex = 0; iColIndex < lstDescriptionColumns.Count; iColIndex++)
                {
                    sColumnLabel = ((StagingColumnDefinition)lstColDefs[lstDescriptionColumns[iColIndex]]).getName();
                    if (sColumnLabel == "Description")
                        miDescriptionColumn = lstDescriptionColumns[iColIndex];
                }
                if (miDescriptionColumn == -1)
                    miDescriptionColumn = lstDescriptionColumns[0];
            }
            else if (lstDescriptionColumns.Count == 0)
                miDescriptionColumn = 0;  // for tables that do not have a description.  point to the code
            else
                miDescriptionColumn = lstDescriptionColumns[0];
        }

        private void CellSelectedInGrid()
        {
            //Save the value
            //int columnIndex = dataGridViewPicker.CurrentCell.ColumnIndex;
            int rowIndex = dataGridViewPicker.CurrentCell.RowIndex;

            if (rowIndex >= 0)
            {
                if (miInputCol >= 0)
                    msSelectedValue = dataGridViewPicker[miInputCol, rowIndex].Value.ToString();
    			else
	    			msSelectedValue = dataGridViewPicker[0, rowIndex].Value.ToString();

                // If the selected value is a range, then grab the first value
                if (msSelectedValue.IndexOf("-") >= 0)
                {
                    // if the dash is the last character in the cell, then don't take the substring
                    if (msSelectedValue.IndexOf("-") != (msSelectedValue.Length - 1))
                        msSelectedValue = msSelectedValue.Substring(0, msSelectedValue.IndexOf("-"));
                }

                // For now just grab the value of the next column regardless of the table type
                if (miDescriptionColumn < 0)
                    miDescriptionColumn = 1; // get 2nd colmn value - treat it as the description to show.

                if (dataGridViewPicker.Columns.Count == 1)
                    msGetSelectedValueLabel = "";
			    else
				    msGetSelectedValueLabel = dataGridViewPicker[miDescriptionColumn, rowIndex].Value.ToString();
            }
            else
                msSelectedValue = "";

            // Close the dialog
            DialogResult = DialogResult.OK;
        }

    }
}
