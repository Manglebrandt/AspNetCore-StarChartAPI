using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
	[Route("")]
	[ApiController]
	public class CelestialObjectController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public CelestialObjectController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpGet("{id:int}", Name = "GetById")]
		public IActionResult GetById(int id)
		{
			var celestialObject = _context.CelestialObjects.Find(id);
			if (celestialObject == null)
				return NotFound();

			celestialObject.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
			return Ok(celestialObject);
		}

		[HttpGet("{name}")]
		public IActionResult GetByName(string name)
		{
			var celestialObjects = _context.CelestialObjects.Where(x => x.Name == name);
			if (!celestialObjects.Any())
				return NotFound();

			foreach (var celestialObject in celestialObjects)
				celestialObject.Satellites = _context.CelestialObjects
					.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();

			return Ok(celestialObjects.ToList());
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			var celestialObjects = _context.CelestialObjects;
			foreach (var celestialObject in celestialObjects)
				celestialObject.Satellites =
					_context.CelestialObjects.Where(x => x.OrbitedObjectId == celestialObject.Id).ToList();

			return Ok(celestialObjects);
		}

		[HttpPost]
		public IActionResult Create([FromBody] CelestialObject celestialObject)
		{
			_context.CelestialObjects.Add(celestialObject);
			_context.SaveChanges();

			return CreatedAtRoute("GetById", new { id = celestialObject.Id });
		}

		[HttpPut("{id}")]
		public IActionResult Update(int id, CelestialObject celestialObject)
		{
			var existingCelestialObject = _context.CelestialObjects.Find(id);
			if (existingCelestialObject == null) return NotFound();

			existingCelestialObject.Name = celestialObject.Name;
			existingCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
			existingCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

			_context.Update(existingCelestialObject);
			_context.SaveChanges();

			return NoContent();
		}

		[HttpPatch("{id}/{name}")]
		public IActionResult RenameObject(int id, string name)
		{
			var existingCelestialObject = _context.CelestialObjects.Find(id);
			if (existingCelestialObject == null) return NotFound();

			existingCelestialObject.Name = name;
			_context.Update(existingCelestialObject);
			_context.SaveChanges();

			return NoContent();
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			var celestialObjects = _context.CelestialObjects.Where(x => x.Id == id || x.OrbitedObjectId == id).ToList();
			if (!celestialObjects.Any()) return NotFound();

			foreach (var celestialObject in celestialObjects) _context.RemoveRange(celestialObject);

			_context.SaveChanges();
			return NoContent();
		}
	}
}
