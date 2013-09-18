///<reference path="~/Areas/Admin/Scripts/_references.js" />
var cms = (function ($) { //define our namespace
    //constructors for nodes

    function nodeViewModel() {
        this.id = undefined;
        this.order = undefined;
        this.displayName = undefined;
        this.urlName = undefined;
        this.iconImageName = undefined;
        this.nodeType = undefined;
    }

    function sectionViewModel() {
        var self = this;
        nodeViewModel.call(self);

        this.pageNodes = ko.observableArray();
        this.pageTemplates = ko.observableArray();
        this.url = undefined;
    }

    sectionViewModel.prototype = new nodeViewModel();
    sectionViewModel.constructor = nodeViewModel.constructor;

    function pageViewModel() {
        var self = this;
        nodeViewModel.call(self);

        this.pageNodes = ko.observableArray();
        this.pageTemplates = ko.observableArray();
        this.editUrl = undefined;
        this.url = undefined;
        this.parentNode = undefined;
    }

    pageViewModel.prototype = new nodeViewModel();
    pageViewModel.constructor = nodeViewModel.constructor;

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

    //header viewmodel and functions

    function headerNavViewModel() {
        var self = this;
    }

    function initDropDowns() {
        $('#header .dropdown-toggle').dropdown();
    }

    function initTooltips() {
        $('button.btn-tooltip').tooltip();
    }

    //use this function to run anything for every page 
    // * if it's not used on every page use the init() function in a subclass to initialize page specific components
    (function () {
        var headerNav = $('#header');
        var headerViewModel = new headerNavViewModel();
        ko.applyBindings(headerViewModel, headerNav[0]);

        //init plugins
        initTooltips();
        initDropDowns();
    })();
    //

    return {
        viewmodel: undefined,
        init: function () {
            throw new Error("This method must be implemented by each page to initialize it.");
        },
        utils: {
            mapPath: undefined,
            mapJsonToSectionViewModel: function (json) {
                var section = new sectionViewModel();
                section.nodeType = section instanceof sectionViewModel ? "section" : undefined;
                section.id = json.Id;
                section.order = json.Order;
                section.urlName = json.UrlName;
                section.displayName = json.DisplayName;
                section.iconImageName = cms.utils.mapPath('~/Areas/Admin/Content/Images/icons/' + json.IconImageName);
                section.url = cms.utils.mapPath("~/" + section.urlName);
                ko.utils.arrayForEach(json.PageNodes, function (pageNode) {
                    var page = cms.utils.mapJsonToPageViewModel(pageNode, section);
                    section.pageNodes.push(page);
                });
                ko.utils.arrayForEach(json.PageTemplates, function (pageTemplate) {
                    var pageTemp = cms.utils.mapJsonToPageTemplateViewModel(pageTemplate);
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
            mapJsonToPageViewModel: function (json, parentNode) {
                var page = new pageViewModel();
                page.nodeType = page instanceof pageViewModel ? "page" : undefined;
                page.id = json.Id;
                page.order = json.Order;
                page.displayName = json.DisplayName;
                page.urlName = json.urlName;
                page.iconImageName = cms.utils.mapPath('~/Areas/Admin/Content/Images/icons/' + json.IconImageName);
                page.parentNode = parentNode;
                page.editUrl = cms.utils.mapPath("~/Admin/EditPage/" + json.Id);
                page.url = parentNode ? parentNode.url + "/" + json.UrlName : null;
                ko.utils.arrayForEach(json.PageNodes, function (pageNode) {
                    var newPage = cms.utils.mapJsonToPageViewModel(pageNode, page);
                    page.pageNodes.push(newPage);
                });
                ko.utils.arrayForEach(json.PageTemplates, function (pageTemplate) {
                    var pageTemp = cms.utils.mapJsonToPageTemplateViewModel(pageTemplate);
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
        }
    };
}($));