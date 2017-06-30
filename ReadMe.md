# Helpdesk Webpart

Here are a few of the files I wrote for a visual webpart for a SharePoint website

It uses two radgrids to create a nested grid and form a helpdesk list. 

The number of radgrids are determined by the number of statusid(1-6)
and they are grouped by these status ids. 

On the server side (codebehid), you'll see it take a lot of manipulation
to get telrik's RadGrid to behave as I want it to.

On the client side of the list there is some JavaScript within the ASP file to 
set the filter dropdowns and to hide the paging when a grid is not expanded. 

Here are some screenshots to help visualize:

![alt text](https://github.com/imthecritic/Helpdesk/blob/master/Screen%20Shot%202017-06-30%20at%208.55.30%20AM.png "Collapsed")

![alt text](https://github.com/imthecritic/Helpdesk/blob/master/Screen%20Shot%202017-06-30%20at%208.58.32%20AM.png "Expanded")

![alt text](https://github.com/imthecritic/Helpdesk/blob/master/Screen%20Shot%202017-06-30%20at%208.58.56%20AM.png "Status Dropdown")

![alt text](https://github.com/imthecritic/Helpdesk/blob/master/Screen%20Shot%202017-06-30%20at%208.59.12%20AM.png "Closed")

