Imports System.Data
Imports System.Data.OracleClient
Imports System.Globalization
Imports System.Drawing
Imports System.Net
Imports System.IO
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.html.simpleparser
Imports iTextSharp.text.pdf.draw
Imports System.Net.Mail
Imports System.Web.Services
Imports System.Web.Script.Services


'**********************************************************************************************
'* Created By: Anand kumar
'* Created On: 10 January,2016
'* Modify By: Anand kumar
'* Modify On: 15 April,2021
'* Department: ITS, Tata Steel
'* 
'* Purpose: This page is used for profile Entry of contract employee of logged in Vendor .
'* Modification  ----------------------------------------------------------------------------------  
'*      CMR No                 Date          Modification reason
'* 1) 2016/01/16/J9/T2        21/01/2016     change global class from CLMclass To CLMVendClass
'* 2)2016/01/16/J19/T2        08/02/2016     get the safety pass number for renewal process based on location wise.  
'* 3)2016/04/91/J11/T6        26/05/2016     Provision to stop adding Employees under expired request number .
'* 4)2016/04/91/J28/T1        08/07/2016     Apply for training renewal 60 days before expiry date of Safety pass. 
'* 5)2016/04/91/J28/T1        08/07/2016     After Adding workmen details,  workmen  category should be count from correct applied category.
'* 6)2016/04/91/J28/T1        08/07/2016     A alert message should be given when safety pass blocked and vendor try to applied for renewal.
'* 7)2020/09/38/J312/T1       02/03/2021     Enhancement by applying intrgration logic with JNTVTI certification.
'* 8)****/**/**/****/**       15/04/2021     JNVTI certification NTTF safety test and Medical test
'9) WI2259: Allow skill entry during SP new/renewal and also allow to attach exemption certificate, created  By: Avik Mukherjee, Created Date: 02-AUG-2021
'10)WI2689: Allow Vendor partner to enter vaccination detauils during safety pass new / renewal, created by : Avik Mukherjee, created on: 18-Aug-2021
'11) WI4247: Allow vendor to apply renewal of safety pass max of 90 days prior based on company location, created by: Avik Mukherjee, Created on: 07-Oct-2021
'12) WI5073: Restrict vendor not to allow Skill waiver off for which skill waiver off already taken in previous new/renewal of safety pass
'13) WI6447: Add Waive off Days to allow without JNTVTI Certification for the specific waive off days created by: Prasun Chakraborty, Created on: 05-Jan-2022
'14) WI9047: Enhancement in page to allow safety pass related data to be eligible during profile creation for new cases, created by: Avik Mukherjee, created on: 15-APR-2022
'  15.)     -                  11/05/2022      PD Screen Enhancement of CLM
'      (Modification by Souvik Chakraborty)
'  DATED:   22/12/2022   Anand Kumar         CMR :WI15268                        Addition of address related columns (Village, PO, Thana,  Dist., State, PIN  )in Database and in UI while profile is getting created

'***************************************************************************************************
Partial Class ospEmpDetails
    Inherits System.Web.UI.Page
    Dim con As New OracleConnection(ConfigurationManager.ConnectionStrings("OraConnGatepass").ConnectionString)
    Dim AES As AESEncryption
    Public Const ENCRYPT_DECRYPT_KEY As String = "1L0tu+LQ1ux$c@P9"
    'THE KEY Is USED For ENCRYPTING PII DATA, IT Is SAME For QA And PRD WHICH Is DEIFFERENT FROM DEV. In Case Of MISMATCH APPLICATION MAY Not WORK PROPERLY

    Dim clmClass As New CLMVendClass()       'changed from CLMclass to CLMVendClass
    Dim comp_cd As String = ""
    Dim vVencode As String = ""
    Dim Loc As String = ""
    Dim vLocCd As String = ""
    Dim err_tr As HtmlTableRow
    Dim err_cnt As Int16 = 0
    Dim WR As String = ""
    Dim SV As String = ""
    Dim VC As String = ""
    Dim DV As String = ""
    Dim FM As String = ""

    Dim SH As String = ""
    Dim SF As String = ""

    'sandeep
    Dim WA As String = ""
    Dim SA As String = ""
    Dim VA As String = ""
    Dim DA As String = ""
    Dim FA As String = ""
    Dim DH As String = ""

    Dim WA_desc As String = ""
    Dim SA_desc As String = ""
    Dim VA_desc As String = ""
    Dim DA_desc As String = ""
    Dim FA_desc As String = ""
    'end


    Dim WR_desc As String = ""
    Dim SV_desc As String = ""
    Dim VC_desc As String = ""
    Dim DV_desc As String = ""
    Dim FM_desc As String = ""
    Dim SH_desc As String = ""
    Dim SF_desc As String = ""
    Dim DH_desc As String = ""

    Dim SPN As String = ""
    Dim SPR As String = ""


    Dim msg_incomp As String = ""
    Dim msg_complete As String = ""
    Dim msg_reject As String = ""
    Dim msg_incomp_val As String = ""
    Dim msg_complete_val As String = ""
    Dim msg_reject_val As String = ""
    Dim locationCode As String = ""
    Dim location As String = ""
    Dim vendorCode As String = ""
    Dim category As String = ""
    Dim dept As String = ""
    Dim firstname As String = ""
    Dim lastname As String = ""
    Dim fatherName As String = ""
    Dim spouse As String = ""
    Dim gender As String = ""
    Dim emergencyNo As String = ""
    Dim phoneNo As String = ""
    Dim bloodGroup As String = ""
    Dim uniqueIDVal As String = ""
    Dim uniqueIDType As String = ""
    Dim identityMark As String = ""
    Dim areaofWork As String = ""
    Dim birthAge As String = ""
    Dim dob As String = ""
    Dim FullAddress As String = ""
    Dim address1 As String = ""
    Dim address2 As String = ""
    Dim address3 As String = ""
    Dim country As String = ""
    Dim country_name As String = ""
    Dim qualification As String = ""
    Dim profile_status As String = ""
    Dim verify_status As String = ""
    Dim dobcertno As String = ""
    Dim drvcertno As String = ""
    Dim passcertno As String = ""
    Dim affirmative As String = ""
    Dim UAN As String = ""
    Dim IP As String = ""
    Dim TradeIrisDataPresent As Boolean = "false"
    '-------------------------Souvik Begins 3
    Dim s_ctm_code As String = ""
    Shared dtTradList As New DataTable
    '--------------------------Souvik Ends 3

    'OSJ2756 -  Updated all the ddlSkillTrade => SelectedValue, selectedindex etc has been replaced with the text, 

    ''' <summary>
    ''' Added reject message label control visible set as false.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Session("Comp_code") = "1000"
        Session("VendCode") = "BC18"

        Rejmsg.Visible = False
        cmbCategory.Enabled = True
        For Each li As WebControls.ListItem In cmbCategory.Items
            li.Enabled = True
        Next
        Dim decode_ReqNo As String = ""
        If Page.Request.QueryString("ReqNo") <> "" Then
            decode_ReqNo = b64decode(Page.Request.QueryString("ReqNo"))
        End If

        If Session("Comp_code") = "9501" Or Session("Comp_code") = "9502" Or Session("Comp_code") = "9500" Then
            Session("comp_name_d") = "Jamipol"
        Else
            Session("comp_name_d") = "Tata Steel"
        End If


        Dim reqNo = decode_ReqNo

        If Session("VendCode") <> "" Then
            vVencode = Session("VendCode")
            comp_cd = Session("Comp_code")
            'If txtstdtpv.Text = "__/__/____" Or txtstdtpv.Text = "" Then
            'Else
            '    txtenddtpv.Text = futureDate(txtstdtpv.Text)
            'End If
        Else
            Response.Redirect("CLMHome.aspx")
            Exit Sub
        End If

        If Session("VendCode") <> "" Then

            ''req_detail()
            employeeType()
            ReqType()

            'profile
            vLocCd = GetLocationName(comp_cd)

        Else
            Response.Redirect("http://tatasteel.co.in/")
        End If

        If Not IsPostBack Then
            Try
                populatewaiveoffReason()

                ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where acm_type='SKJNTVTI' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    chk_waive.Visible = True
                    drp_waiveoff.Visible = True
                    lbl_waiveoff.Visible = True
                    lbl_waivereason.Visible = True
                    drptypeassessment.Enabled = True
                    spn_msg.Visible = True
                    drptypeassessment.Visible = True
                    Label2.Visible = True
                    spn_type.Visible = True

                Else
                    chk_waive.Visible = False
                    drp_waiveoff.Visible = False
                    lbl_waiveoff.Visible = False
                    lbl_waivereason.Visible = False
                    chk_waive.Checked = True
                    drptypeassessment.Enabled = False
                    spn_msg.Visible = False
                    drptypeassessment.Visible = False
                    Label2.Visible = False
                    spn_type.Visible = False

                End If

            Catch ex As Exception

            End Try
            req_detail()
            If reqNo <> "" Then
                ReqClick(reqNo)
                reqNo = ""
            End If

            'profile
            '   vLocCd = GetLocationName(comp_cd)

            GetAreaOfWork(cmbWorkArea, "AOW")
            FillDropDown(cmbAffirmative, "AFRM") ' TO Fill Affirmative Drop down
            FillDropDown(cmbUniqID, "ICAD")

            'Address
            GetAddressType()
            GetCountry()
            cmbAddCountry.SelectedValue = "IND"


            GetState()
            cmbAddState.SelectedValue = "JH"
            GetCity(cmbAddState.SelectedValue)

            GetDistrict(cmbAddState.SelectedValue)

            'Qualification 
            FillDropDown(cmbQualType, "QTYP")

            'Nominee 
            GetShare()
            GetRelation()
            GetPaymentGrp()
            getExpDom()
            getExpLocState()
            GetSkillType()
            'getSkillTrade()
            GetTrainingLocation()
            FillDropDown(cmbTraningType, "TRNG")
            Dim sqlAgency As String = "select * from t_Cemp_Type_Master where CTM_TYPE ='AGEN' and CTM_STATUS='A' AND (CTM_VALUE IS NULL OR CTM_VALUE='" + Session("Comp_Code") + "') order by CTM_SEQ"
            'Change by anand on 20170427 End ***
            Dim dtAgency As New DataTable()
            dtAgency = getRecord(sqlAgency, con)
            cmbTrnAgency.Items.Clear()
            If dtAgency.Rows.Count > 0 Then
                cmbTrnAgency.DataSource = dtAgency
                cmbTrnAgency.DataTextField = "CTM_TYPE_DESC"
                cmbTrnAgency.DataValueField = "CTM_TYPE_CODE"
                cmbTrnAgency.DataBind()
                cmbTrnAgency.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
            End If
            'FillDropDown(cmbTrnAgency, "AGEN")
            FillDropDown(cmbTrnResult, "RSLT")

            'START ADD BY PRASUN ON 11032022
            ActiveControlsForFormA()

            'END ADD BY PRASUN ON 11032022            

        End If

        If ddlSkillTrade.Text.Trim().Contains("-") = False Then
            ddlSkillTrade.Text = ddlSkillTrade.Text.Trim() + "-"
        End If


    End Sub
    ''' <summary>
    ''' Service method for all trade in textbox.
    ''' </summary>
    ''' <param name="prefixText"></param>
    ''' <param name="count"></param>
    ''' <returns></returns>
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetTradeName(ByVal prefixText As String, ByVal count As Int16) As String()  'OSJ2756 - Added a New Function
        Dim tradList As String = ""
        Dim ob As New ospEmpDetails
        'tradList += "select CTM_TYPE_CODE, CTM_TYPE_DESC, (CTM_TYPE_DESC || ' - ' || CTM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTD' AND CTM_STATUS='A' AND ROWNUM <= 20 ORDER BY CTM_SEQ"
        'tradList += "select CTM_TYPE_CODE, CTM_TYPE_DESC, (CTM_TYPE_DESC || ' - ' || CTM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTD' AND CTM_STATUS='A' ORDER BY CTM_SEQ"
        dtTradList = ob.getAllSkillTrade()
        'dtTradList = ob.getRecord(tradList, ob.con)
        Dim random As New Random()
        Dim items(dtTradList.Rows.Count - 1) As String
        For i As Integer = 0 To dtTradList.Rows.Count - 1
            items(i) = dtTradList.Rows(i).Item(0).ToString + "-" + dtTradList.Rows(i).Item(1).ToString
        Next
        Return (From m In items Where m.ToUpper.Contains(prefixText.ToUpper) Select m).ToArray()
    End Function

    ''' <summary>
    ''' Service Method for display the trade in checkbox in which the trade name is present in IRIS of safety pass number.
    ''' </summary>
    ''' <param name="prefixText"></param>
    ''' <param name="count"></param>
    ''' <returns></returns>
    <WebMethod(EnableSession:=True)>
    <Script.Services.ScriptMethod()>
    Public Shared Function GetTradeNameIris(ByVal prefixText As String, ByVal count As Int16) As String()  'OSJ2756 - Added a New Function
        Dim tradList As String = ""
        Dim ob As New ospEmpDetails
        'tradList += "select CTM_TYPE_CODE, CTM_TYPE_DESC, (CTM_TYPE_DESC || ' - ' || CTM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTD' AND CTM_STATUS='A' AND ROWNUM <= 20 ORDER BY CTM_SEQ"
        'tradList += "select CTM_TYPE_CODE, CTM_TYPE_DESC, (CTM_TYPE_DESC || ' - ' || CTM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTD' AND CTM_STATUS='A' ORDER BY CTM_SEQ"
        dtTradList = ob.getSkillTrade()
        'dtTradList = ob.getRecord(tradList, ob.con)

        Dim random As New Random()
        Dim items(dtTradList.Rows.Count - 1) As String
        For i As Integer = 0 To dtTradList.Rows.Count - 1
            items(i) = dtTradList.Rows(i).Item(0).ToString + "-" + dtTradList.Rows(i).Item(1).ToString
        Next
        Return (From m In items Where m.ToUpper.Contains(prefixText.ToUpper) Select m).ToArray()
    End Function
    Private Sub populatewaiveoffReason()
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        ls_sql = "select '0' val,'--Select--' des from dual union select ctm_TYPE_DESC val,CTM_TYPE_DESC des from hrace.t_cemp_type_master t1 where t1.ctm_type='SKW' and substr(t1.CTM_TYPE_CODE,5,4)=:ACM_COMPANY_CODE"
        cmd = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
        dt = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            drp_waiveoff.DataSource = dt
            drp_waiveoff.DataValueField = "val"
            drp_waiveoff.DataTextField = "des"
            drp_waiveoff.DataBind()
        End If
    End Sub
    'Protected Sub Page_UnLoad(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
    '    If Session("polverenddt") Is Nothing Then

    '    Else
    '        txtenddtpv.Text = Session("polverenddt")
    '    End If

    'End Sub
    Public Sub GetTrainingLocation()
        Dim cmd As New OracleCommand()
        Dim dtLocation As New DataTable()
        Dim sqlLocation As String = " select * from T_LOCATION_MASTER where LOC_LOCATION_CODE in (select CMP_LOC_CD from T_COMPANY_MASTER where CMP_COMPANY_CODE=:CMP_COMPANY_CODE)"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmd.Connection = con
        cmd.CommandText = sqlLocation
        cmd.Parameters.Add(New OracleParameter(":CMP_COMPANY_CODE", Session("Comp_code")))
        Dim da = New OracleDataAdapter(cmd)
        da.Fill(dtLocation)
        If con.State = ConnectionState.Open Then
            con.Close()

        End If
        cmbTrnLoc.Items.Clear()
        If dtLocation.Rows.Count > 0 Then
            cmbTrnLoc.DataSource = dtLocation
            cmbTrnLoc.DataTextField = "LOC_LOCATION_NAME"
            cmbTrnLoc.DataValueField = "LOC_LOCATION_CODE"
            cmbTrnLoc.DataBind()
            cmbTrnLoc.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
            cmbTrnLoc.Items.Insert(1, New WebControls.ListItem("OTHER", "Othr"))
        End If
    End Sub
    Public Sub GetSkillType()
        Dim sqlSkillType As String = "select * from t_Cemp_Type_Master where CTM_TYPE ='SKIL' and CTM_STATUS='A' order by CTM_SEQ"
        Dim dtSkillType As New DataTable()
        dtSkillType = getRecord(sqlSkillType, con)
        cmbSkSkillType.Items.Clear()
        If dtSkillType.Rows.Count > 0 Then
            cmbSkSkillType.DataSource = dtSkillType
            cmbSkSkillType.DataTextField = "CTM_TYPE_DESC"
            cmbSkSkillType.DataValueField = "CTM_TYPE_CODE"
            cmbSkSkillType.DataBind()
            cmbSkSkillType.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))


        End If
    End Sub

    Public Sub ShowhideRenewSkillCert()
        Dim sqlTradeLOCOSCCheck As String = "select * from t_cwm_action_mapping where ACM_TYPE = 'LOCOSC' and ACM_FLAG = 'Y' and ACM_COMPANY_CODE in (" + Session("Comp_code") + ")"
        Dim dtTradeLOCOSCCheck As New DataTable()
        Try
            dtTradeLOCOSCCheck = getRecord(sqlTradeLOCOSCCheck, con)
            If (dtTradeLOCOSCCheck.Rows.Count() > 0 And Session("requestType") = "SPR") Then
                renewalSkillTR.Visible = True
                Session("renewSkillCert") = ddlScfr.SelectedValue
            Else
                renewalSkillTR.Visible = False
                Session("renewSkillCert") = ""
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub ddlScfr_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlScfr.SelectedIndexChanged
        Session("renewSkillCert") = ddlScfr.SelectedValue
    End Sub
    Protected Sub cmbSkSkillType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbSkSkillType.SelectedIndexChanged
        Dim vSkillType As String = ""

        vSkillType = cmbSkSkillType.SelectedValue
        Session("spno") = TxtSpno.Text

        Dim sqlTradeIRISCheck As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO='" + TxtSpno.Text + "' AND TCD_CERT_CATEG<>'FAIL'"
        Dim dtTradeIRISCheck As New DataTable()
        dtTradeIRISCheck = getRecord(sqlTradeIRISCheck, con)
        If (dtTradeIRISCheck.Rows.Count() > 0) Then
            ddlSkillTrade_AutoCompleteExtender.MinimumPrefixLength = "1"
            ddlSkillTrade_AutoCompleteExtender.ServiceMethod = "GetTradeNameIris"
            LabelAllTrade.Visible = True
            CheckBoxAllTrade.Visible = True
        End If
        'getSkillTrade()
        GetTraningSkillCD(vSkillType)
    End Sub

    Protected Sub drptypeassessment_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drptypeassessment.SelectedIndexChanged

        Session("assesment_type_selected") = drptypeassessment.SelectedValue
        ddlSkillTrade.Text = "-"

    End Sub

    ''' <summary>
    ''' Function of the check box of all trade code option in check changed event of include all trade code checkbox
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub CheckBoxAllTrade_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckBoxAllTrade.CheckedChanged
        'If(CheckBoxAllTrade.Checked) Then
        '    getAllSkillTrade()
        '     else
        '    getSkillTrade()
        'End If
        If (CheckBoxAllTrade.Checked) Then

            ddlSkillTrade_AutoCompleteExtender.MinimumPrefixLength = "1"
            ddlSkillTrade_AutoCompleteExtender.ServiceMethod = "GetTradeName"

        Else
            ddlSkillTrade_AutoCompleteExtender.MinimumPrefixLength = "1"
            ddlSkillTrade_AutoCompleteExtender.ServiceMethod = "GetTradeNameIris"
            LabelAllTrade.Visible = True
            CheckBoxAllTrade.Visible = True
        End If

    End Sub
    ''' <summary>
    ''' Function to get all skill trade codes and populated in trade textbox.
    ''' </summary>
    Public Function getAllSkillTrade() As DataTable
        Dim spno As String = Session("spno")
        Dim ReqNo As String = ""
        Dim CompCode As String = Session("Comp_code")
        If Session("requestnumber") <> "" Then
            ReqNo = Session("requestnumber")
        End If
        Dim assesment_type As String = "0"
        If Session("assesment_type_selected") <> "" Then
            assesment_type = Session("assesment_type_selected")
        End If
        Dim sqlTradeLOCOSCCheck As String = "select * from t_cwm_action_mapping where ACM_TYPE = 'LOCOSC' and ACM_FLAG = 'Y' and ACM_COMPANY_CODE = '" + CompCode + "' "
        Dim dtTradeLOCOSCCheck As New DataTable()
        Dim dtSkillTrade As New DataTable()
        Try
            dtTradeLOCOSCCheck = getRecord(sqlTradeLOCOSCCheck, con)
            If (dtTradeLOCOSCCheck.Rows.Count() > 0 And Session("requestType") = "SPN") Then
                Dim sqlSkillTrade As String = "select SPM_TYPE_CODE CTM_TYPE_CODE, SPM_TYPE_DESC CTM_TYPE_DESC, (SPM_TYPE_DESC || ' - ' || SPM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_SKILL_PAY_MASTER where SPM_STATUS='A' and SPM_LOCATION = '" + CompCode + "' and SPM_ASSESSMENT_TYPE = '" + assesment_type + "' ORDER BY SPM_TYPE_DESC"
                Dim ob As New ospEmpDetails
                dtSkillTrade = ob.getRecord(sqlSkillTrade, ob.con)
            ElseIf (dtTradeLOCOSCCheck.Rows.Count() > 0 And Session("requestType") = "SPR" And Session("renewSkillCert") = "Yes") Then
                Dim sqlSkillTrade As String = "select SPM_TYPE_CODE CTM_TYPE_CODE, SPM_TYPE_DESC CTM_TYPE_DESC, (SPM_TYPE_DESC || ' - ' || SPM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_SKILL_PAY_MASTER where SPM_STATUS='A' and SPM_LOCATION = '" + CompCode + "' and SPM_ASSESSMENT_TYPE = '" + assesment_type + "' ORDER BY SPM_TYPE_DESC"
                Dim ob As New ospEmpDetails
                dtSkillTrade = ob.getRecord(sqlSkillTrade, ob.con)
            Else
                Dim sqlSkillTrade As String = "select CTM_TYPE_CODE, CTM_TYPE_DESC, (CTM_TYPE_DESC || ' - ' || CTM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTD' AND CTM_STATUS='A' ORDER BY CTM_SEQ"
                Dim ob As New ospEmpDetails
                dtSkillTrade = ob.getRecord(sqlSkillTrade, ob.con)

            End If
        Catch ex As Exception

        End Try

        'ddlSkillTrade.Items.Clear()
        'If dtSkillTrade.Rows.Count > 0 Then
        '    ddlSkillTrade.DataSource = dtSkillTrade
        '    ddlSkillTrade.DataTextField = "CTM_TYPE_DESC"
        '    ddlSkillTrade.DataValueField = "CTM_TYPE_CODE"
        '    ddlSkillTrade.DataBind()
        '    ddlSkillTrade.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        'End If
        Return dtSkillTrade
    End Function
    ''' <summary>
    ''' Function to get the skill trade code in which the trade data is present in IRIS of safety pass number and populated in trade textbox.
    ''' </summary>
    Public Function getSkillTrade() As DataTable
        Dim dtSkillTrade As New DataTable()
        Dim spno As String = Session("spno")
        Dim ReqNo As String = ""
        Dim CompCode As String = Session("Comp_code")
        If Session("requestnumber") <> "" Then
            ReqNo = Session("requestnumber")
        End If
        Dim assesment_type As String = "0"
        If Session("assesment_type_selected") <> "" Then
            assesment_type = Session("assesment_type_selected")
        End If
        If (spno.ToString() <> "") Then
            Dim sqlTradeIRISCheck As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO='" + spno + "' AND TCD_CERT_CATEG<>'FAIL'"
            Dim dtTradeIRISCheck As New DataTable()
            Try
                dtTradeIRISCheck = getRecord(sqlTradeIRISCheck, con)
                If (dtTradeIRISCheck.Rows.Count() > 0) Then

                    Dim sqlTradeLOCOSCCheck As String = "select * from t_cwm_action_mapping where ACM_TYPE = 'LOCOSC' and ACM_FLAG = 'Y' and ACM_COMPANY_CODE = '" + CompCode + "' "
                    Dim dtTradeLOCOSCCheck As New DataTable()
                    Try
                        dtTradeLOCOSCCheck = getRecord(sqlTradeLOCOSCCheck, con)
                        If (dtTradeLOCOSCCheck.Rows.Count() > 0 And Session("requestType") = "SPN") Then
                            Dim sqlSkillTrade As String = "select SPM_TYPE_CODE CTM_TYPE_CODE, SPM_TYPE_DESC CTM_TYPE_DESC, (SPM_TYPE_DESC || ' - ' || SPM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_SKILL_PAY_MASTER where SPM_STATUS='A' and SPM_LOCATION = '" + CompCode + "' and SPM_ASSESSMENT_TYPE = '" + assesment_type + "' ORDER BY SPM_TYPE_DESC"
                            dtSkillTrade = getRecord(sqlSkillTrade, con)
                        ElseIf (dtTradeLOCOSCCheck.Rows.Count() > 0 And Session("requestType") = "SPR" And Session("renewSkillCert") = "Yes") Then
                            Dim sqlSkillTrade As String = "select SPM_TYPE_CODE CTM_TYPE_CODE, SPM_TYPE_DESC CTM_TYPE_DESC, (SPM_TYPE_DESC || ' - ' || SPM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_SKILL_PAY_MASTER where SPM_STATUS='A' and SPM_LOCATION = '" + CompCode + "' and SPM_ASSESSMENT_TYPE = '" + assesment_type + "' ORDER BY SPM_TYPE_DESC"
                            dtSkillTrade = getRecord(sqlSkillTrade, con)
                        Else
                            Dim sqlSkillTrade As String = "select CTM_TYPE_CODE, CTM_TYPE_DESC, (CTM_TYPE_DESC || ' - ' || CTM_TYPE_CODE) as CTM_TYPE_DESC_2 from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTD' AND CTM_STATUS='A' AND CTM_TYPE_CODE IN(SELECT TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO='" + spno + "' AND TCD_CERT_CATEG<>'FAIL') ORDER BY CTM_SEQ"
                            dtSkillTrade = getRecord(sqlSkillTrade, con)

                        End If
                    Catch ex As Exception

                    End Try
                    'ddlSkillTrade.Items.Clear()
                    If dtSkillTrade.Rows.Count > 0 Then
                        'ddlSkillTrade.SelectedValue = Nothing
                        'ddlSkillTrade.DataSource = dtSkillTrade
                        'ddlSkillTrade.DataTextField = "CTM_TYPE_DESC"
                        'ddlSkillTrade.DataValueField = "CTM_TYPE_CODE"
                        'ddlSkillTrade.DataBind()
                        'ddlSkillTrade.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))   
                        TradeIrisDataPresent = True
                    End If
                Else
                    dtSkillTrade = getAllSkillTrade()
                    TradeIrisDataPresent = False
                End If
            Catch ex As Exception
                dtSkillTrade = getAllSkillTrade()
                TradeIrisDataPresent = False
            End Try
        Else
            dtSkillTrade = getAllSkillTrade()
            TradeIrisDataPresent = False
        End If
        Return dtSkillTrade
    End Function
    Public Sub getSkillAssessment()
        If ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) = "SKTD0028" Then
            'If ddlSkillTrade.SelectedValue = "SKTD0028" Then
            Dim sqlSkillTrade As String = "select CTM_TYPE_CODE, CTM_TYPE_DESC from T_CEMP_TYPE_MASTER where CTM_TYPE='SKTP' AND CTM_STATUS='A' and CTM_VALUE='" + cmbSkSkill.SelectedValue + "'  ORDER BY CTM_SEQ "
            Dim dtskillassessement As New DataTable()
            dtskillassessement = getRecord(sqlSkillTrade, con)
            drp_skillassess.Items.Clear()
            If dtskillassessement.Rows.Count > 0 Then
                drp_skillassess.Visible = True
                drp_skillassess.DataSource = dtskillassessement
                drp_skillassess.DataTextField = "CTM_TYPE_DESC"
                drp_skillassess.DataValueField = "CTM_TYPE_CODE"
                drp_skillassess.DataBind()
                drp_skillassess.Items.Insert(0, New WebControls.ListItem("Select", "NA"))
            Else
                drp_skillassess.DataSource = Nothing
                'drp_skillassess.DataTextField = "CTM_TYPE_DESC"
                'drp_skillassess.DataValueField = "CTM_TYPE_CODE"
                drp_skillassess.DataBind()
                drp_skillassess.Visible = False
            End If
        End If

    End Sub

    'Protected Sub ddlSkillTrade_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSkillTrade.SelectedIndexChanged
    '    Dim vSkilledTrades As String = ""
    '    vSkilledTrades = ddlSkillTrade.SelectedValue
    '    Dim ls_sql As String = String.Empty
    '    If (vSkilledTrades = "SKTD0029") Then
    '        lblOthSkillTrade.Visible = True
    '        txtOthSkillTrade.Visible = True
    '        drp_skillassess.Visible = False
    '        lblSkillassess.Visible = False
    '        ddlSKAss.SelectedValue = "Yes"
    '        ddlSKAss.Enabled = True
    '    ElseIf (vSkilledTrades = "SKTD0028") Then
    '        drp_skillassess.Visible = True
    '        lblSkillassess.Visible = True
    '        lblOthSkillTrade.Visible = False
    '        txtOthSkillTrade.Visible = False
    '        FileUploadSkill.Enabled = False
    '        ddlSKAss.SelectedValue = "Yes"
    '        ddlSKAss.Enabled = False
    '        txtSkRemarks.Text = ""
    '        txtSkRemarks.Enabled = False
    '        getSkillAssessment()
    '    Else
    '        lblOthSkillTrade.Visible = False
    '        txtOthSkillTrade.Visible = False
    '        drp_skillassess.Visible = False
    '        lblSkillassess.Visible = False
    '        FileUploadSkill.Enabled = True
    '        ddlSKAss.SelectedValue = "Yes"
    '        ddlSKAss.Enabled = True
    '    End If

    'End Sub
    Public Sub GetTraningSkillCD(ByVal vSkillType As String)
        Dim sqlSkill As String = " Select Upper(CTM_TYPE_DESC) CTM_TYPE_DESC ,CTM_TYPE_CODE from t_Cemp_Type_Master where CTM_TYPE ='" + vSkillType + "' and CTM_STATUS='A' order by CTM_TYPE_DESC,CTM_SEQ"
        Dim dtSkill As New DataTable()
        dtSkill = getRecord(sqlSkill, con)
        cmbSkSkill.Items.Clear()
        If dtSkill.Rows.Count > 0 Then
            cmbSkSkill.DataSource = dtSkill
            cmbSkSkill.DataTextField = "CTM_TYPE_DESC"
            cmbSkSkill.DataValueField = "CTM_TYPE_CODE"
            cmbSkSkill.DataBind()
            cmbSkSkill.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    'changed the query by sneha modak- to get SP based on Location wise(cmr:2016/01/16/J19/T2)
    Public Function Renewal_candidate(ByVal vCompCode As String, ByVal vendCode As String, ByVal spNo As String) As String
        Dim sql = "SELECT CED_SAFETY_PASS_NO, CED_SP_ISSUED_ON, CED_SP_VALID_TILL, CED_SP_ENABLED, CED_SP_BLOCKED FROM HRACE.t_cemp_details"
        sql += "    WHERE ced_sp_blocked = 'N'"
        sql += "  AND CED_SP_ENABLED IN ('Y','N')"
        ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   start****
        'sql += "  AND to_date(ced_sp_valid_till,'dd/mm/yyyy') < to_date(SYSDATE + 30 ,'dd/mm/yyyy')"
        Dim chk As String = getcompSPRenewal(vCompCode)

        If chk = "Y" Then
            Dim dur As Integer = CInt(getcompSPDur(vCompCode))
            sql += "  AND to_date(ced_sp_valid_till,'dd/mm/yyyy') <= to_date(SYSDATE + " + dur.ToString + " ,'dd/mm/yyyy')"
        ElseIf chk = "N" Then
            sql += "  AND to_date(ced_sp_valid_till,'dd/mm/yyyy') < to_date(SYSDATE + 60 ,'dd/mm/yyyy')"
        End If
        ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   End****
        sql += "   AND ced_company_code = '" + vCompCode + "'"
        sql += " and ced_safety_pass_no LIKE '" + spNo + "%' "
        'ShowMessage(sql)
        Return sql
    End Function

    <System.Web.Services.WebMethod()>
    <System.Web.Script.Services.ScriptMethod()>
    Public Shared Function GetsafetyNumber(ByVal prefixText As String, ByVal count As Integer) As String()
        Dim clm As New ospEmpDetails
        Dim vCompCode As String = clm.Session("Comp_code")
        Dim vendCode As String = clm.Session("VendCode")
        Dim Sql As String = ""

        Sql = "SELECT * FROM(" + clm.Renewal_candidate(vCompCode, vendCode, prefixText.ToUpper()) + ") WHERE ROWNUM <= 10 ORDER BY ced_safety_pass_no"

        Dim dtNumber As New DataTable

        dtNumber = clm.getRecord(Sql, clm.con)
        Dim random As New Random()
        Dim items(dtNumber.Rows.Count - 1) As String
        For i As Integer = 0 To dtNumber.Rows.Count - 1
            items(i) = dtNumber.Rows(i).Item(0).ToString
        Next
        Return (From m In items Where m.ToUpper.Contains(prefixText.ToUpper) Select m).ToArray()
    End Function
    Public Sub req_detail()

        Dim sql As String = "select TO_CHAR(SPR.SRQ_CREATED_DT,'DD/MM/YYYY')SRQ_CREATED_DT,SPR.SRQ_REQ_NO,SPR.SRQ_WORK_ORDER ,SRQ_LOCATION_CD, (SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_TYPE IN 'SPRT' AND CTM_VALUE IN SPR.SRQ_REQ_TYPE) SRQ_REQ_TYPE,"
        sql += "(Select CDM_DESC from HRACE.T_WORKFLOW_CODE_MASTER w  where(Trim(W.CDM_CODE) = SPRS.SRS_STATUS) and trim(W.CDM_SEQ_NO) = SPRS.SRS_SUB_STATUS) STATUS,"
        sql += "    (select nvl(SUM(SPRD.SRD_EMP_APV_COUNT),0) SRD_EMP_APV_COUNT from HRACE.t_sp_request_dtl SPRD where SPRD.SRD_REQ_NO=SPR.SRQ_REQ_NO)SRD_EMP_APV_COUNT"
        sql += " from HRACE.t_sp_req_status SPRS, HRACE.T_SP_REQUEST  SPR"
        sql += " where(SPRS.SRS_REQ_NO = SPR.SRQ_REQ_NO) AND SPR.SRQ_VENDOR_CODE= TRIM(UPPER('" + Session("VendCode") + "'))AND SPR.SRQ_COMPANY_CD= TRIM('" + comp_cd + "')"
        sql += " AND SPRS.SRS_STATUS='H1' AND SPRS.SRS_SUB_STATUS='5'"
        sql += " ORDER BY SRQ_REQ_NO DESC"

        Dim dt As DataTable = getRecord(sql, con)

        If dt.Rows.Count > 0 Then
            gvReq.DataSource = dt
            gvReq.DataBind()
            Loc = dt.Rows(0).Item("SRQ_LOCATION_CD")
            lblpagemsg.Text = "Note: To display the details click on the Request number."
        End If

    End Sub

#Region "Common func"
    Public Sub ReqType()
        Try

            Dim dt As DataTable = clmClass.get_codetype("SPRT", comp_cd)

            If dt.Rows.Count > 0 Then
                If Not IsDBNull(dt.Rows(0).Item("CTM_VALUE")) Then
                    '   SPN = dt.Rows(0).Item("CTM_VALUE")
                    SPN = dt.Rows(0).Item("CTM_TYPE_DESC")
                End If

                If Not IsDBNull(dt.Rows(1).Item("CTM_VALUE")) Then
                    ' SPR = dt.Rows(1).Item("CTM_VALUE")
                    SPR = dt.Rows(1).Item("CTM_TYPE_DESC")
                End If

            End If

        Catch

        End Try
    End Sub
    Protected Sub cmbTraningType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbTraningType.SelectedIndexChanged
        Dim vTrainingType As String = ""
        vTrainingType = cmbTraningType.SelectedValue
        'GetCourse(vTrainingType)
        FillDropDown(cmbTrnCource, vTrainingType)
        ' mpAddTraining.Show()
    End Sub
    Protected Sub btnSaveTraining_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveTraining.Click
        Dim ls_sql As String = String.Empty
        Dim sqlTraining As String = ""
        Dim vTrainingID As String = ""
        Dim vStartDt As String = ""
        Dim vEndDt As String = ""
        Dim certid As String = String.Empty
        If cmbTrnAgency.SelectedIndex = 0 Then
            ShowMessage("Please select agency")
            Exit Sub
        End If
        If cmbTrnLoc.SelectedIndex = 0 Then
            ShowMessage("Please select training location")
            Exit Sub
        End If
        If cmbTraningType.SelectedIndex = 0 Then
            ShowMessage("Please select training type")
            Exit Sub
        End If
        If cmbTrnCource.SelectedIndex = 0 Then
            ShowMessage("Please select course")
            Exit Sub
        End If
        If txtTrnStartDt.Text.Trim.Equals("__/__/____") Then
            ShowMessage("Please select training start date")
            Exit Sub
        End If
        If txtTrnEndDt.Text.Trim.Equals("__/__/____") Then
            ShowMessage("Please select training end date")
            Exit Sub
        End If
        If cmbTrnLoc.SelectedValue = "Othr" Then
            If txtTrnRemarks.Text.Trim.Equals("") Then
                ShowMessage("Please provide training location on remarks field")
                Exit Sub
            End If
        End If
        Dim filename As String = String.Empty
        If fileuploadtrn.HasFile = False Then
            'ShowMessage("Please Upload File")
            'Exit Sub
        Else
            filename = Path.GetFileName(fileuploadtrn.PostedFile.FileName)
            Dim contentType As String = fileuploadtrn.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fileuploadtrn.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fileuploadtrn.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If
        Try
            vStartDt = txtTrnStartDt.Text.ToString.Trim
            vEndDt = txtTrnEndDt.Text.ToString.Trim
            vTrainingID = GetID("seq_cemp_training")
            If fileuploadtrn.HasFile = False Then
                certid = "0"
            Else
                certid = TrnCWETrnSeqNo("")
            End If
            updateprevtrnvalidity(TxtSpno.Text.Trim.ToUpper)
            Dim cmdtraininginsert As New OracleCommand
            sqlTraining = "insert into t_cwm_cemp_trns_tmp(cctt_trn_id, cctt_safety_pass_no, cctt_trn_agency, cctt_trn_loc, cctt_trn_type, cctt_course_cd, cctt_start_dt, cctt_end_dt, cctt_result, cctt_remarks, cctt_created_by, cctt_created_dt, CCTT_CERT_NO,CCTT_REQ_NO,CCTT_TAG) values (:cctt_trn_id, :cctt_safety_pass_no, :cctt_trn_agency, :cctt_trn_loc, :cctt_trn_type, :cctt_course_cd, TO_DATE(:cctt_start_dt,'DD/MM/YYYY'), TO_DATE(:cctt_end_dt,'DD/MM/YYYY'), :cctt_result, :cctt_remarks, :cctt_created_by, sysdate,:CCTT_CERT_NO,:CCTT_REQ_NO,'N')"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmdtraininginsert.Connection = con
            cmdtraininginsert.CommandText = sqlTraining
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_trn_id", vTrainingID))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_safety_pass_no", TxtSpno.Text.Trim.ToUpper))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_trn_agency", cmbTrnAgency.SelectedValue))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_trn_loc", cmbTrnLoc.SelectedValue))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_trn_type", cmbTraningType.SelectedValue))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_course_cd", cmbTrnCource.SelectedValue))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_start_dt", vStartDt))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_end_dt", vEndDt))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_result", cmbTrnResult.SelectedValue))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_end_dt", vEndDt))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_remarks", txtTrnRemarks.Text.ToString.Trim.Replace("'", "''")))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":cctt_created_by", Session("VendCode")))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":CCTT_CERT_NO", certid))
            cmdtraininginsert.Parameters.Add(New OracleParameter(":CCTT_REQ_NO", Session("requestnumber")))

            cmdtraininginsert.ExecuteNonQuery()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If



            If fileuploadtrn.HasFile = True Then
                Dim cmdfiletrn As New OracleCommand
                Dim ls_sql1 As String = String.Empty
                filename = Path.GetFileName(fileuploadtrn.PostedFile.FileName)
                Using fs As Stream = fileuploadtrn.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql1 = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfiletrn.CommandText = ls_sql1
                        cmdfiletrn.Connection = con
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_DOC_ID", certid))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "TRN"))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfiletrn.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfiletrn.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If
            ShowMessage("Training saved successfully")
            'btnSearch_Click(sender, e)
            GetTraining(TxtSpno.Text.Trim.ToUpper())
            For Each gvrow As GridViewRow In gvTraining.Rows
                Dim chkbox As CheckBox = gvrow.FindControl("chkSelectTraining")
                Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                If reqno.Value.Trim = Session("requestnumber").ToString Then
                    chkbox.Enabled = True
                Else
                    chkbox.Enabled = False
                End If
            Next

            clearTraining()

            'If TxtReqNo.Text <> "" Then '  'Added to refresh the bulk activate gridview  -----sneha modak(CMR NO:2016/01/16/J20/T4)
            '    BulkSafety()
            'End If

        Catch ex As Exception

        End Try

    End Sub
    Private Function getcompSPDur(ByVal compcode As String) As String
        'WI4247: allowable date for renewal apply for eligible company location
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim chkcomp As String = "N"
        Try
            ls_sql = "select ACM_CATEGORY from hrace.t_cwm_action_mapping mp where mp.ACM_TYPE='SRT' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                chkcomp = dt.Rows(0).Item("ACM_CATEGORY")
            Else
                chkcomp = "60"
            End If

        Catch ex As Exception

        End Try
        Return chkcomp
        'WI4247: End of code
    End Function
    Private Function getcompSPRenewal(ByVal compcode As String) As String
        'WI4247: to check company location in which vendor can able to raise renewal of SP max of 90 days prior, creted by : Avik Mukherjee, created on: 07-OCt-2021
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim chkcomp As String = "N"
        Try
            ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping mp where mp.ACM_TYPE='SRT' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                chkcomp = "Y"
            Else
                chkcomp = "N"
            End If

        Catch ex As Exception

        End Try
        Return chkcomp
        'WI4247: End of code
    End Function
    Private Sub updateprevtrnvalidity(ByVal safetypass As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            ls_sql = "select CCTT_TRN_ID from T_CWM_CEMP_TRNS_TMP where CCTT_SAFETY_PASS_NO=:CCTT_SAFETY_PASS_NO  and CCTT_REQ_NO=(select max(CCTT_REQ_NO) from T_CWM_CEMP_TRNS_TMP where CCTT_SAFETY_PASS_NO=:CCTT_SAFETY_PASS_NO)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CCTT_SAFETY_PASS_NO", safetypass))
            'cmd.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    ls_sql = "update T_CWM_CEMP_TRNS_TMP set CCTT_TAG='Y' where CCTT_TRN_ID=:CCTT_TRN_ID and CCTT_SAFETY_PASS_NO=:CCTT_SAFETY_PASS_NO"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":CCTT_SAFETY_PASS_NO", safetypass))
                    cmd.Parameters.Add(New OracleParameter(":CCTT_TRN_ID", dt.Rows(i).Item("CCTT_TRN_ID")))
                    cmd.ExecuteNonQuery()
                    i = i + 1
                End While
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Sub GetTraining(ByVal vSPNo As String)
        Dim cmd As New OracleCommand()
        Dim dtTraining As New DataTable()
        Dim sqlTraining As String = "SELECT t7.DM_NAME,t1.CCTT_CERT_NO,t1.CCTT_TRN_ID, T1.CCTT_TRN_AGENCY AGENCY_CD, T2.CTM_TYPE_DESC AGENCY_NAME, T1.CCTT_TRN_LOC LOCATION_CD, nvl(T3.LOC_LOCATION_NAME,decode(CCTT_TRN_LOC,'Othr','OTHER')) LOCATION_NAME, T1.CCTT_TRN_TYPE TRAINING_TYPE, T4.CTM_TYPE_DESC TRAINING_NAME, T1.CCTT_COURSE_CD COURCE_CD, T5.CTM_TYPE_DESC COURCE_NAME, T1.CCTT_RESULT RESULT_CD, T6.CTM_TYPE_DESC RESULT_DESC, TO_CHAR(T1.CCTT_START_DT, 'DD/MM/YYYY') CCTT_START_DT, TO_CHAR(T1.CCTT_END_DT, 'DD/MM/YYYY') CCTT_END_DT, T1.CCTT_REMARKS,T1.CCTT_REQ_NO FROM T_CWM_CEMP_TRNS_TMP T1, T_CEMP_TYPE_MASTER T2, T_LOCATION_MASTER  T3, T_CEMP_TYPE_MASTER T4, T_CEMP_TYPE_MASTER T5, T_CEMP_TYPE_MASTER T6,T_DOCUMENT_MASTER T7 WHERE T1.CCTT_CERT_NO=T7.DM_DOC_ID(+) and T1.CCTT_TRN_AGENCY = T2.CTM_TYPE_CODE AND T1.CCTT_TRN_LOC = T3.LOC_LOCATION_CODE(+) AND T1.CCTT_TRN_TYPE = T4.CTM_TYPE_CODE AND T1.CCTT_COURSE_CD = T5.CTM_TYPE_CODE AND T1.CCTT_RESULT = T6.CTM_TYPE_CODE AND T1.CCTT_SAFETY_PASS_NO =:CCTT_SAFETY_PASS_NO and T7.DM_FILE_TYPE(+)='TRN' order by CCTT_TRN_ID desc"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmd.Connection = con
        cmd.CommandText = sqlTraining
        cmd.Parameters.Add(New OracleParameter(":CCTT_SAFETY_PASS_NO", vSPNo))
        Dim da = New OracleDataAdapter(cmd)
        da.Fill(dtTraining)
        If con.State = ConnectionState.Open Then
            con.Close()

        End If
        If dtTraining.Rows.Count > 0 Then
            gvTraining.DataSource = dtTraining
            gvTraining.DataBind()
            pnlTrainingDetail.Visible = True
        Else
            gvTraining.DataSource = Nothing
            gvTraining.DataBind()
            pnlTrainingDetail.Visible = False
        End If
        'clearTraining()
    End Sub
    Public Sub clearTraining()
        Try

            cmbTrnAgency.SelectedValue = 0
            cmbTrnLoc.SelectedValue = 0
            cmbTraningType.SelectedValue = 0
            ''  cmbTrnCource.SelectedValue = 0
            cmbTrnCource.Items.Clear()
            txtTrnEndDt.Text = ""
            txtTrnStartDt.Text = ""
            cmbTrnResult.SelectedValue = 0
            txtTrnRemarks.Text = ""
            btnSaveTraining.Enabled = True
            btnUpdateTraining.Enabled = False
            lbl_fileuploadtrn.Text = String.Empty
            If cmbTrnAgency.Enabled = False Then
                cmbTrnAgency.Enabled = True

            End If
            If cmbTrnLoc.Enabled = False Then
                cmbTrnLoc.Enabled = True
            End If
            If cmbTraningType.Enabled = False Then
                cmbTraningType.Enabled = True
            End If
            If cmbTrnCource.Enabled = False Then
                cmbTrnCource.Enabled = True

            End If
            If txtTrnEndDt.Enabled = False Then
                txtTrnEndDt.Enabled = True

            End If
            If txtTrnStartDt.Enabled = False Then
                txtTrnStartDt.Enabled = True

            End If
            If cmbTrnResult.Enabled = False Then
                cmbTrnResult.Enabled = True
            End If
            If txtTrnRemarks.Enabled = False Then
                txtTrnRemarks.Enabled = True
            End If
            If fileuploadtrn.Enabled = False Then
                fileuploadtrn.Enabled = True
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Function TrnCWETrnSeqNo(ByVal id As String) As String
        Dim vExpSeqNo As String = ""
        Dim sqlQualSeqNo As String = "select (HRACE.SEQ_CWM_TRAIN_CERTID.nextval) SEQNO from dual "
        Dim dtQualSeqNo As New DataTable()
        dtQualSeqNo = getRecord(sqlQualSeqNo, con)
        If dtQualSeqNo.Rows.Count > 0 Then
            vExpSeqNo = dtQualSeqNo.Rows(0)("SEQNO")
        End If

        dtQualSeqNo.Dispose()
        Return vExpSeqNo

    End Function
    Public Function GetOperationType() As String
        Dim sqlOperationType As String = " "
        Dim strOperationType As String = ""
        Dim cmd As New OracleCommand()
        Dim dtOperationType As New DataTable()
        sqlOperationType += "SELECT CTM_VALUE FROM T_CEMP_TYPE_MASTER M WHERE substr(CTM_TYPE_CODE,'-4','4')= :CTM_TYPE_CODE AND CTM_TYPE='GPRP' AND CTM_STATUS='A'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmd.Connection = con
        cmd.CommandText = sqlOperationType
        cmd.Parameters.Add(New OracleParameter(":CTM_TYPE_CODE", Session("Comp_code")))
        Dim da = New OracleDataAdapter(cmd)
        da.Fill(dtOperationType)
        If (dtOperationType.Rows.Count > 0) Then
            sqlOperationType = dtOperationType.Rows(0).Item("CTM_VALUE").ToString()
        End If

        Return sqlOperationType
    End Function
    Public Sub employeeType()
        Dim dtTable As DataTable = clmClass.get_codetype("SPET", comp_cd)

        If dtTable.Rows.Count > 0 Then
            If Not IsDBNull(dtTable.Rows(0).Item("CTM_VALUE")) Then
                WR = dtTable.Rows(0).Item("CTM_VALUE")
            End If

            If Not IsDBNull(dtTable.Rows(1).Item("CTM_VALUE")) Then
                SV = dtTable.Rows(1).Item("CTM_VALUE")
            End If

            If Not IsDBNull(dtTable.Rows(2).Item("CTM_VALUE")) Then
                DV = dtTable.Rows(2).Item("CTM_VALUE")
            End If

            If Not IsDBNull(dtTable.Rows(3).Item("CTM_VALUE")) Then
                FM = dtTable.Rows(3).Item("CTM_VALUE")
            End If

            If Not IsDBNull(dtTable.Rows(4).Item("CTM_VALUE")) Then
                VC = dtTable.Rows(4).Item("CTM_VALUE")
            End If

            'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            If Not IsDBNull(dtTable.Rows(5).Item("CTM_VALUE")) Then
                SH = dtTable.Rows(5).Item("CTM_VALUE")
            End If

            If Not IsDBNull(dtTable.Rows(6).Item("CTM_VALUE")) Then
                SF = dtTable.Rows(6).Item("CTM_VALUE")
            End If

            'sandeep
            If Not IsDBNull(dtTable.Rows(7).Item("CTM_VALUE")) Then
                SA = dtTable.Rows(7).Item("CTM_VALUE")
            End If
            If Not IsDBNull(dtTable.Rows(8).Item("CTM_VALUE")) Then
                WA = dtTable.Rows(8).Item("CTM_VALUE")
            End If
            If Not IsDBNull(dtTable.Rows(9).Item("CTM_VALUE")) Then
                DA = dtTable.Rows(9).Item("CTM_VALUE")
            End If
            If Not IsDBNull(dtTable.Rows(10).Item("CTM_VALUE")) Then
                FA = dtTable.Rows(10).Item("CTM_VALUE")
            End If
            If Not IsDBNull(dtTable.Rows(11).Item("CTM_VALUE")) Then
                VA = dtTable.Rows(11).Item("CTM_VALUE")
            End If
            'end
            If Not IsDBNull(dtTable.Rows(12).Item("CTM_VALUE")) Then
                DH = dtTable.Rows(12).Item("CTM_VALUE")
            End If

            If Not IsDBNull(dtTable.Rows(0).Item("CTM_TYPE_DESC")) Then
                WR_desc = dtTable.Rows(0).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(1).Item("CTM_TYPE_DESC")) Then
                SV_desc = dtTable.Rows(1).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(2).Item("CTM_TYPE_DESC")) Then
                DV_desc = dtTable.Rows(2).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(3).Item("CTM_TYPE_DESC")) Then
                FM_desc = dtTable.Rows(3).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(4).Item("CTM_TYPE_DESC")) Then
                VC_desc = dtTable.Rows(4).Item("CTM_TYPE_DESC")
            End If

            'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            If Not IsDBNull(dtTable.Rows(5).Item("CTM_TYPE_DESC")) Then
                SH_desc = dtTable.Rows(5).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(6).Item("CTM_TYPE_DESC")) Then
                SF_desc = dtTable.Rows(6).Item("CTM_TYPE_DESC")
            End If

            'sandeep

            If Not IsDBNull(dtTable.Rows(7).Item("CTM_TYPE_DESC")) Then
                SA_desc = dtTable.Rows(7).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(8).Item("CTM_TYPE_DESC")) Then
                WA_desc = dtTable.Rows(8).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(9).Item("CTM_TYPE_DESC")) Then
                DA_desc = dtTable.Rows(9).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(10).Item("CTM_TYPE_DESC")) Then
                FA_desc = dtTable.Rows(10).Item("CTM_TYPE_DESC")
            End If

            If Not IsDBNull(dtTable.Rows(11).Item("CTM_TYPE_DESC")) Then
                VA_desc = dtTable.Rows(11).Item("CTM_TYPE_DESC")
            End If
            'end 
            If Not IsDBNull(dtTable.Rows(12).Item("CTM_TYPE_DESC")) Then
                DH_desc = dtTable.Rows(12).Item("CTM_TYPE_DESC")
            End If
        Else
            Response.Redirect("CLMHome.aspx")
        End If

    End Sub
    Public Function getRecord(ByVal sql As String, ByVal cn As OracleConnection) As DataTable
        Dim cmd As New OracleCommand(sql, cn)
        cmd.CommandTimeout = 100
        If cn.State = ConnectionState.Closed Then
            cn.Open()
        End If
        Dim da As New OracleDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        If cn.State = ConnectionState.Open Then
            cn.Close()
        End If
        da.Dispose()
        Return dt
    End Function
    Public Function getRecord(ByVal cmd As OracleCommand, ByVal cn As OracleConnection) As DataTable
        cmd.CommandTimeout = 100
        If cn.State = ConnectionState.Closed Then
            cn.Open()
        End If
        Dim da As New OracleDataAdapter(cmd)
        Dim dt As New DataTable()
        da.Fill(dt)
        If cn.State = ConnectionState.Open Then
            cn.Close()
        End If
        da.Dispose()
        Return dt
    End Function
    Public Sub SaveData(ByVal sql As String, ByVal cn As OracleConnection)
        Dim cmd As New OracleCommand(sql, cn)
        Try
            If cn.State = ConnectionState.Closed Then
                cn.Open()
            End If
            cmd.ExecuteNonQuery()
            If cn.State = ConnectionState.Open Then
                cn.Close()
            End If
        Catch ex As Exception
            '   lblErrMsg.Text = ex.Message()
            Throw
        Finally
            If cn.State = ConnectionState.Open Then
                cn.Close()
            End If
        End Try
    End Sub
    Public Sub ShowMessage(ByVal vMgs As String)
        ' Show the message inside the page's modal (Confirm and Copy buttons) instead of a JavaScript alert.
        Dim msgHtml As String = Server.HtmlEncode(vMgs)
        msgHtml = msgHtml.Replace(vbCrLf, "<br />").Replace(vbLf, "<br />").Replace("\n", "<br />")
        divMessageContent.InnerHtml = msgHtml
        MPopUpMessage.Show()
    End Sub
    Public Sub ErrorRow(ByVal tblError As HtmlTable, ByVal vErrMsg As String)
        err_cnt = err_cnt + 1
        err_tr = New HtmlTableRow
        err_tr.Cells.Add(New HtmlTableCell)
        ' tblProfileErrorList.Rows.Add(err_tr)
        tblError.Rows.Add(err_tr)
        err_tr.Cells(0).InnerText = err_cnt & ") " + vErrMsg
        err_tr.Style("color") = "red"
        err_tr.Style("Bold") = True
        err_tr.Style("height") = "3px"

    End Sub
#End Region

#Region "Query Functions"
    Public Function categoryCount(ByVal cat As String, ByVal reqNo As String) As Integer
        'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
        'Dim qry As String = "select count(*) count from HRACE.t_cemp_details_tmp where CET_REQUEST_NO='" + reqNo + "' and CET_CATEGORY='" + cat + "'"
        Dim catParamCount As String() = cat.Split(",")
        Dim qry As String = "select count(*) count from HRACE.t_cemp_details_tmp where CET_REQUEST_NO='" + reqNo + "'"
        If (catParamCount.Count > 1) Then
            qry += " and CET_CATEGORY IN(" + cat + ")"
        Else
            qry += " and CET_CATEGORY='" + cat + "'"
        End If

        Dim dt As DataTable = getRecord(qry, con)
        Dim catcount As Integer = dt.Rows(0).Item("count")
        Return catcount
    End Function
    Public Sub count_emp(ByVal count As Integer, ByVal cat As String, ByVal reqNo As String)


        Dim catcount As Integer = categoryCount(cat, reqNo)
        Dim count_diff As Integer = 0

        count_diff = count - catcount
        Lblcount.Visible = True
        Lblcount.Text = count_diff.ToString + "/" + count.ToString
        LblempLeft.Visible = True
        'If CInt(count) = CInt(catcount) Then
        If count_diff = 0 Then

            btnSaveProfile.Visible = False
            ' Exit Sub
        End If



    End Sub
    Public Sub stop_profileEntry(ByVal count As Integer, ByVal cat As String, ByVal reqNo As String)
        Dim catcount As Integer = categoryCount(cat, reqNo)
        If catcount = count Then
            tabcontainer1.Style.Add("display", "none")
            ShowMessage("No employee left for Profile Entry.Click on safety Pass Number for Updating/Viewing records")
            Exit Sub
        Else
            tabcontainer1.Style.Remove("display")
        End If
    End Sub
    Public Function GET_SP_no() As String
        Dim SP_no As String = ""
        Dim cmd3 As New OracleCommand
        Dim drs2 As OracleDataReader
        cmd3.Connection = con
        cmd3.CommandText = " select HRACE_SP_SEQ.nextval from dual "
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        drs2 = cmd3.ExecuteReader()
        If drs2.Read Then
            SP_no = drs2(0).ToString()
        End If
        drs2.Close()
        If con.State = ConnectionState.Open Then
            con.Close()
        End If
        Return SP_no
    End Function
    Public Function GetID(ByVal vSeqName As String) As String
        Dim vSeqNo As String = ""

        Dim sqlSequence As String = "SELECT " + vSeqName + ".NEXTVAL  FROM DUAL"
        Dim dtSequence As New DataTable()
        dtSequence = getRecord(sqlSequence, con)
        If dtSequence.Rows.Count > 0 Then
            vSeqNo = dtSequence.Rows(0).Item(0)
        End If
        Return vSeqNo
    End Function
    'added new(25/4/2016)
    Public Function t_cemp_details_tmp_qry() As String
        Dim qry As String = "select CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY, NVL((SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE CTM_TYPE IN 'SPET' AND substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_VALUE IN CET_CATEGORY),(select CTM_TYPE_DESC from HRACE.t_cemp_type_master where  substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_TYPE_CODE IN CET_CATEGORY)) CET_CATEGORY_TYPE,CET_LOC_CODE"
        qry += ",CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,CET_FATHER_NAME,cet_spouse_name,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_UNIQUE_ID_TYPE,"
        qry += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,to_char(CET_DOB,'dd/MM/yyyy') CET_DOB,CET_AGE,CET_AFFIRMATIVE ,"
        qry += " (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_PROFILE_STATUS) CET_PROFILE_STATUS,"
        qry += " (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_DOCVER_STATUS) CET_DOCVER_STATUS,"
        qry += "  CET_POLICE_VERIFICATION, CET_WO_VERIFICATION, CET_AGE_VERIFICATION, CET_ADDRESS_VERIFICATION,CET_UAN_NO,CET_IP_NO, "
        'START ADD BY PRASUN ON 03112022
        qry += "CET_PAN_NO,CET_ADLT_NAME,CET_ADLT_REL,CET_ADLT_ADDRESS,CET_ADLT_MOBILE_NO,CET_NATIONALITY,CET_AADHAR_NO,CET_EMP_PLACE,CET_RELAY_DATA,CET_MEDICAL_CENTRE"
        'END ADD BY PRASUN ON 03112022
        qry += " from HRACE.t_cemp_details_tmp "
        Return qry

    End Function
    'added the category description from t_cemp_type_master table by sneha modak(cmr:2016/01/16/J19/T2)
    Public Function emp_detail_qry() As String
        Dim qry As String = "select CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY, NVL((SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE CTM_TYPE IN 'SPET' AND substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_VALUE IN CET_CATEGORY),(select CTM_TYPE_DESC from HRACE.t_cemp_type_master where  substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_TYPE_CODE IN CET_CATEGORY)) CET_CATEGORY_TYPE,CET_LOC_CODE"
        qry += ",CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,CET_FATHER_NAME,cet_spouse_name,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_UNIQUE_ID_TYPE,"
        qry += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,to_char(CET_DOB,'dd/MM/yyyy') CET_DOB,CET_AGE,CET_AFFIRMATIVE ,"
        'START ADD BY PRASUN ON 03112022
        qry += "CET_PAN_NO,CET_ADLT_NAME,CET_ADLT_REL,CET_ADLT_ADDRESS,CET_ADLT_MOBILE_NO,CET_NATIONALITY,CET_AADHAR_NO,CET_EMP_PLACE,CET_RELAY_DATA,"
        'END ADD BY PRASUN ON 03112022
        qry += " (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_PROFILE_STATUS) CET_PROFILE_STATUS,"
        qry += " (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_DOCVER_STATUS) CET_DOCVER_STATUS,"
        qry += "  CET_POLICE_VERIFICATION, CET_WO_VERIFICATION, CET_AGE_VERIFICATION, CET_ADDRESS_VERIFICATION, DECODE(CET_REQ_STATUS,'R','REJECTED','C','COMPLETED','IN PROGRESS') CET_REQ_STATUS,CET_UAN_NO,CET_IP_NO,CET_MEDICAL_CENTRE "
        qry += " from HRACE.t_cemp_details_tmp where CET_REQUEST_NO='" + Session("requestnumber") + "' "
        Return qry

    End Function
    Public Function emp_addrs_detail_qry(ByVal spno As String) As String
        'Dim qry As String = " select CCA_ADDRESS_ID,CCA_SAFETY_PASS_NO,CCA_ADDR_TYPE,CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE"
        Dim qry As String = " select CCA_ADDRESS_ID,CCA_SAFETY_PASS_NO,CCA_ADDR_TYPE,CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE, CCA_VILLAGE, CCA_PO, CCA_THANA, CCA_DISTRICT_CD "
        qry += " from HRACE.T_CWM_CEMP_ADDRS_TMP where CCA_SAFETY_PASS_NO='" + spno + "' and CCA_REQ_NO='" + Session("requestnumber") + "'"
        Return qry
    End Function
    Public Function emp_quali_detail_qry(ByVal spno As String) As String
        Dim qry As String = " select cql_qual_id,CQL_REQ_NO,cql_comp_code, cql_safety_pass_no, cql_qual_type, cql_qual_code,cql_remarks"
        qry += " from HRACE.T_CWM_CEMP_QUALIFICATIONS_TMP where cql_safety_pass_no='" + spno + "'"
        Return qry
    End Function
    Public Function emp_exp_detail_qry(ByVal spno As String) As String
        Dim ls_sql As String = String.Empty
        ls_sql = "select CWET_CERT_NO,DM_NAME,CWET_SERIAL_NO,UPPER(CWET_COMP_NAME) CWET_COMP_NAME,CWET_EXP_YR,TO_CHAR(CWET_ST_DT,'dd/mm/yyyy') stdt,TO_CHAR(CWET_END_DT,'dd/mm/yyyy') enddt,UPPER(CWET_DESIGNATION) CWET_DESIGNATION,NVL(UPPER(a.CTM_TYPE_DESC),UPPER(CWET_WORKING_AREA)) domain, CWET_WORKING_AREA ,UPPER(b.CIT_CITY_NAME) area, CWET_WORK_LOCATION from T_CWM_EXP_TMP,t_DOCUMENT_MASTER,t_CEMP_TYPE_MASTER a,t_CITY_MASTER b where  CWET_CERT_NO=DM_DOC_ID(+) and CWET_WORKING_AREA=a.CTM_TYPE_CODE(+) and trim(substr(CWET_WORK_LOCATION,4,4))=b.CIT_CITY_CODE and CWET_SAFETY_PASS_NO='" + spno + "' and CWET_COMP_CODE='" + Session("Comp_code") + "' and a.CTM_TYPE(+)='EXDM' and trim(substr(CWET_WORK_LOCATION,0,4))=b.CIT_STATE_CODE order by CWET_SERIAL_NO "
        Return ls_sql
    End Function
    Public Function emp_nominee_detail_qry(ByVal spno As String) As String
        Dim qry As String = " select ccn_nominee_id,CCN_REQ_NO,ccn_comp_code,ccn_safety_pass_no, ccn_relation_cd, ccn_nominee_name,to_char(ccn_nominee_dob,'dd/MM/yyyy') ccn_nominee_dob,ccn_pymt_grp,ccn_share,ccn_remarks,ccn_nominee_address"
        qry += " from HRACE.T_CWM_CEMP_NOMINEES_TMP where ccn_safety_pass_no='" + spno + "'"
        Return qry
    End Function
    'added the category description from t_cemp_type_master table by sneha modak(cmr:2016/01/16/J19/T2)
    Public Function T_CEMP_DETAILS_qry() As String
        Dim qry As String = "select CED_SAFETY_PASS_NO,CED_COMPANY_CODE,CED_VENDOR_CODE,CED_CATEGORY,nvl((SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE CTM_TYPE IN 'SPET' AND substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND  CTM_VALUE IN CED_CATEGORY),'') CED_CATEGORY_TYPE,CED_LOC_CODE"
        qry += ",CED_DEPT_CODE,CED_FIRSTNAME,CED_MIDDLENAME,CED_LASTNAME,CED_FATHER_NAME,CED_HUSBAND_NAME,CED_GENDER,CED_EMERGENCY_NO,CED_PHONE_NO,CED_UNIQUE_ID_TYPE,CED_ADDRESS1,"
        qry += "  CED_UNIQUE_ID_VALUE, CED_IDENTIFICATION_MARK ,CED_AREA_OF_WORK,to_char(CED_DOB,'dd/MM/yyyy') CED_DOB,CED_AGE,CED_AFFIRMATIVE,CED_BLOOD_GROUP,ced_work_based_on ,ced_sp_enabled "
        ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   start****
        qry += "  ,CED_SP_BLOCKED,CED_UAN_NO,CED_IP_NO"
        ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   End****
        'START ADD BY PRASUN 02092022
        qry += ",CED_PAN_NO,CED_ADLT_NAME,CED_ADLT_REL,CED_ADLT_ADDRESS,CED_ADLT_MOBILE_NO,CED_NATIONALITY,CED_AADHAR_NO,CED_EMP_PLACE,CED_RELAY_DATA"
        'END ADD BY PRASUN 02092022
        qry += " from HRACE.t_cemp_details "
        Return qry
    End Function
    Public Function T_CWM_CEMP_ADDRS_qry(ByVal spno As String) As String
        'Dim qry As String = " select CCA_ADDRESS_ID,CCA_SAFETY_PASS_NO,CCA_ADDR_TYPE,CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE"
        Dim qry As String = " select CCA_ADDRESS_ID,CCA_SAFETY_PASS_NO,CCA_ADDR_TYPE,CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE, CCA_VILLAGE, CCA_PO, CCA_THANA, CCA_DISTRICT_CD "
        qry += " from HRACE.T_CWM_CEMP_ADDRS where CCA_SAFETY_PASS_NO='" + spno + "'"
        Return qry
    End Function
    Public Function T_CWM_CEMP_QUALIFICATIONS_qry(ByVal spno As String) As String
        Dim qry As String = " select cql_qual_id,cql_comp_code, cql_safety_pass_no, cql_qual_type, cql_qual_code,cql_remarks"
        qry += " from HRACE.T_CWM_CEMP_QUALIFICATIONS where cql_safety_pass_no='" + spno + "'"
        Return qry
    End Function
    Public Function T_CWM_CEMP_EXP_qry(ByVal spno As String) As String
        Dim qry As String = " select CWE_SAFETY_PASS_NO from T_CWM_EXP where CWE_SAFETY_PASS_NO='" + spno + "'"

        Return qry
    End Function
    Public Function T_CWM_CEMP_SKILL_qry(ByVal spno As String) As String
        Dim qry As String = " select CCS_SAFETY_PASS_NO from T_CWM_CEMP_SKILL where CCS_SAFETY_PASS_NO='" + spno + "'"

        Return qry
    End Function
    Public Function T_CWM_CEMP_TRN_qry(ByVal spno As String) As String
        Dim qry As String = " select CCT_SAFETY_PASS_NO from t_cwM_cemp_trns where CCT_SAFETY_PASS_NO='" + spno + "'"

        Return qry
    End Function
    Public Function T_CWM_CEMP_PV_qry(ByVal spno As String) As String

        Dim qry As String = " select CED_SAFETY_PASS_NO from T_CEMP_DETAILS where CED_SAFETY_PASS_NO='" + spno + "' and CED_PV_ISSUED_ON is not null"
        Return qry
    End Function
    Public Function T_CWM_CEMP_NOMINEES_qry(ByVal spno As String) As String
        Dim qry As String = " Select ccn_nominee_id,ccn_comp_code,ccn_safety_pass_no, ccn_relation_cd, ccn_nominee_name,to_char(ccn_nominee_dob,'dd/MM/yyyy') ccn_nominee_dob,ccn_pymt_grp,ccn_share,ccn_remarks,ccn_nominee_address"
        qry += " from HRACE.T_CWM_CEMP_NOMINEES where ccn_safety_pass_no='" + spno + "'"
        Return qry
    End Function
    Public Sub empView()

        Try
            'If ((comp_cd = "1003" Or comp_cd = "3000") Or (comp_cd = "1000" And Txtdeprt.Text.ToString = "502")) Then
            '    tabSkill.Enabled = False
            'Else
            '    tabSkill.Enabled = True
            'End If
            '**************vaccination eligible for following location*********'
            Dim ls_vace As String = "select * from t_cwm_action_mapping where ACM_TYPE='VACE' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
            Dim cmdvac As OracleCommand = New OracleCommand(ls_vace, con)
            cmdvac.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))
            Dim dtvace As DataTable = getRecord(cmdvac, con)
            If dtvace.Rows.Count > 0 Then
                tabvaccination.Enabled = True
            Else
                tabvaccination.Enabled = False
            End If



        Catch ex As Exception

        End Try
        Dim qry As String = emp_detail_qry()
        Dim safetyPassnumber As String = ""
        Dim dt As DataTable = clmClass.getRecord(qry, con)

        status_variables()


        Dim dtemp As New DataTable
        dtemp.Columns.Add("CET_SAFETY_PASSNO")
        dtemp.Columns.Add("CET_NAME")
        dtemp.Columns.Add("CET_CATEGORY")
        dtemp.Columns.Add("CET_UNIQUE_ID_VALUE")
        dtemp.Columns.Add("STATUS")
        dtemp.Columns.Add("VERIFY")
        dtemp.Columns.Add("CET_DOB")
        dtemp.Columns.Add("OVER_ALL_STATUS")
        Dim dr As DataRow
        If dt.Rows.Count > 0 Then

            For i = 0 To dt.Rows.Count - 1

                Dim count_reject As Integer = 0
                Dim count_incomp As Integer = 0

                Dim profile_status As String = ""
                Dim verify_status As String = ""
                safetyPassnumber = ""
                safetyPassnumber = Trim(dt.Rows(i).Item("CET_SAFETY_PASSNO"))

                makeprofileincomplete(safetyPassnumber, Session("requestnumber"))
                Dim sql As String = emp_detail_qry() + "and CET_SAFETY_PASSNO ='" + safetyPassnumber + "' and CET_REQUEST_NO='" + Session("requestnumber") + "'"
                Dim dtv As New DataTable
                dtv = getRecord(sql, con)

                If dtv.Rows.Count > 0 Then
                    If Not IsDBNull(dt.Rows(i).Item("CET_PROFILE_STATUS")) Then
                        profile_status = dt.Rows(i).Item("CET_PROFILE_STATUS")
                    End If

                    If Not IsDBNull(dt.Rows(i).Item("CET_DOCVER_STATUS")) Then
                        verify_status = dt.Rows(i).Item("CET_DOCVER_STATUS")
                    End If
                End If



                dr = dtemp.NewRow
                dr("CET_SAFETY_PASSNO") = dt.Rows(i).Item("CET_SAFETY_PASSNO")
                dr("CET_NAME") = dt.Rows(i).Item("CET_FIRSTNAME") + " " + dt.Rows(i).Item("CET_LASTNAME")
                dr("CET_CATEGORY") = dt.Rows(i).Item("CET_CATEGORY_TYPE") 'change
                dr("CET_UNIQUE_ID_VALUE") = dt.Rows(i).Item("CET_UNIQUE_ID_VALUE")
                dr("CET_DOB") = dt.Rows(i).Item("CET_DOB")
                If (dt.Rows(i).Item("CET_REQ_STATUS") = "REJECTED") Then
                    dr("OVER_ALL_STATUS") = String.Format("<font color='red'>{0}</font>", dt.Rows(i).Item("CET_REQ_STATUS"))
                ElseIf (dt.Rows(i).Item("CET_REQ_STATUS") = "IN PROGRESS") Then
                    dr("OVER_ALL_STATUS") = String.Format("<font color='#926e04'>{0}</font>", dt.Rows(i).Item("CET_REQ_STATUS"))
                Else
                    dr("OVER_ALL_STATUS") = dt.Rows(i).Item("CET_REQ_STATUS")
                End If
                Dim reqNo As String = Session("requestnumber")

                Dim dtm As DataTable = empDetails_tmp_table(safetyPassnumber, reqNo)

                If profile_status <> "" Then
                    If dtm.Rows.Count = 0 Then

                        Dim locCheck = CheckWireFrameLoc()
                        Dim skillCheck = CheckSkillTemp(reqNo, safetyPassnumber)
                        Dim dtReq_Category As Boolean = False
                        dtReq_Category = ChecReqCategory(reqNo, safetyPassnumber)
                        If locCheck And skillCheck And dtReq_Category Then
                            dr("STATUS") = msg_complete
                            update_profile_Status(safetyPassnumber, msg_complete_val, reqNo)
                        Else
                            dr("STATUS") = msg_incomp
                            update_profile_Status(safetyPassnumber, msg_incomp_val, reqNo)
                        End If
                    End If
                    If dtm.Rows.Count > 0 Then

                        If IsDBNull(dtm.Rows(0).Item(0)) Then
                            dr("STATUS") = msg_incomp
                            update_profile_Status(safetyPassnumber, msg_incomp_val, reqNo)
                        End If
                        If profile_status = msg_complete And Not IsDBNull(dtm.Rows(0).Item(0)) Then

                            If dtm.Rows(0).Item(0).ToString = "COMPLETED" Then
                                dr("STATUS") = profile_status
                                update_profile_Status(safetyPassnumber, msg_complete_val, reqNo)
                            ElseIf dtm.Rows(0).Item(0).ToString = "INCOMPLETE" Then
                                dr("STATUS") = msg_incomp
                                update_profile_Status(safetyPassnumber, msg_incomp_val, reqNo)
                            End If

                        ElseIf profile_status = msg_incomp Then


                            If dtm.Rows.Count > 0 Then
                                dr("STATUS") = dtm.Rows(0).Item("STATUS")
                                update_profile_Status(safetyPassnumber, msg_complete_val, reqNo)
                                '''''''procedure to make incomplete for TGS''''''''''''''''
                                makeprofileincomplete(safetyPassnumber, reqNo)

                                '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                            Else
                                dr("STATUS") = String.Format("<font color='red'>" + profile_status + "</font>")
                            End If

                        End If

                    End If
                ElseIf profile_status = "" Then


                    If dtm.Rows.Count = 0 Then
                        dr("STATUS") = String.Format("<font color='red'>" + msg_incomp + "</font>")
                        update_profile_Status(safetyPassnumber, msg_incomp_val, reqNo)
                    Else
                        dr("STATUS") = dtm.Rows(0).Item("STATUS")
                        update_profile_Status(safetyPassnumber, msg_complete_val, reqNo)
                        '''''''procedure to make incomplete for TGS''''''''''''''''
                        makeprofileincomplete(safetyPassnumber, reqNo)

                        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                    End If

                End If

                Dim dtDetail As DataTable = t_cemp_detail_dt(safetyPassnumber)

                Dim dverify As DataTable = doc_verification(safetyPassnumber)
                count_reject = 0
                For j = 0 To dverify.Rows.Count - 1
                    If Not IsDBNull(dverify.Rows(j).Item("SDV_VERF_FLAG")) Then
                        If dverify.Rows(j).Item("SDV_VERF_FLAG") = "N" Then
                            count_reject = count_reject + 1
                        End If
                    End If
                Next

                If verify_status <> "" Then
                    If verify_status = msg_complete Then
                        dr("VERIFY") = verify_status
                        If count_reject > 0 Then
                            dr("VERIFY") = String.Format("<font color='red'>" + msg_reject + "</font>")
                            'update_doc(safetyPassnumber, msg_reject_val, reqNo)   'update value
                        End If
                    ElseIf verify_status = msg_incomp Then
                        dr("VERIFY") = String.Format("<font color='red'>" + verify_status + "</font>")
                        If dtDetail.Rows.Count > 0 Then

                            If Session("reqType") <> "Renew" Then
                                ' update_doc(safetyPassnumber, msg_complete_val, reqNo)
                                dr("VERIFY") = msg_complete
                            End If

                            'update value
                        End If
                    ElseIf verify_status = msg_reject Then
                        dr("VERIFY") = String.Format("<font color='red'>" + "RETURNED" + "</font>")
                    End If
                ElseIf verify_status = "" Then
                    If dtDetail.Rows.Count > 0 Then
                        dr("VERIFY") = msg_complete
                        'update_doc(safetyPassnumber, msg_complete_val, reqNo)   'update value

                    Else
                        If count_reject > 0 Then
                            dr("VERIFY") = String.Format("<font color='red'>" + "RETURNED" + "</font>")
                            'update_doc(safetyPassnumber, msg_reject_val, reqNo)   'update value
                        End If
                    End If
                End If

                'add in the row of datatable
                dtemp.Rows.Add(dr)
            Next
        End If

        If dtemp.Rows.Count > 0 Then
            GridViewEmp.DataSource = dtemp
            GridViewEmp.DataBind()
            rejectStatus()
            lblpagemsg.Text = "Note: To display details of contract employee,click on the Safety Pass number."
        Else
            lblpagemsg.Text = "Note: A click on the respective Employee Type, will open a section to fill employee details."
        End If

    End Sub
    Public Sub status_variables()
        Dim dt_msg As DataTable = clmClass.get_codetype("STA", comp_cd)

        If dt_msg.Rows.Count > 0 Then

            msg_complete = dt_msg.Rows(0).Item("CTM_TYPE_DESC")
            msg_incomp = dt_msg.Rows(1).Item("CTM_TYPE_DESC")
            msg_reject = dt_msg.Rows(2).Item("CTM_TYPE_DESC")

            msg_complete_val = dt_msg.Rows(0).Item("ctm_value")
            msg_incomp_val = dt_msg.Rows(1).Item("ctm_value")
            msg_reject_val = dt_msg.Rows(2).Item("ctm_value")
        Else
            Response.Redirect("CLMHome.aspx")
            Exit Sub
        End If
    End Sub

    'Public Function empDetails_tmp_table(ByVal safetyPassnumber As String, ByVal reqNo As String) As DataTable
    '    Dim sql As String = "select case when  cdt.cet_safety_passno is not NULL  "
    '    sql += " and cat.cca_safety_pass_no  is not NULL"
    '    sql += " and cnt.ccn_safety_pass_no is not NULL"
    '    sql += "   and  cqt.cql_safety_pass_no is not NULL THEN '" + msg_complete + "' "
    '    sql += "  ELSE '" + msg_incomp + "' END STATUS "
    '    sql += "  from  HRACE.t_cemp_details_tmp cdt, hrace.t_cwm_cemp_addrs_tmp cat, HRACE.t_cwm_cemp_nominees_tmp cnt, HRACE.t_cwm_cemp_qualifications_tmp cqt"
    '    sql += "  where trim(cdt.cet_safety_passno)='" + safetyPassnumber + "' "
    '    sql += " and cdt.cet_request_no='" + reqNo + "' "
    '    sql += " and trim(cat.cca_safety_pass_no)='" + safetyPassnumber + "'"
    '    sql += " and trim(cnt.ccn_safety_pass_no)='" + safetyPassnumber + "'"
    '    sql += " and trim(cqt.cql_safety_pass_no)='" + safetyPassnumber + "'"
    '    Dim dtm As DataTable = clmClass.getRecord(sql, con)
    '    Return dtm
    'End Function
    Public Function empDetails_tmp_table(ByVal safetyPassnumber As String, ByVal reqNo As String) As DataTable
        Dim chkskillforEP As String = "N"
        Dim ls_sql As String = String.Empty
        Dim dtep As New DataTable
        ls_sql = "select ACM_COMPANY_CODE,ACM_CATEGORY from t_cwm_action_mapping where ACM_TYPE='SKE' and ACM_FLAG='Y' and ACM_COMPANY_CODE='" + comp_cd + "'"
        dtep = getRecord(ls_sql, con)
        If dtep.Rows.Count > 0 Then
            If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1000" And (dtep.Rows(0).Item("ACM_CATEGORY") = Txtdeprt.Text.Trim.ToString) Then
                chkskillforEP = "Y"
            End If
            If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1003" Or dtep.Rows(0).Item("ACM_COMPANY_CODE") = "3000" Then
                chkskillforEP = "Y"
            End If
        Else
            chkskillforEP = "N"
        End If
        Dim qry As String = t_cemp_details_tmp_qry() + "where CET_SAFETY_PASSNO='" + safetyPassnumber + "' and CET_REQUEST_NO='" + reqNo + "'"
        Dim dt As DataTable = getRecord(qry, con)
        Dim sql As String = "select distinct case when  cdt.cet_safety_passno is not NULL  "

        If dt.Rows.Count > 0 Then

            If Not IsDBNull(dt.Rows(0).Item("CET_LOCATION_CODE")) Then
                If ((dt.Rows(0).Item("CET_LOCATION_CODE").ToString = "1003" Or dt.Rows(0).Item("CET_LOCATION_CODE").ToString = "3000") Or (dt.Rows(0).Item("CET_LOCATION_CODE").ToString = "1000" And dt.Rows(0).Item("CET_DEPT_CODE").ToString = "502")) Then

                    sql += " and cat.cca_safety_pass_no  is not NULL"
                    sql += " and cnt.ccn_safety_pass_no is not NULL"
                    sql += " and cnsnt.cnd_safetypass_num is not NULL"

                    'ADD AGE MENDATORY
                    sql += "   And  cqt.cql_safety_pass_no Is Not NULL And skl.CCST_SAFETY_PASS_NO Is Not NULL  Then '" + msg_complete + "' "
                    sql += "  ELSE '" + msg_incomp + "' END STATUS "
                    sql += "  from  HRACE.t_cemp_details_tmp cdt, hrace.t_cwm_cemp_addrs_tmp cat, HRACE.t_cwm_cemp_nominees_tmp cnt, HRACE.t_cwm_cemp_qualifications_tmp cqt, HRACE.T_CWM_CEMP_SKILL_TMP skl, hrace.t_cemp_piiconsent_details cnsnt"
                    sql += "  where trim(cdt.cet_safety_passno)='" + safetyPassnumber + "' "
                    sql += " and cdt.cet_request_no='" + reqNo + "' "
                    sql += " and trim(cat.cca_safety_pass_no)='" + safetyPassnumber + "'"
                    sql += " and trim(cnsnt.cnd_safetypass_num)='" + safetyPassnumber + "'"
                    sql += " and trim(cnt.ccn_safety_pass_no)='" + safetyPassnumber + "'"
                    sql += " and trim(cqt.cql_safety_pass_no)='" + safetyPassnumber + "'"
                    sql += " and TRIM (skl.CCST_SAFETY_PASS_NO) = '" + safetyPassnumber + "'"
                    sql += " and  CAT.CCA_REQ_NO='" + reqNo + "' and CQT.CQL_REQ_NO='" + reqNo + "' "
                    '*******changes where skill attachment is not mandatory**************
                    sql += "and SKL.CCST_REQ_NO='" + reqNo + "'"
                    '*******************************************************
                    'sql += " And SKL.CCST_REQ_NO ='" + reqNo + "' and (((CCST_ASSESSMENT_RESULT is null or CCST_ASSESSMENT_RESULT is not null) and (CCST_SKTP_CP_CD='NA' or CCST_SKTP_CP_CD is null ) and (CCST_ASSESSMENT_TYPE not in ('D','T') and CCST_ASSESSMENT_TYPE='0' )) or (CCST_ASSESSMENT_RESULT='PASS' and ((CCST_SKTP_CP_CD<>'NA' or CCST_SKTP_CP_CD='NA')  and CCST_ASSESSMENT_TYPE in('D','T') and (CCST_CERT_NO is not null and CCST_CERT_NO > 0))))  "

                    sql += " and (CDT.CET_DOB_CERT_NO  IS NOT NULL AND CDT.CET_DOB_CERT_NO<>'0') "


                Else

                    sql += " and cat.cca_safety_pass_no  is not NULL"
                    sql += " and cnt.ccn_safety_pass_no is not NULL"
                    sql += " and cnsnt.cnd_safetypass_num is not NULL"

                    'ADD AGE MENDATORY
                    sql += "   And  cqt.cql_safety_pass_no Is Not NULL And skl.CCST_SAFETY_PASS_NO Is Not NULL  Then '" + msg_complete + "' "
                    sql += "  ELSE '" + msg_incomp + "' END STATUS "
                    sql += "  from  HRACE.t_cemp_details_tmp cdt, hrace.t_cwm_cemp_addrs_tmp cat, HRACE.t_cwm_cemp_nominees_tmp cnt, HRACE.t_cwm_cemp_qualifications_tmp cqt, HRACE.T_CWM_CEMP_SKILL_TMP skl, hrace.t_cemp_piiconsent_details cnsnt"
                    sql += "  where trim(cdt.cet_safety_passno)='" + safetyPassnumber + "' "
                    sql += " and cdt.cet_request_no='" + reqNo + "' "
                    sql += " and trim(cat.cca_safety_pass_no)='" + safetyPassnumber + "'"
                    sql += " and trim(cnsnt.cnd_safetypass_num)='" + safetyPassnumber + "'"
                    sql += " and trim(cnt.ccn_safety_pass_no)='" + safetyPassnumber + "'"
                    sql += " and trim(cqt.cql_safety_pass_no)='" + safetyPassnumber + "'"
                    sql += " and TRIM (skl.CCST_SAFETY_PASS_NO) = '" + safetyPassnumber + "'"
                    sql += " and  CAT.CCA_REQ_NO='" + reqNo + "' and CQT.CQL_REQ_NO='" + reqNo + "' and SKL.CCST_REQ_NO='" + reqNo + "' and (((CCST_ASSESSMENT_RESULT is null or CCST_ASSESSMENT_RESULT is not null) and (CCST_SKTP_CP_CD='NA' or CCST_SKTP_CP_CD is null ) and (CCST_ASSESSMENT_TYPE not in ('D','T') and CCST_ASSESSMENT_TYPE='0' )) or (CCST_ASSESSMENT_RESULT='PASS' and ((CCST_SKTP_CP_CD<>'NA' or CCST_SKTP_CP_CD='NA')  and CCST_ASSESSMENT_TYPE in('D','T') and (CCST_CERT_NO is not null and CCST_CERT_NO > 0))))  "
                    sql += " and (CDT.CET_DOB_CERT_NO  IS NOT NULL AND CDT.CET_DOB_CERT_NO<>'0') "
                End If

            End If
        End If
        Dim dtm As DataTable = clmClass.getRecord(sql, con)
        Return dtm
    End Function
    Public Sub update_doc(ByVal safetyNo As String, ByVal status As String, ByVal reqNo As String)
        Dim UpdateDocstatus As String = "UPDATE HRACE.t_cemp_details_tmp SET CET_DOCVER_STATUS='" + status + "'  WHERE CET_SAFETY_PASSNO='" + safetyNo + "' and CET_REQUEST_NO='" + reqNo + "'"
        Dim cmd_upd_att As New OracleCommand(UpdateDocstatus, con)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd_upd_att.ExecuteNonQuery()
        Catch ex As Exception

        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Private Sub makeprofileincomplete(ByVal safetypass As String, ByVal reqno As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim result As String = String.Empty

        Try
            If chk_waive.Checked = True Then
                Exit Sub
            End If
            ls_sql = "select TCD_CLM_SKILL_CD  from hrps.T_TD_CLM_DOC@ace_iris,hrace.t_cwm_cemp_skill_tmp where TCD_SP_NO=:TCD_SP_NO and TCD_SP_NO=CCST_SAFETY_PASS_NO and TCD_CLM_SKILL_CD=CCST_SKTD_CP_CD and TCD_VALID_TAG='Y'  and CCST_REQ_NO=:CCST_REQ_NO and UPPER(TCD_CERT_CATEG)<>'FAIL'"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":TCD_SP_NO", safetypass))
            cmd.Parameters.Add(New OracleParameter(":CCST_REQ_NO", reqno))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                Try
                    ls_sql = "update t_cwm_cemp_skill_tmp set CCST_ASSESSMENT_RESULT='PASS' where CCST_SAFETY_PASS_NO=:TCD_SP_NO and CCST_REQ_NO=:CCST_REQ_NO and CCST_WAIVE_OFF='N' and CCST_ASSESSMENT_TYPE in('D','T')"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":TCD_SP_NO", safetypass))
                    cmd.Parameters.Add(New OracleParameter(":CCST_REQ_NO", reqno))
                    cmd.ExecuteNonQuery()
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                Catch ex As Exception
                    ShowMessage("Error while updating skill records")
                    Exit Sub
                End Try


            End If
            ls_sql = "select nvl(CCST_ASSESSMENT_RESULT,'NA') CCST_ASSESSMENT_RESULT from hrace.t_cwm_cemp_skill_tmp where CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_REQ_NO=:CCST_REQ_NO and CCST_WAIVE_OFF='N' and CCST_ASSESSMENT_TYPE in('D','T')"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", safetypass))
            cmd.Parameters.Add(New OracleParameter(":CCST_REQ_NO", reqno))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                result = dt.Rows(0).Item("CCST_ASSESSMENT_RESULT")
            Else
                result = "NF"
            End If
            If result.Equals("NA") Or result.Equals("FAIL") Then
                result = "NA"
            End If
            If result.Equals("NA") Then
                If ((comp_cd = "1003" Or comp_cd = "3000") Or (comp_cd = "1000" And Txtdeprt.Text.ToString = "502")) Then
                Else
                    Dim locCheck = CheckWireFrameLoc()

                    If locCheck = False Then
                        update_profile_Status(safetypass, msg_incomp_val, reqno)
                    End If
                    'update_profile_Status(safetypass, msg_incomp_val, reqno)
                End If

            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Sub update_profile_Status(ByVal safetyNo As String, ByVal status As String, ByVal reqNo As String)
        Dim UpdateProfilestatus As String = "UPDATE HRACE.t_cemp_details_tmp SET CET_PROFILE_STATUS='" + status + "', cet_modified_by='" + Session("VendCode") + "',cet_modified_date=sysdate WHERE CET_SAFETY_PASSNO='" + safetyNo + "' and CET_REQUEST_NO='" + reqNo + "' and CET_DOCVER_STATUS='I'"
        Dim cmd_upd_att As New OracleCommand(UpdateProfilestatus, con)
        Try
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd_upd_att.ExecuteNonQuery()
        Catch ex As Exception

        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        End Try
    End Sub
    Public Function doc_verification(ByVal safetyNo As String) As DataTable
        Dim qry As String = "SELECT SDV.SDV_SAFETYPASS_NO,SDV.SDV_VERF_TYPE,SDV.SDV_VERF_FLAG FROM HRACE.t_sp_doc_verification SDV WHERE SDV.SDV_SAFETYPASS_NO='" + safetyNo + "'"
        Dim dt As DataTable = clmClass.getRecord(qry, con)
        Return dt
    End Function
    Public Function t_cemp_detail_dt(ByVal safetyPassNo As String) As DataTable

        Dim qry As String = T_CEMP_DETAILS_qry() + " where CED_SAFETY_PASS_NO='" + safetyPassNo + "' "

        Dim dt As DataTable = clmClass.getRecord(qry, con)
        Return dt
    End Function
    Public Sub cat(ByVal category As String)
        'sandeep
        If category = WR Or category = WA Then
            GetCategory(String.Format("'{0}','{1}'", WR_desc, WA_desc))
        ElseIf category = DV Or category = DA Or category = DH Then
            GetCategory(String.Format("'{0}','{1}','{2}'", DV_desc, DA_desc, DH_desc))
            'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            'ElseIf category = SV Then
            'GetCategory(SV_desc)
        ElseIf category = SV Or category = SH Or category = SF Or category = SA Then
            GetCategory(String.Format("'{0}','{1}','{2}','{3}'", SV_desc, SH_desc, SF_desc, SA_desc))
        ElseIf category = FM Or category = FA Then
            GetCategory(String.Format("'{0}','{1}'", FM_desc, FA_desc))
        ElseIf category = VC Or category = VA Then
            GetCategory(String.Format("'{0}','{1}'", VC_desc, VA_desc))
        Else
            'added to get the category description from t_cemp_type_master table which are not comes under SPET category
            Dim sqlCategory As String = t_Cemp_Type_Master() + "  where CTM_TYPE_CODE ='" + category + "' "
            Dim dtCategory As New DataTable()
            dtCategory = getRecord(sqlCategory, con)
            cmbCategory.Items.Clear()
            If dtCategory.Rows.Count > 0 Then
                cmbCategory.DataSource = dtCategory
                cmbCategory.DataTextField = "CTM_TYPE_DESC"
                'cmbCategory.DataValueField = "CTM_VALUE"
                cmbCategory.DataValueField = "CTM_TYPE_CODE"
                cmbCategory.DataBind()
            End If
        End If
    End Sub
    Public Sub profile_details(ByVal sp_no As String)
        txtFName.Text = ""
        txtLName.Text = ""
        txtDOB.Text = ""
        cmbSex.SelectedValue = "0"
        txtFatherName.Text = ""
        txtHusName.Text = ""
        txtPhNo.Text = ""
        txtEmrgNo.Text = ""
        cmbUniqID.SelectedValue = "0"
        txtIdentiFication.Text = ""
        cmbAffirmative.SelectedValue = "0"
        txtUniqIDNo.Text = ""
        cmbWorkArea.SelectedValue = "0"
        TxtSpno.Text = ""
        ddlMedCentre.SelectedValue = "0"

        Dim qry As String = emp_detail_qry()
        qry = qry + "and CET_SAFETY_PASSNO ='" + sp_no + "'"
        Dim dt As DataTable = getRecord(qry, con)

        If dt.Rows.Count > 0 Then
            If Not IsDBNull(dt.Rows(0).Item("CET_CATEGORY")) Then
                cmbCategory.Items.Clear()
                'GetCategory(DV_desc) not
                'cmbCategory.SelectedValue = dt.Rows(0).Item("CET_CATEGORY") not
                Dim category As String = dt.Rows(0).Item("CET_CATEGORY")
                cat(category)
                'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,   Change: supervisor sub catagorized into SV,SH,SF
                cmbCategory.SelectedValue = category
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_FIRSTNAME")) Then
                txtFName.Text = dt.Rows(0).Item("CET_FIRSTNAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_LASTNAME")) Then
                txtLName.Text = dt.Rows(0).Item("CET_LASTNAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_DOB")) Then
                txtDOB.Text = dt.Rows(0).Item("CET_DOB")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_GENDER")) Then
                Try
                    cmbSex.SelectedValue = dt.Rows(0).Item("CET_GENDER")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_MEDICAL_CENTRE")) Then
                Try
                    ddlMedCentre.SelectedValue = dt.Rows(0).Item("CET_MEDICAL_CENTRE")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_FATHER_NAME")) Then
                txtFatherName.Text = dt.Rows(0).Item("CET_FATHER_NAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_SPOUSE_NAME")) Then
                txtHusName.Text = dt.Rows(0).Item("CET_SPOUSE_NAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_PHONE_NO")) Then
                txtPhNo.Text = dt.Rows(0).Item("CET_PHONE_NO")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_EMERGENCY_NO")) Then
                txtEmrgNo.Text = dt.Rows(0).Item("CET_EMERGENCY_NO")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_UNIQUE_ID_TYPE")) Then
                Try
                    cmbUniqID.SelectedValue = dt.Rows(0).Item("CET_UNIQUE_ID_TYPE")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_UNIQUE_ID_VALUE")) Then
                txtUniqIDNo.Text = dt.Rows(0).Item("CET_UNIQUE_ID_VALUE")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_IDENTIFICATION_MARK")) Then
                txtIdentiFication.Text = dt.Rows(0).Item("CET_IDENTIFICATION_MARK")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_AFFIRMATIVE")) Then
                Try
                    cmbAffirmative.SelectedValue = dt.Rows(0).Item("CET_AFFIRMATIVE")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_AREA_OF_WORK")) Then
                Try

                    cmbWorkArea.SelectedValue = dt.Rows(0).Item("CET_AREA_OF_WORK")
                Catch ex As Exception

                End Try
            End If

            txtuan.Text = dt.Rows(0).Item("CET_UAN_NO").ToString.Trim
            txtip.Text = dt.Rows(0).Item("CET_IP_NO").ToString.Trim
            btnSaveProfile.Visible = False
            btnUpdateProfile.Visible = True

            TxtSpno.Text = dt.Rows(0).Item("CET_SAFETY_PASSNO")

            'START ADD BY PRASUN ON 11032022

            If Not IsDBNull(dt.Rows(0).Item("CET_PAN_NO")) Then
                txtPAN.Text = AESEncryption.Decrypt(dt.Rows(0).Item("CET_PAN_NO"), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_NAME")) Then
                txtAdltName.Text = dt.Rows(0).Item("CET_ADLT_NAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_REL")) Then
                Try
                    cmbAdltRelation.SelectedValue = dt.Rows(0).Item("CET_ADLT_REL")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_ADDRESS")) Then
                txtAdltAddress.Text = dt.Rows(0).Item("CET_ADLT_ADDRESS")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_MOBILE_NO")) Then
                txtAdltMobile.Text = AESEncryption.Decrypt(dt.Rows(0).Item("CET_ADLT_MOBILE_NO"), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_NATIONALITY")) Then
                Try
                    cmbNationality.SelectedValue = dt.Rows(0).Item("CET_NATIONALITY")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_AADHAR_NO")) Then
                txtAADHAR.Text = AESEncryption.Decrypt(dt.Rows(0).Item("CET_AADHAR_NO"), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_EMP_PLACE")) Then
                Try
                    cmbPlaceOfEmployment.SelectedValue = dt.Rows(0).Item("CET_EMP_PLACE")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_RELAY_DATA")) Then
                Try
                    cmbRelayData.SelectedValue = dt.Rows(0).Item("CET_RELAY_DATA")
                Catch ex As Exception

                End Try
            End If
            'END ADD BY PRASUN ON 11032022



            Lblspno.Visible = True
            TxtSpno.Visible = True
        End If
    End Sub

    Public Sub address_details(ByVal sp_no As String)
        txtAddHouseNo.Text = ""
        txtAddMobile.Text = ""
        txtAddName.Text = ""
        txtAddPIN.Text = ""
        txtAddStreet.Text = ""
        txtAddVillage.Text = ""
        txtAddPO.Text = ""
        txtAddThana.Text = ""
        txtLandLine.Text = ""
        txtAddEmail.Text = ""
        cmbAddCity.Items.Clear()
        cmbAddDistrict.Items.Clear()

        cmbAddState.SelectedValue = "JH"
        cmbAddCountry.SelectedValue = "IND"
        GetCity(cmbAddState.SelectedValue)
        GetDistrict(cmbAddState.SelectedValue)

        Dim qry_add As String = emp_addrs_detail_qry(sp_no)
        Dim dt_add As DataTable = getRecord(qry_add, con)
        If dt_add.Rows.Count > 0 Then
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_ADDR_TYPE")) Then
                Try
                    cmbAddressType.SelectedValue = dt_add.Rows(0).Item("CCA_ADDR_TYPE")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_HOUSE_NO")) Then
                txtAddHouseNo.Text = dt_add.Rows(0).Item("CCA_HOUSE_NO")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_MOBILE")) Then
                txtAddMobile.Text = dt_add.Rows(0).Item("CCA_MOBILE")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_NAME")) Then
                txtAddName.Text = dt_add.Rows(0).Item("CCA_NAME")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_PIN")) Then
                txtAddPIN.Text = dt_add.Rows(0).Item("CCA_PIN")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_STREET")) Then
                txtAddStreet.Text = dt_add.Rows(0).Item("CCA_STREET")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_LAND_LINE")) Then
                txtLandLine.Text = dt_add.Rows(0).Item("CCA_LAND_LINE")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_EMAIL")) Then
                txtAddEmail.Text = dt_add.Rows(0).Item("CCA_EMAIL")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_CITY")) Then
                Try
                    cmbAddCity.SelectedValue = dt_add.Rows(0).Item("CCA_CITY")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_STATE")) Then
                Try
                    cmbAddState.SelectedValue = dt_add.Rows(0).Item("CCA_STATE")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_COUNTRY")) Then
                Try
                    cmbAddCountry.SelectedValue = dt_add.Rows(0).Item("CCA_COUNTRY")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_VILLAGE")) Then
                txtAddVillage.Text = dt_add.Rows(0).Item("CCA_VILLAGE")
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_PO")) Then
                txtAddPO.Text = dt_add.Rows(0).Item("CCA_PO")
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_THANA")) Then
                txtAddThana.Text = dt_add.Rows(0).Item("CCA_THANA")
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_DISTRICT_CD")) Then
                Try
                    cmbAddDistrict.SelectedValue = dt_add.Rows(0).Item("CCA_DISTRICT_CD")
                Catch ex As Exception

                End Try
            End If

            btnSaveAddress.Visible = False
            'btnUpdateAddress.Visible = True
            'btnUpdateAddress.Enabled = False
        Else
            clearAddress()
            btnSaveAddress.Visible = True
            btnUpdateAddress.Visible = False
        End If
    End Sub

    Public Sub quali_details(ByVal sp_no As String)

        cmbQualType.SelectedValue = 0
        cmbQualification.Items.Clear()
        txtQualRemarks.Text = ""

        Dim qry_quali As String = emp_quali_detail_qry(sp_no)
        Dim dt_quali As DataTable = getRecord(qry_quali, con)
        If dt_quali.Rows.Count > 0 Then
            If Not IsDBNull(dt_quali.Rows(0).Item("cql_qual_type")) Then
                Try
                    cmbQualType.SelectedValue = dt_quali.Rows(0).Item("cql_qual_type")
                Catch ex As Exception

                End Try
            End If
            Dim vQualType As String = cmbQualType.SelectedValue
            FillDropDown(cmbQualification, vQualType)

            If Not IsDBNull(dt_quali.Rows(0).Item("cql_qual_code")) Then
                Try
                    cmbQualification.SelectedValue = dt_quali.Rows(0).Item("cql_qual_code")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt_quali.Rows(0).Item("cql_remarks")) Then
                txtQualRemarks.Text = dt_quali.Rows(0).Item("cql_remarks")
            End If

            ' btnSaveQual.Visible = False
            btnUpdateQual.Visible = True
            'btnUpdateQual.Enabled = False
        Else
            clearQualification()
            btnSaveQual.Visible = True
            btnUpdateQual.Visible = False
        End If


    End Sub
    Public Sub exp_details(ByVal sp_no As String)

        getExpDom()
        getExpLocState()

        txtcompname.Text = ""
        txtstdt.Text = ""
        txtenddt.Text = ""

        Dim qry_exp As String = emp_exp_detail_qry(sp_no)
        Dim dt_exp As DataTable = getRecord(qry_exp, con)
        If dt_exp.Rows.Count > 0 Then
            btnSaveExp.Visible = False
            btnUpdateExp.Visible = True
            btnUpdateExp.Enabled = False
        Else
            clearexperience()
            btnSaveExp.Visible = True
            btnUpdateExp.Visible = False
        End If


    End Sub
    Public Sub nominee_details(ByVal sp_no As String)
        cmbNomRelation.SelectedValue = 0
        txtNomName.Text = ""
        txtNomDOB.Text = ""
        cmbNomPayGrp.SelectedValue = 0
        cmbNomShare.SelectedValue = 0
        txtNomRemarks.Text = ""
        txtNomineeAddress.Text = ""
        Dim qry_nomi As String = emp_nominee_detail_qry(sp_no)
        Dim dt_nomi As DataTable = getRecord(qry_nomi, con)

        If dt_nomi.Rows.Count > 0 Then

            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_relation_cd")) Then
                Try
                    cmbNomRelation.SelectedValue = dt_nomi.Rows(0).Item("ccn_relation_cd")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_nominee_name")) Then
                txtNomName.Text = dt_nomi.Rows(0).Item("ccn_nominee_name")
            End If

            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_pymt_grp")) Then
                Try
                    cmbNomPayGrp.SelectedValue = dt_nomi.Rows(0).Item("ccn_pymt_grp")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_share")) Then
                Try
                    cmbNomShare.SelectedValue = dt_nomi.Rows(0).Item("ccn_share")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_remarks")) Then
                txtNomRemarks.Text = dt_nomi.Rows(0).Item("ccn_remarks")
            End If

            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_nominee_dob")) Then
                txtNomDOB.Text = dt_nomi.Rows(0).Item("ccn_nominee_dob")
            End If

            If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_nominee_address")) Then
                txtNomineeAddress.Text = dt_nomi.Rows(0).Item("ccn_nominee_address")
            End If

            btnSaveNominee.Visible = False
            btnUpdateNominee.Visible = True
            btnUpdateNominee.Enabled = False
        Else
            clearNominee()
            btnSaveNominee.Visible = True
        End If
    End Sub
    Public Function verification_flag(ByVal sp_no As String) As String
        Dim flag As String = ""
        Dim sql As String = emp_detail_qry() + "and CET_SAFETY_PASSNO ='" + sp_no + "'"

        Dim dtv As DataTable = getRecord(sql, con)
        If dtv.Rows.Count > 0 Then


            status_variables()

            If Not IsDBNull(dtv.Rows(0).Item("CET_DOCVER_STATUS")) Then

                If dtv.Rows(0).Item("CET_DOCVER_STATUS") = msg_complete Then        'ADDED FOR VERIFIED DOCUMENTS
                    flag = "Y"
                ElseIf dtv.Rows(0).Item("CET_DOCVER_STATUS") = msg_reject Then   'ADDED FOR REJECTED DOCUMENTS
                    flag = "R"
                Else
                    flag = "N"
                End If
                ' flag = "Y"
            Else
                flag = "N"
            End If

        End If

        Return flag
    End Function
    Public Sub Renewal_profile_details(ByVal sp_no As String)
        txtFName.Text = ""
        txtLName.Text = ""
        txtDOB.Text = ""
        cmbSex.SelectedValue = "0"
        txtFatherName.Text = ""
        txtHusName.Text = ""
        txtPhNo.Text = ""
        txtEmrgNo.Text = ""
        cmbUniqID.SelectedValue = "0"
        txtIdentiFication.Text = ""
        cmbAffirmative.SelectedValue = "0"
        txtUniqIDNo.Text = ""
        cmbWorkArea.SelectedValue = "0"
        TxtSpno.Text = ""
        ddlMedCentre.SelectedValue = "0"

        Dim qry As String = t_cemp_details_tmp_qry() + "where CET_SAFETY_PASSNO='" + sp_no + "' and CET_REQUEST_NO='" + Session("requestnumber") + "'"
        Dim dt As DataTable = getRecord(qry, con)

        If dt.Rows.Count > 0 Then

            If Not IsDBNull(dt.Rows(0).Item("CET_LOCATION_CODE")) Then
                'If ((dt.Rows(0).Item("CET_LOCATION_CODE").ToString = "1003" Or dt.Rows(0).Item("CET_LOCATION_CODE").ToString = "3000") Or (dt.Rows(0).Item("CET_LOCATION_CODE").ToString = "1000" And dt.Rows(0).Item("CET_DEPT_CODE").ToString = "502")) Then
                '    tabSkill.Enabled = False
                'Else
                '    tabSkill.Enabled = True
                'End If

            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_CATEGORY")) Then
                cmbCategory.Items.Clear()
                Dim category As String = dt.Rows(0).Item("CET_CATEGORY")
                cat(category)
                cmbCategory.SelectedValue = category
                'If category <> "" Then
                '    cmbCategory.Enabled = False
                'Else

                'End If
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_FIRSTNAME")) Then
                txtFName.Text = dt.Rows(0).Item("CET_FIRSTNAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_LASTNAME")) Then
                txtLName.Text = dt.Rows(0).Item("CET_LASTNAME")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_DEPT_CODE")) Then   'Added by sneha modak
                Txtdeprt.Text = dt.Rows(0).Item("CET_DEPT_CODE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_DOB")) Then
                txtDOB.Text = dt.Rows(0).Item("CET_DOB")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_GENDER")) Then
                cmbSex.SelectedValue = dt.Rows(0).Item("CET_GENDER")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_MEDICAL_CENTRE")) Then
                ddlMedCentre.SelectedValue = dt.Rows(0).Item("CET_MEDICAL_CENTRE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_FATHER_NAME")) Then
                txtFatherName.Text = dt.Rows(0).Item("CET_FATHER_NAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_SPOUSE_NAME")) Then
                txtHusName.Text = dt.Rows(0).Item("CET_SPOUSE_NAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_PHONE_NO")) Then
                txtPhNo.Text = dt.Rows(0).Item("CET_PHONE_NO")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_EMERGENCY_NO")) Then
                txtEmrgNo.Text = dt.Rows(0).Item("CET_EMERGENCY_NO")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_UNIQUE_ID_VALUE")) Then
                txtUniqIDNo.Text = dt.Rows(0).Item("CET_UNIQUE_ID_VALUE")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_IDENTIFICATION_MARK")) Then
                txtIdentiFication.Text = dt.Rows(0).Item("CET_IDENTIFICATION_MARK")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_AFFIRMATIVE")) Then
                Try
                    cmbAffirmative.SelectedValue = dt.Rows(0).Item("CET_AFFIRMATIVE")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_AREA_OF_WORK")) Then
                Try
                    cmbWorkArea.SelectedValue = dt.Rows(0).Item("CET_AREA_OF_WORK")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_UNIQUE_ID_TYPE")) Then
                Try
                    cmbUniqID.SelectedValue = dt.Rows(0).Item("CET_UNIQUE_ID_TYPE")
                Catch ex As Exception

                End Try
            End If

            If Not IsDBNull(dt.Rows(0).Item("CET_PAN_NO")) Then
                txtPAN.Text = AESEncryption.Decrypt(dt.Rows(0).Item("CET_PAN_NO"), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)

            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_NAME")) Then
                txtAdltName.Text = dt.Rows(0).Item("CET_ADLT_NAME")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_REL")) Then
                Try
                    cmbAdltRelation.SelectedValue = dt.Rows(0).Item("CET_ADLT_REL")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_ADDRESS")) Then
                txtAdltAddress.Text = dt.Rows(0).Item("CET_ADLT_ADDRESS")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_ADLT_MOBILE_NO")) Then
                txtAdltMobile.Text = AESEncryption.Decrypt(dt.Rows(0).Item("CET_ADLT_MOBILE_NO"), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)

            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_NATIONALITY")) Then
                Try
                    cmbNationality.SelectedValue = dt.Rows(0).Item("CET_NATIONALITY")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_AADHAR_NO")) Then
                txtAADHAR.Text = AESEncryption.Decrypt(dt.Rows(0).Item("CET_AADHAR_NO"), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)

            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_EMP_PLACE")) Then
                Try
                    cmbPlaceOfEmployment.SelectedValue = dt.Rows(0).Item("CET_EMP_PLACE")
                Catch ex As Exception

                End Try
            End If
            If Not IsDBNull(dt.Rows(0).Item("CET_RELAY_DATA")) Then
                Try
                    cmbRelayData.SelectedValue = dt.Rows(0).Item("CET_RELAY_DATA")
                Catch ex As Exception

                End Try
            End If
            'END ADD BY PRASUN ON 11032022

            txtuan.Text = dt.Rows(0).Item("CET_UAN_NO").ToString.Trim
            txtip.Text = dt.Rows(0).Item("CET_IP_NO").ToString.Trim

            btnSaveProfile.Visible = False
            btnUpdateProfile.Visible = False

            ''''''''''''''''''''''''''''''''''''''
            btnUpdateProfile.Visible = True

            cmbCategory.Enabled = False
            Txtdeprt.Enabled = False
            TxtSpno.Enabled = False
            txtFName.Enabled = False
            txtLName.Enabled = False
            txtDOB.Enabled = False
            cmbSex.Enabled = False
            cmbWorkArea.Enabled = False
            'ddlMedCentre.Enabled = False
            txtFatherName.Enabled = False
            txtHusName.Enabled = False
            txtIdentiFication.Enabled = False
            cmbAffirmative.Enabled = False
            cmbUniqID.Enabled = False
            txtUniqIDNo.Enabled = False
            If cmbNationality.SelectedValue <> "[Select]" Then
                cmbNationality.Enabled = False

            End If
            If txtAADHAR.Text <> "" Then
                txtAADHAR.Enabled = False

            End If
            If txtPAN.Text <> "" Then
                txtPAN.Enabled = False
            End If


            ''''''''''''''''''''''''''''''''''''''



            TxtSpno.Text = dt.Rows(0).Item("CET_SAFETY_PASSNO")

            'START ADD BY PRASUN ON 11032022



            Lblspno.Visible = True
            TxtSpno.Visible = True
        End If
    End Sub
    Public Sub Renewal_address_details(ByVal sp_no As String)
        txtAddHouseNo.Text = ""
        txtAddMobile.Text = ""
        txtAddName.Text = ""
        txtAddPIN.Text = ""
        txtAddStreet.Text = ""
        txtLandLine.Text = ""
        txtAddEmail.Text = ""
        cmbAddCity.Items.Clear()
        'cmbAddState.SelectedValue = "JH"
        'cmbAddCountry.SelectedValue = "IND"
        'GetCity(cmbAddState.SelectedValue)

        txtAddVillage.Text = ""
        txtAddPO.Text = ""
        txtAddThana.Text = ""
        cmbAddDistrict.Items.Clear()

        GetAddress(sp_no)
        For Each gvrow As GridViewRow In gvAddress.Rows
            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectAddress")
            Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
            If reqno.Value.Trim = Session("requestnumber").ToString Then
                chkbox.Enabled = True
            Else
                chkbox.Enabled = False
            End If

        Next

        Dim qry_add As String = T_CWM_CEMP_ADDRS_qry(sp_no)
        Dim dt_add As DataTable = getRecord(qry_add, con)
        If dt_add.Rows.Count > 0 Then
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_ADDR_TYPE")) Then
                cmbAddressType.SelectedValue = dt_add.Rows(0).Item("CCA_ADDR_TYPE")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_HOUSE_NO")) Then
                txtAddHouseNo.Text = dt_add.Rows(0).Item("CCA_HOUSE_NO")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_MOBILE")) Then
                txtAddMobile.Text = dt_add.Rows(0).Item("CCA_MOBILE")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_NAME")) Then
                txtAddName.Text = dt_add.Rows(0).Item("CCA_NAME")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_PIN")) Then
                txtAddPIN.Text = dt_add.Rows(0).Item("CCA_PIN")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_STREET")) Then
                txtAddStreet.Text = dt_add.Rows(0).Item("CCA_STREET")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_LAND_LINE")) Then
                txtLandLine.Text = dt_add.Rows(0).Item("CCA_LAND_LINE")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_EMAIL")) Then
                txtAddEmail.Text = dt_add.Rows(0).Item("CCA_EMAIL")
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_COUNTRY")) Then
                Try
                    GetCountry()
                    cmbAddCountry.SelectedValue = dt_add.Rows(0).Item("CCA_COUNTRY")
                Catch ex As Exception
                End Try
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_STATE")) Then
                Try
                    GetState()
                    cmbAddState.SelectedValue = dt_add.Rows(0).Item("CCA_STATE")
                Catch ex As Exception
                End Try
            End If
            If Not IsDBNull(dt_add.Rows(0).Item("CCA_CITY")) Then
                Try
                    GetCity(cmbAddState.SelectedValue)
                    cmbAddCity.SelectedValue = dt_add.Rows(0).Item("CCA_CITY")
                Catch ex As Exception
                End Try
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_VILLAGE")) Then
                txtAddVillage.Text = dt_add.Rows(0).Item("CCA_VILLAGE")
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_PO")) Then
                txtAddPO.Text = dt_add.Rows(0).Item("CCA_PO")
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_THANA")) Then
                txtAddThana.Text = dt_add.Rows(0).Item("CCA_THANA")
            End If

            If Not IsDBNull(dt_add.Rows(0).Item("CCA_DISTRICT_CD")) Then
                Try
                    GetDistrict(cmbAddState.SelectedValue)
                    cmbAddDistrict.SelectedValue = dt_add.Rows(0).Item("CCA_DISTRICT_CD")
                Catch ex As Exception

                End Try
            End If

            'btnSaveAddress.Visible = False
        Else
            Dim qry As String = T_CEMP_DETAILS_qry() + "where CED_SAFETY_PASS_NO='" + sp_no + "'"
            Dim dt As DataTable = getRecord(qry, con)
            If dt.Rows.Count > 0 Then
                If Not IsDBNull(dt.Rows(0).Item("CED_ADDRESS1")) Then
                    txtAddHouseNo.Text = dt.Rows(0).Item("CED_ADDRESS1")
                    clearAddress()
                End If
            Else
                clearAddress()
            End If
        End If
    End Sub
    Public Sub Renewal_quali_details(ByVal sp_no As String)
        cmbQualType.SelectedValue = 0
        cmbQualification.Items.Clear()
        txtQualRemarks.Text = ""
        GetQualification(sp_no)
        For Each gvrow As GridViewRow In gvQualification.Rows
            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectQual")
            Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
            If reqno.Value.Trim = Session("requestnumber").ToString Then
                chkbox.Enabled = True
            Else
                chkbox.Enabled = False
            End If
        Next
        Dim qry_quali As String = T_CWM_CEMP_QUALIFICATIONS_qry(sp_no)
        Dim dt_quali As DataTable = getRecord(qry_quali, con)
        If dt_quali.Rows.Count > 0 Then
            If Not IsDBNull(dt_quali.Rows(0).Item("cql_qual_type")) Then
                cmbQualType.SelectedValue = dt_quali.Rows(0).Item("cql_qual_type")
            End If
            Dim vQualType As String = cmbQualType.SelectedValue
            FillDropDown(cmbQualification, vQualType)

            If Not IsDBNull(dt_quali.Rows(0).Item("cql_qual_code")) Then
                cmbQualification.SelectedValue = dt_quali.Rows(0).Item("cql_qual_code")
            End If

            If Not IsDBNull(dt_quali.Rows(0).Item("cql_remarks")) Then
                txtQualRemarks.Text = dt_quali.Rows(0).Item("cql_remarks")
            End If

            ' btnSaveQual.Visible = False

        Else
            clearQualification()

        End If


    End Sub
    Public Sub Renewal_exp_details(ByVal sp_no As String)


        Dim qry_exp As String = emp_exp_detail_qry(sp_no)
        Dim dt_exp As DataTable = getRecord(qry_exp, con)
        If dt_exp.Rows.Count > 0 Then
            GetExp(sp_no)
            'btnSaveExp.Visible = False
            For Each gvrow As GridViewRow In grvExp.Rows
                Dim chkbox As CheckBox = gvrow.FindControl("chkSelectExp")
                Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                If reqno.Value.Trim = Session("requestnumber").ToString Then
                    chkbox.Enabled = True
                Else
                    chkbox.Enabled = False
                End If
            Next
        Else
            clearexperience()

        End If


    End Sub
    Public Sub Renewal_trn_details(ByVal sp_no As String)
        Dim qry_trn As String = T_CWM_CEMP_TRN_qry(sp_no)
        Dim dt_trn As DataTable = getRecord(qry_trn, con)


        clearTraining()
        GetTraining(sp_no)
        'btnSaveTraining.Visible = False
        For Each gvrow As GridViewRow In gvTraining.Rows
            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectTraining")
            Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
            If reqno.Value.Trim = Session("requestnumber").ToString Then
                chkbox.Enabled = True
            Else
                chkbox.Enabled = False
            End If
        Next





    End Sub
    Public Sub Renewal_skill_details(ByVal sp_no As String)
        Dim qry_skill As String = T_CWM_CEMP_SKILL_qry(sp_no)
        Dim dt_skill As DataTable = getRecord(qry_skill, con)
        ' If dt_skill.Rows.Count > 0 Then
        clearSkill()
        'btnSaveSkill.Visible = False
        getskill(sp_no)
        Dim status As String = "N"
        For Each gvrow As GridViewRow In gvSkill.Rows
            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectSkill")
            Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
            If reqno.Value.Trim = Session("requestnumber").ToString Then
                chkbox.Enabled = True
                status = "Y"
            Else
                chkbox.Enabled = False
            End If
        Next
        If status.Equals("N") Then
            btnUpdateSkill.Visible = False
            btnSaveSkill.Visible = True
        Else
            btnUpdateSkill.Visible = True
            btnSaveSkill.Visible = False
        End If

        Dim lb_remarks_PD As New Label
        For Each gvrow As GridViewRow In gvSkill.Rows
            lb_remarks_PD = gvrow.FindControl("lbl_remarks_PD")
            If (lb_remarks_PD.Text.ToString = "Skill Training Failed") Then
                gvrow.Enabled = False
            End If
        Next

        ' Else


        'End If


    End Sub

    'Public Sub Renewal_PV_details(ByVal sp_no As String)


    '    ' getPV(sp_no)
    '    'btnsavepv.Visible = False




    'End Sub
    Public Sub Renewal_AGEDRV_details(ByVal sp_no As String)
        Dim status As String = "N"
        Dim qry_age As String = T_CWM_CEMP_PV_qry(sp_no)
        Dim dt_age As DataTable = getRecord(qry_age, con)
        getagedrv(sp_no)


        'btnsaveage.Visible = False
        If Session("reqtype") = "Renew" Then
            For Each gvrow As GridViewRow In grdage.Rows
                Dim chkbox As CheckBox = gvrow.FindControl("chkSelectage")
                Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                If reqno.Value.Trim = Session("requestnumber").ToString Then
                    status = "Y"
                    chkbox.Enabled = True
                Else
                    chkbox.Enabled = False
                End If
            Next
        End If

        If status.Equals("N") Then

            btnsaveage.Visible = True
            btnupdateage.Visible = False
        Else

            btnsaveage.Visible = False
            btnupdateage.Visible = True
        End If


    End Sub
    Public Sub Renewal_nominee_details(ByVal sp_no As String)
        cmbNomRelation.SelectedValue = 0
        txtNomName.Text = ""
        txtNomDOB.Text = ""
        cmbNomPayGrp.SelectedValue = 0
        cmbNomShare.SelectedValue = 0
        txtNomRemarks.Text = "N"
        txtNomineeAddress.Text = ""
        Dim status As String = String.Empty
        Dim qry_nomi As String = T_CWM_CEMP_NOMINEES_qry(sp_no)
        Dim dt_nomi As DataTable = getRecord(qry_nomi, con)
        GetNominee(sp_no)
        For Each gvrow As GridViewRow In gvNominee.Rows
            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectNominee")
            Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
            If reqno.Value.Trim = Session("requestnumber").ToString Then
                status = "Y"
                chkbox.Enabled = True
            Else
                chkbox.Enabled = False
            End If

        Next
        If status.Equals("Y") Then
            btnSaveNominee.Visible = False
            btnUpdateNominee.Visible = True
        Else
            btnSaveNominee.Visible = True
            btnUpdateNominee.Visible = False
        End If
        'If dt_nomi.Rows.Count > 0 Then

        '    If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_relation_cd")) Then
        '        cmbNomRelation.SelectedValue = dt_nomi.Rows(0).Item("ccn_relation_cd")
        '    End If

        '    If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_nominee_name")) Then
        '        txtNomName.Text = dt_nomi.Rows(0).Item("ccn_nominee_name")
        '    End If

        '    If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_pymt_grp")) Then
        '        cmbNomPayGrp.SelectedValue = dt_nomi.Rows(0).Item("ccn_pymt_grp")
        '    End If
        '    If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_share")) Then
        '        cmbNomShare.SelectedValue = dt_nomi.Rows(0).Item("ccn_share")
        '    End If
        '    If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_remarks")) Then
        '        txtNomRemarks.Text = dt_nomi.Rows(0).Item("ccn_remarks")
        '    End If

        '    If Not IsDBNull(dt_nomi.Rows(0).Item("ccn_nominee_dob")) Then
        '        txtNomDOB.Text = dt_nomi.Rows(0).Item("ccn_nominee_dob")
        '    End If
        '    ' btnSaveNominee.Visible = False

        'Else
        '    clearNominee()

        'End If
    End Sub
#End Region
    Public Sub ReqClick(ByVal Req_No As String)
        Dim sql As String = "select srq_req_no, SPR.SRQ_REQ_TYPE, sprd.srd_emp_cat, SPRD.SRD_EMP_APV_COUNT, SPR.srq_dept_code, SPR.srq_company_cd, SPR.srq_location_cd, to_char(SRQ_CREATED_DT,'dd/MM/yyyy') SRQ_CREATED_DT  from HRACE.T_SP_REQUEST SPR , HRACE.t_sp_request_dtl SPRD  where spr.srq_req_no='" + Req_No + "'  and   SPRD.SRD_REQ_NO=SPR.SRQ_REQ_NO"
        Dim dt As DataTable = getRecord(sql, con)

        If dt.Rows.Count > 0 Then

            If Not IsDBNull(dt.Rows(0).Item("srq_dept_code")) Then
                Txtdeprt.Text = dt.Rows(0).Item("srq_dept_code")
            End If

            If Not IsDBNull(dt.Rows(0).Item("srq_location_cd")) Then
                Loc = dt.Rows(0).Item("srq_location_cd")
            End If

            If Not IsDBNull(dt.Rows(0).Item("srq_req_no")) Then
                lblreq.Text = lblreq.Text
                lblreq.Text = lblreq.Text + dt.Rows(0).Item("srq_req_no")
                Session("requestnumber") = ""
                Session("requestnumber") = dt.Rows(0).Item("srq_req_no")
            End If

            If Not IsDBNull(dt.Rows(0).Item("SRQ_REQ_TYPE")) Then
                Session("requestType") = dt.Rows(0).Item("SRQ_REQ_TYPE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("SRQ_CREATED_DT")) Then
                Session("requestDate") = dt.Rows(0).Item("SRQ_CREATED_DT")
            End If

            For i = 0 To dt.Rows.Count - 1

                If dt.Rows(i).Item("srd_emp_cat") = SV Then
                    lnkSup.Text = lnkSup.Text
                    lnkSup.Text = lnkSup.Text + dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                    Session("supvsr") = dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                ElseIf dt.Rows(i).Item("srd_emp_cat") = WR Then
                    lnkWrk.Text = lnkWrk.Text
                    lnkWrk.Text = lnkWrk.Text + dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                    Session("worker") = dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                ElseIf dt.Rows(i).Item("srd_emp_cat") = DV Then
                    LnkDR.Text = LnkDR.Text
                    LnkDR.Text = LnkDR.Text + dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                    Session("Driver") = dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                ElseIf dt.Rows(i).Item("srd_emp_cat") = FM Then
                    LnkFM.Text = LnkFM.Text
                    LnkFM.Text = LnkFM.Text + dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                    Session("FM") = dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString

                ElseIf dt.Rows(i).Item("srd_emp_cat") = VC Then
                    LnkVC.Text = LnkVC.Text
                    LnkVC.Text = LnkVC.Text + dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                    Session("VC") = dt.Rows(i).Item("SRD_EMP_APV_COUNT").ToString
                End If
            Next

            pnlShw.Visible = False
            Pnlcategory.Visible = True



            If Session("requestType") = "SPN" Then
                empView()
                PnlSafetyRenewal.Style.Add("display", "none")

                Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            ElseIf Session("requestType") = "SPR" Then
                empView()
                PnlSafetyRenewal.Style.Remove("display")
                RenewalProcessGridview(Session("requestnumber"))

            End If



        End If
    End Sub
    Public Function RequestNumberCreateDate(ByVal days As String) As Date
        'Dim sql1 As String = "SELECT TO_CHAR(ADD_MONTHS( TO_DATE('" + days + "','DD/MM/YYYY'), 10 ) - 1,'DD/MM/YYYY') as dateResult FROM DUAL"
        Dim sql1 As String = "SELECT TO_CHAR(ADD_MONTHS( TO_DATE('" + days + "','DD/MM/YYYY'), 3 ) - 1,'DD/MM/YYYY') as dateResult FROM DUAL"
        Dim dt1 As DataTable = getRecord(sql1, con)
        Dim dateResult As Date = DateTime.ParseExact(dt1.Rows(0).Item("dateResult").ToString, "dd/MM/yyyy", CultureInfo.InvariantCulture)
        ''
        Return dateResult
    End Function
#Region "Link Functions"
    Protected Sub lnk_Request_No_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim loc As String = ""
        Dim gvrow As GridViewRow
        gvrow = CType(sender, LinkButton).Parent.Parent
        Dim Req_No As String = CType(gvrow.FindControl("lnk_Request_No"), LinkButton).Text
        Dim Req_type As String = CType(gvrow.FindControl("lbl_RQ"), Label).Text
        lnkNoti.Visible = False
        Session("Req_type") = Req_type
        ReqClick(Req_No)

        If Session("Req_type") = SPN Then
            cmbCategory.Enabled = True
        ElseIf Session("Req_type") = SPR Then
            RenewalProcessGridview(Req_No)

            PanelEmp.Style.Add("display", "none")
            PnlSafetyRenewal.Style.Remove("display")
            'cmbCategory.Enabled = False
            Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            If Today >= dateResult Then
                PnlSafetyRenewal.Style.Add("display", "none")
                lblpagemsg.Text = " The Request Number has expired.To get details of employee click  on safety pass number."
            Else

            End If
            rejectReStatus()
        End If

    End Sub
    Protected Sub lblreq_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblreq.Click

        If Session("requestnumber") <> "" Then
            Dim req As String = Session("requestnumber")
            Dim en_ReqNo As String = ""
            en_ReqNo = b64encode(req)


            Response.Redirect("ospprofileChecklist.aspx?ReqNo=" + en_ReqNo)
        Else
            Response.Redirect("ospEmpDetails.aspx")
        End If
    End Sub
    Protected Sub lnkSup_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkSup.Click


        If Session("Req_type") = SPN Then
            PnlSafetyRenewal.Style.Add("display", "none")
            btnSaveProfile.Visible = True
            'GetCategory(SV_desc)
            'sandeep
            GetCategory(String.Format("'{0}','{1}','{2}','{3}'", SV_desc, SH_desc, SF_desc, SA_desc))
            'end
            empView()
            Dim count As Integer = Session("supvsr")
            If Session("requestnumber") <> "" And count <> 0 Then

                tabcontainer1.Style.Remove("display")
                tabcontainer1.ActiveTabIndex = 0
                'stop_profileEntry(count, SV, Session("requestnumber"))
                'sandeep
                stop_profileEntry(count, String.Format("'{0}','{1}','{2}','{3}'", SV, SH, SF, SA), Session("requestnumber"))
                'end
                clearAll()
            ElseIf count = 0 Then

                tabcontainer1.Style.Add("display", "none")
                Exit Sub
            End If
        ElseIf Session("Req_type") = SPR Then
            Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            If Today >= dateResult Then
                PnlSafetyRenewal.Style.Add("display", "none")
                ShowMessage(" The Request Number has expired. You cannot add safety pass number")
                Exit Sub
            Else
            End If


            ShowMessage("Please Enter the safety pass Number for renewal process")
            PnlSafetyRenewal.Style.Remove("display")
        End If
    End Sub
    Protected Sub LnkDR_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session("Req_type") = SPN Then
            PnlSafetyRenewal.Style.Add("display", "none")
            btnSaveProfile.Visible = True
            'sandeep
            GetCategory(String.Format("'{0}','{1}','{2}'", DV_desc, DA_desc, DH_desc))
            'end
            empView()
            Dim count As Integer = Session("Driver")
            If Session("requestnumber") <> "" And count <> 0 Then

                tabcontainer1.Style.Remove("display")
                tabcontainer1.ActiveTabIndex = 0
                stop_profileEntry(count, String.Format("'{0}','{1}','{2}'", DV, DA, DH), Session("requestnumber"))
                clearAll()

            ElseIf count = 0 Then

                tabcontainer1.Style.Add("display", "none")
                Exit Sub
            End If

        ElseIf Session("Req_type") = SPR Then
            Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            If Today >= dateResult Then
                PnlSafetyRenewal.Style.Add("display", "none")
                ShowMessage(" The Request Number has expired. You cannot add safety pass number")
                Exit Sub
            Else
            End If

            ShowMessage("Please Enter the safety pass Number for renewal process")
            PnlSafetyRenewal.Style.Remove("display")
        End If
    End Sub
    Protected Sub LnkFM_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkFM.Click
        If Session("Req_type") = SPN Then
            PnlSafetyRenewal.Style.Add("display", "none")
            btnSaveProfile.Visible = True
            GetCategory(String.Format("'{0}','{1}'", FM_desc, FA_desc))
            empView()
            Dim count As Integer = Session("FM")
            If Session("requestnumber") <> "" And count <> 0 Then

                tabcontainer1.Style.Remove("display")
                tabcontainer1.ActiveTabIndex = 0
                'sandeep
                stop_profileEntry(count, String.Format("'{0}','{1}'", FM, FA), Session("requestnumber"))
                clearAll()
            ElseIf count = 0 Then

                tabcontainer1.Style.Add("display", "none")
                Exit Sub
            End If
        ElseIf Session("Req_type") = SPR Then

            Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            If Today >= dateResult Then
                PnlSafetyRenewal.Style.Add("display", "none")
                ShowMessage(" The Request Number has expired. You cannot add safety pass number")
                Exit Sub
            Else
            End If

            ShowMessage("Please Enter the safety pass Number for renewal process")
            PnlSafetyRenewal.Style.Remove("display")
        End If
    End Sub
    Protected Sub lnkWrk_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        If Session("Req_type") = SPN Then
            PnlSafetyRenewal.Style.Add("display", "none")
            btnSaveProfile.Visible = True
            'sandeep
            GetCategory(String.Format("'{0}','{1}'", WR_desc, WA_desc))
            'end
            empView()
            Dim count As Integer = Session("worker")
            If Session("requestnumber") <> "" And count <> 0 Then

                tabcontainer1.Style.Remove("display")
                tabcontainer1.ActiveTabIndex = 0
                'sandeep
                stop_profileEntry(count, String.Format("'{0}','{1}'", WR, WA), Session("requestnumber"))
                clearAll()
            ElseIf count = 0 Then

                tabcontainer1.Style.Add("display", "none")
                Exit Sub
            End If
        ElseIf Session("Req_type") = SPR Then
            Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            If Today >= dateResult Then
                PnlSafetyRenewal.Style.Add("display", "none")
                ShowMessage(" The Request Number has expired. You cannot add safety pass number")
                Exit Sub
            Else
            End If

            ShowMessage("Please Enter the safety pass Number for renewal process")
            PnlSafetyRenewal.Style.Remove("display")
        End If
    End Sub
    Protected Sub LnkVC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LnkVC.Click
        If Session("Req_type") = SPN Then
            PnlSafetyRenewal.Style.Add("display", "none")
            GetCategory(String.Format("'{0}','{1}'", VC_desc, VA_desc))
            empView()
            Dim count As Integer = Session("VC")
            If Session("requestnumber") <> "" And count <> 0 Then

                tabcontainer1.Style.Remove("display")
                tabcontainer1.ActiveTabIndex = 0
                'sandeep
                stop_profileEntry(count, String.Format("'{0}','{1}'", VC, VA), Session("requestnumber"))
                clearAll()
            ElseIf count = 0 Then

                tabcontainer1.Style.Add("display", "none")
                Exit Sub
            End If
        ElseIf Session("Req_type") = SPR Then
            Dim dateResult As Date = RequestNumberCreateDate(Session("requestDate"))
            If Today >= dateResult Then
                PnlSafetyRenewal.Style.Add("display", "none")
                ShowMessage(" The Request Number has expired. You cannot add safety pass number")
                Exit Sub
            Else
            End If


            ShowMessage("Please Enter the safety pass Number for renewal process")
            PnlSafetyRenewal.Style.Remove("display")
        End If
    End Sub
    Protected Sub lnk_spno_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim loc As String = ""
        Dim gvrow As GridViewRow
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        If Session("comp_code") Is Nothing Then
            Response.Redirect("http://tatasteel.co.in/")
        End If

        Dim locCheck = CheckWireFrameLoc()

        Try
            ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where acm_type='SKJNTVTI' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                If (Session("requestType") <> "SPR" And Session("requestType") <> "SPN") Or (Txtdeprt.Text.Trim = "502" And comp_cd = "1000") Then
                    chk_waive.Visible = False
                    drp_waiveoff.Visible = False
                    lbl_waiveoff.Visible = False
                    lbl_waivereason.Visible = False
                    chk_waive.Checked = True
                    drptypeassessment.Enabled = False
                    spn_msg.Visible = False
                    drptypeassessment.Visible = False
                    Label2.Visible = False
                    spn_type.Visible = False


                Else
                    chk_waive.Visible = True
                    drp_waiveoff.Visible = True
                    lbl_waiveoff.Visible = True
                    lbl_waivereason.Visible = True
                    drptypeassessment.Enabled = True
                    spn_msg.Visible = True
                    drptypeassessment.Visible = True
                    Label2.Visible = True
                    spn_type.Visible = True
                End If
                If locCheck = False Then
                    divFlowChart.Visible = False
                Else
                    divAllFlowChart.Visible = True
                End If
            Else
                chk_waive.Visible = False
                drp_waiveoff.Visible = False
                lbl_waiveoff.Visible = False
                lbl_waivereason.Visible = False
                chk_waive.Checked = True
                drptypeassessment.Enabled = False
                spn_msg.Visible = False
                drptypeassessment.Visible = False
                Label2.Visible = False
                spn_type.Visible = False

                divAllFlowChart.Visible = False
            End If
        Catch ex As CookieException
        End Try

        gvrow = CType(sender, LinkButton).Parent.Parent
        Dim sp_no As String = CType(gvrow.FindControl("lnk_spno"), LinkButton).Text
        Dim category As String = GetCategorySafety(sp_no)
        Session("categorysaf") = category

        If locCheck = False Then
            If tabcontainer1.Style("display") = "none" Then
                tabcontainer1.Style.Remove("display")
            End If
        Else
            tabcontainer1.Style.Add("display", "none")
        End If
        'If tabcontainer1.Style("display") = "none" Then
        '    tabcontainer1.Style.Remove("display")
        'End If

        ibtnClosesubmit.Enabled = True
        clearAddress()
        clearSkill()
        clearexperience()
        clearTraining()
        clearQualification()
        clearpv()
        clearagedrv()
        clearmed()
        Dim flag As String = verification_flag(sp_no)
        If flag = "N" Or flag = "R" Then
            'CType(gvrow.FindControl("lnk_spno"), LinkButton).ForeColor = Color.LightGoldenrodYellow
            profile_details(sp_no)
            'address_details(sp_no)
            quali_details(sp_no)
            exp_details(sp_no)
            nominee_details(sp_no)
            GetAddress(sp_no)
            GetQualification(sp_no)
            GetExp(sp_no)
            GetNominee(sp_no)
            getskill(sp_no)
            GetTraining(sp_no)
            'getPV(sp_no)
            getagedrv(sp_no)
            getvaccination(sp_no)
            lbl_mednote.Text = "Medical verification is not applicable as document verification not done yet"
            fupdlfitnesscer.Enabled = False
            fupdlundertake.Enabled = False
            fupdlwcc.Enabled = False
            btnsavemed.Visible = False
            '   show_photo(sp_no)
            ' btnUpload.Enabled = True
            Btnreset.Visible = False
            btnUpdateProfile.Visible = True

            LblAddMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblAddMsg.ForeColor = Color.Green

            LblQualiMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblQualiMsg.ForeColor = Color.Green

            LblNomiMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblNomiMsg.ForeColor = Color.Green

            LblExpMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblExpMsg.ForeColor = Color.Green

            LblSkillMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblSkillMsg.ForeColor = Color.Green

            LblTrnMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblTrnMsg.ForeColor = Color.Green

            'LblPvMsg.Text = "For any Updation of values check on the checkbox and then press update"
            'LblPvMsg.ForeColor = Color.Green

            LblAgeMsg.Text = "For any Updation of values check on the checkbox and then press update"
            LblAgeMsg.ForeColor = Color.Green

        Else
            profile_details(sp_no)
            address_details(sp_no)
            quali_details(sp_no)
            nominee_details(sp_no)
            GetAddress(sp_no)
            GetQualification(sp_no)
            GetNominee(sp_no)
            GetExp(sp_no)
            getskill(sp_no)
            GetTraining(sp_no)
            'getPV(sp_no)
            getagedrv(sp_no)
            getvaccination(sp_no)
            clearmed()
            getfitnesscer(sp_no)
            Dim fitnessstatus As String = getfitnessstatus(sp_no)
            If fitnessstatus.Equals("TUNFIT") Or fitnessstatus.Equals("FUFIT") Then
                getfitnesscer(sp_no)
                Dim statuscount As Integer = 0
                statuscount = getnoofmedicalver(sp_no)

                lbl_mednote.Text = "Please provide special medical verification details"

                fupdlfitnesscer.Enabled = True
                fupdlundertake.Enabled = True
                fupdlwcc.Enabled = True
                If btnsavemed.Visible = False Then
                    btnsavemed.Visible = True
                End If
                For Each gvrowmed As GridViewRow In gvmed.Rows
                    Dim chk As CheckBox = gvrowmed.FindControl("chkSelectmed")
                    chk.Enabled = True
                Next
                'Else
                '    lbl_mednote.Text = "You are not able to submit medical verification report.Please contact NTTF."
                '    fupdlfitnesscer.Enabled = False
                '    fupdlundertake.Enabled = False
                '    fupdlwcc.Enabled = False
                '    If btnsavemed.Visible = True Then
                '        btnsavemed.Visible = False
                '    End If
                '    If btnupdatemed.Visible = True Then
                '        btnupdatemed.Visible = False
                '    End If
                '    For Each gvrowmed As GridViewRow In gvmed.Rows
                '        Dim chk As CheckBox = gvrowmed.FindControl("chkSelectmed")
                '        chk.Enabled = False
                '    Next


            ElseIf fitnessstatus.Equals("FIT") Then

                lbl_mednote.Text = "Medical verification not required-----medically fit."
                btnsavemed.Visible = False
                btnupdatemed.Visible = False
                fupdlfitnesscer.Enabled = False
                fupdlundertake.Enabled = False
                fupdlwcc.Enabled = False
            ElseIf fitnessstatus.Equals("PUNFIT") Then
                getfitnesscer(sp_no)
                lbl_mednote.Text = "Medical verification is not applicable---permanent unfit."
                btnsavemed.Visible = False
                btnupdatemed.Visible = False
                fupdlfitnesscer.Enabled = False
                fupdlundertake.Enabled = False
                fupdlwcc.Enabled = False
                For Each gvrowmed As GridViewRow In gvmed.Rows
                    Dim chk As CheckBox = gvrowmed.FindControl("chkSelectmed")
                    chk.Enabled = False
                Next

                '    ElseIf fitnessstatus.Equals("FUFIT") Then
                '    getfitnesscer(sp_no)
                '        lbl_mednote.Text = "Medical verification is not applicable---follow up fit."
                '        btnsavemed.Visible = False
                '    btnupdatemed.Visible = False
                '    fupdlfitnesscer.Enabled = False
                '    fupdlundertake.Enabled = False
                '    fupdlwcc.Enabled = False
                '    For Each gvrowmed As GridViewRow In gvmed.Rows
                '        Dim chk As CheckBox = gvrowmed.FindControl("chkSelectmed")
                '        chk.Enabled = False
                '    Next
            End If
            If fitnessstatus.Equals("NA") Then
                lbl_mednote.Text = "Medical verification is not applicable--no medical record found."
                btnsavemed.Visible = False
                btnupdatemed.Visible = False
                fupdlfitnesscer.Enabled = False
                fupdlundertake.Enabled = False
                fupdlwcc.Enabled = False
                gvmed.DataSource = Nothing
                gvmed.DataBind()
            End If

            ' show_photo(sp_no)
            btnUpdateAddress.Visible = False
            btnUpdateProfile.Visible = False
            btnUpdateQual.Visible = False
            btnUpdateNominee.Visible = False
            btnUpdateSkill.Visible = False
            btnUpdateTraining.Visible = False
            btnupdateage.Visible = False
            'btnupdatepv.Visible = False
            Btnreset.Visible = False

            For i As Integer = 0 To gvAddress.Rows.Count - 1
                DirectCast(gvAddress.Rows(i).Cells(0).FindControl("chkSelectAddress"), CheckBox).Enabled = False
            Next

            For j As Integer = 0 To gvQualification.Rows.Count - 1
                DirectCast(gvQualification.Rows(j).Cells(0).FindControl("chkSelectQual"), CheckBox).Enabled = False
            Next

            For k As Integer = 0 To gvNominee.Rows.Count - 1
                DirectCast(gvNominee.Rows(k).Cells(0).FindControl("chkSelectNominee"), CheckBox).Enabled = False
            Next

            For M As Integer = 0 To grvExp.Rows.Count - 1
                DirectCast(grvExp.Rows(M).Cells(0).FindControl("chkSelectExp"), CheckBox).Enabled = False
            Next

            For N As Integer = 0 To gvSkill.Rows.Count - 1
                DirectCast(gvSkill.Rows(N).Cells(0).FindControl("chkSelectSkill"), CheckBox).Enabled = False
            Next

            For P As Integer = 0 To gvTraining.Rows.Count - 1
                DirectCast(gvTraining.Rows(P).Cells(0).FindControl("chkSelectTraining"), CheckBox).Enabled = False
            Next

            'For Q As Integer = 0 To gvpv.Rows.Count - 1
            '    DirectCast(gvpv.Rows(Q).Cells(0).FindControl("chkSelectPV"), CheckBox).Enabled = False
            'Next
            For R As Integer = 0 To grdage.Rows.Count - 1
                DirectCast(grdage.Rows(R).Cells(0).FindControl("chkSelectage"), CheckBox).Enabled = False
            Next
            'btnUpload.Enabled = True

            LblAddMsg.Text = "No Updation of values can be done"
            LblAddMsg.ForeColor = Color.Red

            LblQualiMsg.Text = "No Updation of values can be done"
            LblQualiMsg.ForeColor = Color.Red



            LblNomiMsg.Text = "No Updation of values can be done"
            LblNomiMsg.ForeColor = Color.Red

            LblExpMsg.Text = "No Updation of values can be done"
            LblExpMsg.ForeColor = Color.Red

            btnUpdateExp.Visible = False

            LblSkillMsg.Text = "No Updation of values can be done"
            LblSkillMsg.ForeColor = Color.Red

            btnUpdateSkill.Visible = False

            LblTrnMsg.Text = "No Updation of values can be done"
            LblTrnMsg.ForeColor = Color.Red

            btnUpdateTraining.Visible = False

            'LblPvMsg.Text = "No Updation of values can be done"
            'LblPvMsg.ForeColor = Color.Red

            LblAgeMsg.Text = "No Updation of values can be done"
            LblAgeMsg.ForeColor = Color.Red


            'btnupdatepv.Visible = False
            btnSaveTraining.Visible = False
            btnSaveExp.Visible = False

        End If
        Dim vDBO As String = CType(gvrow.FindControl("hfEmpViewDOB"), HiddenField).Value
        Dim dob As Date = DateTime.ParseExact(vDBO, "dd/MM/yyyy", CultureInfo.InvariantCulture)
        Dim age As Double = GetAge(dob)
        Dim trainee As String = cmbCategory.Items(0).Value.Substring(0, 1) + "A"

        If age >= 18 And age <= 20 Then
            cmbCategory.Items.FindByValue(trainee).Enabled = True
            cmbCategory.SelectedValue = trainee
            cmbCategory.Enabled = False
        Else
            cmbCategory.Items.FindByValue(trainee).Enabled = False
            cmbCategory.Enabled = True
        End If

        Dim lb_remarks_PD As New Label
        For Each gvrowsk As GridViewRow In gvSkill.Rows
            lb_remarks_PD = gvrowsk.FindControl("lbl_remarks_PD")
            If (lb_remarks_PD.Text.ToString = "Skill Training Failed") Then
                gvrowsk.Enabled = False
            End If
        Next
        ShowhideRenewSkillCert()

        If locCheck Then
            Dim dtReq_MedCategory As New DataTable
            Dim dtAssetype As New DataTable

            dtReq_MedCategory = CheckReq_MedCategory(Session("requestnumber"), sp_no)
            dtAssetype = CheckAssesmnetType(Session("requestnumber"), sp_no)

            If dtReq_MedCategory.Rows.Count > 0 Then
                If (dtReq_MedCategory.Rows(0).Item("CET_REQ_CATEGORY").ToString = "1") Then

                    If dtAssetype.Rows.Count > 0 Then
                        If ((dtReq_MedCategory.Rows(0).Item("CET_MEDICAL_CENTRE").ToString = "O") And (dtAssetype.Rows(0).Item("CCST_ASSESSMENT_TYPE").ToString = "D")) Then
                            divFlowChart.Visible = True
                            divFlowChart3.Visible = False
                            divFlowChart1.Visible = False
                            divFlowChart2.Visible = False
                            PaintWireFrame(sp_no)
                        ElseIf ((dtReq_MedCategory.Rows(0).Item("CET_MEDICAL_CENTRE").ToString = "A") And (dtAssetype.Rows(0).Item("CCST_ASSESSMENT_TYPE").ToString = "D")) Then
                            divFlowChart1.Visible = True
                            divFlowChart3.Visible = False
                            divFlowChart.Visible = False
                            divFlowChart2.Visible = False
                            PaintWireFrame1(sp_no)
                        ElseIf ((dtReq_MedCategory.Rows(0).Item("CET_MEDICAL_CENTRE").ToString = "O") And (dtAssetype.Rows(0).Item("CCST_ASSESSMENT_TYPE").ToString = "T")) Then
                            divFlowChart2.Visible = True
                            divFlowChart3.Visible = False
                            divFlowChart.Visible = False
                            divFlowChart1.Visible = False
                            PaintWireFrame2(sp_no)
                        ElseIf ((dtReq_MedCategory.Rows(0).Item("CET_MEDICAL_CENTRE").ToString = "A") And (dtAssetype.Rows(0).Item("CCST_ASSESSMENT_TYPE").ToString = "T")) Then
                            divFlowChart3.Visible = True
                            divFlowChart2.Visible = False
                            divFlowChart.Visible = False
                            divFlowChart1.Visible = False
                            PaintWireFrame3(sp_no)
                        Else
                            divFlowChart3.Visible = True
                            divFlowChart2.Visible = False
                            divFlowChart.Visible = False
                            divFlowChart1.Visible = False
                            PaintWireFrame3(sp_no)
                        End If
                    Else
                        divFlowChart3.Visible = True
                        divFlowChart1.Visible = False
                        divFlowChart2.Visible = False
                        divFlowChart.Visible = False
                        PaintWireFrame3(sp_no)
                    End If
                Else
                    divFlowChart3.Visible = True
                    divFlowChart1.Visible = False
                    divFlowChart2.Visible = False
                    divFlowChart.Visible = False
                    PaintWireFrame3(sp_no)
                End If
            Else
                divFlowChart3.Visible = True
                divFlowChart1.Visible = False
                divFlowChart2.Visible = False
                divFlowChart.Visible = False
                PaintWireFrame3(sp_no)
            End If
        Else
            divFlowChart3.Visible = True
            divFlowChart1.Visible = False
            divFlowChart2.Visible = False
            divFlowChart.Visible = False
            PaintWireFrame3(sp_no)
        End If
    End Sub
    Private Function GetCategorySafety(ByVal spno As String) As String
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim category As String
        Try
            ls_sql = "select CET_CATEGORY from t_cemp_details_tmp where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", spno))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                category = dt.Rows(0).Item("CET_CATEGORY")
            End If
        Catch ex As Exception

        End Try
        Return category
    End Function
    Private Sub getvaccination(ByVal sp As String)
        'WI2689: get vaccination details entered during profile entry
        'created by : Avik Mukherjee
        'Created on: 18-Aug-2021
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim cnt As Integer = 0
        Try
            ls_sql = "select VACT_SP_NO,VACT_SP_NO,VACT_VAC_DOS,VACT_VAC_NAME,to_char(VACT_VAC_DOS_DT,'dd/mm/yyyy') VACT_VAC_DOS_DT, VACT_VAC_CERTNO,VACT_EXEMP,VACT_EXEMP_CERTNO from hrace.t_cemp_vaccination_tmp tmp where tmp.VACT_SP_NO=:VACT_SP_NO and tmp.VACT_STATUS='Y' order by VACT_VAC_DOS"
            cmd = New OracleCommand(ls_sql, con)
            'cmd.Parameters.Add(New OracleParameter(":VACT_REQ_NO", Session("requestnumber").trim))
            cmd.Parameters.Add(New OracleParameter(":VACT_SP_NO", sp))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While cnt < dt.Rows.Count
                    If Not IsDBNull(dt.Rows(0).Item("VACT_VAC_DOS")) Then

                        If dt.Rows(cnt).Item("VACT_VAC_DOS") = "1" Then
                            drp_vaccinedose.SelectedValue = "1"
                        ElseIf dt.Rows(cnt).Item("VACT_VAC_DOS") = "2" Then
                            drp_vaccinedose.SelectedValue = "2"
                            drp_vaccinedose.Enabled = False
                        Else
                            drp_vaccinedose.SelectedValue = "0"
                        End If

                    End If
                    If Not IsDBNull(dt.Rows(0).Item("VACT_VAC_NAME")) Then
                        drp_vaccinename.SelectedValue = dt.Rows(0).Item("VACT_VAC_NAME")
                        drp_vaccinename.Enabled = False
                    End If
                    If Not IsDBNull(dt.Rows(cnt).Item("VACT_VAC_DOS_DT")) Then
                        If dt.Rows(cnt).Item("VACT_VAC_DOS") = "1" Then
                            txt_fdose.Text = dt.Rows(cnt).Item("VACT_VAC_DOS_DT")
                        End If
                        If dt.Rows(cnt).Item("VACT_VAC_DOS") = "2" Then
                            txt_sdose.Text = dt.Rows(cnt).Item("VACT_VAC_DOS_DT")
                        End If
                    End If
                    If Not IsDBNull(dt.Rows(0).Item("VACT_EXEMP")) Then
                        If dt.Rows(0).Item("VACT_EXEMP") = "Y" Then
                            chk_exem.Checked = True
                        Else
                            chk_exem.Checked = False
                        End If

                    End If
                    If Not IsDBNull(dt.Rows(0).Item("VACT_VAC_CERTNO")) Then
                        If dt.Rows(0).Item("VACT_VAC_CERTNO") = "0" Then
                            lnk_vacdoc.Visible = False
                            hdvacsrlno.Value = dt.Rows(0).Item("VACT_VAC_CERTNO")
                        Else
                            lnk_vacdoc.Visible = True
                            hdvacsrlno.Value = dt.Rows(0).Item("VACT_VAC_CERTNO")
                        End If

                    Else
                        lnk_vacdoc.Visible = False
                    End If
                    If Not IsDBNull(dt.Rows(0).Item("VACT_EXEMP_CERTNO")) Then
                        If dt.Rows(0).Item("VACT_EXEMP_CERTNO") = "0" Then
                            lnk_exemp.Visible = False
                            hd_exemp.Value = dt.Rows(0).Item("VACT_EXEMP_CERTNO")
                        Else
                            lnk_exemp.Visible = True
                            hd_exemp.Value = dt.Rows(0).Item("VACT_EXEMP_CERTNO")
                        End If

                    Else
                        lnk_exemp.Visible = False
                    End If
                    'If Not IsDBNull(dt.Rows(0).Item("VACT_RTPCR_DATE")) Then
                    '    txt_rtprc.Text = dt.Rows(0).Item("VACT_RTPCR_DATE")
                    'End If
                    'If Not IsDBNull(dt.Rows(0).Item("VACT_VAC_CERTNO")) Then
                    '    hdvacsrlno.Value = dt.Rows(0).Item("VACT_VAC_CERTNO")
                    'End If
                    'If Not IsDBNull(dt.Rows(0).Item("VACT_RTPCR_CERTNO")) Then
                    '    hdrtpcrsrlno.Value = dt.Rows(0).Item("VACT_RTPCR_CERTNO")
                    'End If



                    If drp_vaccinedose.SelectedValue <> "0" Then
                        chk_exem.Checked = False
                        updt_exemp.Enabled = False
                        chk_exem.Enabled = False
                        If drp_vaccinedose.SelectedValue = "1" Then
                            txt_fdose.Enabled = True
                            txt_sdose.Enabled = True
                        End If
                        If drp_vaccinedose.SelectedValue = "2" Then
                            txt_fdose.Enabled = True
                            txt_sdose.Enabled = True
                        End If
                    End If
                    cnt = cnt + 1
                End While

            End If
        Catch ex As Exception
            ShowMessage(ex.Message)
        End Try
    End Sub
    Private Function getnoofmedicalver(ByVal spno As String) As Integer
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim count As Integer = 0
        Try
            ls_sql = "select count(CMTT_SAFETY_PASS_NO) cnt from t_cwm_med_dtl where CMTT_SAFETY_PASS_NO=:CMTT_SAFETY_PASS_NO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CMTT_SAFETY_PASS_NO", spno))
            dt = getRecord(cmd, con)
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            If dt.Rows.Count > 0 Then
                count = Convert.ToUInt64(dt.Rows(0).Item("cnt").ToString)

            End If
        Catch ex As Exception

        End Try
        Return count
    End Function
    Private Function getfitnessstatus(ByVal spno As String) As String
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim fitstatus As String = String.Empty
        Try
            ls_sql = "select CMH_FIT_STATUS from T_CWM_CEMP_MEDICAL_HDR where CMH_TEST_DT=(select max(CMH_TEST_DT) from T_CWM_CEMP_MEDICAL_HDR where CMH_SAFETY_PASS_NO=:CMH_SAFETY_PASS_NO) and CMH_SAFETY_PASS_NO=:CMH_SAFETY_PASS_NO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CMH_SAFETY_PASS_NO", spno))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                fitstatus = dt.Rows(0).Item("CMH_FIT_STATUS")
            Else
                fitstatus = "NA"
            End If
        Catch ex As Exception

        End Try
        Return fitstatus
    End Function
    Private Sub getagedrv(ByVal spno As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Try
            ls_sql = "select b.DM_NAME ""DOB"",b.DM_DOC_ID ""DOBDOCID"",c.DM_NAME ""DRV"",d.DM_NAME ""PASS"",c.DM_DOC_ID ""DRVDOCID"",d.DM_DOC_ID ""PASSDOCID"",a.CET_REQUEST_NO from t_cemp_details_tmp a,t_document_master b,t_document_master c,t_document_master d where b.DM_DOC_ID=a.CET_DOB_CERT_NO and c.DM_DOC_ID(+)=a.CET_DRV_CERT_NO and d.DM_DOC_ID(+)=a.CET_PASS_CERT_NO and a.CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO ORDER BY TO_NUMBER(CET_REQUEST_NO) DESC"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                grdage.DataSource = dt
                grdage.DataBind()



                If dt.Rows(0).Item("DOBDOCID").ToString <> "" Then
                    hdfageold.Value = dt.Rows(0).Item("DOBDOCID").ToString.Trim
                    imbageold.Visible = True
                    chkageold.Visible = True
                Else
                    hdfageold.Value = ""
                    imbageold.Visible = False
                    chkageold.Visible = False
                End If
                If dt.Rows(0).Item("DRVDOCID").ToString <> "" Then
                    hdfdriverold.Value = dt.Rows(0).Item("DRVDOCID").ToString.Trim
                    imbdriverold.Visible = True
                    chkdriverold.Visible = True
                Else
                    hdfdriverold.Value = ""
                    imbdriverold.Visible = False
                    chkdriverold.Visible = False
                End If
                If dt.Rows(0).Item("PASSDOCID").ToString <> "" Then
                    hdfpassold.Value = dt.Rows(0).Item("PASSDOCID").ToString.Trim
                    imgpassold.Visible = True
                    chkpassold.Visible = True
                Else
                    hdfpassold.Value = ""
                    imgpassold.Visible = False
                    chkpassold.Visible = False
                End If
                Dim status As String = "N"
                For Each gvrow As GridViewRow In grdage.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectage")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        status = "Y"
                    End If
                Next
                If status = "Y" Then
                    hdfpassold.Value = ""
                    imgpassold.Visible = False
                    chkpassold.Visible = False

                    hdfdriverold.Value = ""
                    imbdriverold.Visible = False
                    chkdriverold.Visible = False

                    hdfageold.Value = ""
                    imbageold.Visible = False
                    chkageold.Visible = False

                End If

                btnupdateage.Visible = True
                btnsaveage.Visible = False
            Else
                grdage.DataSource = Nothing
                grdage.DataBind()

                hdfageold.Value = ""
                imbageold.Visible = False
                chkageold.Visible = False

                hdfdriverold.Value = ""
                imbdriverold.Visible = False
                chkdriverold.Visible = False

                hdfpassold.Value = ""
                imgpassold.Visible = False
                chkpassold.Visible = False

            End If
        Catch ex As Exception

        End Try
    End Sub

    'Protected Sub chkSelectPV(ByVal sender As Object, ByVal e As System.EventArgs)
    '    Dim vIsRowSelected As Boolean = False
    '    'clearQualification()
    '    Try
    '        Dim gvrow As GridViewRow
    '        gvrow = CType(sender, CheckBox).Parent.Parent
    '        If CType(gvrow.FindControl("chkSelectPV"), CheckBox).Checked = True Then
    '            vIsRowSelected = True
    '            Dim vPVID As String = CType(gvrow.FindControl("hidpvid"), HiddenField).Value
    '            Session("PVID") = vPVID

    '            Dim vValidFrom As String = gvrow.Cells(1).Text
    '            Dim vValidTo As String = gvrow.Cells(2).Text
    '            Dim vcertname As String = CType(gvrow.FindControl("lnkdownloadpv"), LinkButton).Text
    '            Dim vcertid As String = CType(gvrow.FindControl("hidpvcerno"), HiddenField).Value

    '            txtstdtpv.Text = vValidFrom
    '            txtenddtpv.Text = vValidTo
    '            lblcertpvname.Text = vcertname
    '            btnsavepv.Visible = False
    '            Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
    '            If status.Equals("Y") Then
    '                btnupdatepv.Visible = False
    '                btnupdatepv.Enabled = False
    '            Else
    '                btnupdatepv.Visible = True
    '                btnupdatepv.Enabled = True
    '            End If

    '            hidcertnopv.Value = vcertid
    '        Else
    '            btnupdatepv.Enabled = False
    '            clearpv()

    '        End If

    '    Catch ex As Exception
    '        Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
    '        Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
    '        Dim strErrMsg As String = ex.Message.ToString.Substring(0)
    '        ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
    '    End Try
    'End Sub
    'Protected Sub btnupdatepv_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnupdatepv.Click
    '    Dim ls_sql As String = String.Empty
    '    Dim cmd As OracleCommand
    '    Dim dt As New DataTable
    '    If txtstdtpv.Text.Trim = "__/__/____" Then
    '        ShowMessage("Please enter valid from date")
    '        Exit Sub
    '    End If
    '    If txtenddtpv.Text.Trim = "__/__/____" Then
    '        ShowMessage("Please enter valid to date")
    '        Exit Sub
    '    End If
    '    If fupdlpv.HasFile = True Then


    '        Dim contentType As String = fupdlpv.PostedFile.ContentType
    '        If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
    '            If (fupdlpv.PostedFile.ContentLength > 512000) Then
    '                ShowMessage("Your file size is " + (fupdlpv.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
    '                Exit Sub
    '            End If
    '        Else
    '            ShowMessage("Please Upload pdf file only")
    '            Exit Sub
    '        End If
    '    End If

    '    Try
    '        ls_sql = "update T_CWM_PV_DTL_TMP set CPDT_ST_DT=to_date(:CPDT_ST_DT,'dd/mm/yyyy'),CPDT_END_DT=to_date(:CPDT_END_DT,'dd/mm/yyyy') where CPDT_PV_ID=:CPDT_PV_ID"
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()

    '        End If
    '        cmd = New OracleCommand(ls_sql, con)
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_ST_DT", txtstdtpv.Text.Trim))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_END_DT", txtenddtpv.Text.Trim))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_PV_ID", Convert.ToInt64(Session("PVID"))))
    '        cmd.ExecuteNonQuery()
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()

    '        End If
    '        ls_sql = "update t_cemp_details_tmp set CET_PV_ISSUED_ON=to_date(:CET_PV_ISSUED_ON,'dd/mm/yyyy'),CET_PV_VALID_TILL=to_date(:CET_PV_VALID_TILL,'dd/mm/yyyy'),CET_PV_RENEWAL_DT=to_date(:CET_PV_RENEWAL_DT,'dd/mm/yyyy') where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
    '        cmd = New OracleCommand(ls_sql, con)
    '        cmd.Parameters.Add(New OracleParameter(":CET_PV_ISSUED_ON", txtstdtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CET_PV_VALID_TILL", txtenddtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CET_PV_RENEWAL_DT", txtstdtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
    '        cmd.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
    '        cmd.ExecuteNonQuery()

    '        If fupdlpv.HasFile = True Then
    '            Dim cmdfilepv As New OracleCommand
    '            Using fs As Stream = fupdlpv.PostedFile.InputStream
    '                Using br As BinaryReader = New BinaryReader(fs)
    '                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

    '                    If con.State = ConnectionState.Open Then
    '                        con.Close()
    '                    End If
    '                    Dim filename As String = Path.GetFileName(fupdlpv.PostedFile.FileName)

    '                    ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
    '                    If con.State = ConnectionState.Closed Then
    '                        con.Open()

    '                    End If
    '                    cmdfilepv.CommandText = ls_sql
    '                    cmdfilepv.Connection = con
    '                    cmdfilepv.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidcertnopv.Value))
    '                    cmdfilepv.Parameters.Add(New OracleParameter(":DM_NAME", filename))

    '                    cmdfilepv.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
    '                    'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
    '                    cmdfilepv.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
    '                    cmdfilepv.ExecuteNonQuery()
    '                    If con.State = ConnectionState.Open Then
    '                        con.Close()
    '                    End If

    '                End Using
    '            End Using
    '        End If
    '        Dim ls_chkPV As String = String.Empty
    '        Dim cmd_chkPV As OracleCommand
    '        Dim dt_chkPV As New DataTable
    '        Try
    '            ls_chkPV = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='PV' and SDV_VERF_FLAG='N'"
    '            If con.State = ConnectionState.Closed Then
    '                con.Open()
    '            End If
    '            cmd_chkPV = New OracleCommand(ls_chkPV, con)
    '            cmd_chkPV.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
    '            cmd_chkPV.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim))
    '            dt_chkPV = getRecord(cmd_chkPV, con)
    '            If dt_chkPV.Rows.Count > 0 Then
    '                updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "PV")
    '            End If
    '        Catch ex As Exception

    '        End Try

    '        getPV(TxtSpno.Text.Trim)
    '        ShowMessage("Police verification updated successfully")
    '        Session.Remove("polverenddt")

    '        clearpv()
    '    Catch ex As Exception

    '    End Try

    'End Sub
    Private Sub clearpv()
        'txtstdtpv.Text = "__/__/____"
        'txtenddtpv.Text = "__/__/____"

        'lblcertpvname.Text = ""
        'Session.Remove("PVID")
        'hidcertnopv.Value = ""
    End Sub

    'Protected Sub txtstdtpv_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtstdtpv.TextChanged
    '    Dim ls_sql As String = String.Empty
    '    Dim dt As New DataTable
    '    Dim cmd As OracleCommand
    '    Try
    '        ls_sql = "select TO_CHAR(add_months(to_date('" + txtstdtpv.Text.Trim + "','dd/mm/yyyy'),12)-1,'dd/mm/yyyy') enddt from dual"
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()
    '        End If
    '        cmd = New OracleCommand(ls_sql, con)
    '        dt = getRecord(cmd, con)
    '        If dt.Rows.Count > 0 Then
    '            Session("polverenddt") = futureDate(txtstdtpv.Text)
    '        End If
    '    Catch ex As Exception

    '    End Try
    'End Sub
    Public Function futureDate(ByVal Getdate As String) As String

        Dim PVvalue As String = ""
        Dim dtPV As DataTable = clmClass.get_codetype("PVV", comp_cd)
        If dtPV.Rows.Count > 0 Then
            PVvalue = dtPV.Rows(0).Item("ctm_value")
        End If
        '12
        Dim sql As String = "SELECT TO_CHAR(ADD_MONTHS( TO_DATE('" + Getdate + "','DD/MM/YYYY'), " + PVvalue + " ) - 1,'DD/MM/YYYY') as dateResult FROM DUAL"
        Dim dt As DataTable = getRecord(sql, con)
        Dim dateResult As String = dt.Rows(0).Item("dateResult")
        Return dateResult
    End Function
    ''' <summary>
    ''' Added function for checking if medical data is present for safety pass number at the time of skill entry.
    ''' </summary>
    ''' <param name="SPNO"></param>
    ''' <param name="SPREQNO"></param>
    ''' <param name="LOCCODE"></param>
    ''' <param name="FlagCode"></param>
    ''' <returns></returns>
    Public Function checkMedicalExists(ByVal SPNO As String, ByVal SPREQNO As String, ByVal LOCCODE As String, ByVal FlagCode As String) As Integer

        Dim res As Integer = 0
        Dim count As String = ""
        Try
            Dim sql As String = " select count(*) cnt from hrace.T_CWM_MEDICAL_HDR_TMP where CMT_SAFETY_PASS_NO=:pSPNO and CMT_REQUEST_NO=:pSPREQNO and CMT_COMP_CODE=:pLOCCODE and CMT_DEL_FLAG=:pFlagCode "
            Dim cmd As New OracleCommand(sql, con)
            cmd.Parameters.AddWithValue(":pSPNO", SPNO)
            cmd.Parameters.AddWithValue(":pSPREQNO", SPREQNO)
            cmd.Parameters.AddWithValue(":pLOCCODE", LOCCODE)
            cmd.Parameters.AddWithValue(":pFlagCode", FlagCode)

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            count = cmd.ExecuteScalar().ToString()

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If count <> "0" Then
                res = 1
            Else
                res = 0
            End If
        Catch ex As Exception

        End Try
        Return res
    End Function


    Protected Sub btnSaveSkill_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveSkill.Click


        Dim spno As String = TxtSpno.Text.Trim.ToString()

        Dim sqlSkill As String = ""
        Dim vSkillID As String = ""
        Dim certskill As String = String.Empty
        Dim vErrorCount As Integer = 0
        Dim filename As String = String.Empty

        Dim vSkilledTrades As String = ""
        Dim vOtherSkilledTrades As String = ""
        Dim vskillassessment As String = String.Empty
        Dim vendorst As String = "N"
        Dim waivetag As String = "N"
        Dim waivetagreason As String = String.Empty
        Dim Decl_check As String = IIf(chkTermsCondition.Checked, "Y", "N") ' added by prasun on 03012022 'WI6447

        'WI6447: User must accept the terms and condition
        ' added by prasun on 03012022
        If Decl_check = "N" Then
            ShowMessage("Please accept the terms and conditions")
            Exit Sub
        End If
        'end added by prasun on 03012022

        Try
            Dim SPReqNumber As String = Session("requestnumber").trim
            Dim SPReqLocCode As String = Session("Comp_code").trim
            Dim SPReqType As String = getSPReqType(SPReqNumber)
            Dim MedChkLocation As Integer = 0
            Dim MedChkOFSP As Integer = 0
            Dim MedicalExists As Integer = 0

            MedChkLocation = getMedChkLocation("MEDFITCHKL", SPReqLocCode, SPReqType, "Y")

            Dim locCheck = CheckWireFrameLoc()
            Dim dtReq_Category As Boolean = False
            dtReq_Category = ChecReqCategory(SPReqNumber, spno)

            If MedChkLocation = 1 Then
                If locCheck = True And Session("requestType") = "SPN" And drptypeassessment.SelectedValue.Trim = "D" And dtReq_Category = True Then
                Else
                    MedicalExists = checkMedicalExists(spno, SPReqNumber, SPReqLocCode, "N")
                    If MedicalExists = 1 Then
                        MedChkOFSP = getMedChkOFSPFIT(spno, SPReqNumber, SPReqLocCode, "N", "FIT")

                        If MedChkOFSP = 0 Then

                            ShowMessage("*You cannot save/update skills certification details for safety pass number " + spno + "  as his /her medical test result is fail. Please arrange to get the medical test done again.")
                            Exit Sub
                        End If
                    Else
                        ShowMessage("*You cannot save/update skills certification details for safety pass number " + spno + "  as his/ her medical examination result is pending. Please wait for result or try after some time. You can also check the status from portal.")
                        Exit Sub
                    End If
                End If
            End If

        Catch ex As Exception
        End Try

        If drptypeassessment.SelectedValue = "0" And chk_waive.Checked = False And chk_waive.Visible = True Then
            ShowMessage("Please select skill assessment type")
            Exit Sub
        End If
        If chk_waive.Checked = True And chk_waive.Visible = True Then
            If drp_waiveoff.SelectedValue = "0" Then
                ShowMessage("Please choose skill waiver off reason")
                Exit Sub
            Else
                If drp_waiveoff.SelectedValue = "MSD/Emergency" Then
                    Dim ls_sqlsk As String = String.Empty
                    Dim cmdwaiver As OracleCommand
                    Dim dtwaiver As New DataTable
                    Try
                        'WI5073: Restrict vendor not to allow Skill waiver off for which skill waiver off already taken in previous new/renewal of safety pass, created by:Avik Mukherjee, created on: 17-Nov-2021
                        ls_sqlsk = "select ccs_comp_code from hrace.t_cwm_cemp_skill sk where sk.ccs_safety_pass_no=:ccs_safety_pass_no and  sk.ccs_waive_off_resn='MSD/Emergency' and sk.ccs_waive_off='Y' and sk.ccs_validity_date='31-Dec-9999'"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmdwaiver = New OracleCommand(ls_sqlsk, con)
                        cmdwaiver.Parameters.Add(New OracleParameter(":ccs_safety_pass_no", TxtSpno.Text.Trim))
                        dtwaiver = getRecord(cmdwaiver, con)
                        If dtwaiver.Rows.Count > 0 Then
                            ShowMessage("Skill Waiver of for MSD/emergency already approved in previous application from location" + dtwaiver.Rows(0).Item("ccs_comp_code") + ".In this application proper skill certification is required.")
                            Exit Sub

                        End If
                    Catch ex As Exception

                    End Try
                End If
            End If
        Else
            ' drp_waiveoff.SelectedValue = "0"
        End If
        'START ADD BY PRASUN CHAKRABORTY 24122021 'WI6447: Input box validation
        Dim waive_days As Integer = 0
        If chk_waive.Checked = True And chk_waive.Visible = True Then
            If drp_waiveoff.SelectedValue <> "0" Then

                Dim strLoginLocDtls As String
                strLoginLocDtls = "  select distinct ACM_COMPANY_CODE || ' - ' || ACM_REMARKS as Loccode from HRACE.t_cwm_action_mapping where ACM_TYPE = 'WAVLOC' and ACM_FLAG = 'Y' AND ACM_COMPANY_CODE in ('" + Session("Comp_code") + "') order by Loccode"

                Dim dtListofLocations As DataTable

                dtListofLocations = getRecord(strLoginLocDtls, con)
                If dtListofLocations.Rows.Count > 0 Then
                    Dim strWaveOfDays As String = ""
                    Dim sqlWaveOfDays As String
                    sqlWaveOfDays = "  select CTM_SEQ from hrace.t_cemp_type_master t1 where t1.ctm_type='SKW' and t1.CTM_TYPE_DESC = '" + drp_waiveoff.SelectedValue + "' and substr(t1.CTM_TYPE_CODE,5,4) = '" + Session("Comp_code") + "'"

                    Dim dtWaveOfDays As DataTable

                    dtWaveOfDays = getRecord(sqlWaveOfDays, con)
                    If dtWaveOfDays.Rows.Count > 0 Then
                        strWaveOfDays = dtWaveOfDays.Rows(0).Item("CTM_SEQ").ToString
                    Else
                        strWaveOfDays = "365"
                    End If

                    If txt_WAIVE_DAYS.Text.Trim().Length = 0 Then
                        ShowMessage("Please provide waiver off days")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) <= 0 Then
                        ShowMessage("Waiver off days should be greater than 0")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) > strWaveOfDays Then
                        ShowMessage("Waiver off days should not be greater than " + strWaveOfDays + "")
                        Exit Sub
                    Else
                        waive_days = CType(txt_WAIVE_DAYS.Text.Trim(), Integer)
                    End If
                Else

                    If txt_WAIVE_DAYS.Text.Trim().Length = 0 Then
                        ShowMessage("Please provide waiver off days")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) <= 0 Then
                        ShowMessage("Waiver off days should be greater than 0")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) > 365 Then
                        ShowMessage("Waiver off days should not be greater than 365")
                        Exit Sub
                    Else
                        waive_days = CType(txt_WAIVE_DAYS.Text.Trim(), Integer)
                    End If
                End If
            End If
        End If
        'END  ADD BY PRASUN CHAKRABORTY 24122021
        If drptypeassessment.Enabled = True And drptypeassessment.SelectedValue = "0" And chk_waive.Checked = False And chk_waive.Visible = True Then
            ShowMessage("Please provide skill assessment type")
            Exit Sub
        Else
            If drptypeassessment.SelectedValue <> "0" And chk_waive.Checked = False And chk_waive.Visible = True Then
            Else

                drptypeassessment.SelectedValue = "0"
            End If

        End If
        vSkilledTrades = ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-"))
        If (ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) = "SKTD0029" Or ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) = "SKTD0028") Then
            ShowMessage("trade as other/E&P is now obsolate.Please do not select trade as other/E&P")
            btnSaveSkill.Enabled = False
            Exit Sub
        Else
            btnSaveSkill.Enabled = True

        End If
        If FileUploadSkill.HasFile = False And chkskilold.Checked = False Then

            If (vSkilledTrades <> "SKTD0028" And vSkilledTrades <> "SKTD0029" And (chk_waive.Visible = True And chk_waive.Checked = False)) Then
                'ShowMessage("Please Upload File")
                'Exit Sub
            ElseIf (vSkilledTrades = "SKTD0028" Or vSkilledTrades = "SKTD0029") Then

            Else
                '***************** comment code ********************************'
                If Session("reqtype") <> "Renew" Then
                    Dim chkskillforEP As String = "N"
                    Dim ls_sql As String = String.Empty
                    Dim dtep As New DataTable
                    ls_sql = "select ACM_COMPANY_CODE,ACM_CATEGORY from t_cwm_action_mapping where ACM_TYPE='SKE' and ACM_FLAG='Y' and ACM_COMPANY_CODE='" + comp_cd + "'"
                    dtep = getRecord(ls_sql, con)
                    If dtep.Rows.Count > 0 Then
                        If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1000" And (dtep.Rows(0).Item("ACM_CATEGORY") = Txtdeprt.Text.Trim.ToString) Then
                            chkskillforEP = "Y"
                        End If
                        If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "3000" Or dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1003" Then
                            chkskillforEP = "Y"

                        End If
                    Else
                        chkskillforEP = "N"
                    End If
                    'ShowMessage("SNTI given Skill Certificate is mandatory. So, please attach.")
                    If chkskillforEP = "Y" Then
                    Else
                        ShowMessage("TSL given skill certificate for the selected Trade is mandatory.")
                        Exit Sub
                    End If

                Else
                    Dim ls_sqlsk As String = String.Empty
                    Dim cmd_sk As OracleCommand
                    Dim dt_sk As New DataTable
                    Try
                        ls_sqlsk = "select ACM_CATEGORY from hrace.T_CWM_ACTION_MAPPING  where ACM_COMPANY_CODE=:ACM_COMPANY_CODE And ACM_TYPE ='SKC' and ACM_FLAG='Y' and ACM_CATEGORY='SKC'"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmd_sk = New OracleCommand(ls_sqlsk, con)
                        cmd_sk.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))

                        dt_sk = getRecord(cmd_sk, con)

                        If dt_sk.Rows.Count > 0 Then

                            '''''''''''''checking for vendor''''''''''''
                            ls_sqlsk = "Select ACM_TYPE from hrace.t_cwm_action_mapping where ACM_CATEGORY=:ACM_CATEGORY and ACM_FLAG='N' and ACM_TYPE='SKC'"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            cmd_sk = New OracleCommand(ls_sqlsk, con)
                            cmd_sk.Parameters.Add(New OracleParameter(":ACM_CATEGORY", vVencode))
                            dt_sk.Clear()
                            dt_sk = getRecord(cmd_sk, con)
                            If dt_sk.Rows.Count > 0 Then
                                vendorst = "Y"
                            Else
                                If vendorst = "N" Then
                                    '''''''''''''checking for department''''''''
                                    ls_sqlsk = "select CET_DEPT_CODE from hrace.t_cemp_details_tmp where CET_REQUEST_NO=:CET_REQUEST_NO and CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO"
                                    If con.State = ConnectionState.Closed Then
                                        con.Open()
                                    End If
                                    cmd_sk = New OracleCommand(ls_sqlsk, con)
                                    cmd_sk.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
                                    cmd_sk.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
                                    dt_sk.Clear()
                                    dt_sk = getRecord(cmd_sk, con)
                                    Dim dept As String = String.Empty
                                    If dt_sk.Rows.Count > 0 Then
                                        dept = dt_sk.Rows(0).Item("CET_DEPT_CODE")
                                        '''''''''checking for department exist''''''''''''
                                        ls_sqlsk = "Select ACM_TYPE from hrace.t_cwm_action_mapping where ACM_CATEGORY=:ACM_CATEGORY and ACM_FLAG='N' and ACM_TYPE='SKC'"
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If
                                        cmd_sk = New OracleCommand(ls_sqlsk, con)
                                        cmd_sk.Parameters.Add(New OracleParameter(":ACM_CATEGORY", dept))
                                        dt_sk.Clear()
                                        dt_sk = getRecord(cmd_sk, con)
                                        If dt_sk.Rows.Count > 0 Then
                                        Else
                                            Dim chkskillforEP As String = "N"
                                            Dim ls_sql As String = String.Empty
                                            Dim dtep As New DataTable
                                            ls_sql = "select ACM_COMPANY_CODE,ACM_CATEGORY from t_cwm_action_mapping where ACM_TYPE='SKE' and ACM_FLAG='Y' and ACM_COMPANY_CODE='" + comp_cd + "'"
                                            dtep = getRecord(ls_sql, con)
                                            If dtep.Rows.Count > 0 Then
                                                If dtep.Rows.Count > 0 Then
                                                    If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1000" And (dtep.Rows(0).Item("ACM_CATEGORY") = Txtdeprt.Text.Trim.ToString) Then
                                                        chkskillforEP = "Y"
                                                    End If
                                                    If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "3000" Or dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1003" Then
                                                        chkskillforEP = "Y"

                                                    End If
                                                Else
                                                    chkskillforEP = "N"
                                                End If
                                            End If
                                            If chkskillforEP = "Y" Then
                                            Else
                                                ShowMessage("TSL given skill certificate for the selected Trade is mandatory.")
                                                Exit Sub
                                            End If



                                        End If
                                    End If
                                    vendorst = "N"
                                End If
                                ' ShowMessage("TSL given skill certificate for the selected Trade is mandatory.")
                                'Exit Sub
                            End If

                            '''''''''''''''''''''''''''''''''''''''''''

                        End If


                    Catch ex As Exception

                    End Try

                End If
                '***************************************************************'
            End If
        ElseIf FileUploadSkill.HasFile = True And chkskilold.Checked = True Then
            ShowMessage("choose either file upload or check previous upload documents option for skill")
            Exit Sub
        ElseIf FileUploadSkill.HasFile = True And chkskilold.Checked = False Then
            filename = Path.GetFileName(FileUploadSkill.PostedFile.FileName)
            Dim contentType As String = FileUploadSkill.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (FileUploadSkill.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (FileUploadSkill.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        Try
            vErrorCount = CheckSkillMandatoryFields()
            If vErrorCount > 0 Then
                tblSkillErrorLst.Visible = True
                ' mpAddSkill.Show()
                Exit Sub
            Else
                tblSkillErrorLst.Visible = False
            End If
            If FileUploadSkill.HasFile = True Or chkskilold.Checked = True Then
                certskill = TrnCWESKILLSeqNo("")
            Else
                certskill = "0"
            End If

            If (vSkilledTrades = "SKTD0029") Then
                vOtherSkilledTrades = txtOthSkillTrade.Text.ToString.Trim.Replace("'", "''")
                vskillassessment = "NA"
                sendmailtoagencyskill(spno)
            ElseIf (vSkilledTrades = "SKTD0028") Then

                vskillassessment = drp_skillassess.SelectedValue.ToString
                If vskillassessment = "" Or vskillassessment = "NA" Then
                    ShowMessage("Please select skill set for assessment")
                    Exit Sub
                End If
            Else
                vOtherSkilledTrades = "NA"
                vskillassessment = "NA"
            End If
            updateprevskillvalidity(spno)
            If chk_waive.Checked And chk_waive.Visible = True Then
                waivetag = "Y"
                waivetagreason = drp_waiveoff.SelectedValue
            ElseIf chk_waive.Checked = False And chk_waive.Visible = True Then
                waivetag = "N"
                waivetagreason = String.Empty
                If drptypeassessment.SelectedValue = "0" Then
                    ShowMessage("Please select assessment type")
                    Exit Sub
                End If
            End If

            If waivetag = "N" And drptypeassessment.SelectedValue = "0" And drptypeassessment.Visible = True Then
                ShowMessage("Some issue occurs please try after sometimes")
                Exit Sub
            End If
            If waivetag = "Y" And drptypeassessment.SelectedValue <> "0" And drptypeassessment.Visible = True Then
                ShowMessage("Some issue occurs please refresh your application")
                Exit Sub
            End If

            ' --------------------Souvik Begins 1

            pop_comp_cd_stp()

            'Try
            '    If Session("Comp_code").ToString().Trim() = Session("Comp_cd_stop_same_trade").ToString().Trim() And drptypeassessment.SelectedValue = "0" Then
            '        ShowMessage("Please Select Assessment Type")
            '        Exit Sub
            '    End If
            'Catch ex As Exception
            '    'ShowMessage("Please Select Assessment Type")
            '    'Exit Sub
            'End Try

            Try
                If Session("Comp_code").ToString().Trim() = Session("Comp_cd_stop_same_trade").ToString().Trim() Then


                    Dim dt_chk As New DataTable

                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If

                    'Dim qry_chk As String = "select to_char(CCST_ASSESSMENT_DATE,'YYYY/MM/DD') from T_CWM_CEMP_SKILL_TMP where CCST_COMP_CODE='" & Session("Comp_code").ToString().Trim() & "' and CCST_SAFETY_PASS_NO='" & spno.Trim() & "' and CCST_SKILL_CD='" & cmbSkSkill.SelectedValue.ToString().Trim() & "' and CCST_SKTD_CP_CD='" & ddlSkillTrade.SelectedValue.ToString().Trim() & "' and CCST_CREATED_BY='" & Session("VendCode").ToString().Trim() & "' and CCST_ASSESSMENT_DATE is not null and (sysdate-CCST_ASSESSMENT_DATE)<=15"
                    Dim qry_chk As String = "select s.CCST_ASSESSMENT_DATE, c.TCD_STRT_DT from HRACE.T_CWM_CEMP_SKILL_TMP s, hrps.t_td_clm_doc@ace_iris c where s.CCST_SAFETY_PASS_NO=c.TCD_SP_NO and s.CCST_SKTD_CP_CD=c.TCD_CLM_SKILL_CD and s.CCST_COMP_CODE='" & Session("Comp_code").ToString().Trim() & "' and s.CCST_SAFETY_PASS_NO='" & spno.Trim() & "' and s.CCST_SKILL_CD='" & cmbSkSkill.SelectedValue.ToString().Trim() & "' and s.CCST_SKTD_CP_CD='" & ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) & "' and s.CCST_CREATED_BY='" & Session("VendCode").ToString().Trim() & "' and s.CCST_ASSESSMENT_DATE is not null and (sysdate-s.CCST_ASSESSMENT_DATE)<=15 and UPPER(c.TCD_CERT_CATEG)='FAIL' and c.TCD_STRT_DT is not null and c.TCD_STRT_DT>s.CCST_ASSESSMENT_DATE"
                    Dim cmd_chk = New OracleCommand(qry_chk, con)

                    dt_chk.Clear()
                    dt_chk = getRecord(cmd_chk, con)
                    If dt_chk.Rows.Count > 0 Then
                        ShowMessage("Can.t Apply for the Selected Skill and Trade Now. Plesae Try After 15 Days of Your Skill Assessment Date.")
                        Exit Sub
                    End If

                End If
            Catch ex As Exception

            End Try

            Dim flg_stat As String = "null"

            'Code to ByPass a skill request in which the candidate has already passed
            Try
                If Session("Comp_code").ToString().Trim() = Session("Comp_cd_stop_same_trade").ToString().Trim() Then
                    Dim dt_chk2 As New DataTable

                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If

                    'Dim qry_chk2 As String = "select * from HRACE.T_SP_REQUEST r, HRACE.T_CWM_CEMP_SKILL_TMP s where r.SRQ_COMPANY_CD=s.CCST_COMP_CODE and r.SRQ_REQ_NO=s.CCST_REQ_NO and r.srq_req_type='SPR' and s.ccst_assessment_type in ('D','T') and s.ccst_req_flag is null and s.CCST_COMP_CODE='" & Session("Comp_code").ToString().Trim() & "' and s.CCST_SAFETY_PASS_NO='" & spno.Trim() & "' and s.CCST_SKTD_CP_CD='" & ddlSkillTrade.SelectedValue.ToString().Trim() & "' and s.CCST_CREATED_BY='" & Session("VendCode").ToString().Trim() & "' and s.CCST_ASSESSMENT_DATE is not null"
                    Dim qry_chk2 As String = "select * from HRACE.T_CWM_CEMP_SKILL s where s.CCS_COMP_CODE='" & Session("Comp_code").ToString().Trim() & "' and s.CCS_SAFETY_PASS_NO='" & spno.Trim() & "' and s.CCS_SKTD_CP_CD='" & ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) & "' and s.CCS_VALIDITY_DATE > sysdate and s.CCS_DELETE_FLG is null"
                    Dim cmd_chk2 = New OracleCommand(qry_chk2, con)
                    dt_chk2.Clear()
                    dt_chk2 = getRecord(cmd_chk2, con)

                    If dt_chk2.Rows.Count > 0 Then
                        flg_stat = "'B'"
                    End If
                End If
            Catch ex As Exception

            End Try

            ' --------------------Souvik Ends 1

            Dim SPRSkCertReqd As String = ""
            If (Session("renewSkillCert") = "Yes") Then
                SPRSkCertReqd = "Y"
            Else
                SPRSkCertReqd = "N"
            End If

            Dim cmdinsertskill As New OracleCommand
            'sqlSkill = "insert into t_cwm_cemp_skill_TMP (ccst_req_no,ccst_comp_code, ccst_safety_pass_no,ccst_loc_code , ccst_skill_type_cd, ccst_skill_cd, ccst_remarks,CCSt_CERT_NO,ccst_created_by, ccst_created_dt  ) values(:ccst_req_no,:ccst_comp_code,:ccst_safety_pass_no,:ccst_loc_code,:ccst_skill_type_cd,:ccst_skill_cd, :ccst_remarks,:CCSt_CERT_NO,:ccst_created_by,sysdate)"
            'sqlSkill = "insert into t_cwm_cemp_skill_TMP (ccst_req_no,ccst_comp_code, ccst_safety_pass_no,ccst_loc_code , ccst_skill_type_cd, ccst_skill_cd, ccst_remarks,CCSt_CERT_NO,ccst_created_by, ccst_created_dt, CCST_SKTD_CP_CD, CCST_SKTD_OTH_REMRK,CCST_VALIDITY_DATE,CCST_SKTP_CP_CD,CCST_ASSESSMENT_TYPE,CCST_WAIVE_OFF,CCST_WAIVE_OFF_RESN) values(:ccst_req_no,:ccst_comp_code,:ccst_safety_pass_no,:ccst_loc_code,:ccst_skill_type_cd,:ccst_skill_cd, :ccst_remarks,:CCSt_CERT_NO,:ccst_created_by,sysdate,:CCST_SKTD_CP_CD,:CCST_SKTD_OTH_REMRK,to_date('31/12/9999','DD/MM/YYYY'),:CCST_SKTP_CP_CD,:CCST_ASSESSMENT_TYPE,:CCST_WAIVE_OFF,:CCST_WAIVE_OFF_RESN)"
            'below line edit BY PRASUN CHAKRABORTY 24122021 'WI6447:
            sqlSkill = "insert into t_cwm_cemp_skill_TMP (ccst_req_no,ccst_comp_code, ccst_safety_pass_no,ccst_loc_code , ccst_skill_type_cd, ccst_skill_cd, ccst_remarks,CCSt_CERT_NO,ccst_created_by, ccst_created_dt, CCST_SKTD_CP_CD, CCST_SKTD_OTH_REMRK,CCST_VALIDITY_DATE,CCST_SKTP_CP_CD,CCST_ASSESSMENT_TYPE,CCST_WAIVE_OFF,CCST_WAIVE_OFF_RESN, CCST_WAIVE_DAYS, CCST_DECL_CHECK, CCST_REQ_FLAG, CCST_SKILL_ATT) values(:ccst_req_no,:ccst_comp_code,:ccst_safety_pass_no,:ccst_loc_code,:ccst_skill_type_cd,:ccst_skill_cd, :ccst_remarks,:CCSt_CERT_NO,:ccst_created_by,sysdate,:CCST_SKTD_CP_CD,:CCST_SKTD_OTH_REMRK,to_date('31/12/9999','DD/MM/YYYY'),:CCST_SKTP_CP_CD,:CCST_ASSESSMENT_TYPE,:CCST_WAIVE_OFF,:CCST_WAIVE_OFF_RESN, :CCST_WAIVE_DAYS,:CCST_DECL_CHECK," & flg_stat & ",:CCST_SKILL_ATT)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmdinsertskill.CommandText = sqlSkill
            cmdinsertskill.Connection = con
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_req_no", Session("requestnumber")))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_comp_code", Session("Comp_code")))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_safety_pass_no", spno))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_loc_code", ddlSKAss.SelectedValue))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_skill_type_cd", cmbSkSkillType.SelectedValue))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_skill_cd", cmbSkSkill.SelectedValue))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_remarks", txtSkRemarks.Text.ToString.Trim.Replace("'", "''")))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCSt_CERT_NO", certskill))
            cmdinsertskill.Parameters.Add(New OracleParameter(":ccst_created_by", Session("VendCode")))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_SKTD_CP_CD", ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-"))))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_SKTD_OTH_REMRK", vOtherSkilledTrades))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_SKTP_CP_CD", vskillassessment))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_ASSESSMENT_TYPE", drptypeassessment.SelectedValue.Trim))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_WAIVE_OFF", waivetag))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_WAIVE_OFF_RESN", waivetagreason))
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_WAIVE_DAYS", IIf(waive_days > 0, waive_days, DBNull.Value))) 'ADD BY PRASUN CHAKRABORTY 24122021 'WI6447
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_DECL_CHECK", Decl_check)) 'ADD BY PRASUN CHAKRABORTY 03012022 'WI6447
            cmdinsertskill.Parameters.Add(New OracleParameter(":CCST_SKILL_ATT", SPRSkCertReqd))
            cmdinsertskill.ExecuteNonQuery()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            'cmdinsertskill.Parameters.Add(New OracleParameter(":ccs_created_by", vUserID))
            'sqlSkill = sqlSkill + vCompCD + " ','"
            'sqlSkill = sqlSkill + vSPNo + "','"
            'sqlSkill = sqlSkill + ddlSKAss.SelectedValue + "','"
            'sqlSkill = sqlSkill + cmbSkSkillType.SelectedValue + "','"
            'sqlSkill = sqlSkill + cmbSkSkill.SelectedValue + "','"
            'sqlSkill = sqlSkill + txtSkRemarks.Text.ToString.Trim.Replace("'", "''") + "','"
            'sqlSkill = sqlSkill + certskill + "','"
            'sqlSkill = sqlSkill + vUserID + "',"
            'sqlSkill = sqlSkill + "SYSDATE" + ")"
            'SaveData(sqlSkill, con)
            ''''''''''''''''''''''''attach file with skill''''''''''''''''''''''''''''''''''
            If FileUploadSkill.HasFile = True Then
                Dim cmdfileskill As New OracleCommand
                Dim ls_sql As String = String.Empty
                filename = Path.GetFileName(FileUploadSkill.PostedFile.FileName)
                Using fs As Stream = FileUploadSkill.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileskill.CommandText = ls_sql
                        cmdfileskill.Connection = con
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_DOC_ID", certskill))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "SKILL"))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileskill.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            ElseIf chkskilold.Checked = True Then
                Dim cmdfileskill As New OracleCommand
                Dim ls_sql As String = String.Empty
                filename = Path.GetFileName(FileUploadSkill.PostedFile.FileName)
                Using fs As Stream = FileUploadSkill.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        'ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) "
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid"
                        '  ls_sql = ls_sql + "VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate)"
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileskill.CommandText = ls_sql
                        cmdfileskill.Connection = con
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_DOC_ID", certskill))
                        cmdfileskill.Parameters.Add(New OracleParameter(":olddocid", hdfskilold.Value.Trim))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "SKILL"))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileskill.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If
            getskill(spno)
            Dim status As String = "N"
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvSkill.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectSkill")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                        status = "Y"
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If
            If status.Equals("N") Then
                btnSaveSkill.Visible = True
                btnUpdateSkill.Visible = False
            Else
                btnSaveSkill.Visible = False
                btnUpdateSkill.Visible = True
            End If
            ShowMessage("Skill has been added successfully")

            empView()
            btnSaveSkill.Visible = False
            btnUpdateSkill.Visible = False
        Catch ex As Exception
        End Try

    End Sub

    '-----------------------------souvik begins 4
    Private Sub pop_comp_cd_stp()
        Session("Comp_cd_stop_same_trade") = ""

        Try
            If Session("CTMCode_stp") <> "" Then
                s_ctm_code = Session("CTMCode_stp")
            Else
                Session("CTMCode_stp") = "SSKF"
                s_ctm_code = Session("CTMCode_stp")
            End If
        Catch ex As Exception
            Session("CTMCode_stp") = "SSKF"
            s_ctm_code = Session("CTMCode_stp")
        End Try

        Try
            Dim dt_chk_stp As New DataTable
            Dim qry_chk_stp As String = "select ctm_type_code, substr(ctm_type_code,1,4),substr(ctm_type_code,-4) from HRACE.T_CEMP_TYPE_MASTER where CTM_TYPE='" + s_ctm_code + "' and substr(ctm_type_code,-4)='" + Session("Comp_code").ToString().Trim() + "' and CTM_STATUS='A'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            Dim cmd_chk_stp = New OracleCommand(qry_chk_stp, con)

            dt_chk_stp.Clear()
            dt_chk_stp = getRecord(cmd_chk_stp, con)
            If dt_chk_stp.Rows.Count > 0 Then
                Session("Comp_cd_stop_same_trade") = Session("Comp_code").ToString().Trim()
            End If
        Catch ex As Exception

        End Try
    End Sub
    '-----------------------------souvik ends 4
    Private Sub sendmailtoagencyskill(ByVal spno As String)
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            Dim code As String = "SELECT DISTINCT a.TCA_EMAIL_ID as email FROM hrace.t_cwm_alert a WHERE TCA_COMP_CODE ='3000' and TCA_DEPT_CODE='ALL' and TCA_ALERT_TYPE='SKIL' and TCA_ALERT_CODE in('HEAD','CC')"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(code, con)
            'cmd.Parameters.Add(New OracleParameter(":agm_comp_code", Session("Comp_Code")))
            'cmd.Parameters.Add(New OracleParameter(":AGM_AGENT_DEPTT", Txtdeprt.Text))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    If Not IsDBNull(dt.Rows(i).Item("email")) Then
                        Dim mail As New System.Net.Mail.MailMessage()
                        mail.To.Add(dt.Rows(i).Item("email"))
                        mail.From = New Net.Mail.MailAddress("clm.support@tatasteel.com")
                        mail.Subject = "Request for skill assessment-TGS"
                        mail.SubjectEncoding = System.Text.Encoding.UTF8
                        mail.Body = "Dear Sir/Madam <br> Please note that Safetypass No:<b>" + spno + "(Request No:-" + Session("requestnumber") + ") </b> generate request for skill assessment. Request you to take necessary action.<br/><br/>From<br/>Safety Department.<br><br><b>* This is a machine generated message. Please do not reply.</b>"
                        mail.BodyEncoding = System.Text.Encoding.UTF8
                        mail.IsBodyHtml = True
                        mail.Priority = MailPriority.High
                        Dim client As New SmtpClient()
                        client.Credentials = New System.Net.NetworkCredential(" ", "")
                        client.Port = 25
                        client.Host = "144.0.11.253"
                        'client.Port = 587
                        'client.Host = "smtp.gmail.com"
                        client.EnableSsl = False
                        client.Send(mail)
                        i = i + 1
                    End If
                End While
            End If
        Catch
        End Try

    End Sub
    Private Sub updateprevskillvalidity(ByVal safetypass As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            ls_sql = "select CCST_REQ_NO,CCST_COMP_CODE from T_CWM_CEMP_SKILL_TMP where CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_COMP_CODE=:CCST_COMP_CODE and CCST_REQ_NO=(select max(CCST_REQ_NO) from T_CWM_CEMP_SKILL_TMP where CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_COMP_CODE=:CCST_COMP_CODE)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", safetypass))
            cmd.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    ls_sql = "update T_CWM_CEMP_SKILL_TMP set CCST_VALIDITY_DATE=sysdate where CCST_REQ_NO=:CCST_REQ_NO and CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_COMP_CODE=:CCST_COMP_CODE "
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", safetypass))
                    cmd.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_Code")))
                    cmd.Parameters.Add(New OracleParameter(":CCST_REQ_NO", dt.Rows(i).Item("CCST_REQ_NO")))
                    cmd.ExecuteNonQuery()
                    i = i + 1
                End While
            End If
        Catch ex As Exception

        End Try
    End Sub

    'Protected Sub btnsavepv_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsavepv.Click
    '    Dim ls_sql As String = String.Empty
    '    Dim cmd As OracleCommand
    '    Dim dt As New DataTable
    '    Dim filename As String = String.Empty
    '    Dim pvid As String = String.Empty
    '    Dim certno As String = String.Empty
    '    'Dim pvid As String = String.Empty
    '    Dim pvcertid As String = String.Empty

    '    If txtstdtpv.Text.Trim = "__/__/____" Then
    '        ShowMessage("Please enter valid from date")
    '        Exit Sub
    '    End If
    '    If txtenddtpv.Text.Trim = "__/__/____" Then
    '        ShowMessage("Please enter valid to date")
    '        Exit Sub
    '    End If
    '    If fupdlpv.HasFile = False Then
    '        ShowMessage("Please provide police verification attachment")
    '    Else
    '        filename = Path.GetFileName(fupdlpv.PostedFile.FileName)
    '        Dim contentType As String = fupdlpv.PostedFile.ContentType
    '        If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
    '            If (fupdlpv.PostedFile.ContentLength > 512000) Then
    '                ShowMessage("Your file size is " + (fupdlpv.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
    '                Exit Sub
    '            End If
    '        Else
    '            ShowMessage("Please Upload pdf file only")
    '            Exit Sub
    '        End If
    '    End If

    '    Try
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()
    '        End If
    '        pvid = TrnCWEPVSeqNo("")
    '        pvcertid = TrnCWEPVCertNo("")
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()
    '        End If
    '        ls_sql = "insert into T_CWM_PV_DTL_TMP(CPDT_PV_ID,CPDT_SAFETY_PASS_NO,CPDT_COMP_CODE,CPDT_ST_DT,CPDT_END_DT,CPDT_CERT_NO,CPDT_CREATED_BY,CPDT_CREATED_DT,CPDT_REQ_NO) values(:CPDT_PV_ID,:CPDT_SAFETY_PASS_NO,:CPDT_COMP_CODE,TO_DATE(:CPDT_ST_DT,'DD/MM/YYYY'),TO_DATE(:CPDT_END_DT,'DD/MM/YYYY'),:CPDT_CERT_NO,:CPDT_CREATED_BY,sysdate,:CPDT_REQ_NO)"
    '        cmd = New OracleCommand(ls_sql, con)
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_PV_ID", pvid))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_SAFETY_PASS_NO", TxtSpno.Text.Trim))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_COMP_CODE", Session("Comp_Code")))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_ST_DT", txtstdtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_END_DT", txtenddtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_CERT_NO", pvcertid))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_CREATED_BY", Session("VendCode")))
    '        cmd.Parameters.Add(New OracleParameter(":CPDT_REQ_NO", Session("requestnumber")))
    '        cmd.ExecuteNonQuery()
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()
    '        End If
    '        ls_sql = "update t_cemp_details_tmp set CET_PV_ISSUED_ON=to_date(:CET_PV_ISSUED_ON,'dd/mm/yyyy'),CET_PV_VALID_TILL=to_date(:CET_PV_VALID_TILL,'dd/mm/yyyy'),CET_PV_RENEWAL_DT=to_date(:CET_PV_RENEWAL_DT,'dd/mm/yyyy') where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
    '        cmd = New OracleCommand(ls_sql, con)
    '        cmd.Parameters.Add(New OracleParameter(":CET_PV_ISSUED_ON", txtstdtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CET_PV_VALID_TILL", txtenddtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CET_PV_RENEWAL_DT", txtstdtpv.Text))
    '        cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
    '        cmd.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
    '        cmd.ExecuteNonQuery()
    '        If con.State = ConnectionState.Open Then
    '            con.Close()
    '        End If


    '        If fupdlpv.HasFile = True Then
    '            Dim cmdfiletrn As New OracleCommand
    '            Dim ls_sql1 As String = String.Empty
    '            filename = Path.GetFileName(fupdlpv.PostedFile.FileName)
    '            Using fs As Stream = fupdlpv.PostedFile.InputStream
    '                Using br As BinaryReader = New BinaryReader(fs)
    '                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

    '                    ls_sql1 = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
    '                    If con.State = ConnectionState.Closed Then
    '                        con.Open()

    '                    End If
    '                    cmdfiletrn.CommandText = ls_sql1
    '                    cmdfiletrn.Connection = con
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_DOC_ID", pvcertid))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_NAME", filename))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "PV"))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
    '                    'cmdfiletrn.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
    '                    'cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
    '                    cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
    '                    cmdfiletrn.ExecuteNonQuery()
    '                    If con.State = ConnectionState.Open Then
    '                        con.Close()
    '                    End If
    '                End Using
    '            End Using
    '        End If
    '        getPV(TxtSpno.Text.Trim)
    '        ShowMessage("Police Verification Details Added Successfully")
    '        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "PV")
    '        Session.Remove("polverenddt")
    '        clearpv()
    '    Catch ex As Exception

    '    End Try
    'End Sub
    'Private Sub getPV(ByVal spno As String)
    '    Dim ls_sql As String = String.Empty
    '    Dim cmd As OracleCommand
    '    Dim dt As New DataTable()
    '    Dim status As String = "N"

    '    Try
    '        ls_sql = "select CPDT_PV_ID,to_char(CPDT_ST_DT,'dd/mm/yyyy') stdt,to_char(CPDT_END_DT,'dd/mm/yyyy') enddt,CPDT_CERT_NO,DM_NAME,CPDT_REQ_NO from T_CWM_PV_DTL_TMP,T_DOCUMENT_MASTER where CPDT_CERT_NO=DM_DOC_ID and DM_FILE_TYPE='PV' and CPDT_SAFETY_PASS_NO=:spno order by CPDT_CREATED_DT"
    '        If con.State = ConnectionState.Closed Then
    '            con.Open()
    '        End If
    '        cmd = New OracleCommand(ls_sql, con)
    '        cmd.Parameters.Add(New OracleParameter(":spno", spno))
    '        dt = getRecord(cmd, con)
    '        If dt.Rows.Count > 0 Then
    '            gvpv.DataSource = dt
    '            gvpv.DataBind()
    '            For Each gvrow As GridViewRow In gvpv.Rows
    '                Dim chkbox As CheckBox = gvrow.FindControl("chkSelectPV")
    '                Dim reqno As HiddenField = gvrow.FindControl("hdreqno")

    '                If reqno.Value = Session("requestnumber") Then
    '                    chkbox.Enabled = True
    '                    status = "Y"
    '                Else
    '                    chkbox.Enabled = False
    '                    'status = "N"
    '                End If
    '            Next
    '            If status.Equals("N") Then
    '                btnsavepv.Visible = True
    '                btnupdatepv.Visible = False
    '            Else
    '                btnsavepv.Visible = False
    '                btnupdatepv.Visible = True
    '            End If
    '        Else
    '            btnsavepv.Visible = True
    '            btnupdatepv.Visible = False
    '        End If
    '        checkpvapprovedsprenew(spno)

    '    Catch ex As Exception
    '        gvpv.DataSource = Nothing
    '        gvpv.DataBind()
    '    End Try

    'End Sub
    Public Function TrnCWEPVCertNo(ByVal id As String) As String
        Dim vPVSeqCertNo As String = ""
        Dim sqlPVSeqCertNo As String = "Select (HRACE.seq_cemp_pv_docid.nextval) SEQNO from dual "
        Dim dtPVSeqCertNo As New DataTable()
        dtPVSeqCertNo = getRecord(sqlPVSeqCertNo, con)
        If dtPVSeqCertNo.Rows.Count > 0 Then
            vPVSeqCertNo = dtPVSeqCertNo.Rows(0)("SEQNO")
        End If

        dtPVSeqCertNo.Dispose()
        Return vPVSeqCertNo

    End Function
    Public Function TrnCWEPVSeqNo(ByVal id As String) As String
        Dim vPVSeqNo As String = ""
        Dim sqlPVSeqNo As String = "Select (HRACE.SEQ_CEMP_PV.nextval) SEQNO from dual "
        Dim dtPVSeqNo As New DataTable()
        dtPVSeqNo = getRecord(sqlPVSeqNo, con)
        If dtPVSeqNo.Rows.Count > 0 Then
            vPVSeqNo = dtPVSeqNo.Rows(0)("SEQNO")
        End If

        dtPVSeqNo.Dispose()
        Return vPVSeqNo

    End Function
    Public Function TrnCWESKILLSeqNo(ByVal id As String) As String
        Dim vSkillSeqNo As String = ""
        Dim sqlSkillSeqNo As String = "Select (HRACE.SEQ_CEMP_SKILL.nextval) SEQNO from dual "
        Dim dtSkillSeqNo As New DataTable()
        dtSkillSeqNo = getRecord(sqlSkillSeqNo, con)
        If dtSkillSeqNo.Rows.Count > 0 Then
            vSkillSeqNo = dtSkillSeqNo.Rows(0)("SEQNO")
        End If

        dtSkillSeqNo.Dispose()
        Return vSkillSeqNo

    End Function
    ''' <summary>
    ''' 23/02/2024 TCS.2164315 serial no added for the gridview gv skill for row index on fetching the fields value for reapply skill.
    ''' </summary>
    ''' <param name="spno"></param>
    Private Sub getskill(ByVal spno As String)


        Dim sqlSkill As String = "Select rownum-1 as rowno,t1.CCST_CERT_NO,t1.ccst_skill_type_cd,t2.ctm_type_desc SKILL_TYPE,t1.ccst_skill_cd, t3.ctm_type_desc SKILL_NAME, t1.ccst_remarks ,CCST_SAFETY_PASS_NO,ccst_loc_code,t4.DM_NAME,CCST_SKTD_CP_CD, NVL(t5.ctm_type_desc,'NA') as Skill_Trades,NVL(t6.ctm_type_desc,'NA') as Skill_assessment, NVL(CCST_SKTD_OTH_REMRK,'NA') CCST_SKTD_OTH_REMRK,t1.CCST_REQ_NO,nvl(CCST_SKTP_CP_CD,'NA') CCST_SKTP_CP_CD,to_char(t1.CCST_ASSESSMENT_DATE,'dd/mm/yyyy') CCST_ASSESSMENT_DATE,decode(trim(t1.CCST_ASSESSMENT_RESULT),'RET','Returned','PASS','Pass','FAIL','Not Ok','ABS','Absent',NULL,'NA','NA','NA') ""Assement_Result"",nvl(decode(CCST_ASSESSMENT_TYPE,'D','Direct Assessment','T','Traning Cum Assessment'),'NA') CCST_ASSESSMENT_TYPE1,CCST_ASSESSMENT_TYPE, "
        sqlSkill += "case when ((SELECT distinct MAX(SKC_SCRN_ATTEMPT)SKC_SCRN_ATTEMPT FROM HRACE.T_SKILL_CERTIFICATION where SKC_REQ_NO = t1.CCST_REQ_NO and SKC_SCREEN_RESULT = 'F')>2 )"
        sqlSkill += "or ((SELECT distinct MAX(SKC_ASSMNT_ATTEMPT)SKC_ASSMNT_ATTEMPT FROM HRACE.T_SKILL_CERTIFICATION where SKC_REQ_NO = t1.CCST_REQ_NO )>2 and (select count(TMP.CCST_REQ_NO) from hrps.T_TD_CLM_DOC@ace_iris,hrace.t_cwm_cemp_skill_tmp TMP where TCD_SP_NO=t1.CCST_SAFETY_PASS_NO and TCD_SP_NO=TMP.CCST_SAFETY_PASS_NO and TCD_CLM_SKILL_CD=TMP.CCST_SKTD_CP_CD and TCD_VALID_TAG='Y' and TMP.CCST_REQ_NO=t1.CCST_REQ_NO and UPPER(NVL(TCD_CERT_CATEG,'NA')) NOT IN ('PASS','SILVER','PLATINUM','GOLD') AND NVL(TCD_UPDATE_DATE,TCD_CREATE_DT)>TMP.CCST_ASSESSMENT_DATE and TMP.CCST_REQ_FLAG IS NOT NULL)>0)"
        sqlSkill += "then 'Skill Training Failed' else CCST_REMARKS_PD end CCST_REMARKS_PD,"
        sqlSkill += "TO_CHAR(ccst_validity_date,'DD/MM/YYYY') CCST_VALIDITY_DATE, NVL(CCST_WAIVE_OFF_RESN,'NA') CCST_WAIVE_OFF_RESN, CCST_WAIVE_OFF, CCST_ASSESSMENT_TYPE" ' ADD BY PRASUN ON 03012022 'WI6447
        sqlSkill += " from t_Cwm_Cemp_Skill_tmp t1, T_CEMP_TYPE_MASTER t2, T_CEMP_TYPE_MASTER t3,T_DOCUMENT_MASTER t4,T_CEMP_TYPE_MASTER t5,T_CEMP_TYPE_MASTER t6 where t1.CCST_CERT_NO=t4.DM_DOC_ID(+) and  t1.ccst_skill_type_cd = t2.ctm_type_code and t1.ccst_skill_cd = t3.ctm_type_code and t1.CCST_SKTD_CP_CD = t5.ctm_type_code (+) and t1.CCST_SKTP_CP_CD = t6.ctm_type_code (+) and t1.ccst_SAFETY_PASS_NO =:ccst_SAFETY_PASS_NO  "
        If (iscompanycodesforreapplyprovision(comp_cd)) Then
            sqlSkill += "order by t1.CCST_CREATED_DT,rowno desc"
        Else
            sqlSkill += "order by t1.CCST_CREATED_DT"
        End If

        Dim dtSkill As New DataTable()
        Dim cmd1 As New OracleCommand
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmd1.Connection = con
        cmd1.CommandText = sqlSkill
        cmd1.Parameters.Add(New OracleParameter(":ccst_SAFETY_PASS_NO", spno))
        ' cmd1.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
        cmd1.ExecuteNonQuery()
        Dim da1 = New OracleDataAdapter(cmd1)
        da1.Fill(dtSkill)
        'dtSkill = getRecord(sqlSkill, con)

        If dtSkill.Rows.Count > 0 Then

            gvSkill.DataSource = dtSkill
            gvSkill.DataBind()
            pnlSkillDetail.Visible = True
            btnUpdateSkill.Visible = True
            btnSaveSkill.Visible = False
            bindqueryskill()

            If dtSkill.Rows(0).Item("CCST_CERT_NO").ToString <> "" And dtSkill.Rows(0).Item("CCST_CERT_NO").ToString <> "0" Then
                hdfskilold.Value = dtSkill.Rows(0).Item("CCST_CERT_NO").ToString
                imgskillold.Visible = True
                chkskilold.Visible = True
            Else
                hdfskilold.Value = ""
                imgskillold.Visible = False
                chkskilold.Visible = False
            End If

            Dim status As String = "N"
            For Each gvrow As GridViewRow In gvSkill.Rows

                Dim reqno As HiddenField = gvrow.FindControl("hdreqno")

                If reqno.Value.Trim = Session("requestnumber").ToString Then
                    status = "Y"
                End If
            Next
            If status = "Y" Then

                hdfskilold.Value = ""
                imgskillold.Visible = False
                chkskilold.Visible = False
            End If

            status = "N"
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvSkill.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectSkill")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                        status = "Y"
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If
            If status.Equals("N") Then
                btnSaveSkill.Visible = True
                btnUpdateSkill.Visible = False

            Else
                btnSaveSkill.Visible = False
                btnUpdateSkill.Visible = True
            End If

            If dtSkill.Rows(0).Item("Assement_Result") = "Returned" Then
                lblErrorMsgSkill.Text = "Status:-Returned.Request to re apply."
                Exit Sub
            End If
            If dtSkill.Rows(0).Item("Assement_Result") = "Not Ok" Then
                lblErrorMsgSkill.Text = "Status:-Not Ok.Request to re apply after 15 days."

                Exit Sub
            End If
            If dtSkill.Rows(0).Item("Assement_Result") = "Absent" Then
                lblErrorMsgSkill.Text = "Status:-Absent.Request to re apply after 15 days."

                Exit Sub
            End If


        Else
            gvSkill.DataSource = Nothing
            gvSkill.DataBind()
            btnUpdateSkill.Enabled = False
            btnSaveSkill.Enabled = True
            btnSaveSkill.Visible = True
            pnlSkillDetail.Visible = True
            btnUpdateSkill.Enabled = False
        End If
        SkillAssessmentMsg() ''START ADD BY PRASUN ON 03012022 'WI6447
    End Sub
    'START ADD BY PRASUN ON 03012022
    'WI6447 add label alert message
    Private Sub SkillAssessmentMsg()
        Try
            lblSkillAssmntMsg.Text = ""
            If gvSkill.Rows.Count > 0 Then
                Dim AssResult As String = gvSkill.Rows(0).Cells(8).Text.ToUpper()
                Dim valDate As String = CType(gvSkill.Rows(0).FindControl("hdvalidity_date"), HiddenField).Value.ToUpper()
                Dim Waive_Off As String = CType(gvSkill.Rows(0).FindControl("hdWaive_Off"), HiddenField).Value.ToUpper()
                Dim Assmnt_Type As String = CType(gvSkill.Rows(0).FindControl("hdAssmnt_Type"), HiddenField).Value.ToUpper()

                If AssResult = "NA" AndAlso Waive_Off = "N" AndAlso valDate = "31/12/9999" AndAlso (Assmnt_Type = "D" OrElse Assmnt_Type = "T") Then
                    lblSkillAssmntMsg.Text = "Your skill certification result is pending at JNTVTI. Please contact JNTVTI to publish result"
                End If
            End If
        Catch ex As Exception

        End Try

    End Sub
    'END ADD BY PRASUN ON 03012022
    Private Sub bindqueryskill()
        Dim ls_sql As String = String.Empty
        Dim retremarks As String = String.Empty
        Try
            For Each gvrow As GridViewRow In gvSkill.Rows
                Dim remarks As Label = gvrow.FindControl("ccst_remarks")
                Dim result As String = gvrow.Cells(8).Text.ToString
                If result.Trim = "Returned" Then
                    retremarks = "Comments:-" + remarks.Text.Trim
                    remarks.Text = String.Empty

                End If
            Next
            lbl_retcommments.Text = retremarks
        Catch ex As Exception

        End Try
    End Sub
    Public Function CheckSkillMandatoryFields() As Integer
        Dim vErrorCount As Integer = 0
        If cmbSkSkillType.SelectedValue = "0" Then
            ErrorRow(tblSkillErrorLst, "Select Skill Type")
        End If

        If cmbSkSkill.SelectedValue = "0" Then
            ErrorRow(tblSkillErrorLst, "Select Skill")
        End If

        If ddlSkillTrade.Text.Trim() = "" Or ddlSkillTrade.Text.Trim() = "-" Then
            ErrorRow(tblSkillErrorLst, "Select Trade")
        End If

        Dim vSkilledTrades As String = ""
        vSkilledTrades = ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-"))

        If (vSkilledTrades = "SKTD0029") Then
            If txtOthSkillTrade.Text = "" Then
                'ErrorRow(tblSkillErrorLst, "Please fill the details Of trade In field ""other trade"" above")
                ErrorRow(tblSkillErrorLst, "Mentioned the trade description In the ""other trade"" box.")
            End If
        Else
        End If

        vErrorCount = err_cnt
        Return vErrorCount
    End Function
    Protected Sub btnProfile_spno_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim loc As String = ""
        Dim gvrow As GridViewRow
        gvrow = CType(sender, Button).Parent.Parent
        Dim sp_no As String = CType(gvrow.FindControl("lnk_spno"), LinkButton).Text
        Dim profileStatus As String = CType(gvrow.FindControl("lbl_stat"), LinkButton).Text

        Dim req As String = Session("requestnumber")

        Dim en_SpNo As String = ""
        Dim en_ReqNo As String = ""



        en_ReqNo = b64encode(req)
        en_SpNo = b64encode(sp_no)


        Response.Redirect("ospProfilePrint.aspx?SP_no=" + en_SpNo + "&ReqNo=" + en_ReqNo)



    End Sub
    Private Function sendmailtoagency(ByVal spno As String) As String
        Dim reqno As String = ""
        Dim ls_sql As String = ""
        Dim COemail As String = "N/A"
        Dim CAemail As String = "N/A"
        Dim vendname As String = String.Empty
        Dim deptname As String = String.Empty
        Dim name As String = String.Empty
        Dim safno As String = String.Empty
        Dim vendcode As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim wo As String = String.Empty
        Dim dtwo As New DataTable
        Dim dtcomail As New DataTable
        Dim dtcamail As New DataTable
        Try
            ls_sql = "Select distinct CET_SAFETY_PASSNO,CET_REQUEST_NO reqno,CET_DEPT_CODE,CDP_DEPT_NAME,CET_FIRSTNAME||' '||CET_MIDDLENAME||' '||CET_LASTNAME name,VDT_VENDOR_NAME,VDT_VENDOR_CODE from t_cemp_details_tmp,t_cnt_dept_master,t_vendor_details where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CDP_DEPT_CODE=CET_DEPT_CODE and VDT_VENDOR_CODE=CET_VENDOR_CODE and CET_REQUEST_NO=(select max(CET_REQUEST_NO) from t_cemp_details_tmp where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO) and CDP_COMP_CODE=CET_LOCATION_CODE"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", spno))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                reqno = dt.Rows(0).Item("reqno")
                deptname = dt.Rows(0).Item("CDP_DEPT_NAME")
                name = dt.Rows(0).Item("name")
                vendname = dt.Rows(0).Item("VDT_VENDOR_NAME")
                safno = dt.Rows(0).Item("CET_SAFETY_PASSNO")
                vendcode = dt.Rows(0).Item("VDT_VENDOR_CODE")
            End If
            dt.Clear()
            ls_sql = "select SRQ_WORK_ORDER from t_sp_request where SRQ_REQ_NO=:SRQ_REQ_NO "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":SRQ_REQ_NO", reqno))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                wo = dt.Rows(0).Item("SRQ_WORK_ORDER")
                ls_sql = "Select distinct ebeln PONo , banfn PRNo, sch.sch_cont_owner ContractOwner, sch.sch_cont_admin ContractAdmin , sci_hazard_lvl Hazard from CTDRDB.T_EKET_MM@CTDR_ACS_DBL , sapsur.t_shopping_cart_item@hrace_ebuy , sapsur.t_shopping_cart_hdr@hrace_ebuy sch where mandt = '600'  and ebeln = :ebeln and banfn <> ' ' and banfn is not null and (sci_fod_no = banfn or sci_fod_no = substr(banfn,2,10) ) and sci_Cart_no = sch_Cart_no"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":ebeln", wo))
                dtwo = getRecord(cmd, con)
                If dtwo.Rows.Count > 0 Then
                    ' Dim reviewdate As String = getdateMed(txtMedValStDt.Text)
                    Dim COpno As String = ""
                    Dim CApno As String = ""
                    If Not IsDBNull(dtwo.Rows(0).Item("ContractOwner")) Then
                        COpno = dtwo.Rows(0).Item("ContractOwner")
                    End If
                    If Not IsDBNull(dtwo.Rows(0).Item("ContractAdmin")) Then
                        CApno = dtwo.Rows(0).Item("ContractAdmin")
                    End If

                    If Not IsDBNull(dtwo.Rows(0).Item("ContractOwner")) Then
                        Dim ls_COMAIL As String = "select EMA_EMAIL_ID From v_empl_all where EMA_PERNO=:EMA_PERNO"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmd = New OracleCommand(ls_COMAIL, con)
                        cmd.Parameters.Add(New OracleParameter(":EMA_PERNO", COpno))
                        dtcomail = getRecord(cmd, con)
                        If dtcomail.Rows.Count > 0 Then
                            COemail = dtcomail.Rows(0).Item("EMA_EMAIL_ID")
                        End If
                    End If
                    If Not IsDBNull(dtwo.Rows(0).Item("ContractAdmin")) Then
                        Dim ls_CAMAIL As String = "select EMA_EMAIL_ID From v_empl_all where EMA_PERNO=:EMA_PERNO"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmd = New OracleCommand(ls_CAMAIL, con)
                        cmd.Parameters.Add(New OracleParameter(":EMA_PERNO", CApno))
                        dtcamail = getRecord(cmd, con)
                        If dtcamail.Rows.Count > 0 Then
                            CAemail = dtcamail.Rows(0).Item("EMA_EMAIL_ID")
                        End If
                    End If
                    'sendmailcaco(name, COemail, CAemail, vendcode, vendname, safno, deptname)

                End If
            End If

        Catch ex As Exception
            ShowMessage("Error occurs during process")

        End Try
        Return CAemail + "(CA)" + COemail + "(CO)"
    End Function
    Protected Sub ddlSkillTrade_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlSkillTrade.TextChanged
        Dim vSkilledTrades As String = ""
        vSkilledTrades = ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-"))
        Dim ls_sql As String = String.Empty
        If (vSkilledTrades = "SKTD0029") Then
            lblOthSkillTrade.Visible = True
            txtOthSkillTrade.Visible = False
            drp_skillassess.Visible = False
            lblSkillassess.Visible = False
            ddlSKAss.SelectedValue = "Yes"
            ddlSKAss.Enabled = True
            btnSaveSkill.Enabled = False
            btnUpdateSkill.Enabled = False
            ShowMessage("trade as other is now obsolate.Please do not choose trade as others")
            Exit Sub
        ElseIf (vSkilledTrades = "SKTD0028") Then
            drp_skillassess.Visible = True
            lblSkillassess.Visible = True
            lblOthSkillTrade.Visible = False
            txtOthSkillTrade.Visible = False
            FileUploadSkill.Enabled = False
            ddlSKAss.SelectedValue = "Yes"
            ddlSKAss.Enabled = False
            txtSkRemarks.Text = ""
            txtSkRemarks.Enabled = False
            btnSaveSkill.Enabled = True
            btnUpdateSkill.Enabled = True
            getSkillAssessment()
        Else
            lblOthSkillTrade.Visible = False
            txtOthSkillTrade.Visible = False
            drp_skillassess.Visible = False
            lblSkillassess.Visible = False
            FileUploadSkill.Enabled = True
            ddlSKAss.SelectedValue = "Yes"
            ddlSKAss.Enabled = True
            btnSaveSkill.Enabled = True
            btnUpdateSkill.Enabled = True
            If btnUpdateSkill.Visible = True Then
                btnSaveSkill.Enabled = False
            Else
                getskill(TxtSpno.Text.Trim)
            End If
        End If

    End Sub
    Protected Sub btnMed_spno_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim location As String = String.Empty
        Dim vendorcode As String = String.Empty
        Dim vendorname As String = String.Empty
        Dim vendoraddress As String = String.Empty
        Dim vendcontact As String = String.Empty
        Dim vendmobile As String = String.Empty
        Dim deptname As String = String.Empty
        Dim name As String = String.Empty
        Dim DOB As String = String.Empty
        Dim identification As String = String.Empty
        Dim guardian As String = String.Empty
        Dim localaddress As String = String.Empty
        Dim pincode As String = String.Empty
        Dim wphone As String = String.Empty
        Dim emergency As String = String.Empty
        Dim spvalid As String = String.Empty
        Dim qual As String = String.Empty
        Dim idcard As String = String.Empty
        Dim idnumber As String = String.Empty
        Dim gender As String = String.Empty
        Dim affirmative As String = String.Empty
        Dim skill As String = String.Empty
        Dim skilltype As String = String.Empty
        Dim cacomail As String = String.Empty
        Dim meddt As String = "No Medical Date Found"
        Dim reqdate As String = String.Empty
        Dim trndt As String = String.Empty
        Dim category As String = String.Empty
        Dim medtime As String = String.Empty
        Dim trntime As String = String.Empty

        Dim gvrow As GridViewRow
        gvrow = CType(sender, Button).Parent.Parent

        Dim sp_no As String = CType(gvrow.FindControl("lnk_spno"), LinkButton).Text
        Try
            ls_sql = "select CET_SAFETY_PASSNO from hrace.t_cemp_details_tmp where CET_REQUEST_NO='" + Session("requestnumber") + "' and CET_SAFETY_PASSNO='" + sp_no + "' and CET_DOCVER_STATUS='C'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt.Clear()
            dt = getRecord(ls_sql, con)
            If dt.Rows.Count = 0 Then
                ShowMessage("Document Verification not done. You Can Download Form After Document Verification")
                Exit Sub
            Else
                ls_sql = "select to_char(SRQ_CREATED_DT,'dd/mm/yyyy') reqdt from t_sp_request where SRQ_REQ_NO='" + Session("requestnumber") + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                dt.Clear()
                dt = getRecord(ls_sql, con)
                If dt.Rows.Count > 0 Then
                    reqdate = dt.Rows(0).Item("reqdt")
                Else
                    reqdate = "NA"
                End If
                ls_sql = "select CMP_COMPANY_NAME from t_company_master where CMP_COMPANY_CODE=:CMP_COMPANY_CODE"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":CMP_COMPANY_CODE", comp_cd))
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    location = dt.Rows(0).Item("CMP_COMPANY_NAME")
                End If
                ls_sql = "select VDT_VENDOR_NAME,VDT_VENDOR_CODE,VDT_COMPANY_CODE,lower(nvl(VDT_LOCAL_ADDRESS1,'NA')) address,nvl(VDT_PHONE1,'NA') phone,nvl(VDT_PHONE2,'NA') mobile from t_vendor_details where VDT_VENDOR_CODE=:VDT_VENDOR_CODE and VDT_COMPANY_CODE=:VDT_COMPANY_CODE"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":VDT_VENDOR_CODE", vVencode))
                cmd.Parameters.Add(New OracleParameter(":VDT_COMPANY_CODE", comp_cd))
                dt.Clear()
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    vendorcode = dt.Rows(0).Item("VDT_VENDOR_CODE")
                    vendorname = dt.Rows(0).Item("VDT_VENDOR_NAME")
                    vendoraddress = dt.Rows(0).Item("address")
                    vendcontact = dt.Rows(0).Item("phone")
                    vendmobile = dt.Rows(0).Item("mobile")
                End If
                ls_sql = "select CED_FIRSTNAME||' '||nvl(CED_MIDDLENAME,'')||nvl(CED_LASTNAME,'') name,to_char(CED_DOB,'dd/mm/yyyy') DOB,CED_GENDER,to_char(trunc(CED_SP_VALID_TILL),'dd/mm/yyyy') valid,CED_AFFIRMATIVE,nvl(CED_IDENTIFICATION_MARK,'NA') identy,nvl(CED_FATHER_NAME,nvl(CED_HUSBAND_NAME,'NA')) guardian,nvl(CED_EMERGENCY_NO,'NA') emergency,nvl(a.CTM_TYPE_DESC,'NA') type,nvl(CED_UNIQUE_ID_VALUE,'NA') typevalue,nvl(CED_PHONE_NO,'NA') CED_PHONE_NO,CDP_DEPT_NAME,nvl(b.CTM_TYPE_DESC,'NA') category from t_cemp_details,T_CNT_DEPT_MASTER,t_cemp_type_master a,t_cemp_type_master b where CED_SAFETY_PASS_NO=:CED_SAFETY_PASS_NO and CED_DEPT_CODE=CDP_DEPT_CODE and CDP_COMP_CODE=CED_COMPANY_CODE and a.CTM_TYPE_CODE=CED_UNIQUE_ID_TYPE and CED_CATEGORY=b.CTM_TYPE_CODE"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":CED_SAFETY_PASS_NO", sp_no))
                dt.Clear()
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    name = dt.Rows(0).Item("name")
                    DOB = dt.Rows(0).Item("DOB")
                    gender = dt.Rows(0).Item("CED_GENDER")
                    affirmative = dt.Rows(0).Item("CED_AFFIRMATIVE")
                    guardian = dt.Rows(0).Item("guardian")
                    emergency = dt.Rows(0).Item("emergency")
                    identification = dt.Rows(0).Item("identy")
                    wphone = dt.Rows(0).Item("CED_PHONE_NO")
                    deptname = dt.Rows(0).Item("CDP_DEPT_NAME")
                    idcard = dt.Rows(0).Item("type")
                    idnumber = dt.Rows(0).Item("typevalue")
                    category = dt.Rows(0).Item("category")
                    If IsDBNull(dt.Rows(0).Item("valid")) Then
                        spvalid = "NA"
                    Else
                        spvalid = dt.Rows(0).Item("valid")
                    End If
                End If

                Dim dt1 As New DataTable
                ls_sql = "select nvl(CCA_NAME,'')||' '||nvl(CCA_HOUSE_NO,'')||' '||nvl(CCA_STREET,'') address,CCA_PIN from T_CWM_CEMP_ADDRS where CCA_SAFETY_PASS_NO='" + sp_no + "' and CCA_ADDRESS_ID =(select max(CCA_ADDRESS_ID) from  T_CWM_CEMP_ADDRS where CCA_SAFETY_PASS_NO='" + sp_no + "')"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If


                dt1 = getRecord(ls_sql, con)
                If dt1.Rows.Count > 0 Then
                    localaddress = dt1.Rows(0).Item("address")
                    pincode = dt1.Rows(0).Item("CCA_PIN")
                End If
            End If

        Catch ex As Exception

        End Try
        Dim dt2 As New DataTable
        ls_sql = "select  B.CTM_TYPE_DESC skilltype,C.CTM_TYPE_DESC skill from hrace.t_cwm_cemp_skill a,t_cemp_type_master b,hrace.t_cemp_type_master c where A.CCS_SKILL_CD=B.CTM_TYPE_CODE and A.CCS_SKILL_TYPE_CD=C.CTM_TYPE_CODE and (A.CCS_DELETE_FLG<>'Y' or A.CCS_DELETE_FLG is null)  and A.CCS_SAFETY_PASS_NO='" + sp_no + "'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        dt2 = getRecord(ls_sql, con)
        If dt2.Rows.Count > 0 Then
            skilltype = dt2.Rows(0).Item("skilltype")
            skill = dt2.Rows(0).Item("skill")
        End If
        cacomail = sendmailtoagency(sp_no)
        '======================New medical date implemented by TCS L2 Team 13/08/2025===========================================
        Dim dt3 As New DataTable

        ls_sql = "SELECT TO_CHAR(SBD_BOOKING_DATE, 'dd/mm/yyyy') meddt,DECODE(SBD_SLOT_TYPE,'SL2','2 PM – 6 PM','9 AM – 1 PM') medtime,TO_CHAR(SBD_BOOKING_DATE, 'dd/mm/yyyy') trndt,"
        ls_sql += "DECODE(SBD_SLOT_TYPE,'SL2','2 PM – 6 PM','9 AM – 1 PM') trntime FROM hrace.T_TRNG_SLOT_BK_DTLS WHERE SBD_REQ_NO = '" + Session("requestnumber") + "' AND SBD_SAFETY_PASS_NO = '" + sp_no + "' AND SBD_BOOKING_TYPE='MED' AND  SBD_COMPANY_CD='" + comp_cd + "'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        dt3 = getRecord(ls_sql, con)
        If (dt3.Rows.Count > 0) Then
            meddt = dt3.Rows(0).Item("meddt")
            trndt = dt3.Rows(0).Item("trndt")
            medtime = dt3.Rows(0).Item("medtime")
            trntime = dt3.Rows(0).Item("trntime")
        Else
            ls_sql = "select to_char(CMT_MED_DT,'dd/mm/yyyy') meddt,to_char(CMT_MED_DT,'hh:mi:ss AM') medtime,nvl(to_char(CMT_TRN_DT,'dd/mm/yyyy'),'') trndt,to_char(CMT_TRN_DT,'hh:mi:ss') trntime  from T_CWM_MED_TRN_DTL where CMT_REQ_NO='" + Session("requestnumber") + "' and CMT_SAFETY_PASS_NO='" + sp_no + "'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt3 = getRecord(ls_sql, con)
            If dt3.Rows.Count > 0 Then
                meddt = dt3.Rows(0).Item("meddt")
                If IsDBNull(dt3.Rows(0).Item("trndt")) Then
                Else
                    trndt = dt3.Rows(0).Item("trndt")
                End If

                medtime = dt3.Rows(0).Item("medtime")
                If IsDBNull(dt3.Rows(0).Item("trntime")) Then
                Else
                    trntime = dt3.Rows(0).Item("trntime")
                End If
            End If
        End If
        '========================================***===========================================
        Response.Clear()
        HttpContext.Current.Response.Clear()
        Response.Buffer = True
        Response.ContentType = "application/pdf"
        Response.AddHeader("content-disposition", "attachment;filename=Medical Examination Form.pdf")
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        Dim pdfDoc As New Document(PageSize.A4, 10.0F, 10.0F, 1.0F, 0.0F)
        Dim tabheader As New PdfPTable(1)
        tabheader.WidthPercentage = 70
        Dim cellheader As New PdfPCell()

        cellheader.AddElement(New Phrase("CONTRACTOR EMPLOYEE'S MEDICAL EXAMINATION FORM (Srl No:           )", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
        cellheader.HorizontalAlignment = Element.ALIGN_CENTER
        cellheader.Border = iTextSharp.text.Rectangle.NO_BORDER

        tabheader.AddCell(cellheader)
        ' Dim line As LineSeparator = New LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)
        'Dim p As Paragraph = New Paragraph(New Chunk())


        Dim tabsubheader As New PdfPTable(2)
        tabsubheader.WidthPercentage = 90
        Dim widths As Single() = New Single() {70.0F, 70.0F}
        tabsubheader.SetWidths(widths)
        Dim cellsubheader As New PdfPCell()
        cellsubheader.AddElement(New Phrase("Name Of The Centre:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader.HorizontalAlignment = Element.ALIGN_LEFT
        cellsubheader.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader)
        Dim cellsubheader1 As New PdfPCell()
        cellsubheader1.AddElement(New Phrase("Medical Exam Date:" + meddt + "  Time:" + medtime, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader1.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsubheader1.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader1)
        Dim cellsubheader2 As New PdfPCell()
        cellsubheader2.AddElement(New Phrase("Location:" + location + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader2.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsubheader2.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader2)
        Dim cellsubheader3 As New PdfPCell()
        cellsubheader3.AddElement(New Phrase("Training Date:" + trndt + "  Time:" + trntime, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader3.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsubheader3.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader3)
        'Dim p1 As Paragraph = New Paragraph(New Chunk())
        Dim line2 As LineSeparator = New LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)
        Dim tabsubheader2 As New PdfPTable(2)
        tabsubheader2.WidthPercentage = 90
        tabsubheader2.SetWidths(widths)
        Dim cell1subheader As New PdfPCell()
        cell1subheader.AddElement(New Phrase("Request Number With Date: " + Session("requestnumber") + " (Dated:" + reqdate + ")", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cell1subheader.HorizontalAlignment = Element.ALIGN_LEFT
        cell1subheader.Border = iTextSharp.text.Rectangle.NO_BORDER

        tabsubheader2.AddCell(cell1subheader)
        Dim cellsub1header1 As New PdfPCell()
        cellsub1header1.AddElement(New Phrase("Safety Pass No:" + sp_no + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub1header1.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub1header1.Border = iTextSharp.text.Rectangle.NO_BORDER

        tabsubheader2.AddCell(cellsub1header1)


        'Dim cellsub1header2 As New PdfPCell()
        'cellsub1header2.Colspan = 2
        'cellsub1header2.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        'cellsub1header2.HorizontalAlignment = Element.ALIGN_RIGHT
        'cellsub1header2.Border = iTextSharp.text.Rectangle.NO_BORDER
        'tabsubheader2.AddCell(cellsub1header2)
        Dim tabsubheader3 As New PdfPTable(2)
        tabsubheader3.WidthPercentage = 90
        Dim widths1 As Single() = New Single() {50.0F, 70.0F}
        tabsubheader3.SetWidths(widths1)
        Dim cellsub2header As New PdfPCell()
        cellsub2header.AddElement(New Phrase("VENDOR DETAILS", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        cellsub2header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub2header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader3.AddCell(cellsub2header)
        Dim cellsub3header As New PdfPCell()
        cellsub3header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub3header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub3header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub3header)
        Dim cellsub4header As New PdfPCell()
        cellsub4header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub4header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub4header.AddElement(New Phrase("Vendor Code:" + vendorcode + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub4header)
        Dim cellsub5header As New PdfPCell()
        cellsub5header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub5header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub5header.AddElement(New Phrase("Dept Name:" + deptname + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub5header)
        Dim cellsub6header As New PdfPCell()
        cellsub6header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub6header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub6header.AddElement(New Phrase("Vendor Name:" + vendorname + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub6header)
        Dim cellsub7header As New PdfPCell()
        cellsub7header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub7header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub7header.AddElement(New Phrase("Local address:" + vendoraddress + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub7header)
        Dim cellsub8header As New PdfPCell()
        cellsub8header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub8header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub8header.AddElement(New Phrase("Contact No:" + vendcontact + "       Mobile No:" + vendmobile, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub8header)
        Dim cellsub9header As New PdfPCell()
        cellsub9header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub9header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub9header.AddElement(New Phrase("Plant Code:" + comp_cd + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub9header)
        Dim cellsub10header As New PdfPCell()
        cellsub10header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub10header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub10header.AddElement(New Phrase("Email ID CA/CO:" + cacomail.ToLower(), FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub10header)
        Dim cellsub11header As New PdfPCell()
        cellsub11header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub11header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub11header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub11header)

        Dim cellsub12header As New PdfPCell()
        cellsub12header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub12header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub12header.AddElement(New Phrase("PARTICIPANT'S DETAILS", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsubheader3.AddCell(cellsub12header)
        Dim cellsub13header As New PdfPCell()
        cellsub13header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub13header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub13header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsubheader3.AddCell(cellsub13header)
        Dim tabsubheader4 As New PdfPTable(3)
        tabsubheader4.WidthPercentage = 90
        Dim widths2 As Single() = New Single() {40.0F, 20.0F, 30.0F}
        tabsubheader4.SetWidths(widths2)
        Dim cellsub14header As New PdfPCell()
        cellsub14header.Colspan = 3
        cellsub14header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub14header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub14header.AddElement(New Phrase("Name:" + name + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub14header)

        Dim cellsub17header As New PdfPCell()
        cellsub17header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub17header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub17header.AddElement(New Phrase("Date Of Birth:" + DOB + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub17header)
        Dim cellsub18header As New PdfPCell()
        cellsub18header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub18header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub18header.AddElement(New Phrase("Gender:" + gender + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub18header)
        Dim cellsub19header As New PdfPCell()
        cellsub19header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub19header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub19header.AddElement(New Phrase("Affirmative:" + affirmative + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub19header)
        Dim cellsub20header As New PdfPCell()
        cellsub20header.Colspan = 3
        cellsub20header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub20header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub20header.AddElement(New Phrase("Identification Marks:" + identification, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub20header)
        Dim cellsub23header As New PdfPCell()
        cellsub23header.Colspan = 3
        cellsub23header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub23header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub23header.AddElement(New Phrase("Father's/Husband's Name:" + guardian + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub23header)
        Dim cellsub26header As New PdfPCell()
        cellsub26header.Colspan = 3
        cellsub26header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub26header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub26header.AddElement(New Phrase("Local Address:" + localaddress + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub26header)
        Dim cellsub29header As New PdfPCell()
        cellsub29header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub29header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub29header.AddElement(New Phrase("Pin Code:" + pincode + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub29header)
        Dim cellsub30header As New PdfPCell()
        cellsub30header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub30header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub30header.AddElement(New Phrase("Phone No:" + wphone + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub30header)
        Dim cellsub31header As New PdfPCell()
        cellsub31header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub31header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub31header.AddElement(New Phrase("Emergency Contact No:" + emergency + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub31header)
        Dim cellsub32header As New PdfPCell()
        cellsub32header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub32header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub32header.AddElement(New Phrase("SP Valid Till Date:" + spvalid + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub32header)
        Dim cellsub33header As New PdfPCell()
        cellsub33header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub33header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub33header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub33header)
        Dim cellsub34header As New PdfPCell()
        cellsub34header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub34header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub34header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub34header)
        Dim cellsub35header As New PdfPCell()
        cellsub35header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub35header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub35header.AddElement(New Phrase("ID Card type:" + idcard + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub35header)
        Dim cellsub36header As New PdfPCell()
        cellsub36header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub36header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub36header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub36header)
        Dim cellsub37header As New PdfPCell()
        cellsub37header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub37header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub37header.AddElement(New Phrase("ID Card No:" + idnumber + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub37header)
        Dim cellsub38header As New PdfPCell()
        cellsub38header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub38header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub38header.AddElement(New Phrase("Category:" + category + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub38header)
        Dim cellsub39header As New PdfPCell()
        cellsub39header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub39header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub39header.AddElement(New Phrase("Skill Type:" + skill + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub39header)
        Dim cellsub40header As New PdfPCell()
        cellsub40header.Colspan = 5
        cellsub40header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub40header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub40header.AddElement(New Phrase("Skill: " + skilltype + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub40header)
        Dim tabsub4header As New PdfPTable(1)
        tabsub4header.WidthPercentage = 80
        'Dim cellsub41header As New PdfPCell()
        'cellsub41header.HorizontalAlignment = Element.ALIGN_LEFT
        'cellsub41header.Border = iTextSharp.text.Rectangle.NO_BORDER
        'cellsub41header.AddElement(New Phrase("NOMINEE DETAILS:", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        'tabsub4header.AddCell(cellsub41header)
        Dim tabsub5header As New PdfPTable(1)
        tabsub5header.WidthPercentage = 90
        Dim cellsub42header As New PdfPCell()
        cellsub42header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub42header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub42header.AddElement(New Phrase("DECLARATION:", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsub5header.AddCell(cellsub42header)
        Dim cellsub142header As New PdfPCell()
        cellsub142header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub142header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub142header.AddElement(New Phrase("I hereby declare that the above mentioned information is correct to the best of my knowledge and I bear the responsibility for the correctness of the above mentioned particular.", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsub5header.AddCell(cellsub142header)
        Dim cellsub143header As New PdfPCell()
        cellsub143header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub143header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub143header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsub5header.AddCell(cellsub143header)
        Dim tabsub6header As New PdfPTable(2)
        tabsub6header.WidthPercentage = 90
        Dim cellsub43header As New PdfPCell()
        cellsub43header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub43header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub43header.AddElement(New Phrase("Signature/Thumb Impression Of Candidate", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        tabsub6header.AddCell(cellsub43header)
        Dim cellsub44header As New PdfPCell()
        cellsub44header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub44header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub44header.AddElement(New Phrase("                       Vendor's Signature and Stamp", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        tabsub6header.AddCell(cellsub44header)
        Dim tabsub7header As New PdfPTable(1)
        tabsub7header.WidthPercentage = 90
        Dim cellsub45header As New PdfPCell()
        cellsub45header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub45header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub45header.AddElement(New Phrase("MEDICAL EXAMINATION REPORT : TO BE FILLED BY DOCTOR                Vision Test Required(Yes / No)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsub7header.AddCell(cellsub45header)
        Dim tabsub8header As New PdfPTable(2)
        tabsub8header.WidthPercentage = 90
        Dim widths4 As Single() = New Single() {80.0F, 70.0F}
        tabsub8header.SetWidths(widths4)
        Dim cellsub46header As New PdfPCell()
        cellsub46header.AddElement(New Phrase("Height (in cms):………………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub46header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub46header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub46header)
        Dim cellsub47header As New PdfPCell()
        cellsub47header.AddElement(New Phrase("Respiratory System:……………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub47header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub47header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub47header)
        Dim cellsub48header As New PdfPCell()
        cellsub48header.AddElement(New Phrase("Weight(in Kg):…………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub48header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub48header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub48header)
        Dim cellsub49header As New PdfPCell()
        cellsub49header.AddElement(New Phrase("Blood Pressure(in mm of Hg):……………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub49header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub49header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub49header)
        Dim cellsub50header As New PdfPCell()
        cellsub50header.AddElement(New Phrase("Ability to Walk closed eyes:OK/ Not OK", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub50header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub50header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub50header)
        Dim cellsub51header As New PdfPCell()
        cellsub51header.AddElement(New Phrase("Pulse/min:……………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub51header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub51header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub51header)
        Dim cellsub52header As New PdfPCell()
        cellsub52header.AddElement(New Phrase("Romberg's Sign:Positive/Negative", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub52header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub52header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub52header)
        Dim cellsub53header As New PdfPCell()
        cellsub53header.AddElement(New Phrase("CVS:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub53header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub53header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub53header)
        Dim cellsub54header As New PdfPCell()
        cellsub54header.AddElement(New Phrase("Physical", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub54header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub54header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub54header)
        Dim cellsub55header As New PdfPCell()
        cellsub55header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub55header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub55header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub55header)
        Dim cellsub56header As New PdfPCell()
        cellsub56header.AddElement(New Phrase("Hearing:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub56header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub56header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub56header)
        Dim cellsub57header As New PdfPCell()
        cellsub57header.AddElement(New Phrase("Other Heart Related Abnormalities:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub57header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub57header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub57header)
        Dim cellsub58header As New PdfPCell()
        cellsub58header.AddElement(New Phrase("Left Ear:Ok/ Not Ok        Right Ear:Ok/ Not Ok", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub58header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub58header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub58header)
        Dim cellsub59header As New PdfPCell()
        cellsub59header.AddElement(New Phrase("(Like Congenital , Arrhythmia, Coronary disease etc.)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub59header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub59header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub59header)
        Dim cellsub60header As New PdfPCell()
        cellsub60header.AddElement(New Phrase("CNS", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub60header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub60header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub60header)
        Dim cellsub61header As New PdfPCell()
        cellsub61header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub61header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub61header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub61header)
        Dim cellsub62header As New PdfPCell()
        cellsub62header.AddElement(New Phrase("Other Abnormalities", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub62header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub62header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub62header)
        Dim cellsub63header As New PdfPCell()
        cellsub63header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub63header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub63header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub63header)
        Dim cellsub64header As New PdfPCell()
        cellsub64header.AddElement(New Phrase("(Like Facial Palsy, Missing / Extra / Arthritic Finger etc)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub64header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub64header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub64header)
        Dim cellsub65header As New PdfPCell()
        cellsub65header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub65header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub65header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub65header)
        Dim cellsub66header As New PdfPCell()
        cellsub66header.AddElement(New Phrase("Eye/Vision", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub66header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub66header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub66header)
        Dim cellsub67header As New PdfPCell()
        cellsub67header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub67header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub67header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub67header)
        Dim cellsub68header As New PdfPCell()
        cellsub68header.AddElement(New Phrase("Colour Perception:   Ok/Defective", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub68header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub68header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub68header)
        Dim cellsub69header As New PdfPCell()
        cellsub69header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub69header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub69header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub69header)
        Dim cellsub70header As New PdfPCell()
        cellsub70header.AddElement(New Phrase("Right Eye(With / Without Glasses)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub70header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub70header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub70header)
        Dim cellsub71header As New PdfPCell()
        cellsub71header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub71header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub71header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub71header)
        Dim cellsub72header As New PdfPCell()
        cellsub72header.AddElement(New Phrase("Left Eye(With / Without Glasses)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub72header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub72header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub72header)
        Dim cellsub73header As New PdfPCell()
        cellsub73header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub73header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub73header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub73header)
        Dim cellsub74header As New PdfPCell()
        cellsub74header.AddElement(New Phrase("Squint:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub74header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub74header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub74header)
        Dim cellsub75header As New PdfPCell()
        cellsub75header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub75header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub75header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub75header)
        Dim cellsub76header As New PdfPCell()
        cellsub76header.AddElement(New Phrase("Eye Comments:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub76header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub76header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub76header)
        Dim cellsub77header As New PdfPCell()
        cellsub77header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub77header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub77header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub77header)
        Dim cellsub78header As New PdfPCell()
        cellsub78header.AddElement(New Phrase("Pathology Report", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub78header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub78header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub78header)
        Dim cellsub79header As New PdfPCell()
        cellsub79header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub79header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub79header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub79header)
        Dim cellsub80header As New PdfPCell()
        cellsub80header.AddElement(New Phrase("Haemoglobin(in gm%):…………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub80header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub80header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub80header)
        Dim cellsub81header As New PdfPCell()
        cellsub81header.AddElement(New Phrase("Blood Group:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub81header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub81header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub81header)
        Dim cellsub82header As New PdfPCell()
        cellsub82header.AddElement(New Phrase("Random Blood Sugar(in mg%):…………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub82header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub82header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub82header)
        Dim cellsub83header As New PdfPCell()
        cellsub83header.AddElement(New Phrase("MR no.:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub83header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub83header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub83header)
        Dim cellsub86header As New PdfPCell()
        cellsub86header.AddElement(New Phrase("History(if any):     Hydrocele/Hernia                    Amputation/ Polydoctyl", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub86header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub86header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub86header)
        Dim cellsub87header As New PdfPCell()
        cellsub87header.AddElement(New Phrase("          Communicable Diseases                          Epilepsy", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub87header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub87header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub87header)
        Dim cellsub88header As New PdfPCell()
        cellsub88header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub88header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub88header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub88header)
        Dim cellsub89header As New PdfPCell()
        cellsub89header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub89header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub89header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub89header)
        Dim cellsub84header As New PdfPCell()
        cellsub84header.AddElement(New Phrase("REHABILITATION COMMITTEE REPORTS (If applicable):", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub84header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub84header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub84header)
        Dim cellsub85header As New PdfPCell()
        cellsub85header.Colspan = 2
        cellsub85header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub85header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub85header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub85header)
        Dim cellsub90header As New PdfPCell()
        cellsub90header.AddElement(New Phrase("Comments:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub90header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub90header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub90header)
        Dim cellsub91header As New PdfPCell()
        cellsub91header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub91header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub91header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub91header)
        Dim cellsub95header As New PdfPCell()
        cellsub95header.Colspan = 2
        cellsub95header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub95header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub95header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub95header)

        Dim cellsub93header As New PdfPCell()
        cellsub93header.Colspan = 2
        cellsub93header.AddElement(New Phrase("Chairman Signature                     Member Signature                     Member Signature                     Member Signature ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub93header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub93header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub93header)
        Dim cellsub96header As New PdfPCell()
        cellsub96header.Colspan = 2
        cellsub96header.AddElement(New Phrase("UNDERTAKING DETAILS(If applicable):", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub96header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub96header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub96header)
        Dim cellsub97header As New PdfPCell()
        cellsub97header.Colspan = 2
        cellsub97header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub97header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub97header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub97header)
        Dim cellsub98header As New PdfPCell()
        cellsub98header.Colspan = 2
        cellsub98header.AddElement(New Phrase("This is to certify that the above person on the basis of medical examination", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub98header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub98header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub98header)
        Dim cellsub99header As New PdfPCell()
        cellsub99header.Colspan = 2
        cellsub99header.AddElement(New Phrase("is found FIT / UNFIT to work with TSL & its associate company                            Medical Examiner's Signature              Stamp              Date", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub99header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub99header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub99header)
        Dim cellsub100header As New PdfPCell()
        cellsub100header.Colspan = 2
        cellsub100header.AddElement(New Phrase("DECLARATION OF CANDIDATE:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub100header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub100header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub100header)
        Dim cellsub101header As New PdfPCell()
        cellsub101header.Colspan = 2
        cellsub101header.AddElement(New Phrase("I have been medically examined by the doctor with my consent.                           Signature  & Thumb Impresssion of Candidate", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub101header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub101header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub101header)
        Dim sr As New StringReader(sw.ToString())


        Dim htmlparser As New HTMLWorker(pdfDoc)
        Dim pdfWrite As PdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream)

        'pdfDoc.Add(table1)
        pdfDoc.Open()
        'Dim bfTimes As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont. , False)

        'Dim times As New iTextSharp.text.Font(bfTimes)
        pdfDoc.Add(tabheader)
        pdfDoc.Add(tabsubheader)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(tabsubheader2)
        pdfDoc.Add(New Phrase(" "))
        pdfDoc.Add(line2)
        Dim cb As PdfContentByte = pdfWrite.DirectContent
        Dim rect = New iTextSharp.text.Rectangle(465, 650, 550, 730)
        rect.Border = iTextSharp.text.Rectangle.BOX
        rect.BorderWidth = 1
        rect.BorderColor = New BaseColor(0, 0, 0)
        'cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 70.0, 70.0, 0)
        cb.Rectangle(rect)
        Dim ct As ColumnText = New ColumnText(cb)
        Dim cpharse1 As Phrase = New Phrase("Affix recent passport size photograph attested by contractor with vendor stamp", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD))
        ct.SetSimpleColumn(cpharse1, 465, 645, 550, 710, 10, Element.ALIGN_LEFT)
        'ct.AddElement(New Phrase("Affix recent passport size photograph attested by contractor with vendor stamp", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD)))
        ct.Go()
        Dim cb1 As PdfContentByte = pdfWrite.DirectContent
        Dim rect1 = New iTextSharp.text.Rectangle(320, 250, 540, 350)
        rect1.Border = iTextSharp.text.Rectangle.BOX
        rect1.BorderWidth = 1
        rect1.BorderColor = New BaseColor(0, 0, 0)
        'cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 70.0, 70.0, 0)
        cb1.Rectangle(rect1)
        Dim ct1 As ColumnText = New ColumnText(cb1)
        Dim cphrase As Phrase = New Phrase("Doctor Comments (if any):", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL))
        ct1.SetSimpleColumn(cphrase, 320, 245, 540, 345, 20, Element.ALIGN_TOP)
        'ct1.AddElement(New Phrase("Doctor Comments (if any):", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL)))
        ct1.Go()
        pdfDoc.Add(tabsubheader3)
        pdfDoc.Add(tabsubheader4)
        pdfDoc.Add(tabsub4header)
        pdfDoc.Add(tabsub5header)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(tabsub6header)
        pdfDoc.Add(New Phrase(" "))
        pdfDoc.Add(line2)
        pdfDoc.Add(tabsub7header)
        pdfDoc.Add(tabsub8header)
        htmlparser.Parse(sr)
        pdfDoc.Close()
        Response.Write(pdfDoc)
        HttpContext.Current.Response.End()
    End Sub
    Protected Sub btnMedRenew_spno_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim location As String = String.Empty
        Dim vendorcode As String = String.Empty
        Dim vendorname As String = String.Empty
        Dim vendoraddress As String = String.Empty
        Dim vendcontact As String = String.Empty
        Dim vendmobile As String = String.Empty
        Dim deptname As String = String.Empty
        Dim name As String = String.Empty
        Dim DOB As String = String.Empty
        Dim identification As String = String.Empty
        Dim guardian As String = String.Empty
        Dim localaddress As String = String.Empty
        Dim pincode As String = String.Empty
        Dim wphone As String = String.Empty
        Dim emergency As String = String.Empty
        Dim spvalid As String = String.Empty
        Dim qual As String = String.Empty
        Dim idcard As String = String.Empty
        Dim idnumber As String = String.Empty
        Dim gender As String = String.Empty
        Dim affirmative As String = String.Empty
        Dim skill As String = String.Empty
        Dim skilltype As String = String.Empty
        Dim cacomail As String = String.Empty
        Dim meddt As String = "No Medical Date Found"
        Dim reqdate As String = String.Empty
        Dim trndt As String = String.Empty
        Dim category As String = String.Empty
        Dim medtime As String = String.Empty
        Dim trntime As String = String.Empty

        Dim gvrow As GridViewRow
        gvrow = CType(sender, Button).Parent.Parent

        Dim sp_no As String = CType(gvrow.FindControl("lnk_Renew_spno"), LinkButton).Text
        Try
            ls_sql = "select CET_SAFETY_PASSNO from hrace.t_cemp_details_tmp where CET_REQUEST_NO='" + Session("requestnumber") + "' and CET_SAFETY_PASSNO='" + sp_no + "' and CET_DOCVER_STATUS='C'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt.Clear()
            dt = getRecord(ls_sql, con)
            If dt.Rows.Count = 0 Then
                ShowMessage("Document Verification not done. You Can Download Form After Document Verification")
                Exit Sub
            Else
                ls_sql = "select to_char(SRQ_CREATED_DT,'dd/mm/yyyy') reqdt from t_sp_request where SRQ_REQ_NO='" + Session("requestnumber") + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                dt.Clear()
                dt = getRecord(ls_sql, con)
                If dt.Rows.Count > 0 Then
                    reqdate = dt.Rows(0).Item("reqdt")
                Else
                    reqdate = "NA"
                End If
                ls_sql = "select CMP_COMPANY_NAME from t_company_master where CMP_COMPANY_CODE=:CMP_COMPANY_CODE"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":CMP_COMPANY_CODE", comp_cd))
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    location = dt.Rows(0).Item("CMP_COMPANY_NAME")
                End If
                ls_sql = "select VDT_VENDOR_NAME,VDT_VENDOR_CODE,VDT_COMPANY_CODE,lower(nvl(VDT_LOCAL_ADDRESS1,'NA')) address,nvl(VDT_PHONE1,'NA') phone,nvl(VDT_PHONE2,'NA') mobile from t_vendor_details where VDT_VENDOR_CODE=:VDT_VENDOR_CODE and VDT_COMPANY_CODE=:VDT_COMPANY_CODE"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":VDT_VENDOR_CODE", vVencode))
                cmd.Parameters.Add(New OracleParameter(":VDT_COMPANY_CODE", comp_cd))
                dt.Clear()
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    vendorcode = dt.Rows(0).Item("VDT_VENDOR_CODE")
                    vendorname = dt.Rows(0).Item("VDT_VENDOR_NAME")
                    vendoraddress = dt.Rows(0).Item("address")
                    vendcontact = dt.Rows(0).Item("phone")
                    vendmobile = dt.Rows(0).Item("mobile")
                End If
                ls_sql = "select CED_FIRSTNAME||' '||nvl(CED_MIDDLENAME,'')||nvl(CED_LASTNAME,'') name,to_char(CED_DOB,'dd/mm/yyyy') DOB,CED_GENDER,to_char(trunc(CED_SP_VALID_TILL),'dd/mm/yyyy') valid,CED_AFFIRMATIVE,nvl(CED_IDENTIFICATION_MARK,'NA') identy,nvl(CED_FATHER_NAME,nvl(CED_HUSBAND_NAME,'NA')) guardian,nvl(CED_EMERGENCY_NO,'NA') emergency,nvl(a.CTM_TYPE_DESC,'NA') type,nvl(CED_UNIQUE_ID_VALUE,'NA') typevalue,nvl(CED_PHONE_NO,'NA') CED_PHONE_NO,CDP_DEPT_NAME,nvl(b.CTM_TYPE_DESC,'NA') category from t_cemp_details,T_CNT_DEPT_MASTER,t_cemp_type_master a,t_cemp_type_master b where CED_SAFETY_PASS_NO=:CED_SAFETY_PASS_NO and CED_DEPT_CODE=CDP_DEPT_CODE(+) and CDP_COMP_CODE(+)=CED_COMPANY_CODE and a.CTM_TYPE_CODE=CED_UNIQUE_ID_TYPE and CED_CATEGORY=b.CTM_TYPE_CODE"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":CED_SAFETY_PASS_NO", sp_no))
                dt.Clear()
                dt = getRecord(cmd, con)
                If dt.Rows.Count > 0 Then
                    name = dt.Rows(0).Item("name")
                    DOB = dt.Rows(0).Item("DOB")
                    gender = dt.Rows(0).Item("CED_GENDER")
                    affirmative = dt.Rows(0).Item("CED_AFFIRMATIVE")
                    guardian = dt.Rows(0).Item("guardian")
                    emergency = dt.Rows(0).Item("emergency")
                    identification = dt.Rows(0).Item("identy")
                    wphone = dt.Rows(0).Item("CED_PHONE_NO")
                    deptname = dt.Rows(0).Item("CDP_DEPT_NAME")
                    idcard = dt.Rows(0).Item("type")
                    idnumber = dt.Rows(0).Item("typevalue")
                    category = dt.Rows(0).Item("category")
                    If IsDBNull(dt.Rows(0).Item("valid")) Then
                        spvalid = "NA"
                    Else
                        spvalid = dt.Rows(0).Item("valid")
                    End If
                End If

                Dim dt1 As New DataTable
                ls_sql = "select nvl(CCA_NAME,'')||' '||nvl(CCA_HOUSE_NO,'')||' '||nvl(CCA_STREET,'') address,CCA_PIN from T_CWM_CEMP_ADDRS where CCA_SAFETY_PASS_NO='" + sp_no + "' and CCA_ADDRESS_ID =(select max(CCA_ADDRESS_ID) from  T_CWM_CEMP_ADDRS where CCA_SAFETY_PASS_NO='" + sp_no + "')"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If


                dt1 = getRecord(ls_sql, con)
                If dt1.Rows.Count > 0 Then
                    localaddress = dt1.Rows(0).Item("address")
                    pincode = dt1.Rows(0).Item("CCA_PIN")
                End If
            End If

        Catch ex As Exception

        End Try
        Dim dt2 As New DataTable
        ls_sql = "select  B.CTM_TYPE_DESC skilltype,C.CTM_TYPE_DESC skill from hrace.t_cwm_cemp_skill a,t_cemp_type_master b,hrace.t_cemp_type_master c where A.CCS_SKILL_CD=B.CTM_TYPE_CODE and A.CCS_SKILL_TYPE_CD=C.CTM_TYPE_CODE and (A.CCS_DELETE_FLG<>'Y' or A.CCS_DELETE_FLG is null)  and A.CCS_SAFETY_PASS_NO='" + sp_no + "'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        dt2 = getRecord(ls_sql, con)
        If dt2.Rows.Count > 0 Then
            skilltype = dt2.Rows(0).Item("skilltype")
            skill = dt2.Rows(0).Item("skill")
        End If
        cacomail = sendmailtoagency(sp_no)
        '======================New medical date implemented by TCS L2 Team 13/08/2025===========================================
        Dim dt3 As New DataTable
        ls_sql = "SELECT TO_CHAR(SBD_BOOKING_DATE, 'dd/mm/yyyy') meddt,DECODE(SBD_SLOT_TYPE,'SL2','2 PM – 6 PM','9 AM – 1 PM') medtime,TO_CHAR(SBD_BOOKING_DATE, 'dd/mm/yyyy') trndt,"
        ls_sql += "DECODE(SBD_SLOT_TYPE,'SL2','2 PM – 6 PM','9 AM – 1 PM') trntime FROM hrace.T_TRNG_SLOT_BK_DTLS WHERE SBD_REQ_NO = '" + Session("requestnumber") + "' AND SBD_SAFETY_PASS_NO = '" + sp_no + "' AND SBD_BOOKING_TYPE='MED' AND  SBD_COMPANY_CD='" + comp_cd + "'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        dt3 = getRecord(ls_sql, con)
        If (dt3.Rows.Count > 0) Then
            meddt = dt3.Rows(0).Item("meddt")
            trndt = dt3.Rows(0).Item("trndt")
            medtime = dt3.Rows(0).Item("medtime")
            trntime = dt3.Rows(0).Item("trntime")
        Else
            ls_sql = "select to_char(CMT_MED_DT,'dd/mm/yyyy') meddt,to_char(CMT_MED_DT,'hh:mi:ss AM') medtime,to_char(CMT_TRN_DT,'dd/mm/yyyy') trndt,to_char(CMT_TRN_DT,'hh:mi:ss AM') trntime  from T_CWM_MED_TRN_DTL where CMT_REQ_NO='" + Session("requestnumber") + "' and CMT_SAFETY_PASS_NO='" + sp_no + "'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt3 = getRecord(ls_sql, con)
            If dt3.Rows.Count > 0 Then
                meddt = dt3.Rows(0).Item("meddt")
                trndt = dt3.Rows(0).Item("trndt")
                medtime = dt3.Rows(0).Item("medtime")
                trntime = dt3.Rows(0).Item("trntime")
            End If
        End If

        '=============================================******==========================================================================


        'ls_sql = "select to_char(CMT_MED_DT,'dd/mm/yyyy') meddt,to_char(CMT_MED_DT,'hh:mi:ss AM') medtime,to_char(CMT_TRN_DT,'dd/mm/yyyy') trndt,to_char(CMT_TRN_DT,'hh:mi:ss AM') trntime  from T_CWM_MED_TRN_DTL where CMT_REQ_NO='" + Session("requestnumber") + "' and CMT_SAFETY_PASS_NO='" + sp_no + "'"
        'If con.State = ConnectionState.Closed Then
        '    con.Open()
        'End If
        'dt3 = getRecord(ls_sql, con)
        'If dt3.Rows.Count > 0 Then
        '    meddt = dt3.Rows(0).Item("meddt")
        '    trndt = dt3.Rows(0).Item("trndt")
        '    medtime = dt3.Rows(0).Item("medtime")
        '    trntime = dt3.Rows(0).Item("trntime")
        'End If

        Response.Clear()
        HttpContext.Current.Response.Clear()
        Response.Buffer = True
        Response.ContentType = "application/pdf"
        Response.AddHeader("content-disposition", "attachment;filename=Medical Examination Form.pdf")
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Dim sw As New StringWriter()
        Dim hw As New HtmlTextWriter(sw)
        Dim pdfDoc As New Document(PageSize.A4, 10.0F, 10.0F, 1.0F, 0.0F)
        Dim tabheader As New PdfPTable(1)
        tabheader.WidthPercentage = 70
        Dim cellheader As New PdfPCell()

        cellheader.AddElement(New Phrase("CONTRACTOR EMPLOYEE'S MEDICAL EXAMINATION FORM (Srl No:           )", FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)))
        cellheader.HorizontalAlignment = Element.ALIGN_CENTER
        cellheader.Border = iTextSharp.text.Rectangle.NO_BORDER

        tabheader.AddCell(cellheader)
        ' Dim line As LineSeparator = New LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)
        'Dim p As Paragraph = New Paragraph(New Chunk())


        Dim tabsubheader As New PdfPTable(2)
        tabsubheader.WidthPercentage = 90
        Dim widths As Single() = New Single() {70.0F, 70.0F}
        tabsubheader.SetWidths(widths)
        Dim cellsubheader As New PdfPCell()
        cellsubheader.AddElement(New Phrase("Name Of The Centre:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader.HorizontalAlignment = Element.ALIGN_LEFT
        cellsubheader.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader)
        Dim cellsubheader1 As New PdfPCell()
        cellsubheader1.AddElement(New Phrase("Medical Exam Date:" + meddt + "  Time:" + medtime, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader1.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsubheader1.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader1)
        Dim cellsubheader2 As New PdfPCell()
        cellsubheader2.AddElement(New Phrase("Location:" + location + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader2.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsubheader2.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader2)
        Dim cellsubheader3 As New PdfPCell()
        cellsubheader3.AddElement(New Phrase("Training Date:" + trndt + "  Time:" + trntime, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsubheader3.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsubheader3.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader.AddCell(cellsubheader3)
        'Dim p1 As Paragraph = New Paragraph(New Chunk())
        Dim line2 As LineSeparator = New LineSeparator(1.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)
        Dim tabsubheader2 As New PdfPTable(2)
        tabsubheader2.WidthPercentage = 90
        tabsubheader2.SetWidths(widths)
        Dim cell1subheader As New PdfPCell()
        cell1subheader.AddElement(New Phrase("Request Number With Date: " + Session("requestnumber") + " (Dated:" + reqdate + ")", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cell1subheader.HorizontalAlignment = Element.ALIGN_LEFT
        cell1subheader.Border = iTextSharp.text.Rectangle.NO_BORDER

        tabsubheader2.AddCell(cell1subheader)
        Dim cellsub1header1 As New PdfPCell()
        cellsub1header1.AddElement(New Phrase("Safety Pass No:" + sp_no + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub1header1.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub1header1.Border = iTextSharp.text.Rectangle.NO_BORDER

        tabsubheader2.AddCell(cellsub1header1)


        'Dim cellsub1header2 As New PdfPCell()
        'cellsub1header2.Colspan = 2
        'cellsub1header2.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        'cellsub1header2.HorizontalAlignment = Element.ALIGN_RIGHT
        'cellsub1header2.Border = iTextSharp.text.Rectangle.NO_BORDER
        'tabsubheader2.AddCell(cellsub1header2)
        Dim tabsubheader3 As New PdfPTable(2)
        tabsubheader3.WidthPercentage = 90
        Dim widths1 As Single() = New Single() {50.0F, 70.0F}
        tabsubheader3.SetWidths(widths1)
        Dim cellsub2header As New PdfPCell()
        cellsub2header.AddElement(New Phrase("VENDOR DETAILS", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        cellsub2header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub2header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsubheader3.AddCell(cellsub2header)
        Dim cellsub3header As New PdfPCell()
        cellsub3header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub3header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub3header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub3header)
        Dim cellsub4header As New PdfPCell()
        cellsub4header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub4header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub4header.AddElement(New Phrase("Vendor Code:" + vendorcode + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub4header)
        Dim cellsub5header As New PdfPCell()
        cellsub5header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub5header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub5header.AddElement(New Phrase("Dept Name:" + deptname + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub5header)
        Dim cellsub6header As New PdfPCell()
        cellsub6header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub6header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub6header.AddElement(New Phrase("Vendor Name:" + vendorname + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub6header)
        Dim cellsub7header As New PdfPCell()
        cellsub7header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub7header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub7header.AddElement(New Phrase("Local address:" + vendoraddress + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub7header)
        Dim cellsub8header As New PdfPCell()
        cellsub8header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub8header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub8header.AddElement(New Phrase("Contact No:" + vendcontact + "       Mobile No:" + vendmobile, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub8header)
        Dim cellsub9header As New PdfPCell()
        cellsub9header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub9header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub9header.AddElement(New Phrase("Plant Code:" + comp_cd + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub9header)
        Dim cellsub10header As New PdfPCell()
        cellsub10header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub10header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub10header.AddElement(New Phrase("Email ID CA/CO:" + cacomail.ToLower(), FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub10header)
        Dim cellsub11header As New PdfPCell()
        cellsub11header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub11header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub11header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader3.AddCell(cellsub11header)

        Dim cellsub12header As New PdfPCell()
        cellsub12header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub12header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub12header.AddElement(New Phrase("PARTICIPANT'S DETAILS", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsubheader3.AddCell(cellsub12header)
        Dim cellsub13header As New PdfPCell()
        cellsub13header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub13header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub13header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsubheader3.AddCell(cellsub13header)
        Dim tabsubheader4 As New PdfPTable(3)
        tabsubheader4.WidthPercentage = 90
        Dim widths2 As Single() = New Single() {40.0F, 20.0F, 30.0F}
        tabsubheader4.SetWidths(widths2)
        Dim cellsub14header As New PdfPCell()
        cellsub14header.Colspan = 3
        cellsub14header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub14header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub14header.AddElement(New Phrase("Name:" + name + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub14header)

        Dim cellsub17header As New PdfPCell()
        cellsub17header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub17header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub17header.AddElement(New Phrase("Date Of Birth:" + DOB + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub17header)
        Dim cellsub18header As New PdfPCell()
        cellsub18header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub18header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub18header.AddElement(New Phrase("Gender:" + gender + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub18header)
        Dim cellsub19header As New PdfPCell()
        cellsub19header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub19header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub19header.AddElement(New Phrase("Affirmative:" + affirmative + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub19header)
        Dim cellsub20header As New PdfPCell()
        cellsub20header.Colspan = 3
        cellsub20header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub20header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub20header.AddElement(New Phrase("Identification Marks:" + identification, FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub20header)
        Dim cellsub23header As New PdfPCell()
        cellsub23header.Colspan = 3
        cellsub23header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub23header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub23header.AddElement(New Phrase("Father's/Husband's Name:" + guardian + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub23header)
        Dim cellsub26header As New PdfPCell()
        cellsub26header.Colspan = 3
        cellsub26header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub26header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub26header.AddElement(New Phrase("Local Address:" + localaddress + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub26header)
        Dim cellsub29header As New PdfPCell()
        cellsub29header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub29header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub29header.AddElement(New Phrase("Pin Code:" + pincode + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub29header)
        Dim cellsub30header As New PdfPCell()
        cellsub30header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub30header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub30header.AddElement(New Phrase("Phone No:" + wphone + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub30header)
        Dim cellsub31header As New PdfPCell()
        cellsub31header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub31header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub31header.AddElement(New Phrase("Emergency Contact No:" + emergency + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub31header)
        Dim cellsub32header As New PdfPCell()
        cellsub32header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub32header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub32header.AddElement(New Phrase("SP Valid Till Date:" + spvalid + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub32header)
        Dim cellsub33header As New PdfPCell()
        cellsub33header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub33header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub33header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub33header)
        Dim cellsub34header As New PdfPCell()
        cellsub34header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub34header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub34header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub34header)
        Dim cellsub35header As New PdfPCell()
        cellsub35header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub35header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub35header.AddElement(New Phrase("ID Card type:" + idcard + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub35header)
        Dim cellsub36header As New PdfPCell()
        cellsub36header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub36header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub36header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub36header)
        Dim cellsub37header As New PdfPCell()
        cellsub37header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub37header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub37header.AddElement(New Phrase("ID Card No:" + idnumber + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub37header)
        Dim cellsub38header As New PdfPCell()
        cellsub38header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub38header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub38header.AddElement(New Phrase("Category:" + category + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub38header)
        Dim cellsub39header As New PdfPCell()
        cellsub39header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub39header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub39header.AddElement(New Phrase("Skill Type:" + skill + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub39header)
        Dim cellsub40header As New PdfPCell()
        cellsub40header.Colspan = 5
        cellsub40header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub40header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub40header.AddElement(New Phrase("Skill: " + skilltype + "", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsubheader4.AddCell(cellsub40header)
        Dim tabsub4header As New PdfPTable(1)
        tabsub4header.WidthPercentage = 80
        'Dim cellsub41header As New PdfPCell()
        'cellsub41header.HorizontalAlignment = Element.ALIGN_LEFT
        'cellsub41header.Border = iTextSharp.text.Rectangle.NO_BORDER
        'cellsub41header.AddElement(New Phrase("NOMINEE DETAILS:", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        'tabsub4header.AddCell(cellsub41header)
        Dim tabsub5header As New PdfPTable(1)
        tabsub5header.WidthPercentage = 90
        Dim cellsub42header As New PdfPCell()
        cellsub42header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub42header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub42header.AddElement(New Phrase("DECLARATION:", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsub5header.AddCell(cellsub42header)
        Dim cellsub142header As New PdfPCell()
        cellsub142header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub142header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub142header.AddElement(New Phrase("I hereby declare that the above mentioned information is correct to the best of my knowledge and I bear the responsibility for the correctness of the above mentioned particular.", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsub5header.AddCell(cellsub142header)
        Dim cellsub143header As New PdfPCell()
        cellsub143header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub143header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub143header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        tabsub5header.AddCell(cellsub143header)
        Dim tabsub6header As New PdfPTable(2)
        tabsub6header.WidthPercentage = 90
        Dim cellsub43header As New PdfPCell()
        cellsub43header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub43header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub43header.AddElement(New Phrase("Signature/Thumb Impression Of Candidate", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        tabsub6header.AddCell(cellsub43header)
        Dim cellsub44header As New PdfPCell()
        cellsub44header.HorizontalAlignment = Element.ALIGN_RIGHT
        cellsub44header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub44header.AddElement(New Phrase("                       Vendor's Signature and Stamp", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        tabsub6header.AddCell(cellsub44header)
        Dim tabsub7header As New PdfPTable(1)
        tabsub7header.WidthPercentage = 90
        Dim cellsub45header As New PdfPCell()
        cellsub45header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub45header.Border = iTextSharp.text.Rectangle.NO_BORDER
        cellsub45header.AddElement(New Phrase("MEDICAL EXAMINATION REPORT : TO BE FILLED BY DOCTOR                Vision Test Required(Yes / No)", FontFactory.GetFont("Arial", 8, iTextSharp.text.Font.BOLD)))
        tabsub7header.AddCell(cellsub45header)
        Dim tabsub8header As New PdfPTable(2)
        tabsub8header.WidthPercentage = 90
        Dim widths4 As Single() = New Single() {80.0F, 70.0F}
        tabsub8header.SetWidths(widths4)
        Dim cellsub46header As New PdfPCell()
        cellsub46header.AddElement(New Phrase("Height (in cms):………………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub46header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub46header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub46header)
        Dim cellsub47header As New PdfPCell()
        cellsub47header.AddElement(New Phrase("Respiratory System:……………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub47header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub47header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub47header)
        Dim cellsub48header As New PdfPCell()
        cellsub48header.AddElement(New Phrase("Weight(in Kg):…………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub48header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub48header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub48header)
        Dim cellsub49header As New PdfPCell()
        cellsub49header.AddElement(New Phrase("Blood Pressure(in mm of Hg):……………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub49header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub49header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub49header)
        Dim cellsub50header As New PdfPCell()
        cellsub50header.AddElement(New Phrase("Ability to Walk closed eyes:OK/ Not OK", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub50header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub50header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub50header)
        Dim cellsub51header As New PdfPCell()
        cellsub51header.AddElement(New Phrase("Pulse/min:……………………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub51header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub51header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub51header)
        Dim cellsub52header As New PdfPCell()
        cellsub52header.AddElement(New Phrase("Romberg's Sign:Positive/Negative", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub52header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub52header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub52header)
        Dim cellsub53header As New PdfPCell()
        cellsub53header.AddElement(New Phrase("CVS:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub53header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub53header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub53header)
        Dim cellsub54header As New PdfPCell()
        cellsub54header.AddElement(New Phrase("Physical", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub54header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub54header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub54header)
        Dim cellsub55header As New PdfPCell()
        cellsub55header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub55header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub55header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub55header)
        Dim cellsub56header As New PdfPCell()
        cellsub56header.AddElement(New Phrase("Hearing:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub56header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub56header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub56header)
        Dim cellsub57header As New PdfPCell()
        cellsub57header.AddElement(New Phrase("Other Heart Related Abnormalities:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub57header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub57header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub57header)
        Dim cellsub58header As New PdfPCell()
        cellsub58header.AddElement(New Phrase("Left Ear:Ok/ Not Ok        Right Ear:Ok/ Not Ok", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub58header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub58header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub58header)
        Dim cellsub59header As New PdfPCell()
        cellsub59header.AddElement(New Phrase("(Like Congenital , Arrhythmia, Coronary disease etc.)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub59header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub59header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub59header)
        Dim cellsub60header As New PdfPCell()
        cellsub60header.AddElement(New Phrase("CNS", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub60header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub60header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub60header)
        Dim cellsub61header As New PdfPCell()
        cellsub61header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub61header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub61header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub61header)
        Dim cellsub62header As New PdfPCell()
        cellsub62header.AddElement(New Phrase("Other Abnormalities", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub62header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub62header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub62header)
        Dim cellsub63header As New PdfPCell()
        cellsub63header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub63header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub63header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub63header)
        Dim cellsub64header As New PdfPCell()
        cellsub64header.AddElement(New Phrase("(Like Facial Palsy, Missing / Extra / Arthritic Finger etc)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub64header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub64header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub64header)
        Dim cellsub65header As New PdfPCell()
        cellsub65header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub65header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub65header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub65header)
        Dim cellsub66header As New PdfPCell()
        cellsub66header.AddElement(New Phrase("Eye/Vision", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub66header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub66header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub66header)
        Dim cellsub67header As New PdfPCell()
        cellsub67header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub67header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub67header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub67header)
        Dim cellsub68header As New PdfPCell()
        cellsub68header.AddElement(New Phrase("Colour Perception:   Ok/Defective", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub68header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub68header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub68header)
        Dim cellsub69header As New PdfPCell()
        cellsub69header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub69header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub69header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub69header)
        Dim cellsub70header As New PdfPCell()
        cellsub70header.AddElement(New Phrase("Right Eye(With / Without Glasses)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub70header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub70header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub70header)
        Dim cellsub71header As New PdfPCell()
        cellsub71header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub71header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub71header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub71header)
        Dim cellsub72header As New PdfPCell()
        cellsub72header.AddElement(New Phrase("Left Eye(With / Without Glasses)", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub72header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub72header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub72header)
        Dim cellsub73header As New PdfPCell()
        cellsub73header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub73header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub73header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub73header)
        Dim cellsub74header As New PdfPCell()
        cellsub74header.AddElement(New Phrase("Squint:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub74header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub74header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub74header)
        Dim cellsub75header As New PdfPCell()
        cellsub75header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub75header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub75header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub75header)
        Dim cellsub76header As New PdfPCell()
        cellsub76header.AddElement(New Phrase("Eye Comments:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub76header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub76header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub76header)
        Dim cellsub77header As New PdfPCell()
        cellsub77header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub77header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub77header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub77header)
        Dim cellsub78header As New PdfPCell()
        cellsub78header.AddElement(New Phrase("Pathology Report", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub78header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub78header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub78header)
        Dim cellsub79header As New PdfPCell()
        cellsub79header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub79header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub79header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub79header)
        Dim cellsub80header As New PdfPCell()
        cellsub80header.AddElement(New Phrase("Haemoglobin(in gm%):…………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub80header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub80header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub80header)
        Dim cellsub81header As New PdfPCell()
        cellsub81header.AddElement(New Phrase("Blood Group:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub81header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub81header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub81header)
        Dim cellsub82header As New PdfPCell()
        cellsub82header.AddElement(New Phrase("Random Blood Sugar(in mg%):…………", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub82header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub82header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub82header)
        Dim cellsub83header As New PdfPCell()
        cellsub83header.AddElement(New Phrase("MR no.:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub83header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub83header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub83header)
        Dim cellsub86header As New PdfPCell()
        cellsub86header.AddElement(New Phrase("History(if any):     Hydrocele/Hernia                    Amputation/ Polydoctyl", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub86header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub86header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub86header)
        Dim cellsub87header As New PdfPCell()
        cellsub87header.AddElement(New Phrase("          Communicable Diseases                          Epilepsy", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub87header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub87header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub87header)
        Dim cellsub88header As New PdfPCell()
        cellsub88header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub88header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub88header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub88header)
        Dim cellsub89header As New PdfPCell()
        cellsub89header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub89header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub89header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub89header)
        Dim cellsub84header As New PdfPCell()
        cellsub84header.AddElement(New Phrase("REHABILITATION COMMITTEE REPORTS (If applicable):", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub84header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub84header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub84header)
        Dim cellsub85header As New PdfPCell()
        cellsub85header.Colspan = 2
        cellsub85header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub85header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub85header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub85header)
        Dim cellsub90header As New PdfPCell()
        cellsub90header.AddElement(New Phrase("Comments:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub90header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub90header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub90header)
        Dim cellsub91header As New PdfPCell()
        cellsub91header.AddElement(New Phrase("", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub91header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub91header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub91header)
        Dim cellsub95header As New PdfPCell()
        cellsub95header.Colspan = 2
        cellsub95header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub95header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub95header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub95header)

        Dim cellsub93header As New PdfPCell()
        cellsub93header.Colspan = 2
        cellsub93header.AddElement(New Phrase("Chairman Signature                     Member Signature                     Member Signature                     Member Signature ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub93header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub93header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub93header)
        Dim cellsub96header As New PdfPCell()
        cellsub96header.Colspan = 2
        cellsub96header.AddElement(New Phrase("UNDERTAKING DETAILS(If applicable):", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub96header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub96header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub96header)
        Dim cellsub97header As New PdfPCell()
        cellsub97header.Colspan = 2
        cellsub97header.AddElement(New Phrase(" ", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub97header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub97header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub97header)
        Dim cellsub98header As New PdfPCell()
        cellsub98header.Colspan = 2
        cellsub98header.AddElement(New Phrase("This is to certify that the above person on the basis of medical examination", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub98header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub98header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub98header)
        Dim cellsub99header As New PdfPCell()
        cellsub99header.Colspan = 2
        cellsub99header.AddElement(New Phrase("is found FIT / UNFIT to work with TSL & its associate company                            Medical Examiner's Signature              Stamp              Date", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub99header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub99header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub99header)
        Dim cellsub100header As New PdfPCell()
        cellsub100header.Colspan = 2
        cellsub100header.AddElement(New Phrase("DECLARATION OF CANDIDATE:", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.BOLD)))
        cellsub100header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub100header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub100header)
        Dim cellsub101header As New PdfPCell()
        cellsub101header.Colspan = 2
        cellsub101header.AddElement(New Phrase("I have been medically examined by the doctor with my consent.                           Signature  & Thumb Impresssion of Candidate", FontFactory.GetFont("Arial", 7, iTextSharp.text.Font.NORMAL)))
        cellsub101header.HorizontalAlignment = Element.ALIGN_LEFT
        cellsub101header.Border = iTextSharp.text.Rectangle.NO_BORDER
        tabsub8header.AddCell(cellsub101header)
        Dim sr As New StringReader(sw.ToString())


        Dim htmlparser As New HTMLWorker(pdfDoc)
        Dim pdfWrite As PdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream)

        'pdfDoc.Add(table1)
        pdfDoc.Open()
        'Dim bfTimes As BaseFont = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont. , False)

        'Dim times As New iTextSharp.text.Font(bfTimes)
        pdfDoc.Add(tabheader)
        pdfDoc.Add(tabsubheader)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(tabsubheader2)
        pdfDoc.Add(New Phrase(" "))
        pdfDoc.Add(line2)
        Dim cb As PdfContentByte = pdfWrite.DirectContent
        Dim rect = New iTextSharp.text.Rectangle(465, 650, 550, 730)
        rect.Border = iTextSharp.text.Rectangle.BOX
        rect.BorderWidth = 1
        rect.BorderColor = New BaseColor(0, 0, 0)
        'cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 70.0, 70.0, 0)
        cb.Rectangle(rect)
        Dim ct As ColumnText = New ColumnText(cb)
        Dim cpharse1 As Phrase = New Phrase("Affix recent passport size photograph attested by contractor with vendor stamp", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD))
        ct.SetSimpleColumn(cpharse1, 465, 645, 550, 710, 10, Element.ALIGN_LEFT)
        'ct.AddElement(New Phrase("Affix recent passport size photograph attested by contractor with vendor stamp", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.BOLD)))
        ct.Go()
        Dim cb1 As PdfContentByte = pdfWrite.DirectContent
        Dim rect1 = New iTextSharp.text.Rectangle(320, 250, 540, 350)
        rect1.Border = iTextSharp.text.Rectangle.BOX
        rect1.BorderWidth = 1
        rect1.BorderColor = New BaseColor(0, 0, 0)
        'cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "", 70.0, 70.0, 0)
        cb1.Rectangle(rect1)
        Dim ct1 As ColumnText = New ColumnText(cb1)
        Dim cphrase As Phrase = New Phrase("Doctor Comments (if any):", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL))
        ct1.SetSimpleColumn(cphrase, 320, 245, 540, 345, 20, Element.ALIGN_TOP)
        'ct1.AddElement(New Phrase("Doctor Comments (if any):", FontFactory.GetFont("Arial", 6, iTextSharp.text.Font.NORMAL)))
        ct1.Go()
        pdfDoc.Add(tabsubheader3)
        pdfDoc.Add(tabsubheader4)
        pdfDoc.Add(tabsub4header)
        pdfDoc.Add(tabsub5header)
        pdfDoc.Add(New Paragraph(""))
        pdfDoc.Add(tabsub6header)
        pdfDoc.Add(New Phrase(" "))
        pdfDoc.Add(line2)
        pdfDoc.Add(tabsub7header)
        pdfDoc.Add(tabsub8header)
        htmlparser.Parse(sr)
        pdfDoc.Close()
        Response.Write(pdfDoc)
        HttpContext.Current.Response.End()
    End Sub
    Protected Sub status_histry_click(ByVal sender As Object, ByVal e As System.EventArgs)

        Response.Redirect("ospEmpDetails.aspx")
    End Sub
    Public Sub clearAll()
        clearProfile()
        clearAddress()
        clearNominee()
        clearQualification()

        btnUpdateAddress.Visible = False
        gvAddress.DataSource = Nothing
        gvAddress.DataBind()

        btnUpdateNominee.Visible = False
        gvNominee.DataSource = Nothing
        gvNominee.DataBind()

        btnUpdateQual.Visible = False
        gvQualification.DataSource = Nothing
        gvQualification.DataBind()

        clearSkill()
        clearTraining()
        'clearpv()
        clearmed()
        clearagedrv()
        clearexperience()

        btnUpdateSkill.Visible = False
        gvSkill.DataSource = Nothing
        gvSkill.DataBind()

        btnUpdateTraining.Visible = False
        gvTraining.DataSource = Nothing
        gvTraining.DataBind()

        'btnupdatepv.Visible = False
        'gvpv.DataSource = Nothing
        'gvpv.DataBind()

        btnupdatemed.Visible = False
        gvmed.DataSource = Nothing
        gvmed.DataBind()

        btnupdateage.Visible = False
        grdage.DataSource = Nothing
        grdage.DataBind()

        btnUpdateExp.Visible = False
        grvExp.DataSource = Nothing
        grvExp.DataBind()
    End Sub

    Protected Sub chkSelectAddress(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim vIsRowSelected As Boolean = False
        clearAddress()
        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent


            Dim chkSelect As CheckBox = CType(gvrow.FindControl("chkSelectAddress"), CheckBox)

            If chkSelect.Enabled = True And chkSelect.Checked = True Then

                vIsRowSelected = True
                Dim vAddressID As String = CType(gvrow.FindControl("hidAddressID"), HiddenField).Value
                Session("AddressID") = vAddressID


                Dim vAddressRemark As String = CType(gvrow.FindControl("hidremark"), HiddenField).Value
                Session("AddressRemark") = vAddressRemark

                Dim vAddressdocid As String = CType(gvrow.FindControl("hiddocseq"), HiddenField).Value
                Session("Addressdocid") = vAddressdocid

                Dim vAddressType As String = CType(gvrow.FindControl("hidAddType"), HiddenField).Value

                Dim vName As String = gvrow.Cells(2).Text.Trim().Replace("&nbsp;", "")
                Dim vHouseNo As String = gvrow.Cells(3).Text.Trim().Replace("&nbsp;", "")
                Dim vStreet As String = gvrow.Cells(4).Text.Trim().Replace("&nbsp;", "")
                Dim vCityCD As String = CType(gvrow.FindControl("hidAddCity"), HiddenField).Value

                Dim vVillage As String = gvrow.Cells(5).Text.Trim().Replace("&nbsp;", "")
                Dim vPO As String = gvrow.Cells(6).Text.Trim().Replace("&nbsp;", "")
                Dim vThana As String = gvrow.Cells(7).Text.Trim().Replace("&nbsp;", "")
                Dim vDistrictCD As String = CType(gvrow.FindControl("hidAddDistrict"), HiddenField).Value

                Dim vStateCD As String = CType(gvrow.FindControl("hidAddState"), HiddenField).Value
                Dim vCountryCD As String = CType(gvrow.FindControl("hidAddCountry"), HiddenField).Value
                'Dim vPin As String = gvrow.Cells(8).Text.Trim().Replace("&nbsp;", "")

                'Dim vMobile As String = gvrow.Cells(9).Text.Trim().Replace("&nbsp;", "")
                'Dim vEmailID As String = gvrow.Cells(10).Text.Trim().Replace("&nbsp;", "")
                'Dim vLandLine As String = gvrow.Cells(11).Text.Trim().Replace("&nbsp;", "")

                Dim vPin As String = gvrow.Cells(12).Text.Trim().Replace("&nbsp;", "")

                Dim vMobile As String = gvrow.Cells(13).Text.Trim().Replace("&nbsp;", "")
                Dim vEmailID As String = gvrow.Cells(14).Text.Trim().Replace("&nbsp;", "")
                Dim vLandLine As String = gvrow.Cells(15).Text.Trim().Replace("&nbsp;", "")

                Dim filename As String = CType(gvrow.FindControl("lnkexp"), LinkButton).Text
                GetCity(vStateCD)

                cmbAddressType.SelectedValue = vAddressType
                txtAddName.Text = vName
                txtAddHouseNo.Text = vHouseNo
                txtAddStreet.Text = vStreet
                cmbAddCity.SelectedValue = vCityCD
                cmbAddState.SelectedValue = vStateCD
                cmbAddCountry.SelectedValue = vCountryCD
                txtAddPIN.Text = vPin
                txtAddMobile.Text = vMobile
                txtAddEmail.Text = vEmailID
                txtLandLine.Text = vLandLine
                lbladdattachname.Text = filename

                GetDistrict(vStateCD)
                txtAddVillage.Text = vVillage
                txtAddPO.Text = vPO
                txtAddThana.Text = vThana
                If vDistrictCD = "" Then
                    cmbAddDistrict.SelectedValue = 0
                Else
                    cmbAddDistrict.SelectedValue = vDistrictCD
                End If

                btnUpdateAddress.Enabled = True
                Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
                If status.Equals("Y") Then
                    btnUpdateAddress.Visible = False
                Else
                    btnUpdateAddress.Visible = True
                End If

                btnSaveAddress.Visible = False

            ElseIf chkSelect.Enabled = True And chkSelect.Checked = False Then
                btnSaveAddress.Visible = True
                btnUpdateAddress.Visible = False
                clearAddress()
            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Public Sub clearSkill()

        cmbSkSkillType.SelectedValue = 0
        cmbSkSkillType.SelectedIndex = 0
        txtQualRemarks.Text = String.Empty
        'txtQualRemarks.Visible = False
        cmbSkSkill.Items.Clear()
        txtSkRemarks.Text = ""
        ddlSKAss.SelectedValue = "NA"

        'btnSaveSkill.Enabled = True
        btnUpdateSkill.Enabled = False

        cmbSkSkillType.Enabled = True
        cmbSkSkill.Enabled = True
        lbl_fileuploadskill.Text = String.Empty
        txtSkRemarks.Enabled = True
        ddlSKAss.Enabled = True
        FileUploadSkill.Enabled = True
        lbl_fileuploadskill.Text = ""

        ddlSkillTrade.Text = "-"
        'ddlSkillTrade.SelectedIndex = 0
        txtOthSkillTrade.Text = ""
        txtOthSkillTrade.Visible = False

    End Sub
    Protected Sub chkSelectQual(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim vIsRowSelected As Boolean = False
        clearQualification()
        If lblcertname.Visible = False Then
            lblcertname.Text = True
        End If
        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent
            If CType(gvrow.FindControl("chkSelectQual"), CheckBox).Checked = True Then
                vIsRowSelected = True
                Dim vQualID As String = CType(gvrow.FindControl("hidQualID"), HiddenField).Value
                Session("QualID") = vQualID

                Dim vQualType As String = CType(gvrow.FindControl("hidQualType"), HiddenField).Value
                Dim vQualCD As String = CType(gvrow.FindControl("hidQualCD"), HiddenField).Value
                Dim vcertid As String = CType(gvrow.FindControl("hidqualCERT"), HiddenField).Value
                Dim vcertname As String = CType(gvrow.FindControl("lnkqual"), LinkButton).Text
                Dim remarks As String = gvrow.Cells(4).Text.Trim
                'Fill The drop down for Qualification based on Qualification Type
                FillDropDown(cmbQualification, vQualType)
                cmbQualType.SelectedValue = vQualType
                cmbQualification.SelectedValue = vQualCD
                txtQualRemarks.Text = remarks.Trim.Replace("&nbsp;", "")
                lblcertname.Text = vcertname
                hdqualcertid.Value = vcertid
                hdqualid.Value = vQualID
                Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
                If status.Equals("Y") Then
                    btnUpdateQual.Visible = False
                    btnUpdateQual.Enabled = False
                Else
                    btnUpdateQual.Visible = True
                    btnUpdateQual.Enabled = True
                End If

            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Protected Sub chkSelectAge(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim vIsRowSelected As Boolean = False

        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent
            If CType(gvrow.FindControl("chkSelectAge"), CheckBox).Checked = True Then
                vIsRowSelected = True
                Dim vageID As String = CType(gvrow.FindControl("hdage"), HiddenField).Value
                Dim vdrvID As String = CType(gvrow.FindControl("hddrv"), HiddenField).Value
                Dim vpassID As String = CType(gvrow.FindControl("hdpass"), HiddenField).Value
                Dim dobfile As String = CType(gvrow.FindControl("lnkdownloadage"), LinkButton).Text
                Dim drvfile As String = CType(gvrow.FindControl("lnkdownloaddrv"), LinkButton).Text
                Dim passfile As String = CType(gvrow.FindControl("lnkdownloadpass"), LinkButton).Text

                hiddob.Value = vageID
                hiddrv.Value = vdrvID
                hidpass.Value = vpassID
                lbl_dobfile.Text = dobfile
                lbl_drvfile.Text = drvfile
                lbl_passfile.Text = passfile
                Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
                If status.Equals("Y") Then
                    btnupdateage.Enabled = False
                    btnupdateage.Visible = False
                Else
                    btnupdateage.Enabled = True
                    btnupdateage.Visible = True
                End If

            Else
                clearagedrv()
                btnupdateage.Enabled = False
            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Protected Sub chkSelectExp(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim vIsRowSelected As Boolean = False
        clearexperience()
        getExpDom()
        getExpLocState()
        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent
            If CType(gvrow.FindControl("chkSelectExp"), CheckBox).Checked = True Then
                vIsRowSelected = True
                Dim vExpID As String = CType(gvrow.FindControl("hidsrl"), HiddenField).Value
                Dim vExpSafetypass As String = CType(gvrow.FindControl("hidsafety"), HiddenField).Value
                Dim vcompname As String = gvrow.Cells(2).Text
                Dim vstdt As String = gvrow.Cells(3).Text
                Dim venddt As String = gvrow.Cells(4).Text
                Dim vdesig As String = gvrow.Cells(5).Text
                Dim vworkarea As String = CType(gvrow.FindControl("hidworkarea"), HiddenField).Value
                Dim vworkstate As String = CType(gvrow.FindControl("hidworklocation"), HiddenField).Value
                Dim attachmentname As String = CType(gvrow.FindControl("lnkexp"), LinkButton).Text
                Dim certno As String = CType(gvrow.FindControl("hidcertno"), HiddenField).Value
                ' Dim vworkarea As String =CType(gvrow.FindControl("hidworkarea"),Hiddenfield).value
                Session("ExpID") = vExpID
                hidexpsafety.Value = vExpSafetypass
                txtcompname.Text = vcompname
                txtstdt.Text = vstdt
                txtenddt.Text = venddt
                txtdesignation.Text = vdesig
                lbl_uploadedexp.Text = attachmentname
                hidcertno.Value = certno
                Try
                    drpexparea.SelectedValue = vworkarea
                Catch ex As Exception
                    If txt_otherdom.Visible = False Then
                        txt_otherdom.Visible = True
                    End If
                    txt_otherdom.Text = vworkarea
                    drpexparea.SelectedValue = "EXDM0038"
                End Try


                drpexpstate.SelectedValue = vworkstate.Substring(0, 2)
                getExpLoc(drpexpstate.SelectedValue)
                drpexploc.SelectedValue = vworkstate.Substring(4, 2)

                btnSaveExp.Visible = False
                Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
                If status.Equals("Y") Then
                    btnUpdateExp.Visible = False
                    btnUpdateExp.Enabled = False
                Else
                    btnUpdateExp.Visible = True
                    btnUpdateExp.Enabled = True
                End If

            Else
                clearexperience()
                btnSaveExp.Visible = True
                btnUpdateExp.Enabled = False
            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Protected Sub chkSelectSkill(ByVal sender As Object, ByVal e As System.EventArgs)
        GetSkillType()
        'getSkillTrade()
        Dim vIsRowSelected As Boolean = False
        chk_waive.Checked = False
        txt_WAIVE_DAYS.Text = "" 'ADDED BY PRASUN CHAKRABORTY 27122021 'WI6447
        dv_WAIVE_DAYS.Visible = False 'ADDED BY PRASUN CHAKRABORTY 27122021 'WI6447
        populatewaiveoffReason()

        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent
            If CType(gvrow.FindControl("chkSelectSkill"), CheckBox).Checked = True Then
                vIsRowSelected = True
                Dim vSkillType As String = CType(gvrow.FindControl("hidSkillType"), HiddenField).Value
                Dim vSkillCD As String = CType(gvrow.FindControl("hidSkillCD"), HiddenField).Value
                Dim attachmentname As String = CType(gvrow.FindControl("lnkdownloadskill"), LinkButton).Text
                Dim certno As String = CType(gvrow.FindControl("hidskillcertno"), HiddenField).Value
                Dim assessmenttype As String = CType(gvrow.FindControl("hidSkillassessmenttype"), HiddenField).Value
                Dim hitradeinfo As String = CType(gvrow.FindControl("hidgrdtradeinfo"), HiddenField).Value

                ' Dim vworkarea As String =CType(gvrow.FindControl("hidworkarea"),Hiddenfield).value
                Dim vspecialisation As String = gvrow.Cells(3).Text.Trim
                Dim vassessment As String = gvrow.Cells(4).Text.Trim
                cmbSkSkillType.SelectedValue = vSkillType
                GetTraningSkillCD(vSkillType)
                cmbSkSkill.SelectedValue = vSkillCD
                If vspecialisation = "OK" Or vspecialisation = "Not Ok" Or vspecialisation = "Returned" Then
                Else
                    txtSkRemarks.Text = vspecialisation
                End If
                If assessmenttype.ToString.Trim = "NA" Then
                    drptypeassessment.SelectedValue = "0"
                Else
                    drptypeassessment.SelectedValue = assessmenttype.ToString.Trim
                End If

                ddlSKAss.SelectedValue = vassessment
                lbl_fileuploadskill.Text = attachmentname
                hidcertnoskill.Value = certno
                Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
                If status.Equals("Y") Then
                    btnUpdateSkill.Enabled = False
                    btnUpdateSkill.Visible = False
                Else
                    btnUpdateSkill.Enabled = True
                    btnUpdateSkill.Visible = True
                End If

                btnSaveSkill.Visible = False
                btnSaveSkill.Visible = False


                Dim vSkilledTrades As String = CType(gvrow.FindControl("hidSkillTradeCD"), HiddenField).Value
                'If (Not ddlSkillTrade.Items.FindByValue(vSkilledTrades) Is Nothing) Then

                '    ddlSkillTrade.SelectedValue = vSkilledTrades
                'End If

                ddlSkillTrade.Text = vSkilledTrades + "-" + hitradeinfo
                Session("spno") = TxtSpno.Text
                Dim sqlTradeIRISCheck As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO='" + TxtSpno.Text + "' AND TCD_CERT_CATEG<>'FAIL'"
                Dim dtTradeIRISCheck As New DataTable()
                dtTradeIRISCheck = getRecord(sqlTradeIRISCheck, con)
                If (dtTradeIRISCheck.Rows.Count() > 0) Then
                    ddlSkillTrade_AutoCompleteExtender.MinimumPrefixLength = "1"
                    ddlSkillTrade_AutoCompleteExtender.ServiceMethod = "GetTradeNameIris"
                    LabelAllTrade.Visible = True
                    CheckBoxAllTrade.Visible = True
                End If
                Dim vOtherSkilledTrades As String = gvrow.Cells(6).Text.Trim
                If (vSkilledTrades = "SKTD0029") Then
                    txtOthSkillTrade.Text = vOtherSkilledTrades.ToString.Trim.Replace("'", "''")
                    txtOthSkillTrade.Visible = True
                    lblOthSkillTrade.Visible = True
                    lblSkillassess.Visible = False
                    drp_skillassess.Visible = False
                    ddlSKAss.SelectedValue = "Yes"
                    ddlSKAss.Enabled = True
                ElseIf (vSkilledTrades = "SKTD0028") Then
                    getSkillAssessment()
                    Dim assessment As String = CType(gvrow.FindControl("hidSkillAssessment"), HiddenField).Value
                    drp_skillassess.SelectedValue = assessment
                    txtOthSkillTrade.Visible = False
                    lblOthSkillTrade.Visible = False
                    lblSkillassess.Visible = True
                    drp_skillassess.Visible = True
                    FileUploadSkill.Enabled = False
                    ddlSKAss.SelectedValue = "Yes"
                    ddlSKAss.Enabled = False
                    txtSkRemarks.Enabled = False

                Else
                    txtOthSkillTrade.Visible = False
                    lblOthSkillTrade.Visible = False
                    lblSkillassess.Visible = False
                    drp_skillassess.Visible = False
                    ddlSKAss.SelectedValue = "Yes"
                    ddlSKAss.Enabled = False
                End If
            Else
                ' btnUpdateSkill.Enabled = False
                ' btnSaveSkill.Enabled = True
                'btnUpdateSkill.Visible = False
                'btnSaveSkill.Visible = True
                clearSkill()
            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Protected Sub chkSelectTraining(ByVal sender As Object, ByVal e As System.EventArgs)
        GetTrainingLocation()
        FillDropDown(cmbTraningType, "TRNG")
        'FillDropDown(cmbTrnAgency, "AGEN")
        Dim sqlAgency As String = "select * from t_Cemp_Type_Master where CTM_TYPE ='AGEN' and CTM_STATUS='A' AND (CTM_VALUE IS NULL OR CTM_VALUE='" + Session("Comp_Code") + "') order by CTM_SEQ"
        'Change by anand on 20170427 End ***
        Dim dtAgency As New DataTable()
        dtAgency = getRecord(sqlAgency, con)
        cmbTrnAgency.Items.Clear()
        If dtAgency.Rows.Count > 0 Then
            cmbTrnAgency.DataSource = dtAgency
            cmbTrnAgency.DataTextField = "CTM_TYPE_DESC"
            cmbTrnAgency.DataValueField = "CTM_TYPE_CODE"
            cmbTrnAgency.DataBind()
            cmbTrnAgency.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
        FillDropDown(cmbTrnResult, "RSLT")
        Dim vIsRowSelected As Boolean = False
        clearTraining()
        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent
            If CType(gvrow.FindControl("chkSelectTraining"), CheckBox).Checked = True Then
                vIsRowSelected = True
                Dim vTrainingID As String = CType(gvrow.FindControl("hidTrainingID"), HiddenField).Value
                Session("TrainingID") = vTrainingID

                Dim vTrnAgency As String = CType(gvrow.FindControl("hidTrnAgency"), HiddenField).Value
                Dim vTrnLocation As String = CType(gvrow.FindControl("hidTrnLoc"), HiddenField).Value
                Dim vTrnType As String = CType(gvrow.FindControl("hidTrnType"), HiddenField).Value
                Dim vTrnCourse As String = CType(gvrow.FindControl("hidTrnCourceCD"), HiddenField).Value
                Dim vTrnResult As String = CType(gvrow.FindControl("hidTrnResult"), HiddenField).Value
                Dim vattachmentname As String = CType(gvrow.FindControl("lnkdownloadTrn"), LinkButton).Text
                Dim vcertno As String = CType(gvrow.FindControl("hidTrncerno"), HiddenField).Value
                cmbTrnAgency.SelectedValue = vTrnAgency
                cmbTrnLoc.SelectedValue = vTrnLocation
                cmbTraningType.SelectedValue = vTrnType
                cmbTrnResult.SelectedValue = vTrnResult
                FillDropDown(cmbTrnCource, vTrnType)
                cmbTrnCource.SelectedValue = vTrnCourse
                txtTrnStartDt.Text = gvrow.Cells(5).Text
                txtTrnEndDt.Text = gvrow.Cells(6).Text
                txtTrnRemarks.Text = gvrow.Cells(8).Text
                lbl_fileuploadtrn.Text = vattachmentname
                hidcertrnnoTrns.Value = vcertno
                btnSaveTraining.Visible = False
                Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
                If status.Equals("Y") Then
                    btnUpdateTraining.Visible = False
                    btnUpdateTraining.Enabled = False
                Else
                    btnUpdateTraining.Visible = True
                    btnUpdateTraining.Enabled = True
                End If

            Else
                clearTraining()
                btnSaveTraining.Visible = True
                'btnUpdateTraining.Visible = False
                btnUpdateTraining.Enabled = False

            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Protected Sub btnUpdateSkill_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateSkill.Click


        Dim sqlUpdSkill As String = ""
        Dim vSPNo As String = TxtSpno.Text.Trim.ToUpper().ToString()
        Dim vErrorCount As Integer = 0
        Dim certskill As String = String.Empty
        Dim ls_sql As String = String.Empty
        Dim vSkilledTrades As String = ""
        Dim cmdassess As OracleCommand
        Dim dtassess As New DataTable
        Dim status As String = String.Empty
        Dim vendorst As String = "N"
        Dim waivetag As String = String.Empty
        Dim waivetagreason As String = String.Empty
        Dim statusupdate As String = String.Empty
        Dim Decl_check As String = IIf(chkTermsCondition.Checked, "Y", "N") ' added by prasun on 03012022 'WI6447

        ' added by prasun on 03012022 'WI6447: check if user accepted the terms and condition
        If Decl_check = "N" Then
            ShowMessage("Please accept the terms and conditions")
            Exit Sub
        End If
        'end added by prasun on 03012022

        Try
            Dim SPReqNumber As String = Session("requestnumber").trim
            Dim SPReqLocCode As String = Session("Comp_code").trim
            Dim SPReqType As String = getSPReqType(SPReqNumber)
            Dim MedChkLocation As Integer = 0
            Dim MedChkOFSP As Integer = 0

            MedChkLocation = getMedChkLocation("MEDFITCHKL", SPReqLocCode, SPReqType, "Y")

            Dim locCheck = CheckWireFrameLoc()
            Dim dtReq_Category As Boolean = False
            dtReq_Category = ChecReqCategory(SPReqNumber, vSPNo)

            If MedChkLocation = 1 Then
                If locCheck = True And Session("requestType") = "SPN" And drptypeassessment.SelectedValue.Trim = "D" And dtReq_Category = True Then
                Else
                    Dim MedicalExists As Integer
                    MedicalExists = checkMedicalExists(vSPNo, SPReqNumber, SPReqLocCode, "N")
                    If MedicalExists = 1 Then
                        MedChkOFSP = getMedChkOFSPFIT(vSPNo, SPReqNumber, SPReqLocCode, "N", "FIT")

                        If MedChkOFSP = 0 Then

                            ShowMessage("*You cannot save/update skills certification details for safety pass number " + vSPNo + "  as his /her medical test result is fail. Please arrange to get the medical test done again.")
                            Exit Sub
                        End If
                    Else
                        ShowMessage("*You cannot save/update skills certification details for safety pass number " + vSPNo + "  as his/ her medical examination result is pending. Please wait for result or try after some time. You can also check the status from portal.")
                        Exit Sub
                    End If
                End If
            End If

        Catch ex As Exception
        End Try

        If drptypeassessment.SelectedValue = "0" And chk_waive.Checked = False And chk_waive.Visible = True Then
            ShowMessage("Please select skill assessment type")
            Exit Sub
        End If
        If chk_waive.Checked = True And chk_waive.Visible = True Then
            If drp_waiveoff.SelectedValue = "0" Then
                ShowMessage("Please choose skill waiver off reason")
                Exit Sub
            End If
        Else
            ' drp_waiveoff.SelectedValue = "0"
        End If
        'START ADD BY PRASUN CHAKRABORTY 24122021 'WI6447 waive off days validation
        Dim waive_days As Integer = 0
        If chk_waive.Checked = True And chk_waive.Visible = True Then
            If drp_waiveoff.SelectedValue <> "0" Then

                Dim strLoginLocDtls As String
                strLoginLocDtls = "  select distinct ACM_COMPANY_CODE || ' - ' || ACM_REMARKS as Loccode from HRACE.t_cwm_action_mapping where ACM_TYPE = 'WAVLOC' and ACM_FLAG = 'Y' AND ACM_COMPANY_CODE in ('" + Session("Comp_code") + "') order by Loccode"

                Dim dtListofLocations As DataTable

                dtListofLocations = getRecord(strLoginLocDtls, con)
                If dtListofLocations.Rows.Count > 0 Then
                    Dim strWaveOfDays As String = ""
                    Dim sqlWaveOfDays As String
                    sqlWaveOfDays = "  select CTM_SEQ from hrace.t_cemp_type_master t1 where t1.ctm_type='SKW' and t1.CTM_TYPE_DESC = '" + drp_waiveoff.SelectedValue + "' and substr(t1.CTM_TYPE_CODE,5,4) = '" + Session("Comp_code") + "'"

                    Dim dtWaveOfDays As DataTable

                    dtWaveOfDays = getRecord(sqlWaveOfDays, con)
                    If dtWaveOfDays.Rows.Count > 0 Then
                        strWaveOfDays = dtWaveOfDays.Rows(0).Item("CTM_SEQ").ToString
                    Else
                        strWaveOfDays = "365"
                    End If

                    If txt_WAIVE_DAYS.Text.Trim().Length = 0 Then
                        ShowMessage("Please provide waiver off days")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) <= 0 Then
                        ShowMessage("Waiver off days should be greater than 0")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) > strWaveOfDays Then
                        ShowMessage("Waiver off days should not be greater than " + strWaveOfDays + "")
                        Exit Sub
                    Else
                        waive_days = CType(txt_WAIVE_DAYS.Text.Trim(), Integer)
                    End If
                Else

                    If txt_WAIVE_DAYS.Text.Trim().Length = 0 Then
                        ShowMessage("Please provide waiver off days")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) <= 0 Then
                        ShowMessage("Waiver off days should be greater than 0")
                        Exit Sub
                    ElseIf CType(txt_WAIVE_DAYS.Text.Trim(), Integer) > 365 Then
                        ShowMessage("Waiver off days should not be greater than 365")
                        Exit Sub
                    Else
                        waive_days = CType(txt_WAIVE_DAYS.Text.Trim(), Integer)
                    End If
                End If
            End If
        End If
        'END  ADD BY PRASUN CHAKRABORTY 24122021
        If drptypeassessment.Enabled = True And drptypeassessment.SelectedValue = "0" And chk_waive.Checked = False And chk_waive.Visible = True Then
            ShowMessage("Please provide skill assessment type")
            Exit Sub
        Else
            If drptypeassessment.SelectedValue <> "0" And chk_waive.Checked = False And chk_waive.Visible = True Then
            Else

                drptypeassessment.SelectedValue = "0"
            End If

        End If
        If (ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) = "SKTD0029") Then
            ShowMessage("trade as other is now obsolate.Please do not select trade as other")
            btnUpdateSkill.Enabled = False
            Exit Sub
        Else
            btnUpdateSkill.Enabled = True
        End If


        '''''''''''''''''''''enable update operation if assessment result is return'''''''''''
        ls_sql = "select CCST_SAFETY_PASS_NO from hrace.t_cwm_cemp_skill_tmp where CCST_REQ_NO=:CCST_REQ_NO and CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_ASSESSMENT_DATE is null and CCST_ASSESSMENT_RESULT='RET'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmdassess = New OracleCommand(ls_sql, con)
        cmdassess.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
        cmdassess.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", TxtSpno.Text.Trim))
        dtassess = getRecord(cmdassess, con)
        If dtassess.Rows.Count > 0 Then
            status = "Y"
        End If
        ''''''''''''''''''''''''disable update operation if assessement date entered''''''''''''''''''
        ls_sql = "select CCST_ASSESSMENT_DATE from hrace.t_cwm_cemp_skill_tmp where CCST_REQ_NO=:CCST_REQ_NO and CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and ((CCST_ASSESSMENT_RESULT ='PASS')) "
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmdassess = New OracleCommand(ls_sql, con)
        cmdassess.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
        cmdassess.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", TxtSpno.Text.Trim))
        dtassess.Clear()
        dtassess = getRecord(cmdassess, con)
        If dtassess.Rows.Count > 0 Then
            ls_sql = "select CCST_ASSESSMENT_DATE from hrace.t_cwm_cemp_skill_tmp where CCST_REQ_NO=:CCST_REQ_NO and CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and ((CCST_ASSESSMENT_RESULT ='PASS') and (CCST_CERT_NO='0' or CCST_CERT_NO is null )) "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmdassess = New OracleCommand(ls_sql, con)
            cmdassess.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
            cmdassess.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", TxtSpno.Text.Trim))
            dtassess.Clear()
            dtassess = getRecord(cmdassess, con)
            If dtassess.Rows.Count > 0 Then
                statusupdate = "Y"
            Else
                statusupdate = "N"
            End If

        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ls_sql = "select to_char(CCST_ASSESSMENT_DATE,'dd/mm/yyyy') ""assessdate"" from hrace.t_cwm_cemp_skill_tmp where CCST_REQ_NO=:CCST_REQ_NO and CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_ASSESSMENT_DATE is not null and CCST_SKTP_CP_CD<>'NA' and CCST_ASSESSMENT_RESULT in('FAIL','ABS')"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmdassess = New OracleCommand(ls_sql, con)
        cmdassess.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
        cmdassess.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", TxtSpno.Text.Trim))
        dtassess.Clear()
        dtassess = getRecord(cmdassess, con)
        If dtassess.Rows.Count > 0 Then
            Dim datesource As String = dtassess.Rows(0).Item("assessdate").ToString
            Dim dtdiff As New DataTable
            ls_sql = "select trunc(sysdate) - to_date(:dtassess,'dd/mm/yyyy') datedifference from dual "
            cmdassess = New OracleCommand(ls_sql, con)
            cmdassess.Parameters.Add(New OracleParameter(":dtassess", datesource))
            dtdiff = getRecord(cmdassess, con)
            If dtdiff.Rows.Count > 0 Then
                Dim datecntdiff As Integer = Convert.ToInt64(dtdiff.Rows(0).Item("datedifference").ToString)
                If datecntdiff > 15 Then
                Else
                    ShowMessage("Please re apply for skill assessment after 15 days")
                    Exit Sub

                End If
            End If
        End If
        vSkilledTrades = ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-"))

        vErrorCount = CheckSkillMandatoryFields()
        If vErrorCount > 0 Then
            tblSkillErrorLst.Visible = True
            ' mpAddSkill.Show()
            Exit Sub
        Else
            tblSkillErrorLst.Visible = False
        End If
        If FileUploadSkill.HasFile = True Then


            Dim contentType As String = FileUploadSkill.PostedFile.ContentType
            Dim type As String = contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf")
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (FileUploadSkill.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size Is " + (FileUploadSkill.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        Else
            If (vSkilledTrades = "SKTD0028" Or vSkilledTrades = "SKTD0029") Then
                'ShowMessage("Please Upload File")
                'Exit Sub
            Else
                If Session("reqtype") <> "Renew" Then

                    'ShowMessage("SNTI given Skill Certificate is mandatory. So, please attach.")
                    Dim chkskillforEP As String = "N"
                    Dim ls_sqlep As String = String.Empty
                    Dim dtep As New DataTable
                    ls_sqlep = "select ACM_COMPANY_CODE,ACM_CATEGORY from t_cwm_action_mapping where ACM_TYPE='SKE' and ACM_FLAG='Y' and ACM_COMPANY_CODE='" + comp_cd + "'"
                    dtep = getRecord(ls_sqlep, con)
                    If dtep.Rows.Count > 0 Then
                        If dtep.Rows(0).Item("ACM_COMPANY_CODE") And (dtep.Rows(0).Item("ACM_CATEGORY") = Txtdeprt.Text.Trim.ToString) Then
                            chkskillforEP = "Y"

                        End If
                        If dtep.Rows(0).Item("ACM_COMPANY_CODE") Or dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1003" Then
                            chkskillforEP = "Y"
                        Else
                            chkskillforEP = "N"
                        End If

                    End If
                    If chkskillforEP = "Y" Then

                    Else
                        Dim cmd As OracleCommand
                        Dim dt As New DataTable
                        ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where acm_type='SKJNTVTI' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
                        cmd = New OracleCommand(ls_sql, con)
                        cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
                        dt = getRecord(cmd, con)
                        If dt.Rows.Count > 0 Then
                        Else
                            ShowMessage("TSL given skill certificate for the selected Trade is mandatory.")
                            Exit Sub
                        End If

                    End If


                Else
                    Dim ls_sqlsk As String = String.Empty
                    Dim cmd_sk As OracleCommand
                    Dim dt_sk As New DataTable
                    Try
                        ls_sqlsk = "select ACM_CATEGORY from hrace.T_CWM_ACTION_MAPPING  where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_TYPE='SKC' and ACM_FLAG='Y' and ACM_CATEGORY='SKC'"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmd_sk = New OracleCommand(ls_sqlsk, con)
                        cmd_sk.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))

                        dt_sk = getRecord(cmd_sk, con)
                        If dt_sk.Rows.Count > 0 Then
                            ls_sqlsk = "select ACM_TYPE from hrace.t_cwm_action_mapping where ACM_CATEGORY=:ACM_CATEGORY and ACM_FLAG='N' and ACM_TYPE='SKC'"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            cmd_sk = New OracleCommand(ls_sqlsk, con)
                            cmd_sk.Parameters.Add(New OracleParameter(":ACM_CATEGORY", vVencode))
                            dt_sk.Clear()
                            dt_sk = getRecord(cmd_sk, con)
                            If dt_sk.Rows.Count > 0 Then
                                vendorst = "Y"
                            Else
                                If vendorst = "N" Then
                                    '''''''''''''checking for department''''''''
                                    ls_sqlsk = "select CET_DEPT_CODE from hrace.t_cemp_details_tmp where CET_REQUEST_NO=:CET_REQUEST_NO and CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO"
                                    If con.State = ConnectionState.Closed Then
                                        con.Open()
                                    End If
                                    cmd_sk = New OracleCommand(ls_sqlsk, con)
                                    cmd_sk.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
                                    cmd_sk.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
                                    dt_sk.Clear()
                                    dt_sk = getRecord(cmd_sk, con)
                                    Dim dept As String = String.Empty
                                    If dt_sk.Rows.Count > 0 Then
                                        dept = dt_sk.Rows(0).Item("CET_DEPT_CODE")
                                        '''''''''checking for department exist''''''''''''
                                        ls_sqlsk = "Select ACM_TYPE from hrace.t_cwm_action_mapping where ACM_CATEGORY=:ACM_CATEGORY and ACM_FLAG='N' and ACM_TYPE='SKC'"
                                        If con.State = ConnectionState.Closed Then
                                            con.Open()
                                        End If
                                        cmd_sk = New OracleCommand(ls_sqlsk, con)
                                        cmd_sk.Parameters.Add(New OracleParameter(":ACM_CATEGORY", dept))
                                        dt_sk.Clear()
                                        dt_sk = getRecord(cmd_sk, con)
                                        If dt_sk.Rows.Count > 0 Then
                                        Else
                                            Dim chkskillforEP As String = "N"
                                            Dim ls_sqlep As String = String.Empty
                                            Dim dtep As New DataTable
                                            '''WI2259: allow company code checking for skill exemption for E&P and TGS, created By: Avik Mukherjee, Created on: 02-Aug-2021
                                            ls_sqlep = "select ACM_COMPANY_CODE,ACM_CATEGORY from t_cwm_action_mapping where ACM_TYPE='SKE' and ACM_FLAG='Y' and ACM_COMPANY_CODE='" + comp_cd + "'"
                                            dtep = getRecord(ls_sqlep, con)
                                            If dtep.Rows.Count > 0 Then
                                                If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1000" And (dtep.Rows(0).Item("ACM_CATEGORY") = Txtdeprt.Text.Trim.ToString) Then
                                                    chkskillforEP = "Y"

                                                End If
                                                If dtep.Rows(0).Item("ACM_COMPANY_CODE") = "3000" Or dtep.Rows(0).Item("ACM_COMPANY_CODE") = "1003" Then
                                                    chkskillforEP = "Y"

                                                End If
                                                If chkskillforEP = "Y" Then
                                                Else
                                                    Dim cmd As OracleCommand
                                                    Dim dt As New DataTable
                                                    ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where acm_type='SKJNTVTI' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
                                                    cmd = New OracleCommand(ls_sql, con)
                                                    cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
                                                    dt = getRecord(cmd, con)
                                                    If dt.Rows.Count > 0 Then
                                                    Else
                                                        ShowMessage("TSL given skill certificate for the selected Trade is mandatory.")
                                                        Exit Sub
                                                    End If

                                                End If
                                            End If

                                        End If


                                    End If
                                    vendorst = "N"
                                End If
                                'ShowMessage("TSL given skill certificate for the selected Trade is mandatory.")
                                'Exit Sub
                            End If
                        End If

                    Catch ex As Exception

                    End Try

                End If
            End If
        End If

        Dim flagdata As String = "N"

        Try
            'flagdata = checkExistingData(TxtSpno.Text.ToString.Trim().ToUpper, cmbSkSkillType.SelectedValue, cmbSkSkill.SelectedValue, vCompCD)

            Dim cmdupdate As New OracleCommand
            Dim dt As New DataTable()
            If hidcertnoskill.Value = "0" Or hidcertnoskill.Value = "" Then
                If FileUploadSkill.HasFile = True Then
                    certskill = TrnCWESKILLSeqNo("")
                Else
                    certskill = "0"
                End If
            Else
                certskill = hidcertnoskill.Value
            End If

            Dim vOtherSkilledTrades As String = ""
            Dim vskillassessment As String = String.Empty
            If (vSkilledTrades = "SKTD0029") Then
                vOtherSkilledTrades = txtOthSkillTrade.Text.ToString.Trim.Replace("'", "''")
                vskillassessment = "NA"
            ElseIf (vSkilledTrades = "SKTD0028") Then

                vskillassessment = drp_skillassess.SelectedValue
                If vskillassessment = "" Or vskillassessment = "NA" Then
                    ShowMessage("Please select skill set for assessment")
                    Exit Sub
                End If

            Else
                vOtherSkilledTrades = "NA"
                vskillassessment = "NA"
            End If
            If chk_waive.Checked And chk_waive.Visible = True Then
                waivetag = "Y"
                waivetagreason = drp_waiveoff.SelectedValue
            Else
                waivetag = "N"
                waivetagreason = String.Empty
            End If
            If waivetag = "N" And drptypeassessment.SelectedValue = "0" And drptypeassessment.Enabled = True Then
                ShowMessage("Some issue occurs please try after sometimes")
                Exit Sub
            End If
            If waivetag = "Y" And drptypeassessment.SelectedValue <> "0" And drptypeassessment.Enabled = True Then
                ShowMessage("Some issue occurs please refresh your application")
                Exit Sub
            End If
            If statusupdate = "N" Then

            End If
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            If FileUploadSkill.Enabled = False Then
                certskill = "0"
            End If

            '--------------------------------------souvik begins 2

            pop_comp_cd_stp()

            'Try
            '    If Session("Comp_code").ToString().Trim() = Session("Comp_cd_stop_same_trade").ToString().Trim() And drptypeassessment.SelectedValue = "0" Then
            '        ShowMessage("Please Select Assessment Type")
            '        Exit Sub
            '    End If
            'Catch ex As Exception

            'End Try
            '--------------------------------------souvik ends 2

            Dim SPRSkCertReqd As String = ""
            If (Session("renewSkillCert") = "Yes") Then
                SPRSkCertReqd = "Y"
            Else
                SPRSkCertReqd = "N"
            End If

            If statusupdate = "N" Or statusupdate = "" Then
                'sqlUpdSkill = "Update T_CWM_CEMP_SKILL_TMP set CCST_REMARKS=:CCST_REMARKS,CCST_MODIFIED_BY=:CCST_MODIFIED_BY,CCST_MODIFIED_DT=sysdate,CCST_CREATED_DT=sysdate,ccst_loc_code=:ccst_loc_code,CCST_SKILL_TYPE_CD=:CCST_SKILL_TYPE_CD,CCST_SKILL_CD=:CCST_SKILL_CD,CCST_CERT_NO=:CCST_CERT_NO, CCST_SKTD_CP_CD=:CCST_SKTD_CP_CD,CCST_SKTP_CP_CD=:CCST_SKTP_CP_CD,CCST_SKTD_OTH_REMRK=:CCST_SKTD_OTH_REMRK,CCST_ASSESSMENT_RESULT=NULL,CCST_ASSESSMENT_DATE=NULL,CCST_ASSESSMENT_VALIDITY=NULL,CCST_ASSESSMENT_TYPE=:CCST_ASSESSMENT_TYPE,CCST_WAIVE_OFF=:CCST_WAIVE_OFF,CCST_WAIVE_OFF_RESN=:CCST_WAIVE_OFF_RESN,CCST_REMARKS_PD=null where CCST_safety_pass_no=:CCST_safety_pass_no  and CCST_COMP_CODE=:CCST_COMP_CODE and CCST_REQ_NO=:CCST_REQ_NO"
                'below EDIT BY PRASUN CHAKRABORTY 24122021 'WI6447

                Dim req_sql As String = String.Empty
                Dim dtareq As New DataTable
                Dim cmdReq As New OracleCommand
                req_sql = "select CCST_safety_pass_no,CCST_REQ_NO,CCST_REQ_FLAG from hrace.t_cwm_cemp_skill_tmp where CCST_safety_pass_no=:CCST_safety_pass_no  and CCST_COMP_CODE=:CCST_COMP_CODE and CCST_REQ_NO=:CCST_REQ_NO and CCST_REQ_FLAG = 'R'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmdReq = New OracleCommand(req_sql, con)
                cmdReq.Parameters.Add(New OracleParameter(":CCST_safety_pass_no", TxtSpno.Text.ToString.Trim().ToUpper))
                cmdReq.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_code")))
                cmdReq.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
                dtareq.Clear()
                dtareq = getRecord(cmdReq, con)
                If dtareq.Rows.Count > 0 Then
                    sqlUpdSkill = "Update T_CWM_CEMP_SKILL_TMP set CCST_REMARKS=:CCST_REMARKS,CCST_MODIFIED_BY=:CCST_MODIFIED_BY,CCST_MODIFIED_DT=sysdate,CCST_CREATED_DT=sysdate,ccst_loc_code=:ccst_loc_code,CCST_SKILL_TYPE_CD=:CCST_SKILL_TYPE_CD,CCST_SKILL_CD=:CCST_SKILL_CD,CCST_CERT_NO=:CCST_CERT_NO, CCST_SKTD_CP_CD=:CCST_SKTD_CP_CD,CCST_SKTP_CP_CD=:CCST_SKTP_CP_CD,CCST_SKTD_OTH_REMRK=:CCST_SKTD_OTH_REMRK,CCST_ASSESSMENT_RESULT=NULL,CCST_ASSESSMENT_DATE=NULL,CCST_ASSESSMENT_VALIDITY=NULL,CCST_ASSESSMENT_TYPE=:CCST_ASSESSMENT_TYPE,CCST_WAIVE_OFF=:CCST_WAIVE_OFF,CCST_WAIVE_OFF_RESN=:CCST_WAIVE_OFF_RESN,CCST_REMARKS_PD=null,ccst_req_flag=null, CCST_WAIVE_DAYS=:CCST_WAIVE_DAYS, CCST_DECL_CHECK = :CCST_DECL_CHECK, CCST_SKILL_ATT = :CCST_SKILL_ATT where CCST_safety_pass_no=:CCST_safety_pass_no  and CCST_COMP_CODE=:CCST_COMP_CODE and CCST_REQ_NO=:CCST_REQ_NO"
                Else
                    sqlUpdSkill = "Update T_CWM_CEMP_SKILL_TMP set CCST_REMARKS=:CCST_REMARKS,CCST_MODIFIED_BY=:CCST_MODIFIED_BY,CCST_MODIFIED_DT=sysdate,CCST_CREATED_DT=sysdate,ccst_loc_code=:ccst_loc_code,CCST_SKILL_TYPE_CD=:CCST_SKILL_TYPE_CD,CCST_SKILL_CD=:CCST_SKILL_CD,CCST_CERT_NO=:CCST_CERT_NO, CCST_SKTD_CP_CD=:CCST_SKTD_CP_CD,CCST_SKTP_CP_CD=:CCST_SKTP_CP_CD,CCST_SKTD_OTH_REMRK=:CCST_SKTD_OTH_REMRK,CCST_ASSESSMENT_RESULT=NULL,CCST_ASSESSMENT_DATE=NULL,CCST_ASSESSMENT_VALIDITY=NULL,CCST_ASSESSMENT_TYPE=:CCST_ASSESSMENT_TYPE,CCST_WAIVE_OFF=:CCST_WAIVE_OFF,CCST_WAIVE_OFF_RESN=:CCST_WAIVE_OFF_RESN,CCST_REMARKS_PD=null, CCST_WAIVE_DAYS=:CCST_WAIVE_DAYS, CCST_DECL_CHECK = :CCST_DECL_CHECK, CCST_SKILL_ATT = :CCST_SKILL_ATT where CCST_safety_pass_no=:CCST_safety_pass_no  and CCST_COMP_CODE=:CCST_COMP_CODE and CCST_REQ_NO=:CCST_REQ_NO"
                End If

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmdupdate.Connection = con
                cmdupdate.CommandText = sqlUpdSkill
                If status = "Y" Then
                    cmdupdate.Parameters.Add(New OracleParameter(":CCST_REMARKS", " "))
                Else
                    cmdupdate.Parameters.Add(New OracleParameter(":CCST_REMARKS", txtSkRemarks.Text.ToString.Trim.Replace("'", "''")))
                End If
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_MODIFIED_BY", Session("VendCode")))
                cmdupdate.Parameters.Add(New OracleParameter(":ccst_loc_code", ddlSKAss.SelectedValue))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_safety_pass_no", TxtSpno.Text.ToString.Trim().ToUpper))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_SKILL_TYPE_CD", cmbSkSkillType.SelectedValue))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_SKILL_CD", cmbSkSkill.SelectedValue))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_code")))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_CERT_NO", certskill))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_SKTD_CP_CD", ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-"))))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_SKTP_CP_CD", vskillassessment))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_SKTD_OTH_REMRK", vOtherSkilledTrades))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_ASSESSMENT_TYPE", drptypeassessment.SelectedValue.Trim))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_WAIVE_OFF", waivetag.Trim))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_WAIVE_OFF_RESN", waivetagreason.Trim))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_WAIVE_DAYS", IIf(waive_days > 0, waive_days, DBNull.Value))) 'ADD BY PRASUN CHAKRABORTY 24122021 'WI6447
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_DECL_CHECK", Decl_check)) 'ADD BY PRASUN CHAKRABORTY 24122021 'WI6447
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_SKILL_ATT", SPRSkCertReqd))
            ElseIf statusupdate = "Y" Then
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                sqlUpdSkill = "Update T_CWM_CEMP_SKILL_TMP set CCST_MODIFIED_BY=:CCST_MODIFIED_BY,CCST_MODIFIED_DT=sysdate,CCST_CERT_NO=:CCST_CERT_NO, CCST_DECL_CHECK = :CCST_DECL_CHECK where CCST_safety_pass_no=:CCST_safety_pass_no  and CCST_COMP_CODE=:CCST_COMP_CODE and CCST_REQ_NO=:CCST_REQ_NO"
                cmdupdate.Connection = con
                cmdupdate.CommandText = sqlUpdSkill
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_MODIFIED_BY", Session("VendCode")))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_safety_pass_no", TxtSpno.Text.ToString.Trim().ToUpper))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_code")))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_REQ_NO", Session("requestnumber")))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_CERT_NO", certskill))
                cmdupdate.Parameters.Add(New OracleParameter(":CCST_DECL_CHECK", Decl_check)) 'ADD BY PRASUN CHAKRABORTY 24122021 'WI6447
            End If

            cmdupdate.ExecuteNonQuery()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim cmdupdate1 As New OracleCommand
            If FileUploadSkill.Enabled = False Then
                sqlUpdSkill = "update hrace.T_cemp_details_tmp set CET_PROFILE_STATUS='I' where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
                cmdupdate1.Connection = con
                cmdupdate1.CommandText = sqlUpdSkill
                cmdupdate1.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
                cmdupdate1.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
                cmdupdate1.ExecuteNonQuery()

            End If



            '''''''''''''''''''''''''''''update file attachment for skill  ''''''''''''''
            If FileUploadSkill.HasFile = True Then

                Dim filename As String = Path.GetFileName(FileUploadSkill.PostedFile.FileName)
                Dim contentType As String = FileUploadSkill.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (FileUploadSkill.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (FileUploadSkill.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
                'Dim ls_sql As String = String.Empty
                Dim cmdfileskill As New OracleCommand
                Using fs As Stream = FileUploadSkill.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                        filename = Path.GetFileName(FileUploadSkill.PostedFile.FileName)
                        If hidcertnoskill.Value.Trim = "" Or hidcertnoskill.Value.Trim = "0" Then
                            'hidcertnoskill.Value = TrnCWESKILLSeqNo("")
                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If

                            cmdfileskill = New OracleCommand(ls_sql, con)
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_DOC_ID", certskill))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "SKILL"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileskill.ExecuteNonQuery()
                        Else
                            ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileskill = New OracleCommand(ls_sql, con)
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_DOC_ID", certskill))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileskill.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If

                        End If

                    End Using
                End Using
            End If





            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            ''''''''''''check reject case or not''''''
            Dim ls_chkskill As String = String.Empty
            Dim cmd_chkskilll As OracleCommand
            Dim dt_chkskill As New DataTable
            Try
                ls_chkskill = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='SK' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkskilll = New OracleCommand(ls_chkskill, con)
                cmd_chkskilll.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkskilll.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", vSPNo))
                dt_chkskill = getRecord(cmd_chkskilll, con)
                If dt_chkskill.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), vSPNo, "SK")
                End If
            Catch ex As Exception

            End Try
            ''''''''''''''''''''''''''''''''''''''''''

            hidcertnoskill.Value = "0"
            'btnSearch_Click(sender, e)
            getskill(vSPNo)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvSkill.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectSkill")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If
            clearSkill()
            ShowMessage("Skill has been updated successfully")
            btnUpdateSkill.Enabled = True
            btnUpdateSkill.Visible = True
            btnSaveSkill.Visible = False
            sendmailtoagencyskill(vSPNo)
        Catch ex As Exception
            ShowMessage(ex.Message)
        End Try

    End Sub
    Protected Sub chkSelectNominee(ByVal sender As Object, ByVal e As System.EventArgs)


        Dim vIsRowSelected As Boolean = False
        clearNominee()


        Dim gvrow As GridViewRow
        gvrow = CType(sender, CheckBox).Parent.Parent
        If CType(gvrow.FindControl("chkSelectNominee"), CheckBox).Checked = True Then
            vIsRowSelected = True
            Dim vNomineeID As String = CType(gvrow.FindControl("hidNomineeID"), HiddenField).Value
            Session("NomineeID") = vNomineeID

            Dim vRelation As String = CType(gvrow.FindControl("hidRelationCd"), HiddenField).Value
            Dim vNomName As String = gvrow.Cells(2).Text.Trim()
            Dim vNomDOB As String = gvrow.Cells(3).Text
            Dim vNomPayGrp As String = CType(gvrow.FindControl("hidPayGrpCD"), HiddenField).Value
            Dim vNomShare As String = gvrow.Cells(5).Text
            Dim vNomRemarks As String = gvrow.Cells(6).Text.Trim().Replace("&nbsp;", "")
            Dim vNomAddress As String = gvrow.Cells(7).Text.Trim().Replace("&nbsp;", "")

            cmbNomRelation.SelectedValue = vRelation
            txtNomName.Text = vNomName
            txtNomDOB.Text = vNomDOB
            cmbNomPayGrp.SelectedValue = vNomPayGrp
            cmbNomShare.SelectedValue = vNomShare
            txtNomRemarks.Text = vNomRemarks
            txtNomineeAddress.Text = vNomAddress

            Dim status As String = checkrenewaleligible(TxtSpno.Text.Trim, Session("requestnumber"))
            If status.Equals("Y") Then
                btnUpdateNominee.Visible = False
                btnUpdateNominee.Enabled = False
            Else
                btnUpdateNominee.Visible = True
                btnUpdateNominee.Enabled = True
            End If


        End If




    End Sub

#End Region

#Region "Profile"
    Public Sub FillDropDown(ByVal cmbObject As DropDownList, ByVal vCode As String, Optional ByVal vMultipleCD As String = "N")



        Dim sql As String = ""
        Dim vTempCode As String = ""
        If vMultipleCD = "N" Then
            sql = clmClass.get_CodeValue(vCode)
        Else
            Dim arrCode = vCode.Split(",")
            For i = 0 To arrCode.Length - 1
                vTempCode = vTempCode & "'" & arrCode(i) & "',"
            Next
            vTempCode = vTempCode.Substring(0, Len(vTempCode) - 1)
            sql = clmClass.get_CodeValue(vTempCode)
        End If


        Dim dt As New DataTable()
        dt = getRecord(sql, con)
        cmbObject.Items.Clear()
        If dt.Rows.Count > 0 Then
            cmbObject.DataSource = dt

            cmbObject.DataTextField = "CTM_TYPE_DESC"
            cmbObject.DataValueField = "CTM_TYPE_CODE"
            cmbObject.DataBind()
            cmbObject.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Public Function GetLocationName(ByVal vCompCD As String) As String
        Dim vLocCD As String = ""
        Dim dtLoc As New DataTable()

        Dim sqlLocation As String = "select CMP_COMPANY_CODE,CMP_LOC_CD  from T_COMPANY_MASTER    where CMP_COMPANY_CODE='" + vCompCD + "'"
        dtLoc = getRecord(sqlLocation, con)
        If dtLoc.Rows.Count > 0 Then
            vLocCD = dtLoc.Rows(0)("CMP_LOC_CD")
        End If
        Return vLocCD
    End Function
    Public Sub GetAreaOfWork(ByVal cmbObject As DropDownList, ByVal vCode As String, Optional ByVal vMultipleCD As String = "N")
        Dim sql As String = ""
        Dim vTempCode As String = ""
        If vMultipleCD = "N" Then
            sql = clmClass.get_CodeValue(vCode)
        Else
            Dim arrCode = vCode.Split(",")
            For i = 0 To arrCode.Length - 1
                vTempCode = vTempCode & "'" & arrCode(i) & "',"
            Next
            vTempCode = vTempCode.Substring(0, Len(vTempCode) - 1)
            sql = clmClass.get_CodeValue(vCode)
        End If


        Dim dt As New DataTable()
        dt = getRecord(sql, con)
        cmbObject.Items.Clear()
        If dt.Rows.Count > 0 Then
            cmbObject.DataSource = dt

            cmbObject.DataTextField = "CTM_TYPE_DESC"
            cmbObject.DataValueField = "CTM_TYPE_DESC"
            cmbObject.DataBind()
            cmbObject.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Public Function t_Cemp_Type_Master() As String
        Dim sql As String = "select * from t_Cemp_Type_Master "
        Return sql
    End Function
    Public Sub GetCategory(ByVal DESCRIPTION As String)
        'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
        'Dim sqlCategory As String = t_Cemp_Type_Master() + " where CTM_STATUS ='A' AND CTM_TYPE_DESC='" + DESCRIPTION + "' and CTM_TYPE='SPET' "
        Dim typeDescParamCount As String() = DESCRIPTION.Split(",")
        Dim sqlCategory As String = t_Cemp_Type_Master() + " where CTM_STATUS ='A'   "
        If typeDescParamCount.Count > 1 Then
            sqlCategory = sqlCategory + " AND CTM_TYPE_DESC IN(" + DESCRIPTION + ")"
        Else
            sqlCategory = sqlCategory + " AND CTM_TYPE_DESC='" + DESCRIPTION + "'"
        End If
        sqlCategory = sqlCategory + " and CTM_TYPE='SPET' AND SUBSTR(CTM_TYPE_CODE,-4,4)='" + comp_cd + "'"
        Dim dtCategory As New DataTable()
        dtCategory = getRecord(sqlCategory, con)
        cmbCategory.Items.Clear()
        If dtCategory.Rows.Count > 0 Then
            cmbCategory.DataSource = dtCategory
            cmbCategory.DataTextField = "CTM_TYPE_DESC"
            cmbCategory.DataValueField = "CTM_VALUE"
            cmbCategory.DataBind()
        End If
    End Sub
    Public Function CheckProfileMandatoryFields() As Integer
        Dim vErrorCount As Integer = 0
        Dim vDOB As Date
        Dim vTodayDate As Date
        vTodayDate = DateTime.ParseExact(Date.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "dd/MM/yyyy", CultureInfo.InvariantCulture)

        If txtFName.Text = "" Then
            ErrorRow(tblProfileErrorList, "Enter First Name")

        End If

        If txtFatherName.Text = "" And txtHusName.Text.Trim = "" Then
            ErrorRow(tblProfileErrorList, "Enter Father/Husband Name")
        End If

        If cmbSex.SelectedValue = "0" Then
            ErrorRow(tblProfileErrorList, "Select Gender")
        End If

        If txtDOB.Text.Trim = "" Then
            ErrorRow(tblProfileErrorList, "Enter Date of birth")
        Else
            Try
                Dim db1 As String = txtDOB.Text.Replace("-", "/")
                vDOB = DateTime.ParseExact(db1.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
            Catch ex As Exception
                ErrorRow(tblProfileErrorList, "Enter a valid Date of Birth in DD/MM/YYYY Format")
            End Try
        End If


        If txtPhNo.Text = "" Then
            ErrorRow(tblProfileErrorList, "Enter personal Phone Number")
        End If
        If txtPhNo.Text.Trim <> "" And txtPhNo.Text.Length < 10 Then
            ErrorRow(tblProfileErrorList, "Enter a valid mobile Number")
        End If

        If txtEmrgNo.Text = "" Then
            ErrorRow(tblProfileErrorList, "Enter Emergency Number")
        End If
        If txtEmrgNo.Text.Trim <> "" And txtEmrgNo.Text.Length < 10 Then
            ErrorRow(tblProfileErrorList, "Enter a valid Emergency Number")
        End If


        If cmbWorkArea.SelectedValue = "0" Then
            ErrorRow(tblProfileErrorList, "Select Work Area")
        End If

        If ddlMedCentre.SelectedValue = "0" Then
            ErrorRow(tblProfileErrorList, "Select Medical Centre")
        End If



        If cmbUniqID.SelectedValue = "0" Then
            ErrorRow(tblProfileErrorList, "Select Unique Identity Type")
        End If


        If cmbCategory.SelectedValue = "0" Then
            ErrorRow(tblProfileErrorList, "Select Catgory")
        End If

        If txtUniqIDNo.Text = "" Then
            ErrorRow(tblProfileErrorList, "Enter Unique Identity Number")
        End If

        If txtIdentiFication.Text = "" Then
            ErrorRow(tblProfileErrorList, "Enter Identification Mark")
        End If

        If cmbAffirmative.SelectedValue = "0" Then
            ErrorRow(tblProfileErrorList, "Select Affirmative")
        End If



        If vDOB > vTodayDate.AddYears(-18) Then
            ErrorRow(tblProfileErrorList, "As per the Law, people below 18 years of age are not allowed to work in " & Session("comp_name_d") & " .")
        End If

        vErrorCount = err_cnt
        Return vErrorCount
    End Function
    Public Sub getUniqueID()
        Dim sql As String = t_Cemp_Type_Master() + " where CTM_TYPE ='ICAD' and CTM_STATUS='A' and CTM_TYPE_CODE='DRV'"
        Dim dt As DataTable
        dt = getRecord(sql, con)
        If dt.Rows.Count > 0 Then
            cmbUniqID.Items.Clear()
            '   cmbUniqID.DataSource = dt
            '  cmbUniqID.DataTextField = "CTM_TYPE_DESC"
            ' cmbUniqID.DataValueField = "CTM_VALUE"
            cmbUniqID.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
            cmbUniqID.Items.Insert(1, New WebControls.ListItem(dt.Rows(0).Item("CTM_TYPE_DESC"), dt.Rows(0).Item("CTM_TYPE_CODE")))
        End If
    End Sub
    Protected Sub btnSaveProfile_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveProfile.Click
        If txtDOB.Text <> "" And txtDOB.Text <> "__/__/____" And txtDOB.Text <> "__-__-____" Then
            Dim db As String = txtDOB.Text.Replace("-", "/")
            Dim dob As Date = DateTime.ParseExact(db, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            Dim age As Double = GetAge(dob)
            Dim maxAge As Integer = GetMaxAge()
            If age < 18 Then
                ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " & Session("comp_name_d") & " .")
                Exit Sub
            ElseIf age > maxAge Then
                hfActionPerformed.Value = "S"
                ageMessage.InnerText = "You need to attach department’s chief approval for person above " & maxAge.ToString & " years of age at the time of generating safety pass."
                pnlConfirmDocSubmision.Visible = True
                MPopUpConfirmDocSubmision.Show()
            Else
                Dim ls_sql1 As String = "select trunc(sysdate)-(select trunc(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:SRQ_REQ_NO) from dual"
                Dim timediff As Integer = 0
                Dim cmd1 As New OracleCommand(ls_sql1, con)
                cmd1.Parameters.Add(New OracleParameter(":SRQ_REQ_NO", Session("requestnumber")))

                Dim dt1 As New DataTable
                dt1 = getRecord(cmd1, con)
                If dt1.Rows.Count > 0 Then
                    timediff = Convert.ToUInt64(dt1.Rows(0).Item(0).ToString)
                End If
                If timediff > 10 Then
                    ShowMessage("Your request is too old. You cannot able to proceed")
                    Exit Sub
                Else

                    SaveProfile()
                End If
            End If
        End If
    End Sub

    'procedure below added by souvik
    Protected Sub txtUniqIDNo_valchanged(ByVal sender As Object, ByVal e As System.EventArgs)
        Try
            Try
                If cmbUniqID.SelectedIndex <= 0 Then
                    ShowMessage("Please Select Unique ID Type First")
                    txtUniqIDNo.Text = ""
                    Return
                End If
            Catch ex As Exception
                ShowMessage("Please Select Unique ID Type First")
                txtUniqIDNo.Text = ""
                Return
            End Try

            Dim vSPNO As String = ""
            Dim cat As String = ""
            Dim vCategory As String = ""

            vCategory = cmbCategory.SelectedValue.ToString.ToUpper
            'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            If vCategory = SF Or vCategory = SV Or vCategory = SH Or vCategory = SA Then
                vCategory = SV
            ElseIf vCategory = WR Or vCategory = WA Then
                vCategory = WR
            ElseIf vCategory = DV Or vCategory = DA Or vCategory = DH Then
                vCategory = DV
            ElseIf vCategory = VC Or vCategory = VA Then
                vCategory = VC
            ElseIf vCategory = FM Or vCategory = FA Then
                vCategory = FM
            End If


            Dim dtCatVal As DataTable = clmClass.get_codetype(vCategory, comp_cd)
            If dtCatVal.Rows.Count > 0 Then
                cat = dtCatVal.Rows(0).Item("CTM_VALUE").ToString
            End If


            Dim vMMYY As String = Today.ToString("MMyy")
            Dim vSerialNo As String = GET_SP_no()
            If (Session("Comp_code") = "1000") Then
                vSPNO = "R" + cat + vMMYY + vSerialNo

            Else
                vSPNO = GetSPInitial() + cat + vMMYY + vSerialNo

            End If


            '' Duplicate ID Proof Check
            Dim sqlDuplicateID As String = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CET_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null)"
            Dim dtDuplicateID As New DataTable()
            dtDuplicateID = getRecord(sqlDuplicateID, con)
            If dtDuplicateID.Rows.Count > 0 Then
                ErrorRow(tblProfileErrorList, "This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows(0)("CET_SAFETY_PASSNO") + ",please use this SP No. to raise request")
                tblProfileErrorList.Visible = True
                Exit Sub
            End If
            '''''''''''''''check uniq ID number already exist or not'''''''''

            '2 lines below edited & added by souvik
            'sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim + "'"
            'sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + vSPNO.Trim() + "'"
            sqlDuplicateID = "Select CED_SAFETY_PASS_NO, CED_VENDOR_CODE, CED_COMPANY_CODE from t_cemp_details where CED_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "'"

            dtDuplicateID = getRecord(sqlDuplicateID, con)
            If dtDuplicateID.Rows.Count > 0 Then

                Dim dtVendorInfo As New DataTable()
                Dim msg_add As String = ""
                Dim str_vendor_emails As String = "SELECT nvl(vdt_email1, '') mail1, nvl(vdt_phone1, '') phone1, vdt_vendor_name vendorname FROM HRACE.t_vendor_details WHERE vdt_vendor_code = '" + dtDuplicateID.Rows(0)("CED_VENDOR_CODE").ToString().Trim() + "' AND vdt_company_code = '" + dtDuplicateID.Rows(0)("CED_COMPANY_CODE").ToString().Trim() + "'"
                dtVendorInfo = getRecord(str_vendor_emails, con)
                If dtVendorInfo.Rows.Count > 0 Then
                    If dtVendorInfo.Rows(0)("mail1").ToString().Trim() <> "" Then
                        msg_add = " (" & "Vendor Code: " & dtDuplicateID.Rows(0)("CED_VENDOR_CODE").ToString().Trim() & ", Vendor Name: " & dtVendorInfo.Rows(0)("vendorname").ToString().Trim() & ", Phone: " & dtVendorInfo.Rows(0)("phone1").ToString().Trim() & ", Email: " & dtVendorInfo.Rows(0)("mail1").ToString().Trim() & "). "
                    End If
                End If

                txtUniqIDNo.Text = ""
                ErrorRow(tblProfileErrorList, "This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows(0)("CED_SAFETY_PASS_NO") + ",please use the SP No. to raise request." + msg_add)
                tblProfileErrorList.Visible = True
                Exit Sub
            End If


        Catch ex As Exception

        End Try
    End Sub
    Public Sub setLocationCode(ByVal sp_no As String, ByVal req_no As String)
        Dim cmd As OracleCommand
        location = ""
        Dim sqlLocCd As String = "  select tmp.cet_loc_code from hrace.t_cemp_details_tmp tmp where tmp.cet_safety_passno=:sp_no and tmp.cet_request_no=:req_no"
        cmd = New OracleCommand(sqlLocCd, con)
        cmd.Parameters.Add(New OracleParameter(":sp_no", sp_no))
        cmd.Parameters.Add(New OracleParameter(":req_no", req_no))
        Dim dt As DataTable = getRecord(cmd, con)
        location = dt.Rows(0).Item(0)
    End Sub

    Private Sub SaveProfile()
        Dim sqlProfile As String = ""
        Dim vSPNO As String = ""
        Dim vAgency As String = ""
        Dim vCategory As String = ""
        Dim count As Integer = 0
        Dim vErrorCount As Integer = 0
        Dim agency_code As String = String.Empty
        Dim cmd As OracleCommand
        vErrorCount = CheckProfileMandatoryFields()

        If vErrorCount > 0 Then
            tblProfileErrorList.Visible = True
            Exit Sub
        Else
            tblProfileErrorList.Visible = False
        End If

        vCategory = cmbCategory.SelectedValue.ToString.ToUpper
        'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
        If vCategory = SF Or vCategory = SV Or vCategory = SH Or vCategory = SA Then
            vCategory = SV
        ElseIf vCategory = WR Or vCategory = WA Then
            vCategory = WR
        ElseIf vCategory = DV Or vCategory = DA Or vCategory = DH Then
            vCategory = DV
        ElseIf vCategory = VC Or vCategory = VA Then
            vCategory = VC
        ElseIf vCategory = FM Or vCategory = FA Then
            vCategory = FM
        End If
        Dim cat As String = ""

        Dim dtCatVal As DataTable = clmClass.get_codetype(vCategory, comp_cd)
        If dtCatVal.Rows.Count > 0 Then
            cat = dtCatVal.Rows(0).Item("CTM_VALUE").ToString
        End If

        Dim vMMYY As String = Today.ToString("MMyy")
        Dim vSerialNo As String = GET_SP_no()
        If (Session("Comp_code") = "1000") Then
            vSPNO = "R" + cat + vMMYY + vSerialNo
            Session("vSPNO") = vSPNO
        Else
            vSPNO = GetSPInitial() + cat + vMMYY + vSerialNo
            Session("vSPNO") = vSPNO
        End If


        '' Duplicate ID Proof Check
        Dim sqlDuplicateID As String = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CET_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null)"
        Dim dtDuplicateID As New DataTable()
        dtDuplicateID = getRecord(sqlDuplicateID, con)
        If dtDuplicateID.Rows.Count > 0 Then
            ErrorRow(tblProfileErrorList, "This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows(0)("CET_SAFETY_PASSNO") + ",please use this SP No. to raise request")
            tblProfileErrorList.Visible = True
            Exit Sub
        End If
        '''''''''''''''check uniq ID number already exist or not'''''''''

        sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim + "'"

        dtDuplicateID = getRecord(sqlDuplicateID, con)
        If dtDuplicateID.Rows.Count > 0 Then
            ErrorRow(tblProfileErrorList, "This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows(0)("CED_SAFETY_PASS_NO") + ",please use this SP No. to raise request")
            tblProfileErrorList.Visible = True
            Exit Sub
        End If

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If cmbUniqID.SelectedValue = "ADC" Then
            Dim strchk As String = txtUniqIDNo.Text
            Dim st As Boolean = strchk.Contains(" ")
            If st Then
                ShowMessage("This is not a valid Adhaar number")
                Exit Sub
            End If
            If strchk.Length = 12 Then
            Else
                ShowMessage("This is not a valid Adhaar number")
                Exit Sub
            End If
            Dim numeric As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If (numeric.IsMatch(strchk)) Then
            Else
                ShowMessage("This is not a valid Adhaar number")
                Exit Sub
            End If
        ElseIf cmbUniqID.SelectedValue = "PAN" Then
            Dim strchk As String = txtUniqIDNo.Text
            Dim st As Boolean = strchk.Contains(" ")
            If st Then
                ShowMessage("This is not a valid PAN number")
                Exit Sub
            End If
            If strchk.Length = 10 Then
            Else
                ShowMessage("This is not a valid PAN number")
                Exit Sub
            End If
            Dim alphanumeric As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("[A-Z]{5}\d{4}[A-Z]{1}")
            If (alphanumeric.IsMatch(strchk)) Then
            Else
                ShowMessage("This is not a valid PAN number")
                Exit Sub
            End If
        End If
        If txtPhNo.Text.Trim = "" Then
            Dim ls_sqlphchk As String = String.Empty
            Dim cmdphchk As OracleCommand
            Dim dtchk As New DataTable
            Try
                ls_sqlphchk = "select ACM_COMPANY_CODE from hrace.t_action_mapping where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_TYPE='PH' and ACM_CATEGORY=:ACM_CATEGORY and ACM_END_DT >=trunc(sysdate)"
                cmdphchk = New OracleCommand(ls_sqlphchk, con)
                cmdphchk.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))
                cmdphchk.Parameters.Add(New OracleParameter(":ACM_CATEGORY", cmbCategory.SelectedValue.ToString))
                dtchk = getRecord(cmdphchk, con)
                If dtchk.Rows.Count > 0 Then
                    ShowMessage("Please enter mobile number")
                    Exit Sub
                End If

            Catch ex As Exception

            End Try


        Else
            Dim numericph As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If (numericph.IsMatch(txtPhNo.Text.Trim)) Then
                If txtPhNo.Text.ToString.Length = 10 Then
                Else
                    ShowMessage("Please enter 10 digit mobile number")
                    Exit Sub
                End If
            Else
                ShowMessage("Please provide valid mobile number")
                Exit Sub
            End If
        End If

        Dim locCheck = CheckWireFrameLoc()

        'Start add by Prasun Chakraborty on 11032022
        If Not IsFormAValid() Then
            Exit Sub
        End If
        'End add by Prasun Chakraborty on 11032022

        Dim hidepan As String = ""
        Dim hideaadhar As String = ""
        Dim hidemobile As String = ""


        sqlProfile = " insert into T_CEMP_DETAILS_TMP(CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY,CET_LOC_CODE"
        sqlProfile += ",CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,CET_FATHER_NAME,CET_SPOUSE_NAME,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_UNIQUE_ID_TYPE,CET_MEDICAL_CENTRE,CET_REQ_CATEGORY,"
        'Start Edit by Prasun Chakraborty on 11032022
        If pnlFormA.Visible Then
            sqlProfile += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,CET_DOB,CET_AGE,CET_AFFIRMATIVE,CET_CREATED_BY,CET_CREATED_DATE,"
            sqlProfile += "CET_PAN_NO,CET_ADLT_NAME,CET_ADLT_REL,CET_ADLT_ADDRESS,CET_ADLT_MOBILE_NO,CET_NATIONALITY,CET_AADHAR_NO,CET_EMP_PLACE,CET_RELAY_DATA)"
            hidepan = AESEncryption.Encrypt(txtPAN.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)

            hideaadhar = AESEncryption.Encrypt(txtAADHAR.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)
            hidemobile = AESEncryption.Encrypt(txtAdltMobile.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256)

        Else
            sqlProfile += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,CET_DOB,CET_AGE,CET_AFFIRMATIVE,CET_CREATED_BY,CET_CREATED_DATE)"

        End If

        sqlProfile += " values('"
        sqlProfile = sqlProfile + vSPNO + "','"
        sqlProfile = sqlProfile + Session("requestnumber") + "','"
        sqlProfile = sqlProfile + comp_cd + "','"
        sqlProfile = sqlProfile + Session("VendCode") + "','"
        sqlProfile = sqlProfile + cmbCategory.SelectedValue + "','"
        sqlProfile = sqlProfile + vLocCd + "','"
        sqlProfile = sqlProfile + Txtdeprt.Text + "','"
        sqlProfile = sqlProfile + txtFName.Text.ToString.Trim().ToUpper() + "','"
        sqlProfile = sqlProfile + txtLName.Text.ToString.Trim().ToUpper() + "','"
        sqlProfile = sqlProfile + txtFatherName.Text.ToString.Trim().ToUpper() + "','"
        sqlProfile = sqlProfile + txtHusName.Text.ToString.Trim().ToUpper() + "','"
        sqlProfile = sqlProfile + cmbSex.SelectedValue + "','"

        sqlProfile = sqlProfile + txtEmrgNo.Text.ToString.Trim() + "','"
        sqlProfile = sqlProfile + txtPhNo.Text.ToString.Trim() + "','"
        sqlProfile = sqlProfile + cmbUniqID.SelectedValue + "','"


        If (locCheck = True And Session("requestType") = "SPN") Then
            sqlProfile = sqlProfile + ddlMedCentre.SelectedValue + "','"
            sqlProfile = sqlProfile + "1" + "','"
        Else
            sqlProfile = sqlProfile + ddlMedCentre.SelectedValue + "','"
            sqlProfile = sqlProfile + "0" + "','"
        End If
        sqlProfile = sqlProfile + txtUniqIDNo.Text.ToString.Trim().ToUpper + "','"
        sqlProfile = sqlProfile + txtIdentiFication.Text.ToString.Trim().ToUpper().Replace("'", "''") + "','"
        sqlProfile = sqlProfile + cmbWorkArea.Text.ToString.Trim() + "',"
        sqlProfile = sqlProfile + "to_date('" + txtDOB.Text.ToString.Trim() + "','DD/MM/YYYY')" + ","
        sqlProfile = sqlProfile + "to_char(sysdate,'yyyy') - to_char(to_date('" + txtDOB.Text.ToString.Trim() + "','DD/MM/YYYY'),'yyyy')" + ",'"
        sqlProfile = sqlProfile + cmbAffirmative.SelectedValue + "','"
        sqlProfile = sqlProfile + Session("VendCode") + "',"

        'Start Edit by Prasun Chakraborty on 11032022
        'sqlProfile = sqlProfile + "SYSDATE" + ")"
        If pnlFormA.Visible Then
            sqlProfile = sqlProfile + "SYSDATE, '"
            sqlProfile = sqlProfile + hidepan + "','"
            sqlProfile = sqlProfile + txtAdltName.Text + "','"
            sqlProfile = sqlProfile + cmbAdltRelation.SelectedValue + "','"
            sqlProfile = sqlProfile + txtAdltAddress.Text + "','"
            sqlProfile = sqlProfile + hidemobile + "','"
            sqlProfile = sqlProfile + cmbNationality.SelectedValue + "','"
            sqlProfile = sqlProfile + hideaadhar + "','"
            sqlProfile = sqlProfile + cmbPlaceOfEmployment.SelectedValue + "','"
            sqlProfile = sqlProfile + cmbRelayData.SelectedValue + "')"
        Else
            sqlProfile = sqlProfile + "SYSDATE" + ")"
        End If
        'End Edit by Prasun Chakraborty on 11032022
        SaveData(sqlProfile, con)


        If (comp_cd = "1000") Then
            If cmbCategory.SelectedValue = "VC" Then
                agency_code = "VCP"
            Else
                agency_code = "RTC"
            End If
        Else
            setLocationCode(vSPNO, Session("requestnumber"))
            Dim sqlAgencyCd = "select am.sam_agency_code from hrace.t_safety_agency_master am where am.sam_location_code=:loc_cd"
            cmd = New OracleCommand(sqlAgencyCd, con)
            cmd.Parameters.Add(New OracleParameter(":loc_cd", vLocCd))
            Dim dtAgendyCd As DataTable = getRecord(cmd, con)
            agency_code = dtAgendyCd.Rows(0).Item(0)
        End If





        Try
            'WI9047: Enhancement in page to allow safety pass related data to be eligible during profile creation for new cases
            Dim str1 As String = String.Empty
            safetyPassdetails(vSPNO, Session("requestnumber"))
            Dim compst As String = getAragyaCompLoc(Session("Comp_Code"))
            If compst = "Y" Then
                str1 = "INSERT INTO HRACE.T_CEMP_DETAILS (CED_SAFETY_PASS_NO, CED_REQ_NO, CED_AGENCY_CODE, CED_COMPANY_CODE, CED_VENDOR_CODE, CED_CATEGORY, CED_LOC_CODE, CED_DEPT_CODE, CED_FIRSTNAME, CED_LASTNAME,"
                str1 += "CED_FATHER_NAME, CED_HUSBAND_NAME, CED_ADDRESS1, CED_ADDRESS2, CED_ADDRESS3, CED_COUNTRY, CED_EMERGENCY_NO, CED_PHONE_NO, CED_GENDER, CED_BLOOD_GROUP, CED_UNIQUE_ID_TYPE, CED_UNIQUE_ID_VALUE, CED_IDENTIFICATION_MARK,"
                str1 += "CED_QUALIFIATION, CED_AREA_OF_WORK, CED_CREATED_DATE, CED_CREATED_BY, CED_AGE, CED_DOB, CED_AFFIRMATIVE, CED_WORK_BASED_ON, CED_SUBLOC_CODE, CED_PV_ISSUED_ON, CED_PV_VALID_TILL, CED_POLICE_VERIFICATION, CED_MED_FIT, CED_DOB_CERT_NO, CED_DRV_CERT_NO, CED_PASS_CERT_NO, CED_UAN_NO, CED_IP_NO,CED_FLAG)"
                str1 += " VALUES('" + vSPNO + "','" + Session("requestnumber") + "','" + agency_code.Trim + "','" + Session("Comp_Code") + "','" + vendorCode.Trim + "','" + cmbCategory.SelectedValue + "','" + location.Trim + "','" + dept.Trim + "','" + firstname + "','" + lastname + "'"
                str1 += " ,'" + fatherName + "','" + spouse + "','NA','','','" + country + "','" + emergencyNo + "','" + phoneNo + "',"
                str1 += "'" + gender + "','NA','" + uniqueIDType + "','" + uniqueIDVal + "','" + identityMark + "',"
                str1 += "'" + qualification + "','" + areaofWork + "',sysdate,'System','" + birthAge + "',to_date('" + dob + "','DD/MM/YYYY'),'" + affirmative + "','NA','NA',null,null,'Y','N','" + dobcertno + "','" + drvcertno + "','" + passcertno + "','" + UAN + "','" + IP + "','Y' ) "
                SaveData(str1, con)
                'WI9047: End of code
            End If

            btnSaveProfile.Visible = False
            btnUpdateProfile.Visible = True
            ShowMessage("Saved Sucessfully")
            tabcontainer1.Style.Remove("display")
            BtnNext.Visible = True
            Lblspno.Visible = True
            TxtSpno.Visible = True
            TxtSpno.Text = vSPNO
            empView()

            lblpfesiErrMsg.Text = ""
            mpconfirmsubmit.Show()

            'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            'If vCategory = SV Then
            'count = Session("supvsr")
            If vCategory = SV Or vCategory = SH Or vCategory = SF Then
                count = Session("supvsr")
                vCategory = String.Format("'{0}','{1}','{2}'", SV, SH, SF)
            ElseIf vCategory = DV Or vCategory = DA Or vCategory = DH Then
                count = Session("Driver")
                vCategory = String.Format("'{0}','{1}','{2}'", DV, DA, DH)
            ElseIf vCategory = WR Or vCategory = WA Then
                count = Session("worker")
                vCategory = String.Format("'{0}','{1}'", WR, WA)
            ElseIf vCategory = FM Or vCategory = FA Then
                count = Session("FM")
                vCategory = String.Format("'{0}','{1}'", FM, FA)
            ElseIf vCategory = VC Or VA Then
                count = Session("VC")
                vCategory = String.Format("'{0}','{1}'", VC, VA)
            End If

            count_emp(count, vCategory, Session("requestnumber"))


        Catch ex As Exception
            ShowMessage(ex.ToString)
        End Try
    End Sub
    Private Function GetSPInitial() As String
        Dim locCd As String = GetLocationName(Session("Comp_code"))
        Dim sqlAgencyCd = " select am.sam_agency_code from hrace.t_safety_agency_master am where am.sam_location_code=:loc_cd"
        Dim command As OracleCommand = New OracleCommand(sqlAgencyCd, con)
        command.Parameters.Add(New OracleParameter(":loc_cd", locCd))
        Dim dtAgendyCd As DataTable = getRecord(command, con)
        Return dtAgendyCd.Rows(0).Item(0).ToString().Substring(0, 1)
    End Function

    'Protected Sub btnAge_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAge.Click
    '    Dim vAge As String
    '    Dim vDOBYear As String
    '    Dim vTodayYear As String
    '    Try
    '        vDOBYear = DateTime.ParseExact(txtDOB.Text.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy")
    '        vTodayYear = DateTime.ParseExact(Date.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "dd/MM/yyyy", CultureInfo.InvariantCulture)

    '        vAge = (CInt(vTodayYear) - CInt(vDOBYear)).ToString()
    '        btnAge.Text = vAge + " Years"
    '    Catch ex As Exception
    '    End Try
    'End Sub
    Protected Sub btnUpdateProfile_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateProfile.Click
        If txtDOB.Text <> "" And txtDOB.Text <> "__/__/____" Then
            Dim dob As Date = DateTime.ParseExact(txtDOB.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            Dim age As Double = GetAge(dob)
            Dim maxAge As Integer = GetMaxAge()
            Dim trainee As String = cmbCategory.Items(0).Value.Substring(0, 1) + "A"
            If age < 18 Then
                ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " & Session("comp_name_d") & " .")
            ElseIf age >= 18 And age <= 20 Then
                UpdateProfile()
                cmbCategory.Items.FindByValue(trainee).Enabled = True
                cmbCategory.SelectedValue = trainee
                cmbCategory.Enabled = False
            ElseIf age > maxAge Then
                hfActionPerformed.Value = "U"
                ageMessage.InnerText = "You need to attach department’s chief approval for person above " & maxAge.ToString & " years of age at the time of generating safety pass."
                pnlConfirmDocSubmision.Visible = True
                MPopUpConfirmDocSubmision.Show()
            Else
                UpdateProfile()
                cmbCategory.Items.FindByValue(trainee).Enabled = False
                cmbCategory.Enabled = True
            End If
        End If
    End Sub
    Private Sub UpdateProfile()
        err_cnt = 0
        Dim vSPNo As String = ""
        Dim sqlUpdProfile As String = ""

        Dim vSysDate As String = Today.ToString("dd/MM/yyyy")



        If TxtSpno.Text.Trim = "" Then
            ShowMessage("Please Select Safety Pass No from the Gridview")
            Exit Sub
        Else
            vSPNo = TxtSpno.Text.Trim.ToUpper
        End If

        Dim vErrorCount As Integer = 0
        vErrorCount = CheckProfileMandatoryFields()
        If vErrorCount > 0 Then
            tblProfileErrorList.Visible = True
            Exit Sub
        Else
            tblProfileErrorList.Visible = False
        End If
        Dim sqlDuplicateID As String = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null) and CET_SAFETY_PASSNO <>'" + TxtSpno.Text.Trim + "' "
        Dim dtDuplicateID As New DataTable()
        dtDuplicateID = getRecord(sqlDuplicateID, con)
        If dtDuplicateID.Rows.Count > 0 Then

            ErrorRow(tblProfileErrorList, "This ID Card (Aadhar Number) already Exists in system for SP No:  " + dtDuplicateID.Rows(0)("CET_SAFETY_PASSNO") + " ,please use this SP No. to raise request")
            tblProfileErrorList.Visible = True
            Exit Sub
        End If
        '''''''''''''''check uniq ID number already exist or not'''''''''
        sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.ToString.Trim().ToUpper + "' and CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim + "'"
        dtDuplicateID = getRecord(sqlDuplicateID, con)
        If dtDuplicateID.Rows.Count > 0 Then
            ErrorRow(tblProfileErrorList, "This ID Card (Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows(0)("CED_SAFETY_PASS_NO") + ",please use this SP No. to raise request")
            tblProfileErrorList.Visible = True
            Exit Sub
        End If
        If cmbUniqID.SelectedValue = "ADC" Then
            Dim strchk As String = txtUniqIDNo.Text
            Dim st As Boolean = strchk.Contains(" ")
            If st Then
                ShowMessage("This is not a valid Adhaar number")
                Exit Sub
            End If
            If strchk.Length = 12 Then
            Else
                ShowMessage("This is not a valid Adhaar number")
                Exit Sub
            End If
            Dim numeric As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If (numeric.IsMatch(strchk)) Then
            Else
                ShowMessage("This is not a valid Adhaar number")
                Exit Sub
            End If
        ElseIf cmbUniqID.SelectedValue = "PAN" Then
            Dim strchk As String = txtUniqIDNo.Text
            Dim st As Boolean = strchk.Contains(" ")
            If st Then
                ShowMessage("This is not a valid PAN number")
                Exit Sub
            End If
            If strchk.Length = 10 Then
            Else
                ShowMessage("This is not a valid PAN number")
                Exit Sub
            End If
            Dim alphanumeric As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("[A-Z]{5}\d{4}[A-Z]{1}")
            If (alphanumeric.IsMatch(strchk)) Then
            Else
                ShowMessage("This is not a valid PAN number")
                Exit Sub
            End If
        End If
        If txtPhNo.Text.Trim = "" Then
            Dim ls_sqlphchk As String = String.Empty
            Dim cmdphchk As OracleCommand
            Dim dtchk As New DataTable
            Try
                ls_sqlphchk = "select ACM_COMPANY_CODE from hrace.t_action_mapping where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_TYPE='PH' and ACM_CATEGORY=:ACM_CATEGORY and ACM_END_DT >=trunc(sysdate)"
                cmdphchk = New OracleCommand(ls_sqlphchk, con)
                cmdphchk.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", comp_cd))
                cmdphchk.Parameters.Add(New OracleParameter(":ACM_CATEGORY", cmbCategory.SelectedValue.ToString))
                dtchk = getRecord(cmdphchk, con)
                If dtchk.Rows.Count > 0 Then
                    ShowMessage("Please enter mobile number")
                    Exit Sub
                End If

            Catch ex As Exception

            End Try


        Else
            Dim numericph As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If (numericph.IsMatch(txtPhNo.Text.Trim)) Then
                If txtPhNo.Text.ToString.Length = 10 Then
                Else
                    ShowMessage("Please enter 10 digit mobile number")
                    Exit Sub
                End If
            Else
                ShowMessage("Please provide valid mobile number")
                Exit Sub
            End If
        End If

        Dim locCheck = CheckWireFrameLoc()

        'Start add by Prasun Chakraborty on 11032022
        If Not IsFormAValid() Then
            Exit Sub
        End If
        'End add by Prasun Chakraborty on 11032022


        sqlUpdProfile = "update t_cemp_details_tmp set "

        sqlUpdProfile = sqlUpdProfile + "CET_FIRSTNAME ='" + txtFName.Text.ToString.Trim().ToUpper() + "',"

        sqlUpdProfile = sqlUpdProfile + "CET_LASTNAME ='" + txtLName.Text.ToString.Trim().ToUpper() + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_FATHER_NAME ='" + txtFatherName.Text.ToString.Trim().ToUpper() + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_SPOUSE_NAME ='" + txtHusName.Text.ToString.Trim().ToUpper() + "',"

        sqlUpdProfile = sqlUpdProfile + "CET_EMERGENCY_NO ='" + txtEmrgNo.Text.ToString.Trim() + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_PHONE_NO ='" + txtPhNo.Text.ToString.Trim() + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_AREA_OF_WORK ='" + cmbWorkArea.Text.ToString.Trim() + "',"


        sqlUpdProfile = sqlUpdProfile + "CET_DOB =" + " to_date('" + txtDOB.Text.ToString.Trim + "','DD/MM/YYYY')" + ","
        sqlUpdProfile = sqlUpdProfile + "CET_GENDER ='" + cmbSex.SelectedValue + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_LOCATION_CODE ='" + comp_cd + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_LOC_CODE ='" + vLocCd + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_VENDOR_CODE ='" + Session("VendCode") + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_DEPT_CODE ='" + Txtdeprt.Text.ToString.Trim + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_UNIQUE_ID_TYPE ='" + cmbUniqID.SelectedValue + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_UNIQUE_ID_VALUE ='" + txtUniqIDNo.Text.Trim.ToUpper + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_IDENTIFICATION_MARK ='" + txtIdentiFication.Text.Trim.ToUpper().Replace("'", "''") + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_CATEGORY ='" + cmbCategory.SelectedValue + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_AFFIRMATIVE ='" + cmbAffirmative.SelectedValue + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_BY ='" + Session("VendCode") + "',"

        If (locCheck = True And Session("requestType") = "SPN") Then
            sqlUpdProfile = sqlUpdProfile + "CET_MEDICAL_CENTRE ='" + ddlMedCentre.SelectedValue + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_REQ_CATEGORY ='1',"
        Else
            sqlUpdProfile = sqlUpdProfile + "CET_MEDICAL_CENTRE ='" + ddlMedCentre.SelectedValue + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_REQ_CATEGORY ='0',"
        End If

        'Start Edit by Prasun Chakraborty on 11032022
        'sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE "
        If pnlFormA.Visible Then
            sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE, "
            sqlUpdProfile = sqlUpdProfile + "CET_PAN_NO ='" + AESEncryption.Encrypt(txtPAN.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256) + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_NAME ='" + txtAdltName.Text + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_REL ='" + cmbAdltRelation.SelectedValue + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_ADDRESS ='" + txtAdltAddress.Text + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_MOBILE_NO = '" + AESEncryption.Encrypt(txtAdltMobile.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256) + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_NATIONALITY = '" + cmbNationality.SelectedValue + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_AADHAR_NO = '" + AESEncryption.Encrypt(txtAADHAR.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256) + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_EMP_PLACE = '" + cmbPlaceOfEmployment.SelectedValue + "',"
            sqlUpdProfile = sqlUpdProfile + "CET_RELAY_DATA = '" + cmbRelayData.SelectedValue + "'"
            'sqlProfile = sqlProfile + cmbNationality.SelectedValue + "','"
            'sqlProfile = sqlProfile + txtAADHAR.Text + "','"
            'sqlProfile = sqlProfile + cmbPlaceOfEmployment.SelectedValue + "')"
        Else
            sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE "
        End If
        'End Edit by Prasun Chakraborty on 11032022

        sqlUpdProfile = sqlUpdProfile + " where CET_SAFETY_PASSNO = '" + vSPNo + "'"
        sqlUpdProfile = sqlUpdProfile + " and  CET_REQUEST_NO = '" + Session("requestnumber") + "'"

        Try
            SaveData(sqlUpdProfile, con)
            ShowMessage("Updated Sucessfully")
            Renewal_profile_details(vSPNo)
            empView()
            btnUpdateProfile.Visible = True
            lblpfesiErrMsg.Text = ""
            mpconfirmsubmit.Show()
        Catch ex As Exception
            ShowMessage("Error While Updating Record")
        End Try
    End Sub
    Protected Sub Btnreset_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Btnreset.Click
        clearProfile()
    End Sub
    Public Sub clearProfile()
        txtFName.Text = ""
        txtLName.Text = ""
        txtDOB.Text = ""
        cmbSex.SelectedValue = "0"
        txtFatherName.Text = ""
        txtHusName.Text = ""
        txtPhNo.Text = ""
        txtEmrgNo.Text = ""
        cmbUniqID.SelectedValue = "0"
        txtIdentiFication.Text = ""
        cmbAffirmative.SelectedValue = "0"
        txtUniqIDNo.Text = ""
        Btnreset.Visible = True
        cmbWorkArea.SelectedValue = "0"
        btnSaveProfile.Visible = True
        TxtSpno.Text = ""
        Lblspno.Visible = False
        TxtSpno.Visible = False
        cmbCategory.SelectedIndex = 0
        ddlMedCentre.SelectedValue = "0"
    End Sub

#End Region

#Region "address"
    Public Sub clearAddress()
        txtAddHouseNo.Text = ""
        txtAddMobile.Text = ""
        txtAddName.Text = ""
        txtAddPIN.Text = ""
        txtAddStreet.Text = ""
        txtLandLine.Text = ""
        txtAddEmail.Text = ""
        lbladdattachname.Text = ""
        cmbAddCity.Items.Clear()
        cmbAddState.SelectedValue = "JH"
        cmbAddCountry.SelectedValue = "IND"
        GetCity(cmbAddState.SelectedValue)
        btnSaveAddress.Enabled = True
        btnUpdateAddress.Enabled = False
        lbladdattachname.Text = String.Empty

        txtAddVillage.Text = ""
        txtAddPO.Text = ""
        txtAddThana.Text = ""
        cmbAddDistrict.Items.Clear()
        GetDistrict(cmbAddState.SelectedValue)

    End Sub
    Public Sub GetAddressType()
        Dim sqlAddressType As String = clmClass.get_CodeValue("ETYP")
        sqlAddressType = sqlAddressType + ",CTM_TYPE_DESC"
        Dim dtAddressType As New DataTable()
        dtAddressType = getRecord(sqlAddressType, con)
        cmbAddressType.Items.Clear()

        If dtAddressType.Rows.Count > 0 Then
            cmbAddressType.DataSource = dtAddressType
            cmbAddressType.DataTextField = "CTM_TYPE_DESC"
            cmbAddressType.DataValueField = "CTM_TYPE_CODE"
            cmbAddressType.DataBind()
            cmbAddressType.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Public Sub GetAddress(ByVal vSPNo As String)
        '   clearAddress()
        'Dim sqlAddress As String = "SELECT CCA_CERT_NO,T1.CCA_ADDRESS_ID, T3.CMT_COUNTRY_NAME, T4.SMT_STATE_NAME, T5.CIT_CITY_NAME, T1.CCA_WORKMEN_TYPE EMP_TYPE, T1.CCA_ADDR_TYPE ADDRESS_TYPE, T2.CTM_TYPE_DESC ADDRESS_TYPE_DESC, T1.CCA_NAME CCA_NAME, T1.CCA_HOUSE_NO HOUSE_NO, T1.CCA_STREET STREET, T1.CCA_CITY CITY_CD, T1.CCA_STATE STATE_CD, T1.CCA_COUNTRY COUNTRY_CD, T1.CCA_PIN, T1.CCA_MOBILE, T1.CCA_EMAIL, T1.CCA_LAND_LINE, TO_CHAR(T1.CCA_START_DT, 'DD/MM/YYYY') CCA_START_DT, TO_CHAR(T1.CCA_END_DT, 'DD/MM/YYYY') CCA_END_DT, T1.CCA_REMARKS,nvl(T6.DM_NAME,' ') DM_NAME,T1.CCA_REQ_NO,CCA_REMARKS,CCA_CERT_NO FROM T_CWM_CEMP_ADDRS_TMP T1, T_CEMP_TYPE_MASTER T2, T_COUNTRY_MASTER T3, T_STATE_MASTER T4 , T_CITY_MASTER T5,T_DOCUMENT_MASTER T6 WHERE T1.CCA_ADDR_TYPE = T2.CTM_TYPE_CODE AND T1.CCA_COUNTRY = T3.CMT_COUNTRY_CODE AND T1.CCA_COUNTRY = T4.SMT_COUNTRY_CODE AND  T1.CCA_STATE = T4.SMT_STATE_CODE AND T1.CCA_COUNTRY = T5.CIT_COUNTRY_CODE AND  T1.CCA_STATE = T5.CIT_STATE_CODE AND T1.CCA_CITY = T5.CIT_CITY_CODE AND  T1.CCA_SAFETY_PASS_NO = '" + vSPNo + "' AND T1.CCA_COMP_CD = '" + comp_cd + "' and T1.CCA_CERT_NO=T6.DM_DOC_ID(+) order by T1.CCA_REQ_NO desc "
        Dim sqlAddress As String = "SELECT CCA_CERT_NO,T1.CCA_ADDRESS_ID, T3.CMT_COUNTRY_NAME, T4.SMT_STATE_NAME, T5.CIT_CITY_NAME, T1.CCA_WORKMEN_TYPE EMP_TYPE, T1.CCA_ADDR_TYPE ADDRESS_TYPE, T2.CTM_TYPE_DESC ADDRESS_TYPE_DESC, T1.CCA_NAME CCA_NAME, T1.CCA_HOUSE_NO HOUSE_NO, T1.CCA_STREET STREET, T1.CCA_CITY CITY_CD, T1.CCA_STATE STATE_CD, T1.CCA_COUNTRY COUNTRY_CD, T1.CCA_PIN, T1.CCA_MOBILE, T1.CCA_EMAIL, T1.CCA_LAND_LINE, TO_CHAR(T1.CCA_START_DT, 'DD/MM/YYYY') CCA_START_DT, TO_CHAR(T1.CCA_END_DT, 'DD/MM/YYYY') CCA_END_DT, T1.CCA_REMARKS,nvl(T6.DM_NAME,' ') DM_NAME,T1.CCA_REQ_NO,CCA_REMARKS,CCA_CERT_NO, T1.CCA_VILLAGE, T1.CCA_PO, T1.CCA_THANA, T1.CCA_DISTRICT_CD , T7.DST_DISTRICT_NAME FROM T_CWM_CEMP_ADDRS_TMP T1, T_CEMP_TYPE_MASTER T2, T_COUNTRY_MASTER T3, T_STATE_MASTER T4 , T_CITY_MASTER T5,T_DOCUMENT_MASTER T6, hrace.t_district_master T7 WHERE T1.CCA_ADDR_TYPE = T2.CTM_TYPE_CODE AND T1.CCA_COUNTRY = T3.CMT_COUNTRY_CODE AND T1.CCA_COUNTRY = T4.SMT_COUNTRY_CODE AND  T1.CCA_STATE = T4.SMT_STATE_CODE AND T1.CCA_COUNTRY = T5.CIT_COUNTRY_CODE AND  T1.CCA_STATE = T5.CIT_STATE_CODE AND T1.CCA_CITY = T5.CIT_CITY_CODE AND  T1.CCA_SAFETY_PASS_NO = '" + vSPNo + "' AND T1.CCA_COMP_CD = '" + comp_cd + "' and T1.CCA_CERT_NO=T6.DM_DOC_ID(+) AND T1.CCA_COUNTRY = T7.DST_COUNTRY_CODE(+) AND  T1.CCA_STATE = T7.DST_STATE_CODE(+) AND T1.CCA_DISTRICT_CD = T7.DST_DISTRICT_CODE(+) order by T1.CCA_REQ_NO desc "

        Dim dtAddress As New DataTable()
        dtAddress = getRecord(sqlAddress, con)

        If dtAddress.Rows.Count > 0 Then

            gvAddress.DataSource = dtAddress
            gvAddress.DataBind()
            pnlAddressDetail.Visible = True

            If dtAddress.Rows(0).Item("CCA_CERT_NO").ToString <> "" Then
                hddaddressold.Value = dtAddress.Rows(0).Item("CCA_CERT_NO").ToString
                imgaddressold.Visible = True
                ChkoldAddress.Visible = True
            Else
                hddaddressold.Value = ""
                imgaddressold.Visible = False
                ChkoldAddress.Visible = False
            End If
        Else

            gvAddress.DataSource = Nothing
            gvAddress.DataBind()
            pnlAddressDetail.Visible = False

            hddaddressold.Value = ""
            imgaddressold.Visible = False
            ChkoldAddress.Visible = False
        End If
    End Sub
    Public Sub GetCountry()
        Dim sqlCountry As String = "select * from T_COUNTRY_MASTER"
        Dim dtCountry As New DataTable()
        dtCountry = getRecord(sqlCountry, con)
        cmbAddCountry.Items.Clear()
        If dtCountry.Rows.Count > 0 Then
            cmbAddCountry.DataSource = dtCountry
            cmbAddCountry.DataTextField = "CMT_COUNTRY_NAME"
            cmbAddCountry.DataValueField = "CMT_COUNTRY_CODE"
            cmbAddCountry.DataBind()
            cmbAddCountry.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Public Sub GetState()
        Dim sqlState As String = "select * from t_State_Master where SMT_STATE_CODE	not in ('ALL','JHH1') order by SMT_STATE_NAME	  "
        Dim dtState As New DataTable()
        dtState = getRecord(sqlState, con)
        cmbAddState.Items.Clear()
        If dtState.Rows.Count > 0 Then
            cmbAddState.DataSource = dtState
            cmbAddState.DataTextField = "SMT_STATE_NAME"
            cmbAddState.DataValueField = "SMT_STATE_CODE"
            cmbAddState.DataBind()
            cmbAddState.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Public Sub GetCity(ByVal vStateCD As String)


        Dim sqlCity As String
        If vStateCD = "0" Then
            sqlCity = "select * from t_CITY_Master "
        Else
            sqlCity = "select * from t_CITY_Master where CIT_STATE_CODE='" + vStateCD + "' order by CIT_CITY_NAME"
        End If

        Dim dtCity As New DataTable()
        dtCity = getRecord(sqlCity, con)
        cmbAddCity.Items.Clear()
        If dtCity.Rows.Count > 0 Then
            cmbAddCity.DataSource = dtCity
            cmbAddCity.DataTextField = "CIT_CITY_NAME"
            cmbAddCity.DataValueField = "CIT_CITY_CODE"
            cmbAddCity.DataBind()
            cmbAddCity.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Protected Sub cmbAddState_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbAddState.SelectedIndexChanged
        Dim vStateCd As String = ""
        vStateCd = cmbAddState.SelectedValue
        GetCity(vStateCd)

        GetDistrict(vStateCd)

    End Sub
    Public Function CheckAddressMandatoryFields() As Integer
        Dim vErrorCount As Integer = 0
        If cmbAddressType.SelectedValue = "0" Then
            ErrorRow(tblAddErrorLst, "Select Address Type")
        End If

        If txtAddName.Text = "" Then
            ErrorRow(tblAddErrorLst, "Enter Name")
        End If

        If txtAddHouseNo.Text = "" Then
            ErrorRow(tblAddErrorLst, "Enter House Number")
        End If
        If cmbAddCity.SelectedValue = "0" Then
            ErrorRow(tblAddErrorLst, "Enter City Name")
        End If

        If cmbAddState.SelectedValue = "0" Then
            ErrorRow(tblAddErrorLst, "Select State")
        End If

        If cmbAddCountry.SelectedValue = "0" Then
            ErrorRow(tblAddErrorLst, "Select Country")
        End If

        If txtAddPIN.Text = "" Then
            ErrorRow(tblAddErrorLst, "Enter PIN Number")
        End If
        If txtAddPIN.Text.Length < 6 Then
            ErrorRow(tblAddErrorLst, "Enter a valid PIN Number")
        End If

        If txtAddVillage.Text = "" Then
            ErrorRow(tblAddErrorLst, "Enter Village name")
        End If

        If txtAddPO.Text = "" Then
            ErrorRow(tblAddErrorLst, "Enter Post Office name")
        End If

        If txtAddThana.Text = "" Then
            ErrorRow(tblAddErrorLst, "Enter Thana name")
        End If

        If cmbAddDistrict.SelectedValue = "0" Then
            ErrorRow(tblAddErrorLst, "Select District")
        End If


        vErrorCount = err_cnt
        Return vErrorCount
    End Function
    Protected Sub btnSaveAddress_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveAddress.Click
        Dim sqlAddress As String = ""
        Dim vAddressID As String = ""
        Dim vSPNo As String = TxtSpno.Text.Trim.ToUpper()
        Dim filename As String = String.Empty
        If vSPNo = "" Then
            ShowMessage("Address could not be saved as specifc employee safety pass number was not found.click on the safety pass number to add details.")
            Exit Sub
        End If
        If txtAddMobile.Text.Trim.Equals("") Then
            ShowMessage("Please enter mobile number of vendor")
            Exit Sub
        End If
        If txtAddEmail.Text.Trim.Equals("") Then
            ShowMessage("Please enter email id of vendor")
            Exit Sub
        End If
        If fupdl_add.HasFile = False And ChkoldAddress.Checked = False Then
            ShowMessage("Please Upload File")
            Exit Sub
        ElseIf fupdl_add.HasFile = True And ChkoldAddress.Checked = True Then
            ShowMessage("choose either file upload or check previous upload documents option")
            Exit Sub
        ElseIf fupdl_add.HasFile = True And ChkoldAddress.Checked = False Then
            filename = Path.GetFileName(fupdl_add.PostedFile.FileName)
            Dim contentType As String = fupdl_add.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdl_add.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdl_add.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        Dim vErrorCount As Integer = 0
        vErrorCount = CheckAddressMandatoryFields()
        If vErrorCount > 0 Then
            tblAddErrorLst.Visible = True
            Exit Sub
        Else
            tblAddErrorLst.Visible = False
        End If

        Dim sql As String = emp_addrs_detail_qry(vSPNo) + "and CCA_ADDR_TYPE='" + cmbAddressType.SelectedValue + "'"
        Dim dt As DataTable = getRecord(sql, con)
        If dt.Rows.Count > 0 Then
            ShowMessage("The type of address is already saved")
            Exit Sub
        End If
        'updateprevaddvalidity(vSPNo)

        vAddressID = GetID("seq_cemp_address")

        'sqlAddress = "INSERT INTO  HRACE.T_CWM_CEMP_ADDRS_TMP ( CCA_ADDRESS_ID,CCA_REQ_NO, CCA_COMP_CD, CCA_SAFETY_PASS_NO,CCA_WORKMEN_TYPE, CCA_ADDR_TYPE,  CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE,CCA_START_DT,CCA_END_DT,CCA_CREATED_BY, CCA_CREATED_DT,CCA_REMARKS,CCA_CERT_NO ) VALUES('"
        sqlAddress = "INSERT INTO  HRACE.T_CWM_CEMP_ADDRS_TMP ( CCA_ADDRESS_ID,CCA_REQ_NO, CCA_COMP_CD, CCA_SAFETY_PASS_NO,CCA_WORKMEN_TYPE, CCA_ADDR_TYPE,  CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_VILLAGE, CCA_PO, CCA_THANA, CCA_DISTRICT_CD, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE,CCA_START_DT,CCA_END_DT,CCA_CREATED_BY, CCA_CREATED_DT,CCA_REMARKS,CCA_CERT_NO ) VALUES('"

        sqlAddress = sqlAddress + vAddressID + "','"
        sqlAddress = sqlAddress + Session("requestnumber") + "','"
        sqlAddress = sqlAddress + comp_cd + "','"
        sqlAddress = sqlAddress + vSPNo + "','"
        sqlAddress = sqlAddress + cmbCategory.SelectedValue.ToString.ToUpper + "','"
        sqlAddress = sqlAddress + cmbAddressType.SelectedValue + "','"
        sqlAddress = sqlAddress + txtAddName.Text.ToString.Trim.ToUpper + "','"
        sqlAddress = sqlAddress + txtAddHouseNo.Text.ToString.Trim.ToUpper + "','"
        sqlAddress = sqlAddress + txtAddStreet.Text.ToString.Trim.ToUpper + "','"
        sqlAddress = sqlAddress + cmbAddCity.SelectedValue + "','"
        sqlAddress = sqlAddress + txtAddVillage.Text.ToString.Trim.ToUpper + "','"
        sqlAddress = sqlAddress + txtAddPO.Text.ToString.Trim.ToUpper + "','"
        sqlAddress = sqlAddress + txtAddThana.Text.ToString.Trim.ToUpper + "','"
        sqlAddress = sqlAddress + cmbAddDistrict.SelectedValue + "','"
        sqlAddress = sqlAddress + cmbAddState.SelectedValue + "','"
        sqlAddress = sqlAddress + cmbAddCountry.SelectedValue + "','"
        sqlAddress = sqlAddress + txtAddPIN.Text.ToString.Trim + "','"
        sqlAddress = sqlAddress + txtAddMobile.Text.ToString.Trim + "','"
        sqlAddress = sqlAddress + txtAddEmail.Text.ToString.Trim + "','"
        sqlAddress = sqlAddress + txtLandLine.Text.ToString.Trim + "',"

        sqlAddress = sqlAddress + "to_date(to_char(sysdate,'DD/MM/YYYY'),'DD/MM/YYYY')" + ","
        sqlAddress = sqlAddress + "to_date('31/12/9999','DD/MM/YYYY')" + ",'"
        sqlAddress = sqlAddress + Session("VendCode") + "',"
        sqlAddress = sqlAddress + "SYSDATE" + ","
        If ChkoldAddress.Checked = True Then
            sqlAddress = sqlAddress + "'O'" + ","
            sqlAddress = sqlAddress + hddaddressold.Value + ")"
        Else
            sqlAddress = sqlAddress + "'N'" + ","
            sqlAddress = sqlAddress + vAddressID + ")"
        End If


        Try
            If ChkoldAddress.Checked = False Then
                If fupdl_add.HasFile = True Then
                    Dim cmdfileadd As New OracleCommand
                    Dim ls_sql As String = String.Empty
                    filename = Path.GetFileName(fupdl_add.PostedFile.FileName)
                    Using fs As Stream = fupdl_add.PostedFile.InputStream
                        Using br As BinaryReader = New BinaryReader(fs)
                            Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileadd.CommandText = ls_sql
                            cmdfileadd.Connection = con
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_DOC_ID", vAddressID))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "ADD"))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileQual.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            'cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileadd.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Using
                    End Using
                End If
            End If
            SaveData(sqlAddress, con)

            ' btnSaveAddress.Visible = False
            BtnNext.Visible = True
            ShowMessage("Address Saved Sucessfully")
            clearAddress()
            GetAddress(vSPNo)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvAddress.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectAddress")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If
            'btnSaveAddress.Visible = False
            empView()

        Catch ex As Exception
            ShowMessage(ex.ToString)
        End Try
    End Sub
    Private Sub updateprevaddvalidity(ByVal safetypass As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            ls_sql = "select CCA_ADDRESS_ID,CCA_COMP_CD from T_CWM_CEMP_ADDRS_TMP where CCA_SAFETY_PASS_NO=:CCA_SAFETY_PASS_NO and CCA_COMP_CD=:CCA_COMP_CD and CCA_REQ_NO=(select max(CCA_REQ_NO) from T_CWM_CEMP_ADDRS_TMP where CCA_SAFETY_PASS_NO=:CCA_SAFETY_PASS_NO and CCA_COMP_CD=:CCA_COMP_CD)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CCA_SAFETY_PASS_NO", safetypass))
            cmd.Parameters.Add(New OracleParameter(":CCA_COMP_CD", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    ls_sql = "update T_CWM_CEMP_ADDRS_TMP set CCA_END_DT=sysdate where CCA_ADDRESS_ID=:CCA_ADDRESS_ID and CCA_SAFETY_PASS_NO=:CCA_SAFETY_PASS_NO and CCA_COMP_CD=:CCA_COMP_CD "
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":CCA_SAFETY_PASS_NO", safetypass))
                    cmd.Parameters.Add(New OracleParameter(":CCA_COMP_CD", Session("Comp_Code")))
                    cmd.Parameters.Add(New OracleParameter(":CCA_ADDRESS_ID", dt.Rows(i).Item("CCA_ADDRESS_ID")))
                    cmd.ExecuteNonQuery()
                    i = i + 1
                End While
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub btnUpdateAddress_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateAddress.Click
        Dim vAddressID As String = ""
        Dim sqlUpdAddress As String = ""
        Dim vAddressRemark As String = ""
        Dim vAddressdocid As String

        Dim vSPNo As String = TxtSpno.Text.Trim.ToUpper()
        If txtAddMobile.Text.Trim.Equals("") Then
            ShowMessage("Please enter mobile number of vendor")
            Exit Sub
        End If
        If txtAddEmail.Text.Trim.Equals("") Then
            ShowMessage("Please enter email id of vendor")
            Exit Sub
        End If
        Dim vErrorCount As Integer = 0
        vErrorCount = CheckAddressMandatoryFields()
        If vErrorCount > 0 Then
            tblAddErrorLst.Visible = True
            Exit Sub
        Else
            tblAddErrorLst.Visible = False
        End If
        If fupdl_add.HasFile = True Then


            Dim contentType As String = fupdl_add.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdl_add.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdl_add.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        'For i As Integer = 0 To gvAddress.Rows.Count - 1
        '    Dim TickAddress As CheckBox = DirectCast(gvAddress.Rows(i).Cells(0).FindControl("chkSelectAddress"), CheckBox)
        '    If TickAddress.Checked = True Then
        '        btnUpdateAddress.Enabled = True
        '    Else
        '        ShowMessage("please tick the checkbox for updation")
        '    End If
        'Next

        vAddressID = Session("AddressID")
        vAddressRemark = Session("AddressRemark")
        vAddressdocid = Session("Addressdocid")
        If cmbAddressType.SelectedValue = "0" Then
            ShowMessage("Please Select Address Type")
            Exit Sub
        End If
        If cmbAddState.SelectedValue = "0" Then
            ShowMessage("Please Select State")
            Exit Sub
        End If

        If cmbAddCountry.SelectedValue = "0" Then
            ShowMessage("Please Select State")
            Exit Sub
        End If


        Try
            If fupdl_add.HasFile = True Then

                If fupdl_add.HasFile = False Then

                End If
                Dim filename As String = Path.GetFileName(fupdl_add.PostedFile.FileName)
                Dim contentType As String = fupdl_add.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdl_add.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (fupdl_add.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If

                If vAddressRemark = "O" Then

                    vAddressdocid = GetID("seq_cemp_address")
                    Dim cmdfileadd As New OracleCommand
                    Dim ls_sql As String = String.Empty
                    filename = Path.GetFileName(fupdl_add.PostedFile.FileName)
                    Using fs As Stream = fupdl_add.PostedFile.InputStream
                        Using br As BinaryReader = New BinaryReader(fs)
                            Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileadd.CommandText = ls_sql
                            cmdfileadd.Connection = con
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_DOC_ID", vAddressdocid))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "ADD"))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileQual.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            'cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileadd.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Using
                    End Using
                    'End If
                Else

                    Dim ls_sql As String = String.Empty
                    Dim cmdfileadd As New OracleCommand
                    Using fs As Stream = fupdl_add.PostedFile.InputStream
                        Using br As BinaryReader = New BinaryReader(fs)
                            Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If


                            ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileadd.CommandText = ls_sql
                            cmdfileadd.Connection = con
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_DOC_ID", vAddressdocid))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileadd.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileadd.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If

                        End Using
                    End Using

                End If
            End If


            sqlUpdAddress = "UPDATE HRACE.T_CWM_CEMP_ADDRS_TMP SET "

            sqlUpdAddress = sqlUpdAddress + "CCA_ADDR_TYPE ='" + cmbAddressType.SelectedValue + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_START_DT =" + "SYSDATE" + ","
            sqlUpdAddress = sqlUpdAddress + "CCA_END_DT =" + "to_date('" + "31/12/9999" + "','DD/MM/YYYY')" + ","
            sqlUpdAddress = sqlUpdAddress + "CCA_NAME ='" + txtAddName.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_HOUSE_NO ='" + txtAddHouseNo.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_STREET ='" + txtAddStreet.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_CITY ='" + cmbAddCity.SelectedValue + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_VILLAGE ='" + txtAddVillage.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_PO ='" + txtAddPO.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_THANA ='" + txtAddThana.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_DISTRICT_CD ='" + cmbAddDistrict.SelectedValue + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_STATE ='" + cmbAddState.SelectedValue + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_COUNTRY ='" + cmbAddCountry.SelectedValue + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_PIN ='" + txtAddPIN.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_MOBILE ='" + txtAddMobile.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_EMAIL ='" + txtAddEmail.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_LAND_LINE ='" + txtLandLine.Text.ToString.Trim + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_MODIFIED_BY ='" + Session("VendCode") + "',"
            sqlUpdAddress = sqlUpdAddress + "CCA_MODIFIED_DT =" + "SYSDATE" + ""
            ' If fupdl_add.HasFile = True And vAddressRemark = "O" Then
            'MsgBox(vAddressRemark)
            sqlUpdAddress = sqlUpdAddress + ",CCA_CERT_NO ='" + vAddressdocid.Trim + "'"
            'End If
            sqlUpdAddress = sqlUpdAddress + " where CCA_SAFETY_PASS_NO = '" + vSPNo + "' and CCA_ADDR_TYPE ='" + cmbAddressType.SelectedValue + "' and CCA_REQ_NO='" + Session("requestnumber") + "'"

            SaveData(sqlUpdAddress, con)
            updatedocstatus(Session("requestnumber"), vSPNo, "AP")
            ShowMessage("Updated Sucessfully")
            address_details(vSPNo)
            btnUpdateAddress.Visible = True
            GetAddress(TxtSpno.Text)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvAddress.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectAddress")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If
            clearAddress()
        Catch ex As Exception
            ShowMessage("Error While Updating Record")
        End Try

    End Sub

#End Region

#Region "Nominee"
    Public Sub GetShare()
        cmbNomShare.Items.Clear()
        For i = 100 To 1 Step -1
            cmbNomShare.Items.Insert(0, New WebControls.ListItem(i, i))
        Next
        cmbNomShare.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
    End Sub
    Public Sub clearNominee()
        cmbNomRelation.SelectedValue = 0
        txtNomName.Text = ""
        txtNomDOB.Text = ""
        cmbNomPayGrp.SelectedValue = 0
        cmbNomShare.SelectedValue = 0
        txtNomRemarks.Text = ""
        txtNomineeAddress.Text = ""
        btnSaveNominee.Enabled = True
        btnUpdateNominee.Enabled = False

    End Sub
    Public Sub GetRelation()
        Dim sqlRelation As String = clmClass.get_CodeValue("REL")
        Dim dtRelation As New DataTable()
        dtRelation = getRecord(sqlRelation, con)
        cmbNomRelation.Items.Clear()
        If dtRelation.Rows.Count > 0 Then
            cmbNomRelation.DataSource = dtRelation
            cmbNomRelation.DataTextField = "CTM_TYPE_DESC"
            cmbNomRelation.DataValueField = "CTM_TYPE_CODE"
            cmbNomRelation.DataBind()
            cmbNomRelation.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If

        'START ADD BY PRASUN CHAKROBRTY ON 11032022
        cmbAdltRelation.Items.Clear()
        If dtRelation.Rows.Count > 0 Then
            cmbAdltRelation.DataSource = dtRelation
            cmbAdltRelation.DataTextField = "CTM_TYPE_DESC"
            cmbAdltRelation.DataValueField = "CTM_TYPE_CODE"
            cmbAdltRelation.DataBind()
            cmbAdltRelation.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
        'END START ADD BY PRASUN CHAKROBRTY ON 11032022
    End Sub
    Public Sub GetPaymentGrp()
        Dim sqlPayGrp As String = clmClass.get_CodeValue("PGRP")
        Dim dtPayGrp As New DataTable()
        dtPayGrp = getRecord(sqlPayGrp, con)
        cmbNomPayGrp.Items.Clear()
        If dtPayGrp.Rows.Count > 0 Then
            cmbNomPayGrp.DataSource = dtPayGrp
            cmbNomPayGrp.DataTextField = "CTM_TYPE_DESC"
            cmbNomPayGrp.DataValueField = "CTM_TYPE_CODE"
            cmbNomPayGrp.DataBind()
            cmbNomPayGrp.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub
    Public Sub GetNominee(ByVal vSPNo As String)
        Dim sqlNominee As String = " Select t1.ccn_nominee_id, t1.ccn_relation_cd, t2.ctm_type_desc REL_NAME, t1.ccn_nominee_name, to_char(t1.ccn_nominee_dob, 'DD/MM/YYYY') ccn_nominee_dob, t1.ccn_pymt_grp, t3.ctm_type_desc pay_DESC, t1.ccn_share, t1.ccn_remarks, t1.CCN_REQ_NO, NVL(t1.ccn_nominee_address, 'NA') ccn_nominee_address from T_CWM_CEMP_NOMINEES_TMP t1, T_CEMP_TYPE_MASTER t2, T_CEMP_TYPE_MASTER t3 where t1.ccn_relation_cd = t2.ctm_type_code and t1.ccn_pymt_grp = t3.ctm_type_code and t1.ccn_safety_pass_no = '" + vSPNo + "' AND CCN_DELETE_FLAG = 'N' "
        Dim dtNominee As New DataTable()
        dtNominee = getRecord(sqlNominee, con)

        If dtNominee.Rows.Count > 0 Then

            gvNominee.DataSource = dtNominee
            gvNominee.DataBind()
            pnlNomineeDetail.Visible = True
        Else
            gvNominee.DataSource = Nothing
            gvNominee.DataBind()
            pnlNomineeDetail.Visible = False

        End If
    End Sub
    Protected Sub btnSaveNominee_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveNominee.Click
        Dim sqlNomniee As String = ""
        Dim vNomineeID As String = ""
        Dim vSPNO As String = TxtSpno.Text.Trim.ToUpper()

        If vSPNO = "" Then
            ShowMessage("Nominee could not be saved as specifc employee safety pass number was not found.click on safety pass number to add details.")
            Exit Sub
        End If



        Dim vDOB As String = ""
        vDOB = txtNomDOB.Text.ToString.Trim

        Dim vErrorCount As Integer = 0
        vErrorCount = CheckNomineeMandatoryFields()
        If vErrorCount > 0 Then
            tblNomErrorLst.Visible = True
            'mpAddNominee.Show()
            Exit Sub
        Else
            tblNomErrorLst.Visible = False
        End If

        Dim sql As String = emp_nominee_detail_qry(vSPNO) + "and ccn_pymt_grp='" + cmbNomPayGrp.SelectedValue + "' and CCN_REQ_NO='" + Session("requestnumber") + "'"
        Dim dt As DataTable = getRecord(sql, con)
        If dt.Rows.Count > 0 Then
            ShowMessage("The type of Payment group is already saved")
            Exit Sub
        End If

        Dim vIsValid As Boolean = False
        vIsValid = ValidateNominee(vSPNO)
        If vIsValid = False Then
            Dim vPayGrp As String = ""
            Dim vRelation As String = ""
            vPayGrp = cmbNomPayGrp.SelectedItem.Text
            vRelation = cmbNomRelation.SelectedItem.Text

            ShowMessage("Share Value exceeds from 100% for Payment Group : " + vPayGrp)

        Else
            updateprevnominee(vSPNO)
            vNomineeID = GetID("seq_cemp_nominee")

            sqlNomniee = "insert into HRACE.T_CWM_CEMP_NOMINEES_TMP (ccn_nominee_id,CCN_REQ_NO,ccn_comp_code,ccn_safety_pass_no, ccn_relation_cd, ccn_nominee_name,ccn_nominee_dob,ccn_pymt_grp, ccn_share,  ccn_remarks, ccn_nominee_address, ccn_created_by, ccn_created_dt) values ('"
            sqlNomniee = sqlNomniee + vNomineeID + "','"
            sqlNomniee = sqlNomniee + Session("requestnumber") + "','"
            sqlNomniee = sqlNomniee + comp_cd + "','"
            sqlNomniee = sqlNomniee + vSPNO + "','"
            sqlNomniee = sqlNomniee + cmbNomRelation.SelectedValue + "','"
            sqlNomniee = sqlNomniee + txtNomName.Text.ToString.Trim() + "',"
            sqlNomniee = sqlNomniee + "to_date('" + vDOB + "','DD/MM/YYYY')" + ",'"
            sqlNomniee = sqlNomniee + cmbNomPayGrp.SelectedValue + "','"
            sqlNomniee = sqlNomniee + cmbNomShare.SelectedValue + "','"
            sqlNomniee = sqlNomniee + txtNomRemarks.Text.ToString.Trim.Replace("'", "''") + "','"
            sqlNomniee = sqlNomniee + txtNomineeAddress.Text.ToString.Trim.Replace("'", "''") + "','"
            sqlNomniee = sqlNomniee + Session("VendCode") + "',"
            sqlNomniee = sqlNomniee + "SYSDATE" + ")"
            Try
                SaveData(sqlNomniee, con)
                '   btnSaveNominee.Visible = False
                BtnNext.Visible = False
                ShowMessage("Nominee Saved Sucessfully")
                GetNominee(vSPNO)
                Renewal_nominee_details(vSPNO)
                empView()
            Catch ex As Exception
                ShowMessage(ex.ToString)
            End Try

        End If


    End Sub
    Private Sub updateprevnominee(ByVal safetypass As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            ls_sql = "select CCN_NOMINEE_ID,CCN_COMP_CODE from T_CWM_CEMP_NOMINEES_TMP where CCN_SAFETY_PASS_NO=:CCN_SAFETY_PASS_NO and CCN_REQ_NO=(select max(CCN_REQ_NO) from T_CWM_CEMP_NOMINEES_TMP where CCN_SAFETY_PASS_NO=:CCN_SAFETY_PASS_NO AND CCN_REQ_NO<>'" + Session("requestnumber") + "')"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CCN_SAFETY_PASS_NO", safetypass))
            ' cmd.Parameters.Add(New OracleParameter(":CQL_COMP_CODE", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    ls_sql = "update T_CWM_CEMP_NOMINEES_TMP set CCN_DELETE_FLAG='Y' where CCN_NOMINEE_ID=:CCN_NOMINEE_ID and CCN_SAFETY_PASS_NO=:CCN_SAFETY_PASS_NO"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":CCN_SAFETY_PASS_NO", safetypass))

                    cmd.Parameters.Add(New OracleParameter(":CCN_NOMINEE_ID", dt.Rows(i).Item("CCN_NOMINEE_ID")))
                    cmd.ExecuteNonQuery()
                    i = i + 1
                End While
            End If
        Catch ex As Exception

        End Try
    End Sub
    Public Function ValidateNominee(ByVal vSPNo As String) As Boolean
        Dim vIsValid As Boolean = False
        Dim sqlNomValid As String = ""

        Dim dtValidNom As New DataTable
        Dim vPayGrp As String = cmbNomPayGrp.SelectedValue
        Dim vShare As String = cmbNomShare.SelectedValue
        sqlNomValid = "select nvl(sum(k.ccn_share),0) tot_share from T_CWM_CEMP_NOMINEES_tmp k  where k.ccn_safety_pass_no='" + vSPNo + "' and k.ccn_pymt_grp='" + vPayGrp + "' and CCN_DELETE_FLAG	='N' and k.CCN_REQ_NO='" + Session("requestnumber") + "'"
        dtValidNom = getRecord(sqlNomValid, con)

        Dim vTotalShare As Double = 0
        vTotalShare = CDbl(dtValidNom.Rows(0)("tot_share")) + CDbl(vShare)

        If vTotalShare > 100 Then
            vIsValid = False
        Else
            vIsValid = True
        End If


        Return vIsValid
    End Function
    Public Function CheckNomineeMandatoryFields() As Integer
        Dim vErrorCount As Integer = 0
        Dim vTodayDate As Date
        Dim vNomDOB As Date
        vTodayDate = DateTime.ParseExact(Date.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "dd/MM/yyyy", CultureInfo.InvariantCulture)

        If cmbNomRelation.SelectedValue = "0" Then
            ErrorRow(tblNomErrorLst, "Select Relationship")
        End If

        If txtNomName.Text = "" Then
            ErrorRow(tblNomErrorLst, "Enter Nominee Name")
        End If
        'Nominee DOB validation
        If txtNomDOB.Text.Trim = "" Then
            ErrorRow(tblNomErrorLst, "Enter Date of Birth of the Nominee")
        Else
            Try
                vNomDOB = DateTime.ParseExact(txtNomDOB.Text.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                If vNomDOB > vTodayDate Then
                    ErrorRow(tblNomErrorLst, "Nominee DOB can not be a future Date")
                End If
            Catch ex As Exception
                ErrorRow(tblNomErrorLst, "Enter a valid Date of Birth of the Nominee in DD/MM/YYYY Format")
            End Try
        End If

        'START ADD BY PRASUN 25082022
        If txtNomineeAddress.Visible = True Then
            If txtNomineeAddress.Text.Trim.Length = 0 Then
                ErrorRow(tblNomErrorLst, "Nominee Address can not be blank")
            End If
        End If
        'START ADD BY PRASUN 25082022


        If cmbNomPayGrp.SelectedValue = "0" Then
            ErrorRow(tblNomErrorLst, "Select Payment Group")
        End If

        If cmbNomShare.SelectedValue = "0" Then
            ErrorRow(tblNomErrorLst, "Select Percentage Share")
        End If

        vErrorCount = err_cnt
        Return vErrorCount
    End Function
    Protected Sub btnUpdateNominee_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateNominee.Click
        Dim vSPNo As String = TxtSpno.Text.Trim.ToUpper()
        Dim vErrorCount As Integer = 0
        vErrorCount = CheckNomineeMandatoryFields()
        If vErrorCount > 0 Then
            tblQualErrorLst.Visible = True
            Exit Sub
        Else
            tblQualErrorLst.Visible = False
        End If

        Dim sqlUpdNome As String = ""

        sqlUpdNome = "update HRACE.T_CWM_CEMP_NOMINEES_TMP  set "
        sqlUpdNome = sqlUpdNome + " ccn_relation_cd ='" + cmbNomRelation.SelectedValue + "',"
        sqlUpdNome = sqlUpdNome + "ccn_nominee_name ='" + txtNomName.Text.ToString.Trim() + "',"
        sqlUpdNome = sqlUpdNome + "ccn_nominee_dob = to_date('" + txtNomDOB.Text.ToString.Trim + "','dd/mm/yyyy'),"
        sqlUpdNome = sqlUpdNome + "ccn_pymt_grp ='" + cmbNomPayGrp.SelectedValue + "',"
        sqlUpdNome = sqlUpdNome + "ccn_share ='" + cmbNomShare.SelectedValue + "',"
        sqlUpdNome = sqlUpdNome + "ccn_remarks ='" + txtNomRemarks.Text.ToString.Trim.Replace("'", "''") + "',"
        sqlUpdNome = sqlUpdNome + "ccn_nominee_address ='" + txtNomineeAddress.Text.ToString.Trim.Replace("'", "''") + "',"
        sqlUpdNome = sqlUpdNome + "CCN_MODIFIED_BY ='" + Session("VendCode") + "',"
        sqlUpdNome = sqlUpdNome + "CCN_MODIFIED_DT =  Sysdate"

        sqlUpdNome = sqlUpdNome + " where CCN_SAFETY_PASS_NO = '" + vSPNo + "' and ccn_nominee_id ='" + Session("NomineeID") + "' and  ccn_pymt_grp ='" + cmbNomPayGrp.SelectedValue + "' and CCN_REQ_NO='" + Session("requestnumber") + "'"

        Try
            SaveData(sqlUpdNome, con)
            ShowMessage("Nominee Updated Sucessfully")
            nominee_details(vSPNo)
            btnUpdateQual.Visible = True
            GetNominee(TxtSpno.Text)
            Renewal_nominee_details(TxtSpno.Text)
        Catch ex As Exception
            ShowMessage(ex.ToString)
            ShowMessage("error while updating record")
        End Try
    End Sub
#End Region

#Region "Qualification"
    Protected Sub cmbQualType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbQualType.SelectedIndexChanged
        Dim vQualType As String = ""
        vQualType = cmbQualType.SelectedValue
        If vQualType.Equals("ILT") Then
            lblcertname.Visible = False
        Else
            lblcertname.Visible = True
        End If
        FillDropDown(cmbQualification, vQualType)
    End Sub
    Public Sub clearQualification()
        cmbQualType.SelectedValue = 0
        cmbQualification.Items.Clear()
        txtQualRemarks.Text = ""
        btnSaveQual.Enabled = True
        lblcertname.Text = String.Empty
        ' btnUpdateQual.Enabled = False
        btnUpdateQual.Visible = False
        hdqualid.Value = ""
        hdqualcertid.Value = ""
    End Sub
    Public Sub GetQualification(ByVal vSPNo As String)
        '  clearQualification()
        cmbQualification.Items.Clear()
        Dim sqlQuali As String = "Select t1.CQL_QUAL_ID,t1.CQL_CERT_NO,t1.cql_qual_type,t2.ctm_type_desc QUAL_TYPE, t1.cql_qual_code , t3.ctm_type_desc QUAL_NAME, t1.cql_remarks,t4.DM_NAME,t1.CQL_REQ_NO from T_CWM_CEMP_QUALIFICATIONS_TMP t1, T_CEMP_TYPE_MASTER t2, T_CEMP_TYPE_MASTER t3,T_DOCUMENT_MASTER t4 where t1.cql_qual_type = t2.ctm_type_code and t1.cql_qual_code = t3.ctm_type_code(+) and t1.cql_SAFETY_PASS_NO = '" + vSPNo + "' and   t1.cql_comp_code = '" + comp_cd + "' and t4.DM_DOC_ID(+)=t1.CQL_CERT_NO and t4.DM_FILE_TYPE(+)='QUAL' order by t1.CQL_QUAL_ID desc"
        Dim dtQuali As New DataTable()
        dtQuali = getRecord(sqlQuali, con)

        If dtQuali.Rows.Count > 0 Then
            gvQualification.DataSource = dtQuali
            gvQualification.DataBind()
            pnlQualDetail.Visible = True
            If btnSaveQual.Visible = False Then
                btnSaveQual.Visible = True
                btnUpdateQual.Enabled = False
            End If

        Else
            gvQualification.DataSource = Nothing
            gvQualification.DataBind()
            pnlQualDetail.Visible = False
        End If
    End Sub
    Public Function CheckQualificationMandatoryFields() As Integer
        Dim vErrorCount As Integer = 0
        If cmbQualType.SelectedValue = "0" Then
            ErrorRow(tblQualErrorLst, "Select Qualification Type")
        End If

        If cmbQualification.SelectedValue = "0" Then
            ErrorRow(tblQualErrorLst, "Select Qualification")
        End If
        vErrorCount = err_cnt
        Return vErrorCount
    End Function
    Protected Sub btnSaveQual_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveQual.Click
        Dim sqlQual As String = ""
        Dim vQualID As String = ""
        Dim sqlcert As String = ""
        Dim vSPNO As String = TxtSpno.Text.Trim.ToString.ToUpper
        Dim vCertID As String = "0"
        Dim filename As String = String.Empty
        If vSPNO = "" Then
            ShowMessage("Qualification could Not be saved As specifc employee safety pass number was Not found")
            Exit Sub
        End If
        If fupdlqual.HasFile = False Then
            If cmbQualType.SelectedValue <> "ILT" Or cmbQualType.SelectedValue <> "BMAT" Then
                If Session("categorysaf").ToString.Substring(0, 1) <> "S" Then
                Else
                    ShowMessage("Please Upload Qualification Document")
                    Exit Sub
                End If
            End If

            If Session("categorysaf").ToString.Substring(0, 1) = "S" And (cmbQualType.SelectedValue <> "ILT" Or cmbQualType.SelectedValue <> "BMAT") Then
                ShowMessage("Please Upload Qualification Document")
                Exit Sub
            End If
        Else
            filename = Path.GetFileName(fupdlqual.PostedFile.FileName)
            Dim contentType As String = fupdlqual.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdlqual.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdlqual.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If
        Dim vErrorCount As Integer = 0

        vErrorCount = CheckQualificationMandatoryFields()
        If vErrorCount > 0 Then
            tblQualErrorLst.Visible = True

            Exit Sub
        Else
            tblQualErrorLst.Visible = False
        End If

        Dim sql As String = emp_quali_detail_qry(vSPNO) + "And cql_qual_type='" + cmbQualType.SelectedValue + "' and CQL_REQ_NO='" + Session("requestnumber") + "'"
        Dim dt As DataTable = getRecord(sql, con)
        If dt.Rows.Count > 0 Then
            ShowMessage("The type of qualification is already saved for this request")
            Exit Sub
        End If
        'updateprevqualvalidity(vSPNO)
        vQualID = GetID("seq_cemp_qualification")
        If fupdlqual.HasFile = True Then
            vCertID = GetID("seq_cwm_qual_certid")
        Else
            vCertID = "0"
        End If

        sqlQual = "INSERT INTO HRACE.T_CWM_CEMP_QUALIFICATIONS_TMP ( cql_qual_id,CQL_REQ_NO,cql_comp_code, cql_safety_pass_no, cql_qual_type, cql_qual_code,cql_remarks, cql_created_by, cql_created_dt,CQL_CERT_NO) values ('"
        sqlQual = sqlQual + vQualID + "','"
        sqlQual = sqlQual + Session("requestnumber") + "','"
        sqlQual = sqlQual + comp_cd + "','"
        sqlQual = sqlQual + TxtSpno.Text.Trim.ToUpper() + "','"
        sqlQual = sqlQual + cmbQualType.SelectedValue + "','"
        sqlQual = sqlQual + cmbQualification.SelectedValue + "','"
        sqlQual = sqlQual + txtQualRemarks.Text.ToString.Trim.Replace("'", "''") + "','"
        sqlQual = sqlQual + Session("VendCode") + "',"
        sqlQual = sqlQual + "SYSDATE,'"
        sqlQual = sqlQual + vCertID + "')"



        Try

            If fupdlqual.HasFile = True Then
                Dim cmdfileQual As New OracleCommand
                Dim ls_sql As String = String.Empty
                filename = Path.GetFileName(fupdlqual.PostedFile.FileName)
                Using fs As Stream = fupdlqual.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileQual.CommandText = ls_sql
                        cmdfileQual.Connection = con
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_DOC_ID", vCertID))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "QUAL"))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileQual.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileQual.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If
            SaveData(sqlQual, con)
            ' btnSaveQual.Visible = False
            BtnNext.Visible = True
            ShowMessage("Qualification Saved Sucessfully")
            GetQualification(vSPNO)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvQualification.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectQual")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If

            empView()
        Catch ex As Exception
            ShowMessage(ex.ToString)
        End Try

    End Sub
    Private Sub updateprevqualvalidity(ByVal safetypass As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            ls_sql = "select CQL_QUAL_ID,CQL_COMP_CODE from T_CWM_CEMP_QUALIFICATIONS_TMP where CQL_SAFETY_PASS_NO=:CQL_SAFETY_PASS_NO and CQL_COMP_CODE=:CQL_COMP_CODE and CQL_REQ_NO=(select max(CQL_REQ_NO) from T_CWM_CEMP_QUALIFICATIONS_TMP where CQL_SAFETY_PASS_NO=:CQL_SAFETY_PASS_NO and CQL_COMP_CODE=:CQL_COMP_CODE)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CQL_SAFETY_PASS_NO", safetypass))
            cmd.Parameters.Add(New OracleParameter(":CQL_COMP_CODE", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    ls_sql = "update T_CWM_CEMP_QUALIFICATIONS_TMP set CQL_VALIDITY_DATE=sysdate where CQL_QUAL_ID=:CQL_QUAL_ID and CQL_SAFETY_PASS_NO=:CQL_SAFETY_PASS_NO and CQL_COMP_CODE=:CQL_COMP_CODE "
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":CQL_SAFETY_PASS_NO", safetypass))
                    cmd.Parameters.Add(New OracleParameter(":CQL_COMP_CODE", Session("Comp_Code")))
                    cmd.Parameters.Add(New OracleParameter(":CQL_QUAL_ID", dt.Rows(i).Item("CQL_QUAL_ID")))
                    cmd.ExecuteNonQuery()
                    i = i + 1
                End While
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub btnUpdateQual_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateQual.Click
        Dim vQualID As String = ""
        Dim vSPNo As String = TxtSpno.Text.Trim.ToUpper()
        Dim vErrorCount As Integer = 0
        Dim vcertId As String = "0"
        vErrorCount = CheckQualificationMandatoryFields()
        If vErrorCount > 0 Then
            tblQualErrorLst.Visible = True
            Exit Sub
        Else
            tblQualErrorLst.Visible = False
        End If

        If fupdlqual.HasFile = True Then


            Dim contentType As String = fupdlqual.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdlqual.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdlqual.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If



        If cmbQualType.SelectedValue = "0" Then
            ShowMessage("Please Select Qualification Type")
            Exit Sub
        End If
        If cmbQualification.SelectedValue = "0" Or cmbQualification.SelectedValue = "" Then
            ShowMessage("Please Select Qualification")
            Exit Sub
        End If
        If cmbQualType.SelectedValue = "ILT" Or cmbQualType.SelectedValue = "BMAT" Then
            hdqualcertid.Value = "0"

        End If
        'If cmbQualType.SelectedValue <> "ILT" Or cmbQualType.SelectedValue <> "BMAT" Then
        '    If Session("categorysaf").ToString.Substring(0, 1) = "S" Then
        '        ShowMessage("Please upload document")
        '        Exit Sub
        '    End If
        'End If
        If cmbQualType.SelectedValue <> "ILT" And cmbQualType.SelectedValue <> "BMAT" Then
            If fupdlqual.HasFile = False Then
                If hdqualcertid.Value = "0" Or hdqualcertid.Value = "" Then
                    ShowMessage("Please upload document")
                    Exit Sub
                End If
            End If

        End If
        If fupdlqual.HasFile = True Then
            If hdqualcertid.Value = "0" Then
                vcertId = GetID("seq_cwm_qual_certid")
            End If

        End If
        Dim sqlUpdQual As String = ""
        If vcertId = 0 Then
            vcertId = hdqualcertid.Value
        End If
        sqlUpdQual = "update HRACE.T_CWM_CEMP_QUALIFICATIONS_TMP set "
        sqlUpdQual = sqlUpdQual + "CQL_QUAL_TYPE ='" + cmbQualType.SelectedValue + "',"
        sqlUpdQual = sqlUpdQual + "CQL_QUAL_CODE ='" + cmbQualification.SelectedValue + "',"
        sqlUpdQual = sqlUpdQual + "CQL_REMARKS ='" + txtQualRemarks.Text.ToString.Trim.Replace("'", "''") + "',"
        sqlUpdQual = sqlUpdQual + "CQL_MODIFIED_BY ='" + Session("VendCode") + "',"
        sqlUpdQual = sqlUpdQual + "CQL_MODIFIED_DT =  Sysdate,"
        sqlUpdQual = sqlUpdQual + " CQL_CERT_NO='" + vcertId + "'"

        sqlUpdQual = sqlUpdQual + " where cql_safety_pass_no = '" + vSPNo + "' and CQL_QUAL_ID ='" + hdqualid.Value + "'"

        Try
            If fupdlqual.HasFile = True Then
                Dim ls_sql As String = String.Empty
                Dim cmdfileQual As New OracleCommand
                Using fs As Stream = fupdlqual.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))
                        Dim filename As String = Path.GetFileName(fupdlqual.PostedFile.FileName)
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        If hdqualcertid.Value = "0" Or hdqualcertid.Value = "" Then

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If

                            cmdfileQual = New OracleCommand(ls_sql, con)
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_DOC_ID", vcertId))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "QUAL"))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileQual.ExecuteNonQuery()
                        Else
                            ls_sql = "update T_DOCUMENT_MASTER set DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_NAME=:DM_NAME,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='QUAL'"
                            cmdfileQual = New OracleCommand(ls_sql, con)
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_DOC_ID", hdqualcertid.Value))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            'cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileQual.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileQual.ExecuteNonQuery()
                        End If

                    End Using
                End Using
            End If


            SaveData(sqlUpdQual, con)
            ''''''''''''''''''''''''''''''''''check any previous details exist then update status in T_CEMP_DETAIL_TMP table to I'''''''''''''''''''
            Dim ls_chkQP As String = String.Empty
            Dim cmd_chkQP As OracleCommand
            Dim dt_chkQP As New DataTable
            Try
                ls_chkQP = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='QP' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkQP = New OracleCommand(ls_chkQP, con)
                cmd_chkQP.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkQP.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", vSPNo))
                dt_chkQP = getRecord(cmd_chkQP, con)
                If dt_chkQP.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), vSPNo, "QP")
                End If
            Catch ex As Exception

            End Try


            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''


            ShowMessage("Updated Sucessfully")
            vcertId = ""
            'quali_details(vSPNo)
            clearQualification()
            btnUpdateQual.Visible = True
            GetQualification(TxtSpno.Text)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvQualification.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectQual")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If

            If con.State = ConnectionState.Open Then
                con.Close()
            End If
        Catch ex As Exception
            ShowMessage("Error While Updating Record")
        End Try
    End Sub
#End Region
    Protected Sub btnSaveExp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSaveExp.Click
        Dim spno As String = TxtSpno.Text.Trim.ToString()
        Dim ls_sql As String = String.Empty
        Dim convertdt As String = String.Empty
        Dim convertenddt As String = String.Empty

        Dim parsedStartDate As DateTime
        Dim parsedEndDate As DateTime

        Dim dtvalid As String = String.Empty
        Dim cmd As New OracleCommand
        Dim ls_arrlist As New ArrayList
        Dim filename As String = String.Empty
        If FileUploadExp.HasFile = False Then
            'ShowMessage("Please Upload File")
            'Exit Sub
        Else
            filename = Path.GetFileName(FileUploadExp.PostedFile.FileName)
            Dim contentType As String = FileUploadExp.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (FileUploadExp.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (FileUploadExp.PostedFile.ContentLength / 1024).ToString("0.00") + "KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        Try
            If txtcompname.Text.Trim.Equals("") Then
                ShowMessage("Company name should not left blank")
                Exit Sub
            End If
            'If txtexpyr.Text.Trim.Equals("") Then
            '    ShowMessage("Year of experience should not left blank")
            '    Exit Sub
            'End If
            If txtdesignation.Text.Trim.Equals("") Then
                ShowMessage("designation should not left blank")
                Exit Sub
            End If
            If drpexparea.SelectedValue.Equals("--Selected--") Then
                ShowMessage("Please select working area")
                Exit Sub
            End If
            If drpexpstate.SelectedValue.ToString.Equals("--Selected--") Then
                ShowMessage("Please select state")
                Exit Sub
            End If

            If drpexploc.SelectedValue.ToString.Equals("--Selected--") Then
                ShowMessage("Please select city")
                Exit Sub
            End If
            If txtstdt.Text.Trim.Equals("__/__/____") Then
                'lbl_msgExp.Text = "start date shouldn't left blank for row:" & gv.RowIndex + 1
                ShowMessage("Start date should not left blank")
            Else

                convertdt = txtstdt.Text.Trim
                Dim day As String = convertdt.Substring(0, convertdt.IndexOf("/"))
                Dim mon As String = convertdt.Substring(convertdt.IndexOf("/") + 1, 2)
                Dim yr As String = convertdt.Substring(6, 4)
                convertdt = mon + "/" + day + "/" + yr
                'ls_stdt.Text = convertdt

                If Not DateTime.TryParseExact(txtstdt.Text.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, parsedStartDate) Then
                    ShowMessage("Invalid Start Date format. Please use DD/MM/YYYY.")
                    Exit Sub
                End If
            End If
            If txtenddt.Text.Trim.Equals("__/__/____") Then
                txtenddt.Text = ""
            Else

                ' Dim convertdt As String = String.Empty
                convertenddt = txtenddt.Text.Trim
                Dim day As String = convertenddt.Substring(0, convertdt.IndexOf("/"))
                Dim mon As String = convertenddt.Substring(convertdt.IndexOf("/") + 1, 2)
                Dim yr As String = convertenddt.Substring(6, 4)
                convertenddt = mon + "/" + day + "/" + yr
                'ls_enddt.Text = convertdt

                If Not DateTime.TryParseExact(txtenddt.Text.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, parsedEndDate) Then
                    ShowMessage("Invalid End Date format. Please use DD/MM/YYYY.")
                    Exit Sub
                End If
            End If
            If txtenddt.Text.Trim <> "" Then
                dtvalid = checkDateValid(convertenddt, convertdt)
                If dtvalid.Equals("Y") Then

                Else
                    ShowMessage("start date must be less than end date")
                    Exit Sub
                End If
            End If
            If drpexparea.SelectedValue = "EXDM0038" Then
                If txt_otherdom.Text.Trim.Equals("") Then
                    ShowMessage("Please specify other working area")
                    Exit Sub
                End If
            End If

            Dim locCheck = CheckWireFrameLoc()
            Dim minimumRequiredEndDate As DateTime = parsedStartDate.AddYears(2)
            If locCheck Then
                If parsedEndDate < minimumRequiredEndDate Then
                    ShowMessage("Minimum years of experience should be at least 2 years.")
                    Exit Sub
                End If
                If FileUploadExp.HasFile = False Then
                    ShowMessage("Please Upload File")
                    Exit Sub
                End If
            End If

            Dim srl As String = String.Empty
            Dim certid As String = String.Empty
            Dim workdom As String = String.Empty
            srl = getMaxSrl(spno)
            certid = TrnCWEXPSeqNo("")

            ' updateprevexpvalidity(spno)
            ls_sql = "insert into T_CWM_EXP_TMP(CWET_REQ_NO,CWET_SERIAL_NO,CWET_COMP_CODE,CWET_SAFETY_PASS_NO,CWET_COMP_NAME,CWET_ST_DT,CWET_END_DT,CWET_DESIGNATION,CWET_WORKING_AREA,CWET_WORK_LOCATION,CWET_CREATED_BY,CWET_CREATED_ON,CWET_MODIFY_BY,CWET_MODIFY_ON,CWET_CERT_NO) values(:CWET_REQ_NO,:CWET_SERIAL_NO,:CWET_COMP_CODE,:CWET_SAFETY_PASS_NO,:CWET_COMP_NAME,TO_DATE(:CWET_ST_DT,'mm/dd/yyyy'),TO_DATE(:CWET_END_DT,'mm/dd/yyyy'),:CWET_DESIGNATION,:CWET_WORKING_AREA,:CWET_WORK_LOCATION,:CWET_CREATED_BY,sysdate,:CWET_MODIFY_BY,sysdate,:CWET_CERT_NO)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd.Connection = con
            cmd.CommandText = ls_sql
            cmd.Parameters.Add(New OracleParameter(":CWET_REQ_NO", Session("requestnumber")))
            cmd.Parameters.Add(New OracleParameter(":CWET_SERIAL_NO", srl))
            cmd.Parameters.Add(New OracleParameter(":CWET_COMP_CODE", Session("Comp_code")))
            cmd.Parameters.Add(New OracleParameter(":CWET_SAFETY_PASS_NO", spno))
            cmd.Parameters.Add(New OracleParameter(":CWET_COMP_NAME", txtcompname.Text.Trim.ToUpper()))
            cmd.Parameters.Add(New OracleParameter(":CWET_ST_DT", convertdt))

            cmd.Parameters.Add(New OracleParameter(":CWET_END_DT", convertenddt))
            cmd.Parameters.Add(New OracleParameter(":CWET_DESIGNATION", txtdesignation.Text.ToUpper()))

            If drpexparea.SelectedValue = "EXDM0038" Then
                workdom = txt_otherdom.Text.Trim
            Else
                workdom = drpexparea.SelectedValue
            End If
            cmd.Parameters.Add(New OracleParameter(":CWET_WORKING_AREA", workdom.ToUpper()))
            Dim loc As String = drpexpstate.SelectedValue.PadRight(4, " ")
            loc = loc + drpexploc.SelectedValue.PadRight(4, " ")
            cmd.Parameters.Add(New OracleParameter(":CWET_WORK_LOCATION", loc))
            ' cmd.Parameters.Add(New OracleParameter(":CWE_WORK_LOCATION", loc))
            'cmd.Parameters.Add(New OracleParameter(":CWE_CREATED_BY", Session("userid")))
            'cmd.Parameters.Add(New OracleParameter(":CWE_MODIFY_BY", Session("userid")))
            cmd.Parameters.Add(New OracleParameter(":CWET_CREATED_BY", Session("VendCode")))
            cmd.Parameters.Add(New OracleParameter(":CWET_MODIFY_BY", Session("VendCode")))
            cmd.Parameters.Add(New OracleParameter(":CWET_CERT_NO", certid))
            cmd.ExecuteNonQuery()
            cmd.Dispose()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If FileUploadExp.HasFile = True Then
                Dim cmdfileexp As New OracleCommand
                Using fs As Stream = FileUploadExp.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileexp.CommandText = ls_sql
                        cmdfileexp.Connection = con
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_DOC_ID", certid))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "EXP"))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileexp.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileexp.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileexp.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileexp.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If

            ShowMessage("Experience has been added successfully")
            clearexperience()
            GetExp(spno)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In grvExp.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectExp")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If

        Catch ex As Exception
            ShowMessage("Error Occurs")
        End Try
    End Sub
    Private Sub updateprevexpvalidity(ByVal safetypass As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim i As Integer = 0
        Try
            ls_sql = "select CWET_SERIAL_NO,CWET_COMP_CODE from T_CWM_EXP_TMP where CWET_SAFETY_PASS_NO=:CWET_SAFETY_PASS_NO and CWET_COMP_CODE=:CWET_COMP_CODE and CWET_REQ_NO=(select max(CWET_REQ_NO) from T_CWM_EXP_TMP where CWET_SAFETY_PASS_NO=:CWET_SAFETY_PASS_NO and CWET_COMP_CODE=:CWET_COMP_CODE)"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CWET_SAFETY_PASS_NO", safetypass))
            cmd.Parameters.Add(New OracleParameter(":CWET_COMP_CODE", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                While i < dt.Rows.Count
                    ls_sql = "update T_CWM_EXP_TMP set CWET_VALIDITY_DATE=sysdate where CWET_SERIAL_NO=:CWET_SERIAL_NO and CWET_SAFETY_PASS_NO=:CWET_SAFETY_PASS_NO and CWET_COMP_CODE=:CWET_COMP_CODE "
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd = New OracleCommand(ls_sql, con)
                    cmd.Parameters.Add(New OracleParameter(":CWET_SAFETY_PASS_NO", safetypass))
                    cmd.Parameters.Add(New OracleParameter(":CWET_COMP_CODE", Session("Comp_Code")))
                    cmd.Parameters.Add(New OracleParameter(":CWET_SERIAL_NO", dt.Rows(i).Item("CWET_SERIAL_NO")))
                    cmd.ExecuteNonQuery()
                    i = i + 1
                End While
            End If
        Catch ex As Exception

        End Try
    End Sub
    Protected Sub btnUpdateExp_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateExp.Click
        Dim spno As String = TxtSpno.Text.Trim.ToString()
        Dim ls_sql As String = String.Empty
        Dim convertdt As String = String.Empty
        Dim convertenddt As String = String.Empty

        Dim parsedStartDate As DateTime
        Dim parsedEndDate As DateTime

        Dim dtvalid As String = String.Empty
        Dim cmd As New OracleCommand
        Dim ls_arrlist As New ArrayList
        drpexparea.Enabled = True
        drpexploc.Enabled = True
        txtstdt.Enabled = True
        txtenddt.Enabled = True
        txtcompname.Enabled = True
        If lbl_uploadedexp.Text <> "" Then
            lbl_uploadedexp.Text = String.Empty
        End If
        Try
            If txtcompname.Text.Trim.Equals("") Then
                ShowMessage("Company name should not left blank")
                Exit Sub
            End If
            If txtdesignation.Text.Trim.Equals("") Then
                ShowMessage("Designation should not left blank")
                Exit Sub
            End If
            If drpexparea.SelectedValue.ToString.Equals("--Selected--") Then
                ShowMessage("Please select work area")
                Exit Sub
            End If

            If ddlMedCentre.SelectedValue = "0" Then
                ErrorRow(tblProfileErrorList, "Please select Medical Centre")
                Exit Sub
            End If

            If drpexpstate.SelectedValue.ToString.Equals("--Selected--") Then
                ShowMessage("Please select state")
                Exit Sub
            End If
            If drpexploc.SelectedValue.ToString.Equals("--Selected--") Then
                ShowMessage("Please select city")
                Exit Sub
            End If
            If txtstdt.Text.Trim.Equals("__/__/____") Then
                'lbl_msgExp.Text = "start date shouldn't left blank for row:" & gv.RowIndex + 1
                ShowMessage("Start date should not left blank")
            Else

                convertdt = txtstdt.Text.Trim
                Dim day As String = convertdt.Substring(0, convertdt.IndexOf("/"))
                Dim mon As String = convertdt.Substring(convertdt.IndexOf("/") + 1, 2)
                Dim yr As String = convertdt.Substring(6, 4)
                convertdt = mon + "/" + day + "/" + yr
                'ls_stdt.Text = convertdt

                If Not DateTime.TryParseExact(txtstdt.Text.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, parsedStartDate) Then
                    ShowMessage("Invalid Start Date format. Please use DD/MM/YYYY.")
                    Exit Sub
                End If
            End If
            If txtenddt.Text.Trim.Equals("__/__/____") Or txtenddt.Text.Trim.Equals("") Then
                txtenddt.Text = ""
            Else

                ' Dim convertdt As String = String.Empty
                convertenddt = txtenddt.Text.Trim
                Dim day As String = convertenddt.Substring(0, convertdt.IndexOf("/"))
                Dim mon As String = convertenddt.Substring(convertdt.IndexOf("/") + 1, 2)
                Dim yr As String = convertenddt.Substring(6, 4)
                convertenddt = mon + "/" + day + "/" + yr
                'ls_enddt.Text = convertdt

                If Not DateTime.TryParseExact(txtenddt.Text.Trim, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, parsedEndDate) Then
                    ShowMessage("Invalid End Date format. Please use DD/MM/YYYY.")
                    Exit Sub
                End If
            End If
            If txtenddt.Text.Trim <> "" Then
                dtvalid = checkDateValid(convertenddt, convertdt)
                If dtvalid.Equals("Y") Then

                Else
                    ShowMessage("start date must be less than end date")
                    Exit Sub
                End If
            End If

            Dim locCheck = CheckWireFrameLoc()
            Dim minimumRequiredEndDate As DateTime = parsedStartDate.AddYears(2)
            If locCheck Then
                If parsedEndDate < minimumRequiredEndDate Then
                    ShowMessage("Minimum years of experience should be at least 2 years.")
                    Exit Sub
                End If
                If FileUploadExp.HasFile = False Then
                    ShowMessage("Please Upload File")
                    Exit Sub
                End If
            End If

            'Dim srl As String = String.Empty
            'srl = getMaxSrl(spno)
            ls_sql = "update  T_CWM_EXP_TMP set CWET_COMP_CODE=:CWET_COMP_CODE,CWET_COMP_NAME=:CWET_COMP_NAME,CWET_ST_DT=TO_DATE(:CWET_ST_DT,'mm/dd/yyyy'),CWET_END_DT=TO_DATE(:CWET_END_DT,'mm/dd/yyyy'),CWET_DESIGNATION=:CWET_DESIGNATION,CWET_WORKING_AREA=:CWET_WORKING_AREA,CWET_WORK_LOCATION=:CWET_WORK_LOCATION,CWET_MODIFY_BY=:CWET_MODIFY_BY,CWET_MODIFY_ON=sysdate where CWET_REQ_NO=:CWET_REQ_NO and CWET_SERIAL_NO=:CWET_SERIAL_NO and CWET_COMP_CODE=:CWET_COMP_CODE and CWET_SAFETY_PASS_NO=:CWET_SAFETY_PASS_NO and CWET_CERT_NO=:CWET_CERT_NO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd.Connection = con
            cmd.CommandText = ls_sql
            Dim loc As String = drpexpstate.SelectedValue.PadRight(4, " ")
            loc = loc + drpexploc.SelectedValue.PadRight(4, " ")

            cmd.Parameters.Add(New OracleParameter(":CWET_REQ_NO", Session("requestnumber")))
            cmd.Parameters.Add(New OracleParameter(":CWET_SERIAL_NO", Session("ExpID")))
            cmd.Parameters.Add(New OracleParameter(":CWET_COMP_CODE", Session("Comp_code")))
            cmd.Parameters.Add(New OracleParameter(":CWET_SAFETY_PASS_NO", hidexpsafety.Value))
            cmd.Parameters.Add(New OracleParameter(":CWET_COMP_NAME", txtcompname.Text.Trim.ToUpper()))
            cmd.Parameters.Add(New OracleParameter(":CWET_ST_DT", convertdt))
            cmd.Parameters.Add(New OracleParameter(":CWET_END_DT", convertenddt))
            cmd.Parameters.Add(New OracleParameter(":CWET_DESIGNATION", txtdesignation.Text.ToUpper()))
            'cmd.Parameters.Add(New OracleParameter(":CWE_MODIFY_BY", Session("userid")))
            cmd.Parameters.Add(New OracleParameter(":CWET_MODIFY_BY", Session("VendCode")))
            cmd.Parameters.Add(New OracleParameter(":CWET_CERT_NO", hidcertno.Value))
            cmd.Parameters.Add(New OracleParameter(":CWET_WORK_LOCATION", loc))
            If drpexparea.SelectedValue = "EXDM0038" Then

                cmd.Parameters.Add(New OracleParameter(":CWET_WORKING_AREA", txt_otherdom.Text.Trim.ToUpper()))
            Else
                cmd.Parameters.Add(New OracleParameter(":CWET_WORKING_AREA", drpexparea.SelectedValue))
            End If

            cmd.ExecuteNonQuery()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            If FileUploadExp.HasFile = True Then
                If FileUploadExp.HasFile = False Then
                    ShowMessage("Please Upload File")
                    Exit Sub
                End If
                Dim filename As String = Path.GetFileName(FileUploadExp.PostedFile.FileName)
                Dim contentType As String = FileUploadExp.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (FileUploadExp.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (FileUploadExp.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
                Dim cmdfileexp As New OracleCommand
                Using fs As Stream = FileUploadExp.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))
                        ls_sql = "select DM_NAME from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID"
                        Dim cmdchkfile As New OracleCommand
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmdchkfile.Connection = con
                        cmdchkfile.CommandText = ls_sql
                        cmdchkfile.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidcertno.Value))
                        Dim da = New OracleDataAdapter(cmdchkfile)
                        Dim dtfilechk As New DataTable
                        da.Fill(dtfilechk)
                        If dtfilechk.Rows.Count = 0 Then
                            ''''''''''''''''''add code'''''''''''''''''''''
                            Dim cmdfileexpchk As New OracleCommand

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileexpchk.CommandText = ls_sql
                            cmdfileexpchk.Connection = con
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidcertno.Value))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "EXP"))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            'cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            cmdfileexpchk.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileexpchk.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        Else

                            ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileexp.CommandText = ls_sql
                            cmdfileexp.Connection = con
                            cmdfileexp.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidcertno.Value))
                            cmdfileexp.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                            cmdfileexp.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            'cmdfileexp.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfileexp.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileexp.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End If
                    End Using
                End Using
            End If
            Dim ls_chkEX As String = String.Empty
            Dim cmd_chkEX As OracleCommand
            Dim dt_chkEX As New DataTable
            Try
                ls_chkEX = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='EX' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkEX = New OracleCommand(ls_chkEX, con)
                cmd_chkEX.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkEX.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", spno))
                dt_chkEX = getRecord(cmd_chkEX, con)
                If dt_chkEX.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), spno, "EX")
                End If
            Catch ex As Exception

            End Try


            ShowMessage("Experience has been updated successfully")
            clearexperience()
            GetExp(spno)

            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In grvExp.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectExp")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If


        Catch ex As Exception
            ShowMessage("Error Occurs")
        End Try

    End Sub
    Protected Sub btnUpdateTraining_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateTraining.Click

        Dim vTrainingID As String = ""
        Dim vSPNo As String = TxtSpno.Text.Trim.ToUpper()
        Dim vErrorCount As Integer = 0
        Dim vStartDt As String = ""
        Dim vEndDt As String = ""
        Dim sqlUpdTrn As String = String.Empty
        Dim trncertid As String = String.Empty

        vStartDt = txtTrnStartDt.Text.Trim()
        'vEndDt = txtTrnEndDt.Text.Trim()





        vTrainingID = Session("TrainingID")

        If cmbTrnAgency.SelectedValue = "0" Then
            ShowMessage("Please Select Agency")
            ' mpAddTraining.Show()
            Exit Sub
        End If
        If cmbTrnLoc.SelectedValue = "0" Then
            ShowMessage("Please Select Location")
            ' mpAddTraining.Show()
            Exit Sub
        End If
        If cmbTraningType.SelectedValue = "0" Then
            ShowMessage("Please Select Training Type")
            ' mpAddTraining.Show()
            Exit Sub
        End If
        If cmbTrnCource.SelectedValue = "0" Then
            ShowMessage("Please Select Training Course")
            ' mpAddTraining.Show()
            Exit Sub
        End If
        If cmbTrnResult.SelectedValue = "0" Then
            ShowMessage("Please Select Training Result")
            ' mpAddTraining.Show()
            Exit Sub
        End If



        Try
            If hidcertrnnoTrns.Value = "0" Or hidcertrnnoTrns.Value = "" Then
                If fileuploadtrn.HasFile = True Then
                    If (fileuploadtrn.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (fileuploadtrn.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                    trncertid = TrnCWETrnSeqNo("")
                Else
                    trncertid = "0"
                End If
            Else
                trncertid = hidcertrnnoTrns.Value
            End If

            Dim cmdupdatecert As New OracleCommand
            Dim dt As New DataTable()
            sqlUpdTrn = "Update T_CWM_CEMP_Trns_TMP set CCTT_TRN_AGENCY=:CCTT_TRN_AGENCY,CCTT_TRN_LOC=:CCTT_TRN_LOC,CCTT_TRN_TYPE=:CCTT_TRN_TYPE,CCTT_COURSE_CD=:CCTT_COURSE_CD,CCTT_START_DT=TO_DATE(:CCTT_START_DT,'DD/MM/YYYY'),CCTT_END_DT=TO_DATE(:CCTT_END_DT,'DD/MM/YYYY'),CCTT_RESULT=:CCTT_RESULT,CCTT_MODIFIED_BY=:CCTT_MODIFIED_BY,CCTT_MODIFIED_DT=sysdate,CCTT_REMARKS=:CCTT_REMARKS,CCTT_CERT_NO=:CCTT_CERT_NO where CCTT_SAFETY_PASS_NO=:CCTT_SAFETY_PASS_NO and CCTT_TRN_ID=:CCTT_TRN_ID"


            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmdupdatecert.Connection = con
            cmdupdatecert.CommandText = sqlUpdTrn
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_TRN_AGENCY", cmbTrnAgency.SelectedValue))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_TRN_LOC", cmbTrnLoc.SelectedValue))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_TRN_Type", cmbTraningType.SelectedValue))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_COURSE_CD", cmbTrnCource.SelectedValue))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_START_DT", txtTrnStartDt.Text))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_RESULT", cmbTrnResult.SelectedValue))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_END_DT", txtTrnEndDt.Text))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_REMARKS", txtTrnRemarks.Text.ToString.Trim.Replace("'", "''")))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_MODIFIED_BY", Session("VendCode")))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_SAFETY_PASS_NO", TxtSpno.Text.ToString.Trim().ToUpper))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_TRN_ID", vTrainingID))
            cmdupdatecert.Parameters.Add(New OracleParameter(":CCTT_CERT_NO", trncertid))
            cmdupdatecert.ExecuteNonQuery()
            If con.State = ConnectionState.Open Then
                con.Close()
            End If





            '''''''''''''''''''''''''''''update file attachment for skill ''''''''''''''
            If fileuploadtrn.HasFile = True Then
                If fileuploadtrn.HasFile = False Then
                    ShowMessage("Please Upload File")
                    Exit Sub
                End If
                Dim filename As String = Path.GetFileName(fileuploadtrn.PostedFile.FileName)
                Dim contentType As String = fileuploadtrn.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fileuploadtrn.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (fileuploadtrn.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
                Dim ls_sql As String = String.Empty
                Dim cmdfiletrn As New OracleCommand
                Using fs As Stream = fileuploadtrn.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                        filename = Path.GetFileName(fileuploadtrn.PostedFile.FileName)
                        If hidcertrnnoTrns.Value.Trim = "" Or hidcertrnnoTrns.Value.Trim = "0" Then

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If

                            cmdfiletrn = New OracleCommand(ls_sql, con)
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_DOC_ID", trncertid))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "TRN"))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfiletrn.ExecuteNonQuery()
                        Else
                            ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfiletrn = New OracleCommand(ls_sql, con)
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidcertrnnoTrns.Value))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                            cmdfiletrn.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfiletrn.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If

                        End If

                    End Using
                End Using

            End If





            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''




            hidcertrnnoTrns.Value = String.Empty
            Dim ls_chkTR As String = String.Empty
            Dim cmd_chkTR As OracleCommand
            Dim dt_chkTR As New DataTable
            Try
                ls_chkTR = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='TR' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkTR = New OracleCommand(ls_chkTR, con)
                cmd_chkTR.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkTR.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", vSPNo))
                dt_chkTR = getRecord(cmd_chkTR, con)
                If dt_chkTR.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), vSPNo, "TR")
                End If
            Catch ex As Exception

            End Try


            'btnSearch_Click(sender, e)
            'GetExp(vSPNo)
            GetTraining(vSPNo)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In gvTraining.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectTraining")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If

            clearTraining()
            ShowMessage("Training has been updated successfully")
            btnUpdateTraining.Enabled = False
            btnUpdateTraining.Visible = False
            btnSaveTraining.Visible = True
        Catch ex As Exception

        End Try

    End Sub
    Public Function TrnCWEXPSeqNo(ByVal id As String) As String
        Dim vExpSeqNo As String = ""
        Dim sqlExpSeqNo As String = "select (HRACE.SEQ_CWM_EXP.nextval) SEQNO from dual "
        Dim dtExpSeqNo As New DataTable()
        dtExpSeqNo = getRecord(sqlExpSeqNo, con)
        If dtExpSeqNo.Rows.Count > 0 Then
            vExpSeqNo = dtExpSeqNo.Rows(0)("SEQNO")
        End If

        dtExpSeqNo.Dispose()
        Return vExpSeqNo

    End Function
    Private Function checkDateValid(ByVal enddt As String, ByVal stdt As String) As String
        Dim valid As String = String.Empty
        If enddt.Substring(0, 2).ToString.Trim = stdt.Substring(0, 2).ToString.Trim Then
            If enddt.Substring(6, 4).ToString.Trim = stdt.Substring(6, 4).ToString.Trim Then
                If enddt.Substring(3, 2).ToString.Trim > stdt.Substring(3, 2).ToString.Trim Then
                    valid = "Y"
                Else
                    valid = "N"
                End If
            ElseIf enddt.Substring(6, 4).ToString.Trim < stdt.Substring(6, 4).ToString.Trim Then
                valid = "N"
            Else
                valid = "Y"
            End If
        ElseIf enddt.Substring(0, 2).ToString.Trim < stdt.Substring(0, 2).ToString.Trim Then
            If enddt.Substring(6, 4).ToString.Trim <= stdt.Substring(6, 4).ToString.Trim Then
                valid = "N"
            Else
                valid = "Y"
            End If
        ElseIf enddt.Substring(0, 2).ToString.Trim > stdt.Substring(0, 2).ToString.Trim Then
            If enddt.Substring(6, 4).ToString.Trim < stdt.Substring(6, 4).ToString.Trim Then
                valid = "N"
            Else
                valid = "Y"
            End If
        End If
        Return valid
    End Function
    Private Function getMaxSrl(ByVal spno As String) As String
        Dim srl As String = String.Empty
        Dim ls_sql As String = String.Empty
        Dim dt As New DataTable
        Try
            ls_sql = "select max(CWET_SERIAL_NO)+1 srlno from t_CWM_EXP_TMP where CWET_SAFETY_PASS_NO='" + spno + "'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt = getRecord(ls_sql, con)
            If dt.Rows.Count > 0 Then
                If dt.Rows(0).Item(0).ToString.Trim = "" Then
                    srl = "1"
                Else
                    srl = dt.Rows(0).Item(0).ToString.Trim

                End If
            End If

        Catch ex As Exception

        End Try
        Return srl

    End Function
    Private Sub getExpLoc(ByVal state As String)
        Dim ls_sql As String = String.Empty
        Dim dt As New DataTable
        Dim cmd As New OracleCommand
        Try
            ls_sql = "select CIT_CITY_CODE,CIT_CITY_NAME from T_CITY_MASTER where CIT_STATE_CODE=:CIT_STATE_CODE"

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd.Connection = con
            cmd.CommandText = ls_sql
            cmd.Parameters.Add(New OracleParameter(":CIT_STATE_CODE", state))
            Dim da = New OracleDataAdapter(cmd)
            da.Fill(dt)
            If dt.Rows.Count > 0 Then
                drpexploc.DataSource = dt
                drpexploc.DataTextField = "CIT_CITY_NAME"
                drpexploc.DataValueField = "CIT_CITY_CODE"
                drpexploc.DataBind()
            Else
                ls_sql = "select CIT_CITY_CODE,CIT_CITY_NAME from T_CITY_MASTER where CIT_STATE_CODE='JH'"
                dt = getRecord(ls_sql, con)
                If dt.Rows.Count > 0 Then
                    drpexploc.DataSource = dt
                    drpexploc.DataTextField = "CIT_CITY_NAME"
                    drpexploc.DataValueField = "CIT_CITY_CODE"
                    drpexploc.DataBind()
                    drpexpstate.SelectedValue = "JH"
                End If
            End If

            drpexploc.Items.Insert(0, "--Selected--")
        Catch ex As Exception

        End Try

    End Sub
    Private Sub clearexperience()
        drpexpstate.Enabled = True
        txt_otherdom.Text = ""
        txt_otherdom.Visible = False
        txtcompname.Text = ""
        txtexpyr.Text = ""
        txtstdt.Text = ""
        txtenddt.Text = ""
        txtdesignation.Text = ""
        getExpDom()
        getExpLoc("")
        drpexparea.Enabled = True
        drpexploc.Enabled = True
        txtstdt.Enabled = True
        txtenddt.Enabled = True
        txtcompname.Enabled = True
        txtdesignation.Enabled = True
        If FileUploadExp.Enabled = False Then
            FileUploadExp.Enabled = True
        End If
        lbl_uploadedexp.Text = String.Empty




    End Sub
    Protected Sub drpexparea_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpexparea.SelectedIndexChanged
        If drpexparea.SelectedValue = "EXDM0038" Then
            txt_otherdom.Visible = True
            If txt_otherdom.Enabled = False Then
                txt_otherdom.Enabled = True
            End If
        Else
            txt_otherdom.Visible = False
        End If
    End Sub
    Private Sub GetExp(ByVal vspno As String)
        Dim ls_sql As String = String.Empty
        Dim dt As New DataTable
        Dim cmd As OracleCommand

        getExpDom()
        getExpLocState()



        ls_sql = "select d.CWET_CERT_NO,DM_NAME,d.CWET_SAFETY_PASS_NO,d.CWET_SERIAL_NO,UPPER(CWET_COMP_NAME) CWET_COMP_NAME,CWET_EXP_YR,TO_CHAR(d.CWET_ST_DT,'dd/mm/yyyy') stdt,TO_CHAR(d.CWET_END_DT,'dd/mm/yyyy') enddt,UPPER(d.CWET_DESIGNATION) CWET_DESIGNATION,NVL(UPPER(a.CTM_TYPE_DESC),d.CWET_WORKING_AREA) domain,d.CWET_WORKING_AREA workarea, d.CWET_WORK_LOCATION ,UPPER(b.CIT_CITY_NAME) area, CWET_WORK_LOCATION,d.CWET_REQ_NO from T_CWM_EXP_TMP d,t_DOCUMENT_MASTER,t_CEMP_TYPE_MASTER a,t_CITY_MASTER b where  d.CWET_CERT_NO=DM_DOC_ID(+) and d.CWET_WORKING_AREA=a.CTM_TYPE_CODE(+) and trim(substr(d.CWET_WORK_LOCATION,4,4))=b.CIT_CITY_CODE and d.CWET_SAFETY_PASS_NO='" + vspno + "' and a.CTM_TYPE(+)='EXDM' and trim(substr(d.CWET_WORK_LOCATION,0,4))=b.CIT_STATE_CODE order by d.CWET_SERIAL_NO "
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmd = New OracleCommand(ls_sql, con)


        dt = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            grvExp.DataSource = dt
            grvExp.DataBind()
            btnSaveExp.Visible = True
            btnSaveExp.Enabled = True
            btnUpdateExp.Enabled = True
            btnUpdateExp.Visible = True

        Else
            btnSaveExp.Visible = True
            btnSaveExp.Enabled = True
            'btnUpdateExp.Enabled = False
            btnUpdateExp.Visible = False
            'ShowMessage("No record found")
            grvExp.DataSource = Nothing
            grvExp.DataBind()

        End If

        clearexperience()






    End Sub
    Private Sub getExpDom()
        Dim ls_sql As String = String.Empty
        Dim dt As New DataTable
        Try
            ls_sql = "select CTM_TYPE_CODE,CTM_TYPE_DESC from t_cemp_type_master where CTM_TYPE='EXDM' and CTM_STATUS='A' order by CTM_TYPE_DESC"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt = getRecord(ls_sql, con)
            If dt.Rows.Count > 0 Then
                drpexparea.DataSource = dt
                drpexparea.DataTextField = "CTM_TYPE_DESC"
                drpexparea.DataValueField = "CTM_TYPE_CODE"
                drpexparea.DataBind()
            End If
            drpexparea.Items.Insert(0, "--Selected--")
        Catch ex As Exception

        End Try

    End Sub
    Protected Sub drpexpstate_OnselectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles drpexpstate.SelectedIndexChanged

        getExpLoc(drpexpstate.SelectedValue)
    End Sub
    Private Sub getExpLocState()
        Dim ls_sql As String = String.Empty
        Dim dt As New DataTable
        Try
            ls_sql = "select SMT_STATE_CODE,SMT_STATE_NAME from t_STATE_MASTER"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            dt = getRecord(ls_sql, con)
            If dt.Rows.Count > 0 Then
                drpexpstate.DataSource = dt
                drpexpstate.DataTextField = "SMT_STATE_NAME"
                drpexpstate.DataValueField = "SMT_STATE_CODE"
                drpexpstate.DataBind()
            End If
            drpexparea.Items.Insert("--Select--", 0)
        Catch ex As Exception

        End Try

    End Sub

    'Protected Sub tabcontainer1_ActiveTabChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabcontainer1.ActiveTabChanged
    '    If (tabcontainer1.ActiveTabIndex = 0) Then
    '        cmbCategory.Focus()
    '    ElseIf (tabcontainer1.ActiveTabIndex = 1) Then

    '        cmbAddressType.Focus()
    '    ElseIf (tabcontainer1.ActiveTabIndex = 2) Then
    '        cmbQualType.Focus()
    '    ElseIf (tabcontainer1.ActiveTabIndex = 3) Then
    '        cmbNomRelation.Focus()

    '    End If

    'End Sub
    Protected Sub tabcontainer1_ActiveTabChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabcontainer1.ActiveTabChanged
        If (tabcontainer1.ActiveTabIndex = 0) Then
            cmbCategory.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 1) Then
            cmbSkSkillType.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 2) Then
            cmbAddressType.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 3) Then
            fupdlage.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 4) Then
            cmbQualType.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 5) Then
            txtcompname.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 6) Then
            cmbTrnAgency.Focus()
        ElseIf (tabcontainer1.ActiveTabIndex = 7) Then
            fupdlfitnesscer.Focus()

        ElseIf (tabcontainer1.ActiveTabIndex = 8) Then
            cmbNomRelation.Focus()

        End If

    End Sub

    '#Region "PHOTO"


    '    Protected Sub btnShowPhoto_Click(ByVal sender As Object, ByVal e As System.EventArgs)
    '        Dim vSPNo As String = ""
    '        vSPNo = TxtSpno.Text.Trim.ToUpper()

    '        If vSPNo = "" Then
    '            ShowMessage(" Choose the safety Pass number to uplaod the Photo")
    '            Exit Sub
    '        End If

    '        If FileUpload.HasFile Then
    '            Dim imgContentType As String = ""
    '            Dim imglength As Long = 0
    '            imgContentType = FileUpload.PostedFile.ContentType
    '            imglength = FileUpload.PostedFile.ContentLength
    '            If imglength >= 102400 Then
    '                ShowMessage(" File size should be less than 100 Kb")
    '                Exit Sub
    '            End If
    '        End If


    '        Dim dtEmpPhoto As DataTable = photo(vSPNo)

    '        If dtEmpPhoto.Rows.Count > 0 Then
    '            If Not IsDBNull(dtEmpPhoto.Rows(0)("CTP_PHOTO")) Then
    '                imgEmpPhoto.ImageUrl = "~/imgHandler.ashx?SPNO=" & vSPNo
    '            ElseIf IsDBNull(dtEmpPhoto.Rows(0)("CTP_PHOTO")) Then

    '            End If
    '        Else

    '            If (FileUpload.HasFile) Then
    '                Dim file_stream As Stream = FileUpload.PostedFile.InputStream
    '                Dim file_length As Integer = FileUpload.PostedFile.ContentLength
    '                Dim file_type As String = FileUpload.PostedFile.ContentType
    '                Dim full_file_name() As String = Split(FileUpload.PostedFile.FileName, "\")
    '                Dim file_name As String = full_file_name(UBound(full_file_name))
    '                Dim File_content(file_length) As Byte
    '                Dim file_status As Integer = file_stream.Read(File_content, 0, file_length)
    '                Dim file_cmd As New OracleCommand()


    '                Dim insert_qry As String = " INSERT INTO T_CEMP_PHOTP_TMP (CTP_SPNO,CTP_PHOTO,CTP_CREATED_BY,CTP_CREATED_DATE) VALUES('" + vSPNo + "',:File_content,'" + vVencode + "',SYSDATE)"

    '                file_cmd.CommandText = insert_qry
    '                file_cmd.Connection = con
    '                file_cmd.CommandType = CommandType.Text

    '                Dim file As New OracleParameter("File_content", OracleType.Blob, File_content.Length)
    '                file.Value = File_content
    '                file.Direction = ParameterDirection.Input
    '                file_cmd.Parameters.Add(file)

    '                con.Open()
    '                file_cmd.ExecuteNonQuery()
    '                con.Close()

    '                ShowMessage(" Image Got uploaded")
    '            End If

    '            If dtEmpPhoto.Rows.Count > 0 Then
    '                If Not IsDBNull(dtEmpPhoto.Rows(0)("CTP_PHOTO")) Then
    '                    imgEmpPhoto.ImageUrl = "~/imgHandler.ashx?SPNO=" & vSPNo
    '                ElseIf IsDBNull(dtEmpPhoto.Rows(0)("CTP_PHOTO")) Then
    '                End If
    '            End If

    '        End If


    '    End Sub

    '    Public Function photo(ByVal vSPNo As String) As DataTable
    '        Dim sqlPhoto As String = "select CTP_PHOTO from t_cemp_photp_tmp t7 where trim(CTP_SPNO)='" + vSPNo.Trim + "' "
    '        Dim dtEmpPhoto As New DataTable()
    '        dtEmpPhoto = getRecord(sqlPhoto, con)
    '        Return dtEmpPhoto
    '    End Function


    '    Public Sub show_photo(ByVal vSPNo As String)

    '        Dim dtEmpPhoto As DataTable = photo(vSPNo)

    '        If dtEmpPhoto.Rows.Count > 0 Then
    '            If Not IsDBNull(dtEmpPhoto.Rows(0)("CTP_PHOTO")) Then
    '                imgEmpPhoto.ImageUrl = "~/imgHandler.ashx?SPNO=" & vSPNo
    '            ElseIf IsDBNull(dtEmpPhoto.Rows(0)("CTP_PHOTO")) Then

    '            End If
    '        End If
    '    End Sub
    '#End Region
    Protected Sub BtnNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnNext.Click

        tabcontainer1.ActiveTabIndex = tabcontainer1.ActiveTabIndex + 1

    End Sub
    Protected Sub BtnPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles BtnPrev.Click
        tabcontainer1.ActiveTabIndex = tabcontainer1.ActiveTabIndex - 1
    End Sub
    Protected Sub GridViewEmp_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles GridViewEmp.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            If TryCast(e.Row.FindControl("lbl_stat"), LinkButton).Text = msg_complete Then
                TryCast(e.Row.FindControl("BtnPrint"), Button).Enabled = True
            Else
                TryCast(e.Row.FindControl("BtnPrint"), Button).Enabled = False
            End If


        End If
    End Sub
    ''' <summary>
    ''' Added by Priyaraj on 29th Feb,2024 for Profile status linkbutton function logic for showing the incomplete tab
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>

    Protected Sub emp_profile_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim db As New DBConnection
        Dim qry As String = String.Empty
        Dim qry1 As String = String.Empty
        Dim req_no As String = String.Empty
        Dim parameters As OracleParameter()
        req_no = lblreq.Text.Split(":")(1)
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dtfile As DataTable

        qry = ""
        qry = "select CET_PROFILE_STATUS,CET_SAFETY_PASSNO,CET_DOB_CERT_NO,CET_LOCATION_CODE,CET_DEPT_CODE from hrace.T_CEMP_DETAILS_TMP "
        qry += "where CET_REQUEST_NO=:reqno"
        parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no)
                    }
        dtfile = db.GetDataFromQuery(qry, parameters)
        If dtfile.Rows.Count > 0 Then
            Dim passno As String = dtfile.Rows(0)("CET_SAFETY_PASSNO").ToString()
            Dim cerno As String = dtfile.Rows(0)("CET_DOB_CERT_NO").ToString()
            Dim locationcd As String = dtfile.Rows(0)("CET_LOCATION_CODE").ToString()
            Dim deptcd As String = dtfile.Rows(0)("CET_DEPT_CODE").ToString()
            If dtfile.Rows(0)("CET_PROFILE_STATUS").ToString() = "I" Then
                Try
                    qry = ""                                           'For personal info tab
                    qry = "select * from hrace.T_CEMP_DETAILS_TMP "
                    qry += "where CET_REQUEST_NO=:reqno "
                    qry += "and CET_SAFETY_PASSNO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 0)
                        ShowMessage("Incomplete at personal Info Tab, please fill up")
                        Exit Try
                    End If

                    qry = ""                                                    'For address info tab
                    qry = "select * from hrace.T_CWM_CEMP_ADDRS_TMP "
                    qry += "where CCA_REQ_NO=:reqno "
                    qry += "and CCA_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 2)
                        ShowMessage("Incomplete at Address Tab, please fill up")
                        Exit Try
                    End If

                    qry = ""                                                    'For skill info tab
                    qry = "select * from hrace.t_cwm_cemp_skill_TMP "
                    qry += "where CCST_REQ_NO=:reqno "
                    qry += "and CCST_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 1)
                        ShowMessage("Incomplete at skill Tab, please fill up")
                        Exit Try
                    End If


                    If cerno = "0" Then                                        'For age proof tab
                        lnk_Renew_spno_Click1(passno, 3)
                        ShowMessage("Incomplete at Age Proof and Others Tab , please fill up")
                        Exit Try
                    End If


                    qry = ""                                                    'For Qualification info tab
                    qry = "select * from hrace.T_CWM_CEMP_QUALIFICATIONS_TMP "
                    qry += "where CQL_REQ_NO=:reqno "
                    qry += "and CQL_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 4)
                        ShowMessage("Incomplete at Qualification Tab, please fill up")
                        Exit Try
                    End If


                    qry = ""                                                    'For Experience info tab
                    qry = "select CWET_SAFETY_PASS_NO from hrace.t_cwm_exp_tmp "
                    qry += "where CWET_REQ_NO=:reqno "
                    qry += "and CWET_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 5)
                        ShowMessage("Incomplete at Experience Tab, please fill up")
                        Exit Try
                    End If


                    qry = ""                                                    'For Nominee info tab
                    qry = "select * from hrace.T_CWM_CEMP_NOMINEES_TMP "
                    qry += "where CCN_REQ_NO=:reqno "
                    qry += "and CCN_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 8)
                        ShowMessage("Incomplete at Nominee Tab, please fill up")
                        Exit Try
                    End If


                    qry = ""                                                    'For consent info tab
                    qry = "select * from hrace.t_cemp_piiconsent_details "
                    qry += "where CND_SAFETYPASS_NUM=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        lnk_Renew_spno_Click1(passno, 10)
                        ShowMessage("Incomplete at Consent Tab, please fill up")
                        Exit Try
                    End If



                    If (locationcd = "1000" And deptcd = "502") Then                  'For location checking where skill assessment is not mandatory
                        Exit Try
                    End If


                    qry = ""
                    qry = "select ACM_COMPANY_CODE,ACM_CATEGORY from hrace.T_CWM_ACTION_MAPPING "
                    qry += "where ACM_TYPE='RESNAIRIS' "
                    qry += "and ACM_FLAG='Y' "
                    qry += "and ACM_COMPANY_CODE=:locd "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("locd", locationcd)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count > 0 Then
                        Exit Try
                    End If



                    qry = ""
                    qry = "select CCST_SKTD_CP_CD from hrace.T_CWM_CEMP_SKILL_TMP "
                    qry += "where CCST_SAFETY_PASS_NO=: passno "
                    qry += "and CCST_REQ_NO=:reqno "
                    qry += "and CCST_CREATED_DT = (select max(CCST_CREATED_DT) from T_CWM_CEMP_SKILL_TMP where CCST_SAFETY_PASS_NO =:passno and CCST_REQ_NO=:reqno)"

                    parameters = New OracleParameter() _
                {
                  New OracleParameter("passno", passno),
                  New OracleParameter("reqno", req_no)
                }

                    dt = db.GetDataFromQuery(qry, parameters)

                    qry1 = ""
                    qry1 = "select TCD_CLM_SKILL_CD,TCD_CERT_CATEG from t_td_clm_doc@ace_iris "
                    qry1 += "where TCD_SP_NO=:passno "
                    qry1 += "and TCD_CREATE_DT=(select max(TCD_CREATE_DT) from hrps.t_td_clm_doc@ace_iris where TCD_SP_NO=:passno)"

                    parameters = New OracleParameter() _
                {
                  New OracleParameter("passno", passno)
                }

                    dt1 = db.GetDataFromQuery(qry1, parameters)
                    If dt.Rows.Count > 0 And dt1.Rows.Count > 0 Then
                        Dim tradeCLM As String = dt.Rows(0).Item("CCST_SKTD_CP_CD")
                        Dim tradeIRIS As String = dt1.Rows(0).Item("TCD_CLM_SKILL_CD")
                        If tradeCLM <> tradeIRIS Then                                     'for trade code checking.
                            ShowMessage("Selected trade is not matched with your skill assessment result. Please contact with respective skill assessment agency.")
                            Exit Try
                        End If
                    ElseIf dt1.Rows.Count = 0 Then
                        ShowMessage("Skill assessment result is pending, please contact with skill assessment agency for pending result.")
                        Exit Try

                    ElseIf (dt1.Rows(0).Item("TCD_CERT_CATEG").ToString = "FAIL") Or (dt1.Rows(0).Item("TCD_CERT_CATEG").ToString = "") Then
                        ShowMessage("Skill assessment result is fail, please re-appear for skill assessment.")
                        Exit Try
                    End If
                Catch ex As Exception

                End Try
            Else

                ShowMessage("Complete")
            End If
        End If

    End Sub
    Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click
        Try
            lblAddValidation.Text = String.Empty 'WI6447 START ADDED BY PRASUN ON 07012022
            If txtRenewSpno.Text = "" Then
                ShowMessage("Provide the Safety pass number for renewal Process.")
                Exit Sub
            End If
            Dim sql As String = "select to_char(ced_dob,'dd/mm/yyyy') ""CED_DOB"" from hrace.t_cemp_details  where ced_safety_pass_no=:spno"

            Dim cmd As New OracleCommand(sql, con)
            cmd.Parameters.Add(New OracleParameter(":spno", txtRenewSpno.Text.Trim().ToUpper()))
            Dim adapter As New OracleDataAdapter(cmd)
            con.Open()
            Dim dt As New DataTable()
            adapter.Fill(dt)
            If (dt.Rows.Count > 0) Then
                Dim dob As Date = DateTime.ParseExact(dt.Rows(0)(0), "dd/MM/yyyy", CultureInfo.InvariantCulture)
                Dim age As Double = GetAge(dob)
                Dim maxAge As Integer = GetMaxAge()
                If age < 18 Then
                    ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " & Session("comp_name_d") & " .")
                    Exit Sub
                ElseIf age > maxAge Then
                    hfActionPerformed.Value = "A"
                    ageMessage.InnerText = "You need to attach department’s chief approval for person above " & maxAge.ToString & " years of age at the time of generating safety pass."
                    pnlConfirmDocSubmision.Visible = True
                    MPopUpConfirmDocSubmision.Show()
                Else
                    Dim ls_sql1 As String = "select trunc(sysdate)-(select trunc(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:SRQ_REQ_NO) from dual"
                    Dim timediff As Integer = 0
                    Dim cmd1 As New OracleCommand(ls_sql1, con)
                    cmd1.Parameters.Add(New OracleParameter(":SRQ_REQ_NO", Session("requestnumber")))

                    Dim dt1 As New DataTable
                    dt1 = getRecord(cmd1, con)
                    If dt1.Rows.Count > 0 Then
                        timediff = Convert.ToUInt64(dt1.Rows(0).Item(0).ToString)
                    End If
                    If timediff > 10 Then
                        ShowMessage("Error: Your request is too old. You cannot able to proceed.\nSolution: Kindly raise another request, this one will be cancelled automatically.")
                        Exit Sub
                    Else
                        AddSafetyPass()
                    End If

                End If
            Else
                Dim query As String = "WITH input AS ( SELECT :safetyPass AS safetyPass FROM dual ), data AS ( SELECT t.CET_REQUEST_NO, t.CET_VENDOR_CODE, v.VDT_VENDOR_NAME, v.VDT_PHONE1, v.VDT_PHONE2, v.VDT_EMAIL1 FROM input i JOIN HRACE.T_CEMP_DETAILS_TMP t ON t.CET_SAFETY_PASSNO = i.safetyPass AND t.CET_REQ_STATUS IS NULL LEFT JOIN HRACE.T_VENDOR_DETAILS v ON v.VDT_VENDOR_CODE = t.CET_VENDOR_CODE AND v.VDT_COMPANY_CODE = t.CET_LOCATION_CODE FETCH FIRST 1 ROW ONLY ) SELECT CASE WHEN EXISTS (SELECT 1 FROM data) THEN 'Kindly contact with the vendor for rejecting the request number ' || d.CET_REQUEST_NO || '. Please find vendor details: (' || 'Code: ' || d.CET_VENDOR_CODE || ', Name: ' || d.VDT_VENDOR_NAME || ', Phone: ' || COALESCE( CASE WHEN d.VDT_PHONE1 = d.VDT_PHONE2 THEN d.VDT_PHONE1 WHEN d.VDT_PHONE1 IS NOT NULL AND d.VDT_PHONE2 IS NOT NULL THEN '(' || d.VDT_PHONE1 || ' / ' || d.VDT_PHONE2 || ')' ELSE COALESCE(d.VDT_PHONE1, d.VDT_PHONE2) END, 'Not Available' ) || ', Email: ' || NVL(d.VDT_EMAIL1, 'Not Available') || '). Once rejected then apply for new safety pass.' ELSE 'No pending request found against this safety pass number. Kindly check wht you typed.' END AS MESSAGE FROM dual LEFT JOIN data d ON 1 = 1"
                Dim errorQuote As String = "Error: No safety pass exists."
                Dim parameters As New Dictionary(Of String, String) From {
                    {"safetyPass", txtRenewSpno.Text.Trim().ToUpper()}
                }
                Dim solutionQuote As String = "Solution: " & getSingleQuoteFromQuery(query:=query, parameter:=parameters)
                ShowMessage(errorQuote & "\n" & solutionQuote)
                'ShowMessage("Error: No safety pass exists.\nSolution: Kindly check the safety pass number you typed.")
                txtRenewSpno.Text = ""
            End If
        Catch ex As Exception

        Finally
            If (con.State = ConnectionState.Open) Then
                con.Close()
            End If
        End Try
    End Sub

    Private Function getSingleQuoteFromQuery(ByVal query As String, Optional ByVal parameter As Dictionary(Of String, String) = Nothing) As String
        Dim cmd As New OracleCommand(query, con)
        Try
            If con.State = ConnectionState.Closed Then con.Open()
            If parameter IsNot Nothing Then
                For Each item In parameter
                    cmd.Parameters.Add(item.Key, item.Value)
                Next
            End If

            Dim result As Object = cmd.ExecuteScalar()
            If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                Return result.ToString()
            End If
        Catch ex As Exception
            Return String.Empty
        Finally
            If con.State = ConnectionState.Open Then con.Close()
        End Try
        Return String.Empty
    End Function

    Private Sub AddSafetyPass()
        Try
            Dim jsrCompanyCodes As New List(Of String) From {
                "1000", "1003", "1111", "1112", "1113", "1114", "1115", "1116", "1001", "3001",
                "6000", "7000", "3000", "9400", "9500", "9501", "9502", "9503", "1004", "1006"
            }

            PanelEmp.Style.Add("display", "none")
            Dim spNo As String = txtRenewSpno.Text.Trim.ToUpper()
            Dim errorQuote As String = String.Empty
            Dim solutionQuote As String = String.Empty

            If txtRenewSpno.Text = "" Then
                ShowMessage("Provide the Safety pass number for renewal Process.")
                Exit Sub
            End If

            Dim reqNo As String = lblreq.Text.Split(":")(1)
            'Dim SV_count As Integer = CInt(lnkSup.Text.Split(":")(1))
            'Dim WR_count As Integer = CInt(lnkWrk.Text.Split(":")(1))
            'Dim DV_count As Integer = CInt(LnkDR.Text.Split(":")(1))
            'Dim VC_count As Integer = CInt(LnkVC.Text.Split(":")(1))
            'Dim FM_count As Integer = CInt(LnkFM.Text.Split(":")(1))
            Dim SV_count As Integer = 0
            Dim WR_count As Integer = 0
            Dim DV_count As Integer = 0
            Dim VC_count As Integer = 0
            Dim FM_count As Integer = 0
            If (lnkSup.Text.Split(":")(1).Trim <> "") Then
                SV_count = CInt(lnkSup.Text.Split(":")(1))
            End If
            If (lnkWrk.Text.Split(":")(1).Trim <> "") Then
                WR_count = CInt(lnkWrk.Text.Split(":")(1))
            End If
            If (LnkDR.Text.Split(":")(1).Trim <> "") Then
                DV_count = CInt(LnkDR.Text.Split(":")(1))
            End If
            If (LnkVC.Text.Split(":")(1).Trim <> "") Then
                VC_count = CInt(LnkVC.Text.Split(":")(1))
            End If
            If (LnkFM.Text.Split(":")(1).Trim <> "") Then
                FM_count = CInt(LnkFM.Text.Split(":")(1))
            End If

            Dim sqlSPStatus = T_CEMP_DETAILS_qry() + " Where ced_safety_pass_no = '" + spNo + "'"
            Dim dtActive = getRecord(sqlSPStatus, con)
            If dtActive.Rows.Count > 0 Then

                ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   start****CED_SP_BLOCKED
                'If dtActive.Rows(0)("ced_sp_enabled") = "Y" Then
                '    ShowMessage("This Safety Pass Is already Active")
                '    txtRenewSpno.Text = ""
                '    Exit Sub
                'End If

                If dtActive.Rows(0)("CED_SP_BLOCKED") = "Y" Then
                    errorQuote = "Error: This Safety pass is Blocked "
                    solutionQuote = "Solution: Please contact to gate pass section for unblocking."
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                End If
                ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   end****

            Else
                ' vendor details to be shown with request number.
                Dim query As String = "WITH input AS ( SELECT :safetyPass AS safetyPass FROM dual ), data AS ( SELECT t.CET_REQUEST_NO, t.CET_VENDOR_CODE, v.VDT_VENDOR_NAME, v.VDT_PHONE1, v.VDT_PHONE2, v.VDT_EMAIL1 FROM input i JOIN HRACE.T_CEMP_DETAILS_TMP t ON t.CET_SAFETY_PASSNO = i.safetyPass AND t.CET_REQ_STATUS IS NULL LEFT JOIN HRACE.T_VENDOR_DETAILS v ON v.VDT_VENDOR_CODE = t.CET_VENDOR_CODE AND v.VDT_COMPANY_CODE = t.CET_LOCATION_CODE FETCH FIRST 1 ROW ONLY ) SELECT CASE WHEN EXISTS (SELECT 1 FROM data) THEN 'Kindly contact with the vendor for rejecting the request number ' || d.CET_REQUEST_NO || '. Please find vendor details: (' || 'Code: ' || d.CET_VENDOR_CODE || ', Name: ' || d.VDT_VENDOR_NAME || ', Phone: ' || COALESCE( CASE WHEN d.VDT_PHONE1 = d.VDT_PHONE2 THEN d.VDT_PHONE1 WHEN d.VDT_PHONE1 IS NOT NULL AND d.VDT_PHONE2 IS NOT NULL THEN '(' || d.VDT_PHONE1 || ' / ' || d.VDT_PHONE2 || ')' ELSE COALESCE(d.VDT_PHONE1, d.VDT_PHONE2) END, 'Not Available' ) || ', Email: ' || NVL(d.VDT_EMAIL1, 'Not Available') || '). Once rejected then apply for new safety pass.' ELSE 'No pending request found.' END AS MESSAGE FROM dual LEFT JOIN data d ON 1 = 1"
                errorQuote = "Error: No safety pass exists.Check the safety pass number"
                Dim parameters As New Dictionary(Of String, String) From {
                    {"safetyPass", spNo}
                }
                solutionQuote = "Solution: " & getSingleQuoteFromQuery(query:=query, parameter:=parameters)
                ShowMessage(errorQuote & "\n" & solutionQuote)
                txtRenewSpno.Text = ""
                Exit Sub
            End If


            Dim sql_ActiveSpno = Renewal_candidate(comp_cd, Session("VendCode"), spNo)
            Dim dt_ActiveSpno As DataTable = getRecord(sql_ActiveSpno, con)
            If dt_ActiveSpno.Rows.Count > 0 Then

            Else
                ' vendor code can be mismatched, kindly contact with safety dept for updating the vendor code.
                Dim query As String = "select to_char(CED_SP_VALID_TILL, 'dd-MON-yyyy') from hrace.t_cemp_details where CED_SAFETY_PASS_NO = :safetyPass"
                Dim parameters As New Dictionary(Of String, String) From {
                    {"safetyPass", spNo}
                }
                Dim validity As String = getSingleQuoteFromQuery(query:=query, parameter:=parameters)
                errorQuote = "Error: Safety pass " & spNo & " not authorized for renewal "
                solutionQuote = "Solution: Safety pass validity is more than 60 days upto (" & validity & "), you can not process further."
                ShowMessage(errorQuote & "\n" & solutionQuote)
                txtRenewSpno.Text = ""
                Exit Sub
            End If


            Dim sql_check As String = Renewal_candidate(comp_cd, Session("VendCode"), spNo) + "  and  CED_REQ_NO is not null"
            Dim dt_check As DataTable = getRecord(sql_check, con)
            If dt_check.Rows.Count > 0 Then
                Dim query As String = "WITH input AS ( SELECT :safetyPass AS safetyPass, :vendorCode AS vendorCode FROM dual ), data AS ( SELECT i.safetyPass, i.vendorCode, c.CED_VENDOR_CODE, t.CET_PROFILE_STATUS, t.CET_REQUEST_NO, t.CET_VENDOR_CODE, t.CET_LOCATION_CODE, CASE WHEN c.CED_VENDOR_CODE <> i.vendorCode THEN 'Y' ELSE 'N' END AS vendor_mismatch FROM input i LEFT JOIN HRACE.T_CEMP_DETAILS c ON c.CED_SAFETY_PASS_NO = i.safetyPass JOIN HRACE.T_CEMP_DETAILS_TMP t ON t.CET_SAFETY_PASSNO = i.safetyPass WHERE t.CET_REQ_STATUS IS NULL ) SELECT CASE WHEN vendor_mismatch = 'Y' AND CET_PROFILE_STATUS = 'I' THEN 'Renewal request (' || CET_REQUEST_NO || ') has already been raised from another vendor. Here is the details (' || ( SELECT 'Code: ' || VDT_VENDOR_CODE || ',' || ' Name: ' || VDT_VENDOR_NAME || ',' || ' Phone: ' || ',' || CASE WHEN VDT_PHONE1 = VDT_PHONE2 THEN VDT_PHONE2 WHEN VDT_PHONE2 <> VDT_PHONE1 THEN '(' || VDT_PHONE1 || ' / ' || VDT_PHONE2 || ')' WHEN VDT_PHONE1 IS NULL AND VDT_PHONE2 IS NULL THEN 'Not Available' WHEN VDT_PHONE1 IS NULL THEN VDT_PHONE2 WHEN VDT_PHONE2 IS NULL THEN VDT_PHONE1 END || ',' || ' Email: ' || VDT_EMAIL1 FROM HRACE.T_VENDOR_DETAILS WHERE VDT_VENDOR_CODE = CET_VENDOR_CODE AND VDT_COMPANY_CODE = CET_LOCATION_CODE AND ROWNUM = 1 ) || '). Kindly contact with him and get the request number rejected from their end.' WHEN vendor_mismatch = 'Y' THEN 'Safety pass not authorised for renewal due to sp belongs to this vendor (' || CED_VENDOR_CODE || ') kindly contact to safety department to update the vendor code.' else 'This safety pass belongs to your vendor & pending in queue for renewal process. Kindly try adding another safety pass' END AS message FROM data"
                Dim parameters As New Dictionary(Of String, String) From {
                    {"safetyPass", spNo},
                    {"vendorCode", Session("VendCode").ToString()}
                }
                errorQuote = "Error: The safety pass Number : " + spNo + "  is already added for renewal process."
                solutionQuote = "Solution: " & getSingleQuoteFromQuery(query:=query, parameter:=parameters)
                ShowMessage(errorQuote & "\n" & solutionQuote)
                lblAddValidation.Text = errorQuote 'WI6447 START ADDED BY PRASUN ON 07012022
                txtRenewSpno.Text = ""
                Exit Sub
            Else
                lblAddValidation.Text = String.Empty 'WI6447 START ADDED BY PRASUN ON 07012022
            End If



            Dim dt_category_check As DataTable = t_cemp_detail_dt(spNo)
            Dim category_onCheck As String = ""
            If dt_category_check.Rows.Count > 0 Then
                category_onCheck = dt_category_check.Rows(0).Item("CED_CATEGORY")
            End If

            Dim sqlFM As String = "select * from HRACE.t_cemp_type_master m where  ctm_value='" + category_onCheck + "'  AND m.ctm_type='FMC' "
            Dim dtfm As DataTable = getRecord(sqlFM, con)

            Dim sqlVc As String = "select * from HRACE.t_cemp_type_master m where  ctm_value='" + category_onCheck + "'  AND m.ctm_type='VCC' "
            Dim dtvc As DataTable = getRecord(sqlVc, con)


            If SV_count = 0 And (category_onCheck = SV Or category_onCheck = SH Or category_onCheck = SF Or category_onCheck = SA) Then
                errorQuote = "Error: You cannot add the safety pass number: " + spNo + " as you have not requested supervisor for renewal process."
                If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                    solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                Else
                    solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                End If
                ShowMessage(errorQuote & "\n" & solutionQuote)
                txtRenewSpno.Text = ""
                Exit Sub
            ElseIf WR_count = 0 And (category_onCheck = WR Or category_onCheck = WA) Then
                errorQuote = "Error: You cannot add the safety pass number: " + spNo + "  as you have not requested workres for renewal process."
                If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                    solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                Else
                    solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                End If
                ShowMessage(errorQuote & "\n" & solutionQuote)
                txtRenewSpno.Text = ""
                Exit Sub
            ElseIf DV_count = 0 And (category_onCheck = DV Or category_onCheck = DA Or category_onCheck = DH) Then
                errorQuote = "Error: You cannot add the safety pass number: " + spNo + "   as you have not requested drivers for renewal process."
                If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                    solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                Else
                    solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                End If
                ShowMessage(errorQuote & "\n" & solutionQuote)
                txtRenewSpno.Text = ""
                Exit Sub
            ElseIf FM_count = 0 And (category_onCheck <> WR Or category_onCheck <> DV Or category_onCheck <> SV Or category_onCheck <> SH _
                Or category_onCheck <> SF Or category_onCheck <> WA Or category_onCheck <> DA Or category_onCheck <> SA Or category_onCheck <> DH) Then

                If dtfm.Rows.Count > 0 Then
                    errorQuote = "Error: You cannot add the safety pass number: " + spNo + "  as you have not requested Facility Manager for renewal process."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                Else

                    If dtvc.Rows.Count > 0 Then
                        If VC_count = 0 Then
                            errorQuote = "Error: You cannot add the safety pass number: " + spNo + "  as you have not requested Video Capsule for renewal process."
                            If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                                solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                            Else
                                solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                            End If
                            ShowMessage(errorQuote & "\n" & solutionQuote)
                            txtRenewSpno.Text = ""
                            Exit Sub
                        End If
                    Else  'Added (26/05/16)
                        If (category_onCheck = WR Or category_onCheck = DV Or category_onCheck = SV Or category_onCheck = SH Or category_onCheck = SF _
                            Or category_onCheck = WA Or category_onCheck = DA Or category_onCheck = SA Or category_onCheck = DH) Then

                        ElseIf (category_onCheck <> WR Or category_onCheck <> DV Or category_onCheck <> SV Or category_onCheck <> SH Or category_onCheck <> SF _
                                Or category_onCheck <> WA Or category_onCheck <> DA Or category_onCheck <> SA Or category_onCheck <> DH) Then
                            errorQuote = "Error: You cannot add the safety pass number : " + spNo + " for renewal process as it does not comes under any category of requested employee."
                            If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                                solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                            Else
                                solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                            End If
                            ShowMessage(errorQuote & "\n" & solutionQuote)
                            txtRenewSpno.Text = ""
                            Exit Sub
                        End If

                    End If

                End If

            ElseIf VC_count = 0 And (category_onCheck <> WR Or category_onCheck <> DV Or category_onCheck <> SV Or category_onCheck <> SH Or category_onCheck <> SF _
                                     Or category_onCheck <> WA Or category_onCheck <> DA Or category_onCheck <> SA Or category_onCheck <> DH) Then

                If dtfm.Rows.Count > 0 Then
                    errorQuote = "Error: You cannot add the safety pass number: " + spNo + "  as you have not requested Facility Manager for renewal process."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                Else
                    If dtvc.Rows.Count > 0 Then
                        If VC_count = 0 Then
                            errorQuote = "Error: You cannot add the safety pass number: " + spNo + "  as you have not requested  Video capsule for renewal process."
                            If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                                solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                            Else
                                solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                            End If
                            ShowMessage(errorQuote & "\n" & solutionQuote)
                            txtRenewSpno.Text = ""
                            Exit Sub
                        End If
                    Else 'Added (26/05/16)
                        If (category_onCheck = WR Or category_onCheck = DV Or category_onCheck = SV Or category_onCheck = SH Or category_onCheck = SF _
                            Or category_onCheck = WA Or category_onCheck = DA Or category_onCheck = SA Or category_onCheck = DH) Then

                        ElseIf (category_onCheck <> WR Or category_onCheck <> DV Or category_onCheck <> SV Or category_onCheck <> SH Or category_onCheck <> SF _
                                Or category_onCheck <> WA Or category_onCheck <> DA Or category_onCheck <> SA Or category_onCheck <> DH) Then
                            errorQuote = "Error: You cannot add the safety pass number : " + spNo + " for renewal process as it does not comes under any category of requested employee."
                            If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                                solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                            Else
                                solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                            End If
                            ShowMessage(errorQuote & "\n" & solutionQuote)
                            txtRenewSpno.Text = ""
                            Exit Sub
                        End If

                    End If

                End If

            End If

            ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   start****

            If (category_onCheck = "WR" Or category_onCheck = "WA") Then
                Dim WR_renewal As Integer = check_count_renewal(category_onCheck, reqNo, WR)

                If WR_renewal = -2 Then
                    errorQuote = "Error: You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                ElseIf WR_renewal = WR_count Then
                    errorQuote = "Error: No more workers can be added for renewal process."
                    solutionQuote = "Solution: Kindly raise a fresh request."
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    Exit Sub
                End If

            ElseIf (category_onCheck = "SV" Or category_onCheck = "SH" Or category_onCheck = "SF" Or category_onCheck = "SA") Then
                Dim SV_renewal As Integer = check_count_renewal(category_onCheck, reqNo, SV)
                If SV_renewal = -2 Then
                    errorQuote = "Error: You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                ElseIf SV_renewal = SV_count Then
                    errorQuote = "Error: No more Supervisor can be added for renewal process."
                    solutionQuote = "Solution: Kindly raise fresh request."
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    Exit Sub
                End If

            ElseIf (category_onCheck = "DV" Or category_onCheck = "DA" Or category_onCheck = "DH") Then
                Dim dv_renewal As Integer = check_count_renewal(category_onCheck, reqNo, DV)
                If dv_renewal = -2 Then
                    errorQuote = "Error: You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                ElseIf dv_renewal = DV_count Then
                    errorQuote = "Error: No more Drivers can be added for renewal process."
                    solutionQuote = "Solution: Kindly raise fresh request."
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    Exit Sub
                End If
            ElseIf (category_onCheck = "FM" Or category_onCheck = "FA") Then

                Dim FM_renewal As Integer = check_count_renewal(category_onCheck, reqNo, FM)
                If FM_renewal = -2 Then
                    errorQuote = "Error: You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                ElseIf FM_renewal = FM_count Then
                    errorQuote = "No more Facility Managers can be added for renewal process."
                    solutionQuote = "Solution: Kindly raise fresh request."
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    Exit Sub
                End If
            ElseIf (category_onCheck = "VC" Or category_onCheck = "VA") Then
                Dim VC_renewal As Integer = check_count_renewal(category_onCheck, reqNo, VC)

                If VC_renewal = -2 Then
                    errorQuote = "Error: You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of video capsule/ Facility Manager."
                    If Not jsrCompanyCodes.Contains(Session("Comp_code").ToString) Then
                        solutionQuote = "Solution: Kindly contact to saftey department to change/update the safety pass category."
                    Else
                        solutionQuote = "Solution: Kindly contact to JNTVTI to change/update the safety pass category."
                    End If
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    txtRenewSpno.Text = ""
                    Exit Sub
                ElseIf VC_renewal = VC_count Then
                    errorQuote = "Error: No more Video capsule delegates can be added for renewal process."
                    solutionQuote = "Solution: Kindly raise fresh request."
                    ShowMessage(errorQuote & "\n" & solutionQuote)
                    Exit Sub
                End If
            End If

            ''Add/modify/Comment by anand ON 20160706 WRT CMR No:2016/04/91/J28/T1   end****


            Dim sql As String = Renewal_candidate(comp_cd, Session("VendCode"), spNo)
            Dim dt As DataTable = getRecord(sql, con)

            If dt.Rows.Count > 0 Then
                Dim Updqry As String = "  update t_cemp_details set CED_REQ_NO='" + reqNo + "' where ced_safety_pass_no='" + spNo + "' "
                Dim cmd_upd_att As New OracleCommand(Updqry, con)
                Try
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd_upd_att.ExecuteNonQuery()

                Catch ex As Exception

                Finally
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                End Try

                'insert into t_cemp_details
                Renewal_insert_T_cemp_details_tmp(spNo, reqNo)


                RenewalProcessGridview(reqNo)
                txtRenewSpno.Text = ""
            Else
                ShowMessage("The safety pass number cannot be added for renewal process")
            End If

        Catch ex As Exception
            ShowMessage(ex.ToString)
        End Try
    End Sub
    Public Sub clearVariables()
        spouse = ""
        fatherName = ""
        lastname = ""
        firstname = ""
        category = ""
        vendorCode = ""
        location = ""
        locationCode = ""
        gender = ""
        emergencyNo = ""
        phoneNo = ""
        bloodGroup = ""
        uniqueIDVal = ""
        uniqueIDType = ""
        identityMark = ""
        areaofWork = ""
        birthAge = ""
        dob = ""
        affirmative = ""
        address1 = ""
        address2 = ""
        address3 = ""
        country = ""
        qualification = ""
        profile_status = ""
        verify_status = ""
        dobcertno = ""
        drvcertno = ""
        passcertno = ""
        UAN = ""
        IP = ""
    End Sub
    Public Sub safetyPassdetails(ByVal safetyPassNo As String, ByVal reqNo As String)

        Try
            clearVariables()

            Dim qry As String = "select CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY,CET_LOC_CODE,CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,"
            qry += "CET_FATHER_NAME,CET_SPOUSE_NAME,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_BLOOD_GROUP,CET_UNIQUE_ID_TYPE,CET_UNIQUE_ID_VALUE,CET_IDENTIFICATION_MARK, "
            qry += " CET_AREA_OF_WORK, CET_AGE, to_char(CET_DOB,'dd/MM/yyyy') CET_DOB,CET_AFFIRMATIVE,"
            qry += " (select ctm_type_desc from t_cemp_type_master where substr(CTM_TYPE_CODE, '-4', '4') = '" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_PROFILE_STATUS) CET_PROFILE_STATUS,"
            qry += " (select ctm_type_desc from t_cemp_type_master where substr(CTM_TYPE_CODE, '-4', '4') = '" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_DOCVER_STATUS)    CET_DOCVER_STATUS"
            qry += " from t_cemp_details_tmp cdt "
            qry += " where CET_SAFETY_PASSNO='" + safetyPassNo + "' "
            qry += " and CET_REQUEST_NO='" + reqNo + "' "


            Dim dt As DataTable = clmClass.getRecord(qry, con)

            If dt.Rows.Count > 0 Then


                If Not IsDBNull(dt.Rows(0).Item("CET_LOCATION_CODE")) Then
                    locationCode = dt.Rows(0).Item("CET_LOCATION_CODE")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_LOC_CODE")) Then
                    location = dt.Rows(0).Item("CET_LOC_CODE")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_VENDOR_CODE")) Then
                    vendorCode = dt.Rows(0).Item("CET_VENDOR_CODE")
                End If
                If Not IsDBNull(dt.Rows(0).Item("CET_CATEGORY")) Then
                    category = dt.Rows(0).Item("CET_CATEGORY")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_DEPT_CODE")) Then
                    dept = dt.Rows(0).Item("CET_DEPT_CODE")
                    ViewState("deptchk") = dept.Trim
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_FIRSTNAME")) Then
                    firstname = dt.Rows(0).Item("CET_FIRSTNAME")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_LASTNAME")) Then
                    lastname = dt.Rows(0).Item("CET_LASTNAME")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_FATHER_NAME")) Then
                    fatherName = dt.Rows(0).Item("CET_FATHER_NAME")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_SPOUSE_NAME")) Then
                    spouse = dt.Rows(0).Item("CET_SPOUSE_NAME")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_GENDER")) Then
                    gender = dt.Rows(0).Item("CET_GENDER")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_EMERGENCY_NO")) Then
                    emergencyNo = dt.Rows(0).Item("CET_EMERGENCY_NO")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_PHONE_NO")) Then
                    phoneNo = dt.Rows(0).Item("CET_PHONE_NO")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_BLOOD_GROUP")) Then
                    bloodGroup = dt.Rows(0).Item("CET_BLOOD_GROUP")
                End If
                If Not IsDBNull(dt.Rows(0).Item("CET_UNIQUE_ID_VALUE")) Then
                    uniqueIDVal = dt.Rows(0).Item("CET_UNIQUE_ID_VALUE")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_IDENTIFICATION_MARK")) Then
                    identityMark = dt.Rows(0).Item("CET_IDENTIFICATION_MARK")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_UNIQUE_ID_TYPE")) Then
                    uniqueIDType = dt.Rows(0).Item("CET_UNIQUE_ID_TYPE")
                End If
                If Not IsDBNull(dt.Rows(0).Item("CET_AREA_OF_WORK")) Then
                    areaofWork = dt.Rows(0).Item("CET_AREA_OF_WORK")
                End If

                If Not IsDBNull(dt.Rows(0).Item("CET_AGE")) Then
                    birthAge = dt.Rows(0).Item("CET_AGE")
                End If
                If Not IsDBNull(dt.Rows(0).Item("CET_DOB")) Then
                    dob = dt.Rows(0).Item("CET_DOB")
                End If
                If Not IsDBNull(dt.Rows(0).Item("CET_AFFIRMATIVE")) Then
                    affirmative = dt.Rows(0).Item("CET_AFFIRMATIVE")
                End If

            End If

        Catch ex As Exception
            ShowMessage(ex.ToString)
        End Try
    End Sub
    Public Sub Renewal_insert_T_cemp_details_tmp(ByVal spNo As String, ByVal reqNo As String)
        'safety pass details
        Dim locationCode As String = ""
        Dim location As String = ""
        Dim vendorCode As String = ""
        Dim category As String = ""
        Dim dept As String = ""
        Dim firstname As String = ""
        Dim lastname As String = ""
        Dim fatherName As String = ""
        Dim spouse As String = ""
        Dim gender As String = ""
        Dim emergencyNo As String = ""
        Dim phoneNo As String = ""
        Dim bloodGroup As String = ""
        Dim uniqueIDVal As String = ""
        Dim uniqueIDType As String = ""
        Dim identityMark As String = ""
        Dim areaofWork As String = ""
        Dim birthAge As String = ""
        Dim dob As String = ""
        Dim affirmative As String = ""

        'START ADD BY PRASUN CHAKRABORTY 02092022
        Dim ced_pan_no As String = ""
        Dim ced_adlt_name As String = ""
        Dim ced_adlt_rel As String = ""
        Dim ced_adlt_address As String = ""
        Dim ced_adlt_mobile_no As String = ""
        Dim ced_nationality As String = ""
        Dim ced_aadhar_no As String = ""
        Dim ced_emp_place As String = ""
        Dim ced_relay_data As String = ""
        'END ADD BY PRASUN CHAKRABORTY 02092022
        Dim UAN_no As String = ""
        Dim ip_no As String = ""

        Dim arr_cmd As New ArrayList()
        Dim qry As String = T_CEMP_DETAILS_qry() + " where CED_SAFETY_PASS_NO='" + spNo + "'"
        Dim dt As DataTable = clmClass.getRecord(qry, con)

        If dt.Rows.Count > 0 Then


            If Not IsDBNull(dt.Rows(0).Item("CED_COMPANY_CODE")) Then
                locationCode = dt.Rows(0).Item("CED_COMPANY_CODE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_LOC_CODE")) Then
                location = dt.Rows(0).Item("CED_LOC_CODE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_VENDOR_CODE")) Then
                vendorCode = dt.Rows(0).Item("CED_VENDOR_CODE")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CED_CATEGORY")) Then
                category = dt.Rows(0).Item("CED_CATEGORY")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_DEPT_CODE")) Then
                dept = dt.Rows(0).Item("CED_DEPT_CODE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_FIRSTNAME")) Then
                firstname = dt.Rows(0).Item("CED_FIRSTNAME")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_LASTNAME")) Then
                lastname = dt.Rows(0).Item("CED_LASTNAME")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_FATHER_NAME")) Then
                fatherName = dt.Rows(0).Item("CED_FATHER_NAME")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_HUSBAND_NAME")) Then
                spouse = dt.Rows(0).Item("CED_HUSBAND_NAME")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_GENDER")) Then
                gender = dt.Rows(0).Item("CED_GENDER")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_EMERGENCY_NO")) Then
                emergencyNo = dt.Rows(0).Item("CED_EMERGENCY_NO")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_PHONE_NO")) Then
                phoneNo = dt.Rows(0).Item("CED_PHONE_NO")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_BLOOD_GROUP")) Then
                bloodGroup = dt.Rows(0).Item("CED_BLOOD_GROUP")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CED_UNIQUE_ID_VALUE")) Then
                uniqueIDVal = dt.Rows(0).Item("CED_UNIQUE_ID_VALUE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_IDENTIFICATION_MARK")) Then
                identityMark = dt.Rows(0).Item("CED_IDENTIFICATION_MARK")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_UNIQUE_ID_TYPE")) Then
                uniqueIDType = dt.Rows(0).Item("CED_UNIQUE_ID_TYPE")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CED_AREA_OF_WORK")) Then
                areaofWork = dt.Rows(0).Item("CED_AREA_OF_WORK")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_AGE")) Then
                birthAge = dt.Rows(0).Item("CED_AGE")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CED_DOB")) Then
                dob = dt.Rows(0).Item("CED_DOB")
            End If
            If Not IsDBNull(dt.Rows(0).Item("CED_AFFIRMATIVE")) Then
                affirmative = dt.Rows(0).Item("CED_AFFIRMATIVE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_UAN_NO")) Then
                UAN_no = dt.Rows(0).Item("CED_UAN_NO")
            End If


            If Not IsDBNull(dt.Rows(0).Item("CED_IP_NO")) Then
                ip_no = dt.Rows(0).Item("CED_IP_NO")
            End If

            'START ADD BY PRASUN ON 02092022

            If Not IsDBNull(dt.Rows(0).Item("CED_PAN_NO")) Then
                ced_pan_no = dt.Rows(0).Item("CED_PAN_NO")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_ADLT_NAME")) Then
                ced_adlt_name = dt.Rows(0).Item("CED_ADLT_NAME")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_ADLT_REL")) Then
                ced_adlt_rel = dt.Rows(0).Item("CED_ADLT_REL")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_ADLT_ADDRESS")) Then
                ced_adlt_address = dt.Rows(0).Item("CED_ADLT_ADDRESS")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_ADLT_MOBILE_NO")) Then
                ced_adlt_mobile_no = dt.Rows(0).Item("CED_ADLT_MOBILE_NO")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_NATIONALITY")) Then
                ced_nationality = dt.Rows(0).Item("CED_NATIONALITY")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_EMP_PLACE")) Then
                ced_emp_place = dt.Rows(0).Item("CED_EMP_PLACE")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_AADHAR_NO")) Then
                ced_aadhar_no = dt.Rows(0).Item("CED_AADHAR_NO")
            End If

            If Not IsDBNull(dt.Rows(0).Item("CED_RELAY_DATA")) Then
                ced_relay_data = dt.Rows(0).Item("CED_RELAY_DATA")
            End If

            'END ADD BY PRASUN ON 02092022
        End If

        Dim ls_sqlupdate As String = String.Empty
        Dim cmd_update As OracleCommand
        Dim dt_update As New DataTable
        Try
            ls_sqlupdate = "select SRQ_DEPT_CODE from hrace.t_sp_request where SRQ_REQ_NO=:SRQ_REQ_NO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd_update = New OracleCommand(ls_sqlupdate, con)
            cmd_update.Parameters.Add(New OracleParameter(":SRQ_REQ_NO", Session("requestnumber")))
            dt_update = getRecord(cmd_update, con)
            If dt_update.Rows.Count > 0 Then
                dept = dt_update.Rows(0).Item("SRQ_DEPT_CODE")
            End If
        Catch ex As Exception
            ShowMessage("Error occurs")
            Exit Sub
        End Try
        'Dim sql As String = " Select CET_SAFETY_PASSNO from HRACE.t_cemp_details_tmp where cet_safety_passno='" + spNo + "'"
        'Dim dt_tmp As DataTable = getRecord(sql, con)
        'If dt_tmp.Rows.Count = 0 Then

        Dim sqlProfile As String = ""
        sqlProfile = " insert into HRACE.T_CEMP_DETAILS_TMP(CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY,CET_LOC_CODE"
        sqlProfile += ",CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,CET_FATHER_NAME,CET_SPOUSE_NAME,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_UNIQUE_ID_TYPE,"
        'START EDIT BY PRASUN ON 02092022
        If pnlFormA.Visible Then
            sqlProfile += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,CET_DOB,CET_AGE,CET_AFFIRMATIVE,CET_BLOOD_GROUP,CET_CREATED_BY,CET_CREATED_DATE, CET_PROFILE_STATUS,CET_DOCVER_STATUS,CET_SCHEDULE_STATUS,CET_UAN_NO,CET_IP_NO,"
            sqlProfile += "  CET_PAN_NO,CET_ADLT_NAME,CET_ADLT_REL,CET_ADLT_ADDRESS,CET_ADLT_MOBILE_NO,CET_NATIONALITY,CET_AADHAR_NO,CET_EMP_PLACE,CET_RELAY_DATA)"
        Else
            sqlProfile += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,CET_DOB,CET_AGE,CET_AFFIRMATIVE,CET_BLOOD_GROUP,CET_CREATED_BY,CET_CREATED_DATE, CET_PROFILE_STATUS,CET_DOCVER_STATUS,CET_SCHEDULE_STATUS,CET_UAN_NO,CET_IP_NO)"
        End If

        sqlProfile += " values("
        sqlProfile = sqlProfile + ":sp_no,"
        sqlProfile = sqlProfile + ":req_no,"
        sqlProfile = sqlProfile + ":loc_cd,"
        sqlProfile = sqlProfile + ":vend_code,"
        sqlProfile = sqlProfile + ":cat,"
        sqlProfile = sqlProfile + ":loc,"
        sqlProfile = sqlProfile + ":dept,"
        sqlProfile = sqlProfile + ":fname,"
        sqlProfile = sqlProfile + ":lname,"
        sqlProfile = sqlProfile + ":fatherName,"
        sqlProfile = sqlProfile + ":spouse,"
        sqlProfile = sqlProfile + ":gend,"

        sqlProfile = sqlProfile + ":emgNo,"
        sqlProfile = sqlProfile + ":phNo,"
        sqlProfile = sqlProfile + ":uIDType,"
        sqlProfile = sqlProfile + ":uIDVal,"
        sqlProfile = sqlProfile + ":identMark,"
        sqlProfile = sqlProfile + ":areaWrk,"
        sqlProfile = sqlProfile + "to_date(:dbo,'DD/MM/YYYY'),"
        sqlProfile = sqlProfile + "to_char(sysdate,'yyyy') - to_char(to_date(:dbo,'DD/MM/YYYY'),'yyyy'),"
        sqlProfile = sqlProfile + ":aff,"
        sqlProfile = sqlProfile + ":bloodgrp,"
        sqlProfile = sqlProfile + ":vend_code,"
        If pnlFormA.Visible Then
            sqlProfile = sqlProfile + "SYSDATE" + " ,'I','I','I',:uan,:ip,"
            sqlProfile = sqlProfile + ":cet_pan_no,"
            sqlProfile = sqlProfile + ":cet_adlt_name,"
            sqlProfile = sqlProfile + ":cet_adlt_rel,"
            sqlProfile = sqlProfile + ":cet_adlt_address,"
            sqlProfile = sqlProfile + ":cet_adlt_mobile_no,"
            sqlProfile = sqlProfile + ":cet_nationality,"
            sqlProfile = sqlProfile + ":cet_aadhar_no,"
            sqlProfile = sqlProfile + ":cet_emp_place,"
            sqlProfile = sqlProfile + ":cet_relay_data )"
        Else
            sqlProfile = sqlProfile + "SYSDATE" + " ,'I','I','I',:uan,:ip )"

        End If

        Dim ins_cmd1 As New OracleCommand(sqlProfile, con)
        ins_cmd1.Parameters.Add(New OracleParameter(":sp_no", spNo))
        ins_cmd1.Parameters.Add(New OracleParameter(":req_no", reqNo))
        ins_cmd1.Parameters.Add(New OracleParameter(":loc_cd", locationCode))
        ins_cmd1.Parameters.Add(New OracleParameter(":vend_code", Session("VendCode")))
        ins_cmd1.Parameters.Add(New OracleParameter(":cat", category))
        ins_cmd1.Parameters.Add(New OracleParameter(":loc", location))
        ins_cmd1.Parameters.Add(New OracleParameter(":dept", dept))
        ins_cmd1.Parameters.Add(New OracleParameter(":fname", firstname))
        ins_cmd1.Parameters.Add(New OracleParameter(":lname", lastname))
        ins_cmd1.Parameters.Add(New OracleParameter(":fatherName", fatherName))
        ins_cmd1.Parameters.Add(New OracleParameter(":spouse", spouse))
        ins_cmd1.Parameters.Add(New OracleParameter(":gend", gender))
        ins_cmd1.Parameters.Add(New OracleParameter(":emgNo", emergencyNo))
        ins_cmd1.Parameters.Add(New OracleParameter(":phNo", phoneNo))
        ins_cmd1.Parameters.Add(New OracleParameter(":uIDType", uniqueIDType))
        ins_cmd1.Parameters.Add(New OracleParameter(":uIDVal", uniqueIDVal))
        ins_cmd1.Parameters.Add(New OracleParameter(":identMark", identityMark))
        ins_cmd1.Parameters.Add(New OracleParameter(":areaWrk", areaofWork))
        ins_cmd1.Parameters.Add(New OracleParameter(":dbo", dob))
        ins_cmd1.Parameters.Add(New OracleParameter(":aff", affirmative))
        ins_cmd1.Parameters.Add(New OracleParameter(":bloodgrp", bloodGroup))

        ins_cmd1.Parameters.Add(New OracleParameter(":uan", UAN_no.ToString))
        ins_cmd1.Parameters.Add(New OracleParameter(":ip", ip_no.ToString))

        If pnlFormA.Visible = True Then
            'START ADD BY PRASUN 02092022

            ins_cmd1.Parameters.Add(New OracleParameter(":cet_pan_no", ced_pan_no.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_adlt_name", ced_adlt_name.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_adlt_rel", ced_adlt_rel.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_adlt_address", ced_adlt_address.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_adlt_mobile_no", ced_adlt_mobile_no.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_nationality", ced_nationality.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_aadhar_no", ced_aadhar_no.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_emp_place", ced_emp_place.ToString))
            ins_cmd1.Parameters.Add(New OracleParameter(":cet_relay_data", ced_relay_data.ToString))
            'END ADD BY PRASUN 02092022
        End If

        arr_cmd.Add(ins_cmd1)

        '''''''''''''''''''''''''''''''add data to t_cemp_details for biometric enable'''''''''''''''''''



        '''''''''''''''''''''''''''''end of code'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        'ElseIf dt_tmp.Rows.Count > 0 Then

        'Dim Updatestatus As String = " update HRACE.T_CEMP_DETAILS_TMP set CET_REQUEST_NO='" + reqNo + "',CET_DOCVER_STATUS='I',CET_SCHEDULE_STATUS='I',CET_MODIFIED_BY ='" + vVencode + "',CET_MODIFIED_DATE =sysdate where CET_SAFETY_PASSNO='" + spNo + "' "
        'Dim ins_cmd2 As New OracleCommand(Updatestatus, con)
        'arr_cmd.Add(ins_cmd2)
        'End If

        If arr_cmd.Count > 0 Then
            Dim counter As Integer = 0
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim tran_Ins As OracleTransaction
            tran_Ins = con.BeginTransaction()
            Try
                For counter = 0 To arr_cmd.Count - 1
                    Dim con_ins As New OracleCommand()
                    con_ins = arr_cmd.Item(counter)
                    con_ins.Transaction = tran_Ins
                    con_ins.ExecuteNonQuery()
                Next
                tran_Ins.Commit()

            Catch ex As Exception
                tran_Ins.Rollback()
                ShowMessage(ex.ToString)
            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If

    End Sub
    Public Function check_count_renewal(ByVal cat As String, ByVal reqNo As String, ByVal RenewalCatChck As String) As Integer
        Dim count As Integer = 0

        Dim SpNo As String = txtRenewSpno.Text.Trim
        'CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
        If cat = WR Or cat = SV Or cat = DV Or cat = SH Or cat = SF Or cat = WA Or cat = SA Or cat = DA Or cat = DH Then
            '  Dim sql_check As String = T_CEMP_DETAILS_qry() + " where CED_CATEGORY='" + cat + "' and  CED_REQ_NO='" + reqNo + "' "
            'Dim sql_check As String = t_cemp_details_tmp_qry() + " where CET_CATEGORY='" + cat + "' and  CET_REQUEST_NO='" + reqNo + "' "

            Dim sql_check As String = t_cemp_details_tmp_qry()
            If cat = SV Or cat = SH Or cat = SF Or cat = SA Then
                sql_check += " where CET_CATEGORY IN (" + String.Format("'{0}','{1}','{2}','{3}'", SV, SH, SF, SA) + ")"
            ElseIf cat = WR Or cat = WA Then
                sql_check += " where CET_CATEGORY IN (" + String.Format("'{0}','{1}'", WR, WA) + ")"
            ElseIf cat = DA Or cat = DV Or cat = DH Then
                sql_check += " where CET_CATEGORY IN (" + String.Format("'{0}','{1}','{2}'", DV, DA, DH) + ")"
            End If
            sql_check += "  and CET_REQUEST_NO='" + reqNo + "' "
            Dim dt_check As DataTable = getRecord(sql_check, con)

            If dt_check.Rows.Count > 0 Then
                count = dt_check.Rows.Count
            Else
                count = -1
            End If

        Else
            Dim sql As String = "select * from hrace.t_cemp_details b, HRACE.t_cemp_type_master m where b.ced_safety_pass_no='" + SpNo + "' and ced_category='" + cat + "'  and b.ced_category=m.ctm_value "


            Dim sqlVC As String = sql + " AND m.ctm_type='VCC'"
            Dim dt As DataTable = getRecord(sqlVC, con)
            If dt.Rows.Count > 0 Then
                If RenewalCatChck = VC Or RenewalCatChck = VA Then
                    Dim category As String = ""
                    If Not IsDBNull(dt.Rows(0).Item("ced_category")) Then
                        category = dt.Rows(0).Item("ced_category")
                    End If

                    ' Dim sql_check As String = T_CEMP_DETAILS_qry() + " where CED_CATEGORY='" + category + "' and  CED_REQ_NO='" + reqNo + "' "
                    ' Dim sql_check As String = t_cemp_details_tmp_qry() + " where CET_CATEGORY='" + cat + "' and  CET_REQUEST_NO='" + reqNo + "' "
                    Dim sql_check As String = "select count(*) from hrace.t_cemp_details_tmp h , HRACE.t_cemp_type_master m where h.cet_request_no='" + reqNo + "' and h.cet_category=m.ctm_value AND m.ctm_type='VCC'"
                    Dim dt_check As DataTable = getRecord(sql_check, con)


                    If (CInt(dt_check.Rows(0).Item(0)) > 0) Then
                        count = dt_check.Rows.Count
                        If count = "0" Then
                            count = -1
                        End If
                    Else
                        count = -1          'allow the category to add
                    End If
                Else
                    count = -1
                End If
            Else



                Dim sqlFM As String = sql + " AND m.ctm_type='FMC'"
                Dim dt1 As DataTable = getRecord(sqlFM, con)
                If dt1.Rows.Count > 0 Then
                    If RenewalCatChck = FM Or RenewalCatChck = FA Then
                        Dim category As String = ""
                        If Not IsDBNull(dt1.Rows(0).Item("ced_category")) Then
                            category = dt1.Rows(0).Item("ced_category")
                        End If

                        ' Dim sql_check As String = T_CEMP_DETAILS_qry() + " where CED_CATEGORY='" + category + "' and  CED_REQ_NO='" + reqNo + "' "
                        '  Dim sql_check As String = t_cemp_details_tmp_qry() + " where CET_CATEGORY='" + cat + "' and  CET_REQUEST_NO='" + reqNo + "' "
                        Dim sql_check As String = "select count(*) from hrace.t_cemp_details_tmp h , HRACE.t_cemp_type_master m where h.cet_request_no='" + reqNo + "' and h.cet_category=m.ctm_value AND m.ctm_type='FMC'"
                        Dim dt_check As DataTable = getRecord(sql_check, con)



                        If (CInt(dt_check.Rows(0).Item(0)) > 0) Then
                            count = dt_check.Rows.Count
                            If count = "0" Then
                                count = -1
                            End If
                        Else
                            count = -1   'allow the category to add
                        End If
                    Else
                        count = -1
                    End If
                Else

                    count = -2      ' the category is not allowed for renewal process

                End If



            End If


        End If

        Return count
    End Function
    Public Sub RenewalProcessGridview(ByVal ReqNo As String)
        PanelEmp.Style.Add("display", "none")

        Dim sql As String = emp_detail_qry()
        Dim dt As DataTable = getRecord(sql, con)


        status_variables()
        Dim dtemp As New DataTable
        Dim safetyPassnumber As String = ""
        dtemp.Columns.Add("CED_SAFETY_PASSNO")
        dtemp.Columns.Add("CED_NAME")
        dtemp.Columns.Add("CED_CATEGORY")
        dtemp.Columns.Add("CED_UNIQUE_ID_VALUE")
        dtemp.Columns.Add("STATUS")
        dtemp.Columns.Add("VERIFY")
        dtemp.Columns.Add("OVER_ALL_STATUS")

        Dim dr As DataRow

        If dt.Rows.Count > 0 Then
            For i = 0 To dt.Rows.Count - 1
                safetyPassnumber = dt.Rows(i).Item("CET_SAFETY_PASSNO")
                dr = dtemp.NewRow
                dr("CED_SAFETY_PASSNO") = dt.Rows(i).Item("CET_SAFETY_PASSNO")
                dr("CED_NAME") = dt.Rows(i).Item("CET_FIRSTNAME") + " " + dt.Rows(i).Item("CET_LASTNAME")
                dr("CED_CATEGORY") = dt.Rows(i).Item("CET_CATEGORY_TYPE") 'change
                dr("CED_UNIQUE_ID_VALUE") = dt.Rows(i).Item("CET_UNIQUE_ID_VALUE")
                dr("STATUS") = dt.Rows(i).Item("CET_PROFILE_STATUS")
                If (dt.Rows(i).Item("CET_REQ_STATUS") = "REJECTED") Then
                    dr("OVER_ALL_STATUS") = String.Format("<font color='red'>{0}</font>", dt.Rows(i).Item("CET_REQ_STATUS"))
                ElseIf (dt.Rows(i).Item("CET_REQ_STATUS") = "IN PROGRESS") Then
                    dr("OVER_ALL_STATUS") = String.Format("<font color='#926e04'>{0}</font>", dt.Rows(i).Item("CET_REQ_STATUS"))
                Else
                    dr("OVER_ALL_STATUS") = dt.Rows(i).Item("CET_REQ_STATUS")
                End If


                'Dim str As String = T_CEMP_DETAILS_qry() + " where CED_SAFETY_PASS_NO='" + safetyPassnumber + "'  and CED_SP_ENABLED='Y' and CED_SP_BLOCKED='N' and CED_POLICE_VERIFICATION='Y' and  CED_PV_VALID_TILL>= to_date(sysdate,'dd/MM/rrrr')"
                Dim str As String = "select count(*) from HRACE.t_cemp_details_tmp  where cet_docver_status='C' and cet_safety_passno ='" + safetyPassnumber + "' and cet_request_no='" + ReqNo + "' and cet_pv_valid_till is not null"
                Dim dtstr As DataTable = getRecord(str, con)

                If (CInt(dtstr.Rows(0).Item(0)) > 0) Then
                    dr("VERIFY") = msg_complete
                Else
                    dr("VERIFY") = String.Format("<font color='red'>" + msg_incomp + "</font>")
                End If
                dtemp.Rows.Add(dr)
            Next
        End If

        If dtemp.Rows.Count > 0 Then
            GridViewRenewEmp.DataSource = dtemp
            GridViewRenewEmp.DataBind()
            lblpagemsg.Text = "Note: To display details of contract employee click on the Safety Pass number."
        Else
            lblpagemsg.Text = "Note: A click on the respective Employee Type,will open a section to fill employee details."
        End If

    End Sub
    Protected Sub lnk_Renew_spno_Click(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim loc As String = ""
        Dim gvrow As GridViewRow
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        gvrow = CType(sender, LinkButton).Parent.Parent
        Dim sp_no As String = CType(gvrow.FindControl("lnk_Renew_spno"), LinkButton).Text
        Dim category As String = GetCategorySafety(sp_no)
        Session("categorysaf") = category

        If tabcontainer1.Style("display") = "none" Then
            tabcontainer1.Style.Remove("display")
        End If
        Session("reqtype") = "Renew"


        Renewal_profile_details(sp_no)
        Renewal_address_details(sp_no)
        Renewal_nominee_details(sp_no)
        Renewal_quali_details(sp_no)
        Renewal_exp_details(sp_no)
        Renewal_skill_details(sp_no)
        Renewal_trn_details(sp_no)
        'Renewal_PV_details(sp_no)
        Renewal_AGEDRV_details(sp_no)
        'btnUpdateAddress.Visible = False
        'btnUpdateProfile.Visible = False
        'btnUpdateQual.Visible = False
        'btnUpdateNominee.Visible = False
        'btnUpdateExp.Visible = False
        'btnUpdateSkill.Visible = False
        'btnUpdateTraining.Visible = False
        'btnupdateage.Visible = False
        'de select police verification check box and update button id already approved''''
        checkrenewalapprove(sp_no, Session("requestnumber"))
        checkpvapprovedsprenew(sp_no)
        'cmbCategory.Enabled = False
        Btnreset.Visible = False

        ibtnClosesubmit.Enabled = False
        If Session("comp_code") Is Nothing Then
            Response.Redirect("http://tatasteel.co.in/")
        End If
        Try
            ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where acm_type='SKJNTVTI' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                ''''***** change to allow all type of request for interlocking and include 502 dept of 1000 location in this category*******'''
                If (Session("requestType") <> "SPR" And Session("requestType") <> "SPN") Or (Txtdeprt.Text.Trim = "502" And comp_cd = "1000") Then
                    chk_waive.Visible = False
                    drp_waiveoff.Visible = False
                    lbl_waiveoff.Visible = False
                    lbl_waivereason.Visible = False
                    chk_waive.Checked = True
                    drptypeassessment.Enabled = False
                    spn_msg.Visible = False
                    drptypeassessment.Visible = False
                    Label2.Visible = False
                    spn_type.Visible = False


                Else
                    chk_waive.Visible = True
                    drp_waiveoff.Visible = True
                    lbl_waiveoff.Visible = True
                    lbl_waivereason.Visible = True
                    drptypeassessment.Enabled = True
                    spn_msg.Visible = True
                    drptypeassessment.Visible = True
                    Label2.Visible = True
                    spn_type.Visible = True
                End If
            Else
                chk_waive.Visible = False
                drp_waiveoff.Visible = False
                lbl_waiveoff.Visible = False
                lbl_waivereason.Visible = False
                chk_waive.Checked = True
                drptypeassessment.Enabled = False
                spn_msg.Visible = False
                drptypeassessment.Visible = False
                Label2.Visible = False
                spn_type.Visible = False
            End If
            ShowhideRenewSkillCert()

        Catch ex As Exception

        End Try




    End Sub

    ''' <summary>
    '''  Added by Priyaraj on 29th Feb,2024 for redirection to profile page 
    ''' </summary>
    ''' <param name="sp_no"></param>
    ''' <param name="num"></param>
    Protected Sub lnk_Renew_spno_Click1(ByVal sp_no As String, ByVal num As Integer)
        Dim loc As String = String.Empty
        Dim gvrow As GridViewRow
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim category As String = GetCategorySafety(sp_no)
        Session("categorysaf") = category
        If tabcontainer1.Style("display") = "none" Then
            tabcontainer1.Style.Remove("display")
        End If
        Session("reqtype") = "Renew"



        Renewal_profile_details(sp_no)
        Renewal_address_details(sp_no)
        Renewal_nominee_details(sp_no)
        Renewal_quali_details(sp_no)
        Renewal_exp_details(sp_no)
        Renewal_skill_details(sp_no)
        Renewal_trn_details(sp_no)
        Renewal_AGEDRV_details(sp_no)
        checkrenewalapprove(sp_no, Session("requestnumber"))
        checkpvapprovedsprenew(sp_no)
        Btnreset.Visible = False

        ibtnClosesubmit.Enabled = False
        If Session("comp_code") Is Nothing Then
            Response.Redirect("http://tatasteel.co.in/")
        End If
        Try
            ls_sql = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where acm_type='SKJNTVTI' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", Session("Comp_code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                ''''***** change to allow all type of request for interlocking and include 502 dept of 1000 location in this category*******'''
                If (Session("requestType") <> "SPR" And Session("requestType") <> "SPN") Or (Txtdeprt.Text.Trim = "502" And comp_cd = "1000") Then
                    chk_waive.Visible = False
                    drp_waiveoff.Visible = False
                    lbl_waiveoff.Visible = False
                    lbl_waivereason.Visible = False
                    chk_waive.Checked = True
                    drptypeassessment.Enabled = False
                    spn_msg.Visible = False
                    drptypeassessment.Visible = False
                    Label2.Visible = False
                    spn_type.Visible = False


                Else
                    chk_waive.Visible = True
                    drp_waiveoff.Visible = True
                    lbl_waiveoff.Visible = True
                    lbl_waivereason.Visible = True
                    drptypeassessment.Enabled = True
                    spn_msg.Visible = True
                    drptypeassessment.Visible = True
                    Label2.Visible = True
                    spn_type.Visible = True
                End If


            Else
                chk_waive.Visible = False
                drp_waiveoff.Visible = False
                lbl_waiveoff.Visible = False
                lbl_waivereason.Visible = False
                chk_waive.Checked = True
                drptypeassessment.Enabled = False
                spn_msg.Visible = False
                drptypeassessment.Visible = False
                Label2.Visible = False
                spn_type.Visible = False

            End If
            If num = 0 Then
                tabcontainer1.ActiveTabIndex = 0

            ElseIf num = 1 Then
                tabcontainer1.ActiveTabIndex = 1

            ElseIf num = 2 Then
                tabcontainer1.ActiveTabIndex = 2

            ElseIf num = 3 Then
                tabcontainer1.ActiveTabIndex = 3

            ElseIf num = 4 Then
                tabcontainer1.ActiveTabIndex = 4

            ElseIf num = 5 Then
                tabcontainer1.ActiveTabIndex = 5

            ElseIf num = 8 Then
                tabcontainer1.ActiveTabIndex = 8

            Else
                tabcontainer1.ActiveTabIndex = 10
            End If
            ShowhideRenewSkillCert()
        Catch ex As Exception

        End Try

    End Sub
    Public Function checkrenewaleligible(ByVal spno As String, ByVal reqno As String) As String
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim status As String = "N"
        Try
            ls_sql = "select CET_SAFETY_PASSNO from t_cemp_details_TMP where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + reqno + "' and CET_PV_ISSUED_ON is not null and CET_PV_VALID_TILL is not null and CET_DOCVER_STATUS not in('R','I')"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                status = "Y"

            End If
        Catch ex As Exception

        End Try
        Return status
    End Function
    Public Sub checkrenewalapprove(ByVal spno As String, ByVal reqno As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Try
            ls_sql = "select CET_SAFETY_PASSNO from t_cemp_details_TMP where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + reqno + "' and CET_PV_ISSUED_ON is not null and CET_PV_VALID_TILL is not null and CET_DOCVER_STATUS not in('R','I') and CET_REQ_STATUS='C'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                btnSaveProfile.Visible = False
                btnUpdateProfile.Visible = False
                btnsaveage.Visible = False
                btnupdateage.Visible = False
                btnSaveQual.Visible = False
                btnUpdateQual.Visible = False
                btnSaveSkill.Visible = False
                btnUpdateSkill.Visible = False
                btnSaveTraining.Visible = False
                btnUpdateTraining.Visible = False
                btnSaveExp.Visible = False
                btnUpdateExp.Visible = False
                'btnsavepv.Visible = False
                'btnupdatepv.Visible = False
                btnSaveNominee.Visible = False
                btnUpdateNominee.Visible = False
                btnSaveAddress.Visible = False
                btnUpdateAddress.Visible = False
                btn_savevac.Visible = False
            End If
        Catch ex As Exception

        End Try
    End Sub
    Private Sub checkpvapprovedsprenew(ByVal spno As String)
        Dim ls_sql As String = String.Empty
        Dim dt As New DataTable
        Dim cmd As OracleCommand
        Dim i As Integer = 0
        Try
            ls_sql = "select CET_DOCVER_STATUS,CET_REQUEST_NO from t_cemp_details_tmp where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", spno))
            'cmd.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                'While i < dt.Rows.Count
                '    Dim status As String = dt.Rows(i).Item("CET_DOCVER_STATUS").ToString
                '    Dim reqfetch As String = dt.Rows(i).Item("CET_REQUEST_NO").ToString
                '    If status.Equals("C") Then
                '        For Each gvrow As GridViewRow In gvpv.Rows
                '            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectPV")
                '            If reqfetch = Session("requestnumber") Then
                '                chkbox.Enabled = False
                '            End If
                '        Next
                '    Else
                '        For Each gvrow As GridViewRow In gvpv.Rows
                '            Dim chkbox As CheckBox = gvrow.FindControl("chkSelectPV")
                '            Dim reqno As HiddenField = gvrow.FindControl("hdreqno")

                '            If reqno.Value = Session("requestnumber") Then
                '                chkbox.Enabled = True
                '            Else
                '                chkbox.Enabled = False
                '            End If
                '        Next
                '    End If
                '    i = i + 1
                'End While

            End If

        Catch ex As Exception

        End Try
    End Sub

#Region "encode string by 64bit"
    Public Shared Function b64encode(ByVal StrEncode As String) As String

        Dim encodedString As String

        encodedString = (Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(StrEncode)))

        Return (encodedString)

    End Function
    Public Shared Function b64decode(ByVal StrDecode As String) As String
        Dim decodedString As String
        decodedString = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(StrDecode))
        Return decodedString
    End Function
#End Region
    Protected Sub lnkNoti_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles lnkNoti.Click
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Try
            ls_sql = "select A.CET_REQUEST_NO, A.CET_SAFETY_PASSNO,decode(B.SDV_VERF_TYPE,'PV','Police Verification','AP','Address Verification','QP','Qualification','AG','Age Proof','DL','Driving Licence','PA','Passport Verification','Skill','Skill Verification','TR','Training Verification','EX','Experice Verification') verifytype, B.SDV_REMARKS from t_cemp_details_tmp a,t_sp_doc_verification b where A.CET_REQUEST_NO=B.SDV_REQ_NO and a.CET_SAFETY_PASSNO=B.SDV_SAFETYPASS_NO and A.CET_VENDOR_CODE=:CET_VENDOR_CODE and B.SDV_VERF_FLAG='N'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_VENDOR_CODE", Session("VendCode")))
            dt = getRecord(cmd, con)
            Dim dtview As New DataView(dt)
            If dt.Rows.Count > 0 Then
                grd_noti.DataSource = dt
                grd_noti.DataBind()
                gvReq.Visible = False
                grd_noti.Visible = True
                btn_downloadnoti.Visible = True
                lblpagemsg.Text = ""
            Else
                ShowMessage("No notification yet.")
            End If

        Catch ex As Exception

        End Try
    End Sub
    Protected Sub btn_downloadnoti_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn_downloadnoti.Click
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim gridview1 As New GridView
        Try
            ls_sql = "select A.CET_REQUEST_NO ""Request Number"", A.CET_SAFETY_PASSNO ""Safetypass Number"",decode(B.SDV_VERF_TYPE,'PV','Police Verification','AP','Address Verification','QP','Qualification','AG','Age Proof','DL','Driving Licence','PA','Passport Verification','Skill','Skill Verification','TR','Training Verification','EX','Experice Verification') ""Document Type"", B.SDV_REMARKS ""Approver Remarks"" from t_cemp_details_tmp a,t_sp_doc_verification b,t_sp_request c  where A.CET_REQUEST_NO=B.SDV_REQ_NO and a.CET_SAFETY_PASSNO=B.SDV_SAFETYPASS_NO and A.CET_VENDOR_CODE=:CET_VENDOR_CODE and B.SDV_VERF_FLAG='N' "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_VENDOR_CODE", Session("VendCode")))
            dt = getRecord(cmd, con)
            'Dim dtview As New DataView(dt)

            gridview1.DataSource = dt
            gridview1.DataBind()
            Response.Clear()
            Response.Buffer = True
            Response.AddHeader("content-disposition", "attachment;filename=Notification.xls")
            Response.Charset = ""
            Response.ContentType = "application/vnd.ms-excel"
            Dim sw As New StringWriter()
            Dim hw As New HtmlTextWriter(sw)
            gridview1.RenderControl(hw)
            Response.Output.Write(sw.ToString())
            Response.Flush()
            Response.End()
        Catch ex As Exception
        End Try

    End Sub

    'sandeep
    Protected Sub txtDOB_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtDOB.TextChanged
        If txtDOB.Text <> "" And txtDOB.Text <> "__/__/____" And txtDOB.Text <> "__-__-____" Then
            Dim db As String = txtDOB.Text.Replace("-", "/")

            Dim dob As Date = DateTime.ParseExact(db, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            Dim age As Double = GetAge(dob)
            Dim trainee As String = cmbCategory.Items(0).Value.Substring(0, 1) + "A"
            If age >= 18 And age <= 20 Then
                cmbCategory.Items.FindByValue(trainee).Enabled = True
                cmbCategory.SelectedValue = trainee
                cmbCategory.Enabled = False
            ElseIf age < 18 Then
                ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " & Session("comp_name_d") & " .")
                txtDOB.Text = ""
            Else
                cmbCategory.Items.FindByValue(trainee).Enabled = False
                cmbCategory.Enabled = True
            End If
        End If
    End Sub
    Private Function GetAge(ByVal dob As Date) As Double
        Dim today As Date = Date.Today
        Dim fullYears As Integer = today.Year - dob.Year

        If (dob.Month > today.Month) OrElse (dob.Month = today.Month AndAlso dob.Day > today.Day) Then
            fullYears -= 1
        End If

        Dim birthdayThisCycle As Date = dob.AddYears(fullYears)

        Dim nextBirthdayCycle As Date = dob.AddYears(fullYears + 1)


        Dim totalDaysInAgeYearCycle As Double = (nextBirthdayCycle - birthdayThisCycle).TotalDays


        Dim daysLivedInCurrentAgeYear As Double = (today - birthdayThisCycle).TotalDays


        Dim fractionalYears As Double = 0.0
        If totalDaysInAgeYearCycle > 0 Then
            fractionalYears = daysLivedInCurrentAgeYear / totalDaysInAgeYearCycle
        End If

        Return Math.Round(fullYears + fractionalYears, 2)
    End Function

    Private Function GetMaxAge() As Integer
        Try
            Dim maxAge As Integer = 60
            Dim ls_sql As String = "SELECT at.ACM_TYPE,at.ACM_CATEGORY FROM HRACE.t_cwm_action_mapping at where at.ACM_TYPE = 'MXAGE' and at.ACM_FLAG = 'Y' AND at.ACM_COMPANY_CODE = :companyCode"
            Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":companyCode", comp_cd))
            Dim dt As DataTable = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                maxAge = dt.Rows(0).Item("ACM_CATEGORY").ToString.Trim
            End If
            Return maxAge
        Catch ex As Exception
        End Try
    End Function

    Protected Sub btnConfirmDocSubmision_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnConfirmDocSubmision.Click
        If (hfActionPerformed.Value = "S") Then
            SaveProfile()
        ElseIf (hfActionPerformed.Value = "U") Then
            UpdateProfile()
        ElseIf (hfActionPerformed.Value = "A") Then
            AddSafetyPass()
        End If
    End Sub
    Protected Sub btnCancelDocSubmisio_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCancelDocSubmisio.Click
        pnlConfirmDocSubmision.Visible = False
        MPopUpConfirmDocSubmision.Hide()
    End Sub
    Protected Sub btnYes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnYes.Click
        Dim spno As String = String.Empty
        Dim req_no As String = String.Empty

        spno = hndSPNo.Value.ToString
        req_no = hndRNo.Value.ToString

        UpdateRejectRequest(spno, req_no)

    End Sub
    Protected Sub btnNo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnNo.Click

    End Sub
    Protected Sub btnrepplyYes_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnrepplyYes.Click
        Dim spno As String = String.Empty
        Dim req_no As String = String.Empty

        spno = hndSPNo.Value.ToString
        req_no = hndRNo.Value.ToString

        UpdateRejectRequest(spno, req_no)
    End Sub

    Protected Sub btnrepplyNo_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnrepplyNo.Click

    End Sub


    ''' <summary>
    ''' Added the logic of blinking in the reject button .
    ''' </summary>
    Public Sub rejectStatus()
        Dim lnk_sp As New LinkButton
        Dim btnrej As New Button
        Dim ls_sql As String = String.Empty
        Dim cnt As Integer = 0
        Dim dt As New DataTable()
        Dim req_no = String.Empty
        For Each gv As GridViewRow In GridViewEmp.Rows
            lnk_sp = gv.FindControl("lnk_spno")
            btnrej = gv.FindControl("btnRejectSPReq")
            Dim spno As String = lnk_sp.Text
            req_no = lblreq.Text.Split(":")(1)
            Try
                ls_sql = "select CET_DOCVER_STATUS,CET_REQ_STATUS from t_cemp_details_tmp where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + req_no + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                dt = getRecord(ls_sql, con)
                If dt.Rows(0).Item("CET_REQ_STATUS").ToString.Trim <> "" Then
                    btnrej.Enabled = False
                    btnrej.CssClass = "btnStyle"
                Else
                    If dt.Rows(0).Item("CET_DOCVER_STATUS").ToString.Trim.Equals("I") Then
                        btnrej.Enabled = True
                    Else
                        btnrej.Enabled = False
                        btnrej.CssClass = "btnStyle"
                    End If

                End If
                If (btnrej.Enabled) Then
                    Rejmsg.Visible = True
                End If
            Catch ex As Exception

            End Try
        Next
    End Sub
    ''' <summary>
    ''' Code added for removing the pv entry
    ''' Added By vishal(2602256):calculating locking period from Created date to current date for Fail candidate
    ''' TCS2164315 (23/02/2024) Addititon of reapply attempt count check and the locking period check and condition for reapply option check.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub GridViewEmp_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles GridViewEmp.RowCommand
        Dim ls_sql As String = String.Empty
        Dim ls_sql1 As String = String.Empty
        Dim ls_sql_pv As String = String.Empty
        Dim spno As String = String.Empty
        Dim req_no As String = String.Empty
        Dim arr_cmd As New ArrayList()
        Dim cmd As New OracleCommand
        Dim cmd1 As New OracleCommand
        Dim cmd2 As New OracleCommand
        Dim Period As Int32
        Dim buttton_reapply As Button = GridViewEmp.FindControl("btnReapply")

        If e.CommandName = "REJECT_SP_REQ" Then
            spno = e.CommandArgument.ToString
            req_no = lblreq.Text.Split(":")(1)
            hndSPNo.Value = spno
            hndRNo.Value = req_no

            Try
                spno = e.CommandArgument.ToString
                req_no = lblreq.Text.Split(":")(1)
                Dim lockingperiodflag As Boolean = False
                Dim reapplyingflag As Boolean = False




                If iscompanycodesforreapplyprovision(Session("Comp_code").ToString) Then


                    Dim lssqlskilldata As String = "select * from hrace.t_cwm_cemp_skill_tmp where CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_REQ_NO=:CCST_REQ_NO and CCST_COMP_CODE=:CCST_COMP_CODE"

                    Dim dtqryskilldata As New DataTable
                    Dim cmdskilldata As OracleCommand = New OracleCommand(lssqlskilldata, con)
                    cmdskilldata.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", spno))
                    cmdskilldata.Parameters.Add(New OracleParameter(":CCST_REQ_NO", req_no))
                    cmdskilldata.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", Session("Comp_code").ToString))
                    dtqryskilldata = getRecord(cmdskilldata, con)
                    If dtqryskilldata.Rows.Count > 0 Then
                        For Each dtskilldata In dtqryskilldata.Rows
                            Dim tradecd As String = dtskilldata("CCST_SKTD_CP_CD").ToString
                            Dim skilltypecode As String = dtskilldata("CCST_SKILL_TYPE_CD").ToString
                            Dim skillcode As String = dtskilldata("CCST_SKILL_CD").ToString
                            Dim lockingPeriod As Int32 = GetSafetyPassResult(spno, req_no, tradecd, Session("Comp_code").ToString)
                            Period = getLockingDays(Session("Comp_code").ToString)
                            Dim counter As String = getassessmentcounter(spno, req_no, tradecd, Session("Comp_code").ToString)
                            If lockingPeriod <= Period And lockingPeriod > 0 And Period > 0 And counter < 3 Then
                                lockingperiodflag = True
                                Exit For
                            ElseIf lockingPeriod > Period And Period > 0 Then
                                If counter < 3 And reapplyskillcheck(spno, req_no, skilltypecode, skillcode, tradecd, Session("Comp_code").ToString) = "N" Then
                                    reapplyingflag = True
                                    Exit For
                                End If
                            ElseIf lockingPeriod = 0 And isresultpresentoffail(spno) Then
                                reapplyingflag = True
                                Exit For
                            Else

                            End If
                        Next

                    End If
                    If lockingperiodflag = True Then
                        ShowMessage("Selected Safety Pass No is under locking period of " + Session("lockingdays") + " days. You can Reapply after locking period.  Rejection cannot be done.")
                        Return

                    ElseIf reapplyingflag = True Then
                        pnlConfirmReapplyReq.Visible = True
                        ModalPopupConfirmReapplyReq.Show()

                    Else

                        pnlConfirmRejectReq.Visible = True
                        ModalPopupConfirmRejectReq.Show()
                    End If

                Else
                    If iscompanycodesforreapplyprovision(Session("Comp_code").ToString) = False Then

                        UpdateRejectRequest(spno, req_no)

                    End If
                End If

                ''--End



            Catch ex As Exception
                ShowMessage("Error occurs operation reverted")
            End Try
        End If

    End Sub
    Public Function UpdateRejectRequest(ByVal spno As String, ByVal req_no As String) As String
        Dim ls_sql As String = String.Empty
        Dim ls_sql1 As String = String.Empty
        Dim ls_sql_pv As String = String.Empty
        'Dim spno As String = String.Empty
        'Dim req_no As String = String.Empty
        Dim arr_cmd As New ArrayList()
        Dim cmd As New OracleCommand
        Dim cmd1 As New OracleCommand
        Dim cmd2 As New OracleCommand

        ls_sql = "update t_cemp_details_tmp set CET_REQ_STATUS='R',CET_MODIFIED_BY='" + Session("VendCode") + "',CET_MODIFIED_DATE=sysdate where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + req_no + "'"
        If con.State = ConnectionState.Closed Then
            con.Open()
        End If
        cmd.Connection = con
        cmd.CommandText = ls_sql
        arr_cmd.Add(cmd)
        ''''''''''' check request type''''''''''''''''''''''''''''''
        'WI9047:Remove biometric details if request has been rejected for new cases
        Dim companycd As String = String.Empty
        companycd = getAragyaCompLoc(Session("Comp_code"))
        If companycd = "Y" Then
            If Session("requestType") = "SPN" Then
                ls_sql = "delete from t_cemp_photo where CEP_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from photo table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_CEMP_ADDRS where CCA_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from address table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_CEMP_NOMINEES  where CCN_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from nominees table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_CEMP_QUALIFICATIONS  where CQL_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from qualification table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_CEMP_SKILL  where CCS_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from Skill table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_CEMP_TRNS where CCT_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from Trans table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_EXP  where CWE_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from exp table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql = "delete from T_CWM_CEMP_MEDICAL_DTL   where CCM_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from medical table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                ls_sql1 = "delete from HRACE.t_cwm_cemp_medical_hdr where CMH_SAFETY_PASS_NO ='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd1 = New OracleCommand(ls_sql1, con)
                cmd1.Connection = con
                cmd1.CommandText = ls_sql1
                arr_cmd.Add(cmd1)

                'Code added for removing the pv entry.
                ls_sql_pv = "delete from hrace.T_CWM_PV_DTL where CPD_SAFETY_PASS_NO ='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql_pv, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql_pv
                arr_cmd.Add(cmd)


                ls_sql = "delete from t_cemp_details where CED_SAFETY_PASS_NO='" + spno + "'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)
                ls_sql = "insert into T_REQ_REJ_AUDIT(REQ_SP_NO,REQ_NO,REQ_ACTION,REQ_CREATED_BY,REQ_CREATED_DATE) values('" + spno + "','" + req_no + "','Deletion of data from details table due to rejection of new safety pass request','" + Session("VendCode") + "',sysdate)"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Connection = con
                cmd.CommandText = ls_sql
                arr_cmd.Add(cmd)

                'WI9047: End of code

            End If

            ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        End If

        If Session("requestType") = "SPR" Then
            ls_sql = " update HRACE.T_CEMP_DETAILS set CED_REQ_NO='' where CED_SAFETY_PASS_NO='" + spno + "'"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd.Connection = con
            cmd.CommandText = ls_sql
            arr_cmd.Add(cmd)
        End If

        If arr_cmd.Count > 0 Then
            Dim counter As Integer = 0
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            Dim tran_Ins As OracleTransaction
            tran_Ins = con.BeginTransaction()
            Try
                For counter = 0 To arr_cmd.Count - 1
                    Dim con_ins As New OracleCommand()
                    con_ins = arr_cmd.Item(counter)
                    con_ins.Transaction = tran_Ins
                    con_ins.ExecuteNonQuery()
                Next
                tran_Ins.Commit()
                empView()
                ShowMessage("Record has been rejected successfully")
            Catch ex As Exception
                tran_Ins.Rollback()
                ShowMessage("Error occurs operation reverted")

            Finally
                If con.State = ConnectionState.Open Then
                    con.Close()
                End If
            End Try
        End If
        ' Response.Write("<script>alert('Record has beeen rejected successfully');</script>")
        Return ""
    End Function

    ''' <summary>
    ''' TCS2164315 (23/02/2024) condition check for reapply attempt of safety pass number.
    ''' </summary>
    ''' <param name="spno"></param>
    ''' <param name="reqno"></param>
    ''' <param name="tradecode"></param>
    ''' <param name="compcode"></param>
    ''' <returns></returns>
    Public Function getassessmentcounter(ByVal spno As String, ByVal reqno As String, ByVal tradecode As String, ByVal compcode As String) As Int32
        Dim lssql As String = "select RSH_ATTEMPT_COUNT from hrace.T_REAPPLY_SKILL_HIST where RSH_SP_NO=:RSH_SP_NO and RSH_SP_REQ_NO=:RSH_SP_REQ_NO and RSH_COMP_CODE=:RSH_COMP_CODE and RSH_TRADE_CD=:RSH_TRADE_CD"
        Dim counter As Integer = 0
        Dim cmdcounter = New OracleCommand(lssql, con)
        cmdcounter.Parameters.Add(New OracleParameter(":RSH_SP_NO", spno))
        cmdcounter.Parameters.Add(New OracleParameter(":RSH_SP_REQ_NO", reqno))
        cmdcounter.Parameters.Add(New OracleParameter(":RSH_TRADE_CD", tradecode))
        cmdcounter.Parameters.Add(New OracleParameter(":RSH_COMP_CODE", compcode))
        Dim dtcounter As New DataTable
        dtcounter = getRecord(cmdcounter, con)
        If dtcounter.Rows.Count > 0 Then
            If (Not IsDBNull(dtcounter.Rows(0).Item("RSH_ATTEMPT_COUNT"))) Then
                counter = dtcounter.Rows(0).Item("RSH_ATTEMPT_COUNT")
                If (counter = 0) Then
                    counter = 1
                End If
            End If
        End If
        Return counter
    End Function
    Public Function getAragyaCompLoc(vcompcode As Object) As String
        Dim st As String = "N"
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt1 As New DataTable
        Try
            ls_sql = "select ACM_TYPE from t_cwm_action_mapping where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_FLAG='Y' and ACM_TYPE='RR'"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", vcompcode))
            dt1 = getRecord(cmd, con)

            If dt1.Rows.Count > 0 Then
                st = "Y"
            Else
                st = "N"
            End If
        Catch ex As Exception

        End Try
        Return st
    End Function



    Protected Sub GridViewRenewEmp_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles GridViewRenewEmp.RowCommand
        Dim ls_sql As String = String.Empty
        Dim spno As String = String.Empty
        Dim req_no As String = String.Empty
        Dim cmd As New OracleCommand
        Dim cmd1 As New OracleCommand
        Dim dt As New DataTable()
        Dim btnrerej As New Button
        Dim arr_cmd As New ArrayList()
        If e.CommandName = "REJECT_SP_RE_REQ" Then
            spno = e.CommandArgument.ToString
            req_no = lblreq.Text.Split(":")(1)
            Try
                ''''''''
                ls_sql = "select CET_REQ_STATUS,CET_DOCVER_STATUS from t_cemp_details_tmp where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + req_no + "'"
                'CET_DOCVER_STATUS,
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                dt = getRecord(ls_sql, con)
                If dt.Rows.Count > 0 Then


                    'If dt.Rows(0).Item("CET_REQ_STATUS").ToString.Trim <> "" Then
                    If dt.Rows(0).Item("CET_REQ_STATUS").ToString.Trim.Equals("R") Then
                        ShowMessage("Request Already rejected. Rejection of request is not possible")
                        Exit Sub

                    Else
                        If dt.Rows(0).Item("CET_REQ_STATUS").ToString.Trim.Equals("C") Then
                            ShowMessage("Request Already completed. Rejection of request is not possible")
                            Exit Sub

                            '        btnrerej.Enabled = True
                            'If dt.Rows(0).Item("CET_REQ_STATUS").ToString.Trim.Equals("R") Then
                            'btnrerej.Enabled = False
                            'btnrerej.Enabled = False
                            'Else
                            'btnrerej.Enabled = True
                        End If
                        If dt.Rows(0).Item("CET_DOCVER_STATUS").trim = "C" Then
                            ShowMessage("Document Verification has beem completed. Rejection of request is not possible")
                            Exit Sub
                        End If
                    End If

                    '''''''''

                    ls_sql = "update t_cemp_details_tmp set CET_REQ_STATUS='R',CET_MODIFIED_BY='" + Session("VendCode") + "',CET_MODIFIED_DATE=sysdate where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + req_no + "' and CET_DOCVER_STATUS <>'C'"
                    If con.State = ConnectionState.Closed Then
                        con.Open()

                    End If


                    cmd.Connection = con
                    cmd.CommandText = ls_sql

                    arr_cmd.Add(cmd)
                    ls_sql = "update t_cemp_details set ced_req_no=NULL where ced_safety_pass_no=:ced_safety_pass_no"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    cmd1 = New OracleCommand(ls_sql, con)
                    cmd1.Parameters.Add(New OracleParameter("ced_safety_pass_no", spno.Trim))
                    arr_cmd.Add(cmd1)

                    If arr_cmd.Count > 0 Then
                        Dim counter As Integer = 0
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim tran_Ins As OracleTransaction
                        tran_Ins = con.BeginTransaction()
                        Try
                            For counter = 0 To arr_cmd.Count - 1
                                Dim con_ins As New OracleCommand()
                                con_ins = arr_cmd.Item(counter)
                                con_ins.Transaction = tran_Ins
                                con_ins.ExecuteNonQuery()
                            Next
                            tran_Ins.Commit()

                            ShowMessage("Request has been rejected successfully")


                            '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

                        Catch ex As Exception
                            tran_Ins.Rollback()
                            ShowMessage("Error occurs during request rejection")
                        Finally
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Try
                    End If
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If

                End If

                empView()


            Catch ex As Exception
                ShowMessage("Error occurs operation reverted")
            End Try
        End If


    End Sub
    ''' <summary>
    ''' Added logic of blinking feature for renewal safety pass cases.
    ''' </summary>
    Public Sub rejectReStatus()
        Dim lnk_Renew_spno As New LinkButton
        Dim btnrerej As New Button

        Dim ls_sql As String = String.Empty
        Dim cnt As Integer = 0
        Dim dt As New DataTable()
        Dim req_no = String.Empty
        For Each gv As GridViewRow In GridViewRenewEmp.Rows
            lnk_Renew_spno = gv.FindControl("lnk_Renew_spno")
            btnrerej = gv.FindControl("btnRejectSPReReq")

            Dim spno As String = lnk_Renew_spno.Text
            req_no = lblreq.Text.Split(":")(1)
            Try
                ls_sql = "select CET_DOCVER_STATUS from t_cemp_details_tmp where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + req_no + "' and CET_DOCVER_STATUS in('C')"

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                dt = getRecord(ls_sql, con)
                If dt.Rows.Count > 0 Then
                    btnrerej.Enabled = False
                    btnrerej.CssClass = "btnStyle"
                End If
                If (btnrerej.Enabled) Then
                    Rejmsg.Visible = True
                End If

            Catch ex As Exception

            End Try
        Next
    End Sub
    Protected Sub downloadqual(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='QUAL'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadexp(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='EXP'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadtrn(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='TRN'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadfit(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='DFC'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadun(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='UTC'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadwcc(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='WCC'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadskill(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='SKILL'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadage(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='AGE'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloaddrv(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='DRV'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadpass(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='PASS'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadadd(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID "
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub downloadpv(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = Convert.ToInt64(ls_lnk.CommandArgument)
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='PV'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub
    Protected Sub btnsaveage_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsaveage.Click
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim filenameage As String = String.Empty
        Dim filenamedrv As String = String.Empty
        Dim filenamepass As String = String.Empty
        Try
            If fupdldrv.HasFile = True And chkdriverold.Checked = True Then
                ShowMessage("choose either file upload or check previous upload documents option for driving licence")
                Exit Sub
            End If
            If fupdlpass.HasFile = True And chkpassold.Checked = True Then
                ShowMessage("choose either file upload or check previous upload documents option for passport documents")
                Exit Sub
            End If
            If fupdlage.HasFile = False And chkageold.Checked = False Then
                ShowMessage("Please Upload Age Proof")
                Exit Sub
            ElseIf fupdlage.HasFile = True And chkageold.Checked = True Then
                ShowMessage("choose either file upload or check previous upload documents option for age proof")
                Exit Sub
            ElseIf fupdlage.HasFile = True And chkageold.Checked = False Then
                filenameage = Path.GetFileName(fupdlage.PostedFile.FileName)
                Dim contentType As String = fupdlage.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdlage.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (fupdlage.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
            End If
            If fupdldrv.HasFile = False And chkdriverold.Checked = False Then
                If Session("categorysaf").ToString.Substring(0, 1).Equals("D") Then
                    ShowMessage("Please Upload Driving License")
                    Exit Sub
                End If
                'ShowMessage("Please Upload Driving License")
                'Exit Sub
            ElseIf fupdldrv.HasFile = True And chkdriverold.Checked = False Then
                filenamedrv = Path.GetFileName(fupdldrv.PostedFile.FileName)
                Dim contentType As String = fupdldrv.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdldrv.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (fupdldrv.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
            End If
            If fupdlpass.HasFile = False Then

            ElseIf fupdlpass.HasFile = True And chkpassold.Checked = False Then
                filenamepass = Path.GetFileName(fupdlpass.PostedFile.FileName)
                Dim contentType As String = fupdlpass.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdlpass.PostedFile.ContentLength > 512000) Then
                        ShowMessage("Your file size is " + (fupdlpass.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
            End If
            Dim ageid As String = TrnCWEAgeDrvSeqNo("")
            Dim drvid As String = "0"
            Dim passid As String = "0"
            If fupdldrv.HasFile = True Or chkdriverold.Checked = True Then
                drvid = TrnCWEAgeDrvSeqNo("")
            Else
                drvid = "0"
            End If
            If fupdlpass.HasFile = True Or chkpassold.Checked = True Then
                passid = TrnCWEAgeDrvSeqNo("")
            Else
                passid = "0"
            End If
            ls_sql = "update T_CEMP_DETAILS_TMP set CET_DOB_CERT_NO=:CET_DOB_CERT_NO,CET_DRV_CERT_NO=:CET_DRV_CERT_NO,CET_PASS_CERT_NO=:CET_PASS_CERT_NO where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CET_DOB_CERT_NO", ageid))
            cmd.Parameters.Add(New OracleParameter(":CET_DRV_CERT_NO", drvid))
            cmd.Parameters.Add(New OracleParameter(":CET_PASS_CERT_NO", passid))
            cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
            cmd.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
            cmd.ExecuteNonQuery()
            If fupdlage.HasFile = True Then
                Dim cmdfileage As New OracleCommand
                ls_sql = String.Empty
                filenameage = Path.GetFileName(fupdlage.PostedFile.FileName)
                Using fs As Stream = fupdlage.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileage.CommandText = ls_sql
                        cmdfileage.Connection = con
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_DOC_ID", ageid))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_NAME", filenameage))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "AGE"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileage.ExecuteNonQuery()
                        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "AG")
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            ElseIf chkageold.Checked = True Then
                Dim cmdfileage As New OracleCommand
                ls_sql = String.Empty
                filenameage = Path.GetFileName(fupdlage.PostedFile.FileName)
                Using fs As Stream = fupdlage.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) "
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid"
                        '  ls_sql = ls_sql + "VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate)"
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileage.CommandText = ls_sql
                        cmdfileage.Connection = con
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_DOC_ID", ageid))
                        cmdfileage.Parameters.Add(New OracleParameter(":olddocid", hdfageold.Value))
                        ' cmdfileage.Parameters.Add(New OracleParameter(":DM_NAME", filenameage))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "AGE"))
                        ' cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileage.ExecuteNonQuery()
                        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "AG")
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If

            If fupdldrv.HasFile = True Then
                Dim cmdfiledrv As New OracleCommand
                ls_sql = String.Empty
                filenamedrv = Path.GetFileName(fupdldrv.PostedFile.FileName)
                Using fs As Stream = fupdldrv.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfiledrv.CommandText = ls_sql
                        cmdfiledrv.Connection = con
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_DOC_ID", drvid))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_NAME", filenamedrv))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "DRV"))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfiledrv.ExecuteNonQuery()
                        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "DL")
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using

            ElseIf chkdriverold.Checked = True Then
                Dim cmdfileage As New OracleCommand
                ls_sql = String.Empty
                filenameage = Path.GetFileName(fupdldrv.PostedFile.FileName)
                Using fs As Stream = fupdldrv.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) "
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid"
                        '  ls_sql = ls_sql + "VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate)"
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileage.CommandText = ls_sql
                        cmdfileage.Connection = con
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_DOC_ID", drvid))
                        cmdfileage.Parameters.Add(New OracleParameter(":olddocid", hdfdriverold.Value))
                        ' cmdfileage.Parameters.Add(New OracleParameter(":DM_NAME", filenameage))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "DRV"))
                        ' cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileage.ExecuteNonQuery()
                        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "DL")
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If

            If fupdlpass.HasFile = True Then
                Dim cmdfilepass As New OracleCommand
                ls_sql = String.Empty
                filenamepass = Path.GetFileName(fupdlpass.PostedFile.FileName)
                Using fs As Stream = fupdlpass.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfilepass.CommandText = ls_sql
                        cmdfilepass.Connection = con
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_DOC_ID", passid))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_NAME", filenamepass))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "PASS"))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfilepass.ExecuteNonQuery()
                        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "PA")
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            ElseIf chkpassold.Checked = True Then
                Dim cmdfileage As New OracleCommand
                ls_sql = String.Empty
                filenameage = Path.GetFileName(fupdlpass.PostedFile.FileName)
                Using fs As Stream = fupdlpass.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) "
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid"
                        '  ls_sql = ls_sql + "VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate)"
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileage.CommandText = ls_sql
                        cmdfileage.Connection = con
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_DOC_ID", passid))
                        cmdfileage.Parameters.Add(New OracleParameter(":olddocid", hdfpassold.Value))
                        ' cmdfileage.Parameters.Add(New OracleParameter(":DM_NAME", filenameage))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "PASS"))
                        ' cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileage.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileage.ExecuteNonQuery()
                        updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "PA")
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If
            getagedrv(TxtSpno.Text)
            If Session("reqtype") = "Renew" Then
                For Each gvrow As GridViewRow In grdage.Rows
                    Dim chkbox As CheckBox = gvrow.FindControl("chkSelectage")
                    Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                    If reqno.Value.Trim = Session("requestnumber").ToString Then
                        chkbox.Enabled = True
                    Else
                        chkbox.Enabled = False
                    End If
                Next
            End If
            empView()
        Catch ex As Exception

        End Try
    End Sub
    Public Function TrnCWEAgeDrvSeqNo(ByVal id As String) As String
        Dim vageSeqNo As String = ""
        Dim sqlageSeqNo As String = "Select (HRACE.SEQ_CEMP_AGE_DRV_ID.nextval) SEQNO from dual "
        Dim dtageSeqNo As New DataTable()
        dtageSeqNo = getRecord(sqlageSeqNo, con)
        If dtageSeqNo.Rows.Count > 0 Then
            vageSeqNo = dtageSeqNo.Rows(0)("SEQNO")
        End If

        dtageSeqNo.Dispose()
        Return vageSeqNo

    End Function
    Public Function TrnCWEMedSeqNo(ByVal id As String) As String
        Dim vmedSeqNo As String = ""
        Dim sqlmedSeqNo As String = "Select (HRACE.SEQ_CEMP_MED_ID.nextval) SEQNO from dual "
        Dim dtmedSeqNo As New DataTable()
        dtmedSeqNo = getRecord(sqlmedSeqNo, con)
        If dtmedSeqNo.Rows.Count > 0 Then
            vmedSeqNo = dtmedSeqNo.Rows(0)("SEQNO")
        End If

        dtmedSeqNo.Dispose()
        Return vmedSeqNo

    End Function
    Protected Sub btnupdateage_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnupdateage.Click
        Dim agestatus As String = "N"
        Dim drvstatus As String = "N"
        Dim passstatus As String = "N"
        If fupdlage.HasFile = True Then


            Dim contentType As String = fupdlage.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdlage.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdlage.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If
        If fupdldrv.HasFile = True Then


            Dim contentType As String = fupdldrv.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdldrv.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdldrv.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        If fupdlpass.HasFile = True Then


            Dim contentType As String = fupdlpass.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdlpass.PostedFile.ContentLength > 512000) Then
                    ShowMessage("Your file size is " + (fupdlpass.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        If fupdlage.HasFile = True Then
            agestatus = "Y"
            Dim cmdfileage As New OracleCommand
            Using fs As Stream = fupdlage.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    Dim filename As String = Path.GetFileName(fupdlage.PostedFile.FileName)

                    Dim ls_sql As String = "update T_DOCUMENT_MASTER Set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                    If con.State = ConnectionState.Closed Then
                        con.Open()

                    End If
                    cmdfileage.CommandText = ls_sql
                    cmdfileage.Connection = con
                    cmdfileage.Parameters.Add(New OracleParameter(":DM_DOC_ID", hiddob.Value))
                    cmdfileage.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                    cmdfileage.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                    'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                    cmdfileage.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                    cmdfileage.ExecuteNonQuery()
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If

                End Using
            End Using
        End If

        If fupdldrv.HasFile = True Then
            drvstatus = "Y"
            Dim ls_sql As String = String.Empty
            Dim cmdfiledrv As New OracleCommand
            Using fs As Stream = fupdldrv.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    Dim filename As String = Path.GetFileName(fupdldrv.PostedFile.FileName)
                    If hiddrv.Value.Trim = "" Or hiddrv.Value.Trim = "0" Then
                        hiddrv.Value = TrnCWEAgeDrvSeqNo("")
                        ls_sql = "update T_CEMP_DETAILS_TMP set CET_DRV_CERT_NO=:CET_DRV_CERT_NO where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmdfiledrv = New OracleCommand(ls_sql, con)
                        cmdfiledrv.Parameters.Add(New OracleParameter(":CET_DRV_CERT_NO", hiddrv.Value))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
                        cmdfiledrv.ExecuteNonQuery()
                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If

                        cmdfiledrv = New OracleCommand(ls_sql, con)
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_DOC_ID", hiddrv.Value))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "DRV"))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfiledrv.ExecuteNonQuery()
                    Else
                        ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfiledrv = New OracleCommand(ls_sql, con)
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_DOC_ID", hiddrv.Value))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfiledrv.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfiledrv.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If

                    End If

                End Using
            End Using
        End If

        If fupdlpass.HasFile = True Then
            passstatus = "Y"
            Dim cmdfilepass As New OracleCommand
            Dim ls_sql As String = String.Empty
            Using fs As Stream = fupdlpass.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    Dim filename As String = Path.GetFileName(fupdlpass.PostedFile.FileName)
                    If hidpass.Value.Trim = "" Or hidpass.Value.Trim = "0" Then
                        hidpass.Value = TrnCWEAgeDrvSeqNo("")
                        ls_sql = "update T_CEMP_DETAILS_TMP set CET_PASS_CERT_NO=:CET_PASS_CERT_NO where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        cmdfilepass = New OracleCommand(ls_sql, con)
                        cmdfilepass.Parameters.Add(New OracleParameter(":CET_PASS_CERT_NO", hidpass.Value))
                        cmdfilepass.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim))
                        cmdfilepass.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", Session("requestnumber")))
                        cmdfilepass.ExecuteNonQuery()
                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfilepass = New OracleCommand(ls_sql, con)
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidpass.Value))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "PASS"))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfilepass.ExecuteNonQuery()
                    Else
                        ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfilepass.CommandText = ls_sql
                        cmdfilepass.Connection = con
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_DOC_ID", hidpass.Value))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfilepass.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfilepass.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End If


                End Using
            End Using
        End If
        If agestatus = "Y" Then
            Dim ls_chkage As String = String.Empty
            Dim cmd_chkage As OracleCommand
            Dim dt_chkage As New DataTable
            Try
                ls_chkage = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='AG' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkage = New OracleCommand(ls_chkage, con)
                cmd_chkage.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkage.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim))
                dt_chkage = getRecord(cmd_chkage, con)
                If dt_chkage.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "AG")
                End If
            Catch ex As Exception

            End Try

        End If
        If drvstatus = "Y" Then
            Dim ls_chkDL As String = String.Empty
            Dim cmd_chkDL As OracleCommand
            Dim dt_chkDL As New DataTable
            Try
                ls_chkDL = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='DL' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkDL = New OracleCommand(ls_chkDL, con)
                cmd_chkDL.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkDL.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim))
                dt_chkDL = getRecord(cmd_chkDL, con)
                If dt_chkDL.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "DL")
                End If
            Catch ex As Exception

            End Try


        End If
        If passstatus = "Y" Then
            Dim ls_chkPA As String = String.Empty
            Dim cmd_chkPA As OracleCommand
            Dim dt_chkPA As New DataTable
            Try
                ls_chkPA = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='PA' and SDV_VERF_FLAG='N'"
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd_chkPA = New OracleCommand(ls_chkPA, con)
                cmd_chkPA.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                cmd_chkPA.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim))
                dt_chkPA = getRecord(cmd_chkPA, con)
                If dt_chkPA.Rows.Count > 0 Then
                    updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "PA")
                End If
            Catch ex As Exception

            End Try


        End If
        btnupdateage.Enabled = False
        getagedrv(TxtSpno.Text.Trim)
        If Session("reqtype") = "Renew" Then
            For Each gvrow As GridViewRow In grdage.Rows
                Dim chkbox As CheckBox = gvrow.FindControl("chkSelectage")
                Dim reqno As HiddenField = gvrow.FindControl("hdreqno")
                If reqno.Value.Trim = Session("requestnumber").ToString Then
                    chkbox.Enabled = True
                Else
                    chkbox.Enabled = False
                End If
            Next
        End If
        clearagedrv()
    End Sub
    Private Sub clearagedrv()
        lbl_dobfile.Text = ""
        lbl_drvfile.Text = ""
        lbl_passfile.Text = "'"
        hiddob.Value = ""
        hiddrv.Value = ""
        hidpass.Value = ""
    End Sub
    Protected Sub btnsavemed_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnsavemed.Click
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim filenamefit As String = String.Empty
        Dim filenameunder As String = String.Empty
        Dim filenamewcc As String = String.Empty
        Try
            If fupdlfitnesscer.HasFile = False Then
                ShowMessage("Please Upload Fitness Certificate")
                Exit Sub
            Else
                filenamefit = Path.GetFileName(fupdlfitnesscer.PostedFile.FileName)
                Dim contentType As String = fupdlfitnesscer.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdlfitnesscer.PostedFile.ContentLength > 1048576) Then
                        ShowMessage("Your file size is " + (fupdlfitnesscer.PostedFile.ContentLength / 1048576).ToString("0.00") + " MB " + "Please upload file within 1MB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
            End If
            If fupdlundertake.HasFile = False Then
                ShowMessage("Please Upload Undertaking Certificate")
                Exit Sub
            Else
                filenameunder = Path.GetFileName(fupdldrv.PostedFile.FileName)
                Dim contentType As String = fupdlundertake.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdlundertake.PostedFile.ContentLength > 1048576) Then
                        ShowMessage("Your file size is " + (fupdlundertake.PostedFile.ContentLength / 1048576).ToString("0.00") + " MB " + "Please upload file within 1MB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
            End If
            If fupdlwcc.HasFile = False Then

            Else
                filenamewcc = Path.GetFileName(fupdlpass.PostedFile.FileName)
                Dim contentType As String = fupdlwcc.PostedFile.ContentType
                If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                    If (fupdlwcc.PostedFile.ContentLength > 1048576) Then
                        ShowMessage("Your file size is " + (fupdlwcc.PostedFile.ContentLength / 1048576).ToString("0.00") + " MB " + "Please upload file within 1MB")
                        Exit Sub
                    End If
                Else
                    ShowMessage("Please Upload pdf file only")
                    Exit Sub
                End If
            End If
            Dim fitid As String = TrnCWEAgeDrvSeqNo("")
            Dim underid As String = "0"
            Dim wccid As String = "0"
            If fupdlfitnesscer.HasFile = True Then
                underid = TrnCWEAgeDrvSeqNo("")
            Else
                underid = "0"
            End If
            If fupdlwcc.HasFile = True Then
                wccid = TrnCWEAgeDrvSeqNo("")
            Else
                wccid = "0"
            End If

            If fupdlfitnesscer.HasFile = True Then
                Dim cmdfilefit As New OracleCommand
                ls_sql = String.Empty
                filenamefit = Path.GetFileName(fupdlfitnesscer.PostedFile.FileName)
                Using fs As Stream = fupdlfitnesscer.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfilefit.CommandText = ls_sql
                        cmdfilefit.Connection = con
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_DOC_ID", fitid))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_NAME", filenamefit))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "DFC"))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfilefit.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfilefit.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If

            If fupdlundertake.HasFile = True Then
                Dim cmdfileunder As New OracleCommand
                ls_sql = String.Empty
                filenameunder = Path.GetFileName(fupdlundertake.PostedFile.FileName)
                Using fs As Stream = fupdlundertake.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfileunder.CommandText = ls_sql
                        cmdfileunder.Connection = con
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_DOC_ID", underid))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_NAME", filenameunder))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "UTC"))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfileunder.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfileunder.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If

            If fupdlwcc.HasFile = True Then
                Dim cmdfilewcc As New OracleCommand
                ls_sql = String.Empty
                filenamewcc = Path.GetFileName(fupdlwcc.PostedFile.FileName)
                Using fs As Stream = fupdlwcc.PostedFile.InputStream
                    Using br As BinaryReader = New BinaryReader(fs)
                        Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                        If con.State = ConnectionState.Closed Then
                            con.Open()

                        End If
                        cmdfilewcc.CommandText = ls_sql
                        cmdfilewcc.Connection = con
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_DOC_ID", wccid))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_NAME", filenamewcc))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "WCC"))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                        'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                        cmdfilewcc.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                        cmdfilewcc.ExecuteNonQuery()
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Using
                End Using
            End If
            Dim medid As String = TrnCWEMedSeqNo("")
            Dim validdate As String = getdate()
            ls_sql = "insert into T_CWM_MED_DTL(CMTT_MED_ID,CMTT_SAFETY_PASS_NO,CMTT_FIT_CERT_NO,CMTT_UN_CERT_NO,CMTT_WCDP_CERT_NO,CMTT_VALID_DATE,CMTT_CREATED_BY,CMTT_CREATED_DATE,CMTT_COMP_CODE,CMTT_STATUS) values(:CMTT_MED_ID,:CMTT_SAFETY_PASS_NO,:CMTT_FIT_CERT_NO,:CMTT_UN_CERT_NO,:CMTT_WCDP_CERT_NO,to_date(:CMTT_VALID_DATE,'dd/mm/yyyy'),:CMTT_CREATED_BY,sysdate,:CMTT_COMP_CODE,'Y')"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CMTT_MED_ID", medid))
            cmd.Parameters.Add(New OracleParameter(":CMTT_SAFETY_PASS_NO", TxtSpno.Text.Trim))
            cmd.Parameters.Add(New OracleParameter(":CMTT_FIT_CERT_NO", fitid))
            cmd.Parameters.Add(New OracleParameter(":CMTT_UN_CERT_NO", underid))
            cmd.Parameters.Add(New OracleParameter(":CMTT_WCDP_CERT_NO", wccid))
            cmd.Parameters.Add(New OracleParameter(":CMTT_VALID_DATE", validdate))
            cmd.Parameters.Add(New OracleParameter(":CMTT_CREATED_BY", Session("VendCode")))
            cmd.Parameters.Add(New OracleParameter(":CMTT_COMP_CODE", Session("Comp_Code")))
            'cmd.Parameters.Add(New OracleParameter(":CMTT_CREATED_DATE", Session("VendCode")))
            cmd.ExecuteNonQuery()
        Catch ex As Exception

        End Try
        getfitnesscer(TxtSpno.Text.Trim)
        clearmed()
    End Sub
    Private Function getdate() As String
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim nextdate As String = String.Empty
        Try
            ls_sql = "select TO_CHAR(ADD_MONTHS(sysdate,3)-1,'dd/mm/yyyy') next from dual"
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                nextdate = dt.Rows(0).Item("next").ToString
            End If
        Catch ex As Exception

        End Try
        Return nextdate
    End Function
    Private Sub getfitnesscer(ByVal spno As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Try
            ls_sql = "select a.CMTT_MED_ID ""MEDID"",to_char(a.CMTT_VALID_DATE,'dd/mm/yyyy') validdate, b.DM_NAME ""FITCER"",c.DM_NAME ""UNCER"",d.DM_NAME ""WCC"",b.Dm_DOC_ID ""FITNO"",c.DM_DOC_ID ""UNNO"",d.DM_DOC_ID ""WCCNO"" from T_DOCUMENT_MASTER b ,t_cwm_med_dtl a,t_document_master c,t_document_master d  where a.CMTT_FIT_CERT_NO=b.DM_DOC_ID and a.CMTT_UN_CERT_NO=c.DM_DOC_ID and d.DM_DOC_ID=a.CMTT_WCDP_CERT_NO and a.CMTT_SAFETY_PASS_NO=:CMTT_SAFETY_PASS_NO and CMTT_COMP_CODE=:CMTT_COMP_CODE and CMTT_STATUS='Y' "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":CMTT_SAFETY_PASS_NO", spno))
            cmd.Parameters.Add(New OracleParameter(":CMTT_COMP_CODE", Session("Comp_Code")))
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
                gvmed.DataSource = dt
                gvmed.DataBind()
            Else
                gvmed.DataSource = Nothing
                gvmed.DataBind()
            End If
        Catch ex As Exception


        End Try
    End Sub
    Protected Sub btnupdatemed_click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnupdatemed.Click

        If fupdlfitnesscer.HasFile = True Then


            Dim contentType As String = fupdlfitnesscer.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf(" / ") + 1, contentType.Length - contentType.IndexOf(" / ") - 1).Equals("pdf") Then
                If (fupdlfitnesscer.PostedFile.ContentLength > 1048576) Then
                    ShowMessage("Your file size is " + (fupdlfitnesscer.PostedFile.ContentLength / 1048576).ToString("0.00") + " MB " + "Please upload file within 1MB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If
        If fupdlundertake.HasFile = True Then


            Dim contentType As String = fupdlundertake.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdlundertake.PostedFile.ContentLength > 1048576) Then
                    ShowMessage("Your file size is " + (fupdlundertake.PostedFile.ContentLength / 1048576).ToString("0.00") + " MB " + "Please upload file within 1MB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        If fupdlwcc.HasFile = True Then


            Dim contentType As String = fupdlwcc.PostedFile.ContentType
            If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
                If (fupdlwcc.PostedFile.ContentLength > 1048576) Then
                    ShowMessage("Your file size is " + (fupdlwcc.PostedFile.ContentLength / 1048576).ToString("0.00") + " MB " + "Please upload file within 1MB")
                    Exit Sub
                End If
            Else
                ShowMessage("Please Upload pdf file only")
                Exit Sub
            End If
        End If

        If fupdlfitnesscer.HasFile = True Then
            Dim cmdfilefit As New OracleCommand
            Using fs As Stream = fupdlfitnesscer.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    Dim filename As String = Path.GetFileName(fupdlfitnesscer.PostedFile.FileName)

                    Dim ls_sql As String = "update T_DOCUMENT_MASTER Set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                    If con.State = ConnectionState.Closed Then
                        con.Open()

                    End If
                    cmdfilefit.CommandText = ls_sql
                    cmdfilefit.Connection = con
                    cmdfilefit.Parameters.Add(New OracleParameter(":DM_DOC_ID", hdfit.Value))
                    cmdfilefit.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                    cmdfilefit.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                    'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                    cmdfilefit.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                    cmdfilefit.ExecuteNonQuery()
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If

                End Using
            End Using
        End If

        If fupdlundertake.HasFile = True Then
            Dim ls_sql As String = String.Empty
            Dim cmdfileunder As New OracleCommand
            Using fs As Stream = fupdlundertake.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    Dim filename As String = Path.GetFileName(fupdlundertake.PostedFile.FileName)

                    ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                    If con.State = ConnectionState.Closed Then
                        con.Open()

                    End If
                    cmdfileunder = New OracleCommand(ls_sql, con)
                    cmdfileunder.Parameters.Add(New OracleParameter(":DM_DOC_ID", hdunder.Value))
                    cmdfileunder.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                    cmdfileunder.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                    'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                    cmdfileunder.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                    cmdfileunder.ExecuteNonQuery()
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If



                End Using
            End Using
        End If


        If fupdlwcc.HasFile = True Then
            Dim cmdfilewcc As New OracleCommand

            Using fs As Stream = fupdlwcc.PostedFile.InputStream
                Using br As BinaryReader = New BinaryReader(fs)
                    Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If
                    Dim filename As String = Path.GetFileName(fupdlwcc.PostedFile.FileName)

                    Dim ls_sql As String = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID"
                    If con.State = ConnectionState.Closed Then
                        con.Open()

                    End If
                    cmdfilewcc = New OracleCommand(ls_sql, con)

                    cmdfilewcc.Connection = con
                    cmdfilewcc.Parameters.Add(New OracleParameter(":DM_DOC_ID", hdwcc.Value))
                    cmdfilewcc.Parameters.Add(New OracleParameter(":DM_NAME", filename))

                    cmdfilewcc.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                    'cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("userid")))
                    cmdfilewcc.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                    cmdfilewcc.ExecuteNonQuery()
                    If con.State = ConnectionState.Open Then
                        con.Close()
                    End If



                End Using
            End Using
        End If
        btnupdatemed.Enabled = False
        btnupdatemed.Visible = False
        getfitnesscer(TxtSpno.Text.Trim)
        clearmed()
    End Sub
    Protected Sub chkSelectMed(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim vIsRowSelected As Boolean = False

        Try
            Dim gvrow As GridViewRow
            gvrow = CType(sender, CheckBox).Parent.Parent
            If CType(gvrow.FindControl("chkSelectMed"), CheckBox).Checked = True Then
                vIsRowSelected = True
                Dim vfitID As String = CType(gvrow.FindControl("hidfit"), HiddenField).Value
                Dim vunderID As String = CType(gvrow.FindControl("hidunder"), HiddenField).Value
                Dim vwccID As String = CType(gvrow.FindControl("hidwcc"), HiddenField).Value
                Dim vmedID As String = CType(gvrow.FindControl("hidmed"), HiddenField).Value
                Dim fitfile As String = CType(gvrow.FindControl("lnkdownloadfit"), LinkButton).Text
                Dim underfile As String = CType(gvrow.FindControl("lnkdownloadunder"), LinkButton).Text
                Dim wccfile As String = CType(gvrow.FindControl("lnkdownloadwcc"), LinkButton).Text

                hdfit.Value = vfitID
                hdunder.Value = vunderID
                hdwcc.Value = vwccID
                hdmedid.Value = vmedID
                lbl_filefitness.Text = fitfile
                lbl_fileunder.Text = underfile
                lbl_filewcc.Text = wccfile
                'btnupdatemed.Enabled = True
                'btnupdatemed.Visible = True
            Else
                clearmed()
                btnupdatemed.Enabled = False
                btnupdatemed.Visible = False
            End If

        Catch ex As Exception
            Dim vLineNum As String = ex.StackTrace.ToString().Substring(CInt(ex.StackTrace.ToString().LastIndexOf(":") + 1).ToString(0))
            Dim vPageName1 As String = Me.Page.ToString().Substring(4, Me.Page.ToString().Substring(4).Length - 5) + ".aspx"
            Dim strErrMsg As String = ex.Message.ToString.Substring(0)
            ClientScript.RegisterStartupScript(Me.GetType(), "alert", "alert('Error: " & strErrMsg & "\nPage Name: " & vPageName1 & "\nLine Number: " & vLineNum & "');", True)
        End Try
    End Sub
    Private Sub clearmed()
        lbl_filefitness.Text = ""
        lbl_fileunder.Text = ""
        lbl_filewcc.Text = ""
        hdmedid.Value = ""
        hdunder.Value = ""
        hdwcc.Value = ""
        hdfit.Value = ""
    End Sub
    Private Sub updatedocstatus(ByVal reqno As String, ByVal spno As String, ByVal type As String)
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim status As String = String.Empty
        Try


            ls_sql = "delete T_SP_DOC_VERIFICATION where SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_REQ_NO=:SDV_REQ_NO and SDV_VERF_TYPE=:SDV_VERF_TYPE "
            If con.State = ConnectionState.Closed Then
                con.Open()
            End If
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", spno))
            cmd.Parameters.Add(New OracleParameter(":SDV_REQ_NO", reqno))
            cmd.Parameters.Add(New OracleParameter(":SDV_VERF_TYPE", type))
            cmd.ExecuteNonQuery()

            ''''''check if any rejection is pending'''''''
            ls_sql = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_FLAG='N'"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", spno))
            cmd.Parameters.Add(New OracleParameter(":SDV_REQ_NO", reqno))
            dt.Clear()
            dt = getRecord(cmd, con)
            If dt.Rows.Count > 0 Then
            Else
                ls_sql = "update t_cemp_details_tmp set CET_DOCVER_STATUS='I' where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO and CET_DOCVER_STATUS='R' "
                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", spno))
                cmd.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", reqno))
                cmd.ExecuteNonQuery()
            End If
            ''''''''''''''''''''''''''''''''''''''''''''''

        Catch ex As Exception

        End Try

    End Sub
    Private Sub cmbSkSkill_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbSkSkill.SelectedIndexChanged
        If ddlSkillTrade.Text.Trim().Substring(0, ddlSkillTrade.Text.Trim().IndexOf("-")) = "SKTD0028" Then
            getSkillAssessment()
        End If

    End Sub
    Private Sub imgaddressold_Click(sender As Object, e As ImageClickEventArgs) Handles imgaddressold.Click
        Try
            Dim id As Long = hddaddressold.Value
            Dim bytes As Byte()
            Dim fileName, contentType As String
            Using cmd As OracleCommand = New OracleCommand()
                cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID"
                cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
                cmd.Connection = con
                con.Open()
                Using sdr As OracleDataReader = cmd.ExecuteReader()
                    sdr.Read()
                    bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                    contentType = sdr("DM_FILE_TYPE").ToString()
                    fileName = sdr("DM_NAME").ToString()
                End Using

                con.Close()
            End Using


            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.ContentType = contentType
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.[End]()
        Catch ex As Exception

        End Try

    End Sub
    Private Sub imbageold_Click(sender As Object, e As ImageClickEventArgs) Handles imbageold.Click
        Try
            Dim id As Long = hdfageold.Value
            Dim bytes As Byte()
            Dim fileName, contentType As String
            Using cmd As OracleCommand = New OracleCommand()
                cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID"
                cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
                cmd.Connection = con
                con.Open()
                Using sdr As OracleDataReader = cmd.ExecuteReader()
                    sdr.Read()
                    bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                    contentType = sdr("DM_FILE_TYPE").ToString()
                    fileName = sdr("DM_NAME").ToString()
                End Using

                con.Close()
            End Using


            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.ContentType = contentType
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.[End]()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub imbdriverold_Click(sender As Object, e As ImageClickEventArgs) Handles imbdriverold.Click
        Try
            Dim id As Long = hdfdriverold.Value
            Dim bytes As Byte()
            Dim fileName, contentType As String
            Using cmd As OracleCommand = New OracleCommand()
                cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID"
                cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
                cmd.Connection = con
                con.Open()
                Using sdr As OracleDataReader = cmd.ExecuteReader()
                    sdr.Read()
                    bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                    contentType = sdr("DM_FILE_TYPE").ToString()
                    fileName = sdr("DM_NAME").ToString()
                End Using

                con.Close()
            End Using


            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.ContentType = contentType
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.[End]()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub imgpassold_Click(sender As Object, e As ImageClickEventArgs) Handles imgpassold.Click
        Try
            Dim id As Long = hdfpassold.Value
            Dim bytes As Byte()
            Dim fileName, contentType As String
            Using cmd As OracleCommand = New OracleCommand()
                cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID"
                cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
                cmd.Connection = con
                con.Open()
                Using sdr As OracleDataReader = cmd.ExecuteReader()
                    sdr.Read()
                    bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                    contentType = sdr("DM_FILE_TYPE").ToString()
                    fileName = sdr("DM_NAME").ToString()
                End Using

                con.Close()
            End Using


            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.ContentType = contentType
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.[End]()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub imgskillold_Click(sender As Object, e As ImageClickEventArgs) Handles imgskillold.Click
        Try

            Dim id As Long = hdfskilold.Value
            Dim bytes As Byte()
            Dim fileName, contentType As String
            Using cmd As OracleCommand = New OracleCommand()
                cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID"
                cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
                cmd.Connection = con
                con.Open()
                Using sdr As OracleDataReader = cmd.ExecuteReader()
                    sdr.Read()
                    bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                    contentType = sdr("DM_FILE_TYPE").ToString()
                    fileName = sdr("DM_NAME").ToString()
                End Using

                con.Close()
            End Using


            Response.Clear()
            Response.Buffer = True
            Response.Charset = ""
            Response.Cache.SetCacheability(HttpCacheability.NoCache)
            Response.ContentType = contentType
            Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
            Response.BinaryWrite(bytes)
            Response.Flush()
            Response.[End]()
        Catch ex As Exception

        End Try
    End Sub
    Private Sub ibtnCloseconfirmsubmit_Click(sender As Object, e As EventArgs) Handles ibtnCloseconfirmsubmit.Click

        If Len(txtuan.Text.Trim) <> 12 And txtuan.Text.Trim.ToUpper <> "NA" Then
            lblpfesiErrMsg.Text = "UAN Number(under EPFO Act) should be 12 digit. Put NA if not applicable."
            txtuan.Text = ""
            mpconfirmsubmit.Show()
            Exit Sub
        End If

        If Len(txtip.Text.Trim) <> 10 And txtip.Text.Trim.ToUpper <> "NA" Then
            lblpfesiErrMsg.Text = "IP Number(under ESIC Act) should be 10 digit. Put NA if not applicable."
            txtip.Text = ""
            mpconfirmsubmit.Show()

            Exit Sub
        End If

        If txtuan.Text.Trim.ToUpper <> "NA" Then
            Dim sqlDuplicateID As String = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UAN_NO='" + txtuan.Text.Trim().ToUpper() + "'   and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null) and CET_SAFETY_PASSNO <>'" + TxtSpno.Text.Trim + "' "
            Dim dtDuplicateID As New DataTable()
            dtDuplicateID = getRecord(sqlDuplicateID, con)
            If dtDuplicateID.Rows.Count > 0 Then
                lblpfesiErrMsg.Text = "This UAN Number already Exists In system For SP No : " + dtDuplicateID.Rows(0)("CET_SAFETY_PASSNO")
                txtuan.Text = ""
                mpconfirmsubmit.Show()
                Exit Sub
            End If
            '''''''''''''''check uniq ID number already exist or not'''''''''
            sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UAN_NO='" + txtuan.Text.Trim().ToUpper() + "'  AND CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim + "'"
            dtDuplicateID = getRecord(sqlDuplicateID, con)
            If dtDuplicateID.Rows.Count > 0 Then
                lblpfesiErrMsg.Text = "This UAN Number already Exists In system For SP No : " + dtDuplicateID.Rows(0)("CED_SAFETY_PASS_NO")
                txtuan.Text = ""
                mpconfirmsubmit.Show()
                Exit Sub
            End If
        End If


        If txtip.Text.Trim.ToUpper <> "NA" Then
            Dim sqlDuplicateID As String = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_IP_NO='" + txtip.Text.Trim().ToUpper() + "'   and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null) and CET_SAFETY_PASSNO <>'" + TxtSpno.Text.Trim + "' "
            Dim dtDuplicateID As New DataTable()
            dtDuplicateID = getRecord(sqlDuplicateID, con)
            If dtDuplicateID.Rows.Count > 0 Then
                lblpfesiErrMsg.Text = "This IP Number already Exists In system For SP No : " + dtDuplicateID.Rows(0)("CET_SAFETY_PASSNO")
                txtuan.Text = ""
                mpconfirmsubmit.Show()
                Exit Sub
            End If
            '''''''''''''''check uniq ID number already exist or not'''''''''
            sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_IP_NO='" + txtip.Text.Trim().ToUpper() + "'  AND CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim + "'"
            dtDuplicateID = getRecord(sqlDuplicateID, con)
            If dtDuplicateID.Rows.Count > 0 Then
                lblpfesiErrMsg.Text = "This IP Number already Exists In system For SP No : " + dtDuplicateID.Rows(0)("CED_SAFETY_PASS_NO")
                txtuan.Text = ""
                mpconfirmsubmit.Show()
                Exit Sub
            End If
        End If





        Dim sqlUpdProfile As String = ""
        Dim vSPNo As String = ""
        vSPNo = TxtSpno.Text.Trim.ToUpper
        sqlUpdProfile = "update t_cemp_details_tmp set "

        sqlUpdProfile = sqlUpdProfile + "CET_UAN_NO ='" + txtuan.Text.Trim().ToUpper() + "',"

        sqlUpdProfile = sqlUpdProfile + "CET_IP_NO ='" + txtip.Text.ToString.Trim().ToUpper() + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_BY ='" + Session("VendCode") + "',"
        sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE "

        sqlUpdProfile = sqlUpdProfile + " where CET_SAFETY_PASSNO = '" + vSPNo + "'"
        sqlUpdProfile = sqlUpdProfile + " and  CET_REQUEST_NO = '" + Session("requestnumber") + "'"

        Try
            SaveData(sqlUpdProfile, con)

            'Renewal_profile_details(vSPNo)
            'empView()
            'btnUpdateProfile.Visible = True

            'mpconfirmsubmit.Show()
        Catch ex As Exception
            ShowMessage("Error While Updating Record")
        End Try
    End Sub
    Private Sub txtuan_TextChanged(sender As Object, e As EventArgs) Handles txtuan.TextChanged
        ' MsgBox(Len(txtuan.Text))
        ' MsgBox(Len(txtuan.Text.Length) <> 12)
        'MsgBox(txtuan.Text.Trim.ToUpper <> "NA")
        If Len(txtuan.Text.Trim) <> 12 And txtuan.Text.Trim.ToUpper <> "NA" Then
            lblpfesiErrMsg.Text = "UAN Number(under EPFO Act) should be 12 digit. Put NA if not applicable."
            txtuan.Text = ""
            mpconfirmsubmit.Show()
        Else
            lblpfesiErrMsg.Text = ""
            mpconfirmsubmit.Show()
        End If
    End Sub
    Private Sub txtip_TextChanged(sender As Object, e As EventArgs) Handles txtip.TextChanged
        If Len(txtip.Text.Trim) <> 10 And txtip.Text.Trim.ToUpper <> "NA" Then
            lblpfesiErrMsg.Text = "IP Number(under ESIC Act) should be 10 digit. Put NA if not applicable."
            txtip.Text = ""
            mpconfirmsubmit.Show()
        Else
            lblpfesiErrMsg.Text = ""
            mpconfirmsubmit.Show()
        End If
    End Sub
    Private Sub chk_waive_CheckedChanged(sender As Object, e As EventArgs) Handles chk_waive.CheckedChanged
        If chk_waive.Checked Then
        Else
            drp_waiveoff.SelectedValue = "0"
            'ADD BY PRASUN CHAKRABORTY ON 24122021
            'WI6447: if waive off unchecked then reset
            txt_WAIVE_DAYS.Text = ""
            dv_WAIVE_DAYS.Visible = False
            'END ADD BY PRASUN CHAKRABORTY ON 24122021
        End If
    End Sub
    'ADD BY PRASUN CHAKRABORTY ON 24122021
    'WI6447: handle funtionality if waive off reason chosen
    Protected Sub drp_waiveoff_SelectedIndexChanged(sender As Object, e As EventArgs)
        If drp_waiveoff.SelectedValue <> "0" Then
            dv_WAIVE_DAYS.Visible = True
        Else
            txt_WAIVE_DAYS.Text = ""
        End If
    End Sub
    'END ADD BY PRASUN CHAKRABORTY ON 24122021
    Public Function getSPReqType(ByVal SPReqNumber As String) As String

        Dim vSPReqType As String = ""
        Dim sqlSPReqType As String = " select SRQ_REQ_TYPE from hrace.t_sp_request where SRQ_REQ_NO='" + SPReqNumber + "' "
        Dim dtSPReqType As New DataTable()
        dtSPReqType = getRecord(sqlSPReqType, con)
        If dtSPReqType.Rows.Count > 0 Then
            vSPReqType = dtSPReqType.Rows(0)("SRQ_REQ_TYPE").trim.ToString()
        End If

        dtSPReqType.Dispose()
        Return vSPReqType

    End Function
    Public Function getMedChkLocation(ByVal CodeType As String, ByVal LocCode As String, ByVal Reqtype As String, ByVal FlagCode As String) As Integer

        Dim res As Integer = 0
        Dim count As String = ""
        Try
            Dim sql As String = "SELECT count(*) cnt FROM hrace.t_cwm_action_mapping where ACM_TYPE=:pCodeType and ACM_COMPANY_CODE=:pLocCode and substr(ACM_CATEGORY,-3,3)=:pReqtype and acm_flag=:pFlagCode "
            Dim cmd As New OracleCommand(sql, con)
            cmd.Parameters.AddWithValue(":pCodeType", CodeType)
            cmd.Parameters.AddWithValue(":pLocCode", LocCode)
            cmd.Parameters.AddWithValue(":pReqtype", Reqtype)
            cmd.Parameters.AddWithValue(":pFlagCode", FlagCode)

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            count = cmd.ExecuteScalar().ToString()

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If count <> "0" Then
                res = 1
            Else
                res = 0
            End If

        Catch ex As Exception

        End Try
        Return res
    End Function
    Public Function getMedChkOFSPFIT(ByVal SPNO As String, ByVal SPREQNO As String, ByVal LOCCODE As String, ByVal FlagCode As String, ByVal MEDSTS As String) As Integer

        Dim res As Integer = 0
        Dim count As String = ""
        Try
            '''''***** WI:297 allowed skill entery for new cases if medical status for worker found FIT or follow up Fit******'
            Dim sql As String = " select count(*) cnt from hrace.T_CWM_MEDICAL_HDR_TMP where CMT_SAFETY_PASS_NO=:pSPNO and CMT_REQUEST_NO=:pSPREQNO and CMT_COMP_CODE=:pLOCCODE and CMT_DEL_FLAG=:pFlagCode and CMT_FIT_STATUS in('FIT','FUFIT') "
            Dim cmd As New OracleCommand(sql, con)
            cmd.Parameters.AddWithValue(":pSPNO", SPNO)
            cmd.Parameters.AddWithValue(":pSPREQNO", SPREQNO)
            cmd.Parameters.AddWithValue(":pLOCCODE", LOCCODE)
            cmd.Parameters.AddWithValue(":pFlagCode", FlagCode)
            ' cmd.Parameters.AddWithValue(":pMEDSTS", MEDSTS)

            If con.State = ConnectionState.Closed Then
                con.Open()
            End If

            count = cmd.ExecuteScalar().ToString()

            If con.State = ConnectionState.Open Then
                con.Close()
            End If

            If count <> "0" Then
                res = 1
            Else
                res = 0
            End If
        Catch ex As Exception

        End Try
        Return res
    End Function

    Private Sub btn_savevac_Click(sender As Object, e As EventArgs) Handles btn_savevac.Click
        'WI2689: save vaccination details during profile entry
        'created by : Avik Mukherjee
        'Created on: 18-Aug-2021
        Dim ls_sql As String = String.Empty
        Dim arr_cmd As New ArrayList()
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim fdose As String = String.Empty
        Dim sdose As String = String.Empty
        Dim rtpcr As String = String.Empty
        ' Dim cerrtpcr As String = "0"
        Dim certvac As String = "-1"
        Dim certvacE As String = "-1"
        Dim filename As String = String.Empty
        Dim exmp As String = "N"
        Dim ls_chkTR As String = String.Empty
        Dim cmd_chkTR As OracleCommand
        Dim dt_chkTR As New DataTable
        If chk_exem.Checked = True Then
            exmp = "Y"
        Else
            exmp = "N"

        End If
        If drp_vaccinedose.SelectedValue = "1" Then
            If txt_sdose.Text <> "__/__/____" And txt_sdose.Text.Trim <> "" Then
                ShowMessage("You have selected vaccination dose 1.Please remove vaccination dose 2 date")
                txt_sdose.Text = "__/__/____"
                Exit Sub
            End If
        End If
        'If Session("requesttype") = "SPN" Then
        '    If drp_vaccinedose.SelectedValue = "0" Then
        '        ShowMessage("For new request vaccination is mandatory")
        '        Exit Sub
        '    End If
        '    If updt_vac.HasFile = False Then
        '        ShowMessage("For new request vaccination certificate upload is mandatory")
        '        Exit Sub
        '    End If
        'End If
        Try
            If txt_fdose.Text.Trim = "__/__/____" Then
                fdose = String.Empty
            Else
                fdose = txt_fdose.Text.Trim
            End If
            If txt_sdose.Text.Trim = "__/__/____" Then
                sdose = String.Empty
            Else
                sdose = txt_sdose.Text.Trim

            End If

            If drp_vaccinedose.SelectedValue <> "0" Then

                If drp_vaccinedose.SelectedValue = "1" Then
                    If txt_fdose.Text.Trim = "__/__/____" Or txt_fdose.Text.Trim = "" Then
                        ShowMessage("vaccination details for dose 1 is mandatory")
                        Exit Sub
                    End If
                End If
                If drp_vaccinedose.SelectedValue = "2" Then
                    If (txt_fdose.Text.Trim = "__/__/____" Or txt_fdose.Text.Trim = "") And (txt_sdose.Text.Trim = "__/__/____" Or txt_sdose.Text.Trim = "") Then
                        ShowMessage("vaccibnation details for dose 1 and dose 2 are mandatory")
                        Exit Sub
                    End If
                End If
            End If



            If updt_vac.HasFile = False Then
                certvac = "0"
            Else
                certvac = TrnCWETrnSeqNo("")
            End If

            If updt_exemp.HasFile = False Then
                certvacE = "0"
            Else
                certvacE = TrnCWETrnSeqNo("")
            End If
            If con.State = ConnectionState.Closed Then
                con.Open()

            End If
            updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "CV")
            updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "VE")
            ls_sql = "update t_cemp_vaccination_tmp set VACT_STATUS='N',VACT_MODIFIED_BY=:VACT_MODIFIED_BY,VACT_MODIFIED_DATE=sysdate where VACT_SP_NO=:VACT_SP_NO and VACT_STATUS='Y'"
            cmd = New OracleCommand(ls_sql, con)
            cmd.Parameters.Add(New OracleParameter(":VACT_SP_NO", TxtSpno.Text.Trim))
            cmd.Parameters.Add(New OracleParameter(":VACT_MODIFIED_BY", Session("Vendcode")))
            arr_cmd.Add(cmd)
            If drp_vaccinedose.SelectedValue = "1" Then
                ls_sql = "insert into t_cemp_vaccination_tmp(VACT_REQ_NO,VACT_SP_NO,VACT_COMP_CODE,VACT_VEND_CODE,VACT_STATUS,VACT_CREATED_DATE,VACT_CREATED_BY,VACT_VAC_DOS,VACT_VAC_NAME,VACT_VAC_DOS_DT,VACT_VAC_CERTNO,VACT_EXEMP_CERTNO,VACT_EXEMP) values(:VACT_REQ_NO,:VACT_SP_NO,:VACT_COMP_CODE,:VACT_VEND_CODE,:VACT_STATUS,sysdate,:VACT_CREATED_BY,:VACT_VAC_DOS,:VACT_VAC_NAME,to_date(:VACT_VAC_DOS_DT,'dd/mm/yyyy'),:VACT_VAC_CERTNO,:VACT_EXEMP_CERTNO,:VACT_EXEMP)"
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":VACT_REQ_NO", Session("requestnumber")))
                cmd.Parameters.Add(New OracleParameter(":VACT_SP_NO", TxtSpno.Text.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_COMP_CODE", Session("Comp_code")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VEND_CODE", Session("VendCode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_STATUS", "Y"))
                cmd.Parameters.Add(New OracleParameter(":VACT_CREATED_BY", Session("Vendcode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS", "1"))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_NAME", drp_vaccinename.SelectedValue))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS_DT", fdose.Trim))
                'cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS2_DT", sdose.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_CERTNO", certvac.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP", exmp.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP_CERTNO", certvacE.Trim))

                arr_cmd.Add(cmd)
            End If
            If drp_vaccinedose.SelectedValue = "2" Then
                ls_sql = "insert into t_cemp_vaccination_tmp(VACT_REQ_NO,VACT_SP_NO,VACT_COMP_CODE,VACT_VEND_CODE,VACT_STATUS,VACT_CREATED_DATE,VACT_CREATED_BY,VACT_VAC_DOS,VACT_VAC_NAME,VACT_VAC_DOS_DT,VACT_VAC_CERTNO,VACT_EXEMP_CERTNO,VACT_EXEMP) values(:VACT_REQ_NO,:VACT_SP_NO,:VACT_COMP_CODE,:VACT_VEND_CODE,:VACT_STATUS,sysdate,:VACT_CREATED_BY,:VACT_VAC_DOS,:VACT_VAC_NAME,to_date(:VACT_VAC_DOS_DT,'dd/mm/yyyy'),:VACT_VAC_CERTNO,:VACT_EXEMP_CERTNO,:VACT_EXEMP)"
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":VACT_REQ_NO", Session("requestnumber")))
                cmd.Parameters.Add(New OracleParameter(":VACT_SP_NO", TxtSpno.Text.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_COMP_CODE", Session("Comp_code")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VEND_CODE", Session("VendCode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_STATUS", "Y"))
                cmd.Parameters.Add(New OracleParameter(":VACT_CREATED_BY", Session("Vendcode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS", "1"))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_NAME", drp_vaccinename.SelectedValue))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS_DT", fdose.Trim))
                'cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS2_DT", sdose.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_CERTNO", certvac.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP", exmp.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP_CERTNO", certvacE.Trim))

                arr_cmd.Add(cmd)
                ls_sql = "insert into t_cemp_vaccination_tmp(VACT_REQ_NO,VACT_SP_NO,VACT_COMP_CODE,VACT_VEND_CODE,VACT_STATUS,VACT_CREATED_DATE,VACT_CREATED_BY,VACT_VAC_DOS,VACT_VAC_NAME,VACT_VAC_DOS_DT,VACT_VAC_CERTNO,VACT_EXEMP_CERTNO,VACT_EXEMP) values(:VACT_REQ_NO,:VACT_SP_NO,:VACT_COMP_CODE,:VACT_VEND_CODE,:VACT_STATUS,sysdate,:VACT_CREATED_BY,:VACT_VAC_DOS,:VACT_VAC_NAME,to_date(:VACT_VAC_DOS_DT,'dd/mm/yyyy'),:VACT_VAC_CERTNO,:VACT_EXEMP_CERTNO,:VACT_EXEMP)"
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":VACT_REQ_NO", Session("requestnumber")))
                cmd.Parameters.Add(New OracleParameter(":VACT_SP_NO", TxtSpno.Text.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_COMP_CODE", Session("Comp_code")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VEND_CODE", Session("VendCode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_STATUS", "Y"))
                cmd.Parameters.Add(New OracleParameter(":VACT_CREATED_BY", Session("Vendcode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS", "2"))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_NAME", drp_vaccinename.SelectedValue))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS_DT", sdose.Trim))
                'cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS2_DT", sdose.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_CERTNO", certvac.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP", exmp.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP_CERTNO", certvacE.Trim))

                arr_cmd.Add(cmd)
            End If
            If drp_vaccinedose.SelectedValue = "0" And drp_vaccinedose.Enabled = "False" And chk_exem.Checked = True Then
                ls_sql = "insert into t_cemp_vaccination_tmp(VACT_REQ_NO,VACT_SP_NO,VACT_COMP_CODE,VACT_VEND_CODE,VACT_STATUS,VACT_CREATED_DATE,VACT_CREATED_BY,VACT_VAC_DOS,VACT_VAC_NAME,VACT_VAC_DOS_DT,VACT_VAC_CERTNO,VACT_EXEMP_CERTNO,VACT_EXEMP) values(:VACT_REQ_NO,:VACT_SP_NO,:VACT_COMP_CODE,:VACT_VEND_CODE,:VACT_STATUS,sysdate,:VACT_CREATED_BY,:VACT_VAC_DOS,:VACT_VAC_NAME,to_date(:VACT_VAC_DOS_DT,'dd/mm/yyyy'),:VACT_VAC_CERTNO,:VACT_EXEMP_CERTNO,:VACT_EXEMP)"
                cmd = New OracleCommand(ls_sql, con)
                cmd.Parameters.Add(New OracleParameter(":VACT_REQ_NO", Session("requestnumber")))
                cmd.Parameters.Add(New OracleParameter(":VACT_SP_NO", TxtSpno.Text.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_COMP_CODE", Session("Comp_code")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VEND_CODE", Session("VendCode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_STATUS", "Y"))
                cmd.Parameters.Add(New OracleParameter(":VACT_CREATED_BY", Session("Vendcode")))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS", "0"))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_NAME", drp_vaccinename.SelectedValue))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS_DT", sdose.Trim))
                'cmd.Parameters.Add(New OracleParameter(":VACT_VAC_DOS2_DT", sdose.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_VAC_CERTNO", certvac.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP", exmp.Trim))
                cmd.Parameters.Add(New OracleParameter(":VACT_EXEMP_CERTNO", certvacE.Trim))
                arr_cmd.Add(cmd)
            End If

            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            If chk_exem.Checked = False Then

                If updt_vac.HasFile = True Then
                    Dim cmdfileskill As New OracleCommand
                    'Dim ls_sql As String = String.Empty
                    filename = Path.GetFileName(updt_vac.PostedFile.FileName)
                    Using fs As Stream = updt_vac.PostedFile.InputStream
                        Using br As BinaryReader = New BinaryReader(fs)
                            Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileskill.CommandText = ls_sql
                            cmdfileskill.Connection = con
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_DOC_ID", certvac.Trim))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "VAC"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            arr_cmd.Add(cmdfileskill)
                            'cmdfileskill.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Using
                    End Using
                    If arr_cmd.Count > 0 Then
                        Dim counter As Integer = 0
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim tran_Ins As OracleTransaction
                        tran_Ins = con.BeginTransaction()
                        Try
                            For counter = 0 To arr_cmd.Count - 1
                                Dim con_ins As New OracleCommand()
                                con_ins = arr_cmd.Item(counter)
                                con_ins.Transaction = tran_Ins
                                con_ins.ExecuteNonQuery()
                            Next
                            tran_Ins.Commit()
                            ls_chkTR = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='VAC' and SDV_VERF_FLAG='N'"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            cmd_chkTR = New OracleCommand(ls_chkTR, con)
                            'cmd_chkTR.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                            cmd_chkTR.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim))
                            dt_chkTR = getRecord(cmd_chkTR, con)
                            If dt_chkTR.Rows.Count > 0 Then
                                updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "VAC")
                            End If
                            '******* check exemption vaccination details dic ver status************'
                            ls_chkTR = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='VACE' and SDV_VERF_FLAG='N'"
                            If con.State = ConnectionState.Closed Then
                                con.Open()
                            End If
                            cmd_chkTR = New OracleCommand(ls_chkTR, con)
                            'cmd_chkTR.Parameters.Add(New OracleParameter(":SDV_REQ_NO", Session("requestnumber")))
                            cmd_chkTR.Parameters.Add(New OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim))
                            dt_chkTR = getRecord(cmd_chkTR, con)
                            If dt_chkTR.Rows.Count > 0 Then
                                updatedocstatus(Session("requestnumber"), TxtSpno.Text.Trim, "VACE")
                            End If
                            ShowMessage("vaccination details updated")

                        Catch ex As Exception
                            tran_Ins.Rollback()
                            ShowMessage(ex.ToString)
                        Finally
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Try
                    End If
                End If
            Else


                If updt_exemp.HasFile = True Then
                    Dim cmdfileskill As New OracleCommand
                    'Dim ls_sql As String = String.Empty
                    filename = Path.GetFileName(updt_exemp.PostedFile.FileName)
                    Using fs As Stream = updt_exemp.PostedFile.InputStream
                        Using br As BinaryReader = New BinaryReader(fs)
                            Dim bytes As Byte() = br.ReadBytes(CType(fs.Length, Int32))

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) "
                            If con.State = ConnectionState.Closed Then
                                con.Open()

                            End If
                            cmdfileskill.CommandText = ls_sql
                            cmdfileskill.Connection = con
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_DOC_ID", certvacE.Trim))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_NAME", filename))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_TYPE", "VACE"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_FILE_CONTENT", bytes))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_PROJECT", "CWM"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODULE", "VPSS"))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_COMP_CODE", Session("Comp_code")))
                            'cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("userid")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_MODIFIED_BY", Session("VendCode")))
                            cmdfileskill.Parameters.Add(New OracleParameter(":DM_CREATED_BY", Session("VendCode")))
                            arr_cmd.Add(cmdfileskill)
                            'cmdfileskill.ExecuteNonQuery()
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Using
                    End Using
                End If

                If arr_cmd.Count > 0 Then
                    Dim counter As Integer = 0
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    Dim tran_Ins As OracleTransaction
                    tran_Ins = con.BeginTransaction()
                    Try
                        For counter = 0 To arr_cmd.Count - 1
                            Dim con_ins As New OracleCommand()
                            con_ins = arr_cmd.Item(counter)
                            con_ins.Transaction = tran_Ins
                            con_ins.ExecuteNonQuery()
                        Next
                        tran_Ins.Commit()

                        ShowMessage("vaccination details updated")

                    Catch ex As Exception
                        tran_Ins.Rollback()
                        ShowMessage(ex.ToString)
                    Finally
                        If con.State = ConnectionState.Open Then
                            con.Close()
                        End If
                    End Try
                End If
            End If
            getvaccination(TxtSpno.Text.Trim)

        Catch ex As Exception
            ShowMessage("vaccination details not updated" + ex.Message)
        End Try
    End Sub

    Private Sub chk_exem_CheckedChanged(sender As Object, e As EventArgs) Handles chk_exem.CheckedChanged
        If chk_exem.Checked = True Then
            drp_vaccinedose.SelectedValue = "0"
            drp_vaccinename.SelectedValue = "0"
            txt_fdose.Text = "__/__/____"
            txt_sdose.Text = "__/__/____"
            txt_fdose.Enabled = False
            txt_sdose.Enabled = False
            drp_vaccinedose.Enabled = False
            drp_vaccinename.Enabled = False
            updt_vac.Enabled = False
        Else
            txt_fdose.Enabled = True
            txt_sdose.Enabled = True
            drp_vaccinedose.Enabled = True
            drp_vaccinename.Enabled = True
            updt_vac.Enabled = True
        End If
    End Sub

    Private Sub lnk_vacdoc_Click(sender As Object, e As EventArgs) Handles lnk_vacdoc.Click
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = hdvacsrlno.Value
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='VAC'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub

    Private Sub lnk_exemp_Click(sender As Object, e As EventArgs) Handles lnk_exemp.Click
        Dim ls_lnk As LinkButton = sender
        Dim id As Long = hd_exemp.Value
        Dim bytes As Byte()
        Dim fileName, contentType As String
        Using cmd As OracleCommand = New OracleCommand()
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='VACE'"
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id)
            cmd.Connection = con
            con.Open()
            Using sdr As OracleDataReader = cmd.ExecuteReader()
                sdr.Read()
                bytes = CType(sdr("Dm_FILE_CONTENT"), Byte())
                contentType = sdr("DM_FILE_TYPE").ToString()
                fileName = sdr("DM_NAME").ToString()
            End Using

            con.Close()
        End Using


        Response.Clear()
        Response.Buffer = True
        Response.Charset = ""
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.ContentType = contentType
        Response.AppendHeader("Content-Disposition", "attachment; filename=" & fileName)
        Response.BinaryWrite(bytes)
        Response.Flush()
        Response.[End]()
    End Sub

    Protected Sub download_Click(sender As Object, e As EventArgs) Handles download.Click
        Dim response As System.Web.HttpResponse = System.Web.HttpContext.Current.Response
        response.ClearContent()
        response.Clear()
        response.ContentType = "application/pdf"
        response.AddHeader("Content-Disposition", "attachment; filename=PrivacyConsent_Contract Worker.pdf")
        response.TransmitFile(Server.MapPath("~/App_Data/SETTLEMENT/PrivacyConsent_Contract Worker.pdf"))
        response.Flush()
        response.[End]()
    End Sub

    Protected Sub btn_saveconsent_Click(sender As Object, e As EventArgs) Handles btn_saveconsent.Click

        If Not consent_details.HasFile Then
            ShowMessage("Please upload the consent pdf file!")
            Exit Sub
        End If

        Dim contentType As String = consent_details.PostedFile.ContentType

        If contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf") Then
            If (consent_details.PostedFile.ContentLength > 1024000) Then
                Dim textFileSize As String = (consent_details.PostedFile.ContentLength / (1024 * 1024)).ToString("0.00") & "MB/" & (consent_details.PostedFile.ContentLength / 1024).ToString("0.00") & "KB"

                ShowMessage("Your file size is " + textFileSize + " Please upload file within 1MB/1024KB")
                Exit Sub
            End If
        Else
            ShowMessage("Only pdf files allowed")
            Exit Sub
        End If


        Dim cmdConsentinsert As OracleCommand = Nothing
        Try
            Dim sqlConsent As String = ""
            Dim safetyPass As String = ""
            If Session("vSPNO") = Nothing Then
                safetyPass = TxtSpno.Text
            Else
                safetyPass = Session("vSPNO").ToString()
            End If
            Dim spconsent As String = safetyPass + "_pii.pdf"
            Dim dt As DataTable
            Dim intResult As Integer = 0

            sqlConsent = "  Select  COUNT(*) CNT from hrace.t_cemp_piiconsent_details " +
                            "WHERE cnd_safetypass_num = :cnd_safetypass_num"

            cmdConsentinsert = New OracleCommand(sqlConsent, con)
            cmdConsentinsert.Parameters.Clear()
            cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_safetypass_num", safetyPass))

            dt = getRecord(cmdConsentinsert, con)

            If dt.Rows(0).Item("CNT") = 0 Then

                sqlConsent = " insert into hrace.t_cemp_piiconsent_details " +
                "(cnd_safetypass_num ,cnd_vend_code ,cnd_consent_present ,cnd_created_date ,cnd_created_by  ,cnd_consent_dcmt,cnd_consent_path)" +
                "values" +
                "(:cnd_safetypass_num ,:cnd_vend_code ,:cnd_consent_present , sysdate,:cnd_created_by ,:cnd_consent_dcmt,:cnd_consent_path)"

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                cmdConsentinsert = New OracleCommand()
                cmdConsentinsert.Connection = con
                cmdConsentinsert.CommandText = sqlConsent
                cmdConsentinsert.Parameters.Clear()
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_safetypass_num", safetyPass))
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_vend_code", Session("VendCode").ToString))
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_consent_present", "Y"))
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_created_by", Session("VendCode").ToString))
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_consent_dcmt", spconsent))
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_consent_path", Server.MapPath("consent//")))

                intResult = cmdConsentinsert.ExecuteNonQuery()
            ElseIf dt.Rows(0).Item("CNT") > 0 Then

                sqlConsent = "UPDATE hrace.t_cemp_piiconsent_details " +
                 "SET CND_MODIFIED_DATE = SYSDATE, CND_MODIFIED_BY = :cnd_modified_by " +
                "WHERE  CND_SAFETYPASS_NUM = :cnd_safetypass_num"

                If con.State = ConnectionState.Closed Then
                    con.Open()
                End If

                cmdConsentinsert = New OracleCommand()
                cmdConsentinsert.Connection = con
                cmdConsentinsert.CommandText = sqlConsent
                cmdConsentinsert.Parameters.Clear()

                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_modified_by", Session("VendCode").ToString))
                cmdConsentinsert.Parameters.Add(New OracleParameter(":cnd_safetypass_num", safetyPass))

                intResult = cmdConsentinsert.ExecuteNonQuery()
            End If

            If intResult > 0 Then
                consent_details.SaveAs(Server.MapPath("consent//" + spconsent))
                ShowMessage("Consent Uploaded Successfully!")
            Else
                ShowMessage("Oops! something went wrong!")
            End If

            empView()
        Catch ex As Exception
            ShowMessage("Error Occured while uploading file! " & ex.Message)
        Finally
            If con.State = ConnectionState.Open Then
                con.Close()
            End If
            cmdConsentinsert = Nothing
        End Try


    End Sub
    Protected Sub cmbUniqID_SelectedIndexChanged(sender As Object, e As EventArgs)
        txtUniqIDNo.Text = ""
    End Sub

    Public Sub GetDistrict(ByVal vStateCD As String)

        Dim sqlDistrict As String
        If vStateCD = "0" Then
            sqlDistrict = "select * from hrace.t_district_master "
        Else
            sqlDistrict = "select * from hrace.t_district_master where DST_STATE_CODE='" + vStateCD + "' order by DST_DISTRICT_NAME"
        End If

        Dim dtDistrict As New DataTable()
        dtDistrict = getRecord(sqlDistrict, con)
        cmbAddDistrict.Items.Clear()
        If dtDistrict.Rows.Count > 0 Then
            cmbAddDistrict.DataSource = dtDistrict
            cmbAddDistrict.DataTextField = "DST_DISTRICT_NAME"
            cmbAddDistrict.DataValueField = "DST_DISTRICT_CODE"
            cmbAddDistrict.DataBind()
            cmbAddDistrict.Items.Insert(0, New WebControls.ListItem("[Select]", "0"))
        End If
    End Sub

    Private Sub ActiveControlsForFormA()
        Dim ls_sql As String = "select * from hrace.t_lin_master where LIM_COMPANY_CODE=:LIM_COMPANY_CODE"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":LIM_COMPANY_CODE", Session("Comp_code")))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            pnlFormA.Visible = True
            lblNomineeAddress.Visible = True
            txtNomineeAddress.Visible = True
        Else
            pnlFormA.Visible = False
            lblNomineeAddress.Visible = False
            txtNomineeAddress.Visible = False
        End If
        PopulateRelayCombo()
    End Sub

    Private Sub PopulateRelayCombo()
        Dim strQry As String = String.Empty

        strQry = "select ACM_REMARKS from hrace.T_CWM_ACTION_MAPPING WHERE ACM_TYPE = 'RELAY' and ACM_FLAG = 'Y' ORDER BY ACM_CATEGORY"

        Dim cmd As OracleCommand = New OracleCommand(strQry, con)
        Dim dt As DataTable = getRecord(cmd, con)
        cmbRelayData.DataSource = dt
        cmbRelayData.DataTextField = "ACM_REMARKS"
        cmbRelayData.DataValueField = "ACM_REMARKS"
        cmbRelayData.DataBind()

        cmbRelayData.Items.Insert(0, New WebControls.ListItem("[Select]", "[Select]"))
    End Sub

    Private Function IsFormAValid() As Boolean
        Dim blReturn As Boolean = True
        If pnlFormA.Visible Then
            'PAN Validation
            Dim strchk As String = txtPAN.Text
            If strchk.Trim.Length = 0 Then
                ShowMessage("PAN No is required")
                blReturn = False
            End If
            Dim st As Boolean = strchk.Contains(" ")
            If st Then
                ShowMessage("This is not a valid PAN number")
                blReturn = False
            End If
            If strchk.Length = 10 Then
            Else
                ShowMessage("This is not a valid PAN number")
                blReturn = False
            End If
            Dim alphanumeric As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("[A-Z]{5}\d{4}[A-Z]{1}")
            If (alphanumeric.IsMatch(strchk)) Then
            Else
                ShowMessage("This is not a valid PAN number")
                blReturn = False
            End If
            'PAN Validation
            'Relation Validaton
            If cmbAdltRelation.SelectedValue = "0" Then
                ShowMessage("Relationship with adult person is required")
                blReturn = False
            End If
            'Relation Validaton
            'Name Validation            
            If txtAdltName.Text.Trim.Length = 0 Then
                ShowMessage("Adult person name is required")
                blReturn = False
            End If
            'Name Validation
            'Address Validaton
            If txtAdltAddress.Text.Trim.Length = 0 Then
                ShowMessage("Adult person address is required")
                blReturn = False
            End If
            'Address No Validaton
            'Mobile Validaton
            Dim numericph As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If (numericph.IsMatch(txtAdltMobile.Text.Trim)) Then
                If txtAdltMobile.Text.ToString.Length = 10 Then
                Else
                    ShowMessage("Please enter 10 digit mobile number")
                    blReturn = False
                End If
            Else
                ShowMessage("Please provide valid mobile number")
                blReturn = False
            End If
            'Mobile No Validaton
            'Aadhar No Validaton
            Dim strchkAadhar As String = txtAADHAR.Text
            Dim stAadhar As Boolean = strchk.Contains(" ")
            If stAadhar Then
                ShowMessage("This is not a valid Adhaar number")
                blReturn = False
            End If
            If strchkAadhar.Length = 12 Then
            Else
                ShowMessage("This is not a valid Adhaar number")
                blReturn = False
            End If
            Dim numeric As System.Text.RegularExpressions.Regex = New System.Text.RegularExpressions.Regex("^[0-9]+$")
            If (numeric.IsMatch(strchkAadhar)) Then
            Else
                ShowMessage("This is not a valid Adhaar number")
                blReturn = False
            End If
            'Aadhar No Validaton
            'Nationality Validaton
            If cmbNationality.SelectedValue = "[Select]" Then
                ShowMessage("Nationality is required")
                blReturn = False
            End If
            'Nationality No Validaton
            'PlaceOfEmployment Validaton
            If cmbPlaceOfEmployment.SelectedValue = "[Select]" Then
                ShowMessage("Place of Employment is required")
                blReturn = False
            End If
            'PlaceOfEmployment No Validaton
            'RelayData Validaton
            If cmbRelayData.SelectedValue = "[Select]" Then
                ShowMessage("Relay data is required")
                blReturn = False
            End If
            'RelayData No Validaton
        End If

        Return blReturn
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Function to check the safety pass result for trade code and calculate the locking period
    '''  Added By vishal(2602256):calculating locking period from Created date to current date for Fail candidate
    ''' </summary>
    ''' <param name="SPNO"></param>
    ''' <param name="REQNO"></param>
    ''' <param name="tradecd"></param>
    ''' <returns></returns>
    Private Function GetSafetyPassResult(ByVal SPNO As String, ByVal REQNO As String, ByVal tradecd As String, ByVal compcode As String) As Int32
        Dim lockingPeriod As Int32 = 0

        Dim ls_sql As String = "select TCD_CLM_SKILL_CD,TCD_CREATE_DT  from hrps.T_TD_CLM_DOC@ace_iris,hrace.t_cwm_cemp_skill_tmp,hrace.t_sp_request" +
                    " where TCD_SP_NO=:TCD_SP_NO and TCD_SP_NO=CCST_SAFETY_PASS_NO and ccst_req_no = srq_req_no and TCD_CLM_SKILL_CD=CCST_SKTD_CP_CD" +
                    " and CCST_REQ_NO=:CCST_REQ_NO and UPPER(TCD_CERT_CATEG)='FAIL' and srq_req_type IN ('SPN') and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD" +
                    " and TCD_CREATE_DT=(Select max(TCD_CREATE_DT) from hrps.T_TD_CLM_DOC@ace_iris where TCD_SP_NO=:TCD_SP_NO and UPPER(TCD_CERT_CATEG)='FAIL')" +
                    " and CCST_COMP_CODE in(select acm_company_code from hrace.t_cwm_action_mapping where acm_type='SKREPLY15' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:CCST_COMP_CODE)"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":TCD_SP_NO", SPNO))
        cmd.Parameters.Add(New OracleParameter(":CCST_REQ_NO", REQNO))
        cmd.Parameters.Add(New OracleParameter(":TCD_CLM_SKILL_CD", tradecd))
        cmd.Parameters.Add(New OracleParameter(":CCST_COMP_CODE", compcode))

        Dim dt As DataTable = getRecord(cmd, con)

        If dt.Rows.Count > 0 Then
            Dim resultpublishDate As String = dt.Rows(0).Item("TCD_CREATE_DT")
            Dim diff As TimeSpan = Convert.ToDateTime(Date.Now()).Subtract(resultpublishDate)
            lockingPeriod = diff.TotalDays.ToString()
            If lockingPeriod = 0 Then
                lockingPeriod = 1
            End If
        End If

        Return lockingPeriod
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Company codes checking for the reapply option.
    ''' </summary>
    ''' <param name="compcode"></param>
    ''' <returns></returns>
    Protected Function iscompanycodesforreapplyprovision(ByVal compcode As String) As Boolean
        Dim isreapplyprovision As Boolean = False
        Dim ls_sql_compcodes As String = String.Empty
        Dim cmdcompcodeforreapplyprovision As OracleCommand
        Dim dtcompcodes As New DataTable
        ls_sql_compcodes = "select ACM_CATEGORY from hrace.t_cwm_action_mapping where acm_type='SKREPLY15' and ACM_FLAG='Y' and ACM_COMPANY_CODE=" + compcode
        cmdcompcodeforreapplyprovision = New OracleCommand(ls_sql_compcodes, con)
        dtcompcodes = getRecord(cmdcompcodeforreapplyprovision, con)
        If dtcompcodes.Rows.Count > 0 Then
            isreapplyprovision = True
            Session("lockingdays") = dtcompcodes.Rows(0).Item("ACM_CATEGORY")
        End If
        Return isreapplyprovision
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) fetching the locking days for reapply option.
    ''' </summary>
    ''' <param name="compcode"></param>
    ''' <returns></returns>
    Public Function getLockingDays(ByVal compcode As String) As Int32
        Dim ls_sql As String = String.Empty
        Dim cmd As OracleCommand
        Dim dt As New DataTable
        Dim period As Int32 = 0
        Try
            ls_sql = "select ACM_CATEGORY from hrace.t_cwm_action_mapping where acm_type='SKREPLY15' and ACM_FLAG='Y' and ACM_COMPANY_CODE=" + compcode
            cmd = New OracleCommand(ls_sql, con)
            dt = getRecord(cmd, con)

            If dt.Rows.Count > 0 Then
                period = dt.Rows(0).Item("ACM_CATEGORY")
            Else
                period = 0
            End If
        Catch ex As Exception

        End Try
        Return period
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) condition checked for result published in iris for fail result of safety pass number, trade code and result puplished after next date of reapply
    ''' </summary>
    ''' <param name="spno"></param>
    ''' <param name="tradecode"></param>
    ''' <param name="reqno"></param>
    ''' <returns></returns>
    Protected Function isresultpublishedwithfailresult(ByVal spno As String, ByVal tradecode As String, ByVal reqno As String) As Boolean
        Dim isresultpublished As Boolean = False
        Dim sqlirisresultpublished = "select TCD_CLM_SKILL_CD from hrps.T_TD_CLM_DOC@ace_iris,hrace.T_REAPPLY_SKILL_HIST where TCD_SP_NO=:TCD_SP_NO and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD and UPPER(TCD_CERT_CATEG)='FAIL' "
        sqlirisresultpublished += "and TCD_SP_NO=RSH_SP_NO and RSH_SP_REQ_NO=:RSH_SP_REQ_NO and TCD_CREATE_DT =(Select max(TCD_CREATE_DT) from hrps.T_TD_CLM_DOC@ace_iris where TCD_SP_NO=:TCD_SP_NO and UPPER(TCD_CERT_CATEG)='FAIL') "
        sqlirisresultpublished += "and TCD_CREATE_DT>=(select max(RSH_TIMESTAMP) from hrace.T_REAPPLY_SKILL_HIST where RSH_SP_NO=RSH_SP_NO and RSH_SP_REQ_NO=:RSH_SP_REQ_NO AND RSH_TRADE_CD=TCD_CLM_SKILL_CD)  "
        Dim cmdirisresultpublished As OracleCommand = New OracleCommand(sqlirisresultpublished, con)
        cmdirisresultpublished.Parameters.Add(New OracleParameter(":TCD_SP_NO", spno))
        cmdirisresultpublished.Parameters.Add(New OracleParameter(":TCD_CLM_SKILL_CD", tradecode))
        cmdirisresultpublished.Parameters.Add(New OracleParameter(":RSH_SP_REQ_NO", reqno))
        Dim dtirisresultpublished As DataTable = getRecord(cmdirisresultpublished, con)
        If dtirisresultpublished.Rows.Count > 0 Then
            isresultpublished = True
        End If
        Return isresultpublished
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Condition for checking if the result declared as PASS in IRIS of safety pass number, trade code and company code.
    ''' </summary>
    ''' <param name="spno"></param>
    ''' <param name="Tradecode"></param>
    ''' <returns></returns>
    Protected Function isresultpass(ByVal spno As String, ByVal Tradecode As String) As Boolean
        Dim isresultpublished As Boolean = False
        Dim sqlirisresultpublishedwithpass = "select TCD_CLM_SKILL_CD from hrps.T_TD_CLM_DOC@ace_iris where TCD_SP_NO=:TCD_SP_NO and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD and UPPER(TCD_CERT_CATEG)<>'FAIL' "
        sqlirisresultpublishedwithpass += "and TCD_CREATE_DT =(Select max(TCD_CREATE_DT) from hrps.T_TD_CLM_DOC@ace_iris where TCD_SP_NO=:TCD_SP_NO and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD)"
        Dim cmdirisresultpublishedwithpass As OracleCommand = New OracleCommand(sqlirisresultpublishedwithpass, con)
        cmdirisresultpublishedwithpass.Parameters.Add(New OracleParameter(":TCD_SP_NO", spno))
        cmdirisresultpublishedwithpass.Parameters.Add(New OracleParameter(":TCD_CLM_SKILL_CD", Tradecode))
        Dim dtirisresultpublishedwithpass As DataTable = getRecord(cmdirisresultpublishedwithpass, con)
        If dtirisresultpublishedwithpass.Rows.Count > 0 Then
            isresultpublished = True
        End If
        Return isresultpublished
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Condition check for checking the skill already reapplied or not.
    ''' </summary>
    ''' <param name="spno"></param>
    ''' <param name="reqno"></param>
    ''' <param name="skilltypecode"></param>
    ''' <param name="skillcode"></param>
    ''' <param name="tradecode"></param>
    ''' <param name="SPReqLocCode"></param>
    ''' <returns></returns>
    Protected Function reapplyskillcheck(ByVal spno As String, ByVal reqno As String, ByVal skilltypecode As String, ByVal skillcode As String, ByVal tradecode As String, ByVal SPReqLocCode As String) As String
        Dim reapplyflag As String = "N"
        Dim sqlreapplysp As String = "select CCST_REAPPLY_SP from hrace.t_cwm_cemp_skill_tmp,hrace.T_REAPPLY_SKILL_HIST where CCST_SAFETY_PASS_NO=RSH_SP_NO and CCST_REQ_NO=RSH_SP_REQ_NO and CCST_SKTD_CP_CD=RSH_TRADE_CD and CCST_SAFETY_PASS_NO=:CCST_SAFETY_PASS_NO and CCST_REQ_NO=:CCST_REQ_NO and CCST_SKILL_TYPE_CD=:CCST_SKILL_TYPE_CD and CCST_SKILL_CD=:CCST_SKILL_CD and CCST_SKTD_CP_CD=:CCST_SKTD_CP_CD and CCST_COMP_CODE in(select acm_company_code from hrace.t_cwm_action_mapping where acm_type='SKREPLY15' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE)"
        Dim cmd_get As OracleCommand = New OracleCommand(sqlreapplysp, con)
        cmd_get.Parameters.Add(New OracleParameter(":CCST_SAFETY_PASS_NO", spno))
        cmd_get.Parameters.Add(New OracleParameter(":CCST_REQ_NO", reqno))
        cmd_get.Parameters.Add(New OracleParameter(":CCST_SKILL_TYPE_CD", skilltypecode))
        cmd_get.Parameters.Add(New OracleParameter(":CCST_SKILL_CD", skillcode))
        cmd_get.Parameters.Add(New OracleParameter(":CCST_SKTD_CP_CD", tradecode))
        cmd_get.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", SPReqLocCode))

        Dim dtRapplyFlag As DataTable = getRecord(cmd_get, con)
        If dtRapplyFlag.Rows.Count > 0 Then
            If dtRapplyFlag.Rows(0).Item("CCST_REAPPLY_SP").ToString() = "" Then
                reapplyflag = "N"
            ElseIf isresultpublishedwithfailresult(spno, tradecode, reqno) Then
                reapplyflag = "N"
            Else
                reapplyflag = "Y"
            End If
        End If
        Return reapplyflag
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Reapply option process starts. Updates the request flag, assessment date as null,Insert the safety pass,request number, trade code,company code,attempt count, timestamp in the T_REAPPLY_SKILL_HIST.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub gvSkill_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles gvSkill.RowCommand
        If e.CommandName = "REAPPLY_SP_REQ" Then
            Try
                Dim spno As String = TxtSpno.Text.ToString
                Dim arr_cmd As New ArrayList()
                Dim rowindex As Integer = Convert.ToInt32(e.CommandArgument)
                Dim req_no As String = CType(gvSkill.Rows(rowindex).FindControl("hdreqno"), HiddenField).Value
                Dim skilltypecode As String = CType(gvSkill.Rows(rowindex).FindControl("hidSkillType"), HiddenField).Value
                Dim skillcode As String = CType(gvSkill.Rows(rowindex).FindControl("hidSkillCD"), HiddenField).Value
                Dim tradecode As String = CType(gvSkill.Rows(rowindex).FindControl("hidSkillTradeCD"), HiddenField).Value
                Dim SPReqLocCode As String = Session("Comp_code").trim
                Dim reapplyflag As String = String.Empty
                If getSPReqType(req_no) <> "SPN" Then
                    ShowMessage("The reapply option is only applicable for new safety pass!")
                    Return
                End If
                reapplyflag = reapplyskillcheck(spno, req_no, skilltypecode, skillcode, tradecode, SPReqLocCode)
                If reapplyflag = "Y" Then
                    ShowMessage("You are already Re-applied, Please Wait till result declared")
                    Return
                ElseIf getassessmentcounter(spno, req_no, tradecode, comp_cd) > 3 Then
                    ShowMessage("You had reapplied 3 times for trade code and cannot be reapplied further")
                    Return
                Else
                    Dim ls_sql_reapply As String = "update hrace.t_cwm_cemp_skill_tmp set CCST_ASSESSMENT_RESULT=null,ccst_created_dt=sysdate," +
                    "CCST_MODIFIED_DT=sysdate,CCST_MODIFIED_BY=:CCST_MODIFIED_BY,ccst_assessment_date=null,ccst_assmnt_time=null,ccst_req_flag=null,CCST_REAPPLY_SP='Y', ccst_validity_date = '31-dec-9999'" +
                    " where CCST_SAFETY_PASS_NO=:TCD_SP_NO and CCST_REQ_NO=:CCST_REQ_NO and CCST_ASSESSMENT_TYPE in('D','T') and CCST_SKTD_CP_CD=:CCST_SKTD_CP_CD and CCST_SKILL_TYPE_CD=:CCST_SKILL_TYPE_CD and CCST_SKILL_CD=:CCST_SKILL_CD and CCST_COMP_CODE in(select acm_company_code from hrace.t_cwm_action_mapping where acm_type='SKREPLY15' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE)"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    Dim cmd_update As OracleCommand = New OracleCommand(ls_sql_reapply, con)
                    cmd_update.Parameters.Add(New OracleParameter(":TCD_SP_NO", spno))
                    cmd_update.Parameters.Add(New OracleParameter(":CCST_REQ_NO", req_no))
                    cmd_update.Parameters.Add(New OracleParameter(":CCST_MODIFIED_BY", Session("VendCode").ToString))
                    cmd_update.Parameters.Add(New OracleParameter(":CCST_SKTD_CP_CD", tradecode))
                    cmd_update.Parameters.Add(New OracleParameter(":CCST_SKILL_TYPE_CD", skilltypecode))
                    cmd_update.Parameters.Add(New OracleParameter(":CCST_SKILL_CD", skillcode))
                    cmd_update.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", SPReqLocCode))

                    arr_cmd.Add(cmd_update)

                    If (getassessmentcounter(spno, req_no, tradecode, comp_cd) > 0) Then
                        Dim updatereapplyskillattempt As String = "update hrace.T_REAPPLY_SKILL_HIST set RSH_ATTEMPT_COUNT=RSH_ATTEMPT_COUNT+1,RSH_MODIFIED_DATE=sysdate,RSH_MODIFIED_BY=:RSH_MODIFIED_BY,RSH_TIMESTAMP=sysdate where RSH_SP_NO=:RSH_SP_NO and RSH_SP_REQ_NO=:RSH_SP_REQ_NO and RSH_COMP_CODE=:RSH_COMP_CODE and RSH_TRADE_CD=:RSH_TRADE_CD"
                        Dim cmd_update_reapply_attempt As OracleCommand = New OracleCommand(updatereapplyskillattempt, con)
                        cmd_update_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_SP_NO", spno))
                        cmd_update_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_SP_REQ_NO", req_no))
                        cmd_update_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_TRADE_CD", tradecode))
                        cmd_update_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_MODIFIED_BY", Session("VendCode").ToString))
                        cmd_update_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_COMP_CODE", SPReqLocCode))

                        arr_cmd.Add(cmd_update_reapply_attempt)
                    Else
                        Dim insertreapplyskillattempt As String = "insert into hrace.T_REAPPLY_SKILL_HIST(RSH_SP_NO,RSH_SP_REQ_NO,RSH_TRADE_CD,RSH_ATTEMPT_COUNT,RSH_COMP_CODE,RSH_CREATED_DATE,RSH_CREATED_BY)values(:RSH_SP_NO,:RSH_SP_REQ_NO,:RSH_TRADE_CD,0,:RSH_COMP_CODE,sysdate,:RSH_CREATED_BY)"
                        Dim cmd_insert_reapply_attempt As OracleCommand = New OracleCommand(insertreapplyskillattempt, con)
                        cmd_insert_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_SP_NO", spno))
                        cmd_insert_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_SP_REQ_NO", req_no))
                        cmd_insert_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_TRADE_CD", tradecode))
                        cmd_insert_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_CREATED_BY", Session("VendCode").ToString))
                        cmd_insert_reapply_attempt.Parameters.Add(New OracleParameter(":RSH_COMP_CODE", SPReqLocCode))
                        arr_cmd.Add(cmd_insert_reapply_attempt)
                    End If

                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If

                    Dim sqlUpdate As String = "update hrace.t_cemp_details_tmp set cet_req_status=null "
                    sqlUpdate += "where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO and CET_LOCATION_CODE in(select acm_company_code from hrace.t_cwm_action_mapping where acm_type='SKREPLY15' and ACM_FLAG='Y' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE)"
                    If con.State = ConnectionState.Closed Then
                        con.Open()
                    End If
                    Dim cmdStatus As OracleCommand = New OracleCommand(sqlUpdate, con)
                    cmdStatus.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", spno))
                    cmdStatus.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", req_no))
                    cmdStatus.Parameters.Add(New OracleParameter(":ACM_COMPANY_CODE", SPReqLocCode))
                    arr_cmd.Add(cmdStatus)


                    If arr_cmd.Count > 0 Then
                        Dim counter As Integer = 0
                        If con.State = ConnectionState.Closed Then
                            con.Open()
                        End If
                        Dim tran_Ins As OracleTransaction
                        tran_Ins = con.BeginTransaction()
                        Try
                            For counter = 0 To arr_cmd.Count - 1
                                Dim con_ins As New OracleCommand()
                                con_ins = arr_cmd.Item(counter)
                                con_ins.Transaction = tran_Ins
                                con_ins.ExecuteNonQuery()
                            Next
                            tran_Ins.Commit()

                            ShowMessage("Record has been Re-applyed successfully")
                            getskill(spno)
                        Catch ex As Exception
                            tran_Ins.Rollback()
                            ShowMessage("Error occurs operation reverted")
                        Finally
                            If con.State = ConnectionState.Open Then
                                con.Close()
                            End If
                        End Try
                    End If
                End If

            Catch ex As Exception
                ShowMessage("Error occurs operation reverted")
            End Try
        End If
    End Sub
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Condition check if the result is declared in IRIS of safety pass number of fail result.
    ''' </summary>
    ''' <param name="spno"></param>
    ''' <returns></returns>
    Protected Function isresultpresentoffail(ByVal spno As String) As Boolean
        Dim isresultpublished As Boolean = False
        Dim sqlirisresultpublished = "select TCD_CLM_SKILL_CD from hrps.T_TD_CLM_DOC@ace_iris where TCD_SP_NO=:TCD_SP_NO and UPPER(TCD_CERT_CATEG)='FAIL' "
        sqlirisresultpublished += "and TCD_CREATE_DT =(Select max(TCD_CREATE_DT) from hrps.T_TD_CLM_DOC@ace_iris where TCD_SP_NO=:TCD_SP_NO and  UPPER(TCD_CERT_CATEG)='FAIL' )"
        Dim cmdirisresultpublished As OracleCommand = New OracleCommand(sqlirisresultpublished, con)
        cmdirisresultpublished.Parameters.Add(New OracleParameter(":TCD_SP_NO", spno))
        Dim dtirisresultpublished As DataTable = getRecord(cmdirisresultpublished, con)
        If dtirisresultpublished.Rows.Count > 0 Then
            isresultpublished = True
        End If
        Return isresultpublished
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Condition check of overall request status check for the reapply visibility logic
    ''' </summary>
    ''' <param name="spno"></param>
    ''' <param name="reqno"></param>
    ''' <returns></returns>
    Protected Function requeststatus(ByVal spno As String, ByVal reqno As String) As String
        Dim flag As String = ""
        Dim sqlstatus As String = "select  (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_REQ_STATUS) CET_REQ_STATUS from hrace.t_cemp_details_tmp where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO"
        Dim cmdstatus As OracleCommand = New OracleCommand(sqlstatus, con)
        cmdstatus.Parameters.Add(New OracleParameter(":CET_SAFETY_PASSNO", spno))
        cmdstatus.Parameters.Add(New OracleParameter(":CET_REQUEST_NO", reqno))
        Dim dtstatus As DataTable = getRecord(cmdstatus, con)
        If dtstatus.Rows.Count > 0 Then
            status_variables()

            If Not IsDBNull(dtstatus.Rows(0).Item("CET_REQ_STATUS")) Then

                If dtstatus.Rows(0).Item("CET_REQ_STATUS") = msg_complete Then        'ADDED FOR VERIFIED REQUEST
                    flag = "Y"
                ElseIf dtstatus.Rows(0).Item("CET_REQ_STATUS") = msg_reject Then   'ADDED FOR REJECTED REQUESTS
                    flag = "R"
                Else
                    flag = "N"
                End If
            Else
                flag = "N"
            End If
        End If
        Return flag
    End Function
    ''' <summary>
    ''' TCS.2164315 (23/02/2024) Condition check in row data bound for reapply flag option and status of message.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub gvSkill_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim spno As String = TxtSpno.Text.ToString
            Dim skilltypecd As String = TryCast(e.Row.FindControl("hidSkillType"), HiddenField).Value
            Dim skillcd As String = TryCast(e.Row.FindControl("hidSkillCD"), HiddenField).Value
            Dim req_no As String = TryCast(e.Row.FindControl("hdreqno"), HiddenField).Value
            Dim tradecd As String = TryCast(e.Row.FindControl("hidSkillTradeCD"), HiddenField).Value
            Dim lockingPeriod As Int32 = GetSafetyPassResult(spno, req_no, tradecd, Session("Comp_code").ToString)
            Dim Period As Int32 = getLockingDays(Session("Comp_code").ToString)
            Dim reapplyflag As String = reapplyskillcheck(spno, req_no, skilltypecd, skillcd, tradecd, Session("Comp_code").ToString)
            Dim resultpresentoffail As Boolean = isresultpresentoffail(spno)
            If (iscompanycodesforreapplyprovision(Session("Comp_code").ToString)) Then
                gvSkill.Columns(25).Visible = True
                gvSkill.Columns(26).Visible = True
                If getSPReqType(req_no) = "SPN" And Not isresultpass(spno, tradecd) And Period > 0 And requeststatus(spno, req_no) = "N" Then
                    If lockingPeriod <= Period And lockingPeriod > 0 And getassessmentcounter(spno, req_no, tradecd, Session("Comp_code").ToString) < 3 Then
                        TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                        TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Under locking period"

                    ElseIf lockingPeriod > Period Then
                        If reapplyflag = "Y" Then
                            TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                            TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Already reapplied and result pending"
                        ElseIf getassessmentcounter(spno, req_no, tradecd, Session("Comp_code").ToString) >= 3 Then
                            TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                            TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Reapply cannot be done after 3 attempt"
                        Else
                            TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = True
                            TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Reapply skill"
                        End If
                    ElseIf lockingPeriod = 0 And resultpresentoffail Then
                        If reapplyflag = "Y" Then
                            TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                            TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Already reapplied and result pending"
                        ElseIf getassessmentcounter(spno, req_no, tradecd, Session("Comp_code").ToString) >= 3 Then
                            TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                            TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Reapply cannot be done after 3 attempt"
                        Else
                            TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = True
                            TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Reapply skill"
                        End If

                    ElseIf getassessmentcounter(spno, req_no, tradecd, Session("Comp_code").ToString) >= 3 Then
                        TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                        TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Reapply cannot be done after 3 attempt"
                    Else
                        TryCast(e.Row.FindControl("btnReapply"), Button).Enabled = False
                        TryCast(e.Row.FindControl("reapplyremarks"), Label).Text = "Skill applied ,Result pending"
                    End If
                Else
                    TryCast(e.Row.FindControl("btnReapply"), Button).Visible = False
                    TryCast(e.Row.FindControl("reapplyremarks"), Label).Visible = False
                End If
            Else
                TryCast(e.Row.FindControl("btnReapply"), Button).Visible = False
                TryCast(e.Row.FindControl("reapplyremarks"), Label).Visible = False
            End If

        End If
    End Sub

    Protected Sub lnkbShowFlowchart_click(ByVal sender As Object, ByVal e As System.EventArgs)

        If (divFlowChartDtls.Visible) Then
            divFlowChartDtls.Visible = False
        Else
            divFlowChartDtls.Visible = True
        End If
    End Sub

    Protected Sub btn02_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn02.Click
        If tabcontainer1.Style("display") = "none" Then
            tabcontainer1.Style.Remove("display")
        Else
            tabcontainer1.Style.Add("display", "none")
        End If
    End Sub

    Protected Sub btn03_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn03.Click
        ScriptManager.RegisterClientScriptBlock(Me, Me.[GetType](), "popup", "window.open('https://tmh.tatasteel.co.in/account-management/login','_blank')", True)
    End Sub

    Protected Sub btn05_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn05.Click
        ScriptManager.RegisterClientScriptBlock(Me, Me.[GetType](), "popup", "window.open('https://clm.tatasteel.co.in/CLM/default.aspx','_blank')", True)
    End Sub

    Public Sub PaintWireFrame(ByVal SpNo As String)
        btn01.BackColor = Color.LightSteelBlue
        btn02.BackColor = Color.LightSteelBlue
        btn03.BackColor = Color.LightSteelBlue
        btn04.BackColor = Color.LightSteelBlue
        btn05.BackColor = Color.LightSteelBlue
        btn06.BackColor = Color.LightSteelBlue
        btn07.BackColor = Color.LightSteelBlue
        btn08.BackColor = Color.LightSteelBlue

        btn09.BackColor = Color.LightSteelBlue
        btn10.BackColor = Color.LightSteelBlue
        btn11.BackColor = Color.LightSteelBlue
        btn12.BackColor = Color.LightSteelBlue
        btn13.BackColor = Color.LightSteelBlue
        btn14.BackColor = Color.LightSteelBlue
        btn15.BackColor = Color.LightSteelBlue
        btn16.BackColor = Color.LightSteelBlue
        btn17.BackColor = Color.LightSteelBlue

        Dim SPReqNumber As String = Session("requestnumber").trim

        Dim ls_sql As String = "select distinct SRS_REQ_NO from hrace.T_SP_REQ_STATUS where SRS_REQ_NO = :ReqNo and SRS_STATUS = 'H1' and SRS_SUB_STATUS ='5' and SRS_AGENT_TYP = 'HR'"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            btn01.BackColor = Color.Green
            'If (Session("requestType") = "SPN") Then
            '    btn12.BackColor = Color.Green
            'End If
        Else
            btn01.BackColor = Color.Orange
        End If

        Dim ls_sql_pc As String = "select distinct CET_REQUEST_NO,CET_PROFILE_STATUS,CET_DOCVER_STATUS from hrace.t_cemp_details_tmp where CET_REQUEST_NO = :ReqNo and CET_SAFETY_PASSNO =:SpNo and CET_LOCATION_CODE =:comp_cd "
        Dim cmd_pc As OracleCommand = New OracleCommand(ls_sql_pc, con)
        cmd_pc.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_pc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pc.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pc As DataTable = getRecord(cmd_pc, con)
        If dt_pc.Rows.Count > 0 Then
            If (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "C") Then
                btn02.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "R") Then
                btn02.BackColor = Color.Red
            Else
                If (btn01.BackColor = Color.Orange) Then
                    btn02.BackColor = Color.LightSteelBlue
                Else
                    btn02.BackColor = Color.Orange
                End If
            End If

            If (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "C") Then
                btn04.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "R") Then
                btn04.BackColor = Color.Red
            Else
                If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange) Then
                    btn04.BackColor = Color.LightSteelBlue
                Else
                    btn04.BackColor = Color.Orange
                End If
            End If
        End If

        Dim ls_sql_md As String = "select CMH_SAFETY_PASS_NO from hrace.t_Cwm_Cemp_Medical_Hdr where CMH_SAFETY_PASS_NO = :SpNo and CMH_COMP_CODE =:comp_cd and TRUNC(CMH_VALIDITY_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_md As OracleCommand = New OracleCommand(ls_sql_md, con)
        cmd_md.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_md.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_md.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_md As DataTable = getRecord(cmd_md, con)
        If dt_md.Rows.Count > 0 Then
            btn03.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange) Then
                btn03.BackColor = Color.LightSteelBlue
            Else
                btn03.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sks As String = "select SKC_ASSESSMENT_DATE from hrace.t_skill_certification where SKC_SAFETY_PASS_NO =:SpNo and SKC_REQ_NO =:ReqNo  and SKC_COMPANY_CD=:comp_cd "
        Dim cmd_sks As OracleCommand = New OracleCommand(ls_sql_sks, con)
        cmd_sks.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sks.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sks.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sks As DataTable = getRecord(cmd_sks, con)
        If dt_sks.Rows.Count > 0 Then
            If (dt_sks.Rows(0).Item("SKC_ASSESSMENT_DATE").ToString <> "") Then
                btn05.BackColor = Color.Green
            Else
                If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange) Then
                    btn05.BackColor = Color.LightSteelBlue
                Else
                    btn05.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange) Then
                btn05.BackColor = Color.LightSteelBlue
            Else
                btn05.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_bio As String = "select CEP_FINGERPRINTR from hrace.t_cemp_photo where CEP_SAFETY_PASS_NO=:SpNo "
        Dim cmd_bio As OracleCommand = New OracleCommand(ls_sql_bio, con)
        cmd_bio.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_bio As DataTable = getRecord(cmd_bio, con)
        If dt_bio.Rows.Count > 0 Then
            If Not IsDBNull(dt_bio.Rows(0)("CEP_FINGERPRINTR")) Then
                btn06.BackColor = Color.Green
            Else
                If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange) Then
                    btn06.BackColor = Color.LightSteelBlue
                Else
                    btn06.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange) Then
                btn06.BackColor = Color.LightSteelBlue
            Else
                btn06.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_ska As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO=:SpNo AND TCD_CERT_CATEG<>'FAIL' "
        Dim cmd_ska As OracleCommand = New OracleCommand(ls_sql_ska, con)
        cmd_ska.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_ska As DataTable = getRecord(cmd_ska, con)
        If dt_ska.Rows.Count > 0 Then
            btn07.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange) Then
                btn07.BackColor = Color.LightSteelBlue
            Else
                btn07.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sts As String = "SELECT t.cst_safety_no FROM HRACE.T_CWM_SAFETY_TRN t where t.cst_safety_no=:SpNo and t.cst_request_no=:ReqNo and t.CST_COMPANY_CODE=:comp_cd "
        Dim cmd_sts As OracleCommand = New OracleCommand(ls_sql_sts, con)
        cmd_sts.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sts.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sts.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sts As DataTable = getRecord(cmd_sts, con)
        If dt_sts.Rows.Count > 0 Then
            btn08.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange) Then
                btn08.BackColor = Color.LightSteelBlue
            Else
                btn08.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_skf As String = "select CCT_SAFETY_PASS_NO from hrace.t_cwm_cemp_trns where CCT_SAFETY_PASS_NO = :SpNo and TRUNC(CCT_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_skf As OracleCommand = New OracleCommand(ls_sql_skf, con)
        cmd_skf.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_skf.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt_skf As DataTable = getRecord(cmd_skf, con)
        If dt_skf.Rows.Count > 0 Then
            btn09.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange) Then
                btn09.BackColor = Color.LightSteelBlue
            Else
                btn09.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sfa As String = "select CED_SAFETY_PASS_NO from hrace.t_cemp_details where CED_SAFETY_PASS_NO=:SpNo and CED_COMPANY_CODE=:comp_cd and CED_SP_ENABLED='Y' "
        Dim cmd_sfa As OracleCommand = New OracleCommand(ls_sql_sfa, con)
        cmd_sfa.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sfa.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sfa As DataTable = getRecord(cmd_sfa, con)
        If dt_sfa.Rows.Count > 0 Then
            btn10.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange Or btn09.BackColor = Color.Orange) Then
                btn10.BackColor = Color.LightSteelBlue
            Else
                btn10.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp As String = "select G.GRW_SAFETY_PASS_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate) "
        Dim cmd_gp As OracleCommand = New OracleCommand(ls_sql_gp, con)
        cmd_gp.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp As DataTable = getRecord(cmd_gp, con)
        If dt_gp.Rows.Count > 0 Then
            btn11.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange Or btn09.BackColor = Color.Orange Or btn10.BackColor = Color.Orange) Then
                btn11.BackColor = Color.LightSteelBlue
            Else
                btn11.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_bu As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'A1' and RQS_SUBSTATUS = '3' "
        Dim cmd_gp_bu As OracleCommand = New OracleCommand(ls_sql_gp_bu, con)
        cmd_gp_bu.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_bu As DataTable = getRecord(cmd_gp_bu, con)
        If dt_gp_bu.Rows.Count > 0 Then
            btn12.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange Or btn09.BackColor = Color.Orange Or btn10.BackColor = Color.Orange Or btn11.BackColor = Color.Orange) Then
                btn12.BackColor = Color.LightSteelBlue
            Else
                btn12.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_cc As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'B1' and RQS_SUBSTATUS = '5' "
        Dim cmd_gp_cc As OracleCommand = New OracleCommand(ls_sql_gp_cc, con)
        cmd_gp_cc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_cc As DataTable = getRecord(cmd_gp_cc, con)
        If dt_gp_cc.Rows.Count > 0 Then
            btn13.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange Or btn09.BackColor = Color.Orange Or btn10.BackColor = Color.Orange Or btn11.BackColor = Color.Orange Or btn12.BackColor = Color.Orange) Then
                btn13.BackColor = Color.LightSteelBlue
            Else
                btn13.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_sec As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) "
        Dim cmd_gp_sec As OracleCommand = New OracleCommand(ls_sql_gp_sec, con)
        cmd_gp_sec.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_sec.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_sec As DataTable = getRecord(cmd_gp_sec, con)
        If dt_gp_sec.Rows.Count > 0 Then
            btn14.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange Or btn09.BackColor = Color.Orange Or btn10.BackColor = Color.Orange Or btn11.BackColor = Color.Orange Or btn12.BackColor = Color.Orange Or btn13.BackColor = Color.Orange) Then
                btn14.BackColor = Color.LightSteelBlue
            Else
                btn14.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_rel As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) and CGP_RELEASE_DT is not null "
        Dim cmd_gp_rel As OracleCommand = New OracleCommand(ls_sql_gp_rel, con)
        cmd_gp_rel.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_rel.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_rel As DataTable = getRecord(cmd_gp_rel, con)
        If dt_gp_rel.Rows.Count > 0 Then
            btn15.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange Or btn03.BackColor = Color.Orange Or btn05.BackColor = Color.Orange Or btn06.BackColor = Color.Orange Or btn07.BackColor = Color.Orange Or btn08.BackColor = Color.Orange Or btn09.BackColor = Color.Orange Or btn10.BackColor = Color.Orange Or btn11.BackColor = Color.Orange Or btn12.BackColor = Color.Orange Or btn13.BackColor = Color.Orange Or btn14.BackColor = Color.Orange) Then
                btn15.BackColor = Color.LightSteelBlue
            Else
                btn15.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pv As String = "SELECT CPD_SAFETY_PASS_NO FROM hrace.T_CWM_PV_DTL WHERE CPD_SAFETY_PASS_NO=:SpNo and CPD_COMP_CODE =:comp_cd  and TRUNC(CPD_END_DT) >= TRUNC(SYSDATE)  "
        Dim cmd_pv As OracleCommand = New OracleCommand(ls_sql_pv, con)
        cmd_pv.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pv.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pv As DataTable = getRecord(cmd_pv, con)
        If dt_pv.Rows.Count > 0 Then
            btn16.BackColor = Color.Green
        Else
            If (btn01.BackColor = Color.Orange Or btn02.BackColor = Color.Orange) Then
                btn16.BackColor = Color.LightSteelBlue
            Else
                btn16.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pmj As String = "select CED_SAFETY_PASS_NO from t_cemp_details where CED_PMJJBY_DOC_ID is not null and CED_SAFETY_PASS_NO=:SpNo  "
        Dim cmd_pmj As OracleCommand = New OracleCommand(ls_sql_pmj, con)
        cmd_pmj.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_pmj As DataTable = getRecord(cmd_pmj, con)
        If dt_pmj.Rows.Count > 0 Then
            btn17.BackColor = Color.Green
        Else
            If (btn16.BackColor = Color.Orange Or btn02.BackColor = Color.Orange) Then
                btn17.BackColor = Color.LightSteelBlue
            Else
                btn17.BackColor = Color.Orange
            End If
        End If
    End Sub

    Public Function CheckWireFrameLoc() As Boolean
        Dim locCheck As Boolean
        Dim ls_sql As String = "SELECT at.ACM_TYPE FROM HRACE.t_cwm_action_mapping at where at.ACM_TYPE = 'ASSB' and at.ACM_FLAG = 'Y' AND at.ACM_COMPANY_CODE = :companyCode"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":companyCode", comp_cd))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            locCheck = True
        Else
            locCheck = False
        End If
        Return locCheck
    End Function

    Public Function CheckSkillTemp(ByVal ReqNo As String, ByVal SpNo As String) As Boolean
        Dim locCheck As Boolean = True

        'Dim ls_sql As String = "select CCST_SKTD_CP_CD from hrace.T_CWM_CEMP_SKILL_TMP where CCST_SAFETY_PASS_NO=: passno and CCST_REQ_NO=:reqno and CCST_COMP_CODE=:companyCode"
        'Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        'cmd.Parameters.Add(New OracleParameter(":reqno", ReqNo))
        'cmd.Parameters.Add(New OracleParameter(":passno", SpNo))
        'cmd.Parameters.Add(New OracleParameter(":companyCode", comp_cd))
        'Dim dt As DataTable = getRecord(cmd, con)
        'If dt.Rows.Count > 0 Then
        '    locCheck = True
        'Else
        '    locCheck = False
        'End If

        Dim db As New DBConnection
        Dim qry As String = String.Empty
        Dim qry1 As String = String.Empty
        Dim req_no As String = String.Empty
        Dim parameters As OracleParameter()
        req_no = ReqNo
        Dim dt As New DataTable
        Dim dt1 As New DataTable
        Dim dtfile As DataTable

        qry = ""
        qry = "select CET_PROFILE_STATUS,CET_SAFETY_PASSNO,CET_DOB_CERT_NO,CET_LOCATION_CODE,CET_DEPT_CODE from hrace.T_CEMP_DETAILS_TMP "
        qry += "where CET_REQUEST_NO=:reqno"
        parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no)
                    }
        dtfile = db.GetDataFromQuery(qry, parameters)
        If dtfile.Rows.Count > 0 Then
            Dim passno As String = dtfile.Rows(0)("CET_SAFETY_PASSNO").ToString()
            Dim cerno As String = dtfile.Rows(0)("CET_DOB_CERT_NO").ToString()
            Dim locationcd As String = dtfile.Rows(0)("CET_LOCATION_CODE").ToString()
            Dim deptcd As String = dtfile.Rows(0)("CET_DEPT_CODE").ToString()
            If dtfile.Rows(0)("CET_PROFILE_STATUS").ToString() = "I" Then
                Try
                    qry = ""                                           'For personal info tab
                    qry = "select * from hrace.T_CEMP_DETAILS_TMP "
                    qry += "where CET_REQUEST_NO=:reqno "
                    qry += "and CET_SAFETY_PASSNO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If

                    qry = ""                                                    'For address info tab
                    qry = "select * from hrace.T_CWM_CEMP_ADDRS_TMP "
                    qry += "where CCA_REQ_NO=:reqno "
                    qry += "and CCA_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If

                    qry = ""                                                    'For skill info tab
                    qry = "select * from hrace.t_cwm_cemp_skill_TMP "
                    qry += "where CCST_REQ_NO=:reqno "
                    qry += "and CCST_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If


                    If cerno = "0" Then                                        'For age proof tab
                        locCheck = False
                    End If


                    qry = ""                                                    'For Qualification info tab
                    qry = "select * from hrace.T_CWM_CEMP_QUALIFICATIONS_TMP "
                    qry += "where CQL_REQ_NO=:reqno "
                    qry += "and CQL_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If


                    qry = ""                                                    'For Experience info tab
                    qry = "select CWET_SAFETY_PASS_NO from hrace.t_cwm_exp_tmp "
                    qry += "where CWET_REQ_NO=:reqno "
                    qry += "and CWET_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If


                    qry = ""                                                    'For Nominee info tab
                    qry = "select * from hrace.T_CWM_CEMP_NOMINEES_TMP "
                    qry += "where CCN_REQ_NO=:reqno "
                    qry += "and CCN_SAFETY_PASS_NO=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("reqno", req_no),
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If


                    qry = ""                                                    'For consent info tab
                    qry = "select * from hrace.t_cemp_piiconsent_details "
                    qry += "where CND_SAFETYPASS_NUM=:passno "

                    parameters = New OracleParameter() _
                    {
                      New OracleParameter("passno", passno)
                    }
                    dtfile = db.GetDataFromQuery(qry, parameters)
                    If dtfile.Rows.Count = 0 Then
                        locCheck = False
                    End If

                Catch ex As Exception

                End Try
            Else

                locCheck = True
            End If
        End If

        Return locCheck
    End Function

    Public Function CheckAssesmnetType(ByVal ReqNo As String, ByVal SpNo As String) As DataTable
        Dim ls_sql As String = "select nvl(CCST_ASSESSMENT_TYPE,'NA') as CCST_ASSESSMENT_TYPE from hrace.T_CWM_CEMP_SKILL_TMP where CCST_SAFETY_PASS_NO=:passno and CCST_REQ_NO=:reqno and CCST_COMP_CODE=:companyCode and CCST_CREATED_DT = (select max(CCST_CREATED_DT) from hrace.T_CWM_CEMP_SKILL_TMP where CCST_SAFETY_PASS_NO=:passno and CCST_REQ_NO=:reqno and CCST_COMP_CODE=:companyCode)"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":reqno", ReqNo))
        cmd.Parameters.Add(New OracleParameter(":passno", SpNo))
        cmd.Parameters.Add(New OracleParameter(":companyCode", comp_cd))
        Dim dt As DataTable = getRecord(cmd, con)
        Return dt
    End Function

    Public Function CheckReq_MedCategory(ByVal ReqNo As String, ByVal spno As String) As DataTable
        Dim ls_sql As String = "SELECT nvl(CET_REQ_CATEGORY,'0') as CET_REQ_CATEGORY,nvl(CET_MEDICAL_CENTRE,'NA') as CET_MEDICAL_CENTRE FROM HRACE.t_cemp_details_tmp  where CET_SAFETY_PASSNO = :spno and CET_REQUEST_NO=:reqno and CET_LOCATION_CODE=:companyCode"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":reqno", ReqNo))
        cmd.Parameters.Add(New OracleParameter(":spno", spno))
        cmd.Parameters.Add(New OracleParameter(":companyCode", comp_cd))
        Dim dt As DataTable = getRecord(cmd, con)
        Return dt
    End Function

    Public Function ChecReqCategory(ByVal ReqNo As String, ByVal spno As String) As Boolean
        Dim locCheck As Boolean
        Dim ls_sql As String = "SELECT nvl(CET_REQ_CATEGORY,'0') as CET_REQ_CATEGORY FROM HRACE.t_cemp_details_tmp  where CET_SAFETY_PASSNO = :spno and CET_REQUEST_NO=:reqno and CET_LOCATION_CODE=:companyCode and CET_REQ_CATEGORY='1' "
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":reqno", ReqNo))
        cmd.Parameters.Add(New OracleParameter(":spno", spno))
        cmd.Parameters.Add(New OracleParameter(":companyCode", comp_cd))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            locCheck = True
        Else
            locCheck = False
        End If
        Return locCheck
    End Function

    Protected Sub lnkbShowFlowchart1_click(ByVal sender As Object, ByVal e As System.EventArgs)

        If (divFlowChartDtls1.Visible) Then
            divFlowChartDtls1.Visible = False
        Else
            divFlowChartDtls1.Visible = True
        End If
    End Sub

    Protected Sub btn102_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn102.Click
        If tabcontainer1.Style("display") = "none" Then
            tabcontainer1.Style.Remove("display")
        Else
            tabcontainer1.Style.Add("display", "none")
        End If
    End Sub

    Protected Sub btn104_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn104.Click
        ScriptManager.RegisterClientScriptBlock(Me, Me.[GetType](), "popup", "window.open('https://tmh.tatasteel.co.in/account-management/login','_blank')", True)
    End Sub

    Protected Sub btn106_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn106.Click
        ScriptManager.RegisterClientScriptBlock(Me, Me.[GetType](), "popup", "window.open('https://clm.tatasteel.co.in/CLM/default.aspx','_blank')", True)
    End Sub

    Protected Sub lnkbShowFlowchart2_click(ByVal sender As Object, ByVal e As System.EventArgs)

        If (divFlowChartDtls2.Visible) Then
            divFlowChartDtls2.Visible = False
        Else
            divFlowChartDtls2.Visible = True
        End If
    End Sub

    Protected Sub btn202_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn202.Click
        If tabcontainer1.Style("display") = "none" Then
            tabcontainer1.Style.Remove("display")
        Else
            tabcontainer1.Style.Add("display", "none")
        End If
    End Sub

    Protected Sub btn203_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn203.Click
        ScriptManager.RegisterClientScriptBlock(Me, Me.[GetType](), "popup", "window.open('https://tmh.tatasteel.co.in/account-management/login','_blank')", True)
    End Sub

    Protected Sub lnkbShowFlowchart3_click(ByVal sender As Object, ByVal e As System.EventArgs)

        If (divFlowChartDtls3.Visible) Then
            divFlowChartDtls3.Visible = False
        Else
            divFlowChartDtls3.Visible = True
        End If
    End Sub

    Protected Sub btn302_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn302.Click
        If tabcontainer1.Style("display") = "none" Then
            tabcontainer1.Style.Remove("display")
        Else
            tabcontainer1.Style.Add("display", "none")
        End If
    End Sub

    Protected Sub btn304_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btn304.Click
        ScriptManager.RegisterClientScriptBlock(Me, Me.[GetType](), "popup", "window.open('https://tmh.tatasteel.co.in/account-management/login','_blank')", True)
    End Sub

    Public Sub PaintWireFrame1(ByVal SpNo As String)
        btn101.BackColor = Color.LightSteelBlue
        btn102.BackColor = Color.LightSteelBlue
        btn103.BackColor = Color.LightSteelBlue
        btn104.BackColor = Color.LightSteelBlue
        btn105.BackColor = Color.LightSteelBlue
        btn106.BackColor = Color.LightSteelBlue
        btn107.BackColor = Color.LightSteelBlue
        btn108.BackColor = Color.LightSteelBlue

        btn109.BackColor = Color.LightSteelBlue
        btn110.BackColor = Color.LightSteelBlue
        btn111.BackColor = Color.LightSteelBlue
        btn112.BackColor = Color.LightSteelBlue
        btn113.BackColor = Color.LightSteelBlue
        btn114.BackColor = Color.LightSteelBlue
        btn115.BackColor = Color.LightSteelBlue
        btn116.BackColor = Color.LightSteelBlue
        btn117.BackColor = Color.LightSteelBlue

        Dim SPReqNumber As String = Session("requestnumber").trim

        Dim ls_sql As String = "select distinct SRS_REQ_NO from hrace.T_SP_REQ_STATUS where SRS_REQ_NO = :ReqNo and SRS_STATUS = 'H1' and SRS_SUB_STATUS ='5' and SRS_AGENT_TYP = 'HR'"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            btn101.BackColor = Color.Green
            'If (Session("requestType") = "SPN") Then
            '    btn112.BackColor = Color.Green
            'End If
        Else
            btn101.BackColor = Color.Orange
        End If

        Dim ls_sql_pc As String = "select distinct CET_REQUEST_NO,CET_PROFILE_STATUS,CET_DOCVER_STATUS from hrace.t_cemp_details_tmp where CET_REQUEST_NO = :ReqNo and CET_SAFETY_PASSNO =:SpNo and CET_LOCATION_CODE =:comp_cd "
        Dim cmd_pc As OracleCommand = New OracleCommand(ls_sql_pc, con)
        cmd_pc.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_pc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pc.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pc As DataTable = getRecord(cmd_pc, con)
        If dt_pc.Rows.Count > 0 Then
            If (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "C") Then
                btn102.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "R") Then
                btn102.BackColor = Color.Red
            Else
                If (btn101.BackColor = Color.Orange) Then
                    btn102.BackColor = Color.LightSteelBlue
                Else
                    btn102.BackColor = Color.Orange
                End If
            End If

            'If (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "C") Then
            '    btn105.BackColor = Color.Green
            'ElseIf (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "R") Then
            '    btn105.BackColor = Color.Red
            'Else
            '    If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange) Then
            '        btn105.BackColor = Color.LightSteelBlue
            '    Else
            '        btn105.BackColor = Color.Orange
            '    End If
            'End If
        End If

        Dim ls_sql_bio As String = "select CEP_FINGERPRINTR from hrace.t_cemp_photo where CEP_SAFETY_PASS_NO=:SpNo "
        Dim cmd_bio As OracleCommand = New OracleCommand(ls_sql_bio, con)
        cmd_bio.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_bio As DataTable = getRecord(cmd_bio, con)
        If dt_bio.Rows.Count > 0 Then
            If Not IsDBNull(dt_bio.Rows(0)("CEP_FINGERPRINTR")) Then
                btn103.BackColor = Color.Green
            Else
                If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange) Then
                    btn103.BackColor = Color.LightSteelBlue
                Else
                    btn103.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange) Then
                btn103.BackColor = Color.LightSteelBlue
            Else
                btn103.BackColor = Color.Orange
            End If
        End If

        If dt_pc.Rows.Count > 0 Then
            If (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "C") Then
                btn105.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "R") Then
                btn105.BackColor = Color.Red
            Else
                If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange) Then
                    btn105.BackColor = Color.LightSteelBlue
                Else
                    btn105.BackColor = Color.Orange
                End If
            End If
        End If

        Dim ls_sql_md As String = "select CMH_SAFETY_PASS_NO from hrace.t_Cwm_Cemp_Medical_Hdr where CMH_SAFETY_PASS_NO = :SpNo and CMH_COMP_CODE =:comp_cd and TRUNC(CMH_VALIDITY_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_md As OracleCommand = New OracleCommand(ls_sql_md, con)
        cmd_md.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_md.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_md.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_md As DataTable = getRecord(cmd_md, con)
        If dt_md.Rows.Count > 0 Then
            btn104.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange) Then
                btn104.BackColor = Color.LightSteelBlue
            Else
                btn104.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sks As String = "select SKC_ASSESSMENT_DATE,SKC_SKTD_CP_CD from hrace.t_skill_certification where SKC_SAFETY_PASS_NO =:SpNo and SKC_REQ_NO =:ReqNo  and SKC_COMPANY_CD=:comp_cd "
        Dim cmd_sks As OracleCommand = New OracleCommand(ls_sql_sks, con)
        cmd_sks.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sks.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sks.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sks As DataTable = getRecord(cmd_sks, con)
        If dt_sks.Rows.Count > 0 Then
            If (dt_sks.Rows(0).Item("SKC_ASSESSMENT_DATE").ToString <> "") Then
                btn106.BackColor = Color.Green

                Dim ls_sql_ska As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO=:SpNo AND TCD_CERT_CATEG<>'FAIL' and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD and TCD_VALID_TAG='Y' "
                Dim cmd_ska As OracleCommand = New OracleCommand(ls_sql_ska, con)
                cmd_ska.Parameters.Add(New OracleParameter(":SpNo", SpNo))
                cmd_ska.Parameters.Add(New OracleParameter(":TCD_CLM_SKILL_CD", dt_sks.Rows(0).Item("SKC_SKTD_CP_CD").ToString))
                Dim dt_ska As DataTable = getRecord(cmd_ska, con)
                If dt_ska.Rows.Count > 0 Then
                    btn107.BackColor = Color.Green
                Else
                    If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange) Then
                        btn107.BackColor = Color.LightSteelBlue
                    Else
                        btn107.BackColor = Color.Orange
                    End If
                End If

            Else
                If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange) Then
                    btn106.BackColor = Color.LightSteelBlue
                Else
                    btn106.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange) Then
                btn106.BackColor = Color.LightSteelBlue
            Else
                btn106.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sts As String = "SELECT t.cst_safety_no FROM HRACE.T_CWM_SAFETY_TRN t where t.cst_safety_no=:SpNo and t.cst_request_no=:ReqNo and t.CST_COMPANY_CODE=:comp_cd "
        Dim cmd_sts As OracleCommand = New OracleCommand(ls_sql_sts, con)
        cmd_sts.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sts.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sts.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sts As DataTable = getRecord(cmd_sts, con)
        If dt_sts.Rows.Count > 0 Then
            btn108.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange) Then
                btn108.BackColor = Color.LightSteelBlue
            Else
                btn108.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_skf As String = "select CCT_SAFETY_PASS_NO from hrace.t_cwm_cemp_trns where CCT_SAFETY_PASS_NO = :SpNo and TRUNC(CCT_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_skf As OracleCommand = New OracleCommand(ls_sql_skf, con)
        cmd_skf.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_skf.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt_skf As DataTable = getRecord(cmd_skf, con)
        If dt_skf.Rows.Count > 0 Then
            btn109.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange) Then
                btn109.BackColor = Color.LightSteelBlue
            Else
                btn109.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sfa As String = "select CED_SAFETY_PASS_NO from hrace.t_cemp_details where CED_SAFETY_PASS_NO=:SpNo and CED_COMPANY_CODE=:comp_cd and CED_SP_ENABLED='Y' "
        Dim cmd_sfa As OracleCommand = New OracleCommand(ls_sql_sfa, con)
        cmd_sfa.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sfa.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sfa As DataTable = getRecord(cmd_sfa, con)
        If dt_sfa.Rows.Count > 0 Then
            btn110.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange Or btn109.BackColor = Color.Orange) Then
                btn110.BackColor = Color.LightSteelBlue
            Else
                btn110.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp As String = "select G.GRW_SAFETY_PASS_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate) "
        Dim cmd_gp As OracleCommand = New OracleCommand(ls_sql_gp, con)
        cmd_gp.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp As DataTable = getRecord(cmd_gp, con)
        If dt_gp.Rows.Count > 0 Then
            btn111.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange Or btn109.BackColor = Color.Orange Or btn110.BackColor = Color.Orange) Then
                btn111.BackColor = Color.LightSteelBlue
            Else
                btn111.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_bu As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'A1' and RQS_SUBSTATUS = '3' "
        Dim cmd_gp_bu As OracleCommand = New OracleCommand(ls_sql_gp_bu, con)
        cmd_gp_bu.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_bu As DataTable = getRecord(cmd_gp_bu, con)
        If dt_gp_bu.Rows.Count > 0 Then
            btn112.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange Or btn109.BackColor = Color.Orange Or btn110.BackColor = Color.Orange Or btn111.BackColor = Color.Orange) Then
                btn112.BackColor = Color.LightSteelBlue
            Else
                btn112.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_cc As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'B1' and RQS_SUBSTATUS = '5' "
        Dim cmd_gp_cc As OracleCommand = New OracleCommand(ls_sql_gp_cc, con)
        cmd_gp_cc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_cc As DataTable = getRecord(cmd_gp_cc, con)
        If dt_gp_cc.Rows.Count > 0 Then
            btn113.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange Or btn109.BackColor = Color.Orange Or btn110.BackColor = Color.Orange Or btn111.BackColor = Color.Orange Or btn112.BackColor = Color.Orange) Then
                btn113.BackColor = Color.LightSteelBlue
            Else
                btn113.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_sec As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) "
        Dim cmd_gp_sec As OracleCommand = New OracleCommand(ls_sql_gp_sec, con)
        cmd_gp_sec.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_sec.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_sec As DataTable = getRecord(cmd_gp_sec, con)
        If dt_gp_sec.Rows.Count > 0 Then
            btn114.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange Or btn109.BackColor = Color.Orange Or btn110.BackColor = Color.Orange Or btn111.BackColor = Color.Orange Or btn112.BackColor = Color.Orange Or btn113.BackColor = Color.Orange) Then
                btn114.BackColor = Color.LightSteelBlue
            Else
                btn114.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_rel As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) and CGP_RELEASE_DT is not null "
        Dim cmd_gp_rel As OracleCommand = New OracleCommand(ls_sql_gp_rel, con)
        cmd_gp_rel.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_rel.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_rel As DataTable = getRecord(cmd_gp_rel, con)
        If dt_gp_rel.Rows.Count > 0 Then
            btn115.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange Or btn103.BackColor = Color.Orange Or btn104.BackColor = Color.Orange Or btn106.BackColor = Color.Orange Or btn107.BackColor = Color.Orange Or btn108.BackColor = Color.Orange Or btn109.BackColor = Color.Orange Or btn110.BackColor = Color.Orange Or btn111.BackColor = Color.Orange Or btn112.BackColor = Color.Orange Or btn113.BackColor = Color.Orange Or btn114.BackColor = Color.Orange) Then
                btn115.BackColor = Color.LightSteelBlue
            Else
                btn115.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pv As String = "SELECT CPD_SAFETY_PASS_NO FROM hrace.T_CWM_PV_DTL WHERE CPD_SAFETY_PASS_NO=:SpNo and CPD_COMP_CODE =:comp_cd  and TRUNC(CPD_END_DT) >= TRUNC(SYSDATE)  "
        Dim cmd_pv As OracleCommand = New OracleCommand(ls_sql_pv, con)
        cmd_pv.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pv.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pv As DataTable = getRecord(cmd_pv, con)
        If dt_pv.Rows.Count > 0 Then
            btn116.BackColor = Color.Green
        Else
            If (btn101.BackColor = Color.Orange Or btn102.BackColor = Color.Orange) Then
                btn116.BackColor = Color.LightSteelBlue
            Else
                btn116.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pmj As String = "select CED_SAFETY_PASS_NO from t_cemp_details where CED_PMJJBY_DOC_ID is not null and CED_SAFETY_PASS_NO=:SpNo  "
        Dim cmd_pmj As OracleCommand = New OracleCommand(ls_sql_pmj, con)
        cmd_pmj.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_pmj As DataTable = getRecord(cmd_pmj, con)
        If dt_pmj.Rows.Count > 0 Then
            btn117.BackColor = Color.Green
        Else
            If (btn116.BackColor = Color.Orange Or btn102.BackColor = Color.Orange) Then
                btn117.BackColor = Color.LightSteelBlue
            Else
                btn117.BackColor = Color.Orange
            End If
        End If
    End Sub

    Public Sub PaintWireFrame2(ByVal SpNo As String)
        btn201.BackColor = Color.LightSteelBlue
        btn202.BackColor = Color.LightSteelBlue
        btn203.BackColor = Color.LightSteelBlue
        btn204.BackColor = Color.LightSteelBlue
        btn205.BackColor = Color.LightSteelBlue
        btn206.BackColor = Color.LightSteelBlue
        btn207.BackColor = Color.LightSteelBlue
        btn208.BackColor = Color.LightSteelBlue

        btn209.BackColor = Color.LightSteelBlue
        btn210.BackColor = Color.LightSteelBlue
        btn211.BackColor = Color.LightSteelBlue
        btn212.BackColor = Color.LightSteelBlue
        btn213.BackColor = Color.LightSteelBlue
        btn214.BackColor = Color.LightSteelBlue
        btn215.BackColor = Color.LightSteelBlue
        btn216.BackColor = Color.LightSteelBlue
        btn217.BackColor = Color.LightSteelBlue

        Dim SPReqNumber As String = Session("requestnumber").trim

        Dim ls_sql As String = "select distinct SRS_REQ_NO from hrace.T_SP_REQ_STATUS where SRS_REQ_NO = :ReqNo and SRS_STATUS = 'H1' and SRS_SUB_STATUS ='5' and SRS_AGENT_TYP = 'HR'"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            btn201.BackColor = Color.Green
            'If (Session("requestType") = "SPN") Then
            '    btn212.BackColor = Color.Green
            'End If
        Else
            btn201.BackColor = Color.Orange
        End If

        Dim ls_sql_pc As String = "select distinct CET_REQUEST_NO,CET_PROFILE_STATUS,CET_DOCVER_STATUS from hrace.t_cemp_details_tmp where CET_REQUEST_NO = :ReqNo and CET_SAFETY_PASSNO =:SpNo and CET_LOCATION_CODE =:comp_cd "
        Dim cmd_pc As OracleCommand = New OracleCommand(ls_sql_pc, con)
        cmd_pc.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_pc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pc.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pc As DataTable = getRecord(cmd_pc, con)
        If dt_pc.Rows.Count > 0 Then
            btn202.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange) Then
                btn202.BackColor = Color.LightSteelBlue
            Else
                btn202.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_md As String = "select CMH_SAFETY_PASS_NO from hrace.t_Cwm_Cemp_Medical_Hdr where CMH_SAFETY_PASS_NO = :SpNo and CMH_COMP_CODE =:comp_cd and TRUNC(CMH_VALIDITY_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_md As OracleCommand = New OracleCommand(ls_sql_md, con)
        cmd_md.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_md.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_md.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_md As DataTable = getRecord(cmd_md, con)
        If dt_md.Rows.Count > 0 Then
            btn203.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange) Then
                btn203.BackColor = Color.LightSteelBlue
            Else
                btn203.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sks As String = "select SKC_ASSESSMENT_DATE,SKC_SKTD_CP_CD from hrace.t_skill_certification where SKC_SAFETY_PASS_NO =:SpNo and SKC_REQ_NO =:ReqNo  and SKC_COMPANY_CD=:comp_cd "
        Dim cmd_sks As OracleCommand = New OracleCommand(ls_sql_sks, con)
        cmd_sks.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sks.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sks.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sks As DataTable = getRecord(cmd_sks, con)
        If dt_sks.Rows.Count > 0 Then
            If (dt_sks.Rows(0).Item("SKC_ASSESSMENT_DATE").ToString <> "") Then
                Dim ls_sql_ska As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO=:SpNo AND TCD_CERT_CATEG<>'FAIL' and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD and TCD_VALID_TAG='Y' "
                Dim cmd_ska As OracleCommand = New OracleCommand(ls_sql_ska, con)
                cmd_ska.Parameters.Add(New OracleParameter(":SpNo", SpNo))
                cmd_ska.Parameters.Add(New OracleParameter(":TCD_CLM_SKILL_CD", dt_sks.Rows(0).Item("SKC_SKTD_CP_CD").ToString))
                Dim dt_ska As DataTable = getRecord(cmd_ska, con)
                If dt_ska.Rows.Count > 0 Then
                    btn204.BackColor = Color.Green
                Else
                    If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange) Then
                        btn204.BackColor = Color.LightSteelBlue
                    Else
                        btn204.BackColor = Color.Orange
                    End If
                End If
            Else
                If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange) Then
                    btn204.BackColor = Color.LightSteelBlue
                Else
                    btn204.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange) Then
                btn204.BackColor = Color.LightSteelBlue
            Else
                btn204.BackColor = Color.Orange
            End If
        End If

        If dt_pc.Rows.Count > 0 Then
            btn202.BackColor = Color.Green
            If (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "C") Then
                btn205.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "R") Then
                btn205.BackColor = Color.Red
            Else
                If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange) Then
                    btn205.BackColor = Color.LightSteelBlue
                Else
                    btn205.BackColor = Color.Orange
                End If
            End If

            If (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "C") Then
                btn206.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "R") Then
                btn206.BackColor = Color.Red
            Else
                If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange) Then
                    btn206.BackColor = Color.LightSteelBlue
                Else
                    btn206.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn201.BackColor = Color.Orange) Then
                btn202.BackColor = Color.LightSteelBlue
            Else
                btn202.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_bio As String = "select CEP_FINGERPRINTR from hrace.t_cemp_photo where CEP_SAFETY_PASS_NO=:SpNo "
        Dim cmd_bio As OracleCommand = New OracleCommand(ls_sql_bio, con)
        cmd_bio.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_bio As DataTable = getRecord(cmd_bio, con)
        If dt_bio.Rows.Count > 0 Then
            If Not IsDBNull(dt_bio.Rows(0)("CEP_FINGERPRINTR")) Then
                btn207.BackColor = Color.Green
            Else
                If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange) Then
                    btn207.BackColor = Color.LightSteelBlue
                Else
                    btn207.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange) Then
                btn207.BackColor = Color.LightSteelBlue
            Else
                btn207.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sts As String = "SELECT t.cst_safety_no FROM HRACE.T_CWM_SAFETY_TRN t where t.cst_safety_no=:SpNo and t.cst_request_no=:ReqNo and t.CST_COMPANY_CODE=:comp_cd "
        Dim cmd_sts As OracleCommand = New OracleCommand(ls_sql_sts, con)
        cmd_sts.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sts.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sts.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sts As DataTable = getRecord(cmd_sts, con)
        If dt_sts.Rows.Count > 0 Then
            btn208.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange) Then
                btn208.BackColor = Color.LightSteelBlue
            Else
                btn208.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_skf As String = "select CCT_SAFETY_PASS_NO from hrace.t_cwm_cemp_trns where CCT_SAFETY_PASS_NO = :SpNo and TRUNC(CCT_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_skf As OracleCommand = New OracleCommand(ls_sql_skf, con)
        cmd_skf.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_skf.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt_skf As DataTable = getRecord(cmd_skf, con)
        If dt_skf.Rows.Count > 0 Then
            btn209.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange) Then
                btn209.BackColor = Color.LightSteelBlue
            Else
                btn209.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sfa As String = "select CED_SAFETY_PASS_NO from hrace.t_cemp_details where CED_SAFETY_PASS_NO=:SpNo and CED_COMPANY_CODE=:comp_cd and CED_SP_ENABLED='Y' "
        Dim cmd_sfa As OracleCommand = New OracleCommand(ls_sql_sfa, con)
        cmd_sfa.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sfa.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sfa As DataTable = getRecord(cmd_sfa, con)
        If dt_sfa.Rows.Count > 0 Then
            btn210.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange Or btn209.BackColor = Color.Orange) Then
                btn210.BackColor = Color.LightSteelBlue
            Else
                btn210.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp As String = "select G.GRW_SAFETY_PASS_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate) "
        Dim cmd_gp As OracleCommand = New OracleCommand(ls_sql_gp, con)
        cmd_gp.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp As DataTable = getRecord(cmd_gp, con)
        If dt_gp.Rows.Count > 0 Then
            btn211.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange Or btn209.BackColor = Color.Orange Or btn210.BackColor = Color.Orange) Then
                btn211.BackColor = Color.LightSteelBlue
            Else
                btn211.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_bu As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'A1' and RQS_SUBSTATUS = '3' "
        Dim cmd_gp_bu As OracleCommand = New OracleCommand(ls_sql_gp_bu, con)
        cmd_gp_bu.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_bu As DataTable = getRecord(cmd_gp_bu, con)
        If dt_gp_bu.Rows.Count > 0 Then
            btn212.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange Or btn209.BackColor = Color.Orange Or btn210.BackColor = Color.Orange Or btn211.BackColor = Color.Orange) Then
                btn212.BackColor = Color.LightSteelBlue
            Else
                btn212.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_cc As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'B1' and RQS_SUBSTATUS = '5' "
        Dim cmd_gp_cc As OracleCommand = New OracleCommand(ls_sql_gp_cc, con)
        cmd_gp_cc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_cc As DataTable = getRecord(cmd_gp_cc, con)
        If dt_gp_cc.Rows.Count > 0 Then
            btn213.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange Or btn209.BackColor = Color.Orange Or btn210.BackColor = Color.Orange Or btn211.BackColor = Color.Orange Or btn212.BackColor = Color.Orange) Then
                btn213.BackColor = Color.LightSteelBlue
            Else
                btn213.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_sec As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) "
        Dim cmd_gp_sec As OracleCommand = New OracleCommand(ls_sql_gp_sec, con)
        cmd_gp_sec.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_sec.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_sec As DataTable = getRecord(cmd_gp_sec, con)
        If dt_gp_sec.Rows.Count > 0 Then
            btn214.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange Or btn209.BackColor = Color.Orange Or btn210.BackColor = Color.Orange Or btn211.BackColor = Color.Orange Or btn212.BackColor = Color.Orange Or btn213.BackColor = Color.Orange) Then
                btn214.BackColor = Color.LightSteelBlue
            Else
                btn214.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_rel As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) and CGP_RELEASE_DT is not null "
        Dim cmd_gp_rel As OracleCommand = New OracleCommand(ls_sql_gp_rel, con)
        cmd_gp_rel.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_rel.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_rel As DataTable = getRecord(cmd_gp_rel, con)
        If dt_gp_rel.Rows.Count > 0 Then
            btn215.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange Or btn203.BackColor = Color.Orange Or btn204.BackColor = Color.Orange Or btn205.BackColor = Color.Orange Or btn206.BackColor = Color.Orange Or btn207.BackColor = Color.Orange Or btn208.BackColor = Color.Orange Or btn209.BackColor = Color.Orange Or btn210.BackColor = Color.Orange Or btn211.BackColor = Color.Orange Or btn212.BackColor = Color.Orange Or btn213.BackColor = Color.Orange Or btn214.BackColor = Color.Orange) Then
                btn215.BackColor = Color.LightSteelBlue
            Else
                btn215.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pv As String = "SELECT CPD_SAFETY_PASS_NO FROM hrace.T_CWM_PV_DTL WHERE CPD_SAFETY_PASS_NO=:SpNo and CPD_COMP_CODE =:comp_cd  and TRUNC(CPD_END_DT) >= TRUNC(SYSDATE)  "
        Dim cmd_pv As OracleCommand = New OracleCommand(ls_sql_pv, con)
        cmd_pv.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pv.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pv As DataTable = getRecord(cmd_pv, con)
        If dt_pv.Rows.Count > 0 Then
            btn216.BackColor = Color.Green
        Else
            If (btn201.BackColor = Color.Orange Or btn202.BackColor = Color.Orange) Then
                btn216.BackColor = Color.LightSteelBlue
            Else
                btn216.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pmj As String = "select CED_SAFETY_PASS_NO from t_cemp_details where CED_PMJJBY_DOC_ID is not null and CED_SAFETY_PASS_NO=:SpNo  "
        Dim cmd_pmj As OracleCommand = New OracleCommand(ls_sql_pmj, con)
        cmd_pmj.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_pmj As DataTable = getRecord(cmd_pmj, con)
        If dt_pmj.Rows.Count > 0 Then
            btn217.BackColor = Color.Green
        Else
            If (btn216.BackColor = Color.Orange Or btn202.BackColor = Color.Orange) Then
                btn217.BackColor = Color.LightSteelBlue
            Else
                btn217.BackColor = Color.Orange
            End If
        End If
    End Sub

    Public Sub PaintWireFrame3(ByVal SpNo As String)
        btn301.BackColor = Color.LightSteelBlue
        btn302.BackColor = Color.LightSteelBlue
        btn303.BackColor = Color.LightSteelBlue
        btn304.BackColor = Color.LightSteelBlue
        btn305.BackColor = Color.LightSteelBlue
        btn306.BackColor = Color.LightSteelBlue
        btn307.BackColor = Color.LightSteelBlue
        btn308.BackColor = Color.LightSteelBlue

        btn309.BackColor = Color.LightSteelBlue
        btn310.BackColor = Color.LightSteelBlue
        btn311.BackColor = Color.LightSteelBlue
        btn312.BackColor = Color.LightSteelBlue
        btn313.BackColor = Color.LightSteelBlue
        btn314.BackColor = Color.LightSteelBlue
        btn315.BackColor = Color.LightSteelBlue
        btn316.BackColor = Color.LightSteelBlue
        btn317.BackColor = Color.LightSteelBlue

        Dim SPReqNumber As String = Session("requestnumber").trim

        Dim ls_sql As String = "select distinct SRS_REQ_NO from hrace.T_SP_REQ_STATUS where SRS_REQ_NO = :ReqNo and SRS_STATUS = 'H1' and SRS_SUB_STATUS ='5' and SRS_AGENT_TYP = 'HR'"
        Dim cmd As OracleCommand = New OracleCommand(ls_sql, con)
        cmd.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt As DataTable = getRecord(cmd, con)
        If dt.Rows.Count > 0 Then
            btn301.BackColor = Color.Green
            'If (Session("requestType") = "SPN") Then
            '    btn312.BackColor = Color.Green
            'End If
        Else
            btn301.BackColor = Color.Orange
        End If

        Dim ls_sql_pc As String = "select distinct CET_REQUEST_NO,CET_PROFILE_STATUS,CET_DOCVER_STATUS from hrace.t_cemp_details_tmp where CET_REQUEST_NO = :ReqNo and CET_SAFETY_PASSNO =:SpNo and CET_LOCATION_CODE =:comp_cd "
        Dim cmd_pc As OracleCommand = New OracleCommand(ls_sql_pc, con)
        cmd_pc.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_pc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pc.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pc As DataTable = getRecord(cmd_pc, con)
        If dt_pc.Rows.Count > 0 Then
            btn302.BackColor = Color.Green
            'If (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "C") Then
            '    btn306.BackColor = Color.Green
            'ElseIf (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "R") Then
            '    btn306.BackColor = Color.Red
            'Else
            '    If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange) Then
            '        btn306.BackColor = Color.LightSteelBlue
            '    Else
            '        btn306.BackColor = Color.Orange
            '    End If
            'End If

            'If (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "C") Then
            '    btn307.BackColor = Color.Green
            'ElseIf (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "R") Then
            '    btn307.BackColor = Color.Red
            'Else
            '    If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange) Then
            '        btn307.BackColor = Color.LightSteelBlue
            '    Else
            '        btn307.BackColor = Color.Orange
            '    End If
            'End If
        Else
            If (btn301.BackColor = Color.Orange) Then
                btn302.BackColor = Color.LightSteelBlue
            Else
                btn302.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_bio As String = "select CEP_FINGERPRINTR from hrace.t_cemp_photo where CEP_SAFETY_PASS_NO=:SpNo "
        Dim cmd_bio As OracleCommand = New OracleCommand(ls_sql_bio, con)
        cmd_bio.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_bio As DataTable = getRecord(cmd_bio, con)
        If dt_bio.Rows.Count > 0 Then
            If Not IsDBNull(dt_bio.Rows(0)("CEP_FINGERPRINTR")) Then
                btn303.BackColor = Color.Green
            Else
                If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange) Then
                    btn303.BackColor = Color.LightSteelBlue
                Else
                    btn303.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange) Then
                btn303.BackColor = Color.LightSteelBlue
            Else
                btn303.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_md As String = "select CMH_SAFETY_PASS_NO from hrace.t_Cwm_Cemp_Medical_Hdr where CMH_SAFETY_PASS_NO = :SpNo and CMH_COMP_CODE =:comp_cd and TRUNC(CMH_VALIDITY_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_md As OracleCommand = New OracleCommand(ls_sql_md, con)
        cmd_md.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_md.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_md.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_md As DataTable = getRecord(cmd_md, con)
        If dt_md.Rows.Count > 0 Then
            btn304.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange) Then
                btn304.BackColor = Color.LightSteelBlue
            Else
                btn304.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sks As String = "select SKC_ASSESSMENT_DATE,SKC_SKTD_CP_CD from hrace.t_skill_certification where SKC_SAFETY_PASS_NO =:SpNo and SKC_REQ_NO =:ReqNo  and SKC_COMPANY_CD=:comp_cd "
        Dim cmd_sks As OracleCommand = New OracleCommand(ls_sql_sks, con)
        cmd_sks.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sks.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sks.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sks As DataTable = getRecord(cmd_sks, con)
        If dt_sks.Rows.Count > 0 Then
            If (dt_sks.Rows(0).Item("SKC_ASSESSMENT_DATE").ToString <> "") Then
                Dim ls_sql_ska As String = "Select TCD_CLM_SKILL_CD FROM hrps.t_td_clm_doc@ace_iris WHERE TCD_SP_NO=:SpNo AND TCD_CERT_CATEG<>'FAIL' and TCD_CLM_SKILL_CD=:TCD_CLM_SKILL_CD and TCD_VALID_TAG='Y' "
                Dim cmd_ska As OracleCommand = New OracleCommand(ls_sql_ska, con)
                cmd_ska.Parameters.Add(New OracleParameter(":SpNo", SpNo))
                cmd_ska.Parameters.Add(New OracleParameter(":TCD_CLM_SKILL_CD", dt_sks.Rows(0).Item("SKC_SKTD_CP_CD").ToString))
                Dim dt_ska As DataTable = getRecord(cmd_ska, con)
                If dt_ska.Rows.Count > 0 Then
                    btn305.BackColor = Color.Green
                Else
                    If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange) Then
                        btn305.BackColor = Color.LightSteelBlue
                    Else
                        btn305.BackColor = Color.Orange
                    End If
                End If
            Else
                If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange) Then
                    btn305.BackColor = Color.LightSteelBlue
                Else
                    btn305.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange) Then
                btn305.BackColor = Color.LightSteelBlue
            Else
                btn305.BackColor = Color.Orange
            End If
        End If

        If dt_pc.Rows.Count > 0 Then
            btn302.BackColor = Color.Green
            If (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "C") Then
                btn306.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_PROFILE_STATUS").ToString = "R") Then
                btn306.BackColor = Color.Red
            Else
                If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange) Then
                    btn306.BackColor = Color.LightSteelBlue
                Else
                    btn306.BackColor = Color.Orange
                End If
            End If

            If (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "C") Then
                btn307.BackColor = Color.Green
            ElseIf (dt_pc.Rows(0).Item("CET_DOCVER_STATUS").ToString = "R") Then
                btn307.BackColor = Color.Red
            Else
                If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange) Then
                    btn307.BackColor = Color.LightSteelBlue
                Else
                    btn307.BackColor = Color.Orange
                End If
            End If
        Else
            If (btn301.BackColor = Color.Orange) Then
                btn302.BackColor = Color.LightSteelBlue
            Else
                btn302.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sts As String = "SELECT t.cst_safety_no FROM HRACE.T_CWM_SAFETY_TRN t where t.cst_safety_no=:SpNo and t.cst_request_no=:ReqNo and t.CST_COMPANY_CODE=:comp_cd "
        Dim cmd_sts As OracleCommand = New OracleCommand(ls_sql_sts, con)
        cmd_sts.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sts.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        cmd_sts.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sts As DataTable = getRecord(cmd_sts, con)
        If dt_sts.Rows.Count > 0 Then
            btn308.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange) Then
                btn308.BackColor = Color.LightSteelBlue
            Else
                btn308.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_skf As String = "select CCT_SAFETY_PASS_NO from hrace.t_cwm_cemp_trns where CCT_SAFETY_PASS_NO = :SpNo and TRUNC(CCT_START_DT) >= (select TRUNC(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:ReqNo) "
        Dim cmd_skf As OracleCommand = New OracleCommand(ls_sql_skf, con)
        cmd_skf.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_skf.Parameters.Add(New OracleParameter(":ReqNo", SPReqNumber))
        Dim dt_skf As DataTable = getRecord(cmd_skf, con)
        If dt_skf.Rows.Count > 0 Then
            btn309.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange) Then
                btn309.BackColor = Color.LightSteelBlue
            Else
                btn309.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_sfa As String = "select CED_SAFETY_PASS_NO from hrace.t_cemp_details where CED_SAFETY_PASS_NO=:SpNo and CED_COMPANY_CODE=:comp_cd and CED_SP_ENABLED='Y' "
        Dim cmd_sfa As OracleCommand = New OracleCommand(ls_sql_sfa, con)
        cmd_sfa.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_sfa.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_sfa As DataTable = getRecord(cmd_sfa, con)
        If dt_sfa.Rows.Count > 0 Then
            btn310.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange Or btn309.BackColor = Color.Orange) Then
                btn310.BackColor = Color.LightSteelBlue
            Else
                btn310.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp As String = "select G.GRW_SAFETY_PASS_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate) "
        Dim cmd_gp As OracleCommand = New OracleCommand(ls_sql_gp, con)
        cmd_gp.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp As DataTable = getRecord(cmd_gp, con)
        If dt_gp.Rows.Count > 0 Then
            btn311.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange Or btn309.BackColor = Color.Orange Or btn310.BackColor = Color.Orange) Then
                btn311.BackColor = Color.LightSteelBlue
            Else
                btn311.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_bu As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'A1' and RQS_SUBSTATUS = '3' "
        Dim cmd_gp_bu As OracleCommand = New OracleCommand(ls_sql_gp_bu, con)
        cmd_gp_bu.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_bu As DataTable = getRecord(cmd_gp_bu, con)
        If dt_gp_bu.Rows.Count > 0 Then
            btn312.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange Or btn309.BackColor = Color.Orange Or btn310.BackColor = Color.Orange Or btn311.BackColor = Color.Orange) Then
                btn312.BackColor = Color.LightSteelBlue
            Else
                btn312.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_cc As String = "select RQS_PROPOSAL_ID from hrace.t_req_status S where S.RQS_PROPOSAL_ID in (select G.GRW_GATEPASS_REQ_NO from hrace.t_gprequest_workmen_details G where G.GRW_SAFETY_PASS_NO=:SpNo and trunc(GRW_GP_VALID_TILL)>= trunc(sysdate)) and RQS_STATUS = 'B1' and RQS_SUBSTATUS = '5' "
        Dim cmd_gp_cc As OracleCommand = New OracleCommand(ls_sql_gp_cc, con)
        cmd_gp_cc.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_gp_cc As DataTable = getRecord(cmd_gp_cc, con)
        If dt_gp_cc.Rows.Count > 0 Then
            btn313.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange Or btn309.BackColor = Color.Orange Or btn310.BackColor = Color.Orange Or btn311.BackColor = Color.Orange Or btn312.BackColor = Color.Orange) Then
                btn313.BackColor = Color.LightSteelBlue
            Else
                btn313.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_sec As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) "
        Dim cmd_gp_sec As OracleCommand = New OracleCommand(ls_sql_gp_sec, con)
        cmd_gp_sec.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_sec.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_sec As DataTable = getRecord(cmd_gp_sec, con)
        If dt_gp_sec.Rows.Count > 0 Then
            btn314.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange Or btn309.BackColor = Color.Orange Or btn310.BackColor = Color.Orange Or btn311.BackColor = Color.Orange Or btn312.BackColor = Color.Orange Or btn313.BackColor = Color.Orange) Then
                btn314.BackColor = Color.LightSteelBlue
            Else
                btn314.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_gp_rel As String = "select CGP_SAFETY_PASS_NO from hrace.t_cemp_gatepass where CGP_SAFETY_PASS_NO =:SpNo and CGP_COMP_CODE=:comp_cd and CGP_GP_ENABLED='Y' and CGP_GP_BLOCKED = 'N' and trunc(CGP_GP_VALID_TILL)>=trunc(sysdate) and CGP_RELEASE_DT is not null "
        Dim cmd_gp_rel As OracleCommand = New OracleCommand(ls_sql_gp_rel, con)
        cmd_gp_rel.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_gp_rel.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_gp_rel As DataTable = getRecord(cmd_gp_rel, con)
        If dt_gp_rel.Rows.Count > 0 Then
            btn315.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange Or btn303.BackColor = Color.Orange Or btn304.BackColor = Color.Orange Or btn305.BackColor = Color.Orange Or btn306.BackColor = Color.Orange Or btn307.BackColor = Color.Orange Or btn308.BackColor = Color.Orange Or btn309.BackColor = Color.Orange Or btn310.BackColor = Color.Orange Or btn311.BackColor = Color.Orange Or btn312.BackColor = Color.Orange Or btn313.BackColor = Color.Orange Or btn314.BackColor = Color.Orange) Then
                btn315.BackColor = Color.LightSteelBlue
            Else
                btn315.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pv As String = "SELECT CPD_SAFETY_PASS_NO FROM hrace.T_CWM_PV_DTL WHERE CPD_SAFETY_PASS_NO=:SpNo and CPD_COMP_CODE =:comp_cd  and TRUNC(CPD_END_DT) >= TRUNC(SYSDATE)  "
        Dim cmd_pv As OracleCommand = New OracleCommand(ls_sql_pv, con)
        cmd_pv.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        cmd_pv.Parameters.Add(New OracleParameter(":comp_cd", comp_cd))
        Dim dt_pv As DataTable = getRecord(cmd_pv, con)
        If dt_pv.Rows.Count > 0 Then
            btn316.BackColor = Color.Green
        Else
            If (btn301.BackColor = Color.Orange Or btn302.BackColor = Color.Orange) Then
                btn316.BackColor = Color.LightSteelBlue
            Else
                btn316.BackColor = Color.Orange
            End If
        End If

        Dim ls_sql_pmj As String = "select CED_SAFETY_PASS_NO from t_cemp_details where CED_PMJJBY_DOC_ID is not null and CED_SAFETY_PASS_NO=:SpNo  "
        Dim cmd_pmj As OracleCommand = New OracleCommand(ls_sql_pmj, con)
        cmd_pmj.Parameters.Add(New OracleParameter(":SpNo", SpNo))
        Dim dt_pmj As DataTable = getRecord(cmd_pmj, con)
        If dt_pmj.Rows.Count > 0 Then
            btn317.BackColor = Color.Green
        Else
            If (btn316.BackColor = Color.Orange Or btn302.BackColor = Color.Orange) Then
                btn317.BackColor = Color.LightSteelBlue
            Else
                btn317.BackColor = Color.Orange
            End If
        End If
    End Sub
End Class

