﻿using System.Configuration;

namespace AltMstest.Core.Configuration
{
    public class AssemblyInfo
    {
        private readonly string _assembly;
        private readonly bool _parallel;

        public AssemblyInfo(string assembly, bool parallel)
        {
            _assembly = assembly;
            _parallel = parallel;
        }

        public string Assembly
        {
            get { return _assembly; }
        }

        public bool Parallel
        {
            get { return _parallel; }
        }
    }
    public class AssemblyConfigElement : ConfigurationElement
    {
        public string Folder
        {
            get
            {
                int lastDash = Assembly.LastIndexOf('\\');
                return Assembly.Substring(0, lastDash);
            }
        }

        public string FileName
        {
            get
            {
                int lastDash = Assembly.LastIndexOf('\\');
                return Assembly.Substring(lastDash + 1, Assembly.Length - lastDash - 1);
            }
        }

        public AssemblyInfo Info
        {
            get { return new AssemblyInfo(Assembly, RunParallel); }
        }

        [ConfigurationProperty("Assembly", IsRequired = true, IsKey = false)]
        public string Assembly
        {
            get { return (string)this["Assembly"]; }
            set { this["Assembly"] = value; }
        }

        [ConfigurationProperty("Category", IsRequired = false, IsKey = false)]
        public string Category
        {
            get { return (string)this["Category"]; }
            set { this["Category"] = value; }
        }

        [ConfigurationProperty("RunParallel", IsRequired = false, IsKey = false, DefaultValue = true)]
        public bool RunParallel
        {
            get { return (bool)this["RunParallel"]; }
            set { this["RunParallel"] = value; }
        }

    }
}