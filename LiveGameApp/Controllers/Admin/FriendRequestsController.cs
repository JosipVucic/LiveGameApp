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
    public class FriendRequestsController : IntKeyController<FriendRequestViewModel, Friendrequest>
    {
        public FriendRequestsController(LiveGameAppContext ctx, ILogger<FriendRequestsController> logger): base(ctx, logger)
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

        protected override async Task<List<FriendRequestViewModel>> GenerateList(IQueryable<Friendrequest> query)
        {
            return await query.Select(m => new FriendRequestViewModel
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

        protected override Task<Friendrequest> GenerateEntry(FriendRequestViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Friendrequest { Message = model.Message, SenderId = model.SenderId, RecipientId = model.RecipientId });
        }

        protected override FriendRequestViewModel GenerateViewModel(Friendrequest entry)
        {
            if (entry == null) return null;
            return new FriendRequestViewModel
            {
                id = entry.Id,
                Message = entry.Message,
                SenderId = entry.SenderId,
                RecipientId = entry.RecipientId,

            };
        }

        protected override Task UpdateEntry(Friendrequest entry, FriendRequestViewModel model)
        {
            entry.Message = model.Message;
            entry.SenderId = model.SenderId;
            entry.RecipientId = model.RecipientId;

            return Task.CompletedTask;
        }
    }
}
