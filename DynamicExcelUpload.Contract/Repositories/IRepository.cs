using System.Data;
using System.Threading.Tasks;

namespace DynamicExcelUpload.Contract.Repositories
{
    public interface IRepository
    {
        Task ExecuteCreateTable(string query);
        Task<DataTable> GetData(string query);
        void ExecuteInsertData(DataTable dt, string tableName);
    }
}
