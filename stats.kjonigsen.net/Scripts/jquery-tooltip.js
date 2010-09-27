/// <reference path="jquery-1.4.1.js"/>
/*
* jQuery tooltip plugin
*/

(function ($) {

    this.getTooltip = function (context, tooltipSelector) {
        return $(tooltipSelector, context);
    };

    var moving = function (context, tooltipSelector) {

        // setup tooltips
        $(context).mouseover(function (e) {

            var tooltip = getTooltip(this, tooltipSelector);

            var top = e.pageY + 10;
            var left = e.pageX + 20;

            tooltip.css({
                position: "absolute",
                zIndex: "9999",
                top: top,
                left: left
            });

            tooltip.show();

        }).mousemove(function (e) {

            var tooltip = getTooltip(this, tooltipSelector);
            tooltip.css({
                top: e.pageY + 10,
                left: e.pageX + 20
            });

        }).mouseout(function () {

            var tooltip = getTooltip(this, tooltipSelector);
            tooltip.delay('500').hide();

        });

    };

    var static = function (context, tooltipSelector) {

        // setup tooltips
        $(context).mouseover(function () {

            var tooltip = getTooltip(this, tooltipSelector);

            var position = $(this).position();
            var top = position.top + $(this).height();
            var left = position.left;

            var tooltipText = tooltip.html();
            $(this).append('<div id="tmpTooltip" class="tooltip" style="position: absolute; display: block; z-index: 9998; border: 1px solid black; background-color: white; opacity: 0; top: ' + top + '; left: ' + left + '; height: ' + tooltip.height() + '; width: ' + tooltip.width() + ';">' + tooltipText + '</div>');

            tooltip.css({
                position: "absolute",
                zIndex: "9999",
                top: top,
                left: left
            });

            tooltip.show();

        }).mouseout(function () {

            $("#tmpTooltip").remove();

            var tooltip = getTooltip(this, tooltipSelector);
            tooltip.delay('500').hide();

        });
    };

    $.fn.tooltip = function (tooltipSelector, followMouse) {

        // prepare parameters

        if (followMouse === NaN) {
            followMouse = false;
        }

        // hide tooltips if visible
        $(tooltipSelector, this).css("display", "none");

        if (followMouse) {
            moving(this, tooltipSelector);
        }
        else {
            static(this, tooltipSelector);
        }
    }

})(jQuery);
