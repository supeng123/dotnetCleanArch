using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Product;
using Infrastructure.Repository;

namespace Infrastructure.Specifications.product
{
    public class TypeListSpecification : BaseSpecification<Product, string>
    {
        public TypeListSpecification()
        {
            AddSelect(x => x.Type);
            ApplyDistinct();
        }
    }
}