using System.ComponentModel;
using System.Windows.Threading;

namespace SmartReader.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged 
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                SmartDispatcher.BeginInvoke(() => PropertyChanged(this, e));

            }
        }

        protected void RaiseProperyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }
    }
}
