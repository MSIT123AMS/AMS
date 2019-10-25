using AMS.Models;
using isRock.LineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace WebApplication5.Controllers
{
    public class LineBotWebHookController : isRock.LineBot.LineWebHookControllerBase
    {
        const string channelAccessToken = @"wJvLiDuDsJpYsgTqSPXQwu35UoXbtmVPXn8Q1/oWN8REU5mbLG0qBffnpgSlNWH3yncYUa3OAgyWoe8gPb8F1nFveUGakkBJ2UHqUKSXElkHhypyGWz7Ndhojww+2P0+ikiFbIIkz6nhMQwetqG1gwdB04t89/1O/w1cDnyilFU=
";
         string AdminUserId ;

        [Route("api/LineWebHookSample")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            
                //設定ChannelAccessToken(或抓取Web.Config)
                this.ChannelAccessToken = channelAccessToken;
                //取得Line Event(範例，只取第一個)
                var LineEvent = this.ReceivedMessage.events.FirstOrDefault();
                //配合Line verify 
                if (LineEvent.replyToken == "00000000000000000000000000000000") return Ok();
                //回覆訊息
                if (LineEvent.type == "message")
                {
                    this.AdminUserId = this.ReceivedMessage.events.FirstOrDefault().source.userId;//取得用戶資訊
                    //string EmpId = "MSIT1230015";//測試用用戶
                    string st1 = "10:00";//設定最晚打卡的上班時間   
                    DateTime dt1 = Convert.ToDateTime(st1);
                    DateTime todate = DateTime.Now.Date;//今天的日期
                    Entities d = new Entities();
                    Attendances a = new Attendances();
                    if (LineEvent.message.type == "text")
                    {
                        if (LineEvent.message.text == "打卡")
                        {
                            var bot = new Bot(channelAccessToken);
                            List<TemplateActionBase> actions = new List<TemplateActionBase>();
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
                        
                        
                        switch (LineEvent.message.text)
                        {
                            
                            case "上班":
                                if(DateTime.Now < dt1)
                                {
                                    a.LineID=AdminUserId;
                                    a.Date = todate;
                                    a.Onduty = DateTime.Now;
                                    d.Attendances.Add(a);
                                try
                                {
                                    d.SaveChanges();
                                    this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.ToString()}");
                                }
                                catch
                                {
                                    var query = d.Attendances.Where(p => p.LineID==AdminUserId && p.Date == todate && p.Onduty != null).First();
                                    this.ReplyMessage(LineEvent.replyToken, "已經在" + $"{query.Onduty}打過卡了");
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
                                var query = d.Attendances.Where( p => p.LineID == AdminUserId && p.Date == todate && p.Onduty != null).First();
                                query.Offduty = DateTime.Now;

                                this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.ToString()}");

                                d.SaveChanges();

                            }

                            catch
                            {
                                this.ReplyMessage(LineEvent.replyToken, "小幫手提醒您,今天上班未打卡,請申請補打卡!");
                            }

                            break;

                    }





                }


            }
                //response OK
                return Ok();
            
            //catch (Exception ex)
            //{
            //    //如果發生錯誤，傳訊息給Admin
            //    this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
            //    //response OK
            //    return Ok();
            //}
        }
    }
}
