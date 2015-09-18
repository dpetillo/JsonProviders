using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace JsonProviders
{
    public static class Providers
    {
        private static ProvidersImpl _providersImpl = new ProvidersImpl();

        public static T GetProvider<T>(string name)
        {
            return _providersImpl.GetProvider<T>(name);
        }

        public static IEnumerable<string> GetProviders<T>()
        {
            return _providersImpl.GetProviders<T>();
        }

        public static void Configure(string fileName)
        {
            using (var fs = File.OpenRead(fileName))
            {
                _providersImpl.Parse(fs);
            }
        }

        public static void Configure(Stream configStream)
        {
            _providersImpl.Parse(configStream);
        }

        public static void SetOverride(string o)
        {
            _providersImpl.SetOverride(o);
        }

        public static void ClearForUnitTesting()
        {
            _providersImpl = new ProvidersImpl();
        }

    }

}
