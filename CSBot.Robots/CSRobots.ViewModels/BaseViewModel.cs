using System;
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

        protected BaseViewModel()
        {
            RegisterForMessages();
            if (IsInDesignMode) SetDesignTimeInfo();
        }

        public bool IsProcessing
        {
            get { return _isProcessing; }
            set { SetStructPropertyValue(ref _isProcessing, value); }
        }

        public virtual void OnValidationComplete() { }

        public virtual void Reset()
        {
            IsProcessing = false;
        }

        public virtual void SetPropertyValue<T>(ref T currentValue, T newValue, Action<T> extraFunction = null, Action voidAfterSetAction = null) where T : class
        {
            if (currentValue == newValue) return;

            currentValue = newValue;

            PropertyHasChanged();

            if (extraFunction != null) extraFunction(newValue);

            if (voidAfterSetAction != null) voidAfterSetAction();
        }

        public virtual void SetPropertyValue<T>(ref T currentValue, T newValue, Action extraFunction) where T : class
        {
            if (currentValue == newValue) return;

            currentValue = newValue;

            PropertyHasChanged();

            if (extraFunction != null) extraFunction();
        }

        public virtual void SetStructPropertyValue<T>(ref T currentValue, T newValue, Action<T> extraFunction = null, Action voidActionAfterSetAction = null)
        {
            currentValue = newValue;

            PropertyHasChanged();

            if (extraFunction != null) extraFunction(newValue);

            if (voidActionAfterSetAction != null) voidActionAfterSetAction();
        }

        public virtual void SetStructPropertyValue<T>(ref T currentValue, T newValue, Action extraFunction)
        {
            currentValue = newValue;

            PropertyHasChanged();

            if (extraFunction != null) extraFunction();
        }

        public virtual void SetValue<T>(ref T currentValue, T newValue, Action<T> voidOldValueAction = null, Action voidAfterSetAction = null) where T : class
        {
            var oldVal = currentValue;

            if (currentValue == newValue) return;

            currentValue = newValue;

            PropertyHasChanged();

            if (voidOldValueAction != null) voidOldValueAction(oldVal);

            if (voidAfterSetAction != null) voidAfterSetAction();
        }

        protected abstract void RegisterForMessages();
        protected abstract void SetDesignTimeInfo();

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
