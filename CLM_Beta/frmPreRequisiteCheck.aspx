<%@ Page Language="C#"  MasterPageFile="~/MenuMaster.Master" AutoEventWireup="true" CodeFile="frmPreRequisiteCheck.aspx.cs" Inherits="frmPreRequisiteCheck" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderStyleContent" runat="server">
    <style>

        .pagination-outer {
            padding: 10px;
            text-align: center;
        }

        .pagination-outer a, .pagination-outer span {
            margin: 0 5px;
            padding: 5px 10px;
            background-color: #f5f5f5;
            border: 1px solid #ddd;
            color: #428bca;
            text-decoration: none;
        }

        .pagination-outer .aspNetDisabled {
            color: #999;
            background-color: #eee;
        }

       .section-header {
        font-weight: bold;
        margin-bottom: 10px;
        font-size:18px;
    }

    .option-row {
        border-bottom: 1px solid #ccc;
        padding: 15px 0;
    }

    .btn-purple {
        background-color: #6f42c1;
        color: white;
        border: none;
    }

    .btn-light-green {
        background-color: #d4edda;
        color: #155724;
        border: none;
    }

    .btn-light-blue {
        background-color: #cce5ff;
        color: #004085;
        border: none;
    }

    .search-bar {
        display: flex;
        align-items: center;
    }

    .search-bar input {
        flex: 1;
        margin-right: 5px;
    }

    .search-bar select {
        width: 150px;
    }

    .divider {
        border-top: 1px solid #ccc;
        margin: 20px 0;
    }   


          @keyframes progressAnimation {
            from { width: 0%; }
            to { width: 75%; } /* Adjust based on current step */
        }
        .TextBoxUpperCase {
            text-transform: uppercase;
            font-family: Arial;
            font-size: 14px;
            color: #555;
        }
        .mandatory {
            font-weight: bold;
            color: Red;
            vertical-align: middle;
        }    
         .modalBackground {
        background-color: #003366; /* Dark Blue */
        opacity: 0.7;
    }

    .modalPopup {
        background-color: #FFFFFF;
        border: 2px solid #0077CC; /* Medium Blue */
        border-radius: 10px;
        padding: 15px;
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
    }

    /* Header Styles */
    .popupHeader {
        background-color: #0077CC; /* Medium Blue */
        color: #FFFFFF;
        text-align: center;
        padding: 10px;
        font-weight: bold;
        border-radius: 7px 7px 0 0;
    }
        /* Error Message Style */
    .errorMessage {
        color: #FF0000;
        font-weight: bold;
        margin-bottom: 10px;
        display: block;
    }
    /* Label and Textbox Styles */
    .labelStyle {
        font-weight: bold;
        color: #003366; /* Dark Blue */
        margin-bottom: 5px;
        display: block;
    }

    .textBoxStyle {
        width: 95%;
        padding: 8px;
        margin-bottom: 10px;
        border: 1px solid #ADD8E6; /* Light Blue */
        border-radius: 5px;
        box-sizing: border-box;
    }

    /* Button Styles */
    .buttonStyle {
        background-color: #0077CC; /* Medium Blue */
        color: #FFFFFF;
        padding: 10px 20px;
        border: none;
        border-radius: 5px;
        cursor: pointer;
        transition: background-color 0.3s ease;
        margin: 5px;
    }

        .buttonStyle:hover {
            background-color: #0055AA; /* Darker Blue */
        }
         .popupText {
        color: #003366; /* Dark Blue */
        font-size: 1.1em;
        line-height: 1.6;
        margin-bottom: 20px;
    }

    /* Button Styles */
    .buttonContainer {
        text-align: center;
    }
     /*.CompletionListCssClass1 {
            font-family: arial;
            font-size: 0.6em;
            font-weight: normal;
            border: solid 1px #006699;
            line-height: 20px;
            padding: 10px;
            background-color: #428BCA;
            margin-left: 10px;
            width: 100% !important;
            overflow: auto;
        }

        .CompletionListItemCssClass1 {
            border-bottom: dotted 1px #006699;
            cursor: pointer;
            color: black;
            width: 100% !important;
        }

        .CompletionListHighlightedItemCssClass1 {
            color: White;
            background-color: darkcyan;
            cursor: pointer;
            width: 100% !important;
        }*/
     /* General completion list style */
