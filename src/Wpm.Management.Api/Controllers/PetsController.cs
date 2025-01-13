using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wpm.Management.Api.DataAccess;
namespace Wpm.Management.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController(ManagementDbContext dbContext, ILogger<PetsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pets = await dbContext.Pets.Include(p => p.Breed).ToListAsync();
        return pets != null ? Ok(pets) : NotFound();
    }

    [HttpGet("{id}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var pet = await dbContext.Pets.Include(p => p.Breed)
        .FirstOrDefaultAsync(p => p.Id == id);
        return pet != null ? Ok(pet) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewPet newPet)
    {
        try
        {
            var pet = newPet.ToPet();
            await dbContext.Pets.AddAsync(pet);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute(nameof(GetById), new { id = pet.Id }, pet);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
    }
}
public record NewPet(string Name, int Age, int BreedId)
{
    public Pet ToPet() => new Pet() { Name = Name, Age = Age, BreedId = BreedId };
}