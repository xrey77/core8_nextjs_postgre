using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Helpers;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Retrieve User ID")]
    [Authorize]    
    [ApiController]
    [Route("[controller]")]
    public class GetbyidController : ControllerBase
    {

    private readonly IRabbitMQProducer _rabbitMQProducer;
    private IUserService _userService;
    private IAuthService _authService;
    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<GetbyidController> _logger;

    public GetbyidController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        IAuthService authService,
        ILogger<GetbyidController> logger,
        IRabbitMQProducer rabbitMQProducer
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _authService = authService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
        _rabbitMQProducer = rabbitMQProducer;
    }  

        [HttpGet("/api/getbyid/{id}")]
        public async Task<IActionResult> getByuserid(int id) {
            try {
                var user = await _userService.GetById(id);
                var qrcode = user.Qrcodeurl == "" ? null : user.Qrcodeurl;
                user.Qrcodeurl = qrcode;


                var model = _mapper.Map<UserModel>(user);
                var xroles = await _authService.getRolename(user.RolesId);     
                model.Roles = xroles?.Name;


                // START - Publish to RabbitMQ=============
                try
                {
                    await _rabbitMQProducer.PublishUserRegisteredEvent(model);
                }
                catch (UnauthorizedAccessException ex) when (ex.Message.Contains("RabbitMQ credentials"))
                {
                    return StatusCode(500, new { 
                        success = false, 
                        message = $"Messaging password service configuration error. Please contact administrator, {ex.Message}" 
                    });
                }
                catch (AppException ex)
                {
                    return StatusCode(500, new { 
                        success = false, 
                        message = $"An unexpected error occurred, {ex.Message}" 
                    });
                }
                // END - Publish to RabbitMQ==============





                return Ok(new {
                    message = "User found, please wait.",
                    user = model
                });

            } catch(AppException ex) {
                return NotFound(new {
                    message = ex.Message
                });

            }
        }
    }
    
}