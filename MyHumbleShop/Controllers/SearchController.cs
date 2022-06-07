using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyHumbleShop.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchRepo _searchRepo;

        public SearchController(ISearchRepo searchRepo)
        {
            _searchRepo = searchRepo;
        }

        [HttpGet("SearchByCategory")]
        public async Task<ActionResult<ServiceResponse<List<ProductByCategoryDto>>>> GetAllProductByCategory(string category)
        {
            var response = await _searchRepo.SearchByCategory(category);
            if (response == null)
                return NotFound();
            return Ok(response);
        }
            //[HttpGet("SearchByCategory")]
            //public async Task<ActionResult<ServiceResponse<List<ProductByCategoryDto>>>> GetAllProductByCategory(ProductByCategoryDto category)
            //{
            //    var response = await _searchRepo.SearchByCategory(category.Category);
            //    if (response == null)
            //        return NotFound();
            //    return Ok(response);
            //}
        }
}
