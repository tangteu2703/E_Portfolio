using System.Net;
using System.Net.Sockets;

namespace E_Common
{
    public class LogExtension
    {
        /// <summary>
        /// Ghi log ra file. Nếu file đã tồn tại thì thêm vào dòng đầu tiên, nếu chưa có thì tạo mới.
        /// </summary>
        /// <param name="filePath">Thư mục chứa file log.</param>
        /// <param name="fileName">Tên file log, ví dụ "log.txt".</param>
        /// <param name="message">Thông điệp log.</param>
        public static void WriteLog(string filePath, string fileName, string message)
        {
            try
            {
                // Tạo đường dẫn đầy đủ tới thư mục gốc log trong wwwroot
                var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }

                // Tạo đường dẫn đầy đủ tới file log
                string fullPath = Path.Combine(rootPath, fileName);

                // Dòng log mới
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}";

                if (File.Exists(fullPath))
                {
                    // Đọc tất cả nội dung cũ
                    string[] oldLines = File.ReadAllLines(fullPath);

                    // Ghi đè: dòng mới lên đầu, dòng cũ phía sau
                    using (StreamWriter writer = new StreamWriter(fullPath, false)) // false = overwrite
                    {
                        writer.WriteLine(logEntry);
                        foreach (string line in oldLines)
                        {
                            writer.WriteLine(line);
                        }
                    }
                }
                else
                {
                    // File chưa tồn tại, tạo mới
                    File.WriteAllText(fullPath, logEntry + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi ghi log: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy tên máy (hostname)
        /// </summary>
        public static string GetMachineName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Lấy IP v4 của máy hiện tại
        /// </summary>
        public static string GetLocalIPAddress()
        {
            string hostName = Dns.GetHostName();
            var ipAddresses = Dns.GetHostAddresses(hostName);

            // Ưu tiên IPv4
            var ipv4 = ipAddresses.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            return ipv4?.ToString() ?? "Không tìm thấy IPv4";
        }

    }
}
