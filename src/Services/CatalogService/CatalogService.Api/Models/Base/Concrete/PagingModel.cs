﻿using CatalogService.Api.Models.Base.Abstract;

namespace CatalogService.Api.Models.Base.Concrete
{
    public class PagingModel : IModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 8;
    }
}
