using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace passbook.Model
{
  

    public abstract class PassGeneratorRequest
    {
        public string passTypeIdentifier { get; set; }
        public int formatVersion { get; set; }
        public string serialNumber { get; set; }
        public string description { get; set; }
        public string teamIdentifier { get; set; }
        public string organizationName { get; set; }


        public object foregroundColor { get; set; }
        public string backgroundColor { get; set; }
        public string logoText { get; set; }
        //有效期
        public string relevantDate { get; set; }
        //条形码
        public BarCode barcode { get;  set; }
        //优惠券实体
        public coupons coupon { get; set; }
        public bool suppressStripShine { get; set; }
        //经纬度
        public List<location> locations { get; set; }
        public string authenticationToken { get; set; }
        public string webServiceURL { get; set; }
        public List<int> associatedStoreIdentifiers { get; set; }
        //public string authorizationCode { get; set; }

        public PassGeneratorRequest()
        {
            suppressStripShine =true;
            passTypeIdentifier = "pass.velo.coupon";
            formatVersion = 1;
            teamIdentifier =  "QRRWVLWU9B";
            serialNumber = "123456789";
            description = "维络城2";
             logoText = "          维络城3";
             organizationName = "维络城";

            foregroundColor = "rgb(255, 255, 255)";
            backgroundColor = "rgb(16, 16, 16)";
            associatedStoreIdentifiers = new List<int>();
            associatedStoreIdentifiers.Add(462169708);
            //associatedStoreIdentifiers = "462169708";
        }

        public void AddBarCode(string message, BarcodeType type, string encoding, string altText)
        {

         

            barcode = new BarCode();
            barcode.format = type.ToString();
            barcode.message = message;
            barcode.messageEncoding = encoding;
            barcode.altText = altText;
        }

        
    }

    public class location
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
        public int altitude { get; set; }
        public  string relevantText { get; set; }
          public location()
        { }
          public location(double ilongitude, double ilatitude, int ialtitude, string irelevantText)
        {
            longitude = ilongitude;
            latitude = ilatitude;
            altitude = ialtitude;
            relevantText = irelevantText;

        }
    }

    public class associatedStoreIdentifiers
    {

    }

    public class BarCode
    {
        public string format { get; set; }
        public string message { get; set; }
        public string messageEncoding { get; set; }
        public string altText { get; set; }
    }
    public enum BarcodeType
    {
        PKBarcodeFormatQR,
        PKBarcodeFormatPDF417,
        PKBarcodeFormatAztec,
        PKBarcodeFormatText
    }
    public class Pass
    {
        private string packagePathAndName;

        public Pass(string packagePathAndName)
        {
            this.packagePathAndName = packagePathAndName;
        }

        public byte[] GetPackage()
        {
            byte[] contents = File.ReadAllBytes(packagePathAndName);
            return contents;
        }
    }
    public class CouponPassGeneratorRequest : PassGeneratorRequest
    {
       
    }
    public class EventPassGeneratorRequest : PassGeneratorRequest
    {
        public string EventName { get; set; }
        public string VenueName { get; set; }
    }
    public class StoreCardGeneratorRequest : PassGeneratorRequest
    {
        public double Balance { get; set; }
        public object OwnersName { get; set; }
    }

    public class coupons
    {
        /// <summary>
        /// 标题
        /// </summary>
        public List<Fields> primaryFields { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public List<Fields> auxiliaryFields { get; set; }
        /// <summary>
        /// 背后
        /// </summary>
        public List<Fields> backFields { get; set; }
    }

    public class Fields
    {
        /// <summary>
        /// 键名
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 展示
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string value { get; set; }
        public Fields()
        { }
        public Fields(string ikey, string ilabel, string ivalue)
        {
            key = ikey;
            label = ilabel;
            value = ivalue;

        }
        //public string textAlignment { get; set; }
        //public string changeMessage { get; set; }
    }




}
