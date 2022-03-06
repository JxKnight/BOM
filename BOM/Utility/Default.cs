using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using BOM.Utility;

namespace BOM.Utility
{
    public class Default
    {
        DataAccess dataAccess = new DataAccess();
        public bool VerifySignature(string encodedtext, string signID, Object whereClause)
        {
            string textFromPattern = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@SignID", signID.ToString());
            string query = "select top 1 * from SignaturePattern with(nolock) where SignID = @SignID";
            using (SqlDataReader sqlReader = dataAccess.queryDB(query, cmd))
            {
                while (sqlReader.Read())
                {
                    string whereString = string.Empty;
                    Type type = whereClause.GetType();
                    for (int i = 0; i < whereClause.GetType().GetProperties().Length; i++)
                    {
                        if (i == whereClause.GetType().GetProperties().Length - 1)
                        {
                            whereString = whereString + whereClause.GetType().GetProperties()[i].Name + " = '" + whereClause.GetType().GetProperty(whereClause.GetType().GetProperties()[i].Name).GetValue(whereClause, null).ToString() + "'";
                        }
                        else
                        {
                            whereString = whereString + whereClause.GetType().GetProperties()[i].Name + " = '" + whereClause.GetType().GetProperty(whereClause.GetType().GetProperties()[i].Name).GetValue(whereClause, null).ToString() + "' and ";
                        }
                    }
                    string[] z = sqlReader["SignPattern"].ToString().Replace(":||:", "/").Split('/');
                    string y = sqlReader["SignTable"].ToString();
                    string query1 = "select top 1 " + string.Join(",", z) + " from " + y + " with(nolock) where " + whereString;
                    cmd = new SqlCommand();
                    using (SqlDataReader sqlReader1 = dataAccess.queryDB(query1, cmd))
                    {
                        string[] temp = new string[sqlReader1.FieldCount];
                        while (sqlReader1.Read())
                        {
                            for (int k = 0; k < sqlReader1.FieldCount; k++)
                            {
                                temp[k] = sqlReader1[z[k]].ToString();
                            }
                        }
                        textFromPattern = string.Join("", temp);
                    }
                }
            }
            if (textFromPattern != null)
            {
                return ComputeSha256Hash(textFromPattern, encodedtext);
            }
            else
            {
                return false;
            }
        }
        public bool ComputeSha256Hash(string rawData, string encoded)
        {
            string temp = string.Empty;
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                temp = builder.ToString();
                if (temp == encoded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public string CreateMerchentToken(string merchantid)
        {
            DateTime Now = DateTime.Now;
            string TokenString = Generate64String(16,"MerchantSession");
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@MerchantID", merchantid);
            cmd.Parameters.AddWithValue("@TokenString", TokenString);
            cmd.Parameters.AddWithValue("CreatedDateTime", Now.ToString());
            cmd.Parameters.AddWithValue("UpdateDateTime", Now.ToString());
            cmd.Parameters.AddWithValue("ExpiredTime", Now.AddMinutes(10).ToString());
            string query = "insert into MerchantToken (MerchantID,TokenString,CreatedDateTime,UpdateDateTime,ExpiredTime) values(@MerchantID,@TokenString,@CreatedDateTime,@UpdateDateTime,@ExpiredTime)";
            if (dataAccess.execQuery(query, cmd))
            {
                return TokenString;
            }
            else
            {
                return null;
            }
        }
        private string Generate64String(int num,string session)
        {
            bool check = false;
            string query = string.Empty;
            switch (session)
            {
                case "MerchantSession":
                    query = "Select top 1 * from MerchantToken with(nolock) where TokenString = @SessionString";
                    break;
                default:
                    break;
            }
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789/-+=-!";
            var stringChars = new char[num];
            var random = new Random();
            SqlCommand cmd = new SqlCommand();
            while (check==false)
            {

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);
                cmd.Parameters.AddWithValue("@SessionString", finalString);
                using (SqlDataReader dataReader = dataAccess.queryDB(query, cmd))
                {
                    if (dataReader.Read())
                    {
                        check = false;
                    }
                    else
                    {
                        check = true;
                        return finalString;
                    }
                }
               
            }
            return null;
        }
        public string checkTokenFromDB(string sessionString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@SessionString", sessionString);
            string query = "Select top 1 * from MerchantToken with(nolock) where TokenString = @SessionString and ExpiredTime >= getDate()";
            SqlDataReader dataReader = dataAccess.queryDB(query, cmd);
            if (dataReader.Read())
            {
               return dataReader["MerchantID"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        public bool UpdateToken(string merchantid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.AddWithValue("@MerchantId", merchantid);
            string query = "update MerchantToken set ExpiredTime = DATEADD(MINUTE, 10, getDate()), UpdateDateTime = getDate() where MerchantID = @MerchantId and DATEADD(MINUTE, -2, (select top 1 ExpiredTime from MerchantToken with(nolock) where MerchantID = @MerchantId)) < GETDATE()";
            if (dataAccess.execQuery(query, cmd))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        public string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
        public string verifyHeader(System.Net.Http.Headers.HttpRequestHeaders headers)
        {
            if(headers != null)
            {
                if (headers.Contains("Signature"))
                {
                    if (headers.Contains("Token"))
                    {
                        return string.Empty;
                    }
                    else
                    {
                        return "No Token Found";
                    }
                }
                else
                {
                    return "No Signature Found";
                }
            }
            else
            {
                return "No Header Found";
            }
        }
        public string HeaderToken(string tokenString)
        {
            if(tokenString.Substring(tokenString.Length - 8) != DateTime.Now.ToString("ddMMyyyy"))
            {
                return tokenString.Substring(0, 16);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}