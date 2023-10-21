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
    public class PlayersController : StringKeyController<PlayerViewModel, Player>
    {

        public PlayersController(LiveGameAppContext ctx, ILogger<PlayersController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Player);
            orderSelectors = new Dictionary<string, Expression<Func<Player, object>>>
            {
                [nameof(Player.UserId).ToLower()] = m => m.UserId,
                [nameof(Player.PlanId).ToLower()] = m => m.PlanId,
                ["id"] = m => m.UserId.ToString() + "-" + m.PlanId.ToString(),
            };
        }

        protected override async Task<List<PlayerViewModel>> GenerateList(IQueryable<Player> query)
        {
            return await query.Select(m => new PlayerViewModel
            {
                id = m.UserId.ToString() + "-" + m.PlanId.ToString(),
                UserId = m.UserId,
                PlanId = m.PlanId,

            }).ToListAsync();
        }

        protected override IQueryable<Player> Filter(IQueryable<Player> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Player entry)
        {
            return entry.UserId.ToString() + "-" + entry.PlanId.ToString();
        }

        protected override async Task<Player> GetEntry(IQueryable<Player> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.PlanId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Player> GetEntriesByIdList(IQueryable<Player> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.PlanId.ToString()));
        }

        protected override Task<Player> GenerateEntry(PlayerViewModel model)
        {
            return Task.FromResult(new Player { UserId = model.UserId, PlanId = model.PlanId });
        }

        protected override PlayerViewModel GenerateViewModel(Player entry)
        {
            return new PlayerViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.PlanId.ToString(),
                UserId = entry.UserId,
                PlanId = entry.PlanId,

            };
        }

        protected override Task UpdateEntry(Player entry, PlayerViewModel model)
        {
            entry.UserId = model.UserId;
            entry.PlanId = model.PlanId;

            return Task.CompletedTask;
        }
    }
}
