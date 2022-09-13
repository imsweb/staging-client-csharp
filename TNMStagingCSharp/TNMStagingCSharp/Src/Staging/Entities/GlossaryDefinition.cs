/*
 * Copyright (C) 2020 Information Management Services, Inc.
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class GlossaryDefinition
    {
        [JsonProperty("name", Order = 1)]
        private String _name;
        [JsonProperty("definition", Order = 2)]
        private String _definition;
        [JsonProperty("alternate_names", Order = 3)]
        private List<String> _alternateNames;
        [JsonProperty("last_modified", Order = 4)]
        private DateTime _lastModified;

        public GlossaryDefinition()
        {
        }

        public GlossaryDefinition(String name, String definition, List<String> alternateNames, DateTime lastModified)
        {
            _name = name;
            _definition = definition;
            _alternateNames = alternateNames;
            _lastModified = lastModified;
        }

        public String getName()
        {
            return _name;
        }

        public void setName(String name)
        {
            _name = name;
        }

        public String getDefinition()
        {
            return _definition;
        }

        public void setDefinition(String definition)
        {
            _definition = definition;
        }

        public List<String> getAlternateNames()
        {
            return _alternateNames;
        }

        public void setAlternateNames(List<String> alternateNames)
        {
            _alternateNames = alternateNames;
        }

        public DateTime getLastModified()
        {
            return _lastModified;
        }

        public void setLastModified(DateTime lastModified)
        {
            _lastModified = lastModified;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (o == null || GetType() != o.GetType())
                return false;

            GlossaryDefinition that = (GlossaryDefinition)o;
            return Equals(_name, that._name) &&
                   Equals(_definition, that._definition) &&
                   _alternateNames.SequenceEqual(that._alternateNames) &&
                   Equals(_lastModified, that._lastModified);
        }

        public override int GetHashCode()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");
            MyStringBuilder.Append(_name);
            MyStringBuilder.Append(_definition);
            MyStringBuilder.Append(_alternateNames);
            MyStringBuilder.Append(_lastModified);
            return MyStringBuilder.ToString().GetHashCode();
        }
    }
}


/*
package com.imsweb.staging.entities;

import java.util.Date;
import java.util.List;
import java.util.Objects;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonPropertyOrder;

*/
