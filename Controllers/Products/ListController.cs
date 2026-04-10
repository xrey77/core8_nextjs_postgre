using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Helpers;

namespace core8_nextjs_postgre.Controllers.Products
{
    [ApiExplorerSettings(GroupName = "List All Products")]
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase 
    {
        private IProductService _productService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;  
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ListController> _logger;
        private readonly IRabbitMQProducer _rabbitMQProducer;

        public ListController(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IProductService productService,
            IMapper mapper,
            ILogger<ListController> logger,
            IRabbitMQProducer rabbitMQProducer
            )
        {
            _configuration = configuration;  
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
            _env = env;        
            _rabbitMQProducer = rabbitMQProducer;
        }  

        [HttpGet("/api/listproducts/{page}")]
        public async Task<IActionResult> ListProducts(int page) {
            try {                
                int totalpage = await _productService.TotPage();
                var products = await _productService.ListAll(page);
                
                if (products != null) {

                    var model = _mapper.Map<IList<ProductModel>>(products);

                    // START - Publish to RabbitMQ=============
                    try
                    {
                        // FIX: 'await' is now perfectly valid here
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

                    return Ok(new { totpage = totalpage, page = page, products = model });
                } else {
                    return NotFound(new { message = "No Data found." });
                }
            } catch(AppException ex) {
               return BadRequest(new { Message = ex.Message });
            }
        }
    }    
}



// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using core8_nextjs_postgre.Services;
// using core8_nextjs_postgre.Models.dto;
// using core8_nextjs_postgre.Helpers;


// namespace core8_nextjs_postgre.Controllers.Products
// {
//     [ApiExplorerSettings(GroupName = "List All Products")]
//     [ApiController]
//     [Route("[controller]")]
//     public class ListController : ControllerBase {

//         private IProductService _productService;
//         private IMapper _mapper;
//         private readonly IConfiguration _configuration;  
//         private readonly IWebHostEnvironment _env;
//         private readonly ILogger<ListController> _logger;
//         private readonly IRabbitMQProducer _rabbitMQProducer;

//         public ListController(
//             IConfiguration configuration,
//             IWebHostEnvironment env,
//             IProductService productService,
//             IMapper mapper,
//             ILogger<ListController> logger,
//             IRabbitMQProducer rabbitMQProducer
//             )
//         {
//             _configuration = configuration;  
//             _productService = productService;
//             _mapper = mapper;
//             _logger = logger;
//             _env = env;        
//             _rabbitMQProducer = rabbitMQProducer;
//         }  

//         [HttpGet("/api/listproducts/{page}")]
//         public IActionResult ListProducts(int page) {
//             try {                
//                 int totalpage = _productService.TotPage();
//                 var products = _productService.ListAll(page);
//                 if (products != null) {


//                     // START - Publish to RabbitMQ=============
//                     try
//                     {
//                         await _rabbitMQProducer.PublishUserRegisteredEvent(model);
//                     }
//                     catch (UnauthorizedAccessException ex) when (ex.Message.Contains("RabbitMQ credentials"))
//                     {
//                         return StatusCode(500, new { 
//                             success = false, 
//                             message = $"Messaging password service configuration error. Please contact administrator, {ex.Message}" 
//                         });
//                     }
//                     catch (AppException ex)
//                     {
//                         return StatusCode(500, new { 
//                             success = false, 
//                             message = $"An unexpected error occurred, {ex.Message}" 
//                         });
//                     }
//                     // END - Publish to RabbitMQ==============


//                     var model = _mapper.Map<IList<ProductModel>>(products);
//                     return Ok(new {totpage = totalpage, page = page, products=model});
//                 } else {
//                     return NotFound(new { message="No Data found."});
//                 }
//             } catch(AppException ex) {
//                return BadRequest(new {Message = ex.Message});
//             }
//         }
//     }    
// }