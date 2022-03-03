using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DicomDirExporter.Service;
using RepoDb;

namespace DicomDirExporter.Repository.Base
{
    public interface IRepository<T>
    {
        IEnumerable<T> Get(Expression<Func<T, bool>> lambda);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> lambda);
        List<T> GetAll();
        void Add(T list);
        void Update(T list);
        void Update(T item, IEnumerable<Field> fields);
        void Upsert(T item);
        void Remove(string id);
    }


    public abstract class RepositoryBase<T> : IRepository<T>
    {
        protected string ConnectionString = ConfigService.GetInstance().GetConnectionString();
        public abstract IEnumerable<T> Get(Expression<Func<T, bool>> lambda);
        public abstract Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> lambda);
        public abstract List<T> GetAll();
        public abstract void Add(T list);
        public abstract void Update(T list);
        public abstract void Update(T item, IEnumerable<Field> fields);
        public abstract void Upsert(T item);
        public abstract void Remove(string id);
    }
}