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
            init: (function () {
                var form = $('#upload-file-form');
                form.modal({
                    show: false
                });
                var triggerButton = $('#create-section-trigger');
                form.on('show.bs.modal', function () {
                    triggerButton.addClass('active');
                });
                form.on('hide.bs.modal', function () {
                    triggerButton.removeClass('active');
                });

                //init unobstrusive validation
                var formElement = form.children('div.modal-dialog').children('form').first();
                $.validator.unobtrusive.parse(formElement);
            })(),
            show: function (data) {
                var form = $('#upload-file-form');
                form.modal('show');
            },
            hide: function () {
                var form = $('#upload-file-form');
                form.modal('hide');
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