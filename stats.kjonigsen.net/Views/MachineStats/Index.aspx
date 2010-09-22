<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<stats.kjonigsen.net.Models.MachineStats>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Index</h2>

    <table>
        <tr>
            <th>
                Updates
            </th>
            <th>
                CPU
            </th>
            <th>
                RAM
            </th>
            <th>
                WebConnections
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr id="statsRow">
            <td class="updates">
            </td>
            <td class="cpu">
                <%: item.CPU %>
            </td>
            <td class="ram">
                <%: item.RAM %>
            </td>
            <td class="www">
                <%: item.WebConnections %>
            </td>
        </tr>
    
    <% } %>

    </table>

<script language="javascript" type="text/javascript">

    var updates = 0;

    function updateStats(data) {
        var statsRow = $("#statsRow");

        $(".cpu", statsRow).html(data.CPU);
        $(".ram", statsRow).html(data.RAM);
        $(".www", statsRow).html(data.WebConnections);
        $(".updates", statsRow).html(++updates);
    }

    function setupUpdate() {
        setTimeout("getData();", 10000);
    }

    function getData() {
        $.getJSON("/MachineStats/Json", updateStats);
        setupUpdate();
    }

    setupUpdate();

/*
var data; $.getJSON("/machineStats/jsonRef", function(data2) { data = data2; } );
$("h2").append("<div id='referers'></div>");

var table = "<table><tr><th>ID</th><th>Url</th><th>Referer</th></tr>";
for (var i=0; i < data.length; i++) { table += "<tr><td>" + data[i].ID + "</td><td>" + data[i].Url + "</td><td>" + data[i].Referer + "</td></tr>"; }
table += "</table>";
$("#referers").append(table);

*/
</script>

</asp:Content>

