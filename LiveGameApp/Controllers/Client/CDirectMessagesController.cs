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
using LiveGameApp.ViewModels.Filters;
using System.Text.Json;

namespace LiveGameApp.Controllers
{

    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class CDirectMessagesController : IntKeyController<DirectMessageViewModel, Directmessage>
    {
        public CDirectMessagesController(LiveGameAppContext ctx, ILogger<CDirectMessagesController> logger): base(ctx, logger)
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

        protected override IQueryable<Directmessage> SafetyFilter(IQueryable<Directmessage> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            query = query.Where(m => m.RecipientId == userId || m.SenderId == userId);

            return base.SafetyFilter(query);
        }

        protected override IQueryable<Directmessage> Filter(IQueryable<Directmessage> query, LoadParams loadParams)
        {
            DirectMessageFilter filter = JsonSerializer.Deserialize<DirectMessageFilter>(loadParams.Filter);

            query = query.Where(m => m.RecipientId == filter.RecipientId || m.SenderId == filter.SenderId);

            return query.Where(m => m.Content.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Directmessage> GenerateEntry(DirectMessageViewModel model)
        {
            if (model == null) return Task.FromResult<Directmessage>(null);

            DateTime now = DateTime.Now.ToUniversalTime();

            return Task.FromResult(new Directmessage { 
                Datetime = now, 
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
