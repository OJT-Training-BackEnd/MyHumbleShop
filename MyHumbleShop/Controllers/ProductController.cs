using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        [HttpPost("ExprotToExcel")]
        public async Task<ActionResult<ServiceResponse<List<Products>>>> ExprotToExcel()
        {
            List<Products> _products = await _productRepo.ListProduct();
            DataTable dt = new DataTable("Product");
            dt.Columns.AddRange(new DataColumn[2]{
                new DataColumn("productName"),
                new DataColumn("price")
            });

            foreach (var emp in _products)
            {
                dt.Rows.Add(emp.ProductName, emp.Price);
            }
            using (ClosedXML.Excel.XLWorkbook wb = new ClosedXML.Excel.XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                        , "Product.xlsx"
                        );
                }
            }

        }
    }
}
