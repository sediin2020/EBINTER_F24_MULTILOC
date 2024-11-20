using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Sediin.PraticheRegionali.DOM.DAL;
using Sediin.PraticheRegionali.DOM.Entitys;
using static Sediin.PraticheRegionali.DOM.Providers.LiquidazioneIdProvider;

namespace Sediin.PraticheRegionali.DOM.Providers
{
    public delegate void OnSuccessSendMailLiquidazioneReport(string processoId, string username, string tipoImport, int index, int totale, string message);

    public delegate string OnSendMailLiquidazioneReport(SendMailLiquidazioneEmailResultModel model);//, HttpContext context);

    public delegate void OnErrorFileMailLiquidazioneReport(string base64, string tipo, string username, string ruolo);

    public class LiquidazioneIdProvider
    {
        public event OnSuccessSendMailLiquidazioneReport OnSuccessSendMailLiquidazioneReport;

        public event OnSendMailLiquidazioneReport OnSendMailLiquidazioneReport;

        public event OnErrorFileMailLiquidazioneReport OnErrorFileMailLiquidazione;

        public class SendMailLiquidazioneEmailResultModel
        {
            public string Importo { get; set; }

            public string Nominativo { get; set; }

            public string Email { get; set; }

            public string Iban { get; set; }

            public string Body { get; set; }

            public string TipoRichiesta { get; set; }
        }

        public List<string> ErrorList { get; set; }

        public string Ruolo { get; set; }

        public string Username { get; set; }

        public string BodyMail { get; set; }

        //public string BodyMailAzienda { get; set; }

        //public string BodyMailDipendente { get; set; }

        public HttpContext CurrentHttpContext { get; set; }

        public void ProcessSendMailLiquidazione(int liquidazioneId)
        {
            ErrorList = new List<string>();

            var _id = Guid.NewGuid().ToString();

            try
            {

                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", 0, 0, "Attendere, preparazione dati in corso...");

                #region MyRegion

                UnitOfWork unitOfWork = new UnitOfWork();

                var _liquidazione = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == liquidazioneId)?.FirstOrDefault();

                var _emailesito = _liquidazione.MailInviate.Where(x => x.Inviata == true);

                List<SendMailLiquidazioneEmailResultModel> _listEmail = new List<SendMailLiquidazioneEmailResultModel>();

                var _x = 0;

                var _totaleRighe = _liquidazione.LiquidazionePraticheRegionali.Count();

                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", 0, _totaleRighe, "Inizion invio mail in corso...");

                var _xx = 0;

                var _email = "";

                foreach (var item in _liquidazione.LiquidazionePraticheRegionali)
                {
                    _email = !item.PraticheRegionaliImprese.TipoRichiesta.IsTipoRichiestaDipendente.GetValueOrDefault()
                        ? item.PraticheRegionaliImprese.Azienda.Email : item.PraticheRegionaliImprese.Dipendente.Email;

                    SendMailLiquidazioneEmailResultModel _mail = new SendMailLiquidazioneEmailResultModel
                    {
                        Importo = item.PraticheRegionaliImprese.ImportoContributoNetto.GetValueOrDefault().ToString("n"),
                        Iban = item.PraticheRegionaliImprese.Iban.ToUpper().RemoveWhiteSpace(),
                        Nominativo = item.PraticheRegionaliImprese.Azienda?.RagioneSociale,
                        TipoRichiesta = item.PraticheRegionaliImprese.TipoRichiesta.Descrizione,
                        Email = _email,
                        Body = BodyMail,
                    };

                    _xx++;

                    if (_emailesito.FirstOrDefault(x => item.LiquidazioneId == x.LiquidazioneId
                    && item.PraticheRegionaliImpreseId == x.PraticheRegionaliImpreseId
                    && x.Email.ToUpper() == _email.ToUpper()) != null)
                    {
                        OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", Interlocked.Increment(ref _x), _totaleRighe, $"Email già stato inviata {_email}");
                        continue;
                    }

                    var _mess = OnSendMailLiquidazioneReport?.Invoke(_mail);

                    if (!string.IsNullOrWhiteSpace(_mess))
                    {
                        ErrorList.Add(_mess);
                    }

                    LiquidazionePraticheRegionaliMailInviatiEsito _esito = new LiquidazionePraticheRegionaliMailInviatiEsito
                    {
                        LiquidazioneId = liquidazioneId,
                        Esito = _mess,
                        Inviata = string.IsNullOrWhiteSpace(_mess),
                        Email = _email,
                        PraticheRegionaliImpreseId = item.PraticheRegionaliImpreseId
                    };

                    unitOfWork.LiquidazionePraticheRegionaliMailInviatiEsitoRepository.Insert(_esito);
                    unitOfWork.Save(false);

                    if (_xx % 25 == 0)
                    {
                        OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", Interlocked.Increment(ref _x), _totaleRighe, _mess + "<br/><span class='text-danger'>Attendere, attesa di 1 minuto</span>");
                        Thread.Sleep(60000);
                    }
                    else
                    {
                        OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", Interlocked.Increment(ref _x), _totaleRighe, _mess);
                    }
                }

                #endregion

                UnitOfWork unitOfWork1 = new UnitOfWork();
                var _li = unitOfWork.LiquidazioneRepository.Get(x => x.LiquidazioneId == liquidazioneId).FirstOrDefault();
                _li.MailDaInviareTotale = _totaleRighe;
                unitOfWork.LiquidazioneRepository.Update(_li);
                unitOfWork.Save(false);

                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", _totaleRighe, _totaleRighe, "Processo terminato");

                if (ErrorList.Count() > 0)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(string.Join(Environment.NewLine, ErrorList));

                        var _base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
                        OnErrorFileMailLiquidazione?.Invoke(_base64, "SendMail", Username, Ruolo);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                OnSuccessSendMailLiquidazioneReport?.Invoke(_id, Username, "SendMail", 0, 0, "Si e verificcato un errore, " + ex.Message);
            }
        }
    }
}
