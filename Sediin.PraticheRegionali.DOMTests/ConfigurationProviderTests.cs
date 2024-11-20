using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sediin.PraticheRegionali.DOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sediin.PraticheRegionali.Utils;

namespace Sediin.PraticheRegionali.DOM.Tests
{
    [TestClass()]
    public class ConfigurationProviderTests
    {
        [TestMethod()]
        public void SaveConfiguration()
        {
            ConfigurationProvider p = new ConfigurationProvider();
            p.SaveConfiguration();
        }
    }
}