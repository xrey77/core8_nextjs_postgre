using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;


namespace core8_nextjs_postgre.Controllers.Auth;
    
[ApiExplorerSettings(GroupName = "Sign-up or Account Registration")]
[ApiController]
[Route("[controller]")]
public class RegisterController : ControllerBase
{
    private IAuthService _authService;
/*    private EmailService _emailService;    */
    private IMapper _mapper;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _configuration;  
    private readonly ILogger<RegisterController> _logger;
    private readonly IRabbitMQProducer _rabbitMQProducer;
    
    public RegisterController(
        IWebHostEnvironment env,
        IAuthService userService,
/*        EmailService emailService, */
        IConfiguration configuration,
        IMapper mapper,
        IRabbitMQProducer rabbitMQProducer,
        ILogger<RegisterController> logger
        )
    {   
        _authService = userService;
/*        _emailService = emailService; */
        _configuration = configuration;  
        _mapper = mapper;
        _logger = logger;
        _env = env;
        _rabbitMQProducer = rabbitMQProducer;
    }  

    [HttpPost("/signup")]
    public async Task<IActionResult> signup(UserRegister model) {
            var user = _mapper.Map<User>(model);

            try
            {
                user.LastName = model.Lastname;
                user.FirstName = model.Firstname;
                user.Email = model.Email;
                user.Mobile = model.Mobile;
                user.UserName = model.Username;
                user.Isactivated = 1;

                User newUser = await _authService.SignupUser(user, model.Password);
/*
                string fullname = model.Firstname + " " + model.Lastname;
                string emailaddress = model.Email;
                string htmlmsg = "<div><p>Please click Activate button below to confirm you email address and activate your account.</p>"+
                            "<a href=\"https://localhost:7280/api/activateuser/id=" + user.Id.ToString() + "\" style=\"background-color: green;color:white;text-decoration: none;border-radius: 20px; \">&nbsp;&nbsp; Activate Account &nbsp;&nbsp;</a></div>";
                string subject = "Barclays Account Activation";                
                IF YOU WISH TO USE USER EMAIL ACTIVATION, JUST UNCOMMENT _emailService.sendMail
                _emailService.sendMail(emailaddress, fullname, subject, htmlmsg);
                //and comment  user.Isactivated = 1;
*/    

                // Create the payload for RabbitMQ
                var emailPayload = new
                {
                    UserId = newUser.Id,
                    Email = newUser.Email,
                    FullName = $"{newUser.FirstName} {newUser.LastName}"
                };

                // START - Publish to RabbitMQ===========
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
                // END - Publish to RabbitMQ=============

                return Ok(new {message = "You have registered successfully, please login now."});
            }
            catch (AppException ex)
            {
                return BadRequest(new {message = ex.Message });
            }
    }
}
    
