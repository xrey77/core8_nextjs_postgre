using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Google.Authenticator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using core8_nextjs_postgre.Models;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Services;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Enable or Disable 2-Factor Authentication")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ActivatemfaController : ControllerBase {

    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<ActivatemfaController> _logger;

    public ActivatemfaController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<ActivatemfaController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  

        [HttpPatch("/api/enablemfa/{id}")]
        public IActionResult EnableMFA(int id,MfaModel model) {
            if (model.Twofactorenabled == true) {
                var user = _userService.GetById(id);
                if(user != null) {
                    QRCode qrimageurl = new QRCode();
                    var fullname = user.FirstName + " " + user.LastName;
                    TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
                    var setupInfo = twoFactor.GenerateSetupCode(fullname, user.Email, user.Secretkey, false, 3);
                    var imageUrl = setupInfo.QrCodeSetupImageUrl;
                    _userService.ActivateMfa(id, true, imageUrl);
                    return Ok(new {statuscode = 200, message="2-Factor Authenticator has been enabled.", qrcodeurl = imageUrl});
                } else {
                    return BadRequest(new {statuscode = 400, message="User not found."});
                }

            } else {
                _userService.ActivateMfa(id, false, null);
                return Ok(new {statuscode = 200, message="2-Factor Authenticator has been disabled."});
            }
        }
    }    
}