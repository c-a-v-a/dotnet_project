namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.DTOs;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class CustomerMapper
{
    public partial CustomerModel ToViewModel(Customer customer);

    public partial Customer ToEntity(CustomerModel model);

    public partial void ToEntity(CustomerModel source, [MappingTarget] Customer target);

    public partial VehicleShortDto ToShortDto(Vehicle vehicle);

    public List<Vehicle> MapVehicles(List<VehicleShortDto> shortDtos) => new();
}
