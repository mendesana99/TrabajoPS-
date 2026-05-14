using System.Collections.Generic;

namespace Application.DTOs
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = new List<T>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (Total + PageSize - 1) / PageSize;
        public bool HasNext => Page < TotalPages;
    }
}
