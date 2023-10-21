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
using System.Security.Claims;

namespace LiveGameApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class CPlansController : IntKeyController<CPlanViewModel, Plan>
    {
        public CPlansController(LiveGameAppContext ctx, ILogger<CPlansController> logger) : base(ctx, logger)
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

        protected override IQueryable<Plan> SafetyFilter(IQueryable<Plan> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            int[] playPlanIds = ctx.Player.Where(m => m.UserId == userId).Select(m => m.PlanId).ToArray();
            int[] specPlanIds = ctx.Spectator.Where(m => m.UserId == userId).Select(m => m.PlanId).ToArray();

            query = query.Include(m => m.Game).Include(m => m.HostUser).Include(m => m.Type).Include(m => m.PrivacyType).Where(m => m.PrivacyType.Name.ToLower().Equals("public") || playPlanIds.Contains(m.Id) || specPlanIds.Contains(m.Id) || m.HostUserId == userId);

            return query;
        }

        protected override Task<List<CPlanViewModel>> GenerateList(IQueryable<Plan> query)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            return query.Select(m => new CPlanViewModel
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

                Host = m.HostUser.UserName,
                Game = m.Game.Name,
                Type = m.Type.Name,
                PrivacyType = m.PrivacyType.Name,
                IsHost = m.HostUserId == userId

            }).ToListAsync();
        }

        protected override IQueryable<Plan> Filter(IQueryable<Plan> query, LoadParams loadParams)
        {
            PlanFilter filter = JsonSerializer.Deserialize<PlanFilter>(loadParams.Filter);

            if (filter.q != null)
            {
                query = query.Include(m => m.Game).Include(m => m.HostUser).Where(m => m.Name.ToLower().Contains(filter.q.ToLower()) || m.Game.Name.ToLower().Contains(filter.q.ToLower()) || m.HostUser.UserName.ToLower().Contains(filter.q.ToLower()));
            }
            if (filter.PrivacyType != null)
            {
                query = query.Include(m => m.PrivacyType).Where(m => m.PrivacyType.Name.ToLower().Contains(filter.PrivacyType.ToLower()));
            }
            if (filter.Type != null)
            {
                query = query.Include(m => m.Type).Where(m => m.Type.Name.ToLower().Contains(filter.Type.ToLower()));
            }
            if (filter.StartTime.HasValue)
            {
                query = query.Where(m => m.Datetime.CompareTo(filter.StartTime.Value)>=0);
            }
            if (filter.EndTime.HasValue)
            {
                query = query.Where(m => m.Datetime.CompareTo(filter.EndTime.Value) <= 0);
            }
            if (filter.ShowMine)
            {
                int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

                int[] playPlanIds = ctx.Player.Where(m => m.UserId == userId).Select(m => m.PlanId).ToArray();
                int[] specPlanIds = ctx.Spectator.Where(m => m.UserId == userId).Select(m => m.PlanId).ToArray();

                query = query.Where(m => playPlanIds.Contains(m.Id) || specPlanIds.Contains(m.Id) || m.HostUserId == userId);
            }

            return query;
        }

        protected override async Task<Plan> GenerateEntry(CPlanViewModel model)
        {
            if (model == null) 
                return null;
            
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            
            if (userId != model.HostUserId) 
                return null;

            var generalEntry = new Reviewable { AverageRating = null };
            ctx.Add(generalEntry);
            await ctx.SaveChangesAsync();

            int typeId = model.TypeId;
            if (typeId == 0)
                if (model.Type != null)
                    typeId = (await ctx.Plantype.Where(m => m.Name.ToLower().Contains(model.Type.ToLower())).FirstOrDefaultAsync()).Id;
                else return null;

            int privacyTypeId = model.PrivacyTypeId;
            if (privacyTypeId == 0)
                if (model.PrivacyType != null)
                    privacyTypeId = (await ctx.Privacytype.Where(m => m.Name.ToLower().Contains(model.PrivacyType.ToLower())).FirstOrDefaultAsync()).Id;
                else return null;

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
                TypeId = typeId,
                PrivacyTypeId = privacyTypeId,
            };
        }

        protected override async Task RemoveEntry(Plan entry)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            if (userId != entry.HostUserId) return;

            Reviewable baseExisting = await ctx.Reviewable.Where(m => m.Id == entry.Id).FirstOrDefaultAsync();

            ctx.Remove(baseExisting);
        }

        protected override async Task RemoveEntry(List<Plan> entries)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            List<int> hosts = entries.ConvertAll(m => m.HostUserId);
            foreach (int host in hosts)
            {
                if (host != userId)
                    return;
            }

            List<Reviewable> baseExisting;
            List<int> keys = entries.ConvertAll(m => m.Id);

            baseExisting = await ctx.Reviewable.Where(m => keys.Contains(m.Id)).ToListAsync();

            ctx.RemoveRange(baseExisting);
        }

        protected override CPlanViewModel GenerateViewModel(Plan entry)
        {
            if (entry == null) return null;

            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);
            var PlayerEntries = ctx.Player.Where(m => m.PlanId == entry.Id);
            var SpectatorEntries = ctx.Spectator.Where(m => m.PlanId == entry.Id);
            int[] PlayerIds = PlayerEntries.Select(m => m.UserId).ToArray();
            int[] SpectatorIds = SpectatorEntries.Select(m => m.UserId).ToArray();

            if (ctx.Privacytype.Find(entry.PrivacyTypeId).Name.ToLower().Equals("private"))
            {
                if (userId != entry.HostUserId)
                {
                    if (!PlayerIds.Contains(userId) && !SpectatorIds.Contains(userId))
                        return null;
                }
            }

            string Host = ctx.Appuser.Find(entry.HostUserId).UserName;
            string Game = ctx.Game.Find(entry.GameId).Name;
            string Type = ctx.Plantype.Find(entry.TypeId).Name;

            string[] Players = PlayerEntries.Include(m => m.User).Select(m => m.User.UserName).ToArray();
            string[] Spectators = SpectatorEntries.Include(m => m.User).Select(m => m.User.UserName).ToArray();

            double? average = ctx.Reviewable.Find(entry.Id).AverageRating;
            string averageString = null;
            if (average.HasValue)
                averageString = String.Format("{0:0.00}", average);

            return new CPlanViewModel
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

                Host = Host,
                Game = Game,
                Type = Type,
                IsHost = userId == entry.HostUserId,
                IsPlayer = PlayerIds.Contains(userId),
                IsSpectator = SpectatorIds.Contains(userId),

                Players = Players,
                Spectators = Spectators,
                Rating = averageString,
            };
        }

        protected override async Task UpdateEntry(Plan entry, CPlanViewModel model)
        {
            int userId = int.Parse(User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value);

            var PlayerEntries = ctx.Player.Where(m => m.PlanId == entry.Id);
            var SpectatorEntries = ctx.Spectator.Where(m => m.PlanId == entry.Id);
            int[] PlayerIds = PlayerEntries.Select(m => m.UserId).ToArray();
            int[] SpectatorIds = SpectatorEntries.Select(m => m.UserId).ToArray();

            if (PlayerIds.Contains(userId) && !model.IsPlayer)
                ctx.Remove(await PlayerEntries.Where(m => m.UserId == userId).FirstOrDefaultAsync());
            if (SpectatorIds.Contains(userId) && !model.IsSpectator)
                ctx.Remove(await SpectatorEntries.Where(m => m.UserId == userId).FirstOrDefaultAsync());

            if (userId != entry.HostUserId || userId != model.HostUserId) return;

            var Players = PlayerEntries.Include(m => m.User).Select(m => m.User.UserName).ToList();
            var Spectators = SpectatorEntries.Include(m => m.User).Select(m => m.User.UserName).ToList();

            Players = Players.Intersect(model.Players).ToList();
            Spectators = Spectators.Intersect(model.Spectators).ToList();

            var kickedPlayers = PlayerEntries.Include(m => m.User).Where(m => !Players.Contains(m.User.UserName));
            var kickedSpectators = SpectatorEntries.Include(m => m.User).Where(m => !Spectators.Contains(m.User.UserName));

            ctx.RemoveRange(kickedPlayers);
            ctx.RemoveRange(kickedSpectators);

            entry.Name = model.Name;
            entry.Datetime = model.Datetime.ToUniversalTime();
            entry.Location = model.Location;
            entry.MaxPlayers = model.MaxPlayers;
            entry.MaxSpectators = model.MaxSpectators;
            entry.HostUserId = model.HostUserId;
            entry.GameId = model.GameId;
            entry.TypeId = model.TypeId;
            entry.PrivacyTypeId = model.PrivacyTypeId;

            return;
        }
    }
}
