using System;

using TNMStagingCSharp.Src.DecisionEngine;

namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicInput : IInput
    {

        private String _key;
        private String _default;
        private String _table;
        private Boolean _usedForStaging;

        /**
         * Default constrctor
         */
        public BasicInput()
        {
        }

        /**
         * Construct with an input key
         * @param key input key
         */
        public BasicInput(String key)
        {
            setKey(key);
        }

        /**
         * Construct with an input key and table
         * @param key input key
         * @param table table
         */
        public BasicInput(String key, String table)
        {
            setKey(key);
            setTable(table);
        }

        public String getKey()
        {
            return _key;
        }

        public void setKey(String key)
        {
            _key = key;
        }

        public String getDefault()
        {
            return _default;
        }

        public void setDefault(String aDefault)
        {
            _default = aDefault;
        }

        public String getTable()
        {
            return _table;
        }

        public void setTable(String table)
        {
            _table = table;
        }

        public Boolean getUsedForStaging()
        {
            return _usedForStaging;
        }

        public void setUsedForStaging(Boolean usedForStaging)
        {
            _usedForStaging = usedForStaging;
        }
    }

}
