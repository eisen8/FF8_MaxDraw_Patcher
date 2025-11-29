using System.Threading.Tasks;
using Windows.Storage;

namespace FF8_MaxDraw_Patcher.Services.Interfaces
{
    public interface IUIService
    {
        Task<StorageFile> FilePicker();
        void RequestPathBoxFocus();
    }
}