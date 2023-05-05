/*
 * Copyright (C) 2018-2022 Information Management Services, Inc.
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.EOD;

namespace TNMStagingCSharp.Src.Staging.Toronto
{
    public class TorontoSchemaLookup : SchemaLookup
    {
        private static readonly HashSet<String> _ALLOWED_KEYS = new HashSet<String>(new List<String>
                { StagingData.PRIMARY_SITE_KEY,
                  StagingData.HISTOLOGY_KEY,
                  TorontoInput.AGE_DX.toString() });


        //========================================================================================================================
        // Constructor
        // @param site primary site
        // @param histology histology
        //========================================================================================================================
        public TorontoSchemaLookup(String site, String histology) : base(site, histology)
        {
        }


        public override HashSet<String> getAllowedKeys()
        {
            return _ALLOWED_KEYS;
        }
    }


}