.CompletionListCssClass1 {
    font-family: arial;
    font-size: 0.9em; /* Increased for better readability */
    font-weight: normal;
    border: solid 1px #006699;
    line-height: 1.4em; /* Adjusted for better spacing */
    padding: 5px; /* Reduced padding */
    background-color: #f8f9fa; /* Light gray background */
    margin-left: 0px; /* Removed margin */
    width: 300px; /* Set a fixed width to match the textbox */
    max-height: 200px; /* Set a maximum height for scrolling */
    overflow-y: auto; /* Enable vertical scrolling */
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); /* Subtle shadow */
}

/* Style for each item in the list */
.CompletionListItemCssClass1 {
    border-bottom: dotted 1px #006699;
    cursor: pointer;
    color: #495057; /* Darker text color */
    padding: 6px; /* Adjusted padding */
    width: 100%;
    box-sizing: border-box; /* Include padding and border in the element's total width and height */
}

/* Style for highlighted item */
.CompletionListHighlightedItemCssClass1 {
    color: white;
    background-color: #007bff; /* Bootstrap primary color */
    cursor: pointer;
    width: 100%;
    box-sizing: border-box; /* Include padding and border in the element's total width and height */
}


/* WOView Styles */
.wo-view-container {
    margin-top: 20px;
    /*margin-right: 15px;*/
    text-align: left;
    float: right;
    width: 98%; /* Take full width */
}

.wo-view-table {
    width: 100%;
    border: 1px dotted #ccc;
    font-size: 0.9em; /* Responsive font size */
}

.section-header-cell {
    font-weight: bolder;
    text-align: left;
    font-style: italic;
    color: #007bff; /* Bootstrap primary color */
    padding: 0.5em;
}

.label-cell {
    font-weight: bold;
    padding: 0.5em;
    text-align: left;
}

.value-cell {
    padding: 0.5em;
}

