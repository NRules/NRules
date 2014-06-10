using QuickGraph;

namespace NRules.Debugger.Visualizer.Model
{
    public class ReteEdge : Edge<ReteNode>
    {
        public ReteEdge(ReteNode source, ReteNode target) : base(source, target)
        {
        }
    }
}