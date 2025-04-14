using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace ShareModels
{
    public interface IService<T>
    {
        Task<IEnumerable<T>> Get();
        Task<T> Get(int id);
        Task<T> Post(T value);
        Task<bool> Update(int id, T value);
        Task<bool> Delete(int id);
    }





}
