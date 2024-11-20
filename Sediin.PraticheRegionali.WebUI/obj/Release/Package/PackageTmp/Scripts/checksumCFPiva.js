$.validator.addMethod('checksumcfpiva', function (value, element, params) {

    if (String(params.required).toLowerCase() == "false") {
        if (value == "") {
            return true;
        }
    }

    if (value == '' || value == undefined || value == null) {
        return false;
    }

    var cf = String(value).toUpperCase();

    if (String(params.requiredpivaorcf).toLowerCase() == "true") {
        if (cf.length == 11) {
            if (new RegExp("[0-9]{11}").test(value)) {
                return true;
            }
        }
    }

    return validateCodiceFiscale(cf);

});

$.validator.unobtrusive.adapters.add("checksumcfpiva", ["requiredpivaorcf", "required"], function (options) {
    options.rules["checksumcfpiva"] = options.params;
    options.messages["checksumcfpiva"] = options.message;
});
