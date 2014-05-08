using QuickGraph;

namespace NRules.Debug.Visualizer.Model
{
    public class ReteEdge : Edge<ReteNode>
    {
        public ReteEdge(ReteNode source, ReteNode target) : base(source, target)
        {
        }
    }
}