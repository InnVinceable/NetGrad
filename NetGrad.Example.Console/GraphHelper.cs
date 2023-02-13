using csdot;
using csdot.Attributes.DataTypes;
using csdot.Attributes.Types;

namespace NetGrad.Example.Console
{
    internal class GraphHelper
    {
        public static void GenerateGraph(Value value)
        {
            var graph = new Graph("G");

            graph = RecurseValueGraph(graph, value);
            graph.attributes["rankdir"].TranslateToValue("LR");
            DotDocument d = new DotDocument();
            d.SaveToFile(graph, "./graph.dot");

        }

        private static Graph RecurseValueGraph(Graph graph, Value value, Node parent = null, IList<(string child, string parent)> edges = null)
        {
            if (edges == null)
            {
                edges = new List<(string child, string parent)>();
            }

            var node = new Node("\"" + value.Guid.ToString() + "\"");
            var label = new Label();
            node
                .attributes[nameof(Label).ToLower()]
                .TranslateToValue(value.ToString());

            node
                .attributes[nameof(Shape).ToLower()]
                .TranslateToValue("record");

            graph.AddElements(node);

            if (parent != null && !edges.Any(x => (x.child == node.ID && x.parent == parent.ID) || (x.parent == node.ID && x.child == parent.ID)))
            {
                var parentEdge = new Edge();
                parentEdge.Transition = new List<Transition>()
                    {
                        new Transition(node, EdgeOp.undirected),
                        new Transition(parent, EdgeOp.unspecified)
                    };
                graph.AddElement(parentEdge);
                edges.Add((node.ID, parent.ID));
            }

            if (value.Children != null)
            {
                foreach (var previous in value.Children)
                {
                    RecurseValueGraph(graph, previous, node, edges);
                }
            }


            return graph;
        }

    }
}