/* Media query for smaller screens */
@media (max-width: 768px) {
    .wo-view-table {
        font-size: 0.8em; /* Further reduce font size on smaller screens */
    }

    .section-header-cell,
    .label-cell,
    .value-cell {
        display: block; /* Stack the cells */
        width: 100%;
        box-sizing: border-box; /* Include padding in width */
    }
}


    </style>

 <style>
      /* Image slider*/
    :root {
      --w: 720px; /* slider width */
      --h: 460px; /* slider height */
      --bg: #0f172a; /* slate-900 */
      --fg: #e2e8f0; /* slate-200 */
      --accent: #2563eb; /* blue-600 */
    }
  
    .slider {
      height: var(--h);
      position: relative;
      overflow: hidden;
      border-radius: 12px;
      box-shadow: 0 10px 30px rgba(0,0,0,.35);
      background: #111827;
    }
    .slides {
      display: flex;
      width: 100%;
      height: 100%;
      transition: transform .5s ease;
    }
    .slide {
      min-width: 100%;
      height: 100%;
      display: grid;
      /*place-items: center;*/
      font-size: clamp(1.4rem, 4vw, 2.2rem);
      letter-spacing: .5px;
      font-weight: 600;
    }
    .slide img {
        padding:7px;
        border-radius:15px;
        height:420px;
    }
    .slide:nth-child(1) { background: linear-gradient(to right, #2e3e8f, #774c8c); }
    .slide:nth-child(2) { background: linear-gradient(to right, #2e3e8f, #774c8c); }
    .slide:nth-child(3) { background: linear-gradient(to right, #2e3e8f, #774c8c); }
    .slide:nth-child(4) { background: linear-gradient(to right, #2e3e8f, #774c8c); }
    .slide:nth-child(5) { background: linear-gradient(to right, #2e3e8f, #774c8c); }
    .slide:nth-child(6)  { background: linear-gradient(to right, #2e3e8f, #774c8c); }  
    .slide:nth-child(7)  { background: linear-gradient(to right, #2e3e8f, #774c8c); }  
    .slide:nth-child(8)  { background: linear-gradient(to right, #2e3e8f, #774c8c); } 
    .slide:nth-child(9)  { background: linear-gradient(to right, #2e3e8f, #774c8c); }  
    .slide:nth-child(10) { background: linear-gradient(to right, #2e3e8f, #774c8c); } 
    .slide:nth-child(11) { background: linear-gradient(to right, #2e3e8f, #774c8c); } 
    .slide:nth-child(12) { background: linear-gradient(to right, #2e3e8f, #774c8c); } 


    /* controls */
    .controls {
      position: absolute;
      inset: 0;
      display: flex;
      align-items: center;
      justify-content: space-between;
      pointer-events: none;
    }
    .btn-slide {
      pointer-events: auto;
      background: rgba(0,0,0,.35);
      border: 1px solid rgba(255,255,255,.25);
      color: var(--fg);
      width: 42px; height: 42px; border-radius: 50%;
      display: grid; place-items: center;
      margin: 0 10px;
      cursor: pointer;
      transition: background .2s ease, transform .08s ease;
      user-select: none;
    }
    .btn-slide:hover { background: rgba(0,0,0,.55); }
    .btn-slide:active { transform: scale(.96); }
    .dots {
      position: absolute; bottom: 10px; left: 50%; transform: translateX(-50%);
      display: flex; gap: 8px;
      background: rgba(0,0,0,.25);
      padding: 6px 10px; border-radius: 999px;
      border: 1px solid rgba(255,255,255,.15);
    }
    .dot {
      width: 10px; height: 10px; border-radius: 50%;
      background: rgba(255,255,255,.35); cursor: pointer;
      transition: transform .08s ease, background .2s ease;
    }
    .dot.active { background: var(--accent); transform: scale(1.2); }
    /* responsive height */
    @media (max-width: 560px) { :root { --h: 240px; } }
  </style>


</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
   

<div runat="server" id="gridDiv">
    
<asp:Panel ID="pnlMain" runat="server" CssClass="mt-4">
    <div class="section-header">Kindly select one of the below options:</div>
      <div class="divider"></div>
    <!-- Create Fresh Request Section -->
    <div class="option-row">
        <div class="row mt-2">
            <div class="col-md-3">
                <asp:RadioButton ID="rbCreate" runat="server" GroupName="RequestOption" Text="Create New Request" Checked="true" AutoPostBack="true" OnCheckedChanged="rbCreate_CheckedChanged" />

            </div>
            <div class="col-md-3">
                <asp:Button ID="btnGenerate" runat="server" Text="Click here to generate" CssClass="btn btn-success btn-block" OnClick="btnGenerate_Click" />
            </div>
           <%-- <div class="col-md-2">
                <asp:Button ID="btnReqNo" runat="server" Text="2509492864" CssClass="btn btn-light-green btn-block" />
            </div>
            <div class="col-md-2">
                <asp:Button ID="btnDate" runat="server" Text="18/09/2025" CssClass="btn btn-purple btn-block" />
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnContinue" runat="server" Text="Continue Here" CssClass="btn btn-primary btn-block" OnClick="btnContinue_Click" />
            </div>--%>
        </div>
    </div>

    <!-- Search Existing Request Section -->
    <div class="option-row">
        <div class="row mt-2">
            <div class="col-md-3">
                <asp:RadioButton ID="rbSearch" runat="server" GroupName="RequestOption" Text="Search Existing Request" AutoPostBack="true" OnCheckedChanged="rbSearch_CheckedChanged" />
            </div>
            <div class="col-md-3">
                 <asp:DropDownList ID="ddlSearchFilter" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select Filter" Value="" />
                    <asp:ListItem Text="Safety Pass No." Value="SPN" />
                    <asp:ListItem Text="Req No." Value="REQ" />
                    <asp:ListItem Text="Aadhaar No." Value="ADC" />
                </asp:DropDownList>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtSearch" runat="server" AutoComplete="Off" CssClass="form-control" placeholder="Search Here" />
               
            </div>
            <div class="col-md-3">
                <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-success btn-block" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>
        <br />
       <%--<div class="section-header">Search Results / Last 15 Requests</div>--%>
        <br />
    <!-- GridView or results section goes here -->
</asp:Panel>
    <!-- Grid View -->
    <asp:GridView runat="server" ID="gvReq" CssClass="table table-striped table-bordered table-hover datatable table-responsive"
        AutoGenerateColumns="false" Width="100%" HeaderStyle-BackColor="#FCFCFC" HeaderStyle-ForeColor="black"
        AllowPaging="true" PageSize="10" OnPageIndexChanging="gvReq_PageIndexChanging">
        <Columns>
             <asp:TemplateField HeaderText="SL.NO">
                <ItemTemplate>
                    <%#Container.DataItemIndex + 1 %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Req No.">
                <ItemTemplate>
                    <asp:Label ID="lnk_Request_No" runat="server" Text='<%# Eval("SRQ_REQ_NO") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Created Date">
                <ItemTemplate>
                    <asp:Label ID="lbl_date" runat="server" Text='<%# Eval("SRQ_CREATED_DT", "{0:dd/MM/yyyy}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Request Type">
                <ItemTemplate>
                    <asp:Label ID="lbl_RQ" runat="server" Text='<%# Eval("SRQ_REQ_TYPE") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Work Order">
                <ItemTemplate>
                    <asp:Label ID="lbl_WO" runat="server" Text='<%# Eval("SRQ_WORK_ORDER") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status"  Visible="false" >
                <ItemTemplate>
                    <asp:Label ID="lblStatus" Visible="false" runat="server" Text='<%# Eval("STATUS") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
             <asp:TemplateField HeaderText="Approved Employee"  Visible="false" >
                <ItemTemplate>
                    <asp:Label ID="lbl_emp" Visible="false" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SRD_EMP_APV_COUNT") %>'> </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
          <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:LinkButton ID="btnEdit" runat="server" Text="Click to Continue Editing"
                        CssClass="btn btn-primary btn-sm"
                        OnClick="lnk_Request_No_Click"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

        <PagerStyle CssClass="pagination-outer" HorizontalAlign="Center" />
    </asp:GridView>
</div>

<div runat="server" id="authDiv" visible="false">
<div class="row">
    <div class="col-md-12">
           <div class="slider" id="slider">
            <div class="slides" id="slides">
              <div class="slide"><img src="images/Slide1.png"/></div>
              <div class="slide"><img src="images/Slide2.png"/></div>
              <div class="slide"><img src="images/Slide3.png"/></div>
              <div class="slide"><img src="images/Slide4.png"/></div>
              <div class="slide"><img src="images/Slide5.png"/></div>
                <div class="slide"><img src="images/Slide6.png"/></div>
                <div class="slide"><img src="images/Slide8.png"/></div>
                <div class="slide"><img src="images/Slide9.png"/></div>
                <div class="slide"><img src="images/Slide10.png"/></div>
                <div class="slide"><img src="images/Slide11.png"/></div>
                <div class="slide"><img src="images/Slide12.png"/></div>
            </div>

            <div class="controls">
              <button class="btn-slide" id="prev" aria-label="Previous">◀</button>
              <button class="btn-slide" id="next" aria-label="Next">▶</button>
            </div>

            <div class="dots" id="dots" aria-label="Slide indicators"></div>
          </div>
    </div>
</div>
    <div class="row">
    <div class="col-md-12">
       
<div style="margin-top:15px"> <h5>I have read and understood the detailed process flow and pre-requisites for creation of new safety pass. and have all the required
details to proceed for safety pass creation. </h5></div>
    <!-- Simulated reCAPTCHA -->
 <!-- Add inside your ASPX Content section -->
<div style="display:flex;justify-content: center;">
    <div id="fakeRecaptcha"
    style="border: 1px solid #d3d3d3; padding: 10px; width: 304px; height: 74px; background-color: #f9f9f9; font-family: Arial, sans-serif; box-shadow: 0 0 2px rgba(0,0,0,0.2); display: flex; align-items: center; justify-content: space-between; position: relative;">

    <!-- Left Side: Checkbox + Label -->
    <div style="display: flex; align-items: center;">
        <input type="checkbox" id="chkRobot" onclick="simulateRecaptcha()" style="width: 20px; height: 20px;" />
        <label for="chkRobot" style="margin-left: 10px; font-size: 16px; font-weight: 500;">I'm not a robot</label>
        <div id="spinner" class="spinner-border text-secondary" role="status" style="width: 20px; height: 20px; margin-left: 10px; display: none;">
            <span class="visually-hidden">Loading...</span>
        </div>
        <span id="checkmark" style="font-size: 22px; color: green; margin-left: 10px; display: none;">&#10003;</span>
    </div>

    <!-- Right Side: Logo & Links -->
    <div style="display: flex; align-items: center; font-size: 9px; color: #555;">
        <img src="https://www.gstatic.com/recaptcha/api2/logo_48.png" width="28" style="margin-right: 4px;" />
        <div>
            <div>reCAPTCHA</div>
            <div>
                <a href="https://www.google.com/intl/en/policies/privacy/" target="_blank">Privacy</a> -
                <a href="https://www.google.com/intl/en/policies/terms/" target="_blank">Terms</a>
            </div>`
        </div>
    </div>

</div>
<div style="display:flex;align-items: center;justify-content: center;margin-left: 15px;">
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="btn btn-outline-primary custom-cancel" OnClick="btnCancel_Click" Style="margin-right:10px;" />
    <asp:Button ID="btnSubmit" runat="server" Text="Save and Continue" CssClass="btn btn-primary custom-submit" OnClick="btnComplete_Click" />
</div>
</div>

    <br />
 <!-- Action Buttons -->
<div class="row">
<div class="col-md-12 text-center">
    
</div>
</div>
    </div>
     </div>
</div>

<div id="requisitionBody" runat="server" visible="false" > 
    <div class="row">
         <div class="form-group col-md-3">
            <asp:Label ID="lbl_wono" runat="server" Text="Work Order" /><span class="mandatory">*</span>            
            <asp:TextBox ID="txt_wo_number" OnTextChanged="txt_wo_number_TextChanged" AutoPostBack="true" runat="server" CssClass="form-control TextBoxUpperCase" />

            <ajaxToolkit:AutoCompleteExtender ID="FilteredTextBoxExtender_WO_num" runat="server"
                                                            DelimiterCharacters="" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetWorkorderList"
                                                            ServicePath="" TargetControlID="txt_wo_number" CompletionInterval="100" EnableCaching="true"
                                                            OnClientShowing="clientShowing2" CompletionListCssClass="CompletionListCssClass1"
                                                            CompletionListHighlightedItemCssClass="CompletionListHighlightedItemCssClass1"
                                                            CompletionListItemCssClass="CompletionListItemCssClass1">
                                                        </ajaxToolkit:AutoCompleteExtender>
                                                        <script type="text/javascript">
                                                            function clientShowing2(source, args) {
                                                                source._popupBehavior._element.style.zIndex = 100000;
                                                            }
                                                        </script> 
        </div> 
        <div class="form-group col-md-3">
            <asp:Label ID="Lbl_depart" runat="server" Text="Department" /><span class="mandatory">*</span>            
            <asp:TextBox ID="tbxDept" runat="server" CssClass="form-control TextBoxUpperCase" />

            <ajaxToolkit:AutoCompleteExtender ID="AutoCompleteExtender4" runat="server"
                                                            DelimiterCharacters="" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetDeptList"
                                                            ServicePath="" TargetControlID="tbxDept" CompletionInterval="100" EnableCaching="true"
                                                            OnClientShowing="clientShowing2" CompletionListCssClass="CompletionListCssClass1"
                                                            CompletionListHighlightedItemCssClass="CompletionListHighlightedItemCssClass1"
                                                            CompletionListItemCssClass="CompletionListItemCssClass1">
                                                        </ajaxToolkit:AutoCompleteExtender>
                                                        <script type="text/javascript">
                                                            function clientShowing2(source, args) {
                                                                source._popupBehavior._element.style.zIndex = 100000;
                                                            }
                                                        </script> 
        </div>         

        <div class="form-group col-md-3">
            <asp:Label ID="Lbl_remarks" runat="server" Text="Requisition Type" /><span class="mandatory">*</span>
            <asp:DropDownList ID="ddl_type_requisition" runat="server"  CssClass="form-control" />
        </div>

        <div class="form-group col-md-3">
            <asp:Label ID="Label2" runat="server" Text="Applying Reason" /><span class="mandatory">*</span>
            <asp:DropDownList ID="DropDownremark" runat="server" CssClass="form-control" />
        </div>

        </div>
      <div class="row">
            <div class="col-md-3">
                <asp:Label ID="lblWorkmenType" runat="server" Text="Workmen Type" /><span class="mandatory">*</span>  
                 <asp:DropDownList ID="ddlWorkmenType" runat="server" CssClass="form-control">
                    <asp:ListItem Text="Select Workmen Type" Value="0" />
                    <asp:ListItem Text="Worker" Value="WKR" />
                    <asp:ListItem Text="Supervisor" Value="SPV" />
                    <asp:ListItem Text="Driver" Value="DRV" />
                     <asp:ListItem Text="Facility Manager" Value="FMG" />
                     <asp:ListItem Text="Video Capsule" Value="VDC" />
                </asp:DropDownList>
            </div>
          <div class="col-md-3">
              <asp:Label ID="lblRequiredEmpl" runat="server" Text="Required Employee Count" /><span class="mandatory">*</span>  
                <asp:TextBox ID="Txt_required_emp" runat="server" AutoComplete="Off" CssClass="form-control" />               
            </div>
            <div class="col-md-3">
                <asp:Label ID="lblRemarks" runat="server" Text="Remarks" />
                <asp:TextBox ID="txtremarks" runat="server" AutoComplete="Off" CssClass="form-control" placeholder="Enter Remarks" />
               
            </div>
            <div class="col-md-3">                
                <asp:Button ID="ButtonApply" Style="margin-top: 8%;" runat="server" Text="Apply" CssClass="btn btn-success btn-block" OnClick="ButtonApply_Click" />
            </div>
        </div>
   <div class="row">
    <div runat="server" ID="wo" Visible="false">
        <div id="WOView" class="wo-view-container">
            <table class="table uniform-table wo-view-table">
                <tr>
                    <td colspan="2" class="section-header-cell">CONTRACTOR'S DETAILS</td>
                    <td colspan="2" class="section-header-cell">WORK ORDER DETAILS</td>
                </tr>
                <tr>
                    <td class="label-cell">
                        <asp:Label ID="Label11" runat="server" CssClass="cslabel" Text="Labour Licence "></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="txtlabourLicence" runat="server" CssClass="cslabel1"></asp:Label>
                    </td>
                    <td class="label-cell">
                        <asp:Label ID="Label1" runat="server" CssClass="cslabel" Text="Work Order Number "></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="LblWoNumber" runat="server" CssClass="cslabel1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label-cell">
                        <asp:Label ID="Lbl_manpower" runat="server" CssClass="cslabel" Text="Labour Licence Quota"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="txt_manpower" runat="server" CssClass="cslabel1"></asp:Label>
                    </td>
                    <td class="label-cell">
                        <asp:Label ID="lbl_wovalid_dt" runat="server" CssClass="cslabel" Text="Work Order Validity"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="txt_wovalid_dt" runat="server" CssClass="cslabel1"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label-cell">
                        <asp:Label ID="lbl_manpower_req" runat="server" CssClass="cslabel" Text="Safety Pass Quota"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="Txt_manpower_req" runat="server" CssClass="cslabel1 "></asp:Label>
                    </td>
                    <td class="label-cell">
                        <asp:Label ID="Label3" runat="server" CssClass="cslabel" Text="RFID Activated" Visible="false"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="Lbl_ActiveRFIDWo" runat="server" CssClass="cslabel1" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label-cell">
                        <asp:Label ID="Lbl_activeRFID" runat="server" CssClass="cslabel" Text="Total Active RFID"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="txt_activeRFID" runat="server" CssClass="cslabel1"></asp:Label>
                    </td>
                    <td class="label-cell">
                        <asp:Label ID="Label5" runat="server" CssClass="cslabel" Text="Approved RFID Under Process" Visible="false"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="lbl_Rfid_que_wo" runat="server" CssClass="cslabel1" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label-cell">
                        <asp:Label ID="rfid_queue" runat="server" CssClass="cslabel" Text="Total RFID Under Process"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="txt_rfid_queue" runat="server" CssClass="cslabel1" ForeColor="Blue"></asp:Label>
                    </td>
                    <td class="label-cell">
                        <asp:Label ID="Label6" runat="server" CssClass="cslabel" Text="Requested Certificates" Visible="false"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="Lbl_Register_RFID" runat="server" CssClass="cslabel1" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="label-cell">
                        <asp:Label ID="lbl_RFID_Quota" runat="server" CssClass="cslabel" Text="Total RFID Quota Available"></asp:Label>
                    </td>
                    <td class="value-cell">
                        <asp:Label ID="Txt_RFID_Quota" runat="server" CssClass="cslabel1 "></asp:Label>
                    </td>
                    <td class="label-cell">
                        <asp:LinkButton ID="LnkRFID_list" runat="server" CssClass="cslabel" Text="click to View details of RFID" Visible="false"></asp:LinkButton>
                    </td>
                    <td></td>
                </tr>
            </table>
        </div>
    </div>
</div>   
</div>  
   

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="scriptsSection" runat="Server">
    <script type="text/javascript">
        function simulateRecaptcha() {
            var checkbox = document.getElementById("chkRobot");
            var spinner = document.getElementById("spinner");
            var checkmark = document.getElementById("checkmark");

            if (checkbox.checked) {
                spinner.style.display = "inline-block";
                checkmark.style.display = "none";

                setTimeout(function () {
                    spinner.style.display = "none";
                    checkmark.style.display = "inline-block";
                }, 1500);
            } else {
                spinner.style.display = "none";
                checkmark.style.display = "none";
            }
        }

        function checkFakeRecaptcha() {
            var checkbox = document.getElementById("chkRobot");
            var checkmark = document.getElementById("checkmark");

            if (!checkbox.checked || checkmark.style.display !== "inline-block") {
                alert("Please verify you're not a robot.");
                return false;
            }
            return true;
        }
    </script>

   <script>
    (function () {
      const slides = document.getElementById('slides');
      const prev = document.getElementById('prev');
      const next = document.getElementById('next');
      const dotsWrap = document.getElementById('dots');

      const total = slides.children.length;
      let index = 0;
      let timer = null;
      const intervalMs = 3000; // autoplay interval

      // build dots
      for (let i = 0; i < total; i++) {
        const dot = document.createElement('span');
        dot.className = 'dot' + (i === 0 ? ' active' : '');
        dot.dataset.idx = i;
        dot.addEventListener('click', (e) => goTo(+e.target.dataset.idx));
        dotsWrap.appendChild(dot);
      }
      const dots = Array.from(dotsWrap.children);

      function render() {
        slides.style.transform = `translateX(-${index * 100}%)`;
        dots.forEach((d, i) => d.classList.toggle('active', i === index));
      }
      function goTo(i) {
        index = (i + total) % total;
        render();
        resetAutoplay();
      }
      function nextSlide() { goTo(index + 1); }
      function prevSlide() { goTo(index - 1); }
        
        function startAutoplay() {
            if (timer) return; // prevent multiple intervals
            timer = setInterval(nextSlide, intervalMs);
        }

        function stopAutoplay() {
            if (timer) {
            clearInterval(timer);
            timer = null;
            }
        }

        function resetAutoplay() {
            stopAutoplay();
            startAutoplay();
        }
     

      // controls
        next.addEventListener('click', (e) => {
          e.preventDefault();  // stop postback
          e.stopPropagation();
            nextSlide();
            resetAutoplay();
        });
        prev.addEventListener('click', (e) => {
          e.preventDefault();
          e.stopPropagation();
            prevSlide();
            resetAutoplay();
        });

      // pause on hover for accessibility
      const slider = document.getElementById('slider');
      slider.addEventListener('mouseenter', stopAutoplay);
      slider.addEventListener('mouseleave', startAutoplay);

      // init
      render();
      startAutoplay();
    })();
  </script>
</asp:Content>


