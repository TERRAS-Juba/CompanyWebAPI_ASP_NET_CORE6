using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers.V2;
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "Administrator")]
[ApiExplorerSettings(GroupName = "v2")]
public class CompaniesController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger<CompaniesController> _logger;
    private readonly IMapper _mapper;
    private readonly IDataShaper<CompanyDto> _dataShaper;
    private readonly IDataShaper<CompanyJoinEmployeeDto> _dataJoinShaper;

    public CompaniesController(IRepositoryManager repository, ILogger<CompaniesController> logger, IMapper mapper, IDataShaper<CompanyDto> dataShaper, IDataShaper<CompanyJoinEmployeeDto> dataJoinShaper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
        _dataShaper = dataShaper;
        _dataJoinShaper = dataJoinShaper;
    }

    [HttpOptions]
    public IActionResult GetCompaniesReadOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST");
        return Ok();

    }
}