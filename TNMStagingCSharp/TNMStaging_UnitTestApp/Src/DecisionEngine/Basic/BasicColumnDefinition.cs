using System;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicColumnDefinition : IColumnDefinition
    {

        private String _key;
        private ColumnType _type;

        /**
         * Default constructor
         */
        public BasicColumnDefinition()
        {
        }

        /**
         * Construct with a key and type
         * @param key a column key
         * @param type a column type
         */
        public BasicColumnDefinition(String key, ColumnType type)
        {
            setKey(key);
            setType(type);
        }

        public String getKey()
        {
            return _key;
        }

        /**
         * Set the column key
         * @param key a column key
         */
        public void setKey(String key)
        {
            _key = key;
        }

        public ColumnType getType()
        {
            return _type;
        }

        /**
         * Set the column type
         * @param type a column type
         */
        public void setType(ColumnType type)
        {
            _type = type;
        }
    }

}
