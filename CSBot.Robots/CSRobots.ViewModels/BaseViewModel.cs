using System.Diagnostics;
using GalaSoft.MvvmLight;

namespace CSRobots.ViewModels
{
    public abstract class BaseViewModel : ViewModelBase
    {
        private static string GetPropertyName(StackFrame frame)
        {
            return frame.GetMethod().Name.Substring(4);
        }

        private bool _isProcessing;

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { SetPropertyValue(out _isProcessing, value); }
        }

        public virtual void OnValidationComplete() { }

        public virtual void Reset()
        {
            IsProcessing = false;
        }

        public virtual void SetPropertyValue<T>(out T currentValue, T newValue)
        {
            currentValue = newValue;
            PropertyHasChanged();
        }
       
        private void PropertyHasChanged()
        {
            var currentFrame = 2;
            var frame = new StackFrame(currentFrame);
            var propertyName = string.Empty;
            if (frame.GetMethod().Name.Length > 4) propertyName = GetPropertyName(frame);
            while (!frame.GetMethod().Name.StartsWith("set_"))
            {
                currentFrame++;
                frame = new StackFrame(currentFrame);
                if (frame.GetMethod().Name.Length > 4) propertyName = GetPropertyName(frame);
            }
            RaisePropertyChanged(propertyName);
        }
    }
}
