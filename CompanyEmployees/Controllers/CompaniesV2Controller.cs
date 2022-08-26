using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers;

[ApiVersion("2.0",Deprecated = true)]
[Route("api/{v:apiversion}/companies")]
[ApiController]
public class CompaniesV2Controller : ControllerBase
{
    [HttpOptions]
    public IActionResult GetCompaniesReadOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PATCH");
        return Ok();

    }
}