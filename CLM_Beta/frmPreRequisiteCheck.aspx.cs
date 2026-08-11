using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OracleClient;
using System.Globalization;
using System.Configuration;
using System.Web.Services;
using System.Web.Script.Services;

public partial class frmPreRequisiteCheck : System.Web.UI.Page
{
    OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString);
    string comp_cd = "";
    string vVencode = "";

    string WR = "";
    string SV = "";
    string VC = "";
    string DV = "";
    string FM = "";
    string SH = "";
    string SF = "";

    string SPN = "";
    string SPR = "";
    string submit_code = "";
    string str_vendor_emails = "";
    string s_ctm_code = "";
    string str_ctm_code_det = "";
    string workorder = "";
    string str_vendor = "";
    string gatepass_count = "";
    string RFID_count = "";

    public string vVend_UserID = "";

    CLMVendClass clmClass = new CLMVendClass();

    protected void Page_Load(object sender, EventArgs e)
    {
        Session["VendCode"] = "T224";
        Session["Comp_code"] = "1112";
        Session.Remove("Progress");
        if (!string.IsNullOrEmpty(Session["VendCode"] as string))
        {
            vVencode = Session["VendCode"] as string;
            comp_cd = Session["Comp_code"] as string;
        }
        else
        {
            Response.Redirect("frmInterface.aspx");
            return;
        }

        if (!string.IsNullOrEmpty(Session["VendCode"] as string))
        {
            employeeType();
            vVend_UserID = Session["VendCode"].ToString();

            if (vVend_UserID.Length > 10)
            {
                vVend_UserID = vVend_UserID.Substring(0, 10);
            }
        }
        else
        {
            Response.Redirect("frmInterface.aspx");
            return;
        }

        str_vendor = "Select VDT_VENDOR_NAME,VDT_LOCATION_CODE,VDT_EMAIL1,VDT_EMAIL2 from HRACE.T_VENDOR_DETAILS  where VDT_VENDOR_CODE='" + vVencode + "' and VDT_COMPANY_CODE='" + comp_cd + "'";
        str_vendor_emails = "SELECT nvl(vdt_email1, '') mail, vdt_vendor_name vendorname FROM HRACE.t_vendor_details WHERE vdt_vendor_code = '" + vVencode + "' AND vdt_company_code = '" + comp_cd + "' UNION SELECT nvl(cvo_email, '') mail, vdt_vendor_name vendorname FROM t_cwm_vendor_owners, t_vendor_details WHERE cvo_vendor_code = '" + vVencode + "' AND cvo_comp_code = '" + comp_cd + "' AND cvo_comp_code = vdt_company_code AND cvo_vendor_code = vdt_vendor_code";
        str_ctm_code_det = "select ctm_type_code, substr(ctm_type_code,1,3),substr(ctm_type_code,-4) from HRACE.T_CEMP_TYPE_MASTER where CTM_TYPE='" + s_ctm_code + "' and substr(ctm_type_code,-4)='" + comp_cd + "' and CTM_STATUS='A'";
        workorder = "SELECT A.WOD_WO_NUMBER,vmd_labour_licence_no, vmd_labour_capacity,to_char(vmd_licence_expire_dt,'DD-Mon-YYYY') LabourLicenceValidity , vmd_contractor_capacity ,VMD_Vendor_Blocked,VMD_STOP_ISSUE_GP,to_char(A.WOD_WO_TO_DATE,'DD-Mon-YYYY')  as WOD_WO_TO_DATE,A.WOD_VENDOR_CODE,A.WOD_LOCATION_CODE,A.WOD_COMP_CODE from HRACE.T_WORKORDER_DETAILS A ,hrace.t_vendor_misc_details where A.WOD_VENDOR_CODE='" + vVencode + "'  AND a.wod_comp_code ='" + comp_cd + "' and A.wod_vendor_code =vmd_vendor_code and A.wod_comp_code=vmd_comp_code and  to_Date(WOD_WO_TO_DATE,'DD/MM/RRRR') > to_date(sysdate,'DD/MM/RRRR') and VMD_Vendor_Blocked ='N' and VMD_STOP_ISSUE_GP ='N' ";
        gatepass_count = "SELECT  DISTINCT COUNT(CGP.CGP_GATEPASS_NO) GATEPASS_COUNT FROM HRACE.T_CEMP_GATEPASS CGP WHERE  CGP.CGP_VENDOR_CODE='" + vVencode + "' AND CGP.CGP_COMP_CODE='" + comp_cd + "' ";
        RFID_count = "select count(t.ced_safety_pass_no) RFID_COUNT from HRACE.t_cemp_details t where t.ced_vendor_code='" + vVencode + "' and t.ced_company_code='" + comp_cd + "' and t.ced_sp_enabled='Y' and t.ced_sp_blocked='N'";

        if (!IsPostBack)
        {
            Session.Remove("Progress");
            req_detail();
            int childId = 0;
            if (int.TryParse(Request.QueryString["childId"], out childId))
            {
                ViewState["ChildId"] = childId;
            }
            saral_location_check();
        }
    }

    public void saral_location_check()
    {
        string sql = "SELECT at.ACM_TYPE FROM HRACE.t_cwm_action_mapping at where at.ACM_TYPE = 'ASSB' and at.ACM_FLAG = 'Y' AND at.ACM_COMPANY_CODE = '" + Session["Comp_code"] + "' ";

        DataTable dt = getRecord(sql, con);

        if (dt.Rows.Count > 0)
        {

        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Message", "alert('You are not authorized to access the link in the current location.'); document.location.href='frmInterface.aspx';", true);
            return;
        }
    }

    protected void btnComplete_Click(object sender, EventArgs e)
    {
        int childId = ViewState["ChildId"] != null ? (int)ViewState["ChildId"] : 0;
        if (childId > 0)
        {

            MenuMaster.MarkChildAsCompleted(childId);
            // Redirect to the next incomplete form
            var siteMaster = (MenuMaster)this.Master;
            var nextIncomplete = siteMaster.GetFirstIncompleteForm(siteMaster.GetSampleMenu());

            if (nextIncomplete != null)
            {
                Response.Redirect(string.Format("{0}?childId={1}", nextIncomplete.FormPage, nextIncomplete.ChildId));
            }
            else
            {
                // lblMsg.Text = "All steps completed!";
            }
        }
    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {

    }

    public void req_detail()
    {
        string sql = "select TO_CHAR(SPR.SRQ_CREATED_DT,'DD/MM/YYYY') SRQ_CREATED_DT, SPR.SRQ_REQ_NO, SPR.SRQ_WORK_ORDER, SRQ_LOCATION_CD, (SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_TYPE IN 'SPRT' AND CTM_VALUE IN SPR.SRQ_REQ_TYPE) SRQ_REQ_TYPE,";
        sql += "(Select CDM_DESC from HRACE.T_WORKFLOW_CODE_MASTER w where(Trim(W.CDM_CODE) = SPRS.SRS_STATUS) and trim(W.CDM_SEQ_NO) = SPRS.SRS_SUB_STATUS) STATUS,";
        sql += "(select nvl(SUM(SPRD.SRD_EMP_APV_COUNT),0) SRD_EMP_APV_COUNT from HRACE.t_sp_request_dtl SPRD where SPRD.SRD_REQ_NO=SPR.SRQ_REQ_NO) SRD_EMP_APV_COUNT";
        sql += " from HRACE.t_sp_req_status SPRS, HRACE.T_SP_REQUEST SPR";
        sql += " where(SPRS.SRS_REQ_NO = SPR.SRQ_REQ_NO) AND SPR.SRQ_VENDOR_CODE= TRIM(UPPER('" + Session["VendCode"] + "')) AND SPR.SRQ_COMPANY_CD= TRIM('" + comp_cd + "')";
        sql += " AND SPRS.SRS_STATUS='H1' AND SPRS.SRS_SUB_STATUS='5' and SPR.SRQ_CREATED_DT >= SYSDATE - 90 ";
        sql += " ORDER BY SRQ_REQ_NO DESC";

        DataTable dt = getRecord(sql, con);

        if (dt.Rows.Count > 0)
        {
            gvReq.DataSource = dt;
            gvReq.DataBind();
            Session["GridData"] = dt;
            gvReq.Visible = true;
        }
    }

    public DataTable getRecord(string sql, OracleConnection cn)
    {
        OracleCommand cmd = new OracleCommand(sql, cn);
        cmd.CommandTimeout = 100;

        if (cn.State == ConnectionState.Closed)
        {
            cn.Open();
        }

        OracleDataAdapter da = new OracleDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        if (cn.State == ConnectionState.Open)
        {
            cn.Close();
        }

        da.Dispose();
        return dt;
    }

    protected void lnk_Request_No_Click(object sender, EventArgs e)
    {
        string loc = "";
        GridViewRow gvrow = (GridViewRow)((LinkButton)sender).Parent.Parent;
        string Req_No = ((Label)gvrow.FindControl("lnk_Request_No")).Text;
        string Req_type = ((Label)gvrow.FindControl("lbl_RQ")).Text;
        Session["Req_type"] = Req_type;
        ReqClick(Req_No);
        gridDiv.Visible = false;
        authDiv.Visible = true;

        MenuMaster menuMaster = new MenuMaster();
        menuMaster.req_check();
        var nextIncomplete = menuMaster.GetFirstIncompleteForm(menuMaster.GetSampleMenu());

        if (nextIncomplete != null && nextIncomplete.ChildId != 101)
        {
            Response.Redirect(string.Format("{0}?childId={1}", nextIncomplete.FormPage, nextIncomplete.ChildId));
        }
        else
        {
            // lblMsg.Text = "All steps completed!";
        }
    }
    protected void gvReq_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvReq.PageIndex = e.NewPageIndex;
        gvReq.DataSource = Session["GridData"]; // Already stored data
        gvReq.DataBind(); // Rebind from session
    }

    protected void rbCreate_CheckedChanged(object sender, EventArgs e)
    {
        rbSearch.Checked = false;
        // Show create section logic
        gvReq.Visible = false;
        RefreshData();
    }

    protected void rbSearch_CheckedChanged(object sender, EventArgs e)
    {
        rbCreate.Checked = false;
        // Show search section logic
        gvReq.Visible = true;
        requisitionBody.Visible = false;
        RefreshData();
    }

    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        if (rbCreate.Checked)
        {
            gvReq.Visible = false;
            requisitionBody.Visible = true;
            Vendor_submit_code();
            //employeeType();
            ReqType();
            requisition_type();
            apply_remark();
            vendor_details();
        }
        else
        {
            ShowMessage("Please select for Create Fresh Request");
            return;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        if (rbSearch.Checked)
        {
            string sql = "select TO_CHAR(SPR.SRQ_CREATED_DT,'DD/MM/YYYY') SRQ_CREATED_DT, SPR.SRQ_REQ_NO, SPR.SRQ_WORK_ORDER, SRQ_LOCATION_CD, (SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_TYPE IN 'SPRT' AND CTM_VALUE IN SPR.SRQ_REQ_TYPE) SRQ_REQ_TYPE,";
            sql += "(Select CDM_DESC from HRACE.T_WORKFLOW_CODE_MASTER w where(Trim(W.CDM_CODE) = SPRS.SRS_STATUS) and trim(W.CDM_SEQ_NO) = SPRS.SRS_SUB_STATUS) STATUS,";
            sql += "(select nvl(SUM(SPRD.SRD_EMP_APV_COUNT),0) SRD_EMP_APV_COUNT from HRACE.t_sp_request_dtl SPRD where SPRD.SRD_REQ_NO=SPR.SRQ_REQ_NO) SRD_EMP_APV_COUNT";
            sql += " from HRACE.t_sp_req_status SPRS, HRACE.T_SP_REQUEST SPR";
            sql += " where(SPRS.SRS_REQ_NO = SPR.SRQ_REQ_NO) AND SPR.SRQ_VENDOR_CODE= TRIM(UPPER('" + Session["VendCode"] + "')) AND SPR.SRQ_COMPANY_CD= TRIM('" + comp_cd + "')";
            sql += " AND SPRS.SRS_STATUS='H1' AND SPRS.SRS_SUB_STATUS='5' and SPR.SRQ_CREATED_DT >= SYSDATE - 90 ";
            if (ddlSearchFilter.SelectedValue == "REQ")
            {
                sql += " AND SPR.SRQ_REQ_NO = '" + txtSearch.Text + "'";
            }
            else if (ddlSearchFilter.SelectedValue == "SPN")
            {
                sql += " AND SPR.SRQ_REQ_NO IN (SELECT CET_REQUEST_NO FROM HRACE.T_CEMP_DETAILS_TMP WHERE CET_SAFETY_PASSNO = '" + txtSearch.Text + "') ";
            }
            else if (ddlSearchFilter.SelectedValue == "ADC")
            {
                sql += " AND SPR.SRQ_REQ_NO IN (SELECT CET_REQUEST_NO FROM HRACE.T_CEMP_DETAILS_TMP WHERE CET_UNIQUE_ID_TYPE = 'ADC' AND CET_UNIQUE_ID_VALUE = '" + txtSearch.Text + "') ";
            }
            sql += " ORDER BY SRQ_REQ_NO DESC";

            DataTable dt = getRecord(sql, con);

            if (dt.Rows.Count > 0)
            {
                gvReq.DataSource = dt;
                gvReq.DataBind();
                Session["GridData"] = dt;
            }
            else
            {
                gvReq.DataSource = dt;
                gvReq.DataBind();
                Session["GridData"] = dt;
                ShowMessage("Record Not found");
                return;

            }
        }
        else
        {
            ShowMessage("Please select for Search Existing Request");
            return;
        }
    }

    public void ShowMessage(string vMgs)
    {
        string vScript = string.Format("alert('{0}');", vMgs);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", vScript, true);
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        Response.Redirect("RequestForm.aspx");
    }
    public void ReqClick(string Req_No)
    {
        string sql = "select srq_req_no, SPR.SRQ_REQ_TYPE, sprd.srd_emp_cat, SPRD.SRD_EMP_APV_COUNT, SPR.srq_dept_code, SPR.srq_company_cd, SPR.srq_location_cd, to_char(SRQ_CREATED_DT,'dd/MM/yyyy') SRQ_CREATED_DT  from HRACE.T_SP_REQUEST SPR , HRACE.t_sp_request_dtl SPRD  where spr.srq_req_no='" + Req_No + "'  and   SPRD.SRD_REQ_NO=SPR.SRQ_REQ_NO";
        DataTable dt = getRecord(sql, con);

        if (dt.Rows.Count > 0)
        {

            if (dt.Rows[0]["srq_req_no"] != DBNull.Value)
            {
                Session["requestnumber"] = "";
                Session["requestnumber"] = dt.Rows[0]["srq_req_no"].ToString();
            }

            if (dt.Rows[0]["SRQ_REQ_TYPE"] != DBNull.Value)
            {
                Session["requestType"] = dt.Rows[0]["SRQ_REQ_TYPE"].ToString();
            }

            if (dt.Rows[0]["SRQ_CREATED_DT"] != DBNull.Value)
            {
                Session["requestDate"] = dt.Rows[0]["SRQ_CREATED_DT"].ToString();
            }

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                if (dt.Rows[i]["srd_emp_cat"].ToString() == SV)
                {
                    Session["supvsr"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == WR)
                {
                    Session["worker"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == DV)
                {
                    Session["Driver"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == FM)
                {
                    Session["FM"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == VC)
                {
                    Session["VC"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                }
            }
        }
    }

    public void employeeType()
    {
        DataTable dtTable = clmClass.get_codetype("SPET", comp_cd);
        if (dtTable.Rows.Count > 0)
        {
            if (dtTable.Rows[0]["CTM_VALUE"] != DBNull.Value)
            {
                WR = dtTable.Rows[0]["CTM_VALUE"].ToString();
            }

            if (dtTable.Rows[1]["CTM_VALUE"] != DBNull.Value)
            {
                SV = dtTable.Rows[1]["CTM_VALUE"].ToString();
            }

            if (dtTable.Rows[2]["CTM_VALUE"] != DBNull.Value)
            {
                DV = dtTable.Rows[2]["CTM_VALUE"].ToString();
            }

            if (dtTable.Rows[3]["CTM_VALUE"] != DBNull.Value)
            {
                FM = dtTable.Rows[3]["CTM_VALUE"].ToString();
            }

            if (dtTable.Rows[4]["CTM_VALUE"] != DBNull.Value)
            {
                VC = dtTable.Rows[4]["CTM_VALUE"].ToString();
            }
        }
        else
        {
            Response.Redirect("CLMHome.aspx");
            return;
        }
    }

    public void ReqType()
    {
        try
        {
            DataTable dt = clmClass.get_codetype("SPRT", comp_cd);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["CTM_VALUE"] != DBNull.Value)
                {
                    SPN = dt.Rows[0]["CTM_VALUE"].ToString();
                    //SPN = dt.Rows(0).Item("CTM_TYPE_DESC")
                }

                if (dt.Rows[1]["CTM_VALUE"] != DBNull.Value)
                {
                    SPR = dt.Rows[1]["CTM_VALUE"].ToString();
                    //SPR = dt.Rows(1).Item("CTM_TYPE_DESC")
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void requisition_type()
    {
        try
        {
            string sql = "select ctm_seq,KK.CTM_TYPE_CODE,KK.CTM_TYPE_DESC,kk.CTM_VALUE,kk.ctm_remarks from hrace.t_cemp_type_master KK where CTM_TYPE = 'SPRT' AND substr(CTM_TYPE_CODE,'-4','4')= '" + comp_cd + "'  and CTM_STATUS = 'A' AND CTM_VALUE NOT IN (SELECT DISTINCT CTM_VALUE FROM HRACE.t_cemp_type_master where CTM_TYPE_CODE = 'REQTYPE' and CTM_STATUS = 'A') order by ctm_seq";
            DataTable dt = getRecord(sql, con);

            if (dt.Rows.Count > 0)
            {
                ddl_type_requisition.DataSource = dt;
                ddl_type_requisition.DataTextField = "CTM_TYPE_DESC";
                ddl_type_requisition.DataValueField = "CTM_VALUE";
                ddl_type_requisition.DataBind();
                ddl_type_requisition.Items.Insert(0, new ListItem("Select", "0"));
            }
            else
            {
                // Handle the case where no data is returned, if needed
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void Vendor_submit_code()
    {
        try
        {
            DataTable dt = clmClass.get_codetype("SPA", comp_cd);

            if (dt.Rows.Count > 0)
            {
                submit_code = dt.Rows[0]["CTM_VALUE"].ToString();
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void apply_remark()
    {
        try
        {
            string qry = clmClass.get_CodeValue("SPAR");
            DataTable dt = getRecord(qry, con);

            if (dt.Rows.Count > 0)
            {
                DropDownremark.DataSource = dt;
                DropDownremark.DataTextField = "CTM_TYPE_DESC";
                DropDownremark.DataValueField = "CTM_VALUE";
                DropDownremark.DataBind();
                DropDownremark.Items.Insert(0, new ListItem("Select", "0"));
            }
        }
        catch (Exception ex)
        {

        }
    }

    public double SP_VALUE()
    {
        double value = 0.0;
        DataTable dtvalue = clmClass.get_codetype("SPV", comp_cd);
        if (dtvalue.Rows.Count > 0 && dtvalue.Rows[0]["ctm_value"] != DBNull.Value)
        {
            value = Convert.ToDouble(dtvalue.Rows[0]["ctm_value"]);
        }
        return value;
    }

    public void RefreshData()
    {
        txt_manpower.Text = "";
        Txt_manpower_req.Text = "";
        DropDownremark.SelectedValue = "0";
        Txt_RFID_Quota.Text = "";
        txt_wo_number.Text = "";
        txt_wovalid_dt.Text = "";
        txtlabourLicence.Text = "";
        txt_activeRFID.Text = "";
        tbxDept.Text = "";
        ddl_type_requisition.SelectedValue = "0";
        ddlSearchFilter.SelectedValue = "0";
        txtSearch.Text = "";

        ddlWorkmenType.SelectedValue = "0";
        Txt_required_emp.Text = "";
        txtremarks.Text = "";
        wo.Visible = false;
    }


    protected void txt_wo_number_TextChanged(object sender, EventArgs e)
    {
        try
        {
            double value = SP_VALUE();

            //get the work order validity and labour licence of the vendor
            string sqlWorkOrder = workorder;
            sqlWorkOrder += " AND  A.WOD_WO_NUMBER = '" + txt_wo_number.Text + "'  ";
            DataTable dt = getRecord(sqlWorkOrder, con);

            string sqlgatepass = gatepass_count;
            DataTable dtCount = getRecord(sqlgatepass, con);

            string strActiveRfid = RFID_count;

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["VMD_Vendor_Blocked"].ToString() == "N" && dt.Rows[0]["VMD_STOP_ISSUE_GP"].ToString() == "N")
                {
                    wo.Visible = true;
                    txt_wovalid_dt.Text = dt.Rows[0]["WOD_WO_TO_DATE"].ToString();
                    txtlabourLicence.Text = dt.Rows[0]["LabourLicenceValidity"].ToString();


                    txt_manpower.Text = dt.Rows[0]["vmd_labour_capacity"].ToString();
                    Txt_manpower_req.Text = (Convert.ToDouble(value) * Convert.ToInt32(txt_manpower.Text)).ToString();



                    DataTable dtQuota = GetSPVendorQuota();

                    if (dtQuota.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(dtQuota.Rows[0]["SP_QUOTA_AVL"].ToString()) <= 0)
                        {
                            ShowMessage("Quota exceeded. From your side, please release the quota.");
                            ButtonApply.Enabled = false;
                            return;
                        }
                    }
                    else
                    {
                        ShowMessage("no data found in quota table!");
                        ButtonApply.Enabled = false;
                        return;
                    }

                    int SP_IN_PROG = Convert.ToInt32(dtQuota.Rows[0]["SP_IN_PROG"].ToString());
                    int SP_APPROVED = Convert.ToInt32(dtQuota.Rows[0]["SP_APPROVED"].ToString());
                    int SP_ACTIVE_SP = Convert.ToInt32(dtQuota.Rows[0]["SP_ACTIVE_SP"].ToString());
                    int SP_COM_REJ_REQ = Convert.ToInt32(dtQuota.Rows[0]["SP_COM_REJ_REQ"].ToString());
                    int SP_REJ = Convert.ToInt32(dtQuota.Rows[0]["SP_REJ"].ToString());
                    int SP_QUOTA_AVL = Convert.ToInt32(dtQuota.Rows[0]["SP_QUOTA_AVL"].ToString());

                    txt_rfid_queue.Text = ((SP_APPROVED - SP_COM_REJ_REQ - SP_REJ) + SP_IN_PROG).ToString();
                    Txt_RFID_Quota.Text = SP_QUOTA_AVL.ToString();
                    txt_activeRFID.Text = SP_ACTIVE_SP.ToString();



                    LblWoNumber.Text = txt_wo_number.Text;

                    if (Convert.ToDecimal(Txt_RFID_Quota.Text) == 0 || Convert.ToDecimal(Txt_RFID_Quota.Text) < 0)
                    {
                        Txt_RFID_Quota.Text = Txt_RFID_Quota.Text;
                        Txt_RFID_Quota.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        Txt_RFID_Quota.Text = Txt_RFID_Quota.Text;
                        Txt_RFID_Quota.ForeColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    ShowMessage("The Vendor is blocked");
                    ButtonApply.Enabled = false;
                }
            }
            else
            {
                ShowMessage("The Vendor is blocked/ labour licence has expired");
                ButtonApply.Enabled = false;
            }
        }
        catch (Exception ex)
        {
        }
    }

    protected void ButtonApply_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txt_wo_number.Text))
        {
            ShowMessage("Please Enter Work Order No.");
            return;
        }

        if (string.IsNullOrEmpty(tbxDept.Text))
        {
            ShowMessage("Please Enter Department");
            return;
        }

        if (ddl_type_requisition.SelectedValue == "0")
        {
            ShowMessage("Please select Requisition Type");
            return;
        }

        if (DropDownremark.SelectedValue == "0")
        {
            ShowMessage("Please select Applying Reason");
            return;
        }

        if (ddlWorkmenType.SelectedValue == "0")
        {
            ShowMessage("Please select Workmen Type");
            return;
        }

        if (string.IsNullOrEmpty(Txt_required_emp.Text) || Convert.ToInt64(Txt_required_emp.Text) <= 0)
        {
            ShowMessage("Please Enter Single requisition per request");
            return;
        }

        List<OracleCommand> arr_List = new List<OracleCommand>();
        string errmsg = "";
        string SP_REQ_NO = "";
        string reqst = string.Empty;

        string ls_sqlapp1 = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where ACM_TYPE='SPA' and ACM_CATEGORY='SPA' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_FLAG='Y'";

        DataTable dtapp1 = new DataTable();
        try
        {
            using (OracleConnection conec = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString))
            {
                if (conec.State == ConnectionState.Closed)
                {
                    conec.Open();
                }

                using (OracleCommand cmdapp1 = new OracleCommand(ls_sqlapp1, conec))
                {
                    cmdapp1.Parameters.Add(new OracleParameter(":ACM_COMPANY_CODE", comp_cd));
                    using (OracleDataAdapter da = new OracleDataAdapter(cmdapp1))
                    {
                        da.Fill(dtapp1);
                    }
                }
            }

            if (dtapp1.Rows.Count > 0)
            {
                if (Convert.ToInt64(Txt_required_emp.Text.ToString()) > 1)
                {
                    ShowMessage("Only Single requisition allowed per request");
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error checking company code: " + ex.Message);
            return;
        }

        try
        {
            double value = SP_VALUE();

            string sqlWorkOrder = workorder;
            sqlWorkOrder += " AND  A.WOD_WO_NUMBER = '" + txt_wo_number.Text + "'  ";
            DataTable dt = getRecord(sqlWorkOrder, con);

            string sqlgatepass = gatepass_count;
            DataTable dtCount = getRecord(sqlgatepass, con);

            string strActiveRfid = RFID_count;

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["VMD_Vendor_Blocked"].ToString() == "N" && dt.Rows[0]["VMD_STOP_ISSUE_GP"].ToString() == "N")
                {
                    wo.Visible = true;
                    txt_wovalid_dt.Text = dt.Rows[0]["WOD_WO_TO_DATE"].ToString();
                    txtlabourLicence.Text = dt.Rows[0]["LabourLicenceValidity"].ToString();
                    txt_manpower.Text = dt.Rows[0]["vmd_labour_capacity"].ToString();
                    Txt_manpower_req.Text = (Convert.ToDouble(value) * Convert.ToInt32(txt_manpower.Text)).ToString();

                    DataTable dtQuota1 = GetSPVendorQuota();

                    if (dtQuota1.Rows.Count > 0)
                    {
                        if (Convert.ToDecimal(dtQuota1.Rows[0]["SP_QUOTA_AVL"].ToString()) <= 0)
                        {
                            ShowMessage("Quota exceeded. From your side, please release the quota.");
                            ButtonApply.Enabled = false;
                            return;
                        }
                    }
                    else
                    {
                        ShowMessage("no data found in quota table!");
                        ButtonApply.Enabled = false;
                        return;
                    }

                    int SP_IN_PROG = Convert.ToInt32(dtQuota1.Rows[0]["SP_IN_PROG"].ToString());
                    int SP_APPROVED = Convert.ToInt32(dtQuota1.Rows[0]["SP_APPROVED"].ToString());
                    int SP_ACTIVE_SP = Convert.ToInt32(dtQuota1.Rows[0]["SP_ACTIVE_SP"].ToString());
                    int SP_COM_REJ_REQ = Convert.ToInt32(dtQuota1.Rows[0]["SP_COM_REJ_REQ"].ToString());
                    int SP_REJ = Convert.ToInt32(dtQuota1.Rows[0]["SP_REJ"].ToString());
                    int SP_QUOTA_AVL = Convert.ToInt32(dtQuota1.Rows[0]["SP_QUOTA_AVL"].ToString());

                    txt_rfid_queue.Text = (((SP_APPROVED - SP_COM_REJ_REQ - SP_REJ) + SP_IN_PROG)).ToString();
                    Txt_RFID_Quota.Text = SP_QUOTA_AVL.ToString();
                    txt_activeRFID.Text = SP_ACTIVE_SP.ToString();


                    LblWoNumber.Text = txt_wo_number.Text;

                    if (Txt_RFID_Quota.Text == "0" || Convert.ToDecimal(Txt_RFID_Quota.Text) < 0)
                    {
                        Txt_RFID_Quota.Text = Txt_RFID_Quota.Text;
                        Txt_RFID_Quota.ForeColor = System.Drawing.Color.Red;
                    }
                    else
                    {
                        Txt_RFID_Quota.Text = Txt_RFID_Quota.Text;
                        Txt_RFID_Quota.ForeColor = System.Drawing.Color.Green;
                    }
                }
                else
                {
                    ShowMessage("The Vendor is blocked");
                    ButtonApply.Enabled = false;
                    return;
                }
            }
            else
            {
                ShowMessage("The Vendor is blocked/ labour licence has expired/ Work Order Validity Not Found.");
                ButtonApply.Enabled = false;
                return;
            }
        }
        catch (Exception ex)
        {
            return;
        }

        DataTable dtQuota = GetSPVendorQuota();

        if (dtQuota.Rows.Count > 0)
        {
            if (Convert.ToDecimal(dtQuota.Rows[0]["SP_QUOTA_AVL"].ToString()) < Convert.ToDecimal(Txt_required_emp.Text))
            {
                ShowMessage("The number of required employees (" + Txt_required_emp.Text + ") cannot exceed the number of quotas " + dtQuota.Rows[0]["SP_QUOTA_AVL"] + " remaining.");
                return;
            }
        }
        else
        {
            ShowMessage("no data found in quota table!");
            return;
        }


        string insert_T_SP_REQUEST_DTL = " INSERT INTO HRACE.T_SP_REQUEST_DTL (SRD_REQ_NO , SRD_EMP_CAT, SRD_EMP_COUNT, SRD_EMP_APV_COUNT, SRD_REMARKS, SRD_CREATED_DT, SRD_CREATED_BY, SRD_FLAG, SRD_MODIFIED_BY, SRD_MODIFIED_DATE) ";     // changed SRD_REMARKS1 from SRD_REMARKS (29/01/2016)
        string insert_T_SP_REQ_APPROVER = "  INSERT INTO HRACE.T_SP_REQ_APPROVER (SRA_REQ_NO,SRA_AGENT_PERNO,SRA_CREATED_BY,SRA_CREATED_ON,SRA_REMARKS) ";

        errmsg = validate_onApply();
        if (errmsg != "")
        {
            ShowMessage(errmsg);
        }
        else
        {
            string req_type = ddl_type_requisition.SelectedValue.ToString();
            string active_rfid = txt_activeRFID.Text;
            string work_order = txt_wo_number.Text;
            int total_employee = Convert.ToInt32(Txt_required_emp.Text.Trim());
            string depart = tbxDept.Text.Split('-')[0];
            string location = Session["Location"] as string;
            string remark = DropDownremark.SelectedItem.ToString();
            string ls_deparchk = string.Empty;

            try
            {
                ls_deparchk = "Select CDP_DEPT_NAME from t_cnt_dept_master where CDP_DEPT_CODE=:CDP_DEPT_CODE and CDP_COMP_CODE=:CDP_COMP_CODE";

                using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    using (OracleCommand cmddepartchk = new OracleCommand(ls_deparchk, conn))
                    {
                        cmddepartchk.Parameters.Add(new OracleParameter(":CDP_DEPT_CODE", depart.Trim()));
                        cmddepartchk.Parameters.Add(new OracleParameter(":CDP_COMP_CODE", comp_cd));

                        using (OracleDataReader dtdepartchk = cmddepartchk.ExecuteReader())
                        {
                            if (dtdepartchk.HasRows)
                            {
                            }
                            else
                            {
                                ShowMessage("Please choose correct department");
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error checking department: " + ex.Message);
                return;
            }

            SP_REQ_NO = GET_SP_Request_no();

            string T_SP_REQUEST = "";

            T_SP_REQUEST = "  INSERT INTO HRACE.T_SP_REQUEST  (SRQ_REQ_NO,SRQ_VENDOR_CODE,SRQ_REQ_TYPE,SRQ_TOT_ACTIVE_SPCOUNT,SRQ_WORK_ORDER,SRQ_DEPT_CODE,SRQ_CREATED_DT,SRQ_CREATED_BY,SRQ_MODIFIED_DT,SRQ_MODIFIED_BY,SRQ_COMPANY_CD,SRQ_LOCATION_CD) ";
            T_SP_REQUEST += "VALUES('" + SP_REQ_NO + "','" + vVencode + "','" + req_type + "','" + active_rfid + "','" + work_order + "','" + depart + "',sysdate,'" + vVend_UserID + "','','','" + comp_cd + "','" + location + "')";

            OracleCommand cmd_T_SP_REQUEST = new OracleCommand(T_SP_REQUEST, con);
            arr_List.Add(cmd_T_SP_REQUEST);

            string T_SP_REQUEST_QUOTA = "";
            T_SP_REQUEST_QUOTA = "UPDATE HRACE.T_SP_QUOTA_VEND SET SP_QUOTA_AVL = SP_QUOTA_AVL - " + total_employee.ToString() + ", SP_APPROVED = SP_APPROVED + " + total_employee.ToString() + ", SP_MODIFIED_BY = '" + vVend_UserID + "', SP_MODIFIED_DT = SYSDATE  WHERE SP_VEND_CODE = '" + vVencode + "' AND SP_COMP_CODE = '" + comp_cd + "'";
            OracleCommand cmd_T_SP_REQUEST_QUOTA = new OracleCommand(T_SP_REQUEST_QUOTA, con);
            arr_List.Add(cmd_T_SP_REQUEST_QUOTA);

            if (ddlWorkmenType.SelectedValue == "WKR")
            {
                string T_SP_REQUEST_DTL = "";
                T_SP_REQUEST_DTL = insert_T_SP_REQUEST_DTL;
                T_SP_REQUEST_DTL += "VALUES('" + SP_REQ_NO + "','" + WR + "','" + Txt_required_emp.Text + "','" + Txt_required_emp.Text + "','" + txtremarks.Text + "',sysdate,'" + vVend_UserID + "','Y','','')";
                OracleCommand cmd_T_SP_REQ_DTL_W = new OracleCommand(T_SP_REQUEST_DTL, con);
                arr_List.Add(cmd_T_SP_REQ_DTL_W);
            }

            else if (ddlWorkmenType.SelectedValue == "DRV")
            {
                string T_SP_REQUEST_DTL = "";
                T_SP_REQUEST_DTL = insert_T_SP_REQUEST_DTL;
                T_SP_REQUEST_DTL += "VALUES('" + SP_REQ_NO + "','" + DV + "','" + Txt_required_emp.Text + "','" + Txt_required_emp.Text + "','" + txtremarks.Text + "',sysdate,'" + vVend_UserID + "','Y','','')";
                OracleCommand cmd_T_SP_REQ_DTL_D = new OracleCommand(T_SP_REQUEST_DTL, con);
                arr_List.Add(cmd_T_SP_REQ_DTL_D);
            }

            else if (ddlWorkmenType.SelectedValue == "SPV")
            {
                string T_SP_REQUEST_DTL = "";
                T_SP_REQUEST_DTL = insert_T_SP_REQUEST_DTL;
                T_SP_REQUEST_DTL += "VALUES('" + SP_REQ_NO + "','" + SV + "','" + Txt_required_emp.Text + "','" + Txt_required_emp.Text + "','" + txtremarks.Text + "',sysdate,'" + vVend_UserID + "','Y','','')";
                OracleCommand cmd_T_SP_REQ_DTL_S = new OracleCommand(T_SP_REQUEST_DTL, con);
                arr_List.Add(cmd_T_SP_REQ_DTL_S);
            }

            else if (ddlWorkmenType.SelectedValue == "FMG")
            {
                string T_SP_REQUEST_DTL = "";
                T_SP_REQUEST_DTL = insert_T_SP_REQUEST_DTL;
                T_SP_REQUEST_DTL += "VALUES('" + SP_REQ_NO + "','" + FM + "','" + Txt_required_emp.Text + "','" + Txt_required_emp.Text + "','" + txtremarks.Text + "',sysdate,'" + vVend_UserID + "','Y','','')";
                OracleCommand cmd_T_SP_REQ_DTL_FM = new OracleCommand(T_SP_REQUEST_DTL, con);
                arr_List.Add(cmd_T_SP_REQ_DTL_FM);
            }

            else if (ddlWorkmenType.SelectedValue == "VDC")
            {
                string T_SP_REQUEST_DTL = "";
                T_SP_REQUEST_DTL = insert_T_SP_REQUEST_DTL;
                T_SP_REQUEST_DTL += "VALUES('" + SP_REQ_NO + "','" + VC + "','" + Txt_required_emp.Text + "','" + Txt_required_emp.Text + "','" + txtremarks.Text + "',sysdate,'" + vVend_UserID + "','Y','','')";
                OracleCommand cmd_T_SP_REQ_DTL_VC = new OracleCommand(T_SP_REQUEST_DTL, con);
                arr_List.Add(cmd_T_SP_REQ_DTL_VC);
            }

            string T_SP_REQ_STATUS = "";
            T_SP_REQ_STATUS = "INSERT INTO T_SP_REQ_STATUS (SRS_REQ_NO,SRS_AGENT,SRS_STATUS,SRS_SUB_STATUS,SRS_AGENT_REMARK,SRS_AGENT_TYP,SRS_CREATED_DT,SRS_CREATED_BY,SRS_MODIFIED_DT,SRS_MODIFIED_BY) ";
            T_SP_REQ_STATUS += "VALUES('" + SP_REQ_NO + "','" + vVencode + "','V','1','" + remark + "','" + submit_code + "',sysdate,'" + vVend_UserID + "','','')";

            OracleCommand cmd_T_SP_REQ_STATUS = new OracleCommand(T_SP_REQ_STATUS, con);
            arr_List.Add(cmd_T_SP_REQ_STATUS);
            string ls_sqlapp = "select ACM_COMPANY_CODE from hrace.t_cwm_action_mapping where ACM_TYPE='SPA' and ACM_CATEGORY='SPA' and ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_FLAG='Y'";
            OracleCommand cmdapp = new OracleCommand();
            DataTable dtapp = new DataTable();
            try
            {
                using (OracleConnection conng = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString))
                {
                    if (conng.State == ConnectionState.Closed)
                    {
                        conng.Open();
                    }

                    cmdapp = new OracleCommand(ls_sqlapp, conng);
                    cmdapp.Parameters.Add(new OracleParameter(":ACM_COMPANY_CODE", Session["Comp_Code"]));
                    using (OracleDataAdapter da = new OracleDataAdapter(cmdapp))
                    {
                        da.Fill(dtapp);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error checking approval mapping: " + ex.Message);
                return;
            }

            if (dtapp.Rows.Count > 0)
            {
                T_SP_REQ_STATUS = "INSERT INTO T_SP_REQ_STATUS (SRS_REQ_NO,SRS_AGENT,SRS_STATUS,SRS_SUB_STATUS,SRS_AGENT_REMARK,SRS_AGENT_TYP,SRS_CREATED_DT,SRS_CREATED_BY,SRS_MODIFIED_DT,SRS_MODIFIED_BY) ";
                T_SP_REQ_STATUS += "VALUES('" + SP_REQ_NO + "','" + vVencode + "','H1','5','" + remark + "','HR',sysdate,'SYSTEM','','')";

                OracleCommand cmd_T_SP_REQ_STATUS1 = new OracleCommand(T_SP_REQ_STATUS, con);
                arr_List.Add(cmd_T_SP_REQ_STATUS1);
                reqst = "Y";
            }
            else
            {
                reqst = "N";
            }

            if (arr_List.Count > 0)
            {
                int counter = 0;
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                OracleTransaction tran_Ins = con.BeginTransaction();
                try
                {
                    for (counter = 0; counter < arr_List.Count; counter++)
                    {
                        OracleCommand con_ins = arr_List[counter];
                        con_ins.Connection = con;
                        con_ins.Transaction = tran_Ins;
                        con_ins.ExecuteNonQuery();
                    }
                    tran_Ins.Commit();

                    if (reqst == "N")
                    {
                        string Req_No = SP_REQ_NO;
                        string Req_type = req_type;
                        Session["Req_type"] = ddl_type_requisition.SelectedItem.ToString();
                        ReqClick(Req_No);
                        gridDiv.Visible = false;
                        authDiv.Visible = true;
                        ShowMessage(" Your Request is Generated.REQUEST NUMBER: " + SP_REQ_NO);
                    }
                    else if (reqst == "Y")
                    {
                        //ShowMessage(" Your Request is Generated And Approved. REQUEST NUMBER: " + SP_REQ_NO);

                        string Req_No = SP_REQ_NO;
                        string Req_type = req_type;
                        Session["Req_type"] = ddl_type_requisition.SelectedItem.ToString();
                        ReqClick(Req_No);
                        gridDiv.Visible = false;
                        authDiv.Visible = true;
                        ShowMessage(" Your Request is Generated And Approved. REQUEST NUMBER: " + SP_REQ_NO);
                    }

                    wo.Visible = false;
                    requisitionBody.Visible = false;
                }
                catch (Exception ex)
                {
                    tran_Ins.Rollback();
                }
                finally
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }

            RefreshData();
        }
    }

    public string validate_onApply()
    {
        string errmsg = "";

        if (string.IsNullOrEmpty(txt_wo_number.Text))
        {
            errmsg = "Work Order is Mandatory !!!";
        }
        else if (string.IsNullOrEmpty(tbxDept.Text))
        {
            errmsg = "please select Department ";
        }
        else if (ddl_type_requisition.SelectedValue == "0")
        {
            errmsg = "select the type of requisition ";
        }
        else if (DropDownremark.SelectedValue == "0")
        {
            errmsg = "Application remark field cannot be left blank ";
        }
        else if (Txt_required_emp.Text == "0")
        {
            errmsg = "Total Required Employees cannot be ZERO in Number !!";
        }
        else if (Convert.ToInt32(Txt_RFID_Quota.Text) == 0 || Convert.ToInt32(Txt_RFID_Quota.Text) < 0)
        {
            errmsg = "You cannot apply for safety training certificate as no Quota available.";
        }

        return errmsg;
    }

    public string GET_SP_Request_no()
    {
        string SP_Request_no = "";
        using (OracleConnection congatepass = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString))
        {
            using (OracleCommand cmd3 = new OracleCommand("SELECT HRACE_SP_REQ_SEQ.nextval FROM dual", congatepass))
            {
                if (congatepass.State == ConnectionState.Closed)
                {
                    congatepass.Open();
                }

                using (OracleDataReader drs2 = cmd3.ExecuteReader())
                {
                    if (drs2.Read())
                    {
                        SP_Request_no = DateTime.Now.ToString("yyMM") + drs2[0].ToString();
                    }
                }
            }
        }
        return SP_Request_no;
    }

    public void vendor_details()
    {
        try
        {
            using (OracleConnection congatepass = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString))
            {
                if (congatepass.State == ConnectionState.Closed)
                {
                    congatepass.Open();
                }

                using (OracleCommand user_comm = new OracleCommand(str_vendor, congatepass))
                {
                    using (OracleDataReader dr = user_comm.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Session["vendorName"] = dr["VDT_VENDOR_NAME"].ToString();
                            Session["Location"] = dr["VDT_LOCATION_CODE"].ToString();
                        }
                        else
                        {
                            Response.Redirect("http://tatasteel.co.in/");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }


    private DataTable GetSPVendorQuota()
    {
        string strQry = "SELECT  NVL(SP_LAB_CAP,0) SP_LAB_CAP, NVL(SP_IN_PROG,0) SP_IN_PROG, NVL(SP_APPROVED,0) SP_APPROVED," +
                        "NVL(SP_INACTIVE_RFID,0) SP_INACTIVE_RFID, NVL(SP_ACTIVE_SP,0) SP_ACTIVE_SP, " +
                        "NVL(SP_COM_REJ_REQ,0) SP_COM_REJ_REQ,  NVL(SP_REJ,0) SP_REJ, NVL(SP_QUOTA_AVL,0) SP_QUOTA_AVL " +
                        "FROM HRACE.T_SP_QUOTA_VEND " +
                        "where SP_VEND_CODE = :SP_VEND_CODE " +
                        "AND SP_COMP_CODE = :SP_COMP_CODE";

        DataTable dt = new DataTable();
        try
        {
            using (OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString))
            {
                using (OracleCommand cmdQuota = new OracleCommand(strQry, con))
                {
                    cmdQuota.Parameters.Add(new OracleParameter(":SP_VEND_CODE", vVencode));
                    cmdQuota.Parameters.Add(new OracleParameter(":SP_COMP_CODE", comp_cd));

                    using (OracleDataAdapter da = new OracleDataAdapter(cmdQuota))
                    {
                        da.Fill(dt);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exception appropriately (logging, etc.)
            Console.Error.WriteLine("Error in GetSPVendorQuota: " + ex.Message);
        }
        return dt;
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod]
    public static string[] GetWorkorderList(string prefixText, int count)
    {
        frmPreRequisiteCheck clm = new frmPreRequisiteCheck();
        string vCompCode = clm.Session["Comp_code"] as string;
        string vendCode = clm.Session["VendCode"] as string;
        string SqlWorkOrder = "";

        string strSql = "SELECT CTM_VALUE FROM hrace.T_CEMP_TYPE_MASTER WHERE  substr(CTM_TYPE_CODE,'-4','4')='" + vCompCode + "' and CTM_TYPE='RQ' ";
        DataTable dtWorkOrder1 = clm.getRecord(strSql, clm.con);
        string ValidityDays = "";
        if (dtWorkOrder1.Rows.Count > 0)
        {
            ValidityDays = dtWorkOrder1.Rows[0]["ctm_value"].ToString();
        }

        SqlWorkOrder = "SELECT * FROM( select distinct wod_wo_number from HRACE.t_workorder_details WHERE  wod_wo_number LIKE '" + prefixText.ToUpper() + "%'  and wod_comp_code = '" + vCompCode + "'  and (wod_wo_to_date)> trunc(sysdate + " + ValidityDays + " ) and WOD_VENDOR_CODE ='" + vendCode + "' ) WHERE ROWNUM <= 10 ORDER BY wod_wo_number";

        DataTable dtWorkOrder = clm.getRecord(SqlWorkOrder, clm.con);

        string[] items = new string[dtWorkOrder.Rows.Count];
        for (int i = 0; i < dtWorkOrder.Rows.Count; i++)
        {
            items[i] = dtWorkOrder.Rows[i][0].ToString();
        }
        return items.Where(m => m.ToUpper().Contains(prefixText.ToUpper())).ToArray();
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod]
    public static string[] GetDeptList(string prefixText, string count)
    {
        frmPreRequisiteCheck clm = new frmPreRequisiteCheck();
        string SqlDept = "";
        string vCompCode = clm.Session["Comp_code"] as string;
        string vendCode = clm.Session["VendCode"] as string;

        SqlDept = "SELECT * FROM( select distinct cdp_dept_code || '-' || cdp_dept_name,cdp_dept_code from HRACE.t_cnt_dept_master ";
        SqlDept += " WHERE  ( cdp_dept_code LIKE '" + prefixText.ToUpper() + "%' or cdp_dept_name  LIKE '" + prefixText.ToUpper() + "%') ";
        SqlDept += " and cdp_comp_code = '" + vCompCode + "') WHERE ROWNUM <= 15 ORDER BY cdp_dept_code";

        DataTable dtDeptList = clm.getRecord(SqlDept, clm.con);

        string[] items = new string[dtDeptList.Rows.Count];
        for (int i = 0; i < dtDeptList.Rows.Count; i++)
        {
            items[i] = dtDeptList.Rows[i][0].ToString();
        }
        return items.Where(m => m.ToUpper().Contains(prefixText.ToUpper())).ToArray();
    }
}
