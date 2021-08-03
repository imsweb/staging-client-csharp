// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;


namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class SchemaLookup
    {
        private Dictionary<String, String> _inputs = new Dictionary<String, String>(100, StringComparer.Ordinal);
        private String msHashCode = "";

        // Default constructor
        public SchemaLookup()
        {
        }

        // Constructor
        // @param inputs a Map of key-value pairs
        public SchemaLookup(Dictionary<String, String> inputs)
        {
            _inputs = inputs;
            CreateHash();
        }

        // Constructor
        // @param site primary site
        // @param histology histology
        public SchemaLookup(String site, String histology)
        {
            if (site != null)
                if (site.Length > 0)
                    setInput(StagingData.PRIMARY_SITE_KEY, site);
            if (histology != null)
                if (histology.Length > 0)
                    setInput(StagingData.HISTOLOGY_KEY, histology);
            CreateHash();
        }

        // Returns a list of allowable keys.  If null, all keys are allowed
        // @return a set of keys
        public virtual ReadOnlyCollection<String> getAllowedKeys()
        {
            return null;
        }

        // Return a list of keys that are set.
        // @return a set of keys
        public HashSet<String> getKeys()
        {
            return new HashSet<String>(_inputs.Keys);
        }

        // Return a Map of all inputs
        // @return a Map of all inputs
        public Dictionary<String, String> getInputs()
        {
            return _inputs;
        }

        // Get the value of a single input
        // @param key key of input
        // @return value of input
        public String getInput(String key)
        {
            String sRetval = "";
            _inputs.TryGetValue(key, out sRetval);

            return sRetval;
        }

        // Set the value of a single input.
        // @param key key of input
        // @param value value of input
        public void setInput(String key, String value)
        {
            if (getAllowedKeys() != null && !getAllowedKeys().Contains(key))
                throw new System.InvalidOperationException("The input key " + key + " is not allowed for lookups");

            _inputs[key] = value;
            CreateHash();
        }

        // Return all elements from the input
        protected void clearInputs()
        {
            _inputs.Clear();
            CreateHash();
        }

        // Get primary site
        // @return primary site
        public String getSite()
        {
            String sRetval = "";
            _inputs.TryGetValue(StagingData.PRIMARY_SITE_KEY, out sRetval);

            return sRetval;
        }

        // Set primary site
        // @param site primary site
        public void setSite(String site)
        {
            _inputs[StagingData.PRIMARY_SITE_KEY] = site;
            CreateHash();
        }

        // Get histology
        // @return histology
        public String getHistology()
        {
            String sRetval = "";
            _inputs.TryGetValue(StagingData.HISTOLOGY_KEY, out sRetval);

            return sRetval;
        }

        // Set histology
        // @param hist histology
        public void setHistology(String hist)
        {
            _inputs[StagingData.HISTOLOGY_KEY] = hist;
            CreateHash();
        }

        // Return true if the inputs contain a discriminator.  A key that is not site or hist which has a non-null/non-empty value is considered a discriminator
        // @return true or false indicating whether a discriminator exists
        public bool hasDiscriminator()
        {
            bool hasDiscriminator = false;

            foreach (KeyValuePair<String, String> entry in _inputs)
            {
                String key = entry.Key;

                if (StagingData.STANDARD_LOOKUP_KEYS.Contains(key))
                    continue;

                String value = entry.Value;
                if (value != null && !(value.Length == 0))
                {
                    hasDiscriminator = true;
                    break;
                }
            }

            return hasDiscriminator;
        }

        public void CreateHash()
        {
            StringBuilder MyStringBuilder = new StringBuilder("");

            if (_inputs != null)
            {
                foreach (KeyValuePair<String, String> entry in _inputs)
                {
                    MyStringBuilder.Append(entry.Key);
                    MyStringBuilder.Append(entry.Value);
                }
            }

            //miHashCode = MyStringBuilder.ToString().GetHashCode();
            msHashCode = MyStringBuilder.ToString();
        }

        public String GetHashString()
        {
            return msHashCode;
        }


    }
}

