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
    public class OwnsController : StringKeyController<OwnViewModel, Owns>
    {

        public OwnsController(LiveGameAppContext ctx, ILogger<OwnsController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Owns);
            orderSelectors = new Dictionary<string, Expression<Func<Owns, object>>>
            {
                [nameof(Owns.UserId).ToLower()] = m => m.UserId,
                [nameof(Owns.GameId).ToLower()] = m => m.GameId,
                ["id"] = m => m.UserId.ToString() + "-" + m.GameId.ToString(),
            };
        }

        protected override async Task<List<OwnViewModel>> GenerateList(IQueryable<Owns> query)
        {
            return await query.Select(m => new OwnViewModel
            {
                id = m.UserId.ToString() + "-" + m.GameId.ToString(),
                UserId = m.UserId,
                GameId = m.GameId,

            }).ToListAsync();
        }

        protected override IQueryable<Owns> Filter(IQueryable<Owns> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Owns entry)
        {
            return entry.UserId.ToString() + "-" + entry.GameId.ToString();
        }

        protected override async Task<Owns> GetEntry(IQueryable<Owns> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.GameId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Owns> GetEntriesByIdList(IQueryable<Owns> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.GameId.ToString()));
        }

        protected override Task<Owns> GenerateEntry(OwnViewModel model)
        {
            return Task.FromResult(new Owns { UserId = model.UserId, GameId = model.GameId });
        }

        protected override OwnViewModel GenerateViewModel(Owns entry)
        {
            return new OwnViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.GameId.ToString(),
                UserId = entry.UserId,
                GameId = entry.GameId,

            };
        }

        protected override Task UpdateEntry(Owns entry, OwnViewModel model)
        {
            entry.UserId = model.UserId;
            entry.GameId = model.GameId;

            return Task.CompletedTask;
        }
    }
}
