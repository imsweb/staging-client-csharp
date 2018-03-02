// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingTableRow: ITableRow
    {

        [JsonProperty("inputs")]
        [JsonConverter(typeof(CustomDictionaryConverter<String, List<StagingRange>>))]
        private Dictionary<String, List<Range>> _inputs = new Dictionary<String, List<Range>>(20);
        [JsonProperty("endpoint")]
        [JsonConverter(typeof(CustomListConverter<StagingEndpoint>))]
        private List<IEndpoint> _endpoints = new List<IEndpoint>(20);

        private int iNumInputs = 0;
        private String[] arrKeys;
        private List<Range>[] arrValues;

        public List<Range> getColumnInput(String key)
        {
            for (int i = 0; i < iNumInputs; i++)
            {
                if (arrKeys[i] == key) return arrValues[i];
            }
            return null;
        }

        public void ConvertColumnInput()
        {
            iNumInputs = _inputs.Count;
            arrKeys = new String[iNumInputs];
            arrValues = new List<Range>[iNumInputs];

            int iIndex = 0;
            foreach (KeyValuePair<String, List<Range>> entry in _inputs)
            {
                arrKeys[iIndex] = entry.Key;
                arrValues[iIndex] = entry.Value;
                iIndex++;
            }
        }

        public Dictionary<String, List<Range>> getInputs()
        {
            return _inputs;
        }

        public void setInputs(Dictionary<String, List<Range>> inputs)
        {
            _inputs = inputs;
        }

        // Add a single columns input list
        // @param key key
        // @param range range
        public void addInput(String key, List<Range> range)
        {
            _inputs[key] = range;
        }

        public List<IEndpoint> getEndpoints()
        {
            return _endpoints;
        }

        public void setEndpoints(List<IEndpoint> endpoints)
        {
            _endpoints = endpoints;
        }

        public void addEndpoint(StagingEndpoint endpoint)
        {
            _endpoints.Add(endpoint);
        }
    }
}

  