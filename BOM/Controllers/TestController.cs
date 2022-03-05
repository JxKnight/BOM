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
    public class TestController : ApiController
    {
        DataAccess dataAccess = new DataAccess();
        Default Default = new Default();
        Status error = new Status();
        // GET: Test
        [Route("Test")]
        public Status createToken()
        {
            error.ReturnCode = 200;
            error.Message = "Success";
            error.Data = Default.CreateMerchentToken("1058");
            return error;
        }

        [Route("Test2")]
        public Status verifySession(string x,Merchant.Merchant1 y)
        {
            error.ReturnCode = 200;
            error.Message = "Success";
            error.Data = Default.EncryptString(x, Newtonsoft.Json.JsonConvert.SerializeObject(y).ToString());
            return error;
        }

        [Route("Test3")]
        public Status updateSession(InputRequest req)
        {
            error.ReturnCode = 200;
            error.Message = "Success";
            error.Data = Default.DecryptString("yiqSdAy5rI-vURVg", req.data);
            return error;
        }
        [Route("Test4")]
        public Status test1()
        {
            error.ReturnCode = 200;
            error.Message = "Success";
            error.Data = null;
            return error;
        }
    }
}