using AMS.Models;
using isRock.LineBot;
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


namespace WebApplication5.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
       public const string channelAccessToken = @"ehC2bzsC2xmmwK5J59gcEK4ihHfRlYfb8kQFxVR2jn0B9vlAtMfvAwXXn5KfJfeQlC+5Higk86SmFJkwGn3bwDHH1uvL2X4vwahMbdMCeIFJttH9jNekMNBw6RHL0hJaQq2oEDSKKf0ocx3CQTFaO1GUYhWQfeY8sLGRXgo3xvw=";
        public string AdminUserId ;
         
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
                        string st1 = "16:00";//設定最晚打卡的上班時間   
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

                            }
                            //if (LineEvent.message.text == "請假")//新增MessageAction請假
                            //{
                                
                            //    var bot = new Bot(channelAccessToken);
                            //    List<TemplateActionBase> actions = new List<TemplateActionBase>();
                            //    this.ReplyMessage(LineEvent.replyToken, $"你好,{q.EmployeeName}");
                            //    actions.Add(new MessageAction() { label = "請假", text = "請假" });

                            //    var ButtonTempalteMsg = new isRock.LineBot.ConfirmTemplate()
                            //    {
                            //        text = "請假",
                            //        altText = "請在手機上檢視",
                            //        actions = actions
                            //    };
                            //    bot.PushMessage(AdminUserId, ButtonTempalteMsg);
                            //}

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
                                #region Line請假回訊息區
                                case "請假":
                                    {
                                        var bot2 = new Bot(channelAccessToken);
                                        List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                                        //actions2.Add(new UriAction() { label = "請假", uri = new Uri("line://app/1612776942-bm461AoW") });
                                        //actions2.Add(new UriAction() { label = "查詢", uri = new Uri("line://app/1612776942-9MbWYMog") });
                                        actions2.Add(new MessageAction() { label = "請假申請", text = "請假申請" });
                                        actions2.Add(new MessageAction() { label = "請假查詢", text = "請假查詢" });
                                        actions2.Add(new MessageAction() { label = "測試請假", text = "測試請假" });
                                        var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                                        {
                                            text = "請假",
                                            altText = "請在手機上檢視",
                                            thumbnailImageUrl = new Uri("https://imgs.fun1shot.com/21d825754c12110fc0b5cc2778e7bc99.jpg"),
                                            actions = actions2
                                        };
                                        bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);
                                        break;

                                    }

                                case "測試請假":
                                    {
                                      
                                        break;

                                    }


                                case "請假申請":
                                    {
                                        var bot2 = new Bot(channelAccessToken);
                                        List<TemplateActionBase> actions2 = new List<TemplateActionBase>();

                                        //actions2.Add(new UriAction() { label = "請假", uri = new Uri("line://app/1612776942-bm461AoW") });
                                        //actions2.Add(new UriAction() { label = "查詢", uri = new Uri("line://app/1612776942-9MbWYMog") });
                                        actions2.Add(new DateTimePickerAction() { label = "請假開始時間", mode = "datetime" });
                                        actions2.Add(new DateTimePickerAction() { label = "請假開始結束", mode = "datetime" });
                                        actions2.Add(new MessageAction() { label = "假別", text = "假別" });
                                        var ButtonTempalteMsg2 = new isRock.LineBot.ButtonsTemplate()
                                        {
                                            text = "請假申請",
                                            altText = "請在手機上檢視",
                                            thumbnailImageUrl = new Uri("https://pgw.udn.com.tw/gw/photo.php?u=https://uc.udn.com.tw/photo/2019/01/26/99/5848687.jpg&x=0&y=0&sw=0&sh=0&exp=3600"),
                                            actions = actions2
                                        };
                                        bot2.PushMessage(AdminUserId, ButtonTempalteMsg2);
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
                                        Bot bot = new Bot(ChannelAccessToken);
                                        bot.PushMessage(AdminUserId, msg);
                                        break;
                                    }
                                    #endregion


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

    public class LeaveRequstLine : ConversationEntity
    {
        [Question("請問您要請假的假別是?")]
        [Order(1)]
        public string 假別 { get; set; }

        [Question("請問您要請假開始日期是?")]
        [Order(2)]
        public DateTime 請假開始日期 { get; set; }

        [Question("請問您要請假結束日期是?")]
        [Order(3)]
        public string 請假結束日期 { get; set; }

    }


}
