///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function (window, $) {
    cms.viewmodel = {
        //upload files
        uploadFileForm: {
            show: function (data) {
                var form = $('#upload-file-form');
                form.modal('show');
                
                var triggerButton = $('#add-image-trigger');
                triggerButton.addClass('active');
            },
            hide: function (hideModal) {
                var form = $('#upload-file-form');
                if (hideModal) {
                    form.modal('hide');
                }

                var triggerButton = $('#add-image-trigger');
                triggerButton.removeClass('active');
            }
        }
    };
    cms.init = function () {

        ko.applyBindings(cms.viewmodel, $('#media-page')[0]);

        //setup the create section form view model
        var uploadFileForm = $('#upload-file-form');
        ko.applyBindings(cms.viewmodel.uploadFileForm, uploadFileForm[0]);
    };
} (window, jQuery));