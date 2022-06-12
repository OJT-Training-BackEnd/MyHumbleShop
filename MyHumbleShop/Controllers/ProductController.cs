using ClosedXML.Excel;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyHumbleShop.Dtos.Product;
using MyHumbleShop.Models;
using MyHumbleShop.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System;
using System.IO;
using System.Net.Http;
using OfficeOpenXml;

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
        public async Task<ActionResult<ServiceResponse<List<EditProductDto>>>> EditProduct(string id, EditProductDto editProductDto)
        {

            return Ok(await _productRepo.EditProduct(editProductDto, id));

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
            dt.Columns.AddRange(new DataColumn[6]{
                new DataColumn("productName"),
                new DataColumn("description"),
                new DataColumn("price"),
                new DataColumn("quantity"),
                new DataColumn("categoryID"),
                new DataColumn("status")

            });

            foreach (var emp in _products)
            {
                dt.Rows.Add(emp.ProductName, emp.Description, emp.Price, emp.Quantity, emp.Category, emp.Status);
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
        [AllowAnonymous]
        [Route("ReadFile")]
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<Products>>>> ReadFileExcel()
        {

            try
            {
                IFormFileCollection httpRequest = HttpContext.Request.Form.Files;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                IFormFile Inputfile = null;
                Stream FileStream = null;



                Inputfile = httpRequest[0];
                FileStream = Inputfile.OpenReadStream();
                ExcelPackage.LicenseContext = LicenseContext.Commercial;
/*                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
*/                using (var package = new ExcelPackage(FileStream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;

                    for (var row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var name = worksheet.Cells[row, 1].Value?.ToString();
                            var description = worksheet.Cells[row, 2].Value?.ToString();
                            var price = worksheet.Cells[row, 3].Value?.ToString();
                            var quantity = worksheet.Cells[row, 4].Value?.ToString();
                            var categoryId = worksheet.Cells[row, 5].Value?.ToString();
                           /* var status = Boolean.Parse(worksheet.Cells[row, 6].Value?.ToString());*/

                            var product = new Products()
                            {
                                ProductName = name,
                                Description = description,
                                Price = price,
                                Quantity = quantity,
                                Category = categoryId,
                              /*  Status = status*/
                            };
                            var products = await _productRepo.AddProduct(product);
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
                return new ServiceResponse<List<Products>>()
                {
                    
                    Success = true,
                    Message = "Ok"
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}


 



    

