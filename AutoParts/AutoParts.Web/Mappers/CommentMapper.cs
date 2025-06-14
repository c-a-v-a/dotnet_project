namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.DTOs;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class CommentMapper
{
    public partial CommentModel ToViewModel(Comment comment);

    public partial Comment ToEntity(CommentModel model);

    public partial void ToEntity(CommentModel source, [MappingTarget] Comment target);

    public partial UserShortDto ToShortDto(User user);

    public User FromShortDto(UserShortDto shortDto) => null!;
}
