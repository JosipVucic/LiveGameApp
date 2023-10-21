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
    public class SpectatorsController : StringKeyController<SpectatorViewModel, Spectator>
    {

        public SpectatorsController(LiveGameAppContext ctx, ILogger<SpectatorsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Spectator);
            orderSelectors = new Dictionary<string, Expression<Func<Spectator, object>>>
            {
                [nameof(Spectator.UserId).ToLower()] = m => m.UserId,
                [nameof(Spectator.PlanId).ToLower()] = m => m.PlanId,
                ["id"] = m => m.UserId.ToString() + "-" + m.PlanId.ToString(),
            };
        }

        protected override async Task<List<SpectatorViewModel>> GenerateList(IQueryable<Spectator> query)
        {
            return await query.Select(m => new SpectatorViewModel
            {
                id = m.UserId.ToString() + "-" + m.PlanId.ToString(),
                UserId = m.UserId,
                PlanId = m.PlanId,

            }).ToListAsync();
        }

        protected override IQueryable<Spectator> Filter(IQueryable<Spectator> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Spectator entry)
        {
            return entry.UserId.ToString() + "-" + entry.PlanId.ToString();
        }

        protected override async Task<Spectator> GetEntry(IQueryable<Spectator> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.PlanId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Spectator> GetEntriesByIdList(IQueryable<Spectator> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.PlanId.ToString()));
        }

        protected override Task<Spectator> GenerateEntry(SpectatorViewModel model)
        {
            return Task.FromResult(new Spectator {UserId=model.UserId, PlanId= model.PlanId });
        }

        protected override SpectatorViewModel GenerateViewModel(Spectator entry)
        {
            return new SpectatorViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.PlanId.ToString(),
                UserId = entry.UserId,
                PlanId = entry.PlanId,

            };
        }

        protected override Task UpdateEntry(Spectator entry, SpectatorViewModel model)
        {
            entry.UserId = model.UserId;
            entry.PlanId = model.PlanId;

            return Task.CompletedTask;
        }
    }
}
