using Microsoft.AspNetCore.Mvc;
using ScanPlanet.API.Core.DTOs;
using ScanPlanet_API;
using System.Linq;

namespace ScanPlanet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScannerController : ControllerBase
    {
        private readonly List<string> _allowedExtensions = new() { ".jpeg", ".png", ".jpg" , ".gif"};
        private readonly long _maxAllowedSizeInBytes = 8 * 1024 * 1024; // this for 8 MB

        [HttpGet]
        public  IActionResult Scan( [FromForm] RequestDTO dto)
        {
            if(!ModelState.IsValid)
                return BadRequest();

            var image = dto.Image;

            var fileExtension = Path.GetExtension(image.FileName).ToLower();
            if (!_allowedExtensions.Contains(fileExtension))
                return BadRequest("Only jpeg, jpg, png, gif images are allowed");

            if (image.Length > _maxAllowedSizeInBytes)
                return BadRequest($"Max allowed size is {_maxAllowedSizeInBytes / 1024 / 1024}MB");


            

            using var dataStream = new MemoryStream();
            dto.Image.CopyTo(dataStream);
            
            var imageBytes = dataStream.ToArray();
            
            var data = new MLModel.ModelInput()
            {
                ImageSource = imageBytes,
            };
            
            var result = MLModel.Predict(data);
            ResponseDTO response = new ResponseDTO 
            { 
                PredictedLabel = result.PredictedLabel
            };
            ;
            return Ok(response);
        }
    }
}
