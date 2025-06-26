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
            var fileName = $"WorkSheet_{now:yyyyMMdd}.txt";
            LogExtension.WriteLog(filePath, fileName, $"**********   Start JobHostedService  **********");

            var _jobList = new List<JobTask>
            {
                new JobTask { TaskCode = "W_0009",TaskName = "TurnOn-ToolWorkSheet-00-09 ", RunAt = new TimeSpan(9, 0, 0), Status = true },
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
                var fileName = $"WorkSheet_{now:yyyyMMdd}.txt";

                var nextRun = now.Date.Add(job.RunAt);
                if (nextRun <= now) nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                LogExtension.WriteLog(filePath, fileName, $"[JOB] Lên lịch {nextRun:yyyy-MM-dd HH:mm:ss} để chạy task: {job.TaskName}");

                await Task.Delay(delay, ct);


                try
                {
                    var date = nextRun.Date;
                    LogExtension.WriteLog(filePath, fileName, $"[JOB] Bắt đầu chạy task: {job.TaskName} lúc {DateTime.Now}");

                    switch (job.TaskCode)
                    {
                        case "W_0009":
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_00_09(date.AddHours(0), date.AddHours(9));
                            break;
                        case "W_0912":
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_09_12(date.AddHours(9), date.AddHours(12));
                            break;
                        case "W_1224":
                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_12_24(date.AddHours(12), date.AddDays(1));
                            break;
                    }
                    LogExtension.WriteLog(filePath, fileName, $"[JOB] ✅✅✅ Hoàn tất: {job.TaskName} ✅✅✅ ");
                }
                catch (Exception ex)
                {
                    LogExtension.WriteLog(filePath, fileName, $"[JOB] ❌❌❌ Lỗi {job.TaskName}: {ex.Message}");
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.WhenAll(_scheduledTasks);
        }
    }

}


