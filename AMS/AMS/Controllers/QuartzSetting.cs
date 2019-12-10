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
        public const string channelAccessToken = @"ehC2bzsC2xmmwK5J59gcEK4ihHfRlYfb8kQFxVR2jn0B9vlAtMfvAwXXn5KfJfeQlC+5Higk86SmFJkwGn3bwDHH1uvL2X4vwahMbdMCeIFJttH9jNekMNBw6RHL0hJaQq2oEDSKKf0ocx3CQTFaO1GUYhWQfeY8sLGRXgo3xvw=";
        public static readonly string SchedulingStatus = ConfigurationManager.AppSettings["ExecuteTaskServiceCallSchedulingStatus"];
        LineBotWebHookController lineBot = new LineBotWebHookController();   
                     
        public Task Execute(IJobExecutionContext context)
        {
            var task = Task.Run(() =>
            {
                if (SchedulingStatus.Equals("ON"))
                {
                    Entities d = new Entities();                 
                    Attendances a = new Attendances();                   
                    lineBot.ChannelAccessToken = channelAccessToken;
                    //取得Line Event(範例，只取第一個)
                    //var LineEvent = lineBot.ReceivedMessage.events.FirstOrDefault();
                    //配合Line verify 
                    DateTime today = DateTime.Now.Date;//今天的日期  
                    DateTime yesterday = today.AddDays(-1);//////前天日期
                    var bot = new Bot(channelAccessToken);
                    try
                    {
                        var day_uncheck = (from e in d.Employees.AsEnumerable()/////////判斷前有無打卡
                                           join att in d.Attendances.AsEnumerable().Where(p => p.Date == yesterday) on e.EmployeeID equals att.EmployeeID into g
                                           from att in g.DefaultIfEmpty()
                                           select new SerchAttendancesViewModel
                                           {
                                               EmployeeName = e.EmployeeName,
                                               Date = att == null ? null : att.Date.ToString("yyyy/MM/dd"),
                                               StartTime = att == null ? null : att.OnDuty,
                                               EndTime = att == null ? null : att.OffDuty,
                                               LineID=e.LineID
                                           });
                        var fund_uncheck_emp = day_uncheck.Where(p => p.Date == null&&p.LineID!=null);
                        foreach (var all_emp_uncheck in fund_uncheck_emp)
                        {
                            bot.PushMessage(all_emp_uncheck.LineID, "哈哈哈哈哈哈哈阿");

                        }



                        using (var message = new MailMessage("wingrovepank@gmail.com", "hauwei.pong@gmail.com"))
                        {
                            message.Subject = "未打卡通知信";
                            //message.Body = $"你今天{},請申請補打卡";
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