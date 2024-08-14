/*
 * Copyright (C) 2018 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TNMStagingCSharp.Src.Staging.Entities;

namespace TNMStagingCSharp.Src.Staging.EOD
{
    public class EodSchemaLookup : SchemaLookup
    {
        private static readonly HashSet<String> _ALLOWED_KEYS = new HashSet<String>(new List<String>
                { StagingData.PRIMARY_SITE_KEY,
                  StagingData.HISTOLOGY_KEY,
                  EodInput.SEX.toString(),
                  EodInput.BEHAVIOR.toString(),
                  EodInput.YEAR_DX.toString(),
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


        public override HashSet<String> getAllowedKeys()
        {
            return _ALLOWED_KEYS;
        }
    }


}
