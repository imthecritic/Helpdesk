<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HelpdeskList.ascx.cs" Inherits="helpdesk.NestedRadgridHelpdesk.HelpdeskList" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

 <script type="text/javascript">

     function ddlAssignedToIndexChanged(sender, args) {
         var tableView = $find(m_AssignedToTableViewId);
         var filterVal = args.get_item().get_value();
         if (filterVal == "All") {
             tableView.filter("AssignedToId", filterVal, "NoFilter");
         }
         else {
             filterVal = parseInt(filterVal);
             tableView.filter("AssignedToId", filterVal, "EqualTo");
         }
     }
     function ddlCategoryIndexChanged(sender, args) {
         var tableView = $find(m_CategoryToTableViewId);
         var filterVal = args.get_item().get_value();
         if (filterVal == "All") {
             tableView.filter("CategoryId", filterVal, "NoFilter");
         }
         else {
             tableView.filter("CategoryId", filterVal, "EqualTo");
         }
     }
     function ddlStatusIndexChanged(sender, args) {
         var tableView = $find(m_StatusToTableViewId);
         var filterVal = args.get_item().get_value();
         if (filterVal == "All") {
             tableView.filter("StatusId", filterVal, "NoFilter");
         }
         else {
             tableView.filter("StatusId", filterVal, "EqualTo");
         }
     }
     function ddlPriorityIndexChanged(sender, args) {
         var tableView = $find(m_PriorityToTableViewId);
         var filterVal = args.get_item().get_value();
         if (filterVal == "All") {
             tableView.filter("PriorityId", filterVal, "NoFilter");
         }
         else {
             tableView.filter("PriorityId", filterVal, "EqualTo");
         }
     }

     function hasClass(element, cls) { // not used
         return (' ' + element.className + ' ').indexOf(' ' + cls + ' ') > -1; 
     }


     function onGridCreate() { // not used
         var elements = document.getElementsByTagName("tr");
            for (var i = 0; i < elements.length; i++) {
                if (elements[i].className == "rgPager") {
                    elements[i].style.visibility = 'hidden';
                    elements[i].style.display = 'none';
                 }
            }
     }

     function expandCollapseGrids(string) {
         var pageElements = document.getElementsByTagName("tr");
         for (var n = 0; n < pageElements.length; n++) {
             if (pageElements[n].className == "rgPager") {
                 pageElements[n].style.visibility = 'hidden';
                 pageElements[n].style.display = 'none';
             }
         }

         var expandedGrids = string.split('@');
         for (var i = 0; i < expandedGrids.length-1; i++) {
                 var grid = document.getElementById(expandedGrids[i]);
                 var elements = grid.getElementsByTagName("tr");
                 for (var j = 0; j < elements.length; j++) {
                     if (elements[j].className == "rgPager") {
                         elements[j].style.visibility = 'visible';
                         elements[j].style.display = 'table-row';
                     }
                 }
             }
     }

</script>

<style type="text/css"> 
    .RadGrid_Default .rgMasterTable, .RadGrid_Default .rgDetailTable, .RadGrid_Default .rgGroupPanel table, .RadGrid_Default .rgCommandRow table, .RadGrid_Default .rgEditForm table, .RadGrid_Default .rgPager table {
        table-layout:fixed !important;
    }
</style>

<div style ="width:100%">
<asp:HiddenField ID="gvHiddenField" runat="server" value="" />
<asp:HiddenField ID="hdnExpandedGridIds" runat="server" value="" ClientIDMode="static" />
<asp:TextBox runat="server" ID="txtSearch" placeholder= "Search the Helpdesk." OnTextChanged="txtName_TextChanged" AutoPostBack="true" 
            ToolTip="Search Helpdesk" Font-Names="Calibri" Style="margin-bottom: 10px" Width="250px"></asp:TextBox>
            <img src="../../_layouts/15/Helpdesk/images/view.png" style="width:20px;height:20px;margin-left:3px;vertical-align:top;cursor:pointer;" />
            <asp:Button runat="server" ID="btnClearFilters" OnClick="btnClearFilters_Click" CssClass="button" Text ="Clear Filters" style="margin-right:10px;display:inline-block;"/>
            <asp:Button runat="server" ID="btnNewItem" OnClick="btnNewItem_Click" CssClass="button" Text ="New Item" style="margin-right:10px;display:inline-block;"/>
