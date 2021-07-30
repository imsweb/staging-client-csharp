// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public interface ITable
    {
        //========================================================================================================================
        // A unique identifier for the table
        // @return a String representing the table identifier
        //========================================================================================================================
        String getId();

        String getAlgorithm();

        String getVersion();

        String getName();

        String getTitle();

        String getDescription();

        String getSubtitle();

        String getNotes();

        String getFootnotes();

        DateTime getLastModified();

        //========================================================================================================================
        // Return a list of the column definitions
        // @return a List of ColumnDefinition
        //========================================================================================================================
        List<IColumnDefinition> getColumnDefinitions();

        //========================================================================================================================
        // Returns a list of input keys that are references in the table rows.  References are in the format "{{key}}".
        // @return a list of input keys
        //========================================================================================================================
        HashSet<String> getExtraInput();

        void setExtraInput(HashSet<String> extraInput);

        List<List<String>> getRawRows();

        //========================================================================================================================
        // Return the data of the table as a list of rows
        // @return a List of TableRow objects
        //========================================================================================================================
        List<ITableRow> getTableRows();

        void addTableRow(ITableRow row);

        void clearTableRows();


        List<IColumnDefinition> getInputColumnDefinitions();
        void GenerateInputColumnDefinitions();
    }
}


