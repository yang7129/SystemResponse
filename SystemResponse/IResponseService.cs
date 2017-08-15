using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SystemResponse
{
    // 注意: 您可以使用 [重構] 功能表上的 [重新命名] 命令同時變更程式碼和組態檔中的介面名稱 "IResponseService"。
    [ServiceContract]
    public interface IResponseService : Ibase
    {
        [OperationContract]
        ReturnResult ResponseData(InResponseData InValue);//string SendUrl, string PostString, MethodType methodType, ContentType contentType, SendEncoding sendEncoding, string sendEncode, string Authorization, EncodingType encodingType);
        [OperationContract]
        ReturnResult ResponseDataForGet_UTF8(string SendUrl);
    }
    
    [DataContract]
    public class InResponseData
    {
        [DataMember]
        public string sendUrl { get; set; }
        [DataMember]
        public string sendPostString { get; set; }
        [DataMember]
        public SendMethodType sendmethodType { get; set; }
        [DataMember]
        public SendContentType sendcontentType { get; set; }
         [DataMember]
        public SendEncoding sendEncoding { get; set; }
         [DataMember]
        public string sendEncode { get; set; }
        [DataMember]
         public string sendAuthorization { get; set; }
        [DataMember]
        public ReponEncodingType reponencodingType  { get; set; }
        public InResponseData()
        {
            sendUrl = "";
            sendPostString = "";
            sendmethodType = SendMethodType.Get;
            sendcontentType = SendContentType.applicationxwwwformurlencoded;
            sendEncoding = SendEncoding.Default;
            sendEncode = "utf-8";
            sendAuthorization = "";
            reponencodingType = ReponEncodingType.UTF8;
        }
    }
}
