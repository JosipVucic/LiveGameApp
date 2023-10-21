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
    public class ReviewsController : StringKeyController<ReviewViewModel, Reviews>
    {

        public ReviewsController(LiveGameAppContext ctx, ILogger<ReviewsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Reviews);
            orderSelectors = new Dictionary<string, Expression<Func<Reviews, object>>>
            {
                [nameof(Reviews.UserId).ToLower()] = m => m.UserId,
                [nameof(Reviews.ReviewableId).ToLower()] = m => m.ReviewableId,
                [nameof(Reviews.Rating).ToLower()] = m => m.Rating,
                [nameof(Reviews.Content).ToLower()] = m => m.Content,
                ["id"] = m => m.UserId.ToString() + "-" + m.ReviewableId.ToString(),
            };
        }

        protected override async Task<List<ReviewViewModel>> GenerateList(IQueryable<Reviews> query)
        {
            return await query.Select(m => new ReviewViewModel
            {
                id = m.UserId.ToString() + "-" + m.ReviewableId.ToString(),
                UserId = m.UserId,
                ReviewableId = m.ReviewableId,

            }).ToListAsync();
        }

        protected override IQueryable<Reviews> Filter(IQueryable<Reviews> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Reviews entry)
        {
            return entry.UserId.ToString() + "-" + entry.ReviewableId.ToString();
        }

        protected override async Task<Reviews> GetEntry(IQueryable<Reviews> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.ReviewableId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Reviews> GetEntriesByIdList(IQueryable<Reviews> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.ReviewableId.ToString()));
        }

        protected override Task<Reviews> GenerateEntry(ReviewViewModel model)
        {
            return Task.FromResult(new Reviews { UserId = model.UserId, ReviewableId = model.ReviewableId });
        }

        protected override ReviewViewModel GenerateViewModel(Reviews entry)
        {
            return new ReviewViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.ReviewableId.ToString(),
                UserId = entry.UserId,
                ReviewableId = entry.ReviewableId,

            };
        }

        protected override Task UpdateEntry(Reviews entry, ReviewViewModel model)
        {
            entry.UserId = model.UserId;
            entry.ReviewableId = model.ReviewableId;

            return Task.CompletedTask;
        }

    }
}
