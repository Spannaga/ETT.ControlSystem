using ProcessBulkUpload.Utilities;
using Main.Control.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessBulkUpload
{
    public class BulkUploadCommon
    {
        public CampaignDetails CommonExcelValuesAssignedCampaignDetails(CampaignDetails campaignDetails)
        {
            DataTable dt = new DataTable();
            var ColumnNameList = new List<string>();

            DbProviderFactory factory = DbProviderFactories.GetFactory("System.Data.OleDb");

            string exten = System.IO.Path.GetExtension(campaignDetails.FilePath);
            string connString = Utility.GetImportConnectionString(campaignDetails.FilePath, exten);


            using (DbConnection connection = factory.CreateConnection())
            {
                if (connection != null)
                {
                    connection.ConnectionString = connString;
                    try
                    {
                        connection.Open();
                    }
                    catch (OleDbException ex)
                    {
                        if (ex.ErrorCode == -2147467259)
                        {
                            connection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + campaignDetails.FilePath + ";" + "Extended Properties=\"Excel 12.0;HDR=Yes;\"";
                            connection.Open();
                        }
                    }

                    if (campaignDetails != null && campaignDetails.IsBatchStatus == false)
                    {
                        using (DbCommand command = connection.CreateCommand())
                        {
                            string sheetName = string.Empty;
                            DataTable sheetTable = connection.GetSchema("Tables");
                            foreach (DataRow row in sheetTable.Rows)
                            {
                                sheetName = (string)row["TABLE_NAME"];
                                if (!string.IsNullOrWhiteSpace(sheetName) && sheetName == "Users$")
                                {
                                    command.CommandText = "SELECT * FROM [Users$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                                 "[" + Constants.T_Email_Address + "]" + " IS NOT NULL OR " +
                                                 "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL ";

                                    DbDataReader dr = command.ExecuteReader();
                                    dt = dr.GetSchemaTable();

                                    //Header Details
                                    if (dt != null)
                                    {
                                        List<ChampaignExcelHeaderDetails> ChampaignHeaderDetailsList = new List<ChampaignExcelHeaderDetails>();
                                        foreach (DataRow datarow in dt.Rows)
                                        {
                                            ChampaignExcelHeaderDetails _headers = new ChampaignExcelHeaderDetails();
                                            string strColumnName = string.Empty;
                                            strColumnName = datarow["ColumnName"].ToString();
                                            strColumnName = strColumnName.Trim();
                                            if (!string.IsNullOrEmpty(strColumnName) && (Constants.T_Name != strColumnName && Constants.T_Email_Address != strColumnName && Constants.T_EIN != strColumnName && Constants.T_Phone_Number != strColumnName && Constants.T_Address != strColumnName && Constants.T_Business_Name != strColumnName))
                                            {
                                                _headers.HeaderName = strColumnName;
                                                ChampaignHeaderDetailsList.Add(_headers);
                                            }
                                        }
                                        if (ChampaignHeaderDetailsList != null && ChampaignHeaderDetailsList.Any())
                                        {
                                            campaignDetails.ChampaignExcelHeaderDetailsList = ChampaignHeaderDetailsList;
                                        }
                                    }
                                    int rowNUmber = 1;
                                    long supportUserId = 0;
                                    int indexingSupportUserCount = 0;
                                    int supportUserAssignedCount = 0;
                                    supportUserAssignedCount = campaignDetails.SupportAdminUserList[indexingSupportUserCount].TotalAssignedCount;

                                    while (dr.Read())
                                    {
                                        CampaignAssignedDetails campaignAssignedDetails = new CampaignAssignedDetails();
                                        supportUserId = campaignDetails.SupportUserList[indexingSupportUserCount];
                                        if (supportUserAssignedCount >= rowNUmber)
                                        {
                                            campaignAssignedDetails.SupportUserId = supportUserId;
                                        }
                                        else
                                        {
                                            indexingSupportUserCount += 1;
                                            supportUserAssignedCount += campaignDetails.SupportAdminUserList[indexingSupportUserCount].TotalAssignedCount;
                                            if (supportUserAssignedCount >= rowNUmber)
                                            {
                                                campaignAssignedDetails.SupportUserId = campaignDetails.SupportUserList[indexingSupportUserCount];
                                            }
                                        }

                                        if (dr[Constants.T_Name] != null)
                                        {
                                            campaignAssignedDetails.Name = dr[Constants.T_Name].ToString();
                                        }

                                        if (dr[Constants.T_Email_Address] != null)
                                        {
                                            campaignAssignedDetails.EmailAddress = dr[Constants.T_Email_Address].ToString();
                                        }

                                        if (dr[Constants.T_Business_Name] != null)
                                        {
                                            campaignAssignedDetails.BusinessName = dr[Constants.T_Business_Name].ToString();
                                        }

                                        //if (dr[Constants.T_EIN] != null)
                                        //{
                                        //    campaignAssignedDetails.EIN = dr[Constants.T_EIN].ToString();
                                        //}

                                        if (dr[Constants.T_Phone_Number] != null)
                                        {
                                            campaignAssignedDetails.PhoneNumber = dr[Constants.T_Phone_Number].ToString();
                                        }

                                        //if (dr[Constants.T_Address] != null)
                                        //{
                                        //    campaignAssignedDetails.Address = dr[Constants.T_Address].ToString();
                                        //}

                                        if (campaignDetails.ChampaignExcelHeaderDetailsList != null && campaignDetails.ChampaignExcelHeaderDetailsList.Any())
                                        {
                                            campaignAssignedDetails.ChampaignExcelValueDetailsList = new List<ChampaignExcelValueDetails>();

                                            foreach (var item in campaignDetails.ChampaignExcelHeaderDetailsList)
                                            {
                                                ChampaignExcelValueDetails _headerValues = new ChampaignExcelValueDetails();
                                                if (dr[item.HeaderName] != null)
                                                {
                                                    _headerValues.ExcelHeaderValue = Utility.RemoveSpecialChars(dr[item.HeaderName].ToString());
                                                    _headerValues.ExcelHeaderName = item.HeaderName;
                                                }
                                                campaignAssignedDetails.ChampaignExcelValueDetailsList.Add(_headerValues);
                                            }
                                        }
                                        campaignAssignedDetails.ProductName = Project.ExpressTruckTax.ToString();

                                        campaignDetails.CampaignAssignedDetailsList.Add(campaignAssignedDetails);

                                        rowNUmber++;
                                    }
                                }
                                else if (!string.IsNullOrWhiteSpace(sheetName) && sheetName == "User$")
                                {
                                    command.CommandText = "SELECT * FROM [User$] WHERE " + Constants.T_Name + " IS NOT NULL OR " +
                                                 "[" + Constants.T_Phone_Number + "]" + " IS NOT NULL OR " +
                                                 "[" + Constants.T_Return_Number + "]" + " IS NOT NULL ";

                                    DbDataReader dr = command.ExecuteReader();
                                    dt = dr.GetSchemaTable();

                                    //Header Details
                                    if (dt != null)
                                    {
                                        List<ChampaignExcelHeaderDetails> ChampaignHeaderDetailsList = new List<ChampaignExcelHeaderDetails>();
                                        foreach (DataRow datarow in dt.Rows)
                                        {
                                            ChampaignExcelHeaderDetails _headers = new ChampaignExcelHeaderDetails();
                                            string strColumnName = string.Empty;
                                            strColumnName = datarow["ColumnName"].ToString();
                                            strColumnName = strColumnName.Trim();
                                            if (!string.IsNullOrEmpty(strColumnName) && (Constants.T_Name != strColumnName && Constants.T_Phone_Number != strColumnName && Constants.T_Return_Number != strColumnName))
                                            {
                                                _headers.HeaderName = strColumnName;
                                                ChampaignHeaderDetailsList.Add(_headers);
                                            }
                                        }
                                        if (ChampaignHeaderDetailsList != null && ChampaignHeaderDetailsList.Any())
                                        {
                                            campaignDetails.ChampaignExcelHeaderDetailsList = ChampaignHeaderDetailsList;
                                        }
                                    }
                                    int rowNUmber = 1;
                                    long supportUserId = 0;
                                    int indexingSupportUserCount = 0;
                                    int supportUserAssignedCount = 0;
                                    supportUserAssignedCount = campaignDetails.SupportAdminUserList[indexingSupportUserCount].TotalAssignedCount;

                                    while (dr.Read())
                                    {
                                        CampaignAssignedDetails campaignAssignedDetails = new CampaignAssignedDetails();
                                        supportUserId = campaignDetails.SupportUserList[indexingSupportUserCount];
                                        if (supportUserAssignedCount >= rowNUmber)
                                        {
                                            campaignAssignedDetails.SupportUserId = supportUserId;
                                        }
                                        else
                                        {
                                            indexingSupportUserCount += 1;
                                            supportUserAssignedCount += campaignDetails.SupportAdminUserList[indexingSupportUserCount].TotalAssignedCount;
                                            if (supportUserAssignedCount >= rowNUmber)
                                            {
                                                campaignAssignedDetails.SupportUserId = campaignDetails.SupportUserList[indexingSupportUserCount];
                                            }
                                        }

                                        if (dr[Constants.T_Name] != null)
                                        {
                                            campaignAssignedDetails.Name = dr[Constants.T_Name].ToString();
                                        }


                                        if (dr[Constants.T_Phone_Number] != null)
                                        {
                                            campaignAssignedDetails.PhoneNumber = dr[Constants.T_Phone_Number].ToString();
                                        }
                                        if (dr[Constants.T_Return_Number] != null)
                                        {
                                            campaignAssignedDetails.ReturnNumber = Utility.GetLong(dr[Constants.T_Return_Number].ToString());
                                        }


                                        if (campaignDetails.ChampaignExcelHeaderDetailsList != null && campaignDetails.ChampaignExcelHeaderDetailsList.Any())
                                        {
                                            campaignAssignedDetails.ChampaignExcelValueDetailsList = new List<ChampaignExcelValueDetails>();

                                            foreach (var item in campaignDetails.ChampaignExcelHeaderDetailsList)
                                            {
                                                ChampaignExcelValueDetails _headerValues = new ChampaignExcelValueDetails();
                                                if (dr[item.HeaderName] != null)
                                                {
                                                    _headerValues.ExcelHeaderValue = Utility.RemoveSpecialChars(dr[item.HeaderName].ToString());
                                                    _headerValues.ExcelHeaderName = item.HeaderName;
                                                }
                                                campaignAssignedDetails.ChampaignExcelValueDetailsList.Add(_headerValues);
                                            }
                                        }
                                        campaignAssignedDetails.ProductName = Project.ExpressTruckTax.ToString();

                                        campaignDetails.CampaignAssignedDetailsList.Add(campaignAssignedDetails);

                                        rowNUmber++;
                                    }
                                }
                            }
                        }
                    }

                    if (campaignDetails != null && campaignDetails.IsBatchProcess == false)
                    {
                        if (campaignDetails.CampaignSupportUserDetailsList == null)
                        {
                            campaignDetails.CampaignSupportUserDetailsList = new List<CampaignSupportUserDetails>();
                        }

                        if (campaignDetails.SupportAdminUserList != null)
                        {
                            foreach (var adminUser in campaignDetails.SupportAdminUserList)
                            {
                                CampaignSupportUserDetails campaignSupportUserDetails = new CampaignSupportUserDetails();
                                campaignSupportUserDetails.AdminUserId = campaignDetails.AdminUserId;
                                campaignSupportUserDetails.SupportUserId = adminUser.UserId;
                                campaignSupportUserDetails.NoOfUserAssigned = adminUser.TotalAssignedCount;
                                campaignSupportUserDetails.NoOfPending = adminUser.TotalAssignedCount;
                                campaignDetails.CampaignSupportUserDetailsList.Add(campaignSupportUserDetails);
                            }
                        }
                    }
                    connection.Close();
                }
            }
            return campaignDetails;
        }
    }
}
