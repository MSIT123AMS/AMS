using AMS.Models;
using isRock.LineBot;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using WebApplication5.Controllers;

namespace AMS.Controllers
{
    public class QuartzSetting
    {
    }
    public class ExecuteTaskServiceCallScheduler
    {
        private static readonly string ScheduleCronExpression = ConfigurationManager.AppSettings["ExecuteTaskScheduleCronExpression"];

        public static async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                var scheduler = await StdSchedulerFactory.GetDefaultScheduler();

                if (!scheduler.IsStarted)
                {
                    await scheduler.Start();
                }

                var job = JobBuilder.Create<ExecuteTaskServiceCallJob>()
                    .WithIdentity("ExecuteTaskServiceCallJob1", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("ExecuteTaskServiceCallTrigger1", "group1")
                    .WithCronSchedule(ScheduleCronExpression)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {

            }
        }
    }
    public class ExecuteTaskServiceCallJob : IJob
    {
        internal Entities db = new Entities();
        public const string channelAccessToken = @"wJvLiDuDsJpYsgTqSPXQwu35UoXbtmVPXn8Q1/oWN8REU5mbLG0qBffnpgSlNWH3yncYUa3OAgyWoe8gPb8F1nFveUGakkBJ2UHqUKSXElkHhypyGWz7Ndhojww+2P0+ikiFbIIkz6nhMQwetqG1gwdB04t89/1O/w1cDnyilFU=
";

        public static readonly string SchedulingStatus = ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    LineBotWebHookController LineBot = new LineBotWebHookController();
                    string EmployeeID = "MSIT1230005";
                    DateTime d = new DateTime();

                    //設定ChannelAccessToken(或抓取Web.Config)
                    LineBot.ChannelAccessToken = channelAccessToken;
                    //var query = db.Attendances.Where(p => p.Date == d.Date && p.EmployeeID == EmployeeID).First();
                    try
                    {

                        var bot = new Bot(channelAccessToken);
                        //var LineEvent = LineBot.ReceivedMessage.events.FirstOrDefault();
                        bot.PushMessage("U169cd14d449bd344525284f52fec1d6b", $"現在時間{DateTime.Now.AddHours(8)}");
                        //if(query.station!=null)
                        using (var message = new MailMessage("wingrovepank@gmail.com", "hauwei.pong@gmail.com"))
                        {
                            message.Subject = "Message Subject test";
                            message.Body = "Message body test at " + DateTime.Now;
                            using (SmtpClient client = new SmtpClient
                            {
                                EnableSsl = true,
                                Host = "smtp.gmail.com",
                                Port = 587,
                                Credentials = new NetworkCredential("wingrovepank@gmail.com", "sss22040")
                            })
                            {
                                client.Send(message);
                            }
                        } //Do whatever stuff you want
                    }
                    catch (Exception ex)
                    {

                    }

                }

            });

            return task;
        }

    }

}