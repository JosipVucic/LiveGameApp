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
    public class CInvitationsController : IntKeyController<CInvitationViewModel, Invitation>
    {
        public CInvitationsController(LiveGameAppContext ctx, ILogger<CInvitationsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Invitation);
            orderSelectors = new Dictionary<string, Expression<Func<Invitation, object>>>
            {
                [nameof(Invitation.Id).ToLower()] = m => m.Id,
                [nameof(Invitation.Message).ToLower()] = m => m.Message,
                [nameof(Invitation.RecipientId).ToLower()] = m => m.RecipientId,
                [nameof(Invitation.TypeId).ToLower()] = m => m.TypeId,
                [nameof(Invitation.PlanId).ToLower()] = m => m.PlanId,
            };
        }

        protected override IQueryable<Invitation> SafetyFilter(IQueryable<Invitation> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            query = query.Include(m => m.Type).Include(m => m.Plan).Where(m => m.RecipientId == userId);

            return query;
        }
        protected override async Task<List<CInvitationViewModel>> GenerateList(IQueryable<Invitation> query)
        {
            return await query.Select(m => new CInvitationViewModel
            {
                id = m.Id,
                Message = m.Message,
                RecipientId = m.RecipientId,
                TypeId = m.TypeId,
                PlanId = m.PlanId,

                Plan = m.Plan.Name,
                Type = m.Type.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Invitation> Filter(IQueryable<Invitation> query, LoadParams loadParams)
        {
            return query.Where(m => m.Message.Contains(loadParams.SimpleFilter));
        }

        protected override async Task<Invitation> GenerateEntry(CInvitationViewModel model)
        {
            if (model == null) return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            var hostId = ctx.Plan.Find(model.PlanId).HostUserId;
            if (hostId != userId)
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

            if (ctx.Invitation.Where(m => m.RecipientId == model.RecipientId && m.TypeId == typeId && m.PlanId == planId).Count() > 0) return null;

            return new Invitation
            {
                Message = model.Message,
                RecipientId = model.RecipientId,
                TypeId = typeId,
                PlanId = planId,
            };
        }

        protected override CInvitationViewModel GenerateViewModel(Invitation entry)
        {
            if (entry == null) return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            int hostId = ctx.Plan.Find(entry.PlanId).HostUserId;
            if (userId != entry.RecipientId && userId != hostId)
                return null;

            var typeEntry = ctx.Inviterequesttype.Find(entry.TypeId);
            var planEntry = ctx.Plan.Find(entry.PlanId);

            return new CInvitationViewModel
            {
                id = entry.Id,
                Message = entry.Message,
                RecipientId = entry.RecipientId,
                TypeId = entry.TypeId,
                PlanId = entry.PlanId,

                Type = typeEntry.Name,
                Plan = planEntry.Name,

            };
        }

        protected override Task UpdateEntry(Invitation entry, CInvitationViewModel model)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            if (userId != entry.RecipientId) return Task.CompletedTask;

            if (model.IsAccepted)
            {
                var playerEntry = ctx.Player.Find(entry.RecipientId, entry.PlanId);
                var spectatorEntry = ctx.Spectator.Find(entry.RecipientId, entry.PlanId);

                if (playerEntry == null && spectatorEntry == null)
                {
                    string type = ctx.Inviterequesttype.Find(entry.TypeId).Name;
                    if (type.ToLower().Equals("play"))
                    {
                        ctx.Add(new Player { UserId = entry.RecipientId, PlanId = entry.PlanId });
                    }
                    else if (type.ToLower().Equals("spectate"))
                    {
                        ctx.Add(new Spectator { UserId = entry.RecipientId, PlanId = entry.PlanId });
                    }
                }
            }

            ctx.Remove(entry);

            return Task.CompletedTask;
        }
    }
}
