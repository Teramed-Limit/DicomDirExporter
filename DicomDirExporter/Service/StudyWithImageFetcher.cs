using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.View;
using RepoDb;
using RepoDb.Enumerations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DicomDirExporter.Service
{
    public class StudyWithImageFetcher
    {
        private readonly StudyWithImageViewRepository _studyWithImageViewRepository = new StudyWithImageViewRepository();

        public async Task<IEnumerable<StudyWithImageView>> FetchStudies(string studyInstanceUID)
        {
            var where = new[]
            {
                new QueryField("StudyInstanceUID", Operation.Equal, studyInstanceUID)
            };

            return await _studyWithImageViewRepository.GetAsync(where);
        }
    }
}