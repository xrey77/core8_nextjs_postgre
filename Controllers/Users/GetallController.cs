using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Entities;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Helpers;

namespace core8_nextjs_postgre.Controllers.Users
{
    [ApiExplorerSettings(GroupName = "List All Users")]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class GetallController : ControllerBase {
       

    private IUserService _userService;

    private IMapper _mapper;
    private readonly IConfiguration _configuration;  

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<GetallController> _logger;

    public GetallController(
        IConfiguration configuration,
        IWebHostEnvironment env,
        IUserService userService,
        IMapper mapper,
        ILogger<GetallController> logger
        )
    {
        _configuration = configuration;  
        _userService = userService;
        _mapper = mapper;
        _logger = logger;
        _env = env;        
    }  

        [HttpGet]
        public IActionResult getAllusers() {
            try {                
                var user = _userService.GetAll();
                var model = _mapper.Map<IList<UserModel>>(user);
                return Ok(model);
            } catch(AppException ex) {
               return BadRequest(new {statuscode = 400, Message = ex.Message});
            }
        }
    }
    
}