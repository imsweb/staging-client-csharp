// Copyright(C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

using TNMStagingCSharp.Src.Tools;
using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingMapping : IMapping
    {
        [JsonProperty("id", Order = 1)]
        private String _id;
        [JsonProperty("name", Order = 2)]
        private String _name;
        [JsonProperty("inclusion_tables", Order = 3)]
        [JsonConverter(typeof(CustomListConverter_StagingTablePath_ITablePath))]
        private List<ITablePath> _inclusionTables;
        [JsonProperty("exclusion_tables", Order = 4)]
        [JsonConverter(typeof(CustomListConverter_StagingTablePath_ITablePath))]
        private List<ITablePath> _exclusionTables;
        [JsonProperty("initial_context", Order = 5)]
        [JsonConverter(typeof(CustomListConverter_StagingKeyValue_IKeyValue))]
        private List<IKeyValue> _initialContext;
        [JsonProperty("tables", Order = 6)]
        [JsonConverter(typeof(CustomListConverter_StagingTablePath_ITablePath))]
        private List<ITablePath> _tablePaths;


        int miHashCode;


        // Default constructor
        public StagingMapping()
        {
        }

        // Constructs with a name and title
        // @param id identifier
        // @param name name
        public StagingMapping(String id, String name)
        {
            setId(id);
            setName(name);
            ComputeHashCode();
        }

        public String getId()
        {
            return _id;
        }

        public void setId(String id)
        {
            _id = id;
            ComputeHashCode();
        }

        public String getName()
        {
            return _name;
        }

        public void setName(String name)
        {
            _name = name;
            ComputeHashCode();
        }

        public List<ITablePath> getInclusionTables()
        {
            return _inclusionTables;
        }

        public void setInclusionTables(List<ITablePath> inclusionTables)
        {
            _inclusionTables = inclusionTables;
            ComputeHashCode();
        }

        public List<ITablePath> getExclusionTables()
        {
            return _exclusionTables;
        }

        public void setExclusionTables(List<ITablePath> exclusionTables)
        {
            _exclusionTables = exclusionTables;
            ComputeHashCode();
        }

        public List<IKeyValue> getInitialContext()
        {
            return _initialContext;
        }

        public void setInitialContext(List<IKeyValue> initialContext)
        {
            _initialContext = initialContext;
            ComputeHashCode();
        }

        public List<ITablePath> getTablePaths()
        {
            return _tablePaths;
        }

        public void setTablePaths(List<ITablePath> tablePaths)
        {
            _tablePaths = tablePaths;
            ComputeHashCode();
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            StagingMapping mapping = (StagingMapping)o;

            return Equals(_id, mapping._id) &&
                   Equals(_name, mapping._name) &&
                   Equals(_inclusionTables, mapping._inclusionTables) &&
                   Equals(_exclusionTables, mapping._exclusionTables) &&
                   Equals(_initialContext, mapping._initialContext) &&
                   Equals(_tablePaths, mapping._tablePaths);
        }

        public override int GetHashCode()
        {
            return miHashCode;
        }
        private void ComputeHashCode()
        {
            miHashCode = GetHashString().GetHashCode();
        }

        public String GetHashString()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_id);
            MyStringBuilder.Append(_name);
            if (_inclusionTables != null)
            {
                for (int i = 0; i < _inclusionTables.Count; i++)
                {
                    MyStringBuilder.Append(_inclusionTables[i].GetHashString());
                }
            }
            if (_exclusionTables != null)
            {
                for (int i = 0; i < _exclusionTables.Count; i++)
                {
                    MyStringBuilder.Append(_exclusionTables[i].GetHashString());
                }
            }
            if (_initialContext != null)
            {
                for (int i = 0; i < _initialContext.Count; i++)
                {
                    MyStringBuilder.Append(_initialContext[i].getKey());
                    MyStringBuilder.Append(_initialContext[i].getValue());
                }
            }
            if (_tablePaths != null)
            {
                for (int i = 0; i < _tablePaths.Count; i++)
                {
                    MyStringBuilder.Append(_tablePaths[i].GetHashString());
                }
            }
            return MyStringBuilder.ToString();
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            ComputeHashCode();
        }


    }
}


