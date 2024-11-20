$.validator.addMethod('praticheimportorimborsatocontributorichiestodefaultvalidation', function (value, element, params) {

    $("#ImportoContributo-span").html("0,00");
    $("#ImportoContributoNetto-span").html("0,00");
    $("#ImportoIRPEF-span").html("0,00");
    $("#containerImportoCalcolati").html("");

    if ($("#ImportoRichiesto").val() == null || $("#ImportoRichiesto").val() == "") {
        return false;
    }

    if ($("#PercentualeContributo").val() == null || $("#PercentualeContributo").val() == "") {
        return false;
    }

    var _data;

    $.ajax({
        async: false,
        type: 'POST',
        url: "/Pratiche/CalcolaImportoRimborsatoContributo",
        data: {
            tipoRichiestaId: $("#TipoRichiestaId").val(),
            importo: $("#ImportoRichiesto").val(),
            percentuale: $("#PercentualeContributo").val()
        },
        success: function (data) {
            _data = data;
        },
    });

    if (_data == null || _data == undefined) {
        return false;
    }

    if (_data.isValid == false) {
        return false;
    }
    else {
       //$("#PercentualeContributo-span").html(toLocalCurrency(_data.importiCalcolati.PercentualeContributo));
        //$("#PercentualeContributo").keypress(function () {
        //    let keyupTimer;
        //    clearTimeout(keyupTimer);
        //    keyupTimer = setTimeout(function () {
        //        $("#PercentualeContributo").val(_data.importiCalcolati.PercentualeContributo) 
        //    }, 800);
        //});

        $("#AliquoteIRPEF-span").html(toLocalCurrency(_data.importiCalcolati.AliquoteIRPEF));
        $("#ContributoImportoMinimo-span").html(toLocalCurrency(_data.importiCalcolati.ContributoImportoMinimo));
        $("#ContributoImportoMassimo-span").html(toLocalCurrency(_data.importiCalcolati.ContributoImportoMassimo));
        $("#ContributoFisso-span").html(toLocalCurrency(_data.importiCalcolati.ContributoFisso));
        $("#ImportoContributoNetto-span").html(toLocalCurrency(_data.importiCalcolati.ImportoContributoNetto));
        $("#ImportoContributo-span").html(toLocalCurrency(_data.importiCalcolati.ImportoContributo));
        $("#ImportoIRPEF-span").html(toLocalCurrency(_data.importiCalcolati.ImportoIRPEF));
        $("#containerImportoCalcolati").html(_data.html);
        return true;
    }

    return false;
});

$.validator.unobtrusive.adapters.add("praticheimportorimborsatocontributorichiestodefaultvalidation", function (options) {
    options.rules["praticheimportorimborsatocontributorichiestodefaultvalidation"] = options.params;
    options.messages["praticheimportorimborsatocontributorichiestodefaultvalidation"] = options.message;

    $("#" + options.element.id).on("blur", function () {
        $("#" + options.element.id).valid();
    });

    $("#" + options.element.id).keypress(function () {
        let keyupTimer;
        clearTimeout(keyupTimer);
        keyupTimer = setTimeout(function () {
            $("#" + options.element.id).valid();
        }, 800);
    });
});

