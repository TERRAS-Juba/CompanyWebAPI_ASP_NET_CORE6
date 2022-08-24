using System.Diagnostics.SymbolStore;
using AutoMapper;
using CompanyEmployees.ModelBinders;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
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
    public async Task<IActionResult> GetAllCompanies()
    {
        var companies = await _repository.Company.GetAllCompanies(trackChanges: false);
        /*var companiesDto = companies.Select(c => new CompanyDto()
        {
            Name = c.Name,
            FullAddress = c.Address+" "+c.Country
        });*/
        var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
        return Ok(companiesDto);
    }

    [HttpGet("{companyId}", Name = "GetCompanyById")]
    public async Task <IActionResult> GetCompany(Guid companyId)
    {
        var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var companyDto = _mapper.Map<CompanyDto>(company);
        return Ok(companyDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyForCreationDto)
    {
        if (companyForCreationDto == null)
        {
            _logger.LogError("CompanyForCreationDto object sent from client is null.");
            return BadRequest("CompanyForCreationDto object is null");
        }
        var company = _mapper.Map<Company>(companyForCreationDto);
        _repository.Company.CreateCompany(company);
        await _repository.SaveChanges();
        var companyToReturn = _mapper.Map<CompanyDto>(company);
        return CreatedAtRoute("GetCompanyById", new { companyId = companyToReturn.Id }, companyToReturn);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection(IEnumerable<Guid> ids)
    {
        if (ids == null)
        {
            _logger.LogError("Parameter ids is null");
            return BadRequest("Parameter ids is null");
        }

        var companyCollection = await _repository.Company.GetByIds(ids, trackChanges: false);
        if (ids.Count() != companyCollection.Count())
        {
            _logger.LogError("Some ids are not valid in a collection");
            return NotFound();
        }

        var companyCollectionToReturn = _mapper.Map<CompanyDto>(companyCollection);
        return Ok(companyCollectionToReturn);
    }

    [HttpPost("collection")]
    public async Task<IActionResult> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companies)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the CompanyForCreationDto object");
            return UnprocessableEntity(ModelState);
        }
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

        await _repository.SaveChanges();

        var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companiesToAdd);
        var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
        return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
    }

    [HttpDelete("{companyId}")]
    public async Task<IActionResult> DeleteCompany(Guid companyId)
    {
        var company = await _repository.Company.GetCompany(companyId, trackChanges: false);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }
        _repository.Company.DeleteCompany(company);
        await _repository.SaveChanges();
        return NoContent();
    }
    [HttpPut("{companyId}")]
    public async Task<IActionResult> UpdateCompany(Guid companyId,[FromBody]CompanyForUpdateDto companyForUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the CompanyForUpdateDto object");
            return UnprocessableEntity(ModelState);
        }
        if (companyForUpdateDto == null)
        {
            _logger.LogError("CompanyForUpdateDto object sent from client is null.");
            return BadRequest("CompanyForUpdateDto object is null");
        }
        var company = await _repository.Company.GetCompany(companyId, trackChanges: true);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        _mapper.Map(companyForUpdateDto, company);
        await _repository.SaveChanges();
        return NoContent();
    }

    [HttpPatch("{companyId}")]
    public async Task<IActionResult> PartiallyUpdateCOmpany(Guid companyId,
        JsonPatchDocument<CompanyForUpdateDto> companyForUpdateDto)
    {
        if (companyForUpdateDto == null)
        {
            _logger.LogError("CompanyForUpdateDto object sent from client is null.");
            return BadRequest("CompanyForUpdateDto object is null");
        }
        var company = await _repository.Company.GetCompany(companyId, trackChanges: true);
        if (company == null)
        {
            _logger.LogInformation($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
        }

        var companyToPatch = _mapper.Map<CompanyForUpdateDto>(company);
        companyForUpdateDto.ApplyTo(companyToPatch,ModelState);
        TryValidateModel(companyToPatch);
        if(!ModelState.IsValid)
        {
            _logger.LogError("Invalid model state for the patch document");
            return UnprocessableEntity(ModelState);
        }
        _mapper.Map(companyToPatch, company);
        await _repository.SaveChanges();
        return NoContent();
    }
}