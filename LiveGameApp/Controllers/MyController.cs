using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LiveGameApp.Models;
using LiveGameApp.ViewModels;

namespace LiveGameApp.Controllers
{
    public abstract class MyController<TKey, TModel, TDBModel> : ControllerBase, ICustomController<TKey, TModel>
        where TDBModel : class
    {
        protected readonly LiveGameAppContext ctx;
        protected readonly ILogger<MyController<TKey, TModel, TDBModel>> logger;
        protected Dictionary<string, Expression<Func<TDBModel, object>>> orderSelectors;
        protected string dbSetName;
        public MyController(LiveGameAppContext ctx, ILogger<MyController<TKey, TModel, TDBModel>> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
            this.dbSetName = null!;
            this.orderSelectors = null!;
        }

        [HttpGet]
        public virtual async Task<List<TModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            IQueryable<TDBModel> query = ((DbSet<TDBModel>)ctx[dbSetName]).AsQueryable();
            int count;

            if (loadParams.Ids != null)
            {
                query = GetEntriesByIdList(query, loadParams);
                count = query.Count();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(loadParams.Filter))
                {
                    query = Filter(query, loadParams);
                }

                if (loadParams.Field != null)
                {
                    if (orderSelectors.TryGetValue(loadParams.Field.ToLower(), out var expr))
                    {
                        query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                    }
                }

                query = SafetyFilter(query);

                count = query.Count();

                query = query.Skip(loadParams.Start)
                             .Take(loadParams.PerPage);
            }

            List<TModel> list = await GenerateList(query);

            Response.Headers.Add("Content-Range", "entries " + loadParams.Start + "-" + (loadParams.Start + list.Count) + "/" + count);

            return list;
        }

        protected virtual IQueryable<TDBModel> SafetyFilter(IQueryable<TDBModel> query)
        {
            return query;
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<TModel>> GetOne(TKey id)
        {
            IQueryable<TDBModel> query = ((DbSet<TDBModel>)ctx[dbSetName]).AsQueryable();
            var existing = GenerateViewModel(await GetEntry(query, id));

            if (existing == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                return existing;
            }
        }

        [HttpPost]
        public virtual async Task<IActionResult> Create(TModel model)
        {
            TDBModel entry = await GenerateEntry(model);

            if (entry == null) return new BadRequestResult();

            ctx.Add(entry);
            await ctx.SaveChangesAsync();

            TKey id = GetId(entry);
            var addedItem = await GetOne(id);

            return CreatedAtAction(nameof(GetOne), new { id = id }, addedItem.Value);
        }

        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(TKey id)
        {
            IQueryable<TDBModel> query = ((DbSet<TDBModel>)ctx[dbSetName]).AsQueryable();
            TDBModel existing = await GetEntry(query, id);
            if (existing == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                TModel removed = GenerateViewModel(existing);
                await RemoveEntry(existing);
                await ctx.SaveChangesAsync();

                return Ok(removed);
            };
        }

        [HttpDelete]
        public virtual async Task<IActionResult> DeleteMany([FromQuery] LoadParams loadParams)
        {
            IQueryable<TDBModel> query = ((DbSet<TDBModel>)ctx[dbSetName]).AsQueryable();
            List<TDBModel> existing = await GetEntriesByIdList(query, loadParams).ToListAsync();
            if (existing == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid ids = {loadParams.IdList}");
            }
            else
            {
                await RemoveEntry(existing);
                await ctx.SaveChangesAsync();

                return Ok(loadParams.IdList);
            };
        }

        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Update(TKey id, TModel model)
        {
            IQueryable<TDBModel> query = ((DbSet<TDBModel>)ctx[dbSetName]).AsQueryable();
            if (!GetId(model).Equals(id))
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {GetId(model)}");
            }
            else
            {
                var existing = await GetEntry(query, id);
                if (existing == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                await UpdateEntry(existing, model);

                await ctx.SaveChangesAsync();

                var updatedItem = await GetOne(GetId(existing));

                return Ok(updatedItem);
            }
        }

        [HttpPut]
        public virtual async Task<IActionResult> UpdateMany([FromQuery] LoadParams loadParams, TModel model)
        {
            IQueryable<TDBModel> query = ((DbSet<TDBModel>)ctx[dbSetName]).AsQueryable();
            var existings = await GetEntriesByIdList(query,loadParams).ToListAsync();
            if (existings == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {loadParams.IdList}");
            }
            foreach (var existing in existings)
            {
                await UpdateEntry(existing, model);
            }
            await ctx.SaveChangesAsync();

            return Ok(loadParams.IdList);
        }
        protected abstract Task<List<TModel>> GenerateList(IQueryable<TDBModel> query);
        protected abstract IQueryable<TDBModel> Filter(IQueryable<TDBModel> query, LoadParams loadParams);
        protected abstract TKey GetId(TDBModel entry);
        protected abstract TKey GetId(TModel model);
        protected abstract Task<TDBModel> GetEntry(IQueryable<TDBModel> query, TKey id);
        protected abstract IQueryable<TDBModel> GetEntriesByIdList(IQueryable<TDBModel> query, LoadParams loadParams);
        protected abstract Task<TDBModel> GenerateEntry(TModel model);
        protected abstract TModel GenerateViewModel(TDBModel entry);
        protected abstract Task UpdateEntry(TDBModel entry, TModel model);
        protected abstract Task RemoveEntry(TDBModel entry);
        protected abstract Task RemoveEntry(List<TDBModel> entries);



    }
}
