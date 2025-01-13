using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wpm.Management.Api.DataAccess;

namespace Wpm.Management.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BreedsController(ManagementDbContext dbContext, ILogger<BreedsController> logger) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var breeds = await dbContext.Breeds.ToListAsync();
            if (breeds == null)
            {
                return NotFound();
            }
            return Ok(breeds);
        }

        [HttpGet("{id}", Name = nameof(GetBreedById))]
        public async Task<IActionResult> GetBreedById(int id)
        {
            var breed = await dbContext.Breeds.FirstOrDefaultAsync(b => b.Id == id);
            if (breed == null)
            {
                return NotFound();
            }
            return Ok(breed);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewBreed newBreed)
        {
            try
            {
                var breed = newBreed.ToBreed();
                await dbContext.Breeds.AddAsync(breed);
                await dbContext.SaveChangesAsync();

                return CreatedAtRoute(nameof(GetBreedById), new { id = breed.Id }, breed);
            }
            catch (System.Exception ex)
            {
                logger.LogError(ex.ToString());
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }

    public record NewBreed(string Name)
    {
        public Breed ToBreed() => new Breed(0, Name);
    }
}