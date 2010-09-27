<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<stats.kjonigsen.net.Models.MachineStats>" %>
<% if (false) { %> <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script> <% } %>

<h2>Machine-stats</h2>

<ul id="stats" class="compact">
    <li>CPU used: <span class="cpu"><%= Model.CPU %></span></li>
    <li>Free RAM: <span class="ram"><%= Model.RAM %></span></li>
    <li>Connections: <span class="www"><%= Model.WebConnections %></span></li>
</ul>

<script type="text/javascript">

    $(document).ready(function () {
        var interval = 10000;
        var update = function () {
            $.getJSON('<%: Url.Action ("Json", "Stats") %>', function (data) {
                var stats = $("#stats");

                $(".cpu", stats).html(data.CPU);
                $(".ram", stats).html(data.RAM);
                $(".www", stats).html(data.WebConnections);
            });

            window.setTimeout(update, interval);
        };

        window.setTimeout(update, interval);
    });

</script>

