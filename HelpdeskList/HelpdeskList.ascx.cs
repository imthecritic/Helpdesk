using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using Helpdesk.BLL.List;
using Helpdesk.DAL.List;
using Helpdesk.DAL.Core;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Collections;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Data;
using System.Web;
using Utility;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Text;
using System.Text.RegularExpressions;
using Helpdesk.BLL.Reference;

namespace NestedRadgridHelpdesk.HelpdeskList
{
    [ToolboxItemAttribute(false)]
    public partial class HelpdeskList : WebPart
    {
        private const string _versionNumber = "1.7";
        private bool _isFiltering = false;
        private string _filterString = "";
        private Hashtable _whereClause = new Hashtable();
        bool _firstGrid = false;
        private bool _isExpanded = false;
        public enum _modes { Admin, AssignedToMe, MyTickets };
        protected _modes _mode;
        [System.Web.UI.WebControls.WebParts.WebBrowsable(true),
        System.Web.UI.WebControls.WebParts.WebDisplayName("Mode"),
        System.Web.UI.WebControls.WebParts.WebDescription("Select the Mode"),
        System.Web.UI.WebControls.WebParts.Personalizable(
        System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared)]

        public _modes Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        private const string _ascxPath = @"~/_controltemplates/15/Helpdesk/HelpdeskList_UserControl1.ascx";


        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        //[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]

        public HelpdeskList()
        {

        }


        protected override void CreateChildControls()
        {
            Control control = Page.LoadControl(_ascxPath);
            Controls.Add(control);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            gvOuter.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;

            this.CssClass = "helpdesk-item-webpart";
            bool hasLink = false;
            HtmlLink linkCss = new HtmlLink();

            string fileName = "Lists.css";
            string RESOURCE_PATH = "../../_layouts/15/Helpdesk/css/";


            linkCss.ID = "helpdesk_css";
            linkCss.Attributes["type"] = "text/css";
            linkCss.Attributes["rel"] = "stylesheet";
            linkCss.Attributes["href"] = base.ResolveUrl(RESOURCE_PATH) + fileName + "?version=" + _versionNumber;

            if (!IsAdmin())
            {
                HttpContext.Current.Response.Redirect("MyTickets.aspx"); //if the user does not have the permissions redirect to only their tickets
            }


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

            if (ViewState["whereClause"] != null)
            {
                _whereClause = (Hashtable)ViewState["whereClause"];
            }
            else
            {
                // HashTable isn't in view state, so we need to load it from scratch.
                _whereClause = new Hashtable();
            }

            if (!this.Page.IsPostBack)
            {
                gvOuter.Rebind();
                //Page.ClientScript.RegisterStartupScript(this.GetType(), "hidePager", "onGridCreate()", true);


            }

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

        void gvHelpdesk_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataItem item = (GridDataItem)((RadGrid)sender).Parent.Parent;
            int statusId = (int)item.GetDataKeyValue("StatusId");
            RadGrid gvHelpdesk = (RadGrid)item.FindControl("gvHelpdesk");
            List<HelpdeskItem> items = new List<HelpdeskItem>();
            SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
            string name = txtSearch.Text;
            if (_mode == _modes.MyTickets)
            {

                foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }
            else if (_mode == _modes.AssignedToMe)
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }
            else
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
                
            }

