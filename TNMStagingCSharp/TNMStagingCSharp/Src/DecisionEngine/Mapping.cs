// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public interface IMapping
    {
        //========================================================================================================================
        // A unique identifier for the mapping
        // @return a String representing the mapping identifier
        //========================================================================================================================
        String getId();

        //========================================================================================================================
        // Return a list of table names containing the inclusion conditions
        // @return the list of table names
        //========================================================================================================================
        List<ITablePath> getInclusionTables();

        //========================================================================================================================
        // Return a list of table names containing the exclusion conditions
        // @return the list of table names
        //========================================================================================================================
        List<ITablePath> getExclusionTables();

        //========================================================================================================================
        // A list of initial key/value pairs which will be set at the start of the mapping
        // @return a List of key/value pairs
        //========================================================================================================================
        List<IKeyValue> getInitialContext();

        //========================================================================================================================
        // The list of table paths, in order, which will be processed
        // @return a List of TablePath objects
        //========================================================================================================================
        List<ITablePath> getTablePaths();

        String GetHashString();
    }
}


