/*
* jQuery carousel plugin 
* v 1.0 / 8/10/2012
* Invoke this plugin on a form element that contains an input[type="file], and a button[type="submit"]
*/
(function ($) {
    $.widget('timOfficer.fileUpload', {
        options: {
            formElement: null,
            newButtonClassName: 'fileUpload-browse',
            newButtonText: 'Browse',
            newInputClassName: 'fileUpload-input',
            fileInputClickCallback: undefined,
            inputOnChangeCallback: undefined
        },
        _fileInputElement: undefined,
        _newButtonElement: undefined,
        _newFileInputElement: undefined,
        _create: function () {
        },
        _init: function () {
            var self = this;
            var isInputElement = self.element.is("input");
            if (!isInputElement) {
                throw new Error("The element must be a input element.");
            }
            self._fileInputElement = this.element;
            self._newFileInputElement = $('<input />');
            self._newFileInputElement.addClass(self.options.newInputClassName);
            self._fileInputElement.hide();
            self._newButtonElement = $('<button></button>');
            self._newButtonElement.addClass(self.options.newButtonClassName);
            self._newButtonElement.text(self.options.newButtonText);
            //events
            self._newButtonElement.click(function (event) {
                event.preventDefault();
                self._fileInputElement.click();
                if ($.isFunction(self.options.fileInputClickCallback)) {
                    self.options.fileInputClickCallback.call(self._fileInputElement, event);
                }
            });
            self._newFileInputElement.click(function (event) {
                event.preventDefault();
                self._fileInputElement.click();
                if ($.isFunction(self.options.fileInputClickCallback)) {
                    self.options.fileInputClickCallback.call(self._fileInputElement, event);
                }
            });
            self._fileInputElement.change(function (event) {
                self._newFileInputElement.val(self._fileInputElement.val());
                if ($.isFunction(self.options.inputOnChangeCallback)) {
                    self.options.inputOnChangeCallback.call(self._fileInputElement, event);
                }
            });
            //inject elements
            self._fileInputElement.after(self._newFileInputElement);
            self._newFileInputElement.after(self._newButtonElement);
        }

    });
} (jQuery));