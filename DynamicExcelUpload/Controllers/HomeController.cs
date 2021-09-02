using DynamicExcelUpload.Contract.BLL;
using DynamicExcelUpload.Core.Helpers;
using DynamicExcelUpload.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicExcelUpload.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUploadBLL _bll;
        private readonly IWebHostEnvironment _webHost;

        public HomeController(IUploadBLL bll, IWebHostEnvironment webHost)
        {
            _webHost = webHost;
            _bll = bll;
        }

        public IActionResult Index()
        {
            string webRootPath = _webHost.WebRootPath;
            string webPath = Path.Combine(webRootPath, "upload");
            if (!Directory.Exists(webPath))
            {
                Directory.CreateDirectory(webPath);
            }

            return View(new ExcelModel());
        }

        public IActionResult GetListFile()
        {
            try
            {
                string webRootPath = _webHost.WebRootPath;
                string webPath = Path.Combine(webRootPath, "upload");
                string[] fileArray = Directory.GetFiles(webPath);

                var result = fileArray.Select(c => new FileModel
                {
                    FileName = Path.GetFileName(c),
                    Path = c
                });

                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> GetData(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                return Ok(new List<string>());
            }

            try
            {
                string webRootPath = _webHost.WebRootPath;
                string webPath = Path.Combine(webRootPath, "upload");
                string[] fileArray = Directory.GetFiles(webPath);
                if (fileArray.Any())
                {
                    var fileName = fileArray[0];
                    var datas = await _bll.GetData(Path.GetFileNameWithoutExtension(fileName));
                    var a = datas.Select().Select(x => x.ItemArray.Select((a, i) => new { Name = datas.Columns[i].ColumnName, Value = a })
                                                                                   .ToDictionary(a => a.Name, a => a.Value));
                    return Ok(a);
                }
                else
                    return Ok(new List<string>());
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public FileResult DownloadFile(string fileName)
        {
            var webRootPath = _webHost.WebRootPath;
            string webPath = Path.Combine(webRootPath, "upload/" + fileName);

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(webPath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", fileName);
        }

        public async Task<IActionResult> Upload(IFormFile file)
        {
            var validationMsg = ValidateExcelFile(file);

            if (!String.IsNullOrEmpty(validationMsg))
                return BadRequest(new { success = false, error = validationMsg });

            try
            {
                var webRootPath = _webHost.WebRootPath;
                string webPath = Path.Combine(webRootPath, "upload");
                var webFilePath = Path.Combine(webPath, file.FileName);

                if (!Directory.Exists(webPath))
                {
                    Directory.CreateDirectory(webPath);
                }

                using (var str = System.IO.File.Create(webFilePath))
                {
                    file.CopyTo(str);
                }

                using (var stream = new MemoryStream())
                {
                    file.CopyTo(stream);

                    using var package = new ExcelPackage(stream);

                    var dtTable = ExcelHelper.GetDataTableFromExcel(package, true);

                    await _bll.CreateTable(dtTable, Path.GetFileNameWithoutExtension(file.FileName.Trim()));
                    _bll.InsertData(dtTable, Path.GetFileNameWithoutExtension(file.FileName.Trim()));
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return Ok(new
            {
                IsSuccess = true,
                Message = "",
                Filename = file.FileName
            });
        }

        private string ValidateExcelFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
                return "file is empty";

            if (!Path.GetExtension(file.FileName).Contains(".xls", StringComparison.OrdinalIgnoreCase))
                return "Not Support file extension";

            return string.Empty;
        }
    }
}
