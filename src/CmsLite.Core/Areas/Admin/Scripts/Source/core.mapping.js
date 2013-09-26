///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function ($, cms) {
    //constructors for nodes

    function nodeViewModel() {
        this.id = undefined;
        this.order = undefined;
        this.displayName = undefined;
        this.urlName = undefined;
        this.iconImageName = undefined;
        this.nodeType = undefined;
        this.pageNodes = ko.observableArray();
        this.pageTemplates = ko.observableArray();
        this.url = undefined;
    }

    function sectionNodeViewModel() {
        var self = this;
        nodeViewModel.call(self);
    }
    sectionNodeViewModel.prototype = new nodeViewModel();
    sectionNodeViewModel.constructor = nodeViewModel.constructor;

    function pageNodeViewModel() {
        var self = this;
        nodeViewModel.call(self);

        this.editUrl = undefined;
        this.parentNode = undefined;
    }
    pageNodeViewModel.prototype = new nodeViewModel();
    pageNodeViewModel.constructor = nodeViewModel.constructor;

    function sectionTemplateViewModel() {
        var self = this;
        self.id = undefined;
        self.controllerName = undefined;
        self.name = undefined;
    }

    function pageTemplateViewModel() {
        var self = this;
        self.id = undefined;
        self.actionName = undefined;
        self.name = undefined;
    }
    
    cms.mapping = {
        mapJsonToSectionNodeViewModel: function (json) {
            var section = new sectionNodeViewModel();
            section.nodeType = section instanceof sectionNodeViewModel ? "section" : undefined;
            section.id = json.Id;
            section.order = json.Order;
            section.urlName = json.UrlName;
            section.url = cms.utils.mapPath("~/" + section.urlName);
            section.displayName = json.DisplayName;
            section.iconImageName = cms.utils.mapPath('~/Areas/Admin/Content/Images/icons/' + json.IconImageName);
            section.isSelected = ko.observable(false);
            ko.utils.arrayForEach(json.PageNodes, function (pageNode) {
                var page = cms.mapping.mapJsonToPageNodeViewModel(pageNode, section);
                section.pageNodes.push(page);
            });
            ko.utils.arrayForEach(json.PageTemplates, function (pageTemplate) {
                var pageTemp = cms.mapping.mapJsonToPageTemplateViewModel(pageTemplate);
                section.pageTemplates.push(pageTemp);
            });
            return section;
        },
        mapJsonToSectionTemplate: function (json) {
            var sectionTemplate = new sectionTemplateViewModel();
            sectionTemplate.id = json.Id;
            sectionTemplate.controllerName = json.ControllerName;
            sectionTemplate.name = json.Name;
            return sectionTemplate;
        },
        mapJsonToPageNodeViewModel: function (json, parentNode) {
            var page = new pageNodeViewModel();
            page.nodeType = page instanceof pageNodeViewModel ? "page" : undefined;
            page.id = json.Id;
            page.order = json.Order;
            page.displayName = json.DisplayName;
            page.urlName = parentNode ? parentNode.url + "/" + json.UrlName : null;
            page.url = parentNode ? parentNode.url + "/" + json.UrlName : null;
            page.iconImageName = cms.utils.mapPath('~/Areas/Admin/Content/Images/icons/' + json.IconImageName);
            page.parentNode = parentNode;
            page.editUrl = cms.utils.mapPath("~/Admin/Pages/EditPage/" + json.Id);
            page.isSelected = ko.observable(false);
            ko.utils.arrayForEach(json.PageNodes, function (pageNode) {
                var newPage = cms.mapping.mapJsonToPageNodeViewModel(pageNode, page);
                page.pageNodes.push(newPage);
            });
            ko.utils.arrayForEach(json.PageTemplates, function (pageTemplate) {
                var pageTemp = cms.mapping.mapJsonToPageTemplateViewModel(pageTemplate);
                page.pageTemplates.push(pageTemp);
            });
            return page;
        },
        mapJsonToPageTemplateViewModel: function (json) {
            var pageTemp = new pageTemplateViewModel();
            pageTemp.id = json.Id;
            pageTemp.actionName = json.ActionName;
            pageTemp.name = json.Name;
            return pageTemp;
        }
    };
}($, cms));