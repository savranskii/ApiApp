using ApiApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace ApiApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EntityController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;

        public EntityController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Insert entity to the store
        /// </summary>
        /// <param name="insert">JSON value</param>
        /// <returns>A newly created Entity</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item unable to deserialize or item is null</response>
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Add(string insert)
        {
            Entity? entity;

            try
            {
                entity = JsonSerializer.Deserialize<Entity>(insert);
            }
            catch (Exception error)
            {
                return BadRequest($"Unable to deserialize. {error.Message}.");
            }

            if (entity == null)
            {
                return BadRequest("Invalid json data.");
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
            _memoryCache.Set(entity.Id.ToString(), entity, cacheEntryOptions);

            return CreatedAtAction(nameof(Get), new { get = entity.Id }, entity);
        }

        /// <summary>
        /// Retrieve entity by id from the store
        /// </summary>
        /// <param name="get">Entity Id</param>
        /// <returns>Entity</returns>
        /// <response code="200">Returns entity</response>
        /// <response code="404">If the item is not found</response>
        [HttpGet()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get(string get)
        {
            if (!_memoryCache.TryGetValue<Entity>(get, out var entity))
            {
                return NotFound(get);
            }

            return Ok(JsonSerializer.Serialize(entity));
        }
    }
}
