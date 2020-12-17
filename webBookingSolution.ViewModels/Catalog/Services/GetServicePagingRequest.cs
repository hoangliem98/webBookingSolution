using System;
using System.Collections.Generic;
using System.Text;
using webBookingSolution.ViewModels.Common;

namespace webBookingSolution.ViewModels.Catalog.Services
{
    public class GetServicePagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
