using System.Data;
using System.Threading.Tasks;

namespace DynamicExcelUpload.Contract.BLL
{
    public interface IUploadBLL
    {
        Task CreateTable(DataTable dt, string fileName);
        void InsertData(DataTable dt, string fileName);
        Task<DataTable> GetData(string fileName);
    }
}
