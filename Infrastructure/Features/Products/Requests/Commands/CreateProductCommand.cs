using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DTOs.Products;
using MediatR;

namespace Infrastructure.Features.Products.Requests.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public required CreateProductDto createProductDto { get; set; }
    }
}