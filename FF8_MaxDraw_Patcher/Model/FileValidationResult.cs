namespace FF8_MaxDraw_Patcher.Model
{

    /// <summary>
    /// The result of the file validation.
    /// </summary>
    public class FileValidationResult
    {
        /// <summary>
        /// Whether validation passed or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// A message describing the validation result.
        /// </summary>
        public string Message { get; set; } = string.Empty;

    }
}
