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

namespace LiveGameApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class CFriendRequestsController : IntKeyController<CFriendRequestViewModel, Friendrequest>
    {
        public CFriendRequestsController(LiveGameAppContext ctx, ILogger<CFriendRequestsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Friendrequest);
            orderSelectors = new Dictionary<string, Expression<Func<Friendrequest, object>>>
            {
                [nameof(Friendrequest.Id).ToLower()] = m => m.Id,
                [nameof(Friendrequest.SenderId).ToLower()] = m => m.SenderId,
                [nameof(Friendrequest.RecipientId).ToLower()] = m => m.RecipientId,
                [nameof(Friendrequest.Message).ToLower()] = m => m.Message,
            };
        }

        protected override IQueryable<Friendrequest> SafetyFilter(IQueryable<Friendrequest> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            query = query.Where(m => m.RecipientId == userId);

            return query;
        }

        protected override async Task<List<CFriendRequestViewModel>> GenerateList(IQueryable<Friendrequest> query)
        {
            return await query.Select(m => new CFriendRequestViewModel
            {
                id = m.Id,
                Message = m.Message,
                SenderId = m.SenderId,
                RecipientId = m.RecipientId,

            }).ToListAsync();
        }

        protected override IQueryable<Friendrequest> Filter(IQueryable<Friendrequest> query, LoadParams loadParams)
        {
            return query.Where(m => m.Message.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Friendrequest> GenerateEntry(CFriendRequestViewModel model)
        {
            if (model == null) return Task.FromResult<Friendrequest>(null);

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            if (model.SenderId != userId) return Task.FromResult<Friendrequest>(null);

            if (ctx.Friendrequest.Where(m => m.SenderId == model.SenderId && m.RecipientId == model.RecipientId).Count() > 0) return Task.FromResult<Friendrequest>(null);

            return Task.FromResult(new Friendrequest 
            { 
                Message = model.Message,
                SenderId = model.SenderId,
                RecipientId = model.RecipientId 
            });
        }

        protected override CFriendRequestViewModel GenerateViewModel(Friendrequest entry)
        {
            if (entry == null) return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            if (userId != entry.RecipientId && userId != entry.SenderId) return null;

            return new CFriendRequestViewModel
            {
                id = entry.Id,
                Message = entry.Message,
                SenderId = entry.SenderId,
                RecipientId = entry.RecipientId,

            };
        }

        protected override Task UpdateEntry(Friendrequest entry, CFriendRequestViewModel model)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            if (userId != entry.RecipientId) return Task.CompletedTask;

            if (model.IsAccepted)
            {
                int id1 = entry.SenderId;
                int id2 = entry.RecipientId;
                if(id1 > id2)
                {
                    int tmp = id1;
                    id1 = id2;
                    id2 = tmp;
                }

                var friendEntry = ctx.Friend.Find(id1, id2);

                if (friendEntry == null)
                {
                        ctx.Add(new Friend { UserLowId = id1, UserHighId = id2 });                    
                }
            }

            ctx.Remove(entry);

            return Task.CompletedTask;
        }
    }
}
