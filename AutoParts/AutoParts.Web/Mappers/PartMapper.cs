namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class PartMapper
{
    public partial PartModel ToViewModel(Part part);

    public partial Part ToEntity(PartModel model);

    public partial void ToEntity(PartModel source, [MappingTarget] Part target);
}
