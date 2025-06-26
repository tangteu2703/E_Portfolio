using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;
using System.Drawing;
using System.Drawing.Imaging;
using Image = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;

public class YoloService
{
    private readonly YoloScorer<YoloCocoP5Model> _scorer;

    public YoloService()
    {
        var modelPath = Path.Combine(AppContext.BaseDirectory, "Weights", "yolov5n.onnx");
        _scorer = new YoloScorer<YoloCocoP5Model>(modelPath);
    }

    public (string Label, byte[] CroppedImageBytes)? DetectPlate(byte[] imageData)
    {
        using var imageSharp = ConvertToImageSharp(imageData);

        var predictions = _scorer.Predict(imageSharp);
        var plate = predictions.FirstOrDefault(x =>
            x.Label.Name.Contains("license", StringComparison.OrdinalIgnoreCase) ||
            x.Label.Name.Contains("car", StringComparison.OrdinalIgnoreCase));

        if (plate == null) return null;

        var rect = new SixLabors.ImageSharp.Rectangle(
            (int)plate.Rectangle.X,
            (int)plate.Rectangle.Y,
            (int)plate.Rectangle.Width,
            (int)plate.Rectangle.Height);

        // ✅ Đây là dòng fix lỗi
        var cropped = imageSharp.Clone(img => img.Crop(rect));

        using var msOut = new MemoryStream();
        cropped.Save(msOut, new JpegEncoder());

        return (plate.Label.Name, msOut.ToArray());
    }

    private Image<Rgba32> ConvertToImageSharp(byte[] imageBytes)
    {
        using var ms = new MemoryStream(imageBytes);
        return Image.Load<Rgba32>(ms);
    }
}
