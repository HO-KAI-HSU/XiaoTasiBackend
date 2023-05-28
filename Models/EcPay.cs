using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using ECPay.Payment.Integration;

namespace travelManagement.Models
{
    public class EcPay
    {
        /*綠界參數設定*/
        public string MerchantID = "2000132";
        public string HashKey = "5294y06JbISpM5x9";
        public string HashIV = "v77hoKGq4kWxNNIS";
        public string Destination = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5";
        public string ReturnURL = "https://vendor-stage.ecpay.com.tw/Receive.php";
        public string ClientRedirectURL = "";
        Order order = new Order();


        public Dictionary<string, string> sendEcPay(string orderCode, string payWay)
        {
            Dictionary<string, string> orderList = this.OrderData(orderCode);
            Hashtable orderHash = this.CreateHashTable(orderList);
            string checksum = this.CreateHashValue(orderHash);
            int res = order.updateOrder("check_sum", checksum, orderCode);
            if (res == 1)
            {
                orderHash.Add("CheckMacValue", checksum);
                Dictionary<string, string> orderDic = this.HashtableToDictionary<string, string>(orderHash);
                return orderDic;

            }
            else
            {
                orderHash.Add("CheckMacValue", checksum);
                Dictionary<string, string> orderDic = this.HashtableToDictionary<string, string>(orderHash);
                return orderDic;
            }
        }

        public string sendEcPayTest(string orderCode, string payWay)
        {
            Dictionary<string, string> orderList = this.OrderData(orderCode);
            Hashtable orderHash = this.CreateHashTable(orderList);
            string checksum = this.CreateHashValue(orderHash);
            return checksum;
        }

        //建立傳入參數的雜湊表
        private Hashtable CreateHashTable(Dictionary<string, string> od)
        {

            /*資料串接*/
            string orderid = (od.ContainsKey("MerchantTradeNo")) ? od["MerchantTradeNo"] : "";  //訂單編號
            string tradedateDb = (od.ContainsKey("MerchantTradeDate")) ? od["MerchantTradeDate"] : "";  //交易時間
            string tradeDate = DateTime.Parse(tradedateDb).ToString("yyyy/MM/dd HH:mm:ss");//宣告一個目前的時間
            string cash = (od.ContainsKey("TotalAmount")) ? od["TotalAmount"] : "";  //交易金額
            string payment = (od.ContainsKey("ChoosePayment")) ? od["ChoosePayment"] : "";  //付款方式
            string amount = (od.ContainsKey("Amount")) ? od["Amount"] : "";  //訂購數量
            string travelName = (od.ContainsKey("TravelName")) ? od["TravelName"] : "";  //旅遊行程名稱

            /*傳給綠界使用的參數*/
            Hashtable h = new Hashtable();
            h.Add("MerchantID", this.MerchantID);
            h.Add("MerchantTradeNo", orderid);
            h.Add("MerchantTradeDate", tradeDate);
            h.Add("PaymentType", "aio");
            h.Add("TotalAmount", Double.Parse(cash).ToString());
            h.Add("TradeDesc", "促銷方案");
            h.Add("ItemName", travelName);
            h.Add("ReturnURL", this.ReturnURL);
            h.Add("ChoosePayment", payment);
            h.Add("EncryptType", "1");
            h.Add("NeedExtraPaidInfo", "Y");  //訂購數量
            return h;

        }


        //SHA256雜湊值
        private string CreateHashValue(Hashtable t)
        {
            ArrayList ary = new ArrayList(t.Keys);
            Every8d every8 = new Every8d();
            ary.Sort();

            string combineStr = "";


            foreach (string id in ary)
            {
                combineStr += id + "=" + t[id] + "&";
            }

            string sha256Str = "HashKey=" + this.HashKey + "&" + combineStr + "HashIV=" + this.HashIV;
            string urlEnLower = every8.urlEncode(sha256Str).ToLower();

            //string urlEnLower = "hashkey%3d5294y06jbispm5x9%26choosepayment%3dall%26encrypttype%3d1%26itemname%3dapple+iphone+7+%e6%89%8b%e6%a9%9f%e6%ae%bc%26merchantid%3d2000132%26merchanttradedate%3d2020%2f04%2f24+17%3a14%3a51%26merchanttradeno%3dor20200424171451%26needextrapaidinfo%3dy%26paymenttype%3daio%26returnurl%3dhttps%3a%2f%2fvendor-stage.ecpay.com.tw%2freceive.php%26totalamount%3d5%26tradedesc%3d%e4%bf%83%e9%8a%b7%e6%96%b9%e6%a1%88%26hashiv%3dv77hokgq4kwxnnis";
            //return urlEnLower;
            return SHA256Encoder.Encrypt(urlEnLower);
        }


        //訂單基本資料查詢
        private Dictionary<string, string> OrderData(string orderCode)
        {

            Dictionary<string, string> od = new Dictionary<string, string>();
            DataTable dtOrder = order.searchOrder(orderCode);
            od.Add("MerchantTradeNo", dtOrder.Rows[0]["order_code"].ToString().Trim());
            od.Add("MerchantTradeDate", dtOrder.Rows[0]["trade_date"].ToString().Trim());
            od.Add("TotalAmount", dtOrder.Rows[0]["trade_amt"].ToString().Trim());
            od.Add("ChoosePayment", "ALL");
            od.Add("Amount", dtOrder.Rows[0]["reservation_num"].ToString().Trim());
            od.Add("TravelName", dtOrder.Rows[0]["travel_name"].ToString().Trim());
            return od;
        }

        public Dictionary<K, V> HashtableToDictionary<K, V>(Hashtable table)
        {
            return table
              .Cast<DictionaryEntry>()
              .ToDictionary(kvp => (K)kvp.Key, kvp => (V)kvp.Value);
        }

    }
}
