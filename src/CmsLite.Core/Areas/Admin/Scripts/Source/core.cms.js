///<reference path="~/Areas/Admin/Scripts/_references.js" />
var cms = (function ($) { //define our namespace

    //header viewmodel and functions
    function initTooltips() {
        $('button.btn-tooltip').tooltip();
    }

    //use this function to run anything for every page 
    // * if it's not used on every page use the init() function in a subclass to initialize page specific components
    $(document).ready(function () {
        var header = $('#header');
        var headerViewModel = {

        };
        ko.applyBindings(headerViewModel, header[0]);

        //init plugins
        initTooltips();
    });

    return {
        headerviewmodel: {},
        viewmodel: undefined,
        init: function () {
            throw new Error("This method must be implemented by each page to initialize it.");
        },
        utils: {},
        mapping: {}
    };
}($));