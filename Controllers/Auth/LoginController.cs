using System.Text;
using AutoMapper;
using System;
using System.Diagnostics;
using core8_nextjs_postgre.Services;
using Microsoft.AspNetCore.Mvc;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Helpers;
using System.Security.Claims;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace core8_nextjs_postgre.Controllers.Auth;
    
[ApiExplorerSettings(GroupName = "Sign-in to User Account")]
[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly IJWTTokenServices _jwttokenservice;

    private IAuthService _authService;
    private IMapper _mapper;
    private readonly IConfiguration _configuration;  
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<LoginController> _logger;
    private readonly IRabbitMQProducer _rabbitMQProducer;

    public LoginController(
        IJWTTokenServices jWTTokenServices,
        IConfiguration configuration,
        IWebHostEnvironment env,
        IAuthService authService,
        IMapper mapper,
        ILogger<LoginController> logger,
        IRabbitMQProducer rabbitMQProducer
        )
    {
        _jwttokenservice = jWTTokenServices;        
        _configuration = configuration;  
        _authService = authService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
        _rabbitMQProducer = rabbitMQProducer;
    }  

    [HttpPost("/signin")]
    public async Task<IActionResult> signin([FromBody]UserLogin model) 
    {
        try 
        {
            var xuser = await _authService.SignUser(model.Username, model.Password);
            if (xuser is not null) 
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, xuser.Id.ToString()),
                        new Claim(ClaimTypes.Name, xuser.UserName)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };
                
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var xroles = await _authService.getRolename(xuser.RolesId);
                var qrcode = xuser.Qrcodeurl == "" ? null : xuser.Qrcodeurl;                

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
                    message = "Login Successfull, please wait..",
                    id = xuser.Id,
                    lastname = xuser.LastName,
                    firstname = xuser.FirstName,
                    username = xuser.UserName,
                    roles = xroles.Name,
                    isactivated = xuser.Isactivated,
                    isblocked = xuser.Isblocked,
                    profilepic = xuser.Profilepic,
                    qrcodeurl = qrcode,
                    token = tokenString
                });
            } 
            else 
            {
                return NotFound(new { message = "Username not found.." });
            }
        }
        catch (AppException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
 }    
