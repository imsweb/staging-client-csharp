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
            webBrTableNotes.DocumentText = "";

            StagingTable thisTable = mStaging.getTable(msTableId);
            if (thisTable != null)
            {
                lblTableName.Text = thisTable.getName();
                lblTitle.Text = thisTable.getTitle();
                lblSubtitle.Text = thisTable.getSubtitle();
                lblDescription.Text = thisTable.getDescription();

                String sNotes = thisTable.getNotes();

                CommonMark.CommonMarkSettings settings = CommonMark.CommonMarkSettings.Default.Clone();
                settings.RenderSoftLineBreaksAsLineBreaks = true;
                String result = CommonMark.CommonMarkConverter.Convert(sNotes, settings);
                result = result.Replace("<p>", "<p style=\"font-family=Microsoft Sans Serif;font-size:11px\">");
                webBrTableNotes.DocumentText = result;

                // Set the rows to auto size to view the contents of larger text cells
                dataGridViewTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // Obtain the number of columns and column labels for the table
                /*
                int iNumColumns = TNMStage_get_table_num_columns(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId);
                int iNumRows = TNMStage_get_table_num_rows(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId);
                for (iColIndex = 0; iColIndex < iNumColumns; iColIndex++)
                {
                    // Add a new column to the table data view
                    String ^ ColumnName = gcnew String("ColumnName");
                    ColumnName += iColIndex;
                    String ^ ColumnLabel = gcnew String(TNMStage_get_table_column_name(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId, iColIndex));
                    dataGridViewTable->Columns->Add(ColumnName, ColumnLabel);

                    // Get the column type.  If the type is "INPUT" OR "DESCRIPTION", then display the column.  Otherwise skip it.
                    iType = TNMStage_get_table_column_type(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId, iColIndex);
                    if ((iType == TNM_COLUMN_INPUT) && (iInputCol == -1))
                        iInputCol = iColIndex;
                }
                */

            }
        }

        /*
		    // Set widths of columns and their text wrap mode
		    iViewableWidth = dataGridViewTable->Width;
		    if (iInputCol >= 0)
			    iViewableWidth = dataGridViewTable->Width - 50; // remove size of input column
		    if (iNumColumns > 1)
			    iWidthDivisor = (int)((double)iViewableWidth / (double)(iNumColumns - 1));
		    else
			    iWidthDivisor = 300;
		    for (i = 0; i < dataGridViewTable->Columns->Count; i++)
		    {
			    if (i == iInputCol)
			    {
				    if (dataGridViewTable->Columns->Count == 1)
					    dataGridViewTable->Columns[i]->Width = iWidthDivisor;
				    else
					    dataGridViewTable->Columns[i]->Width = 100;
			    }
			    else
				    dataGridViewTable->Columns[i]->Width = iWidthDivisor;
			    dataGridViewTable->Columns[i]->DefaultCellStyle->WrapMode = DataGridViewTriState::True;
			    dataGridViewTable->Columns[i]->SortMode = DataGridViewColumnSortMode::NotSortable;
		    }


		    // Now loop through the table rows and add each cell to the table
		    for (iRowIndex = 0; iRowIndex < iNumRows; iRowIndex++)
		    {
			    dataGridViewTable->RowCount++;
			    dataGridViewTable->Rows[iRowIndex]->Height = 40;

			    for (iColIndex = 0; iColIndex < iNumColumns; iColIndex++)
			    {
				    // Get the column type.  If the type is "INPUT" OR "DESCRIPTION", then display the column.  Otherwise skip it.
				    strptrTableId = (char*)Marshal::StringToHGlobalAnsi(gsTableId).ToPointer();
				    iType = TNMStage_get_table_column_type(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId, iColIndex);

				    if ((iType == TNM_COLUMN_INPUT) || (iType == TNM_COLUMN_DESCRIPTION))
				    {
					    // Nothing to do here - just display the string obtained from the DLL
					    String ^ CellValue = gcnew String(TNMStage_get_table_cell_value(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId, iRowIndex, iColIndex));
					    dataGridViewTable[iColIndex, iRowIndex]->Value = CellValue;

					    if (iType == TNM_COLUMN_INPUT)
						    dataGridViewTable->Columns[iColIndex]->DefaultCellStyle->Alignment = DataGridViewContentAlignment::MiddleCenter;
					    else
						    dataGridViewTable->Columns[iColIndex]->DefaultCellStyle->Alignment = DataGridViewContentAlignment::TopLeft;
				    }
				    else if (iType == TNM_COLUMN_ENDPOINT)   // has to be TNM_COLUMN_ENDPOINT
				    {
					    // Endpoints need a small text adjustment in order to make the viewing a little nicer
					    // You need to remove the "VALUE:" string at the beginning of the string
					    String ^ CellValue = gcnew String(TNMStage_get_table_cell_value(sAlgorithmName.c_str(), sAlgorithmVersion.c_str(), strptrTableId, iRowIndex, iColIndex));
					    if (CellValue->StartsWith("VALUE:"))
						    CellValue = CellValue->Replace("VALUE:","");
					    dataGridViewTable[iColIndex, iRowIndex]->Value = CellValue;

					    dataGridViewTable->Columns[iColIndex]->DefaultCellStyle->Alignment = DataGridViewContentAlignment::MiddleCenter;
				    }

				    // free pointer
				    Marshal::FreeHGlobal(IntPtr(strptrTableId));
			    }
		    }
         */



        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
