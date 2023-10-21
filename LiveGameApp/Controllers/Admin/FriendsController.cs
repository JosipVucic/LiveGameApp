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
    public class FriendsController : StringKeyController<FriendViewModel, Friend>
    {

        public FriendsController(LiveGameAppContext ctx, ILogger<FriendsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Friend);
            orderSelectors = new Dictionary<string, Expression<Func<Friend, object>>>
            {
                [nameof(Friend.UserLowId).ToLower()] = m => m.UserLowId,
                [nameof(Friend.UserHighId).ToLower()] = m => m.UserHighId,
                ["id"] = m => m.UserLowId.ToString() + "-" + m.UserHighId.ToString(),
            };
        }

        protected override async Task<List<FriendViewModel>> GenerateList(IQueryable<Friend> query)
        {
            return await query.Select(m => new FriendViewModel
            {
                id = m.UserLowId.ToString() + "-" + m.UserHighId.ToString(),
                UserLowId = m.UserLowId,
                UserHighId = m.UserHighId,

            }).ToListAsync();
        }

        protected override IQueryable<Friend> Filter(IQueryable<Friend> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserLowId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Friend entry)
        {
            return entry.UserLowId.ToString() + "-" + entry.UserHighId.ToString();
        }

        protected override async Task<Friend> GetEntry(IQueryable<Friend> query, string id)
        {
            return await query.Where(m => (m.UserLowId.ToString() + "-" + m.UserHighId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Friend> GetEntriesByIdList(IQueryable<Friend> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserLowId.ToString() + "-" + m.UserHighId.ToString()));
        }

        protected override Task<Friend> GenerateEntry(FriendViewModel model)
        {
            return Task.FromResult(new Friend { UserLowId = model.UserLowId, UserHighId = model.UserHighId });
        }

        protected override FriendViewModel GenerateViewModel(Friend entry)
        {
            return new FriendViewModel
            {
                id = entry.UserLowId.ToString() + "-" + entry.UserHighId.ToString(),
                UserLowId = entry.UserLowId,
                UserHighId = entry.UserHighId,

            };
        }

        protected override Task UpdateEntry(Friend entry, FriendViewModel model)
        {
            entry.UserLowId = model.UserLowId;
            entry.UserHighId = model.UserHighId;

            return Task.CompletedTask;
        }
    }
}
