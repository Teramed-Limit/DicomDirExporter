using DicomDirExporter.Repository;
using Microsoft.Data.SqlClient;
using RepoDb;
using System;

namespace DicomDirExporter.Service
{
    public class DicomArchivingService
    {
        private readonly string _connectionString = ConfigService.GetInstance().GetConnectionString();

        public void ArchiveDicom<T>(T dicomLevelObject) where T : class
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var transaction = connection.EnsureOpen().BeginTransaction())
                {
                    try
                    {
                        connection.Merge(dicomLevelObject, transaction: transaction,trace: new SqlTracer());
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}