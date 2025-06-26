using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Contract.Service.AI
{
    public interface IImageService
    {
        Task<string> DetectLicensePlateAsync(byte[] imageBytes);
        Task<string> RecognizeFaceAsync(byte[] imageBytes);
    }
}
