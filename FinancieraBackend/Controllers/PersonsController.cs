using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Models;

namespace FinancieraBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PersonsController : ControllerBase
    {
        private readonly IPersonService _personService;

        public PersonsController(IPersonService personService)
        {
            _personService = personService;
        }

        [HttpPost]
        public async Task<ActionResult<Persons>> Create([FromBody] CreatePersonDTO dto)
        {
            var result = await _personService.CreatePersonAsync(dto);
            return Ok(result);
        }

        [HttpGet("{identityNumber}")]
        public async Task<ActionResult<Persons>> Get(string identityNumber)
        {
            var result = await _personService.GetPersonAsync(identityNumber);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<Persons>>> GetAll()
        {
            var result = await _personService.GetAllPersonsAsync();
            return Ok(result);
        }

        [HttpPut("{identityNumber}")]
        public async Task<ActionResult<bool>> Update(string identityNumber, [FromBody] UpdatePersonDTO dto)
        {
            var result = await _personService.UpdatePersonAsync(identityNumber, dto);
            return Ok(result);
        }

        [HttpDelete("{identityNumber}")]
        public async Task<ActionResult<bool>> Delete(string identityNumber)
        {
            var result = await _personService.DeletePersonAsync(identityNumber);
            return Ok(result);
        }
    }
}
