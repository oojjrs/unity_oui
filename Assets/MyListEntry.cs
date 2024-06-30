namespace oojjrs.oui
{
    public interface MyListEntry<TValue>
    {
        public int SortingOrder { get; set; }
        public TValue Value { get; set; }
    }
}
