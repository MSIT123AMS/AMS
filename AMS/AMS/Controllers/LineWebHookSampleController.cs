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
        const string AdminUserId = "Uf4247e1d90dc9ebe3518605bae30392d";

        [Route("api/LineWebHookSample")]
        [HttpPost]
        public IHttpActionResult POST()
        {
            try
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
                    //if (LineEvent.message.type == "text") //收到文字
                    //    this.ReplyMessage(LineEvent.replyToken, "你說了:" + LineEvent.message.text);
                    //if (LineEvent.message.type == "sticker") //收到貼圖
                    //    this.ReplyMessage(LineEvent.replyToken, 1, 2);
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
                        if (LineEvent.message.text == "上班" || LineEvent.message.text == "下班")
                        {
                            this.ReplyMessage(LineEvent.replyToken, $"已打卡\n時間:{DateTime.Now.ToString()}");

                            Entities d = new Entities();


                            Attendances a = new Attendances
                            {
                                EmployeeID = "MSIT1230001",
                                Date = DateTime.Now.Date
                            };
                            if (LineEvent.message.text == "上班")
                            {
                                a.OnDuty = DateTime.Now;
                            }
                            else
                            {
                                a.OffDuty = DateTime.Now.ToString();
                            }

                            d.Attendances.Add(a);
                            d.SaveChanges();
                        }
                    }


                }
                //response OK
                return Ok();
            }
            catch (Exception ex)
            {
                //如果發生錯誤，傳訊息給Admin
                this.PushMessage(AdminUserId, "發生錯誤:\n" + ex.Message);
                //response OK
                return Ok();
            }
        }
    }
}
