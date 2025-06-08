namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class UserMapper
{
    public partial UserModel ToViewModel(User user);

    [MapProperty(nameof(User.Id), nameof(UserModel.Id))]
    [MapProperty(nameof(User.FirstName), nameof(UserModel.FirstName))]
    [MapProperty(nameof(User.LastName), nameof(UserModel.LastName))]
    [MapProperty(nameof(User.Email), nameof(UserModel.Email))]
    [MapProperty(nameof(User.Role), nameof(UserModel.Role))]
    private partial User MapUser(UserModel user);
}
