using E_Common;
using E_Contract.Service;
using E_Model.Response.HostedService;
using Microsoft.Extensions.Hosting;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        

namespace E_Service.Hosted
{
    public class JobHostedService : IHostedService
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly List<Task> _scheduledTasks = new();

        public JobHostedService(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var filePath = "/LogFile/HostedService/WorkSheet";
            var fileName = $"WorkSheet_{now:yyyyMM}.txt";
            LogExtension.WriteLog(filePath, fileName, $"**********   Start JobHostedService  **********");

            var _jobList = new List<JobTask>
            {
                new JobTask { TaskCode = "W_0009",TaskName = "TurnOn-ToolWorkSheet-00-09 ", RunAt = new TimeSpan(8, 14, 00), Status = true },
                new JobTask { TaskCode = "W_0009",TaskName = "TurnOn-ToolWorkSheet-00-09 ", RunAt = new TimeSpan(8, 59, 00), Status = true },
                new JobTask { TaskCode = "W_0912",TaskName = "TurnOn-ToolWorkSheet-09-12 ", RunAt = new TimeSpan(12, 00, 0), Status = true },
                new JobTask { TaskCode = "W_1224",TaskName = "TurnOn-ToolWorkSheet-12-24 ", RunAt = new TimeSpan(23, 59, 59), Status = true }
            };

            foreach (var job in _jobList.Where(j => j.Status))
            {
                _scheduledTasks.Add(ScheduleJob(job, cancellationToken));
            }

            return Task.CompletedTask;
        }

        private async Task ScheduleJob(JobTask job, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var now = DateTime.Now;

                var filePath = "/LogFile/HostedService/WorkSheet";
                var fileName = $"WorkSheet_{now:yyyyMM}.txt";

                var nextRun = now.Date.Add(job.RunAt);
                if (nextRun <= now) nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                LogExtension.WriteLog(filePath, fileName, $"[JOB] Calendar {nextRun:yyyy-MM-dd HH:mm:ss} --> run task: {job.TaskName}");

                await Task.Delay(delay, ct);


                try
                {
                    var date = nextRun.Date;
                    LogExtension.WriteLog(filePath, fileName, $"[JOB] Run task: {job.TaskName} at {DateTime.Now}");

                    switch (job.TaskCode)
                    {
                        case "W_0009":
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_00_09(date.AddHours(0), date.AddHours(9));
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_HRBarcode(date.AddHours(0), date.AddHours(9));
                            break;
                        case "W_0912":
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_09_12(date.AddHours(9), date.AddHours(12));
                            break;
                        case "W_1224":
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_12_24(date.AddHours(12), date.AddDays(1));
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_HRBarcode(date.AddHours(9), date.AddDays(1), 2);
                            break;
                    }
                    LogExtension.WriteLog(filePath, fileName, $"[JOB] ***** Completed: {job.TaskName} ***** ");
                }
                catch (Exception ex)
                {
                    LogExtension.WriteLog(filePath, fileName, $"[JOB] ❌❌❌ Error {job.TaskName}: {ex.Message}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var filePath = "/LogFile/HostedService/WorkSheet";
            var fileName = $"WorkSheet_{now:yyyyMM}.txt";
            LogExtension.WriteLog(filePath, fileName, $"**********   Stop JobHostedService at {now:yyyy-MM-dd HH:mm:ss}  **********");

            return Task.WhenAll(_scheduledTasks);
        }

    }

}


