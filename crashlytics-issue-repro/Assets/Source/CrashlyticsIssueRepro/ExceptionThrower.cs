using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace CrashlyticsIssueRepro
{
    public class ExceptionThrower
    {
        /// <summary>
        ///     Fabricates a deep stack and then throws
        /// </summary>
        public void ThrowStack()
        {
            EnlargeStacktraceAndFinallyThrow(50);
        }

        /// <summary>
        ///     Fabricates a deep async callchain and then throws
        /// </summary>
        public async Task ThrowAsync()
        {
            await EnlargeAsyncStacktraceAndFinallyThrow(50);
        }

        public async Task ThrowAsyncTryCatch()
        {
            try {
                await EnlargeAsyncStacktraceAndFinallyThrow(50);
            } catch (Exception ex) {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        ///     Starts many threads and then throws, to enlarge the footprint of the log data sent to Crashlytics.
        /// </summary>
        public void ThrowManyThreads()
        {
            StartThreadsAndFinallyThrow(50);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void EnlargeStacktraceAndFinallyThrow(int count)
        {
            if (count > 0)
                // ReSharper disable once TailRecursiveCall
                EnlargeStacktraceAndFinallyThrow(count - 1);
            else
                throw new Exception("This is a test exception.");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static async Task EnlargeAsyncStacktraceAndFinallyThrow(int count)
        {
            if (count > 0)
                // ReSharper disable once TailRecursiveCall
                await EnlargeAsyncStacktraceAndFinallyThrow(count - 1);
            else
                throw new Exception("This is a test exception.");
        }

        private static void StartThreadsAndFinallyThrow(int count)
        {
            void DoWork()
            {
                Thread.Sleep(1000);
            }

            for (var i = 0; i < count; i++) {
                Thread thread = new(DoWork);
                thread.Start();
            }

            throw new Exception("This is a test exception.");
        }
    }
}