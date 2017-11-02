using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicDefinition : IDefinition
    {

        private String _id;
        private List<BasicInput> _inputs;
        private List<BasicOutput> _outputs;
        private HashSet<IKeyValue> _initialContext;
        private List<IMapping> _mappings;
        private StagingInputErrorHandler _onInvalidInput;

        // parsed fields
        private Dictionary<String, IInput> _parsedInputMap = new Dictionary<String, IInput>();
        private Dictionary<String, IOutput> _parsedOutputMap = new Dictionary<String, IOutput>();

        /**
         * Default constructor
         */
        public BasicDefinition()
        {
        }

        /**
         * Construct with an indentifier
         * @param id a definition identifier
         */
        public BasicDefinition(String id)
        {
            setId(id);
        }

        public String getId()
        {
            return _id;
        }

        public void setId(String id)
        {
            _id = id;
        }

        public List<BasicInput> getInputs()
        {
            return _inputs;
        }

        public void setInputs(List<BasicInput> inputs)
        {
            _inputs = inputs;
        }

        public void addInput(String key)
        {
            if (_inputs == null)
                _inputs = new List<BasicInput>();

            _inputs.Add(new BasicInput(key));
        }

        public void addInput(BasicInput input)
        {
            if (_inputs == null)
                _inputs = new List<BasicInput>();

            _inputs.Add(input);
        }

        public void addInput(String key, String table)
        {
            if (_inputs == null)
                _inputs = new List<BasicInput>();

            _inputs.Add(new BasicInput(key, table));
        }

        public List<BasicOutput> getOutputs()
        {
            return _outputs;
        }

        public void setOutputs(List<BasicOutput> outputs)
        {
            _outputs = outputs;
        }

        public void addOutput(String key)
        {
            if (_outputs == null)
                _outputs = new List<BasicOutput>();

            _outputs.Add(new BasicOutput(key));
        }

        public void addOutput(BasicOutput output)
        {
            if (_outputs == null)
                _outputs = new List<BasicOutput>();

            _outputs.Add(output);
        }

        public HashSet<IKeyValue> getInitialContext()
        {
            return _initialContext;
        }

        public void setInitialContext(HashSet<IKeyValue> initialContext)
        {
            _initialContext = initialContext;
        }

        public void addInitialContext(String key, String value)
        {
            if (_initialContext == null)
                _initialContext = new HashSet<IKeyValue>();

            _initialContext.Add(new BasicKeyValue(key, value));
        }

        public List<IMapping> getMappings()
        {
            return _mappings;
        }

        public StagingInputErrorHandler getOnInvalidInput()
        {
            return _onInvalidInput;
        }

        public void setOnInvalidInput(StagingInputErrorHandler onInvalidInput)
        {
            _onInvalidInput = onInvalidInput;
        }

        public void setMappings(List<IMapping> mappings)
        {
            _mappings = mappings;
        }

        public void addMapping(BasicMapping mapping)
        {
            if (_mappings == null)
                _mappings = new List<IMapping>();

            _mappings.Add(mapping);
        }

        public Dictionary<String, IInput> getInputMap()
        {
            return _parsedInputMap;
        }

        public void setInputMap(Dictionary<String, IInput> parsedInputMap)
        {
            _parsedInputMap = parsedInputMap;
        }

        public Dictionary<String, IOutput> getOutputMap()
        {
            return _parsedOutputMap;
        }

        public void setOutputMap(Dictionary<String, IOutput> parsedOutputMap)
        {
            _parsedOutputMap = parsedOutputMap;
        }
    }
}
