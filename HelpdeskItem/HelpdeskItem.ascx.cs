using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Helpdesk.BLL.Alert;
using Helpdesk.BLL.List;
using Helpdesk.BLL.Reference;
using Helpdesk.DAL.Alert;
using Helpdesk.Utility;
using Telerik.Web.UI;

namespace NestedRadgridHelpdesk.HelpdeskItem
{
    [ToolboxItemAttribute(false)]
    public partial class HelpdeskItem : WebPart
    {
        private const string _versionNumber = "1.5";
        private HelpdeskItem _item = null;
        private bool _isNew = false;
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        //[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public HelpdeskItem()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.CssClass = "helpdesk-item-webpart";
            bool hasLink = false;
            HtmlLink linkCss = new HtmlLink();

            string fileName = "Lists.css";
            string RESOURCE_PATH = "../../_layouts/15/Helpdesk/css/";


            linkCss.ID = "helpdesk_css";
            linkCss.Attributes["type"] = "text/css";
            linkCss.Attributes["rel"] = "stylesheet";
            linkCss.Attributes["href"] = base.ResolveUrl(RESOURCE_PATH) + fileName + "?version=" + _versionNumber;

            foreach (Control ctl in this.Page.Header.Controls)
            {
                if (linkCss.ID == ctl.ID)
                {
                    hasLink = true;
                    break;
                }

            }

            if (hasLink == false)
            {
                //Add HtmlLink instance to the header of the current page
                Page.Header.Controls.Add(linkCss);
            }


            if (this.Page.Request.QueryString["HelpdeskItemId"] == null)
            {
                _item = new HelpdeskItem();
                _isNew = true;
                hdnMode.Text = "EDIT";
                SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
                _item.CreatedById = currentUser.ID;
                _item.Created = DateTime.Now;
                _item.StatusId = (int)Status.Types.New;
                _item.PriorityId = (int)Priority.Types.Medium;
            }

            else
            {
                int helpdeskItemId = int.Parse(this.Page.Request.QueryString["HelpdeskItemId"]);

                _item = new HelpdeskItem(helpdeskItemId);
            }

            if (SystemSettings.RootUrl == "")
                SystemSettings.Refresh();

            if (Page.IsPostBack == false)
            {
                PopulatePage();
                BindDropdowns();
                ShowHideControls();
            }
            ShowAttachments();
        }

        protected void SetToolsForEditor()
        {
            EditorToolGroup main = new EditorToolGroup();
            txtDescription.Tools.Add(main);
            txtComments.Tools.Add(main);

            EditorTool bold = new EditorTool();
            bold.Name = "Bold";
            bold.ShortCut = "CTRL+B";
            main.Tools.Add(bold);

            EditorTool italic = new EditorTool();
            italic.Name = "Italic";
            italic.ShortCut = "CTRL+I";
            main.Tools.Add(italic);

            EditorTool underline = new EditorTool();
            underline.Name = "Underline";
            underline.ShortCut = "CTRL+U";
            main.Tools.Add(underline);
        }

        private void PopulatePage()
        {
            lblTitleValue.Text = _item.Title;
            if (_item.RowId > 0)
            {
                lblIdValue.Text = _item.RowId.ToString();
            }
            else
            {
                lblIdValue.Text = "NEW";
                lblModifiedBy.Visible = false;
                lblModifiedByValue.Visible = false;
                lblModified.Visible = false;
                lblModifiedValue.Visible = false;

            }
            txtTitle.Text = _item.Title;
            lblStatusValue.Text = _item.StatusName;
            try
            {
                lblCreatedByValue.Text = _item.CreatedByUserTitle;
                lblModifiedByValue.Text = _item.ModifiedByUserTitle;
            }
            catch (Exception ex)
            {

            }
            lblSubmittedForValue.Text = _item.SubmittedForTitle;
            lblPriorityValue.Text = _item.PriorityName;
            lblCategoryValue.Text = _item.CategoryName;
            lblModifiedValue.Text = _item.Modified.ToString("f",
                        CultureInfo.CreateSpecificCulture("en-US"));
            lblCreatedValue.Text = _item.Created.ToString("f",
                        CultureInfo.CreateSpecificCulture("en-US"));
            lblAssignedToValue.Text = _item.AssignedToUserTitle;
            lblDescriptionValue.Text = _item.Description;
            lblCommentsValue.Text = _item.ResolutionComments;
            SetToolsForEditor();
            txtDescription.Content = _item.Description;
            txtComments.Content = _item.ResolutionComments;
            if (_item.ResolutionTime != null)
            {
                txtResolutionTime.Value = (double)_item.ResolutionTime;
                lblResolutionTimeValue.Text = _item.ResolutionTime.Value.ToString("F2");

            }
            else
            {
                txtResolutionTime.Value = null;
                lblResolutionTimeValue.Text = "N/A";
            }
        }

