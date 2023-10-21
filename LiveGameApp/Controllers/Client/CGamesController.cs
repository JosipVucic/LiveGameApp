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
using System.Security.Claims;
using LiveGameApp.ViewModels.Filters;
using System.Text.Json;

namespace LiveGameApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class CGamesController : IntKeyController<CGameViewModel, Game>
    {
        public CGamesController(LiveGameAppContext ctx, ILogger<CGamesController> logger) : base(ctx, logger)
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

        protected override async Task<List<CGameViewModel>> GenerateList(IQueryable<Game> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            var owned = ctx.Owns.Where(m => m.UserId == userId).Include(m => m.Game).Select(m => m.Game).AsQueryable();

            return await query.Select(m => new CGameViewModel
            {
                id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Rules = m.Rules,
                MinPlayers = m.MinPlayers,
                MaxPlayers = m.MaxPlayers,
                ImageUrl = m.ImageUrl,
                IsOwned = owned.Contains(m),

            }).ToListAsync();
        }

        protected override IQueryable<Game> Filter(IQueryable<Game> query, LoadParams loadParams)
        {
            GameFilter filter = JsonSerializer.Deserialize<GameFilter>(loadParams.Filter);
            if (filter.OwnerIds != null && filter.OwnerIds.Length != 0)
                query = ctx.Owns.Where(m => filter.OwnerIds.Contains(m.UserId)).Include(m => m.Game).Select(m => m.Game).AsQueryable();
            if (filter.GenreIds != null && filter.GenreIds.Length != 0)
                query = query.Intersect(ctx.Isgenre.Where(m => filter.GenreIds.Contains(m.GenreId)).Include(m => m.Game).Select(m => m.Game));
            if (filter.q != null)
                query = query.Where(m => m.Name.ToLower().Contains(filter.q.ToLower()));

            return query;
        }

        protected override async Task<Game> GenerateEntry(CGameViewModel model)
        {
            if (model == null) return null;

            var generalEntry = new Reviewable { AverageRating = null };
            ctx.Add(generalEntry);
            await ctx.SaveChangesAsync();

            foreach (int genreId in model.GenreIds)
            {
                ctx.Isgenre.Add(new Isgenre { GameId = generalEntry.Id, GenreId = genreId });
            }
            foreach (int authorId in model.AuthorIds)
            {
                ctx.Author.Add(new Author { GameId = generalEntry.Id, UserId = authorId });
            }

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

        protected override CGameViewModel GenerateViewModel(Game entry)
        {
            if (entry == null) return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            var owned = ctx.Owns.Where(m => m.UserId == userId).Include(m => m.Game).Select(m => m.Game).AsQueryable();

            var AuthorEntries = ctx.Author.Where(m => m.GameId == entry.Id);
            var GenreEntries = ctx.Isgenre.Where(m => m.GameId == entry.Id);
            int[] AuthorIds = AuthorEntries.Select(m => m.UserId).ToArray();
            int[] GenreIds = GenreEntries.Select(m => m.GenreId).ToArray();

            string[] Authors = AuthorEntries.Include(m => m.User).Select(m => m.User.UserName).ToArray();
            string[] Genres = GenreEntries.Include(m => m.Genre).Select(m => m.Genre.Name).ToArray();

            double? average = ctx.Reviewable.Find(entry.Id).AverageRating;
            string averageString = null;
            if (average.HasValue)
                averageString = String.Format("{0:0.00}", average);

            return new CGameViewModel
            {
                id = entry.Id,
                Name = entry.Name,
                Description = entry.Description,
                Rules = entry.Rules,
                MinPlayers = entry.MinPlayers,
                MaxPlayers = entry.MaxPlayers,
                ImageUrl = entry.ImageUrl,
                IsOwned = owned.Contains(entry),
                IsAuthor = AuthorIds.Contains(userId),

                AuthorIds = AuthorIds,
                GenreIds = GenreIds,
                Authors = Authors,
                Genres = Genres,
                Rating = averageString

            };
        }

        protected override Task UpdateEntry(Game entry, CGameViewModel model)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            //Console.WriteLine("UserID: ");
            //Console.WriteLine(userId);


            if (model.IsOwned)
            {
                if (ctx.Owns.Find(userId, model.id) == null)
                {
                    ctx.Add(new Owns { 
                        UserId = userId, 
                        GameId = model.id.Value });
                }
            }
            else
            {
                Owns own = ctx.Owns.Find(userId, model.id);
                if (own != null)
                {
                    ctx.Remove(own);
                }
            }

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