            gvHelpdesk.DataSource = items;
            gvHelpdesk.Rebind();
        }

        protected void txtName_TextChanged(object sender, EventArgs e)
        {
            gvOuter_NeedDataSource(null, null);
            gvOuter.DataBind();
        }

        public string GetSearchedText(string stringToSearch, string searchString)
        {
            string result = "";
            bool isLongStringMatch = checkPastConcatenation(stringToSearch, searchString);

            if (SystemSettings.MaxDescriptionLength <= 0)
            {
                SystemSettings.Refresh();
            }
        
            
            if (searchString == "")
            {
                result = stringToSearch;
            }

            else if (Regex.IsMatch(stringToSearch, searchString, RegexOptions.Compiled | RegexOptions.IgnoreCase) && isLongStringMatch)
            {
                result = Regex.Replace(stringToSearch, searchString, match => "<font color=\"red\">" + match.Value + "<font color=\"black\">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                result = result.Substring(0, SystemSettings.MaxDescriptionLength) + "<font color=\"red\"><b>...</b></font>";
            }
            
            else if (Regex.IsMatch(stringToSearch, searchString, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                result =
                    Regex.Replace(stringToSearch, searchString, match => "<font color=\"red\">" + match.Value + "<font color=\"black\">", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }

            else
            {

                result = stringToSearch;

            }
            
            return result;
        }

        private bool checkPastConcatenation(string stringToSearch, string searchString)
        {
            bool isLongStringMatch = false;
            string temp = stringToSearch;
            bool substring = false;

            if (SystemSettings.MaxDescriptionLength <= 0)
            {
                SystemSettings.Refresh();
            }
        
            if (temp.Length > SystemSettings.MaxDescriptionLength)
            {
                temp = temp.Substring(SystemSettings.MaxDescriptionLength, temp.Length - SystemSettings.MaxDescriptionLength);
                substring = true;
            }
            if (substring && Regex.IsMatch(temp, searchString, RegexOptions.Compiled | RegexOptions.IgnoreCase))
            {
                isLongStringMatch = true;
            }
            return isLongStringMatch;

        }

        public string MakeShort(String str)
        {
            if (SystemSettings.MaxDescriptionLength <= 0)
            {
                SystemSettings.Refresh();
            }

            if (str.Contains ("<font color=\"red\"><b>...</b></font>"))
            {
                return str;
            }

            if (str.Length > SystemSettings.MaxDescriptionLength)
            {
                str = str.Substring(0, SystemSettings.MaxDescriptionLength) + "...";
            }

            return str;
        }

        protected void gvOuter_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                int statusId = (int)((GridDataItem)e.Item).GetDataKeyValue("StatusId");
                RadGrid gvHelpdesk = (RadGrid)e.Item.FindControl("gvHelpdesk");
                List<HelpdeskItem> items = new List<HelpdeskItem>();
                string name = txtSearch.Text;

                bool isFirst = false;
                if (_firstGrid == false)
                {
                    _firstGrid = true;
                    isFirst = true;
                }
                else
                {
                    gvHelpdesk.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
                    gvHelpdesk.ShowHeader = false;
                }
                SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
                if (_mode == _modes.MyTickets)
                {

                    foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                        items.Add(new HelpdeskItem(ht));
                }
                else if (_mode == _modes.AssignedToMe)
                {
                    foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                        items.Add(new HelpdeskItem(ht));
                }
                else
                {
                    foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                    {
                        HelpdeskItem itm = new HelpdeskItem(ht);
                        itm.Title = GetSearchedText(itm.Title, name);
                        itm.Description = GetSearchedText(itm.Description, name);
                        items.Add(itm);
                    }
                }
   
                gvHelpdesk.DataSource = items;
                gvHelpdesk.Rebind();
                CollapseAll(gvHelpdesk);

                int RowCount = gvHelpdesk.MasterTableView.Items.Count;
                if (RowCount == 0 && isFirst == false)
                {
                    gvHelpdesk.Visible = false;
                }
                else
                    gvHelpdesk.Visible = true;

                if (!this.Page.IsPostBack)
                {
                    CollapseAll(gvHelpdesk);
                }
            }

        }

        protected void gvOuter_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            DataTable dt = new HelpdeskItemDAL().GetStatusesWithCount();
            gvOuter.DataSource = dt;
        }

        private void CheckIfCollapsed(RadGrid gvHelpdesk)
        {
            if (gvHelpdesk.MasterTableView.Controls.Count > 0)
            {
                foreach (GridItem item in gvHelpdesk.MasterTableView.Controls[0].Controls)
                {
                    if (item is GridGroupHeaderItem)
                    {
                        if (item.Expanded == false)
                            gvHelpdesk.MasterTableView.PagerStyle.Visible = false;
                        else
                            gvHelpdesk.MasterTableView.PagerStyle.Visible = true;
                    }

                }
            }
        }

        private void CollapseAll(RadGrid gvHelpdesk)
        {
            if (gvHelpdesk.MasterTableView.Controls.Count > 0)
            {
                foreach (GridItem item in gvHelpdesk.MasterTableView.Controls[0].Controls)
                {
                    if (item is GridGroupHeaderItem)
                    {
                        item.Expanded = false;
                    }

                }
            }
        }

        protected void gvHelpdesk_PageIndexChanged(object sender, GridPageChangedEventArgs e)
        {
            int statusId = (int)(e.Item.OwnerTableView.DataKeyValues[0]["StatusId"]);
            RadGrid gvHelpdesk = (RadGrid)e.Item.OwnerTableView.OwnerGrid;
            List<HelpdeskItem> items = new List<HelpdeskItem>();
            SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
            if (_mode == _modes.MyTickets)
            {

                foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }
            else if (_mode == _modes.AssignedToMe)
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }
            else
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }

            gvHelpdesk.DataSource = items;
            gvHelpdesk.DataBind();
        }

        protected void gvHelpdesk_PageSizeChanged(object sender, GridPageSizeChangedEventArgs e)
        {
            int statusId = (int)(e.Item.OwnerTableView.DataKeyValues[0]["StatusId"]);
            RadGrid gvHelpdesk = (RadGrid)e.Item.OwnerTableView.OwnerGrid;
            List<HelpdeskItem> items = new List<HelpdeskItem>();
            SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
            if (_mode == _modes.MyTickets)
            {

                foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }
            else if (_mode == _modes.AssignedToMe)
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }
            else
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                    items.Add(new HelpdeskItem(ht));
            }

            gvHelpdesk.DataSource = items;
            gvHelpdesk.DataBind();
        }

        protected void btnNewItem_Click(object sender, EventArgs e)
        {

            HttpContext.Current.Response.Redirect("HelpdeskItem.aspx");
        }

        protected void gvHelpdesk_SortCommand(object sender, GridSortCommandEventArgs e)
        {
            RadGrid gv = (RadGrid)e.Item.OwnerTableView.OwnerGrid;

            foreach (GridItem item in gvOuter.Items)
            {
                if (item is GridDataItem)
                {
                    RadGrid gvHelpdesk = (RadGrid)item.FindControl("gvHelpdesk");

                    if (gvHelpdesk.ClientID != gv.ClientID)
                    {
                        GridSortExpression gsi = new GridSortExpression();
                        gsi.FieldName = e.CommandArgument.ToString();
                        gsi.SortOrder = e.NewSortOrder;

                        gvHelpdesk.MasterTableView.SortExpressions.AddSortExpression(gsi);

                        //gvHelpdesk.MasterTableView.Rebind();
                        //CollapseAll(gvHelpdesk);

                        int statusId = (int)((GridDataItem)item).GetDataKeyValue("StatusId");
                        List<HelpdeskItem> items = new List<HelpdeskItem>();
                        SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
                        if (_mode == _modes.MyTickets)
                        {

                            foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                                items.Add(new HelpdeskItem(ht));
                        }
                        else if (_mode == _modes.AssignedToMe)
                        {
                            foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                                items.Add(new HelpdeskItem(ht));
                        }
                        else
                        {
                            foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                                items.Add(new HelpdeskItem(ht));
                        }

                        gvHelpdesk.DataSource = items;
                        gvHelpdesk.DataBind();
                        CollapseAll(gvHelpdesk);

                    }
                    else
                    {
                        //just bind.
                        int statusId = (int)((GridDataItem)item).GetDataKeyValue("StatusId");
                        List<HelpdeskItem> items = new List<HelpdeskItem>();
                        SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
                        if (_mode == _modes.MyTickets)
                        {

                            foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                                items.Add(new HelpdeskItem(ht));
                        }
                        else if (_mode == _modes.AssignedToMe)
                        {
                            foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                                items.Add(new HelpdeskItem(ht));
                        }
                        else
                        {
                            foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                                items.Add(new HelpdeskItem(ht));
                        }

                        gvHelpdesk.DataSource = items;
                        gvHelpdesk.DataBind();
                        CollapseAll(gvHelpdesk);

                    }
                }
            }
        }


        private void ExpandedState(RadGrid gvOuter, GridGroupHeaderItem header)
        {
            foreach (GridItem itm in gvOuter.Items)
            {
                if (itm is GridDataItem)
                {
                    RadGrid gvHelpdesk = (RadGrid)itm.FindControl("gvHelpdesk");

                    foreach (GridItem item in gvHelpdesk.MasterTableView.Controls[0].Controls)
                    {
                        if (item is GridGroupHeaderItem)
                        {
                            if (item.Expanded == false)
                            {
                                item.Expanded = true;
                                gvHelpdesk.MasterTableView.AllowPaging = true;
                                gvHelpdesk.MasterTableView.PagerStyle.Visible = true;
                            }
                        }

                    }
                }
            }

        }

        protected void gvHelpdesk_ItemCommand(object sender, GridCommandEventArgs e)
        {
            RadGrid gvHelpdesk = (RadGrid)e.Item.OwnerTableView.OwnerGrid;

            if (e.CommandName == RadGrid.ExpandCollapseCommandName)
            {
                //expandcollapse
                //Hdnfields add grid ids
                if (hdnExpandedGridIds.Value.Contains (gvHelpdesk.ClientID + "@")){
                    hdnExpandedGridIds.Value = hdnExpandedGridIds.Value.Replace(gvHelpdesk.ClientID + "@", "");
                }

                else
                {
                    hdnExpandedGridIds.Value += gvHelpdesk.ClientID + "@";
                }

                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPager", "expandCollapseGrids(" + "'" + hdnExpandedGridIds.Value + "'" + ")", true);

            }

            else if (e.CommandName == "Sort")
            {
                e.ExecuteCommand(e.Item);
                e.Canceled = true;
                CollapseAll(gvHelpdesk);

                
            }

            else if (e.CommandName == RadGrid.FilterCommandName)
            {
                Pair filterPair = (Pair)e.CommandArgument;
                GridFilteringItem filteringItem = (GridFilteringItem)e.Item;

                string filter = filterPair.Second.ToString();
                if (filter == "AssignedToId")
                {
                    RadComboBox ddl = (RadComboBox)filteringItem.FindControl("ddlAssignedToFilter");
                    gvHelpdesk.MasterTableView.FilterExpression = @"Int32(it[""AssignedToId""]) = " + ddl.SelectedValue;
                    if (ddl.SelectedIndex != 0 && !_whereClause.Contains("AssignedToId"))
                    {
                        _whereClause.Add("AssignedToId", ddl.SelectedValue);
                    }
                    else if (ddl.SelectedIndex == 0 && !_whereClause.Contains("AssignedToId"))
                    {
                        _whereClause.Add("AssignedToId", "~");
                    }
                    else if (ddl.SelectedIndex != 0 && _whereClause.Contains("AssignedToId"))
                    {
                        _whereClause["AssignedToId"] = ddl.SelectedValue;
                    }

                    else
                    {
                        _whereClause["AssignedToId"] = "~";

                    }

                }

                else if (filter == "CategoryId")
                {
                    RadComboBox ddl = (RadComboBox)filteringItem.FindControl("ddlCategoryFilter");
                    gvHelpdesk.MasterTableView.FilterExpression = @"Int32(it[""CategoryId""])= " + ddl.SelectedValue;
                    if (ddl.SelectedIndex != 0 && !_whereClause.Contains("CategoryId"))
                    {
                        _whereClause.Add("CategoryId", ddl.SelectedValue);
                    }
                    else if (ddl.SelectedIndex == 0 && !_whereClause.Contains("CategoryId"))
                    {
                        _whereClause.Add("CategoryId", "~");
                    }
                    else if (ddl.SelectedIndex != 0 && _whereClause.Contains("CategoryId"))
                    {
                        _whereClause["CategoryId"] = ddl.SelectedValue;
                    }

                    else
                    {
                        _whereClause["CategoryId"] = "~";

                    }
                }

                else if (filter == "StatusId")
                {
                    RadComboBox ddl = (RadComboBox)filteringItem.FindControl("ddlStatusFilter");
                    gvHelpdesk.MasterTableView.FilterExpression = @"Int32(it[""StatusId""])= " + ddl.SelectedValue;
                    if (ddl.SelectedIndex != 0 && !_whereClause.Contains("StatusId"))
                    {
                        _whereClause.Add("StatusId", ddl.SelectedValue);
                    }
                    else if (ddl.SelectedIndex == 0 && !_whereClause.Contains("StatusId"))
                    {
                        _whereClause.Add("StatusId", "~");
                    }
                    else if (ddl.SelectedIndex != 0 && _whereClause.Contains("StatusId"))
                    {
                        _whereClause["StatusId"] = ddl.SelectedValue;
                    }

                    else
                    {
                        _whereClause["StatusId"] = "~";
                    }
                }


                else if (filter == "PriorityId")
                {
                    RadComboBox ddl = (RadComboBox)filteringItem.FindControl("ddlPriorityFilter");
                    gvHelpdesk.MasterTableView.FilterExpression = @"Int32(it[""PriorityId""])= " + ddl.SelectedValue;
                    if (ddl.SelectedIndex != 0 && !_whereClause.Contains("PriorityId"))
                    {
                        _whereClause.Add("PriorityId", ddl.SelectedValue);
                    }
                    else if (ddl.SelectedIndex == 0 && !_whereClause.Contains("PriorityId"))
                    {
                        _whereClause.Add("PriorityId", "~");
                    }
                    else if (ddl.SelectedIndex != 0 && _whereClause.Contains("PriorityId"))
                    {
                        _whereClause["PriorityId"] = ddl.SelectedValue;
                    }

                    else
                    {
                        _whereClause["PriorityId"] = "~";
                    }
                }

                string data = "";
                foreach (string key in _whereClause.Keys)
                {
                    data += key + '@' + _whereClause[key].ToString() + '@';
                }
                gvHiddenField.Value += data;

                string dataString = gvHiddenField.Value;
                if (dataString.Length > 0)
                {

                    var dataSet = dataString.Split('@');

                    List<string> dataArray = new List<string>();
                    foreach (string x in dataSet)
                    {
                        if (x.Length > 0)
                            dataArray.Add(x);
                    }

                    for (int i = 0; i < dataArray.Count; i = i + 2)
                    {
                        string key = dataArray[i];
                        string value = dataArray[i + 1];
                        if (_whereClause.ContainsKey(key))
                        {
                            if (value == "~")
                            {
                                _whereClause.Remove(key);
                            }
                            else
                            {
                                _whereClause[key] = value;
                            }
                        }

                        else
                        {
                            _whereClause.Add(key, value);
                        }
                    }

                }


                _filterString = gvHelpdesk.MasterTableView.FilterExpression;

                _isFiltering = true;

                GridDataItem item = (GridDataItem)((RadGrid)sender).Parent.Parent;
                int statusId = (int)((GridDataItem)item).GetDataKeyValue("StatusId");
                List<HelpdeskItem> items = new List<HelpdeskItem>();
                SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
                if (_mode == _modes.MyTickets)
                {

                    foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                        items.Add(new HelpdeskItem(ht));
                }
                else if (_mode == _modes.AssignedToMe)
                {
                    foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                        items.Add(new HelpdeskItem(ht));
                }
                else
                {
                    foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                        items.Add(new HelpdeskItem(ht));
                }

                gvHelpdesk.DataSource = items;
                if (gvHelpdesk != null)
                {
                    gvHelpdesk.DataBind();
                    CollapseAll(gvHelpdesk);                    

                }
            }
        }


        protected void gvHelpdesk_ItemDataBound(object sender, GridItemEventArgs e)
        {

            if (_isFiltering == true)
            {
                RadGrid gv = (RadGrid)e.Item.OwnerTableView.OwnerGrid;
                _isFiltering = false;

                bool isFirst = true;

                foreach (GridItem item in gvOuter.Items)
                {

                    if (item is GridDataItem)
                    {
                        RadGrid gvHelpdesk = (RadGrid)item.FindControl("gvHelpdesk");

                        if (gvHelpdesk.ClientID != gv.ClientID)
                        {

                            int statusId = (int)((GridDataItem)item).GetDataKeyValue("StatusId");
                            List<HelpdeskItem> items = new List<HelpdeskItem>();
                            SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
                            if (_mode == _modes.MyTickets)
                            {

                                foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                                    items.Add(new HelpdeskItem(ht));
                            }
                            else if (_mode == _modes.AssignedToMe)
                            {
                                foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                                    items.Add(new HelpdeskItem(ht));
                            }
                            else
                            {
                                foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                                    items.Add(new HelpdeskItem(ht));
                            }



                            gvHelpdesk.DataSource = items;
                            gvHelpdesk.DataBind();
                            CollapseAll(gvHelpdesk);
       

                            if (gvHelpdesk.MasterTableView.Items.Count == 0 && isFirst == false)
                            {
                                gvHelpdesk.Visible = false;
                            }
                            else
                                gvHelpdesk.Visible = true;

                        }
                        else
                        {
                            CollapseAll(gvHelpdesk);

                        }

                        isFirst = false;
                    }

                }
                _filterString = "";
            }
        }

        public List<HelpdeskItem> GetAssignedToUsers(List<HelpdeskItem> items)
        {
            ArrayList added = new ArrayList();
            List<HelpdeskItem> assignees = new List<HelpdeskItem>();
            foreach (HelpdeskItem item in items)
            {
                if (item.AssignedToId != null && !added.Contains(item.AssignedToId))
                {
                    assignees.Add(item);
                    added.Add(item.AssignedToId);
                }
            }
            return assignees;
        }

        protected void gvHelpdesk_PreRender(object sender, EventArgs e)
        {

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showPager", "expandCollapseGrids(" + "'" + hdnExpandedGridIds.Value + "'" + ")", true);

            List<HelpdeskItem> items = new List<HelpdeskItem>();
            foreach (Hashtable ht in new HelpdeskItemDAL().GetAll())
                items.Add(new HelpdeskItem(ht));
            items = GetAssignedToUsers(items);
            RadGrid grid = (RadGrid)sender;

            ViewState.Add("whereClause", _whereClause);

            GridDataItem gvHelpdesk = (GridDataItem)((RadGrid)sender).Parent.Parent;
            int statusId = (int)gvHelpdesk.GetDataKeyValue("StatusId"); 
            List<HelpdeskItem> statuslist = new List<HelpdeskItem>();
            SPUser currentUser = SPControl.GetContextWeb(Context).CurrentUser;
            if (_mode == _modes.MyTickets)
            {

                foreach (Hashtable ht in new HelpdeskItemDAL().GetByCreatedById(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    statuslist.Add(new HelpdeskItem(ht));
            }
            else if (_mode == _modes.AssignedToMe)
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByAssignedToId(currentUser.ID, statusId, txtSearch.Text, _whereClause))
                    statuslist.Add(new HelpdeskItem(ht));
            }
            else
            {
                foreach (Hashtable ht in new HelpdeskItemDAL().GetByStatusId(statusId, txtSearch.Text, _whereClause))
                    statuslist.Add(new HelpdeskItem(ht));
            }

            foreach (GridGroupHeaderItem groupHeader in grid.MasterTableView.GetItems(GridItemType.GroupHeader))
            {
                Regex regex = new Regex(@"\d+");

                bool hasCount = regex.IsMatch(groupHeader.DataCell.Text);


                if (hasCount)
                {
                    groupHeader.DataCell.Text = regex.Replace(groupHeader.DataCell.Text, statuslist.Count.ToString());
                }

                else if (statuslist.Count > 0 && !hasCount)
                {
                    groupHeader.DataCell.Text += "<strong> (" + statuslist.Count.ToString() + ") </strong>";
                }

                //if (!groupHeader.DataCell.Text.Contains ("<strong> (" + statuslist.Count.ToString() +") </strong>")){
                //    groupHeader.DataCell.Text += "<strong> (" + statuslist.Count.ToString() +") </strong>";
                //}

               
            }
            foreach (GridFilteringItem filteringItem in grid.MasterTableView.GetItems(GridItemType.FilteringItem))
            {
                RadComboBox ddlAssignedTo = (RadComboBox)filteringItem.FindControl("ddlAssignedToFilter");
                if (ddlAssignedTo != null)
                {
                    ddlAssignedTo.DataSource = items;
                    ddlAssignedTo.DataTextField = "AssignedToUserTitle";
                    ddlAssignedTo.DataValueField = "AssignedToId";
                    ddlAssignedTo.SelectedValue = grid.MasterTableView.GetColumn("AssignedToId").CurrentFilterValue;
                    ddlAssignedTo.DataBind();

                    RadComboBoxItem radComboBoxItem = new RadComboBoxItem();
                    radComboBoxItem.Text = "All";
                    radComboBoxItem.Value = "All";
                    ddlAssignedTo.Items.Insert(0, radComboBoxItem);

                    string strScript = "<script>";
                    strScript += ("var m_AssignedToTableViewId = '" + ((RadGrid)sender).MasterTableView.ClientID + "';");
                    strScript += "</script>";

                    if (this.Page.ClientScript.IsClientScriptBlockRegistered("AssignedToClientId") == false)
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "AssignedToClientId", strScript);
                }

                RadComboBox ddlCategory = (RadComboBox)filteringItem.FindControl("ddlCategoryFilter");
                if (ddlCategory != null)
                {
                    List<BLL.Reference.Category> categories = new List<BLL.Reference.Category>();
                    foreach (Hashtable ht in new DAL.Reference.CategoryDAL().GetAll())
                    {
                        categories.Add(new BLL.Reference.Category(ht));
                    }

                    ddlCategory.DataSource = categories;
                    ddlCategory.DataTextField = "Name";
                    ddlCategory.DataValueField = "RowId";
                    ddlCategory.SelectedValue = grid.MasterTableView.GetColumn("CategoryId").CurrentFilterValue;
                    ddlCategory.DataBind();

                    RadComboBoxItem radComboBoxItem = new RadComboBoxItem();
                    radComboBoxItem.Text = "All";
                    radComboBoxItem.Value = "All";
                    ddlCategory.Items.Insert(0, radComboBoxItem);

                    string strScript = "<script>";
                    strScript += ("var m_CategoryToTableViewId = '" + ((RadGrid)sender).MasterTableView.ClientID + "';");
                    strScript += "</script>";

                    if (this.Page.ClientScript.IsClientScriptBlockRegistered("CategoryToClientId") == false)
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "CategoryToClientId", strScript);

                }

                RadComboBox ddlPriority = (RadComboBox)filteringItem.FindControl("ddlPriorityFilter");
                if (ddlCategory != null)
                {
                    List<BLL.Reference.Priority> priorities = new List<BLL.Reference.Priority>();


                    foreach (Hashtable ht in new DAL.Reference.PriorityDAL().GetAll())
                    {
                        priorities.Add(new BLL.Reference.Priority(ht));
                    }

                    ddlPriority.DataSource = priorities;
                    ddlPriority.DataTextField = "Name";
                    ddlPriority.DataValueField = "RowId";
                    ddlPriority.SelectedValue = grid.MasterTableView.GetColumn("PriorityId").CurrentFilterValue;
                    ddlPriority.DataBind();

                    RadComboBoxItem radComboBoxItem = new RadComboBoxItem();
                    radComboBoxItem.Text = "All";
                    radComboBoxItem.Value = "All";
                    ddlPriority.Items.Insert(0, radComboBoxItem);

                    string strScript = "<script>";
                    strScript += ("var m_PriorityToTableViewId = '" + ((RadGrid)sender).MasterTableView.ClientID + "';");
                    strScript += "</script>";

                    if (this.Page.ClientScript.IsClientScriptBlockRegistered("PriorityToClientId") == false)
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "PriorityToClientId", strScript);
                }

                RadComboBox ddlStatus = (RadComboBox)filteringItem.FindControl("ddlStatusFilter");
                if (ddlCategory != null)
                {
                    List<BLL.Reference.Status> statuses = new List<BLL.Reference.Status>();
                    foreach (Hashtable ht in new DAL.Reference.StatusDAL().GetAll())
                    {
                        statuses.Add(new BLL.Reference.Status(ht));
                    }

                    ddlStatus.DataSource = statuses;
                    ddlStatus.DataTextField = "Name";
                    ddlStatus.DataValueField = "RowId";
                    ddlStatus.SelectedValue = grid.MasterTableView.GetColumn("StatusId").CurrentFilterValue;
                    ddlStatus.DataBind();

                    RadComboBoxItem radComboBoxItem = new RadComboBoxItem();
                    radComboBoxItem.Text = "All";
                    radComboBoxItem.Value = "All";
                    ddlStatus.Items.Insert(0, radComboBoxItem);

                    string strScript = "<script>";
                    strScript += ("var m_StatusToTableViewId = '" + ((RadGrid)sender).MasterTableView.ClientID + "';");
                    strScript += "</script>";

                    if (this.Page.ClientScript.IsClientScriptBlockRegistered("StatusToClientId") == false)
                        this.Page.ClientScript.RegisterStartupScript(this.GetType(), "StatusToClientId", strScript);
                }
            }
        }

        protected string gvHelpdeskGroupCount(int StatusId)
        {
            string countString = "(";


            return countString;
        }


        protected void btnClearFilters_Click(object sender, EventArgs e)
        {
            gvHiddenField.Value = "";
            _whereClause.Clear();
            hdnExpandedGridIds.Value = "";
            txtSearch.Text = "";
            gvOuter_NeedDataSource(null, null);
            gvOuter.DataBind();
        }


        protected void lblTitle_Click(object sender, EventArgs e)
        {
            LinkButton lblTitle = (LinkButton)(sender);
            string rowId = Convert.ToString(lblTitle.CommandArgument);

            string currentURL = HttpContext.Current.Request.Url.AbsolutePath;
            string newURL = currentURL.Substring(0, currentURL.LastIndexOf('/'));
            newURL = newURL + String.Format("/HelpdeskItem.aspx?HelpdeskItemId={0}", rowId);

            HttpContext.Current.Response.Redirect(newURL);
        }

       

    }
}