        private void ShowHideControls()
        {
            if (hdnMode.Text == "VIEW")
            {
                pnlNewFiles.Visible = false;

                if (IsOwner())
                {
                    phTitle.Visible = true;
                    lblTitleValue.Visible = true;
                    txtTitle.Visible = false;
                    lblCategoryValue.Visible = true;
                    ddlCategory.Visible = false;
                    lblPriority.Visible = true;
                    lblPriorityValue.Visible = true;
                    ddlPriority.Visible = false;
                    lblStatus.Visible = true;
                    lblStatusValue.Visible = true;
                    ddlStatus.Visible = false;
                    lblDescriptionValue.Visible = true;
                    txtDescription.Visible = false;
                    lblSubmittedFor.Visible = true;
                    lblSubmittedForValue.Visible = true;
                    ppeSubmittedFor.Visible = false;
                    lblComments.Visible = true;
                    lblCommentsValue.Visible = true;
                    txtComments.Visible = false;
                    lblAssignedTo.Visible = true;
                    lblAssignedToValue.Visible = true;
                    ppeAssignedTo.Visible = false;
                    lblCreated.Visible = true;
                    lblCreatedValue.Visible = true;
                    lblCreatedBy.Visible = true;
                    lblCreatedByValue.Visible = true;
                    lblModified.Visible = true;
                    lblModifiedValue.Visible = true;
                    lblModifiedBy.Visible = true;
                    lblModifiedByValue.Visible = true;
                    lblResolutionTime.Visible = false;
                    lblResolutionTimeValue.Visible = false;
                    txtResolutionTime.Visible = false;
                    btnCancel.Visible = false;
                    btnSave.Visible = false;
                    btnEdit.Visible = true;
                    pnlError.Visible = false;
                }
                else if (IsAdmin())
                {
                    phTitle.Visible = true;
                    lblTitleValue.Visible = true;
                    txtTitle.Visible = false;
                    lblCategoryValue.Visible = true;
                    ddlCategory.Visible = false;
                    lblPriorityValue.Visible = true;
                    ddlPriority.Visible = false;
                    lblStatusValue.Visible = true;
                    ddlStatus.Visible = false;
                    lblDescriptionValue.Visible = true;
                    txtDescription.Visible = false;
                    lblCommentsValue.Visible = true;
                    txtComments.Visible = false;
                    lblAssignedToValue.Visible = true;
                    ppeAssignedTo.Visible = false;
                    lblCreated.Visible = true;
                    lblSubmittedFor.Visible = true;
                    lblSubmittedForValue.Visible = true;
                    ppeSubmittedFor.Visible = false;
                    lblCreatedValue.Visible = true;
                    lblCreatedBy.Visible = true;
                    lblCreatedByValue.Visible = true;
                    lblModified.Visible = true;
                    lblModifiedValue.Visible = true;
                    lblModifiedBy.Visible = true;
                    lblModifiedByValue.Visible = true;
                    lblResolutionTime.Visible = true;
                    lblResolutionTimeValue.Visible = true;
                    txtResolutionTime.Visible = false;
                    btnCancel.Visible = false;
                    btnSave.Visible = false;
                    btnEdit.Visible = true;
                    pnlError.Visible = false;


                }
                else
                {
                    phTitle.Visible = true;
                    lblTitleValue.Visible = true;
                    txtTitle.Visible = false;
                    lblCategoryValue.Visible = true;
                    ddlCategory.Visible = false;
                    lblPriorityValue.Visible = true;
                    ddlPriority.Visible = false;
                    lblStatusValue.Visible = true;
                    ddlStatus.Visible = false;
                    lblDescriptionValue.Visible = true;
                    txtDescription.Visible = false;
                    lblCommentsValue.Visible = true;
                    txtComments.Visible = false;
                    lblAssignedToValue.Visible = true;
                    ppeAssignedTo.Visible = false;
                    lblSubmittedFor.Visible = false;
                    lblSubmittedForValue.Visible = false;
                    ppeSubmittedFor.Visible = false;
                    btnCancel.Visible = false;
                    btnSave.Visible = false;
                    btnEdit.Visible = false;
                    pnlError.Visible = false;

                }

            }

            else if (hdnMode.Text == "EDIT")
            {
                if (IsAdmin())
                {
                    phTitle.Visible = true;
                    lblTitleValue.Visible = false;
                    txtTitle.Visible = true;
                    lblCategoryValue.Visible = false;
                    ddlCategory.Visible = true;
                    lblPriorityValue.Visible = false;
                    ddlPriority.Visible = true;
                    lblStatusValue.Visible = false;
                    ddlStatus.Visible = true;
                    lblDescriptionValue.Visible = false;
                    txtDescription.Visible = true;
                    lblCommentsValue.Visible = false;
                    txtComments.Visible = true;
                    lblAssignedToValue.Visible = false;
                    ppeAssignedTo.Visible = true;
                    if (_item.AssignedToId != null)
                    {
                        ppeAssignedTo.CommaSeparatedAccounts = (GetUserFromId((int)_item.AssignedToId)).ToString();
                    }
                    lblSubmittedFor.Visible = true;
                    lblSubmittedForValue.Visible = false;
                    ppeSubmittedFor.Visible = true;
                    if (_item.SubmittedById != null)
                    {
                        ppeSubmittedFor.CommaSeparatedAccounts = (GetUserFromId((int)_item.SubmittedById)).ToString();
                    }
                    if (_item.CategoryId != null) { ddlCategory.SelectedValue = (_item.CategoryId.ToString()); }
                    if (_item.PriorityId != null) { ddlPriority.SelectedValue = (_item.PriorityId.ToString()); }
                    if (_item.StatusId > 0) { ddlStatus.SelectedValue = (_item.StatusId.ToString()); }
                    if (_isNew)
                    {
                        lblCreated.Visible = false;
                        lblCreatedValue.Visible = false;
                        lblCreatedBy.Visible = false;
                        lblCreatedByValue.Visible = false;
                        lblModified.Visible = false;
                        lblModifiedValue.Visible = false;
                        lblModifiedBy.Visible = false;
                        lblModifiedByValue.Visible = false;
                    }
                    lblResolutionTime.Visible = true;
                    lblResolutionTimeValue.Visible = false;
                    txtResolutionTime.Visible = true;
                    btnCancel.Visible = true;
                    btnSave.Visible = true;
                    btnEdit.Visible = false;
                    pnlNewFiles.Visible = true;
                }

                else if (IsOwner())
                {
                    phTitle.Visible = true;
                    lblTitleValue.Visible = false;
                    txtTitle.Visible = true;
                    lblCategoryValue.Visible = false;
                    ddlCategory.Visible = true;
                    lblPriority.Visible = false;
                    lblPriorityValue.Visible = false;
                    ddlPriority.Visible = false;
                    lblStatus.Visible = false;
                    lblStatusValue.Visible = false;
                    ddlStatus.Visible = false;
                    lblDescriptionValue.Visible = false;
                    txtDescription.Visible = true;
                    lblComments.Visible = false;
                    lblCommentsValue.Visible = false;
                    txtComments.Visible = false;
                    lblAssignedTo.Visible = false;
                    lblAssignedToValue.Visible = false;
                    ppeAssignedTo.Visible = false;
                    lblSubmittedFor.Visible = true;
                    lblSubmittedForValue.Visible = false;
                    ppeSubmittedFor.Visible = true;
                    if (_item.SubmittedById != null)
                    {
                        ppeSubmittedFor.CommaSeparatedAccounts = (GetUserFromId((int)_item.SubmittedById)).ToString();
                    }
                    if (_item.CategoryId != null) { ddlCategory.SelectedValue = (_item.CategoryId.ToString()); }
                    if (_item.PriorityId != null) { ddlPriority.SelectedValue = (_item.PriorityId.ToString()); }
                    if (_item.StatusId > 0) { ddlStatus.SelectedValue = (_item.StatusId.ToString()); }
                    if (_isNew)
                    {
                        lblCreated.Visible = false;
                        lblCreatedValue.Visible = false;
                        lblCreatedBy.Visible = false;
                        lblCreatedByValue.Visible = false;
                        lblModified.Visible = false;
                        lblModifiedValue.Visible = false;
                        lblModifiedBy.Visible = false;
                        lblModifiedByValue.Visible = false;
                    }
                    btnCancel.Visible = true;
                    btnSave.Visible = true;
                    btnEdit.Visible = false;
                    pnlNewFiles.Visible = true;
                }

                else
                {
                    phTitle.Visible = false;
                    lblTitleValue.Visible = true;
                    txtTitle.Visible = false;
                    lblCategoryValue.Visible = true;
                    ddlCategory.Visible = false;
                    lblPriority.Visible = false;
                    lblPriorityValue.Visible = false;
                    ddlPriority.Visible = false;
                    lblStatus.Visible = false;
                    lblStatusValue.Visible = false;
                    ddlStatus.Visible = false;
                    lblDescriptionValue.Visible = true;
                    txtDescription.Visible = false;
                    lblComments.Visible = false;
                    lblCommentsValue.Visible = true;
                    txtComments.Visible = false;
                    lblAssignedTo.Visible = false;
                    lblAssignedToValue.Visible = true;
                    ppeAssignedTo.Visible = false;
                    btnCancel.Visible = false;
                    btnSave.Visible = false;
                    btnEdit.Visible = false;
                    pnlNewFiles.Visible = false;
                    lblSubmittedFor.Visible = false;
                    lblSubmittedForValue.Visible = false;
                    ppeSubmittedFor.Visible = false;

                }

            }

            if (_item.Attachments.Count == 0)
                phFiles.Visible = false;
            else
                phFiles.Visible = true;
        }

