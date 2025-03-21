using BusinessObject.DTO;
using BusinessObject.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Repo.IRepository;
using Services.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{

    /*public class ImageService : IImageService
    {
        private readonly IImageRepository _repo;
        private readonly Cloudinary _cloudinary;
        private readonly long _maxFileSize; // Định nghĩa kích thước tối đa

        public ImageService(IImageRepository repo, IConfiguration config)
        {
            _repo = repo;

            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);

            // Đặt kích thước tối đa (ví dụ: 10MB)
            _maxFileSize = config.GetValue<long>("Cloudinary:MaxFileSize", 10 * 1024 * 1024);
        }

        public async Task<List<Image>> UploadImagesAsync(ImageModel imageModel, long productId)
        {
            if (imageModel.File == null || imageModel.File.Count == 0)
                throw new Exception("No files uploaded.");

            var uploadedImages = new List<Image>();

            foreach (var file in imageModel.File)
            {
                // Kiểm tra kích thước file trước khi upload
                if (file.Length > _maxFileSize)
                    throw new Exception($"File '{file.FileName}' vượt quá giới hạn {_maxFileSize / (1024 * 1024)}MB.");

                try
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = Guid.NewGuid().ToString(),
                        Overwrite = true,
                        Transformation = new Transformation().Quality(80) // Giảm chất lượng để giảm dung lượng
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode != HttpStatusCode.OK)
                        throw new Exception($"Upload thất bại: {uploadResult.Error?.Message}");

                    var image = new Image
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        ProductId = productId
                    };

                    uploadedImages.Add(await _repo.AddAsync(image));
                }
                catch (Exception ex)
                {
                    // Log lỗi (nếu có hệ thống logging)
                    Console.WriteLine($"Lỗi khi upload file {file.FileName}: {ex.Message}");
                    throw new Exception($"Không thể upload file {file.FileName}. Chi tiết: {ex.Message}");
                }
            }

            return uploadedImages;
        }
    }
*/

    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.Extensions.Configuration;
    using SkiaSharp;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    public class ImageService : IImageService
    {
        private readonly IImageRepository _repo;
        private readonly Cloudinary _cloudinary;
        private readonly long _maxFileSize; // Kích thước tối đa (byte)
        private readonly int _maxWidth; // Chiều rộng tối đa ảnh
        private readonly int _maxHeight; // Chiều cao tối đa ảnh
        private readonly int _quality; // Chất lượng nén

        public ImageService(IImageRepository repo, IConfiguration config)
        {
            _repo = repo;

            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);

            // Đọc cấu hình từ appsettings.json
            _maxFileSize = config.GetValue<long>("Cloudinary:MaxFileSize", 5 * 1024 * 1024); // Mặc định 5MB
            _maxWidth = config.GetValue<int>("Cloudinary:MaxWidth", 1024); // Mặc định 1024px
            _maxHeight = config.GetValue<int>("Cloudinary:MaxHeight", 1024); // Mặc định 1024px
            _quality = config.GetValue<int>("Cloudinary:Quality", 80); // Chất lượng ảnh nén (mặc định 80%)
        }

        public async Task<List<Image>> UploadImagesAsync(ImageModel imageModel, long productId)
        {
            if (imageModel.File == null || imageModel.File.Count == 0)
                throw new Exception("No files uploaded.");

            var uploadedImages = new List<Image>();

            foreach (var file in imageModel.File)
            {
                // Kiểm tra kích thước file trước khi xử lý
                if (file.Length > _maxFileSize)
                {
                    Console.WriteLine($"File {file.FileName} quá lớn ({file.Length / (1024 * 1024)}MB), đang xử lý giảm dung lượng...");

                    // Giảm dung lượng ảnh
                    using var resizedStream = ResizeAndCompressImage(file, _maxWidth, _maxHeight, _quality);

                    if (resizedStream.Length > _maxFileSize)
                        throw new Exception($"Sau khi nén, file {file.FileName} vẫn quá lớn ({resizedStream.Length / (1024 * 1024)}MB).");

                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, resizedStream),
                        PublicId = Guid.NewGuid().ToString(),
                        Overwrite = true
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode != HttpStatusCode.OK)
                        throw new Exception($"Upload thất bại: {uploadResult.Error?.Message}");

                    var image = new Image
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        ProductId = productId
                    };

                    uploadedImages.Add(await _repo.AddAsync(image));
                }
                else
                {
                    // Nếu file nhỏ hơn giới hạn, upload trực tiếp
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        PublicId = Guid.NewGuid().ToString(),
                        Overwrite = true
                    };

                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode != HttpStatusCode.OK)
                        throw new Exception($"Upload thất bại: {uploadResult.Error?.Message}");

                    var image = new Image
                    {
                        ImageUrl = uploadResult.SecureUrl.ToString(),
                        ProductId = productId
                    };

                    uploadedImages.Add(await _repo.AddAsync(image));
                }
            }

            return uploadedImages;
        }

        private Stream ResizeAndCompressImage(IFormFile file, int maxWidth, int maxHeight, int quality)
        {
            using var inputStream = file.OpenReadStream();
            using var originalImage = SKBitmap.Decode(inputStream);

            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;

            // Tính toán tỷ lệ resize
            if (originalImage.Width > maxWidth || originalImage.Height > maxHeight)
            {
                float aspectRatio = (float)originalImage.Width / originalImage.Height;
                if (aspectRatio > 1)
                {
                    newWidth = maxWidth;
                    newHeight = (int)(maxWidth / aspectRatio);
                }
                else
                {
                    newHeight = maxHeight;
                    newWidth = (int)(maxHeight * aspectRatio);
                }
            }

            using var resizedImage = originalImage.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.Medium);
            using var image = SKImage.FromBitmap(resizedImage);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, quality);

            var outputStream = new MemoryStream();
            data.SaveTo(outputStream);
            outputStream.Position = 0; // Reset stream về đầu để upload
            return outputStream;
        }
    }


}
