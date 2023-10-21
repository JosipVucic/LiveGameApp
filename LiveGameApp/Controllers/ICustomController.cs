using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveGameApp.ViewModels;

namespace LiveGameApp.Controllers
{
    public interface ICustomController<TKey, TModel>
    {

        public Task<List<TModel>> GetAll([FromQuery] LoadParams loadParams);

        public Task<ActionResult<TModel>> GetOne(TKey id);

        public Task<IActionResult> Create(TModel model);

        public Task<IActionResult> Update(TKey id, TModel model);

        public Task<IActionResult> UpdateMany([FromQuery] LoadParams loadParams, TModel model);

        public Task<IActionResult> Delete(TKey id);

        public Task<IActionResult> DeleteMany([FromQuery] LoadParams loadParams);
    }
}