        private void ShowAttachments()
        {
            pnlAttachments.Controls.Clear();

            foreach (HelpdeskItemToAttachment h2a in _item.Attachments)
            {
                HyperLink lnk = new HyperLink();
                lnk.ID = "lnkAttachment_" + h2a.AttachmentId.ToString();
                lnk.NavigateUrl = SPContext.Current.Web.Url + "/_layouts/Helpdesk/RenderAttachment.aspx?AttachmentId=" + h2a.AttachmentId.ToString();
                lnk.Target = "_BLANK";
                lnk.Text = h2a.Attachment.Title;
                pnlAttachments.Controls.Add(lnk);
                pnlAttachments.Controls.Add(GetBreak());
            }
        }

        private Control GetBreak()
        {
            Literal literal = new Literal();

            literal.Text = "<br />";

            return literal;
        }

        public static void AddErrorMessageToPage(Panel errorPanel, string message)
        {
            if (errorPanel == null)
            {
                Page page = (Page)HttpContext.Current.Handler;
                errorPanel = (Panel)page.FindControl("pnlError");
            }

            if (errorPanel != null)
            {
                Label lblError = new Label();
                lblError.Text = message;
                errorPanel.Controls.Add(lblError);
                errorPanel.Controls.Add(GetBreakLit());
                errorPanel.Visible = true;
            }
        }

