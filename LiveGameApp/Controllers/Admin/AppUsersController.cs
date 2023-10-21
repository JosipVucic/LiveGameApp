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
using System.Security.Claims;

namespace LiveGameApp.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [ApiController]
    [Route("[controller]")]
    [TypeFilter(typeof(ProblemDetailsForSqlException))]
    public class AppUsersController : ControllerBase, ICustomController<int, AppUserViewModel>
    {
        private readonly LiveGameAppContext ctx;
        private readonly ILogger<AppUsersController> logger;
        private static readonly Dictionary<string, Expression<Func<Appuser, object>>> orderSelectors = new()
        {
            [nameof(Appuser.Id).ToLower()] = m => m.Id,
            [nameof(Appuser.UserName).ToLower()] = m => m.UserName,
            [nameof(Appuser.PasswordHash).ToLower()] = m => m.PasswordHash,
            [nameof(Appuser.Email).ToLower()] = m => m.Email,
            [nameof(Appuser.DateOfBirth).ToLower()] = m => m.DateOfBirth,
        };

        public AppUsersController(LiveGameAppContext ctx, ILogger<AppUsersController> logger)
        {
            this.ctx = ctx;
            this.logger = logger;
        }

        [HttpGet("count")]
        public async Task<int> Count([FromQuery] string filter)
        {
            var query = ctx.Appuser.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(m => m.UserName.Contains(filter));
            }
            int count = await query.CountAsync();
            return count;
        }

        [HttpGet]
        public async Task<List<AppUserViewModel>> GetAll([FromQuery] LoadParams loadParams)
        {
            var query = ctx.Appuser.AsQueryable();
            if (loadParams.Ids != null)
            {
                query = query.Where(d => loadParams.IdList.Contains(d.Id));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(loadParams.SimpleFilter))
                {
                    query = query.Where(m => m.UserName.Contains(loadParams.SimpleFilter));
                }

                if (loadParams.Field != null)
                {
                    if (orderSelectors.TryGetValue(loadParams.Field.ToLower(), out var expr))
                    {
                        query = loadParams.Descending ? query.OrderByDescending(expr) : query.OrderBy(expr);
                    }
                }
                query = query.Skip(loadParams.Start)
                             .Take(loadParams.PerPage);
            }
            var msg = User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value;
            
            foreach(var c in User.Claims)
            {
                //msg += (" " + c.ToString());
            }

            var list = await query.Select(m => new AppUserViewModel
            {
                id = m.Id,
                Username = m.UserName,
                Email = m.Email,
                Password = m.PasswordHash,
                DateOfBirth = m.DateOfBirth,

            }).ToListAsync();

            Response.Headers.Add("Content-Range", "entries " + loadParams.Start + "-" + loadParams.Start + list.Count + "/" + list.Count);

            return list;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppUserViewModel>> GetOne(int id)
        {
            var existing = await ctx.Appuser
                                  .Where(m => m.Id == id)
                                  .Select(m => new AppUserViewModel
                                  {
                                      id = m.Id,
                                      Username = m.UserName,
                                      Email = m.Email,
                                      Password = m.PasswordHash,
                                      DateOfBirth = m.DateOfBirth,
                                  })
                                  .FirstOrDefaultAsync();

            if (existing == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"No data for id = {id}");
            }
            else
            {
                return existing;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await ctx.Appuser.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (existing == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
            }
            else
            {
                ctx.Remove(existing);
                await ctx.SaveChangesAsync();

                return NoContent();
            };
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMany([FromQuery] LoadParams loadParams)
        {
            var existing = await ctx.Appuser.Where(d => loadParams.IdList.Contains(d.Id)).ToListAsync();
            if (existing == null)
            {
                return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid ids = {loadParams.IdList}");
            }
            else
            {
                ctx.RemoveRange(existing);
                await ctx.SaveChangesAsync();

                return Ok(loadParams.IdList);
            };
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, AppUserViewModel model)
        {
            if (model.id != id)
            {
                return Problem(statusCode: StatusCodes.Status400BadRequest, detail: $"Different ids {id} vs {model.id}");
            }
            else
            {
                var existing = await ctx.Appuser.Where(d => d.Id == id).FirstOrDefaultAsync();
                if (existing == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {id}");
                }

                existing.UserName = model.Username;
                existing.Email = model.Email;
                existing.PasswordHash = model.Password;
                existing.DateOfBirth = model.DateOfBirth.GetValueOrDefault();

                await ctx.SaveChangesAsync();

                var updatedItem = await GetOne(existing.Id);

                return Ok(updatedItem);
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMany([FromQuery] LoadParams loadParams, AppUserViewModel model)
        {
            var existings = await ctx.Appuser.Where(d => loadParams.IdList.Contains(d.Id)).ToListAsync();
            if (existings == null)
                {
                    return Problem(statusCode: StatusCodes.Status404NotFound, detail: $"Invalid id = {loadParams.IdList}");
                }
            foreach (var existing in existings)
            {
                existing.PasswordHash = model.Password;
                existing.DateOfBirth = model.DateOfBirth.GetValueOrDefault();
            }
                await ctx.SaveChangesAsync();

                return Ok(loadParams.IdList);    
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(AppUserViewModel model)
        {
                var entry = new Appuser { UserName = model.Username, Email = model.Email, PasswordHash = model.Password, DateOfBirth = model.DateOfBirth.GetValueOrDefault() };
                ctx.Add(entry);
                await ctx.SaveChangesAsync();

            var addedItem = await GetOne(entry.Id);

            return CreatedAtAction(nameof(GetOne), new { id = model.id }, addedItem.Value);
        }
    }
}
