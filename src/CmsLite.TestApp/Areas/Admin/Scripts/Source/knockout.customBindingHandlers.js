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
ko.bindingHandlers.bootstrappopover = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        
        var value = valueAccessor(), allBindings = allBindingsAccessor();
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        var animation = allBindings.animation || true;
        var html = allBindings.html || 'right';
        var placement = allBindings.placement || 'right';
        var selector = allBindings.selector || false;
        var trigger = allBindings.trigger || 'click';
        var title = allBindings.title || '';
        //var content = allBindings.content || '';
        var content = $(element).next(allBindings.content).html();
        var delay = allBindings.delay || 0;
        var container = allBindings.container || false;

        $(element).popover({
            animation: animation,
            html: html,
            placement: placement,
            selector: selector,
            trigger: trigger,
            title: title,
            content: content,
            delay: delay,
            container: container
        });
        
        //$(element).mousedown(function (event) {
        //    event.preventDefault();
        //    event.stopPropagation();

        //    switch (event.which) {
        //        case 1:
        //            //alert('Left mouse button pressed');
        //            break;
        //        case 2:
        //            //alert('Middle mouse button pressed');
        //            break;
        //        case 3:
        //            valueUnwrapped.call(viewModel);
        //            break;
        //        default:
        //            //alert('You have a strange mouse');
        //    }
        //});
    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
    }
};

