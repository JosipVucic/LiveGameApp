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
    public class InviteRequestTypesController : IntKeyController<InviteRequestTypeViewModel, Inviterequesttype>
    {
        public InviteRequestTypesController(LiveGameAppContext ctx, ILogger<InviteRequestTypesController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Inviterequesttype);
            orderSelectors = new Dictionary<string, Expression<Func<Inviterequesttype, object>>>
            {
                [nameof(Inviterequesttype.Id).ToLower()] = m => m.Id,
                [nameof(Inviterequesttype.Name).ToLower()] = m => m.Name,
            };
        }

        protected override async Task<List<InviteRequestTypeViewModel>> GenerateList(IQueryable<Inviterequesttype> query)
        {
            return await query.Select(m => new InviteRequestTypeViewModel
            {
                id = m.Id,
                Name = m.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Inviterequesttype> Filter(IQueryable<Inviterequesttype> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Inviterequesttype> GenerateEntry(InviteRequestTypeViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Inviterequesttype
            {
                Name = model.Name,
            });
        }

        protected override InviteRequestTypeViewModel GenerateViewModel(Inviterequesttype entry)
        {
            if (entry == null) return null;
            return new InviteRequestTypeViewModel
            {
                id = entry.Id,
                Name = entry.Name,
            };
        }

        protected override Task UpdateEntry(Inviterequesttype entry, InviteRequestTypeViewModel model)
        {
            entry.Name = model.Name;

            return Task.CompletedTask;
        }
    }
}
