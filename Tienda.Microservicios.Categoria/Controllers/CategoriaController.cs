using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Tienda.Microservicios.Categoria.Dto;
using Tienda.Microservicios.Categoria.Persistencia;

namespace Tienda.Microservicios.Categoria.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _categoriasCollection;

        public CategoriaController(MongoDBSetting mongoDBSetting)
        {
            var client = new MongoClient(mongoDBSetting.DefaultConnection);
            var database = client.GetDatabase(mongoDBSetting.Database);
            _categoriasCollection = database.GetCollection<BsonDocument>(mongoDBSetting.Collection);
        }

        [HttpPost, Route("Crear")]
        public async Task<ActionResult> Crear(string Nombre)
        {

            if (Nombre != null) {
              var registro = new BsonDocument
                {
                    { "CategoriaId", Guid.NewGuid().ToString() },
                    { "NombreCategoria", Nombre }
                };
             await _categoriasCollection.InsertOneAsync(registro);

             return Ok("Categoria guardada");
            }
            return BadRequest("Categoria creada con éxito");
            
        }


        [HttpGet, Route("GetCategorias")]
        public async Task<ActionResult<List<CategoriaDto>>> GetCategorias()
        {
            var categorias = await _categoriasCollection.Find(new BsonDocument()).ToListAsync();
            
            var resultado = categorias.Select(c => new CategoriaDto 
            {
                CategoriaId = Guid.Parse(c["CategoriaId"].AsString),
                NombreCategoria = c["NombreCategoria"].AsString
            }).ToList();

            return Ok(resultado);
        }


        [HttpGet, Route("GetCategoria")]
        public async Task<ActionResult<CategoriaDto>> GetCategoria(string id)
        {
            var filtro = Builders<BsonDocument>.Filter.Eq("CategoriaId", id);
            var categoria = await _categoriasCollection.Find(filtro).FirstOrDefaultAsync();

            if (categoria == null)
                return NotFound("Categoría no encontrada");

            var resultado = new CategoriaDto
            {
                CategoriaId = Guid.Parse(categoria["CategoriaId"].AsString),
                NombreCategoria = categoria["NombreCategoria"].AsString
            };

            return Ok(resultado);
        }
    }
}
