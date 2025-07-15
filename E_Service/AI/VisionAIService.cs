using E_Contract.Service.AI;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Tesseract;
using Yolov5Net.Scorer;
using Microsoft.AspNetCore.Hosting;
using Yolov5Net.Scorer.Models;

namespace E_Service.AI
{
    public class VisionAIService : IVisionAIService
    {
        private readonly YoloScorer<YoloCocoP5Model> _scorer;
        private readonly TesseractEngine _ocrEngine;
        private readonly InferenceSession _faceNet;

        public VisionAIService(IWebHostEnvironment env)
        {
            var modelFolder = Path.Combine(env.WebRootPath, "AI");

            // Load YOLO
            var yoloPath = Path.Combine(modelFolder, "yolov5n.onnx");
            if (File.Exists(yoloPath))
                _scorer = new YoloScorer<YoloCocoP5Model>(yoloPath);

            // Load Tesseract OCR
            var tessdata = Path.Combine(modelFolder, "tessdata");
            if (Directory.Exists(tessdata))
                _ocrEngine = new TesseractEngine(tessdata, "eng", EngineMode.Default);

            // Load FaceNet
            var facePath = Path.Combine(modelFolder, "facenet.onnx");
            if (File.Exists(facePath))
                _faceNet = new InferenceSession(facePath);
        }

        public IList<YoloPrediction> DetectObjects(Image<Rgba32> image) => _scorer.Predict(image);

        public string ReadText(byte[] imageBytes)
        {
            using var ms = new MemoryStream(imageBytes);
            using var img = Pix.LoadFromMemory(imageBytes);
            using var page = _ocrEngine.Process(img);
            return page.GetText().Trim();
        }

        public string ProcessFace(byte[] imageBytes)
        {
            // Gỉa định: return dummy result
            return "Face matched (demo)";
        }
    }

}
