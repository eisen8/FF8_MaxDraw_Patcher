using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Patch;
using FF8_MaxDraw_Patcher.Services;
using FF8_MaxDraw_Patcher.Utils;
using Microsoft.UI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;

namespace FF8_MaxDraw_Patcher.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        private readonly Logger _l;
        private readonly UIService _uiService;
        private readonly Patcher _patcher;
        private readonly FileValidator _validator;

        private bool _isPatching = false;

        [ObservableProperty]
        public string filePath = "Select a file...";
        [ObservableProperty]
        public bool canPatch = false;

        public ObservableCollection<LogEntry> Logs { get; } = new();

        public MainViewModel(UIService uiService, Patcher patcher, FileValidator validator, Logger logger) 
        {
            _patcher = patcher;
            _l = logger;
            _uiService = uiService;
            _validator = validator;
        }

        public void Log(string message, Windows.UI.Color? color = null)
        {
            Logs.Add(new LogEntry
            {
                Message = message,
                Color = color ?? Colors.White
            });
        }

        [RelayCommand]
        public async Task BrowseFile()
        {
            StorageFile file = await _uiService.FilePicker();
            if (file != null)
            {
                FilePath = file.Path;
                _uiService.RequestPathBoxFocus();
                CanPatch = await ValidateFile(FilePath);
            }
        }
       
        [RelayCommand]
        public async Task Patch()
        {
            CanPatch = false;
            if (_isPatching) return;
            _isPatching = true;

            try
            {
                Log("Backing up file...");
                string backupPath = _patcher.BackupFile(FilePath);
                if (string.IsNullOrEmpty(backupPath))
                {
                    Log("Backup failed. Aborting patch.", Colors.Red);
                    return;
                }
                Log($"File backed up to {backupPath}");
                Log("Applying patch...");
                bool result = _patcher.Patch(FilePath);
                if (result)
                {
                    Log("Patch completed successfully.", ColorHelper.FromArgb(255, 0, 204, 102));
                } else
                {
                    Log("Patch was not successful.", Colors.Red);
                }
            }
            catch (Exception e)
            {
                Log("Patch was not successful.", Colors.Red);
            }
            finally
            {
                _isPatching = false;
            }
        }


        private async Task<bool> ValidateFile(string filePath)
        {
            Log($"Validating File: {filePath}");
            var result = _validator.ValidateFile(filePath);
            if (result.Success)
            {
                Log(result.Message, ColorHelper.FromArgb(255, 0, 204, 102));
                return true;
            }
            else
            {
                Log(result.Message, Colors.Red);
                return false;
            }
        }


    }
}