        private static Literal GetBreakLit()
        {
            Literal lit = new Literal();
            lit.Text = @"<br />";
            return lit;
        }


        private void BindDropdowns()
        {

            List<BLL.Reference.Status> statuses = new List<BLL.Reference.Status>();

            foreach (Hashtable ht in new DAL.Reference.StatusDAL().GetAll())
            {
                statuses.Add(new BLL.Reference.Status(ht));
            }

            ddlStatus.DataSource = statuses;
            ddlStatus.DataBind();

            List<BLL.Reference.Priority> priorities = new List<BLL.Reference.Priority>();

            foreach (Hashtable ht in new DAL.Reference.PriorityDAL().GetAll())
            {
                priorities.Add(new BLL.Reference.Priority(ht));
            }

            ddlPriority.DataSource = priorities;
            ddlPriority.DataBind();

            List<BLL.Reference.Category> categories = new List<BLL.Reference.Category>();

            foreach (Hashtable ht in new DAL.Reference.CategoryDAL().GetAll())
            {
                categories.Add(new BLL.Reference.Category(ht));
            }

            ddlCategory.DataSource = categories;
            ddlCategory.DataBind();
        }


        private SPUser GetUserFromPickerControl()
        {
            SPUser User = null;
            SPWeb mySite = SPContext.Current.Web;
            string[] UsersSeparated = ppeAssignedTo.CommaSeparatedAccounts.Split(',');
            SPFieldUserValueCollection UserCollection = new SPFieldUserValueCollection();
            foreach (string UserSeparated in UsersSeparated)
            {
                if (UsersSeparated[0] == "")
                {
                    User = null;
                }
                else
                {
                    mySite.EnsureUser(UserSeparated);
                    User = mySite.SiteUsers[UserSeparated];
                }
 
            }
            return User;
        }

