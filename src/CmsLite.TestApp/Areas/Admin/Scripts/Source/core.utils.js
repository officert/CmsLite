///<reference path="~/Areas/Admin/Scripts/_references.js" />
(function ($, cms) {

    var utils = (function () {
        var self = this;
        self.mapPath = undefined;
        self.formatDateFromJson = function (jsonDate) {
            //uses moment.js - http://momentjs.com/docs/
            return jsonDate ? moment(new Date(parseInt(jsonDate.substr(6), 10))).format('MM/DD/YYYY') : '';
        };
        self.formatDateTimeFromJson = function (jsonDate) {
            //uses moment.js - http://momentjs.com/docs/
            return jsonDate ? moment(new Date(parseInt(jsonDate.substr(6), 10))).format('MM/DD/YYYY @ hh:mm A') : '';
        };
        return self;
    }());

    cms.utils = utils;
}($, cms));