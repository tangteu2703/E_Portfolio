using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;

namespace E_Contract.Service.AI
{
    public interface IVisionAIService
    {
        IList<YoloPrediction> DetectObjects(Image<Rgba32> image);
        string ReadText(byte[] imageBytes);
        string ProcessFace(byte[] imageBytes);
    }
}
