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
    public class ParticipationRequestsController : IntKeyController<ParticipationRequestViewModel, Participationrequest>
    {
        public ParticipationRequestsController(LiveGameAppContext ctx, ILogger<ParticipationRequestsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Participationrequest);
            orderSelectors = new Dictionary<string, Expression<Func<Participationrequest, object>>>
            {
                [nameof(Participationrequest.Id).ToLower()] = m => m.Id,
                [nameof(Participationrequest.Message).ToLower()] = m => m.Message,
                [nameof(Participationrequest.SenderId).ToLower()] = m => m.SenderId,
                [nameof(Participationrequest.TypeId).ToLower()] = m => m.TypeId,
                [nameof(Participationrequest.PlanId).ToLower()] = m => m.PlanId,
            };
        }

        protected override async Task<List<ParticipationRequestViewModel>> GenerateList(IQueryable<Participationrequest> query)
        {
            return await query.Select(m => new ParticipationRequestViewModel
            {
                id = m.Id,
                Message = m.Message,
                SenderId = m.SenderId,
                TypeId = m.TypeId,
                PlanId = m.PlanId,

            }).ToListAsync();
        }

        protected override IQueryable<Participationrequest> Filter(IQueryable<Participationrequest> query, LoadParams loadParams)
        {
            return query.Where(m => m.Message.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Participationrequest> GenerateEntry(ParticipationRequestViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Participationrequest
            {
                Message = model.Message,
                SenderId = model.SenderId,
                TypeId = model.TypeId,
                PlanId = model.PlanId,
            });
        }

        protected override ParticipationRequestViewModel GenerateViewModel(Participationrequest entry)
        {
            if (entry == null) return null;
            return new ParticipationRequestViewModel
            {
                id = entry.Id,
                Message = entry.Message,
                SenderId = entry.SenderId,
                TypeId = entry.TypeId,
                PlanId = entry.PlanId,

            };
        }

        protected override Task UpdateEntry(Participationrequest entry, ParticipationRequestViewModel model)
        {
            entry.Message = model.Message;
            entry.SenderId = model.SenderId;
            entry.TypeId = model.TypeId;
            entry.PlanId = model.PlanId;

            return Task.CompletedTask;
        }
    }
}
