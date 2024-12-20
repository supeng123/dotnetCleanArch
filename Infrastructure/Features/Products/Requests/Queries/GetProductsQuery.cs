using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DTOs.Products;
using Infrastructure.Specifications.product;
using MediatR;

namespace Infrastructure.Features.Products.Requests.Queries
{
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
        public required ProductSpecParams specParams { get; set; }
    }
}