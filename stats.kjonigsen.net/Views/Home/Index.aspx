<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<stats.kjonigsen.net.Models.CompositeStats>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	stats.kjonigsen.net
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <% Html.RenderPartial("/Views/Stats/Index.ascx", Model.Machine); %>
    <% Html.RenderPartial("/Views/Log/Index.ascx", Model.Logs); %>
    <% Html.RenderPartial("/Views/Referer/Index.ascx", Model.Referers); %>

</asp:Content>
