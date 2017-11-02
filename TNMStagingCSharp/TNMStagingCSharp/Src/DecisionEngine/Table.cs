// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public interface ITable
    {
        //========================================================================================================================
        // A unique identifier for the table
        // @return a String representing the table identifier
        //========================================================================================================================
        String getId();

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

        //========================================================================================================================
        // Return the data of the table as a list of rows
        // @return a List of TableRow objects
        //========================================================================================================================
        List<ITableRow> getTableRows();


        List<IColumnDefinition> getInputColumnDefinitions();
        void GenerateInputColumnDefinitions();
    }
}


