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
    public class AuthorsController : StringKeyController<AuthorViewModel, Author>
    {

        public AuthorsController(LiveGameAppContext ctx, ILogger<AuthorsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Author);
            orderSelectors = new Dictionary<string, Expression<Func<Author, object>>>
            {
                [nameof(Author.UserId).ToLower()] = m => m.UserId,
                [nameof(Author.GameId).ToLower()] = m => m.GameId,
                ["id"] = m => m.UserId.ToString() + "-" + m.GameId.ToString(),
            };
        }

        protected override async Task<List<AuthorViewModel>> GenerateList(IQueryable<Author> query)
        {
            return await query.Select(m => new AuthorViewModel
            {
                id = m.UserId.ToString() + "-" + m.GameId.ToString(),
                UserId = m.UserId,
                GameId = m.GameId,

            }).ToListAsync();
        }

        protected override IQueryable<Author> Filter(IQueryable<Author> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Author entry)
        {
            return entry.UserId.ToString() + "-" + entry.GameId.ToString();
        }

        protected override async Task<Author> GetEntry(IQueryable<Author> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.GameId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Author> GetEntriesByIdList(IQueryable<Author> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.GameId.ToString()));
        }

        protected override Task<Author> GenerateEntry(AuthorViewModel model)
        {
            return Task.FromResult(new Author { UserId = model.UserId, GameId = model.GameId });
        }

        protected override AuthorViewModel GenerateViewModel(Author entry)
        {
            return new AuthorViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.GameId.ToString(),
                UserId = entry.UserId,
                GameId = entry.GameId,

            };
        }

        protected override Task UpdateEntry(Author entry, AuthorViewModel model)
        {
            entry.UserId = model.UserId;
            entry.GameId = model.GameId;

            return Task.CompletedTask;
        }
    }
}
