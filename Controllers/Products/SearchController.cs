using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using core8_nextjs_postgre.Services;
using core8_nextjs_postgre.Models.dto;
using core8_nextjs_postgre.Helpers;
using core8_nextjs_postgre.Models;

namespace core8_nextjs_postgre.Controllers.Products
{
    [ApiExplorerSettings(GroupName = "Search Product Description")]
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase {

        private IProductService _productService;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;  
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<SearchController> _logger;
        private readonly IRabbitMQProducer _rabbitMQProducer;

        public SearchController(
            IConfiguration configuration,
            IWebHostEnvironment env,
            IProductService productService,
            IMapper mapper,
            ILogger<SearchController> logger,
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

        [HttpPost("/api/searchproducts")]
        public async Task<IActionResult> SearchProducts(ProductSearch prod) {
            try {                
                var products = await _productService.SearchAll(prod.Search);
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

                    return Ok(new {products=model});
                } else {
                    return NotFound(new { message="No Data found."});
                }
            } catch(AppException ex) {
               return BadRequest(new { Message = ex.Message});
            }
        }
    }    
}