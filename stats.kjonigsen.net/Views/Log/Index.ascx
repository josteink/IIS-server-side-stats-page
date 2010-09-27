<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<stats.kjonigsen.net.Models.LogEntry>>" %>
<% if (false) { %>
<script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script src="../../Scripts/Util.js" type="text/javascript"></script>
<% } %>

<h2>Last requested content pages</h2>

<ul id="requests" class="compact">
    <template xmlns="http://code.kjonigsen.net/template"><!-- 
        <li data-id='{ID}' style='color: #{ColourCode};'>{bStart}{Time}: <a style='color: #{ColourCode};' href='{Url}'>{Name}</a>{bEnd}</li>
    --></template>
</ul>

<p id="logsUpdated">Updating...</p>

<script type="text/javascript">

    $(document).ready(function () {

        var interval = 60000;
        var update = function () {
            $.getJSON('<%: Url.Action ("Json", "Log") %>', function (data) {

                var logs = $("#requests");
                Template.processList(logs, data, 5);
                Template.tagUpdated("#logsUpdated");
            });

            window.setTimeout(update, interval);
        };

        update();
    });

</script>