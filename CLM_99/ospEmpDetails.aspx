<%@ Page Language="VB" AutoEventWireup="false" CodeFile="ospEmpDetails.aspx.vb" MasterPageFile="~/CLM_Master.master" Title="ONLINE SAFETY TRAINING" Inherits="ospEmpDetails" EnableViewState="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>




<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="Server">



    <style type="text/css">
        .modalBackground {
            background-color: Gray;
            filter: alpha(opacity=70);
            opacity: 0.7;
        }


        .ajax__tab_header {
            font-family: verdana,tahoma,helvetica !important;
            font-size: 10.26px !important;
            background: url(WebResource.axd?d=YSip4EyTBxTSTljg9qRqcllppe9gw-f8Z2rycFDHPC4jM0f_IjgZ8wSL-8gdZj1VWcLzcC9b7Fiou0W5uea9DlqtrALcipOWAHwH9gocm_55OBV2cYmuZXHCEhUeBBgpyaiDdlLAMWGa79x01OjbgvqVYGocjvBHgvvyIkvHB6U1&t=637674072980000000) repeat-x bottom !important;
        }










        .modalPopup {
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding: 3px;
            width: 250px;
        }

        .rej-enabled {
            background-color: Red !important;
        }

        .rej-disabled {
            background-color: rgba(192, 192, 192, 1) !important;
        }
    </style>

    <style type="text/css">
        .body {
            background-color: White;
        }

        .btnStyle {
            font-family: Arial;
            font-size: xx-small;
            color: white;
            background-color: #996AD3;
            border-color: White;
            height: 22px;
            font-size: small;
            border-left-width: 1px;
            border-right-width: 1px;
            border-top-width: 1px;
            border-bottom-width: 1px;
        }

        .hide {
            display: none;
        }



        .ddlStyle {
            font-family: Arial;
            font-size: xx-small;
            height: 25px;
            color: black;
            vertical-align: middle;
        }

        .lblStyle {
            font-family: times new Roman;
            font-size: small;
            color: #996AD3;
            font-weight: Bold;
        }

        .tableStyle {
            background-color: #996AD3; /*#996AD3*/
            border: solid 1px grey;
            color: InfoBackground;
            font-family: Times New Roman Baltic;
            text-align: left;
        }

        .tableData {
            border-top: solid 1px #000080;
            border-bottom: solid 1px #000080;
            border-left: solid 1px #000080;
            border-right: solid 1px #000080;
            background-color: #b6cfe6;
            font-size: smaller;
        }


        .TextBoxUpperCase {
            text-transform: uppercase;
            font-family: Arial;
            font-size: xx-small;
            color: black;
        }

        .TextBoxStyle {
            font-family: Arial;
            font-size: xx-small;
            height: 15px;
            color: black;
        }


        .tblErrorList {
            font-family: arial;
            font-size: xx-small;
            color: Red;
            background-color: #FFFFCC;
            border-top-color: Blue;
            border-bottom-color: Blue;
            border-left-color: Blue;
            border-right-color: Blue;
        }

        .mandatory {
            font-weight: bold;
            color: Red;
            vertical-align: middle;
        }

        .ModalTable {
            background-color: #F2F2F2;
            border: 5px solid whitesmoke;
            width: 97%;
        }


        .style15 {
            width: 859px;
        }


        .style16 {
            width: 4%;
        }

        ul {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }

        li {
            float: left;
        }

        a {
            display: block;
            color: black;
            text-decoration: none;
        }

            a:hover {
                color: White;
                background-color: #1B69FA;
            }



        .WatermarkCssClass {
            color: #006699;
            background-color: White;
            font-size: x-small;
        }


        .breadth {
            width: 180px;
            height: 20px;
            top: 0px;
            display: block;
            color: black;
            text-decoration: none;
        }


        .fontarea {
            font-size: smaller;
            font-family: Arial;
            text-align: center;
        }


        .borderLi {
            border: 1px dotted;
        }


        .cstyleText {
            width: 90%;
            font-family: Arial;
            border: 1px solid Silver;
        }


        .cstyleddl {
            width: 61%;
            font-family: Arial;
            font-size: x-small;
        }


        .csremark {
            width: 35%;
            font-family: Arial;
            font-size: x-small;
        }


        .csEmp {
            width: 10%;
            font-family: Arial;
            font-size: x-small;
        }


        .cslabel {
            width: 100%;
        }

        .cslabel1 {
            width: 100%;
            color: Black;
        }

        button:hover {
            background-color: #1B69FA;
            color: White;
        }



        /*      li:hover
    {
      background:#1B69FA;  
      color:White;
        }*/


        .Freezing {
            position: relative;
            table-layout: fixed;
            margin: 0 auto;
            top: expression(this.offsetParent.scrollTop);
            z-index: 5;
        }

        .DivHeaderFeeze {
            height: 140px;
            overflow: auto;
            position: relative;
            margin: 0 auto;
            top: 0px;
            left: 0px;
        }

        .style17 {
            width: 80px;
        }

        .style18 {
            width: 230px;
        }

        .csButton {
            background-color: #3333FF;
            color: white;
            font-family: Arial;
        }

        .cscaption {
            color: #1B69FA;
            font-style: italic;
            text-align: left;
            text-decoration: none;
            font-family: Bookman Old Style;
        }

        .csla {
            color: #09822D;
            font-family: Arial;
            font-size: xx-small;
        }

        .styleM {
            font-family: Arial;
            font-size: x-small;
            font-style: italic;
            border: 1px dotted blue;
        }


        .tooltip {
            display: inline;
            position: relative;
            text-decoration: none;
            top: 0px;
            left: 4px;
        }

            .tooltip:hover:after {
                background: #3333FF;
                background: rgba(0.0.0,.8);
                border-radius: 2px;
                color: #fff;
                content: attr(alt);
                left: 210px;
                padding: 5px 15 px;
                position: absolute;
                z-index: 98;
                width: 80px;
            }

            .tooltip:hover:before {
                border: solid;
                border-color: transparent Black;
                border-width: 4px;
                content: "";
                left: 203px;
                position: absolute;
                z-index: 99;
                top: 0px;
            }

        .CompletionListCssClass1 {
            font-family: arial;
            font-size: 0.6em;
            font-weight: normal;
            border: solid 1px #006699;
            line-height: 20px;
            padding: 10px;
            background-color: #ccffcc;
            margin-left: 10px;
            width: 200px !important;
            overflow: auto;
        }

        .CompletionListItemCssClass1 {
            border-bottom: dotted 1px #006699;
            cursor: pointer;
            color: black;
            width: 200px !important;
        }

        .CompletionListHighlightedItemCssClass1 {
            color: White;
            background-color: blue;
            cursor: pointer;
            width: 200px !important;
        }

        .auto-style1 {
            margin-top: 0px;
        }

        .SkNoteStyle {
            background-color: #E0FFFF; /*#996AD3*/
            border: solid 1px grey;
            color: black;
            font-family: Times New Roman Baltic;
            text-align: left;
        }

        .lblSkNoteStyle {
            font-family: Verdana;
            /*font-family: Times New Roman Baltic;*/
            text-align: left;
            font-size: x-small;
            color: #000000;
            background-color: #FFF5F5;
        }
        /*OSJ2756 Begins 1*/
        .CompletionListCssClass1 {
            font-family: arial;
            font-size: 8pt;
            font-weight: normal;
            border: solid 1px #216300;
            line-height: 20px;
            padding: 10px;
            background-color: #ccffcc;
            margin-left: 10px;
            width: 200px !important;
            height: 300px;
            overflow: auto;
        }

        .CompletionListItemCssClass1 {
            border-bottom: dotted 1px #006699;
            cursor: pointer;
            color: black;
            width: 200px !important;
        }

        .CompletionListHighlightedItemCssClass1 {
            color: White;
            background-color: blue;
            cursor: pointer;
            width: 200px !important;
        }
        /*OSJ2756 Ends 1*/
        /*added code for blinking feature in reject button.*/
        .btnGlowingStyle {
            font-family: Arial;
            font-size: xx-small;
            color: white;
            background-color: #996AD3;
            border-color: White;
            height: 22px;
            font-size: small;
            border-left-width: 1px;
            border-right-width: 1px;
            border-top-width: 1px;
            border-bottom-width: 1px;
            -webkit-animation: glowing 1300ms infinite;
            -moz-animation: glowing 1300ms infinite;
            -o-animation: glowing 1300ms infinite;
            animation: glowing 1300ms infinite;
        }



        @-webkit-keyframes glowing {

            0% {
                background-color: #996AD3;
                -webkit-box-shadow: 0 0 3px #996AD3;
            }

            50% {
                background-color: red;
                -webkit-box-shadow: 0 0 15px red;
            }

            100% {
                background-color: #996AD3;
                -webkit-box-shadow: 0 0 3px #996AD3;
            }
        }

        @keyframes glowing {

            0% {
                background-color: #996AD3;
                box-shadow: 0 0 3px #996AD3;
            }

            50% {
                background-color: red;
                box-shadow: 0 0 15px red;
            }

            100% {
                background-color: #996AD3;
                box-shadow: 0 0 3px #996AD3;
            }
        }


        .textblinker {
            font-family: Arial;
            font-size: xx-small;
            color: white;
            background-color: yellow;
            border-color: White;
            height: 22px;
            font-size: small;
            border-left-width: 1px;
            border-right-width: 1px;
            border-top-width: 1px;
            border-bottom-width: 1px;
            -webkit-animation: textglowing 1300ms infinite;
            -moz-animation: textglowing 1300ms infinite;
            -o-animation: textglowing 1300ms infinite;
            animation: textglowing 1300ms infinite;
        }



        @-webkit-keyframes textglowing {

            0% {
                background-color: white;
                -webkit-box-shadow: 0 0 3px white;
            }

            50% {
                background-color: yellow;
                -webkit-box-shadow: 0 0 15px yellow;
            }

            100% {
                background-color: white;
                -webkit-box-shadow: 0 0 3px white;
            }
        }

        @keyframes textglowing {

            0% {
                background-color: white;
                box-shadow: 0 0 3px white;
            }

            50% {
                background-color: yellow;
                box-shadow: 0 0 15px yellow;
            }

            100% {
                background-color: white;
                box-shadow: 0 0 3px white;
            }
        }

        .textfontstyle {
            color: red;
            text-align: center;
            font-family: Tahoma;
            font-size: medium;
            font-weight: bolder;
        }

        .checkbox input[type="checkbox"] {
            margin-left: 0px;
        }

        .wrapper:after{
  content:" ";
  width: 40px;
  height: 2px;
  margin: 0 10px;
  vertical-align: super;
  background-color:blue;
  display:inline-block;
}
                .wwrapper:after{
  content:" ";
  width: 35px;
  height: 2px;
  margin: 0 10px;
  vertical-align: super;
  background-color:blue;
  display:inline-block;
}
.mbtn
         {
           /*background-image:url('Images/gridHeader1.jpg');*/
           background-repeat:repeat-x;
           cursor:pointer;
           border:none 1px #000000;
            height: 24px;
        }

.modal {
            display: none; /* Hidden by default */
            position: fixed; /* Stay in place */
            z-index: 1; /* Sit on top */
            left: 0;
            top: 0;
            width: 100%; /* Full width */
            height: 100%; /* Full height */
            overflow: auto; /* Enable scroll if needed */
            background-color: rgb(0,0,0); /* Fallback color */
            background-color: rgba(0,0,0,0.4); /* Black w/ opacity */
        }

        /* Modal Content */
        .modal-content {
            background-color: #fefefe;
            margin: 15% auto; /* 15% from the top and centered */
            padding: 20px;
            border: 1px solid #888;
            width: 80%; /* Could be more or less, depending on screen size */
        }

        /* Close Button */
        .close {
            color: #aaa;
            float: right;
            font-size: 28px;
            font-weight: bold;
        }

        .close:hover,
        .close:focus {
            color: black;
            text-decoration: none;
            cursor: pointer;
        }


        .flowchart {
    display: flex;
    align-items: center;
    justify-content: center;
}

.step {
    display: flex;
    align-items: center;
}

.kbtn {
    border-radius: 24px;
    margin: 0 5px;
    padding: 10px;
    cursor: pointer;
}

.arrow {
    width: 20px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}

