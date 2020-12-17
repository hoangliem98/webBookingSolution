using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.ViewModels.Catalog.Halls
{
    public class GetHallPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
