/*
 * Copyright (C) 2018-2022 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.EOD;

namespace TNMStagingCSharp.Src.Staging.Pediatric
{
    public class PediatricSchemaLookup : SchemaLookup
    {
        private static readonly HashSet<String> _ALLOWED_KEYS = new HashSet<String>(new List<String>
                { StagingData.PRIMARY_SITE_KEY,
                  StagingData.HISTOLOGY_KEY,
                  PediatricInput.AGE_DX.toString(),
                  PediatricInput.BEHAVIOR.toString()});

        //========================================================================================================================
        // Constructor
        // @param site primary site
        // @param histology histology
        //========================================================================================================================
        public PediatricSchemaLookup(String site, String histology) : base(site, histology)
        {
        }

        public PediatricSchemaLookup(String site, String histology, String ageDx, String behavior) : base(site, histology)
        {
            setInput(PediatricInput.AGE_DX.toString(), ageDx);
            setInput(PediatricInput.BEHAVIOR.toString(), behavior);
        }

        public override HashSet<String> getAllowedKeys()
        {
            return _ALLOWED_KEYS;
        }
    }


}
