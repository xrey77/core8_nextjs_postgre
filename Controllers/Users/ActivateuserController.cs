using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Helpers;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "Forgot User Password")]
    [ApiController]
    [Route("[controller]")]
    public class ActivateUserController : ControllerBase {
    private IUserService _userService;
    private EmailService _emailService;    
    private readonly IConfiguration _configuration;  
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ActivateUserController> _logger;
    private readonly IRabbitMQProducer _rabbitMQProducer;

    public ActivateUserController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        EmailService emailService,
        IUserService userService,
        ILogger<ActivateUserController> logger,
        IRabbitMQProducer rabbitMQProducer
        )
    {
        _configuration = configuration;  
        _emailService = emailService;
        _userService = userService;
        _logger = logger;
        _env = env;        
        _rabbitMQProducer = rabbitMQProducer;
    }  

        [HttpGet("/api/activateuser/{id}")]
        public async Task<IActionResult> ActivateUser(int id) {
            try
            {
                    //GET USER INFO
                    var user = await _userService.GetById(id);
                    string email = user.Email;
                    string fullname = user.FirstName + " " + user.LastName;
                    string subj = "Account Activation Confirmation";
                    string htmlmsg = "<div><p><strong>Congratiolation</strong>, your Account has been activated successfully..</p></div>";
                   await _userService.ActivateUser(id);
                    //SEND ACTIONVATION CONFIRMATION
                  _emailService.sendMail(email, fullname, subj, htmlmsg);

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

                return Ok(new { message = "Your Account is activated successfully."});
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }    
}