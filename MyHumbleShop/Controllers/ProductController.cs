using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHumbleShop.Controllers
{
    [Authorize(Roles = "ADMIN")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepo _productRepo;

        public ProductController(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }


        [HttpGet("GetAllProduct")]
        public async Task<ActionResult<ServiceResponse<List<Products>>>> GetAllProducts()
        {
            try
            {
                return Ok(await _productRepo.GetProductAll());

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
        

        [HttpGet("GetProductDetails/{id}")]
        public async Task<ActionResult<ServiceResponse<List<Products>>>> GetProductsDetail(string id)
        {
            try
            {
                return Ok(await _productRepo.GetProductDetails(id));

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpPost("AddProduct")]

        public async Task<ActionResult<ServiceResponse<List<Products>>>> AddProduct(Products products)
        {
            try
            {
                return Ok(await _productRepo.AddProduct(products));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpPut("EditProduct")]
        public async Task<ActionResult<ServiceResponse<List<EditProductDto>>>> EditProduct(string id ,EditProductDto editProductDto)
        {
           
                return Ok(await _productRepo.EditProduct(editProductDto,id));
           
        }

        [HttpDelete("RemoveProduct")]
        public async Task<ActionResult<ServiceResponse<List<Products>>>> RemoveProduct(string id)
        {
            try
            {
                return Ok(await _productRepo.DeleteProduct(id));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }
    }
}
