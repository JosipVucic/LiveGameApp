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
    public class ReviewablesController : IntKeyController<ReviewableViewModel, Reviewable>
    {
        public ReviewablesController(LiveGameAppContext ctx, ILogger<ReviewablesController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Reviewable);
            orderSelectors = new Dictionary<string, Expression<Func<Reviewable, object>>>
            {
                [nameof(Reviewable.Id).ToLower()] = m => m.Id,
                [nameof(Reviewable.AverageRating).ToLower()] = m => m.AverageRating,
            };
        }

        protected override async Task<List<ReviewableViewModel>> GenerateList(IQueryable<Reviewable> query)
        {
            return await query.Select(m => new ReviewableViewModel
            {
                id = m.Id,
                AverageRating = m.AverageRating,

            }).ToListAsync();
        }

        protected override IQueryable<Reviewable> Filter(IQueryable<Reviewable> query, LoadParams loadParams)
        {
            return query.Where(m => m.AverageRating.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override Task<Reviewable> GenerateEntry(ReviewableViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Reviewable { AverageRating = model.AverageRating });
        }

        protected override ReviewableViewModel GenerateViewModel(Reviewable entry)
        {
            if (entry == null) return null;
            return new ReviewableViewModel
            {
                id = entry.Id,
                AverageRating = entry.AverageRating,

            };
        }

        protected override Task UpdateEntry(Reviewable entry, ReviewableViewModel model)
        {
            entry.AverageRating = model.AverageRating;

            return Task.CompletedTask;
        }
    }
}
