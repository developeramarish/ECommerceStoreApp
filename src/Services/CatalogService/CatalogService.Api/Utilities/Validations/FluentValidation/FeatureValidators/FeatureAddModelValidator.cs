﻿using CatalogService.Api.Models.FeatureModels;
using FluentValidation;

namespace CatalogService.Api.Utilities.Validations.FluentValidation.FeatureValidators
{
    public class FeatureAddModelValidator : AbstractValidator<FeatureAddModel>
    {
        public FeatureAddModelValidator()
        {
            
        }
    }
}
