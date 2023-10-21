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
using System.Security.Claims;
using LiveGameApp.ViewModels.Filters;
using System.Text.Json;

namespace LiveGameApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class CAppUsersController : IntKeyController<CAppUserViewModel, Appuser>
    {

        public CAppUsersController(LiveGameAppContext ctx, ILogger<CAppUsersController> logger) : base(ctx, logger)
        {
            dbSetName = nameof(ctx.Appuser);
            orderSelectors = new Dictionary<string, Expression<Func<Appuser, object>>>
            {
                [nameof(Appuser.Id).ToLower()] = m => m.Id,
                [nameof(Appuser.UserName).ToLower()] = m => m.UserName,
                [nameof(Appuser.PasswordHash).ToLower()] = m => m.PasswordHash,
                [nameof(Appuser.Email).ToLower()] = m => m.Email,
                [nameof(Appuser.DateOfBirth).ToLower()] = m => m.DateOfBirth,
            };
        }

        protected override async Task<List<CAppUserViewModel>> GenerateList(IQueryable<Appuser> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            var query1 = ctx.Friend.Include(m => m.UserHigh).Where(m => m.UserLowId == userId).Select(m => m.UserHigh);
            var query2 = ctx.Friend.Include(m => m.UserLow).Where(m => m.UserHighId == userId).Select(m => m.UserLow);

            var friends = await query1.Concat(query2).ToListAsync();

            return await query.Select(m => new CAppUserViewModel
            {
                id = m.Id,
                Username = m.UserName,
                IsFriend = friends.Contains(m),
            }).ToListAsync();
        }

        protected override IQueryable<Appuser> Filter(IQueryable<Appuser> query, LoadParams loadParams)
        {
            AppUserFilter filter = JsonSerializer.Deserialize<AppUserFilter>(loadParams.Filter);

            if (filter.FriendsOnly)
            {
                int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
                var query1 = ctx.Friend.Include(m => m.UserHigh).Where(m => m.UserLowId == userId).Select(m => m.UserHigh);
                var query2 = ctx.Friend.Include(m => m.UserLow).Where(m => m.UserHighId == userId).Select(m => m.UserLow);

                query = query1.Concat(query2);
            }
            if (!string.IsNullOrWhiteSpace(filter.q))
            {
                query = query.Where(m => m.UserName.ToLower().Contains(filter.q.ToLower()));
            }

            return query;
        }

        protected override Task<Appuser> GenerateEntry(CAppUserViewModel model)
        {
            return null;
            /*return Task.FromResult(new Appuser { 
                UserName = model.Username, 
                Email = model.Email, 
                PasswordHash = model.Password, 
                DateOfBirth = model.DateOfBirth });*/
        }

        protected override CAppUserViewModel GenerateViewModel(Appuser entry)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            DateTime? DOB = null;

            return new CAppUserViewModel
            {
                id = entry.Id,
                Username = entry.UserName,
                Email = userId == entry.Id ? entry.Email : null,
                DateOfBirth = userId == entry.Id ? entry.DateOfBirth : DOB,

            };
        }

        protected override Task UpdateEntry(Appuser entry, CAppUserViewModel model)
        {
            /*
            entry.UserName = model.Username;
            entry.Email = model.Email;
            entry.PasswordHash = model.Password;
            entry.DateOfBirth = model.DateOfBirth;
            */
            return Task.CompletedTask;
        }
    }
}
