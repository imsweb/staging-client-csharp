using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using TNMStaging_UnitTestApp.Src.Staging;
using TNMStagingCSharp.Src.Staging;
using TNMStagingCSharp.Src.Staging.Entities;
using TNMStagingCSharp.Src.Staging.TNM;
using TNMStagingCSharp.Src.DecisionEngine;

namespace TNMStaging_UnitTestApp.Src.Staging.TNM
{
    class TnmIntegrationSchemaStage
    {

        // * Process all schemas in the TNM file
        // * @param staging Staging object
        // * @param fileName name of file
        // * @param is InputStream
        // * @return IntegrationResult
        public static IntegrationUtils.IntegrationResult processTNMSchema(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream) //throws IOException, InterruptedException 
        {
            return processTNMSchema(staging, fileName, inputStream, -1);
        }


        public class MultiTask_DataObj
        {
            public Dictionary<TnmInput, String> mInputValues;
            public Dictionary<TnmOutput, String> mOutputValues;
            public String msFileName;
            public int miLineNum;
        }

        public static TNMStagingCSharp.Src.Staging.Staging mMultiTask_Staging;
        public static int miMultiTask_FailedCases = 0;
        public static int miMultiTask_ThreadProcessedCases = 0;


        // * Process all schemas in TNM file
        // * @param staging Staging object
        // * @param fileName name of file
        // * @param is InputStream
        // * @param singleLineNumber if not null, only process this line number
        // * @return IntegrationResult
        public static IntegrationUtils.IntegrationResult processTNMSchema(TNMStagingCSharp.Src.Staging.Staging staging, String fileName, Stream inputStream, int singleLineNumber) //throws IOException, InterruptedException 
        {
            Dictionary<TnmOutput, String> output_values = null;
            Dictionary<TnmInput, String> input_values = null;

            // initialize the threads pool
            int n = Math.Min(9, Environment.ProcessorCount + 1);
            ThreadPool.SetMaxThreads(n, n);

            // go over each file
            int processedCases = 0;
            int iLineNumber = 0;
            int iLineCounter = 0;

            MultiTasksExecutor thisMultiTasksExecutor = new MultiTasksExecutor();
            thisMultiTasksExecutor.AddAction(new MultiTasksExecutor.ActionCallBack(MultiTask_TaskCompute));

            int iThreads = thisMultiTasksExecutor.GetNumThreads();

            mMultiTask_Staging = staging;
            miMultiTask_FailedCases = 0;
            miMultiTask_ThreadProcessedCases = 0;
            thisMultiTasksExecutor.StartTasks();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            if (singleLineNumber >= 0)
                System.Diagnostics.Trace.WriteLine("Starting " + fileName + ", line # " + singleLineNumber + " [" + n + " threads]");
            else
                System.Diagnostics.Trace.WriteLine("Starting " + fileName + " [" + (iThreads + 1) + " threads]");

            // loop over each line in the file
            TextReader reader = new StreamReader(inputStream);
            String line = reader.ReadLine();
            String input_line = "";
            String output_line = "";
            String[] input_strs;
            String[] output_strs;
            String[] entries;

            while (line != null)
            {
                iLineNumber++;

                if (iLineNumber >= 0)
                {
                    if (line.IndexOf("input=") >= 0) input_line = line.Trim();
                    if (line.IndexOf("expectedOutput=") >= 0) output_line = line.Trim();

                    if (output_line.Length > 0)
                    {
                        input_line = input_line.Substring(7, input_line.Length - 8).Trim();
                        output_line = output_line.Substring(16, output_line.Length - 17).Trim();

                        input_strs = input_line.Split(",".ToCharArray());
                        output_strs = output_line.Split(",".ToCharArray());

                        // set up a mapping of output field positions in the CSV file
                        output_values = new Dictionary<TnmOutput, String>();
                        input_values = new Dictionary<TnmInput, String>();

                        foreach (String s in input_strs)
                        {
                            entries = s.Split("=".ToCharArray());
                            if (entries.Length == 2)
                            {
                                entries[0] = entries[0].Trim();
                                entries[1] = entries[1].Trim();
                                foreach (TnmInput inp in TnmInput.Values)
                                {
                                    if (inp.toString() == entries[0])
                                    {
                                        input_values.Add(inp, entries[1]);
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine("Line " + iLineNumber + " has " + entries.Length + " cells; it should be 2. (" + input_line + ")");
                            }
                        }

                        foreach (String s in output_strs)
                        {
                            entries = s.Split("=".ToCharArray());
                            if (entries.Length == 2)
                            {
                                entries[0] = entries[0].Trim();
                                entries[1] = entries[1].Trim();
                                foreach (TnmOutput outp in TnmOutput.Values)
                                {
                                    if (outp.toString() == entries[0])
                                    {
                                        output_values.Add(outp, entries[1]);
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine("Line " + iLineNumber + " has " + entries.Length + " cells; it should be 2. (" + output_line + ")");
                            }
                        }

                        processedCases++;

                        MultiTask_DataObj obj = new MultiTask_DataObj();
                        obj.mInputValues = input_values;
                        obj.mOutputValues = output_values;
                        obj.msFileName = fileName;
                        obj.miLineNum = iLineNumber;

                        thisMultiTasksExecutor.AddDataItem(obj);

                        /*
                        iLineCounter++;
                        if (iLineCounter > 50000)
                        {
                            IntegrationUtils.WritelineToLog("Time: " + stopwatch.Elapsed.TotalMilliseconds + " ms.");
                            iLineCounter = 0;
                        }
                        */

                        input_line = "";
                        output_line = "";
                    }

                }

                line = reader.ReadLine();
            }

            thisMultiTasksExecutor.WaitForCompletion();

            stopwatch.Stop();
            String perMs = String.Format("{0,12:F4}", ((float)stopwatch.Elapsed.TotalMilliseconds / processedCases)).Trim();
            IntegrationUtils.WritelineToLog("Completed " + processedCases + " cases for " + fileName + " in " + TNMStaging_UnitTestApp.Src.Staging.IntegrationUtils.GenerateTotalTimeString(stopwatch) + " (" + perMs + " ms/case).");
            IntegrationUtils.WritelineToLog("Threads Completed " + miMultiTask_ThreadProcessedCases + " cases.");
            if (miMultiTask_FailedCases > 0)
                System.Diagnostics.Trace.WriteLine("There were " + miMultiTask_FailedCases + " failures.");
            else
                System.Diagnostics.Trace.WriteLine("");


            System.Diagnostics.Trace.WriteLine("-----------------------------------------------");

            inputStream.Close();

            return new IntegrationUtils.IntegrationResult(processedCases, miMultiTask_FailedCases);
        }

        public static void MultiTask_TaskCompute(int id, Object task_data)
        {
            MultiTask_DataObj thisDataObj = (MultiTask_DataObj)task_data;

            try
            {
                // load up inputs
                TnmStagingData data = new TnmStagingData();
                foreach (KeyValuePair<TnmInput, String> kp in thisDataObj.mInputValues)
                {
                    data.setInput(kp.Key, kp.Value);
                }
                
                // save the expected outputs
                Dictionary<String, String> output = new Dictionary<String, String>(100, StringComparer.Ordinal);
                String sKeyValue = "";
                foreach (KeyValuePair<TnmOutput, String> entry in thisDataObj.mOutputValues)
                {
                    sKeyValue = entry.Key.toString();
                    output[sKeyValue] = entry.Value;
                }


                // run collaborative stage; if no schema found, set the output to empty
                SchemaLookup lookup = new SchemaLookup(data.getInput(TnmInput.PRIMARY_SITE), data.getInput(TnmInput.HISTOLOGY));
                lookup.setInput(TnmStagingData.SEX_KEY, data.getInput(TnmInput.SEX));
                lookup.setInput(TnmStagingData.SSF25_KEY, data.getInput(TnmInput.SSF25));
                List<StagingSchema> schemas = mMultiTask_Staging.lookupSchema(lookup);

                if (schemas.Count == 1)
                    mMultiTask_Staging.stage(data);
                else
                {
                    Dictionary<String, String> outResult = new Dictionary<String, String>(2, StringComparer.Ordinal);
                    outResult["schema_id"] = "<invalid>";

                    data.setOutput(outResult);
                }

                List<String> mismatches = new List<String>();

                // compare results
                foreach (KeyValuePair<String, String> entry in output)
                {
                    String expected = "";
                    output.TryGetValue(entry.Key, out expected);
                    if (expected == null) expected = "";
                    expected = expected.Trim();

                    String actual = "";
                    data.getOutput().TryGetValue(entry.Key, out actual);
                    if (actual == null) actual = "";
                    actual = actual.Trim();

                    if (expected != actual)
                        mismatches.Add("   " + thisDataObj.miLineNum + " --> " + entry.Key + ": EXPECTED '" + expected + "' ACTUAL: '" + actual + "'");
                }

                if (mismatches.Count != 0)
                {
                    String sSchemaIDValue = data.getSchemaId();


                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> [" + sSchemaIDValue + "] Mismatches in " + thisDataObj.msFileName);
                    foreach (String mismatch in mismatches)
                    {
                        IntegrationUtils.WritelineToLog(mismatch);
                    }
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " *** RESULT: " + data.getResult());
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> Input: " + IntegrationUtils.convertInputMap(data.getInput()));
                    IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> Output: " + IntegrationUtils.convertInputMap(data.getOutput()));
                    if (data.getErrors().Count > 0)
                    {
                        IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> ERRORS: ");
                        foreach (Error e in data.getErrors())
                            IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> (" + e.getTable() + ": " + e.getMessage() + ") ");
                    }

                    Interlocked.Increment(ref miMultiTask_FailedCases);
                }
            }
            catch (Exception e)
            {
                IntegrationUtils.WritelineToLog("   " + thisDataObj.miLineNum + " --> Exception processing " + thisDataObj.msFileName + " : " + e.Message);
                Interlocked.Increment(ref miMultiTask_FailedCases);
            }

            Interlocked.Increment(ref miMultiTask_ThreadProcessedCases);
        }


    }

}