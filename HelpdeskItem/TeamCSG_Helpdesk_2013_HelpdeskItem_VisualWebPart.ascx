<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HelpdeskItem.ascx.cs" Inherits="helpdesk.NestedRadgridHelpdesl.HelpdeskItem" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>


<asp:Panel runat="server" ID="pnlHelpdeskItem">
    <div>
        <asp:Button runat="server" ID ="btnBackToHeldeskList" Text="Back to List" CssClass="csgbutton" OnClick="btnBackToHeldeskList_Click"  Style="display:inline-block;margin-right:10px;" />
    </div>

   <asp:PlaceHolder runat="server" ID="phTitle" Visible="false">
    <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblTitle">Title:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <asp:TextBox runat="server" ID ="txtTitle" Visible="false"></asp:TextBox>
       <asp:Label runat="server" ID="lblTitleValue" Font-Names="Calibri"></asp:Label>
   </div><br style="clear:both;" />
    </asp:PlaceHolder>
            <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblId">Item ID:</asp:Label></div>
   <div class="entry" style="margin-top:10px;"><asp:Label runat="server" ID="lblIdValue" Font-Names="Calibri"></asp:Label></div><br style="clear:both;" />
     <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblCategory">Category:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <telerik:RadComboBox runat="server" ID="ddlCategory" DataValueField="RowId" DataTextField="Name"  Visible="false" DropDownAutoWidth="Enabled" style="width:auto;" ></telerik:RadComboBox>
       <asp:Label runat="server" ID="lblCategoryValue" Visible="false" Font-Names="Calibri"></asp:Label>
   </div><br style="clear:both;" />
     <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblDescription">Description of Problem:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <telerik:RadEditor runat="server" ID="txtDescription" RenderMode="Lightweight"  >
       </telerik:RadEditor>
       <asp:Label runat="server" ID="lblDescriptionValue" Visible="false" Font-Names="Calibri"></asp:Label>
   </div>
    <br style="clear:both;" />
    <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblSubmittedFor">Submitted on Behalf of:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <asp:Label runat="server" ID="lblSubmittedForValue" Visible="false" Font-Names="Calibri"></asp:Label>
       <SharePoint:PeopleEditor ID="ppeSubmittedFor" runat="server"  Visible="false" AllowEmpty="true" AllowTypeIn="true" MultiSelect="false" SelectionSet="User" ></SharePoint:PeopleEditor>
   </div><br style="clear:both;" />
      <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblAssignedTo">Assigned To (Primary):</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <asp:Label runat="server" ID="lblAssignedToValue" Visible="false" Font-Names="Calibri"></asp:Label>
       <SharePoint:PeopleEditor ID="ppeAssignedTo" runat="server"  Visible="false" AllowEmpty="true" AllowTypeIn="true" MultiSelect="false" SelectionSet="User" ></SharePoint:PeopleEditor>
   </div><br style="clear:both;" />
 <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblStatus">Status:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <asp:Label runat="server" ID="lblStatusValue" Font-Names="Calibri"></asp:Label>
       <telerik:RadComboBox runat="server" ID="ddlStatus" Visible="false" DataValueField="RowId" DataTextField="Name" DropDownAutoWidth="Enabled" style="width:auto"> </telerik:RadComboBox>
   </div><br style="clear:both;" />
   <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblPriority">Priority:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <asp:Label runat="server" ID="lblPriorityValue" Visible="false" Font-Names="Calibri"></asp:Label>
       <telerik:RadComboBox runat="server" ID="ddlPriority" DataValueField="RowId" DataTextField="Name"  Visible="false" DropDownAutoWidth="Enabled" style="width:auto;" ></telerik:RadComboBox>
   </div><br style="clear:both;" />
       <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblComments">Resolution Questions/Comments:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <telerik:RadEditor runat="server" ID="txtComments" RenderMode="Lightweight"  >
       </telerik:RadEditor>
       <asp:Label runat="server" ID="lblCommentsValue" Visible="false" Font-Names="Calibri"></asp:Label>
   </div><br style="clear:both;" />
   <div class="label" style="margin-top:10px;width:130px;"><asp:Label runat="server" ID="lblResolutionTime" Visible="false">Resolution Time:</asp:Label></div>
   <div class="entry" style="margin-top:10px;">
       <asp:Label runat="server" ID="lblResolutionTimeValue" Visible="false" Font-Names="Calibri"></asp:Label>
       <telerik:RadNumericTextBox runat="server" ID="txtResolutionTime" Visible="false"></telerik:RadNumericTextBox>
   </div><br style="clear:both;" />

    <div class="label" style="margin-top:10px;width:130px;"></div>
    <div class="entry" style="margin-top:10px;">
        <asp:Panel runat="server" ID="pnlNewFiles">
            <asp:FileUpload runat="server" ID="fuNewFiles" AllowMultiple="true" />
        </asp:Panel>
        <asp:PlaceHolder runat="server" ID="phFiles">  
            <asp:Label runat="server">Attachments</asp:Label>
            <asp:Panel runat="server" ID="pnlAttachments"></asp:Panel>
        </asp:PlaceHolder>
    </div>
    <br style="clear:both;" />
    <div class="label" style="margin-top:10px;width:130px;"></div>
    <div class="entry" style="margin-top:10px;"><asp:Label runat="server" ID="lblCreated">Created </asp:Label><asp:Label runat="server" ID="lblCreatedValue" Font-Names="Calibri"></asp:Label>
    <asp:Label runat="server" ID="lblCreatedBy">by </asp:Label> <asp:Label runat="server" ID="lblCreatedByValue" Font-Names="Calibri"></asp:Label></div><br style="clear:both;" /> 
    <div class="label" style="margin-top:10px;width:130px;"></div>
   <div class="entry" style="margin-top:10px;"><asp:Label runat="server" ID="lblModified">Modified </asp:Label> <asp:Label runat="server" ID="lblModifiedValue" Font-Names="Calibri"></asp:Label>
   <asp:Label runat="server" ID="lblModifiedBy">by </asp:Label> <asp:Label runat="server" ID="lblModifiedByValue" Font-Names="Calibri"></asp:Label></div><br style="clear:both;" />
   
</asp:Panel>
<asp:Panel runat="server" ID="pnlError" Visible="false" BorderStyle="Solid" BorderWidth="2" BorderColor="#981e32" 
                BackColor="Pink" ForeColor="#981e32" style="margin-top:10px;margin-bottom:10px;padding:10px;"></asp:Panel><br style="clear:both;" />
<div>
    <asp:Button runat="server" ID ="btnEdit" Text="Edit" Width="150px" CssClass="csgbutton" OnClick="btnEdit_Click" style="margin-right:150px;display:inline-block;"/>
    <asp:Button runat="server" ID ="btnSave" Text="Save" CssClass="csgbutton" OnClick="btnSave_Click" style="margin-right:10px;display:inline-block;"/>
    <asp:Button runat="server" ID ="btnCancel" Text="Cancel" CssClass="csgbutton" OnClick="btnCancel_Click" Style="display:inline-block;margin-right:10px;" OnClientClick="Close();" />
 </div>

<asp:TextBox runat="server" ID="hdnMode" Text="VIEW" Visible="false"></asp:TextBox>