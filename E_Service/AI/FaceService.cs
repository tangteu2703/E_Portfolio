using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using System.Drawing.Imaging;

namespace E_Service.AI
{
    public class FaceService
    {
        private readonly InferenceSession _faceNet;

        public FaceService()
        {
            var modelPath = Path.Combine(AppContext.BaseDirectory, "Weights", "facenet.onnx");
            _faceNet = new InferenceSession(modelPath);
        }

        public string ProcessFace(byte[] imageBytes)
        {
            using var ms = new MemoryStream(imageBytes);
            using var bmp = new Bitmap(ms);
            using var faceImg = new Bitmap(bmp, new Size(160, 160)); // Resize chuẩn cho facenet

            var input = BitmapToTensor(faceImg);
            var inputs = new[] { NamedOnnxValue.CreateFromTensor("input_1", input) };

            using var results = _faceNet.Run(inputs);
            var embedding = results.First().AsEnumerable<float>().ToArray();

            // So sánh với vector giả định
            var knownVector = Enumerable.Repeat(0.5f, 128).ToArray();
            var similarity = CosineSimilarity(embedding, knownVector);

            return similarity > 0.85 ? "Face matched: John Doe" : "Unknown face";
        }

        private DenseTensor<float> BitmapToTensor(Bitmap bmp)
        {
            var tensor = new DenseTensor<float>(new[] { 1, 160, 160, 3 });

            for (int y = 0; y < 160; y++)
            {
                for (int x = 0; x < 160; x++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    tensor[0, y, x, 0] = pixel.R / 255f;
                    tensor[0, y, x, 1] = pixel.G / 255f;
                    tensor[0, y, x, 2] = pixel.B / 255f;
                }
            }

            return tensor;
        }

        private float CosineSimilarity(float[] v1, float[] v2)
        {
            float dot = 0f, mag1 = 0f, mag2 = 0f;

            for (int i = 0; i < v1.Length; i++)
            {
                dot += v1[i] * v2[i];
                mag1 += v1[i] * v1[i];
                mag2 += v2[i] * v2[i];
            }

            return dot / ((float)Math.Sqrt(mag1) * (float)Math.Sqrt(mag2));
        }
    }
}
