using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sediin.PraticheRegionali.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.PraticheRegionali.Utils.Tests
{
    [TestClass()]
    public class ConfigurationProviderTests
    {
        [TestMethod()]
        public void SaveConfigurationTest()
        {
            ConfigurationProvider p= new ConfigurationProvider();
            p.SaveConfiguration();
        }
    }
}