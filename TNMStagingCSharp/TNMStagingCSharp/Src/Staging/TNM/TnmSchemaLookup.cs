// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TNMStagingCSharp.Src.Staging.Entities;

namespace TNMStagingCSharp.Src.Staging.TNM
{
    public class TnmSchemaLookup : SchemaLookup
    {

        private static readonly HashSet<String> _ALLOWED_KEYS = new HashSet<String>(new List<String>
                { TnmStagingData.PRIMARY_SITE_KEY, TnmStagingData.HISTOLOGY_KEY, TnmStagingData.SSF25_KEY, TnmStagingData.SEX_AT_BIRTH_KEY });


        // Constructor
        // @param site primary site
        // @param histology histology
        public TnmSchemaLookup(String site, String histology): base(site, histology)
        {
        }

        public override HashSet<String> getAllowedKeys()
        {
            return _ALLOWED_KEYS;
        }
    }
}


