///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function (ko) {
    ko.bindingHandlers.slidevisible = {
        init: function (element) {
            $(element).hide();
        },
        update: function (element, valueAccessor, allBindingsAccessor) {
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);
            var duration = allBindings.slideDuration || 400;

            if (valueUnwrapped) {
                $(element).slideDown(duration);
            } else {
                $(element).slideUp(duration);
            }
        }
    };
})(ko);

