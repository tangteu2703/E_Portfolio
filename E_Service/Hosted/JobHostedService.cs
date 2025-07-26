using E_Common;
using E_Contract.Service;
using E_Model.Table_SQL.Hosted;
using Microsoft.Extensions.Hosting;

namespace E_Service.Hosted
{
    public class JobHostedService : IHostedService
    {
        private readonly IServiceWrapper _serviceWrapper;
        private readonly List<Task> _scheduledTasks = new();
        private readonly HashSet<string> _runningJobKeys = new();
        private Timer? _refreshTimer;
        private const int REFRESH_INTERVAL_MINUTES = 3;

        public JobHostedService(IServiceWrapper serviceWrapper)
        {
            _serviceWrapper = serviceWrapper;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            string logPath = "/LogFile/HostedService/WorkSheet";
            string logFile = $"WorkSheet_{now:yyyyMM}.txt";
            LogExtension.WriteLog(logPath, logFile, $"***** [START] JobHostedService at {now:yyyy-MM-dd HH:mm:ss} *****");

            _refreshTimer = new Timer(async _ => await ScheduleJobsAsync(logPath, logFile, cancellationToken),
                null, TimeSpan.Zero, TimeSpan.FromMinutes(REFRESH_INTERVAL_MINUTES));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _refreshTimer?.Dispose();
            var now = DateTime.Now;
            string logPath = "/LogFile/HostedService/WorkSheet";
            string logFile = $"WorkSheet_{now:yyyyMM}.txt";
            LogExtension.WriteLog(logPath, logFile, $"***** [STOP] JobHostedService at {now:yyyy-MM-dd HH:mm:ss} *****");
            return Task.WhenAll(_scheduledTasks);
        }

        private async Task ScheduleJobsAsync(string logPath, string logFile, CancellationToken cancellationToken)
        {
            var now = DateTime.Now;
            var jobList = (await _serviceWrapper.TaskHistoried.SelectTaskActive())
                            .Where(x => x.task_date != null)
                            .OrderBy(x => x.task_date.Value)
                            .ToList();

            int overdueIndex = 2;

            foreach (var job in jobList)
            {
                string jobKey = $"{job.task_code}_{job.task_date:yyyyMMddHHmm}";

                if (_runningJobKeys.Contains(jobKey))
                    continue;

                _runningJobKeys.Add(jobKey);

                var taskTime = job.task_date.Value;
                TimeSpan delay;

                if (taskTime <= now)
                {
                    // Tăng dần 1 phút cho mỗi job quá hạn
                    delay = TimeSpan.FromMinutes(overdueIndex++);
                }
                else
                {
                    // Chờ đến đúng thời điểm
                    delay = taskTime - now;
                }

                DateTime runAt = DateTime.Now.Add(delay);
                LogExtension.WriteLog(logPath, logFile, $"[SCHEDULED] '{job.task_name}' ({jobKey}) will run at {runAt:yyyy-MM-dd HH:mm:ss} - delay {delay.TotalMinutes:F1} minutes.");

                var task = ExecuteJobAsync(job, delay, logPath, logFile, cancellationToken);
                _scheduledTasks.Add(task);
            }
        }

        private async Task ExecuteJobAsync(TaskHistoried job, TimeSpan delay, string logPath, string logFile, CancellationToken cancellationToken)
        {
            string jobKey = $"{job.task_code}_{job.task_date:yyyyMMddHHmm}";

            try
            {
                await Task.Delay(delay, cancellationToken);
                LogExtension.WriteLog(logPath, logFile, $"[RUNNING] '{job.task_name} : {job.task_date} with' ({jobKey}) at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                bool isSuccess = await RunJobByCode(job);

                if (isSuccess)
                {
                    LogExtension.WriteLog(logPath, logFile, $"********** [SUCCESS] '{job.task_name}: {job.task_date}' finished successfully.");

                    job.SetUpdateInfo("J05468");
                    await _serviceWrapper.TaskHistoried.UpdateAsync(job, "E_PortalConnection"); // update is_active = 1 đã chạy và thêm task mới + 1 day

                    // 🔄 Select lại để lấy danh sách mới và lên lịch nếu có task mới
                    await ScheduleJobsAsync(logPath, logFile, cancellationToken);
                }
                else
                {
                    LogExtension.WriteLog(logPath, logFile, $"xxxxxxxxxx [FAILED] '{job.task_name} : {job.task_date}' execution failed.");
                }
            }
            catch (Exception ex)
            {
                LogExtension.WriteLog(logPath, logFile, $"xxxxxxxxxx [ERROR] '{job.task_name} : {job.task_date}' failed: {ex.Message}");
            }
            finally
            {
                _runningJobKeys.Remove(jobKey);
            }
        }

        private async Task<bool> RunJobByCode(TaskHistoried job)
        {
            if (job.task_date == null)
                return false;

            var date = job.task_date.Value.Date;

            try
            {
                switch (job.task_code)
                {
                    case "W_0008":
                    case "W_0009":
                        {
                            var (r1, _) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_00_09(date, date.AddHours(9));
                            var (r2, _) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_HRBarcode(date, date.AddHours(9));

                            await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_Detail(date.AddDays(-1).AddHours(2), date.AddHours(2));
                            return r1 && r2;
                        }

                    case "W_0912":
                        {
                            var (r3, _) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_09_12(date.AddHours(9), date.AddHours(12));
                            var (r4, _) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_Detail(date.AddHours(2), date.AddDays(1).AddHours(2));
                            return r3 && r4;
                        }

                    case "W_1224":
                        {
                            var (r5, _) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_12_24(date.AddHours(12), date.AddDays(1));
                            var (r6, _) = await _serviceWrapper.WorkSheet.Convert_WorkSheet_to_HRBarcode(date.AddHours(9), date.AddDays(1), 2);
                            return r5 && r6;
                        }

                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                // Log (English)
                //_logger.LogError(ex, $"[RunJobByCode] Error executing job {job.task_code} on {date:yyyy-MM-dd}");
                return false;
            }
        }

    }

}
