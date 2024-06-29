namespace oojjrs.oui
{
    public partial class MyButton
    {
        public interface DoubleClickInterface
        {
            void OnDoubleClick();
        }

        private DoubleClickInterface DoubleClick { get; set; }

        public void OnDoubleClick()
        {
            if (DoubleClick != default)
                DoubleClick.OnDoubleClick();
        }
    }
}
