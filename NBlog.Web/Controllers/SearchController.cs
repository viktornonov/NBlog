using NBlog.Web.Application.Infrastructure;
using NBlog.Web.Application.Service;
using System;
using System.Linq;
using System.Web.Mvc;

namespace NBlog.Web.Controllers
{
    public partial class SearchController : LayoutController
    {
        public SearchController(IServices services) : base(services) { }

        [HttpGet]
        public ActionResult Index(string q)
        {
            var entries = Services.Entry.GetList();
            var results =
                entries
                    .Where(e => e.Title.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0)
                    .Select(e => new SearchResultModel { Slug = e.Slug, Title = e.Title, Date = e.DateCreated.ToDateString()});

            var model = new IndexModel { Results = results, QueryText = q };
            return View(model);
        }
    }
}
