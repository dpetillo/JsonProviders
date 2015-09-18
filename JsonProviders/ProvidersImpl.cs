using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonProviders
{

    internal class ProvidersImpl
    {
        private ConcurrentDictionary<string, object> _providers;
        private Dictionary<string, Type> _providerTypes;
        private Dictionary<string, JObject> _providerConfigs;

        private List<string> _parseErrors;
        private string _simpleOverride;

        public ProvidersImpl()
        {
            _providers = new ConcurrentDictionary<string, object>();
            _providerTypes = new Dictionary<string, Type>();
            _providerConfigs = new Dictionary<string, JObject>();
            _parseErrors = new List<string>();
            _simpleOverride = null;
        }

        public T GetProvider<T>(string name)
        {
            //check parameters
            if (!_providerTypes.ContainsKey(name) ||
                !_providerConfigs.ContainsKey(name))
            {
                throw new ArgumentException(string.Format("Provider name {0} is not configured", name));
            }

            if (!_providerTypes[name].IsAssignableFrom(typeof(T)))
            {
                throw new ArgumentException(string.Format("Provider type for {0} is not assignable from type {1}", name, typeof(T).Name));
            }

            return (T)_providers.GetOrAdd(name, BuildProvider<T>(name));

        }

        public IEnumerable<string> GetProviders<T>()
        {
            Type t = typeof(T);
            return _providerTypes.Where(o => o.Value != null && o.Value.IsAssignableFrom(t)).Select(o => o.Key).ToList();
        }


        public void SetOverride(string ov)
        {
            _simpleOverride = ov;
        }

        private T BuildProvider<T>(string name)
        {
            T provider = Activator.CreateInstance<T>();
            Type type = typeof(T);

            JObject overrideObj = null;
            if (!string.IsNullOrWhiteSpace(_simpleOverride))
            {
                var overridesProp = _providerConfigs[name].Property("overrides");
                var overrideObjProp = overridesProp != null ? (overridesProp.Value as JObject).Property(_simpleOverride) : null;
                if (overrideObjProp != null)
                {
                    overrideObj = overrideObjProp.Value as JObject;
                }
            }

            foreach (var prop in _providerConfigs[name].Properties())
            {
                var overridenProp = overrideObj != null ? overrideObj.Property(prop.Name) : null;
                if (overridenProp != null && TryApplyConfig(type, provider, overridenProp))
                {
                    continue;
                }

                TryApplyConfig(type, provider, prop);
            }

            return provider;
        }

        private bool TryApplyConfig(Type type, object o, JProperty prop)
        {
            if (prop.Name == "type")
            {
                return false;
            }

            var codeProp = type.GetProperty(prop.Name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (codeProp == null)
            {

                LogParseError("Property '{0}' doesn't exist on target type: {1}", prop.Name, type.Name);
                return false;
            }

            try
            {
                 codeProp.SetValue(o, prop.Value.ToObject(codeProp.PropertyType));
            }
            catch (Exception e)
            {
                LogParseError("Exception setting value: {0}", e.Message);

                return false;
            }
            return true;
        }

        //not thread-safe
        public void Parse(Stream fileStream)
        {
            JObject topObj = null;

            using (var tr = new StreamReader(fileStream))
            using (JsonTextReader reader = new JsonTextReader(tr))
            {
                topObj = (JObject)JToken.ReadFrom(reader);
            }

            foreach (var key in topObj.Properties())
            {
                //run through provider names
                try
                {
                    //ensure is an object associated to this key
                    JObject thisObj = key.Value as JObject;
                    if (thisObj == null || thisObj.Type != JTokenType.Object)
                    {
                        LogParseError("Expected json object");
                        continue;
                    }

                    JProperty typeProp = thisObj.Property("type");
                    if (typeProp == null)
                    {
                        LogParseError("Expected json property 'type'");
                        continue;
                    }

                    if (typeProp.Value.Type != JTokenType.String)
                    {
                        LogParseError("Expected json property 'type' to contain a string");
                        continue;
                    }
                    string typeName = typeProp.Value.Value<string>();
                    Type type = Type.GetType(typeName);
                    if (type == null)
                    {
                        LogParseError("Could not find type of name: {0}", typeName);
                    }

                    _providerConfigs.Add(key.Name, thisObj);
                    _providerTypes.Add(key.Name, type);
                }
                catch (Exception e)
                {
                    LogParseError("Exception parsing providers: {0}", e.Message);
                }
            }

        }

        private void LogParseError(string format, params object[] @params)
        {
            _parseErrors.Add(string.Format(format, @params));
        }


    }
}
