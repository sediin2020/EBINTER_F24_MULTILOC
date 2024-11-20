
$.validator.addMethod('requiredistrue', function (value, element, params) {

    var _isrequiredfield = $("#" + params.isrequiredfield).is(":checked");

    if (_isrequiredfield && value == "") {
        return false;
    }

    return true;
});

$.validator.unobtrusive.adapters.add("requiredistrue", ["isrequiredfield"], function (options) {
    options.rules["requiredistrue"] = options.params;
    options.messages["requiredistrue"] = options.message;
});

