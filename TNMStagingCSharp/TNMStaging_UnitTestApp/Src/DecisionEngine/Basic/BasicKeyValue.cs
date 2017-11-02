using System;

using TNMStagingCSharp.Src.DecisionEngine;

namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicKeyValue : IKeyValue
    {

        private String _key;
        private String _value;

        public BasicKeyValue()
        {
        }

        public BasicKeyValue(String key, String value)
        {
            _key = key;
            _value = value;
        }

        public String getKey()
        {
            return _key;
        }

        public void setKey(String key)
        {
            _key = key;
        }

        public String getValue()
        {
            return _value;
        }

        public void setValue(String value)
        {
            _value = value;
        }
    }

}
