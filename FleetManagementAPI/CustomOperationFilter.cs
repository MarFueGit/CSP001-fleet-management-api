using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FleetManagementAPI
{
    public class CustomOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the description attribute of the controller method
            var descriptionAttribute = (System.ComponentModel.DescriptionAttribute)context.ApiDescription.ActionDescriptor.EndpointMetadata
                .FirstOrDefault(em => em.GetType() == typeof(System.ComponentModel.DescriptionAttribute));

            // If description attribute exists, set it as the operation summary
            if (descriptionAttribute != null)
            {
                operation.Summary = descriptionAttribute.Description;
            }
        }
    }
}
