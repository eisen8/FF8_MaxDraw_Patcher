using FF8_MaxDraw_Patcher.Model;
using System.Threading.Tasks;

namespace FF8_MaxDraw_Patcher.Services.Interfaces
{
    public interface IFileValidator
    {
        Task<FileValidationResult> ValidateFile(string filePath);
    }
}