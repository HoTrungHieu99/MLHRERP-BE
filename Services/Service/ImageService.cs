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
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ImageService : IImageService
    {
        private readonly IImageRepository _repo;
        private readonly Cloudinary _cloudinary;

        public ImageService(IImageRepository repo, IConfiguration config)
        {
            _repo = repo;

            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }

        public async Task<List<Image>> UploadImagesAsync(ImageModel imageModel, long productId)
        {
            if (imageModel.File == null || imageModel.File.Count == 0)
                throw new Exception("No files uploaded.");

            var uploadedImages = new List<Image>();

            foreach (var file in imageModel.File)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    PublicId = Guid.NewGuid().ToString(),
                    Overwrite = true
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("Cloudinary upload failed.");

                var image = new Image
                {
                    ImageUrl = uploadResult.SecureUrl.ToString(),
                    ProductId = productId
                };

                uploadedImages.Add(await _repo.AddAsync(image));
            }

            return uploadedImages;
        }
    }

}
