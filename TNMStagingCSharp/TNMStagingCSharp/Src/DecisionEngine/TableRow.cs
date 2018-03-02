// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    

    public interface ITableRow
    {
        //========================================================================================================================
        // A list of values from the input column represented by the passed key
        // @param key the field name of the column
        // @return a List of Range objects
        //========================================================================================================================
        List<Range> getColumnInput(String key);

        //========================================================================================================================
        // A list of endpoints on the row
        // @return a List of Endpoint objects
        //========================================================================================================================
        List<IEndpoint> getEndpoints();
    }
}

