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
    public class RolesController : IntKeyController<RoleViewModel, Role>
    {
        public RolesController(LiveGameAppContext ctx, ILogger<RolesController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Role);
            orderSelectors = new Dictionary<string, Expression<Func<Role, object>>>
            {
                [nameof(Role.Id).ToLower()] = m => m.Id,
                [nameof(Role.Name).ToLower()] = m => m.Name,
            };
        }

        protected override async Task<List<RoleViewModel>> GenerateList(IQueryable<Role> query)
        {
            return await query.Select(m => new RoleViewModel
            {
                id = m.Id,
                Name = m.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Role> Filter(IQueryable<Role> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Role> GenerateEntry(RoleViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Role
            {
                Name = model.Name,
            });
        }

        protected override RoleViewModel GenerateViewModel(Role entry)
        {
            if (entry == null) return null;
            return new RoleViewModel
            {
                id = entry.Id,
                Name = entry.Name,

            };
        }

        protected override Task UpdateEntry(Role entry, RoleViewModel model)
        {
            entry.Name = model.Name;

            return Task.CompletedTask;
        }
    }
}
