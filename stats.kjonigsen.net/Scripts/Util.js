/// <reference src="jquery-1.4.1.js" />

Dates =
{
    get: function (source) {
        /// <param name="source" type="String|Number|Date">The data to convert to a JS Date instance.</param>
        /// <returns type="Date" />
        var result = source;

        if (typeof result == "string" && result.indexOf("Date") != -1) {
            // asp.net JSON serialized datetime
            var rxMilliseconds = new RegExp("[^0-9]*([0-9]+)[^0-9]*");
            result = result.replace(rxMilliseconds, "$1");
        }

        if (typeof result == "string") {
            // string representation of milliseconds
            var oldResult = result;
            result = Math.round(result);

            if (result == NaN) {
                // might just be string representation of date after all
                result = new Date(oldResult);
            }
        }

        if (typeof result == "number") {
            // int representation of milliseconds
            result = new Date(result);
        }

        if (Date.prototype.isPrototypeOf(result)) {
            result = result;
        }

        return result;
    },

    getTime: function (date) {
        /// <param name="date" type="Date" optional="true">A date-time to extract the time from.</param>
        /// <returns type="String" />

        if (date == undefined) {
            date = new Date();
        }

        var hours = "" + date.getHours();
        var minutes = "" + date.getMinutes();

        if (hours.length == 1) {
            hours = "0" + hours;
        }
        if (minutes.length == 1) {
            minutes = "0" + minutes;
        }
        return hours + ":" + minutes;
    }
};

Util = {
    contains: function (haystack, needle) {
        /// <param name="haystack" type="Array">The array to search trough.</param>
        /// <param name="needle" type="Object|String">The value to look for.</param>
        /// <returns type="Boolean">True if found.</returns>
        for (var i = 0; i < haystack.length; i++) {
            if (haystack[i] == needle) {
                return true;
            }
        }
        return false;
    },

    add: function (array, value) {
        /// <param name="array" type="Array">The array to add to.</param>
        /// <param name="value" type="Object|String">The value to add.</param>
        array[array.length] = value;
    }
};

Rating = {
    getHotness: function (date, scale) {
        /// <param name="date" type="Date">Date to get "hotness" off.</param>
        /// <param name="scale" type="Number">Number to scale by. 1 = 16-hour scale range.</param>
        /// <returns type="Number" />

        if (scale == undefined) {
            scale = 1;
        }

        var now = new Date();
        var ms = (now - date);

	    var hours = ms / 1000 / 60 / 60;
	    var scaled = 1 + (hours * scale);

        var hotness = 15 - scaled;
        hotness = hotness < 0 ? 0 : hotness;

        return hotness;
    },

    getColourCode: function (hotness) {
        /// <param name="hotness" type="Number">Hotness-number from 0 to 15</param>
        /// <returns type="String" />
        var hexes = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"];

        var c1 = Math.round(hotness);
        var c2 = Math.round(hotness / 2);

        var colour = hexes[c1] + hexes[c2] + "0";

        return colour;
    }
};

Template = {
    processList: function (target, items, scale) {
        /// <param name="target" type="jQuery" />
        /// <param name="items" type="Array" />
        /// <param name="scale" type="Number">Number to scale by. 1 = 16-hour scale range.</param>
        /// <param name="template" type="String" />
        var ids = [];
        $("li", target).each(function () {
            var id = $(this).attr("data-id");
            Util.add(ids, id);
        });

        if (scale == undefined) {
            scale = 1;
        }

        var origTemplate = $("template", target).html();
        var template = origTemplate.replace("<!--", "").replace("-->", "");

        var text = "<template xmlns='http://code.kjonigsen.net/template'>" + origTemplate + "</template>";
        for (var i = 0; i < items.length; i++) {
            var item = items[i];

            item.Date = Dates.get(item.Date);

            var itemHotness = Rating.getHotness(item.Date, scale);
            item.ColourCode = Rating.getColourCode(itemHotness);

            var bStart = "";
            var bEnd = "";

            if (false == Util.contains(ids, item.ID)) {
                bStart = "<b>";
                bEnd = "</b>";
            }

            var line = template;

            var keys = ["ID", "Referer", "ShortReferer", "Time", "Name", "Url", "ColourCode"];
            for (var k = 0; k < keys.length; k++) {
                var key = keys[k];
                var value = item[key];

                var oldLine = null;
                while (oldLine != line) {
                    oldLine = line;
                    line = line.replace("{" + key + "}", value);
                }
            }

            line = line.replace("{bStart}", bStart);
            line = line.replace("{bEnd}", bEnd);

            text += line;
        }
        target.html(text);
    },

    tagUpdated: function (selector) {
        /// <param name="selector" type="String" />
        var time = Dates.getTime();
        $(selector).html("Last updated " + time + ".");
    }
};