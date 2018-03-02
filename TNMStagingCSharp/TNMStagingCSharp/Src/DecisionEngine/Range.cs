// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;


namespace TNMStagingCSharp.Src.DecisionEngine
{
    public abstract class Range
    {
        //========================================================================================================================
        // Returns true of the passed value is contained in the range
        // @param value a value to test for
        // @param context a context which is used if the range contains a variable, designated by {{variable}}
        // @return true if the value is in the range
        //========================================================================================================================
        public abstract bool contains(String value, Dictionary<String, String> context);

        public abstract String getLow();

        public abstract String getHigh();
    }
}