        private SPUser GetSubmittedUserFromPickerControl()
        {
            SPUser User = null;
            SPWeb mySite = SPContext.Current.Web;
            string[] UsersSeparated = ppeSubmittedFor.CommaSeparatedAccounts.Split(',');
            SPFieldUserValueCollection UserCollection = new SPFieldUserValueCollection();
            foreach (string UserSeparated in UsersSeparated)
            {
                if (UsersSeparated[0] == "")
                {
                    User = null;
                }
                else
                {
                    mySite.EnsureUser(UserSeparated);
                    User = mySite.SiteUsers[UserSeparated];
                }

            }
            return User;
        }

        private SPUser GetUserFromId(int id)
        {
            return SPContext.Current.Web.Users.GetByID(id);
        }

        private bool IsOwner()
        {
            bool isOwner = false;
            SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
            if (currentUser.ID == _item.CreatedById.Value)
                isOwner = true;
            return isOwner;
        }

        private bool IsAdmin()
        {
            SPGroup reviewerGroup = SPContext.Current.Web.SiteGroups["Helpdesk Admins"];

            bool isMemberOfGroup = false;

            try
            {
                if (reviewerGroup.ContainsCurrentUser)
                    isMemberOfGroup = true;
            }
            catch (Exception ex)
            {

            }
            return isMemberOfGroup;

        }

        private static void createEmailAlert(bool hasChanged, int emailTypeId, HelpdeskItem helpdeskItem)
        {
            if (hasChanged)
            {
                //EmailAlertCreator.createEmailAlert(emailTypeId, helpdeskItem);
                EmailAlertCreator.createEmailAlert(emailTypeId, helpdeskItem);
                
            }
        }

        private void checkifChanged(HelpdeskItem oldHelpdeskItem, HelpdeskItem newHelpdeskItem)
        {
            bool hasChanged = false;
            int emailTypeId = 0;

            if (oldHelpdeskItem == null)
            {
                hasChanged = true;
                emailTypeId = (int)EmailType.Types.HelpdeskTicketAdded;
                createEmailAlert(hasChanged, emailTypeId, newHelpdeskItem);
            }

            else
            {

                if (oldHelpdeskItem.Title != newHelpdeskItem.Title)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketUpdated;
                }

                if (oldHelpdeskItem.Description != newHelpdeskItem.Description)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketUpdated;

                }
                

