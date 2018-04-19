using System.Threading.Tasks;
using CosmicBox.Models;
using CosmicBox.Models.Graph;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace CosmicBox.Controllers {
    [Route("graph")]
    public class GraphQLController : Controller {
        private readonly CosmicContext _context;

        public GraphQLController(CosmicContext context) => _context = context;

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLQuery query) {
            var schema = new Schema {Query = new CosmicBoxQuery(_context)};

            var result = await new DocumentExecuter().ExecuteAsync(_ => {
                _.Schema = schema;
                _.Query = query.Query;
            }).ConfigureAwait(false);

            if (result.Errors?.Count > 0) {
                return BadRequest();
            }

            return Ok(result);
        }
    }
}