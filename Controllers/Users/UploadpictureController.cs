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
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Models;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Upload User Image")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class UploadpictureController : ControllerBase {

    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<UploadpictureController> _logger;

    public UploadpictureController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<UploadpictureController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  
        [HttpPost]
        public IActionResult uploadPicture([FromForm]UploadfileModel model) {
                if (model.Profilepic.FileName is not null)
                {
                    try
                    {
                        string ext= Path.GetExtension(model.Profilepic.FileName);

                        var folderName = Path.Combine("wwwroot", "users/");
                        var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                        var newFilename =pathToSave + "00" + model.Id + ".jpg";

                        using var image = SixLabors.ImageSharp.Image.Load(model.Profilepic.OpenReadStream());
                        image.Mutate(x => x.Resize(100, 100));
                        image.Save(newFilename);
                        var file = "";
                        if (model.Profilepic is not null)
                        {
                             file = "https://localhost:7292/users/00"+model.Id.ToString()+".jpg";
                            _userService.UpdatePicture(model.Id, file);                            
                        }
                        return Ok(new {  message = "Profile Picture has been updated.", profilepic = file});
                        
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new { message =ex.Message});
                    }

                }
                return NotFound(new { message = "Profile Picture not found."});

        }
    }
    
}