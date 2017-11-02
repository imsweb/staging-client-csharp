// Copyright (C) 2017 Information Management Services, Inc.

using System;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public enum ColumnType
    {
        INPUT,
        DESCRIPTION,
        ENDPOINT
    }

    public interface IColumnDefinition
    {
        //========================================================================================================================
        // The key representing the field name
        // @return a String name
        //========================================================================================================================
        String getKey();

        //========================================================================================================================
        // The type of column
        // @return a ColumnType representing the type
        //========================================================================================================================
        ColumnType getType();

    }
}


