using NBlog.Web.Application.Infrastructure;
using System.Collections.Generic;

namespace NBlog.Web.Controllers
{
    public partial class SearchController
    {
        public class IndexModel : LayoutModel
        {
            public string QueryText { get; set; }
            public IEnumerable<SearchResultModel> Results { get; set; }
        }

        public class SearchResultModel
        {
            public string Slug { get; set; }
            public string Title { get; set; }
            public string Date { get; set; }
        }
    }
}