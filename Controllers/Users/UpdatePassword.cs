using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Services;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Update User")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UpdatePasswordController : ControllerBase {
        
    private IUserService _userService;
    private IMapper _mapper;
    private readonly IConfiguration _configuration;  
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<UpdatePasswordController> _logger;
    private readonly IRabbitMQProducer _rabbitMQProducer;

    public UpdatePasswordController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<UpdatePasswordController> logger,
        IRabbitMQProducer rabbitMQProducer
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
        _rabbitMQProducer = rabbitMQProducer;
    }  

        [HttpPatch("/api/updatepassword/{id}")]        
        public async Task<IActionResult> updateUserPassword(int id, [FromBody]UserPasswordUpdate model) {
            var user = _mapper.Map<User>(model);
            user.Id = id;
            try
            {
                await _userService.UpdatePassword(user, model.Password);

                // START - Publish to RabbitMQ=============
                try
                {
                    await _rabbitMQProducer.PublishUserRegisteredEvent(user);
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

                return Ok(new {message="Your profile password has been updated.",user = model});
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message });
            }
        }


    }
}