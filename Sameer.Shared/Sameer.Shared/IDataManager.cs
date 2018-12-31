using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sameer.Shared
{
    public interface IDataManager<T> : IDisposable where T : class, ISameerObject, new()
    {
        Task<List<T>> GetAllDataList();

        Task<T> GetDataById(int id);

        Task<DataActionResult<T>> InsertNewDataItem(T newItem);

        Task<DataActionResult<T>> UpdateDataItem(T currentItem);

        Task<DataActionResult<T>> DeleteDataItem(int itemId);

    }

}
