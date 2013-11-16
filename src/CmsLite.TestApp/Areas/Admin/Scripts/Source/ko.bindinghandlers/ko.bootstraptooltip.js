(function (window, ko) {
    ko.bindingHandlers.bootstraptooltip = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            //http://getbootstrap.com/javascript/#tooltips
            
            var $element = $(element);
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            //properties
            var tooltipoptions = valueUnwrapped;

            $element.tooltip(tooltipoptions);
        },
        update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
            var value = ko.utils.unwrapObservable(valueAccessor());
            var $element = $(element);

            if (value) $element.val(value);
            
            //ko.utils.setTextContent(element, valueAccessor());
        }
    };
}(window, ko));