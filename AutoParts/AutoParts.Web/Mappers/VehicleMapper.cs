namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.DTOs;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class VehicleMapper
{
    public partial VehicleModel ToViewModel(Vehicle vehicle);

    public partial Vehicle ToEntity(VehicleModel model);

    public partial void ToEntity(VehicleModel source, [MappingTarget] Vehicle target);

    public partial CustomerShortDto ToShortDto(Customer customer);

    public Customer FromShortDto(CustomerShortDto shortDto) => null!;
}
