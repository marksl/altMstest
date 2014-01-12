﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace AltMstest.Core
{
    public class TestRun
    {
        public TestRun()
        {
            Classes = new List<ClassTestRun>();
            AssemblyInitialize = new List<MethodInfo>();
            AssemblyCleanup = new List<MethodInfo>();
        }

        public ClassTestRun AddClassTestRun(Type classType)
        {
            var run = new ClassTestRun(classType);
            Classes.Add(run);

            return run;
        }

        public List<TestResult> Run(bool parallel, int? degreeOfParallelism, CancellationToken ct)
        {
            var results = new List<TestResult>();
           
            using (AppConfig.Change(_configFile))
            {
                // Assembly initialize
                foreach (var assemblyInit in AssemblyInitialize)
                {
                    assemblyInit.Invoke(null, new object[] {new MyTestContext()});
                }

                if (parallel)
                {
                    var l = new object();
                    
                    ParallelQuery<ClassTestRun> classTestRuns = Classes.AsParallel();
                    if (degreeOfParallelism.HasValue)
                    {
                        classTestRuns = classTestRuns.WithDegreeOfParallelism(degreeOfParallelism.Value);
                    }

                    foreach (List<TestResult> classResults in classTestRuns.Select(c => c.Run(ct)))
                    {
                        lock (l)
                        {
                            results.AddRange(classResults);
                        }
                    }
                }
                else
                {
                    foreach (List<TestResult> classResults in Classes.Select(c => c.Run(ct)))
                    {
                        results.AddRange(classResults);
                    }
                }

                // Assembly cleanup
                foreach (var assemblyInit in AssemblyCleanup)
                {
                    assemblyInit.Invoke(null, null);
                }
            }

            return results;
        }

        // Apparently only one Assembly Initialize is supported... TODO: Test if you can have more than 1.
        // MsTest only supports 1 assembly initalize... other test runners might support more, so i'll support more.
        public List<MethodInfo> AssemblyInitialize { get; set; }
        public List<MethodInfo> AssemblyCleanup { get; set; }

        public List<ClassTestRun> Classes { get; private set; }

        private string _configFile;
        public void SetConfigFile(string configFilePath)
        {
            _configFile = configFilePath;
        }
    }
}