using System;

using TNMStagingCSharp.Src.DecisionEngine;



namespace TNMStaging_UnitTestApp.Src.DecisionEngine.Basic
{
    public class BasicKeyMapping : IKeyMapping
    {

        String _from;
        String _to;

        /**
         * Default constructor
         */
        public BasicKeyMapping()
        {
        }

        /**
         * Construct with from and to values
         * @param from low value
         * @param to high value
         */
        public BasicKeyMapping(String from, String to)
        {
            _from = from;
            _to = to;
        }

        public String getFrom()
        {
            return _from;
        }

        /**
         * Set the low value
         * @param from low value
         */
        public void setFrom(String from)
        {
            _from = from;
        }

        public String getTo()
        {
            return _to;
        }

        /**
         * Set the high value
         * @param to high value
         */
        public void setTo(String to)
        {
            _to = to;
        }
    }

}
