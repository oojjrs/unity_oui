namespace oojjrs.oui
{
    public partial class MyButton
    {
        public interface DoubleClickInterface
        {
            void OnDoubleClick();
        }

        private DoubleClickInterface[] DoubleClicks { get; set; }

        public void OnDoubleClick()
        {
            if (DoubleClicks != default)
            {
                foreach (var dc in DoubleClicks)
                    dc.OnDoubleClick();
            }
        }
    }
}
