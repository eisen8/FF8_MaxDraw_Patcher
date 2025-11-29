using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FF8_MaxDraw_Patcher.Model;
using FF8_MaxDraw_Patcher.Services.Interfaces;
using FF8_MaxDraw_Patcher.Utils;
using Microsoft.UI;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using static FF8_MaxDraw_Patcher.Utils.Logger;

namespace FF8_MaxDraw_Patcher.ViewModels
{

    /// <summary>
    /// View Model for the Main View.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private readonly Logger _l;
        private readonly IUIService _uiService;
        private readonly IPatcher _patcher;
        private readonly IFileValidator _validator;

        private bool _isPatching = false;

        /// <summary>
        /// The selected file path
        /// </summary>
        [ObservableProperty]
        public string filePath = "Select a file...";

        /// <summary>
        /// Whether we can patch the selected file or not.
        /// </summary>
        [ObservableProperty]
        public bool canPatch = false;

        public MainViewModel(IUIService uiService, IPatcher patcher, IFileValidator validator, Logger logger) 
        {
            _patcher = patcher;
            _l = logger;
            _uiService = uiService;
            _validator = validator;
        }

        /// <summary>
        /// Allows the user to Browse for the file to patch.
        /// </summary>
        [RelayCommand]
        public async Task BrowseFile()
        {
            try
            {
                StorageFile file = await _uiService.FilePicker();
                if (file != null)
                {
                    FilePath = file.Path;
                    _uiService.RequestPathBoxFocus();

                    // Valiate the Selected File is a valid patchable file
                    CanPatch = await ValidateFile(FilePath);
                }
            }
            catch (Exception ex)
            {
                _l.LogFatal(ex, "Browsing for File threw an exception.");
            }
        }
       
        /// <summary>
        /// Patches the selected file.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task Patch()
        {
            if (_isPatching || !CanPatch) return;

            CanPatch = false;
            _isPatching = true;
            try
            {
                // Validate file path is selected (should always be since UI disables button if it is not)
                if (string.IsNullOrEmpty(FilePath))
                {
                    _l.Log("No file selected. Cannot patch.", LogLevel.Error);
                    return;
                }

                // Back up the file
                _l.Log("Backing up file...");
                string backupPath = _patcher.BackupFile(FilePath);
                if (string.IsNullOrEmpty(backupPath))
                {
                    _l.Log("Backup failed. Aborting patch.", LogLevel.Error);
                    return;
                }
                _l.Log($"File backed up to {backupPath}.");

                // Applt patch
                _l.Log("Applying patch...");
                bool result = await _patcher.Patch(FilePath);
                if (result)
                {
                    _l.Log("Patch completed successfully.", Colors.Green);
                } else
                {
                    _l.Log("Patch was not successful.", LogLevel.Error);
                }
            }
            catch (Exception ex)
            {
                _l.LogFatal(ex, "Patching File threw an exception.");
            }
            finally
            {
                _isPatching = false;
            }
        }

        /// <summary>
        /// Validates the selected file.
        /// </summary>
        /// <param name="filePath">The file path to validate.</param>
        /// <returns>True if the file is patchable, false if not.</returns>
        private async Task<bool> ValidateFile(string filePath)
        {
            try
            { 
                _l.Log($"Validating File: {filePath}");
                FileValidationResult result = await _validator.ValidateFile(filePath);
                if (result.Success)
                {
                    _l.Log(result.Message, Colors.Green);
                    return true;
                }
                else
                {
                    _l.Log(result.Message, LogLevel.Error);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _l.LogFatal(ex, "Validating File threw an exception.");
                return false;
            }
        }


    }
}
