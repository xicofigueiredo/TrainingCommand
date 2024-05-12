using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Application.DTO;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingController : ControllerBase
{
    private readonly TrainingService _trainingService;

    private List<string> _errorMessages = new List<string>();

    public TrainingController(TrainingService trainingService)
    {
        _trainingService = trainingService;
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Puttraining(long id, TrainingDTO trainingDTO)
    {
        if (id != trainingDTO.Id)
        {
            return BadRequest();
        }
    
        bool wasUpdated = await _trainingService.Update(id, trainingDTO, _errorMessages, true);
        if (!wasUpdated)
        {
            return BadRequest(_errorMessages);
        }
    
        return NoContent();
    }

    [HttpPost]
    public async Task<ActionResult<IEnumerable<TrainingDTO>>> PostTraining(TrainingDTO trainingDTO)
    {
        TrainingDTO trainingResultDTO = await _trainingService.AddFromRest(trainingDTO, _errorMessages);
        if (trainingResultDTO is not null)
        {
            return Created("", trainingResultDTO);
        }
        else
        {
            return BadRequest(_errorMessages);
        }
    }
}