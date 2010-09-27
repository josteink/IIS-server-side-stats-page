<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<stats.kjonigsen.net.Models.LogEntry>>" %>
<% if (false) { %>
<script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="../../Scripts/jquery-tooltip.js" type="text/javascript"></script>
<script src="../../Scripts/Util.js" type="text/javascript"></script>
<% } %>

<h2>Last unique referers</h2>

<ul id="referers" class="compact">
    <template xmlns="http://code.kjonigsen.net/template"><!-- 
        <li data-id="{ID}" style="color: #{ColourCode};">{bStart}{Time}: <a href="{Referer}" style="color: #{ColourCode};">{ShortReferer}</a>{bEnd}
            <div class="tooltip">
                Full URL: <a href="{Referer}">{Referer}</a><br />
                For <a href="{Url}">{Name}</a>
            </div>
        </li>
    --></template>
</ul>

<p id="referersUpdated">Updating...</p>

<script type="text/javascript">

    $(document).ready(function () {

        var interval = 60000;
        var update = function () {
            $.getJSON('<%: Url.Action ("Json", "Referer") %>', function (data) {

                var logs = $("#referers");
                Template.processList(logs, data);
                $("li", logs).tooltip("div.tooltip");
                Template.tagUpdated("#referersUpdated");
            });

            window.setTimeout(update, interval);
        };

        update();
    });

</script>
