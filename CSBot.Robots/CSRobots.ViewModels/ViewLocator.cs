namespace CSRobots.ViewModels
{
    public class ViewLocator
    {
        public static MainView MainView { get; private set; }

        static ViewLocator()
        {
            MainView = new MainView();
        }
    }
}
