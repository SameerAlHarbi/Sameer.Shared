using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sameer.Shared
{
    public interface IDataManager<T> : IDisposable where T : class, ISameerObject, new()
    {
        Task<List<T>> GetAllDataList();

        Task<List<T>> GetAllDataList(string fieldName, string fieldValue);

        Task<PagedDataResult<T>> GetPagedDataList(int pageNumber=1,int pageSize=100, string sort = "Id");

        Task<T> GetDataById(int id);

        Task<DataActionResult<T>> InsertNewDataItem(T newItem);

        Task<IEnumerable<DataActionResult<T>>> InsertNewDataItems(IEnumerable<T> newItems);

        Task<DataActionResult<T>> UpdateDataItem(T currentItem);

        Task<IEnumerable<DataActionResult<T>>> UpdateDataItems(IEnumerable<T> currentItems);

        Task<DataActionResult<T>> DeleteDataItem(int itemId);

    }

}
