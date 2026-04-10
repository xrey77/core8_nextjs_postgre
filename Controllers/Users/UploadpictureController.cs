using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using AutoMapper;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using System;
using System.Threading.Tasks;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Models;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Upload User Image")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    // REMOVED: [System.Runtime.Versioning.SupportedOSPlatform("windows")] (ImageSharp is cross-platform)
    public class UploadpictureController : ControllerBase 
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;  
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<UploadpictureController> _logger;
        private readonly IRabbitMQProducer _rabbitMQProducer;

        public UploadpictureController(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IUserService userService,
            IMapper mapper,
            ILogger<UploadpictureController> logger,
            IRabbitMQProducer rabbitMQProducer)
        {
            _configuration = configuration;  
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
            _env = env;
            _rabbitMQProducer = rabbitMQProducer;
        }  

        [HttpPost]
        public async Task<IActionResult> UploadPicture([FromForm] UploadfileModel model) 
        {
            // 1. Validation checks
            if (model?.Profilepic == null || model.Profilepic.Length == 0)
            {
                return BadRequest(new { message = "Profile Picture not found or file is empty." });
            }

            try
            {
                // 2. Safely create directory paths using IWebHostEnvironment
                var folderName = Path.Combine(_env.WebRootPath, "users");
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                }

                // Standardized filename (e.g., 005.jpg)
                var filename = $"00{model.Id}.jpg";
                var fullSavePath = Path.Combine(folderName, filename);

                // 3. Process and Save Image asynchronously
                using (var stream = model.Profilepic.OpenReadStream())
                using (var image = await Image.LoadAsync(stream))
                {
                    image.Mutate(x => x.Resize(100, 100));
                    await image.SaveAsync(fullSavePath);
                }

                // 4. Generate dynamic URL (Avoids hardcoding localhost)
                var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
                var fileUrl = $"{baseUrl}/users/{filename}";

                // 5. Update Database
                var user = _userService.UpdatePicture(model.Id, fileUrl);   

                // 6. RabbitMQ Publishing
                try
                {
                    await _rabbitMQProducer.PublishUserRegisteredEvent(model);
                }
                catch (UnauthorizedAccessException ex) when (ex.Message.Contains("RabbitMQ credentials"))
                {
                    _logger.LogError(ex, "RabbitMQ authentication failed.");
                    return StatusCode(500, new { 
                        success = false, 
                        message = $"Messaging service configuration error. Please contact administrator." 
                    });
                }
                catch (AppException ex)
                {
                    _logger.LogError(ex, "AppException during RabbitMQ publish.");
                    return StatusCode(500, new { 
                        success = false, 
                        message = $"An unexpected error occurred: {ex.Message}" 
                    });
                }

                return Ok(new { message = "Profile Picture has been updated.", profilepic = fileUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile picture.");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}



// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Hosting;
// using System.IO;
// using AutoMapper;
// using SixLabors.ImageSharp.Processing;
// using SixLabors.ImageSharp;
// using System;
// using core8_nextjs_postgre.Helpers;
// using core8_nextjs_postgre.Services;
// using core8_nextjs_postgre.Models;

// namespace core8_nextjs_postgre.Controllers.Users
// {
//     [ApiExplorerSettings(GroupName = "Upload User Image")]
//     [Authorize]
//     [ApiController]
//     [Route("api/[controller]")]
//     [System.Runtime.Versioning.SupportedOSPlatform("windows")]
//     public class UploadpictureController : ControllerBase {

//     private IUserService _userService;
//     private IMapper _mapper;
//     private readonly IConfiguration _configuration;  
//     private readonly IWebHostEnvironment _env;
//     private readonly ILogger<UploadpictureController> _logger;
//     private readonly IRabbitMQProducer _rabbitMQProducer;

//     public UploadpictureController(
//         IConfiguration configuration,
//         IWebHostEnvironment env,
//         IUserService userService,
//         IMapper mapper,
//         ILogger<UploadpictureController> logger,
//         IRabbitMQProducer rabbitMQProducer
//         )
//     {
//         _configuration = configuration;  
//         _userService = userService;
//         _mapper = mapper;
//         _logger = logger;
//         _env = env;
//         _rabbitMQProducer = rabbitMQProducer;
//     }  
//         [HttpPost]
//         public async Task<IActionResult> uploadPicture([FromForm]UploadfileModel model) {
//                 if (model.Profilepic.FileName is not null)
//                 {
//                     try
//                     {
//                         string ext= Path.GetExtension(model.Profilepic.FileName);

//                         var folderName = Path.Combine("wwwroot", "users/");
//                         var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

//                         var newFilename =pathToSave + "00" + model.Id + ".jpg";

//                         using var image = SixLabors.ImageSharp.Image.Load(model.Profilepic.OpenReadStream());
//                         image.Mutate(x => x.Resize(100, 100));
//                         image.Save(newFilename);
//                         var file = "";
//                         if (model.Profilepic is not null)
//                         {
//                              file = "https://localhost:7292/users/00"+model.Id.ToString()+".jpg";
//                             var user = _userService.UpdatePicture(model.Id, file);   
//                             // START - Publish to RabbitMQ=============
//                             try
//                             {
//                                 await _rabbitMQProducer.PublishUserRegisteredEvent(model);
//                             }
//                             catch (UnauthorizedAccessException ex) when (ex.Message.Contains("RabbitMQ credentials"))
//                             {
//                                 return StatusCode(500, new { 
//                                     success = false, 
//                                     message = $"Messaging password service configuration error. Please contact administrator, {ex.Message}" 
//                                 });
//                             }
//                             catch (AppException ex)
//                             {
//                                 return StatusCode(500, new { 
//                                     success = false, 
//                                     message = $"An unexpected error occurred, {ex.Message}" 
//                                 });
//                             }
//                             // END - Publish to RabbitMQ==============
                            
//                         }





//                         return Ok(new {  message = "Profile Picture has been updated.", profilepic = file});
                        
//                     }
//                     catch (Exception ex)
//                     {
//                         return BadRequest(new { message =ex.Message});
//                     }

//                 }
//                 return NotFound(new { message = "Profile Picture not found."});

//         }
//     }
    
// }