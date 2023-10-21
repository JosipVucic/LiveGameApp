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
    public class MessagesController : IntKeyController<MessageViewModel, Message>
    {
        public MessagesController(LiveGameAppContext ctx, ILogger<MessagesController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Message);
            orderSelectors = new Dictionary<string, Expression<Func<Message, object>>>
            {
                [nameof(Message.Id).ToLower()] = m => m.Id,
                [nameof(Message.UserId).ToLower()] = m => m.UserId,
                [nameof(Message.RoomId).ToLower()] = m => m.RoomId,
                [nameof(Message.Content).ToLower()] = m => m.Content,
                [nameof(Message.Datetime).ToLower()] = m => m.Datetime,
            };
        }

        protected override async Task<List<MessageViewModel>> GenerateList(IQueryable<Message> query)
        {
            return await query.Select(m => new MessageViewModel
            {
                id = m.Id,
                Datetime = m.Datetime,
                Content = m.Content,
                UserId = m.UserId,
                RoomId = m.RoomId,

            }).ToListAsync();
        }

        protected override IQueryable<Message> Filter(IQueryable<Message> query, LoadParams loadParams)
        {
            return query.Where(m => m.Content.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Message> GenerateEntry(MessageViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Message { 
                Datetime = model.Datetime.ToUniversalTime(), 
                Content = model.Content, 
                RoomId = model.RoomId, 
                UserId = model.UserId });
        }

        protected override MessageViewModel GenerateViewModel(Message entry)
        {
            if (entry == null) return null;
            return new MessageViewModel
            {
                id = entry.Id,
                Datetime = entry.Datetime,
                Content = entry.Content,
                UserId = entry.UserId,
                RoomId = entry.RoomId,

            };
        }

        protected override Task UpdateEntry(Message entry, MessageViewModel model)
        {
            entry.Content = model.Content;
            entry.Datetime = model.Datetime.ToUniversalTime();
            entry.RoomId = model.RoomId;
            entry.UserId = model.UserId;

            return Task.CompletedTask;
        }
    }
}
