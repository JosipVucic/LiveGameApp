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
    public class CParticipationRequestsController : IntKeyController<CParticipationRequestViewModel, Participationrequest>
    {
        public CParticipationRequestsController(LiveGameAppContext ctx, ILogger<CParticipationRequestsController> logger) : base(ctx, logger)
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

        protected override IQueryable<Participationrequest> SafetyFilter(IQueryable<Participationrequest> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            query = query.Include(m => m.Type).Include(m => m.Plan).Where(m => m.Plan.HostUserId == userId);

            return query;
        }

        protected override async Task<List<CParticipationRequestViewModel>> GenerateList(IQueryable<Participationrequest> query)
        {
            
            return await query.Select(m => new CParticipationRequestViewModel
            {
                id = m.Id,
                Message = m.Message,
                SenderId = m.SenderId,
                TypeId = m.TypeId,
                PlanId = m.PlanId,

                Plan = m.Plan.Name,
                Type = m.Type.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Participationrequest> Filter(IQueryable<Participationrequest> query, LoadParams loadParams)
        {
            ParticipationRequestFilter filter = JsonSerializer.Deserialize<ParticipationRequestFilter>(loadParams.Filter);
            if (filter.PlanId != 0)
                query = query.Where(m => m.PlanId == filter.PlanId);

            return query.Where(m => m.Message.Contains(loadParams.SimpleFilter));
        }

        protected override async Task<Participationrequest> GenerateEntry(CParticipationRequestViewModel model)
        {
            if (model == null)
                return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            if (model.SenderId != userId)
                return null;

            int typeId = model.TypeId;
            if (typeId == 0)
                if (model.Type != null)
                    typeId = (await ctx.Inviterequesttype.Where(m => m.Name.ToLower().Contains(model.Type.ToLower())).FirstOrDefaultAsync()).Id;
                else return null;

            int planId = model.PlanId;
            if (planId == 0)
                if (model.Plan != null)
                    planId = (await ctx.Plan.Where(m => m.Name.ToLower().Contains(model.Plan.ToLower())).FirstOrDefaultAsync()).Id;
                else return null;

            if (ctx.Participationrequest.Where(m => m.SenderId == model.SenderId && m.TypeId == typeId && m.PlanId == planId).Count() > 0) return null;

            return new Participationrequest
            {
                Message = model.Message,
                SenderId = model.SenderId,
                TypeId = typeId,
                PlanId = planId,
            };
        }

        protected override CParticipationRequestViewModel GenerateViewModel(Participationrequest entry)
        {
            if (entry == null)
                return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            int hostId = ctx.Plan.Find(entry.PlanId).HostUserId;
            if (userId != entry.SenderId && userId != hostId)
                return null;

            var typeEntry = ctx.Inviterequesttype.Find(entry.TypeId);
            var planEntry = ctx.Plan.Find(entry.PlanId);

            return new CParticipationRequestViewModel
            {
                id = entry.Id,
                Message = entry.Message,
                SenderId = entry.SenderId,
                TypeId = entry.TypeId,
                PlanId = entry.PlanId,

                Type = typeEntry.Name,
                Plan = planEntry.Name,

            };
        }

        protected override Task UpdateEntry(Participationrequest entry, CParticipationRequestViewModel model)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            int hostId = ctx.Plan.Find(entry.PlanId).HostUserId;

            if (userId != hostId) return Task.CompletedTask;

            if (model.IsAccepted)
            {
                var playerEntry = ctx.Player.Find(entry.SenderId, entry.PlanId);
                var spectatorEntry = ctx.Spectator.Find(entry.SenderId, entry.PlanId);

                if (playerEntry == null && spectatorEntry == null)
                {
                    string type = ctx.Inviterequesttype.Find(entry.TypeId).Name;
                    if (type.ToLower().Equals("play"))
                    {
                        ctx.Add(new Player { UserId = entry.SenderId, PlanId = entry.PlanId });
                    }
                    else if (type.ToLower().Equals("spectate"))
                    {
                        ctx.Add(new Spectator { UserId = entry.SenderId, PlanId = entry.PlanId });
                    }
                }
            }

            ctx.Remove(entry);

            return Task.CompletedTask;
        }
    }
}

