﻿//using CareerCompassAPI.Application.Abstraction.Storage.Local;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;

//namespace CareerCompassAPI.Infrastructure.Services.Storage.Local
//{
//    public class LocalStorage : ILocalStorage
//    {
//        private readonly IWebHostEnvironment _webHostEnvironment;

//        public LocalStorage(IWebHostEnvironment webHostEnvironment)
//        {
//            _webHostEnvironment = webHostEnvironment;
//        }

//        public async Task DeleteAsync(string path, string fileName)
//        {
//            File.Delete($"{path}\\{fileName}");
//        }

//        public List<string> GetFiles(string path)
//        {
//            DirectoryInfo dir = new(path);
//            return dir.GetFiles().Select(f => f.Name).ToList();
//        }

//        public bool HasFile(string path, string fileName)
//        {
//            return File.Exists($"{path}\\{fileName}");
//        }

//        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files, string appUserId)
//        {
//            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);
//            if (!Directory.Exists(uploadPath))
//            {
//                Directory.CreateDirectory(uploadPath);
//            }
//            List<(string fileName, string path)> datas = new();
//            foreach (IFormFile file in files)
//            {
//                await CopyFileAsync($"{uploadPath}\\{file.Name}", file);
//                datas.Add((file.Name, $"{path}\\{file.Name}"));
//            }
//            return datas;
//        }
//        private async Task<bool> CopyFileAsync(string path, IFormFile file)
//        {
//            try
//            {
//                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync:false);
//                await file.CopyToAsync(fileStream);
//                await fileStream.FlushAsync();
//                return true;
//            }
//            catch (Exception ex)
//            {

//                throw ex;
//            }
//        }
//    }
//}
