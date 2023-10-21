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
using LiveGameApp.ViewModels;

namespace LiveGameApp.Controllers
{
    public abstract class IntKeyController<TModel, TDBModel> : MyController<int, TModel, TDBModel>
        where TDBModel : class, IIntKeyModel
        where TModel : IIntKeyViewModel
    {
        public IntKeyController(LiveGameAppContext ctx, ILogger<IntKeyController<TModel, TDBModel>> logger) : base(ctx, logger)
        {
        }

        protected override IQueryable<TDBModel> GetEntriesByIdList(IQueryable<TDBModel> query, LoadParams loadParams)
        {
            return query.Where(d => loadParams.IdList.Contains(d.Id));
        }

        protected override int GetId(TDBModel entry)
        {
            return entry.Id;
        }

        protected override int GetId(TModel model)
        {
            return model.id == null ? -1 : model.id.Value;
        }

        protected override async Task<TDBModel> GetEntry(IQueryable<TDBModel> query, int id)
        {
            return await query.Where(m => m.Id == id).FirstOrDefaultAsync();
        }

        protected override Task RemoveEntry(List<TDBModel> entries)
        {
            ctx.RemoveRange(entries);
            return Task.CompletedTask;
        }

        protected override Task RemoveEntry(TDBModel entry)
        {
            ctx.Remove(entry);
            return Task.CompletedTask;
        }

    }
}
