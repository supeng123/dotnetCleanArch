using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Product;
using Infrastructure.DTOs.Products;
using Infrastructure.Features.Products.Requests.Commands;
using Infrastructure.Features.Products.Requests.Queries;
using Infrastructure.Specifications.product;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductMediatRController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductMediatRController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (ModelState.IsValid)
            {
                var command = new CreateProductCommand { createProductDto = createProductDto };
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            return BadRequest("There's something wrong with the data.");
        }

        [HttpGet]
        public async Task<ActionResult<ProductDto>> GetProducts([FromQuery] ProductSpecParams specParams)
        {
            if (ModelState.IsValid)
            {
                var query = new GetProductsQuery { specParams = specParams };
                var response = await _mediator.Send(query);
                return Ok(response);
            }
            return BadRequest("There's something wrong with the data.");
        }
    }
}