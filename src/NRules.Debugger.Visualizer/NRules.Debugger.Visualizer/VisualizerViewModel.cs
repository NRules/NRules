using System.ComponentModel;
using NRules.Debugger.Visualizer.Model;

namespace NRules.Debugger.Visualizer
{
    internal class VisualizerViewModel : INotifyPropertyChanged
    {
        public ReteGraph Graph { get; private set; }

        internal VisualizerViewModel(ReteGraph graph)
        {
            Graph = graph;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}