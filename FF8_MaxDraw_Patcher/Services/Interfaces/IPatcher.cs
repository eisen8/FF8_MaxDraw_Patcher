using System.Threading.Tasks;

namespace FF8_MaxDraw_Patcher.Services.Interfaces
{
    public interface IPatcher
    {
        string BackupFile(string filePath);
        Task<bool> IsFileAlreadyPatched(string filePath);
        Task<bool> IsFilePatchable(string filePath);
        Task<bool> Patch(string filePath);
    }
}