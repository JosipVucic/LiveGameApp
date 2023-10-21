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
    public class InvitationsController : IntKeyController<InvitationViewModel, Invitation>
    {
        public InvitationsController(LiveGameAppContext ctx, ILogger<InvitationsController> logger): base(ctx, logger)
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

        protected override async Task<List<InvitationViewModel>> GenerateList(IQueryable<Invitation> query)
        {
            return await query.Select(m => new InvitationViewModel
            {
                id = m.Id,
                Message = m.Message,
                RecipientId = m.RecipientId,
                TypeId = m.TypeId,
                PlanId = m.PlanId,

            }).ToListAsync();
        }

        protected override IQueryable<Invitation> Filter(IQueryable<Invitation> query, LoadParams loadParams)
        {
            return query.Where(m => m.Message.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Invitation> GenerateEntry(InvitationViewModel model)
        {
            if (model == null) return null;

            return Task.FromResult(new Invitation
            {
                Message = model.Message,
                RecipientId = model.RecipientId,
                TypeId = model.TypeId,
                PlanId = model.PlanId,
            });
        }

        protected override InvitationViewModel GenerateViewModel(Invitation entry)
        {
            if (entry == null) return null;
            return new InvitationViewModel
            {
                id = entry.Id,
                Message = entry.Message,
                RecipientId = entry.RecipientId,
                TypeId = entry.TypeId,
                PlanId = entry.PlanId,

            };
        }

        protected override Task UpdateEntry(Invitation entry, InvitationViewModel model)
        {
            entry.Message = model.Message;
            entry.RecipientId = model.RecipientId;
            entry.TypeId = model.TypeId;
            entry.PlanId = model.PlanId;

            return Task.CompletedTask;
        }
    }
}
