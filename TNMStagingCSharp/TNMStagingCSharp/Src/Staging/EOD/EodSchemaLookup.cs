/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TNMStagingCSharp.Src.Staging.EOD
{

    public class EodSchemaLookup : SchemaLookup
    {
        private static readonly ReadOnlyCollection<String> _ALLOWED_KEYS = new ReadOnlyCollection<String>(new List<String>
                { EodStagingData.PRIMARY_SITE_KEY,
                  EodStagingData.HISTOLOGY_KEY,
                  EodInput.SEX.toString(),
                  EodInput.BEHAVIOR.toString(),
                  EodInput.DISCRIMINATOR_1.toString(),
                  EodInput.DISCRIMINATOR_2.toString() });


        //========================================================================================================================
        // Constructor
        // @param site primary site
        // @param histology histology
        //========================================================================================================================
        public EodSchemaLookup(String site, String histology) : base(site, histology)
        {
        }


        public override ReadOnlyCollection<String> getAllowedKeys()
        {
            return _ALLOWED_KEYS;
        }
    }


}
