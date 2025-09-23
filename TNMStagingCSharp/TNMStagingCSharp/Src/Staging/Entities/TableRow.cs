// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    

    public interface ITableRow
    {
        void ConvertColumnInput();

        //========================================================================================================================
        // Return the list of columns
        // @return a Set of columns
        //========================================================================================================================
        HashSet<String> getColumns();

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

        void addInput(String key, List<Range> range);

        void addEndpoint(IEndpoint endpoint);

    }
}

