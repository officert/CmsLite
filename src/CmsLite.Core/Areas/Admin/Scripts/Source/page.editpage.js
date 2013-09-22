///<reference path="~/Areas/Admin/Scripts/_references.js" />>
(function (window, $) {
    function initTabs() {
        $('#tab-bar').simpleTabs({
            tabs: function (container) {
                return container.children('ul').children('li.simple-tab');
            },
            tabSections: function (container) {
                return container.parent().parent().next('div.page-content').children('div.tab-pane');
            }
        });
    }

    cms.viewmodel = {
        pageNode: undefined,
        toggleToolbar: function () {
            var toolbar = $('#toolbar');
            var pageContent = $('div.page-content');
            if (toolbar.hasClass('collapsed')) {
                toolbar.removeClass('collapsed');
                pageContent.removeClass('collapsed');
            } else {
                toolbar.addClass('collapsed');
                pageContent.addClass('collapsed');
            }
        },
        showpreview: function () {         //this gets called by ckeditor on the custom preview button click -- /areas/admin/script/propertyplugins/richtexteditor/ckeditor/plugins/cms-custompreview/plugin.js
            window.open("/" + cms.viewmodel.pageNode.UrlName, "_blank");
        }
    };
    cms.init = function (pagNode, parentNode) {
        cms.viewmodel.pageNode = cms.utils.mapJsonToPageViewModel(pagNode, null);
        ko.applyBindings(cms.viewmodel, $('#edit-page')[0]);

        //init plugins
        initTabs();
    };
} (window, jQuery));