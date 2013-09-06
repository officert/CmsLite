(function ($) {
    $.widget('cmslite.selectBox', {
        options: {
            //settings
            className: 'select-box',
            defaultText: 'select...'
        },
        _create: function () {
            var self = this;
            if (self.element.prop("tagName") !== "SELECT") {
                throw new Error('You must call this plugin on a select input element.');
            }
            var inputElement = $(self.element);
            inputElement.hide();

            var options = inputElement.children('option');

            var newAnchorElement = $('<a></a>');
            newAnchorElement.text(self.options.defaultText);

            var newListElement = $('<ul></ul>');
            newListElement.addClass(self.options.className);

            options.each(function (i) {
                var option = $(this);
                var newLiElement = $('<li></li>');
                if (i == 0) {
                    newLiElement.text(option.text());
                } else {
                    newLiElement.text(option.val());
                }
                newListElement.append(newLiElement);
            });

            inputElement.after(newAnchorElement);
            newAnchorElement.after(newListElement);
        },
        _destroy: function () {
        }
    });
} (jQuery));