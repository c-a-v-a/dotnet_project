namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class UsedPartMapper
{
    public partial UsedPartModel ToViewModel(UsedPart part);

    public partial UsedPart ToEntity(UsedPartModel model);

    public partial void ToEntity(UsedPartModel source, [MappingTarget] UsedPart target);

    public partial PartModel ToPartModel(Part part);

    public Part FromPartModel(PartModel model) => null!;
}
