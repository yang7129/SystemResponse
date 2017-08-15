using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SystemResponse
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼、svc 和組態檔中的類別名稱 "ResponseService"。
    // 注意: 若要啟動 WCF 測試用戶端以便測試此服務，請在 [方案總管] 中選取 ResponseService.svc 或 ResponseService.svc.cs，然後開始偵錯。
    public class ResponseService : @base, IResponseService
    {
        /// <summary>
        /// 發送資料
        /// </summary>
        /// <param name="SendUrl">網址 <para></para> Ex:http://localhost:12345/AA/BB?TEST=123  http://localhost:12345/AA/BB</param>
        /// <param name="PostString">發送資料<para></para> TEST=123</param>
        /// <param name="methodType">發送形式</param>
        /// <param name="contentType">發送類別</param>
        /// <param name="sendEncoding">發送編碼</param>
        /// <param name="sendEncode">發送編碼型態</param>
        /// <param name="Authorization">發送Auth</param>
        /// <param name="encodingType">讀取編碼</param>
        /// <returns></returns> 
        public ReturnResult ResponseData(InResponseData InValue)//string SendUrl, string PostString = "", MethodType methodType = MethodType.Get, ContentType contentType = ContentType.applicationxwwwformurlencoded, SendEncoding sendEncoding = SendEncoding.Default, string sendEncode = "utf-8", string Authorization = "", EncodingType encodingType = EncodingType.UTF8)
        {
            ReturnResult ReturnResult = new ReturnResult();

            string methodTypeVal = "";
            string contentTypeVal = "";
            System.Reflection.FieldInfo fi = InValue.sendmethodType.GetType().GetField(InValue.sendmethodType.ToString());
            EnumMemberAttribute[] attributes = (EnumMemberAttribute[])fi.GetCustomAttributes(typeof(EnumMemberAttribute), false);
            if (attributes.Length > 0)
                methodTypeVal = attributes[0].Value;

            fi = InValue.sendcontentType.GetType().GetField(InValue.sendcontentType.ToString());
            attributes = (EnumMemberAttribute[])fi.GetCustomAttributes(typeof(EnumMemberAttribute), false);
            if (attributes.Length > 0)
                contentTypeVal = attributes[0].Value;

            if (string.IsNullOrEmpty(methodTypeVal) || string.IsNullOrEmpty(contentTypeVal) || string.IsNullOrEmpty(InValue.reponencodingType.ToString()))
            {
                ReturnResult.ReturnMsgNo = -99;
                ReturnResult.ReturnMsg = "選擇型別錯誤";
                ReturnResult.ErrorCode = "SRP0001";
                return ReturnResult;
            }

            if (string.IsNullOrEmpty(InValue.sendUrl))
            {
                ReturnResult.ReturnMsgNo = -98;
                ReturnResult.ReturnMsg = "發送網址未填";
                ReturnResult.ErrorCode = "SRP0002";
                return ReturnResult;
            } 

            try
            {
                HttpWebRequest WebRequest = (HttpWebRequest)HttpWebRequest.Create(InValue.sendUrl.Trim());
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                if (string.IsNullOrEmpty(InValue.sendAuthorization).Equals(false))
                    WebRequest.Headers.Add("Authorization", InValue.sendAuthorization);

                switch (InValue.sendmethodType)
                {
                    case SendMethodType.Get: WebRequest.Method = "GET"; break;
                    case SendMethodType.Post:
                        byte[] parameterString = null;
                        switch (InValue.sendEncoding)
	                    {
                            case SendEncoding.UTF8: parameterString = Encoding.UTF8.GetBytes(InValue.sendPostString); 
                                break;
                            case SendEncoding.ASCII: parameterString = Encoding.ASCII.GetBytes(InValue.sendPostString); 
                                break;
                            case SendEncoding.GetEncoding: parameterString = Encoding.GetEncoding(InValue.sendEncode).GetBytes(InValue.sendPostString); 
                                break;
                            case SendEncoding.Default: parameterString = Encoding.Default.GetBytes(InValue.sendPostString); 
                                break; 
	                    }  
                        WebRequest.Method = "POST";
                        switch (InValue.sendcontentType)
                        {
                            case SendContentType.applicationxwwwformurlencoded:
                                WebRequest.ContentType = "application/x-www-form-urlencoded";
                                break;
                            case SendContentType.textxml:
                                WebRequest.ContentType = "text/xml";
                                break;
                            case SendContentType.applicationjson:
                                WebRequest.ContentType = "application/json";
                                break;
                        }
                        WebRequest.ContentLength = parameterString.Length;
                        //等待要求逾時之前的毫秒數。預設值為 100,000 毫秒 (100 秒)。
                        Stream newStream = WebRequest.GetRequestStream();
                        newStream.Write(parameterString, 0, parameterString.Length);
                        newStream.Close();
                        break; 
                }
               
                HttpWebResponse WebResponse = (HttpWebResponse)WebRequest.GetResponse();

                StreamReader sr;
                string ReturnString = "";
                switch (InValue.reponencodingType)
                {
                    case ReponEncodingType.Default:
                        sr = new StreamReader(WebResponse.GetResponseStream(), Encoding.Default);
                        ReturnString = sr.ReadToEnd();
                        sr.Close();
                        break;
                    case ReponEncodingType.UTF8:
                        sr = new StreamReader(WebResponse.GetResponseStream(), Encoding.UTF8);
                        ReturnString = sr.ReadToEnd();
                        sr.Close();
                        break;
                } 
                //Convert the stream to a string

                if (string.IsNullOrEmpty(ReturnString).Equals(false))
                    ReturnString = ReturnString.Trim();

                WebResponse.Close();
                ReturnResult.ReturnMsgNo = 1;
                ReturnResult.ReturnMsg = ReturnString; 
                ReturnResult.ErrorCode = "SRP0000";
            }
            catch (Exception Ex)
            {
                ReturnResult.ReturnMsgNo = -999;
                ReturnResult.ReturnMsg = "發生例外錯誤" + Ex.ToString(); 
                ReturnResult.ErrorCode = "SRP0003";
            }
            return ReturnResult; 
        }
        /// <summary>
        /// 發送UTF-8資料<para></para> 
        /// 用UTF-8解析編碼
        /// </summary>
        /// <param name="SendUrl">發送的URL</param>
        /// <returns></returns>
        public ReturnResult ResponseDataForGet_UTF8(string SendUrl)
        {
            InResponseData InResponseData = new InResponseData();
            InResponseData.sendUrl = SendUrl; 
            InResponseData.sendEncoding= SendEncoding.UTF8; 
            return ResponseData(InResponseData);//SendUrl, null, SendMethodType.Get, SendContentType.applicationxwwwformurlencoded, SendEncoding.UTF8, null, null, ReponEncodingType.UTF8);  
        }
        
    }
}
 