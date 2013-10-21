///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function (ko) {
    ko.bindingHandlers.bootstrapmodal = {
        init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

            var $element = $(element);
            var value = valueAccessor(), allBindings = allBindingsAccessor();
            var valueUnwrapped = ko.utils.unwrapObservable(value);

            //properties - bootstrap's
            var backdrop = allBindings.backdrop || true;
            var keyboard = allBindings.keyboard || true;
            var show = allBindings.show || false;
            var remote = allBindings.remote || false;
            //properties - custom

            //event bindings
            var onshow = allBindings.onshow || null;
            var onshown = allBindings.onshown || null;
            var onhide = allBindings.onhide || null;
            var onhiden = allBindings.onhiden || null;

            $element.modal({
                backdrop: backdrop,
                keyboard: keyboard,
                show: show,
                remote: remote
            });

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
        }
        //update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        //}
    };
})(ko);