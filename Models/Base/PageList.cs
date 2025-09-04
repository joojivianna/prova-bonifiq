namespace ProvaPub.Models.Base
{
    public class PageList<T>
    {
        public int TotalCount { get; set; }
        public bool HasNext { get; set; }
        public List<T> Items { get; set; }

        public PageList() { }
        
        public PageList(List<T> items, int totalCount, bool hasNext)
        {
            Items = items;
            TotalCount = totalCount;
            HasNext = hasNext;
        }

    }
}
