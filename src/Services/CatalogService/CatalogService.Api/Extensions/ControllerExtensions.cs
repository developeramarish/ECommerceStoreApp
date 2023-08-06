﻿using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using CatalogService.Api.Attributes;
using CatalogService.Api.Infrastructure.Filters;
using CatalogService.Api.Utilities.Validations.FluentValidation.Base;
using CatalogService.Api.Utilities.Validations.FluentValidation.BrandValidators;
using CatalogService.Api.Utilities.Validations.FluentValidation.CategoryValidators;
using CatalogService.Api.Utilities.Validations.FluentValidation.CommentValidators;
using CatalogService.Api.Utilities.Validations.FluentValidation.FeatureValidators;
using CatalogService.Api.Utilities.Validations.FluentValidation.ProductValidators;

namespace CatalogService.Api.Extensions
{
    public static class ControllerExtensions
    {
        public static void AddControllerSettings(this IServiceCollection services)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(FluentValidationCustomValidationAttribute));
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            }).AddJsonOptions(o =>
            {
                o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //VALIDATORS
            #region Base
            services.AddValidatorsFromAssemblyContaining<StringModelValidator>();
            services.AddValidatorsFromAssemblyContaining<IntModelValidator>();
            services.AddValidatorsFromAssemblyContaining<BoolModelValidator>();
            #endregion
            #region Brand
            services.AddValidatorsFromAssemblyContaining<BrandAddModelValidator>();
            services.AddValidatorsFromAssemblyContaining<BrandUpdateModelValidator>();
            #endregion
            #region Category
            services.AddValidatorsFromAssemblyContaining<CategoryAddModelValidator>();
            services.AddValidatorsFromAssemblyContaining<CategoryUpdateModelValidator>();
            #endregion
            #region Comment
            services.AddValidatorsFromAssemblyContaining<CommentAddModelValidator>();
            services.AddValidatorsFromAssemblyContaining<CommentUpdateModelValidator>();
            #endregion
            #region Feature
            services.AddValidatorsFromAssemblyContaining<FeatureAddModelValidator>();
            services.AddValidatorsFromAssemblyContaining<FeatureUpdateModelValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductFeatureModelValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductFeaturePropertyAddModelValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductFeaturePropertyUpdateModelValidator>();
            #endregion
            #region Product
            services.AddValidatorsFromAssemblyContaining<PriceBetweenModelValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductAddModelValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductUpdateModelValidator>();
            #endregion

            services.AddFluentValidationAutoValidation();
            //services.AddFluentValidationClientsideAdapters(); // for client side
        }
    }
}
