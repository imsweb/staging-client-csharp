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
    public partial class dlgTable : Form
    {
        private Staging mStaging;
        private String msSchemaName;
        private String msTableId;

        public dlgTable(Staging pStaging, String sSchemaName, String sTableId)
        {
            InitializeComponent();

            mStaging = pStaging;
            msSchemaName = sSchemaName;
            msTableId = sTableId;

            lblSchemaName.Text = msSchemaName;
            lblTableId.Text = msTableId;
            lblTableName.Text = "";
            lblTitle.Text = "";
            lblSubtitle.Text = "";
            lblDescription.Text = "";
            //webBrTableNotes.DocumentText = "";

            StagingTable thisTable = mStaging.getTable(msTableId);
            if (thisTable != null)
            {
                lblTableName.Text = thisTable.getName();
                lblTitle.Text = thisTable.getTitle();
                lblSubtitle.Text = thisTable.getSubtitle();
                lblDescription.Text = thisTable.getDescription();

                String sNotes = thisTable.getNotes();
                if (sNotes == null) sNotes = "";

                CommonMark.CommonMarkSettings settings = CommonMark.CommonMarkSettings.Default.Clone();
                settings.RenderSoftLineBreaksAsLineBreaks = true;
                String result = CommonMark.CommonMarkConverter.Convert(sNotes, settings);
                result = result.Replace("<p>", "<p style=\"font-family=Microsoft Sans Serif;font-size:11px\">");
                webBrTableNotes.DocumentText = result;

                // Set the rows to auto size to view the contents of larger text cells
                dataGridViewTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // Obtain the number of columns and column labels for the table
                String ColumnName = "";
                String ColumnLabel = "";
                ColumnType cType = 0;
                int iInputCol = 0;

                List<IColumnDefinition> cols = thisTable.getColumnDefinitions();
                StagingColumnDefinition thisColDef = null;
                int iNumColumns = 0;
                int iNumRows = 0;
                if (thisTable.getRawRows() != null) iNumRows = thisTable.getRawRows().Count;
                if (cols != null)
                {
                    iNumColumns = cols.Count;
                    //int iNumRows = TNMStage_get_table_num_rows(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId);
                    for (int iColIndex = 0; iColIndex < cols.Count; iColIndex++)
                    {
                        thisColDef = (StagingColumnDefinition)cols[iColIndex];
                        // Add a new column to the table data view
                        ColumnName = "ColumnName" + iColIndex + thisColDef.getName();
                        ColumnLabel = thisColDef.getName();
                        dataGridViewTable.Columns.Add(ColumnName, ColumnLabel);

                        // Get the column type.  If the type is "INPUT" OR "DESCRIPTION", then display the column.  Otherwise skip it.
                        cType = thisColDef.getType();
                        if ((cType == ColumnType.INPUT) && (iInputCol == -1))
                            iInputCol = iColIndex;
                    }
                }

                // Set widths of columns and their text wrap mode
                int iViewableWidth = dataGridViewTable.Width;
                int iWidthDivisor = 0;
                if (iInputCol >= 0)
                    iViewableWidth = dataGridViewTable.Width - 120; // remove size of input column
                if (iNumColumns > 1)
                    iWidthDivisor = (int)((double)iViewableWidth / (double)(iNumColumns - 1));
                else
                    iWidthDivisor = 300;

                for (int i = 0; i < dataGridViewTable.Columns.Count; i++)
                {
                    if (i == iInputCol)
                    {
                        if (dataGridViewTable.Columns.Count == 1)
                            dataGridViewTable.Columns[i].Width = iWidthDivisor;
                        else
                            dataGridViewTable.Columns[i].Width = 100;
                    }
                    else
                        dataGridViewTable.Columns[i].Width = iWidthDivisor;
                    dataGridViewTable.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dataGridViewTable.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                // Now loop through the table rows and add each cell to the table
                List<String> thisRow = null;
                String CellValue = "";
                for (int iRowIndex = 0; iRowIndex < iNumRows; iRowIndex++)
                {
                    dataGridViewTable.RowCount++;
                    dataGridViewTable.Rows[iRowIndex].Height = 40;

                    thisRow = null;
                    if (thisTable.getRawRows() != null) thisRow = thisTable.getRawRows()[iRowIndex];


                    for (int iColIndex = 0; iColIndex < iNumColumns; iColIndex++)
                    {
                        thisColDef = (StagingColumnDefinition)cols[iColIndex];
                        // Get the column type.  If the type is "INPUT" OR "DESCRIPTION", then display the column.  Otherwise skip it.
                        cType = thisColDef.getType();

                        CellValue = "";
                        if (thisRow.Count > iColIndex) CellValue = thisRow[iColIndex];
                        if ((cType == ColumnType.INPUT) || (cType == ColumnType.DESCRIPTION))
                        {
                            // Nothing to do here - just display the string obtained from the DLL
                            dataGridViewTable[iColIndex, iRowIndex].Value = CellValue;

                            if (cType == ColumnType.INPUT)
                                dataGridViewTable.Columns[iColIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            else
                                dataGridViewTable.Columns[iColIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.TopLeft;
                        }
                        else if (cType == ColumnType.ENDPOINT)   // has to be TNM_COLUMN_ENDPOINT
                        {
                            // Endpoints need a small text adjustment in order to make the viewing a little nicer
                            // You need to remove the "VALUE:" string at the beginning of the string
                            if (CellValue.StartsWith("VALUE:"))
                                CellValue = CellValue.Replace("VALUE:", "");
                            dataGridViewTable[iColIndex, iRowIndex].Value = CellValue;

                            dataGridViewTable.Columns[iColIndex].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }

                    }
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
