// Copyright (C) 2017 Information Management Services, Inc.

using System;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public interface IDataProvider
    {
        //========================================================================================================================
        // Get a table by identifier
        // @param id the table id
        // @return a Table instance or null if identifier was not found
        //========================================================================================================================
        ITable getTable(String id);

        //========================================================================================================================
        // Get an definition by identifier
        // @param id the definition id
        // @return a Definition instance or null if identifier was not found
        //========================================================================================================================
        IDefinition getDefinition(String id);
    }
}


