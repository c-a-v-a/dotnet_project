namespace AutoParts.Web.Mappers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.DTOs;
using AutoParts.Web.Models;
using Riok.Mapperly.Abstractions;

[Mapper]
public partial class ServiceOrderMapper
{
    public partial ServiceOrderModel ToViewModel(ServiceOrder serviceOrder);

    public partial ServiceOrder ToEntity(ServiceOrderModel model);

    public partial void ToEntity(ServiceOrderModel source, [MappingTarget] ServiceOrder target);

    public partial VehicleShortDto ToShortDto(Vehicle vehicle);

    public Vehicle FromShortDto(VehicleShortDto shortDto) => null!;

    public partial CustomerShortDto ToShortDto(Customer customer);

    public Customer FromShortDto(CustomerShortDto shortDto) => null!;

    public partial UserShortDto ToShortDto(User vehicle);

    public User FromShortDto(UserShortDto shortDto) => null!;

    public List<Vehicle> MapVehicles(List<VehicleShortDto> shortDtos) => new();
}
