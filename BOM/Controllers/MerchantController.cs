using BOM.Models;
using BOM.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;

namespace BOM.Controllers
{
    public class MerchantController : ApiController
    {
        DataAccess dataAccess = new DataAccess();
        Default Default = new Default();
        Status error = new Status();

        //SqlCommand cmd = new SqlCommand();
        //cmd.Parameters.AddWithValue("@MerchantName",merchant.MerchantName);

        //[Route("Example")]
        //public Status Example(Object variable)
        //{
        //    Status error = new Status();
        //    return error;
        //}

        #region API
        [Route("GetMerchant")]
        public Status GetMerchant()
        {
            SqlCommand cmd = new SqlCommand();
            Merchant.Merchant1 merchant = new Merchant.Merchant1();
            List<Merchant.Merchant1> list = new List<Merchant.Merchant1>();
            SqlDataReader sqlReader;
            string query = string.Empty;
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            if (headers.Contains("MerchantCode"))
            {
                cmd.Parameters.AddWithValue("@Merchantcode", headers.GetValues("MerchantCode").First());
                query = "select top 1 * from Merchant with(nolock) where MerchantCode = @Merchantcode";
                sqlReader = dataAccess.queryDB(query, cmd);
                if (sqlReader.Read())
                {
                    merchant.MerchantCode = sqlReader["MerchantCode"].ToString();
                    merchant.MerchantName = sqlReader["MerchantName"].ToString();
                    merchant.MPhoneNumber = new List<string>(sqlReader["MPhoneNumber"].ToString().Split('/'));
                    merchant.MAddress1 = sqlReader["MAddress1"].ToString();
                    merchant.MAddress2 = sqlReader["MAddress2"].ToString();
                    merchant.MPostCode = sqlReader["MPostCode"].ToString();
                    merchant.MCity = sqlReader["MCity"].ToString();
                    merchant.MState = sqlReader["MState"].ToString();
                    if (headers.Contains("SubMerchant") && Convert.ToInt32(headers.GetValues("SubMerchant").First())>0)
                    {
                        list.Add(merchant);
                        List<string> merchantcode = new List<string>();
                        query = "select top "+ Convert.ToInt32(headers.GetValues("SubMerchant").First()) + " MerchantCode from MerchantCount with(nolock) where MainMerchantCode = @Merchantcode";
                        sqlReader = dataAccess.queryDB(query, cmd);
                        while (sqlReader.Read())
                        {
                            merchantcode.Add(sqlReader["MerchantCode"].ToString());
                        }
                        if (merchantcode.Count > 0)
                        {
                            cmd = new SqlCommand();
                            cmd.Parameters.AddWithValue("@MerchantCodes", string.Join(",", merchantcode.ToArray()));
                            query = "select * from merchant with(nolock) where merchantcode in (@MerchantCodes)";
                            sqlReader = dataAccess.queryDB(query, cmd);
                            while (sqlReader.Read())
                            {
                                merchant = new Merchant.Merchant1();
                                merchant.MerchantCode = sqlReader["MerchantCode"].ToString();
                                merchant.MerchantName = sqlReader["MerchantName"].ToString();
                                merchant.MPhoneNumber = new List<string>(sqlReader["MPhoneNumber"].ToString().Split('/'));
                                merchant.MAddress1 = sqlReader["MAddress1"].ToString();
                                merchant.MAddress2 = sqlReader["MAddress2"].ToString();
                                merchant.MPostCode = sqlReader["MPostCode"].ToString();
                                merchant.MCity = sqlReader["MCity"].ToString();
                                merchant.MState = sqlReader["MState"].ToString();
                                list.Add(merchant);
                            }
                            if (list.Count > 1)
                            {
                                error.Data = list;
                                error.ReturnCode = 200;
                                error.Message = "Get Success";
                                return error;
                            }
                            else
                            {
                                error.Data = merchant;
                                error.ReturnCode = 200;
                                error.Message = "Get Success";
                                return error;
                            }
                        }
                        else
                        {
                            error.Data = merchant;
                            error.ReturnCode = 200;
                            error.Message = "Get Success";
                            return error;
                        }
                    }
                    else
                    {
                        error.Data = merchant;
                        error.ReturnCode = 200;
                        error.Message = "Get Success";
                        return error;
                    }
                }
                else
                {
                    error.Data = headers.GetValues("MerchantCode").First();
                    error.ReturnCode = 401;
                    error.Message = "Get Merchant Fail";
                    return error;
                }

            }
            else
            {
                List<AddMerchant> merchanntList = new List<AddMerchant>();
                if (headers.Contains("Null"))
                {
                    query = "select * from merchant with(nolock) where merchantcode = null";
                }
                else
                {
                    query = "select * from merchant with(nolock)";
                }
                sqlReader = dataAccess.queryDB(query, cmd);
                while (sqlReader.Read())
                {
                    if (headers.Contains("Null"))
                    {
                        AddMerchant merchant1 = new AddMerchant();
                        merchant1.MerchantName = sqlReader["MerchantName"].ToString();
                        merchant1.MPhoneNumber = new List<string>(sqlReader["MPhoneNumber"].ToString().Split('/'));
                        merchant1.MAddress1 = sqlReader["MAddress1"].ToString();
                        merchant1.MAddress2 = sqlReader["MAddress2"].ToString();
                        merchant1.MPostCode = sqlReader["MPostCode"].ToString();
                        merchant1.MCity = sqlReader["MCity"].ToString();
                        merchant1.MState = sqlReader["MState"].ToString();
                        merchanntList.Add(merchant1);
                    }
                    else
                    {
                        Merchant.Merchant1 merchant1 = new Merchant.Merchant1();
                        merchant1.MerchantCode = sqlReader["MerchantCode"].ToString();
                        merchant1.MerchantName = sqlReader["MerchantName"].ToString();
                        merchant1.MPhoneNumber = new List<string>(sqlReader["MPhoneNumber"].ToString().Split('/'));
                        merchant1.MAddress1 = sqlReader["MAddress1"].ToString();
                        merchant1.MAddress2 = sqlReader["MAddress2"].ToString();
                        merchant1.MPostCode = sqlReader["MPostCode"].ToString();
                        merchant1.MCity = sqlReader["MCity"].ToString();
                        merchant1.MState = sqlReader["MState"].ToString();
                        list.Add(merchant1);
                    }
                }
                if (headers.Contains("Null"))
                {
                    error.Data = merchanntList;
                }
                else
                {
                    error.Data = list;
                }
                error.ReturnCode = 200;
                error.Message = "Get Success";
                return error;
            }
        }
        [Route("AddMerchant")]
        public Status AddMerchant(InputRequest req)
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            string checkHeader = Default.verifyHeader(headers);
            AddMerchant merchant;
            //AddMerchant merchant = new AddMerchant();
            if (checkHeader == string.Empty)
            {
                merchant = JsonConvert.DeserializeObject<AddMerchant>(Default.DecryptString(headers.GetValues("Token").First(), req.data));
                if (IsAnyNullOrEmpty(merchant))
                {
                    error.ReturnCode = 401;
                    error.Message = "Fail Insert Merchant, There are null value in the object.";
                    error.Data = null;
                    return error;
                }
                else
                {
                    string newPhoneNum = string.Empty;
                    if (merchant.MPhoneNumber.Count > 0)
                    {
                        newPhoneNum = string.Join("/", merchant.MPhoneNumber);
                    }
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@MerchantName", merchant.MerchantName);
                    cmd.Parameters.AddWithValue("@MPhoneNumber", newPhoneNum);
                    cmd.Parameters.AddWithValue("@MAddress1", merchant.MAddress1);
                    cmd.Parameters.AddWithValue("@MAddress2", merchant.MAddress2);
                    cmd.Parameters.AddWithValue("@MPostCode", merchant.MPostCode);
                    cmd.Parameters.AddWithValue("@MCity", merchant.MCity);
                    cmd.Parameters.AddWithValue("@MState", merchant.MState);
                    string query = "Insert into merchant (MerchantName,MPhoneNumber,MAddress1,MAddress2,MPostCode,MCity,MState) values (@MerchantName,@MPhoneNumber,@MAddress1,@MAddress2,@MPostCode,@MCity,@MState)";
                    if (dataAccess.execQuery(query, cmd))
                    {
                        error.ReturnCode = 200;
                        error.Message = "Insert Success";
                        error.Data = merchant;
                        return error;
                    }
                    else
                    {
                        error.ReturnCode = 400;
                        error.Message = "Fail Insert Merchant";
                        error.Data = null;
                        return error;
                    }
                }
            }
            else
            {
                error.ReturnCode = 800;
                error.Message = checkHeader;
                error.Data = req.data;
                return error;
            }
        }
        [Route("CheckMerchant")]
        public Status CheckMerchant()
        {
            int result = 0;
            string query = "select COUNT(MerchantCode)As Result from merchantcount with(nolock)";
            using (SqlDataReader sqlReader = dataAccess.queryDB(query, new SqlCommand()))
            {
                while (sqlReader.Read())
                {
                    result = Convert.ToInt32(sqlReader["Result"]);
                    error.ReturnCode = 200;
                    error.Message = "Get Success";
                    result = result + 1;
                    error.Data = completeMerchantCode(result.ToString());
                }
            }

            return error;
        }
        [Route("VerifyMerchant")]
        public Status verifyMerchant(Merchant.Merchant1 merchant)
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            string checkHeader = Default.verifyHeader(headers);
            if (checkHeader == string.Empty)
            {
                Merchant.MerchantCheckSignatureByNameandPhoneNum obj = new Merchant.MerchantCheckSignatureByNameandPhoneNum() { MerchantName = merchant.MerchantName, MCity = merchant.MCity, MPostCode = merchant.MPostCode, MState = merchant.MState };
                if (Default.VerifySignature(headers.GetValues("Signature").First(), "S0002", obj))
                {
                    int random = 0;
                    bool check = false;
                    random = random4Nums();
                    while (check == false)
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@MerchantId", random);
                        string newQuery = "select * from merchantcount with(nolock) where MerchantId = " + random;
                        using (SqlDataReader sqlReader = dataAccess.queryDB(newQuery, cmd))
                        {
                            if (sqlReader.HasRows)
                            {
                                random = random4Nums();
                            }
                            else
                            {
                                if (assignMerchantId(merchant, random) == true)
                                {
                                    error.ReturnCode = 200;
                                    error.Message = "Verified Merchant";
                                    error.Data = merchant;
                                    check = true;
                                }
                                else
                                {
                                    error.ReturnCode = 401;
                                    error.Message = "Assign MerchantID Fail";
                                    error.Data = null;
                                    check = true;
                                };

                            }
                        }
                        return error;
                    }
                }
                else
                {
                    error.ReturnCode = 402;
                    error.Message = "Wrong Signature";
                    error.Data = null;
                    return error;
                }
            }
            else
            {
                error.ReturnCode = 800;
                error.Message = checkHeader;
                error.Data = merchant;
                return error;
            }
            return error;
        }
        [Route("DeleteMerchant")]
        public Status DeleteMerchant(Merchant merchant)
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            if (headers.Contains("Signature"))
            {
                Merchant.MerchantCheckSignature obj = new Merchant.MerchantCheckSignature() { MerchantCode = merchant.MerchantCode };
                if (Default.VerifySignature(headers.GetValues("Signature").First(), "S0003", obj))
                {
                    string newPhoneNum = string.Empty;
                    bool[] check = new bool[2];
                    check[0] = false;
                    check[1] = false;
                    if (merchant.MPhoneNumber.Count > 0)
                    {
                        newPhoneNum = string.Join("/", merchant.MPhoneNumber);
                    }
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.AddWithValue("@MerchantName", merchant.MerchantName.ToString());
                    cmd.Parameters.AddWithValue("@MPhoneNumber", newPhoneNum.ToString());
                    string query = "delete from merchant where merchantname = @MerchantName and MPhoneNumber= @MPhoneNumber";
                    if (dataAccess.execQuery(query, cmd))
                    {
                        check[0] = true;
                    }
                    else
                    {
                        check[0] = false;
                    }

                    if (merchant.MerchantId != null && check[0] == true)
                    {
                        cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@MerchantCode", merchant.MerchantCode.ToString());
                        string query1 = "delete from merchantcount where MerchantCode = @MerchantCode";
                        if (dataAccess.execQuery(query1, cmd))
                        {
                            check[1] = true;
                        }
                        else
                        {
                            check[1] = false;
                        }
                    }
                    else
                    {
                        check[1] = true;
                    }
                    if (check[0] && check[1] && merchant.MerchantId != null)
                    {
                        error.ReturnCode = 200;
                        error.Message = "Delete Merchant Success";
                        error.Data = null;
                        return error;
                    }
                    else if (check[0] && merchant.MerchantId == null)
                    {
                        error.ReturnCode = 200;
                        error.Message = "Delete Merchant Success";
                        error.Data = null;
                        return error;
                    }
                    else
                    {
                        error.ReturnCode = 401;
                        error.Message = "Delete Merchant Fail";
                        error.Data = null;
                        return error;
                    }
                }
                else
                {
                    error.ReturnCode = 403;
                    error.Message = "Wrong Signature";
                    error.Data = merchant;
                    return error;
                }
            }
            else
            {
                error.ReturnCode = 402;
                error.Message = "Signature Null Value";
                error.Data = merchant;
                return error;
            }
        }
        [Route("EditMerchant")]
        public Status EditMerchant(InputRequest req)
        {
            System.Net.Http.Headers.HttpRequestHeaders headers = this.Request.Headers;
            string checkHeader = Default.verifyHeader(headers);
            Merchant.Merchant1 merchant;
            if (checkHeader == string.Empty)
            {
                merchant = JsonConvert.DeserializeObject<Merchant.Merchant1>(Default.DecryptString(headers.GetValues("Token").First(), req.data));
                Merchant.MerchantCheckSignature check = new Merchant.MerchantCheckSignature() { MerchantCode = merchant.MerchantCode };
                if (Default.VerifySignature(headers.GetValues("Signature").First(), "S0001", check) && Default.checkTokenFromDB(getMerchantID(req.merchantcode), headers.GetValues("Token").First()) && merchant.MerchantCode == req.merchantcode)
                {
                    if (IsAnyNullOrEmpty(merchant))
                    {
                        error.ReturnCode = 402;
                        error.Message = "Fail Edit Merchant, There are null value in the object.";
                        error.Data = merchant;
                        return error;
                    }
                    else
                    {
                        string newPhoneNum = string.Empty;
                        if (merchant.MPhoneNumber.Count > 0)
                        {
                            newPhoneNum = string.Join("/", merchant.MPhoneNumber);
                        }
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.AddWithValue("@MerchantCode", merchant.MerchantCode);
                        cmd.Parameters.AddWithValue("@MerchantName", merchant.MerchantName);
                        cmd.Parameters.AddWithValue("@MPhoneNumber", newPhoneNum);
                        cmd.Parameters.AddWithValue("@MAddress1", merchant.MAddress1);
                        cmd.Parameters.AddWithValue("@MAddress2", merchant.MAddress2);
                        cmd.Parameters.AddWithValue("@MPostCode", merchant.MPostCode);
                        cmd.Parameters.AddWithValue("@MCity", merchant.MCity);
                        cmd.Parameters.AddWithValue("@MState", merchant.MState);
                        string query = "Update merchant set MerchantName = @MerchantName,MPhoneNumber = @MPhoneNumber,MAddress1 = @MAddress1, MAddress2 = @MAddress2, MPostCode = @MPostCode, MCity = @MCity, MState = @MState where MerchantCode = @MerchantCode";
                        if (dataAccess.execQuery(query, cmd))
                        {
                            error.ReturnCode = 200;
                            error.Message = "Update Merchant Success";
                            error.Data = merchant;
                            return error;
                        }
                        else
                        {
                            error.ReturnCode = 400;
                            error.Message = "Update Merchant Fail";
                            error.Data = merchant;
                            return error;
                        }
                    }
                }
                else
                {
                    error.ReturnCode = 403;
                    error.Message = "Wrong Signature";
                    error.Data = merchant;
                    return error;
                }
            }
            else
            {
                error.ReturnCode = 401;
                error.Message = "Signature Null Value";
                error.Data = req.data;
                return error;
            }

        }
        #endregion

        private bool assignMerchantId(Merchant.Merchant1 merchant, int random)
        {
            string newPhoneNum = string.Empty;
            if (merchant.MPhoneNumber.Count > 0)
            {
                newPhoneNum = string.Join("/", merchant.MPhoneNumber);
            }
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@MerchantCode", merchant.MerchantCode);
            cmd.Parameters.AddWithValue("@MerchantName", merchant.MerchantName);
            cmd.Parameters.AddWithValue("@MPhoneNumber", newPhoneNum);
            cmd.Parameters.AddWithValue("@Mstate", merchant.MState);
            string newQuery = "update Merchant set MerchantCode = @MerchantCode where MerchantName = @MerchantName and MPhoneNumber = @MPhoneNumber and Mstate = @Mstate";
            if (dataAccess.execQuery(newQuery, cmd))
            {
                cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@MerchantCode", merchant.MerchantCode);
                cmd.Parameters.AddWithValue("@MerchantId", random);
                string newQuery1 = "Insert into merchantcount (MerchantCode,Merchantid) values (@MerchantCode,@MerchantId)";
                if (dataAccess.execQuery(newQuery1, cmd))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private string completeMerchantCode(string merchant)
        {
            int length = merchant.Length;
            string result = string.Empty;
            for (int i = 0; i < (5 - length); i++)
            {
                result = result + "0";
            }
            return "M" + result + merchant;
        }
        private int random4Nums()
        {
            Random random = new Random();
            return random.Next(1000, 1999);
        }
        private bool IsAnyNullOrEmpty(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public string getMerchantID(string merchantcode)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@Merchantcode", merchantcode);
            string query = "select top 1 * from MerchantCount with(nolock) where MerchantCode = @Merchantcode";
            SqlDataReader sqlReader = dataAccess.queryDB(query, cmd);
            if (sqlReader.Read())
            {
                return sqlReader["MerchantID"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        //[Route("Test")]
        //public Status Example(Merchant.MerchantCheckSignature variable)
        //{
        //    Status error = new Status();
        //    error.Message = VerifySignature("test", "S0001", variable).ToString();
        //    return error;
        //}
    }
}