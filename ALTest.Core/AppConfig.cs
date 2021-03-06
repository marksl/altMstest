﻿using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ALTest.Core
{
    public abstract class AppConfig : IDisposable
    {
        public static AppConfig Change(string configFile, string directory)
        {
            return new ChangeAppConfig(configFile, directory);
        }

        public abstract void Dispose();

        private class ChangeAppConfig : AppConfig
        {
            private readonly string oldConfig =
                AppDomain.CurrentDomain.GetData("APP_CONFIG_FILE").ToString();

            private bool disposedValue;

            private readonly string _oldDirectory;
            private readonly string _configFile;

            public ChangeAppConfig(string configFile, string directory)
            {
                _configFile = configFile;
                _oldDirectory = Directory.GetCurrentDirectory();

                Directory.SetCurrentDirectory(directory);
                if (configFile != null)
                {
                    AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFile);
                    ResetConfigMechanism();
                }
            }

            public override void Dispose()
            {
                if (!disposedValue)
                {
                    if (_configFile != null)
                    {
                        AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", oldConfig);
                        ResetConfigMechanism();
                    }

                    Directory.SetCurrentDirectory(_oldDirectory);

                    disposedValue = true;
                }
                GC.SuppressFinalize(this);
            }

            private static void ResetConfigMechanism()
            {
// ReSharper disable PossibleNullReferenceException
                typeof (ConfigurationManager)
                    .GetField("s_initState", BindingFlags.NonPublic |
                                             BindingFlags.Static)
                    .SetValue(null, 0);

                typeof (ConfigurationManager)
                    .GetField("s_configSystem", BindingFlags.NonPublic |
                                                BindingFlags.Static)
                    .SetValue(null, null);

                typeof (ConfigurationManager)
                    .Assembly.GetTypes().First(x => x.FullName ==
                                                    "System.Configuration.ClientConfigPaths")
                    .GetField("s_current", BindingFlags.NonPublic |
                                           BindingFlags.Static)
                    .SetValue(null, null);
                // ReSharper restore PossibleNullReferenceException
            }
        }
    }
}