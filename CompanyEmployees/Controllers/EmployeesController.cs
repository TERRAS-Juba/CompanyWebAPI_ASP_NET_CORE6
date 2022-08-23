using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers;

[ApiController]
[Route("api/companies/{companyId}/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger<EmployeesController> _logger;
    private readonly IMapper _mapper;

    public EmployeesController(IRepositoryManager repository, ILogger<EmployeesController> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllEmployeesByCompanyId(Guid companyId)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var employees = _repository.Employee.GetAllEmployees(companyId, trackChanges: false);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        return Ok(employeesDto);
    }

    [HttpGet("{employeeId:guid}",Name="GetEmployeeById")]
    public IActionResult GetEmployeeById(Guid companyId,Guid employeeId)
    {
        var company = _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }
        var employee = _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: false);
        if (employee == null)
        {
            _logger.LogInformation($"Employee with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var employeeDto = _mapper.Map<EmployeeDto>(employee);
        return Ok(employeeDto);
    }

    [HttpPost]
    public IActionResult CreateEmployee(Guid companyId, EmployeeForCreationDto employeeForCreationDto)
    {
        if (employeeForCreationDto == null)
        {
            _logger.LogError("EmployeeForCreationDto object sent from client is null.");
            return BadRequest("EmployeeForCreationDto object is null");

        }
        var company = _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var employee = _mapper.Map<Employee>(employeeForCreationDto);
        _repository.Employee.CreateEmployee(companyId,employee);
        _repository.SaveChanges();
        var employeeToReturn  = _mapper.Map<EmployeeDto>(employee);
        return CreatedAtRoute("GetEmployeeById", new {companyId,employeeId=employee.Id }, employeeToReturn);
    }
}