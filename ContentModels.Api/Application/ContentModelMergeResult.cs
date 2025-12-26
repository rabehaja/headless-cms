using ContentModels.Domain;

namespace ContentModels.Api.Application;

public record ContentModelDiff(
    Guid ModelId,
    string Name,
    int SourceVersion,
    int? TargetVersion,
    bool IsConflict,
    string Reason);

public record ContentModelMergePreview(
    List<ContentModel> Adds,
    List<ContentModel> Updates,
    List<ContentModelDiff> Conflicts);
