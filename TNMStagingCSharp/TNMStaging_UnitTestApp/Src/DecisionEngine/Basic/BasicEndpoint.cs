using System;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicEndpoint : IEndpoint
    {

        private EndpointType _type;
        private String _value;
        private String _resultKey;

        /**
         * Default constructor
         */
        public BasicEndpoint()
        {
        }

        /**
         * Construct with a type and value
         * @param type a type
         * @param value a value
         */
        public BasicEndpoint(EndpointType type, String value)
        {
            _type = type;
            _value = value;
        }

        public EndpointType getType()
        {
            return _type;
        }

        public void setType(EndpointType type)
        {
            _type = type;
        }

        public String getValue()
        {
            return _value;
        }

        public void setValue(String value)
        {
            _value = value;
        }

        public String getResultKey()
        {
            return _resultKey;
        }

        public void setResultKey(String resultKey)
        {
            _resultKey = resultKey;
        }

    }
}
