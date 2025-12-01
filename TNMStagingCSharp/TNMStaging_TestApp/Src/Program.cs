using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TNMStaging_UnitTestApp.Src.Staging.TNM;
using TNMStaging_UnitTestApp.Src.Staging.CS;
using TNMStagingCSharp.Src.Tools;


namespace TNMStaging_TestApp.Src
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            CSBigIntegrationTest CSTestObj = new CSBigIntegrationTest();

            CSTestObj.testCSBigIntegrationTestGood();
            CSTestObj.testCSBigIntegrationTestBad();


            TnmBigIntegrationTest TNMTestObj = new TnmBigIntegrationTest();

            TNMTestObj.testTNMBigIntegrationTestGood();
            TNMTestObj.testTNMBigIntegrationTestBad();
            */
        }

        public static void WritelineToLog(String s)
        {
            //Trace.WriteLine(s);
            Console.WriteLine(s);
        }

    }
}
