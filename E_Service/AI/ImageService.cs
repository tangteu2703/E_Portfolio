using E_Contract.Service.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Service.AI
{
    public class ImageService : IImageService
    {
        private readonly YoloService _yoloService;
        private readonly OcrService _ocrService;
        private readonly FaceService _faceService;

        public ImageService(YoloService yoloService, OcrService ocrService, FaceService faceService)
        {
            _yoloService = yoloService;
            _ocrService = ocrService;
            _faceService = faceService;
        }

        /// <summary>
        /// Phát hiện biển số xe từ ảnh, cắt vùng biển số, đọc ký tự bằng OCR.
        /// </summary>
        public async Task<string> DetectLicensePlateAsync(byte[] imageBytes)
        {
            var result = _yoloService.DetectPlate(imageBytes);
            if (result == null)
                return "No license plate detected.";

            var croppedPlateBytes = result.Value.CroppedImageBytes;
            var plateText = _ocrService.ReadText(croppedPlateBytes);
            return plateText;
        }

        /// <summary>
        /// Nhận diện khuôn mặt từ ảnh, sinh vector và so sánh với mẫu.
        /// </summary>
        public async Task<string> RecognizeFaceAsync(byte[] imageBytes)
        {
            return _faceService.ProcessFace(imageBytes);
        }
    }
}
