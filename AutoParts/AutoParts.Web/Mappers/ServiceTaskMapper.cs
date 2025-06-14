namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.DTOs;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class ServiceTaskMapper
{
    public partial ServiceTaskModel ToViewModel(ServiceTask serviceTask);

    public partial ServiceTask ToEntity(ServiceTaskModel model);

    public partial void ToEntity(ServiceTaskModel source, [MappingTarget] ServiceTask target);

    public partial UserShortDto ToShortDto(User user);

    public User FromShortDto(UserShortDto shortDto) => null!;

    public partial UsedPartModel ToPartModel(UsedPart usedPart);

    public List<UsedPart> MapUsedParts(ICollection<UsedPartModel> usedParts) => new();
}
