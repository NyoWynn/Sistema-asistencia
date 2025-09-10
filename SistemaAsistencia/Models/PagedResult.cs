using System.Collections.Generic;

namespace SistemaAsistencia.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
        public int StartPage => Math.Max(1, PageNumber - 2);
        public int EndPage => Math.Min(TotalPages, PageNumber + 2);
    }
}
