///<reference path="~/Areas/Admin/Scripts/_references.js" />
///<reference path="~/Areas/Admin/Scripts/Libs/knockout-2.2.1.debug.js" />
///<reference path="~/Areas/Admin/Scripts/source/cms.js" />
(function (window, $) {
    cms.viewmodel = {
        //modal windows
        hideLoadingModal: function () {
            $('body').modalDialog('hide');
        },
        showLoadingModel: function () {
            $('body').modalDialog('option', {
                containerClassName: 'modalDialog-indicator'
            }).modalDialog('option', {
                containerElement: $('<img>').attr('src', cms.utils.mapPath('~/Areas/Admin/Content/Images/loading.gif'))
            }).modalDialog('show');
        },
        //upload files
        uploadFileForm: {
            show: function (data) {
                var form = $('#upload-file-form');

                ko.applyBindings(cms.viewmodel, form[0]);

                form.modal('show');
            },
            hide: function () {
                var form = $('#upload-file-form');

                form.modal('hide');
            }
        }
    };
    cms.init = function () {

        ko.applyBindings(cms.viewmodel, $('#edit-page')[0]);

        //init plugins
    };
} (window, jQuery));