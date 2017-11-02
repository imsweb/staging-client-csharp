using System;
using System.Collections.Generic;
using System.Linq;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicTableRow : ITableRow
    {

        private Dictionary<String, List<BasicStringRange>> _inputs = new Dictionary<String, List<BasicStringRange>>();
        private String _description;
        private List<IEndpoint> _endpoints = new List<IEndpoint>();

        public List<StringRange> getColumnInput(String key)
        {
            List<BasicStringRange> oRetval = null;
            if (!_inputs.TryGetValue(key, out oRetval)) oRetval = null;
            return oRetval.ToList<StringRange>();
        }

        public Dictionary<String, List<BasicStringRange>> getInputs()
        {
            return _inputs;
        }

        public void setInputs(Dictionary<String, List<BasicStringRange>> inputs)
        {
            _inputs = inputs;
        }

        /**
         * Add a single columns input list
         * @param key an input key
         * @param range a List of BasicStringRange objects
         */
        public void addInput(String key, List<BasicStringRange> range)
        {
            _inputs[key] = range;
        }

        public String getDescription()
        {
            return _description;
        }

        public void setDescription(String description)
        {
            _description = description;
        }

        public List<IEndpoint> getEndpoints()
        {
            return _endpoints;
        }

        public void setEndpoints(List<IEndpoint> endpoints)
        {
            _endpoints = endpoints;
        }

        public void addEndpoint(BasicEndpoint endpoint)
        {
            _endpoints.Add(endpoint);
        }
    }

}
