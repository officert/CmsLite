///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function (window, $, cms) {
    
    cms.viewmodel = (function () {
        var self = this;
        //Properties
        self.selectedNode = ko.observable();
        self.selectNode = function (data) {
            cms.viewmodel.recurseNodes(self.sectionNodes(), function (node) {
                node.isSelected(false);
            });
            data.isSelected(true);
            self.selectedNode(data);
        };
        self.deselectNodes = function () {
            self.selectedNode(null);
            cms.viewmodel.recurseNodes(self.sectionNodes(), function (node) {
                node.isSelected(false);
            });
        };
        self.sectionNodes = ko.observableArray();
        //create sectionNodes
        self.createSectionForm = {
            sectionTemplates: ko.observableArray(),
            selectedSectionTemplateId: ko.observable(),
            displayName: ko.observable(),
            urlName: ko.observable(),
            show: function (data, event) {
                var form = $('#create-sections-form');
                form.modal('show');

                var triggerButton = $('#create-section-trigger');
                triggerButton.addClass('active');
            },
            hide: function (hideModal) {
                var form = $('#create-sections-form');
                if (hideModal) {
                    form.modal('hide');
                }

                var triggerButton = $('#create-section-trigger');
                triggerButton.removeClass('active');
            },
            create: function () {
                var form = $('#create-sections-form');
                var formElement = form.children('div.modal-dialog').children('form').first();
                var isValid = formElement.valid();

                if (isValid) {
                    var formData = {
                        sectionTemplateId: self.createSectionForm.selectedSectionTemplateId(),
                        displayName: self.createSectionForm.displayName(),
                        urlName: self.createSectionForm.urlName()
                    };

                    $.ajax({
                        url: cms.utils.mapPath('~/Admin/Sections/CreateSection'),
                        type: 'POST',
                        data: ko.toJSON(formData),
                        error: function () {
                            alert('error');
                        },
                        beforeSend: function () {
                            cms.viewmodel.createSectionForm.hide(true);
                        },
                        success: function (json) {
                            var newSection = cms.mapping.mapJsonToSectionNodeViewModel(json);
                            self.sectionNodes.push(newSection);
                        }
                    });
                }
            }
        };
        //delete sectionNodes
        self.deleteSectionForm = {
            parentNode: ko.observable(),
            init: (function () {
                var form = $('#delete-section-form');
                form.modal({
                    show: false
                });
                form.on('hide.bs.modal', function () {
                    self.deleteSectionForm.hide();
                });
            })(),
            show: function (data) {
                self.deleteSectionForm.parentNode(data);
                var form = $('#delete-section-form');
                form.modal('show');
            },
            hide: function (hideModal) {
                var form = $('#delete-section-form');
                if (hideModal) {
                    form.modal('hide');
                }
                form.resetValidation();
            },
            delete: function (data) {
                var formData = {
                    Id: data.parentNode().id
                };

                $.ajax({
                    url: cms.utils.mapPath('~/Admin/Sections/DeleteSection'),
                    type: 'POST',
                    data: ko.toJSON(formData),
                    error: function () {
                        alert('error');
                    },
                    beforeSend: function () {
                        self.deleteSectionForm.hide(true);
                    },
                    success: function (json) {
                        var foundSection = ko.utils.arrayFirst(self.sectionNodes(), function (section) {
                            return section.id === data.parentNode().id;
                        });
                        if (foundSection) {
                            self.sectionNodes.remove(foundSection);
                        }
                    }
                });
            }
        };
        //create pages
        self.createPageForm = {
            parentNode: ko.observable(),
            selectedPageTemplateId: ko.observable(),
            displayName: ko.observable(),
            urlName: ko.observable(),
            actionName: ko.observable(),
            show: function (data) {
                self.createPageForm.parentNode(data);
                var form = $('#create-page-form');
                form.modal('show');
            },
            hide: function (hideModal) {
                var form = $('#create-page-form');
                if (hideModal) {
                    form.modal('hide');
                }
                form.resetValidation();
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
                    } else if (data.parentNode().nodeType === "page") {
                        formData.parentPageId = data.parentNode().id;
                    }
                    $.ajax({
                        url: cms.utils.mapPath('~/Admin/Pages/CreatePage'),
                        type: 'POST',
                        data: ko.toJSON(formData),
                        error: function () {
                            alert('error');
                        },
                        beforeSend: function () {
                            self.createPageForm.hide(true);
                        },
                        success: function (json) {
                            var newPage = cms.mapping.mapJsonToPageNodeViewModel(json, data.parentNode());
                            data.parentNode().pageNodes.push(newPage);
                        }
                    });
                }
            }
        };
        //delete pages
        self.deletePageForm = {
            parentNode: ko.observable(),
            init: (function () {
                var form = $('#delete-page-form');
                form.modal({
                    show: false
                });
                form.on('hide.bs.modal', function () {
                    self.deletePageForm.hide();
                });
            })(),
            show: function (data) {
                self.deletePageForm.parentNode(data);
                var form = $('#delete-page-form');
                form.modal('show');
            },
            hide: function (hideModal) {
                var form = $('#delete-page-form');
                if (hideModal) {
                    form.modal('hide');
                }
                form.resetValidation();
            },
            delete: function (data) {
                alert('Not implemented.');
            }
        };
        //utils
        self.recurseNodes = function (nodes, delegate) {  //takes a collection of nodes and a delegate function to apply to all nodes and child nodes
            if (typeof delegate !== 'function') throw new Error("Delegate arg must be a function.");

            ko.utils.arrayForEach(nodes, function (node) {
                if (node.pageNodes() && node.pageNodes().length > 0) {
                    self.recurseNodes(node.pageNodes(), delegate);
                }
                delegate.call(this, node);
            });
        };
        self.getPageListClass = ko.computed(function() {
            return self.selectedNode() ? 'page-content col-md-9' : 'page-content col-md-12';
        });
        self.hideNodeInfo = function () {
            self.deselectNodes();
        };
        return self;
    })();

    //init
    cms.init = function (sectionNodes, sectionTemplates) {
        //setup the page view model
        ko.utils.arrayForEach(sectionNodes, function (sectionNode) {
            var section = cms.mapping.mapJsonToSectionNodeViewModel(sectionNode);
            cms.viewmodel.sectionNodes.push(section);
        });
        ko.utils.arrayForEach(sectionTemplates, function (sectionTemplate) {
            var template = cms.mapping.mapJsonToSectionTemplate(sectionTemplate);
            cms.viewmodel.createSectionForm.sectionTemplates.push(template);
        });

        ko.applyBindings(cms.viewmodel, $('#sections')[0]);

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
}(window, jQuery, cms));