// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TNMStagingCSharp.Src.Staging.Entities;

namespace TNMStagingCSharp.Src.Staging.CS
{
    public class CsSchemaLookup: SchemaLookup
    {
        private static readonly ReadOnlyCollection<String> _ALLOWED_KEYS = new ReadOnlyCollection<String>(new List<String>
                { CsStagingData.PRIMARY_SITE_KEY, CsStagingData.HISTOLOGY_KEY, CsStagingData.SSF25_KEY });


        //========================================================================================================================
        // Constructor
        // @param site primary site
        // @param histology histology
        //========================================================================================================================
        public CsSchemaLookup(String site, String histology): base(site, histology)
        {
        }

        //========================================================================================================================
        // Constructor
        // @param site primary site
        // @param histology histology
        // @param discriminator ssf25
        //========================================================================================================================
        public CsSchemaLookup(String site, String histology, String discriminator): base(site, histology)
        {
            setInput(CsStagingData.SSF25_KEY, discriminator);
        }

        public override ReadOnlyCollection<String> getAllowedKeys()
        {
            return _ALLOWED_KEYS;
        }
    }
}


