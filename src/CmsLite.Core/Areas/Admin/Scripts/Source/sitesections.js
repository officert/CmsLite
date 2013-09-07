///<reference path="~/Areas/Admin/Scripts/_references.js" />
///<reference path="~/Areas/Admin/Scripts/Libs/knockout-2.2.1.debug.js" />
///<reference path="~/Areas/Admin/Scripts/source/cms.js" />
///<reference path="~/Areas/Admin/Scripts/Libs/bootstrap.min.js" />
(function (window, $) {
    function initContextMenus() {
        var contextMenuOptions = {
            getContextMenuFromParent: function (parentElement) {
                return parentElement.next('div.context-menu');
            },
            onShow: function (menu, parentElement) {
                parentElement.addClass('active');
            },
            onHide: function (menu, parentElement) {
                parentElement.removeClass('active');
            },
            contextMenuPosition: function (event) {
                return {
                    top: event.pageY - 42,
                    left: event.pageX - 228
                };
            }
        };
        $('#site-sections > div.page-content ul li.node a').contextmenu(contextMenuOptions);
    }
    var accordionOptions = {
        collapseAll: false,
        headerElements: function () {
            return $(this).find('li.node a');
        },
        headerToContainer: function () {
            return $(this).siblings('ul');
        },
        onOpen: function () {
            $(this).children('i').removeClass('icon-caret-right');
            $(this).children('i').addClass('icon-caret-down');
            //$(this).children('img').attr('src', cms.utils.mapPath('~/Areas/Admin/Content/Images/icons/folderopen.png'));

            cms.viewmodel.allAccordionsOpen(true);
        },
        onClose: function () {
            $(this).children('i').removeClass('icon-caret-down');
            $(this).children('i').addClass('icon-caret-right');
            //$(this).children('img').attr('src', cms.utils.mapPath('~/Areas/Admin/Content/Images/icons/folder.png'));

            var accordions = $('#sitesections-content > ul').accordion('accordions');
            var anyAccordionsOpen = false;
            $(accordions).each(function () {
                this.IsOpen() ? anyAccordionsOpen = true : anyAccordionsOpen = false;
            });
            if (!anyAccordionsOpen) {
                cms.viewmodel.allAccordionsOpen(false);
            }
        },
        IsOpen: function () {
            return this.Content.is(':visible');
        }
    };
    function initAccordions(options) {
        $('#site-sections > div.page-content > ul').accordion(options);
    }

    cms.viewmodel = {
        //Properties
        allAccordionsOpen: ko.observable(false),
        toggleSectionsOpen: function () {
            var toggleSectionOpen;
            if (cms.viewmodel.allAccordionsOpen()) {
                $('#site-sections > div.page-content > ul').accordion('closeAll');
                toggleSectionOpen = false;
            } else {
                $('#site-sections > div.page-content > ul').accordion('openAll');
                toggleSectionOpen = true;
            }
            cms.viewmodel.allAccordionsOpen(toggleSectionOpen);
        },
        //modal windows
        hideLoadingModal: function () {
            $('#loading').modal('hide');
        },
        showLoadingModel: function () {
            $('#loading').modal('show');
        },
        //create sections
        sections: ko.observableArray(),
        createSectionForm: {
            sectionTemplates: ko.observableArray(),
            selectedSectionTemplateId: ko.observable(),
            displayName: ko.observable(),
            urlName: ko.observable(),
            init: (function () {
                var form = $('#create-sections-form');
                form.modal({
                    show: false
                });
                var triggerButton = $('#create-section-trigger');
                form.on('show.bs.modal', function() {
                    triggerButton.addClass('active');
                });
                form.on('hide.bs.modal', function() {
                    triggerButton.removeClass('active');
                });

                //init unobstrusive validation
                var formElement = form.children('div.modal-dialog').children('form').first();
                $.validator.unobtrusive.parse(formElement);
            })(),
            show: function (data, event) {
                var form = $('#create-sections-form');
                form.modal('show');
            },
            hide: function (data, event) {
                var form = $('#create-sections-form');
                form.modal('hide');
            },
            create: function () {
                var form = $('#create-sections-form');
                var formElement = form.children('div.modal-dialog').children('form').first();
                var isValid = formElement.valid();

                if (isValid) {
                    var formData = {
                        sectionTemplateId: cms.viewmodel.createSectionForm.selectedSectionTemplateId(),
                        displayName: cms.viewmodel.createSectionForm.displayName(),
                        urlName: cms.viewmodel.createSectionForm.urlName()
                    };

                    $.ajax({
                        url: cms.utils.mapPath('~/Admin/CreateSection'),
                        type: 'POST',
                        data: ko.toJSON(formData),
                        error: function () {
                            alert('error');
                            cms.viewmodel.hideLoadingModal();
                        },
                        beforeSend: function () {
                            cms.viewmodel.createSectionForm.hide();
                        },
                        success: function (json) {
                            var newSection = cms.utils.mapJsonToSectionViewModel(json);
                            cms.viewmodel.sections.push(newSection);
                        }
                    });
                }
            }
        },
        //delete sections
        deleteSectionForm: {
            parentNode: ko.observable(),
            init: (function () {
                var form = $('#delete-section-form');
                form.modal({
                    show: false
                });
            })(),
            show: function (data) {
                cms.viewmodel.deleteSectionForm.parentNode(data);
                var form = $('#delete-section-form');
                form.modal('show');
            },
            hide: function () {
                var form = $('#delete-section-form');
                form.modal('hide');
            },
            delete: function (data) {
                cms.viewmodel.deleteSectionForm.hide();

                var formData = {
                    Id: data.parentNode().id
                };

                $.ajax({
                    url: cms.utils.mapPath('~/Admin/DeleteSection'),
                    type: 'POST',
                    data: ko.toJSON(formData),
                    error: function () {
                        alert('error');
                        cms.viewmodel.hideLoadingModal();
                    },
                    beforeSend: function () {
                        cms.viewmodel.deleteSectionForm.hide();
                    },
                    success: function (json) {
                        var foundSection = ko.utils.arrayFirst(cms.viewmodel.sections(), function (section) {
                            return section.id === data.parentNode().id;
                        });
                        if (foundSection) {
                            cms.viewmodel.sections.remove(foundSection);
                        }
                    }
                });
            }
        },
        //create pages
        createPageForm: {
            parentNode: ko.observable(),
            selectedPageTemplateId: ko.observable(),
            displayName: ko.observable(),
            urlName: ko.observable(),
            actionName: ko.observable(),
            init: (function () {
                var form = $('#create-page-form');
                form.modal({
                    show: false
                });

                //init unobstrusive validation
                var formElement = form.children('div.modal-dialog').children('form').first();
                $.validator.unobtrusive.parse(formElement);
            })(),
            show: function (data) {
                cms.viewmodel.createPageForm.parentNode(data);
                var form = $('#create-page-form');
                form.modal('show');
            },
            hide: function () {
                var form = $('#create-page-form');
                form.modal('hide');
            },
            create: function (data) {
                var form = $('#create-page-form');
                var formElement = form.children('div.modal-dialog').children('form').first();
                var isValid = formElement.valid();

                if (isValid) {
                    var formData = {
                        parentSectionId: undefined,
                        parentPageId: undefined,
                        pageTemplateId: data.selectedPageTemplateId(),
                        displayName: data.displayName(),
                        urlName: data.urlName()
                    };
                    if (data.parentNode().nodeType === "section") {
                        formData.parentSectionId = data.parentNode().id;
                    }
                    else if (data.parentNode().nodeType === "page") {
                        formData.parentPageId = data.parentNode().id;
                    }
                    $.ajax({
                        url: cms.utils.mapPath('~/Admin/CreatePage'),
                        type: 'POST',
                        data: ko.toJSON(formData),
                        error: function () {
                            alert('error');
                            cms.viewmodel.hideLoadingModal();
                        },
                        beforeSend: function () {
                            cms.viewmodel.createPageForm.hide();
                        },
                        success: function (json) {
                            var newPage = cms.utils.mapJsonToPageViewModel(json, data);
                            data.parentNode().pageNodes.push(newPage);
                        }
                    });
                }
            }
        },
        //delete pages
        deletePageForm: {
            parentNode: ko.observable(),
            init: (function () {
                var form = $('#delete-page-form');
                form.modal({
                    show: false
                });
            })(),
            show: function (data) {
                cms.viewmodel.deletePageForm.parentNode(data);
                var form = $('#delete-page-form');
                form.modal('show');
            },
            hide: function () {
                var form = $('#delete-page-form');
                form.modal('hide');
            },
            delete: function (data) {
                alert('Not implemented.');
            }
        }
    };

    //init
    cms.init = function (sectionNodes, sectionTemplates) {
        //setup the page view model
        ko.utils.arrayForEach(sectionNodes, function (sectionNode) {
            var section = cms.utils.mapJsonToSectionViewModel(sectionNode);
            cms.viewmodel.sections.push(section);
        });
        ko.utils.arrayForEach(sectionTemplates, function (sectionTemplate) {
            var template = cms.utils.mapJsonToSectionTemplate(sectionTemplate);
            cms.viewmodel.createSectionForm.sectionTemplates.push(template);
        });

        ko.applyBindings(cms.viewmodel, $('#site-sections')[0]);

        //init plugins
        initContextMenus();
        initAccordions(accordionOptions);
        $('#site-sections > div.page-content > ul').accordion('openAll');       //start with accordions open

        //jquery ajax setup
        $.ajaxSetup({
            contentType: 'application/json',
            beforeSend: function () {
                cms.viewmodel.showLoadingModel();
            },
            complete: function () {
                cms.viewmodel.hideLoadingModal();

                initContextMenus();
                initAccordions(accordionOptions);
            }
        });

        //setup the create section form view model
        var createSectionForm = $('#create-sections-form');
        ko.applyBindings(cms.viewmodel.createSectionForm, createSectionForm[0]);

        //setup the delete section form view model
        var deleteSectionForm = $('#delete-section-form');
        ko.applyBindings(cms.viewmodel.deleteSectionForm, deleteSectionForm[0]);

        //setup the create page form view model
        var deletePageForm = $('#delete-page-form');
        ko.applyBindings(cms.viewmodel.createPageForm, deletePageForm[0]);

        //setup the create page form view model
        var createPageForm = $('#create-page-form');
        ko.applyBindings(cms.viewmodel.createPageForm, createPageForm[0]);
    };
}(window, jQuery));