$.validator.setDefaults({
    ignore: [],
    validClass: "custom-valid",
    errorClass: "custom-is-invalid",
    errorElement: 'span',
    errorPlacement: function (error, element) {
        var obj = element;
        if (element.attr('valid-error-target')) {
            var obj = $('#' + obj.attr("error-target"));
        }

        if (obj.closest('.form-group').length > 0) {
            error.appendTo(obj.closest('.form-group'));
        } else {
            error.insertAfter(obj);
        }
    },
    highlight: function (element, errorClass) {
        if ($(element).attr("valid-highlight") != "") {
            $($(element).attr("valid-highlight")).addClass(errorClass);
        } //else {
            $(element).addClass(errorClass);
        //}

        if (element.type == "radio" || element.type == "checkbox") {
          $(element).closest('.form-inline').addClass(errorClass);
        } else if ($(element).attr("data-plugin") == "tokenfield") {
            $(element).closest('.tokenfield').addClass(errorClass);
        }
        $(element).closest('.form-group').find('.form-control-label').addClass(errorClass);
    },
    unhighlight: function (element, errorClass) {
        

        if ($(element).attr("valid-highlight") != "") {
            $($(element).attr("valid-highlight")).removeClass(errorClass);
        } //else {
            $(element).removeClass(errorClass);
        //}

        if (element.type == "radio" || element.type == "checkbox") {
            $(element).closest('.form-inline').removeClass(errorClass);
        } else if ($(element).attr("data-plugin") == "tokenfield") {
            $(element).closest('.tokenfield').removeClass(errorClass);
        }
        $(element).closest('.form-group').find('.form-control-label').removeClass(errorClass);
    }
});

$.validator.addMethod('regex', function (value, element, param) {
    return this.optional(element) ||
        value.match(typeof param == 'string' ? new RegExp(param) : param);
},
    'Please enter a value in the correct format.');

var valid_option = {}