using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.ViewModels.Catalog.Menus
{
    public class GetMenuPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
