using DynamicExcelUpload.Contract.BLL;
using DynamicExcelUpload.Contract.Repositories;
using DynamicExcelUpload.Core.Helpers;
using System.Data;
using System.Threading.Tasks;

namespace DynamicExcelUpload.BLL
{
    public class UploadBLL : IUploadBLL
    {
        private readonly IRepository _repo;

        public UploadBLL(IRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateTable(DataTable dt, string fileName)
        {
            var query = QueryHelper.GetQueryCreateTable(SetTableName(fileName), dt);
            await _repo.ExecuteCreateTable(query);
        }

        public void InsertData(DataTable dt, string fileName)
        {
            _repo.ExecuteInsertData(dt, SetTableName(fileName));
        }

        public async Task<DataTable> GetData(string fileName)
        {
            var query = QueryHelper.GetQueryData(SetTableName(fileName));
            var data = await _repo.GetData(query);
            return data;
        }

        private string SetTableName(string fileName)
        {
            return fileName.Trim().Replace(" ", "_");
        }
    }
}
