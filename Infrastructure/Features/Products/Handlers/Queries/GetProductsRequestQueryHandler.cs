using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Interfaces.common;
using Domain.Entities.Product;
using Infrastructure.DTOs.Products;
using Infrastructure.Features.Products.Requests.Queries;
using Infrastructure.Specifications.product;
using MediatR;

namespace Infrastructure.Features.Products.Handlers.Queries
{
    public class GetProductsRequestQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;

        public GetProductsRequestQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }
        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            var spec = new ProductSpecification(request.specParams);
            var products = await unitOfWork.Repository<Product>().ListAsync(spec);
            return mapper.Map<List<ProductDto>>(products);
        }
    }
}