using Sediin.PraticheRegionali.DOM.DbScript;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#pragma warning disable CS0105 // La direttiva using è già presente in questo spazio dei nomi
using Sediin.PraticheRegionali.DOM.DbScript;
#pragma warning restore CS0105 // La direttiva using è già presente in questo spazio dei nomi
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.PraticheRegionali.DOM.DbScript.Tests
{
    [TestClass()]
    public class DbProviderTests
    {
        [TestMethod()]
        public  void Mainest()
        {
            DbProvider p = new DbProvider();
             p.Main();
        }

        [TestMethod()]
        public  void BackupDatabaseTest()
        {
            DbProvider p = new DbProvider();
             p.BackupDatabase(@"c:\temp\bak").Wait();
        }

        [TestMethod()]
        public void RestoreDatabaseZipTest()
        {
            DbProvider p = new DbProvider();
            p.RestoreDatabaseZip(@"C:\temp\bak\EBLAC_638181202191429335.zip");
        }

    }
}