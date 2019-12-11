using AMS.Controllers;
using AMS.Models;
using isRock.LineBot;
using Newtonsoft.Json;
using isRock.LineBot.Conversation;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;


namespace WebApplication5.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        public const string channelAccessToken = @"FHrMW2H48qvfkn/Hxq0y6Sq4JBKJqWpqhQNtZ/hGEEPRUQrjVeT8T14q6s32iIoJuLIIb4t8oYC2ensry6ACSEEBt5lwpbfMkq4Fcevx7e0KxUYF68mJQqkPGkVBpPMwn5Qh25j6lmSte3Pp0yo0UQdB04t89/1O/w1cDnyilFU=";//@"ehC2bzsC2xmmwK5J59gcEK4ihHfRlYfb8kQFxVR2jn0B9vlAtMfvAwXXn5KfJfeQlC+5Higk86SmFJkwGn3bwDHH1uvL2X4vwahMbdMCeIFJttH9jNekMNBw6RHL0hJaQq2oEDSKKf0ocx3CQTFaO1GUYhWQfeY8sLGRXgo3xvw=";
        public string AdminUserId;
        public static string Lat;//緯度
        public static string Long;//經度
        [Route("api/LineWebHookSample")]
        [HttpPut]
        public IHttpActionResult Put(Position position)
        {
            Lat = (position.Lat).Substring(0, 9);
            Long = (position.Long).Substring(0, 10);
            return Json(position);
        }
        public bool flaglocation()
        {
            double MSITLat = 25.033765;////資策會經緯度
            double MSITLong = 121.543412;//資策會經緯度

            if (((MSITLat + 0.0015) > double.Parse(Lat) && (MSITLat - 0.0015) < double.Parse(Lat)) && ((MSITLong + 0.0015) > double.Parse(Long) && (MSITLong - 0.0015) < double.Parse(Long)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [Route("api/LineWebHookSample")]
        [HttpPost]
        public IHttpActionResult POST()
        {



            Entities d = new Entities();
            this.AdminUserId = this.ReceivedMessage.events.FirstOrDefault().source.userId;
            //var FindEmpID = d.Employees.Where(p => p.LineID == AdminUserId).First().EmployeeID;
            Attendances a = new Attendances();
            //設定ChannelAccessToken(或抓取Web.Config)
            this.ChannelAccessToken = channelAccessToken;
            //取得Line Event(範例，只取第一個)
            var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
            //配合Line verify 
            if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
            //回覆訊息

            var responseMsg = "";//設定一個空字串給請假

            //var Liff = isRock.LIFF.Utility.AddLiffApp(channelAccessToken, new Uri("https://1380f17d.ngrok.io/Home/Contact"), isRock.LIFF.ViewType.full);            

            if (d.Employees.Where(p => p.LineID == AdminUserId).FirstOrDefault() != null)//正
            {

                try
                {
                    var q = d.Employees.Where(p => p.LineID == AdminUserId).First();
                    var EmpID = q.EmployeeID;//正

                    if (LineEvent.type == "message")
                    {

                        //取得用戶資訊
                        //string EmpId = "MSIT1230015";//測試用用戶
                        string st1 = "22:00";//設定最晚打卡的上班時間(可以擺到人事) ///要再做每日統計上班時數功能  
                        string st2 = "13:00";//設定最早打卡的下班時間(可以擺到人事) ///要再做每日統計上班時數功能  
                        DateTime dt1 = Convert.ToDateTime(st1);
                        DateTime dt2 = Convert.ToDateTime(st2);
                        DateTime today = DateTime.Now.Date;//今天的日期  
                        DateTime yesterday = today.AddDays(-1);//////前天日期
                        var bot = new Bot(channelAccessToken);

                        if (LineEvent.message.type == "text")
                        {
                            switch (LineEvent.message.text)
                            {
                                case "打卡":
                                    {
                                        var judgeleave_today = d.LeaveRequests.Where(p => p.EmployeeID == EmpID && p.StartTime <= today && p.EndTime >= today && p.ReviewStatusID == 2);
                                        if (judgeleave_today.FirstOrDefault() == null)
                                        {
                                            var day_uncheck = (from e in d.Employees.AsEnumerable()/////////判斷前有無打卡
                                                               join att in d.Attendances.AsEnumerable().Where(p => p.Date == yesterday && p.EmployeeID == EmpID) on e.EmployeeID equals att.EmployeeID into g
                                                               from att in g.DefaultIfEmpty()
                                                               select new SerchAttendancesViewModel
                                                               {
                                                                   EmployeeName = e.EmployeeName,
                                                                   Date = att == null ? null : att.Date.ToString("yyyy/MM/dd"),
                                                                   StartTime = att == null ? null : att.OnDuty,
                                                                   EndTime = att == null ? null : att.OffDuty,

                                                               });
                                            var judgeleave_yesterday = d.LeaveRequests.Where(p => p.EmployeeID == EmpID && p.StartTime <= yesterday && p.EndTime >= yesterday && p.ReviewStatusID == 2);
                                            var workdays = d.WorkingDaySchedule.Where(p => p.Date == yesterday);///判定是否為假日
                                            var wholeday = day_uncheck.FirstOrDefault().Date;/////判定前日有無值
                                            var afternoon = day_uncheck.FirstOrDefault().EndTime;/////判定前日下班有無值
                                            var morning = day_uncheck.FirstOrDefault().StartTime;/////判定前日上班有無值
                                            if (wholeday == null && workdays.FirstOrDefault().WorkingDay == "工作日" && judgeleave_yesterday.FirstOrDefault() == null)
                                            {
                                                a.EmployeeID = EmpID;
                                                a.Date = today.AddDays(-1);
                                                a.station = "全天未打卡";
                                                a.savehours = 0;
                                                d.Attendances.Add(a);
                                                this.ReplyMessage(LineEvent.replyToken, $"前日全天未打卡");
                                                d.SaveChanges();
                                            }
                                            if (morning != null && afternoon == null)
                                            {
                                                var check_yesterday = d.Attendances.Where(p => p.Date == yesterday && p.EmployeeID == EmpID);
                                                check_yesterday.FirstOrDefault().station = "下午未打卡";
                                                check_yesterday.FirstOrDefault().savehours = 4;
                                                this.ReplyMessage(LineEvent.replyToken, $"前日下午未打卡");
                                                d.SaveChanges();
                                            }

                                            var flex = $@"[{{ ""type"": ""flex"",""altText"":""打卡"",""contents"":{{""type"": ""bubble"",""hero"": {{""type"": ""image"",""url"": ""https://ba3c6ba2.ngrok.io/Img/linelocatiom.png"",
                                                   ""size"": ""full"",""aspectRatio"": ""20:13"",""aspectMode"": ""cover""}},""footer"": {{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""spacer"",""size"": ""xxl""
                                                   }},{{""type"": ""button"",""style"": ""primary"",""color"": ""#D75455"",""action"": {{""type"": ""uri"",""label"": ""請先確認定地點"",""uri"": ""line://app/1653574503-YJW285kB""}}}}]}}}}}}]";
                                            bot.PushMessageWithJSON(AdminUserId, flex);

                                        }
                                        else
                                        {
                                            this.ReplyMessage(LineEvent.replyToken, $"您在{judgeleave_today.FirstOrDefault().StartTime}-{judgeleave_today.FirstOrDefault().EndTime}申請的請假已審核通過");
                                        }
                                        break;
                                    }
                                case "定位成功":
                                    {
                                        var flex_check = $@"[{{""type"": ""flex"",""altText"":""簽到"",""contents"":{{""type"": ""bubble"",""body"": {{""type"": ""box"",""layout"": ""horizontal"",""contents"": [{{""type"": ""text"",""text"": ""目前時間:"",""size"": ""md"",""weight"": ""bold"",
                                                         ""color"": ""#FFFFFB""}},{{""type"": ""text"",""text"": ""{DateTime.Now.ToShortTimeString().ToString()}"",""color"": ""#FFFFFB"",""size"": ""md"",""weight"": ""bold"",""align"": ""start""}}],""backgroundColor"": ""#464F69""
                                                         }},""footer"": {{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""button"",""style"": ""primary"",""color"": ""#3A8FB7"",""action"": {{""type"": ""message"",""label"": ""上班"",""text"": ""上班""
                                                         }}}},{{""type"": ""button"",""action"": {{""type"": ""message"",""label"": ""下班"",""text"": ""下班""}},""style"": ""primary"",""color"": ""#5DAC81""}}],""spacing"": ""sm""}}}}}}]";
                                        bot.PushMessageWithJSON(AdminUserId, flex_check);
                                        break;
                                    }
                                case "上班":
                                    try
                                    {
                                        if (flaglocation())
                                        {
                                            if (DateTime.Now < dt1)/////判斷最後可以打卡的上班時間
                                            {
                                                a.EmployeeID = EmpID;
                                                a.Date = today;
                                                a.OnDuty = DateTime.Now.AddHours(8);
                                                a.savehours = 4;////打半天卡給四小時
                                                d.Attendances.Add(a);
                                                try
                                                {
                                                    d.SaveChanges();
                                                    var flex_checkin = $@"[{{""type"": ""flex"",""altText"":""上班打卡成功"",""contents"":{{""type"": ""bubble"",""body"": {{""type"": ""box"",""layout"": ""vertical"",
                                                                       ""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",
                                                                       ""contents"": [],""size"": ""xl"",""wrap"": true,""text"": ""上班打卡成功"",""color"": ""#ffffff"",""weight"": ""bold""}}],""spacing"": ""sm""}},
                                                                       {{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",
                                                                       ""contents"": [],""size"": ""sm"",""wrap"": true,""margin"": ""lg"",""color"": ""#ffffffde"",""text"": ""{DateTime.Now.AddHours(8).ToString()}""
                                                                       }}]}}],""paddingAll"": ""13px"",""backgroundColor"": ""#ffffff1A"",""cornerRadius"": ""2px"",""margin"": ""xl""}}]}}],""paddingAll"": ""20px"",
                                                                       ""backgroundColor"": ""#464F69""}}}}}}]";
                                                    //this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.AddHours(8).ToString()}");
                                                    this.ReplyMessageWithJSON(LineEvent.replyToken, flex_checkin);
                                                }
                                                catch
                                                {
                                                    var query = d.Attendances.Where(p => p.EmployeeID == EmpID && p.Date == today && p.OnDuty != null).FirstOrDefault();
                                                    if (query == null)
                                                    {
                                                        this.ReplyMessage(LineEvent.replyToken, "11111111111111");
                                                    }
                                                    else
                                                    {
                                                        var flex_errormsg = $@"[{{""type"": ""flex"",""altText"":""打卡失敗"",""contents"":{{""type"": ""bubble"",""body"": {{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{
                                                                            ""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""contents"": [],""size"": ""xl"",
                                                                            ""wrap"": true,""text"": ""打卡失敗"",""color"": ""#ffffff"",""weight"": ""bold""}}],""spacing"": ""sm""}},{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{
                                                                            ""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""contents"": [],""size"": ""sm"",""wrap"": true,""margin"": ""lg"",""color"": ""#ffffffde"",
                                                                            ""text"": ""本日已在{query.OnDuty}打過上班卡""}}]}}],""paddingAll"": ""13px"",""backgroundColor"": ""#ffffff1A"",""cornerRadius"": ""2px"",""margin"": ""xl""}}]}}],""paddingAll"": ""20px"",
                                                                            ""backgroundColor"": ""#D75455""}}}}}}]";
                                                        //this.ReplyMessage(LineEvent.replyToken, "已經在" + $"{query.OnDuty}打過卡了");
                                                        this.ReplyMessageWithJSON(LineEvent.replyToken, flex_errormsg);
                                                    }

                                                }


                                                return Ok();
                                            }
                                            else
                                            {
                                                this.ReplyMessage(LineEvent.replyToken, "已超過可打上班卡的時間");
                                            }
                                        }
                                        else
                                        {
                                            this.ReplyMessage(LineEvent.replyToken, "超出可打卡範圍");
                                        }
                                    }
                                    catch
                                    {
                                        this.ReplyMessage(LineEvent.replyToken, "請先確認地點後打卡");

                                    }
                                    break;
                                case "下班":
                                    try
                                    {
                                        if (DateTime.Now > dt2)
                                        {
                                            var query = d.Attendances.Where(p => p.EmployeeID == EmpID && p.Date == today && p.OnDuty != null).First();
                                            query.OffDuty = DateTime.Now.AddHours(8);
                                            query.savehours = 8;
                                            //this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.AddHours(8).ToString()}");
                                            d.SaveChanges();
                                            var flex_checkout = $@"[{{""type"": ""flex"",""altText"":""下班打卡成功"",""contents"":{{""type"": ""bubble"",""body"": {{""type"": ""box"",""layout"": ""vertical"",
                                                                ""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""contents"": [],
                                                                ""size"": ""xl"",""wrap"": true,""text"": ""下班打卡成功"",""color"": ""#ffffff"",""weight"": ""bold""}}],""spacing"": ""sm""}},{{""type"": ""box"",
                                                                ""layout"": ""vertical"",""contents"": [{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""contents"": [],""size"": ""sm"",
                                                                ""wrap"": true,""margin"": ""lg"",""color"": ""#ffffffde"",""text"": ""{DateTime.Now.AddHours(8).ToString()}""}}]}}],""paddingAll"": ""13px"",""backgroundColor"": ""#ffffff1A"",
                                                                ""cornerRadius"": ""2px"",""margin"": ""xl""}}]}}],""paddingAll"": ""20px"",""backgroundColor"": ""#464F69""}}}}}}]";
                                            this.ReplyMessageWithJSON(LineEvent.replyToken, flex_checkout);
                                        }
                                        else
                                        {
                                            this.ReplyMessage(LineEvent.replyToken, "尚未到可打下班卡的時間");
                                        }
                                    }

                                    catch
                                    {
                                        //this.ReplyMessage(LineEvent.replyToken, "小幫手提醒您,今天上班未打卡,請申請補打卡!");
                                        //var bot = new Bot(channelAccessToken);
                                        var flex_nonecheck = @"[{""type"": ""flex"",""altText"":""打卡失敗"",""contents"":{""contents"": [{""type"": ""bubble"",""body"": {""type"": ""box"",""layout"": ""vertical"",
                                                             ""contents"": [{ ""type"": ""box"",""layout"": ""vertical"",""contents"": [{""type"": ""box"",""layout"": ""vertical"",""contents"": [{""type"": ""text"",
                                                             ""text"": ""打卡失敗"",""size"": ""xl"",""color"": ""#ffffff"",""weight"": ""bold"",""wrap"": true},{""type"": ""box"",""layout"": ""vertical"",""contents"": [
                                                             {""type"": ""text"",""text"": ""小幫手提醒您,今天上班未打卡,請申請補打卡!"",""margin"": ""lg"",""color"": ""#ffffffde"",""size"": ""sm"",""wrap"": true}],""margin"": ""xl"",
                                                             ""backgroundColor"": ""#ffffff1A"",""cornerRadius"": ""2px"",""paddingAll"": ""13px""}],""spacing"": ""sm""}]}],""paddingAll"": ""20px"",""backgroundColor"": ""#D75455""
                                                             },""footer"": {""type"": ""box"",""layout"": ""vertical"",""contents"": [{""type"": ""button"",""action"": {""type"": ""uri"",""label"": ""補打卡申請"",
                                                             ""uri"": ""line://app/1653574503-ljRK1nJ8""},""style"": ""primary"",""color"": ""#5DAC81""}]}}]}}]";


                                        a.EmployeeID = EmpID;
                                        a.Date = today;
                                        a.OffDuty = DateTime.Now.AddHours(8);
                                        a.station = "上班未打卡";
                                        d.Attendances.Add(a);
                                        try
                                        {
                                            d.SaveChanges();
                                            bot.PushMessageWithJSON(AdminUserId, flex_nonecheck);
                                        }
                                        catch
                                        {
                                            bot.PushMessageWithJSON(AdminUserId, flex_nonecheck);
                                        }
                                        //List<TemplateActionBase> actions = new List<TemplateActionBase>();

                                        //actions.Add(new UriAction() { label = "申請", uri = new Uri("line://app/1612776942-bm461AoW") });
                                        //actions.Add(new MessageAction() { label = "取消", text = "取消" });

                                        //var ButtonTempalteMsg = new isRock.LineBot.ConfirmTemplate()
                                        //{
                                        //    text = "小幫手提醒您,今天上班未打卡,請申請補打卡!",
                                        //    altText = "請在手機上檢視",
                                        //    actions = actions
                                        //};
                                        //bot.PushMessage(AdminUserId, ButtonTempalteMsg);
                                    }
                                    break;
                                case "補打卡申請":
                                    {

                                        var flexsubmit = @"[{""type"": ""flex"",""altText"":""缺勤紀錄"",""contents"":{""type"": ""bubble"",""body"": {""type"": ""box"",
                                                         ""layout"": ""vertical"",""contents"": [{""type"": ""text"",""text"": ""hello, world""},{""type"": ""button"",
                                                         ""action"": {""type"": ""datetimepicker"",""label"": ""action"",""data"": ""hello"",""mode"": ""date""}}],
                                                         ""action"": {""type"": ""datetimepicker"",""label"": ""action"",""data"": ""hello"",""mode"": ""date""}}}}]";
                                        bot.PushMessageWithJSON(AdminUserId, flexsubmit);
                                    }
                                    break;
                                case "出勤":
                                    {
                                        var bot1 = new Bot(channelAccessToken);
                                        int count = 0;
                                        string[] record = new string[5] { "無紀錄", "無紀錄", "無紀錄", "無紀錄", "無紀錄" };
                                        var searchfive = d.Attendances.Where(p => p.EmployeeID == EmpID && p.station == "未打卡");
                                        int takefive;

                                        if (searchfive.Count() <= 5)
                                        {
                                            takefive = searchfive.Count();
                                        }
                                        else
                                        {
                                            takefive = 5;
                                        }
                                        foreach (var xxxx in searchfive.Take(takefive))
                                        {
                                            record[count] = xxxx.Date.ToLongDateString().ToString();
                                            count++;
                                        };

                                        var flextakefive = $@"[{{""type"": ""flex"",""altText"":""缺勤紀錄"",""contents"":{{""type"": ""bubble"",""header"": {{""type"": ""box"",""layout"": ""vertical"",
                                                           ""contents"": [{{""type"": ""text"",""text"": ""缺勤紀錄"",""size"": ""xxl"",""weight"": ""bold"",""align"": ""center"",""color"": ""#FFFFFB""
                                                           }}],""backgroundColor"": ""#464F69""}},""body"": {{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""box"",""layout"": ""vertical"",
                                                           ""margin"": ""xxl"",""spacing"": ""sm"",""contents"": [{{""type"": ""box"",""layout"": ""horizontal"",""contents"": [{{""type"": ""text"",""text"": ""未打卡紀錄(最近五筆)"",
                                                           ""size"": ""lg"",""weight"": ""bold""}}]}},{{""type"": ""separator""}},{{""type"": ""box"",""layout"": ""horizontal"",""contents"": [{{""type"": ""text"",
                                                           ""text"": ""{ record[0]}""}}]}},{{""type"": ""separator""}},{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""text"": ""{record[1]}""
                                                           }}]}},{{""type"": ""separator""}},{{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""text"": ""{record[2]}""}}]}},{{""type"": ""separator""
                                                           }}, {{""type"": ""box"",""layout"": ""vertical"",""contents"": [{{""type"": ""text"",""text"": ""{record[3]} ""}}]}},{{""type"": ""separator""}},{{""type"": ""box"",""layout"": ""vertical"",
                                                           ""contents"": [{{""type"": ""text"",""text"": ""{record[4]} ""}}]}}]}},{{""type"": ""separator""}}]}},""footer"": {{""type"": ""box"",""layout"": ""vertical"",""contents"": [
                                                           {{""type"": ""button"",""action"": {{""type"": ""uri"",""label"": ""補打卡申請"",""uri"": ""line://app/1653574503-ljRK1nJ8""}},""height"": ""sm"",""style"": ""primary"",""color"": ""#D75455""
                                                           }}]}},""styles"": {{""footer"": {{""separator"": true}}}}}}}}]";

                                        bot1.PushMessageWithJSON(AdminUserId, flextakefive);
                                    }
                                    break;
                                #region Line請假回訊息區
                                //case "請假":
                                //    {
                                //        var bot2 = new Bot(channelAccessToken);
                                //        List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                                //        actions2.Add(new MessageAction() { label = "請假申請", text = "請假申請" });
                                //        actions2.Add(new MessageAction() { label = "請假查詢", text = "請假查詢" });
                                //        actions2.Add(new MessageAction() { label = "text請假", text = "我要請假" });
                                //        var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                                //        {
                                //            text = "請假",
                                //            altText = "請在手機上檢視",
                                //            thumbnailImageUrl = new Uri("https://imgs.fun1shot.com/21d825754c12110fc0b5cc2778e7bc99.jpg"),
                                //            actions = actions2
                                //        };
                                //        bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);


                                //        break;

                                //    }
                                //case "請假申請":
                                //    {
                                //        var bot2 = new Bot(channelAccessToken);
                                //        List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                                //        //actions2.Add(new UriAction() { label = "請假", uri = new Uri("line://app/1612776942-bm461AoW") });
                                //        //actions2.Add(new UriAction() { label = "查詢", uri = new Uri("line://app/1612776942-9MbWYMog") });
                                //        actions2.Add(new DateTimePickerAction() { label = "請假開始時間", mode = "datetime" });
                                //        actions2.Add(new DateTimePickerAction() { label = "請假開始結束", mode = "datetime" });
                                //        actions2.Add(new MessageAction() { label = "假別", text = "假別" });
                                //        var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                                //        {
                                //            text = "請假申請",
                                //            altText = "請在手機上檢視",
                                //            thumbnailImageUrl = new Uri("https://pgw.udn.com.tw/gw/photo.php?u=https://uc.udn.com.tw/photo/2019/01/26/99/5848687.jpg&x=0&y=0&sw=0&sh=0&exp=3600"),
                                //            actions = actions2
                                //        };
                                //        bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);
                                //        break;

                                //    }
                                //case "假別":
                                //    {
                                //        TextMessage msg = new TextMessage("請問你要請什麼假?");
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("事假", "事假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("病假", "病假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("公假", "公假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("喪假", "喪假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("特休假", "特休假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("產假", "產假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("陪產假", "陪產假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("生理假", "生理假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("補休假", "補休假"));
                                //        msg.quickReply.items.Add(new QuickReplyMessageAction("家庭照顧假", "家庭照顧假"));
                                //        bot.PushMessage(AdminUserId, msg);
                                //        break;
                                //    }
                                #endregion
                                default:
                                    //Bot bots = new Bot(ChannelAccessToken);
                                    //TextMessage msgs = new TextMessage("我看不懂");
                                    //bots.PushMessage(AdminUserId, msgs);
                                    break;

                            }
                        }

                        string Lineallstring = "";//設定空字串給最後要回傳的全部資訊


                        var LineMsg = LineEvent.message.text;
                        if (LineMsg == "事假" || LineMsg == "病假" || LineMsg == "公假" || LineMsg == "喪假" || LineMsg == "特休假" || LineMsg == "產假" || LineMsg == "陪產假" || LineMsg == "生理假" || LineMsg == "補休假" || LineMsg == "家庭照顧假")
                        {
                            var bot2 = new Bot(channelAccessToken);
                            List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                            actions2.Add(new DateTimePickerAction() { label = "請假開始時間", mode = "datetime", data = "data" });
                            actions2.Add(new DateTimePickerAction() { label = "請假開始結束", mode = "datetime", data = "data1" });
                            var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                            {
                                text = "請假申請",
                                altText = "請在手機上檢視",
                                thumbnailImageUrl = new Uri("https://i.imgur.com/n19hgxT.gif"),//https://i.imgur.com/Hl9Is3f.gif
                                actions = actions2
                            };
                            bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);

                            Lineallstring += $"申請時間:{DateTime.Now}\n假別:{LineMsg}\n";//把假別加入最後要回傳的字串中

                        }






                        if (LineEvent.message.text == "假別")
                        {
                            TextMessage msg = new TextMessage("請問你要請什麼假?");
                            msg.quickReply.items.Add(new QuickReplyMessageAction("事假", "事假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("病假", "病假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("公假", "公假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("喪假", "喪假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("特休假", "特休假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("產假", "產假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("陪產假", "陪產假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("生理假", "生理假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("補休假", "補休假"));
                            msg.quickReply.items.Add(new QuickReplyMessageAction("家庭照顧假", "家庭照顧假"));
                            bot.PushMessage(AdminUserId, msg);

                        }

                        if (LineEvent.type == "postback")//回傳datetimepickper的值
                        {
                            if (LineEvent.postback.data == "data")
                            {
                                string aa = $"開始時間{this.ReceivedMessage.events[0].postback.Params.datetime}";
                                this.ReplyMessage(LineEvent.replyToken, aa);
                                Lineallstring += $"開始時間{this.ReceivedMessage.events[0].postback.Params.datetime}\n";//把開始時間加入最後要回傳的字串中
                            }
                            if (LineEvent.postback.data == "data1")
                            {
                                string aa = $"結束時間{this.ReceivedMessage.events[0].postback.Params.datetime}";
                                this.ReplyMessage(LineEvent.replyToken, aa);
                                Lineallstring += $"結束時間{this.ReceivedMessage.events[0].postback.Params.datetime}\n";//把結束時間加入最後要回傳的字串中





                            }
                        }



                        if (LineEvent.message.text == "我要請假")
                        {
                            //定義資訊蒐集者
                            InformationCollector<LeaveRequstLine> CIC = new InformationCollector<LeaveRequstLine>(ChannelAccessToken);

                            //取得 http Post RawData(should be JSON)
                            string postData = Request.Content.ReadAsStringAsync().Result;
                            //剖析JSON
                            var ReceivedMessage = Utility.Parsing(postData);
                            //定義接收CIC結果的類別
                            ProcessResult<LeaveRequstLine> result;
                            try
                            {
                                switch (LineEvent.message.text)
                                {
                                    #region Line請假回訊息區
                                    case "我要請假":
                                        {
                                            var bot2 = new Bot(channelAccessToken);
                                            List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                                            actions2.Add(new MessageAction() { label = "假別", text = "假別" });
                                            var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                                            {
                                                text = "請假",
                                                altText = "請在手機上檢視",
                                                thumbnailImageUrl = new Uri("https://i.imgur.com/LrT45vi.gif"),//https://i.imgur.com/n19hgxT.gif
                                                actions = actions2
                                            };
                                            bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);


                                            break;

                                        }
                                    case "請假申請":
                                        {
                                            //var bot2 = new Bot(channelAccessToken);
                                            //List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                                            //actions2.Add(new UriAction() { label = "請假", uri = new Uri("line://app/1612776942-bm461AoW") });
                                            //actions2.Add(new UriAction() { label = "查詢", uri = new Uri("line://app/1612776942-9MbWYMog") });
                                            //////////////////
                                            //actions2.Add(new DateTimePickerAction() { label = "請假開始時間", mode = "datetime" });
                                            //actions2.Add(new DateTimePickerAction() { label = "請假開始結束", mode = "datetime" });
                                            //actions2.Add(new MessageAction() { label = "假別", text = "假別" });
                                            //var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                                            //{
                                            //    text = "請假申請",
                                            //    altText = "請在手機上檢視",
                                            //    thumbnailImageUrl = new Uri("https://pgw.udn.com.tw/gw/photo.php?u=https://uc.udn.com.tw/photo/2019/01/26/99/5848687.jpg&x=0&y=0&sw=0&sh=0&exp=3600"),
                                            //    actions = actions2
                                            //};
                                            //bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);
                                            break;

                                        }
                                    case "假別":
                                        {
                                            TextMessage msg = new TextMessage("請問你要請什麼假?");
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("事假", "事假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("病假", "病假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("公假", "公假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("喪假", "喪假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("特休假", "特休假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("產假", "產假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("陪產假", "陪產假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("生理假", "生理假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("補休假", "補休假"));
                                            msg.quickReply.items.Add(new QuickReplyMessageAction("家庭照顧假", "家庭照顧假"));
                                            bot.PushMessage(AdminUserId, msg);
                                            break;
                                        }


                                        #endregion


                                }



                                if (ReceivedMessage.events[0].message.text == "我要請假")
                                {
                                    //把訊息丟給CIC 
                                    result = CIC.Process(ReceivedMessage.events[0], true);
                                    responseMsg = "開始請假程序\n";
                                }
                                else
                                {
                                    //把訊息丟給CIC 
                                    result = CIC.Process(ReceivedMessage.events[0]);
                                }


                                if (ReceivedMessage.events[0].postback != null)
                                {
                                    bot.PushMessage(AdminUserId, ReceivedMessage.events[0].postback.Params.datetime);
                                }


                                //處理 CIC回覆的結果
                                //switch (result.ProcessResultStatus)
                                //{
                                //    case ProcessResultStatus.Processed:
                                //        //取得候選訊息發送
                                //        responseMsg += result.ResponseMessageCandidate;
                                //        break;
                                //    case ProcessResultStatus.Done:
                                //        var Linetext = result.ConversationState.ConversationEntity;

                                //        responseMsg += result.ResponseMessageCandidate;
                                //        responseMsg += $"\n您申請的請假資訊為\n~~~~以下~~~~~\n";
                                //        responseMsg += $"請假申請時間:\n{DateTime.Now}\n請假假別:\n{Linetext.假別}\n請假結束日期:\n{ Linetext.請假開始日期}\n請假開始日期\n{Linetext.請假結束日期}\n~~~~~~~~~~\n以為您送出待審核" ;
                                //        LeaveRequests Leave = new LeaveRequests();
                                //        break;
                                //    case ProcessResultStatus.Pass:
                                //        responseMsg = $"你說的 '{ReceivedMessage.events[0].message.text}' 我看不懂，如果想要請假，請跟我說 : 『我要請假』";
                                //        break;
                                //    case ProcessResultStatus.Exception:
                                //        //取得候選訊息發送
                                //        responseMsg += result.ResponseMessageCandidate;
                                //        break;
                                //    case ProcessResultStatus.Break:
                                //        //取得候選訊息發送
                                //        responseMsg += result.ResponseMessageCandidate;
                                //        break;
                                //    case ProcessResultStatus.InputDataFitError:
                                //        responseMsg += result.ResponseMessageCandidate;
                                //        responseMsg += "\n時間資料型態請以24h制為準\nex:2020/12/25 18:00";

                                //        break;
                                //    default:
                                //        //取得候選訊息發送
                                //        responseMsg += result.ResponseMessageCandidate;
                                //        break;
                                //        }

                                //回覆用戶訊息
                                isRock.LineBot.Utility.ReplyMessage(ReceivedMessage.events[0].replyToken, responseMsg, ChannelAccessToken);
                                //回覆API OK
                                return Ok();
                            }
                            catch (Exception ex)
                            {
                                //... 略 ...
                                this.ReplyMessage(LineEvent.replyToken, ex.Message + ""); //回傳錯誤之後須刪除
                                return Ok();
                            }

                        }


                    }





                }

                catch (Exception ex)
                {
                    this.ReplyMessage(LineEvent.replyToken, $"{ex.Message}....您不是本公司員工,無法使用打卡功能!");
                }
            }
            else//正
            {
                this.ReplyMessage(LineEvent.replyToken, "請先綁定帳號!");

            }
            //response OK
            return Ok();

        }
    }
    //line 請假回復的順序
    public class LeaveRequstLine : ConversationEntity
    {

        [Question("請問您要請假的假別是?\nex:事假.病假...等等")]
        [Order(1)]
        public string 假別
        { get; set; }

        [Question("請問您要請假開始日期是?")]
        [Order(2)]
        public DateTime 請假開始日期 { get; set; }

        [Question("請問您要請假結束日期是?")]
        [Order(3)]
        public DateTime 請假結束日期 { get; set; }

    }

}


