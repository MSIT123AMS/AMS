using AMS.Controllers;
using AMS.Models;
using isRock.LineBot;
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
       public const string channelAccessToken = @"ehC2bzsC2xmmwK5J59gcEK4ihHfRlYfb8kQFxVR2jn0B9vlAtMfvAwXXn5KfJfeQlC+5Higk86SmFJkwGn3bwDHH1uvL2X4vwahMbdMCeIFJttH9jNekMNBw6RHL0hJaQq2oEDSKKf0ocx3CQTFaO1GUYhWQfeY8sLGRXgo3xvw=";
        public string AdminUserId ;
         
        [Route("api/LineWebHookSample")]
        [HttpPost]
        public IHttpActionResult POST(Position position)
        {
          
            return Json(position);
        }
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
         
           
          
            if (d.Employees.Where(p => p.LineID == AdminUserId).FirstOrDefault()!=null)//正
            {
               
                try
                {
                    var q = d.Employees.Where(p => p.LineID == AdminUserId).First();
                    var EmpID = q.EmployeeID;//正
                  
                    if (LineEvent.type == "message")
                    {

                        //取得用戶資訊
                        //string EmpId = "MSIT1230015";//測試用用戶
                        string st1 = "16:00";//設定最晚打卡的上班時間(可以擺到人事) ///要再做每日統計上班時數功能  
                        DateTime dt1 = Convert.ToDateTime(st1);
                        DateTime todate = DateTime.Now.Date;//今天的日期

                        if (LineEvent.message.type == "text")
                        {
                            if (LineEvent.message.text == "打卡")
                            {
                                var bot = new Bot(channelAccessToken);
                                List<TemplateActionBase> actions = new List<TemplateActionBase>();
                                this.ReplyMessage(LineEvent.replyToken, $"你好,{q.EmployeeName}");
                                actions.Add(new MessageAction() { label = "上班", text = "上班" });
                                actions.Add(new MessageAction() { label = "下班", text = "下班" });

                                var ButtonTempalteMsg = new isRock.LineBot.ConfirmTemplate()
                                {
                                    text = "打卡",
                                    altText = "請在手機上檢視",
                                    actions = actions
                                };
                                bot.PushMessage(AdminUserId, ButtonTempalteMsg);
                                //return RedirectToRoute("Index", "Home");
                        }                     
                        
                        switch (LineEvent.message.text)
                        {

                                case "上班":
                                    if (DateTime.Now < dt1)
                                    {
                                        a.EmployeeID = EmpID;
                                        a.Date = todate;
                                        a.OnDuty = DateTime.Now.AddHours(8);
                                        d.Attendances.Add(a);
                                        try
                                        {
                                            d.SaveChanges();
                                            this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.AddHours(8).ToString()}");
                                        }
                                        catch
                                        {
                                            var query = d.Attendances.Where(p => p.EmployeeID == EmpID && p.Date == todate && p.OnDuty != null).First();
                                            this.ReplyMessage(LineEvent.replyToken, "已經在" + $"{query.OnDuty}打過卡了");
                                        }


                                        return Ok();
                                    }
                                    else
                                    {
                                        this.ReplyMessage(LineEvent.replyToken, "已超過可打卡的時間");
                                    }
                                    break;
                                case "下班":
                                    try
                                    {
                                        var query = d.Attendances.Where(p => p.EmployeeID == EmpID && p.Date == todate && p.OnDuty != null).First();
                                        query.OffDuty = DateTime.Now.AddHours(8);

                                        this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.AddHours(8).ToString()}");

                                        d.SaveChanges();

                                    }

                                    catch
                                    {
                                        //this.ReplyMessage(LineEvent.replyToken, "小幫手提醒您,今天上班未打卡,請申請補打卡!");
                                        var bot = new Bot(channelAccessToken);
                                        List<TemplateActionBase> actions = new List<TemplateActionBase>();

                                        actions.Add(new UriAction() { label = "申請", uri = new Uri("line://app/1612776942-bm461AoW") });
                                        actions.Add(new MessageAction() { label = "取消", text = "取消" });

                                        var ButtonTempalteMsg = new isRock.LineBot.ConfirmTemplate()
                                        {
                                            text = "小幫手提醒您,今天上班未打卡,請申請補打卡!",
                                            altText = "請在手機上檢視",
                                            actions = actions
                                        };
                                        bot.PushMessage(AdminUserId, ButtonTempalteMsg);
                                    }

                                    break;

                                    case "出勤":
                                    {
                                        var bot1 = new Bot(channelAccessToken);
                                        List<TemplateActionBase> actions1 = new List<TemplateActionBase>();

                                        actions1.Add(new UriAction() { label = "補打卡", uri = new Uri("line://app/1612776942-bm461AoW") });
                                        actions1.Add(new UriAction() { label = "查詢", uri = new Uri("line://app/1612776942-9MbWYMog") });

                                        var ButtonTempalteMsg1 = new isRock.LineBot.ConfirmTemplate()
                                        {
                                            text = "出勤",
                                            altText = "請在手機上檢視",
                                            actions = actions1
                                        };
                                        bot1.PushMessage(AdminUserId, ButtonTempalteMsg1);

                                        
                                    }
                                    break;


                            }





                        }


                    }
                }
                catch
                {
                    this.ReplyMessage(LineEvent.replyToken, "您不是本公司員工,無法使用打卡功能!");
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
}
