using E_Contract.Service.AI;
using E_Model.Request.AI;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace E_API.Controllers.AI
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : BaseController
    {
        private readonly IVisionAIService _vision;

        public ImageController(IVisionAIService vision)
        {
            _vision = vision;
        }

        [HttpPost("detect-plate")]
        public IActionResult DetectPlate([FromBody] Base64ImageRequest request)
        {
            if (string.IsNullOrEmpty(request.Base64))
                return BadRequest("Base64 is null or empty.");

            try
            {
                var imageBytes = Convert.FromBase64String(request.Base64);
                using var image = Image.Load<Rgba32>(imageBytes);

                // Detect all objects in the image
                var predictions = _vision.DetectObjects(image);

                var annotationResults = new List<AnnotationResult>();

                string carColor = "Không rõ";

                foreach (var pred in predictions)
                {

                    var rect = new Rectangle(
                        (int)pred.Rectangle.X,
                        (int)pred.Rectangle.Y,
                        (int)pred.Rectangle.Width,
                        (int)pred.Rectangle.Height);

                    string detectedText = "";
                    using (var cropped = image.Clone(ctx => ctx.Crop(rect)))
                    using (var ms = new MemoryStream())
                    {
                        cropped.Save(ms, new JpegEncoder());
                        detectedText = _vision.ReadText(ms.ToArray());
                    }

                    if (pred.Label.Name.Contains("car", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var croppedCar = image.Clone(ctx => ctx.Crop(rect)))
                        {
                            var dominantColor = GetDominantColor(croppedCar);
                            carColor = GetColorNameByHSV(dominantColor);
                        }
                    }

                    annotationResults.Add(new AnnotationResult
                    {
                        Label = pred.Label.Name,
                        Text = detectedText,
                        Color = carColor,
                        Box = new AnnotationResult.BoxInfo
                        {
                            X = rect.X,
                            Y = rect.Y,
                            Width = rect.Width,
                            Height = rect.Height
                        }
                    });
                }

                // Annotate image with detected objects and information
                image.Mutate(ctx =>
                {
                    var font = SystemFonts.CreateFont("Arial", 28);
                    var pen = Pens.Solid(Color.Red, 2);

                    foreach (var result in annotationResults)
                    {
                        var box = result.Box;
                        var rectF = new RectangleF(box.X, box.Y, box.Width, box.Height);
                        ctx.Draw(pen, rectF);

                        var labelText = $"{result.Label}: {result.Text}";
                        var labelPos = new PointF(box.X, box.Y - 40);
                        var labelSize = TextMeasurer.MeasureBounds(labelText, new TextOptions(font)).Size;
                        var labelBg = new RectangleF(labelPos.X - 5, labelPos.Y - 5, labelSize.X + 10, labelSize.Y + 10);
                        ctx.Fill(Color.FromRgba(0, 0, 0, 180), labelBg);
                        ctx.DrawText(labelText, font, Color.Yellow, labelPos);
                    }

                    // Annotate car color if detected
                    var carResult = annotationResults.FirstOrDefault(r => r.Label.Contains("car", StringComparison.OrdinalIgnoreCase));
                    if (carResult != null)
                    {
                        var box = carResult.Box;
                        var colorText = $"Màu: {carColor}";   // xét màu xe theo phần tử xác định đc
                        var colorPos = new PointF(box.X, box.Y + box.Height + 10);
                        var colorSize = TextMeasurer.MeasureBounds(colorText, new TextOptions(font)).Size;
                        var colorBg = new RectangleF(colorPos.X - 5, colorPos.Y - 5, colorSize.X + 10, colorSize.Y + 10);
                        ctx.Fill(Color.FromRgba(0, 0, 0, 180), colorBg);
                        ctx.DrawText(colorText, font, Color.Cyan, colorPos);
                    }
                });

                using var outStream = new MemoryStream();
                image.Save(outStream, new JpegEncoder());
                var base64Annotated = Convert.ToBase64String(outStream.ToArray());

                return Ok(new
                {
                    objects = annotationResults.Select(r => new
                    {
                        label = r.Label,
                        text = r.Text,
                        color = r.Color,
                        box = new { x = r.Box.X, y = r.Box.Y, width = r.Box.Width, height = r.Box.Height }
                    }),
                    annotated_image = base64Annotated
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi xử lý ảnh: " + ex.Message);
            }
        }
        [HttpPost("detect-plate2")]
        public IActionResult DetectPlate2([FromBody] Base64ImageRequest request)
        {
            if (string.IsNullOrEmpty(request.Base64))
                return BadRequest("Base64 is null or empty.");

            try
            {
                var imageBytes = Convert.FromBase64String(request.Base64);
                using var image = Image.Load<Rgba32>(imageBytes);

                var predictions = _vision.DetectObjects(image);
                var annotationResults = new List<AnnotationResult>();

                foreach (var pred in predictions)
                {
                    var rect = new Rectangle(
                        (int)pred.Rectangle.X,
                        (int)pred.Rectangle.Y,
                        (int)pred.Rectangle.Width,
                        (int)pred.Rectangle.Height);

                    string detectedText = "";
                    string carColor = "Không rõ";

                    using (var cropped = image.Clone(ctx => ctx.Crop(rect)))
                    using (var ms = new MemoryStream())
                    {
                        cropped.Save(ms, new JpegEncoder());
                        detectedText = _vision.ReadText(ms.ToArray());

                        //if (pred.Label.Name.Contains("car", StringComparison.OrdinalIgnoreCase))
                        //{
                        var dominantColor = GetDominantColor(cropped);
                        carColor = GetColorNameByHSV(dominantColor);
                        //}
                    }

                    annotationResults.Add(new AnnotationResult
                    {
                        Label = pred.Label.Name,
                        Text = detectedText,
                        Color = carColor,
                        Box = new AnnotationResult.BoxInfo
                        {
                            X = rect.X,
                            Y = rect.Y,
                            Width = rect.Width,
                            Height = rect.Height
                        }
                    });
                }

                image.Mutate(ctx =>
                {
                    var font = SystemFonts.CreateFont("Arial", 28);
                    var pen = Pens.Solid(Color.Red, 2);

                    foreach (var result in annotationResults)
                    {
                        var box = result.Box;
                        var rectF = new RectangleF(box.X, box.Y, box.Width, box.Height);
                        ctx.Draw(pen, rectF);

                        // Vẽ nhãn
                        var labelText = $"{result.Label}: {result.Text}";
                        var labelPos = new PointF(box.X, box.Y - 40);
                        var labelSize = TextMeasurer.MeasureBounds(labelText, new TextOptions(font)).Size;
                        var labelBg = new RectangleF(labelPos.X - 5, labelPos.Y - 5, labelSize.X + 10, labelSize.Y + 10);
                        ctx.Fill(Color.FromRgba(0, 0, 0, 180), labelBg);
                        ctx.DrawText(labelText, font, Color.Yellow, labelPos);

                        // Nếu là car thì vẽ thêm màu
                        if (result.Label.Contains("car", StringComparison.OrdinalIgnoreCase) && result.Color != "Không rõ")
                        {
                            var colorText = $"Màu: {result.Color}";
                            var colorPos = new PointF(box.X, box.Y + box.Height + 10);
                            var colorSize = TextMeasurer.MeasureBounds(colorText, new TextOptions(font)).Size;
                            var colorBg = new RectangleF(colorPos.X - 5, colorPos.Y - 5, colorSize.X + 10, colorSize.Y + 10);
                            ctx.Fill(Color.FromRgba(0, 0, 0, 180), colorBg);
                            ctx.DrawText(colorText, font, Color.Cyan, colorPos);
                        }
                    }
                });

                using var outStream = new MemoryStream();
                image.Save(outStream, new JpegEncoder());
                var base64Annotated = Convert.ToBase64String(outStream.ToArray());

                return Ok(new
                {
                    objects = annotationResults.Select(r => new
                    {
                        label = r.Label,
                        text = r.Text,
                        color = r.Color,
                        box = new { x = r.Box.X, y = r.Box.Y, width = r.Box.Width, height = r.Box.Height }
                    }),
                    annotated_image = base64Annotated
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi xử lý ảnh: " + ex.Message);
            }
        }
        private static Rgba32 GetDominantColor(Image<Rgba32> image)
        {
            long totalR = 0, totalG = 0, totalB = 0;
            int count = 0;

            for (int y = 0; y < image.Height; y += 5)
            {
                for (int x = 0; x < image.Width; x += 5)
                {
                    var pixel = image[x, y];
                    if (pixel.A < 100) continue;

                    totalR += pixel.R;
                    totalG += pixel.G;
                    totalB += pixel.B;
                    count++;
                }
            }

            if (count == 0) return new Rgba32(0, 0, 0);
            return new Rgba32(
                (byte)(totalR / count),
                (byte)(totalG / count),
                (byte)(totalB / count)
            );
        }

        private static string GetColorNameByHSV(Rgba32 color)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float hue = 0f;
            if (delta > 0f)
            {
                if (max == r)
                    hue = 60f * (((g - b) / delta) % 6f);
                else if (max == g)
                    hue = 60f * (((b - r) / delta) + 2f);
                else
                    hue = 60f * (((r - g) / delta) + 4f);
            }
            if (hue < 0) hue += 360f;

            float brightness = max;
            float saturation = (max == 0f) ? 0f : delta / max;

            if (brightness < 0.2f) return "Đen";
            if (saturation < 0.2f && brightness > 0.8f) return "Trắng";
            if (saturation < 0.2f) return "Xám";

            if (hue >= 0 && hue < 30) return "Đỏ";
            if (hue >= 30 && hue < 90) return "Vàng";
            if (hue >= 90 && hue < 150) return "Xanh lá";
            if (hue >= 150 && hue < 210) return "Xanh lơ";
            if (hue >= 210 && hue < 255) return "Xanh dương";
            if (hue >= 255 && hue < 330) return "Tím";

            return "Không rõ";
        }


        [HttpPost("face-id")]
        public IActionResult RecognizeFace([FromBody] Base64ImageRequest request)
        {
            try
            {
                var imageBytes = Convert.FromBase64String(request.Base64);
                var identity = _vision.ProcessFace(imageBytes);
                return Ok(new { identity });
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi nhận diện khuôn mặt: " + ex.Message);
            }
        }


        #region FastAPI Integration (Commented Out)
        //public class AnalyzeResponse
        //{
        //    public List<string> Texts { get; set; }
        //    public string Image_Base64 { get; set; }
        //}
        //[HttpPost("detect-plate-v2")]
        //public async Task<IActionResult> DetectPlateV2([FromBody] Base64ImageRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Base64))
        //        return BadRequest("Base64 is null or empty.");

        //    try
        //    {
        //        // Giải mã Base64 thành byte[]
        //        byte[] imageBytes = Convert.FromBase64String(request.Base64);

        //        // Tạo HTTP Multipart Form
        //        using var content = new MultipartFormDataContent();
        //        var imageContent = new ByteArrayContent(imageBytes);
        //        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // hoặc "image/png"
        //        content.Add(imageContent, "file", "image.jpg");

        //        // Gửi tới FastAPI
        //        using var client = new HttpClient();
        //        var response = await client.PostAsync("http://127.0.0.1:8000/analyze", content);

        //        if (!response.IsSuccessStatusCode)
        //            return StatusCode((int)response.StatusCode, "FastAPI call failed.");

        //        var result = await response.Content.ReadAsStringAsync();

        //        // ✅ Deserialize kết quả trả về từ FastAPI
        //        var parsed = JsonConvert.DeserializeObject<AnalyzeResponse>(result);

        //        return Ok(new
        //        {
        //            objects = parsed.Texts.Select(text => new
        //            {
        //                label = "plate",
        //                text = text
        //            }),
        //            annotated_image = parsed.Image_Base64
        //        });
        //    }
        //    catch (FormatException)
        //    {
        //        return BadRequest("Invalid base64 string.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        //[HttpPost("detect-plate-v3")]
        //public async Task<IActionResult> DetectPlateV3([FromBody] Base64ImageRequest request)
        //{
        //    if (string.IsNullOrEmpty(request.Base64))
        //        return BadRequest("Base64 is null or empty.");

        //    try
        //    {
        //        string api_key = "D9DUxXciRNRPlIFuT1xP";
        //        string model_endpoint = "license-plate-recognition-rxg4e/11"; // ví dụ model trên Roboflow Universe

        //        // Format base64 ảnh nếu chưa có prefix
        //        string base64Image = request.Base64;
        //        if (!base64Image.StartsWith("data:image"))
        //        {
        //            base64Image = "data:image/jpeg;base64," + base64Image;
        //        }

        //        string apiUrl = $"https://detect.roboflow.com/{model_endpoint}?api_key={api_key}";

        //        using (var httpClient = new HttpClient())
        //        {
        //            var content = new StringContent(base64Image);
        //            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

        //            var response = await httpClient.PostAsync(apiUrl, content);
        //            var responseString = await response.Content.ReadAsStringAsync();

        //            return Ok(responseString);
        //        }
        //    }
        //    catch (FormatException)
        //    {
        //        return BadRequest("Invalid base64 string.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        #endregion
    }
}
