using System.Collections.Generic;
using System.Threading.Tasks;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Service;
using RepoDb;

namespace DicomDirExporter.Repository.Base
{
    public interface IViewRepository<T>
    {
        IEnumerable<T> Get(QueryField[] where, IEnumerable<OrderField> order);
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> GetAsync(QueryField[] where, IEnumerable<OrderField> order = null);
    }


    public abstract class ViewRepositoryBase<T> : IViewRepository<T>
    {
        protected string ConnectionString = ConfigService.GetInstance().GetConnectionString();
        public abstract IEnumerable<T> Get(QueryField[] where, IEnumerable<OrderField> order);
        public abstract IEnumerable<T> GetAll();
        public abstract Task<IEnumerable<T>> GetAsync(QueryField[] where, IEnumerable<OrderField> order = null);
    }
}