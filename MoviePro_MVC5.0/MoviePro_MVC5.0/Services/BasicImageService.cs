using Microsoft.AspNetCore.Http;
using MoviePro_MVC5._0.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;



namespace MoviePro_MVC5._0.Services
{
    //Encode decode image files
    public class BasicImageService : IImageService
    {
        private readonly IHttpClientFactory _httpClient;
        //Constructor
        public BasicImageService(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }
        //Decodes image from database for display 
        public string DecodeImage(byte[] poster, string contentType)
        {
            if (poster is null) return null;
            var posterImage = Convert.ToBase64String(poster);
            return $"data:{contentType};base64,{posterImage}";
        }
        //Encodes image to byte array to persist in the database
        public async Task<byte[]> EncodeImageAsync(IFormFile poster)
        {
            if (poster == null) return null;
            using var ms = new MemoryStream();
            await poster.CopyToAsync(ms);
            return ms.ToArray();

        }

        public async Task<byte[]> EncodeImageURLAsync(string imageURL)
        {
            var client = _httpClient.CreateClient();
            var response = await client.GetAsync(imageURL);
            using Stream stream = await response.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
