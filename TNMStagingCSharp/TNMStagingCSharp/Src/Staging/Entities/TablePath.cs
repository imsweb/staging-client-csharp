// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public interface ITablePath
    {
        //========================================================================================================================
        // The table identifier
        // @return a String representing the table identifier
        //========================================================================================================================
        String getId();

        //========================================================================================================================
        // A List of input key redirections.  Each entry maps an input key that the table uses to a new input key to read from instead.  It allows
        // for a single table to be called multiple times and use different input.
        // @return a List of KeyMapping objects
        //========================================================================================================================
        HashSet<IKeyMapping> getInputMapping();

        //========================================================================================================================
        // A List of output key redirections.  Each entry maps an output key that the table uses to a new output key.  It allows for a single table
        // to be called multiple times and produce output on different fields.
        // @return a List of KeyMapping objects
        //========================================================================================================================
        HashSet<IKeyMapping> getOutputMapping();

        HashSet<String> getInputs();

        HashSet<String> getOutputs();

        String GetHashString();
    }
}


