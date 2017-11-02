// Copyright (C) 2017 Information Management Services, Inc.

using System;
using Newtonsoft.Json;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingCode
    {
        [JsonProperty("table", Order = 1)]
        protected String _table;
        [JsonProperty("code", Order = 2)]
        protected String _code;
        [JsonProperty("is_valid", Order = 3)]
        protected bool _isValid;

        public StagingCode()
        {
            setIsValid(false);
        }

        // @param table Table identifier
        // @param code Code
        // @param isValid Boolean indicating whether code is value
        public StagingCode(String table, String code, bool isValid)
        {
            setTable(table);
            setCode(code);
            setIsValid(isValid);
        }

        public String getTable()
        {
            return _table;
        }

        public void setTable(String table)
        {
            _table = table;
        }

        public String getCode()
        {
            return _code;
        }

        public void setCode(String code)
        {
            _code = code;
        }

        public bool getIsValid()
        {
            return _isValid;
        }

        public void setIsValid(bool isValid)
        {
            _isValid = isValid;
        }
    }
}

