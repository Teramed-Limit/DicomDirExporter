using DicomDirExporter.Model;
using DicomDirExporter.Model.Repository;
using DicomDirExporter.Repository.View;
using RepoDb;
using RepoDb.Enumerations;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DicomDirExporter.Service
{
    public class HistoryQueryService
    {
        private readonly DcmFindStudyLevelViewRepository _searchStudyViewRepo = new DcmFindStudyLevelViewRepository();

        public IEnumerable<DbField> GetQueryFields()
        {
            return _searchStudyViewRepo.GetTableFields();
        }

        public async Task<IEnumerable<DcmFindStudyLevelView>> RapidQuery(int day)
        {
            var where = new[]
            {
                new QueryField("StudyDate", Operation.Between,
                    new[]
                    {
                        DateTime.Now.AddDays(-day).ToString("yyyy-MM-dd HH:mm:ss"),
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                    })
            };

            return await _searchStudyViewRepo.GetAsync(where);
        }

        public async Task<IEnumerable<DcmFindStudyLevelView>> ConditionQuery(QueryField[] where)
        {
            return await _searchStudyViewRepo.GetAsync(where);
        }


        public IEnumerable<QueryField> ConvertQueryFields(IEnumerable<UIQueryField> uiQueryFields)
        {
            // filter value is 'null' or ''
            var validFields = uiQueryFields.Where(x => !string.IsNullOrWhiteSpace(x.Value));

            // deal operation is not 'Between','NotBetween','In','NotIn'
            var queryFields = validFields
                .Where(x =>
                    x.Operation == Operation.Equal ||
                    x.Operation == Operation.NotEqual ||
                    x.Operation == Operation.Like ||
                    x.Operation == Operation.NotLike ||
                    x.Operation == Operation.GreaterThan ||
                    x.Operation == Operation.GreaterThanOrEqual ||
                    x.Operation == Operation.LessThan ||
                    x.Operation == Operation.LessThanOrEqual)
                .Select(field => field.ConvertToQueryField()).ToList();

            // deal operation is 'Between','NotBetween','In','NotIn'
            var multiValueFields = validFields
                .Where(x =>
                    x.Operation == Operation.Between ||
                    x.Operation == Operation.NotBetween ||
                    x.Operation == Operation.In ||
                    x.Operation == Operation.NotIn)
                .GroupBy(x => x.Field.Name);

            foreach (var grouping in multiValueFields)
            {
                var name = grouping.Key;
                var operation = Operation.Between;
                var value = new List<string>();
                foreach (var field in grouping)
                {
                    string sysFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                    operation = field.Operation;
                    if (field.UIType == "DatePicker")
                    {
                        value.Add(DateTime
                            .ParseExact(field.Value, sysFormat, CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.AllowWhiteSpaces).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else
                    {
                        value.Add(field.Value);
                    }
                }
                queryFields.Add(new QueryField(name, operation, value));
            }

            return queryFields;
        }
    }
}