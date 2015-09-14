using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.IO;
using JsonProviders;

namespace JsonProvidersUnitTests
{
    public class BasicTestProvider
    {
        public string Test1Property { get; set; }
        public double Test2Property { get; set; }
        public long Test3Property { get; set; }
        public bool Test4Property { get; set; }
        public string[] Test5Property { get; set; }
    }

    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void BasicRead()
        {
            using (Stream configStream = GetResourceStream())
            {
                Providers.Configure(configStream);
            }

            var provider = Providers.GetProvider<BasicTestProvider>("BasicTestProvider");

            Assert.AreEqual("foo", provider.Test1Property);
            Assert.AreEqual(1.0, provider.Test2Property);
            Assert.AreEqual(1, provider.Test3Property);
            Assert.AreEqual(false, provider.Test4Property);
            Assert.AreEqual(2, provider.Test5Property.Length);
        }

        [TestMethod]
        public void ReadWithOverride()
        {
            using (Stream configStream = GetResourceStream())
            {
                Providers.Configure(configStream);
            }

            Providers.SetOverride("Override1");

            var provider = Providers.GetProvider<BasicTestProvider>("BasicTestProvider");

            Assert.AreEqual("foo", provider.Test1Property);
            Assert.AreEqual(1.0, provider.Test2Property);
            Assert.AreEqual(2, provider.Test3Property);
            Assert.AreEqual(true, provider.Test4Property);
            Assert.AreEqual(2, provider.Test5Property.Length);
        }


        private static Stream GetResourceStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("JsonProvidersUnitTests.{0}.{1}", "BasicTests", "providers.json"));
        }
    }
}
