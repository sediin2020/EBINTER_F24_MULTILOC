namespace Sediin.PraticheRegionali.DOM.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Allegati",
                c => new
                    {
                        AllegatoId = c.Int(nullable: false, identity: true),
                        Nome = c.String(nullable: false, maxLength: 1000),
                    })
                .PrimaryKey(t => t.AllegatoId);
            
            CreateTable(
                "dbo.AvvisoUtente",
                c => new
                    {
                        AvvisoUtenteId = c.Int(nullable: false, identity: true),
                        Titolo = c.String(nullable: false, maxLength: 255),
                        Messaggio = c.String(nullable: false),
                        DataInserimento = c.DateTime(nullable: false),
                        DataScadenza = c.DateTime(),
                        Popup = c.Boolean(),
                    })
                .PrimaryKey(t => t.AvvisoUtenteId);
            
            CreateTable(
                "dbo.AvvisoUtenteRuoli",
                c => new
                    {
                        AvvisoUtenteRuoliId = c.Int(nullable: false, identity: true),
                        AvvisoUtenteId = c.Int(nullable: false),
                        Ruolo = c.String(),
                    })
                .PrimaryKey(t => t.AvvisoUtenteRuoliId)
                .ForeignKey("dbo.AvvisoUtente", t => t.AvvisoUtenteId, cascadeDelete: false)
                .Index(t => t.AvvisoUtenteId);
            
            CreateTable(
                "dbo.Azienda",
                c => new
                    {
                        AziendaId = c.Int(nullable: false, identity: true),
                        SportelloId = c.Int(),
                        MatricolaInps = c.String(nullable: false, maxLength: 10),
                        PartitaIva = c.String(nullable: false, maxLength: 11),
                        CodiceFiscale = c.String(nullable: false, maxLength: 16),
                        NomeTitolare = c.String(nullable: false, maxLength: 175),
                        CognomeTitolare = c.String(nullable: false, maxLength: 175),
                        RagioneSociale = c.String(nullable: false, maxLength: 175),
                        AttivitaEconomica = c.String(maxLength: 175),
                        Classificazione = c.String(maxLength: 175),
                        CSC = c.String(nullable: false, maxLength: 175),
                        CodiceIstat = c.String(maxLength: 175),
                        TipologiaId = c.Int(),
                        Partesociale = c.String(maxLength: 1500),
                        DataIscrizione = c.DateTime(nullable: false),
                        DataCessazione = c.DateTime(),
                        Email = c.String(nullable: false, maxLength: 175),
                        Pec = c.String(nullable: false, maxLength: 175),
                        Iban = c.String(nullable: false, maxLength: 27),
                        Indirizzo = c.String(nullable: false, maxLength: 175),
                        RegioneId = c.Int(nullable: false),
                        ProvinciaId = c.Int(nullable: false),
                        ComuneId = c.Int(nullable: false),
                        LocalitaId = c.Int(nullable: false),
                        ReferenteIndirizzo = c.String(maxLength: 175),
                        ReferenteRegioneId = c.Int(),
                        ReferenteProvinciaId = c.Int(),
                        ReferenteComuneId = c.Int(),
                        ReferenteLocalitaId = c.Int(),
                        ReferenteNome = c.String(nullable: false, maxLength: 175),
                        ReferenteCognome = c.String(nullable: false, maxLength: 175),
                        ReferenteEmail = c.String(nullable: false, maxLength: 175),
                        ReferentePec = c.String(nullable: false, maxLength: 175),
                        ReferenteCellulare = c.String(nullable: false, maxLength: 50),
                        RappresentanteIndirizzo = c.String(nullable: false, maxLength: 175),
                        RappresentanteRegioneId = c.Int(nullable: false),
                        RappresentanteProvinciaId = c.Int(nullable: false),
                        RappresentanteComuneId = c.Int(nullable: false),
                        RappresentanteLocalitaId = c.Int(nullable: false),
                        RappresentanteNome = c.String(nullable: false, maxLength: 175),
                        RappresentanteCognome = c.String(nullable: false, maxLength: 175),
                        RappresentanteEmail = c.String(nullable: false, maxLength: 175),
                        RappresentantePec = c.String(nullable: false, maxLength: 175),
                        RappresentanteCellulare = c.String(nullable: false, maxLength: 50),
                        AutorizzoComunicazioni = c.Boolean(),
                    })
                .PrimaryKey(t => t.AziendaId)
                .ForeignKey("dbo.Comuni", t => t.ComuneId, cascadeDelete: false)
                .ForeignKey("dbo.Localita", t => t.LocalitaId, cascadeDelete: false)
                .ForeignKey("dbo.Province", t => t.ProvinciaId, cascadeDelete: false)
                .ForeignKey("dbo.Regioni", t => t.RappresentanteRegioneId, cascadeDelete: false)
                .ForeignKey("dbo.Comuni", t => t.ReferenteComuneId)
                .ForeignKey("dbo.Localita", t => t.ReferenteLocalitaId)
                .ForeignKey("dbo.Province", t => t.ReferenteProvinciaId)
                .ForeignKey("dbo.Regioni", t => t.ReferenteRegioneId)
                .ForeignKey("dbo.Regioni", t => t.RegioneId, cascadeDelete: false)
                .ForeignKey("dbo.Sportello", t => t.SportelloId)
                .ForeignKey("dbo.Tipologia", t => t.TipologiaId)
                .Index(t => t.SportelloId)
                .Index(t => t.TipologiaId)
                .Index(t => t.RegioneId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.ComuneId)
                .Index(t => t.LocalitaId)
                .Index(t => t.ReferenteRegioneId)
                .Index(t => t.ReferenteProvinciaId)
                .Index(t => t.ReferenteComuneId)
                .Index(t => t.ReferenteLocalitaId)
                .Index(t => t.RappresentanteRegioneId)
                .Index(t => t.RappresentanteProvinciaId)
                .Index(t => t.RappresentanteComuneId)
                .Index(t => t.RappresentanteLocalitaId);
            
            CreateTable(
                "dbo.Comuni",
                c => new
                    {
                        ComuneId = c.Int(nullable: false, identity: true),
                        CODCOM = c.String(),
                        DENCOM = c.String(),
                        SIGPRO = c.String(),
                        CODSTA = c.String(),
                        ULTAGG = c.DateTime(),
                        UTEAGG = c.String(),
                    })
                .PrimaryKey(t => t.ComuneId);
            
            CreateTable(
                "dbo.Copertura",
                c => new
                    {
                        CoperturaId = c.Int(nullable: false, identity: true),
                        AziendaId = c.Int(nullable: false),
                        Coperto = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CoperturaId)
                .ForeignKey("dbo.Azienda", t => t.AziendaId, cascadeDelete: false)
                .Index(t => t.AziendaId);
            
            CreateTable(
                "dbo.DelegheSportelloAzienda",
                c => new
                    {
                        DelegheSportelloAziendaId = c.Int(nullable: false, identity: true),
                        SportelloId = c.Int(nullable: false),
                        AziendaId = c.Int(nullable: false),
                        DataInserimento = c.DateTime(),
                        DataDelegaDisdetta = c.DateTime(),
                        DelegaAttiva = c.Boolean(),
                        DocumentoIdentita = c.String(maxLength: 75),
                        DelegaAzienda = c.String(maxLength: 75),
                    })
                .PrimaryKey(t => t.DelegheSportelloAziendaId)
                .ForeignKey("dbo.Azienda", t => t.AziendaId, cascadeDelete: false)
                .ForeignKey("dbo.Sportello", t => t.SportelloId, cascadeDelete: false)
                .Index(t => t.SportelloId)
                .Index(t => t.AziendaId);
            
            CreateTable(
                "dbo.Sportello",
                c => new
                    {
                        SportelloId = c.Int(nullable: false, identity: true),
                        CodiceFiscalePIva = c.String(nullable: false, maxLength: 16),
                        RagioneSociale = c.String(nullable: false, maxLength: 175),
                        Indirizzo = c.String(nullable: false, maxLength: 175),
                        RegioneId = c.Int(nullable: false),
                        ProvinciaId = c.Int(nullable: false),
                        ComuneId = c.Int(nullable: false),
                        LocalitaId = c.Int(nullable: false),
                        Cognome = c.String(nullable: false, maxLength: 175),
                        Nome = c.String(nullable: false, maxLength: 175),
                        Pec = c.String(nullable: false, maxLength: 175),
                        Email = c.String(nullable: false, maxLength: 175),
                        Telefono = c.String(nullable: false, maxLength: 50),
                        Cellulare = c.String(nullable: false, maxLength: 50),
                        Ruolo = c.String(nullable: false),
                        AutorizzoComunicazioni = c.Boolean(),
                    })
                .PrimaryKey(t => t.SportelloId)
                .ForeignKey("dbo.Comuni", t => t.ComuneId, cascadeDelete: false)
                .ForeignKey("dbo.Localita", t => t.LocalitaId, cascadeDelete: false)
                .ForeignKey("dbo.Province", t => t.ProvinciaId, cascadeDelete: false)
                .ForeignKey("dbo.Regioni", t => t.RegioneId, cascadeDelete: false)
                .Index(t => t.RegioneId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.ComuneId)
                .Index(t => t.LocalitaId);
            
            CreateTable(
                "dbo.DelegheSportelloDipendente",
                c => new
                    {
                        DelegheSportelloDipendenteId = c.Int(nullable: false, identity: true),
                        SportelloId = c.Int(nullable: false),
                        DipendenteId = c.Int(nullable: false),
                        DataInserimento = c.DateTime(),
                        DataDelegaDisdetta = c.DateTime(),
                        DelegaAttiva = c.Boolean(),
                        DocumentoIdentita = c.String(maxLength: 75),
                        DelegaDipendente = c.String(maxLength: 75),
                    })
                .PrimaryKey(t => t.DelegheSportelloDipendenteId)
                .ForeignKey("dbo.Dipendente", t => t.DipendenteId, cascadeDelete: false)
                .ForeignKey("dbo.Sportello", t => t.SportelloId, cascadeDelete: false)
                .Index(t => t.SportelloId)
                .Index(t => t.DipendenteId);
            
            CreateTable(
                "dbo.Dipendente",
                c => new
                    {
                        DipendenteId = c.Int(nullable: false, identity: true),
                        CodiceFiscale = c.String(nullable: false, maxLength: 16),
                        Nome = c.String(nullable: false, maxLength: 175),
                        Cognome = c.String(nullable: false, maxLength: 175),
                        Datanascita = c.DateTime(nullable: false),
                        RegioneNascitaId = c.Int(nullable: false),
                        ProvinciaNascitaId = c.Int(nullable: false),
                        ComuneNascitaId = c.Int(nullable: false),
                        Indirizzo = c.String(nullable: false, maxLength: 175),
                        RegioneId = c.Int(nullable: false),
                        ProvinciaId = c.Int(nullable: false),
                        ComuneId = c.Int(nullable: false),
                        LocalitaId = c.Int(nullable: false),
                        Email = c.String(nullable: false, maxLength: 175),
                        Cellulare = c.String(nullable: false, maxLength: 50),
                        Iban = c.String(maxLength: 50),
                        SportelloId = c.Int(),
                        AutorizzoComunicazioni = c.Boolean(),
                    })
                .PrimaryKey(t => t.DipendenteId)
                .ForeignKey("dbo.Comuni", t => t.ComuneId, cascadeDelete: false)
                .ForeignKey("dbo.Comuni", t => t.ComuneNascitaId, cascadeDelete: false)
                .ForeignKey("dbo.Localita", t => t.LocalitaId, cascadeDelete: false)
                .ForeignKey("dbo.Province", t => t.ProvinciaId, cascadeDelete: false)
                .ForeignKey("dbo.Province", t => t.ProvinciaNascitaId, cascadeDelete: false)
                .ForeignKey("dbo.Regioni", t => t.RegioneId, cascadeDelete: false)
                .ForeignKey("dbo.Regioni", t => t.RegioneNascitaId, cascadeDelete: false)
                .ForeignKey("dbo.Sportello", t => t.SportelloId)
                .Index(t => t.RegioneNascitaId)
                .Index(t => t.ProvinciaNascitaId)
                .Index(t => t.ComuneNascitaId)
                .Index(t => t.RegioneId)
                .Index(t => t.ProvinciaId)
                .Index(t => t.ComuneId)
                .Index(t => t.LocalitaId)
                .Index(t => t.SportelloId);
            
            CreateTable(
                "dbo.DipendenteAzienda",
                c => new
                    {
                        DipendenteAziendaId = c.Int(nullable: false, identity: true),
                        AziendaId = c.Int(nullable: false),
                        DipendenteId = c.Int(nullable: false),
                        CCNLCNEL = c.String(maxLength: 175),
                        DataAssunzione = c.DateTime(nullable: false),
                        DataCessazione = c.DateTime(),
                        TipoImpiegoId = c.Int(nullable: false),
                        TipoContrattoId = c.Int(nullable: false),
                        TempoLavoroId = c.Int(nullable: false),
                        DocumentoIdentita = c.String(),
                        DocumentoAltro = c.String(),
                    })
                .PrimaryKey(t => t.DipendenteAziendaId)
                .ForeignKey("dbo.Azienda", t => t.AziendaId, cascadeDelete: false)
                .ForeignKey("dbo.Dipendente", t => t.DipendenteId, cascadeDelete: false)
                .ForeignKey("dbo.TempoLavoro", t => t.TempoLavoroId, cascadeDelete: false)
                .ForeignKey("dbo.TipoContratto", t => t.TipoContrattoId, cascadeDelete: false)
                .ForeignKey("dbo.TipoImpiego", t => t.TipoImpiegoId, cascadeDelete: false)
                .Index(t => t.AziendaId)
                .Index(t => t.DipendenteId)
                .Index(t => t.TipoImpiegoId)
                .Index(t => t.TipoContrattoId)
                .Index(t => t.TempoLavoroId);
            
            CreateTable(
                "dbo.TempoLavoro",
                c => new
                    {
                        TempoLavoroId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(nullable: false, maxLength: 500),
                        TempoPieno = c.Boolean(),
                    })
                .PrimaryKey(t => t.TempoLavoroId);
            
            CreateTable(
                "dbo.TipoContratto",
                c => new
                    {
                        TipoContrattoId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.TipoContrattoId);
            
            CreateTable(
                "dbo.TipoImpiego",
                c => new
                    {
                        TipoImpiegoId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(nullable: false, maxLength: 500),
                    })
                .PrimaryKey(t => t.TipoImpiegoId);
            
            CreateTable(
                "dbo.Localita",
                c => new
                    {
                        LocalitaId = c.Int(nullable: false, identity: true),
                        CODLOC = c.Int(nullable: false),
                        CAP = c.String(),
                        DENLOC = c.String(),
                        SIGPRO = c.String(),
                        CODCOM = c.String(),
                        ULTAGG = c.DateTime(),
                        UTEAGG = c.String(),
                    })
                .PrimaryKey(t => t.LocalitaId);
            
            CreateTable(
                "dbo.Province",
                c => new
                    {
                        ProvinciaId = c.Int(nullable: false, identity: true),
                        SIGPRO = c.String(),
                        CODREG = c.Int(nullable: false),
                        DENPRO = c.String(),
                        ULTAGG = c.DateTime(),
                        UTEAGG = c.String(),
                    })
                .PrimaryKey(t => t.ProvinciaId);
            
            CreateTable(
                "dbo.Regioni",
                c => new
                    {
                        RegioneId = c.Int(nullable: false, identity: true),
                        CODREG = c.Int(nullable: false),
                        DENREG = c.String(),
                        ULTAGG = c.DateTime(),
                        UTEAGG = c.String(),
                    })
                .PrimaryKey(t => t.RegioneId);
            
            CreateTable(
                "dbo.Tipologia",
                c => new
                    {
                        TipologiaId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(maxLength: 500),
                        Partesociale = c.Boolean(),
                    })
                .PrimaryKey(t => t.TipologiaId);
            
            CreateTable(
                "dbo.Azioni",
                c => new
                    {
                        AzioniId = c.Int(nullable: false, identity: true),
                        TipoRichiestaId = c.Int(nullable: false),
                        StatoPraticaId = c.Int(nullable: false),
                        Nome = c.String(maxLength: 75),
                        Action = c.String(maxLength: 75),
                        Controller = c.String(maxLength: 75),
                        IsSubmit = c.Boolean(),
                        ButtonCss = c.String(maxLength: 75),
                        SuccessModalFullScreen = c.Boolean(),
                        SuccessModalOffcanvas = c.Boolean(),
                        TitleSuccessModal = c.String(maxLength: 125),
                        Ordine = c.Int(),
                    })
                .PrimaryKey(t => t.AzioniId);
            
            CreateTable(
                "dbo.AzioniRuolo",
                c => new
                    {
                        AzioniRuoloId = c.Int(nullable: false, identity: true),
                        Action = c.String(maxLength: 55),
                        Ruolo = c.String(maxLength: 25),
                        StatoPraticaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AzioniRuoloId);
            
            CreateTable(
                "dbo.ContatoreAnnuale",
                c => new
                    {
                        ContatoreAnnualeId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImprese = c.String(),
                        DataInizio = c.DateTime(),
                        DataFine = c.DateTime(),
                        TettoMassimoLordo = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ContatoreAnnualeId);
            
            CreateTable(
                "dbo.DichiarazioniDPR",
                c => new
                    {
                        DichiarazioniDPRId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(maxLength: 2500),
                    })
                .PrimaryKey(t => t.DichiarazioniDPRId);
            
            CreateTable(
                "dbo.TipoRichiestaDichiarazioniDPR",
                c => new
                    {
                        TipoRichiestaDichiarazioniDPRId = c.Int(nullable: false, identity: true),
                        DichiarazioniDPRId = c.Int(nullable: false),
                        TipoRichiestaId = c.Int(nullable: false),
                        Obblicatorio = c.Boolean(),
                    })
                .PrimaryKey(t => t.TipoRichiestaDichiarazioniDPRId)
                .ForeignKey("dbo.DichiarazioniDPR", t => t.DichiarazioniDPRId, cascadeDelete: false)
                .ForeignKey("dbo.TipoRichiesta", t => t.TipoRichiestaId, cascadeDelete: false)
                .Index(t => t.DichiarazioniDPRId)
                .Index(t => t.TipoRichiestaId);
            
            CreateTable(
                "dbo.TipoRichiesta",
                c => new
                    {
                        TipoRichiestaId = c.Int(nullable: false, identity: true),
                        Modulo = c.String(nullable: false, maxLength: 50),
                        Descrizione = c.String(nullable: false, maxLength: 500),
                        View = c.String(nullable: false, maxLength: 75),
                        Classe = c.String(nullable: false, maxLength: 500),
                        Note = c.String(),
                        ContributoFisso = c.Decimal(precision: 18, scale: 2),
                        Anno = c.Int(nullable: false),
                        MaxRichiesteAnno = c.Int(),
                        CoperturaMatricolaInps = c.Boolean(),
                        AbilitatoNuovaRichiesta = c.Boolean(),
                        IsTipoRichiestaDipendente = c.Boolean(),
                        ChildClass = c.String(maxLength: 125),
                        AliquoteIRPEF = c.Decimal(precision: 18, scale: 2),
                        IbanAziendaRequired = c.Boolean(),
                        IbanDipendenteRequired = c.Boolean(),
                        BudgetDisponibile = c.Decimal(precision: 18, scale: 2),
                        RequisitiMinimo = c.Int(),
                        RequisitiMassimo = c.Int(),
                        RichiedenteMinimo = c.Int(),
                        RichiedenteMassimo = c.Int(),
                        DprMinimo = c.Int(),
                        DprMassimo = c.Int(),
                        RichiedenteTestoTitolo = c.String(),
                        UnaTantum = c.Boolean(),
                        ContributoPercentuale = c.Decimal(precision: 18, scale: 2),
                        ContributoImportoMinimo = c.Decimal(precision: 18, scale: 2),
                        ContributoImportoMassimo = c.Decimal(precision: 18, scale: 2),
                        PercentualeEnergia = c.Decimal(precision: 18, scale: 2),
                        IbanTitolareRequired = c.Boolean(),
                    })
                .PrimaryKey(t => t.TipoRichiestaId);
            
            CreateTable(
                "dbo.PraticheRegionaliImprese",
                c => new
                    {
                        PraticheRegionaliImpreseId = c.Int(nullable: false, identity: true),
                        AziendaId = c.Int(nullable: false),
                        TipoRichiestaId = c.Int(nullable: false),
                        StatoPraticaId = c.Int(nullable: false),
                        DataInserimento = c.DateTime(nullable: false),
                        DataInvio = c.DateTime(),
                        DataConferma = c.DateTime(),
                        UsernameInvio = c.String(maxLength: 75),
                        UsernameConferma = c.String(maxLength: 75),
                        ProtocolloId = c.String(maxLength: 75),
                        UserInserimento = c.String(),
                        RuoloUserInserimento = c.String(),
                        DipendenteId = c.Int(),
                        SportelloId = c.Int(),
                        ImportoRichiesto = c.Decimal(precision: 18, scale: 2),
                        ImportoContributo = c.Decimal(precision: 18, scale: 2),
                        ImportoContributoNetto = c.Decimal(precision: 18, scale: 2),
                        PercentualeContributo = c.Decimal(precision: 18, scale: 2),
                        AliquoteIRPEF = c.Decimal(precision: 18, scale: 2),
                        ImportoIRPEF = c.Decimal(precision: 18, scale: 2),
                        ContributoImportoMinimo = c.Decimal(precision: 18, scale: 2),
                        ContributoImportoMassimo = c.Decimal(precision: 18, scale: 2),
                        ContributoFisso = c.Decimal(precision: 18, scale: 2),
                        ChildClassRowCount = c.Int(),
                        Iban = c.String(maxLength: 50),
                        Responsabilita = c.Boolean(),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseId)
                .ForeignKey("dbo.Azienda", t => t.AziendaId, cascadeDelete: false)
                .ForeignKey("dbo.Dipendente", t => t.DipendenteId)
                .ForeignKey("dbo.Sportello", t => t.SportelloId)
                .ForeignKey("dbo.StatoPratica", t => t.StatoPraticaId, cascadeDelete: false)
                .ForeignKey("dbo.TipoRichiesta", t => t.TipoRichiestaId, cascadeDelete: false)
                .Index(t => t.AziendaId)
                .Index(t => t.TipoRichiestaId)
                .Index(t => t.StatoPraticaId)
                .Index(t => t.DipendenteId)
                .Index(t => t.SportelloId);
            
            CreateTable(
                "dbo.PraticheRegionaliImpreseAllegati",
                c => new
                    {
                        PraticheRegionaliImpreseAllegatiId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        TipoRichiestaAllegatiId = c.Int(nullable: false),
                        FilenameOriginale = c.String(maxLength: 255),
                        Filename = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseAllegatiId)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.PraticheRegionaliImpreseId);
            
            CreateTable(
                "dbo.PraticheRegionaliImpreseDatiPratica",
                c => new
                    {
                        PraticheRegionaliImpreseDatiPraticaId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        Nome = c.String(maxLength: 255),
                        Valore = c.String(),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseDatiPraticaId)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.PraticheRegionaliImpreseId);
            
            CreateTable(
                "dbo.PraticheRegionaliImpreseDpr",
                c => new
                    {
                        PraticheRegionaliImpreseDprId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        DichiarazioniDPRId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseDprId)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.PraticheRegionaliImpreseId);
            
            CreateTable(
                "dbo.LiquidazionePraticheRegionali",
                c => new
                    {
                        LiquidazionePraticheRegionaliId = c.Int(nullable: false, identity: true),
                        LiquidazioneId = c.Int(nullable: false),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LiquidazionePraticheRegionaliId)
                .ForeignKey("dbo.Liquidazione", t => t.LiquidazioneId, cascadeDelete: false)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.LiquidazioneId)
                .Index(t => t.PraticheRegionaliImpreseId);
            
            CreateTable(
                "dbo.Liquidazione",
                c => new
                    {
                        LiquidazioneId = c.Int(nullable: false, identity: true),
                        DataCreazione = c.DateTime(nullable: false),
                        DataLavorazione = c.DateTime(),
                        StatoLiquidazioneId = c.Int(nullable: false),
                        Allegato = c.String(maxLength: 50),
                        Note = c.String(),
                        MailDaInviareTotale = c.Int(),
                    })
                .PrimaryKey(t => t.LiquidazioneId)
                .ForeignKey("dbo.StatoLiquidazione", t => t.StatoLiquidazioneId, cascadeDelete: false)
                .Index(t => t.StatoLiquidazioneId);
            
            CreateTable(
                "dbo.LiquidazionePraticheRegionaliMailInviatiEsito",
                c => new
                    {
                        LiquidazionePraticheRegionaliMailInviatiEsitoId = c.Int(nullable: false, identity: true),
                        Email = c.String(maxLength: 75),
                        Esito = c.String(maxLength: 1000),
                        LiquidazioneId = c.Int(nullable: false),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        Inviata = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.LiquidazionePraticheRegionaliMailInviatiEsitoId)
                .ForeignKey("dbo.Liquidazione", t => t.LiquidazioneId, cascadeDelete: false)
                .Index(t => t.LiquidazioneId);
            
            CreateTable(
                "dbo.StatoLiquidazione",
                c => new
                    {
                        StatoLiquidazioneId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(maxLength: 75),
                        Ordine = c.Int(),
                    })
                .PrimaryKey(t => t.StatoLiquidazioneId);
            
            CreateTable(
                "dbo.PraticheRegionaliImpreseRequisiti",
                c => new
                    {
                        PraticheRegionaliImpreseRequisitiId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        RequisitiId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseRequisitiId)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.PraticheRegionaliImpreseId);
            
            CreateTable(
                "dbo.PraticheRegionaliImpreseRichidente",
                c => new
                    {
                        PraticheRegionaliImpreseRichidenteId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        Nominativo = c.String(nullable: false, maxLength: 255),
                        CodiceFiscale = c.String(nullable: false, maxLength: 16),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseRichidenteId)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.PraticheRegionaliImpreseId);
            
            CreateTable(
                "dbo.StatoPratica",
                c => new
                    {
                        StatoPraticaId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(maxLength: 75),
                        ReadOnly = c.Boolean(),
                        Ordine = c.Int(),
                    })
                .PrimaryKey(t => t.StatoPraticaId);
            
            CreateTable(
                "dbo.PraticheRegionaliImpreseStatoPraticaStorico",
                c => new
                    {
                        PraticheRegionaliImpreseStatoPraticaStoricoId = c.Int(nullable: false, identity: true),
                        PraticheRegionaliImpreseId = c.Int(nullable: false),
                        StatoPraticaId = c.Int(nullable: false),
                        Note = c.String(),
                        DataInserimento = c.DateTime(nullable: false),
                        MotivazioniId = c.Int(),
                        UserName = c.String(),
                        UserRuolo = c.String(),
                    })
                .PrimaryKey(t => t.PraticheRegionaliImpreseStatoPraticaStoricoId)
                .ForeignKey("dbo.Motivazioni", t => t.MotivazioniId)
                .ForeignKey("dbo.StatoPratica", t => t.StatoPraticaId, cascadeDelete: false)
                .ForeignKey("dbo.PraticheRegionaliImprese", t => t.PraticheRegionaliImpreseId, cascadeDelete: false)
                .Index(t => t.PraticheRegionaliImpreseId)
                .Index(t => t.StatoPraticaId)
                .Index(t => t.MotivazioniId);
            
            CreateTable(
                "dbo.Motivazioni",
                c => new
                    {
                        MotivazioniId = c.Int(nullable: false, identity: true),
                        StatoPraticaId = c.Int(nullable: false),
                        Motivazione = c.String(maxLength: 75),
                        Note = c.String(),
                    })
                .PrimaryKey(t => t.MotivazioniId);
            
            CreateTable(
                "dbo.TipoRichiestaRequisiti",
                c => new
                    {
                        TipoRichiestaRequisitiId = c.Int(nullable: false, identity: true),
                        RequisitiId = c.Int(nullable: false),
                        TipoRichiestaId = c.Int(nullable: false),
                        Obblicatorio = c.Boolean(),
                        ContributoImporto = c.Decimal(precision: 18, scale: 2),
                        ContributoPercentuale = c.Decimal(precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.TipoRichiestaRequisitiId)
                .ForeignKey("dbo.Requisiti", t => t.RequisitiId, cascadeDelete: false)
                .ForeignKey("dbo.TipoRichiesta", t => t.TipoRichiestaId, cascadeDelete: false)
                .Index(t => t.RequisitiId)
                .Index(t => t.TipoRichiestaId);
            
            CreateTable(
                "dbo.Requisiti",
                c => new
                    {
                        RequisitiId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(maxLength: 2500),
                    })
                .PrimaryKey(t => t.RequisitiId);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        LogsId = c.Int(nullable: false, identity: true),
                        Data = c.DateTime(nullable: false),
                        Username = c.String(maxLength: 50),
                        Ruolo = c.String(maxLength: 50),
                        ViewDataJson = c.String(),
                        Model = c.String(maxLength: 255),
                        Message = c.String(),
                        Action = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.LogsId);
            
            CreateTable(
                "dbo.MotivazioniRichiesta",
                c => new
                    {
                        MotivazioniRichiestaId = c.Int(nullable: false, identity: true),
                        TipoRichiestaId = c.Int(nullable: false),
                        Motivazione = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.MotivazioniRichiestaId);
            
            CreateTable(
                "dbo.NavigatioHistory",
                c => new
                    {
                        NavigatioHistoryId = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 75),
                        Data = c.DateTime(),
                        UserHostAddress = c.String(maxLength: 75),
                        CurrentUrl = c.String(),
                        BrowserIsMobileDevice = c.Boolean(),
                        BrowserJScriptVersionMajor = c.Int(),
                        BrowserJScriptVersionMinor = c.Int(),
                        BrowserMajorVersion = c.Int(),
                        BrowserMobileDeviceModel = c.String(maxLength: 255),
                        BrowserName = c.String(maxLength: 255),
                        BrowserVersion = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.NavigatioHistoryId);
            
            CreateTable(
                "dbo.Parentela",
                c => new
                    {
                        ParentelaId = c.Int(nullable: false, identity: true),
                        Descrizione = c.String(maxLength: 500),
                        Note = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.ParentelaId);
            
            CreateTable(
                "dbo.TipoRichiestaAllegati",
                c => new
                    {
                        TipoRichiestaAllegatiId = c.Int(nullable: false, identity: true),
                        AllegatoId = c.Int(nullable: false),
                        TipoRichiestaId = c.Int(nullable: false),
                        Obblicatorio = c.Boolean(),
                        Caricamento = c.Boolean(),
                    })
                .PrimaryKey(t => t.TipoRichiestaAllegatiId)
                .ForeignKey("dbo.Allegati", t => t.AllegatoId, cascadeDelete: false)
                .ForeignKey("dbo.TipoRichiesta", t => t.TipoRichiestaId, cascadeDelete: false)
                .Index(t => t.AllegatoId)
                .Index(t => t.TipoRichiestaId);
            
            CreateTable(
                "dbo.Uniemens",
                c => new
                    {
                        UniemensId = c.Int(nullable: false, identity: true),
                        ID_EBT = c.Int(nullable: false),
                        AziendaId = c.Int(nullable: false),
                        Anno = c.Int(),
                        Mensilita = c.Int(),
                        UniemensBson = c.String(),
                    })
                .PrimaryKey(t => t.UniemensId)
                .ForeignKey("dbo.Azienda", t => t.AziendaId, cascadeDelete: false)
                .Index(t => t.AziendaId);
            

            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Uniemens", "AziendaId", "dbo.Azienda");
            DropForeignKey("dbo.TipoRichiestaAllegati", "TipoRichiestaId", "dbo.TipoRichiesta");
            DropForeignKey("dbo.TipoRichiestaAllegati", "AllegatoId", "dbo.Allegati");
            DropForeignKey("dbo.TipoRichiestaRequisiti", "TipoRichiestaId", "dbo.TipoRichiesta");
            DropForeignKey("dbo.TipoRichiestaRequisiti", "RequisitiId", "dbo.Requisiti");
            DropForeignKey("dbo.TipoRichiestaDichiarazioniDPR", "TipoRichiestaId", "dbo.TipoRichiesta");
            DropForeignKey("dbo.PraticheRegionaliImprese", "TipoRichiestaId", "dbo.TipoRichiesta");
            DropForeignKey("dbo.PraticheRegionaliImpreseStatoPraticaStorico", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.PraticheRegionaliImpreseStatoPraticaStorico", "StatoPraticaId", "dbo.StatoPratica");
            DropForeignKey("dbo.PraticheRegionaliImpreseStatoPraticaStorico", "MotivazioniId", "dbo.Motivazioni");
            DropForeignKey("dbo.PraticheRegionaliImprese", "StatoPraticaId", "dbo.StatoPratica");
            DropForeignKey("dbo.PraticheRegionaliImprese", "SportelloId", "dbo.Sportello");
            DropForeignKey("dbo.PraticheRegionaliImpreseRichidente", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.PraticheRegionaliImpreseRequisiti", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.LiquidazionePraticheRegionali", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.Liquidazione", "StatoLiquidazioneId", "dbo.StatoLiquidazione");
            DropForeignKey("dbo.LiquidazionePraticheRegionaliMailInviatiEsito", "LiquidazioneId", "dbo.Liquidazione");
            DropForeignKey("dbo.LiquidazionePraticheRegionali", "LiquidazioneId", "dbo.Liquidazione");
            DropForeignKey("dbo.PraticheRegionaliImpreseDpr", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.PraticheRegionaliImprese", "DipendenteId", "dbo.Dipendente");
            DropForeignKey("dbo.PraticheRegionaliImpreseDatiPratica", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.PraticheRegionaliImprese", "AziendaId", "dbo.Azienda");
            DropForeignKey("dbo.PraticheRegionaliImpreseAllegati", "PraticheRegionaliImpreseId", "dbo.PraticheRegionaliImprese");
            DropForeignKey("dbo.TipoRichiestaDichiarazioniDPR", "DichiarazioniDPRId", "dbo.DichiarazioniDPR");
            DropForeignKey("dbo.Azienda", "TipologiaId", "dbo.Tipologia");
            DropForeignKey("dbo.Azienda", "SportelloId", "dbo.Sportello");
            DropForeignKey("dbo.Azienda", "RegioneId", "dbo.Regioni");
            DropForeignKey("dbo.Azienda", "ReferenteRegioneId", "dbo.Regioni");
            DropForeignKey("dbo.Azienda", "ReferenteLocalitaId", "dbo.Localita");
            DropForeignKey("dbo.Azienda", "ReferenteComuneId", "dbo.Comuni");
            DropForeignKey("dbo.Azienda", "RappresentanteRegioneId", "dbo.Regioni");
            DropForeignKey("dbo.Azienda", "RappresentanteProvinciaId", "dbo.Province");
            DropForeignKey("dbo.Azienda", "ProvinciaId", "dbo.Province");
            DropForeignKey("dbo.Azienda", "LocalitaId", "dbo.Localita");
            DropForeignKey("dbo.Sportello", "RegioneId", "dbo.Regioni");
            DropForeignKey("dbo.Sportello", "ProvinciaId", "dbo.Province");
            DropForeignKey("dbo.Sportello", "LocalitaId", "dbo.Localita");
            DropForeignKey("dbo.DelegheSportelloDipendente", "SportelloId", "dbo.Sportello");
            DropForeignKey("dbo.DelegheSportelloDipendente", "DipendenteId", "dbo.Dipendente");
            DropForeignKey("dbo.Dipendente", "SportelloId", "dbo.Sportello");
            DropForeignKey("dbo.Dipendente", "RegioneNascitaId", "dbo.Regioni");
            DropForeignKey("dbo.Dipendente", "RegioneId", "dbo.Regioni");
            DropForeignKey("dbo.Dipendente", "ProvinciaNascitaId", "dbo.Province");
            DropForeignKey("dbo.Dipendente", "ProvinciaId", "dbo.Province");
            DropForeignKey("dbo.Dipendente", "LocalitaId", "dbo.Localita");
            DropForeignKey("dbo.Dipendente", "ComuneNascitaId", "dbo.Comuni");
            DropForeignKey("dbo.Dipendente", "ComuneId", "dbo.Comuni");
            DropForeignKey("dbo.DipendenteAzienda", "TipoImpiegoId", "dbo.TipoImpiego");
            DropForeignKey("dbo.DipendenteAzienda", "TipoContrattoId", "dbo.TipoContratto");
            DropForeignKey("dbo.DipendenteAzienda", "TempoLavoroId", "dbo.TempoLavoro");
            DropForeignKey("dbo.DipendenteAzienda", "DipendenteId", "dbo.Dipendente");
            DropForeignKey("dbo.DipendenteAzienda", "AziendaId", "dbo.Azienda");
            DropForeignKey("dbo.DelegheSportelloAzienda", "SportelloId", "dbo.Sportello");
            DropForeignKey("dbo.Sportello", "ComuneId", "dbo.Comuni");
            DropForeignKey("dbo.DelegheSportelloAzienda", "AziendaId", "dbo.Azienda");
            DropForeignKey("dbo.Copertura", "AziendaId", "dbo.Azienda");
            DropForeignKey("dbo.Azienda", "ComuneId", "dbo.Comuni");
            DropForeignKey("dbo.AvvisoUtenteRuoli", "AvvisoUtenteId", "dbo.AvvisoUtente");
            DropIndex("dbo.Uniemens", new[] { "AziendaId" });
            DropIndex("dbo.TipoRichiestaAllegati", new[] { "TipoRichiestaId" });
            DropIndex("dbo.TipoRichiestaAllegati", new[] { "AllegatoId" });
            DropIndex("dbo.TipoRichiestaRequisiti", new[] { "TipoRichiestaId" });
            DropIndex("dbo.TipoRichiestaRequisiti", new[] { "RequisitiId" });
            DropIndex("dbo.PraticheRegionaliImpreseStatoPraticaStorico", new[] { "MotivazioniId" });
            DropIndex("dbo.PraticheRegionaliImpreseStatoPraticaStorico", new[] { "StatoPraticaId" });
            DropIndex("dbo.PraticheRegionaliImpreseStatoPraticaStorico", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.PraticheRegionaliImpreseRichidente", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.PraticheRegionaliImpreseRequisiti", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.LiquidazionePraticheRegionaliMailInviatiEsito", new[] { "LiquidazioneId" });
            DropIndex("dbo.Liquidazione", new[] { "StatoLiquidazioneId" });
            DropIndex("dbo.LiquidazionePraticheRegionali", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.LiquidazionePraticheRegionali", new[] { "LiquidazioneId" });
            DropIndex("dbo.PraticheRegionaliImpreseDpr", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.PraticheRegionaliImpreseDatiPratica", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.PraticheRegionaliImpreseAllegati", new[] { "PraticheRegionaliImpreseId" });
            DropIndex("dbo.PraticheRegionaliImprese", new[] { "SportelloId" });
            DropIndex("dbo.PraticheRegionaliImprese", new[] { "DipendenteId" });
            DropIndex("dbo.PraticheRegionaliImprese", new[] { "StatoPraticaId" });
            DropIndex("dbo.PraticheRegionaliImprese", new[] { "TipoRichiestaId" });
            DropIndex("dbo.PraticheRegionaliImprese", new[] { "AziendaId" });
            DropIndex("dbo.TipoRichiestaDichiarazioniDPR", new[] { "TipoRichiestaId" });
            DropIndex("dbo.TipoRichiestaDichiarazioniDPR", new[] { "DichiarazioniDPRId" });
            DropIndex("dbo.DipendenteAzienda", new[] { "TempoLavoroId" });
            DropIndex("dbo.DipendenteAzienda", new[] { "TipoContrattoId" });
            DropIndex("dbo.DipendenteAzienda", new[] { "TipoImpiegoId" });
            DropIndex("dbo.DipendenteAzienda", new[] { "DipendenteId" });
            DropIndex("dbo.DipendenteAzienda", new[] { "AziendaId" });
            DropIndex("dbo.Dipendente", new[] { "SportelloId" });
            DropIndex("dbo.Dipendente", new[] { "LocalitaId" });
            DropIndex("dbo.Dipendente", new[] { "ComuneId" });
            DropIndex("dbo.Dipendente", new[] { "ProvinciaId" });
            DropIndex("dbo.Dipendente", new[] { "RegioneId" });
            DropIndex("dbo.Dipendente", new[] { "ComuneNascitaId" });
            DropIndex("dbo.Dipendente", new[] { "ProvinciaNascitaId" });
            DropIndex("dbo.Dipendente", new[] { "RegioneNascitaId" });
            DropIndex("dbo.DelegheSportelloDipendente", new[] { "DipendenteId" });
            DropIndex("dbo.DelegheSportelloDipendente", new[] { "SportelloId" });
            DropIndex("dbo.Sportello", new[] { "LocalitaId" });
            DropIndex("dbo.Sportello", new[] { "ComuneId" });
            DropIndex("dbo.Sportello", new[] { "ProvinciaId" });
            DropIndex("dbo.Sportello", new[] { "RegioneId" });
            DropIndex("dbo.DelegheSportelloAzienda", new[] { "AziendaId" });
            DropIndex("dbo.DelegheSportelloAzienda", new[] { "SportelloId" });
            DropIndex("dbo.Copertura", new[] { "AziendaId" });
            DropIndex("dbo.Azienda", new[] { "RappresentanteLocalitaId" });
            DropIndex("dbo.Azienda", new[] { "RappresentanteComuneId" });
            DropIndex("dbo.Azienda", new[] { "RappresentanteProvinciaId" });
            DropIndex("dbo.Azienda", new[] { "RappresentanteRegioneId" });
            DropIndex("dbo.Azienda", new[] { "ReferenteLocalitaId" });
            DropIndex("dbo.Azienda", new[] { "ReferenteComuneId" });
            DropIndex("dbo.Azienda", new[] { "ReferenteProvinciaId" });
            DropIndex("dbo.Azienda", new[] { "ReferenteRegioneId" });
            DropIndex("dbo.Azienda", new[] { "LocalitaId" });
            DropIndex("dbo.Azienda", new[] { "ComuneId" });
            DropIndex("dbo.Azienda", new[] { "ProvinciaId" });
            DropIndex("dbo.Azienda", new[] { "RegioneId" });
            DropIndex("dbo.Azienda", new[] { "TipologiaId" });
            DropIndex("dbo.Azienda", new[] { "SportelloId" });
            DropIndex("dbo.AvvisoUtenteRuoli", new[] { "AvvisoUtenteId" }); 
            DropTable("dbo.Uniemens");
            DropTable("dbo.TipoRichiestaAllegati");
            DropTable("dbo.Parentela");
            DropTable("dbo.NavigatioHistory");
            DropTable("dbo.MotivazioniRichiesta");
            DropTable("dbo.Logs");
            DropTable("dbo.Requisiti");
            DropTable("dbo.TipoRichiestaRequisiti");
            DropTable("dbo.Motivazioni");
            DropTable("dbo.PraticheRegionaliImpreseStatoPraticaStorico");
            DropTable("dbo.StatoPratica");
            DropTable("dbo.PraticheRegionaliImpreseRichidente");
            DropTable("dbo.PraticheRegionaliImpreseRequisiti");
            DropTable("dbo.StatoLiquidazione");
            DropTable("dbo.LiquidazionePraticheRegionaliMailInviatiEsito");
            DropTable("dbo.Liquidazione");
            DropTable("dbo.LiquidazionePraticheRegionali");
            DropTable("dbo.PraticheRegionaliImpreseDpr");
            DropTable("dbo.PraticheRegionaliImpreseDatiPratica");
            DropTable("dbo.PraticheRegionaliImpreseAllegati");
            DropTable("dbo.PraticheRegionaliImprese");
            DropTable("dbo.TipoRichiesta");
            DropTable("dbo.TipoRichiestaDichiarazioniDPR");
            DropTable("dbo.DichiarazioniDPR");
            DropTable("dbo.ContatoreAnnuale");
            DropTable("dbo.AzioniRuolo");
            DropTable("dbo.Azioni");
            DropTable("dbo.Tipologia");
            DropTable("dbo.Regioni");
            DropTable("dbo.Province");
            DropTable("dbo.Localita");
            DropTable("dbo.TipoImpiego");
            DropTable("dbo.TipoContratto");
            DropTable("dbo.TempoLavoro");
            DropTable("dbo.DipendenteAzienda");
            DropTable("dbo.Dipendente");
            DropTable("dbo.DelegheSportelloDipendente");
            DropTable("dbo.Sportello");
            DropTable("dbo.DelegheSportelloAzienda");
            DropTable("dbo.Copertura");
            DropTable("dbo.Comuni");
            DropTable("dbo.Azienda");
            DropTable("dbo.AvvisoUtenteRuoli");
            DropTable("dbo.AvvisoUtente");
            DropTable("dbo.Allegati");
        }
    }
}
