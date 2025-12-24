using ContentModels.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContentModels.Api.Controllers;

[ApiController]
[Authorize]
[Route("standard-fields")]
public class StandardFieldsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetStandardFields()
    {
        var fields = StandardFields.Types.Select(type => new
        {
            Type = type.ToString(),
            Description = GetDescription(type)
        });

        return Ok(fields);
    }

    private static string GetDescription(FieldType type) =>
        type switch
        {
            FieldType.SingleLineText => "Short text value.",
            FieldType.MultiLineText => "Multi-line text or markdown.",
            FieldType.JsonRichText => "Rich text stored as JSON.",
            FieldType.DateTime => "Date or date-time value.",
            FieldType.File => "File or asset reference.",
            FieldType.Group => "Reusable grouping of fields.",
            FieldType.Link => "URL or hyperlink field.",
            FieldType.ModularBlock => "Composable block of nested fields.",
            FieldType.Taxonomy => "Categorization or tags.",
            FieldType.Boolean => "True/false toggle.",
            FieldType.Reference => "Link to another content model entry.",
            FieldType.Custom => "Custom implementation defined by client.",
            FieldType.GlobalField => "Shared global field reference.",
            _ => "Standard field."
        };
}
