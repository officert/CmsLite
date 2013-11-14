(function (window, ko) {

    ko.bindingHandlers.bootstrapmodal = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

            var $element = $(element);
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            var modaloptions = value;
            modaloptions.backdrop = modaloptions.backdrop === null || modaloptions.backdrop === undefined ? true : modaloptions.backdrop;
            modaloptions.keyboard = modaloptions.keyboard === null || modaloptions.keyboard === undefined ? true : modaloptions.keyboard;
            modaloptions.show = modaloptions.show === null || modaloptions.show === undefined ? false : modaloptions.show;
            modaloptions.remote = modaloptions.remote === null || modaloptions.remote === undefined ? false : modaloptions.remote;
            //event bindings
            var onshow = modaloptions.onshow || null;
            var onshown = modaloptions.onshown || null;
            var onhide = modaloptions.onhide || null;
            var onhiden = modaloptions.onhiden || null;

            $element.modal(modaloptions);

            //bind events
            $element.on('show.bs.modal', function () {
                if (onshow && typeof onshow == 'function') {
                    onshow.call();
                }
            });
            $element.on('shown.bs.modal', function () {
                
                if (onshown && typeof onshown == 'function') {
                    onshown.call();
                }
            });
            $element.on('hide.bs.modal', function () {
                if (onhide && typeof onhide == 'function') {
                    onhide.call();
                }
                $element.resetValidation();
            });
            $element.on('hidden.bs.modal', function () {
                if (onhiden && typeof onhiden == 'function') {
                    onhiden.call();
                }
            });

            $.validator.unobtrusive.parse($element);
        }
        //update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //}
    };
}(window, ko))