using System.Diagnostics.SymbolStore;
using AutoMapper;
using CompanyEmployees.ModelBinders;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
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

    [HttpGet("{id}", Name = "GetCompanyById")]
    public IActionResult GetCompany(Guid id)
    {
        var company = _repository.Company.GetCompany(id, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {id} doesn't exist in the database.");
            return NotFound();
        }

        var companyDto = _mapper.Map<CompanyDto>(company);
        return Ok(companyDto);
    }

    [HttpPost]
    public IActionResult CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
    {
        if (companyForCreationDto == null)
        {
            _logger.LogError("CompanyForCreationDto object sent from client is null.");
            return BadRequest("CompanyForCreationDto object is null");
        }

        var company = _mapper.Map<Company>(companyForCreationDto);
        _repository.Company.CreateCompany(company);
        _repository.SaveChanges();
        var companyToReturn = _mapper.Map<CompanyDto>(company);
        return CreatedAtRoute("GetCompanyById", new { Id = companyToReturn.Id }, companyToReturn);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public IActionResult GetCompanyCollection(IEnumerable<Guid> ids)
    {
        if (ids == null)
        {
            _logger.LogError("Parameter ids is null");
            return BadRequest("Parameter ids is null");
        }

        var companyCollection = _repository.Company.GetByIds(ids, trackChanges: false);
        if (ids.Count() != companyCollection.Count())
        {
            _logger.LogError("Some ids are not valid in a collection");
            return NotFound();
        }

        var companyCollectionToReturn = _mapper.Map<CompanyDto>(companyCollection);
        return Ok(companyCollectionToReturn);
    }

    [HttpPost("collection")]
    public IActionResult CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companies)
    {
        if (companies == null)
        {
            _logger.LogError("Parameter companies is null");
            return BadRequest("Parameter companies is null");
        }

        var companiesToAdd = _mapper.Map<IEnumerable<Company>>(companies);
        foreach (var company in companiesToAdd)
        {
            _repository.Company.CreateCompany(company);
        }

        _repository.SaveChanges();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companiesToAdd);
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
        return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
    }
}