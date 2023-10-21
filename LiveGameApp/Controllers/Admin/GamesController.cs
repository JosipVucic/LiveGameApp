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
    public class GamesController : IntKeyController<GameViewModel, Game>
    {

        public GamesController(LiveGameAppContext ctx, ILogger<GamesController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Game);
           orderSelectors = new Dictionary<string, Expression<Func<Game, object>>>
           {
               [nameof(Game.Id).ToLower()] = m => m.Id,
               [nameof(Game.Name).ToLower()] = m => m.Name,
               [nameof(Game.Description).ToLower()] = m => m.Description,
               [nameof(Game.Rules).ToLower()] = m => m.Rules,
               [nameof(Game.MinPlayers).ToLower()] = m => m.MinPlayers,
               [nameof(Game.MaxPlayers).ToLower()] = m => m.MaxPlayers,
               [nameof(Game.ImageUrl).ToLower()] = m => m.ImageUrl,
           };
        }


        protected override async Task<List<GameViewModel>> GenerateList(IQueryable<Game> query)
        {
            return await query.Select(m => new GameViewModel
            {
                id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Rules = m.Rules,
                MinPlayers = m.MinPlayers,
                MaxPlayers = m.MaxPlayers,
                ImageUrl = m.ImageUrl,

            }).ToListAsync();
        }

        protected override IQueryable<Game> Filter(IQueryable<Game> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override async Task<Game> GenerateEntry(GameViewModel model)
        {
            if (model == null) return null;

            var generalEntry = new Reviewable { AverageRating = null };
            ctx.Add(generalEntry);
            await ctx.SaveChangesAsync();
            return new Game
            {
                Id = generalEntry.Id,
                Name = model.Name,
                Description = model.Description,
                Rules = model.Rules,
                MinPlayers = model.MinPlayers,
                MaxPlayers = model.MaxPlayers,
                ImageUrl = model.ImageUrl,
            };
        }

        protected override async Task RemoveEntry(Game entry)
        {
            Reviewable baseExisting = await ctx.Reviewable.Where(m => m.Id == entry.Id).FirstOrDefaultAsync();
            ctx.Remove(baseExisting);
        }

        protected override async Task RemoveEntry(List<Game> entries)
        {
            List<Reviewable> baseExisting;
            List<int> keys = entries.ConvertAll(m => m.Id);

            baseExisting = await ctx.Reviewable.Where(m => keys.Contains(m.Id)).ToListAsync();

            ctx.RemoveRange(baseExisting);
        }

        protected override GameViewModel GenerateViewModel(Game entry)
        {
            if (entry == null) return null;
            return new GameViewModel
            {
                id = entry.Id,
                Name = entry.Name,
                Description = entry.Description,
                Rules = entry.Rules,
                MinPlayers = entry.MinPlayers,
                MaxPlayers = entry.MaxPlayers,
                ImageUrl = entry.ImageUrl,

            };
        }

        protected override Task UpdateEntry(Game entry, GameViewModel model)
        {
            entry.Name = model.Name;
            entry.Description = model.Description;
            entry.Rules = model.Rules;
            entry.MinPlayers = model.MinPlayers;
            entry.MaxPlayers = model.MaxPlayers;
            entry.ImageUrl = model.ImageUrl;

            return Task.CompletedTask;
        }
    }
}
