﻿namespace LocalizationService.Api.Services.Base
{
    public interface IUpdateService<T,U> where T : class, IModel
                                         where U : class, IUpdateModel
    {
        /// <summary>
        /// Update <typeparamref name="U"/>
        /// </summary>
        /// <param name="model"><see cref="T"/></param>
        /// <param name="model"><see cref="U"/></param>
        /// <returns><typeparamref name="T"/></returns>
        DataResult<T> Update(U model);
    }
}
