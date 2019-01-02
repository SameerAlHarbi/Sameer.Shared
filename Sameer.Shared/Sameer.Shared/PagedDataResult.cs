using System.Collections.Generic;

namespace Sameer.Shared
{
    public class PagedDataResult<T> where T:class,ISameerObject
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int PagesCount { get; set; }

        public int DataCount { get; set; }

        public List<T> DataList { get; set; }
    }
}
