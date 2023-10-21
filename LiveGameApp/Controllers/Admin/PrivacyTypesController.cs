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
    public class PrivacyTypesController : IntKeyController<PrivacyTypeViewModel, Privacytype>
    {

        public PrivacyTypesController(LiveGameAppContext ctx, ILogger<PrivacyTypesController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Privacytype);
            orderSelectors = new Dictionary<string, Expression<Func<Privacytype, object>>>
            {
                [nameof(Privacytype.Id).ToLower()] = m => m.Id,
                [nameof(Privacytype.Name).ToLower()] = m => m.Name,
            };
        }

        protected override async Task<List<PrivacyTypeViewModel>> GenerateList(IQueryable<Privacytype> query)
        {
            return await query.Select(m => new PrivacyTypeViewModel
            {
                id = m.Id,
                Name = m.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Privacytype> Filter(IQueryable<Privacytype> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Privacytype> GenerateEntry(PrivacyTypeViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Privacytype
            {
                Name = model.Name,
            });
        }

        protected override PrivacyTypeViewModel GenerateViewModel(Privacytype entry)
        {
            if (entry == null) return null;
            return new PrivacyTypeViewModel
            {
                id = entry.Id,
                Name = entry.Name,

            };
        }

        protected override Task UpdateEntry(Privacytype entry, PrivacyTypeViewModel model)
        {
            entry.Name = model.Name;

            return Task.CompletedTask;
        }
    }
}
