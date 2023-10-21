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
using System.Text.Json;

namespace LiveGameApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class CReviewsController : StringKeyController<CReviewViewModel, Reviews>
    {

        public CReviewsController(LiveGameAppContext ctx, ILogger<CReviewsController> logger) : base(ctx, logger)
        {
            dbSetName = nameof(Reviews);
            orderSelectors = new Dictionary<string, Expression<Func<Reviews, object>>>
            {
                [nameof(Reviews.UserId).ToLower()] = m => m.UserId,
                [nameof(Reviews.ReviewableId).ToLower()] = m => m.ReviewableId,
                [nameof(Reviews.Rating).ToLower()] = m => m.Rating,
                [nameof(Reviews.Content).ToLower()] = m => m.Content,
                ["id"] = m => m.UserId.ToString() + "-" + m.ReviewableId.ToString(),
            };
        }

        protected override async Task<List<CReviewViewModel>> GenerateList(IQueryable<Reviews> query)
        {
            query = query.Include(m => m.User);

            return await query.Select(m => new CReviewViewModel
            {
                id = m.UserId.ToString() + "-" + m.ReviewableId.ToString(),
                UserId = m.UserId,
                ReviewableId = m.ReviewableId,
                Rating = m.Rating,
                Content = m.Content,
                User = m.User.UserName,

            }).ToListAsync();
        }

        protected override IQueryable<Reviews> Filter(IQueryable<Reviews> query, LoadParams loadParams)
        {
            ReviewViewModel filter = JsonSerializer.Deserialize<ReviewViewModel>(loadParams.Filter);

            if (filter.ReviewableId != 0)
                query = query.Where(m => m.ReviewableId == filter.ReviewableId);

            return query;
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
            return query.Where(d => loadParams.StringIdList.Contains(d.UserId.ToString() + "-" + d.ReviewableId.ToString()));
        }

        protected override async Task<Reviews> GenerateEntry(CReviewViewModel model)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            var existing = ctx.Reviews.Find(userId, model.ReviewableId);
            if (existing != null)
                ctx.Remove(existing);

            await ctx.SaveChangesAsync();

            int[] ratings = ctx.Reviews.Where(m => m.ReviewableId == model.ReviewableId).Select(m => m.Rating).ToArray();
            ratings = ratings.Append(model.Rating).ToArray();
            double average = ratings.Average();

            ctx.Reviewable.Find(model.ReviewableId).AverageRating = average;

            return new Reviews
            {
                UserId = userId,
                ReviewableId = model.ReviewableId,
                Rating = model.Rating,
                Content = model.Content,
            };
        }

        public override async Task<IActionResult> Delete(string id)
        {
            int reviewableId = (await GetEntry(ctx.Reviews, id)).ReviewableId;

            var res = await base.Delete(id);

            int[] ratings = ctx.Reviews.Where(m => m.ReviewableId == reviewableId).Select(m => m.Rating).ToArray();
            double? AverageRating;
            if (ratings.Length != 0)
                AverageRating = ratings.Average();
            else
                AverageRating = null;
            ctx.Reviewable.Find(reviewableId).AverageRating = AverageRating;

            await ctx.SaveChangesAsync();

            return res;
        }

        protected override CReviewViewModel GenerateViewModel(Reviews entry)
        {
            string User = ctx.Appuser.Find(entry.UserId).UserName;

            return new CReviewViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.ReviewableId.ToString(),
                UserId = entry.UserId,
                ReviewableId = entry.ReviewableId,
                Rating = entry.Rating,
                Content = entry.Content,
                User = User,

            };
        }

        protected override Task UpdateEntry(Reviews entry, CReviewViewModel model)
        {
            entry.Rating = model.Rating;
            entry.Content = model.Content;

            return Task.CompletedTask;
        }
    }
}
