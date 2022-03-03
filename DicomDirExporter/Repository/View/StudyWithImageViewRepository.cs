using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.Base;
using RepoDb;
using RepoDb.Enumerations;

namespace DicomDirExporter.Repository.View
{
    public class StudyWithImageViewRepository : ViewRepositoryBase<StudyWithImageView>
    {
        private const string TableName = "[dbo].[StudyWithImageView]";

        public override IEnumerable<StudyWithImageView> Get(QueryField[] where, IEnumerable<OrderField> order = null)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var orderBy = OrderField.Parse(new { InstanceNumber = Order.Ascending });
                return connection.Query<StudyWithImageView>(TableName, where, orderBy: orderBy);
            }
        }

        public override async Task<IEnumerable<StudyWithImageView>> GetAsync(
            QueryField[] where, IEnumerable<OrderField> order = null)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var orderBy = OrderField.Parse(new { InstanceNumber = Order.Ascending });
                return await connection.QueryAsync<StudyWithImageView>(TableName, where, orderBy: orderBy);
            }
        }

        public override IEnumerable<StudyWithImageView> GetAll()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                return connection.QueryAll<StudyWithImageView>();
            }
        }
    }
}