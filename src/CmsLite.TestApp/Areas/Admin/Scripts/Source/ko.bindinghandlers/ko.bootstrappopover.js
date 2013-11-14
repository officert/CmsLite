(function (window, ko) {
    ko.bindingHandlers.bootstrappopover = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

            var $element = $(element);
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            //properties
            var popoveroptions = value;

            $element.popover(popoveroptions);
        }
        //update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //}
    };
}(window, ko));