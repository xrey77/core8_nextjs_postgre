using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Models;
using core8_nextjs_postgre.Helpers;


namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Enable or Disable 2-Factor Authentication")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ActivatemfaController : ControllerBase {

    private IUserService _userService;
    private readonly IRabbitMQProducer _rabbitMQProducer;
    private IMapper _mapper;
    private readonly IConfiguration _configuration;  
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ActivatemfaController> _logger;

    public ActivatemfaController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<ActivatemfaController> logger,
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

        [HttpPatch("/api/enablemfa/{id}")]
        public async Task<IActionResult> EnableMFA(int id,MfaModel model) {
            if (model.Twofactorenabled == true) {
                var user = await _userService.GetById(id);
                if(user != null) {
                    QRCode qrimageurl = new QRCode();
                    var fullname = "SUPER CAR INC.";
                    TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                    var setupInfo = twoFactor.GenerateSetupCode(fullname, user.Email, user.Secretkey, false, 3);
                    var imageUrl = setupInfo.QrCodeSetupImageUrl;
                    await _userService.ActivateMfa(id, true, imageUrl);


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


                    return Ok(new {message="2-Factor Authenticator has been enabled.", qrcodeurl = imageUrl});
                } else {
                    return NotFound(new {message="User not found."});
                }

            } else {
                await _userService.ActivateMfa(id, false, null);
                return Ok(new { message="2-Factor Authenticator has been disabled."});
            }
        }
    }    
}