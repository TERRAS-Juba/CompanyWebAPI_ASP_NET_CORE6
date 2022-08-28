using AutoMapper;
using CompanyEmployees.ActionFilters;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompanyEmployees.Controllers.V1;
[ApiVersion("1")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<User> _userManger;
    private readonly ILogger<CompaniesController> _logger;
    private readonly IMapper _mapper;
    private readonly IDataShaper<CompanyDto> _dataShaper;
    private readonly IDataShaper<CompanyJoinEmployeeDto> _dataJoinShaper;
    private readonly IAuthenticationManager _authenticationManager;

    public AuthenticationController(ILogger<CompaniesController> logger, IMapper mapper,
        IDataShaper<CompanyDto> dataShaper, IDataShaper<CompanyJoinEmployeeDto> dataJoinShaper,
        UserManager<User> userManger, IAuthenticationManager authenticationManager)
    {
        _logger = logger;
        _mapper = mapper;
        _dataShaper = dataShaper;
        _dataJoinShaper = dataJoinShaper;
        _userManger = userManger;
        _authenticationManager = authenticationManager;
    }

    [HttpPost("regiser")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForCreationDto userForCreationDto)
    {
        var user = _mapper.Map<User>(userForCreationDto);
        var result = await _userManger.CreateAsync(user, userForCreationDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        await _userManger.AddToRolesAsync(user, userForCreationDto.Roles);
        return StatusCode(201);
    }

    [HttpPost("login")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthentificationDto userForAuthentificationDto)
    {
        if (!await _authenticationManager.ValidateUser(userForAuthentificationDto))
        {
            _logger.LogWarning($"{nameof(Authenticate)}: Authentication failed. Wrong email or password.");
            return Unauthorized($"{nameof(Authenticate)}: Authentication failed. Wrong email or password.");
        }

        return Ok(new { Token = await _authenticationManager.CreateToken() });
    }
}