                if (oldHelpdeskItem.StatusId != newHelpdeskItem.StatusId)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketUpdated;

                }

                if (oldHelpdeskItem.PriorityId !=newHelpdeskItem.PriorityId)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketUpdated;

                }

                if (oldHelpdeskItem.ResolutionComments != newHelpdeskItem.ResolutionComments)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketUpdated;

                }

                if (oldHelpdeskItem.AssignedToId != newHelpdeskItem.AssignedToId)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketReassigned;
                }

                if (oldHelpdeskItem.ResolutionTime != newHelpdeskItem.ResolutionTime || newHelpdeskItem.StatusId== (int)Status.Types.Resolved)
                {
                    hasChanged = true;
                    emailTypeId = (int)EmailType.Types.HelpdeskTicketResolved;

                }


                createEmailAlert(hasChanged, emailTypeId, newHelpdeskItem);
            }
        
        }


        private bool SaveHelpdeskItem()
        {
            bool isValid = true;
            
            HelpdeskItem helpdeskItem = _item;
            HelpdeskItem oldhelpdeskItem = null;          



            if (helpdeskItem == null)
            {
                helpdeskItem = new HelpdeskItem();

            }

            if (helpdeskItem.RowId != 0)
            {
                oldhelpdeskItem = new HelpdeskItem(helpdeskItem.RowId);
            }

            //checks if everything is valid, if not valid changes isValid to false

            if (phTitle.Visible == true && txtTitle.Text.Trim() == "")
            {
                AddErrorMessageToPage(pnlError, "Please enter a valid title.");
                isValid = false;
            }

            if (txtDescription.Content.Trim() == "")
            {
                AddErrorMessageToPage(pnlError, "Please enter a valid description.");
                isValid = false;
            }

            else if (txtDescription.Content.Length > 1000)
            {
                AddErrorMessageToPage(pnlError, "Please shorten the description to less than a 1,000 characters valid description.");
                isValid = false;
            }

            if (isValid)
            {
                helpdeskItem.Title = txtTitle.Text;
                helpdeskItem.Description = txtDescription.Content;
                helpdeskItem.ResolutionComments = txtComments.Content;
                if (txtResolutionTime.Value != null)
                {
                    helpdeskItem.ResolutionTime = (decimal)txtResolutionTime.Value;
                }
                else
                {
                    helpdeskItem.ResolutionTime = null;
                }
                helpdeskItem.Modified = DateTime.Now;
                SPUser assignedToUser = GetUserFromPickerControl();
                if (assignedToUser != null)
                {
                    helpdeskItem.AssignedToId = assignedToUser.ID;
                }
                else
                {
                    helpdeskItem.AssignedToId = null;
                }
                SPUser submittedForUser = GetSubmittedUserFromPickerControl();
                if (submittedForUser != null)
                {
                    helpdeskItem.SubmittedById = submittedForUser.ID;
                }
                else
                {
                    helpdeskItem.SubmittedById = null;
                }
                if (ddlPriority.SelectedValue != "0")
                {
                    helpdeskItem.PriorityId = int.Parse(ddlPriority.SelectedValue);
                }
                else
                {
                    helpdeskItem.PriorityId = null;
                }
                if (ddlPriority.SelectedValue != "0")
                {
                    helpdeskItem.StatusId = int.Parse(ddlStatus.SelectedValue);
                }
                else
                {
                    helpdeskItem.StatusId = 1;
                }
                helpdeskItem.CategoryId = int.Parse(ddlCategory.SelectedValue);
                helpdeskItem.ModifiedById = SPControl.GetContextWeb(Context).CurrentUser.ID;
                helpdeskItem.Save();
                checkifChanged(oldhelpdeskItem, helpdeskItem);


            //Upload any files.
            if (fuNewFiles.HasFiles)
            {
                foreach (HttpPostedFile file in fuNewFiles.PostedFiles)
                {
                    string fileName = System.IO.Path.GetFileName(file.FileName);

                    Attachment link = new Attachment();
                    try
                    {
                        link.Title = fileName;
                        link.FileSize = (int)file.ContentLength;
                        link.ContentType = file.ContentType;
                        byte[] attachmentBytes = new byte[file.InputStream.Length];

                        // read attachment into attachmentBytes
                        file.InputStream.Read(attachmentBytes, 0, attachmentBytes.Length);
                        link.Content = attachmentBytes;

                        link.Save();

                        HelpdeskItemToAttachment h2a = new HelpdeskItemToAttachment();
                        h2a.AttachmentId = link.RowId;
                        h2a.HelpdeskItemId = helpdeskItem.RowId;
                        h2a.Save();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            }

            return isValid;

        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            hdnMode.Text = "EDIT";
            ShowHideControls();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool isValid = SaveHelpdeskItem();
            if (isValid)
            {
                hdnMode.Text = "VIEW";
                _item = new HelpdeskItem(_item.RowId);
                PopulatePage();
                HttpContext.Current.Response.Redirect(SystemSettings.RootUrl + String.Format("HelpdeskItem.aspx?HelpdeskItemId={0}", _item.RowId));
            }
            else
                hdnMode.Text = "EDIT";

            ShowHideControls();
            ShowAttachments();

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (_item.RowId > 0)
            {
                hdnMode.Text = "VIEW";
                ShowHideControls();
            }
            else
            {
                HttpContext.Current.Response.Redirect("Helpdesk.aspx");
            }
        }

        protected void btnBackToHeldeskList_Click(object sender, EventArgs e)
        {
            Button lblTitle = (Button)(sender);
            string rowId = Convert.ToString(lblTitle.CommandArgument);

            string currentURL = HttpContext.Current.Request.Url.AbsolutePath;
            string newURL = currentURL.Substring(0, currentURL.LastIndexOf('/'));
            newURL = newURL + String.Format("/Helpdesk.aspx");

            HttpContext.Current.Response.Redirect(SystemSettings.RootUrl + "Helpdesk.aspx");
        }
    }
}



