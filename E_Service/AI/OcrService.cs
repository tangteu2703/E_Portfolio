using System.Drawing;
using Tesseract;

namespace E_Service.AI
{
    public class OcrService
    {
        private readonly TesseractEngine _engine;

        public OcrService()
        {
            var tessDataPath = Path.Combine(AppContext.BaseDirectory, "Weights", "tessdata");
            _engine = new TesseractEngine(tessDataPath, "eng", EngineMode.Default);
        }

        public string ReadText(byte[] imageBytes)
        {
            // Ghi ảnh ra file tạm để tránh dùng PixConverter
            var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.jpg");
            File.WriteAllBytes(tempPath, imageBytes);

            using var pix = Pix.LoadFromFile(tempPath);
            using var page = _engine.Process(pix);
            File.Delete(tempPath); // cleanup

            return page.GetText().Trim();
        }
    }
}
