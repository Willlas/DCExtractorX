namespace DC
{
    /// <summary>
    /// Class   :   "Settings"
    /// 
    /// Purpose :   headless, in-memory equivalent of the legacy DCExtractor.Settings class.
    /// 
    /// The original Settings class persisted values to a "settings.txt" file next to the executable and was
    /// read/written through a WinForms SettingsForm. That persistence/UI is host-specific (CLI/GUI concern),
    /// so this Core version keeps only the plain data + defaults that the parser/exporter code depends on.
    /// Hosts (CLI, GUI) are free to load/save these values however they like and assign them here at startup.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Enumeration :   "ModelConversionSetting"
        /// 
        /// Purpose     :   governs how the meshes of an mds model are packaged when they are exported to another format(smd/obj).
        /// 
        /// OneModel    :   all meshes will be kept inside a single model and exported as a single mesh to a single model file.
        /// ManyModels  :   each mesh will be unpacked from the model and exported as separate models.
        /// </summary>
        public enum ModelConversionSetting : int { OneModel, ManyModels };

        /// <summary>
        /// Gets/Sets the model conversion setting. This setting governs how meshes are packaged when a model is exported to another format.
        /// </summary>
        public static ModelConversionSetting modelConversion { get; set; } = ModelConversionSetting.OneModel;

        /// <summary>
        /// Gets/Sets whether or not to export animations when converting mds files.
        /// </summary>
        public static bool exportAnimations { get; set; } = false;

        /// <summary>
        /// Gets/Sets whether or not to show warnings about invalid MDS data.
        /// </summary>
        public static bool showMDSWarnings { get; set; } = false;

        /// <summary>
        /// Gets/Sets whether or not to show file extension warnings about invalid file extensions on extracted files.
        /// </summary>
        public static bool showFileExtensionWarnings { get; set; } = true;
    }
}
