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
    public class GameGenresController : IntKeyController<GameGenreViewModel, Gamegenre>
    {

        public GameGenresController(LiveGameAppContext ctx, ILogger<GameGenresController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Gamegenre);
            orderSelectors = new Dictionary<string, Expression<Func<Gamegenre, object>>>
            {
                [nameof(Gamegenre.Id).ToLower()] = m => m.Id,
                [nameof(Gamegenre.Name).ToLower()] = m => m.Name,
            };
        }
        [AllowAnonymous]
        public override Task<List<GameGenreViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            return base.GetAll(loadParams);
        }

        protected override async Task<List<GameGenreViewModel>> GenerateList(IQueryable<Gamegenre> query)
        {
            return await query.Select(m => new GameGenreViewModel
            {
                id = m.Id,
                Name = m.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Gamegenre> Filter(IQueryable<Gamegenre> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Gamegenre> GenerateEntry(GameGenreViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Gamegenre { Name = model.Name });
        }

        protected override GameGenreViewModel GenerateViewModel(Gamegenre entry)
        {
            if (entry == null) return null;
            return new GameGenreViewModel
            {
                id = entry.Id,
                Name = entry.Name,

            };
        }

        protected override Task UpdateEntry(Gamegenre entry, GameGenreViewModel model)
        {
            entry.Name = model.Name;

            return Task.CompletedTask;
        }
    }
}
