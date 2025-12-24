namespace ContentModels.Domain;

public static class StandardFields
{
    public static readonly IReadOnlyCollection<FieldType> Types = new[]
    {
        FieldType.SingleLineText,
        FieldType.MultiLineText,
        FieldType.JsonRichText,
        FieldType.DateTime,
        FieldType.File,
        FieldType.Group,
        FieldType.Link,
        FieldType.ModularBlock,
        FieldType.Taxonomy,
        FieldType.Boolean,
        FieldType.Reference,
        FieldType.Custom,
        FieldType.GlobalField
    };
}
