using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicTablePath : ITablePath
    {

        private String _id;
        private HashSet<IKeyMapping> _input;
        private HashSet<IKeyMapping> _output;

        /**
         * Default constructor
         */
        public BasicTablePath()
        {
        }

        /**
         * Construct with a table name
         * @param id a table identifier
         */
        public BasicTablePath(String id)
        {
            _id = id;
        }

        public String getId()
        {
            return _id;
        }

        public void setId(String id)
        {
            _id = id;
        }

        public HashSet<IKeyMapping> getInputMapping()
        {
            return _input;
        }

        public void setInputMapping(HashSet<IKeyMapping> input)
        {
            _input = input;
        }

        public void addInputMapping(String from, String to)
        {
            if (_input == null)
                _input = new HashSet<IKeyMapping>();

            _input.Add(new BasicKeyMapping(from, to));
        }

        public HashSet<IKeyMapping> getOutputMapping()
        {
            return _output;
        }

        public void setOutputMapping(HashSet<IKeyMapping> output)
        {
            _output = output;
        }

        public void addOutputMapping(String from, String to)
        {
            if (_output == null)
                _output = new HashSet<IKeyMapping>();

            _output.Add(new BasicKeyMapping(from, to));
        }

        public String GetHashString()
        {
            return "";
        }
    }

}
