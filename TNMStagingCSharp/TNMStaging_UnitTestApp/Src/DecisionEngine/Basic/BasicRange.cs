using System;
using System.Collections.Generic;

using TNMStagingCSharp.Src.DecisionEngine;


namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicRange : Range
    {

        private String _low;
        private String _high;
        private bool _usesContext;

        /**
         * Construct a BasicString range that matches any string
         */
        public BasicRange()
        {
            _usesContext = false;
        }

        /**
         * Construct a BasicStringRange with a low and high bound
         * @param low low value
         * @param high high value
         */
        public BasicRange(String low, String high)
        {
            if (low == null || high == null)
                throw new InvalidOperationException("Invalid range");
            if (low.Length != high.Length)
                throw new InvalidOperationException("Range strings must be the same length: '" + low + "-" + high + "'");
            if (low.CompareTo(high) > 0)
                throw new InvalidOperationException("Low value of range is greater than high value: '" + low + "-" + high + "'");

            _low = low;
            _high = high;

            _usesContext = ((_low.StartsWith("{{") && _low.EndsWith("}}")) || (_high.StartsWith("{{") && _high.EndsWith("}}")));
        }

        public override String getLow()
        {
            return _low;
        }

        public override String getHigh()
        {
            return _high;
        }

        /**
         * If low and high are both null, then this range matches all strings
         * @return true if range matches anything
         */
        private bool matchesAll()
        {
            return _low == null && _high == null;
        }

        public override bool contains(String value, Dictionary<String, String> context)
        {
            // make null values match the same as if they were blank
            if (value == null)
                value = "";

            if (_usesContext)
                return (matchesAll() || (DecisionEngineFuncs.translateValue(_low, context).CompareTo(value) <= 0 && DecisionEngineFuncs.translateValue(_high, context).CompareTo(value) >= 0));
            else
                return (matchesAll() || (_low.CompareTo(value) <= 0 && _high.CompareTo(value) >= 0));
        }

    }

}
