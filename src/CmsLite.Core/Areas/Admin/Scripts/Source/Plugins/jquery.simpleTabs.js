/*
* jQuery Simple Tabs plugin 
*
* options:
*
* methods:
*
*/
(function ($) {
    $.widget('cmslite.simpleTabs', {
        options: {
            tabs: null,                     //getter function for tabs
            tabSections: null,              //getter function for tab sections
            defaultTabIndex: 0,
            tabActiveClass: 'active'
        },
        _tabContainer: null,
        _tabs: null,
        _tabSections: null,
        _create: function () {
            var self = this;
            self._tabContainer = self.element;

            //setup tabs
            if (!$.isFunction(self.options.tabs)) {
                throw new Error('You must provide a function that returns your tabs as jquery objects. Your function can take a param and will be passed the container this plugin is called on.');
            } else {
                self._tabs = self.options.tabs.call(this, self._tabContainer);
                self._tabs.click(function (e) {
                    var tab = $(this);
                    e.preventDefault();
                    var tabIndex = tab.index();
                    var tabSectionForIndex = self._tabSections[tabIndex];
                    $(self._tabSections).hide();
                    $(tabSectionForIndex).show();
                    $(self._tabs).removeClass(self.options.tabActiveClass);
                    $(this).addClass(self.options.tabActiveClass);
                });
            }

            //setup tab sections
            if (!$.isFunction(self.options.tabSections)) {
                throw new Error('You must provide a function that returns your tab sections as jquery objects. Your function can take a param and will be passed the container this plugin is called on.');
            } else {
                self._tabSections = self.options.tabSections.call(this, self._tabContainer);
                self._tabSections.hide();
                $(self._tabSections[self.options.defaultTabIndex]).show();
                $(self._tabs[self.options.defaultTabIndex]).addClass(self.options.tabActiveClass);
            }
        },
        _init: function () {
        },
        showTab: function () {
        },
        _destroy: function () {
        }
    });
} (jQuery));