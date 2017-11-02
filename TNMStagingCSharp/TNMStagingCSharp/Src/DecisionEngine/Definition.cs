// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public enum StagingInputErrorHandler
    {
        // continue staging
        CONTINUE,
        // stop staging and return an failed result
        FAIL,
        // if the failed input is used for staging, stop staging and return an failed result; otherwise continue staging
        FAIL_WHEN_USED_FOR_STAGING
    }


    public interface IDefinition
    {
        //========================================================================================================================
        // A unique identifier for the definition
        // @return a String representing the definition identifier
        //========================================================================================================================
        String getId();

        //========================================================================================================================
        // The full list of inputs needed for the definition.
        // @return a Map of input key to Input
        //========================================================================================================================
        Dictionary<String, IInput> getInputMap();

        //========================================================================================================================
        // The full list of outputs produced from processing the definition.
        // @return a Map of input key to Output
        //========================================================================================================================
        //Dictionary<String, IOutput> getOutputMap();
        Dictionary<String, IOutput> getOutputMap();

        //========================================================================================================================
        // A list of initial key/value pairs which will be set at the start of process
        // @return a List of key/value pairs
        //========================================================================================================================
        HashSet<IKeyValue> getInitialContext();

        //========================================================================================================================
        // The list of mappings, in order, which will be processed
        // @return a List of Mapping objects
        //========================================================================================================================
        List<IMapping> getMappings();

        //========================================================================================================================
        // How are invalid inputs handled during staging.  There are 3 choices.  First, continue processing.  Second, stop processing.  Third,
        // stop processing only if the failed input is needed for staging.
        // @return the way to handle invalid input during staging
        //========================================================================================================================
        StagingInputErrorHandler getOnInvalidInput();

    }
}


