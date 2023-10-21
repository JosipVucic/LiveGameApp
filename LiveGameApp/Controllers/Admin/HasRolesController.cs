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
    public class HasRolesController : StringKeyController<HasRoleViewModel, Hasrole>
    {

        public HasRolesController(LiveGameAppContext ctx, ILogger<HasRolesController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Hasrole);
            orderSelectors = new Dictionary<string, Expression<Func<Hasrole, object>>>
            {
                [nameof(Hasrole.UserId).ToLower()] = m => m.UserId,
                [nameof(Hasrole.RoleId).ToLower()] = m => m.RoleId,
                ["id"] = m => m.UserId.ToString() + "-" + m.RoleId.ToString(),
            };
        }

        protected override async Task<List<HasRoleViewModel>> GenerateList(IQueryable<Hasrole> query)
        {
            return await query.Select(m => new HasRoleViewModel
            {
                id = m.UserId.ToString() + "-" + m.RoleId.ToString(),
                UserId = m.UserId,
                RoleId = m.RoleId,

            }).ToListAsync();
        }

        protected override IQueryable<Hasrole> Filter(IQueryable<Hasrole> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Hasrole entry)
        {
            return entry.UserId.ToString() + "-" + entry.RoleId.ToString();
        }

        protected override async Task<Hasrole> GetEntry(IQueryable<Hasrole> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.RoleId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Hasrole> GetEntriesByIdList(IQueryable<Hasrole> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.RoleId.ToString()));
        }

        protected override Task<Hasrole> GenerateEntry(HasRoleViewModel model)
        {
            return Task.FromResult(new Hasrole { UserId = model.UserId, RoleId = model.RoleId });
        }

        protected override HasRoleViewModel GenerateViewModel(Hasrole entry)
        {
            return new HasRoleViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.RoleId.ToString(),
                UserId = entry.UserId,
                RoleId = entry.RoleId,

            };
        }

        protected override Task UpdateEntry(Hasrole entry, HasRoleViewModel model)
        {
            entry.UserId = model.UserId;
            entry.RoleId = model.RoleId;

            return Task.CompletedTask;
        }
    }
}
