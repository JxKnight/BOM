using System.Collections.Generic;

namespace BOM.Models
{
    public class Merchant
    {
        public string MerchantCode { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { get; set;}
        public List<string> MPhoneNumber { get; set; }
        public string MAddress1 { get; set; }
        public string MAddress2 { get; set; }
        public string MPostCode { get; set; }
        public string MCity { get; set;}
        public string MState { get; set; }

        public class Merchant1
        {
            public string MerchantCode { get; set; }
            public string MerchantName { get; set; }
            public List<string> MPhoneNumber { get; set; }
            public string MAddress1 { get; set; }
            public string MAddress2 { get; set; }
            public string MPostCode { get; set; }
            public string MCity { get; set; }
            public string MState { get; set; }
        }
        public class MerchantCheckSignature
        {
            public string MerchantCode { get; set; }
        }
        public class MerchantCheckSignatureByNameandPhoneNum
        {
            public string MerchantName { get; set; }
            public string MPostCode { get; set; }
            public string MCity { get; set; }
            public string MState { get; set; }
        }
    }

    //public class Merchant1
    //{
    //    public string MerchantCode { get; set; }
    //    public string MerchantName { get; set; }
    //    public List<string> MPhoneNumber { get; set; }
    //    public string MAddress1 { get; set; }
    //    public string MAddress2 { get; set; }
    //    public string MPostCode { get; set; }
    //    public string MCity { get; set; }
    //    public string MState { get; set; }
    //}

    public class AddMerchant
    {
        public string MerchantName { get; set; }
        public List<string> MPhoneNumber { get; set; }
        public string MAddress1 { get; set; }
        public string MAddress2 { get; set; }
        public string MPostCode { get; set; }
        public string MCity { get; set; }
        public string MState { get; set; }
    }
}