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
        public DateTime Test6Property { get; set; }
    }

    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void BasicRead()
        {
            Providers.ClearForUnitTesting();

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
            Assert.AreEqual(new DateTime(634708023435110000), provider.Test6Property);
        }

        [TestMethod]
        public void ReadWithOverride1()
        {
            Providers.ClearForUnitTesting();

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
            Assert.AreEqual(new DateTime(634708023435110000), provider.Test6Property);
        }

        [TestMethod]
        public void ReadWithOverride2()
        {
            Providers.ClearForUnitTesting();

            using (Stream configStream = GetResourceStream())
            {
                Providers.Configure(configStream);
            }

            Providers.SetOverride("Override2");

            var provider = Providers.GetProvider<BasicTestProvider>("BasicTestProvider");

            Assert.AreEqual("foo", provider.Test1Property);
            Assert.AreEqual(0.1, provider.Test2Property);
            Assert.AreEqual(1, provider.Test3Property);
            Assert.AreEqual(true, provider.Test4Property);
            Assert.AreEqual(1, provider.Test5Property.Length);
            Assert.AreEqual("test", provider.Test5Property[0]);
            Assert.AreEqual(new DateTime(634708023435110000), provider.Test6Property);
        }



        private static Stream GetResourceStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(string.Format("JsonProvidersUnitTests.{0}.{1}", "BasicTests", "providers.json"));
        }
    }
}
