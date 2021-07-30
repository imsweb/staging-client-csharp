/*
 * Copyright (C) 2021 Information Management Services, Inc.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public interface Metadata
    {
        //========================================================================================================================
        // Name of metadata item
        // @return String
        //========================================================================================================================
        String getName();

        //========================================================================================================================
        // Start year of metadata
        // @return Integer representing year
        //========================================================================================================================
        int getStart();

        //========================================================================================================================
        // End year of metadata
        // @return Integer representing year
        //========================================================================================================================
        int getEnd();
    }
}

