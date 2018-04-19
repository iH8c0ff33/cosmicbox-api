using GraphQL.Types;

namespace CosmicBox.Models.Graph {
    public class BoxType : ObjectGraphType<Box> {
        public BoxType() {
            Field(b => b.Id).Description("The Id of the Cosmic Box");
        }
    }
}