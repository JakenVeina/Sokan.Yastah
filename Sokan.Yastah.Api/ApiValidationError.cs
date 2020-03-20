using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using Sokan.Yastah.Common.OperationModel;

namespace Sokan.Yastah.Api
{
    public class ApiValidationError
        : OperationError
    {
        public static ApiValidationError FromModelState(
                ModelStateDictionary modelState)
            => new ApiValidationError(
                modelState
                    .Where(item => item.Value.Errors.Count > 0)
                    .ToDictionary(
                        item => item.Key.ToCamelCaseFromPascalCase(),
                        item => (IReadOnlyList<string>)item.Value.Errors
                            .Select(error => error.ErrorMessage)
                            .ToArray()));

        public ApiValidationError(
                IReadOnlyDictionary<string, IReadOnlyList<string>> errors)
            : base("The requested operation was invalid")
        {
            Errors = errors;
        }

        public IReadOnlyDictionary<string, IReadOnlyList<string>> Errors { get; }
    }
}
