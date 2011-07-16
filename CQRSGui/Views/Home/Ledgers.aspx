<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<IEnumerable<ReadModel.InventoryItemLedgersDto>>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Title" ContentPlaceHolderID="TitleContent"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
<h2>Ledgers:</h2>
        <% foreach (var x in Model)
           {%>
              
        <%:x.ChangedCount%> <br/>

    <%
           }%>
</asp:Content>
