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
    public class IsGenresController : StringKeyController<IsGenreViewModel, Isgenre>
    {

        public IsGenresController(LiveGameAppContext ctx, ILogger<IsGenresController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Isgenre);
            orderSelectors = new Dictionary<string, Expression<Func<Isgenre, object>>>
            {
                [nameof(Isgenre.GameId).ToLower()] = m => m.GameId,
                [nameof(Isgenre.GenreId).ToLower()] = m => m.GenreId,
                ["id"] = m => m.GameId.ToString() + "-" + m.GenreId.ToString(),
            };
        }

        protected override async Task<List<IsGenreViewModel>> GenerateList(IQueryable<Isgenre> query)
        {
            return await query.Select(m => new IsGenreViewModel
            {
                id = m.GameId.ToString() + "-" + m.GenreId.ToString(),
                GameId = m.GameId,
                GenreId = m.GenreId,

            }).ToListAsync();
        }

        protected override IQueryable<Isgenre> Filter(IQueryable<Isgenre> query, LoadParams loadParams)
        {
            return query.Where(m => m.GameId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Isgenre entry)
        {
            return entry.GameId.ToString() + "-" + entry.GenreId.ToString();
        }

        protected override async Task<Isgenre> GetEntry(IQueryable<Isgenre> query, string id)
        {
            return await query.Where(m => (m.GameId.ToString() + "-" + m.GenreId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Isgenre> GetEntriesByIdList(IQueryable<Isgenre> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.GameId.ToString() + "-" + m.GenreId.ToString()));
        }

        protected override Task<Isgenre> GenerateEntry(IsGenreViewModel model)
        {
            return Task.FromResult(new Isgenre { GameId = model.GameId, GenreId = model.GenreId });
        }

        protected override IsGenreViewModel GenerateViewModel(Isgenre entry)
        {
            return new IsGenreViewModel
            {
                id = entry.GameId.ToString() + "-" + entry.GenreId.ToString(),
                GameId = entry.GameId,
                GenreId = entry.GenreId,

            };
        }

        protected override Task UpdateEntry(Isgenre entry, IsGenreViewModel model)
        {
            entry.GameId = model.GameId;
            entry.GenreId = model.GenreId;

            return Task.CompletedTask;
        }
    }
}
