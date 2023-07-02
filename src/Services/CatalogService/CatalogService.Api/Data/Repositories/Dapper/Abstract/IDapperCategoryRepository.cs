﻿using CatalogService.Api.Models.Base.Concrete;
using CatalogService.Api.Data.Repositories.Base;
using CatalogService.Api.Entities;
using CatalogService.Api.Utilities.Results;

namespace CatalogService.Api.Data.Repositories.Dapper.Abstract;

public interface IDapperCategoryRepository : IGenericRepository<Category, IntModel>
{
    Task<DataResult<Category>> GetByName();
    Task<DataResult<IReadOnlyList<Category>>> GetAllByParentId();
}
