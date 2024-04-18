﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebShop_NULL.Models.ViewModels.AdminPanelModels;

namespace WebShop_NULL.Infrastructure.AdminPanel
{
    public class CreateCategoryPropertyInfoBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if(bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var modelName = bindingContext.ModelName;
            var propertyNameResult = bindingContext.ValueProvider.GetValue($"{modelName}.Name");
            var propertyTypeResult = bindingContext.ValueProvider.GetValue($"{modelName}.Type");
            if (propertyNameResult == ValueProviderResult.None ||
                propertyTypeResult == ValueProviderResult.None ||
                !int.TryParse(propertyTypeResult.FirstValue, out var propertyType) ||
                string.IsNullOrWhiteSpace(propertyNameResult.FirstValue) ||
                propertyType < 0 || propertyType > 2)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                bindingContext.ModelState.AddModelError("propertyError", $"Название свойства не задано.");
                return Task.CompletedTask;
            }

            if (propertyType == 1)
            {
                var propertyMinValueResult = bindingContext.ValueProvider.GetValue($"{modelName}.MinValue");
                if (propertyMinValueResult == ValueProviderResult.None ||
                    !double.TryParse(propertyMinValueResult.FirstValue, out var propertyMinValue))
                    propertyMinValue = long.MinValue;
               
                var propertyMaxValueResult = bindingContext.ValueProvider.GetValue($"{modelName}.MaxValue");
                if (propertyMaxValueResult == ValueProviderResult.None ||
                    !double.TryParse(propertyMaxValueResult.FirstValue, out var propertyMaxValue))
                    propertyMaxValue = long.MaxValue;
                    
                bindingContext.Result = ModelBindingResult.Success(new CreateCategoryNumberPropertyInfo()
                {
                    Name = propertyNameResult.FirstValue,
                    Type = propertyType,
                    MinValue = propertyMinValue,
                    MaxValue = propertyMaxValue
                });
            
                return Task.CompletedTask;
            }

            if (propertyType == 2)
            {
                var propertyOptionsResult = bindingContext.ValueProvider.GetValue($"{modelName}.Options");
                if (propertyOptionsResult == ValueProviderResult.None)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    bindingContext.ModelState.AddModelError("optionError", "Свойство типа 'Опция' должно иметь хотя бы одну опцию");
                    return Task.CompletedTask;
                }
                    
                var options = propertyOptionsResult.Values.Where(str => !string.IsNullOrWhiteSpace(str)).ToList();
                bindingContext.Result = ModelBindingResult.Success(new CreateCategoryOptionPropertyInfo()
                {
                    Name = propertyNameResult.FirstValue,
                    Type = propertyType,
                    Options = options
                });
            
                return Task.CompletedTask;
            }
            
            bindingContext.Result = ModelBindingResult.Success(new CreateCategoryPropertyInfo()
            {
                Name = propertyNameResult.FirstValue,
                Type = propertyType
            });
            
            return Task.CompletedTask;
        }
    }
}