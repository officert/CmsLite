///<reference path="~/Areas/Admin/Scripts/_references.js" />
var cms = (function ($) { //define our namespace

    (function () {
        var header = $('#header');
        var headerViewModel = {

        };
        ko.applyBindings(headerViewModel, header[0]);

        //j!uery ajax setup
        $.ajaxSetup({
            contentType: 'application/json',
            beforeSend: function () {
            },
            complete: function () {
            }
        });
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