</div>
<telerik:RadGrid ID="gvOuter" runat="server" AutoGenerateColumns="False" OnItemDataBound="gvOuter_ItemDataBound" OnNeedDataSource="gvOuter_NeedDataSource"
    AllowPaging="false" AllowSorting="False" GridLines="None" ShowStatusBar="False" ShowGroupPanel="false"
    ShowHeader="false" AlternatingItemStyle-BackColor="White" ItemStyle-BackColor="White" BorderWidth="0px" PageSize="500" HeaderStyle-CssClass ="gridheader" >
    <ClientSettings>
        <Resizing AllowColumnResize="false" />
        <Selecting AllowRowSelect="false" />
    </ClientSettings>
    <MasterTableView HierarchyDefaultExpanded="true" DataKeyNames="StatusId" AlternatingItemStyle-BackColor="White" ItemStyle-BackColor="White" CommandItemDisplay="None" AllowFilteringByColumn="false"
        NoMasterRecordsText="No tasks." 
        ShowFooter="False">
        <Columns>
            <telerik:GridTemplateColumn >
                <ItemTemplate>
                    <div>            
<telerik:RadGrid ID ="gvHelpdesk" runat="server" MasterTableView-NoDetailRecordsText="" EnableLinqExpressions="false" OnItemDataBound="gvHelpdesk_ItemDataBound" OnPreRender="gvHelpdesk_PreRender" 
    OnItemCommand="gvHelpdesk_ItemCommand" OnSortCommand="gvHelpdesk_SortCommand" OnPageIndexChanged="gvHelpdesk_PageIndexChanged" 
    OnPageSizeChanged="gvHelpdesk_PageSizeChanged" AutoGenerateColumns="false"     ClientSettings-AllowGroupExpandCollapse="true" 
  AllowSorting ="true" AllowPaging="true" PageSize="20" AlternatingItemStyle-BackColor ="#B8CFFF"  HeaderStyle-CssClass="gridheader">
   <GroupingSettings GroupContinuesFormatString="" GroupContinuedFormatString=""  GroupSplitDisplayFormat="" GroupSplitFormat="" />
     <MasterTableView CommandItemDisplay="Top" EditMode="InPlace" DataKeyNames="RowId,StatusId" AllowFilteringByColumn="true" > <CommandItemSettings ShowAddNewRecordButton="false" ShowRefreshButton="false" />
         <GroupByExpressions>
            <telerik:GridGroupByExpression >
                <SelectFields>
                    <telerik:GridGroupByField FieldAlias="StatusName" FieldName="StatusName" 
                        HeaderText="Status" HeaderValueSeparator=": " FormatString="<strong>{0}</strong>">
                    </telerik:GridGroupByField>
                </SelectFields>
                <GroupByFields>
                    <telerik:GridGroupByField FieldName="StatusName" SortOrder="Ascending" >
                    </telerik:GridGroupByField>
                </GroupByFields>
                </telerik:GridGroupByExpression>
        </GroupByExpressions>
        <NoRecordsTemplate></NoRecordsTemplate>
        <Columns>
             <telerik:GridTemplateColumn DataField="RowId" HeaderText="Item ID"  AllowSorting="true" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label ID="lblID" runat="server" Text='<%# Eval("RowId") %>' />
                </ItemTemplate>
                <HeaderStyle Width="50px" />
                <ItemStyle Width="50px" />
             </telerik:GridTemplateColumn>   
             <telerik:GridTemplateColumn DataField="Title" HeaderText="Title" AllowSorting="true" SortExpression="Title" AllowFiltering="false">
                <ItemTemplate>
                        <asp:LinkButton runat="server"  ID="lblTitle" OnClick="lblTitle_Click" CommandArgument=<%# Eval("RowId") %> Text ='<%# Eval("Title") %>' > </asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle Width="200px" />
                <ItemStyle Width="200px" />
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="CategoryId" UniqueName="CategoryId" HeaderText="Category" AllowSorting="true" SortExpression="CategoryName">
                <ItemTemplate>
                    <asp:Label ID="lblCategory" runat="server" Text='<%# Eval("CategoryName") %>' />
                </ItemTemplate>
                <HeaderStyle Width="175px" />
                <ItemStyle Width="175px" />
                <FilterTemplate>
                    <telerik:RadComboBox ID="ddlCategoryFilter" OnClientSelectedIndexChanged="ddlCategoryIndexChanged" AllowCustomText="false" runat="server" />
                 </FilterTemplate>
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="AssignedToId" UniqueName="AssignedToId" HeaderText="Assigned To" AllowSorting="true" SortExpression="AssignedToUserTitle" AllowFiltering="true">
                <ItemTemplate>
                    <asp:Label ID="lblAssignedTo" runat="server" Text='<%# Eval("AssignedToUserTitle") %>' />
                </ItemTemplate>
                 <FilterTemplate>
                    <telerik:RadComboBox ID="ddlAssignedToFilter" OnClientSelectedIndexChanged="ddlAssignedToIndexChanged" AllowCustomText="false" runat="server" />
                 </FilterTemplate>
                <HeaderStyle Width="175px" />
                <ItemStyle Width="175px" />
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="StatusId" UniqueName="StatusId" HeaderText="Status" AllowSorting="true" SortExpression ="StatusName">
                <ItemTemplate>
                    <asp:Label ID="lblStatus" runat="server" Text='<%# Eval("StatusName") %>' />
                </ItemTemplate>
                <HeaderStyle Width="175px" />
                <ItemStyle Width="175px" />
                   <FilterTemplate>
                    <telerik:RadComboBox ID="ddlStatusFilter" OnClientSelectedIndexChanged="ddlStatusIndexChanged" AllowCustomText="false" runat="server" />
                 </FilterTemplate>
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="PriorityId" UniqueName="PriorityId" HeaderText="Priority" AllowSorting="true" SortExpression ="PriorityName">
                <ItemTemplate>
                    <asp:Label ID="lblPriority" runat="server" Text='<%# Eval("PriorityName") %>' />
                </ItemTemplate>
                 <HeaderStyle Width="175px" />
                <ItemStyle Width="175px" />
                   <FilterTemplate>
                    <telerik:RadComboBox ID="ddlPriorityFilter" OnClientSelectedIndexChanged="ddlPriorityIndexChanged" AllowCustomText="false" runat="server" />
                 </FilterTemplate>
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="Description" HeaderText="Description" AllowSorting="true" SortExpression ="Description" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label ID="lblDescription" runat="server" Text='<%# MakeShort(Convert.ToString(Eval("Description"))) %>' />
                 <HeaderStyle Width="50px" />
                <ItemStyle Width="50px" />
                </ItemTemplate>
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="CreatedById" HeaderText="Created By" AllowSorting="true" SortExpression="CreatedByUserTitle" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label ID="lblCreatedBy" runat="server" Text='<%# Eval("CreatedByUserTitle") %>' />
                </ItemTemplate>
                 <HeaderStyle Width="100px" />
                <ItemStyle Width="100px" />
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="ModifiedById" HeaderText="Modified By" AllowSorting="true" SortExpression="ModifiedByUserTitle" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label ID="lblModifiedBy" runat="server" Text='<%# Eval("ModifiedByUserTitle") %>' />
                </ItemTemplate>
                 <HeaderStyle Width="100px" />
                <ItemStyle Width="100px" />
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="Created" HeaderText="Created" AllowSorting="true" SortExpression="Created" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label ID="lblCreated" runat="server" Text='<%# Eval("Created", "{0:d}") %>' />
                </ItemTemplate>
                 <HeaderStyle Width="75px" />
                <ItemStyle Width="75px" />
             </telerik:GridTemplateColumn>
             <telerik:GridTemplateColumn DataField="Modified" HeaderText="Modified" AllowSorting="true" SortExpression="Modified" AllowFiltering="false">
                <ItemTemplate>
                    <asp:Label ID="lblModified" runat="server" Text='<%# Eval("Modified", "{0:d}") %>' />
                </ItemTemplate>
                 <HeaderStyle Width="75px" />
                <ItemStyle Width="75px" />
             </telerik:GridTemplateColumn>
        </Columns>
     </MasterTableView>
</telerik:RadGrid>
</div>
              </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

