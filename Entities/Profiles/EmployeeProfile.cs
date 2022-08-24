using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace Entities.Profiles;

public class EmployeeProfile:Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeForCreationDto, Employee>();
        CreateMap<EmployeeForUpdateDto, Employee>();
        CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();

    }
}