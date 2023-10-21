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
using LiveGameApp.ViewModels.Filters;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace LiveGameApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class PlansController : IntKeyController<PlanViewModel, Plan>
    {
        public PlansController(LiveGameAppContext ctx, ILogger<PlansController> logger) : base(ctx, logger)
        {
            dbSetName = nameof(Plan);
            orderSelectors = new Dictionary<string, Expression<Func<Plan, object>>>
            {
                [nameof(Plan.Id).ToLower()] = m => m.Id,
                [nameof(Plan.Name).ToLower()] = m => m.Name,
                [nameof(Plan.Datetime).ToLower()] = m => m.Datetime,
                [nameof(Plan.Location).ToLower()] = m => m.Location,
                [nameof(Plan.MaxPlayers).ToLower()] = m => m.MaxPlayers,
                [nameof(Plan.MaxSpectators).ToLower()] = m => m.MaxSpectators,
                [nameof(Plan.HostUserId).ToLower()] = m => m.HostUserId,
                [nameof(Plan.TypeId).ToLower()] = m => m.TypeId,
                [nameof(Plan.PrivacyTypeId).ToLower()] = m => m.PrivacyTypeId,
            };
        }

        protected override Task<List<PlanViewModel>> GenerateList(IQueryable<Plan> query)
        {
            return query.Select(m => new PlanViewModel
            {
                id = m.Id,
                Name = m.Name,
                Datetime = m.Datetime,
                Location = m.Location,
                MaxPlayers = m.MaxPlayers,
                MaxSpectators = m.MaxSpectators,
                HostUserId = m.HostUserId,
                GameId = m.GameId,
                TypeId = m.TypeId,
                PrivacyTypeId = m.PrivacyTypeId,

            }).ToListAsync();
        }

        protected override IQueryable<Plan> Filter(IQueryable<Plan> query, LoadParams loadParams)
        {
            PlanFilter filter = JsonSerializer.Deserialize<PlanFilter>(loadParams.Filter);
            if (filter.HostUserId != 0)
                query = query.Where(m => m.HostUserId == filter.HostUserId);
            if (filter.q != null)
                query = query.Where(m => m.Name.Contains(filter.q));

            return query;
        }

        protected override async Task<Plan> GenerateEntry(PlanViewModel model)
        {
            if (model == null) return null;

            var generalEntry = new Reviewable { AverageRating = null };
            ctx.Add(generalEntry);
            await ctx.SaveChangesAsync();
            return new Plan
            {
                Id = generalEntry.Id,
                Name = model.Name,
                Datetime = model.Datetime.ToUniversalTime(),
                Location = model.Location,
                MaxPlayers = model.MaxPlayers,
                MaxSpectators = model.MaxSpectators,
                HostUserId = model.HostUserId,
                GameId = model.GameId,
                TypeId = model.TypeId,
                PrivacyTypeId = model.PrivacyTypeId,
            };
        }

        protected override async Task RemoveEntry(Plan entry)
        {
            Reviewable baseExisting = await ctx.Reviewable.Where(m => m.Id == entry.Id).FirstOrDefaultAsync();
            ctx.Remove(baseExisting);
        }

        protected override async Task RemoveEntry(List<Plan> entries)
        {
            List<Reviewable> baseExisting;
            List<int> keys = entries.ConvertAll(m => m.Id);

            baseExisting = await ctx.Reviewable.Where(m => keys.Contains(m.Id)).ToListAsync();

            ctx.RemoveRange(baseExisting);
        }

        protected override PlanViewModel GenerateViewModel(Plan entry)
        {
            if (entry == null) return null;
            return new PlanViewModel
            {
                id = entry.Id,
                Name = entry.Name,
                Datetime = entry.Datetime,
                Location = entry.Location,
                MaxPlayers = entry.MaxPlayers,
                MaxSpectators = entry.MaxSpectators,
                HostUserId = entry.HostUserId,
                GameId = entry.GameId,
                TypeId = entry.TypeId,
                PrivacyTypeId = entry.PrivacyTypeId,
            };
        }

        protected override Task UpdateEntry(Plan entry, PlanViewModel model)
        {
            entry.Name = model.Name;
            entry.Datetime = model.Datetime.ToUniversalTime();
            entry.Location = model.Location;
            entry.MaxPlayers = model.MaxPlayers;
            entry.MaxSpectators = model.MaxSpectators;
            entry.HostUserId = model.HostUserId;
            entry.GameId = model.GameId;
            entry.TypeId = model.TypeId;
            entry.PrivacyTypeId = model.PrivacyTypeId;

            return Task.CompletedTask;
        }
    }
}
