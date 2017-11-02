// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging
{
    public class StagingData
    {
        // key definitions
        public static readonly String PRIMARY_SITE_KEY = "site";
        public static readonly String HISTOLOGY_KEY = "hist";
        public static readonly String YEAR_DX_KEY = "year_dx";

        // set of keys that are standard for all schema lookups; any other keys are considered a discriminator
        public static readonly ReadOnlyCollection<String> STANDARD_LOOKUP_KEYS = new ReadOnlyCollection<String>(new List<String>
                { PRIMARY_SITE_KEY, HISTOLOGY_KEY });


        [JsonProperty("result", Order = 1)]
        private Result _result;
        [JsonProperty("schema_id", Order = 2)]
        private String _schemaId;
        [JsonProperty("input", Order = 3)]
        [JsonConverter(typeof(Dictionary<String, String>))]
        private Dictionary<String, String> _input = new Dictionary<String, String>(100, StringComparer.Ordinal);
        [JsonProperty("output", Order = 4)]
        [JsonConverter(typeof(Dictionary<String, String>))]
        private Dictionary<String, String> _output = new Dictionary<String, String>(100, StringComparer.Ordinal);
        [JsonProperty("errors", Order = 5)]
        [JsonConverter(typeof(List<Error>))]
        private List<Error> _errors = new List<Error>(20);
        [JsonProperty("path", Order = 6)]
        [JsonConverter(typeof(List<String>))]
        private List<String> _path = new List<String>(40);


        public enum Result
        {
            // staging was performed
            STAGED,

            // both primary site and histology must be supplied
            FAILED_MISSING_SITE_OR_HISTOLOGY,

            // no matching schema was found
            FAILED_NO_MATCHING_SCHEMA,

            // multiple matching schemas were found; a discriminator is probably needed
            FAILED_MULITPLE_MATCHING_SCHEMAS,

            // year of DX out of valid range
            FAILED_INVALID_YEAR_DX,

            // a field that was flagged as "fail_on_invalid" has an invalid value
            FAILED_INVALID_INPUT
        }

        // Default constructor
        public StagingData()
        {
        }

        // Construct with input map
        // @param input input map
        public StagingData(Dictionary<String, String> input)
        {
            _input = input;
        }

        // Construct with site/histology
        // @param site primary site
        // @param hist histology
        public StagingData(String site, String hist)
        {
            setInput(PRIMARY_SITE_KEY, site);
            setInput(HISTOLOGY_KEY, hist);
        }

        public Result getResult()
        {
            return _result;
        }

        public void setResult(Result result)
        {
            _result = result;
        }

        public String getSchemaId()
        {
            return _schemaId;
        }

        public void setSchemaId(String schemaId)
        {
            _schemaId = schemaId;
        }

        public Dictionary<String, String> getInput()
        {
            return _input;
        }

        public String getInput(String key)
        {
            String sRetval = "";
            if (!_input.TryGetValue(key, out sRetval))
            {
                return null;
            }
            return sRetval;
        }

        public void setInput(String key, String value)
        {
            _input[key] = value;
        }

        // output getters

        public Dictionary<String, String> getOutput()
        {
            return _output;
        }

        public String getOutput(String key)
        {
            String sRetval = "";
            if (!_output.TryGetValue(key, out sRetval)) sRetval = "";
            return sRetval;
        }

        public void setOutput(Dictionary<String, String> output)
        {
            _output = output;
        }

        // errors

        public List<Error> getErrors()
        {
            return _errors;
        }

        public void setErrors(List<Error> errors)
        {
            _errors = errors;
        }

        public void addError(Error error)
        {
            _errors.Add(error);
        }

        // path

        public List<String> getPath()
        {
            return _path;
        }

        public void setPath(List<String> path)
        {
            _path = path;
        }
    }
}


