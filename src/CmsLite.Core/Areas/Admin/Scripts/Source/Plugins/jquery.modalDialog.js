/*
* options:
*       className: css class for the overlay element
*       styles: object containing css styles to be applied to overlay element
*       animateShow: collection of objects used when animating the overlay element in. arguments are the same as jQuery animate method
*       animateHide: collection of objects used when animating the overlay element out. arguments are the same as jQuery animate method
*       beforeShow: function that gets called before animating the overlay element in.  **to specify a function to be called after the element is animated in, use the jQuery animate callback
*       beforeHide: function that gets called before animating the overlay element out.  **to specify a function to be called after the element is animated out, use the jQuery animate callback
* methods:
*       show: *removes any other elements with the same class as the plugin first. applies all the styles specified in the options. appends the element to the body, 
*             calls the beforeShow function, then animates the element in using animate properies specified in options.
*       hide: calls the beforeHide function, then animates the element out using animate properies specified in options, then removes it from the DOM.
* utilities:
*       setDefaults: takes an object of settings to apply to all widget instances
*/
(function ($) {
    $.widget('cms.modalDialog', {
        options: {
            //default options
            overlayClassName: 'modalDialog-overlay',
            containerClassName: 'modalDialog-container',
            overlayStyles: {
                position: 'fixed',
                top: '0',
                background: '#FFFFFF',
                height: '100%',
                width: '100%',
                'z-index': '2000',
                opacity: '.4',
                display: 'block'
            },
            containerStyles: {
                position: 'fixed',
                'z-index': '2100',
                display: 'block',
                overflow: 'hidden'
            },
            clickToClose: true,
            escToClose: true,
            //callback functions
            onShow: null,
            onHide: null,
            beforeShow: null,
            beforeHide: null,
            containerElement: null,
            speed: 500
        },
        _overlayElement: $('<div>'),
        _create: function () {
            //var self = this;
        },
        //you can set your own show function in place of modalOverlay's show method
        show: function (hideIndicator) {
            var self = this;
            self._initOverlay();
            if (self.options.containerElement !== null && !hideIndicator) {
                self._initContainer();
            }
            if (self.options.beforeShow !== null && $.isFunction(self.options.beforeShow)) {
                self.options.beforeShow.call(self, {
                    overlay: self._overlayElement,
                    container: self.options.containerElement
                });
            }
            if (self.options.onShow !== null && $.isFunction(self.options.onShow)) {
                self.options.onShow.call(self, self.options._overlayElement, self.options.containerElement);
            }
        },
        //you can set your own hide function in place of modalOverlay's close method
        //*if you use your own close function, you need to manually remove the overlay element
        hide: function () {
            var self = this;
            if (self.options.beforeHide !== null && $.isFunction(self.options.beforeHide)) {
                self.options.beforeHide.call(self, {
                    overlay: self._overlayElement,
                    container: self.options.containerElement
                });
            }
            if (self.options.onHide !== null && $.isFunction(self.options.onHide)) {
                self.options.onHide.call(this, {
                    overlay: self._overlayElement,
                    container: self.options.containerElement
                });
            } else {
                self._destroy();
            }
        },
        _destroy: function () {
            var self = this;
            self._overlayElement.hide().remove();
            if (self.options.containerElement !== null) {
                self.options.containerElement.fadeOut(self.options.speed, function () {
                    $(this).remove();
                });
            }
            if (self._overlayElement !== null) {
                self._overlayElement.fadeOut(self.options.speed, function () {
                    $(this).remove();
                });
            }
        },
        //utilies
        _initOverlay: function () {
            //setup our overlay element
            var self = this;
            self._overlayElement.addClass(self.options.overlayClassName);
            //apply css to overlay element, also resets sets styles back if we animate to open
            $(self.options.overlayStyles).each(function () {
                self._overlayElement.css(this, self.options.overlayStyles[this]);
            });
            //bind events
            if (self.options.clickToClose) {
                self._overlayElement.bind('click', function () {
                    self.hide();
                });
            }
            $(self._overlayElement).css({
                display: 'none'
            });
            //append
            $('body').append(self._overlayElement);
            self._overlayElement.fadeIn(self.options.speed);
        },
        _initContainer: function () {
            var self = this;
            self.options.containerElement.addClass(self.options.containerClassName);
            //apply css to overlay element, also resets sets styles back if we animate to open
            $(self.options.containerStyles).each(function () {
                self.options.containerElement.css(this, self.options.containerStyles[this]);
            });
            $(self.options.containerStyles).css({
                display: 'none'
            });
            $('body').append(self.options.containerElement);

            self.options.containerElement.slideDown(self.options.speed);
        }
    });
    $.cms.modalDialog.setDefaults = function (settings) {
        var elements = $(':cms-modalDialog'); //get all widget elements
        elements.each(function () { //apply each style in the settings to each widget
            var element = this;
            //            if (typeof (element) === 'object') {
            //                $(element).each(function () {
            //                    $(element).modalDialog('option', this, settings[this]);
            //                });
            //            }
            //            else {
            $(settings).each(function () {
                $(element).modalDialog('option', this, settings[this]);
            });
            //            }
        });
    };
} (jQuery));