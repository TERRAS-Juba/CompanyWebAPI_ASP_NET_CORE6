using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
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

    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    [HttpGet]
    public async Task<IActionResult> GetAllEmployeesByCompanyId(Guid companyId)
    {
        // Added via filter : ValidateCompanyExistsAttribute))]
        /*
        var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }*/
        var employees = await _repository.Employee.GetAllEmployees(companyId, trackChanges: false);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
        return Ok(employeesDto);
    }
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    [HttpGet("{employeeId:guid}", Name = "GetEmployeeById")]
    public async Task<IActionResult> GetEmployeeById(Guid companyId, Guid employeeId)
    {
        // Added via filter : ValidateCompanyExistsAttribute
        /*var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }
        */
        var employee = await _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: false);
        if (employee == null)
        {
            _logger.LogInformation($"Employee with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var employeeDto = _mapper.Map<EmployeeDto>(employee);
        return Ok(employeeDto);
    }

    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPost]
    public async Task<IActionResult> CreateEmployee(Guid companyId, EmployeeForCreationDto employeeForCreationDto)
    {
        // Added via filter : ValidationFilterAttribute
        /*
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the EmployeeForCreationDto object");
            return UnprocessableEntity(ModelState);
        }
        if (employeeForCreationDto == null)
        {
            _logger.LogError("EmployeeForCreationDto object sent from client is null.");
            return BadRequest("EmployeeForCreationDto object is null");

        }*/
        var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var employee = _mapper.Map<Employee>(employeeForCreationDto);
        _repository.Employee.CreateEmployee(companyId, employee);
        await _repository.SaveChanges();
        var employeeToReturn = _mapper.Map<EmployeeDto>(employee);
        return CreatedAtRoute("GetEmployeeById", new { companyId, employeeId = employee.Id }, employeeToReturn);
    }
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    [HttpDelete("{employeeId}")]
    public async Task<IActionResult> DeleteEmployee(Guid companyId, Guid employeeId)
    {
        // Added via filter : ValidateCompanyExistsAttribute
        /*var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }*/

        var employee = await _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: false);
        if (employee == null)
        {
            _logger.LogInformation($"Employee with id: {employeeId} doesn't exist in the database.");
            return NotFound();
        }

        _repository.Employee.DeleteEmployee(employee);
        await _repository.SaveChanges();
        return NoContent();
    }
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [HttpPut("{employeeId}")]
    public async Task<IActionResult> UpdateEmployee(Guid companyId, Guid employeeId,
        [FromBody] EmployeeForUpdateDto employeeForUpdateDto)
    {
        // Added via filter : ValidationFilterAttribute
        /*if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
            return UnprocessableEntity(ModelState);
        }
        if (employeeForUpdateDto == null)
        {
            _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
            return BadRequest("EmployeeForUpdateDto object is null");
        }*/
        // Added via filter : ValidateCompanyExistsAttribute
        /*
        var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }*/
        var employee = await _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: true);
        if (employee == null)
        {
            _logger.LogInformation($"Employee with id: {employeeId} doesn't exist in the database.");
            return NotFound();
        }

        _mapper.Map(employeeForUpdateDto, employee);
        await _repository.SaveChanges();
        return NoContent();
    }
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    [ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
    [HttpPatch("{employeeId}")]
    public async Task<IActionResult> PartiallyUpdateEmployee(Guid companyId, Guid employeeId,
        JsonPatchDocument<EmployeeForUpdateDto> employeeForUpdateDto)
    {
        // Added via filter : ValidationFilterAttribute
        /*if (employeeForUpdateDto == null)
        {
            _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
            return BadRequest("EmployeeForUpdateDto object is null");
        }
        */
        // Added via filter : ValidateCompanyExistsAttribute
        /*var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }
        */
        var employee = await _repository.Employee.GetEmployee(companyId, employeeId, trackChanges: true);
        if (employee == null)
        {
            _logger.LogInformation($"Employee with id: {employeeId} doesn't exist in the database.");
            return NotFound();
        }

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employee);
        employeeForUpdateDto.ApplyTo(employeeToPatch, ModelState);
        TryValidateModel(employeeToPatch);
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the patch document");
            return UnprocessableEntity(ModelState);
        }

        _mapper.Map(employeeToPatch, employee);
        await _repository.SaveChanges();
        return NoContent();
    }
}