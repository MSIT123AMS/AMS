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
        public const string channelAccessToken = @"RtEJui2f/Ks+t6cbPart9XZFjRexoyPj4IctMkIzd0WFeRh6NTD7AKXbKewGdV5A1R44By3Ij5dTqHofyQA9Dg9KDEON893Isff3290QoewsiREGNVMbsgP8je5DbDKZb1DMWqyrkJEt/lcTZfENNgdB04t89/1O/w1cDnyilFU=";
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
                        

                     
                    }
                    catch (Exception ex)
                    {
                        bot.PushMessage("Uc4c61b2e1e60a1dddbb5724258b9f359",ex.Message);
                    }

                }

            });

            return task;
        }

    }

   
}