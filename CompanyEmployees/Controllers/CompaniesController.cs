using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly IRepositoryManager _repository;
    private readonly ILogger<CompaniesController> _logger;
    private readonly IMapper _mapper;

    public CompaniesController(IRepositoryManager repository, ILogger<CompaniesController> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllCompanies()
    {
        var companies = _repository.Company.GetAllCompanies(trackChanges: false);
        /*var companiesDto = companies.Select(c => new CompanyDto()
        {
            Name = c.Name,
            FullAddress = c.Address+" "+c.Country
        });*/
        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
        return Ok(companiesDto);
    }
}