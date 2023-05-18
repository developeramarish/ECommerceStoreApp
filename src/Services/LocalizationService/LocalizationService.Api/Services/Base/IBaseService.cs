﻿using LocalizationService.Api.Models.Base.Abstract;
using LocalizationService.Api.Models.IncludeOptions;
using LocalizationService.Api.Utilities.Results;

namespace LocalizationService.Api.Services.Base
{
    public interface IBaseService<T,R,Y> where T : class, IModel
                                         where Y : IBaseIncludeOptions
    {
        /// <summary>
        /// Get <typeparamref name="T"/> by <typeparamref name="R"/>
        /// </summary>
        /// <param name="id"><typeparamref name="R"/></param>
        /// <returns><see cref="DataResult{T}<"/></returns>
        DataResult<T> Get(R id, Y options);
        /// <summary>
        /// Get list of <typeparamref name="T"/>
        /// </summary>
        /// <param name="id"><typeparamref name="R"/></param>
        /// <returns><see cref="List{DataResult{T}}"/></returns>
        DataResult<List<T>> GetAll(Y options);
    }
}
