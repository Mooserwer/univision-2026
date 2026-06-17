using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Univision.Core.Models.DTO.Response.Sms;

namespace Univision.Main.Infrastructure.SMS
{

    public interface ISmsService
    {
        Task<SmsServiceModel> SendSmsAsync(SmsServiceModel smsDto);
        string CheckPhoneNumber(string phone);
    }

    public class SmsService
    {
        string _serviceSmsUri = "http://rest.supersms.co:6200/sms/xml";
        string _serviceId = "unico_http";
        string _servicePw = "81205FAW101008GMZRX1";

        public async Task<SmsServiceModel> SendSmsAsyncNew(SmsSingleSendDto smsDto)
        {
            SmsServiceModel resultDto = new SmsServiceModel();

            HttpWebRequest request = null;     //Declare an HTTP-specific implementation of the WebRequest class.
            HttpWebResponse response = null;   //Declare an HTTP-specific implementation of the WebResponse class
            XDocument doc;

            try
            {
                smsDto.PhoneSender = CheckPhoneNumber(smsDto.PhoneSender);
                smsDto.PhoneReceiver = CheckPhoneNumber(smsDto.PhoneReceiver);

                StringBuilder getData = new StringBuilder();
                getData.Append("id=" + HttpUtility.UrlEncode(_serviceId) + "&");
                getData.Append("pwd=" + HttpUtility.UrlEncode(_servicePw) + "&");
                getData.Append("from=" + HttpUtility.UrlEncode(smsDto.PhoneSender) + "&");
                getData.Append("to_country=82&");
                getData.Append("to=" + HttpUtility.UrlEncode(smsDto.PhoneReceiver) + "&");
                if (!string.IsNullOrWhiteSpace(smsDto.Subject))
                    getData.Append("title=" + HttpUtility.UrlEncode(smsDto.Subject) + "&");
                getData.Append("message=" + HttpUtility.UrlEncode(smsDto.Message) + "&");
                getData.Append("report_req=1&");
                getData.Append("charset=1&");

                Uri requestUri = new Uri(_serviceSmsUri + "?" + getData.ToString());

                //Create Request
                request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Method = "GET";
                request.ContentType = "text/xml; encoding='utf-8'";


                //Get Response
                response = (HttpWebResponse)await request.GetResponseAsync();

                //결과 값이 담긴 XML을 읽어옴.
                doc = XDocument.Load(response.GetResponseStream());

                //XML에서 message Node만 빼냄.
                var xml = doc.Descendants("message");

                //읽어온 XML의 ChildNode를 읽어들인다.
                foreach (var childNode in xml)
                {
                    if (childNode.Element("err_code") != null && childNode.Element("err_code").Value.Equals("R000"))
                    {
                        resultDto.response_code = childNode.Element("err_code").Value;
                        resultDto.response_desc = SMSResponseCode.ResponseCodes[childNode.Element("err_code").Value];
                        resultDto.msgid = childNode.Element("msgid").Value;
                        resultDto.smsType = int.Parse(childNode.Element("msg_type").Value);
                    }
                    else if (childNode.Element("err_code") != null && !childNode.Element("err_code").Value.Equals("R000"))
                    {
                        resultDto.response_code = childNode.Element("err_code").Value;

                        if (SMSResponseCode.ResponseCodes.ContainsKey(childNode.Element("err_code").Value))
                            resultDto.response_desc = SMSResponseCode.ResponseCodes[childNode.Element("err_code").Value];
                        else
                        {
                            resultDto.response_desc = SMSResponseCode.ResponseCodes["R992"];
                            resultDto.response_code = "R992";
                        }
                        resultDto.msgid = "";
                        resultDto.smsType = 9;
                    }
                    else if (childNode.Element("err_code") == null)
                    {
                        resultDto.response_desc = SMSResponseCode.ResponseCodes["R993"];
                        resultDto.response_code = "R993";
                        resultDto.msgid = "";
                        resultDto.smsType = 9;
                    }

                }

                resultDto.smsType = smsDto.smsType;

                return resultDto;
            }
            catch (Exception ex)
            {
                resultDto.response_code = "R999";
                resultDto.response_desc = SMSResponseCode.ResponseCodes["R999"].ToString();
                resultDto.exception_message = ex.Message;
                return resultDto;
            }
            finally
            {
                //초기화
                request = null;
                response = null;
                doc = null;
            }
        }


        /// <summary>
        /// 전화번호가 010, 011 등의 휴대폰 번호인지 체크 후,
        /// 휴대폰 번호라면 010 => 10 형식의 국제번호 규격으로 return
        /// 또한 번호의 - 제거
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public string CheckPhoneNumber(string phone)
        {
            string tempNumber = phone;

            //정규식으로 휴대폰번호 검사
            System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("01[016789]\\-\\d{2,4}\\-\\d{3,4}");

            if (r.IsMatch(tempNumber))
                tempNumber = tempNumber.Replace("-", "");

            return tempNumber;
        }
    }
}