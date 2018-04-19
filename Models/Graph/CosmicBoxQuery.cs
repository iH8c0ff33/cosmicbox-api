using System.Linq;
using GraphQL.Types;

namespace CosmicBox.Models.Graph {
    public class CosmicBoxQuery : ObjectGraphType {
        private readonly CosmicContext _context;

        public CosmicBoxQuery(CosmicContext dbContext) {
            this._context = dbContext;

            Field<BoxType>("box", resolve: context => _context.Boxes.FirstOrDefault(b => b.Id == 25));
        }
    }
}