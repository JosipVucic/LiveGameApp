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
    public abstract class StringKeyController<TModel, TDBModel> : MyController<string, TModel, TDBModel>
        where TDBModel : class
        where TModel : IStringKeyViewModel
    {
        public StringKeyController(LiveGameAppContext ctx, ILogger<StringKeyController<TModel, TDBModel>> logger) : base(ctx, logger)
        {
        }

        protected override string GetId(TModel model)
        {
            return model.id;
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
