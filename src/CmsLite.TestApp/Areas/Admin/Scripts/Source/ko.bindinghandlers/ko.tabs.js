///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function (ko) {
    ko.bindingHandlers.tabs = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

            var $element = $(element);
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            //properties - bootstrap's
            var tabSelector = allBindings.tabselector;
            var contentSelector = allBindings.contentselector;

            if (typeof tabSelector != 'function') throw new Error("You must provide a function for the tabselector binding value. " +
                    "Your function will be called and passed the container element the data-bind attribute is on as a first arg.");

            if (contentSelector == null) throw new Error("You must provide a function for the contentSelector binding value. " +
                    "Your function will be called and passed the container element the data-bind attribute is on as a first arg.");

            $element.simpleTabs({
                tabs: tabSelector,
                tabSections: contentSelector
            });
        }
    };
})(ko);