using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using RepoDb;

namespace DicomDirExporter.Repository.Base
{
    public class Repository<T> : RepositoryBase<T> where T : class
    {
        protected string TableName { get; set; }

        public Repository(string tableName)
        {
            TableName = tableName;
        }

        public override IEnumerable<T> Get(Expression<Func<T, bool>> lambda)
        {
            IEnumerable<T> result;

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    result = connection.Query(lambda, trace: new SqlTracer());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }

            return result;
        }

        public IEnumerable<T> Get(QueryField[] where, IEnumerable<OrderField> order = null)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.Query<T>(TableName, where, orderBy: order);
            }
        }

        public override async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> lambda)
        {
            IEnumerable<T> result;

            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    result = await connection.QueryAsync(lambda, trace: new SqlTracer());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }

            return result;
        }

        public async Task<IEnumerable<T>> GetAsync(QueryField[] where, IEnumerable<OrderField> order = null)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    return await connection.QueryAsync<T>(TableName, where, orderBy: order);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public override List<T> GetAll()
        {
            List<T> lstPatients;
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    lstPatients = connection.QueryAll<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

            return lstPatients;
        }

        public override void Add(T item)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Insert(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }
        }

        public override void Update(T item)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Update(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }
        }

        public override void Update(T item, IEnumerable<Field> fields)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Update<T>(item, fields, trace: new SqlTracer());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }
        }

        public override void Remove(string id)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Delete<T>(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }
        }

        public override void Upsert(T item)
        {
            try
            {
                using (var connection = new SqlConnection(ConnectionString))
                {
                    connection.Merge<T>(item, trace: new SqlTracer());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new SystemException(ex.Message, ex);
            }
        }

        public IEnumerable<DbField> GetTableFields()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var helper = connection.GetDbHelper();
                return helper.GetFields(connection, TableName);
            }
        }
    }
}