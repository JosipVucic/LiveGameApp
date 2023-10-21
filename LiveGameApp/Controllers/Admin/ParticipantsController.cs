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
    public class ParticipantsController : StringKeyController<ParticipantViewModel, Participant>
    {
        public ParticipantsController(LiveGameAppContext ctx, ILogger<ParticipantsController> logger):base(ctx, logger)
        {
            dbSetName = nameof(ctx.Participant);
            orderSelectors = new Dictionary<string, Expression<Func<Participant, object>>>
            {
                [nameof(Participant.UserId).ToLower()] = m => m.UserId,
                [nameof(Participant.RoomId).ToLower()] = m => m.RoomId,
                ["id"] = m => m.UserId.ToString() + "-" + m.RoomId.ToString(),
            };
        }

        protected override async Task<List<ParticipantViewModel>> GenerateList(IQueryable<Participant> query)
        {
            return await query.Select(m => new ParticipantViewModel
            {
                id = m.UserId.ToString() + "-" + m.RoomId.ToString(),
                UserId = m.UserId,
                RoomId = m.RoomId,

            }).ToListAsync();
        }

        protected override IQueryable<Participant> Filter(IQueryable<Participant> query, LoadParams loadParams)
        {
            return query.Where(m => m.UserId.ToString().Contains(loadParams.SimpleFilter));
        }

        protected override string GetId(Participant entry)
        {
            return entry.UserId.ToString() + "-" + entry.RoomId.ToString();
        }

        protected override async Task<Participant> GetEntry(IQueryable<Participant> query, string id)
        {
            return await query.Where(m => (m.UserId.ToString() + "-" + m.RoomId.ToString()).Equals(id)).FirstOrDefaultAsync();
        }

        protected override IQueryable<Participant> GetEntriesByIdList(IQueryable<Participant> query, LoadParams loadParams)
        {
            return query.Where(m => loadParams.StringIdList.Contains(m.UserId.ToString() + "-" + m.RoomId.ToString()));
        }

        protected override Task<Participant> GenerateEntry(ParticipantViewModel model)
        {
            return Task.FromResult(new Participant { UserId = model.UserId, RoomId = model.RoomId });
        }

        protected override ParticipantViewModel GenerateViewModel(Participant entry)
        {
            return new ParticipantViewModel
            {
                id = entry.UserId.ToString() + "-" + entry.RoomId.ToString(),
                UserId = entry.UserId,
                RoomId = entry.RoomId,

            };
        }

        protected override Task UpdateEntry(Participant entry, ParticipantViewModel model)
        {
            entry.UserId = model.UserId;
            entry.RoomId = model.RoomId;

            return Task.CompletedTask;
        }
    }
}
