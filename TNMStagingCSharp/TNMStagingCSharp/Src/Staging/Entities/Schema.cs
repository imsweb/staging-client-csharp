/*
 * Copyright (C) 2021 Information Management Services, Inc.
 */
using System;
using System.Collections.Generic;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public enum StagingInputErrorHandler
    {
        // continue staging
        CONTINUE,

        // stop staging and return a failed result
        FAIL,

        // if the failed input is used for staging, stop staging and return a failed result; otherwise continue staging
        FAIL_WHEN_USED_FOR_STAGING
    }

    /**
     * A schema which can be processed by DecisionEngine
     */
    public interface Schema
    {
        /**
         * A unique identifier for the definition
         * @return a String representing the definition identifier
         */
        String getId();

        String getAlgorithm();

        String getVersion();

        String getName();

        String getTitle();

        String getDescription();

        String getSubtitle();

        String getNotes();

        DateTime getLastModified();

        int getSchemaNum();

        String getSchemaSelectionTable();

        HashSet<String> getSchemaDiscriminators();

        List<IInput> getInputs();

        List<IOutput> getOutputs();

        /**
         * The full list of inputs needed for the definition.
         * @return a Map of input key to Input
         */
        Dictionary<String, IInput> getInputMap();

        void setInputMap(Dictionary<String, IInput> parsedInputMap);

        /**
         * The full list of outputs produced from processing the definition.
         * @return a Map of input key to Output
         */
        Dictionary<String, IOutput> getOutputMap();

        void setOutputMap(Dictionary<String, IOutput> parsedOutputMap);

        /**
         * A list of initial key/value pairs which will be set at the start of process
         * @return a Set of key/value pairs         
         */
        HashSet<IKeyValue> getInitialContext();

        /**
         * The list of mappings, in order, which will be processed
         * @return a List of Mapping objects
         */
        List<IMapping> getMappings();

        HashSet<String> getInvolvedTables();

        /**
         * How are invalid inputs handled during staging.  There are 3 choices.  First, continue processing.  Second, stop processing.  Third,
         * stop processing only if the failed input is needed for staging.
         * @return the way to handle invalid input during staging
         */
        StagingInputErrorHandler getOnInvalidInput();
    }
}

