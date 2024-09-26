using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.RequestHelpers;
using Core.Interfaces.common;
using Domain.Entities.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.common
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> repo,
            ISpecification<T> spec, int pageIndex, int pageSize) where T : BaseEntity
        {
            var items = await repo.ListAsync(spec);
            var count = await repo.CountAsync(spec);

            var pagination = new Pagination<T>(pageIndex, pageSize, count, items);

            return Ok(pagination);
        }

        protected async Task<ActionResult> CreatePagedResult<T, TDto>(IGenericRepository<T> repo,
            ISpecification<T> spec, int pageIndex, int pageSize, Func<T, TDto> toDto) where T
                : BaseEntity
        {
            var items = await repo.ListAsync(spec);
            var count = await repo.CountAsync(spec);

            var dtoItems = items.Select(toDto).ToList();

            var pagination = new Pagination<TDto>(pageIndex, pageSize, count, dtoItems);

            return Ok(pagination);
        }
    }
}