using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;


namespace TNMStaging_UnitTestApp.Src.Staging
{
    public class MultiTasksExecutor
    {
        public delegate void ActionCallBack(int id, Object data);

        private bool mbShowLog = false;
        private int miNumTasks = -1;
        private int miMaxBagItemsForWait = -1;
        private int miSleepTimeForMainThread = -1;
        private int miSleepTimeForTaskThread = -1;

        private bool mbAllTasksFinished = false;
        private ConcurrentBag<Object> mBag = null;
        Task[] mTaskList = null;
        public ActionCallBack mActionCallback;
        private int miBagCount = 0;
        private int miNumStartedTasks = 0;
        private int miMinThreads = -1;

        public MultiTasksExecutor()
        {
            mbAllTasksFinished = false;
            mBag = null;
            mTaskList = null;
            mActionCallback = null;

            // Default Values
            mbShowLog = false;
            miNumTasks = Math.Min(9, Environment.ProcessorCount);
            miMinThreads = miNumTasks;
            miMaxBagItemsForWait = 10000;
            miSleepTimeForMainThread = 10;
            miSleepTimeForTaskThread = 0;
        }

        public void SetShowLog(bool b) { mbShowLog = b; }
        public void SetNumTasks(int iNum) { miNumTasks = iNum; }
        public int GetNumThreads() { return miNumTasks; }
        public int GetMaxBagItemsForWait() { return miMaxBagItemsForWait; }
        public void SetMaxBagItemsForWait(int iMax) { miMaxBagItemsForWait = iMax; }
        public void SetSleepTimeForMainThread(int iTime) { miSleepTimeForMainThread = iTime; }
        public void SetSleepTimeForTaskThread(int iTime) { miSleepTimeForTaskThread = iTime; }
        public int GetNumStartedTasks() { return miNumStartedTasks; }
        public void SetMinThreads(int iNum) { miMinThreads = iNum; }

        public void AddAction(ActionCallBack func)
        {
            Log("MultiTasksExecutor::AddAction");
            mActionCallback = func;

            mbAllTasksFinished = false;

            mBag = new ConcurrentBag<Object>();
            miBagCount = 0;
        }

        public void StartTasks()
        {
            Log("MultiTasksExecutor::StartTasks");


            ThreadPool.SetMinThreads(miMinThreads, miMinThreads);

            mTaskList = new Task[miNumTasks];
            for (int i = 0; i < miNumTasks; i++)
            {
                int iId = i;

                mTaskList[i] = new Task(delegate () { TaskToExecute(iId); }, TaskCreationOptions.LongRunning);
                mTaskList[i].Start();
                miNumStartedTasks++;

            }
        }

        public void AddDataItem(Object d)
        {
            Log("MultiTasksExecutor::AddDataItem");

            mBag.Add(d);
            Interlocked.Increment(ref miBagCount);

            while (miBagCount > miMaxBagItemsForWait)
            {
                Thread.Sleep(miSleepTimeForMainThread);
            }
        }

        public bool WaitForCompletion()
        {
            Log("MultiTasksExecutor::WaitForCompletion");
            while (miBagCount > 0)
            {
                Thread.Sleep(miSleepTimeForMainThread);
            }
            mbAllTasksFinished = true;

            try
            {
                Task.WaitAll(mTaskList);
            }
            catch (AggregateException e)
            {
                Log("The following exceptions have been thrown by WaitAll(): ");
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    Log("-------------------------------------------------");
                    Log(e.InnerExceptions[j].ToString());
                }
            }


            Log("   All Tasks Finished");

            mTaskList = null;
            mBag = null;
            return true;
        }

        private void TaskToExecute(int id)
        {
            while (!mbAllTasksFinished)
            {
                if (miBagCount > 0)
                {
                    Object data;
                    if (mBag.TryTake(out data))
                    {
                        mActionCallback(id, data);
                        Interlocked.Decrement(ref miBagCount);
                    }
                    else
                    {
                    }
                }
                else
                {
                    Thread.Sleep(miSleepTimeForTaskThread);
                }
            }
        }

        public void Log(String s)
        {
            if (mbShowLog) Trace.WriteLine(s);
        }
    }
}
