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
using LiveGameApp.Extensions.ExceptionFilters;
using LiveGameApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace LiveGameApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class PlanTypesController : IntKeyController<PlanTypeViewModel, Plantype>
    {
        public PlanTypesController(LiveGameAppContext ctx, ILogger<PlanTypesController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Plantype);
            orderSelectors = new Dictionary<string, Expression<Func<Plantype, object>>>
            {
                [nameof(Plantype.Id).ToLower()] = m => m.Id,
                [nameof(Plantype.Name).ToLower()] = m => m.Name,
            };
        }

        protected override async Task<List<PlanTypeViewModel>> GenerateList(IQueryable<Plantype> query)
        {
            return await query.Select(m => new PlanTypeViewModel
            {
                id = m.Id,
                Name = m.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Plantype> Filter(IQueryable<Plantype> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Plantype> GenerateEntry(PlanTypeViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Plantype
            {
                Name = model.Name,
            });
        }

        protected override PlanTypeViewModel GenerateViewModel(Plantype entry)
        {
            if (entry == null) return null;
            return new PlanTypeViewModel
            {
                id = entry.Id,
                Name = entry.Name,

            };
        }

        protected override Task UpdateEntry(Plantype entry, PlanTypeViewModel model)
        {
            entry.Name = model.Name;

            return Task.CompletedTask;
        }
    }
}
