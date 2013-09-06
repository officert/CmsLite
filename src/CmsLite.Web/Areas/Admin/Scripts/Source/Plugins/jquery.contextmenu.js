(function ($) {
    $.widget('cms.contextmenu', {
        options: {
            //settings
            speed: 100,
            onBeforeShow: undefined,
            onBeforeHide: undefined,
            onShow: undefined,
            onHide: undefined,
            contextMenuPosition: function (event) {
                return {
                    top: event.pageY,
                    left: event.pageX
                };
            },
            activeClass : 'active',
            //callbacks
            //functions
            getContextMenuFromParent: function () { throw new Error("You must supply your own getContextMenuFromParent() function to locate the context menu in relation to the parent. The function takes a single argument, which is a jquery object of the parent element that you called this plugin on."); }
        },
        _menuParentElement: null,
        _menu: null,
        _accordions: [],
        _create: function () {
            var self = this;
            self._menuParentElement = self.element;
            self._menu = self.options.getContextMenuFromParent.call(self._menuParentElement, self._menuParentElement);
            self.hide();

            $('html').click(function () {   //bind click to doc to close menus
                self.hide();
            });

            self._menuParentElement.mousedown(function (event) {
                event.preventDefault();
                event.stopPropagation();

                switch (event.which) {
                    case 1:
                        //alert('Left mouse button pressed');
                        break;
                    case 2:
                        //alert('Middle mouse button pressed');
                        break;
                    case 3:
                        self._menu.css(self.options.contextMenuPosition.call(this, event));
                        self._getAllContextMenus().contextmenu('hide');
                        self.show();
                        break;
                    default:
                        //alert('You have a strange mouse');
                }
            });
        },
        show: function (callback) {
            var self = this;

//            var windowWidth = $(window).width(), windowHeight = $(window).height(), menuOffset = self._menu.offset();

//            if ((menuOffset.top + self._menu.outerHeight(true)) > windowHeight) {
//                self._menu.addClass('top');
//                console.log('top');
//                console.log(menuOffset.top + self._menu.outerHeight(true));
//                self._menu.css({
//                    top: -menuOffset.top
//                });
//            }
//            if (menuOffset.top < 0) {
//                self._menu.addClass('bottom');
//                console.log('bottom');
//            }
            if ($.isFunction(self.options.onBeforeShow)) {
                self.options.onBeforeShow.call(self._menu);
            }
            self._menu.show(self.options.speed, function () {
                if ($.isFunction(callback)) {
                    callback.call(self);
                }
                if ($.isFunction(self.options.onShow)) {
                    self.options.onShow.call(self._menu, self._menu, self._menuParentElement);
                    self._menuParentElement.addClass(self.options.activeClass);
                }
            });
        },
        hide: function (callback) {
            var self = this;
            if ($.isFunction(self.options.onBeforeHide)) {
                self.options.onBeforeHide.call(self._menu);
            }
            self._menu.hide(self.options.speed, function () {

                if ($.isFunction(callback)) {
                    callback.call(self);
                }
                if ($.isFunction(self.options.onHide)) {
                    self.options.onHide.call(self._menu, self._menu, self._menuParentElement);
                    self._menuParentElement.removeClass(self.options.activeClass);
                }
            });
        },
        _destroy: function () {
        },
        _getAllContextMenus: function () {
            return $(':cms-contextmenu');
        }
    });
} (jQuery));