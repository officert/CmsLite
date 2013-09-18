// ** overriding unobtrusive validation must happen BEFORE it gets loaded
//http://stackoverflow.com/questions/5487139/how-can-i-customize-the-unobtrusive-validation-in-asp-net-mvc-3-to-match-my-styl/9231738#9231738

$.validator.setDefaults({                               //add Bootstrap's class names instead of defaults
    highlight: function (element, errorClass) {
        $(element).closest('.form-group').addClass('has-error');
    },
    unhighlight: function (element, errorClass) {
        $(element).closest('.form-group').removeClass('has-error');
    }
});

$.fn.extend({             //remove Bootstrap class names, and validation errors
    resetValidation: function () {
        var form = $(this);
        var formElement = form.children('div.modal-dialog').children('form').first();

        //formElement.removeData('validate');

        var formElements = formElement.find('*[data-val="true"]');
        formElements.each(function () {
            $(this).removeClass('input-validation-error');
            $(this).val('');
            $(this).parent().removeClass('has-error');
            $(this).next('.validation-error-message')
                .removeClass('field-validation-error')
                .html('');
        });
    }
})