﻿using BusinessObject.DTO.Product;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IImageService
    {
        Task<List<Image>> UploadImagesAsync(ImageModel imageModel, long productId);
        Task<List<Image>> UpdateImagesByProductIdAsync(long productId, ImageModel imageModel);
        Task DeleteImagesByProductIdAsync(long productId);
    }
}
