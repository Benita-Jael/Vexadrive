using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace VexaDriveAPI.Swagger
{
    /// <summary>
    /// Operation filter to properly handle IFormFile parameters in Swagger documentation.
    /// This resolves the issue where Swashbuckle cannot automatically generate Operation definitions
    /// for endpoints with [FromForm] IFormFile parameters.
    /// </summary>
    public class FormFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var formFileParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile))
                .ToList();

            if (formFileParams.Any())
            {
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        {
                            "multipart/form-data",
                            new OpenApiMediaType
                            {
                                Schema = new OpenApiSchema
                                {
                                    Type = "object",
                                    Properties = new Dictionary<string, OpenApiSchema>
                                    {
                                        {
                                            "file",
                                            new OpenApiSchema
                                            {
                                                Type = "string",
                                                Format = "binary",
                                                Description = "File to upload"
                                            }
                                        }
                                    },
                                    Required = new HashSet<string> { "file" }
                                }
                            }
                        }
                    }
                };

                // Remove file parameter from path/query parameters if it was incorrectly added
                operation.Parameters = operation.Parameters
                    .Where(p => !formFileParams.Any(f => f.Name == p.Name))
                    .ToList();
            }
        }
    }
}
