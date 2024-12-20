using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Interfaces.common;
using Domain.Entities.Product;
using Infrastructure.DTOs.Products;
using Infrastructure.Features.Products.Requests.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.Features.Products.Handlers.Commands
{
    public class CreateProductRequestCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateProductRequestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = _mapper.Map<Product>(request.createProductDto);
            _unitOfWork.Repository<Product>().Add(productEntity);

            if (await _unitOfWork.Complete())
            {
                return _mapper.Map<ProductDto>(productEntity);
            }

            return new ProductDto();
        }
    }
}