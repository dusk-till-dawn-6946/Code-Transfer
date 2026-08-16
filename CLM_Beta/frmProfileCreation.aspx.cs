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
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf.draw;
using System.Net.Mail;
using System.Web.Services;


public partial class frmProfileCreation : System.Web.UI.Page
{

    OracleConnection con = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString);
    OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString);
    private string strConn = ConfigurationManager.ConnectionStrings["OraConnGatepass"].ConnectionString;
    string comp_cd = "";
    string vVencode = "";
    string WR = "";
    string SV = "";
    string VC = "";
    string DV = "";
    string FM = "";
    string SH = "";
    string SF = "";

    string WA = "";
    string SA = "";
    string VA = "";
    string DA = "";
    string FA = "";
    string DH = "";

    string WR_desc = "";
    string SV_desc = "";
    string VC_desc = "";
    string DV_desc = "";
    string FM_desc = "";
    string SH_desc = "";
    string SF_desc = "";
    string DH_desc = "";
    string WA_desc = "";
    string SA_desc = "";
    string VA_desc = "";
    string DA_desc = "";
    string FA_desc = "";

    string Loc = "";

    string SPN = "";
    string SPR = "";
    string WFHN = "";


    short err_cnt = 0;

    string vLocCd = "";

    CLMVendClass clmClass = new CLMVendClass();
    public const string ENCRYPT_DECRYPT_KEY = "1L0tu+LQ1ux$c@P9";

    string msg_incomp = "";
    string msg_complete = "";
    string msg_reject = "";
    string msg_incomp_val = "";
    string msg_complete_val = "";
    string msg_reject_val = "";
    string locationCode = "";
    string location = "";
    string vendorCode = "";
    string category = "";
    string dept = "";
    string firstname = "";
    string lastname = "";
    string fatherName = "";
    string spouse = "";
    string gender = "";
    string emergencyNo = "";
    string phoneNo = "";
    string bloodGroup = "";
    string uniqueIDVal = "";
    string uniqueIDType = "";
    string identityMark = "";
    string areaofWork = "";
    string birthAge = "";
    string dob = "";
    string FullAddress = "";
    string address1 = "";
    string address2 = "";
    string address3 = "";
    string country = "";
    string country_name = "";
    string qualification = "";
    string profile_status = "";
    string verify_status = "";
    string dobcertno = "";
    string drvcertno = "";
    string passcertno = "";
    string affirmative = "";
    string UAN = "";
    string IP = "";
    bool TradeIrisDataPresent = false;


    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["Comp_code"].ToString() == "9501" || Session["Comp_code"].ToString() == "9502" || Session["Comp_code"].ToString() == "9500")
        {
            Session["comp_name_d"] = "Jamipol";
        }
        else
        {
            Session["comp_name_d"] = "Tata Steel";
        }

        string reqNo = Session["requestnumber"].ToString();

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
            ReqType();
            vLocCd = GetLocationName(comp_cd);
        }
        else
        {
            Response.Redirect("frmInterface.aspx");
            return;
        }

        if (!IsPostBack)
        {
            int childId = 0;
            if (int.TryParse(Request.QueryString["childId"], out childId))
            {
                ViewState["ChildId"] = childId;
            }

            if (!string.IsNullOrEmpty(reqNo))
            {
                ReqClick(reqNo);
                reqNo = "";
            }

            string rtn_str = getSPNO(Session["requestnumber"].ToString());
            if (rtn_str != "NA")
            {
                Session["spno"] = rtn_str;
                profile_details(rtn_str);
                GetAddress(rtn_str);
                getagedrv(rtn_str);
                showpv(rtn_str);
                actionButtonenable(rtn_str);
            }

            GetAreaOfWork(cmbWorkArea, "AOW");
            FillDropDown(cmbAffirmative, "AFRM");
            FillDropDown(cmbUniqID, "ICAD");

            GetAddressType();
            GetCountry();
            cmbAddCountry.SelectedValue = "IND";
            GetState();
            cmbAddState.SelectedValue = "JH";
            GetCity(cmbAddState.SelectedValue);
            GetDistrict(cmbAddState.SelectedValue);

            if (Session["requestType"].ToString() == "SPR")
            {
                Session["reqtype"] = "Renew";
            }
            else
            {
                Session["reqtype"] = "New";
            }
        }
    }

    public void actionButtonenable(string vSPNo)
    {
        string ls_sql = "select distinct CET_REQUEST_NO,CET_PROFILE_STATUS,CET_DOCVER_STATUS from hrace.t_cemp_details_tmp where CET_REQUEST_NO = :ReqNo and CET_SAFETY_PASSNO = :SpNo and CET_LOCATION_CODE = :comp_cd and nvl(CET_DOCVER_STATUS,'NA') = 'C' ";
        OracleCommand cmd = new OracleCommand(ls_sql, con);
        cmd.Parameters.Add(new OracleParameter(":ReqNo", Session["requestnumber"]));
        cmd.Parameters.Add(new OracleParameter(":SpNo", vSPNo));
        cmd.Parameters.Add(new OracleParameter(":comp_cd", comp_cd));
        DataTable dt = getRecord(cmd, con);

        if (dt.Rows.Count > 0)
        {
            actionDivID.Visible = false;
        }
    }

    protected void btnComplete_Click(object sender, EventArgs e)
    {
        //Address section start

        string sqlAddress = "";
        string vAddressID = "";
        string vSPNo = TxtSpno.Text.Trim().ToUpper();
        string filename = string.Empty;

        if (vSPNo == "")
        {
            ShowMessage("Address could not be saved as specifc employee safety pass number was not found.click on the safety pass number to add details.");
            return;
        }
        if (txtAddMobile.Text.Trim().Equals(""))
        {
            ShowMessage("Please enter mobile number of vendor");
            return;
        }
        if (txtAddEmail.Text.Trim().Equals(""))
        {
            ShowMessage("Please enter email id of vendor");
            return;
        }

        if (!fupdl_add.HasFile && !ChkoldAddress.Checked)
        {
            ShowMessage("Please Upload File");
            return;
        }
        else if (fupdl_add.HasFile && ChkoldAddress.Checked)
        {
            ShowMessage("choose either file upload or check previous upload documents option");
            return;
        }
        else if (fupdl_add.HasFile && !ChkoldAddress.Checked)
        {
            filename = Path.GetFileName(fupdl_add.PostedFile.FileName);
            string contentType = fupdl_add.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf"))
            {
                if (fupdl_add.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (fupdl_add.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        string vErrorCount = "";
        vErrorCount = CheckAddressMandatoryFields();

        if (vErrorCount != "NA")
        {
            ShowMessage(vErrorCount);
            return;
        }
        else
        {

        }

        string sql = emp_addrs_detail_qry(vSPNo) + "and CCA_ADDR_TYPE='" + cmbAddressType.SelectedValue + "'";
        DataTable dt1 = getRecord(sql, conn);
        if (dt1.Rows.Count > 0)
        {
            ShowMessage("The type of address is already saved");
            return;
        }

        vAddressID = GetID("seq_cemp_address");

        sqlAddress = "INSERT INTO  HRACE.T_CWM_CEMP_ADDRS_TMP ( CCA_ADDRESS_ID,CCA_REQ_NO, CCA_COMP_CD, CCA_SAFETY_PASS_NO,CCA_WORKMEN_TYPE, CCA_ADDR_TYPE,  CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_VILLAGE, CCA_PO, CCA_THANA, CCA_DISTRICT_CD, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE,CCA_START_DT,CCA_END_DT,CCA_CREATED_BY, CCA_CREATED_DT,CCA_REMARKS,CCA_CERT_NO ) VALUES('";
        sqlAddress = sqlAddress + vAddressID + "','";
        sqlAddress = sqlAddress + Session["requestnumber"] + "','";
        sqlAddress = sqlAddress + comp_cd + "','";
        sqlAddress = sqlAddress + vSPNo + "','";
        sqlAddress = sqlAddress + cmbCategory.SelectedValue.ToString().ToUpper() + "','";
        sqlAddress = sqlAddress + cmbAddressType.SelectedValue + "','";
        sqlAddress = sqlAddress + txtAddName.Text.ToString().Trim().ToUpper() + "','";
        sqlAddress = sqlAddress + txtAddHouseNo.Text.ToString().Trim().ToUpper() + "','";
        sqlAddress = sqlAddress + txtAddStreet.Text.ToString().Trim().ToUpper() + "','";
        sqlAddress = sqlAddress + cmbAddCity.SelectedValue + "','";
        sqlAddress = sqlAddress + txtAddVillage.Text.ToString().Trim().ToUpper() + "','";
        sqlAddress = sqlAddress + txtAddPO.Text.ToString().Trim().ToUpper() + "','";
        sqlAddress = sqlAddress + txtAddThana.Text.ToString().Trim().ToUpper() + "','";
        sqlAddress = sqlAddress + cmbAddDistrict.SelectedValue + "','";
        sqlAddress = sqlAddress + cmbAddState.SelectedValue + "','";
        sqlAddress = sqlAddress + cmbAddCountry.SelectedValue + "','";
        sqlAddress = sqlAddress + txtAddPIN.Text.ToString().Trim() + "','";
        sqlAddress = sqlAddress + txtAddMobile.Text.ToString().Trim() + "','";
        sqlAddress = sqlAddress + txtAddEmail.Text.ToString().Trim() + "','";
        sqlAddress = sqlAddress + txtLandLine.Text.ToString().Trim() + "',";

        sqlAddress = sqlAddress + "to_date(to_char(sysdate,'DD/MM/YYYY'),'DD/MM/YYYY')" + ",";
        sqlAddress = sqlAddress + "to_date('31/12/9999','DD/MM/YYYY')" + ",'";
        sqlAddress = sqlAddress + Session["VendCode"] + "',";
        sqlAddress = sqlAddress + "SYSDATE" + ",";
        if (ChkoldAddress.Checked)
        {
            sqlAddress = sqlAddress + "'O'" + ",'";
            sqlAddress = sqlAddress + hddaddressold.Value + "')";
        }
        else
        {
            sqlAddress = sqlAddress + "'N'" + ",'";
            sqlAddress = sqlAddress + vAddressID + "')";
        }

        try
        {
            if (!ChkoldAddress.Checked)
            {
                if (fupdl_add.HasFile)
                {
                    OracleCommand cmdfileadd = new OracleCommand();
                    string ls_sql1 = string.Empty;
                    filename = Path.GetFileName(fupdl_add.PostedFile.FileName);
                    using (Stream fs = fupdl_add.PostedFile.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            byte[] bytes = br.ReadBytes((int)fs.Length);

                            ls_sql1 = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                            if (con.State == ConnectionState.Closed)
                            {
                                con.Open();
                            }
                            cmdfileadd.CommandText = ls_sql1;
                            cmdfileadd.Connection = con;
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_DOC_ID", vAddressID));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "ADD"));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileadd.ExecuteNonQuery();
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
            SaveData(sqlAddress, con);

            //BtnNext.Visible = true;
            //ShowMessage("Address Saved Sucessfully");
            //ShowMessage("Request Submited Successfully");
            clearAddress();
            GetAddress(vSPNo);
            if (Session["reqtype"].ToString() == "Renew")
            {
                foreach (GridViewRow gvrow in gvAddress.Rows)
                {
                    CheckBox chkbox = (CheckBox)gvrow.FindControl("chkSelectAddress");
                    HiddenField reqno = (HiddenField)gvrow.FindControl("hdreqno");
                    if (reqno.Value.Trim() == Session["requestnumber"].ToString())
                    {
                        chkbox.Enabled = true;
                    }
                    else
                    {
                        chkbox.Enabled = false;
                    }
                }
            }
            //empView();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.ToString());
        }
        //Address section end    

        //PV section start

        string ls_sql_pv = string.Empty;
        OracleCommand cmd_pv;
        DataTable dt_pv = new DataTable();
        string filename_pv = string.Empty;
        string pvid = string.Empty;
        string pvcertid = string.Empty;

        string certno = string.Empty;

        if (txt_frmdt.Text.Trim() == "")
        {
            ShowMessage("Please enter valid from date");
            return;
        }
        if (!updl_file.HasFile)
        {
            ShowMessage("Please provide police verification attachment");
            return;
        }
        else
        {
            filename_pv = Path.GetFileName(updl_file.PostedFile.FileName);
            string contentType = updl_file.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf"))
            {
                if (updl_file.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (updl_file.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        try
        {
            getbasicdetails(Session["spno"].ToString());
            futureDate(txt_frmdt.Text);

            string db = txt_frmdt.Text.Replace("-", "/");
            DateTime originalDate = DateTime.ParseExact(db, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            db = originalDate.ToString("dd/MM/yyyy").Replace("-", "/"); ;
            txt_frmdt.Text = db;

            string db1 = txt_todt.Text.Replace("-", "/");
            DateTime originalDate1 = DateTime.ParseExact(db1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            db1 = originalDate1.ToString("dd/MM/yyyy").Replace("-", "/"); ;
            txt_todt.Text = db1;

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            pvid = TrnCWEPVSeqNo("");
            pvcertid = TrnCWEPVCertNo("");

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            ls_sql_pv = "insert into T_CWM_PV_DTL_TMP(CPDT_PV_ID,CPDT_SAFETY_PASS_NO,CPDT_COMP_CODE,CPDT_ST_DT,CPDT_END_DT,CPDT_CERT_NO,CPDT_CREATED_BY,CPDT_CREATED_DT,CPDT_DOC_TYPE) values(:CPDT_PV_ID,:CPDT_SAFETY_PASS_NO,:CPDT_COMP_CODE,TO_DATE(:CPDT_ST_DT,'DD/MM/YYYY'),TO_DATE(:CPDT_END_DT,'DD/MM/YYYY'),:CPDT_CERT_NO,:CPDT_CREATED_BY,sysdate,:CPDT_DOC_TYPE)";
            cmd_pv = new OracleCommand(ls_sql_pv, con);
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_PV_ID", pvid));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_SAFETY_PASS_NO", Session["spno"].ToString()));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_COMP_CODE", comp_cd));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_ST_DT", txt_frmdt.Text.Trim()));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_END_DT", txt_todt.Text.Trim()));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_CERT_NO", pvcertid));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_CREATED_BY", vVencode));
            cmd_pv.Parameters.Add(new OracleParameter(":CPDT_DOC_TYPE", "pv"));
            cmd_pv.ExecuteNonQuery();

            ls_sql_pv = "insert into t_sp_doc_verification(SDV_SAFETYPASS_NO,SDV_VERF_TYPE,SDV_REQ_NO,SDV_VERF_FLAG,SDV_CATEGORY,SDV_CREATED_BY,SDV_CREATED_DATE) values(:SDV_SAFETYPASS_NO,:SDV_VERF_TYPE,:SDV_REQ_NO,:SDV_VERF_FLAG,:SDV_CATEGORY,:SDV_CREATED_BY,sysdate)";
            cmd_pv = new OracleCommand(ls_sql_pv, con);
            cmd_pv.Parameters.Add(new OracleParameter(":SDV_SAFETYPASS_NO", Session["spno"].ToString()));
            cmd_pv.Parameters.Add(new OracleParameter(":SDV_VERF_TYPE", "PV"));
            cmd_pv.Parameters.Add(new OracleParameter(":SDV_REQ_NO", "PV" + pvid.ToString()));
            cmd_pv.Parameters.Add(new OracleParameter(":SDV_VERF_FLAG", "S"));
            cmd_pv.Parameters.Add(new OracleParameter(":SDV_CATEGORY", hidcat.Value));
            cmd_pv.Parameters.Add(new OracleParameter(":SDV_CREATED_BY", vVencode));
            cmd_pv.ExecuteNonQuery();

            if (updl_file.HasFile)
            {
                OracleCommand cmd_pvfiletrn = new OracleCommand();
                string ls_sql_pv1 = string.Empty;
                filename_pv = Path.GetFileName(updl_file.PostedFile.FileName);

                using (Stream fs = updl_file.PostedFile.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bytes = br.ReadBytes((Int32)fs.Length);

                        ls_sql_pv1 = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        cmd_pvfiletrn.CommandText = ls_sql_pv1;
                        cmd_pvfiletrn.Connection = con;
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_DOC_ID", pvcertid));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_NAME", filename_pv));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "PV"));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_COMP_CODE", comp_cd));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_CREATED_BY", vVencode));
                        cmd_pvfiletrn.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", vVencode));
                        cmd_pvfiletrn.ExecuteNonQuery();

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }

            //ShowMessage("Request Submited Successfully");
            clearpv();
            showpv(Session["spno"].ToString());

            //master page progress update start
            int childId = ViewState["ChildId"] != null ? (int)ViewState["ChildId"] : 0;
            if (childId > 0)
            {

                MenuMaster.MarkChildAsCompleted(childId);
                // Redirect to the next incomplete form
                var siteMaster = (MenuMaster)this.Master;
                var nextIncomplete = siteMaster.GetFirstIncompleteForm(siteMaster.GetSampleMenu());

                string message = "Personal Information Saved Successfully";
                string url = string.Format("{0}?childId={1}", nextIncomplete.FormPage, nextIncomplete.ChildId);
                string script = string.Format("alert('{0}'); window.location = '{1}';", message.Replace("'", "\\'"), url);

                if (nextIncomplete != null)
                {
                    //Response.Redirect(string.Format("{0}?childId={1}", nextIncomplete.FormPage, nextIncomplete.ChildId));
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", script, true);
                }
                else
                {
                    // lblMsg.Text = "All steps completed!";
                }
            }
            //master page progress update end


            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in btn_submit_Click: " + ex.Message);
        }

        //PV section end        

        //Age section start

        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt = new DataTable();
        string filenameage = string.Empty;
        string filenamedrv = string.Empty;
        string filenamepass = string.Empty;

        try
        {
            if (fupdldrv.HasFile && chkdriverold.Checked)
            {
                ShowMessage("choose either file upload or check previous upload documents option for driving licence");
                return;
            }

            if (fupdlpass.HasFile && chkpassold.Checked)
            {
                ShowMessage("choose either file upload or check previous upload documents option for passport documents");
                return;
            }

            if (!fupdlage.HasFile && !chkageold.Checked)
            {
                ShowMessage("Please Upload Age Proof");
                return;
            }
            else if (fupdlage.HasFile && chkageold.Checked)
            {
                ShowMessage("choose either file upload or check previous upload documents option for age proof");
                return;
            }
            else if (fupdlage.HasFile && !chkageold.Checked)
            {
                filenameage = Path.GetFileName(fupdlage.PostedFile.FileName);
                string contentType = fupdlage.PostedFile.ContentType;
                if (contentType.Substring(contentType.IndexOf("/") + 1).Equals("pdf"))
                {
                    if (fupdlage.PostedFile.ContentLength > 512000)
                    {
                        ShowMessage("Your file size is " + (fupdlage.PostedFile.ContentLength / 1024.0).ToString("0.00") + " KB " + "Please upload file within 500KB");
                        return;
                    }
                }
                else
                {
                    ShowMessage("Please Upload pdf file only");
                    return;
                }
            }

            if (!fupdldrv.HasFile && !chkdriverold.Checked)
            {
                if (Session["categorysaf"].ToString().Substring(0, 1).Equals("D"))
                {
                    ShowMessage("Please Upload Driving License");
                    return;
                }
            }
            else if (fupdldrv.HasFile && !chkdriverold.Checked)
            {
                filenamedrv = Path.GetFileName(fupdldrv.PostedFile.FileName);
                string contentType = fupdldrv.PostedFile.ContentType;
                if (contentType.Substring(contentType.IndexOf("/") + 1).Equals("pdf"))
                {
                    if (fupdldrv.PostedFile.ContentLength > 512000)
                    {
                        ShowMessage("Your file size is " + (fupdldrv.PostedFile.ContentLength / 1024.0).ToString("0.00") + " KB " + "Please upload file within 500KB");
                        return;
                    }
                }
                else
                {
                    ShowMessage("Please Upload pdf file only");
                    return;
                }
            }

            if (!fupdlpass.HasFile)
            {
                // No action needed here
            }
            else if (fupdlpass.HasFile && !chkpassold.Checked)
            {
                filenamepass = Path.GetFileName(fupdlpass.PostedFile.FileName);
                string contentType = fupdlpass.PostedFile.ContentType;
                if (contentType.Substring(contentType.IndexOf("/") + 1).Equals("pdf"))
                {
                    if (fupdlpass.PostedFile.ContentLength > 512000)
                    {
                        ShowMessage("Your file size is " + (fupdlpass.PostedFile.ContentLength / 1024.0).ToString("0.00") + " KB " + "Please upload file within 500KB");
                        return;
                    }
                }
                else
                {
                    ShowMessage("Please Upload pdf file only");
                    return;
                }
            }

            string ageid = TrnCWEAgeDrvSeqNo("");
            string drvid = "0";
            string passid = "0";

            if (fupdldrv.HasFile || chkdriverold.Checked)
            {
                drvid = TrnCWEAgeDrvSeqNo("");
            }
            else
            {
                drvid = "0";
            }

            if (fupdlpass.HasFile || chkpassold.Checked)
            {
                passid = TrnCWEAgeDrvSeqNo("");
            }
            else
            {
                passid = "0";
            }

            ls_sql = "update T_CEMP_DETAILS_TMP set CET_DOB_CERT_NO=:CET_DOB_CERT_NO,CET_DRV_CERT_NO=:CET_DRV_CERT_NO,CET_PASS_CERT_NO=:CET_PASS_CERT_NO where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO";

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (cmd = new OracleCommand(ls_sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":CET_DOB_CERT_NO", ageid));
                    cmd.Parameters.Add(new OracleParameter(":CET_DRV_CERT_NO", drvid));
                    cmd.Parameters.Add(new OracleParameter(":CET_PASS_CERT_NO", passid));
                    cmd.Parameters.Add(new OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim()));
                    cmd.Parameters.Add(new OracleParameter(":CET_REQUEST_NO", Session["requestnumber"]));
                    cmd.ExecuteNonQuery();

                    if (fupdlage.HasFile)
                    {
                        string filenameage1 = Path.GetFileName(fupdlage.PostedFile.FileName);

                        ls_sql = string.Empty;

                        byte[] bytes;
                        using (Stream fs = fupdlage.PostedFile.InputStream)
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            bytes = br.ReadBytes((int)fs.Length);
                        }

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmdfileage = new OracleCommand(ls_sql, con))
                        {
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_DOC_ID", ageid));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_NAME", filenameage1));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "AGE"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileage.ExecuteNonQuery();
                        }
                        updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "AG");
                    }
                    else if (chkageold.Checked)
                    {
                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) ";
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid";

                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmdfileage = new OracleCommand(ls_sql, con))
                        {
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_DOC_ID", ageid));
                            cmdfileage.Parameters.Add(new OracleParameter(":olddocid", hiddob.Value));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "AGE"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileage.ExecuteNonQuery();
                        }
                        updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "AG");
                    }

                    if (fupdldrv.HasFile)
                    {
                        string filenamedrv1 = Path.GetFileName(fupdldrv.PostedFile.FileName);
                        ls_sql = string.Empty;

                        byte[] bytes;
                        using (Stream fs = fupdldrv.PostedFile.InputStream)
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            bytes = br.ReadBytes((int)fs.Length);
                        }

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmdfiledrv = new OracleCommand(ls_sql, con))
                        {
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_DOC_ID", drvid));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_NAME", filenamedrv1));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "DRV"));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfiledrv.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfiledrv.ExecuteNonQuery();
                        }
                        updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "DL");
                    }
                    else if (chkdriverold.Checked)
                    {
                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) ";
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid";

                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmdfileage = new OracleCommand(ls_sql, con))
                        {
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_DOC_ID", drvid));
                            cmdfileage.Parameters.Add(new OracleParameter(":olddocid", hiddrv.Value));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "DRV"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileage.ExecuteNonQuery();
                        }
                        updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "DL");
                    }

                    if (fupdlpass.HasFile)
                    {
                        string filenamepass1 = Path.GetFileName(fupdlpass.PostedFile.FileName);
                        ls_sql = string.Empty;

                        byte[] bytes;
                        using (Stream fs = fupdlpass.PostedFile.InputStream)
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            bytes = br.ReadBytes((int)fs.Length);
                        }

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmdfilepass = new OracleCommand(ls_sql, con))
                        {
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_DOC_ID", passid));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_NAME", filenamepass1));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "PASS"));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfilepass.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfilepass.ExecuteNonQuery();
                        }
                        updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "PA");
                    }
                    else if (chkpassold.Checked)
                    {
                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE) ";
                        ls_sql = ls_sql + " SELECT :DM_DOC_ID,DM_NAME,:DM_FILE_TYPE,DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,SYSDATE,:DM_MODIFIED_BY,SYSDATE from T_DOCUMENT_MASTER where DM_DOC_ID=:olddocid";

                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmdfileage = new OracleCommand(ls_sql, con))
                        {
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_DOC_ID", passid));
                            cmdfileage.Parameters.Add(new OracleParameter(":olddocid", hdfpassold.Value));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "PASS"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfileage.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileage.ExecuteNonQuery();
                        }
                        updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "PA");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
        finally
        {
            getagedrv(TxtSpno.Text);

            if (Session["reqtype"].ToString() == "Renew")
            {
                foreach (GridViewRow gvrow in grdage.Rows)
                {
                    CheckBox chkbox = (CheckBox)gvrow.FindControl("chkSelectage");
                    HiddenField reqno = (HiddenField)gvrow.FindControl("hdreqno");
                    if (reqno.Value.Trim() == Session["requestnumber"].ToString())
                    {
                        chkbox.Enabled = true;
                    }
                    else
                    {
                        chkbox.Enabled = false;
                    }
                }
            }
            //empView();
            clearagedrv();
        }

        //Age section end

    }
    protected void btnCancel_Click(object sender, EventArgs e)
    {

    }

    protected void btnUpdateAll_Click(object sender, EventArgs e)
    {
        if (txtDOB.Text != "")
        {
            string db = txtDOB.Text.Replace("-", "/");
            DateTime originalDate = DateTime.ParseExact(db, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            db = originalDate.ToString("dd/MM/yyyy").Replace("-", "/"); ;
            txtDOB.Text = db;
            DateTime dob;
            if (DateTime.TryParseExact(txtDOB.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
            {
                int age = GetAge(dob);
                string trainee = cmbCategory.Items[0].Value.Substring(0, 1) + "A";

                if (age < 18)
                {
                    ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " + Session["comp_name_d"] + " .");
                }
                else if (age >= 18 && age <= 20)
                {
                    UpdateProfile();
                    cmbCategory.Items.FindByValue(trainee).Enabled = true;
                    cmbCategory.SelectedValue = trainee;
                    cmbCategory.Enabled = false;
                }
                else if (age > 60)
                {
                    hfActionPerformed.Value = "U";
                    pnlConfirmDocSubmision.Visible = true;
                    MPopUpConfirmDocSubmision.Show();
                }
                else
                {
                    UpdateProfile();
                    cmbCategory.Items.FindByValue(trainee).Enabled = false;
                    cmbCategory.Enabled = true;
                }
            }
        }
        updateage();
        updateAddress();
        updatePV();
    }

    protected void btnContinue_Click(object sender, EventArgs e)
    {
        //master page progress update start
        int childId = ViewState["ChildId"] != null ? (int)ViewState["ChildId"] : 0;
        if (childId > 0)
        {
            // Redirect to the next incomplete form
            var siteMaster = (MenuMaster)this.Master;
            var nextIncomplete = siteMaster.GetFirstIncompleteForm(siteMaster.GetSampleMenu());
            string url = string.Format("{0}?childId={1}", nextIncomplete.FormPage, nextIncomplete.ChildId);

            if (nextIncomplete != null)
            {
                Response.Redirect(string.Format("{0}?childId={1}", nextIncomplete.FormPage, nextIncomplete.ChildId));
            }
            else
            {
                // lblMsg.Text = "All steps completed!";
            }
        }
        //master page progress update end
    }

    #region Profile  
    protected void cmbUniqID_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtUniqIDNo.Text = "";
    }

    protected void txtUniqIDNo_valchanged(object sender, EventArgs e)
    {
        try
        {
            try
            {
                if (cmbUniqID.SelectedIndex <= 0)
                {
                    ShowMessage("Please Select Unique ID Type First");
                    txtUniqIDNo.Text = "";
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Please Select Unique ID Type First");
                txtUniqIDNo.Text = "";
                return;
            }

            string vSPNO = "";
            string cat = "";
            string vCategory = "";

            vCategory = cmbCategory.SelectedValue.ToString().ToUpper();
            if (vCategory == SF || vCategory == SV || vCategory == SH || vCategory == SA)
            {
                vCategory = SV;
            }
            else if (vCategory == WR || vCategory == WA)
            {
                vCategory = WR;
            }
            else if (vCategory == DV || vCategory == DA || vCategory == DH)
            {
                vCategory = DV;
            }
            else if (vCategory == VC || vCategory == VA)
            {
                vCategory = VC;
            }
            else if (vCategory == FM || vCategory == FA)
            {
                vCategory = FM;
            }


            DataTable dtCatVal = clmClass.get_codetype(vCategory, comp_cd);
            if (dtCatVal.Rows.Count > 0)
            {
                cat = dtCatVal.Rows[0]["CTM_VALUE"].ToString();
            }


            string vMMYY = DateTime.Today.ToString("MMyy");
            string vSerialNo = GET_SP_no();
            if (Session["Comp_code"].ToString() == "1000")
            {
                vSPNO = "R" + cat + vMMYY + vSerialNo;

            }
            else
            {
                vSPNO = GetSPInitial() + cat + vMMYY + vSerialNo;

            }


            //'' Duplicate ID Proof Check
            string sqlDuplicateID = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CET_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.Trim().ToUpper() + "' and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null)";
            DataTable dtDuplicateID = new DataTable();
            dtDuplicateID = getRecord(sqlDuplicateID, con);
            if (dtDuplicateID.Rows.Count > 0)
            {
                ShowMessage("This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows[0]["CET_SAFETY_PASSNO"].ToString() + ",please use this SP No. to raise request");
                return;
            }

            sqlDuplicateID = "Select CED_SAFETY_PASS_NO, CED_VENDOR_CODE, CED_COMPANY_CODE from t_cemp_details where CED_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.Trim().ToUpper() + "'";

            dtDuplicateID = getRecord(sqlDuplicateID, con);
            if (dtDuplicateID.Rows.Count > 0)
            {

                DataTable dtVendorInfo = new DataTable();
                string msg_add = "";
                string str_vendor_emails = "SELECT nvl(vdt_email1, '') mail1, nvl(vdt_phone1, '') phone1, vdt_vendor_name vendorname FROM HRACE.t_vendor_details WHERE vdt_vendor_code = '" + dtDuplicateID.Rows[0]["CED_VENDOR_CODE"].ToString().Trim() + "' AND vdt_company_code = '" + dtDuplicateID.Rows[0]["CED_COMPANY_CODE"].ToString().Trim() + "'";
                dtVendorInfo = getRecord(str_vendor_emails, con);
                if (dtVendorInfo.Rows.Count > 0)
                {
                    if (dtVendorInfo.Rows[0]["mail1"].ToString().Trim() != "")
                    {
                        msg_add = " (" + "Vendor Code: " + dtDuplicateID.Rows[0]["CED_VENDOR_CODE"].ToString().Trim() + ", Vendor Name: " + dtVendorInfo.Rows[0]["vendorname"].ToString().Trim() + ", Phone: " + dtVendorInfo.Rows[0]["phone1"].ToString().Trim() + ", Email: " + dtVendorInfo.Rows[0]["mail1"].ToString().Trim() + "). ";
                    }
                }

                txtUniqIDNo.Text = "";
                ShowMessage("This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows[0]["CED_SAFETY_PASS_NO"].ToString() + ",please use the SP No. to raise request." + msg_add);
                return;
            }


        }
        catch (Exception ex)
        {
            //Handle exception
        }
    }

    public void ShowMessage(string vMgs)
    {
        string vScript = string.Format("alert('{0}');", vMgs);
        ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", vScript, true);
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

    public void ErrorRow(HtmlTable tblError, string vErrMsg)
    {
        err_cnt++;
        HtmlTableRow err_tr = new HtmlTableRow();
        tblError.Rows.Add(err_tr);
        HtmlTableCell err_td = new HtmlTableCell(); // Create a table cell
        err_tr.Cells.Add(err_td); // Add the cell to the row
        err_td.InnerText = err_cnt + ") " + vErrMsg;
        err_tr.Style["color"] = "red";
        err_tr.Style["font-weight"] = "bold"; // Corrected to font-weight
        err_tr.Style["height"] = "3px";
    }

    private string GetSPInitial()
    {
        string locCd = GetLocationName(Session["Comp_code"] as string);
        string sqlAgencyCd = " select am.sam_agency_code from hrace.t_safety_agency_master am where am.sam_location_code=:loc_cd";

        // Use the property to get a new instance of the connection.
        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (OracleCommand command = new OracleCommand(sqlAgencyCd, con))
            {
                command.Parameters.Add(new OracleParameter(":loc_cd", locCd));
                DataTable dtAgendyCd = getRecord(command, con);

                if (dtAgendyCd.Rows.Count > 0 && dtAgendyCd.Rows[0].ItemArray.Length > 0)
                {
                    return dtAgendyCd.Rows[0][0].ToString().Substring(0, 1);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }

    private string GET_SP_no()
    {
        string SP_no = "";
        OracleCommand cmd3 = new OracleCommand();
        OracleDataReader drs2;

        using (OracleConnection con = new OracleConnection(strConn))
        {
            cmd3.Connection = con;
            cmd3.CommandText = " select HRACE_SP_SEQ.nextval from dual ";

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            drs2 = cmd3.ExecuteReader();

            if (drs2.Read())
            {
                SP_no = drs2[0].ToString();
            }

            drs2.Close();
        }

        return SP_no;
    }

    public string GetLocationName(string vCompCD)
    {
        string vLocCD = "";
        DataTable dtLoc = new DataTable();

        string sqlLocation = "select CMP_COMPANY_CODE,CMP_LOC_CD  from T_COMPANY_MASTER    where CMP_COMPANY_CODE='" + vCompCD + "'";
        dtLoc = getRecord(sqlLocation, con);

        if (dtLoc.Rows.Count > 0)
        {
            vLocCD = dtLoc.Rows[0]["CMP_LOC_CD"].ToString();
        }

        return vLocCD;
    }

    public DataTable getRecord(OracleCommand cmd, OracleConnection cn)
    {
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

    public static string b64encode(string StrEncode)
    {
        string encodedString;
        encodedString = (Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(StrEncode)));
        return (encodedString);
    }

    protected void Get_Sup()
    {
        if (Session["Req_type"] != null && (Session["Req_type"].ToString() == SPN || Session["Req_type"].ToString() == WFHN))
        {
            //PnlSafetyRenewal.Style.Add("display", "none");
            //btnSaveProfile.Visible = true;

            GetCategory(string.Format("'{0}','{1}','{2}','{3}'", SV_desc, SH_desc, SF_desc, SA_desc));
            //empView();
            int count = Convert.ToInt32(Session["supvsr"]);
            if (Session["requestnumber"] != null && Session["requestnumber"].ToString() != "" && count != 0)
            {
                contentDiv.Visible = true;
                //stop_profileEntry(count, string.Format("'{0}','{1}','{2}','{3}'", SV, SH, SF, SA), Session["requestnumber"].ToString());
                //clearAll();
            }
            else if (count == 0)
            {
                contentDiv.Visible = false;
                return;
            }
        }
        else if (Session["Req_type"] != null && Session["Req_type"].ToString() == SPR)
        {
            //ShowMessage("Please Enter the safety pass Number for renewal process");
            //PnlSafetyRenewal.Style.Remove("display");
        }
    }

    protected void Get_DR()
    {
        if (Session["Req_type"] != null && (Session["Req_type"].ToString() == SPN || Session["Req_type"].ToString() == WFHN))
        {
            //PnlSafetyRenewal.Style.Add("display", "none");
            //btnSaveProfile.Visible = true;

            GetCategory(string.Format("'{0}','{1}','{2}'", DV_desc, DA_desc, DH_desc));

            //empView();
            int count = Convert.ToInt32(Session["Driver"]);
            if (Session["requestnumber"] != null && Session["requestnumber"].ToString() != "" && count != 0)
            {
                contentDiv.Visible = true;
                //stop_profileEntry(count, string.Format("'{0}','{1}','{2}'", DV, DA, DH), Session["requestnumber"].ToString());
                //clearAll();
            }
            else if (count == 0)
            {
                contentDiv.Visible = false;
                return;
            }
        }
        else if (Session["Req_type"] != null && Session["Req_type"].ToString() == SPR)
        {
            //ShowMessage("Please Enter the safety pass Number for renewal process");
            //PnlSafetyRenewal.Style.Remove("display");
        }
    }

    protected void Get_FM()
    {
        if (Session["Req_type"] != null && (Session["Req_type"].ToString() == SPN || Session["Req_type"].ToString() == WFHN))
        {
            //PnlSafetyRenewal.Style.Add("display", "none");
            //btnSaveProfile.Visible = true;
            GetCategory(string.Format("'{0}','{1}'", FM_desc, FA_desc));
            //empView();
            int count = Convert.ToInt32(Session["FM"]);
            if (Session["requestnumber"] != null && Session["requestnumber"].ToString() != "" && count != 0)
            {
                contentDiv.Visible = true;
                //stop_profileEntry(count, string.Format("'{0}','{1}'", FM, FA), Session["requestnumber"].ToString());
                //clearAll();
            }
            else if (count == 0)
            {
                contentDiv.Visible = false;
                return;
            }
        }
        else if (Session["Req_type"] != null && Session["Req_type"].ToString() == SPR)
        {
            //ShowMessage("Please Enter the safety pass Number for renewal process");
            //PnlSafetyRenewal.Style.Remove("display");
        }
    }

    protected void Get_Wrk()
    {
        if (Session["Req_type"] != null && (Session["Req_type"].ToString() == SPN || Session["Req_type"].ToString() == WFHN))
        {
            //PnlSafetyRenewal.Style.Add("display", "none");
            //btnSaveProfile.Visible = true;

            GetCategory(string.Format("'{0}','{1}'", WR_desc, WA_desc));

            //empView();
            int count = Convert.ToInt32(Session["worker"]);
            if (Session["requestnumber"] != null && Session["requestnumber"].ToString() != "" && count != 0)
            {
                contentDiv.Visible = true;
                //stop_profileEntry(count, string.Format("'{0}','{1}'", WR, WA), Session["requestnumber"].ToString());
                //clearAll();
            }
            else if (count == 0)
            {
                contentDiv.Visible = false;
                return;
            }
        }
        else if (Session["Req_type"] != null && Session["Req_type"].ToString() == SPR)
        {
            //ShowMessage("Please Enter the safety pass Number for renewal process");
            //PnlSafetyRenewal.Style.Remove("display");
        }
    }

    protected void Get_VC()
    {
        if (Session["Req_type"] != null && (Session["Req_type"].ToString() == SPN || Session["Req_type"].ToString() == WFHN))
        {
            //PnlSafetyRenewal.Style.Add("display", "none");
            GetCategory(string.Format("'{0}','{1}'", VC_desc, VA_desc));
            //empView();
            int count = Convert.ToInt32(Session["VC"]);
            if (Session["requestnumber"] != null && Session["requestnumber"].ToString() != "" && count != 0)
            {
                contentDiv.Visible = true;
                //stop_profileEntry(count, string.Format("'{0}','{1}'", VC, VA), Session["requestnumber"].ToString());
                //clearAll();
            }
            else if (count == 0)
            {
                contentDiv.Visible = false;
                return;
            }
        }
        else if (Session["Req_type"] != null && Session["Req_type"].ToString() == SPR)
        {
            //ShowMessage("Please Enter the safety pass Number for renewal process");
            //PnlSafetyRenewal.Style.Remove("display");
        }
    }

    public void ReqClick(string Req_No)
    {
        string sql = "select srq_req_no, SPR.SRQ_REQ_TYPE, sprd.srd_emp_cat, SPRD.SRD_EMP_APV_COUNT, SPR.srq_dept_code, SPR.srq_company_cd, SPR.srq_location_cd, to_char(SRQ_CREATED_DT,'dd/MM/yyyy') SRQ_CREATED_DT  from HRACE.T_SP_REQUEST SPR , HRACE.t_sp_request_dtl SPRD  where spr.srq_req_no='" + Req_No + "'  and   SPRD.SRD_REQ_NO=SPR.SRQ_REQ_NO";
        DataTable dt = getRecord(sql, con);

        if (dt.Rows.Count > 0)
        {
            if (dt.Rows[0]["srq_dept_code"] != DBNull.Value)
            {
                Txtdeprt.Text = dt.Rows[0]["srq_dept_code"].ToString();
            }

            if (dt.Rows[0]["srq_location_cd"] != DBNull.Value)
            {
                Loc = dt.Rows[0]["srq_location_cd"].ToString();
            }

            for (int i = 0; i <= dt.Rows.Count - 1; i++)
            {
                if (dt.Rows[i]["srd_emp_cat"].ToString() == SV)
                {
                    Session["supvsr"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                    Get_Sup();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == WR)
                {
                    Session["worker"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                    Get_Wrk();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == DV)
                {
                    Session["Driver"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                    Get_DR();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == FM)
                {
                    Session["FM"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                    Get_FM();
                }
                else if (dt.Rows[i]["srd_emp_cat"].ToString() == VC)
                {
                    Session["VC"] = dt.Rows[i]["SRD_EMP_APV_COUNT"].ToString();
                    Get_VC();
                }
            }

            //pnlShw.Visible = false;
            //Pnlcategory.Visible = true;

            if (Session["requestType"].ToString() == "SPN")
            {
                //empView();
                //PnlSafetyRenewal.Style.Add("display", "none");               
            }
            else if (Session["requestType"].ToString() == "SPR")
            {
                //empView();
                //PnlSafetyRenewal.Style.Remove("display");
                //RenewalProcessGridview(Session["requestnumber"].ToString());
            }
        }
    }

    public void GetCategory(string DESCRIPTION)
    {
        string[] typeDescParamCount = DESCRIPTION.Split(',');
        string sqlCategory = t_Cemp_Type_Master() + " where CTM_STATUS ='A'   ";

        if (typeDescParamCount.Length > 1)
        {
            sqlCategory = sqlCategory + " AND CTM_TYPE_DESC IN(" + DESCRIPTION + ")";
        }
        else
        {
            sqlCategory = sqlCategory + " AND CTM_TYPE_DESC='" + DESCRIPTION + "'";
        }

        sqlCategory = sqlCategory + " and CTM_TYPE='SPET' AND SUBSTR(CTM_TYPE_CODE,-4,4)='" + comp_cd + "'";

        DataTable dtCategory = getRecord(sqlCategory, con);
        cmbCategory.Items.Clear();

        if (dtCategory.Rows.Count > 0)
        {
            cmbCategory.DataSource = dtCategory;
            cmbCategory.DataTextField = "CTM_TYPE_DESC";
            cmbCategory.DataValueField = "CTM_VALUE";
            cmbCategory.DataBind();
        }
    }

    public string t_Cemp_Type_Master()
    {
        string sql = "select * from t_Cemp_Type_Master ";
        return sql;
    }

    //public void stop_profileEntry(int count, string cat, string reqNo)
    //{
    //    int catcount = categoryCount(cat, reqNo);
    //    if (catcount == count)
    //    {
    //        contentDiv.Visible = false;
    //        ShowMessage("No employee left for Profile Entry.V");
    //        return;
    //    }
    //    else
    //    {
    //        contentDiv.Visible = true;
    //    }
    //}

    //public int categoryCount(string cat, string reqNo)
    //{        
    //    string[] catParamCount = cat.Split(',');
    //    string qry = "select count(*) count from HRACE.t_cemp_details_tmp where CET_REQUEST_NO='" + reqNo + "'";

    //    if (catParamCount.Length > 1)
    //    {
    //        qry += " and CET_CATEGORY IN(" + cat + ")";
    //    }
    //    else
    //    {
    //        qry += " and CET_CATEGORY='" + cat + "'";
    //    }

    //    DataTable dt = getRecord(qry, con);
    //    int catcount = Convert.ToInt32(dt.Rows[0]["count"]);
    //    return catcount;
    //}

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

            //CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            if (dtTable.Rows[5]["CTM_VALUE"] != DBNull.Value)
            {
                SH = dtTable.Rows[5]["CTM_VALUE"].ToString();
            }

            if (dtTable.Rows[6]["CTM_VALUE"] != DBNull.Value)
            {
                SF = dtTable.Rows[6]["CTM_VALUE"].ToString();
            }

            //sandeep
            if (dtTable.Rows[7]["CTM_VALUE"] != DBNull.Value)
            {
                SA = dtTable.Rows[7]["CTM_VALUE"].ToString();
            }
            if (dtTable.Rows[8]["CTM_VALUE"] != DBNull.Value)
            {
                WA = dtTable.Rows[8]["CTM_VALUE"].ToString();
            }
            if (dtTable.Rows[9]["CTM_VALUE"] != DBNull.Value)
            {
                DA = dtTable.Rows[9]["CTM_VALUE"].ToString();
            }
            if (dtTable.Rows[10]["CTM_VALUE"] != DBNull.Value)
            {
                FA = dtTable.Rows[10]["CTM_VALUE"].ToString();
            }
            if (dtTable.Rows[11]["CTM_VALUE"] != DBNull.Value)
            {
                VA = dtTable.Rows[11]["CTM_VALUE"].ToString();
            }
            //end
            if (dtTable.Rows[12]["CTM_VALUE"] != DBNull.Value)
            {
                DH = dtTable.Rows[12]["CTM_VALUE"].ToString();
            }

            if (dtTable.Rows[0]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                WR_desc = dtTable.Rows[0]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[1]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                SV_desc = dtTable.Rows[1]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[2]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                DV_desc = dtTable.Rows[2]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[3]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                FM_desc = dtTable.Rows[3]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[4]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                VC_desc = dtTable.Rows[4]["CTM_TYPE_DESC"].ToString();
            }

            //CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
            if (dtTable.Rows[5]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                SH_desc = dtTable.Rows[5]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[6]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                SF_desc = dtTable.Rows[6]["CTM_TYPE_DESC"].ToString();
            }

            //sandeep

            if (dtTable.Rows[7]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                SA_desc = dtTable.Rows[7]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[8]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                WA_desc = dtTable.Rows[8]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[9]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                DA_desc = dtTable.Rows[9]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[10]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                FA_desc = dtTable.Rows[10]["CTM_TYPE_DESC"].ToString();
            }

            if (dtTable.Rows[11]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                VA_desc = dtTable.Rows[11]["CTM_TYPE_DESC"].ToString();
            }
            //end
            if (dtTable.Rows[12]["CTM_TYPE_DESC"] != DBNull.Value)
            {
                DH_desc = dtTable.Rows[12]["CTM_TYPE_DESC"].ToString();
            }
        }
        else
        {
            Response.Redirect("frmInterface.aspx");
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
                    SPN = dt.Rows[0]["CTM_TYPE_DESC"].ToString();
                }

                if (dt.Rows[1]["CTM_VALUE"] != DBNull.Value)
                {
                    SPR = dt.Rows[1]["CTM_TYPE_DESC"].ToString();
                }

                if (dt.Rows[2]["CTM_VALUE"] != DBNull.Value)
                {
                    WFHN = dt.Rows[2]["CTM_TYPE_DESC"].ToString();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void GetAreaOfWork(DropDownList cmbObject, string vCode, string vMultipleCD = "N")
    {
        string sql = "";
        string vTempCode = "";

        if (vMultipleCD == "N")
        {
            sql = clmClass.get_CodeValue(vCode);
        }
        else
        {
            string[] arrCode = vCode.Split(',');
            for (int i = 0; i <= arrCode.Length - 1; i++)
            {
                vTempCode = vTempCode + "'" + arrCode[i] + "',";
            }
            vTempCode = vTempCode.Substring(0, vTempCode.Length - 1);
            sql = clmClass.get_CodeValue(vTempCode);
        }

        DataTable dt = new DataTable();
        dt = getRecord(sql, con);
        cmbObject.Items.Clear();

        if (dt.Rows.Count > 0)
        {
            cmbObject.DataSource = dt;
            cmbObject.DataTextField = "CTM_TYPE_DESC";
            cmbObject.DataValueField = "CTM_TYPE_DESC";
            cmbObject.DataBind();
            cmbObject.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    public void FillDropDown(DropDownList cmbObject, string vCode, string vMultipleCD = "N")
    {
        string sql = "";
        string vTempCode = "";

        if (vMultipleCD == "N")
        {
            sql = clmClass.get_CodeValue(vCode);
        }
        else
        {
            string[] arrCode = vCode.Split(',');
            for (int i = 0; i <= arrCode.Length - 1; i++)
            {
                vTempCode = vTempCode + "'" + arrCode[i] + "',";
            }
            vTempCode = vTempCode.Substring(0, vTempCode.Length - 1);
            sql = clmClass.get_CodeValue(vTempCode);
        }

        DataTable dt = new DataTable();
        dt = getRecord(sql, con);
        cmbObject.Items.Clear();

        if (dt.Rows.Count > 0)
        {
            cmbObject.DataSource = dt;
            cmbObject.DataTextField = "CTM_TYPE_DESC";
            cmbObject.DataValueField = "CTM_TYPE_CODE";
            cmbObject.DataBind();
            cmbObject.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    protected void txtDOB_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(txtDOB.Text) && txtDOB.Text != "__/__/____" && txtDOB.Text != "__-__-____")
        {
            string db = txtDOB.Text.Replace("-", "/");

            DateTime dob;
            if (DateTime.TryParseExact(db, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
            {
                int age = GetAge(dob);
                string trainee = cmbCategory.Items[0].Value.Substring(0, 1) + "A";

                ListItem traineeItem = cmbCategory.Items.FindByValue(trainee);

                if (age >= 18 && age <= 20 && traineeItem != null)
                {
                    traineeItem.Enabled = true;
                    cmbCategory.SelectedValue = trainee;
                    cmbCategory.Enabled = false;
                }
                else if (age < 18)
                {
                    ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " + Session["comp_name_d"] + " .");
                    txtDOB.Text = "";
                }
                else
                {
                    if (traineeItem != null)
                    {
                        traineeItem.Enabled = false;
                    }
                    cmbCategory.Enabled = true;
                }
            }
            else
            {
                ShowMessage("Invalid Date of Birth format. Please use dd/MM/yyyy.");
                txtDOB.Text = "";
            }
        }
    }

    private int GetAge(DateTime dob)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - dob.Year;
        if (dob > today.AddYears(-age))
        {
            age--;
        }
        return age;
    }

    protected void btnSaveProfile_Click(object sender, EventArgs e)
    {
        if (txtDOB.Text != "")
        {
            string db = txtDOB.Text.Replace("-", "/");
            DateTime originalDate = DateTime.ParseExact(db, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            db = originalDate.ToString("dd/MM/yyyy").Replace("-", "/");
            txtDOB.Text = db;
            DateTime dob;
            if (DateTime.TryParseExact(db, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
            {
                int age = GetAge(dob);
                string trainee = cmbCategory.Items[0].Value.Substring(0, 1) + "A";
                ListItem traineeItem = cmbCategory.Items.FindByValue(trainee);
                if (age < 18)
                {
                    ShowMessage("As per the Law, people below 18 years of age are not allowed to work in " + Session["comp_name_d"] + " .");
                    return;
                }
                else if (age > 60)
                {
                    hfActionPerformed.Value = "S";
                    pnlConfirmDocSubmision.Visible = true;
                    MPopUpConfirmDocSubmision.Show();
                }
                else if (age >= 18 && age <= 20 && traineeItem != null)
                {
                    traineeItem.Enabled = true;
                    cmbCategory.SelectedValue = trainee;
                    cmbCategory.Enabled = false;
                }
                else
                {
                    if (traineeItem != null)
                    {
                        traineeItem.Enabled = false;
                    }
                    cmbCategory.Enabled = true;

                    string ls_sql1 = "select trunc(sysdate)-(select trunc(SRQ_CREATED_DT) from t_sp_request where SRQ_REQ_NO=:SRQ_REQ_NO) from dual";
                    int timediff = 0;

                    using (OracleConnection con = new OracleConnection(strConn))
                    {
                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        using (OracleCommand cmd1 = new OracleCommand(ls_sql1, con))
                        {
                            cmd1.Parameters.Add(new OracleParameter(":SRQ_REQ_NO", Session["requestnumber"]));

                            DataTable dt1 = getRecord(cmd1, con);
                            if (dt1.Rows.Count > 0)
                            {
                                timediff = Convert.ToInt32(dt1.Rows[0][0].ToString());
                            }
                        }
                    }

                    //if (timediff > 10)
                    //{
                    //    ShowMessage("Your request is too old. You cannot able to proceed");
                    //    return;
                    //}
                    //else
                    //{
                    //    SaveProfile();
                    //}

                    SaveProfile();
                }
            }
        }
    }

    protected void btnConfirmDocSubmision_Click(object sender, EventArgs e)
    {
        if (hfActionPerformed.Value == "S")
        {
            SaveProfile();
        }
        else if (hfActionPerformed.Value == "U")
        {
            UpdateProfile();
        }
        else if (hfActionPerformed.Value == "A")
        {
            //AddSafetyPass();
        }
    }

    protected void btnCancelDocSubmisio_Click(object sender, EventArgs e)
    {
        pnlConfirmDocSubmision.Visible = false;
        MPopUpConfirmDocSubmision.Hide();
    }

    private void SaveProfile()
    {
        string sqlProfile = "";
        string vSPNO = "";
        string vAgency = "";
        string vCategory = "";
        int count = 0;
        string agency_code = string.Empty;
        OracleCommand cmd;

        string vErrorCount = "";
        vErrorCount = CheckProfileMandatoryFields();
        if (vErrorCount != "NA")
        {
            ShowMessage(vErrorCount);
            return;
        }
        else
        {

        }
        if (vErrorCount != "NA")
        {
            ShowMessage(vErrorCount);
            return;
        }
        else
        {

        }

        //if (vErrorCount > 0)
        //{
        //    tblProfileErrorList.Visible = true;
        //    return;
        //}
        //else
        //{
        //    tblProfileErrorList.Visible = false;
        //}

        vCategory = cmbCategory.SelectedValue.ToString().ToUpper();
        if (vCategory == SF || vCategory == SV || vCategory == SH || vCategory == SA)
        {
            vCategory = SV;
        }
        else if (vCategory == WR || vCategory == WA)
        {
            vCategory = WR;
        }
        else if (vCategory == DV || vCategory == DA || vCategory == DH)
        {
            vCategory = DV;
        }
        else if (vCategory == VC || vCategory == VA)
        {
            vCategory = VC;
        }
        else if (vCategory == FM || vCategory == FA)
        {
            vCategory = FM;
        }
        string cat = "";

        DataTable dtCatVal = clmClass.get_codetype(vCategory, comp_cd);
        if (dtCatVal.Rows.Count > 0)
        {
            cat = dtCatVal.Rows[0]["CTM_VALUE"].ToString();
        }

        string vMMYY = DateTime.Today.ToString("MMyy");
        string vSerialNo = GET_SP_no();
        if (Session["Comp_code"].ToString() == "1000")
        {
            vSPNO = "R" + cat + vMMYY + vSerialNo;
            Session["vSPNO"] = vSPNO;
        }
        else
        {
            vSPNO = GetSPInitial() + cat + vMMYY + vSerialNo;
            Session["vSPNO"] = vSPNO;
        }


        string sqlDuplicateID = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CET_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.Trim().ToUpper() + "' and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null)";
        DataTable dtDuplicateID = new DataTable();
        dtDuplicateID = getRecord(sqlDuplicateID, con);
        if (dtDuplicateID.Rows.Count > 0)
        {
            ShowMessage("This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows[0]["CET_SAFETY_PASSNO"].ToString() + ",please use this SP No. to raise request");
            return;
        }

        sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UNIQUE_ID_TYPE='" + cmbUniqID.SelectedValue + "'  and CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.Trim().ToUpper() + "' and CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim() + "'";

        dtDuplicateID = getRecord(sqlDuplicateID, con);
        if (dtDuplicateID.Rows.Count > 0)
        {
            ShowMessage("This ID Card(Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows[0]["CED_SAFETY_PASS_NO"].ToString() + ",please use this SP No. to raise request");
            return;
        }

        if (cmbUniqID.SelectedValue == "ADC")
        {
            string strchk = txtUniqIDNo.Text;
            bool st = strchk.Contains(" ");
            if (st)
            {
                ShowMessage("This is not a valid Adhaar number");
                return;
            }
            if (strchk.Length == 12)
            {
            }
            else
            {
                ShowMessage("This is not a valid Adhaar number");
                return;
            }
            Regex numeric = new Regex("^[0-9]+$");
            if ((numeric.IsMatch(strchk)))
            {
            }
            else
            {
                ShowMessage("This is not a valid Adhaar number");
                return;
            }
        }
        else if (cmbUniqID.SelectedValue == "PAN")
        {
            string strchk = txtUniqIDNo.Text;
            bool st = strchk.Contains(" ");
            if (st)
            {
                ShowMessage("This is not a valid PAN number");
                return;
            }
            if (strchk.Length == 10)
            {
            }
            else
            {
                ShowMessage("This is not a valid PAN number");
                return;
            }
            Regex alphanumeric = new Regex("[A-Z]{5}\\\\d{4}[A-Z]{1}");
            if ((alphanumeric.IsMatch(strchk)))
            {
            }
            else
            {
                ShowMessage("This is not a valid PAN number");
                return;
            }
        }
        if (txtPhNo.Text.Trim() == "")
        {
            string ls_sqlphchk = string.Empty;
            OracleCommand cmdphchk;
            DataTable dtchk = new DataTable();
            try
            {
                ls_sqlphchk = "select ACM_COMPANY_CODE from hrace.t_action_mapping where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_TYPE='PH' and ACM_CATEGORY=:ACM_CATEGORY and ACM_END_DT >=trunc(sysdate)";

                // Use the property to get a new instance of the connection.
                using (OracleConnection con1 = new OracleConnection(strConn))
                {
                    if (con1.State == ConnectionState.Closed)
                    {
                        con1.Open();
                    }

                    using (cmdphchk = new OracleCommand(ls_sqlphchk, con1))
                    {
                        cmdphchk.Parameters.Add(new OracleParameter(":ACM_COMPANY_CODE", comp_cd));
                        cmdphchk.Parameters.Add(new OracleParameter(":ACM_CATEGORY", cmbCategory.SelectedValue.ToString()));
                        dtchk = getRecord(cmdphchk, con1);
                        if (dtchk.Rows.Count > 0)
                        {
                            ShowMessage("Please enter mobile number");
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (optional)
            }
        }
        else
        {
            Regex numericph = new Regex("^[0-9]+$");
            if ((numericph.IsMatch(txtPhNo.Text.Trim())))
            {
                if (txtPhNo.Text.Length == 10)
                {
                }
                else
                {
                    ShowMessage("Please enter 10 digit mobile number");
                    return;
                }
            }
            else
            {
                ShowMessage("Please provide valid mobile number");
                return;
            }
        }

        bool locCheck = CheckWireFrameLoc();

        if (!IsFormAValid())
        {
            return;
        }

        string hidepan = "";
        string hideaadhar = "";
        string hidemobile = "";


        sqlProfile = " insert into T_CEMP_DETAILS_TMP(CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY,CET_LOC_CODE";
        sqlProfile += ",CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,CET_FATHER_NAME,CET_SPOUSE_NAME,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_UNIQUE_ID_TYPE,CET_MEDICAL_CENTRE,CET_REQ_CATEGORY,";
        if (pnlFormA.Visible)
        {
            sqlProfile += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,CET_DOB,CET_AGE,CET_AFFIRMATIVE,CET_CREATED_BY,CET_CREATED_DATE,";
            sqlProfile += "CET_PAN_NO,CET_ADLT_NAME,CET_ADLT_REL,CET_ADLT_ADDRESS,CET_ADLT_MOBILE_NO,CET_NATIONALITY,CET_AADHAR_NO,CET_EMP_PLACE,CET_RELAY_DATA)";
            hidepan = AESEncryption.Encrypt(txtPAN.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256);

            hideaadhar = AESEncryption.Encrypt(txtAADHAR.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256);
            hidemobile = AESEncryption.Encrypt(txtAdltMobile.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256);

        }
        else
        {
            sqlProfile += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,CET_DOB,CET_AGE,CET_AFFIRMATIVE,CET_CREATED_BY,CET_CREATED_DATE)";

        }

        sqlProfile += " values('";
        sqlProfile = sqlProfile + vSPNO + "','";
        sqlProfile = sqlProfile + Session["requestnumber"] + "','";
        sqlProfile = sqlProfile + comp_cd + "','";
        sqlProfile = sqlProfile + Session["VendCode"] + "','";
        sqlProfile = sqlProfile + cmbCategory.SelectedValue + "','";
        sqlProfile = sqlProfile + vLocCd + "','";
        sqlProfile = sqlProfile + Txtdeprt.Text + "','";
        sqlProfile = sqlProfile + txtFName.Text.Trim().ToUpper() + "','";
        sqlProfile = sqlProfile + txtLName.Text.Trim().ToUpper() + "','";
        sqlProfile = sqlProfile + txtFatherName.Text.Trim().ToUpper() + "','";
        sqlProfile = sqlProfile + txtHusName.Text.Trim().ToUpper() + "','";
        sqlProfile = sqlProfile + cmbSex.SelectedValue + "','";

        sqlProfile = sqlProfile + txtEmrgNo.Text.Trim() + "','";
        sqlProfile = sqlProfile + txtPhNo.Text.Trim() + "','";
        sqlProfile = sqlProfile + cmbUniqID.SelectedValue + "','";


        if ((locCheck == true && Session["requestType"].ToString() == "SPN"))
        {
            sqlProfile = sqlProfile + ddlMedCentre.SelectedValue + "','";
            sqlProfile = sqlProfile + "1" + "','";
        }
        else
        {
            sqlProfile = sqlProfile + ddlMedCentre.SelectedValue + "','";
            sqlProfile = sqlProfile + "0" + "','";
        }
        sqlProfile = sqlProfile + txtUniqIDNo.Text.Trim().ToUpper() + "','";
        sqlProfile = sqlProfile + txtIdentiFication.Text.Trim().ToUpper().Replace("'", "''") + "','";
        sqlProfile = sqlProfile + cmbWorkArea.Text.Trim() + "',";
        sqlProfile = sqlProfile + "to_date('" + txtDOB.Text.Trim() + "','DD/MM/YYYY'),";
        sqlProfile = sqlProfile + "to_char(sysdate,'yyyy') - to_char(to_date('" + txtDOB.Text.Trim() + "','DD/MM/YYYY'),'yyyy')" + ",'";
        sqlProfile = sqlProfile + cmbAffirmative.SelectedValue + "','";
        sqlProfile = sqlProfile + Session["VendCode"] + "',";

        if (pnlFormA.Visible)
        {
            sqlProfile = sqlProfile + "SYSDATE, '";
            sqlProfile = sqlProfile + hidepan + "','";
            sqlProfile = sqlProfile + txtAdltName.Text + "','";
            sqlProfile = sqlProfile + cmbAdltRelation.SelectedValue + "','";
            sqlProfile = sqlProfile + txtAdltAddress.Text + "','";
            sqlProfile = sqlProfile + hidemobile + "','";
            sqlProfile = sqlProfile + cmbNationality.SelectedValue + "','";
            sqlProfile = sqlProfile + hideaadhar + "','";
            sqlProfile = sqlProfile + cmbPlaceOfEmployment.SelectedValue + "','";
            sqlProfile = sqlProfile + cmbRelayData.SelectedValue + "')";
        }
        else
        {
            sqlProfile = sqlProfile + "SYSDATE" + ")";
        }

        SaveData(sqlProfile, con);


        if ((comp_cd == "1000"))
        {
            if (cmbCategory.SelectedValue == "VC")
            {
                agency_code = "VCP";
            }
            else
            {
                agency_code = "RTC";
            }
        }
        else
        {
            setLocationCode(vSPNO, Session["requestnumber"].ToString());
            string sqlAgencyCd = "select am.sam_agency_code from hrace.t_safety_agency_master am where am.sam_location_code=:loc_cd";

            using (OracleConnection con2 = new OracleConnection(strConn))
            {
                if (con2.State == ConnectionState.Closed)
                {
                    con2.Open();
                }

                using (cmd = new OracleCommand(sqlAgencyCd, con2))
                {
                    cmd.Parameters.Add(new OracleParameter(":loc_cd", vLocCd));
                    DataTable dtAgendyCd = getRecord(cmd, con2);
                    agency_code = dtAgendyCd.Rows[0][0].ToString();
                }
            }
        }




        try
        {
            string str1 = string.Empty;
            safetyPassdetails(vSPNO, Session["requestnumber"].ToString());
            string compst = getAragyaCompLoc(Session["Comp_Code"].ToString());
            if (compst == "Y")
            {
                str1 = "INSERT INTO HRACE.T_CEMP_DETAILS (CED_SAFETY_PASS_NO, CED_REQ_NO, CED_AGENCY_CODE, CED_COMPANY_CODE, CED_VENDOR_CODE, CED_CATEGORY, CED_LOC_CODE, CED_DEPT_CODE, CED_FIRSTNAME, CED_LASTNAME,";
                str1 += "CED_FATHER_NAME, CED_HUSBAND_NAME, CED_ADDRESS1, CED_ADDRESS2, CED_ADDRESS3, CED_COUNTRY, CED_EMERGENCY_NO, CED_PHONE_NO, CED_GENDER, CED_BLOOD_GROUP, CED_UNIQUE_ID_TYPE, CED_UNIQUE_ID_VALUE, CED_IDENTIFICATION_MARK,";
                str1 += "CED_QUALIFIATION, CED_AREA_OF_WORK, CED_CREATED_DATE, CED_CREATED_BY, CED_AGE, CED_DOB, CED_AFFIRMATIVE, CED_WORK_BASED_ON, CED_SUBLOC_CODE, CED_PV_ISSUED_ON, CED_PV_VALID_TILL, CED_POLICE_VERIFICATION, CED_MED_FIT, CED_DOB_CERT_NO, CED_DRV_CERT_NO, CED_PASS_CERT_NO, CED_UAN_NO, CED_IP_NO,CED_FLAG)";
                str1 += " VALUES('" + vSPNO + "','" + Session["requestnumber"] + "','" + agency_code.Trim() + "','" + Session["Comp_Code"] + "','" + vendorCode.Trim() + "','" + cmbCategory.SelectedValue + "','" + location.Trim() + "','" + dept.Trim() + "','" + firstname + "','" + lastname + "'";
                str1 += " ,'" + fatherName + "','" + spouse + "','NA','','','" + country + "','" + emergencyNo + "','" + phoneNo + "',";
                str1 += "'" + gender + "','NA','" + uniqueIDType + "','" + uniqueIDVal + "','" + identityMark + "',";
                str1 += "'" + qualification + "','" + areaofWork + "',sysdate,'System','" + birthAge + "',to_date('" + dob + "','DD/MM/YYYY'),'" + affirmative + "','NA','NA',null,null,'Y','N','" + dobcertno + "','" + drvcertno + "','" + passcertno + "','" + UAN + "','" + IP + "','Y' ) ";
                SaveData(str1, con);
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }

        btnSaveProfile.Visible = false;
        btnSubmit.Visible = true;
        //btnUpdateProfile.Visible = true;
        profile_details(vSPNO);
        ShowMessage("Safety pass number " + vSPNO + " created successfully");
        //tabcontainer1.Style.Remove("display");
        //BtnNext.Visible = true;
        Lblspno.Visible = true;
        TxtSpno.Visible = true;
        TxtSpno.Text = vSPNO;
        Session["spno"] = vSPNO;
        //empView();        
        mpconfirmsubmit.Show();

        //CMR NO:2016/10/22/J16/T1,Date:12/19/2016,Change by: Sandeep,Change: supervisor sub catagorized into SV,SH,SF
        //If vCategory = SV Then
        //count = Session("supvsr")
        if (vCategory == SV || vCategory == SH || vCategory == SF)
        {
            count = Convert.ToInt32(Session["supvsr"]);
            vCategory = string.Format("'{0}','{1}','{2}'", SV, SH, SF);
        }
        else if (vCategory == DV || vCategory == DA || vCategory == DH)
        {
            count = Convert.ToInt32(Session["Driver"]);
            vCategory = string.Format("'{0}','{1}','{2}'", DV, DA, DH);
        }
        else if (vCategory == WR || vCategory == WA)
        {
            count = Convert.ToInt32(Session["worker"]);
            vCategory = string.Format("'{0}','{1}'", WR, WA);
        }
        else if (vCategory == FM || vCategory == FA)
        {
            count = Convert.ToInt32(Session["FM"]);
            vCategory = string.Format("'{0}','{1}'", FM, FA);
        }
        else if (vCategory == VC || vCategory == VA)
        {
            count = Convert.ToInt32(Session["VC"]);
            vCategory = string.Format("'{0}','{1}'", VC, VA);
        }

        //count_emp(count, vCategory, Session["requestnumber"].ToString());

    }

    private void UpdateProfile()
    {
        err_cnt = 0;
        string vSPNo = "";
        string sqlUpdProfile = "";

        string vSysDate = DateTime.Today.ToString("dd/MM/yyyy");

        if (TxtSpno.Text.Trim() == "")
        {
            ShowMessage("Please Select Safety Pass No from the Gridview");
            return;
        }
        else
        {
            vSPNo = TxtSpno.Text.Trim().ToUpper();
        }

        string vErrorCount = "";
        vErrorCount = CheckProfileMandatoryFields();
        if (vErrorCount != "NA")
        {
            ShowMessage(vErrorCount);
            return;
        }
        else
        {

        }

        string sqlDuplicateID = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.Trim().ToUpper() + "' and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null) and CET_SAFETY_PASSNO <>'" + TxtSpno.Text.Trim() + "' ";
        DataTable dtDuplicateID = new DataTable();
        dtDuplicateID = getRecord(sqlDuplicateID, con);
        if (dtDuplicateID.Rows.Count > 0)
        {
            ShowMessage("This ID Card (Aadhar Number) already Exists in system for SP No:  " + dtDuplicateID.Rows[0]["CET_SAFETY_PASSNO"].ToString() + " ,please use this SP No. to raise request");
            return;
        }

        //'''''''''''''''check uniq ID number already exist or not'''''''''
        sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UNIQUE_ID_VALUE='" + txtUniqIDNo.Text.Trim().ToUpper() + "' and CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim() + "'";
        dtDuplicateID = getRecord(sqlDuplicateID, con);
        if (dtDuplicateID.Rows.Count > 0)
        {
            ShowMessage("This ID Card (Aadhar Number) already Exists in system for SP No : " + dtDuplicateID.Rows[0]["CED_SAFETY_PASS_NO"].ToString() + ",please use this SP No. to raise request");
            return;
        }

        if (cmbUniqID.SelectedValue == "ADC")
        {
            string strchk = txtUniqIDNo.Text;
            bool st = strchk.Contains(" ");
            if (st)
            {
                ShowMessage("This is not a valid Adhaar number");
                return;
            }
            if (strchk.Length == 12)
            {
            }
            else
            {
                ShowMessage("This is not a valid Adhaar number");
                return;
            }
            Regex numeric = new Regex("^[0-9]+$");
            if ((numeric.IsMatch(strchk)))
            {
            }
            else
            {
                ShowMessage("This is not a valid Adhaar number");
                return;
            }
        }
        else if (cmbUniqID.SelectedValue == "PAN")
        {
            string strchk = txtUniqIDNo.Text;
            bool st = strchk.Contains(" ");
            if (st)
            {
                ShowMessage("This is not a valid PAN number");
                return;
            }
            if (strchk.Length == 10)
            {
            }
            else
            {
                ShowMessage("This is not a valid PAN number");
                return;
            }
            Regex alphanumeric = new Regex("[A-Z]{5}\\\\d{4}[A-Z]{1}");
            if ((alphanumeric.IsMatch(strchk)))
            {
            }
            else
            {
                ShowMessage("This is not a valid PAN number");
                return;
            }
        }

        if (txtPhNo.Text.Trim() == "")
        {
            string ls_sqlphchk = string.Empty;
            OracleCommand cmdphchk;
            DataTable dtchk = new DataTable();
            try
            {
                ls_sqlphchk = "select ACM_COMPANY_CODE from hrace.t_action_mapping where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_TYPE='PH' and ACM_CATEGORY=:ACM_CATEGORY and ACM_END_DT >=trunc(sysdate)";
                cmdphchk = new OracleCommand(ls_sqlphchk, con);
                cmdphchk.Parameters.Add(new OracleParameter(":ACM_COMPANY_CODE", comp_cd));
                cmdphchk.Parameters.Add(new OracleParameter(":ACM_CATEGORY", cmbCategory.SelectedValue.ToString()));
                dtchk = getRecord(cmdphchk, con);
                if (dtchk.Rows.Count > 0)
                {
                    ShowMessage("Please enter mobile number");
                    return;
                }
            }
            catch (Exception ex)
            {
                // Handle exception (optional)
            }
        }
        else
        {
            Regex numericph = new Regex("^[0-9]+$");
            if ((numericph.IsMatch(txtPhNo.Text.Trim())))
            {
                if (txtPhNo.Text.Length == 10)
                {
                }
                else
                {
                    ShowMessage("Please enter 10 digit mobile number");
                    return;
                }
            }
            else
            {
                ShowMessage("Please provide valid mobile number");
                return;
            }
        }

        bool locCheck = CheckWireFrameLoc();

        //'Start add by Prasun Chakraborty on 11032022
        if (!IsFormAValid())
        {
            return;
        }
        //'End add by Prasun Chakraborty on 11032022

        sqlUpdProfile = "update t_cemp_details_tmp set ";
        sqlUpdProfile = sqlUpdProfile + "CET_FIRSTNAME ='" + txtFName.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_LASTNAME ='" + txtLName.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_FATHER_NAME ='" + txtFatherName.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_SPOUSE_NAME ='" + txtHusName.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_EMERGENCY_NO ='" + txtEmrgNo.Text.Trim() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_PHONE_NO ='" + txtPhNo.Text.Trim() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_AREA_OF_WORK ='" + cmbWorkArea.Text.Trim() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_DOB =" + " to_date('" + txtDOB.Text.Trim() + "','DD/MM/YYYY'),";
        sqlUpdProfile = sqlUpdProfile + "CET_GENDER ='" + cmbSex.SelectedValue + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_LOCATION_CODE ='" + comp_cd + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_LOC_CODE ='" + vLocCd + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_VENDOR_CODE ='" + Session["VendCode"] + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_DEPT_CODE ='" + Txtdeprt.Text.Trim() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_UNIQUE_ID_TYPE ='" + cmbUniqID.SelectedValue + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_UNIQUE_ID_VALUE ='" + txtUniqIDNo.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_IDENTIFICATION_MARK ='" + txtIdentiFication.Text.Trim().ToUpper().Replace("'", "''") + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_CATEGORY ='" + cmbCategory.SelectedValue + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_AFFIRMATIVE ='" + cmbAffirmative.SelectedValue + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_BY ='" + Session["VendCode"] + "',";

        if ((locCheck == true && Session["requestType"].ToString() == "SPN"))
        {
            sqlUpdProfile = sqlUpdProfile + "CET_MEDICAL_CENTRE ='" + ddlMedCentre.SelectedValue + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_REQ_CATEGORY ='1',";
        }
        else
        {
            sqlUpdProfile = sqlUpdProfile + "CET_MEDICAL_CENTRE ='" + ddlMedCentre.SelectedValue + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_REQ_CATEGORY ='0',";
        }

        //'Start Edit by Prasun Chakraborty on 11032022
        //'sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE "
        if (pnlFormA.Visible)
        {
            sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE, ";
            sqlUpdProfile = sqlUpdProfile + "CET_PAN_NO ='" + AESEncryption.Encrypt(txtPAN.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256) + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_NAME ='" + txtAdltName.Text + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_REL ='" + cmbAdltRelation.SelectedValue + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_ADDRESS ='" + txtAdltAddress.Text + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_ADLT_MOBILE_NO = '" + AESEncryption.Encrypt(txtAdltMobile.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256) + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_NATIONALITY = '" + cmbNationality.SelectedValue + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_AADHAR_NO = '" + AESEncryption.Encrypt(txtAADHAR.Text, ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256) + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_EMP_PLACE = '" + cmbPlaceOfEmployment.SelectedValue + "',";
            sqlUpdProfile = sqlUpdProfile + "CET_RELAY_DATA = '" + cmbRelayData.SelectedValue + "'";
            //'sqlProfile = sqlProfile + cmbNationality.SelectedValue + "','"
            //'sqlProfile = sqlProfile + txtAADHAR.Text + "','"
            //'sqlProfile = sqlProfile + cmbPlaceOfEmployment.SelectedValue + "')"
        }
        else
        {
            sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE ";
        }
        //'End Edit by Prasun Chakraborty on 11032022

        sqlUpdProfile = sqlUpdProfile + " where CET_SAFETY_PASSNO = '" + vSPNo + "'";
        sqlUpdProfile = sqlUpdProfile + " and  CET_REQUEST_NO = '" + Session["requestnumber"] + "'";

        try
        {
            SaveData(sqlUpdProfile, con);
            profile_details(vSPNo);
            ShowMessage("Updated Sucessfully");
            //Renewal_profile_details(vSPNo);
            //empView();
            //btnUpdateProfile.Visible = true;
            lblpfesiErrMsg.Text = "";
            //mpconfirmsubmit.Show();           
        }
        catch (Exception ex)
        {
            ShowMessage("Error While Updating Record");
        }
    }

    //private void AddSafetyPass()
    //{
    //    try
    //    {
    //        PanelEmp.Style.Add("display", "none");
    //        string spNo = txtRenewSpno.Text.Trim().ToUpper();

    //        if (string.IsNullOrEmpty(txtRenewSpno.Text))
    //        {
    //            ShowMessage("Provide the Safety pass number for renewal Process.");
    //            return;
    //        }

    //        string reqNo = lblreq.Text.Split(':')[1].Trim();

    //        int SV_count = 0;
    //        int WR_count = 0;
    //        int DV_count = 0;
    //        int VC_count = 0;
    //        int FM_count = 0;

    //        if (!string.IsNullOrEmpty(lnkSup.Text.Split(':')[1].Trim()))
    //        {
    //            SV_count = Convert.ToInt32(lnkSup.Text.Split(':')[1]);
    //        }
    //        if (!string.IsNullOrEmpty(lnkWrk.Text.Split(':')[1].Trim()))
    //        {
    //            WR_count = Convert.ToInt32(lnkWrk.Text.Split(':')[1]);
    //        }
    //        if (!string.IsNullOrEmpty(LnkDR.Text.Split(':')[1].Trim()))
    //        {
    //            DV_count = Convert.ToInt32(LnkDR.Text.Split(':')[1]);
    //        }
    //        if (!string.IsNullOrEmpty(LnkVC.Text.Split(':')[1].Trim()))
    //        {
    //            VC_count = Convert.ToInt32(LnkVC.Text.Split(':')[1]);
    //        }
    //        if (!string.IsNullOrEmpty(LnkFM.Text.Split(':')[1].Trim()))
    //        {
    //            FM_count = Convert.ToInt32(LnkFM.Text.Split(':')[1]);
    //        }

    //        string sqlSPStatus = T_CEMP_DETAILS_qry() + " Where ced_safety_pass_no = '" + spNo + "'";
    //        DataTable dtActive = getRecord(sqlSPStatus, con);
    //        if (dtActive.Rows.Count > 0)
    //        {
    //            if (dtActive.Rows[0]["CED_SP_BLOCKED"].ToString() == "Y")
    //            {
    //                ShowMessage("This Safety Pass Is Blocked");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            ShowMessage("No safety pass exists.Check the safety pass number");
    //            txtRenewSpno.Text = "";
    //            return;
    //        }

    //        string sql_ActiveSpno = Renewal_candidate(comp_cd, Session["VendCode"].ToString(), spNo);
    //        DataTable dt_ActiveSpno = getRecord(sql_ActiveSpno, con);
    //        if (dt_ActiveSpno.Rows.Count <= 0)
    //        {
    //            ShowMessage("The safety pass Number : " + spNo + "  is not authorized for renewal process.");
    //            txtRenewSpno.Text = "";
    //            return;
    //        }

    //        string sql_check = Renewal_candidate(comp_cd, Session["VendCode"].ToString(), spNo) + "  and  CED_REQ_NO is not null";
    //        DataTable dt_check = getRecord(sql_check, con);
    //        if (dt_check.Rows.Count > 0)
    //        {
    //            ShowMessage("The safety pass Number : " + spNo + "  is already added for renewal process.");
    //            lblAddValidation.Text = "The safety pass Number : " + spNo + "  is already added for renewal process."; //WI6447 START ADDED BY PRASUN ON 07012022
    //            txtRenewSpno.Text = "";
    //            return;
    //        }
    //        else
    //        {
    //            lblAddValidation.Text = string.Empty; //WI6447 START ADDED BY PRASUN ON 07012022
    //        }

    //        DataTable dt_category_check = t_cemp_detail_dt(spNo);
    //        string category_onCheck = "";
    //        if (dt_category_check.Rows.Count > 0)
    //        {
    //            category_onCheck = dt_category_check.Rows[0]["CED_CATEGORY"].ToString();
    //        }

    //        string sqlFM = "select * from HRACE.t_cemp_type_master m where  ctm_value='" + category_onCheck + "'  AND m.ctm_type='FMC' ";
    //        DataTable dtfm = getRecord(sqlFM, con);

    //        string sqlVc = "select * from HRACE.t_cemp_type_master m where  ctm_value='" + category_onCheck + "'  AND m.ctm_type='VCC' ";
    //        DataTable dtvc = getRecord(sqlVc, con);

    //        if (SV_count == 0 && (category_onCheck == SV || category_onCheck == SH || category_onCheck == SF || category_onCheck == SA))
    //        {
    //            ShowMessage("You cannot add the safety pass number: " + spNo + " as you have not requested supervisor for renewal process.");
    //            txtRenewSpno.Text = "";
    //            return;
    //        }
    //        else if (WR_count == 0 && (category_onCheck == WR || category_onCheck == WA))
    //        {
    //            ShowMessage("You cannot add the safety pass number: " + spNo + "  as you have not requested workres for renewal process.");
    //            txtRenewSpno.Text = "";
    //            return;
    //        }
    //        else if (DV_count == 0 && (category_onCheck == DV || category_onCheck == DA || category_onCheck == DH))
    //        {
    //            ShowMessage("You cannot add the safety pass number: " + spNo + "   as you have not requested drivers for renewal process.");
    //            txtRenewSpno.Text = "";
    //            return;
    //        }
    //        else if (FM_count == 0 && (category_onCheck != WR && category_onCheck != DV && category_onCheck != SV && category_onCheck != SH
    //            && category_onCheck != SF && category_onCheck != WA && category_onCheck != DA && category_onCheck != SA && category_onCheck != DH))
    //        {
    //            if (dtfm.Rows.Count > 0)
    //            {
    //                ShowMessage("You cannot add the safety pass number: " + spNo + "  as you have not requested Facility Manager for renewal process.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else
    //            {
    //                if (dtvc.Rows.Count > 0)
    //                {
    //                    if (VC_count == 0)
    //                    {
    //                        ShowMessage("You cannot add the safety pass number: " + spNo + "  as you have not requested Video Capsule for renewal process.");
    //                        txtRenewSpno.Text = "";
    //                        return;
    //                    }
    //                }
    //                else //Added (26/05/16)
    //                {
    //                    if ((category_onCheck == WR || category_onCheck == DV || category_onCheck == SV || category_onCheck == SH || category_onCheck == SF
    //                        || category_onCheck == WA || category_onCheck == DA || category_onCheck == SA || category_onCheck == DH))
    //                    {
    //                    }
    //                    else if ((category_onCheck != WR && category_onCheck != DV && category_onCheck != SV && category_onCheck != SH && category_onCheck != SF
    //                            && category_onCheck != WA && category_onCheck != DA && category_onCheck != SA && category_onCheck != DH))
    //                    {
    //                        ShowMessage("You cannot add the safety pass number : " + spNo + " for renewal process as it does not comes under any category of requested employee.");
    //                        txtRenewSpno.Text = "";
    //                        return;
    //                    }
    //                }
    //            }
    //        }
    //        else if (VC_count == 0 && (category_onCheck != WR && category_onCheck != DV && category_onCheck != SV && category_onCheck != SH && category_onCheck != SF
    //                                 && category_onCheck != WA && category_onCheck != DA && category_onCheck != SA && category_onCheck != DH))
    //        {
    //            if (dtfm.Rows.Count > 0)
    //            {
    //                ShowMessage("You cannot add the safety pass number: " + spNo + "  as you have not requested Facility Manager for renewal process.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else
    //            {
    //                if (dtvc.Rows.Count > 0)
    //                {
    //                    if (VC_count == 0)
    //                    {
    //                        ShowMessage("You cannot add the safety pass number: " + spNo + "  as you have not requested  Video capsule for renewal process.");
    //                        txtRenewSpno.Text = "";
    //                        return;
    //                    }
    //                }
    //                else //Added (26/05/16)
    //                {
    //                    if ((category_onCheck == WR || category_onCheck == DV || category_onCheck == SV || category_onCheck == SH || category_onCheck == SF
    //                        || category_onCheck == WA || category_onCheck == DA || category_onCheck == SA || category_onCheck == DH))
    //                    {
    //                    }
    //                    else if ((category_onCheck != WR && category_onCheck != DV && category_onCheck != SV && category_onCheck != SH && category_onCheck != SF
    //                            && category_onCheck != WA && category_onCheck != DA && category_onCheck != SA && category_onCheck != DH))
    //                    {
    //                        ShowMessage("You cannot add the safety pass number : " + spNo + " for renewal process as it does not comes under any category of requested employee.");
    //                        txtRenewSpno.Text = "";
    //                        return;
    //                    }
    //                }
    //            }
    //        }

    //        if ((category_onCheck == "WR" || category_onCheck == "WA"))
    //        {
    //            int WR_renewal = check_count_renewal(category_onCheck, reqNo, WR);

    //            if (WR_renewal == -2)
    //            {
    //                ShowMessage("You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else if (WR_renewal == WR_count)
    //            {
    //                ShowMessage("No more workers can be added for renewal process.");
    //                return;
    //            }
    //        }
    //        else if ((category_onCheck == "SV" || category_onCheck == "SH" || category_onCheck == "SF" || category_onCheck == "SA"))
    //        {
    //            int SV_renewal = check_count_renewal(category_onCheck, reqNo, SV);
    //            if (SV_renewal == -2)
    //            {
    //                ShowMessage("You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else if (SV_renewal == SV_count)
    //            {
    //                ShowMessage("No more Supervisor can be added for renewal process.");
    //                return;
    //            }
    //        }
    //        else if ((category_onCheck == "DV" || category_onCheck == "DA" || category_onCheck == "DH"))
    //        {
    //            int dv_renewal = check_count_renewal(category_onCheck, reqNo, DV);
    //            if (dv_renewal == -2)
    //            {
    //                ShowMessage("You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else if (dv_renewal == DV_count)
    //            {
    //                ShowMessage("No more Drivers can be added for renewal process.");
    //                return;
    //            }
    //        }
    //        else if ((category_onCheck == "FM" || category_onCheck == "FA"))
    //        {
    //            int FM_renewal = check_count_renewal(category_onCheck, reqNo, FM);
    //            if (FM_renewal == -2)
    //            {
    //                ShowMessage("You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of Facility Manager/Video capsule.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else if (FM_renewal == FM_count)
    //            {
    //                ShowMessage("No more Facility Managers can be added for renewal process.");
    //                return;
    //            }
    //        }
    //        else if ((category_onCheck == "VC" || category_onCheck == "VA"))
    //        {
    //            int VC_renewal = check_count_renewal(category_onCheck, reqNo, VC);

    //            if (VC_renewal == -2)
    //            {
    //                ShowMessage("You cannot add the safety pass Number : " + spNo + " for renewal process as the category of employee does not comes under category of video capsule/ Facility Manager.");
    //                txtRenewSpno.Text = "";
    //                return;
    //            }
    //            else if (VC_renewal == VC_count)
    //            {
    //                ShowMessage("No more Video capsule delegates can be added for renewal process.");
    //                return;
    //            }
    //        }

    //        string sql = Renewal_candidate(comp_cd, Session["VendCode"].ToString(), spNo);
    //        DataTable dt = getRecord(sql, con);

    //        if (dt.Rows.Count > 0)
    //        {
    //            string Updqry = "  update t_cemp_details set CED_REQ_NO='" + reqNo + "' where ced_safety_pass_no='" + spNo + "' ";
    //            using (OracleCommand cmd_upd_att = new OracleCommand(Updqry, con))
    //            {
    //                try
    //                {
    //                    if (con.State == ConnectionState.Closed)
    //                    {
    //                        con.Open();
    //                    }
    //                    cmd_upd_att.ExecuteNonQuery();
    //                }
    //                catch (Exception ex)
    //                {
    //                    ShowMessage(ex.Message);
    //                }
    //                finally
    //                {
    //                    if (con.State == ConnectionState.Open)
    //                    {
    //                        con.Close();
    //                    }
    //                }
    //            }

    //            Renewal_insert_T_cemp_details_tmp(spNo, reqNo);
    //            RenewalProcessGridview(reqNo);
    //            txtRenewSpno.Text = "";
    //        }
    //        else
    //        {
    //            ShowMessage("The safety pass number cannot be added for renewal process");
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ShowMessage(ex.ToString());
    //    }
    //}

    public string CheckProfileMandatoryFields()
    {
        string vErrorCount = "NA";
        DateTime vDOB;
        DateTime vTodayDate;
        vTodayDate = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), "dd/MM/yyyy", CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(txtFName.Text))
        {
            vErrorCount = "Enter First Name";
        }

        if (string.IsNullOrEmpty(txtFatherName.Text) && string.IsNullOrEmpty(txtHusName.Text.Trim()))
        {
            vErrorCount = "Enter Father/Husband Name";
        }

        if (cmbSex.SelectedValue == "0")
        {
            vErrorCount = "Select Gender";
        }

        if (string.IsNullOrEmpty(txtDOB.Text.Trim()))
        {
            vErrorCount = "Enter Date of birth";
        }
        else
        {
            try
            {
                string db1 = txtDOB.Text.Replace("-", "/");
                vDOB = DateTime.ParseExact(db1.Trim(), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                vErrorCount = "Enter a valid Date of Birth in DD/MM/YYYY Format";
            }
        }


        if (string.IsNullOrEmpty(txtPhNo.Text))
        {
            vErrorCount = "Enter personal Phone Number";
        }
        if (!string.IsNullOrEmpty(txtPhNo.Text.Trim()) && txtPhNo.Text.Length < 10)
        {
            vErrorCount = "Enter a valid mobile Number";
        }

        if (string.IsNullOrEmpty(txtEmrgNo.Text))
        {
            vErrorCount = "Enter Emergency Number";
        }
        if (!string.IsNullOrEmpty(txtEmrgNo.Text.Trim()) && txtEmrgNo.Text.Length < 10)
        {
            vErrorCount = "Enter a valid Emergency Number";
        }


        if (cmbWorkArea.SelectedValue == "0")
        {
            vErrorCount = "Select Work Area";
        }

        if (ddlMedCentre.SelectedValue == "0")
        {
            vErrorCount = "Select Medical Centre";
        }



        if (cmbUniqID.SelectedValue == "0")
        {
            vErrorCount = "Select Unique Identity Type";
        }


        if (cmbCategory.SelectedValue == "0")
        {
            vErrorCount = "Select Catgory";
        }

        if (string.IsNullOrEmpty(txtUniqIDNo.Text))
        {
            vErrorCount = "Enter Unique Identity Number";
        }

        if (string.IsNullOrEmpty(txtIdentiFication.Text))
        {
            vErrorCount = "Enter Identification Mark";
        }

        if (cmbAffirmative.SelectedValue == "0")
        {
            vErrorCount = "Select Affirmative";
        }


        DateTime dobValue;
        if (DateTime.TryParseExact(txtDOB.Text.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dobValue))
        {
            if (dobValue > vTodayDate.AddYears(-18))
            {
                vErrorCount = "As per the Law, people below 18 years of age are not allowed to work in " + Session["comp_name_d"] + " .";
            }
        }

        return vErrorCount;
    }

    private bool IsFormAValid()
    {
        bool blReturn = true;

        if (pnlFormA.Visible)
        {
            string strchk = txtPAN.Text;

            if (string.IsNullOrWhiteSpace(strchk))
            {
                ShowMessage("PAN No is required");
                blReturn = false;
            }

            if (strchk.Contains(" "))
            {
                ShowMessage("This is not a valid PAN number");
                blReturn = false;
            }

            if (strchk.Length != 10)
            {
                ShowMessage("This is not a valid PAN number");
                blReturn = false;
            }

            Regex alphanumeric = new Regex("^[A-Z]{5}\\d{4}[A-Z]{1}$");
            if (!alphanumeric.IsMatch(strchk))
            {
                ShowMessage("This is not a valid PAN number");
                blReturn = false;
            }

            // Relation Validation
            if (cmbAdltRelation.SelectedValue == "0")
            {
                ShowMessage("Relationship with adult person is required");
                blReturn = false;
            }

            // Name Validation
            if (string.IsNullOrWhiteSpace(txtAdltName.Text))
            {
                ShowMessage("Adult person name is required");
                blReturn = false;
            }

            // Address Validation
            if (string.IsNullOrWhiteSpace(txtAdltAddress.Text))
            {
                ShowMessage("Adult person address is required");
                blReturn = false;
            }

            // Mobile Validation
            Regex numericph = new Regex("^[0-9]+$");
            if (numericph.IsMatch(txtAdltMobile.Text.Trim()))
            {
                if (txtAdltMobile.Text.Length != 10)
                {
                    ShowMessage("Please enter 10 digit mobile number");
                    blReturn = false;
                }
            }
            else
            {
                ShowMessage("Please provide valid mobile number");
                blReturn = false;
            }

            // Aadhar No Validation
            string strchkAadhar = txtAADHAR.Text;
            if (strchkAadhar.Contains(" "))
            {
                ShowMessage("This is not a valid Adhaar number");
                blReturn = false;
            }

            if (strchkAadhar.Length != 12)
            {
                ShowMessage("This is not a valid Adhaar number");
                blReturn = false;
            }

            Regex numeric = new Regex("^[0-9]+$");
            if (!numeric.IsMatch(strchkAadhar))
            {
                ShowMessage("This is not a valid Adhaar number");
                blReturn = false;
            }

            // Nationality Validation
            if (cmbNationality.SelectedValue == "[Select]")
            {
                ShowMessage("Nationality is required");
                blReturn = false;
            }

            // PlaceOfEmployment Validation
            if (cmbPlaceOfEmployment.SelectedValue == "[Select]")
            {
                ShowMessage("Place of Employment is required");
                blReturn = false;
            }

            // RelayData Validation
            if (cmbRelayData.SelectedValue == "[Select]")
            {
                ShowMessage("Relay data is required");
                blReturn = false;
            }
        }

        return blReturn;
    }

    public bool CheckWireFrameLoc()
    {
        bool locCheck;
        string ls_sql = "SELECT at.ACM_TYPE FROM HRACE.t_cwm_action_mapping at where at.ACM_TYPE = 'ASSB' and at.ACM_FLAG = 'Y' AND at.ACM_COMPANY_CODE = :companyCode";

        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (OracleCommand cmd = new OracleCommand(ls_sql, con))
            {
                cmd.Parameters.Add(new OracleParameter(":companyCode", comp_cd));
                DataTable dt = getRecord(cmd, con);

                locCheck = dt.Rows.Count > 0;
            }
        }

        return locCheck;
    }

    private void ActiveControlsForFormA()
    {
        string ls_sql = "select * from hrace.t_lin_master where LIM_COMPANY_CODE=:LIM_COMPANY_CODE";

        using (OracleConnection con = new OracleConnection(strConn))
        {
            using (OracleCommand cmd = new OracleCommand(ls_sql, con))
            {
                cmd.Parameters.Add(new OracleParameter(":LIM_COMPANY_CODE", Session["Comp_code"]));
                DataTable dt = getRecord(cmd, con);

                if (dt.Rows.Count > 0)
                {
                    pnlFormA.Visible = true;
                }
                else
                {
                    pnlFormA.Visible = false;
                }
            }
        }

        PopulateRelayCombo();
    }

    private void PopulateRelayCombo()
    {
        string strQry = string.Empty;
        strQry = "select ACM_REMARKS from hrace.T_CWM_ACTION_MAPPING WHERE ACM_TYPE = 'RELAY' and ACM_FLAG = 'Y' ORDER BY ACM_CATEGORY";

        using (OracleConnection con = new OracleConnection(strConn))
        {
            using (OracleCommand cmd = new OracleCommand(strQry, con))
            {
                DataTable dt = getRecord(cmd, con);
                cmbRelayData.DataSource = dt;
                cmbRelayData.DataTextField = "ACM_REMARKS";
                cmbRelayData.DataValueField = "ACM_REMARKS";
                cmbRelayData.DataBind();
            }
        }

        cmbRelayData.Items.Insert(0, new ListItem("[Select]", "[Select]"));
    }

    public void SaveData(string sql, OracleConnection cn)
    {
        OracleCommand cmd = new OracleCommand(sql, cn);
        try
        {
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            throw;
        }
        finally
        {
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
        }
    }

    public void setLocationCode(string sp_no, string req_no)
    {
        OracleCommand cmd = new OracleCommand();
        Loc = "";
        string sqlLocCd = "  select tmp.cet_loc_code from hrace.t_cemp_details_tmp tmp where tmp.cet_safety_passno=:sp_no and tmp.cet_request_no=:req_no";

        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (cmd = new OracleCommand(sqlLocCd, con))
            {
                cmd.Parameters.Add(new OracleParameter(":sp_no", sp_no));
                cmd.Parameters.Add(new OracleParameter(":req_no", req_no));
                DataTable dt = getRecord(cmd, con);
                Loc = dt.Rows[0][0].ToString();
            }
        }
    }

    public void safetyPassdetails(string safetyPassNo, string reqNo)
    {
        try
        {
            clearVariables();

            string qry = "select CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY,CET_LOC_CODE,CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME," +
                         "CET_FATHER_NAME,CET_SPOUSE_NAME,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_BLOOD_GROUP,CET_UNIQUE_ID_TYPE,CET_UNIQUE_ID_VALUE,CET_IDENTIFICATION_MARK, " +
                         " CET_AREA_OF_WORK, CET_AGE, to_char(CET_DOB,'dd/MM/yyyy') CET_DOB,CET_AFFIRMATIVE," +
                         " (select ctm_type_desc from t_cemp_type_master where substr(CTM_TYPE_CODE, '-4', '4') = '" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_PROFILE_STATUS) CET_PROFILE_STATUS," +
                         " (select ctm_type_desc from t_cemp_type_master where substr(CTM_TYPE_CODE, '-4', '4') = '" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_DOCVER_STATUS)    CET_DOCVER_STATUS" +
                         " from t_cemp_details_tmp cdt " +
                         " where CET_SAFETY_PASSNO='" + safetyPassNo + "' " +
                         " and CET_REQUEST_NO='" + reqNo + "' ";

            DataTable dt = getRecord(qry, con);

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["CET_LOCATION_CODE"] != DBNull.Value)
                {
                    locationCode = dt.Rows[0]["CET_LOCATION_CODE"].ToString();
                }

                if (dt.Rows[0]["CET_LOC_CODE"] != DBNull.Value)
                {
                    location = dt.Rows[0]["CET_LOC_CODE"].ToString();
                }

                if (dt.Rows[0]["CET_VENDOR_CODE"] != DBNull.Value)
                {
                    vendorCode = dt.Rows[0]["CET_VENDOR_CODE"].ToString();
                }
                if (dt.Rows[0]["CET_CATEGORY"] != DBNull.Value)
                {
                    category = dt.Rows[0]["CET_CATEGORY"].ToString();
                }

                if (dt.Rows[0]["CET_DEPT_CODE"] != DBNull.Value)
                {
                    dept = dt.Rows[0]["CET_DEPT_CODE"].ToString();
                    ViewState["deptchk"] = dept.Trim();
                }

                if (dt.Rows[0]["CET_FIRSTNAME"] != DBNull.Value)
                {
                    firstname = dt.Rows[0]["CET_FIRSTNAME"].ToString();
                }

                if (dt.Rows[0]["CET_LASTNAME"] != DBNull.Value)
                {
                    lastname = dt.Rows[0]["CET_LASTNAME"].ToString();
                }

                if (dt.Rows[0]["CET_FATHER_NAME"] != DBNull.Value)
                {
                    fatherName = dt.Rows[0]["CET_FATHER_NAME"].ToString();
                }

                if (dt.Rows[0]["CET_SPOUSE_NAME"] != DBNull.Value)
                {
                    spouse = dt.Rows[0]["CET_SPOUSE_NAME"].ToString();
                }

                if (dt.Rows[0]["CET_GENDER"] != DBNull.Value)
                {
                    gender = dt.Rows[0]["CET_GENDER"].ToString();
                }

                if (dt.Rows[0]["CET_EMERGENCY_NO"] != DBNull.Value)
                {
                    emergencyNo = dt.Rows[0]["CET_EMERGENCY_NO"].ToString();
                }

                if (dt.Rows[0]["CET_PHONE_NO"] != DBNull.Value)
                {
                    phoneNo = dt.Rows[0]["CET_PHONE_NO"].ToString();
                }

                if (dt.Rows[0]["CET_BLOOD_GROUP"] != DBNull.Value)
                {
                    bloodGroup = dt.Rows[0]["CET_BLOOD_GROUP"].ToString();
                }
                if (dt.Rows[0]["CET_UNIQUE_ID_VALUE"] != DBNull.Value)
                {
                    uniqueIDVal = dt.Rows[0]["CET_UNIQUE_ID_VALUE"].ToString();
                }

                if (dt.Rows[0]["CET_IDENTIFICATION_MARK"] != DBNull.Value)
                {
                    identityMark = dt.Rows[0]["CET_IDENTIFICATION_MARK"].ToString();
                }

                if (dt.Rows[0]["CET_UNIQUE_ID_TYPE"] != DBNull.Value)
                {
                    uniqueIDType = dt.Rows[0]["CET_UNIQUE_ID_TYPE"].ToString();
                }
                if (dt.Rows[0]["CET_AREA_OF_WORK"] != DBNull.Value)
                {
                    areaofWork = dt.Rows[0]["CET_AREA_OF_WORK"].ToString();
                }

                if (dt.Rows[0]["CET_AGE"] != DBNull.Value)
                {
                    birthAge = dt.Rows[0]["CET_AGE"].ToString();
                }
                if (dt.Rows[0]["CET_DOB"] != DBNull.Value)
                {
                    dob = dt.Rows[0]["CET_DOB"].ToString();
                }
                if (dt.Rows[0]["CET_AFFIRMATIVE"] != DBNull.Value)
                {
                    affirmative = dt.Rows[0]["CET_AFFIRMATIVE"].ToString();
                }
            }
        }
        catch (Exception ex)
        {
            ShowMessage(ex.ToString());
        }
    }

    public void clearVariables()
    {
        spouse = "";
        fatherName = "";
        lastname = "";
        firstname = "";
        category = "";
        vendorCode = "";
        location = "";
        locationCode = "";
        gender = "";
        emergencyNo = "";
        phoneNo = "";
        bloodGroup = "";
        uniqueIDVal = "";
        uniqueIDType = "";
        identityMark = "";
        areaofWork = "";
        birthAge = "";
        dob = "";
        affirmative = "";
        address1 = "";
        address2 = "";
        address3 = "";
        country = "";
        qualification = "";
        profile_status = "";
        verify_status = "";
        dobcertno = "";
        drvcertno = "";
        passcertno = "";
        UAN = "";
        IP = "";
    }

    public string getAragyaCompLoc(object vcompcode)
    {
        string st = "N";
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt1 = new DataTable();

        try
        {
            ls_sql = "select ACM_TYPE from t_cwm_action_mapping where ACM_COMPANY_CODE=:ACM_COMPANY_CODE and ACM_FLAG='Y' and ACM_TYPE='RR'";

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (cmd = new OracleCommand(ls_sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":ACM_COMPANY_CODE", vcompcode));
                    dt1 = getRecord(cmd, con);

                    if (dt1.Rows.Count > 0)
                    {
                        st = "Y";
                    }
                    else
                    {
                        st = "N";
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }

        return st;
    }

    //public void count_emp(int count, string cat, string reqNo)
    //{
    //    int catcount = categoryCount(cat, reqNo);
    //    int count_diff = 0;

    //    count_diff = count - catcount;
    //    Lblcount.Visible = true;
    //    Lblcount.Text = count_diff.ToString() + "/" + count.ToString();
    //    LblempLeft.Visible = true;

    //    if (count_diff == 0)
    //    {
    //        btnSaveProfile.Visible = false;
    //    }
    //}

    protected void ibtnCloseconfirmsubmit_Click(object sender, EventArgs e)
    {
        if (txtuan.Text.Trim().Length != 12 && txtuan.Text.Trim().ToUpper() != "NA")
        {
            lblpfesiErrMsg.Text = "UAN Number(under EPFO Act) should be 12 digit. Put NA if not applicable.";
            txtuan.Text = "";
            mpconfirmsubmit.Show();
            return;
        }

        if (txtip.Text.Trim().Length != 10 && txtip.Text.Trim().ToUpper() != "NA")
        {
            lblpfesiErrMsg.Text = "IP Number(under ESIC Act) should be 10 digit. Put NA if not applicable.";
            txtip.Text = "";
            mpconfirmsubmit.Show();
            return;
        }

        if (txtuan.Text.Trim().ToUpper() != "NA")
        {
            string sqlDuplicateID = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_UAN_NO='" + txtuan.Text.Trim().ToUpper() + "'   and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null) and CET_SAFETY_PASSNO <>'" + TxtSpno.Text.Trim() + "' ";
            DataTable dtDuplicateID = getRecord(sqlDuplicateID, con);
            if (dtDuplicateID.Rows.Count > 0)
            {
                lblpfesiErrMsg.Text = "This UAN Number already Exists In system For SP No : " + dtDuplicateID.Rows[0]["CET_SAFETY_PASSNO"].ToString();
                txtuan.Text = "";
                mpconfirmsubmit.Show();
                return;
            }

            //'''''''''''''''check uniq ID number already exist or not'''''''''
            sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_UAN_NO='" + txtuan.Text.Trim().ToUpper() + "'  AND CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim() + "'";
            dtDuplicateID = getRecord(sqlDuplicateID, con);
            if (dtDuplicateID.Rows.Count > 0)
            {
                lblpfesiErrMsg.Text = "This UAN Number already Exists In system For SP No : " + dtDuplicateID.Rows[0]["CED_SAFETY_PASS_NO"].ToString();
                txtuan.Text = "";
                mpconfirmsubmit.Show();
                return;
            }
        }

        if (txtip.Text.Trim().ToUpper() != "NA")
        {
            string sqlDuplicateID = "Select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_IP_NO='" + txtip.Text.Trim().ToUpper() + "'   and (CET_REQ_STATUS = 'C' or  CET_REQ_STATUS is null) and CET_SAFETY_PASSNO <>'" + TxtSpno.Text.Trim() + "' ";
            DataTable dtDuplicateID = getRecord(sqlDuplicateID, con);
            if (dtDuplicateID.Rows.Count > 0)
            {
                lblpfesiErrMsg.Text = "This IP Number already Exists In system For SP No : " + dtDuplicateID.Rows[0]["CET_SAFETY_PASSNO"].ToString();
                txtuan.Text = "";
                mpconfirmsubmit.Show();
                return;
            }

            //'''''''''''''''check uniq ID number already exist or not'''''''''
            sqlDuplicateID = "Select CED_SAFETY_PASS_NO from t_cemp_details where CED_IP_NO='" + txtip.Text.Trim().ToUpper() + "'  AND CED_REQ_NO is null  and CED_SAFETY_PASS_NO <> '" + TxtSpno.Text.Trim() + "'";
            dtDuplicateID = getRecord(sqlDuplicateID, con);
            if (dtDuplicateID.Rows.Count > 0)
            {
                lblpfesiErrMsg.Text = "This IP Number already Exists In system For SP No : " + dtDuplicateID.Rows[0]["CED_SAFETY_PASS_NO"].ToString();
                txtuan.Text = "";
                mpconfirmsubmit.Show();
                return;
            }
        }

        string sqlUpdProfile = "";
        string vSPNo = "";
        vSPNo = TxtSpno.Text.Trim().ToUpper();
        sqlUpdProfile = "update t_cemp_details_tmp set ";
        sqlUpdProfile = sqlUpdProfile + "CET_UAN_NO ='" + txtuan.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_IP_NO ='" + txtip.Text.Trim().ToUpper() + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_BY ='" + Session["VendCode"] + "',";
        sqlUpdProfile = sqlUpdProfile + "CET_MODIFIED_DATE =SYSDATE ";
        sqlUpdProfile = sqlUpdProfile + " where CET_SAFETY_PASSNO = '" + vSPNo + "'";
        sqlUpdProfile = sqlUpdProfile + " and  CET_REQUEST_NO = '" + Session["requestnumber"] + "'";

        try
        {
            SaveData(sqlUpdProfile, con);
        }
        catch (Exception ex)
        {
            ShowMessage("Error While Updating Record");
        }
    }

    public void profile_details(string sp_no)
    {
        txtFName.Text = "";
        txtLName.Text = "";
        txtDOB.Text = "";
        cmbSex.SelectedValue = "0";
        txtFatherName.Text = "";
        txtHusName.Text = "";
        txtPhNo.Text = "";
        txtEmrgNo.Text = "";
        cmbUniqID.SelectedValue = "0";
        txtIdentiFication.Text = "";
        cmbAffirmative.SelectedValue = "0";
        txtUniqIDNo.Text = "";
        cmbWorkArea.SelectedValue = "0";
        TxtSpno.Text = "";
        ddlMedCentre.SelectedValue = "0";

        string qry = emp_detail_qry();
        qry = qry + "and CET_SAFETY_PASSNO ='" + sp_no + "'";
        DataTable dt = getRecord(qry, con);

        if (dt.Rows.Count > 0)
        {
            if (dt.Rows[0]["CET_CATEGORY"] != DBNull.Value)
            {
                cmbCategory.Items.Clear();
                string category = dt.Rows[0]["CET_CATEGORY"].ToString();
                cat(category);
                cmbCategory.SelectedValue = category;
            }
            if (dt.Rows[0]["CET_FIRSTNAME"] != DBNull.Value)
            {
                txtFName.Text = dt.Rows[0]["CET_FIRSTNAME"].ToString();
            }
            if (dt.Rows[0]["CET_LASTNAME"] != DBNull.Value)
            {
                txtLName.Text = dt.Rows[0]["CET_LASTNAME"].ToString();
            }
            if (dt.Rows[0]["CET_DOB"] != DBNull.Value)
            {
                txtDOB.Text = dt.Rows[0]["CET_DOB"].ToString();
                string db = txtDOB.Text.Replace("-", "/");
                DateTime originalDate = DateTime.ParseExact(db, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                db = originalDate.ToString("yyyy/MM/dd").Replace("/", "-"); ;
                txtDOB.Text = db;
            }
            if (dt.Rows[0]["CET_GENDER"] != DBNull.Value)
            {
                try
                {
                    cmbSex.SelectedValue = dt.Rows[0]["CET_GENDER"].ToString();
                }
                catch (Exception ex)
                {

                }
            }

            if (dt.Rows[0]["CET_MEDICAL_CENTRE"] != DBNull.Value)
            {
                try
                {
                    ddlMedCentre.SelectedValue = dt.Rows[0]["CET_MEDICAL_CENTRE"].ToString();
                }
                catch (Exception ex)
                {

                }
            }

            if (dt.Rows[0]["CET_FATHER_NAME"] != DBNull.Value)
            {
                txtFatherName.Text = dt.Rows[0]["CET_FATHER_NAME"].ToString();
            }
            if (dt.Rows[0]["CET_SPOUSE_NAME"] != DBNull.Value)
            {
                txtHusName.Text = dt.Rows[0]["CET_SPOUSE_NAME"].ToString();
            }
            if (dt.Rows[0]["CET_PHONE_NO"] != DBNull.Value)
            {
                txtPhNo.Text = dt.Rows[0]["CET_PHONE_NO"].ToString();
            }
            if (dt.Rows[0]["CET_EMERGENCY_NO"] != DBNull.Value)
            {
                txtEmrgNo.Text = dt.Rows[0]["CET_EMERGENCY_NO"].ToString();
            }
            if (dt.Rows[0]["CET_UNIQUE_ID_TYPE"] != DBNull.Value)
            {
                try
                {
                    cmbUniqID.SelectedValue = dt.Rows[0]["CET_UNIQUE_ID_TYPE"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
            if (dt.Rows[0]["CET_UNIQUE_ID_VALUE"] != DBNull.Value)
            {
                txtUniqIDNo.Text = dt.Rows[0]["CET_UNIQUE_ID_VALUE"].ToString();
            }
            if (dt.Rows[0]["CET_IDENTIFICATION_MARK"] != DBNull.Value)
            {
                txtIdentiFication.Text = dt.Rows[0]["CET_IDENTIFICATION_MARK"].ToString();
            }
            if (dt.Rows[0]["CET_AFFIRMATIVE"] != DBNull.Value)
            {
                try
                {
                    cmbAffirmative.SelectedValue = dt.Rows[0]["CET_AFFIRMATIVE"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
            if (dt.Rows[0]["CET_AREA_OF_WORK"] != DBNull.Value)
            {
                try
                {
                    cmbWorkArea.SelectedValue = dt.Rows[0]["CET_AREA_OF_WORK"].ToString();
                }
                catch (Exception ex)
                {

                }
            }

            txtuan.Text = dt.Rows[0]["CET_UAN_NO"].ToString().Trim();
            txtip.Text = dt.Rows[0]["CET_IP_NO"].ToString().Trim();
            btnSaveProfile.Visible = false;
            //btnUpdateProfile.Visible = true;

            TxtSpno.Text = dt.Rows[0]["CET_SAFETY_PASSNO"].ToString();



            if (dt.Rows[0]["CET_PAN_NO"] != DBNull.Value)
            {
                txtPAN.Text = AESEncryption.Decrypt(dt.Rows[0]["CET_PAN_NO"].ToString(), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256);
            }
            if (dt.Rows[0]["CET_ADLT_NAME"] != DBNull.Value)
            {
                txtAdltName.Text = dt.Rows[0]["CET_ADLT_NAME"].ToString();
            }
            if (dt.Rows[0]["CET_ADLT_REL"] != DBNull.Value)
            {
                try
                {
                    cmbAdltRelation.SelectedValue = dt.Rows[0]["CET_ADLT_REL"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
            if (dt.Rows[0]["CET_ADLT_ADDRESS"] != DBNull.Value)
            {
                txtAdltAddress.Text = dt.Rows[0]["CET_ADLT_ADDRESS"].ToString();
            }
            if (dt.Rows[0]["CET_ADLT_MOBILE_NO"] != DBNull.Value)
            {
                txtAdltMobile.Text = AESEncryption.Decrypt(dt.Rows[0]["CET_ADLT_MOBILE_NO"].ToString(), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256);
            }
            if (dt.Rows[0]["CET_NATIONALITY"] != DBNull.Value)
            {
                try
                {
                    cmbNationality.SelectedValue = dt.Rows[0]["CET_NATIONALITY"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
            if (dt.Rows[0]["CET_AADHAR_NO"] != DBNull.Value)
            {
                txtAADHAR.Text = AESEncryption.Decrypt(dt.Rows[0]["CET_AADHAR_NO"].ToString(), ENCRYPT_DECRYPT_KEY, "gtXr+h4M79xgU^?3", "SHA1", 2, "5Km#R3yZM-tsr*#p", 256);
            }
            if (dt.Rows[0]["CET_EMP_PLACE"] != DBNull.Value)
            {
                try
                {
                    cmbPlaceOfEmployment.SelectedValue = dt.Rows[0]["CET_EMP_PLACE"].ToString();
                }
                catch (Exception ex)
                {

                }
            }
            if (dt.Rows[0]["CET_RELAY_DATA"] != DBNull.Value)
            {
                try
                {
                    cmbRelayData.SelectedValue = dt.Rows[0]["CET_RELAY_DATA"].ToString();
                }
                catch (Exception ex)
                {

                }
            }

            Lblspno.Visible = true;
            TxtSpno.Visible = true;

            btnSaveProfile.Visible = false;
            //btnUpdateProfile.Visible = true;

            //---------------------------------------
            //btnUpdateProfile.Visible = true;

            //cmbCategory.Enabled = false;
            //Txtdeprt.Enabled = false;
            //TxtSpno.Enabled = false;
            //txtFName.Enabled = false;
            //txtLName.Enabled = false;
            //txtDOB.Enabled = false;
            //cmbSex.Enabled = false;
            ////ddlMedCentre.Enabled = false;
            //txtFatherName.Enabled = false;
            //txtHusName.Enabled = false;
            //txtIdentiFication.Enabled = false;
            //cmbAffirmative.Enabled = false;
            //cmbUniqID.Enabled = false;
            //txtUniqIDNo.Enabled = false;
            ////cmbWorkArea.Enabled = false;
            ///

            //cmbCategory.Enabled = false;
            Txtdeprt.Enabled = false;
            TxtSpno.Enabled = false;

            //if (cmbNationality.SelectedValue != "[Select]")
            //{
            //    cmbNationality.Enabled = false;
            //}

            //if (txtAADHAR.Text != "")
            //{
            //    txtAADHAR.Enabled = false;
            //}

            //if (txtPAN.Text != "")
            //{
            //    txtPAN.Enabled = false;
            //}
            ageaddressDiv.Visible = true;
        }
    }

    public void cat(string category)
    {
        if (category == WR || category == WA)
        {
            GetCategory(string.Format("'{0}','{1}'", WR_desc, WA_desc));
        }
        else if (category == DV || category == DA || category == DH)
        {
            GetCategory(string.Format("'{0}','{1}','{2}'", DV_desc, DA_desc, DH_desc));
        }
        else if (category == SV || category == SH || category == SF || category == SA)
        {
            GetCategory(string.Format("'{0}','{1}','{2}','{3}'", SV_desc, SH_desc, SF_desc, SA_desc));
        }
        else if (category == FM || category == FA)
        {
            GetCategory(string.Format("'{0}','{1}'", FM_desc, FA_desc));
        }
        else if (category == VC || category == VA)
        {
            GetCategory(string.Format("'{0}','{1}'", VC_desc, VA_desc));
        }
        else
        {
            string sqlCategory = t_Cemp_Type_Master() + "  where CTM_TYPE_CODE ='" + category + "'";
            DataTable dtCategory = new DataTable();
            dtCategory = getRecord(sqlCategory, con);
            cmbCategory.Items.Clear();

            if (dtCategory.Rows.Count > 0)
            {
                cmbCategory.DataSource = dtCategory;
                cmbCategory.DataTextField = "CTM_TYPE_DESC";
                cmbCategory.DataValueField = "CTM_TYPE_CODE";
                cmbCategory.DataBind();
            }
        }
    }

    public string emp_detail_qry()
    {
        string qry = "select CET_SAFETY_PASSNO,CET_REQUEST_NO,CET_LOCATION_CODE,CET_VENDOR_CODE,CET_CATEGORY, NVL((SELECT CTM_TYPE_DESC FROM HRACE.T_CEMP_TYPE_MASTER WHERE CTM_TYPE IN 'SPET' AND substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_VALUE IN CET_CATEGORY),(select CTM_TYPE_DESC from HRACE.t_cemp_type_master where  substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND CTM_TYPE_CODE IN CET_CATEGORY)) CET_CATEGORY_TYPE,CET_LOC_CODE";
        qry += ",CET_DEPT_CODE,CET_FIRSTNAME,CET_LASTNAME,CET_FATHER_NAME,cet_spouse_name,CET_GENDER,CET_EMERGENCY_NO,CET_PHONE_NO,CET_UNIQUE_ID_TYPE,";
        qry += "  CET_UNIQUE_ID_VALUE, CET_IDENTIFICATION_MARK ,CET_AREA_OF_WORK,to_char(CET_DOB,'dd/MM/yyyy') CET_DOB,CET_AGE,CET_AFFIRMATIVE ,";
        qry += "CET_PAN_NO,CET_ADLT_NAME,CET_ADLT_REL,CET_ADLT_ADDRESS,CET_ADLT_MOBILE_NO,CET_NATIONALITY,CET_AADHAR_NO,CET_EMP_PLACE,CET_RELAY_DATA,";
        qry += " (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_PROFILE_STATUS) CET_PROFILE_STATUS,";
        qry += " (select ctm_type_desc from HRACE.t_cemp_type_master where substr(CTM_TYPE_CODE,'-4','4')='" + comp_cd + "' AND ctm_type in 'STA' and ctm_value in CET_DOCVER_STATUS) CET_DOCVER_STATUS,";
        qry += "  CET_POLICE_VERIFICATION, CET_WO_VERIFICATION, CET_AGE_VERIFICATION, CET_ADDRESS_VERIFICATION, DECODE(CET_REQ_STATUS,'R','REJECTED','C','COMPLETED','IN PROGRESS') CET_REQ_STATUS,CET_UAN_NO,CET_IP_NO,CET_MEDICAL_CENTRE ";
        qry += " from HRACE.t_cemp_details_tmp where CET_REQUEST_NO='" + Session["requestnumber"] + "' ";
        return qry;
    }

    public string getSPNO(object reqNo)
    {
        string st = "NA";
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt1 = new DataTable();

        try
        {
            ls_sql = "select CET_SAFETY_PASSNO from t_cemp_details_tmp where CET_REQUEST_NO =:CET_REQUEST_NO";

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (cmd = new OracleCommand(ls_sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":CET_REQUEST_NO", reqNo));
                    dt1 = getRecord(cmd, con);

                    if (dt1.Rows.Count > 0)
                    {
                        st = dt1.Rows[0]["CET_SAFETY_PASSNO"].ToString();
                    }
                    else
                    {
                        st = "NA";
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }

        return st;
    }

    #endregion Profile  

    #region Address  
    public void clearAddress()
    {
        txtAddHouseNo.Text = "";
        txtAddMobile.Text = "";
        txtAddName.Text = "";
        txtAddPIN.Text = "";
        txtAddStreet.Text = "";
        txtLandLine.Text = "";
        txtAddEmail.Text = "";
        //lbladdattachname.Text = "";
        cmbAddCity.Items.Clear();
        cmbAddState.SelectedValue = "JH";
        cmbAddCountry.SelectedValue = "IND";
        GetCity(cmbAddState.SelectedValue);
        btnSubmit.Enabled = true;
        btnUpdate.Enabled = false;
        //lbladdattachname.Text = string.Empty;

        txtAddVillage.Text = "";
        txtAddPO.Text = "";
        txtAddThana.Text = "";
        cmbAddDistrict.Items.Clear();
        GetDistrict(cmbAddState.SelectedValue);
    }

    public void GetDistrict(string vStateCD)
    {
        string sqlDistrict;

        if (vStateCD == "0")
        {
            sqlDistrict = "select * from hrace.t_district_master ";
        }
        else
        {
            sqlDistrict = "select * from hrace.t_district_master where DST_STATE_CODE='" + vStateCD + "' order by DST_DISTRICT_NAME";
        }

        DataTable dtDistrict = new DataTable();
        dtDistrict = getRecord(sqlDistrict, con);
        cmbAddDistrict.Items.Clear();

        if (dtDistrict.Rows.Count > 0)
        {
            cmbAddDistrict.DataSource = dtDistrict;
            cmbAddDistrict.DataTextField = "DST_DISTRICT_NAME";
            cmbAddDistrict.DataValueField = "DST_DISTRICT_CODE";
            cmbAddDistrict.DataBind();
            cmbAddDistrict.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    public void GetAddressType()
    {
        string sqlAddressType = clmClass.get_CodeValue("ETYP");
        //sqlAddressType = sqlAddressType + ",CTM_TYPE_DESC"; // Not needed in C#
        DataTable dtAddressType = new DataTable();
        dtAddressType = getRecord(sqlAddressType, con);
        cmbAddressType.Items.Clear();

        if (dtAddressType.Rows.Count > 0)
        {
            cmbAddressType.DataSource = dtAddressType;
            cmbAddressType.DataTextField = "CTM_TYPE_DESC";
            cmbAddressType.DataValueField = "CTM_TYPE_CODE";
            cmbAddressType.DataBind();
            cmbAddressType.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    public void GetAddress(string vSPNo)
    {
        string sqlAddress = "SELECT CCA_CERT_NO,T1.CCA_ADDRESS_ID, T3.CMT_COUNTRY_NAME, T4.SMT_STATE_NAME, T5.CIT_CITY_NAME, T1.CCA_WORKMEN_TYPE EMP_TYPE, T1.CCA_ADDR_TYPE ADDRESS_TYPE, T2.CTM_TYPE_DESC ADDRESS_TYPE_DESC, T1.CCA_NAME CCA_NAME, T1.CCA_HOUSE_NO HOUSE_NO, T1.CCA_STREET STREET, T1.CCA_CITY CITY_CD, T1.CCA_STATE STATE_CD, T1.CCA_COUNTRY COUNTRY_CD, T1.CCA_PIN, T1.CCA_MOBILE, T1.CCA_EMAIL, T1.CCA_LAND_LINE, TO_CHAR(T1.CCA_START_DT, 'DD/MM/YYYY') CCA_START_DT, TO_CHAR(T1.CCA_END_DT, 'DD/MM/YYYY') CCA_END_DT, T1.CCA_REMARKS,nvl(T6.DM_NAME,' ') DM_NAME,T1.CCA_REQ_NO,CCA_REMARKS,CCA_CERT_NO, T1.CCA_VILLAGE, T1.CCA_PO, T1.CCA_THANA, T1.CCA_DISTRICT_CD , T7.DST_DISTRICT_NAME FROM T_CWM_CEMP_ADDRS_TMP T1, T_CEMP_TYPE_MASTER T2, T_COUNTRY_MASTER T3, T_STATE_MASTER T4 , T_CITY_MASTER T5,T_DOCUMENT_MASTER T6, hrace.t_district_master T7 WHERE T1.CCA_ADDR_TYPE = T2.CTM_TYPE_CODE AND T1.CCA_COUNTRY = T3.CMT_COUNTRY_CODE AND T1.CCA_COUNTRY = T4.SMT_COUNTRY_CODE AND  T1.CCA_STATE = T4.SMT_STATE_CODE AND T1.CCA_COUNTRY = T5.CIT_COUNTRY_CODE AND  T1.CCA_STATE = T5.CIT_STATE_CODE AND T1.CCA_CITY = T5.CIT_CITY_CODE AND  T1.CCA_SAFETY_PASS_NO = '" + vSPNo + "' AND T1.CCA_COMP_CD = '" + comp_cd + "' and T1.CCA_CERT_NO=T6.DM_DOC_ID(+) AND T1.CCA_COUNTRY = T7.DST_COUNTRY_CODE(+) AND  T1.CCA_STATE = T7.DST_STATE_CODE(+) AND T1.CCA_DISTRICT_CD = T7.DST_DISTRICT_CODE(+) order by T1.CCA_REQ_NO desc ";

        DataTable dtAddress = new DataTable();
        dtAddress = getRecord(sqlAddress, con);

        if (dtAddress.Rows.Count > 0)
        {
            btnSubmit.Visible = false;
            btnUpdate.Visible = true;
            btnContinue.Visible = true;
            gvAddress.DataSource = dtAddress;
            gvAddress.DataBind();
            pnlAddressDetail.Visible = true;

            if (dtAddress.Rows[0]["CCA_CERT_NO"].ToString() != "")
            {
                hddaddressold.Value = dtAddress.Rows[0]["CCA_CERT_NO"].ToString();
                imgaddressold.Visible = true;
                ChkoldAddress.Visible = true;
            }
            else
            {
                hddaddressold.Value = "";
                imgaddressold.Visible = false;
                ChkoldAddress.Visible = false;
            }
        }
        else
        {
            btnSubmit.Visible = true;
            btnUpdate.Visible = false;
            btnContinue.Visible = false;
            gvAddress.DataSource = null;
            gvAddress.DataBind();
            pnlAddressDetail.Visible = false;

            hddaddressold.Value = "";
            imgaddressold.Visible = false;
            ChkoldAddress.Visible = false;
        }
    }

    public void GetCountry()
    {
        string sqlCountry = "select * from T_COUNTRY_MASTER";
        DataTable dtCountry = new DataTable();
        dtCountry = getRecord(sqlCountry, con);
        cmbAddCountry.Items.Clear();
        if (dtCountry.Rows.Count > 0)
        {
            cmbAddCountry.DataSource = dtCountry;
            cmbAddCountry.DataTextField = "CMT_COUNTRY_NAME";
            cmbAddCountry.DataValueField = "CMT_COUNTRY_CODE";
            cmbAddCountry.DataBind();
            cmbAddCountry.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    public void GetState()
    {
        string sqlState = "select * from t_State_Master where SMT_STATE_CODE not in ('ALL','JHH1') order by SMT_STATE_NAME ";
        DataTable dtState = new DataTable();
        dtState = getRecord(sqlState, con);
        cmbAddState.Items.Clear();
        if (dtState.Rows.Count > 0)
        {
            cmbAddState.DataSource = dtState;
            cmbAddState.DataTextField = "SMT_STATE_NAME";
            cmbAddState.DataValueField = "SMT_STATE_CODE";
            cmbAddState.DataBind();
            cmbAddState.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    public void GetCity(string vStateCD)
    {
        string sqlCity;
        if (vStateCD == "0")
        {
            sqlCity = "select * from t_CITY_Master ";
        }
        else
        {
            sqlCity = "select * from t_CITY_Master where CIT_STATE_CODE='" + vStateCD + "' order by CIT_CITY_NAME";
        }

        DataTable dtCity = new DataTable();
        dtCity = getRecord(sqlCity, con);
        cmbAddCity.Items.Clear();
        if (dtCity.Rows.Count > 0)
        {
            cmbAddCity.DataSource = dtCity;
            cmbAddCity.DataTextField = "CIT_CITY_NAME";
            cmbAddCity.DataValueField = "CIT_CITY_CODE";
            cmbAddCity.DataBind();
            cmbAddCity.Items.Insert(0, new ListItem("[Select]", "0"));
        }
    }

    protected void cmbAddState_SelectedIndexChanged(object sender, EventArgs e)
    {
        string vStateCd = "";
        vStateCd = cmbAddState.SelectedValue;
        GetCity(vStateCd);

        GetDistrict(vStateCd);
    }

    public string CheckAddressMandatoryFields()
    {
        string vErrorCount = "NA";
        if (cmbAddressType.SelectedValue == "0")
        {
            vErrorCount = "Select Address Type";
        }

        if (txtAddName.Text == "")
        {
            vErrorCount = "Enter Name";
        }

        if (txtAddHouseNo.Text == "")
        {
            vErrorCount = "Enter House Number";
        }
        if (cmbAddCity.SelectedValue == "0")
        {
            vErrorCount = "Enter City Name";
        }

        if (cmbAddState.SelectedValue == "0")
        {
            vErrorCount = "Select State";
        }

        if (cmbAddCountry.SelectedValue == "0")
        {
            vErrorCount = "Select Country";
        }

        if (txtAddPIN.Text == "")
        {
            vErrorCount = "Enter PIN Number";
        }
        if (txtAddPIN.Text.Length < 6)
        {
            vErrorCount = "Enter a valid PIN Number";
        }

        if (txtAddVillage.Text == "")
        {
            vErrorCount = "Enter Village name";
        }

        if (txtAddPO.Text == "")
        {
            vErrorCount = "Enter Post Office name";
        }

        if (txtAddThana.Text == "")
        {
            vErrorCount = "Enter Thana name";
        }

        if (cmbAddDistrict.SelectedValue == "0")
        {
            vErrorCount = "Select District";
        }

        return vErrorCount;
    }


    public void updateAddress()
    {
        //address update starts
        string vAddressID = "";
        string sqlUpdAddress = "";
        string vAddressRemark = "";
        string vAddressdocid = "";

        string vSPNo = TxtSpno.Text.Trim().ToUpper();
        if (txtAddMobile.Text.Trim().Equals(""))
        {
            ShowMessage("Please enter mobile number of vendor");
            return;
        }
        if (txtAddEmail.Text.Trim().Equals(""))
        {
            ShowMessage("Please enter email id of vendor");
            return;
        }

        if (fupdl_add.HasFile)
        {
            string contentType = fupdl_add.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf"))
            {
                if (fupdl_add.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (fupdl_add.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        vAddressID = Session["AddressID"].ToString();
        vAddressRemark = Session["AddressRemark"].ToString();
        vAddressdocid = Session["Addressdocid"].ToString();

        if (cmbAddressType.SelectedValue == "0")
        {
            ShowMessage("Please Select Address Type");
            return;
        }
        if (cmbAddState.SelectedValue == "0")
        {
            ShowMessage("Please Select State");
            return;
        }

        if (cmbAddCountry.SelectedValue == "0")
        {
            ShowMessage("Please Select State");
            return;
        }

        try
        {
            if (fupdl_add.HasFile)
            {
                if (!fupdl_add.HasFile)
                {
                    // No action needed here
                }
                string filename = Path.GetFileName(fupdl_add.PostedFile.FileName);
                string contentType = fupdl_add.PostedFile.ContentType;
                if (contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf"))
                {
                    if (fupdl_add.PostedFile.ContentLength > 512000)
                    {
                        ShowMessage("Your file size is " + (fupdl_add.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB");
                        return;
                    }
                }
                else
                {
                    ShowMessage("Please Upload pdf file only");
                    return;
                }

                if (vAddressRemark == "O")
                {
                    vAddressdocid = GetID("seq_cemp_address");
                    OracleCommand cmdfileadd = new OracleCommand();
                    string ls_sql = string.Empty;
                    filename = Path.GetFileName(fupdl_add.PostedFile.FileName);
                    using (Stream fs = fupdl_add.PostedFile.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            byte[] bytes = br.ReadBytes((int)fs.Length);

                            ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                            if (con.State == ConnectionState.Closed)
                            {
                                con.Open();
                            }
                            cmdfileadd.CommandText = ls_sql;
                            cmdfileadd.Connection = con;
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_DOC_ID", vAddressdocid));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "ADD"));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileadd.ExecuteNonQuery();
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
                else
                {
                    string ls_sql = string.Empty;
                    OracleCommand cmdfileadd = new OracleCommand();
                    using (Stream fs = fupdl_add.PostedFile.InputStream)
                    {
                        using (BinaryReader br = new BinaryReader(fs))
                        {
                            byte[] bytes = br.ReadBytes((int)fs.Length);

                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }

                            ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID";
                            if (con.State == ConnectionState.Closed)
                            {
                                con.Open();
                            }
                            cmdfileadd.CommandText = ls_sql;
                            cmdfileadd.Connection = con;
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_DOC_ID", vAddressdocid));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                            cmdfileadd.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                            cmdfileadd.ExecuteNonQuery();
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }

            sqlUpdAddress = "UPDATE HRACE.T_CWM_CEMP_ADDRS_TMP SET ";
            sqlUpdAddress = sqlUpdAddress + "CCA_ADDR_TYPE ='" + cmbAddressType.SelectedValue + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_START_DT =" + "SYSDATE" + ",";
            sqlUpdAddress = sqlUpdAddress + "CCA_END_DT =" + "to_date('31/12/9999','DD/MM/YYYY')" + ",";
            sqlUpdAddress = sqlUpdAddress + "CCA_NAME ='" + txtAddName.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_HOUSE_NO ='" + txtAddHouseNo.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_STREET ='" + txtAddStreet.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_CITY ='" + cmbAddCity.SelectedValue + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_VILLAGE ='" + txtAddVillage.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_PO ='" + txtAddPO.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_THANA ='" + txtAddThana.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_DISTRICT_CD ='" + cmbAddDistrict.SelectedValue + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_STATE ='" + cmbAddState.SelectedValue + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_COUNTRY ='" + cmbAddCountry.SelectedValue + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_PIN ='" + txtAddPIN.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_MOBILE ='" + txtAddMobile.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_EMAIL ='" + txtAddEmail.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_LAND_LINE ='" + txtLandLine.Text.ToString().Trim() + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_MODIFIED_BY ='" + Session["VendCode"] + "',";
            sqlUpdAddress = sqlUpdAddress + "CCA_MODIFIED_DT =" + "SYSDATE";

            if (fupdl_add.HasFile && vAddressRemark == "O")
            {
                sqlUpdAddress = sqlUpdAddress + ",CCA_CERT_NO ='" + vAddressdocid.Trim() + "'";
            }

            sqlUpdAddress = sqlUpdAddress + " where CCA_SAFETY_PASS_NO = '" + vSPNo + "' and CCA_ADDR_TYPE ='" + cmbAddressType.SelectedValue + "' and CCA_REQ_NO='" + Session["requestnumber"] + "'";

            SaveData(sqlUpdAddress, con);
            updatedocstatus(Session["requestnumber"].ToString(), vSPNo, "AP");
            ShowMessage("Updated Sucessfully");
            address_details(vSPNo);
            btnUpdate.Visible = true;
            btnContinue.Visible = true;
            GetAddress(TxtSpno.Text);
            if (Session["reqtype"].ToString() == "Renew")
            {
                foreach (GridViewRow gvrow in gvAddress.Rows)
                {
                    CheckBox chkbox = (CheckBox)gvrow.FindControl("chkSelectAddress");
                    HiddenField reqno = (HiddenField)gvrow.FindControl("hdreqno");
                    if (reqno.Value.Trim() == Session["requestnumber"].ToString())
                    {
                        chkbox.Enabled = true;
                    }
                    else
                    {
                        chkbox.Enabled = false;
                    }
                }
            }
            clearAddress();
        }
        catch (Exception ex)
        {
            ShowMessage("Error While Updating Record");
        }

        //address update ends
    }
    public string emp_addrs_detail_qry(string spno)
    {
        string qry = " select CCA_ADDRESS_ID,CCA_SAFETY_PASS_NO,CCA_ADDR_TYPE,CCA_NAME, CCA_HOUSE_NO, CCA_STREET,CCA_CITY, CCA_STATE, CCA_COUNTRY, CCA_PIN, CCA_MOBILE, CCA_EMAIL, CCA_LAND_LINE, CCA_VILLAGE, CCA_PO, CCA_THANA, CCA_DISTRICT_CD ";
        qry += " from HRACE.T_CWM_CEMP_ADDRS_TMP where CCA_SAFETY_PASS_NO='" + spno + "' and CCA_REQ_NO='" + Session["requestnumber"] + "'";
        return qry;
    }

    public string GetID(string vSeqName)
    {
        string vSeqNo = "";
        string sqlSequence = "SELECT " + vSeqName + ".NEXTVAL  FROM DUAL";
        DataTable dtSequence = getRecord(sqlSequence, con);

        if (dtSequence.Rows.Count > 0)
        {
            vSeqNo = dtSequence.Rows[0][0].ToString();
        }

        return vSeqNo;
    }

    private void updatedocstatus(string reqno, string spno, string type)
    {
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt = new DataTable();

        try
        {
            ls_sql = "delete T_SP_DOC_VERIFICATION where SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_REQ_NO=:SDV_REQ_NO and SDV_VERF_TYPE=:SDV_VERF_TYPE";

            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (cmd = new OracleCommand(ls_sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":SDV_SAFETYPASS_NO", spno));
                    cmd.Parameters.Add(new OracleParameter(":SDV_REQ_NO", reqno));
                    cmd.Parameters.Add(new OracleParameter(":SDV_VERF_TYPE", type));
                    cmd.ExecuteNonQuery();

                    //'''''''check if any rejection is pending''''''''
                    ls_sql = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_FLAG='N'";
                    cmd.CommandText = ls_sql;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new OracleParameter(":SDV_SAFETYPASS_NO", spno));
                    cmd.Parameters.Add(new OracleParameter(":SDV_REQ_NO", reqno));
                    dt.Clear();
                    dt = getRecord(cmd, con);

                    if (dt.Rows.Count > 0)
                    {
                    }
                    else
                    {
                        ls_sql = "update t_cemp_details_tmp set CET_DOCVER_STATUS='I' where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO and CET_DOCVER_STATUS='R'";

                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }
                        cmd = new OracleCommand(ls_sql, con);
                        cmd.Parameters.Add(new OracleParameter(":CET_SAFETY_PASSNO", spno));
                        cmd.Parameters.Add(new OracleParameter(":CET_REQUEST_NO", reqno));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void address_details(string sp_no)
    {
        txtAddHouseNo.Text = "";
        txtAddMobile.Text = "";
        txtAddName.Text = "";
        txtAddPIN.Text = "";
        txtAddStreet.Text = "";
        txtAddVillage.Text = "";
        txtAddPO.Text = "";
        txtAddThana.Text = "";
        txtLandLine.Text = "";
        txtAddEmail.Text = "";
        cmbAddCity.Items.Clear();
        cmbAddDistrict.Items.Clear();

        cmbAddState.SelectedValue = "JH";
        cmbAddCountry.SelectedValue = "IND";
        GetCity(cmbAddState.SelectedValue);
        GetDistrict(cmbAddState.SelectedValue);

        string qry_add = emp_addrs_detail_qry(sp_no);
        DataTable dt_add = getRecord(qry_add, con);

        if (dt_add.Rows.Count > 0)
        {
            if (dt_add.Rows[0]["CCA_ADDR_TYPE"] != DBNull.Value)
            {
                try
                {
                    cmbAddressType.SelectedValue = dt_add.Rows[0]["CCA_ADDR_TYPE"].ToString();
                }
                catch (Exception ex)
                {
                    // Handle exception (optional)
                }
            }
            if (dt_add.Rows[0]["CCA_HOUSE_NO"] != DBNull.Value)
            {
                txtAddHouseNo.Text = dt_add.Rows[0]["CCA_HOUSE_NO"].ToString();
            }
            if (dt_add.Rows[0]["CCA_MOBILE"] != DBNull.Value)
            {
                txtAddMobile.Text = dt_add.Rows[0]["CCA_MOBILE"].ToString();
            }
            if (dt_add.Rows[0]["CCA_NAME"] != DBNull.Value)
            {
                txtAddName.Text = dt_add.Rows[0]["CCA_NAME"].ToString();
            }
            if (dt_add.Rows[0]["CCA_PIN"] != DBNull.Value)
            {
                txtAddPIN.Text = dt_add.Rows[0]["CCA_PIN"].ToString();
            }
            if (dt_add.Rows[0]["CCA_STREET"] != DBNull.Value)
            {
                txtAddStreet.Text = dt_add.Rows[0]["CCA_STREET"].ToString();
            }
            if (dt_add.Rows[0]["CCA_LAND_LINE"] != DBNull.Value)
            {
                txtLandLine.Text = dt_add.Rows[0]["CCA_LAND_LINE"].ToString();
            }
            if (dt_add.Rows[0]["CCA_EMAIL"] != DBNull.Value)
            {
                txtAddEmail.Text = dt_add.Rows[0]["CCA_EMAIL"].ToString();
            }
            if (dt_add.Rows[0]["CCA_CITY"] != DBNull.Value)
            {
                try
                {
                    cmbAddCity.SelectedValue = dt_add.Rows[0]["CCA_CITY"].ToString();
                }
                catch (Exception ex)
                {
                    // Handle exception (optional)
                }
            }
            if (dt_add.Rows[0]["CCA_STATE"] != DBNull.Value)
            {
                try
                {
                    cmbAddState.SelectedValue = dt_add.Rows[0]["CCA_STATE"].ToString();
                }
                catch (Exception ex)
                {
                    // Handle exception (optional)
                }
            }
            if (dt_add.Rows[0]["CCA_COUNTRY"] != DBNull.Value)
            {
                try
                {
                    cmbAddCountry.SelectedValue = dt_add.Rows[0]["CCA_COUNTRY"].ToString();
                }
                catch (Exception ex)
                {
                }
            }

            if (dt_add.Rows[0]["CCA_VILLAGE"] != DBNull.Value)
            {
                txtAddVillage.Text = dt_add.Rows[0]["CCA_VILLAGE"].ToString();
            }

            if (dt_add.Rows[0]["CCA_PO"] != DBNull.Value)
            {
                txtAddPO.Text = dt_add.Rows[0]["CCA_PO"].ToString();
            }

            if (dt_add.Rows[0]["CCA_THANA"] != DBNull.Value)
            {
                txtAddThana.Text = dt_add.Rows[0]["CCA_THANA"].ToString();
            }

            if (dt_add.Rows[0]["CCA_DISTRICT_CD"] != DBNull.Value)
            {
                try
                {
                    cmbAddDistrict.SelectedValue = dt_add.Rows[0]["CCA_DISTRICT_CD"].ToString();
                }
                catch (Exception ex)
                {
                }
            }

            btnSubmit.Visible = false;
        }
        else
        {
            clearAddress();
            btnSubmit.Visible = true;
            btnUpdate.Visible = false;
            btnContinue.Visible = false;
        }
    }

    protected void gvAddress_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox chkSelectedAddress = (CheckBox)e.Row.FindControl("chkSelectAddress");

            chkSelectedAddress.Checked = true;
            chkSelectAddress(chkSelectedAddress, EventArgs.Empty);
        }
    }

    protected void chkSelectAddress(object sender, EventArgs e)
    {
        bool vIsRowSelected = false;
        clearAddress();

        try
        {
            GridViewRow gvrow = (GridViewRow)((CheckBox)sender).Parent.Parent;

            CheckBox chkSelect = (CheckBox)gvrow.FindControl("chkSelectAddress");

            if (chkSelect.Enabled && chkSelect.Checked)
            {
                vIsRowSelected = true;
                string vAddressID = ((HiddenField)gvrow.FindControl("hidAddressID")).Value;
                Session["AddressID"] = vAddressID;

                string vAddressRemark = ((HiddenField)gvrow.FindControl("hidremark")).Value;
                Session["AddressRemark"] = vAddressRemark;

                string vAddressdocid = ((HiddenField)gvrow.FindControl("hiddocseq")).Value;
                Session["Addressdocid"] = vAddressdocid;

                string vAddressType = ((HiddenField)gvrow.FindControl("hidAddType")).Value;

                string vName = gvrow.Cells[2].Text.Trim().Replace("&nbsp;", "");
                string vHouseNo = gvrow.Cells[3].Text.Trim().Replace("&nbsp;", "");
                string vStreet = gvrow.Cells[4].Text.Trim().Replace("&nbsp;", "");
                string vCityCD = ((HiddenField)gvrow.FindControl("hidAddCity")).Value;

                string vVillage = gvrow.Cells[5].Text.Trim().Replace("&nbsp;", "");
                string vPO = gvrow.Cells[6].Text.Trim().Replace("&nbsp;", "");
                string vThana = gvrow.Cells[7].Text.Trim().Replace("&nbsp;", "");
                string vDistrictCD = ((HiddenField)gvrow.FindControl("hidAddDistrict")).Value;

                string vStateCD = ((HiddenField)gvrow.FindControl("hidAddState")).Value;
                string vCountryCD = ((HiddenField)gvrow.FindControl("hidAddCountry")).Value;

                string vPin = gvrow.Cells[12].Text.Trim().Replace("&nbsp;", "");
                string vMobile = gvrow.Cells[13].Text.Trim().Replace("&nbsp;", "");
                string vEmailID = gvrow.Cells[14].Text.Trim().Replace("&nbsp;", "");
                string vLandLine = gvrow.Cells[15].Text.Trim().Replace("&nbsp;", "");

                string filename = ((LinkButton)gvrow.FindControl("lnkexp")).Text;
                GetCity(vStateCD);

                cmbAddressType.SelectedValue = vAddressType;
                txtAddName.Text = vName;
                txtAddHouseNo.Text = vHouseNo;
                txtAddStreet.Text = vStreet;
                cmbAddCity.SelectedValue = vCityCD;
                cmbAddState.SelectedValue = vStateCD;
                cmbAddCountry.SelectedValue = vCountryCD;
                txtAddPIN.Text = vPin;
                txtAddMobile.Text = vMobile;
                txtAddEmail.Text = vEmailID;
                txtLandLine.Text = vLandLine;
                //lbladdattachname.Text = filename;

                GetDistrict(vStateCD);
                txtAddVillage.Text = vVillage;
                txtAddPO.Text = vPO;
                txtAddThana.Text = vThana;

                if (string.IsNullOrEmpty(vDistrictCD))
                {
                    cmbAddDistrict.SelectedValue = "0";
                }
                else
                {
                    cmbAddDistrict.SelectedValue = vDistrictCD;
                }

                btnUpdate.Enabled = true;
                string status = checkrenewaleligible(TxtSpno.Text.Trim(), Session["requestnumber"] as string);

                if (status.Equals("Y"))
                {
                    btnUpdate.Visible = false;
                    btnContinue.Visible = false;
                }
                else
                {
                    btnUpdate.Visible = true;
                    btnContinue.Visible = true;
                }

                btnSubmit.Visible = false;
            }
            else if (chkSelect.Enabled && !chkSelect.Checked)
            {
                //btnSubmit.Visible = true;
                //btnUpdate.Visible = false;
                clearAddress();
            }
        }
        catch (Exception ex)
        {
        }
    }

    protected void downloadadd(object sender, EventArgs e)
    {
        LinkButton ls_lnk = (LinkButton)sender;
        long id = Convert.ToInt64(ls_lnk.CommandArgument);
        byte[] bytes;
        string fileName, contentType;

        string sql = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID";

        // Use the property to get a new instance of the connection.
        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter(":DM_DOC_ID", id));

                using (OracleDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    bytes = (byte[])sdr["Dm_FILE_CONTENT"];
                    contentType = sdr["DM_FILE_TYPE"].ToString();
                    fileName = sdr["DM_NAME"].ToString();
                }
            }
        }

        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = contentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    #endregion Address

    #region AgeProof  

    protected void gvAge_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox chkSelectedAge = (CheckBox)e.Row.FindControl("chkSelectAge");

            chkSelectedAge.Checked = true;
            chkSelectAge(chkSelectedAge, EventArgs.Empty);
        }
    }
    protected void chkSelectAge(object sender, EventArgs e)
    {
        bool vIsRowSelected = false;

        try
        {
            GridViewRow gvrow = (GridViewRow)((CheckBox)sender).Parent.Parent;

            if (((CheckBox)gvrow.FindControl("chkSelectAge")).Checked)
            {
                vIsRowSelected = true;

                string vageID = ((HiddenField)gvrow.FindControl("hdage")).Value;
                string vdrvID = ((HiddenField)gvrow.FindControl("hddrv")).Value;
                string vpassID = ((HiddenField)gvrow.FindControl("hdpass")).Value;
                string dobfile = ((LinkButton)gvrow.FindControl("lnkdownloadage")).Text;
                string drvfile = ((LinkButton)gvrow.FindControl("lnkdownloaddrv")).Text;
                string passfile = ((LinkButton)gvrow.FindControl("lnkdownloadpass")).Text;

                hiddob.Value = vageID;
                hiddrv.Value = vdrvID;
                hidpass.Value = vpassID;
                //lbl_dobfile.Text = dobfile;
                //lbl_drvfile.Text = drvfile;
                //lbl_passfile.Text = passfile;

                string status = checkrenewaleligible(TxtSpno.Text.Trim(), Session["requestnumber"] as string);

                if (status.Equals("Y"))
                {
                    btnUpdate.Visible = false;
                    btnContinue.Visible = false;
                }
                else
                {
                    btnUpdate.Visible = true;
                    btnContinue.Visible = true;
                }
            }
            else
            {
                clearagedrv();
                //btnUpdate.Visible = false;
            }
        }
        catch (Exception ex)
        {
        }
    }

    public void updateage()
    {
        string agestatus = "N";
        string drvstatus = "N";
        string passstatus = "N";

        if (fupdlage.HasFile)
        {
            string contentType = fupdlage.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1).Equals("pdf"))
            {
                if (fupdlage.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (fupdlage.PostedFile.ContentLength / 1024.0).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        if (fupdldrv.HasFile)
        {
            string contentType = fupdldrv.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1).Equals("pdf"))
            {
                if (fupdldrv.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (fupdldrv.PostedFile.ContentLength / 1024.0).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        if (fupdlpass.HasFile)
        {
            string contentType = fupdlpass.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1).Equals("pdf"))
            {
                if (fupdlpass.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (fupdlpass.PostedFile.ContentLength / 1024.0).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        if (fupdlage.HasFile)
        {
            agestatus = "Y";
            string filename = Path.GetFileName(fupdlage.PostedFile.FileName);

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (OracleCommand cmdfileage = new OracleCommand())
                {
                    cmdfileage.Connection = con;
                    cmdfileage.CommandText = "update T_DOCUMENT_MASTER Set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID";
                    cmdfileage.Parameters.Add(new OracleParameter(":DM_DOC_ID", hiddob.Value));
                    cmdfileage.Parameters.Add(new OracleParameter(":DM_NAME", filename));

                    byte[] bytes;
                    using (Stream fs = fupdlage.PostedFile.InputStream)
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                    }

                    cmdfileage.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                    cmdfileage.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                    cmdfileage.ExecuteNonQuery();
                }
            }
        }

        if (fupdldrv.HasFile)
        {
            drvstatus = "Y";
            string filename = Path.GetFileName(fupdldrv.PostedFile.FileName);

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (OracleCommand cmdfiledrv = new OracleCommand())
                {
                    string ls_sql = string.Empty;
                    cmdfiledrv.Connection = con;

                    byte[] bytes;
                    using (Stream fs = fupdldrv.PostedFile.InputStream)
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                    }

                    if (string.IsNullOrEmpty(hiddrv.Value) || hiddrv.Value.Trim() == "0")
                    {
                        hiddrv.Value = TrnCWEAgeDrvSeqNo("");
                        ls_sql = "update T_CEMP_DETAILS_TMP set CET_DRV_CERT_NO=:CET_DRV_CERT_NO where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO";
                        cmdfiledrv.CommandText = ls_sql;
                        cmdfiledrv.Parameters.Clear();
                        cmdfiledrv.Parameters.Add(new OracleParameter(":CET_DRV_CERT_NO", hiddrv.Value));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim()));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":CET_REQUEST_NO", Session["requestnumber"]));
                        cmdfiledrv.ExecuteNonQuery();

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                        cmdfiledrv.CommandText = ls_sql;
                        cmdfiledrv.Parameters.Clear();
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_DOC_ID", hiddrv.Value));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "DRV"));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                        cmdfiledrv.ExecuteNonQuery();
                    }
                    else
                    {
                        ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID";
                        cmdfiledrv.CommandText = ls_sql;
                        cmdfiledrv.Parameters.Clear();
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_DOC_ID", hiddrv.Value));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                        cmdfiledrv.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                        cmdfiledrv.ExecuteNonQuery();
                    }
                }
            }
        }

        if (fupdlpass.HasFile)
        {
            passstatus = "Y";
            string filename = Path.GetFileName(fupdlpass.PostedFile.FileName);

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (OracleCommand cmdfilepass = new OracleCommand())
                {
                    string ls_sql = string.Empty;
                    cmdfilepass.Connection = con;

                    byte[] bytes;
                    using (Stream fs = fupdlpass.PostedFile.InputStream)
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        bytes = br.ReadBytes((int)fs.Length);
                    }

                    if (string.IsNullOrEmpty(hidpass.Value) || hidpass.Value.Trim() == "0")
                    {
                        hidpass.Value = TrnCWEAgeDrvSeqNo("");
                        ls_sql = "update T_CEMP_DETAILS_TMP set CET_PASS_CERT_NO=:CET_PASS_CERT_NO where CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and CET_REQUEST_NO=:CET_REQUEST_NO";
                        cmdfilepass.CommandText = ls_sql;
                        cmdfilepass.Parameters.Clear();
                        cmdfilepass.Parameters.Add(new OracleParameter(":CET_PASS_CERT_NO", hidpass.Value));
                        cmdfilepass.Parameters.Add(new OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim()));
                        cmdfilepass.Parameters.Add(new OracleParameter(":CET_REQUEST_NO", Session["requestnumber"]));
                        cmdfilepass.ExecuteNonQuery();

                        ls_sql = "insert into T_DOCUMENT_MASTER(DM_DOC_ID,DM_NAME,DM_FILE_TYPE,DM_FILE_CONTENT,DM_PROJECT,DM_MODULE,DM_COMP_CODE,DM_CREATED_BY,DM_CREATED_DATE,DM_MODIFIED_BY,DM_MODIFIED_DATE)VALUES(:DM_DOC_ID,:DM_NAME,:DM_FILE_TYPE,:DM_FILE_CONTENT,:DM_PROJECT,:DM_MODULE,:DM_COMP_CODE,:DM_CREATED_BY,sysdate,:DM_MODIFIED_BY,sysdate) ";
                        cmdfilepass.CommandText = ls_sql;
                        cmdfilepass.Parameters.Clear();
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_DOC_ID", hidpass.Value));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_FILE_TYPE", "PASS"));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_PROJECT", "CWM"));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_MODULE", "VPSS"));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_COMP_CODE", Session["Comp_code"]));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_CREATED_BY", Session["VendCode"]));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                        cmdfilepass.ExecuteNonQuery();
                    }
                    else
                    {
                        ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID";
                        cmdfilepass.CommandText = ls_sql;
                        cmdfilepass.Parameters.Clear();
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_DOC_ID", hidpass.Value));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                        cmdfilepass.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                        cmdfilepass.ExecuteNonQuery();
                    }
                }
            }
        }

        if (agestatus == "Y")
        {
            string ls_chkage = string.Empty;
            OracleCommand cmd_chkage;
            DataTable dt_chkage = new DataTable();

            try
            {
                ls_chkage = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='AG' and SDV_VERF_FLAG='N'";

                // Use the property to get a new instance of the connection.
                using (OracleConnection con = new OracleConnection(strConn))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (cmd_chkage = new OracleCommand(ls_chkage, con))
                    {
                        cmd_chkage.Parameters.Add(new OracleParameter(":SDV_REQ_NO", Session["requestnumber"]));
                        cmd_chkage.Parameters.Add(new OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim()));
                        dt_chkage = getRecord(cmd_chkage, con);

                        if (dt_chkage.Rows.Count > 0)
                        {
                            updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "AG");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (optional)
            }
        }

        if (drvstatus == "Y")
        {
            string ls_chkDL = string.Empty;
            OracleCommand cmd_chkDL;
            DataTable dt_chkDL = new DataTable();

            try
            {
                ls_chkDL = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='DL' and SDV_VERF_FLAG='N'";

                // Use the property to get a new instance of the connection.
                using (OracleConnection con = new OracleConnection(strConn))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (cmd_chkDL = new OracleCommand(ls_chkDL, con))
                    {
                        cmd_chkDL.Parameters.Add(new OracleParameter(":SDV_REQ_NO", Session["requestnumber"]));
                        cmd_chkDL.Parameters.Add(new OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim()));
                        dt_chkDL = getRecord(cmd_chkDL, con);

                        if (dt_chkDL.Rows.Count > 0)
                        {
                            updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "DL");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (optional)
            }
        }

        if (passstatus == "Y")
        {
            string ls_chkPA = string.Empty;
            OracleCommand cmd_chkPA;
            DataTable dt_chkPA = new DataTable();

            try
            {
                ls_chkPA = "select SDV_SAFETYPASS_NO from hrace.t_sp_doc_verification where SDV_REQ_NO=:SDV_REQ_NO and SDV_SAFETYPASS_NO=:SDV_SAFETYPASS_NO and SDV_VERF_TYPE='PA' and SDV_VERF_FLAG='N'";

                // Use the property to get a new instance of the connection.
                using (OracleConnection con = new OracleConnection(strConn))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    using (cmd_chkPA = new OracleCommand(ls_chkPA, con))
                    {
                        cmd_chkPA.Parameters.Add(new OracleParameter(":SDV_REQ_NO", Session["requestnumber"]));
                        cmd_chkPA.Parameters.Add(new OracleParameter(":SDV_SAFETYPASS_NO", TxtSpno.Text.Trim()));
                        dt_chkPA = getRecord(cmd_chkPA, con);

                        if (dt_chkPA.Rows.Count > 0)
                        {
                            updatedocstatus(Session["requestnumber"].ToString(), TxtSpno.Text.Trim(), "PA");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception (optional)
            }
        }

        btnUpdate.Enabled = false;
        getagedrv(TxtSpno.Text.Trim());

        if (Session["reqtype"].ToString() == "Renew")
        {
            foreach (GridViewRow gvrow in grdage.Rows)
            {
                CheckBox chkbox = (CheckBox)gvrow.FindControl("chkSelectage");
                HiddenField reqno = (HiddenField)gvrow.FindControl("hdreqno");

                if (reqno.Value.Trim() == Session["requestnumber"].ToString())
                {
                    chkbox.Enabled = true;
                }
                else
                {
                    chkbox.Enabled = false;
                }
            }
        }

        clearagedrv();
    }

    private void clearagedrv()
    {
        //lbl_dobfile.Text = "";
        //lbl_drvfile.Text = "";
        //lbl_passfile.Text = "";
        hiddob.Value = "";
        hiddrv.Value = "";
        hidpass.Value = "";
    }

    public string checkrenewaleligible(string spno, string reqno)
    {
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt = new DataTable();
        string status = "N";

        try
        {
            ls_sql = "select CET_SAFETY_PASSNO from t_cemp_details_TMP where CET_SAFETY_PASSNO='" + spno + "' and CET_REQUEST_NO='" + reqno + "' and CET_PV_ISSUED_ON is not null and CET_PV_VALID_TILL is not null and CET_DOCVER_STATUS not in('R','I')";

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (cmd = new OracleCommand(ls_sql, con))
                {
                    dt = getRecord(cmd, con);

                    if (dt.Rows.Count > 0)
                    {
                        status = "Y";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exception (optional)
        }

        return status;
    }

    protected void getagedrv(string spno)
    {
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt = new DataTable();

        try
        {
            ls_sql = "select b.DM_NAME DOB,b.DM_DOC_ID DOBDOCID,c.DM_NAME DRV,d.DM_NAME PASS,c.DM_DOC_ID DRVDOCID,d.DM_DOC_ID PASSDOCID,a.CET_REQUEST_NO from t_cemp_details_tmp a,t_document_master b,t_document_master c,t_document_master d where b.DM_DOC_ID=a.CET_DOB_CERT_NO and c.DM_DOC_ID(+)=a.CET_DRV_CERT_NO and d.DM_DOC_ID(+)=a.CET_PASS_CERT_NO and a.CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO ORDER BY TO_NUMBER(CET_REQUEST_NO) DESC";

            // Use the property to get a new instance of the connection.
            using (OracleConnection con = new OracleConnection(strConn))
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                using (cmd = new OracleCommand(ls_sql, con))
                {
                    cmd.Parameters.Add(new OracleParameter(":CET_SAFETY_PASSNO", TxtSpno.Text.Trim()));
                    dt = getRecord(cmd, con);

                    if (dt.Rows.Count > 0)
                    {
                        grdage.DataSource = dt;
                        grdage.DataBind();

                        if (dt.Rows[0]["DOBDOCID"].ToString() != "")
                        {
                            hdfageold.Value = dt.Rows[0]["DOBDOCID"].ToString().Trim();
                            imbageold.Visible = true;
                            chkageold.Visible = true;
                        }
                        else
                        {
                            hdfageold.Value = "";
                            imbageold.Visible = false;
                            chkageold.Visible = false;
                        }

                        if (dt.Rows[0]["DRVDOCID"].ToString() != "")
                        {
                            hdfdriverold.Value = dt.Rows[0]["DRVDOCID"].ToString().Trim();
                            imbdriverold.Visible = true;
                            chkdriverold.Visible = true;
                        }
                        else
                        {
                            hdfdriverold.Value = "";
                            imbdriverold.Visible = false;
                            chkdriverold.Visible = false;
                        }

                        if (dt.Rows[0]["PASSDOCID"].ToString() != "")
                        {
                            hdfpassold.Value = dt.Rows[0]["PASSDOCID"].ToString().Trim();
                            imgpassold.Visible = true;
                            chkpassold.Visible = true;
                        }
                        else
                        {
                            hdfpassold.Value = "";
                            imgpassold.Visible = false;
                            chkpassold.Visible = false;
                        }

                        string status = "N";
                        foreach (GridViewRow gvrow in grdage.Rows)
                        {
                            CheckBox chkbox = (CheckBox)gvrow.FindControl("chkSelectage");
                            HiddenField reqno = (HiddenField)gvrow.FindControl("hdreqno");

                            if (reqno.Value.Trim() == Session["requestnumber"].ToString())
                            {
                                status = "Y";
                            }
                        }

                        if (status == "Y")
                        {
                            hdfpassold.Value = "";
                            imgpassold.Visible = false;
                            chkpassold.Visible = false;

                            hdfdriverold.Value = "";
                            imbdriverold.Visible = false;
                            chkdriverold.Visible = false;

                            hdfageold.Value = "";
                            imbageold.Visible = false;
                            chkageold.Visible = false;
                        }

                        btnUpdate.Visible = true;
                        btnSubmit.Visible = false;
                        btnContinue.Visible = true;
                    }
                    else
                    {
                        grdage.DataSource = null;
                        grdage.DataBind();

                        hdfageold.Value = "";
                        imbageold.Visible = false;
                        chkageold.Visible = false;

                        hdfdriverold.Value = "";
                        imbdriverold.Visible = false;
                        chkdriverold.Visible = false;

                        hdfpassold.Value = "";
                        imgpassold.Visible = false;
                        chkpassold.Visible = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exception (optional)
        }
    }

    public string TrnCWEAgeDrvSeqNo(string id)
    {
        string vageSeqNo = "";
        string sqlageSeqNo = "Select (HRACE.SEQ_CEMP_AGE_DRV_ID.nextval) SEQNO from dual ";
        DataTable dtageSeqNo = new DataTable();
        dtageSeqNo = getRecord(sqlageSeqNo, con);

        if (dtageSeqNo.Rows.Count > 0)
        {
            vageSeqNo = dtageSeqNo.Rows[0]["SEQNO"].ToString();
        }

        dtageSeqNo.Dispose();
        return vageSeqNo;
    }

    protected void downloadage(object sender, EventArgs e)
    {
        LinkButton ls_lnk = (LinkButton)sender;
        long id = Convert.ToInt64(ls_lnk.CommandArgument);
        byte[] bytes;
        string fileName, contentType;

        string sql = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='AGE'";

        // Use the property to get a new instance of the connection.
        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter(":DM_DOC_ID", id));

                using (OracleDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    bytes = (byte[])sdr["Dm_FILE_CONTENT"];
                    contentType = sdr["DM_FILE_TYPE"].ToString();
                    fileName = sdr["DM_NAME"].ToString();
                }
            }
        }

        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = contentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    protected void downloaddrv(object sender, EventArgs e)
    {
        LinkButton ls_lnk = (LinkButton)sender;
        long id = Convert.ToInt64(ls_lnk.CommandArgument);
        byte[] bytes;
        string fileName, contentType;

        string sql = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='DRV'";

        // Use the property to get a new instance of the connection.
        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter(":DM_DOC_ID", id));

                using (OracleDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    bytes = (byte[])sdr["Dm_FILE_CONTENT"];
                    contentType = sdr["DM_FILE_TYPE"].ToString();
                    fileName = sdr["DM_NAME"].ToString();
                }
            }
        }

        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = contentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    protected void downloadpass(object sender, EventArgs e)
    {
        LinkButton ls_lnk = (LinkButton)sender;
        long id = Convert.ToInt64(ls_lnk.CommandArgument);
        byte[] bytes;
        string fileName, contentType;

        string sql = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='PASS'";

        // Use the property to get a new instance of the connection.
        using (OracleConnection con = new OracleConnection(strConn))
        {
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            using (OracleCommand cmd = new OracleCommand(sql, con))
            {
                cmd.Parameters.Add(new OracleParameter(":DM_DOC_ID", id));

                using (OracleDataReader sdr = cmd.ExecuteReader())
                {
                    sdr.Read();
                    bytes = (byte[])sdr["Dm_FILE_CONTENT"];
                    contentType = sdr["DM_FILE_TYPE"].ToString();
                    fileName = sdr["DM_NAME"].ToString();
                }
            }
        }

        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = contentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    #endregion AgeProof

    #region PV

    //protected void txt_frmdt_TextChanged(object sender, EventArgs e)
    //{
    //    string ls_sql = string.Empty;
    //    DataTable dt = new DataTable();
    //    OracleCommand cmd;

    //        if (txt_frmdt.Text.Trim() == "")
    //        {

    //        }
    //        else
    //        {
    //        futureDate(txt_frmdt.Text);
    //        }        
    //}

    protected void gvPV_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckBox chkSelectedPV = (CheckBox)e.Row.FindControl("grdchk");

            chkSelectedPV.Checked = true;
            chkSelectPV(chkSelectedPV, EventArgs.Empty);
        }
    }

    protected void futureDate(string Getdate)
    {
        if (txt_frmdt.Text != "")
        {
            string db = Getdate.Replace("-", "/");
            DateTime originalDate = DateTime.ParseExact(db, "yyyy/MM/dd", CultureInfo.InvariantCulture);
            db = originalDate.ToString("dd/MM/yyyy").Replace("-", "/"); ;
            Getdate = db;

            frmProfileCreation instance = new frmProfileCreation();
            string PVvalue = "";
            DataTable dtPV = get_codetype("PVV", comp_cd);
            if (dtPV.Rows.Count > 0)
            {
                PVvalue = dtPV.Rows[0]["ctm_value"].ToString();
            }


            string dateResult = "";
            using (OracleConnection con = new OracleConnection(strConn))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = con;
                    cmd.CommandText = "SELECT TO_CHAR(ADD_MONTHS( TO_DATE(:Getdate,'DD/MM/YYYY'), :PVvalue ) - 1,'DD/MM/YYYY') as dateResult FROM DUAL";
                    cmd.CommandType = CommandType.Text;

                    cmd.Parameters.Add(new OracleParameter("Getdate", Getdate));
                    cmd.Parameters.Add(new OracleParameter("PVvalue", int.Parse(PVvalue)));


                    con.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();
                            dateResult = reader["dateResult"].ToString();
                            txt_todt.Text = dateResult;
                            string db1 = txt_todt.Text.Replace("-", "/");
                            DateTime originalDate1 = DateTime.ParseExact(db1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            db1 = originalDate1.ToString("yyyy/MM/dd").Replace("/", "-"); ;
                            txt_todt.Text = db1;
                        }
                    }
                }
            }
        }
    }

    public DataTable get_codetype(string ctm_type, string comp_code)
    {
        string query = "select ctm_seq,KK.CTM_TYPE_CODE,KK.CTM_TYPE_DESC,kk.CTM_VALUE,kk.ctm_remarks  from t_cemp_type_master kk";
        string query1 = query + " WHERE KK.CTM_TYPE IN ('" + ctm_type + "') AND substr(CTM_TYPE_CODE,'-4','4')='" + comp_code + "' and CTM_STATUS='A' order by ctm_seq";

        DataTable dtTable = getRecord(query1, con);
        return dtTable;
    }

    private void showpv(string spno)
    {
        string ls_sql = string.Empty;

        ls_sql = "select A.CPDT_SAFETY_PASS_NO,d.CED_FIRSTNAME||' '||d.CED_LASTNAME Name ,to_char(A.CPDT_ST_DT,'dd/mm/yyyy') stdt,to_char(A.CPDT_END_DT,'dd/mm/yyyy') enddt,to_char(A.CPDT_CREATED_DT,'dd/mm/yyyy') crtdt,decode(B.SDV_VERF_FLAG,'S','Submitted','Y','Approved','N','Returned') st,nvl(b.SDV_REMARKS,'N/A') SDV_REMARKS, A.CPDT_PV_ID pvid,C.DM_DOC_ID,C.DM_NAME,decode(A.CPDT_DOC_TYPE,'pv','Police Verification/Undertaken','passport','Passport',NULL,'Not Specified') typefile,A.CPDT_DOC_TYPE from HRACE.T_CWM_PV_DTL_TMP a,HRACE.T_SP_DOC_VERIFICATION b,HRACE.T_DOCUMENT_MASTER c,hrace.t_cemp_details d  where A.CPDT_SAFETY_PASS_NO=B.SDV_SAFETYPASS_NO  and A.CPDT_PV_ID=substr(B.SDV_REQ_NO,3,length(B.SDV_REQ_NO))  and  B.SDV_VERF_TYPE='PV' and A.CPDT_CERT_NO=C.DM_DOC_ID and C.DM_FILE_TYPE='PV' and B.SDV_VERF_FLAG in('S','N') and (CPDT_CREATED_BY=:CPDT_CREATED_BY or CPDT_MODIFIED_BY=:CPDT_CREATED_BY) and CPDT_COMP_CODE=:CPDT_COMP_CODE and A.CPDT_SAFETY_PASS_NO=d.CED_SAFETY_PASS_NO and  A.CPDT_SAFETY_PASS_NO=:spNo ";

        OracleCommand cmd = new OracleCommand(ls_sql, con);
        cmd.Parameters.Add(new OracleParameter(":CPDT_COMP_CODE", comp_cd));
        cmd.Parameters.Add(new OracleParameter(":CPDT_CREATED_BY", vVencode));
        cmd.Parameters.Add(new OracleParameter(":spNo", spno));

        DataTable dt1 = getRecord(cmd, con);

        if (dt1.Rows.Count > 0)
        {
            grdpv.DataSource = dt1;
            grdpv.DataBind();
        }
        else
        {
            grdpv.DataSource = null;
            grdpv.DataBind();
        }
    }


    protected void chkSelectPV(object sender, EventArgs e)
    {
        clearpv();
        bool vIsRowSelected = false;

        try
        {
            GridViewRow gvrow = (GridViewRow)((CheckBox)sender).Parent.Parent;

            if (((CheckBox)gvrow.FindControl("grdchk")).Checked)
            {
                vIsRowSelected = true;
                string vPVID = ((HiddenField)gvrow.FindControl("grdpvid")).Value;
                Session["PVID"] = vPVID;

                string vValidFrom = gvrow.Cells[3].Text;
                string vValidTo = gvrow.Cells[4].Text;
                string vspno = gvrow.Cells[1].Text;
                string vcertname = ((LinkButton)gvrow.FindControl("lnkdownloadpv")).Text;
                string vcertid = ((HiddenField)gvrow.FindControl("hidpvcerno")).Value;
                string type = ((HiddenField)gvrow.FindControl("grddoctype")).Value;
                hidcertid.Value = vcertid;


                lbl_filename.Text = vcertname;
                //getbasicdetails(vspno);                
                btnUpdate.Enabled = true;
                btnUpdate.Visible = true;
                btnContinue.Visible = true;

                txt_frmdt.Text = vValidFrom;
                string db = txt_frmdt.Text.Replace("-", "/");
                DateTime originalDate = DateTime.ParseExact(db, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                db = originalDate.ToString("yyyy/MM/dd").Replace("/", "-"); ;
                txt_frmdt.Text = db;

                txt_todt.Text = vValidTo;
                string db1 = txt_todt.Text.Replace("-", "/");
                DateTime originalDate1 = DateTime.ParseExact(db1, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                db1 = originalDate1.ToString("yyyy/MM/dd").Replace("/", "-"); ;
                txt_todt.Text = db1;
            }
            else
            {
                hidcertid.Value = string.Empty;
                txt_frmdt.Text = "";
                txt_todt.Text = "";
                lbl_filename.Text = string.Empty;
                //btnUpdate.Enabled = false;
                //showpv(Session["spno"].ToString());
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void clearpv()
    {
        txt_frmdt.Text = string.Empty;
        txt_todt.Text = string.Empty;
        lbl_filename.Text = "";
        hidcertid.Value = "";
    }

    protected void downloadpv(object sender, EventArgs e)
    {
        LinkButton ls_lnk = (LinkButton)sender;
        long id = Convert.ToInt64(ls_lnk.CommandArgument);
        byte[] bytes;
        string fileName, contentType;

        using (OracleCommand cmd = new OracleCommand())
        {
            cmd.CommandText = "select DM_NAME,DM_DOC_ID,DM_FILE_CONTENT,DM_FILE_TYPE from T_DOCUMENT_MASTER where DM_DOC_ID=:DM_DOC_ID and DM_FILE_TYPE='PV'";
            cmd.Parameters.AddWithValue(":DM_DOC_ID", id);
            cmd.Connection = con;

            con.Open();

            using (OracleDataReader sdr = cmd.ExecuteReader())
            {
                sdr.Read();
                bytes = (byte[])sdr["Dm_FILE_CONTENT"];
                contentType = sdr["DM_FILE_TYPE"].ToString();
                fileName = sdr["DM_NAME"].ToString();
            }

            con.Close();
        }

        Response.Clear();
        Response.Buffer = true;
        Response.Charset = "";
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.ContentType = contentType;
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
        Response.BinaryWrite(bytes);
        Response.Flush();
        Response.End();
    }

    private void getbasicdetails(string spno)
    {
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt = new DataTable();

        ls_sql = "select CED_FIRSTNAME||' '||CED_LASTNAME as name,CED_UNIQUE_ID_VALUE as idval,nvl(to_char(CED_PV_VALID_TILL,'dd/mm/yyyy'),'N/A') as PV,CED_CATEGORY as Category from hrace.t_cemp_details where CED_SAFETY_PASS_NO=:CED_SAFETY_PASS_NO and CED_COMPANY_CODE=:CED_COMPANY_CODE";

        if (con.State == ConnectionState.Closed)
        {
            con.Open();
        }

        cmd = new OracleCommand(ls_sql, con);
        cmd.Parameters.Add(new OracleParameter(":CED_SAFETY_PASS_NO", spno.Trim()));
        cmd.Parameters.Add(new OracleParameter(":CED_COMPANY_CODE", comp_cd.Trim()));
        dt = getRecord(cmd, con);

        if (dt.Rows.Count > 0)
        {
            hidcat.Value = dt.Rows[0]["Category"].ToString();
        }
        else
        {
            ls_sql = "select a.CET_FIRSTNAME||' '||a.CET_LASTNAME as name,a.CET_UNIQUE_ID_VALUE as idval,nvl(to_char(a.CET_PV_VALID_TILL,'dd/mm/yyyy'),'N/A') as PV,a.CET_CATEGORY as Category from hrace.t_cemp_details_tmp a where a.CET_SAFETY_PASSNO=:CET_SAFETY_PASSNO and a.CET_REQUEST_NO=(select max(CET_REQUEST_NO) from hrace.t_cemp_details_tmp where CET_SAFETY_PASSNO=a.CET_SAFETY_PASSNO and CET_LOCATION_CODE=a.CET_LOCATION_CODE) and CET_LOCATION_CODE=:CET_LOCATION_CODE";

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            cmd = new OracleCommand(ls_sql, con);
            cmd.Parameters.Add(new OracleParameter(":CET_SAFETY_PASSNO", spno.Trim()));
            cmd.Parameters.Add(new OracleParameter(":CET_LOCATION_CODE", comp_cd.Trim()));
            dt = getRecord(cmd, con);

            if (dt.Rows.Count > 0)
            {
                hidcat.Value = dt.Rows[0]["Category"].ToString();
            }
            else
            {
                ShowMessage("Sorry No Record Found!!");
                return;
            }
        }
    }

    public string TrnCWEPVCertNo(string id)
    {
        string vPVSeqCertNo = "";
        //string sqlPVSeqCertNo = "Select (HRACE.seq_cemp_pv_docid.nextval) SEQNO from dual";
        string sqlPVSeqCertNo = "SELECT (MAX(DM_DOC_ID) +1) SEQNO FROM T_DOCUMENT_MASTER";
        DataTable dtPVSeqCertNo = new DataTable();
        dtPVSeqCertNo = getRecord(sqlPVSeqCertNo, con);
        if (dtPVSeqCertNo.Rows.Count > 0)
        {
            vPVSeqCertNo = dtPVSeqCertNo.Rows[0]["SEQNO"].ToString();
        }

        dtPVSeqCertNo.Dispose();
        return vPVSeqCertNo;
    }

    public string TrnCWEPVSeqNo(string id)
    {
        string vPVSeqNo = "";
        string sqlPVSeqNo = "Select (HRACE.SEQ_CEMP_PV.nextval) SEQNO from dual";
        DataTable dtPVSeqNo = new DataTable();
        dtPVSeqNo = getRecord(sqlPVSeqNo, con);
        if (dtPVSeqNo.Rows.Count > 0)
        {
            vPVSeqNo = dtPVSeqNo.Rows[0]["SEQNO"].ToString();
        }

        dtPVSeqNo.Dispose();
        return vPVSeqNo;
    }

    private void updatePV()
    {
        string ls_sql = string.Empty;
        OracleCommand cmd;
        DataTable dt = new DataTable();

        if (txt_frmdt.Text.Trim() == "")
        {
            ShowMessage("Please enter valid from date");
            return;
        }

        string db = txt_frmdt.Text.Replace("-", "/");
        DateTime originalDate = DateTime.ParseExact(db, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        db = originalDate.ToString("dd/MM/yyyy").Replace("-", "/"); ;
        txt_frmdt.Text = db;

        string db1 = txt_todt.Text.Replace("-", "/");
        DateTime originalDate1 = DateTime.ParseExact(db1, "yyyy/MM/dd", CultureInfo.InvariantCulture);
        db1 = originalDate1.ToString("dd/MM/yyyy").Replace("-", "/"); ;
        txt_todt.Text = db1;

        if (updl_file.HasFile)
        {
            string contentType = updl_file.PostedFile.ContentType;
            if (contentType.Substring(contentType.IndexOf("/") + 1, contentType.Length - contentType.IndexOf("/") - 1).Equals("pdf"))
            {
                if (updl_file.PostedFile.ContentLength > 512000)
                {
                    ShowMessage("Your file size is " + (updl_file.PostedFile.ContentLength / 1024).ToString("0.00") + " KB " + "Please upload file within 500KB");
                    return;
                }
            }
            else
            {
                ShowMessage("Please Upload pdf file only");
                return;
            }
        }

        try
        {
            ls_sql = "update T_CWM_PV_DTL_TMP set CPDT_ST_DT=to_date(:CPDT_ST_DT,'dd/mm/yyyy'),CPDT_END_DT=to_date(:CPDT_END_DT,'dd/mm/yyyy'),CPDT_DOC_TYPE=:CPDT_DOC_TYPE where CPDT_PV_ID=:CPDT_PV_ID";

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            cmd = new OracleCommand(ls_sql, con);
            cmd.Parameters.Add(new OracleParameter(":CPDT_ST_DT", txt_frmdt.Text.Trim()));
            cmd.Parameters.Add(new OracleParameter(":CPDT_END_DT", txt_todt.Text.Trim()));
            cmd.Parameters.Add(new OracleParameter(":CPDT_PV_ID", Convert.ToInt64(Session["PVID"])));
            cmd.Parameters.Add(new OracleParameter(":CPDT_DOC_TYPE", "pv"));
            cmd.ExecuteNonQuery();

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            if (updl_file.HasFile)
            {
                OracleCommand cmdfilepv = new OracleCommand();
                string filename;

                using (Stream fs = updl_file.PostedFile.InputStream)
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] bytes = br.ReadBytes((int)fs.Length);

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }

                        filename = Path.GetFileName(updl_file.PostedFile.FileName);

                        ls_sql = "update T_DOCUMENT_MASTER set DM_NAME=:DM_NAME,DM_FILE_CONTENT=:DM_FILE_CONTENT,DM_MODIFIED_BY=:DM_MODIFIED_BY,DM_MODIFIED_DATE=sysdate where DM_DOC_ID=:DM_DOC_ID";

                        if (con.State == ConnectionState.Closed)
                        {
                            con.Open();
                        }

                        cmdfilepv.CommandText = ls_sql;
                        cmdfilepv.Connection = con;
                        cmdfilepv.Parameters.Add(new OracleParameter(":DM_DOC_ID", hidcertid.Value.Trim()));
                        cmdfilepv.Parameters.Add(new OracleParameter(":DM_NAME", filename));
                        cmdfilepv.Parameters.Add(new OracleParameter(":DM_FILE_CONTENT", bytes));
                        cmdfilepv.Parameters.Add(new OracleParameter(":DM_MODIFIED_BY", Session["VendCode"]));
                        cmdfilepv.ExecuteNonQuery();

                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                    }
                }
            }

            ls_sql = "update hrace.t_sp_doc_verification set SDV_VERF_FLAG='S', SDV_REMARKS='',SDV_MODIFIED_BY=:SDV_MODIFIED_BY,SDV_MODIFIED_DATE=sysdate where SDV_REQ_NO=:SDV_REQ_NO";

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            cmd = new OracleCommand(ls_sql, con);
            cmd.Parameters.Add(new OracleParameter(":SDV_REQ_NO", "PV" + Session["PVID"]));
            cmd.Parameters.Add(new OracleParameter(":SDV_MODIFIED_BY", vVencode.Trim()));
            cmd.ExecuteNonQuery();
            showpv(Session["spno"].ToString());
            ShowMessage("Police verification updated successfully");
            clearpv();
        }
        catch (Exception ex)
        {
            ShowMessage(ex.Message);
        }
    }

    #endregion PV
}
