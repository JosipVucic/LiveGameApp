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
    public class RoomsController : IntKeyController<RoomViewModel, Room>
    {
        public RoomsController(LiveGameAppContext ctx, ILogger<RoomsController> logger): base(ctx, logger)
        {
            dbSetName = nameof(ctx.Room);
            orderSelectors = new Dictionary<string, Expression<Func<Room, object>>>
            {
                [nameof(Room.Id).ToLower()] = m => m.Id,
                [nameof(Room.Name).ToLower()] = m => m.Name,
            };
        }

        protected override async Task<List<RoomViewModel>> GenerateList(IQueryable<Room> query)
        {
            return await query.Select(m => new RoomViewModel
            {
                id = m.Id,
                Name = m.Name,

            }).ToListAsync();
        }

        protected override IQueryable<Room> Filter(IQueryable<Room> query, LoadParams loadParams)
        {
            return query.Where(m => m.Name.Contains(loadParams.SimpleFilter));
        }

        protected override Task<Room> GenerateEntry(RoomViewModel model)
        {
            if (model == null) return null;
            return Task.FromResult(new Room
            {
                Name = model.Name,
            });
        }

        protected override RoomViewModel GenerateViewModel(Room entry)
        {
            if (entry == null) return null;
            return new RoomViewModel
            {
                id = entry.Id,
                Name = entry.Name,

            };
        }

        protected override Task UpdateEntry(Room entry, RoomViewModel model)
        {
            entry.Name = model.Name;

            return Task.CompletedTask;
        }
    }
}
