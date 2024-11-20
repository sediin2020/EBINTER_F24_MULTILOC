using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sediin.PraticheRegionali.DOM.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.PraticheRegionali.DOM.DAL.Tests
{
    [TestClass()]
    public class PraticheAziendaUtilityTests
    {
        [TestMethod()]
        public void CalcolaImportoRimborsatoContributoDefaultTest()
        {
            var m = PraticheAziendaUtility.CalcolaImportoRimborsatoContributo(37, 300);
        }

        [TestMethod()]
        public void CalcolaImportoRimborsatoContributoTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void CalcolaRimborsoAziendaEnergiaTest()
        {
            UnitOfWork unitOfWork   = new UnitOfWork();
            var m = PraticheAziendaUtility.CalcolaRimborsoAziendaEnergia(100,50,100,150, unitOfWork.TipoRichiestaRepository.Get(x=>x.TipoRichiestaId== 52).FirstOrDefault());
        }
    }
}