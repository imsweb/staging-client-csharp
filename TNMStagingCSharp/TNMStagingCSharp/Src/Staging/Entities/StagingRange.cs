// Copyright (C) 2017 Information Management Services, Inc.

using System;
using System.Collections.Generic;

namespace TNMStagingCSharp.Src.Staging.Entities
{
    public class StagingRange : DecisionEngine.Range
    {
        private String _low;
        private String _high;

        private bool bLowIsRefContextValue;
        private bool bHighIsRefContextValue;

        // Construct a string range that matches any string
        public StagingRange()
        {

        }

        // Construct a string range with a low and high bound
        // @param low low value
        // @param high high value
        public StagingRange(String low, String high)
        {
            if (low == null || high == null)
                throw new System.InvalidOperationException("Invalid range");

            _low = low;
            _high = high;

            bLowIsRefContextValue = DecisionEngine.DecisionEngineFuncs.isReferenceVariable(low);
            bHighIsRefContextValue = DecisionEngine.DecisionEngineFuncs.isReferenceVariable(high);
        }

        // Return true if the string can converted into a number
        public static bool isNumeric(String value)
        {
            //return NumberUtils.isParsable(value);
            float number;
            return (float.TryParse(value, out number));
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


        // Returns true if the value is contained in the range.  Note that the low and high values will be replaced with context if
        // they are specified that way.  There are two ways in which the values are compared.
        // 1. If the low and high values are different and are both "numeric", then the value will be compared using floats.
        // 2. Otherwise it will be compared using String but the strings must be the same length othersize the method will always return false.
        public override bool contains(String value, Dictionary<String, String> context)
        {
            if (matchesAll())
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

            // if input, low and high values represent decimal numbers then do a float comparison

            if (!low.Equals(high))
            {
                float fConverted, fLow, fHigh;
                bool bValueConv = float.TryParse(value, out fConverted);
                bool bLowConv = float.TryParse(low, out fLow);
                bool bHighConv = float.TryParse(high, out fHigh);

                if ((bLowConv) && (bHighConv))
                {
                    if (!bValueConv) return false;

                    return (fConverted >= fLow) && (fConverted <= fHigh);
                }

            }


            /*
            if (!low.Equals(high) && isNumeric(low) && isNumeric(high))
            {
                if (!isNumeric(value)) return false;

                float fConverted, fLow, fHigh;
                float.TryParse(value, out fConverted);
                float.TryParse(low, out fLow);
                float.TryParse(high, out fHigh);

                return (fConverted >= fLow) && (fConverted <= fHigh);

                //float converted = NumberUtils.createFloat(value);
                //return (converted >= NumberUtils.createFloat(low)) && (converted <= NumberUtils.createFloat(high));
            }
            */

            // if the context value(s) failed or the low and high values are different length, return false
            if (low.Length != high.Length || low.Length != value.Length)
                return false;

            // compare value to low and high
            return String.CompareOrdinal(low, value) <= 0 && String.CompareOrdinal(high, value) >= 0;
        }

    }
}
