using System.ComponentModel;
using NRules.Debug.Visualizer.Model;

namespace NRules.Debug.Visualizer
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