.arrow:after {
    content: '';
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.barrow {
    width: 335px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}

.barrow:after {
    content: '';
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.carrow {
    width: 14px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}
.carrow:after {
    content: '';
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.darrow {
    width: 14px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}
.darrow:after {
    /*content: '';*/
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.earrow {
    width: 335px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}

.earrow:after {
    /*content: '';*/
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.farrow {
    width: 15px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}

.farrow:after {
    content: '';
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.garrow {
    width: 375px;
    height: 4px;
    background-color: mediumslateblue;
    position: relative;
}

.garrow:after {
    content: '';
    position: absolute;
    right: -3px;
    top: -4px;
    width: 0;
    height: 3px;
    border-left: 5px solid mediumslateblue;
    border-top: 5px solid transparent;
    border-bottom: 5px solid transparent;
}

.vertical-line {
    width: 5px;
    height: 134px;
    background-color: mediumslateblue;
    margin: 0px 66px;
    margin-top: 129px;
    vertical-align: super;
    display: inline-block;
    position: absolute;
}

.vertical-line:after {
  /*content: '';*/
  position: absolute;
  bottom: -3px; 
  left: -3px;
  width: 0;
  height: 0;
  border-left: 5px solid transparent;
  border-right: 5px solid transparent;
  border-top: 5px solid mediumslateblue;
}

.avertical-line {
    width: 4px;
    height: 88px;
    background-color: mediumslateblue;
    margin: 0px 77px;
    margin-top: 92px;
    vertical-align: super;
    display: inline-block;
    position: absolute;
}

.avertical-line:after {
  /*content: '';*/
  position: absolute;
  bottom: -3px; 
  left: -3px;
  width: 0;
  height: 0;
  border-left: 5px solid transparent;
  border-right: 5px solid transparent;
  border-top: 5px solid mediumslateblue;
}

.bvertical-line {
    width: 4px;
    height: 80px;
    background-color: mediumslateblue;
    margin: 0px 84px;
    margin-top: 92px;
    vertical-align: super;
    display: inline-block;
    position: absolute;
}

.bvertical-line:after {
    content: '';
    position: absolute;    
    top: -3px;
    left: -4px;    
    width: 0;
    height: 0;
    border-left: 6px solid transparent;
    border-right: 6px solid transparent;
    border-bottom: 7px solid mediumslateblue;
}

.cvertical-line {
    width: 5px; 
    height: 123px;
    background-color: mediumslateblue;
    margin: 0px 81px;
    margin-top: 138px;
    vertical-align: super;
    display: inline-block;
    position: absolute;
}

.cvertical-line:after {
    content: '';
    position: absolute;
    bottom: 123px;
    left: -3px;
    width: 2px;
    height: 0;
    border-left: 5px solid transparent;
    border-right: 5px solid transparent;
    border-bottom: 5px solid mediumslateblue;
}

.dvertical-line {
    width: 5px;
    height: 133px;
    background-color: mediumslateblue;
    margin: 0px 66px;
    margin-top: 130px;
    vertical-align: super;
    display: inline-block;
    position: absolute;
}

.dvertical-line:after {
  /*content: '';*/
  position: absolute;
  bottom: -3px; 
  left: -3px;
  width: 0;
  height: 0;
  border-left: 5px solid transparent;
  border-right: 5px solid transparent;
  border-top: 5px solid mediumslateblue;
}

.evertical-line {
    width: 5px; 
    height: 124px;
    background-color: mediumslateblue;
    margin: 0px 81px;
    margin-top: 145px;
    vertical-align: super;
    display: inline-block;
    position: absolute;
}

.evertical-line:after {
    content: '';
    position: absolute;
    bottom: 123px;
    left: -3px;
    width: 2px;
    height: 0;
    border-left: 5px solid transparent;
    border-right: 5px solid transparent;
    border-bottom: 5px solid mediumslateblue;
}

.multi-line-button {
    height: 55px; /* Adjust height as needed */
    width: 60px; /* Adjust width as needed */
    white-space: normal;
    text-align: center;
  }

    </style>

    <%--OSJ2756 Begins 2--%>
    <script language="text/javascript">
        function OnCertSelected(sender, e) {
            var certValue = e._value
            var vLen = certValue.length;
            var obj = document.getElementById("ctl00_ContentPlaceHolder1_ddlSkillTrade");
            obj.value = certValue;
        }
    </script>
    <%--OSJ2756 Ends 2--%>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <head>
        <title>Profile Entry Screen</title>
        <link href="Admin.css" rel="stylesheet" type="text/css" />
    </head>

    <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


    <script type="text/javascript">

        function funcminddraft() {
            alert("DEPTT CHIEF/GM/EIC/HOD APPROVAL COPY SHOULD BE MANDATORILY ATTACHED IN CLM AS PER STANDARD FORM DURING THIS PROCESS");
        }


        function SetActiveTab(tn) {
            var vActiveTab = $find('<%= tabcontainer1.ClientID %>').get_activeTabIndex();

            var container = $find('<%= tabcontainer1.ClientID %>')

            if (tn == 1 && vActiveTab < 9) {

                container.set_activeTabIndex(vActiveTab + tn);
            }
            if (tn == -1 && vActiveTab > 0) {

                container.set_activeTabIndex(vActiveTab + tn);
            }

        }

        function SHOW_VALUE(ctrl) {

            document.getElementById("hidCtrlName").value = ctrl.name;

            document.getElementById("div1").style.display = 'inline';
            document.getElementById("mymsg").value = ctrl.value;
        }

        function Close_Div() {
            document.getElementById("div1").style.display = 'none';
        }
        function Update_Close_Div() {
            var ctrl = document.getElementById("hidCtrlName").value;
            document.getElementById(ctrl).value = document.getElementById("mymsg").value
            document.getElementById("div1").style.display = 'none';
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }




        function checkEmail(oEmail) {

            var objEmail = document.getElementById(oEmail);
            var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

            if (!filter.test(objEmail.value)) {
                alert('Please provide a valid email address');
                email.focus;
                return false;
            }
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
        function CheckSingleCheckbox(ob) {
            var grid = ob.parentNode.parentNode.parentNode;


            var inputs = grid.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox") {
                    if (ob.checked && inputs[i] != ob && inputs[i].checked) {
                        inputs[i].checked = false;
                    }
                }
            }
        }
        function OnSPSelected(sender, e) {

            var vSPNoName = e._value
            var vSPNo = vSPNoName.substring(0, 12)
            var obj = document.getElementById("ctl00_ContentPlaceHolder1_txtSPNo");
            obj.value = vSPNo;
        }

        //function toggleModal() {
        //    var modal = document.getElementById('myModal');
        //    if (modal.style.display === "none" || modal.style.display === "") {
        //        modal.style.display = "block";
        //    } else {
        //        modal.style.display = "none";
        //    }
        //}

        //function toggleModalAG() {
        //    var modal = document.getElementById('myModalAG');
        //    if (modal.style.display === "none" || modal.style.display === "") {
        //        modal.style.display = "block";
        //    } else {
        //        modal.style.display = "none";
        //    }
        //}


    </script>





    <center>





        <asp:UpdatePanel ID="UpdatePanel2" runat="server">



            <ContentTemplate>
                <div style="left: 0px; top: 15px; width: 1280px; max-height: 1360px; background-color: Window; height: 1360px; border: 1px solid blue">

                    <table width="100%" style="height: 36px">
                        <tr style="border-bottom-style: solid; border-bottom-width: thin; border-bottom-color: #0066FF;">
                            <td class="style16"></td>

                            <td align="center"
                                style="font-size: large; color: Blue; font-family: Times New Roman;"
                                class="style15">ONLINE SAFETY  TRAINING EMPLOYEE DETAILS</td>
                            <td></td>
                        </tr>
                    </table>
                    <%--WI7242  Enhancement in design to add note in profile entry page to help vendor partner to take necessary action--%>
                    <table style="border-collapse: separate; border-spacing: 0 1em; margin-left: 8px">
                        <tr>
                            <td align="left">
                                <asp:Label ID="Label27" runat="server" Text="Note 1: For removal of aadhar from safety pass - please contact with safety agency of particular Location" ForeColor="Red" BackColor="Yellow" Font-Bold="True" Font-Underline="True" Font-Size="Small" Font-Names="Tahoma" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Label ID="Label28" runat="server" Text="Note 2: For Relocation Conversion of Safety Pass – Please apply in CLM beta version – Gate Pass – Safety Pass Relocation Request" ForeColor="Red" BackColor="Yellow" Font-Bold="True" Font-Underline="True" Font-Size="small" Font-Names="Tahoma" />
                            </td>
                        </tr>
                        <tr>
                            <td align="left">
                                <asp:Label ID="Label34" runat="server" Text="Note 3: Direct Assessment to be booked through CLM New >> Workmen Profile >> Slot booking New GP-Direct Assessment And Training cum assessment to be booked through  CLM New >>Workmen Profile >>  Fee payment for TCA & Renewal Cases" ForeColor="Red" BackColor="Yellow" Font-Bold="True" Font-Underline="True" Font-Size="small" Font-Names="Tahoma" />
                            </td>
                        </tr>
                        <tr>
                         <td align="left">
                          <asp:Label ID="Label35" runat="server" ForeColor="Black"
                          Font-Bold="True" Font-Underline="True" Font-Size="small" Font-Names="Tahoma"
                          Text="Note 4: Please book slots for skill and medical assessment now (<span style='color:blue;'>slot booking to be completed before biometric</span>).<span style='color:red;'>Slots for medical and skill must not be booked in same half of the day</span>-Could be done on separate days / separate halves of the same day. (Pls note -Skill assessment takes ~5-6 hours and medical assessment takes ~ 1 hour.)" />
                         </td>
                        </tr>
                        <tr>
                            <td align="center">
                                <asp:Label ID="Rejmsg" CssClass="textblinker textfontstyle" runat="server" Text="Click on Reject button to reject the safety pass request" Font-Underline="True" />
                            </td>
                        </tr>
                    </table>
                    <%--<h3 runat="server" id="showFloawchart" style="font-size:large; color:#0000CC; ">Visualization of Timeline View for GP Issuance</h3>--%>   
                    
                    <div runat="server" id="divAllFlowChart"  visible="false">
                    
                       <div runat="server" id="divFlowChart" visible="false" style="margin-left:10px">   
                           <asp:LinkButton id="lnkbShowFlowchart" Text="Visualization of Timeline View for GP Issuance" Font-Names="Verdana" Font-Size="14pt" Style="display: contents; color:#0000CC;" OnClick="lnkbShowFlowchart_click" runat="server"/>
                           <%--<asp:Button ID="btnShowPopup" CssClass="btn btn-small btn-primary" runat="server" Text="Show Guide" OnClientClick="toggleModal(); return false;" />--%>
                           <br /><br />
                           <div runat="server" id="divFlowChartDtls" style="margin-left:10px;height:220px;">  
<div class="row">                          
<div class="flowchart">
    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn01" Text="BU approval for Safety-Pass" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="BU approval for Safety-Pass" />
        <div class="arrow"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" ID="btn02" Text="Profile Creation" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Profile Creation" />        
        <div class="vertical-line"></div>
        <div class="avertical-line"></div>
        <div class="arrow"></div>        
    </div> 
    
    <div class="step">
        <asp:Button runat="server" ID="btn03" Text="Medical Examination" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Medical Examination" />
        <div class="arrow"></div>
        <div class="bvertical-line"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" ID="btn05" Text="Slot Booking for Skill Assessment" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Slot Booking for Skill Assessment" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn06" Text="Biometric Data Collection" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Biometric Data Collection" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn07" Text="Skill Assessment" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Skill Assessment" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn08" Text="Assignment of slot for Safety Training" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Assignment of slot for Safety Training" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn09" Text="Safety Training" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Training" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn10" Text="Safety Pass Activation" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Pass Activation" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn11" Text="Apply for Gate Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Apply for Gate Pass" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn12" Text="BU Approval for Gate-Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="BU Approval for Gate-Pass" />
        <div class="arrow"></div>
        <div class="cvertical-line"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn13" Text="Contractor Cell Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Contractor Cell Approval" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn14" Text="Security Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Security Approval" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn15" Text="Gate-Pass Issued" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Gate-Pass Issued" />
        
    </div>
</div>
</div>
<br /><br />                               
<div class="row">
<div class="flowchart" style="margin-left: 180px;position: absolute;">
    <div class="step">
    <div class="carrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn04" Text="Document Verification" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Document Verification" />
    <div class="darrow"></div>
    </div>
</div>
</div>
<div class="row">
<div class="flowchart" style="margin-left: 166px;position: absolute;margin-top: 40px;">
    <div class="step">    
    <div class="barrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn16" Text="Upload Police Verification Certificate" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload Police Verification Certificate" />
    <div class="arrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn17" Text="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" />
    <div class="earrow"></div>
    </div>
    <br /><br />
    <br /><br />
</div>
</div>       
</div>
    </div>

                     <div runat="server" id="divFlowChart1" visible="false" style="margin-left:10px">   
                           <asp:LinkButton id="lnkbShowFlowchart1" Text="Visualization of Timeline View for GP Issuance" Font-Names="Verdana" Font-Size="14pt" Style="display: contents; color:#0000CC;" OnClick="lnkbShowFlowchart1_click" runat="server"/>
                           <%--<asp:Button ID="btnShowPopup1" CssClass="btn btn-small btn-primary" runat="server" Text="Show Guide" OnClientClick="toggleModalAG(); return false;" />--%>
                           <br /><br />
                           <div runat="server" id="divFlowChartDtls1" style="margin-left:10px;height:220px;">
                               
<div class="row">                          
<div class="flowchart">
    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn101" Text="BU approval for Safety-Pass" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="BU approval for Safety-Pass" />        
        <div class="arrow"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" ID="btn102" Text="Profile Creation" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Profile Creation" />        
        <div class="vertical-line"></div>        
        <div class="arrow"></div>        
    </div> 
    
    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn103" Text="Biometric Data Collection" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Biometric Data Collection" />        
        <div class="arrow"></div>
        <div class="avertical-line"></div>        
    </div>

    <div class="step">
        <asp:Button runat="server" ID="btn104" Text="Medical Examination" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Medical Examination" />        
        <div class="arrow"></div>
        <div class="bvertical-line"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" ID="btn106" Text="Slot Booking for Skill Assessment" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Slot Booking for Skill Assessment" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn107" Text="Skill Assessment" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Skill Assessment" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn108" Text="Assignment of slot for Safety Training" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Assignment of slot for Safety Training" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn109" Text="Safety Training" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Training" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn110" Text="Safety Pass Activation" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Pass Activation" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn111" Text="Apply for Gate Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Apply for Gate Pass" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn112" Text="BU Approval for Gate-Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="BU Approval for Gate-Pass" />
        <div class="arrow"></div>
        <div class="cvertical-line"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn113" Text="Contractor Cell Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Contractor Cell Approval" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn114" Text="Security Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Security Approval" />
        <div class="arrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn115" Text="Gate-Pass Issued" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Gate-Pass Issued" />
        
    </div>
</div>
</div>
<br /><br />                               
<div class="row">
<div class="flowchart" style="margin-left: 270px;position: absolute;">
    <div class="step">
    <div class="carrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn105" Text="Document Verification" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Document Verification" />
    <div class="darrow"></div>
    </div>
</div>
</div>
<div class="row">
<div class="flowchart" style="margin-left: 166px;position: absolute;margin-top: 40px;">
    <div class="step">    
    <div class="barrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn116" Text="Upload Police Verification Certificate" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload Police Verification Certificate" />
    <div class="arrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn117" Text="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" />
    <div class="earrow"></div>
    </div>
    <br /><br />
    <br /><br />
</div>
</div>      
</div>
    </div>

                    <div runat="server" id="divFlowChart2" visible="false" style="margin-left:10px">   
                           <asp:LinkButton id="lnkbShowFlowchart2" Text="Visualization of Timeline View for GP Issuance" Font-Names="Verdana" Font-Size="14pt" Style="display: contents; color:#0000CC;" OnClick="lnkbShowFlowchart2_click" runat="server"/>                           
                           <br /><br />
                           <div runat="server" id="divFlowChartDtls2" style="margin-left:10px;height:220px;">    


                               <div class="row">                          
<div class="flowchart">
    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn201" Text="BU approval for Safety-Pass" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="BU approval for Safety-Pass" />
        <div class="farrow"></div>
    </div>

    <div class="step">           
        <asp:Button runat="server" ID="btn202" Text="Profile Creation" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Profile Creation" />        
        <div class="dvertical-line"></div>        
        <div class="farrow"></div>        
    </div> 
    
    <div class="step">        
        <asp:Button runat="server" ID="btn203" Text="Medical Examination" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Medical Examination" />
        <div class="farrow"></div>        
    </div>

    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn204" Text="Skill slot booking by PD & assessment" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Skill slot booking by PD & assessment" />
        <div class="farrow"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn205" Text="Profile Completion" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Profile Completion" />
       
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn206" Text="Document Verification" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Document Verification" />                 
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn207" Text="Biometric Data Collection" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Biometric Data Collection" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn208" Text="Safety slot booking by vendor" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety slot booking by vendor" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn209" Text="Safety Training" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Training" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn210" Text="Safety Pass Activation" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Pass Activation" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn211" Text="Apply for Gate Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Apply for Gate Pass" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn212" Text="BU Approval for Gate-Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="BU Approval for Gate-Pass" />
        <div class="farrow"></div>
        <div class="evertical-line"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn213" Text="Contractor Cell Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Contractor Cell Approval" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn214" Text="Security Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Security Approval" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn215" Text="Gate-Pass Issued" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Gate-Pass Issued" />
        
    </div>
</div>
</div>
<br /><br />                               
<div class="row">
<div class="flowchart" style="margin-left: 151px;position: absolute;margin-top: 40px;">
    <div class="step">    
    <div class="garrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn216" Text="Upload Police Verification Certificate" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload Police Verification Certificate" />
    <div class="arrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn217" Text="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" />
    <div class="earrow"></div>
    </div>
    <br /><br />
    <br /><br />
</div>
</div>           
</div>
    </div>

                    <div runat="server" id="divFlowChart3" visible="false" style="margin-left:10px;height:220px;">   
                           <asp:LinkButton id="lnkbShowFlowchart3" Text="Visualization of Timeline View for GP Issuance" Font-Names="Verdana" Font-Size="14pt" Style="display: contents; color:#0000CC;" OnClick="lnkbShowFlowchart3_click" runat="server"/>                           
                           <br /><br />
                           <div runat="server" id="divFlowChartDtls3" style="margin-left:10px">   
                               
                               <div class="row">                          
<div class="flowchart">
    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn301" Text="BU approval for Safety-Pass" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="BU approval for Safety-Pass" />
        <div class="farrow"></div>
    </div>

    <div class="step">           
        <asp:Button runat="server" ID="btn302" Text="Profile Creation" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Profile Creation" />        
        <div class="dvertical-line"></div>        
        <div class="farrow"></div>        
    </div> 
    
    <div class="step">       
        <asp:Button runat="server" Enabled="false" ID="btn303" Text="Biometric Data Collection" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Biometric Data Collection" />        
        <div class="farrow"></div>        
    </div>

    <div class="step">     
        <asp:Button runat="server" ID="btn304" Text="Medical Examination" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Medical Examination" />        
        <div class="farrow"></div>
    </div>

    <div class="step">       
        <asp:Button runat="server" Enabled="false" ID="btn305" Text="Skill slot booking by PD & assessment" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Skill slot booking by PD & assessment" />        
       
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn306" Text="Profile Completion" Style="border-radius: 24px!important;font-size:7px;font-weight: bolder;" CssClass="kbtn multi-line-button" ToolTip="Profile Completion" />        
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn307" Text="Document Verification" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Document Verification" />                 
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn308" Text="Safety slot booking by vendor" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety slot booking by vendor" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn309" Text="Safety Training" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Training" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn310" Text="Safety Pass Activation" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Safety Pass Activation" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn311" Text="Apply for Gate Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Apply for Gate Pass" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn312" Text="BU Approval for Gate-Pass" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="BU Approval for Gate-Pass" />
        <div class="farrow"></div>
        <div class="evertical-line"></div>
    </div>

    <div class="step">        
        <asp:Button runat="server" Enabled="false" ID="btn313" Text="Contractor Cell Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Contractor Cell Approval" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn314" Text="Security Approval" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Security Approval" />
        <div class="farrow"></div>
    </div>

    <div class="step">
        <asp:Button runat="server" Enabled="false" ID="btn315" Text="Gate-Pass Issued" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Gate-Pass Issued" />
        
    </div>
</div>
</div>
<br /><br />                               
<div class="row">
<div class="flowchart" style="margin-left: 151px;position: absolute;margin-top: 40px;">
    <div class="step">    
    <div class="garrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn316" Text="Upload Police Verification Certificate" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload Police Verification Certificate" />
    <div class="arrow"></div>
    <asp:Button runat="server" Enabled="false" ID="btn317" Text="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" Style="border-radius: 24px!important;font-size:7px;" CssClass="kbtn multi-line-button" ToolTip="Upload statutory documents (ESIC ,PF, PMJJY,PMSB )" />
    <div class="earrow"></div>
    </div>
    <br /><br />
    <br /><br />
</div>
</div>   
        
</div>
    </div>
<br />                             
<div class="row">
<img src="Images/Wireframe_legend.PNG" alt="Image" style="width:22%;margin-right: 930px;">
<br />
</div> 
                        </div>                    
                    <%--WI7242  End Prasun Chakraborty created on 03-FEB-2022--%>
                    <asp:UpdateProgress ID="uprgShowbusy" runat="server" DynamicLayout="true" Visible="true">
                        <ProgressTemplate>
                            <div class="divStyle">
                                <table class="divTblStyle" width="200" style="border: solid  1px black">
                                    <tr>
                                        <td align="justify">
                                            <img alt="Data is being saved" src="images/wait.gif" />
                                            Please Wait...
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </ProgressTemplate>
                    </asp:UpdateProgress>

                    <br />

                    <div id="navbar" class="fontarea" style="margin-left: 40px; margin-right: 15px;">
                        <ul>
                            <li class="borderLi">
                                <asp:LinkButton ID="profile_entry" CssClass="breadth" runat="server" Text="APPROVED REQUEST NUMBER" OnClick="status_histry_click"></asp:LinkButton></li>
                            <li class="borderLi">
                                <asp:LinkButton ID="lnkNoti" CssClass="breadth" runat="server" Text="NOTFICATION FOR RETURN DOCUMENT"></asp:LinkButton></li>

                        </ul>
                    </div>

                    <br />

                    <div style="margin-left: 5px; width: 995px">

                        <asp:Panel ID="pnlMain" runat="server" Visible="true" Width="99%" Height="706px">
                            <table id="tblProfileErrorList" runat="server" width="100%" class="tblErrorList"
                                style="background-position: center">
                            </table>

                            <div id="pnlShw" runat="server" visible="true" width="100%" style="height: 300px; overflow: auto;">

                                <asp:GridView ID="gvReq" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    Width="98%" ForeColor="black" BackColor="Plum">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SL.NO">
                                            <ItemTemplate>
                                                <%#Container.DataItemIndex + 1 %>
                                            </ItemTemplate>

                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Dated">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_date" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SRQ_CREATED_DT") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Request Number">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_Request_No" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SRQ_REQ_NO") %>' OnClick="lnk_Request_No_Click"> <%--  --%></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Request Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_RQ" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SRQ_REQ_TYPE") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Work Order">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_WO" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SRQ_WORK_ORDER") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_Status" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "STATUS") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Approved Employee">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_emp" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "SRD_EMP_APV_COUNT") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                    <HeaderStyle BackColor="#3333FF" ForeColor="white" Font-Names="Arial" Font-Size="XX-Small" />
                                    <RowStyle BackColor="#ccffcc" Font-Names="Arial" Font-Size="XX-Small" />
                                    <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                </asp:GridView>

                                <asp:GridView ID="grd_noti" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    Width="97%" CssClass="auto-style1" Height="149px" ForeColor="black" HeaderStyle-BackColor="#0099ff">
                                    <Columns>
                                        <asp:BoundField DataField="CET_REQUEST_NO" HeaderText="Request Number" ItemStyle-Font-Size="Smaller" />
                                        <asp:BoundField DataField="CET_SAFETY_PASSNO" HeaderText="Safetypass No" ItemStyle-Font-Size="Smaller" />
                                        <asp:BoundField DataField="verifytype" HeaderText="Document Type" ItemStyle-Font-Size="Smaller" />
                                        <asp:BoundField DataField="SDV_REMARKS" HeaderText="Approver Remarks" ItemStyle-Font-Size="Smaller" />
                                    </Columns>
                                </asp:GridView>
                                <asp:Button ID="btn_downloadnoti" runat="server" CssClass="btnStyle" Text="Download" Visible="false" />
                            </div>
                            <asp:Label ID="lblpagemsg" Text="" runat="server" CssClass="cslabel" ForeColor="Red" Font-Size="9px"></asp:Label>
                            <asp:Panel ID="Pnlcategory" runat="server" Visible="false" Width="97%">

                                <table width="98%" border="0">
                                    <tr>
                                        <td style="color: black" width="20%" class="styleM">
                                            <asp:LinkButton ID="lblreq" runat="server" Text="REQUEST NUMBER :" ForeColor="blue" ToolTip="Download the Checklist of the Employee" onMouseOver="this.style.color='#0F0'" onMouseOut="this.style.color='#00F'"></asp:LinkButton>

                                        </td>


                                        <td width="15%" class="styleM">
                                            <asp:LinkButton ID="lnkSup" runat="server" OnClick="lnkSup_Click" Text="SUPERVISOR : " ToolTip="click to enter details of supervisor" Style="color: Blue" onMouseOver="this.style.color='#0F0'" onMouseOut="this.style.color='#00F'"></asp:LinkButton>
                                        </td>

                                        <td class="styleM" width="12%">
                                            <asp:LinkButton ID="lnkWrk" runat="server" OnClick="lnkWrk_Click" Text="WORKER : " ToolTip="click to enter details of worker" Style="color: Blue" onMouseOver="this.style.color='#0F0'" onMouseOut="this.style.color='#00F'"></asp:LinkButton>
                                        </td>

                                        <td class="styleM">
                                            <asp:LinkButton ID="LnkDR" runat="server" OnClick="LnkDR_Click" Text="DRIVER : " ToolTip="click to enter details of Driver" Style="color: Blue" onMouseOver="this.style.color='#0F0'" onMouseOut="this.style.color='#00F'"></asp:LinkButton>
                                        </td>

                                        <td class="styleM">
                                            <asp:LinkButton ID="LnkFM" runat="server" OnClick="LnkFM_Click" Text="FACILITY MANAGER: " ToolTip="click to enter details of Manager" Style="color: Blue" onMouseOver="this.style.color='#0F0'" onMouseOut="this.style.color='#00F'"></asp:LinkButton>
                                        </td>
                                        <td class="styleM" id="TDvc" runat="server">
                                            <asp:LinkButton ID="LnkVC" runat="server" OnClick="LnkVC_Click" Text="VIDEO CAPSULE: " ToolTip="click to enter details of video Capsule" Style="color: Blue" onMouseOver="this.style.color='#0F0'" onMouseOut="this.style.color='#00F'"></asp:LinkButton>
                                        </td>
                                    </tr>

                                </table>
                            </asp:Panel>

                            <div id="PnlSafetyRenewal" runat="server" style="display: none">
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Label ID="Lbl" runat="server" Text="Enter Safety Number" Font-Size="XX-Small" Font-Bold="true"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtRenewSpno" runat="server" ForeColor="blue" Font-Size="XX-Small" AutoPostBack="true" Style="text-transform: uppercase"></asp:TextBox>
                                            <div style="font-size: 0.6em; float: left; width: 80%; color: Blue">
                                                <ajaxToolkit:AutoCompleteExtender ID="Txt_add_AutoCompleteExtender" runat="server"
                                                    DelimiterCharacters="" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetsafetyNumber"
                                                    ServicePath="" TargetControlID="txtRenewSpno" CompletionInterval="100" EnableCaching="true"
                                                    OnClientShowing="clientShowing" CompletionListCssClass="CompletionListCssClass1"
                                                    CompletionListHighlightedItemCssClass="CompletionListHighlightedItemCssClass1"
                                                    CompletionListItemCssClass="CompletionListItemCssClass1">
                                                </ajaxToolkit:AutoCompleteExtender>
                                            </div>
                                            <script type="text/javascript">
                                                function clientShowing(source, args) {
                                                    source._popupBehavior._element.style.zIndex = 100000;
                                                }
                                            </script>

                                        </td>

                                        <td>
                                            <asp:Button ID="btnAdd" runat="server" ForeColor="blue" Font-Size="XX-Small" Text="ADD"></asp:Button>
                                        </td>

                                    </tr>
                                    <!-- WI6447 START ADDED BY PRASUN ON 07012022-->
                                    <tr>
                                        <asp:Label ID="lblAddValidation" runat="server" Text="" ForeColor="Red" BackColor="Wheat" Font-Italic="true" Font-Bold="true" Font-Underline="true" Font-Size="Small" />
                                    </tr>
                                    <!-- WI6447 END ADDED BY PRASUN ON 07012022-->
                                </table>
                            </div>

                            <ajaxToolkit:TabContainer ID="tabcontainer1" runat="server" Width="100%"
                                Font-Bold="True" ActiveTabIndex="3" BackColor="white" align="left"
                                Visible="true" Style="display: none;">
                                <ajaxToolkit:TabPanel ID="tabPersonalInfo" runat="server" HeaderText="Personal Info" TabIndex="0">
                                    <HeaderTemplate>Personal Info</HeaderTemplate>
                                    <ContentTemplate>
                                        <div id="divPersonalInfo">
                                            <br />
                                            <table width="100%" border="0">
                                                <tr class="tableStyle">
                                                    <td class="style2">WORKMEN DETAILS </td>
                                                </tr>
                                            </table>
                                            <div id="div4">
                                                <br />
                                                <table border="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblCategory" runat="server" CssClass="lblStyle" Text="Category"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbCategory" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                        <td>
                                                            <asp:Label ID="lblDept" runat="server" CssClass="lblStyle" Text="Department"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="Txtdeprt" runat="server" CssClass="TextBoxUpperCase"
                                                                ReadOnly="True" onblur="checkMandatory(this);"
                                                                Width="175px" ondblclick="SHOW_VALUE(this);"></asp:TextBox></td>
                                                        <td colspan="1">
                                                            <asp:Label ID="Lblspno" runat="server" CssClass="lblStyle" Font-Bold="True"
                                                                Text="Safety Pass Number" Visible="False" ForeColor="Green"></asp:Label></td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="TxtSpno" runat="server" CssClass="TextBoxUpperCase"
                                                                Width="175px" MaxLength="40" Visible="False" ForeColor="Black"
                                                                ReadOnly="True"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblFName" runat="server" CssClass="lblStyle" Text="First Name"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="txtFName" runat="server" CssClass="TextBoxUpperCase" onblur="checkMandatory(this);"
                                                                Width="175px" MaxLength="40" ondblclick="SHOW_VALUE(this);"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblLName" runat="server" CssClass="lblStyle" Text="Last Name" MaxLength="40"></asp:Label></td>
                                                        <td>
                                                            <asp:TextBox ID="txtLName" runat="server" CssClass="TextBoxUpperCase" ondblclick="SHOW_VALUE(this);"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblDOB" runat="server" CssClass="lblStyle" Text="Date Of Birth (DD/MM/YYYY)"
                                                                Width="80px"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="txtDOB" runat="server" CssClass="TextBoxStyle" onblur="checkMandatory(this);" AutoPostBack="true"></asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender4" runat="server" PopupButtonID="txtDOB"
                                                                TargetControlID="txtDOB" Format="dd/MM/yyyy" Enabled="True">
                                                            </ajaxToolkit:CalendarExtender>
                                                            <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender11" runat="server" TargetControlID="txtDOB"
                                                                Mask="99/99/9999" ClearMaskOnLostFocus="False" CultureAMPMPlaceholder=""
                                                                CultureCurrencySymbolPlaceholder="" CultureDateFormat=""
                                                                CultureDatePlaceholder="" CultureDecimalPlaceholder=""
                                                                CultureThousandsPlaceholder="" CultureTimePlaceholder="" Enabled="True">
                                                            </ajaxToolkit:MaskedEditExtender>
                                                        </td>
                                                        <td align="center" rowspan="5">
                                                            <br />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblSex" runat="server" CssClass="lblStyle" Text="Gender"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbSex" runat="server" CssClass="ddlStyle" Width="175px">
                                                                <asp:ListItem Text="[Select]" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Male" Value="M"></asp:ListItem>
                                                                <asp:ListItem Text="Female" Value="F"></asp:ListItem>
                                                                <asp:ListItem Text="Transgender" Value="T"></asp:ListItem>
                                                            </asp:DropDownList></td>
                                                        <td>
                                                            <asp:Label ID="lblFatherName" runat="server" CssClass="lblStyle" Text="Father Name"></asp:Label></td>
                                                        <td>
                                                            <asp:TextBox ID="txtFatherName" runat="server" CssClass="TextBoxUpperCase" MaxLength="80"
                                                                ondblclick="SHOW_VALUE(this);"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblHusName" runat="server" CssClass="lblStyle" Text="Spouse Name"></asp:Label></td>
                                                        <td>
                                                            <asp:TextBox ID="txtHusName" runat="server" CssClass="TextBoxUpperCase" MaxLength="80"
                                                                ondblclick="SHOW_VALUE(this);"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblPhNo" runat="server" CssClass="lblStyle" Text="Mobile No"></asp:Label><span class="mandatory">*</span></td>
                                                        <td>
                                                            <asp:TextBox ID="txtPhNo" runat="server" CssClass="TextBoxStyle" Width="175px" onkeypress="return isNumberKey(event)"
                                                                MaxLength="10"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblEmrgNo" runat="server" CssClass="lblStyle" Text="Emergency No"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="txtEmrgNo" runat="server" CssClass="TextBoxStyle" onkeypress="return isNumberKey(event)"
                                                                MaxLength="11"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblIdentiFication" runat="server" CssClass="lblStyle" Text="Identity Mark"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtIdentiFication" runat="server" CssClass="TextBoxUpperCase" onblur="checkMandatory(this);"
                                                                MaxLength="40" Width="130px" ondblclick="SHOW_VALUE(this);"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblAffirmative" runat="server" CssClass="lblStyle" Text="Affirmative"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbAffirmative" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                        <td>
                                                            <asp:Label ID="lblUniqIDType" runat="server" CssClass="lblStyle" Text="Unique ID Type"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbUniqID" runat="server" CssClass="ddlStyle" AutoPostBack="true" OnSelectedIndexChanged="cmbUniqID_SelectedIndexChanged"></asp:DropDownList></td>
                                                        <td>
                                                            <asp:Label ID="lblUniqIDNo" runat="server" CssClass="lblStyle" Text="Unique ID No"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="txtUniqIDNo" runat="server" CssClass="TextBoxUpperCase" onblur="checkMandatory(this);" OnTextChanged="txtUniqIDNo_valchanged" AutoPostBack="true"
                                                                MaxLength="20"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblWorkArea" runat="server" CssClass="lblStyle" Text="Area of Work"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbWorkArea" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                         <td>
                                                            <asp:Label ID="Label33" runat="server" CssClass="lblStyle" Text="Medical Centre"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>                                                            
                                                            <asp:DropDownList ID="ddlMedCentre" runat="server" CssClass="ddlStyle">
                                                                <asp:ListItem Text="[Select]" Value="0"></asp:ListItem>
                                                                <asp:ListItem Text="Arogya Bhawan" Value="A"></asp:ListItem>
                                                                <asp:ListItem Text="Outside" Value="O"></asp:ListItem>                                                                
                                                            </asp:DropDownList>
                                                        </td>
                                                    </tr>
                                                    <%--Add by Prasun Chakraborty 11032022--%>
                                                    <asp:Panel ID="pnlFormA" runat="server">
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblPAN" runat="server" CssClass="lblStyle" Text="PAN No"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtPAN" runat="server" CssClass="TextBoxUpperCase" MaxLength="10" autocomplete="off">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblAADHAR" runat="server" CssClass="lblStyle" Text="Aadhar No"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtAADHAR" runat="server" CssClass="TextBoxUpperCase" MaxLength="12" autocomplete="off">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblNationality" runat="server" CssClass="lblStyle" Text="Nationality"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cmbNationality" runat="server" CssClass="ddlStyle">
                                                                    <asp:ListItem>[Select]</asp:ListItem>
                                                                    <asp:ListItem>Indian</asp:ListItem>
                                                                    <asp:ListItem>Other</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblPlaceOfEmployment" runat="server" CssClass="lblStyle" Text="Place of Employment"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cmbPlaceOfEmployment" runat="server" CssClass="ddlStyle">
                                                                    <asp:ListItem>[Select]</asp:ListItem>
                                                                    <asp:ListItem>Underground</asp:ListItem>
                                                                    <asp:ListItem>Opencast</asp:ListItem>
                                                                    <asp:ListItem>Surface</asp:ListItem>
                                                                </asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="Label29" runat="server" CssClass="lblStyle" Text="Relay Data"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:DropDownList ID="cmbRelayData" runat="server" CssClass="ddlStyle">
                                                                </asp:DropDownList>
                                                            </td>
                                                        </tr>
                                                        <tr class="tableStyle">
                                                            <td class="style2" colspan="3">ADULT PERSON TO BE CONTACTED IN CASE OF EMERGENCY</td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAdltName" runat="server" CssClass="lblStyle" Text="Name"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtAdltName" runat="server" CssClass="TextBoxUpperCase" MaxLength="100">
                                                                </asp:TextBox>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblAdltRelation" runat="server" CssClass="lblStyle" Text="Relation"></asp:Label><span class="mandatory">*</span> </td>
                                                            <td>
                                                                <asp:DropDownList ID="cmbAdltRelation" runat="server" CssClass="ddlStyle"></asp:DropDownList>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblAdltAddress" runat="server" CssClass="lblStyle" Text="Address"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtAdltAddress" runat="server" CssClass="TextBoxUpperCase" Width="175px" MaxLength="160">
                                                                </asp:TextBox>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:Label ID="lblAdltMobile" runat="server" CssClass="lblStyle" Text="Mobile No" MaxLength="10" onkeypress="return isNumberKey(event)"></asp:Label><span class="mandatory">*</span>
                                                            </td>
                                                            <td>
                                                                <asp:TextBox ID="txtAdltMobile" runat="server" CssClass="TextBoxUpperCase" MaxLength="20">
                                                                </asp:TextBox>
                                                            </td>
                                                        </tr>
                                                    </asp:Panel>
                                                    <%--End Add by Prasun Chakraborty 11032022--%>
                                                </table>
                                                <table width="100%" border="0" class="tableStyle"></table>
                                            </div>
                                            <table width="100%" border="0">
                                                <tr>
                                                    <td>
                                                        <asp:Button ID="btnSaveProfile" runat="server" Text="Save Profile" CssClass="btnStyle" /><asp:Button ID="btnUpdateProfile" runat="server" Text="Update Profile" CssClass="btnStyle"
                                                            OnClientClick="return checkBlankSP();" Visible="False" /><asp:Button ID="Btnreset" runat="server" Text="Reset" CssClass="btnStyle" />
                                                        <--- kindly update UAN Number(under EPFO Act) and IP Number(under ESIC Act). 

                                                    </td>
                                                    <td>
                                                        <asp:Label ID="LblempLeft" runat="server" CssClass="lblStyle" Visible="False"
                                                            ForeColor="Black" Text="MORE EMPLOYEE DETAILS TO BE FILLED  ARE"></asp:Label><asp:Label ID="Lblcount" runat="server" CssClass="btnStyle" Visible="False"></asp:Label></td>
                                                </tr>
                                            </table>
                                            <table border="0" width="100%" class="tableStyle"></table>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>


                                <%--<ajaxToolkit:TabPanel ID="TabPersonalPhoto" runat="server" HeaderText="Personal Image" TabIndex="1">
                                           <HeaderTemplate>
                                               Personal Image
                                           </HeaderTemplate>
                                           <ContentTemplate>
                                         
                            <asp:Panel ID="Panel1" runat="server" Width="100%">
                                <table width="100%" class="ModalTable" border="0">
                           
                                      <tr>
                                        <td colspan="8" class="tableStyle">
                                          IMAGE UPLOAD
                                        </td>
                                    </tr>
                                      
                                </table>

                                
                      
                              <table width="100%" border="0">
                                <tr>
                               
                                  
                                        <td>

                                          <asp:FileUpload id="FileUpload" runat="server" CssClass="lblStyle"/>
                                          <br />
                                            <asp:UpdatePanel ID="aaa" runat="server">
                                            <Triggers>
                                                <asp:PostBackTrigger ControlID="btnUpload" />
                                            </Triggers>
                                            <ContentTemplate>
                                          <asp:Button ID="btnUpload" runat="server" CssClass="btnStyle"
                                                Text="Upload Image"  OnClick="btnShowPhoto_Click" />
                                                </ContentTemplate>
                                        </asp:UpdatePanel>

                                        </td>

                                         <td >
                                 
                                            <asp:ImageMap ID="imgEmpPhoto" runat="server" Height="120px" Width="100px" />
                                            <br />
                                            


                                        </td>
                                </tr>
                                <tr>
                                <td style="color:Purple" >
                                IMAGE UPLOAD INSTRUCTIONS
                                </td>
                                </tr>
                                <tr>
                                <td>
                                <ul>
                                <li>Image Size Equal to or Less Than 100 KB</li>
                                </ul>
                                </td>
                                </tr>
                                </table>

                               
                            </asp:Panel>
                          
                        </ContentTemplate>
                         
                                
                    </ajaxToolkit:TabPanel>--%>

                                <ajaxToolkit:TabPanel ID="tabSkill" runat="server" HeaderText="Skill" TabIndex="1">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlSkillEntry" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable">
                                                <tr class="tableStyle">
                                                    <td>SKILL DETAILS </td>
                                                </tr>
                                            </table>
                                            <table width="100%" class="ModalTable" border="0">
                                                <tr>
                                                    <td colspan="8">
                                                        <asp:Label ID="lbl_skillnote" runat="server" CssClass="lblStyle" Style="color: red"
                                                            Text="* Note:- Please provide latest skill only" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <asp:Label ID="lblErrorMsgSkill" runat="server" CssClass="labelStyle1" Style="color: red" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <asp:Label ID="lbl_othmsg" runat="server" Style="color: red; background-color: yellow" Text="*- Trade (others) do not exists now, please choose specific trade from the drop down list" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <asp:Label ID="lbl_retcommments" runat="server" CssClass="labelStyle1" Style="color: red;" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <div id="dragSkillPopup" class="SectionHeaderL">
                                                            <table width="100%">
                                                                <tr valign="top">
                                                                    <td align="left" valign="top">
                                                                        <asp:Label ID="Label4" runat="server" CssClass="lblStyleBoldWhite"></asp:Label></td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="renewalSkillTR">
                                                     <td style="width: 16%">
                                                        <asp:Label ID="Label32" runat="server" CssClass="lblStyle" Text="Do you want to apply for skill assessment(during renewal)?"></asp:Label><span
                                                            class="mandatory">*</span> </td>
                                                     <td style="width: 6%">
                                                        <asp:DropDownList ID="ddlScfr" runat="server" AutoPostBack="true" CssClass="ddlStyle"
                                                            Width="150px">
                                                            <%--<asp:ListItem Enabled="true" Text="--Select--" Value="NA">--Select--</asp:ListItem>--%>
                                                            <asp:ListItem Enabled="true" Text="No" Value="No">No</asp:ListItem>
                                                            <asp:ListItem Text="Yes" Value="Yes">Yes</asp:ListItem>                                                            
                                                        </asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 6%">
                                                        <asp:Label ID="lblSkSkillType" runat="server" CssClass="lblStyle" Text="Skill type"></asp:Label><span
                                                            class="mandatory">*</span> </td>
                                                    <td style="width: 6%">
                                                        <asp:DropDownList ID="cmbSkSkillType" runat="server" AutoPostBack="true" CssClass="ddlStyle"
                                                            Width="150px">
                                                        </asp:DropDownList></td>
                                                    <td style="width: 4%">
                                                        <asp:Label ID="lblSkSkill" runat="server" CssClass="lblStyle" Text="Skill"></asp:Label><span
                                                            class="mandatory">*</span> </td>
                                                    <td style="width: 10%">
                                                        <asp:DropDownList ID="cmbSkSkill" runat="server" CssClass="ddlStyle" Width="250px" AutoPostBack="true"></asp:DropDownList></td>
                                                    <td style="width: 6%">
                                                        <asp:Label ID="lblSkRemarks" runat="server" CssClass="lblStyle" Text="Specialization"></asp:Label></td>
                                                    <td style="width: 10%">
                                                        <asp:TextBox ID="txtSkRemarks" runat="server" CssClass="TextBoxStyle" Height="40"
                                                            TextMode="MultiLine" Width="250">
                                                        </asp:TextBox></td>
                                                </tr>
                                            </table>
                                            <table class="ModalTable">
                                                <tr></tr>
                                            </table>
                                            <table width="100%">
                                                <tr>
                                                    <td style="width: 6%">
                                                        <asp:Label ID="lblSkillAssessment" runat="server" Text="Skill Assessment" CssClass="lblStyle"></asp:Label></td>
                                                    <td style="width: 6%">
                                                        <asp:DropDownList ID="ddlSKAss" runat="server" CssClass="ddlStyle">
                                                            <asp:ListItem Enabled="true" Text="Not Applicable" Value="NA">Not Applicable</asp:ListItem>
                                                            <asp:ListItem Text="Yes" Value="Yes">Yes</asp:ListItem>
                                                            <asp:ListItem Text="No" Value="No">No</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                    <td style="width: 6%">
                                                        <asp:Label ID="Label2" runat="server" CssClass="lblStyle" Text="Assessment Type"></asp:Label><span
                                                            class="mandatory" runat="server" id="spn_type">*</span> </td>
                                                    <td style="width: 10%">
                                                        <asp:DropDownList ID="drptypeassessment" runat="server" CssClass="ddlStyle" AutoPostBack="true"
                                                            Width="250px">
                                                            <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                            <asp:ListItem Value="T">Training cum Assessment</asp:ListItem>
                                                            <asp:ListItem Value="D">Direct Assessment</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                    <td style="width: 6%">
                                                        <asp:Label ID="lblSkillTrades" runat="server" CssClass="lblStyle" Text="Trade (Select)"></asp:Label><span
                                                            class="mandatory">*</span> </td>
                                                    <td style="width: 10%">
                                                        <%--<asp:DropDownList ID="ddlSkillTrade" runat="server" AutoPostBack="true" CssClass="ddlStyle"
                                                            Width="250px">
                                                        </asp:DropDownList>--%>     <%-- Line Commented by OSJ2756--%>
                                                        <%--OSJ2756 BEGINS 3--%>
                                                        <asp:TextBox ID="ddlSkillTrade" runat="server" Width="250px" />

                                                        <ajaxToolkit:AutoCompleteExtender ID="ddlSkillTrade_AutoCompleteExtender" runat="server"
                                                            DelimiterCharacters="" Enabled="True" MinimumPrefixLength="1" ServiceMethod="GetTradeName"
                                                            ServicePath="" TargetControlID="ddlSkillTrade" CompletionInterval="100" EnableCaching="true"
                                                            OnClientShowing="clientShowing2" CompletionListCssClass="CompletionListCssClass1"
                                                            CompletionListHighlightedItemCssClass="CompletionListHighlightedItemCssClass1"
                                                            CompletionListItemCssClass="CompletionListItemCssClass1">
                                                        </ajaxToolkit:AutoCompleteExtender>
                                                        <script type="text/javascript">
                                                            function clientShowing2(source, args) {
                                                                source._popupBehavior._element.style.zIndex = 100000;
                                                            }
                                                        </script>
                                                        <%--OSJ2756 ENDS 3--%>
                                                    </td>                                                    
                                                </tr>
                                                <tr>
                                                    <td colspan="3"></td>
                                                    <td align="left">
                                                        <asp:CheckBox ID="CheckBoxAllTrade" Visible="false" runat="server" AutoPostBack="true" CssClass="checkbox" />
                                                        <asp:Label ID="LabelAllTrade" Visible="false" runat="server" CssClass="lblStyle" Text="Include All Trade Code"></asp:Label>
                                                    </td>
                                                </tr>

                                            </table>
                                            <table>
                                                <tr>
                                                    <td colspan="6" align="left">
                                                        <asp:CheckBox ID="chk_waive" runat="server" Visible="false" AutoPostBack="true" />
                                                        <asp:Label ID="lbl_waiveoff" runat="server" Text="CLICK HERE TO WAIVE OFF JNTVTI CERTIFICATION" CssClass="lblStyle" Style="background-color: yellow; font-size: 16px" Visible="false" /><span style="font-size: 10px; color: red" visible="false" runat="server" id="spn_msg">((IN THIS CASE, APPROVAL FROM CHIEF/GM/HOD OF DEPTT IS MANDATORY TO BE ATTACHED BELOW)</span>

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="5" align="left">

                                                        <asp:Label ID="lbl_waivereason" runat="server" Text="Choose Waive Off Reason" CssClass="lblStyle" Visible="false" />
                                                        <%--<asp:DropDownList ID="drp_waiveoff" runat="server" Visible="false" onchange="funcminddraft()">
                                                        </asp:DropDownList>--%>
                                                        <asp:DropDownList ID="drp_waiveoff" runat="server" Visible="false" onchange="funcminddraft()"
                                                            OnSelectedIndexChanged="drp_waiveoff_SelectedIndexChanged" AutoPostBack="true">
                                                        </asp:DropDownList>

                                                    </td>
                                                    <td colspan="6" align="left">
                                                        <asp:Label ID="lbl_upload" runat="server" Text="Certificate Upload" CssClass="lblStyle" />

                                                        <asp:FileUpload ID="FileUploadSkill" Width="140" runat="server" /><asp:HiddenField ID="hidcertnoskill"
                                                            runat="server" />
                                                        <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkskilold" Text="Attach previous documents" />
                                                        <asp:HiddenField runat="server" ID="hdfskilold"
                                                            Value="" />
                                                        <asp:ImageButton runat="server" ID="imgskillold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                                                    </td>

                                                </tr>

                                                <tr>
                                                    <td style="width: 1%"></td>
                                                    <td style="width: 1%"></td>
                                                    <td style="width: 1%"></td>
                                                    <td style="width: 1%"></td>
                                                    <td style="width: 1%"></td>
                                                    <td>
                                                        <asp:Label ID="lbl_fileuploadskill" runat="server" CssClass="lblStyle" /></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 7%">
                                                        <asp:Label ID="lblOthSkillTrade" runat="server" CssClass="lblStyle" Text="Other Trade" Visible="false"></asp:Label><asp:Label ID="lblSkillassess" runat="server" CssClass="lblStyle" Text="Skill Set For Assessment" Visible="false" /></td>
                                                    <td colspan="6">
                                                        <asp:TextBox ID="txtOthSkillTrade" runat="server" CssClass="TextBoxStyle" Height="20" Style="text-transform: uppercase"
                                                            MaxLength="179" Width="400" placeholder="Please fill the details of Trade" Visible="false">
                                                        </asp:TextBox>
                                                        <asp:DropDownList ID="drp_skillassess" runat="server" CssClass="ddlStyle" Visible="false" /></td>
                                                </tr>
                                                <!-- WI6447 START ADDED BY PRASUN ON 24122021-->
                                                <tr>
                                                    <td colspan="11">
                                                        <div id="dv_WAIVE_DAYS" runat="server" visible="false">
                                                            <table>
                                                                <tr>
                                                                    <td>
                                                                        <asp:Label ID="Label26" runat="server" Text="How many days skill waiver required" CssClass="lblStyle" />
                                                                        <span class="mandatory">*</span>
                                                                        <asp:TextBox ID="txt_WAIVE_DAYS" runat="server" CssClass="TextBoxStyle" onkeyPress="return WaivDaysKeyPress(this)"> </asp:TextBox>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </div>
                                                    </td>
                                                </tr>
                                                <!-- END ADDED BY PRASUN ON 24122021-->
                                                <tr>
                                                    <td colspan="8">
                                                        <asp:Label ID="lblskilltradenote" runat="server" CssClass="lblSkNoteStyle" Style="color: Red"
                                                            Text="* Note:-"></asp:Label><br></br>
                                                        <asp:Label ID="lblskilltradenote1" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                            Text="-Skill certificate issued by JNTVTI for the selected Trade is mandatory. Please upload scanned copy of the skill certificate (in pdf format)."></asp:Label><br></br>
                                                        <%--<asp:Label ID="lblskilltradenote3" runat="server" CssClass="lblSkNoteStyle"  Style="color: Blue"
                                                            Text="-For Other Trades(Not covered in the list) select ‘Others’and mention the trade description"></asp:Label><br></br>--%>
                                                        <asp:Label ID="lblskilltradenote4" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                            Text="-For Engg & Projects, TGS– Select ‘Engg & Projects’ from dropdown of trade."></asp:Label><br></br>
                                                        <asp:Label ID="lblskilltradenote5" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                            Text="-For renewal of Safety pass, documents uploaded in past which are unchanged i.e., same as previous documents, then don’t re-upload the document through browse option. Only you need to select the ‘Attach previous document’ option."></asp:Label><br></br>
                                                        <asp:Label ID="lblskilltradenote2" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                            Text="-For any query about trade certification, contact JNTVTI(9709983435 or mail to rpm.jntvti@gmail.com)"></asp:Label><br></br>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <asp:Button ID="btnSaveSkill" runat="server" Text="Save" CssClass="btnStyle" Width="80"
                                                            Enabled="true" Visible="true" /><asp:Button ID="btnUpdateSkill" runat="server" Text="Update" CssClass="btnStyle"
                                                                Visible="false" Enabled="false" Width="80" />
                                                        <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkTermsCondition" Text="" />
                                                        <%-- WI6447 ADDED BY PRASUN CHAKRABORTY 03012022 --%>
                                                        <%--<label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>--%>
                                                        <label style="color: red; background-color: yellow">I hereby declare that furnished details and attached documents are true to the best of my knowledge. Skill waiver is taken for the specific job only and the person will not be deployed in job other than the mentioned in skill waiver form.</label>
                                                        <%-- ADDED BY PRASUN CHAKRABORTY 03012022 --%>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tblSkillErrorLst" runat="server" width="100%" class="tblErrorList"></table>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlSkillDetail" runat="server" Width="100%">
                                            <div id="divSkill">
                                                <br />
                                                <%-- WI6447 ADDED BY PRASUN CHAKRABORTY 03012022 --%>
                                                <asp:Label ID="lblSkillAssmntMsg" runat="server" Text="" ForeColor="Red" BackColor="Wheat" Font-Italic="true" Font-Bold="true" Font-Underline="true" Font-Size="Small" />
                                                <br />
                                                <%-- END ADDED BY PRASUN CHAKRABORTY 03012022 --%>
                                                <asp:Label ID="LblSkillMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><table
                                                    border="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <%---23/02/2024  TCS.2164315 Addition of gridview row event in gvskill for maintaining status message and reapply button logic---%>
                                                            <asp:GridView ID="gvSkill" OnRowCommand="gvSkill_RowCommand" OnRowDataBound="gvSkill_RowDataBound" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                                Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%" HeaderStyle-Font-Size="Smaller"
                                                                HeaderStyle-BackColor="#996AD3">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                        ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelectSkill" runat="server" OnCheckedChanged="chkSelectSkill"
                                                                                AutoPostBack="true" />
                                                                            <asp:HiddenField ID="hidgrdtradeinfo" runat="server" Value='<%#Eval("Skill_Trades") %>' />
                                                                            <%--OSJ2756--%>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="SKILL_TYPE" HeaderText="Skill Type" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="SKILL_NAME" HeaderText="Skill Name" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:TemplateField HeaderText="Specialization" ItemStyle-Font-Size="Smaller">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="ccst_remarks" Text='<%#Eval("ccst_remarks") %>' runat="server" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="ccst_loc_code" HeaderText="Skill assessment" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="Skill_Trades" HeaderText="Trade" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="Skill_assessment" HeaderText="Skill Assessment" ItemStyle-Font-Size="Smaller" />

                                                                    <asp:BoundField DataField="CCST_ASSESSMENT_DATE" HeaderText="Skill Assessment Date" ItemStyle-Font-Size="Smaller" Visible="false" />
                                                                    <asp:BoundField DataField="Assement_Result" HeaderText="Assessment Result" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidSkillassessmenttype" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_ASSESSMENT_TYPE") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="CCST_ASSESSMENT_TYPE1" HeaderText="Assessment Type" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="CCST_SKTD_OTH_REMRK" HeaderText="Other Trade" ItemStyle-Font-Size="Smaller" />

                                                                    <asp:TemplateField HeaderText="Attachment" ItemStyle-Font-Size="Smaller">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="lnkdownloadskill" runat="server" Text='<%#Eval("DM_NAME") %>'
                                                                                CommandArgument='<%#Eval("CCST_CERT_NO") %>' OnClick="downloadskill" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidSkillType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccst_skill_type_cd") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidskillcertno" runat="server" Value='<%#Eval("CCST_CERT_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidsafetypassskill" runat="server" Value='<%#Eval("CCST_SAFETY_PASS_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidSkillCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccst_skill_cd") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidSkillTradeCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_SKTD_CP_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdreqno" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_REQ_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">


                                                                        <ItemTemplate>


                                                                            <asp:HiddenField ID="hidSkillAssessment" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_SKTP_CP_CD") %>' />


                                                                        </ItemTemplate>


                                                                        <ItemStyle CssClass="hide" />


                                                                        <HeaderStyle CssClass="hide" />


                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-Font-Size="Smaller" HeaderText="Program Director Remarks">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lbl_remarks_PD" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CCST_REMARKS_PD") %>'> </asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <%-- WI6447 start add by prasun on 03012022--%>
                                                                    <asp:BoundField DataField="CCST_WAIVE_OFF_RESN" HeaderText="Waive Off Reason" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdvalidity_date" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_VALIDITY_DATE") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdWaive_Off" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_WAIVE_OFF") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdAssmnt_Type" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCST_ASSESSMENT_TYPE") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <%-- TCS.2164315 (23/02/2024) addition of two fields fo reapply skill assessment with button reapply option and status remarks.--%>
                                                                    <asp:TemplateField HeaderText="Reapply Skill Assessment" Visible="false" HeaderStyle-Width="40%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:Button runat="server" ID="btnReapply" Text="Reapply Skill" CssClass="btnStyle" CommandName="REAPPLY_SP_REQ" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "rowno")%>' />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Remarks" Visible="false" HeaderStyle-Width="20%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:Label runat="server" ID="reapplyremarks"></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <%--end add by prasun on 03012022--%>
                                                                </Columns>
                                                                <AlternatingRowStyle CssClass="gvAlternatRowStyle" />
                                                                <HeaderStyle CssClass="gvHeaderStyle" />
                                                                <RowStyle CssClass="gvItemStyle" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <%--Change By Anand start--%>
                                <%--Addition of address related columns (Village, PO, Thana,  Dist., State, PIN  )in Database and in UI while profile is getting created--%>
                                <ajaxToolkit:TabPanel ID="tabAddress" runat="server" HeaderText="Address" TabIndex="2">
                                    <ContentTemplate>
                                        <div id="divAddress">
                                            <asp:Panel ID="pnlAddressEntry" runat="server" Width="100%">
                                                <table class="ModalTable" border="0">
                                                    <tr>
                                                        <td colspan="8" class="tableStyle">ADDRESS DETAILS </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="8">
                                                            <asp:Label ID="lblnoteaddmsg" runat="server" CssClass="lblStyle" Style="color: red" Text="* Note:-Please provide mobile number and mail id of associates vendor" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblAddressType" runat="server" CssClass="lblStyle" Text="Address Type"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbAddressType" runat="server" CssClass="ddlStyle"></asp:DropDownList>

                                                        </td>
                                                        <td>
                                                            <asp:Label ID="lblAddName" runat="server" CssClass="lblStyle" Text="Care Of"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddName" runat="server" CssClass="TextBoxUpperCase"
                                                                Width="180px"></asp:TextBox></td>
                                                        <td colspan="1">
                                                            <asp:Label ID="lblAddHouseNo" runat="server" CssClass="lblStyle" Text="House/Plot/Door No/Land Mark"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddHouseNo" runat="server" CssClass="TextBoxUpperCase" MaxLength="50"
                                                                Width="180px"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="1">
                                                            <%--<asp:Label ID="lblAddStreet" runat="server" CssClass="lblStyle"
                                                                Text="Street/Village/PO/Thana"></asp:Label><span class="mandatory">*</span> </td>--%>
                                                            <asp:Label ID="lblAddStreet" runat="server" CssClass="lblStyle"
                                                                Text="Street"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddStreet" runat="server" CssClass="TextBoxUpperCase"
                                                                MaxLength="50" Width="180px"></asp:TextBox></td>
                                                        <td colspan="1">
                                                            <asp:Label ID="lblAddVillage" runat="server" CssClass="lblStyle" Text="Village"></asp:Label>
                                                            <span class="mandatory">*</span>
                                                        </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddVillage" runat="server" CssClass="TextBoxUpperCase" MaxLength="50"
                                                                Width="180">
                                                            </asp:TextBox>
                                                        </td>
                                                        <td colspan="1">
                                                            <asp:Label ID="lblAddPO" runat="server" CssClass="lblStyle" Text="Post Office"></asp:Label>
                                                            <span class="mandatory">*</span>
                                                        </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddPO" runat="server" CssClass="TextBoxUpperCase" MaxLength="50"
                                                                Width="180">
                                                            </asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="1">
                                                            <asp:Label ID="lblAddThana" runat="server" CssClass="lblStyle" Text="Thana"></asp:Label>
                                                            <span class="mandatory">*</span>
                                                        </td>
                                                        <td colspan="1">
                                                            <asp:TextBox ID="txtAddThana" runat="server" CssClass="TextBoxUpperCase" MaxLength="50"
                                                                Width="180">
                                                            </asp:TextBox>
                                                        </td>

                                                        <td>
                                                            <asp:Label ID="lblAddCountry" runat="server" CssClass="lblStyle" Text="Country"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbAddCountry" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                        <td>
                                                            <asp:Label ID="lblAddState" runat="server" CssClass="lblStyle" Text="State"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbAddState" runat="server" AutoPostBack="True"
                                                                CssClass="ddlStyle">
                                                            </asp:DropDownList></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblAddCity" runat="server" CssClass="lblStyle" Text="City"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbAddCity" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                        <td>
                                                            <asp:Label ID="lblAddDistrict" runat="server" CssClass="lblStyle" Text="District"></asp:Label>
                                                            <span class="mandatory">*</span>
                                                        </td>
                                                        <td>
                                                            <asp:DropDownList ID="cmbAddDistrict" runat="server" CssClass="ddlStyle">
                                                            </asp:DropDownList>
                                                        </td>


                                                        <td>
                                                            <asp:Label ID="lblAddPIN" runat="server" CssClass="lblStyle" Text="PIN"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="txtAddPIN" runat="server" CssClass="TextBoxStyle"
                                                                MaxLength="6" onkeypress="return isNumberKey(event)"></asp:TextBox></td>

                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblAddMobile" runat="server" CssClass="lblStyle"
                                                                Text="Mobile No"></asp:Label><span style="color: red">*</span> </td>
                                                        <td>
                                                            <asp:TextBox ID="txtAddMobile" runat="server" CssClass="TextBoxStyle"
                                                                MaxLength="10" onkeypress="return isNumberKey(event)"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblLandLine" runat="server" CssClass="lblStyle" Text="Land Line"></asp:Label></td>
                                                        <td>
                                                            <asp:TextBox ID="txtLandLine" runat="server" CssClass="TextBoxStyle"
                                                                MaxLength="11" onkeypress="return isNumberKey(event)"></asp:TextBox></td>
                                                        <td>
                                                            <asp:Label ID="lblAddEmail" runat="server" CssClass="lblStyle" Text="Email ID"></asp:Label><span style="color: red">*</span> </td>
                                                        <td colspan="3">
                                                            <asp:TextBox ID="txtAddEmail" runat="server" CssClass="TextBoxStyle"
                                                                MaxLength="50" Width="200px"></asp:TextBox></td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lbl_attachment" runat="server" Text="Adhar/Voter/Ration card" CssClass="lblStyle" /><span style="color: red">*</span></td>
                                                        <td colspan="4">
                                                            <asp:FileUpload ID="fupdl_add" runat="server" />

                                                            <asp:CheckBox runat="server" CssClass="lblStyle" ID="ChkoldAddress" Text="Attach previous documents" />
                                                            <asp:HiddenField runat="server" ID="hddaddressold" Value="" />
                                                            <asp:ImageButton runat="server" ID="imgaddressold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />

                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td></td>
                                                        <td>
                                                            <asp:Label ID="lbladdattachname" runat="server" CssClass="lblStyle" /></td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="6">
                                                            <asp:Label ID="lbladdnote" runat="server" CssClass="lblSkNoteStyle" Style="color: Red"
                                                                Text="* Note:-"></asp:Label><br></br>
                                                            <asp:Label ID="lbladdnote1" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                                Text="-For renewal of Safety pass, documents uploaded in past which are unchanged i.e., same as previous documents, then don’t re-upload the document through browse option. Only you need to select the ‘Attach previous document’ option."></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="6">
                                                            <asp:Button ID="btnSaveAddress" runat="server" CssClass="btnStyle"
                                                                OnClientClick="return checkBlankSP();" Text="Save" Width="80px" />

                                                            <%-- </td>
                                                    </tr>
                                                    <tr>
                                                        <td>--%>
                                                            <asp:Button ID="btnUpdateAddress" runat="server" CssClass="btnStyle"
                                                                OnClientClick="return checkBlankSP();" Text="Update" Visible="False"
                                                                Width="80px" />
                                                            <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>

                                                        </td>
                                                    </tr>
                                                </table>
                                                <table id="tblAddErrorLst" runat="server" width="100%" class="tblErrorList"></table>
                                            </asp:Panel>
                                            <asp:Label ID="LblAddMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><asp:Panel ID="pnlAddressDetail" runat="server" Width="97%">
                                                <table border="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="gvAddress" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                                Width="100%">
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
                                                                <HeaderStyle BackColor="#996AD3" ForeColor="black" Font-Names="Arial" Font-Size="XX-Small" />
                                                                <RowStyle BackColor="#ccffcc" Font-Names="Arial" Font-Size="XX-Small" ForeColor="black" />
                                                                <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </div>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <%--Change By Anand End--%>
                                <%--Addition of address related columns (Village, PO, Thana,  Dist., State, PIN  )in Database and in UI while profile is getting created--%>

                                <ajaxToolkit:TabPanel ID="tabAge" runat="server" HeaderText="Age Proof and Others" TabIndex="3">
                                    <ContentTemplate>
                                        <table class="ModalTable" width="100%">
                                            <tr class="tableStyle">
                                                <td>AGE PROOF AND OTHER DOCUMENT DETAILS </td>
                                            </tr>
                                        </table>
                                        <table width="100%">
                                            <tr>
                                                <td style="text-align: left">
                                                    <asp:Label ID="lbl_age" runat="server" Text="Age Proof" CssClass="lblStyle" /><span style="color: red">*</span> </td>
                                                <td style="text-align: left">
                                                    <asp:FileUpload ID="fupdlage" Width="140" runat="server" />
                                                    <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkageold" Text="Attach previous documents" />
                                                    <asp:ImageButton runat="server" ID="imbageold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                                                    <asp:HiddenField runat="server" ID="hdfageold" Value="" />
                                                </td>
                                                <td style="text-align: left">
                                                    <asp:Label ID="lbl_drv" runat="server" Text="Driving License" CssClass="lblStyle" /></td>
                                                <td style="text-align: left">
                                                    <asp:FileUpload ID="fupdldrv" Width="140" runat="server" />
                                                    <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkdriverold" Text="Attach previous documents" />
                                                    <asp:ImageButton runat="server" ID="imbdriverold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                                                    <asp:HiddenField runat="server" ID="hdfdriverold" Value="" />

                                                </td>

                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td>
                                                    <asp:Label ID="lbl_agenote" runat="server" CssClass="lblStyle" Text="Birth Certificate/Aadhaar/Passport/Pan copy" Style="color: red" /></td>
                                                <td></td>
                                                <td>
                                                    <asp:Label ID="lbl_drvnote" Text="(Driving license is mandatory for driver)" CssClass="lblStyle" runat="server" Style="color: red" /></td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td>
                                                    <asp:Label ID="lbl_dobfile" runat="server" CssClass="lblStyle" /></td>
                                                <td></td>
                                                <td>
                                                    <asp:Label ID="lbl_drvfile" runat="server" CssClass="lblStyle" /></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:Label ID="lbl_passport" runat="server" Text="Passport Doc." CssClass="lblStyle" /></td>
                                                <td colspan="2">
                                                    <asp:FileUpload ID="fupdlpass" Width="140" runat="server" />
                                                    <asp:CheckBox runat="server" CssClass="lblStyle" ID="chkpassold" Text="Attach previous documents" />
                                                    <asp:ImageButton runat="server" ID="imgpassold" Height="20" ToolTip="click here to view previous documents" AlternateText="click here to view old attachment" ImageUrl="~/images/pdf.jpeg" />
                                                    <asp:HiddenField runat="server" ID="hdfpassold" Value="" />

                                                </td>
                                            </tr>
                                            <tr>
                                                <td></td>
                                                <td>
                                                    <asp:Label ID="lbl_passfile" runat="server" CssClass="lblStyle" /></td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    <asp:HiddenField ID="hiddob" runat="server" />
                                                    <asp:HiddenField ID="hiddrv" runat="server" />
                                                    <asp:HiddenField ID="hidpass" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Label ID="lblagenote" runat="server" CssClass="lblSkNoteStyle" Style="color: Red"
                                                        Text="* Note:-"></asp:Label><br></br>
                                                    <asp:Label ID="Label1" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                        Text="-Age Proof document is mandatory."></asp:Label><br />
                                                    <asp:Label ID="lblagenote1" runat="server" CssClass="lblSkNoteStyle" Style="color: Blue"
                                                        Text="-For renewal of Safety pass, documents uploaded in past which are unchanged i.e., same as previous documents, then don’t re-upload the document through browse option. Only you need to select the ‘Attach previous document’ option."></asp:Label>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td colspan="4">
                                                    <asp:Button ID="btnsaveage" runat="server" CssClass="btnStyle" Text="Save" />

                                                    <%--</td>
                                                <td>--%>
                                                    <asp:Button ID="btnupdateage" runat="server" CssClass="btnStyle" Text="Update" Visible="false" Enabled="false" />
                                                    <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>

                                                </td>

                                            </tr>
                                        </table>
                                        <asp:Label ID="LblAgeMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><table>
                                            <tr>
                                                <td>
                                                    <asp:GridView ID="grdage" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                        Font-Names="Verdana" Font-Size="Smaller" ForeColor="Black" Width="100%" HeaderStyle-BackColor="#996AD3">
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
                                                                    <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CET_REQUEST_NO") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                                <HeaderStyle CssClass="hide" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </td>
                                            </tr>
                                        </table>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel ID="tabQualification" runat="server" HeaderText="Qualification" TabIndex="4">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlQualEntry" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable" border="0">
                                                <tr>
                                                    <td colspan="8" class="tableStyle">QUALIFICATION DETAILS </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="8">
                                                        <asp:Label ID="lbl_qualnote" runat="server" Text="* Note: Qualification document is mandatory other than illiterate" CssClass="lblStyle" Style="color: red" /></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 12%">
                                                        <asp:Label ID="lblQualType" runat="server" CssClass="lblStyle" Text="Qualification Type"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td style="width: 20%">
                                                        <asp:DropDownList ID="cmbQualType" runat="server" CssClass="ddlStyle" AutoPostBack="true"></asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 10%">
                                                        <asp:Label ID="lblQualification" runat="server" CssClass="lblStyle" Text="Qualification"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td style="width: 20%">
                                                        <asp:DropDownList ID="cmbQualification" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 8%">
                                                        <asp:Label ID="lblQualRemarks" runat="server" CssClass="lblStyle" Text="Remarks"></asp:Label></td>
                                                    <td>
                                                        <asp:TextBox ID="txtQualRemarks" runat="server" CssClass="TextBoxStyle" Width="250"
                                                            Height="40" TextMode="MultiLine"> 
                                                        </asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 8%">
                                                        <asp:Label ID="lblqualdoc" runat="server" CssClass="lblStyle" Text="Qualification Doc." /></td>
                                                    <td>
                                                        <asp:FileUpload ID="fupdlqual" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="hdqualcertid" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style="width: 8%">
                                                        <asp:HiddenField ID="hdqualid" runat="server" />
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblcertname" runat="server" CssClass="lblStyle" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                        <asp:Button ID="btnSaveQual" runat="server" Text="Save" CssClass="btnStyle" Width="80" /><asp:Button ID="btnUpdateQual" runat="server" Text="Update" CssClass="btnStyle" Width="80" Visible="false" />
                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tblQualErrorLst" runat="server" width="100%" class="tblErrorList"></table>
                                        </asp:Panel>
                                        <asp:Label ID="LblQualiMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><asp:Panel ID="pnlQualDetail" runat="server" Width="97%">
                                            <table border="0" width="100%">
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="gvQualification" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                            Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectQual" runat="server" OnCheckedChanged="chkSelectQual" AutoPostBack="true" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="QUAL_TYPE" HeaderText="Qualification Type" />
                                                                <asp:BoundField DataField="QUAL_NAME" HeaderText="Qualification Name" />
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%" HeaderText="Attachment">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkqual" runat="server" Text='<%#Bind("DM_NAME") %>' CommandArgument='<%#Bind("CQL_CERT_NO") %>' OnClick="downloadqual" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="cql_remarks" HeaderText="Remarks" />
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidQualID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CQL_QUAL_ID") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidQualType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CQL_QUAL_TYPE") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidQualCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CQL_QUAL_CODE") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidQualCERT" runat="server" Value='<%#Bind("CQL_CERT_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CQL_REQ_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle BackColor="#996AD3" ForeColor="black" Font-Names="Arial" Font-Size="XX-Small" />
                                                            <RowStyle BackColor="#ccffcc" Font-Names="Arial" Font-Size="XX-Small" ForeColor="black" />
                                                            <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <ajaxToolkit:TabPanel ID="tabExp" runat="server" HeaderText="Experience" TabIndex="5">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel1" runat="server">
                                            <table width="100%" class="ModalTable">
                                                <tr class="tableStyle">
                                                    <td>EXPERIENCE DETAILS </td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td width="20%">
                                                        <asp:Label ID="lblcompname" CssClass="lblStyle" Text="Company Name" runat="server" /><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txtcompname" runat="server" Style="text-transform: uppercase" /></td>
                                                    <td width="20%">
                                                        <asp:Label ID="lblstdt" CssClass="lblStyle" Text="Start Date" runat="server" /><span class="mandatory">*</span> </td>
                                                    <td width="20%">
                                                        <asp:TextBox ID="txtstdt" runat="server" /><ajaxToolkit:CalendarExtender ID="CalendarExtender11" runat="server" PopupButtonID="txtTrnStartDt"
                                                            PopupPosition="BottomLeft" TargetControlID="txtstdt" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender8" runat="server" TargetControlID="txtstdt"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label7" CssClass="lblStyle" Text="End Date" runat="server" /></td>
                                                    <td>
                                                        <asp:TextBox ID="txtenddt" runat="server" /><ajaxToolkit:CalendarExtender ID="CalendarExtender12" runat="server" PopupButtonID="txtTrnStartDt"
                                                            PopupPosition="BottomLeft" TargetControlID="txtenddt" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender9" runat="server" TargetControlID="txtenddt"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label8" CssClass="lblStyle" Text="Designation" runat="server" /><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txtdesignation" runat="server" Style="text-transform: uppercase" /></td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label9" CssClass="lblStyle" Text="Work Area" runat="server" ToolTip="Domain of Work" /><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpexparea" runat="server" AutoPostBack="true" /></td>
                                                    <td>
                                                        <asp:Label ID="Label13" CssClass="lblStyle" Text="State" runat="server" MaxLength="50" /><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="drpexpstate" runat="server" Style="width: 200px" AutoPostBack="true" /><asp:Label ID="Label14" CssClass="lblStyle" Text="Year Of Experience" runat="server" Visible="false" /><td>
                                                            <asp:Label ID="Label10" CssClass="lblStyle" Text="Work Location" runat="server" MaxLength="50" /><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:DropDownList ID="drpexploc" runat="server" Style="width: 200px" /><asp:Label ID="lblexpyr" CssClass="lblStyle" Text="Year Of Experience" runat="server" Visible="false" /><asp:TextBox ID="txtexpyr" runat="server" Visible="false" /></td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:TextBox ID="txt_otherdom" runat="server" CssClass="txtStyle" Visible="false" Style="width: 200px" MaxLength="50" /></td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label12" runat="server" CssClass="lblStyle" Text=" Certificate Upload"></asp:Label></td>
                                                    <td>
                                                        <asp:FileUpload ID="FileUploadExp" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Label ID="lbl_uploadedexp" runat="server" Style="font-size: 12px; font: bold" CssClass="lblStyle" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="hidexpsafety" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table style="margin-left: 830px">
                                                <tr></tr>
                                            </table>
                                            <table width="100%" class="ModalTable" border="0">
                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="hidcertno" runat="server" />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr align="center">
                                                    <td colspan="6" align="left">
                                                        <asp:Button ID="btnSaveExp" runat="server" Text="Save" CssClass="btnStyle" Width="80" Visible="true" /><asp:Button ID="btnUpdateExp" runat="server" Text="Update" CssClass="btnStyle" Width="80" Visible="false" Enabled="false" />
                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:Label ID="LblExpMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><table border="0" width="100%">
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="grvExp" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                            Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%" HeaderStyle-Font-Size="Smaller" HeaderStyle-BackColor="#996AD3">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectExp" runat="server" OnCheckedChanged="chkSelectExp" AutoPostBack="true" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidcertno" runat="server" Value='<%#Eval("CWET_CERT_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="CWET_COMP_Name" HeaderText="Company Name" ItemStyle-Font-Size="Smaller" />
                                                                <asp:BoundField DataField="STDT" HeaderText="Start Date" ItemStyle-Font-Size="Smaller" />
                                                                <asp:BoundField DataField="ENDDT" HeaderText="End Date" ItemStyle-Font-Size="Smaller" />
                                                                <asp:BoundField DataField="CWET_DESIGNATION" HeaderText="Designation" ItemStyle-Font-Size="Smaller" />
                                                                <asp:BoundField DataField="domain" HeaderText="Work Area" ItemStyle-Font-Size="Smaller" />
                                                                <asp:BoundField DataField="area" HeaderText="Work Location" ItemStyle-Font-Size="Smaller" />
                                                                <asp:TemplateField HeaderText="Atttachment" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkexp" runat="server" Text='<%#Eval("DM_NAME") %>' CommandArgument='<%#Eval("CWET_CERT_NO") %>' OnClick="downloadexp" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidcertnochk" runat="server" Value='<%#Eval("CWET_CERT_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidsrl" runat="server" Value='<%#Eval("CWET_SERIAL_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidcompname" runat="server" Value='<%#Eval("CWET_COMP_NAME") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidsafety" runat="server" Value='<%#Eval("CWET_SAFETY_PASS_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidexp" runat="server" Value='<%#Eval("CWET_EXP_YR") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidstdt" runat="server" Value='<%#Eval("STDT") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidenddt" runat="server" Value='<%#Eval("enddt") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hiddesig" runat="server" Value='<%#Eval("CWET_DESIGNATION") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidworkarea" runat="server" Value='<%#Eval("workarea") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidworklocation" runat="server" Value='<%#Eval("CWET_WORK_LOCATION") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CWET_REQ_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle CssClass="gvHeaderStyle" />
                                                            <RowStyle CssClass="gvItemStyle" />
                                                            <AlternatingRowStyle CssClass="gvAlternatRowStyle" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                            </tr></table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <%--<ajaxToolkit:TabPanel ID="tabSkill" runat="server" HeaderText="Skill"
                        TabIndex="4"><ContentTemplate><asp:Panel ID="pnlSkillEntry" runat="server" Width="100%"><table width="100%" class="ModalTable"><tr class="tableStyle"><td >SKILL DETAILS </td></tr></table><table width="100%" class="ModalTable" border="0"><tr><td colspan="8"><asp:Label ID="lbl_skillnote" runat="server" CssClass="lblStyle" style="color:red" Text="* Note:- Please provide latest skill only" /></td></tr><tr><td colspan="6"><div id="dragSkillPopup" class="SectionHeaderL"><table width="100%"><tr valign="top"><td align="left" valign="top"><asp:Label ID="Label4" runat="server" CssClass="lblStyleBoldWhite"></asp:Label></td></tr></table></div></td></tr><tr ><td style="width:6%"><asp:Label ID="lblSkSkillType" runat="server" CssClass="lblStyle" Text="Skill type"></asp:Label><span class="mandatory">*</span> </td><td style="width: 6%"><asp:DropDownList ID="cmbSkSkillType" runat="server" AutoPostBack="true" CssClass="ddlStyle" Width="150px"></asp:DropDownList></td><td style="width: 4%"><asp:Label ID="lblSkSkill" runat="server" CssClass="lblStyle" Text="Skill"></asp:Label><span class="mandatory">*</span> </td><td style="width:10%"><asp:DropDownList ID="cmbSkSkill" runat="server" CssClass="ddlStyle" Width="250px"></asp:DropDownList></td><td style="width:6%"><asp:Label ID="lblSkRemarks" runat="server" CssClass="lblStyle" Text="Specialization"></asp:Label></td><td style="width:10%"><asp:TextBox ID="txtSkRemarks" runat="server" CssClass="TextBoxStyle" Height="40" TextMode="MultiLine" Width="250">
                                            </asp:TextBox></td></tr></table><table class="ModalTable"><tr></tr></table><table width="100%" ><tr><td style="width:8%"><asp:Label ID="lblSkillAssessment" runat="server" Text="Skill Assessment" CssClass="lblStyle"></asp:Label></td><td style="width:8%"><asp:DropDownList ID="ddlSKAss" runat="server" CssClass="ddlStyle"><asp:ListItem Enabled="true" Text="Not Applicable" Value="NA">Not Applicable</asp:ListItem><asp:ListItem Text="Yes" Value="Yes">Yes</asp:ListItem><asp:ListItem  Text="No" Value="No">No</asp:ListItem></asp:DropDownList></td><td style="width:2%"><asp:Label ID="lbl_upload" runat="server" Text="Certificate Upload" CssClass="lblStyle" /></td><td style="width:6%"><asp:FileUpload ID="FileUploadSkill" runat="server" /><asp:HiddenField ID="hidcertnoskill" runat="server"/></td><td style="width:20%"></td></tr><tr><td style="width:1%"></td><td style="width:1%"></td><td style="width:1%"></td><td><asp:Label ID="lbl_fileuploadskill" runat="server" cssclass="lblStyle"/></td></tr><tr><td colspan="6"><asp:Button ID="btnSaveSkill" runat="server" Text="Save" CssClass="btnStyle" Width="80" enabled="false"  /><asp:Button ID="btnUpdateSkill" runat="server" Text="Update" CssClass="btnStyle" Visible="false"  Enabled="false" Width="80" /><br /></td></tr></table><table id="tblSkillErrorLst" runat="server" width="100%" class="tblErrorList"></table></asp:Panel><asp:Panel ID="pnlSkillDetail" runat="server" Width="100%"><div id="divSkill"><br /><asp:Label ID="LblSkillMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><table border="0" width="100%"><tr><td><asp:GridView ID="gvSkill" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                    Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%" HeaderStyle-Font-Size="Smaller" HeaderStyle-BackColor="#996AD3"><Columns><asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                            ItemStyle-HorizontalAlign="Center"><ItemTemplate><asp:CheckBox ID="chkSelectSkill" runat="server" OnCheckedChanged="chkSelectSkill" AutoPostBack ="true" /></ItemTemplate></asp:TemplateField><asp:BoundField DataField="SKILL_TYPE" HeaderText="Skill Type" ItemStyle-Font-Size="Smaller"/><asp:BoundField DataField="SKILL_NAME" HeaderText="Skill Name" ItemStyle-Font-Size="Smaller"/><asp:BoundField DataField="ccst_remarks" HeaderText="Specialization" ItemStyle-Font-Size="Smaller"/><asp:BoundField DataField="ccst_loc_code" HeaderText="Skill assessment" ItemStyle-Font-Size="Smaller"/><asp:TemplateField HeaderText="Attachment" ItemStyle-Font-Size="Smaller"><ItemTemplate><asp:LinkButton ID="lnkdownloadskill" runat="server" Text='<%#Eval("DM_NAME") %>' CommandArgument='<%#Eval("CCST_CERT_NO") %>' OnClick="downloadskill"/></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%"><ItemTemplate><asp:HiddenField ID="hidSkillType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccst_skill_type_cd") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField><asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%"><ItemTemplate><asp:HiddenField ID="hidskillcertno" runat="server" Value='<%#Eval("CCST_CERT_NO") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField><asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%"><ItemTemplate><asp:HiddenField ID="hidsafetypassskill" runat="server" Value='<%#Eval("CCST_SAFETY_PASS_NO") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField><asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%"><ItemTemplate><asp:HiddenField ID="hidSkillCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccst_skill_cd") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField> <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                <ItemTemplate>
                                                                    <asp:HiddenField ID="hidSkillID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem,"CQL_QUAL_ID") %>' />
                                                                </ItemTemplate>
                                                                <ItemStyle CssClass="hide" />
                                                                <HeaderStyle CssClass="hide" />
                                                            </asp:TemplateField> </Columns><AlternatingRowStyle CssClass="gvAlternatRowStyle" /><HeaderStyle CssClass="gvHeaderStyle" /><RowStyle CssClass="gvItemStyle" /></asp:GridView></td></tr></table></div></asp:Panel></ContentTemplate></ajaxToolkit:TabPanel>--%>
                                <ajaxToolkit:TabPanel ID="tabTraining" runat="server" HeaderText="Training" TabIndex="6">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlTrainingEntry" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable" border="0">
                                                <tr class="tableStyle">
                                                    <td>
                                                        <asp:Label ID="Label3" runat="server" CssClass="lblStyleBoldWhite" Text="TRAINING DETAILS"></asp:Label></td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblTrnAgency" runat="server" CssClass="lblStyle" Text="Agency"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbTrnAgency" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblTrnLoc" runat="server" CssClass="lblStyle" Text="Location"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbTrnLoc" runat="server" CssClass="ddlStyle" AutoPostBack="true"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblTrainingType" runat="server" CssClass="lblStyle" Text="Training Type"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbTraningType" runat="server" CssClass="ddlStyle" AutoPostBack="true"></asp:DropDownList></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblTrnCource" runat="server" CssClass="lblStyle" Text="Course"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbTrnCource" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblTrnStartDt" runat="server" CssClass="lblStyle" Text="Training Start Date"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTrnStartDt" runat="server" CssClass="TextBoxStyle" AutoPostBack="true">

                                                        </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender3" runat="server" PopupButtonID="txtTrnStartDt"
                                                            PopupPosition="BottomLeft" TargetControlID="txtTrnStartDt" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender5" runat="server" TargetControlID="txtTrnStartDt"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="lblTrnEndDt" runat="server" CssClass="lblStyle" Text="Training End Date"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txtTrnEndDt" runat="server" CssClass="TextBoxStyle"> 
                                                        </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender1" runat="server" PopupButtonID="txtTrnEndDt"
                                                            PopupPosition="BottomLeft" TargetControlID="txtTrnEndDt" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender6" runat="server" TargetControlID="txtTrnEndDt"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblTrnResult" runat="server" CssClass="lblStyle" Text="Result"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbTrnResult" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblTrnRemarks" runat="server" CssClass="lblStyle" Text="Remarks"></asp:Label></td>
                                                    <td colspan="3">
                                                        <asp:TextBox ID="txtTrnRemarks" runat="server" CssClass="TextBoxStyle" Width="300"
                                                            Height="40" TextMode="MultiLine">
                                                        </asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="1">
                                                        <asp:Label ID="lblfilemsg" runat="server" CssClass="lblStyle" Text="Certificate Upload" /><td>
                                                            <asp:FileUpload ID="fileuploadtrn" runat="server" /><asp:HiddenField ID="hidcertrnnoTrns" runat="server" />
                                                        </td>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Label ID="lbl_fileuploadtrn" runat="server" CssClass="lblStyle" Style="font-size: 10px; font: bold" /></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6" align="left">
                                                        <asp:Button ID="btnSaveTraining" runat="server" Text="Save" CssClass="btnStyle" Width="80" /><asp:Button ID="btnUpdateTraining" runat="server" Text="Update" CssClass="btnStyle" Visible="false" Enabled="false"
                                                            Width="80" />
                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>

                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                        <asp:Panel ID="pnlTrainingDetail" runat="server" Width="100%">
                                            <asp:Label ID="LblTrnMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><div id="divTraining">
                                                <table border="0" width="100%">
                                                    <tr>
                                                        <td>
                                                            <asp:GridView ID="gvTraining" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                                Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%" HeaderStyle-Font-Size="Smaller" HeaderStyle-BackColor="#996AD3">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                        ItemStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <asp:CheckBox ID="chkSelectTraining" runat="server" OnCheckedChanged="chkSelectTraining" AutoPostBack="true" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="AGENCY_NAME" HeaderText="Agency" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="LOCATION_NAME" HeaderText="Location" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="TRAINING_NAME" HeaderText="Training Type" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="COURCE_NAME" HeaderText="Course Code" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="CCTT_START_DT" HeaderText="Start Date" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="CCTT_END_DT" HeaderText="End Date" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="RESULT_DESC" HeaderText="Result" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:BoundField DataField="CCTT_REMARKS" HeaderText="Remarks" ItemStyle-Font-Size="Smaller" />
                                                                    <asp:TemplateField HeaderText="Attachment" ItemStyle-Font-Size="Smaller">
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="lnkdownloadTrn" runat="server" Text='<%#Eval("DM_NAME") %>' CommandArgument='<%#Eval("CCTT_CERT_NO") %>' OnClick="downloadtrn" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrainingID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCTT_TRN_ID") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrncerno" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CCTT_CERT_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrnAgency" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "AGENCY_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrnLoc" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "LOCATION_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrnType" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "TRAINING_TYPE") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrnCourceCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "COURCE_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hidTrnResult" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "RESULT_CD") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CCTT_REQ_NO") %>' />
                                                                        </ItemTemplate>
                                                                        <ItemStyle CssClass="hide" />
                                                                        <HeaderStyle CssClass="hide" />
                                                                    </asp:TemplateField>
                                                                </Columns>
                                                                <AlternatingRowStyle CssClass="gvAlternatRowStyle" />
                                                                <HeaderStyle CssClass="gvHeaderStyle" />
                                                                <RowStyle CssClass="gvItemStyle" />
                                                            </asp:GridView>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </div>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel ID="tabMedical" runat="server" HeaderText="Medical(Abnormality Case)" TabIndex="7">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlmedical" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable" border="0">
                                                <tr class="tableStyle">
                                                    <td>
                                                        <asp:Label ID="Label6" runat="server" CssClass="lblStyleBoldWhite" Text="MEDICAL VERIFICATION(ABNORMALITY CASE)"></asp:Label></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbl_mednote" runat="server" CssClass="lblStyle" /></td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lbl_medmsgheader" runat="server" Text="*Note:-Please upload all files in pdf format maximum 100KB of size" CssClass="lblStyle" /></td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label11" runat="server" Text="Doctor's fitness certificate" CssClass="lblStyle" /><span style="color: red">*</span> </td>
                                                    <td>
                                                        <asp:FileUpload ID="fupdlfitnesscer" runat="server" /></td>
                                                    <td>
                                                        <asp:Label ID="Label15" runat="server" Text="Undertaking Certificate" CssClass="lblStyle" /><span style="color: red">*</span> </td>
                                                    <td>
                                                        <asp:FileUpload ID="fupdlundertake" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Label ID="lbl_filefitness" runat="server" CssClass="lblStyle" /></td>
                                                    <td></td>
                                                    <td>
                                                        <asp:Label ID="lbl_fileunder" runat="server" CssClass="lblStyle" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label19" runat="server" Text="Wellness Clinic Certificate" CssClass="lblStyle" /><span style="color: red">*</span> </td>
                                                    <td>
                                                        <asp:FileUpload ID="fupdlwcc" runat="server" /></td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:Label ID="lbl_filewcc" runat="server" CssClass="lblStyle" /></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:HiddenField ID="hdfit" runat="server" />
                                                        <asp:HiddenField ID="hdunder" runat="server" />
                                                        <asp:HiddenField ID="hdwcc" runat="server" />
                                                        <asp:HiddenField ID="hdmedid" runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="4">
                                                        <asp:Button ID="btnsavemed" runat="server" CssClass="btnStyle" Text="Save" />

                                                        <%--</td>
                                                    <td>--%>
                                                        <asp:Button ID="btnupdatemed" runat="server" CssClass="btnStyle" Text="Update" Visible="false" Enabled="false" />
                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>
                                                    </td>
                                                </tr>
                                            </table>
                                            <asp:Label ID="Label21" runat="server" CssClass="lblStyle" Text=""></asp:Label><table>
                                                <tr>
                                                    <td>
                                                        <asp:GridView ID="gvmed" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                            Font-Names="Verdana" Font-Size="Smaller" ForeColor="Black" Width="100%" HeaderStyle-BackColor="#996AD3">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectmed" runat="server" OnCheckedChanged="chkSelectMed" AutoPostBack="true" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Fitness Certificate" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkdownloadfit" runat="server" Text='<%#Eval("FITCER") %>' CommandArgument='<%#Eval("FITNO") %>' OnClick="downloadfit" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Undertaking Document " ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkdownloadunder" runat="server" Text='<%#Eval("UNCER") %>' CommandArgument='<%#Eval("UNNO") %>' OnClick="downloadun" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Wellness Clinic Document" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkdownloadwcc" runat="server" Text='<%#Eval("WCC") %>' CommandArgument='<%#Eval("WCCNO") %>' OnClick="downloadwcc" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Document Validity" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lbl_medvalid" runat="server" Text='<%#Eval("validdate") %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidfit" runat="server" Value='<%#Bind("FITNO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidunder" runat="server" Value='<%#Bind("UNNO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidwcc" runat="server" Value='<%#Bind("WCCNO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidmed" runat="server" Value='<%#Bind("MEDID") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <%--<ajaxToolkit:TabPanel ID="tabpolver" runat="server" HeaderText="Police Verification"
                        TabIndex="8"><ContentTemplate><table width="100%" class="ModalTable" border="0"><tr  class="tableStyle"><td><asp:Label ID="Label5" runat="server" CssClass="lblStyleBoldWhite" Text="POLICE VERIFICATION DETAILS"></asp:Label></td></tr></table><table><tr><td><asp:Label ID="Label1" runat="server" CssClass="lblStyle" Text="Valid From"></asp:Label><span class="mandatory">*</span> </td><td><asp:TextBox ID="txtstdtpv" runat="server" CssClass="TextBoxStyle" autopostback="true">



                                            </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender5" runat="server" PopupButtonID="txtstdtpv"
                                                PopupPosition="BottomLeft" TargetControlID="txtstdtpv" Format="dd/MM/yyyy"></ajaxToolkit:CalendarExtender><ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txtstdtpv"
                                                Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false"></ajaxToolkit:MaskedEditExtender></td><td><asp:Label ID="Label2" runat="server" CssClass="lblStyle" Text="Valid To"></asp:Label><span class="mandatory">*</span> </td><td><asp:TextBox ID="txtenddtpv" runat="server" CssClass="TextBoxStyle"  EnableViewState="true" > 
                                            </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender6" runat="server" PopupButtonID="txtenddtpv"
                                                PopupPosition="BottomLeft" TargetControlID="txtenddtpv" Format="dd/MM/yyyy" ></ajaxToolkit:CalendarExtender><ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txtenddtpv"
                                                Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false" ></ajaxToolkit:MaskedEditExtender></td><td><asp:Label ID="lblpol" runat="server" CssClass="lblStyle" Text="Upload Doc." /><span style="color:red">*</span> </td><td><asp:FileUpload ID="fupdlpv" runat="server" /></td></tr><tr><td></td><td></td><td></td><td></td><td></td><td><asp:Label ID="lblcertpvname" runat="server" CssClass="lblStyle" /></td></tr><tr><td><asp:HiddenField ID="hidcertnopv" runat="server" /></td></tr></table><table><tr><td><asp:Button ID="btnsavepv" runat="server" CssClass="btnStyle" Text="Save" /></td><td><asp:Button ID="btnupdatepv" runat="server" CssClass="btnStyle" Text="Update" Visible="false" Enabled="true"  /></td></tr></table><table><asp:Label ID="LblPvMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><asp:GridView ID="gvpv" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                    Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%" HeaderStyle-Font-Size="Smaller" HeaderStyle-BackColor="#996AD3"><Columns><asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                            ItemStyle-HorizontalAlign="Center"><ItemTemplate><asp:CheckBox ID="chkSelectPV" runat="server" autopostback="true" OnCheckedChanged="chkSelectPV" /></ItemTemplate></asp:TemplateField><asp:BoundField DataField="stdt" HeaderText="Valid From" ItemStyle-Font-Size="Smaller" /><asp:BoundField DataField="enddt" HeaderText="Valid To" ItemStyle-Font-Size="Smaller" /><asp:TemplateField HeaderText="Attachment" ItemStyle-Font-Size="Smaller"><ItemTemplate><asp:LinkButton ID="lnkdownloadpv" runat="server" Text='<%#Eval("DM_NAME") %>' CommandArgument='<%#Eval("CPDT_CERT_NO") %>' onclick="downloadpv"/></ItemTemplate></asp:TemplateField><asp:TemplateField><ItemTemplate><asp:HiddenField ID="hidpvid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CPDT_PV_ID") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField><asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%"><ItemTemplate><asp:HiddenField ID="hidpvcerno" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CPDT_CERT_NO") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField><asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller"><ItemTemplate><asp:HiddenField ID="hdreqno" runat="server" value='<%#Bind("CPDT_REQ_NO") %>' /></ItemTemplate><ItemStyle CssClass="hide" /><HeaderStyle CssClass="hide" /></asp:TemplateField></Columns><AlternatingRowStyle CssClass="gvAlternatRowStyle" /><HeaderStyle CssClass="gvHeaderStyle" /><RowStyle CssClass="gvItemStyle" /></asp:GridView></table></ContentTemplate></ajaxToolkit:TabPanel>--%>

                                <ajaxToolkit:TabPanel ID="tabNominee" runat="server" HeaderText="Nominee" TabIndex="9">
                                    <ContentTemplate>
                                        <asp:Panel ID="pnlNomineeEntry" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable">
                                                <tr>
                                                    <td colspan="8" class="tableStyle">NOMINEE DETAILS </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="LblNomRelation" runat="server" CssClass="lblStyle" Text="Relation"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbNomRelation" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblNomName" runat="server" CssClass="lblStyle" Text="Nominee Name"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txtNomName" runat="server" CssClass="TextBoxStyle">
                                                        </asp:TextBox></td>
                                                    <td>
                                                        <asp:Label ID="lblNomDOB" runat="server" CssClass="lblStyle" Text="Date Of Birth"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txtNomDOB" runat="server" CssClass="TextBoxStyle">

                                                        </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender2" runat="server" PopupButtonID="txtNomDOB"
                                                            PopupPosition="BottomLeft" TargetControlID="txtNomDOB" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender7" runat="server" TargetControlID="txtNomDOB"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblNomPayGrp" runat="server" CssClass="lblStyle" Text="Payment Group"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbNomPayGrp" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblNomShare" runat="server" CssClass="lblStyle" Text="Share (In %)"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="cmbNomShare" runat="server" CssClass="ddlStyle"></asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="lblNomRemarks" runat="server" CssClass="lblStyle" Text="Remarks"></asp:Label></td>
                                                    <td>
                                                        <asp:TextBox ID="txtNomRemarks" runat="server" CssClass="TextBoxStyle" Width="200"
                                                            Height="40" TextMode="MultiLine">
                                                        </asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="lblNomineeAddress" runat="server" CssClass="lblStyle" Text="Nominee Address"></asp:Label><span class="mandatory">*</span></td>
                                                    <td>
                                                        <asp:TextBox ID="txtNomineeAddress" runat="server" CssClass="TextBoxStyle" Width="200"
                                                            Height="40" TextMode="MultiLine">
                                                        </asp:TextBox></td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <asp:Button ID="btnSaveNominee" runat="server" Text="Save" CssClass="btnStyle" Width="80" /><asp:Button ID="btnUpdateNominee" runat="server" Text="Update" CssClass="btnStyle" Visible="false"
                                                            Width="80" />
                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="tblNomErrorLst" runat="server" width="100%" class="tblErrorList"></table>
                                        </asp:Panel>
                                        <asp:Label ID="LblNomiMsg" runat="server" CssClass="lblStyle" Text=""></asp:Label><asp:Panel ID="pnlNomineeDetail" runat="server" Width="97%">
                                            <table border="0" width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:GridView ID="gvNominee" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                            Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectNominee" runat="server" OnCheckedChanged="chkSelectNominee" AutoPostBack="true" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="REL_NAME" HeaderText="Relation" />
                                                                <asp:BoundField DataField="CCN_NOMINEE_NAME" HeaderText="Nominee Name" />
                                                                <asp:BoundField DataField="CCN_NOMINEE_DOB" HeaderText="Date of Birth" />
                                                                <asp:BoundField DataField="pay_DESC" HeaderText="Payment Grp" />
                                                                <asp:BoundField DataField="CCN_SHARE" HeaderText="Share" />
                                                                <asp:BoundField DataField="ccn_remarks" HeaderText="Remarks" />
                                                                <asp:BoundField DataField="ccn_nominee_address" HeaderText="Address" />

                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidNomineeID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccn_nominee_id") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidRelationCd" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccn_relation_cd") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CCN_REQ_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidPayGrpCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccn_pymt_grp") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle BackColor="#996AD3" ForeColor="black" Font-Names="Arial" Font-Size="XX-Small" />
                                                            <RowStyle BackColor="#ccffcc" Font-Names="Arial" Font-Size="XX-Small" ForeColor="black" />
                                                            <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>

                                <ajaxToolkit:TabPanel ID="tabvaccination" runat="server" HeaderText="Vaccination Details" TabIndex="9">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panel2" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable">
                                                <tr>
                                                    <td colspan="8" class="tableStyle">Vaccination Details</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Label16" runat="server" CssClass="lblStyle" Text="Vaccination Dose"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="drp_vaccinedose" runat="server" CssClass="ddlStyle">
                                                            <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                            <asp:ListItem Value="1">First Dose</asp:ListItem>
                                                            <asp:ListItem Value="2">Second Dose</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                    <td>
                                                        <asp:Label ID="Label17" runat="server" CssClass="lblStyle" Text="Vaccine Name"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:DropDownList ID="drp_vaccinename" runat="server" CssClass="ddlStyle">
                                                            <asp:ListItem Value="0">--Select--</asp:ListItem>
                                                            <asp:ListItem Value="CO">Covaxin</asp:ListItem>
                                                            <asp:ListItem Value="CS">CoviShield</asp:ListItem>
                                                            <asp:ListItem Value="AZ">Astrazeneca</asp:ListItem>
                                                            <asp:ListItem Value="SP">Sputnik V</asp:ListItem>
                                                            <asp:ListItem Value="OT">Other</asp:ListItem>
                                                        </asp:DropDownList></td>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label18" runat="server" CssClass="lblStyle" Text="Date of first Dose:"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:TextBox ID="txt_fdose" runat="server" CssClass="TextBoxStyle">

                                                        </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender5" runat="server" PopupButtonID="txt_fdose"
                                                            PopupPosition="BottomLeft" TargetControlID="txt_fdose" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender1" runat="server" TargetControlID="txt_fdose"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                    <td>
                                                        <asp:Label ID="Label25" runat="server" CssClass="lblStyle" Text="Date of Second Dose:" /></td>
                                                    <td>
                                                        <asp:TextBox ID="txt_sdose" runat="server" CssClass="TextBoxStyle">

                                                        </asp:TextBox><ajaxToolkit:CalendarExtender ID="CalendarExtender6" runat="server" PopupButtonID="txt_sdose"
                                                            PopupPosition="BottomLeft" TargetControlID="txt_sdose" Format="dd/MM/yyyy">
                                                        </ajaxToolkit:CalendarExtender>
                                                        <ajaxToolkit:MaskedEditExtender ID="MaskedEditExtender2" runat="server" TargetControlID="txt_sdose"
                                                            Mask="99/99/9999" MaskType="None" ClearMaskOnLostFocus="false">
                                                        </ajaxToolkit:MaskedEditExtender>
                                                    </td>
                                                </tr>
                                                <tr>

                                                    <td>
                                                        <asp:Label ID="Label20" runat="server" CssClass="lblStyle" Text="Upload Vaccination Certificate"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:FileUpload ID="updt_vac" Width="140" runat="server" />
                                                        <td>
                                                            <asp:Label ID="Label23" runat="server" CssClass="lblStyle" Text="IS Vaccination Exempted?"></asp:Label>
                                                        </td>




                                                        <td>
                                                            <asp:CheckBox ID="chk_exem" runat="server" AutoPostBack="true" />
                                                        </td>
                                                        <td>
                                                            <asp:Label ID="Label22" runat="server" CssClass="lblStyle" Text="Upload Exemption Certificate"></asp:Label><span class="mandatory">*</span> </td>
                                                        <td>
                                                            <asp:FileUpload ID="updt_exemp" Width="140" runat="server" /><asp:HiddenField ID="HiddenField1"
                                                                runat="server" />
                                                        </td>
                                                </tr>
                                                <tr>
                                                    <td></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnk_vacdoc" runat="server" Text="Download Vaccination Document" Style="font: bold; font-size: 9px" Visible="false" /><asp:HiddenField ID="hdvacsrlno"
                                                            runat="server" />
                                                    </td>
                                                    </td>
                                                    <td></td>
                                                    <td></td>
                                                    <td></td>
                                                    <td>
                                                        <asp:LinkButton ID="lnk_exemp" runat="server" Text="Download Exemption Supporting Document" Style="font: bold; font-size: 9px" Visible="false" />
                                                        <asp:HiddenField ID="hd_exemp"
                                                            runat="server" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="6">
                                                        <asp:Button ID="btn_savevac" runat="server" Text="Save" CssClass="btnStyle" Width="80" /><asp:Button ID="btn_resetvac" runat="server" Text="Reset" CssClass="btnStyle" Visible="false"
                                                            Width="80" />

                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>
                                                        <br />
                                                    </td>
                                                </tr>
                                            </table>
                                            <table id="Table1" runat="server" width="100%" class="tblErrorList"></table>
                                        </asp:Panel>
                                        <asp:Label ID="Label24" runat="server" CssClass="lblStyle" Text=""></asp:Label><asp:Panel ID="Panel3" runat="server" Width="97%">
                                            <table border="0" width="100%">
                                                <tr>
                                                    <td align="center">
                                                        <asp:GridView ID="GridView1" runat="server" AllowSorting="true" AutoGenerateColumns="False"
                                                            Font-Names="Verdana" Font-Size="Small" ForeColor="Black" Width="100%">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="Select" HeaderStyle-Width="5%" ItemStyle-Width="5%"
                                                                    ItemStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectNominee" runat="server" OnCheckedChanged="chkSelectNominee" AutoPostBack="true" />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="REL_NAME" HeaderText="Relation" />
                                                                <asp:BoundField DataField="CCN_NOMINEE_NAME" HeaderText="Nominee Name" />
                                                                <asp:BoundField DataField="CCN_NOMINEE_DOB" HeaderText="Date of Birth" />
                                                                <asp:BoundField DataField="pay_DESC" HeaderText="Payment Grp" />
                                                                <asp:BoundField DataField="CCN_SHARE" HeaderText="Share" />
                                                                <asp:BoundField DataField="ccn_remarks" HeaderText="Remarks" />
                                                                <asp:BoundField DataField="ccn_nominee_address" HeaderText="Address" />

                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidNomineeID" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccn_nominee_id") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidRelationCd" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccn_relation_cd") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="" HeaderStyle-Width="5%" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Size="Smaller">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdreqno" runat="server" Value='<%#Bind("CCN_REQ_NO") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hidPayGrpCD" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "ccn_pymt_grp") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle CssClass="hide" />
                                                                    <HeaderStyle CssClass="hide" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                            <HeaderStyle BackColor="#996AD3" ForeColor="black" Font-Names="Arial" Font-Size="XX-Small" />
                                                            <RowStyle BackColor="#ccffcc" Font-Names="Arial" Font-Size="XX-Small" ForeColor="black" />
                                                            <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                                        </asp:GridView>
                                                    </td>
                                                </tr>
                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                                <ajaxToolkit:TabPanel ID="tabconsent" runat="server" HeaderText="Consent" TabIndex="10">
                                    <ContentTemplate>
                                        <asp:Panel ID="Panelconsent" runat="server" Width="100%">
                                            <table width="100%" class="ModalTable">
                                                <tr>
                                                    <td colspan="8" class="tableStyle">Consent Details</td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:LinkButton ID="download" runat="server" CssClass="lblStyle" Text="Download Consent Form"></asp:LinkButton>

                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:Label ID="Labelconsent1" runat="server" CssClass="lblStyle" Text="Upload Consent Form (Attestation is mandatory)"></asp:Label><span class="mandatory">*</span> </td>
                                                    <td>
                                                        <asp:FileUpload ID="consent_details" Width="140" runat="server" />
                                                    </td>
                                                </tr>

                                                <tr>
                                                    <td colspan="6" align="left">
                                                        <asp:Button ID="btn_saveconsent" runat="server" Text="Save" CssClass="btnStyle" Width="80" /><asp:Button ID="btnupdateconsent" runat="server" Text="Update" CssClass="btnStyle" Visible="false" Enabled="false"
                                                            Width="80" />
                                                        <label style="color: red; background-color: yellow">I hereby declare that the above furnished details and attached documents are true to the best of my knowledge.</label>

                                                    </td>
                                                </tr>


                                            </table>
                                        </asp:Panel>
                                    </ContentTemplate>
                                </ajaxToolkit:TabPanel>
                            </ajaxToolkit:TabContainer>


                            <asp:Button ID="BtnPrev" runat="server" Text="Prev" CssClass="btnStyle" Width="8%" Visible="false" />
                            <asp:Button ID="BtnNext" runat="server" Text="Next" CssClass="btnStyle" Width="8%" Visible="false" />
                            <br />

                            <asp:Panel ID="PanelEmp" runat="server" Visible="true" Width="98%" Style="height: 200px; overflow: auto;">
                                <asp:GridView ID="GridViewEmp" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    Width="97%">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SL.NO">
                                            <ItemTemplate>
                                                <%#Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Safety Pass Number">
                                            <ItemTemplate>

                                                <asp:LinkButton ID="lnk_spno" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CET_SAFETY_PASSNO") %>'
                                                    OnClick="lnk_spno_Click"> <%--  --%></asp:LinkButton>
                                                <asp:HiddenField ID="hfEmpViewDOB" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "CET_DOB") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Employee Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CET_NAME") %>'> </asp:Label>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Unique ID">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_uni" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CET_UNIQUE_ID_VALUE") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Category">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_Cat" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CET_CATEGORY") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>



                                        <asp:TemplateField HeaderText="Profile Status">
                                            <ItemTemplate>
                                                 <%-- Added By Priyaraj on 29th Feb,2024 for making the profile staus linkbutton from label--%>
                                                <asp:LinkButton ID="lbl_stat" runat="server" ForeColor="green" OnClick="emp_profile_Click"><%#DataBinder.Eval(Container.DataItem, "STATUS") %></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField HeaderText="Profile Print">
                                            <ItemTemplate>

                                                <asp:Button ID="BtnPrint" runat="server" Text="Profile Print" CssClass="btnStyle" OnClick="btnProfile_spno_Click" BackColor="#3333FF" />

                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Print Medical Form">
                                            <ItemTemplate>

                                                <asp:Button ID="btnMed" runat="server" Text="Medical Examination Form" CssClass="btnStyle" OnClick="btnMed_spno_Click" BackColor="#3333FF" />

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Document Verification Status"
                                            ItemStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_verify" runat="server" Width="90%" Text='<%#DataBinder.Eval(Container.DataItem, "VERIFY") %>' ForeColor="green"></asp:Label>
                                            </ItemTemplate>

                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Overall Status"
                                            ItemStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_verify_overall" runat="server" Width="90%" Text='<%#DataBinder.Eval(Container.DataItem, "OVER_ALL_STATUS") %>' ForeColor="green"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <%--<ItemTemplate>
                                    <asp:Button runat="server" ID="btnRejectSPReq" Text="Reject" CssClass="btnStyle rej-disabled" CommandName="REJECT_SP_REQ" CommandArgument='<%#DataBinder.Eval(Container.DataItem,"CET_SAFETY_PASSNO")%>' Enabled="false" OnClientClick="ConfirmReqReject(this,event)" />
                               </ItemTemplate>--%>
                                            <ItemTemplate>
                                                <asp:Button runat="server" ID="btnRejectSPReq" Text="Reject" CssClass="btnStyle btnGlowingStyle" CommandName="REJECT_SP_REQ" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CET_SAFETY_PASSNO")%>' Enabled="false" OnClientClick="return ConfirmReqReject(this,event)" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                    </Columns>
                                    <HeaderStyle BackColor="#3333FF" ForeColor="white" Font-Names="Arial" Font-Size="XX-Small" />
                                    <RowStyle Font-Names="Arial" Font-Size="XX-Small" BackColor="white" />
                                    <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                </asp:GridView>
                            </asp:Panel>

                            <asp:Panel ID="PanelRenew" runat="server" Visible="true" Width="98%" Style="height: 100px; overflow: auto;">
                                <asp:GridView ID="GridViewRenewEmp" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                                    Width="97%">
                                    <Columns>
                                        <asp:TemplateField HeaderText="SL.NO">
                                            <ItemTemplate>
                                                <%#Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText=" Renewal Safety Number">
                                            <ItemTemplate>

                                                <asp:LinkButton ID="lnk_Renew_spno" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CED_SAFETY_PASSNO") %>' OnClick="lnk_Renew_spno_Click"> <%--  --%></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Employee Name">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_Renew_name" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CED_NAME") %>'> </asp:Label>

                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Unique ID">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_Renew_uni" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CED_UNIQUE_ID_VALUE") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Category">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_Renew_Cat" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "CED_CATEGORY") %>'> </asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>



                                        <asp:TemplateField HeaderText="Profile Status">
                                            <ItemTemplate>
                                                 <%-- Added By Priyaraj on 29th Feb,2024 for making the profile staus linkbutton from label--%>
                                                <asp:LinkButton ID="lbl_Renew_stat" runat="server" ForeColor="green" OnClick="emp_profile_Click"><%#DataBinder.Eval(Container.DataItem, "STATUS") %></asp:LinkButton>

                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Print Medical Form">
                                            <ItemTemplate>

                                                <asp:Button ID="btnMedrenew" runat="server" Text="Medical Examination Form" CssClass="btnStyle" OnClick="btnMedRenew_spno_Click" BackColor="#3333FF" />

                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <%--  <asp:TemplateField HeaderText="Profile Print" >
                    <ItemTemplate>

                    <asp:Button ID="BtnPrint_Renew" runat="server" Text="Profile Print" CssClass="btnStyle"  OnClick="btnProfile_spno_Click" BackColor="#3333FF"/>
                
                    </ItemTemplate>
                 </asp:TemplateField>--%>


                                        <asp:TemplateField HeaderText="Police Verification Status"
                                            ItemStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_verify_Renew" runat="server" Width="90%" Text='<%#DataBinder.Eval(Container.DataItem, "VERIFY") %>' ForeColor="green"></asp:Label>
                                            </ItemTemplate>

                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Overall Status"
                                            ItemStyle-Wrap="false">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_verify_overall" runat="server" Width="90%" Text='<%#DataBinder.Eval(Container.DataItem, "OVER_ALL_STATUS") %>' ForeColor="green"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:Button runat="server" ID="btnRejectSPREReq" Text="Reject" CssClass="btnStyle btnGlowingStyle" CommandName="REJECT_SP_RE_REQ" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CED_SAFETY_PASSNO")%>' OnClientClick="return ConfirmReqReject(this,event)" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle BackColor="#3333FF" ForeColor="white" Font-Names="Arial" Font-Size="XX-Small" />
                                    <RowStyle Font-Names="Arial" Font-Size="XX-Small" BackColor="white" />
                                    <FooterStyle BackColor="#3333FF" Font-Names="Arial" Font-Size="11px" />
                                </asp:GridView>



                            </asp:Panel>
                        </asp:Panel>
                    </div>

                </div>

                <ajaxToolkit:ModalPopupExtender ID="MPopUpConfirmDocSubmision" runat="server" BackgroundCssClass="modalBackground" TargetControlID="lblDummy" PopupControlID="pnlConfirmDocSubmision">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel runat="server" ID="pnlConfirmDocSubmision" CssClass="modalPopup" Width="850px" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" Visible="false">
                    <asp:Label runat="server" ID="lblDummy"></asp:Label>
                    <asp:HiddenField runat="server" ID="hfActionPerformed" />
                    <table cellpadding="2" cellspacing="2" width="100%" style="text-align: center">
                        <tr>
                            <td>
                                <p runat="server" id="ageMessage"></p>
                                <p>Do you want to go ahead?</p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button runat="server" ID="btnConfirmDocSubmision" Text="Yes" />
                                <asp:Button runat="server" ID="btnCancelDocSubmisio" Text="No" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

                <ajaxToolkit:ModalPopupExtender ID="ModalPopupConfirmRejectReq" runat="server" BackgroundCssClass="modalBackground" TargetControlID="Label30" PopupControlID="pnlConfirmRejectReq">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel runat="server" ID="pnlConfirmRejectReq" CssClass="modalPopup" Width="550px" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" Visible="false">
                    <asp:Label runat="server" ID="Label30"></asp:Label>
                    <asp:HiddenField runat="server" ID="hndSPNo" />
                    <asp:HiddenField runat="server" ID="hndRNo" />
                    <table cellpadding="2" cellspacing="2" width="100%" style="text-align: center">
                        <tr>
                            <td>
                                <p>Are you sure,</p>
                                <p>you want reject this safety pass request?</p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button runat="server" ID="btnYes" Text="Yes" />
                                <asp:Button runat="server" ID="btnNo" Text="No" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

                 <ajaxToolkit:ModalPopupExtender ID="ModalPopupConfirmReapplyReq" runat="server" BackgroundCssClass="modalBackground" TargetControlID="Label31" PopupControlID="pnlConfirmReapplyReq">
                </ajaxToolkit:ModalPopupExtender>
                <asp:Panel runat="server" ID="pnlConfirmReapplyReq" CssClass="modalPopup" Width="550px" BorderStyle="Solid" BorderColor="Black" BorderWidth="1px" Visible="false">
                    <asp:Label runat="server" ID="Label31"></asp:Label>
                    <%--<asp:HiddenField runat="server" ID="hndrepplyYes" />
                    <asp:HiddenField runat="server" ID="hndreapplyNo" />--%>
                    <table cellpadding="2" cellspacing="2" width="100%" style="text-align: center">
                        <tr>
                            <td>
                                <p>You can reapply the safety pass no now. </p>
                                <p>Still Do you want to continue to Reject ?</p>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button runat="server" ID="btnrepplyYes" Text="Yes" />
                                <asp:Button runat="server" ID="btnrepplyNo" Text="No" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>


                <asp:Label ID="lblPFESI" runat="server" Style="display: none;"></asp:Label>
                <ajaxToolkit:ModalPopupExtender ID="mpconfirmsubmit" runat="server" TargetControlID="lblPFESI"
                    PopupControlID="pnlconfirmsubmit" BackgroundCssClass="modalBackground" DropShadow="true"
                    PopupDragHandleControlID="dragSubVendor" RepositionMode="RepositionOnWindowResizeAndScroll"
                    Drag="true" OkControlID="ibtnClosesubmit" />
                <asp:Panel ID="pnlconfirmsubmit" runat="server" CssClass="modalPopup" Style="display: none"
                    Width="500px">
                    <table style="background-color: lightgray" width="100%">
                        <tr align="center">
                            <td>Capturing Of UAN and ESIC Number
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td colspan="2">
                                <asp:Label ID="lblpfesiErrMsg" ForeColor="Red" runat="server" Text="" CssClass="lblErrorMsg" Font-Bold="true"></asp:Label>
                            </td>
                            <tr>
                                <td>
                                    <asp:Label ID="lbluan" runat="server" Text="Fill UAN Number(under EPFO Act)[12 digit]:" /></td>
                                <td>
                                    <asp:TextBox ID="txtuan" AutoPostBack="true" runat="server" CssClass="TextBoxUpperCase" MaxLength="12"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label5" runat="server" Text="Fill IP Number(under ESIC Act)[10 digit]:" />
                                </td>
                                <td>
                                    <asp:TextBox ID="txtip" runat="server" AutoPostBack="true" CssClass="TextBoxUpperCase" MaxLength="10"></asp:TextBox>
                                </td>
                            </tr>
                            <tr align="center">
                                <td>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>
                                <td>
                                    <asp:Button ID="ibtnCloseconfirmsubmit" runat="server" CssClass="btnStyle" Enabled="true" Text="Save" />
                                    <asp:Button ID="ibtnClosesubmit" runat="server" CssClass="btnStyle" Text="Cancel" />
                                </td>
                            </tr>
                        </tr>
                    </table>
                 </asp:Panel>

                <%--<div id="myModal" class="modal">
            <div class="modal-content">
                <span class="close" onclick="toggleModal()">&times;</span>
                <img src="Images/Wireframe_pic1.PNG alt="Image" style="width:100%">
            </div>
        </div>
                <div id="myModalAG" class="modal">
            <div class="modal-content">
                <span class="close" onclick="toggleModalAG()">&times;</span>
                <img src="Images/Wireframe_pic2.PNG" alt="Image" style="width:100%">
            </div>--%>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="tabcontainer1$tabQualification$btnsaveQual" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabQualification$btnupdateQual" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabEXP$btnsaveexp" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabEXP$btnupdateexp" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabSkill$btnsaveskill" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabSkill$btnupdateskill" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabTraining$btnsaveTraining" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabTraining$btnupdateTraining" />
                <%--<asp:PostBackTrigger ControlID ="tabcontainer1$tabpolver$btnsavepv" /> --%>
                <%-- <asp:PostBackTrigger ControlID ="tabcontainer1$tabpolver$btnupdatepv" /> --%>
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAddress$btnsaveaddress" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAddress$btnupdateaddress" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabQualification$gvqualification" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAddress$gvaddress" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabTraining$gvTraining" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabSkill$gvskill" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabEXP$grvexp" />
                <%--  <asp:PostBackTrigger ControlID ="tabcontainer1$tabpolver$gvpv" />--%>
                <%--<asp:asyncPostBackTrigger ControlID ="tabcontainer1$tabpolver$txtenddtpv" />--%>
                <asp:PostBackTrigger ControlID="tabcontainer1$tabage$btnsaveage" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabage$btnupdateage" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabage$grdage" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabmedical$btnsavemed" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabmedical$btnupdatemed" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabvaccination$btn_savevac" />

                <asp:PostBackTrigger ControlID="tabcontainer1$tabmedical$gvmed" />
                <asp:PostBackTrigger ControlID="btn_downloadnoti" />
                <asp:PostBackTrigger ControlID="gridviewemp" />
                <asp:PostBackTrigger ControlID="gridviewrenewemp" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabvaccination$lnk_vacdoc" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabvaccination$lnk_exemp" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAddress$imgaddressold" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAge$imbageold" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAge$imbdriverold" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabAge$imgpassold" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabSkill$imgskillold" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabconsent$download" />
                <asp:PostBackTrigger ControlID="tabcontainer1$tabconsent$btn_saveconsent" />
            </Triggers>
        </asp:UpdatePanel>



    </center>
    <div id="div1" style="display: none; filter: progid:DXImageTransform.Microsoft.gradient(enabled='true',startColorstr='Navy', endColorstr='White',gradientType='1')"
        class="divStyle">
        <table class="tableStyle" style="background: lightgray">
            <tr>
                <td style="background-color: Gray">Message
                </td>
            </tr>
            <tr>

                <td>
                    <input type="text" name="mymsg" style="width: 400px; height: 150px" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <input name="hidCtrlName" type="hidden" />
                    <input name="btnOK" value="OK" type="button" onclick="Update_Close_Div();" class="btnStyle" />
                    <input name="Close" value="Close" type="button" onclick="Close_Div();" class="btnStyle" />
                </td>
            </tr>
        </table>
    </div>
    <%--- (23/02/2024) TCS.2164315 Scripts for message box for the popup of reapply status message. --%>
    <script type="text/javascript" src="./Javascript/jquery.min.js"></script>

    <script type="text/javascript">
        //        $(function() { $("#div1").draggable(); });
        function ConfirmReqReject(sender, e) {
            if (!confirm('Are you sure, you want reject this safety pass request?')) {
                //              e.preventDefault();
                return false;
            }
        }

        // WI6447 ADD by prasun chakraborty 24122021

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function () {
            RegisterFunction();
        });

        window.onload = () => {
            RegisterFunction();
        }

        function RegisterFunction() {
            try {
                const myInput = document.getElementById('<%=txt_WAIVE_DAYS.ClientID%>');
                myInput.addEventListener('paste', e => e.preventDefault());
            } catch (e) {

            }

        }

        function WaivDaysKeyPress(obj) {

            var e = event || evt;
            var charCode = e.which || e.keyCode;
            var rtn = false;
            if (obj.value.length < 3)
                rtn = true;
            else
                rtn = false;
            if (rtn == true) {
                if (charCode > 31 && (charCode < 48 || charCode > 57))
                    rtn = false;
                else
                    rtn = true;
            }
            return rtn;
        }
        //end add by prasun chakraborty
    </script>
</asp:Content>
