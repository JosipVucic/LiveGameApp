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
    public class DirectMessagesController : IntKeyController<DirectMessageViewModel, Directmessage>
    {
        public DirectMessagesController(LiveGameAppContext ctx, ILogger<DirectMessagesController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Directmessage);
            orderSelectors = new Dictionary<string, Expression<Func<Directmessage, object>>>
            {
                [nameof(Directmessage.Id).ToLower()] = m => m.Id,
                [nameof(Directmessage.SenderId).ToLower()] = m => m.SenderId,
                [nameof(Directmessage.RecipientId).ToLower()] = m => m.RecipientId,
                [nameof(Directmessage.Content).ToLower()] = m => m.Content,
                [nameof(Directmessage.Datetime).ToLower()] = m => m.Datetime,
            };
        }

        protected override async Task<List<DirectMessageViewModel>> GenerateList(IQueryable<Directmessage> query)
        {
            return await query.Select(m => new DirectMessageViewModel
            {
                id = m.Id,
                Datetime = m.Datetime,
                Content = m.Content,
                SenderId = m.SenderId,
                RecipientId = m.RecipientId,

            }).ToListAsync();
        }

        protected override IQueryable<Directmessage> Filter(IQueryable<Directmessage> query, LoadParams loadParams)
        {
            return query.Where(m => m.Content.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Directmessage> GenerateEntry(DirectMessageViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Directmessage { 
                Datetime = model.Datetime.ToUniversalTime(), 
                Content = model.Content, 
                RecipientId = model.RecipientId, 
                SenderId = model.SenderId });
        }

        protected override Task UpdateEntry(Directmessage entry, DirectMessageViewModel model)
        {
            entry.Content = model.Content;
            entry.Datetime = model.Datetime.ToUniversalTime();
            entry.RecipientId = model.RecipientId;
            entry.SenderId = model.SenderId;

            return Task.CompletedTask;
        }

        protected override DirectMessageViewModel GenerateViewModel(Directmessage entry)
        {
            if (entry == null) return null;
            return new DirectMessageViewModel
            {
                id = entry.Id,
                Datetime = entry.Datetime,
                Content = entry.Content,
                SenderId = entry.SenderId,
                RecipientId = entry.RecipientId,

            };
        }
    }
}
