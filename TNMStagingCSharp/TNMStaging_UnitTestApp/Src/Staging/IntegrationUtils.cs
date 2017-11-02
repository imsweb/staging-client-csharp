using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TNMStaging_UnitTestApp.Src.Staging
{
    public sealed class IntegrationUtils
    {
        public static TestContext TestReportLog;

        // * Private constructor
        private IntegrationUtils()
        {
        }

        public static void WritelineToLog(String s)
        {
            if (TestReportLog != null)
            {
                TestReportLog.WriteLine(s);
            }
            else
            {
                //System.Diagnostics.Trace.WriteLine(s);
                Console.WriteLine(s);
            }
        }


        // * Result object
        public class IntegrationResult
        {

            private long _numCases;
            private long _numFailures;

            public IntegrationResult(long numCases, long numFailures)
            {
                _numCases = numCases;
                _numFailures = numFailures;
            }

            public long getNumCases()
            {
                return _numCases;
            }

            public long getNumFailures()
            {
                return _numFailures;
            }
        }


        public static String convertInputMap(Dictionary<String, String> input)
        {
            List<String> inputValues = new List<String>();
            foreach (KeyValuePair<String, String> entry in input)
            {
                inputValues.Add("\"" + entry.Key + "\": \"" + entry.Value + "\"");
            }

            String sRetval = "";
            for (int i = 0; i < inputValues.Count; i++)
            {
                if (i > 0) sRetval += ", ";
                sRetval += inputValues[i];
            }

            return "{ " + sRetval + " }";
        }

        public static String GenerateTotalTimeString(Stopwatch thisStopwatch)
        {
            String sRetval = "";
            if (thisStopwatch.Elapsed.TotalHours >= 1)
            {
                sRetval = thisStopwatch.Elapsed.TotalHours.ToString() + " h";
            }
            else if (thisStopwatch.Elapsed.TotalMinutes >= 1)
            {
                sRetval = thisStopwatch.Elapsed.TotalMinutes.ToString() + " min";
            }
            else if (thisStopwatch.Elapsed.TotalSeconds >= 1)
            {
                sRetval = thisStopwatch.Elapsed.TotalSeconds.ToString() + " s";
            }
            else
            {
                sRetval = thisStopwatch.Elapsed.TotalMilliseconds.ToString() + " ms";
            }
            return sRetval;
        }

    }
}

