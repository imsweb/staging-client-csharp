using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicMapping : IMapping
    {

        private String _id;
        private List<ITablePath> _inclusionTables;
        private List<ITablePath> _exclusionTables;
        private List<IKeyValue> _initialContext;
        private List<ITablePath> _tablePaths;

        /**
         * Constructor
         * @param id
         */
        public BasicMapping(String id)
        {
            _id = id;
        }

        /**
         * Contruct a BasicAlgorithm with an intial set of table paths
         * @param id String identifier
         * @param tablePaths a List of BasicTablePath objects
         */
        public BasicMapping(String id, List<ITablePath> tablePaths)
        {
            _id = id;
            _tablePaths = tablePaths;
        }

        public String getId()
        {
            return _id;
        }

        public void setId(String id)
        {
            _id = id;
        }

        public List<ITablePath> getInclusionTables()
        {
            return _inclusionTables;
        }

        public void setInclusionTables(List<ITablePath> inclusionTables)
        {
            _inclusionTables = inclusionTables;
        }

        public List<ITablePath> getExclusionTables()
        {
            return _exclusionTables;
        }

        public void setExclusionTables(List<ITablePath> exclusionTables)
        {
            _exclusionTables = exclusionTables;
        }

        public List<IKeyValue> getInitialContext()
        {
            return _initialContext;
        }

        public void setInitialContext(List<IKeyValue> initialContext)
        {
            _initialContext = initialContext;
        }

        public void addInitialContext(String key, String value)
        {
            if (_initialContext == null)
                _initialContext = new List<IKeyValue>();

            _initialContext.Add(new BasicKeyValue(key, value));
        }

        public List<ITablePath> getTablePaths()
        {
            return _tablePaths;
        }

        /**
         * Set the table paths
         * @param tablePaths a List of BasicTablePath objects
         */
        public void setTablePaths(List<ITablePath> tablePaths)
        {
            _tablePaths = tablePaths;
        }

        /**
         * Add a new table path
         * @param path a BasicTablePath
         */
        public void addTablePath(BasicTablePath path)
        {
            if (_tablePaths == null)
                _tablePaths = new List<ITablePath>();

            _tablePaths.Add(path);
        }


        public String GetHashString()
        {
            return "";
        }
    }

}
