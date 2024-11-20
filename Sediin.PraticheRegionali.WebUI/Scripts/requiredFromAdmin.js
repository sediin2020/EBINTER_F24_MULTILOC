
$.validator.addMethod('requiredfromsediinpraticheregionaliadmin', function (value, element, params) {
    return value != "";
});

$.validator.unobtrusive.adapters.add("requiredfromsediinpraticheregionaliadmin", function (options) {
    options.rules["requiredfromsediinpraticheregionaliadmin"] = options.params;
    options.messages["requiredfromsediinpraticheregionaliadmin"] = options.message;
});

