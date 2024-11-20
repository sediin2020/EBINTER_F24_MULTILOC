using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sediin.PraticheRegionali.DOM.DAL;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers.Tests
{
    [TestClass()]
    public class PraticheControllerTests
    {
        [TestMethod()]
        public void VerificaMaxRichiesteAziendaTest()
        {
            UnitOfWork unitOfWork = new UnitOfWork();

            PraticheController p = new PraticheController();

            var _tiporichiesta = unitOfWork.TipoRichiestaRepository.Get().OrderBy(o => o.Descrizione).ToList().FirstOrDefault(xx => xx.AbilitatoNuovaRichiesta == true && xx.TipoRichiestaId == 1);

            p.VerificaMaxRichiesteAzienda(_tiporichiesta, dipendenteId: 53188);
        }

        [TestMethod()]
        public void VerificaTettoMassimoAnnualeTest()
        {
            PraticheController p = new PraticheController();

            //p.VerificaTettoMassimoAnnuale(true, dipendenteId: 37553, importoRichiesto:5000);
            p.VerificaTettoMassimoAnnuale(false, aziendaId: 3187, importoRichiesto:0);
        }
    }
}