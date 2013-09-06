(function ($) {
    $.widget('cms.accordion', {
        options: {
            //settings
            headerClassName: 'accordion-header',
            contentClassName: 'accordion-content',
            collapseAll: true,
            speed: 400,
            //callbacks
            onOpen: null,
            onClose: null,
            onAllOpen: null,
            onAllClose: null,
            //functions
            headerElements: null,
            headerToContainer: null
        },
        _containerElement: null,
        _accordions: [],
        _create: function () {
            var self = this;

            self._containerElement = self.element;
            if (!$.isFunction(self.options.headerElements)) {
                throw new Error("The option headerElements must be a function. Use the function to locate and return a collection jquery elements for the accordion headers.");
            }
            if (!$.isFunction(self.options.headerToContainer)) {
                throw new Error("The option headerToContainer must be a function. Use the function to locate the accordion content element in relation to the accordion header element. 'this' will reference the header element.");
            }

            var headers = self.options.headerElements.call(self.element);

            headers.each(function (i) {
                var contentForHeader = self.options.headerToContainer.call(headers[i]);
                var accordion = {
                    Header: $(headers[i]),
                    Content: $(contentForHeader),
                    hide: function () {
                        this.Content.hide();
                    },
                    Open: function () {
                        this.Content.slideDown(self.options.speed, function () {
                            if ($.isFunction(self.options.onOpen)) {
                                self.options.onOpen.call(accordion.Header);
                            }
                        });
                    },
                    Close: function () {
                        this.Content.slideUp(self.options.speed, function () {
                            if ($.isFunction(self.options.onClose)) {
                                self.options.onClose.call(accordion.Header);
                            }
                        });
                    },
                    IsOpen: function () {
                        return this.Content.is(':visible');
                    }
                };
                accordion.Header.click(function () {
                    //event.preventDefault();                  
                    if (!accordion.Content.is(':visible')) {
                        if (self.options.collapseAll) {
                            self._closeAll();
                        }
                        accordion.Open();
                    } else {
                        accordion.Close();
                    }
                });
                self._accordions.push(accordion);
            });

            self.hideAll();
        },
        _destroy: function () {
        },
        openAll: function () {
            var self = this;
            $(self._accordions).each(function () {
                this.Open();
            });
        },
        closeAll: function () {
            var self = this;
            $(self._accordions).each(function () {
                this.Close();
            });
        },
        hideAll: function () {
            var self = this;
            $(self._accordions).each(function () {
                this.hide();
            });
        },
        accordions: function () {
            var self = this;
            return self._accordions;
        }
    });
} (jQuery));