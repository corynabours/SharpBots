using System.Windows.Media;

namespace CSRobots.ViewModels
{
    public class RobotStatus : BaseViewModel
    {
        private Brush _color;
        public Brush Color
        {
            get { return _color; }
            set { SetPropertyValue(ref _color, value); }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { SetPropertyValue(ref _text, value); }
        }

        protected override void RegisterForMessages(){}
        protected override void SetDesignTimeInfo(){}
    }
}
