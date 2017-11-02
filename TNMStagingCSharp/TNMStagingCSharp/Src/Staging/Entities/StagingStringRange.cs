// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingStringRange : DecisionEngine.StringRange
    {
        private String _low;
        private String _high;

        private bool bLowIsRefContextValue;
        private bool bHighIsRefContextValue;

        // Construct a string range that matches any string
        public StagingStringRange()
        {
        }

        // Construct a string range with a low and high bound
        // @param low low value
        // @param high high value
        public StagingStringRange(String low, String high)
        {
            if (low == null || high == null)
                throw new System.InvalidOperationException("Invalid range");

            _low = low;
            _high = high;

            bLowIsRefContextValue = DecisionEngine.DecisionEngineFuncs.isReferenceVariable(low);
            bHighIsRefContextValue = DecisionEngine.DecisionEngineFuncs.isReferenceVariable(high);
        }

        public override String getLow()
        {
            return _low;
        }

        public override String getHigh()
        {
            return _high;
        }

        // If low and high are both null, then this range matches all strings
        // @return true if this range matches all strings
        public bool matchesAll()
        {
            return _low == null && _high == null;
        }

        public override bool contains(String value, Dictionary<String, String> context)
        {
            if (_low == null && _high == null)
                return true;

            // make null values match the same as if they were blank
            if (value == null)
                value = "";

            // translate the context values if they are there
            String low = _low;
            if (bLowIsRefContextValue)
                low = DecisionEngine.DecisionEngineFuncs.translateValue(_low, context);
            String high = _high;
            if (bHighIsRefContextValue)
                high = DecisionEngine.DecisionEngineFuncs.translateValue(_high, context);

            // if the context value(s) failed or the low and high values are different length, return false
            if (low.Length != high.Length || low.Length != value.Length)
                return false;

            // compare value to low and high
            return String.CompareOrdinal(low, value) <= 0 && String.CompareOrdinal(high, value) >= 0;
        }

    }
}
