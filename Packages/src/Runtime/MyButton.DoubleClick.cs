namespace oojjrs.oui
{
    public partial class MyButton
    {
        public interface DoubleClickInterface
        {
            void OnDoubleClick();
        }

        private DoubleClickInterface[] _doubleClicks;

        public void OnDoubleClick()
        {
            if (_doubleClicks != null)
            {
                foreach (var doubleClick in _doubleClicks)
                    doubleClick.OnDoubleClick();
            }
        }
    }
}
