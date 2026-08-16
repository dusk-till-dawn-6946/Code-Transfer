<%@ Page Language="C#" MasterPageFile="~/MenuMaster.Master" AutoEventWireup="true" CodeFile="frmProfileCreation.aspx.cs" Inherits="frmProfileCreation" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderStyleContent" runat="server">
    <style>
    
        @keyframes progressAnimation {
            from { width: 0%; }
            to { width: 25%; } /* Adjust based on current step */
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
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div style="margin-left:13px">
    <strong style="font-size: 25px;">Profile Creation</strong>
    <h4>Personal Information</h4>
</div>
<div class="row">
    <div class="col-md-12">
        <div class="col-md-12">
        <div class="progress-container">
            <div class="progress" style="height: 8px;">
                <div class="progress-bar progress-bar-animated" role="progressbar"
                     aria-valuenow="25" aria-valuemin="0" aria-valuemax="100">
                </div>
            </div>
            <div class="progress-label">Step 1 of 4</div>
        </div>
    </div>
    </div>
</div>
 <div class="form-section">

     <%--<div runat="server" id="gridDiv">
         <table width="98%" border="0" style="border-collapse: collapse;">
    <tr style="background-color: #E0F2F7;">
        <td width="20%" class="styleM" style="padding: 8px; border: 1px solid #B0E0E6; text-align: center;">
            <asp:LinkButton ID="lblreq" runat="server" Text="REQUEST NUMBER: " Style="color: black;font-weight: bolder;" ToolTip="Download the Checklist of the Employee"
                onMouseOver="this.style.color='Green'" onMouseOut="this.style.color='black'"
                CssClass="link-button" />
        </td>
        <td width="15%" class="styleM" style="padding: 8px; border: 1px solid #B0E0E6; text-align: center;">
            <asp:LinkButton ID="lnkSup" runat="server" OnClick="lnkSup_Click" Text="SUPERVISOR: " ToolTip="click to enter details of supervisor"
                Style="color: black;font-weight: bolder;" onMouseOver="this.style.color='Green'" onMouseOut="this.style.color='black'"
                CssClass="link-button" />
        </td>
        <td class="styleM" width="12%" style="padding: 8px; border: 1px solid #B0E0E6; text-align: center;">
            <asp:LinkButton ID="lnkWrk" runat="server" OnClick="lnkWrk_Click" Text="WORKER: " ToolTip="click to enter details of worker"
                Style="color: black;font-weight: bolder;" onMouseOver="this.style.color='Green'" onMouseOut="this.style.color='black'"
                CssClass="link-button" />
        </td>
        <td class="styleM" style="padding: 8px; border: 1px solid #B0E0E6; text-align: center;">
            <asp:LinkButton ID="LnkDR" runat="server" OnClick="LnkDR_Click" Text="DRIVER: " ToolTip="click to enter details of Driver"
                Style="color: black;font-weight: bolder;" onMouseOver="this.style.color='Green'" onMouseOut="this.style.color='black'"
                CssClass="link-button" />
        </td>
        <td class="styleM" style="padding: 8px; border: 1px solid #B0E0E6; text-align: center;">
            <asp:LinkButton ID="LnkFM" runat="server" OnClick="LnkFM_Click" Text="FACILITY MANAGER: " ToolTip="click to enter details of Manager"
                Style="color: black;font-weight: bolder;" onMouseOver="this.style.color='Green'" onMouseOut="this.style.color='black'"
                CssClass="link-button" />
        </td>
        <td class="styleM" id="TDvc" runat="server" style="padding: 8px; border: 1px solid #B0E0E6; text-align: center;">
            <asp:LinkButton ID="LnkVC" runat="server" OnClick="LnkVC_Click" Text="VIDEO CAPSULE: " ToolTip="click to enter details of video Capsule"
                Style="color: black;font-weight: bolder;" onMouseOver="this.style.color='Green'" onMouseOut="this.style.color='black'"
                CssClass="link-button" />
        </td>
    </tr>
</table>
     </div>--%>

     <div runat="server" id="contentDiv" visible="true">
         <!-- Row 4 -->
    <div class="row">            
        <div class="form-group col-md-4">
            <asp:Label ID="lblUniqIDType" runat="server" Text="Unique ID Type" /><span class="mandatory">*</span>
            <asp:DropDownList ID="cmbUniqID" runat="server" CssClass="form-control" OnSelectedIndexChanged="cmbUniqID_SelectedIndexChanged" AutoPostBack="true" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblUniqIDNo" runat="server" Text="Unique ID No." /><span class="mandatory">*</span>
            <asp:TextBox ID="txtUniqIDNo" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" onkeypress="return isNumberAlphaKey(event)"  onblur="checkMandatory(this);" OnTextChanged="txtUniqIDNo_valchanged" MaxLength="20" AutoPostBack="true" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblAffirmative" runat="server" Text="Affirmative" /><span class="mandatory">*</span>
            <asp:DropDownList ID="cmbAffirmative" runat="server" CssClass="form-control" />
        </div>
    </div>
    <!-- Row 1 -->
    <div class="row">
        <div class="form-group col-md-4">
            <asp:Label ID="lblCategory" runat="server" Text="Category" /><span class="mandatory">*</span>
            <asp:DropDownList ID="cmbCategory" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblDept" runat="server" Text="Department" /><span class="mandatory">*</span>
            <asp:TextBox ID="Txtdeprt" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" ReadOnly="True" onblur="checkMandatory(this);" ondblclick="SHOW_VALUE(this);" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblFName" runat="server" Text="First Name" /><span class="mandatory">*</span>
            <asp:TextBox ID="txtFName" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="40" onblur="checkMandatory(this);" ondblclick="SHOW_VALUE(this);" />
        </div>
    </div>

    <!-- Row 2 -->
    <div class="row">
        <div class="form-group col-md-4">
            <asp:Label ID="lblLName" runat="server" Text="Last Name" />
            <asp:TextBox ID="txtLName" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="40" ondblclick="SHOW_VALUE(this);" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblFatherName" runat="server" Text="Father Name" /><span class="mandatory">*</span>
            <asp:TextBox ID="txtFatherName" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="80" ondblclick="SHOW_VALUE(this);" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblHusName" runat="server" Text="Spouse Name" />
            <asp:TextBox ID="txtHusName" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="80" ondblclick="SHOW_VALUE(this);" />
        </div>
    </div>

    <!-- Row 3 -->
    <div class="row">
        <div class="form-group col-md-4">
            <asp:Label ID="lblSex" runat="server" Text="Gender" /><span class="mandatory">*</span>
            <asp:DropDownList ID="cmbSex" runat="server" CssClass="form-control">
                <asp:ListItem Text="[Select]" Value="0"></asp:ListItem>
                <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                <asp:ListItem Text="Transgender" Value="T"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblIdentiFication" runat="server" Text="Identity Mark" /><span class="mandatory">*</span>
            <asp:TextBox ID="txtIdentiFication" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="40" onblur="checkMandatory(this);" ondblclick="SHOW_VALUE(this);" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="lblDOB" runat="server" Text="Date of Birth" /><span class="mandatory">*</span>
            <asp:TextBox ID="txtDOB" runat="server" CssClass="form-control" TextMode="Date" AutoPostBack="false" />
        </div>
    </div>
    
     <!-- Row 5 -->
    <div class="row">    
        <div class="form-group col-md-4">
            <asp:Label ID="lblWorkArea" runat="server" Text="Area of Work" /><span class="mandatory">*</span>
            <asp:DropDownList ID="cmbWorkArea" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group col-md-4">
            <asp:Label ID="Label33" runat="server" Text="Medical Centre" /><span class="mandatory">*</span>
            <asp:DropDownList ID="ddlMedCentre" runat="server" CssClass="form-control">
                <asp:ListItem Text="[Select]" Value="0"></asp:ListItem>
                <asp:ListItem Text="Arogya Bhawan" Value="A"></asp:ListItem>
                <asp:ListItem Text="Outside" Value="O"></asp:ListItem>                                                                
            </asp:DropDownList>
        </div>        
    </div>

    <!-- Row 6 -->
    <div class="row">
    <div class="form-group col-md-4">
        <asp:Label ID="lblPhNo" runat="server" Text="Phone Number" /><span class="mandatory">*</span>
        <div class="input-group">
            <span class="input-group-addon" style="white-space: nowrap;">
                <span style="display: inline-block; vertical-align: middle;">
                    <img src="images/india-flag.png" alt="India Flag" style="height:16px; vertical-align: middle;" />
                </span>
                <span style="display: inline-block; vertical-align: middle; margin: 0px 6px 0 2px;">+91</span>
            </span>
            <asp:TextBox ID="txtPhNo" runat="server" CssClass="form-control" AutoComplete="Off" onkeypress="return isNumberKey(event)" MaxLength="10" />
        </div>
    </div>

    <div class="form-group col-md-4">
        <asp:Label ID="lblEmrgNo" runat="server" Text="Emergency Contact Number" /><span class="mandatory">*</span>
        <div class="input-group">
            <span class="input-group-addon" style="white-space: nowrap;">
                <span style="display: inline-block; vertical-align: middle;">
                    <img src="images/india-flag.png" alt="India Flag" style="height:16px; vertical-align: middle;" />
                </span>
                <span style="display: inline-block; vertical-align: middle; margin: 0px 6px 0 2px;">+91</span>
            </span>
            <asp:TextBox ID="txtEmrgNo" runat="server" CssClass="form-control" AutoComplete="Off" onkeypress="return isNumberKey(event)" MaxLength="11" />
        </div>
    </div>
        <div class="form-group col-md-4">
            <asp:Label ID="Lblspno" runat="server" Text="Safety Pass Number" ForeColor="Green" Visible="False" />
            <asp:TextBox ID="TxtSpno" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="40" Visible="False" ReadOnly="True" />
        </div>
</div>
         <div runat="server" id="pnlFormA" visible="false">
             <div class="row">    
                <div class="form-group col-md-4">
                    <asp:Label ID="lblPAN" runat="server" Text="PAN No" /><span class="mandatory">*</span>
                    <asp:TextBox ID="txtPAN" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="10" />                    
                </div>
                <div class="form-group col-md-4">
                    <asp:Label ID="lblAADHAR" runat="server" Text="Aadhar No" /><span class="mandatory">*</span>
                    <asp:TextBox ID="txtAADHAR" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="12" />                    
                </div>
                <div class="form-group col-md-4">
                    <asp:Label ID="lblNationality" runat="server" Text="Nationality" /><span class="mandatory">*</span>
                    <asp:DropDownList ID="cmbNationality" runat="server" CssClass="form-control">
                        <asp:ListItem>[Select]</asp:ListItem>
                        <asp:ListItem>Indian</asp:ListItem>
                        <asp:ListItem>Other</asp:ListItem>
                    </asp:DropDownList>
                </div>
               </div>
             <div class="row">    
                <div class="form-group col-md-4">
                    <asp:Label ID="lblPlaceOfEmployment" runat="server" Text="Place of Employment" /><span class="mandatory">*</span>
                    <asp:DropDownList ID="cmbPlaceOfEmployment" runat="server" CssClass="form-control">
                        <asp:ListItem>[Select]</asp:ListItem>
                        <asp:ListItem>Underground</asp:ListItem>
                        <asp:ListItem>Opencast</asp:ListItem>
                        <asp:ListItem>Surface</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="form-group col-md-4">
                    <asp:Label ID="Label29" runat="server" Text="Relay Data" /><span class="mandatory">*</span>
                    <asp:DropDownList ID="cmbRelayData" runat="server" CssClass="form-control" />
                </div>  
                 <div class="form-group col-md-4">
                    <asp:Label ID="lblAdltMobile" runat="server" Text="Mobile No" /><span class="mandatory">*</span>
                    <asp:TextBox ID="txtAdltMobile" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="10" onkeypress="return isNumberKey(event)" />  
                </div>
               </div>

             <div class="row">    
                <div class="form-group col-md-4">
                    <asp:Label ID="lblAdltName" runat="server" Text="Name" /><span class="mandatory">*</span>
                    <asp:TextBox ID="txtAdltName" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="100" />                    
                </div>
                <div class="form-group col-md-4">
                    <asp:Label ID="lblAdltRelation" runat="server" Text="Relation" /><span class="mandatory">*</span>
                    <asp:DropDownList ID="cmbAdltRelation" runat="server" CssClass="form-control" />                  
                </div>
                <div class="form-group col-md-4">
                    <asp:Label ID="lblAdltAddress" runat="server" Text="Address" /><span class="mandatory">*</span>
                    <asp:TextBox ID="txtAdltAddress" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="160" />  
                </div>
               </div>
            </div>

        <div class="row">
            <div class="col-md-12 text-left">                
                <asp:Button ID="btnSaveProfile" runat="server" AutoPostBack="true" Text="Save Profile" OnClick="btnSaveProfile_Click" CssClass="btn btn-primary custom-submit" />
                <%--<asp:Button ID="btnUpdateProfile" runat="server" Visible="false" Text="Update Profile" CssClass="btn btn-primary custom-submit" />--%>
            </div>
        </div>
       
</div>


     <div runat="server" id="ageaddressDiv" visible="false">
       


        <hr />
        <!-- Address Row 1 -->
        <div class="row">
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddressType" runat="server" Text="Address Type" /><span class="mandatory">*</span>
                <asp:DropDownList ID="cmbAddressType" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddName" runat="server" Text="Care of" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddName" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddHouseNo" runat="server" Text="House/Plot/Door No/Land Mark" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddHouseNo" runat="server" CssClass="form-control" AutoComplete="Off" MaxLength="50" />
            </div>
        </div>

        <!-- Address Row 2 -->
        <div class="row">
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddStreet" runat="server" Text="Street" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddStreet" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="50" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddVillage" runat="server" Text="Village" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddVillage" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="50" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddPO" runat="server" Text="Post Office" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddPO" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="50" />
            </div>            
        </div>

        <!-- Address Row 3 -->
        <div class="row">
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddThana" runat="server" Text="Police Station" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddThana" runat="server" CssClass="form-control TextBoxUpperCase" AutoComplete="Off" MaxLength="50" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddCountry" runat="server" Text="Country" /><span class="mandatory">*</span>
                <asp:DropDownList ID="cmbAddCountry" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddState" runat="server" Text="State" /><span class="mandatory">*</span>
                <asp:DropDownList ID="cmbAddState" runat="server" OnSelectedIndexChanged="cmbAddState_SelectedIndexChanged" CssClass="form-control" AutoPostBack="True" />
            </div>
        </div>

         <!-- Address Row 4 -->
        <div class="row">
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddCity" runat="server" Text="City" /><span class="mandatory">*</span>
                <asp:DropDownList ID="cmbAddCity" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddDistrict" runat="server" Text="District" /><span class="mandatory">*</span>
                <asp:DropDownList ID="cmbAddDistrict" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddPIN" runat="server" Text="Pincode" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddPIN" runat="server" CssClass="form-control" AutoComplete="Off" MaxLength="6" onkeypress="return isNumberKey(event)" />
            </div>            
        </div>

        <!-- Address Row 5 -->
        <div class="row">
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddMobile" runat="server" Text="Mobile No" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddMobile" runat="server" CssClass="form-control" AutoComplete="Off" MaxLength="10" onkeypress="return isNumberKey(event)" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblLandLine" runat="server" Text="Land Line" />
                <asp:TextBox ID="txtLandLine" runat="server" CssClass="form-control" AutoComplete="Off" MaxLength="11" onkeypress="return isNumberKey(event)" />
            </div>
            <div class="form-group col-md-4">
                <asp:Label ID="lblAddEmail" runat="server" Text="Email ID" /><span class="mandatory">*</span>
                <asp:TextBox ID="txtAddEmail" runat="server" CssClass="form-control" TextMode="Email" AutoComplete="Off" MaxLength="50" />
            </div>
        </div>

        <!-- Upload Section -->
    <div class="row">
        <div class="form-group col-md-4">
            <asp:Label ID="lbl_attachment" runat="server" Text="Upload Address Proof" /><span class="mandatory">*</span>
            <asp:FileUpload ID="fupdl_add" runat="server" CssClass="form-control" />
            <small>Aadhaar / Voter / Ration Card</small>
            <br />
            <asp:CheckBox runat="server" CssClass="lblStyle" ID="ChkoldAddress" Text="Attach previous documents" />
            <asp:HiddenField runat="server" ID="hddaddressold" Value="" />
            <asp:ImageButton runat="server" ID="imgaddressold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
        </div>
        <%--<div class="form-group col-md-4">
            <asp:Label ID="lblPoliceVerification" runat="server" Text="Upload Police Verification" />
            <asp:FileUpload ID="fuPoliceVerification" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group col-md-4" style="margin-top: 25px;">
            <asp:HyperLink ID="hlDownloadFormat" runat="server" NavigateUrl="#" CssClass="download-link" Text="Download Format" />
        </div>--%>
    </div>

      <div class="row" runat="server" id="pnlAddressDetail">
             <asp:GridView runat="server" ID="gvAddress" CssClass="table table-striped table-bordered table-hover datatable table-responsiv"
                                            AutoGenerateColumns="false" Width="100%" HeaderStyle-BackColor="#428bca" HeaderStyle-ForeColor="White" OnRowDataBound="gvAddress_RowDataBound">
                                            <Columns>
                                                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                        ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelectAddress" runat="server" OnCheckedChanged="chkSelectAddress" AutoPostBack="true" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="ADDRESS_TYPE_DESC" HeaderText="Address type" />
                                                                    <asp:BoundField DataField="CCA_NAME" HeaderText="Care Of" />
                                                                    <asp:BoundField DataField="HOUSE_NO" HeaderText="H/No" />
                                                                    <asp:BoundField DataField="STREET" HeaderText="Street" />
                                                                    <asp:BoundField DataField="CCA_VILLAGE" HeaderText="Village" />
                                                                    <asp:BoundField DataField="CCA_PO" HeaderText="PO" />
                                                                    <asp:BoundField DataField="CCA_THANA" HeaderText="Thana" />
                                                                    <asp:BoundField DataField="CIT_CITY_NAME" HeaderText="City" />
                                                                    <asp:BoundField DataField="DST_DISTRICT_NAME" HeaderText="District" />
                                                                    <asp:BoundField DataField="SMT_STATE_NAME" HeaderText="State" />
                                                                    <asp:BoundField DataField="CMT_COUNTRY_NAME" HeaderText="Country" />
                                                                    <asp:BoundField DataField="CCA_PIN" HeaderText="PIN" />
                                                                    <asp:BoundField DataField="CCA_MOBILE" HeaderText="Mobile" />
                                                                    <asp:BoundField DataField="CCA_EMAIL" HeaderText="Email" />
                                                                    <asp:BoundField DataField="CCA_LAND_LINE" HeaderText="Land Line" />
                                                                    <asp:BoundField DataField="CCA_START_DT" HeaderText="From Dt" Visible="false" />
                                                                    <asp:BoundField DataField="CCA_END_DT" HeaderText="To Dt" Visible="false" />
                                                                    <asp:TemplateField HeaderText="Atttachment">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="lnkexp" runat="server" Text='<%#Eval("DM_NAME") %>' CommandArgument='<%#Eval("CCA_ADDRESS_ID") %>' OnClick="downloadadd" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidAddressID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCA_ADDRESS_ID") %>' />
                                                                            <asp:HiddenField ID="hidremark" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCA_REMARKS") %>' />
                                                                            <asp:HiddenField ID="hiddocseq" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCA_CERT_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CCA_REQ_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidAddType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ADDRESS_TYPE") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidAddState" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "STATE_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidAddCountry" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "COUNTRY_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidAddCity" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CITY_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidAddDistrict" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCA_DISTRICT_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                        </asp:GridView>  
         </div>
        

          <hr />
         <!-- Age Row 1 -->
         <div class="row">
            <div class="form-group col-md-4">
                <asp:Label ID="lbl_age" runat="server" Text="Upload Age Proof" /><span class="mandatory">*</span>
                <asp:FileUpload ID="fupdlage" runat="server" CssClass="form-control" />
                <small>Birth Certificate / Aadhaar / Passport / PAN</small>
                <br />
                <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkageold" Text="Attach previous documents" />
                <asp:ImageButton runat="server" ID="imbageold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                <asp:HiddenField runat="server" ID="hdfageold" Value="" />
            </div> 
             <div class="form-group col-md-4">
                <asp:Label ID="lbl_drv" runat="server" Text="Driving License" />
                <asp:FileUpload ID="fupdldrv" runat="server" CssClass="form-control" />
                <small>(Driving license is mandatory for driver)</small>
                 <br />
                 <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkdriverold" Text="Attach previous documents" />
                 <asp:ImageButton runat="server" ID="imbdriverold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                <asp:HiddenField runat="server" ID="hdfdriverold" Value="" />
            </div> 
             <div class="form-group col-md-4">
                <asp:Label ID="lbl_passport" runat="server" Text="Passport Doc." />
                <asp:FileUpload ID="fupdlpass" runat="server" CssClass="form-control" />
                <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkpassold" Text="Attach previous documents" />
                <asp:ImageButton runat="server" ID="imgpassold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                <asp:HiddenField runat="server" ID="hdfpassold" Value="" />
            </div> 
        </div>

           <div class="row" runat="server" id="Div1">
             <asp:GridView runat="server" ID="grdage" CssClass="table table-striped table-bordered table-hover datatable table-responsiv"
                                            AutoGenerateColumns="false" Width="100%" HeaderStyle-BackColor="#428bca" HeaderStyle-ForeColor="White" OnRowDataBound="gvAge_RowDataBound">
                                            <Columns>
                                                            <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="chkSelectage" runat="server" OnCheckedChanged="chkSelectAge" AutoPostBack="true" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Age Proof" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkdownloadage" runat="server" Text='<%#Eval("DOB") %>' CommandArgument='<%#Eval("DOBDOCID") %>' OnClick="downloadage" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Driving License Proof" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkdownloaddrv" runat="server" Text='<%#Eval("DRV") %>' CommandArgument='<%#Eval("DRVDOCID") %>' OnClick="downloaddrv" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="Passport Proof" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkdownloadpass" runat="server" Text='<%#Eval("PASS") %>' CommandArgument='<%#Eval("PASSDOCID") %>' OnClick="downloadpass" />
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdage" runat="server" Value='<%#Bind("DOBDOCID") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                                <HeaderStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hddrv" runat="server" Value='<%#Bind("DRVDOCID") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                                <HeaderStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hdpass" runat="server" Value='<%#Bind("PASSDOCID") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                                <HeaderStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="HiddenField1" runat="server" Value='<%#Bind("CET_REQUEST_NO") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                                <HeaderStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                        </asp:GridView>  
         </div>

         <hr />
         <!-- Police Verification Row 1 -->
         <div class="row">
           <div class="form-group col-md-4">
            <asp:Label ID="lbl_frmmsg" runat="server" Text="Police Verification Date:" /><span class="mandatory">*</span>
            <asp:TextBox ID="txt_frmdt" runat="server" CssClass="form-control" TextMode="Date" />
        </div>
             <div class="form-group col-md-4" runat="server" id="divToDate" visible="false">
            <asp:Label ID="lbl_todt" runat="server" Text="To Date:" /><span class="mandatory">*</span>
            <asp:TextBox ID="txt_todt" runat="server" Enabled="false" CssClass="form-control" TextMode="Date" />
        </div>
             <div class="form-group col-md-4">
                <asp:Label ID="Label2" runat="server" Text="Police Verification Doc./Undertaking" /><span class="mandatory">*</span>
                <asp:FileUpload ID="updl_file" runat="server" CssClass="form-control" />    
                 <asp:Label ID="lbl_filename" runat="server" Width ="100%" />
                 <asp:HiddenField runat="server" ID="hidcat" />
                 <asp:HiddenField ID="hidcertid" runat="server" /> 
            </div> 
        </div>

           <div class="row" runat="server" id="Div2">
             <asp:GridView runat="server" ID="grdpv" CssClass="table table-striped table-bordered table-hover datatable table-responsiv"
                                            AutoGenerateColumns="false" Width="100%" HeaderStyle-BackColor="#428bca" HeaderStyle-ForeColor="White" OnRowDataBound="gvPV_RowDataBound">
                                            <Columns>
                                      <asp:TemplateField>
                                         <ItemTemplate>
                                             <asp:CheckBox ID="grdchk" runat="server" autopostback="true" OnCheckedChanged="chkSelectPV"/> 
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                     <asp:BoundField DataField="CPDT_SAFETY_PASS_NO" HeaderText="Safety pass No"/>
                                     <asp:TemplateField HeaderText="Workmen Name">
                                         <ItemTemplate>
                                             <asp:Label ID="ls_name" runat="server" Text='<%#Bind("Name") %>'/>
                                         </ItemTemplate> 
                                     </asp:TemplateField>
                                     <asp:BoundField DataField="stdt" HeaderText="From Date" />
                                     <asp:BoundField DataField ="enddt" HeaderText="To Date" />
                                     <asp:BoundField DataField ="crtdt" HeaderText="Apply Date" />
                                      <asp:BoundField DataField ="st" HeaderText="status" />
                                     <asp:BoundField DataField ="SDV_REMARKS" HeaderText="Remarks" />
                                     <asp:BoundField DataField="typefile" HeaderText="Attachment Type" />
                                     <asp:TemplateField HeaderText="Attachment">
                                         <ItemTemplate>
                                           <asp:LinkButton ID="lnkdownloadpv" runat="server" Text='<%#Eval("DM_NAME") %>' CommandArgument='<%#Eval("DM_DOC_ID") %>' onclick="downloadpv"/>
                                         </ItemTemplate>
                                     </asp:TemplateField>
                                      <asp:TemplateField HeaderText="">
                                         <ItemTemplate>
                                             <asp:HiddenField ID="grdpvid" runat="server" value='<%#Bind("pvid") %>' /> 
                                         </ItemTemplate>
                                          <ItemStyle CssClass ="hide" />
                                          <HeaderStyle CssClass ="hide" />
                                     </asp:TemplateField>
                                     <asp:TemplateField HeaderText="">
                                         <ItemTemplate>
                                             <asp:HiddenField ID="grddoctype" runat="server" value='<%#Bind("cpdt_doc_type") %>' /> 
                                         </ItemTemplate>
                                          <ItemStyle CssClass ="hide" />
                                          <HeaderStyle CssClass ="hide" />
                                     </asp:TemplateField>
                                     <asp:TemplateField HeaderText="">
                                         <ItemTemplate>
                                             <asp:HiddenField ID="hidpvcerno" runat="server" value='<%#Bind("DM_DOC_ID") %>' /> 
                                         </ItemTemplate>
                                          <ItemStyle CssClass ="hide" />
                                          <HeaderStyle CssClass ="hide" />
                                     </asp:TemplateField>
                                      
                                 </Columns>
                                        </asp:GridView>  
         </div>

<!-- Action Buttons -->         
        <div class="row" runat="server" id="actionDivID">
            <div class="col-md-12 text-center">
                <asp:Button ID="btnCancel" runat="server" Text="Back" CssClass="btn btn-outline-primary custom-cancel" Visible="false" OnClick="btnCancel_Click" Style="margin-right:10px;" />
                <asp:Button ID="btnSubmit" Visible="True" runat="server" OnClientClick="return checkBlankSP();" Text="Save and Continue" CssClass="btn btn-primary custom-submit" OnClick="btnComplete_Click" />
                <asp:Button ID="btnUpdate" runat="server" OnClientClick="return checkBlankSP();" Text="Update" Visible="False" CssClass="btn btn-primary custom-submit" OnClick="btnUpdateAll_Click" Style="margin-right:10px;" />
                <asp:Button ID="btnContinue" runat="server" OnClientClick="return checkBlankSP();" Text="Next" Visible="False" CssClass="btn btn-outline-primary custom-cancel" OnClick="btnContinue_Click" />
            </div>
        </div>
   </div>

     <ajaxToolkit:ModalPopupExtender ID="MPopUpConfirmDocSubmision" runat="server"
    BackgroundCssClass="modalBackground" TargetControlID="lblDummy" PopupControlID="pnlConfirmDocSubmision">
</ajaxToolkit:ModalPopupExtender>

<asp:Panel runat="server" ID="pnlConfirmDocSubmision" CssClass="modalPopup" Style="display: none">
    <asp:Label runat="server" ID="lblDummy"></asp:Label>
    <asp:HiddenField runat="server" ID="hfActionPerformed" />

    <div class="popupHeader">Confirmation Required</div>

    <div class="popupText">
        <p>You need to attach department’s chief approval for persons above 60 years of age at the time of generating a safety pass.</p>
        <p>Do you want to go ahead?</p>
    </div>

    <div class="buttonContainer">
        <asp:Button runat="server" ID="btnConfirmDocSubmision" Text="Yes" CssClass="buttonStyle" />
        <asp:Button runat="server" ID="btnCancelDocSubmisio" Text="No" CssClass="buttonStyle" />
    </div>
</asp:Panel>

      <asp:Label ID="lblPFESI" runat="server" Style="display: none;"></asp:Label>
               <asp:Label ID="Label1" runat="server" Style="display: none;"></asp:Label>
<ajaxToolkit:ModalPopupExtender ID="mpconfirmsubmit" runat="server" TargetControlID="lblPFESI"
    PopupControlID="pnlconfirmsubmit" BackgroundCssClass="modalBackground" DropShadow="true"
    PopupDragHandleControlID="dragSubVendor" RepositionMode="RepositionOnWindowResizeAndScroll"
    Drag="true" OkControlID="ibtnClosesubmit" />
<asp:Panel ID="pnlconfirmsubmit" runat="server" CssClass="modalPopup" Style="display: none"
    Width="500px">
    <div class="popupHeader">Capturing Of UAN and ESIC Number</div>
    <asp:Label ID="lblpfesiErrMsg" runat="server" Text="" CssClass="errorMessage"></asp:Label>
    <asp:Label ID="lbluan" runat="server" CssClass="labelStyle" Text="Fill UAN Number(under EPFO Act)[12 digit]:" />
    <asp:TextBox ID="txtuan" runat="server" CssClass="textBoxStyle" MaxLength="12"></asp:TextBox>

    <asp:Label ID="Label5" runat="server" CssClass="labelStyle" Text="Fill IP Number(under ESIC Act)[10 digit]:" />
    <asp:TextBox ID="txtip" runat="server" CssClass="textBoxStyle" MaxLength="10"></asp:TextBox>

    <div style="text-align: center; margin-top: 15px;">
        <asp:Button ID="ibtnCloseconfirmsubmit" runat="server" CssClass="buttonStyle" Text="Save" />
        <asp:Button ID="ibtnClosesubmit" runat="server" CssClass="buttonStyle" Text="Cancel" />
    </div>
</asp:Panel>

<asp:HiddenField ID="hiddob" runat="server" />
<asp:HiddenField ID="hiddrv" runat="server" />
<asp:HiddenField ID="hidpass" runat="server" />
 </div>

<script type="text/javascript">
    function checkMandatory(objControl) {
            if (objControl.value == "") {
                objControl.style.background = '#FFFFCC';
                alert("Please enter the value");
                return false;
            }
            else {
                objControl.style.background = 'White';
            }
    }

    function SHOW_VALUE(ctrl) {

            document.getElementById("hidCtrlName").value = ctrl.name;

            document.getElementById("div1").style.display = 'inline';
            document.getElementById("mymsg").value = ctrl.value;
        }
    function checkBlankSP() {
            try {
                var obj = document.getElementById("ctl00_ContentPlaceHolder1_txtSPNo");
                if (obj.value == "") {
                    alert("Please enter the Safety Pass Number");
                    return false;
                }
            }
            catch (e) {

            }
    }
     function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }

    function isNumberAlphaKey(evt) {

        if (document.getElementById("<%= cmbUniqID.ClientID %>").value == "ADC") {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;
            if (document.getElementById("<%= txtUniqIDNo.ClientID %>").value.length >= 12)
                    return false;

            return true;
        }
        else {
             return true;
        }
    }


</script>

</asp:Content>
