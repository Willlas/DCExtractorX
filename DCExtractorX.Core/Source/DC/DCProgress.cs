using System;

namespace DC.IO
{
    /// <summary>
    /// Class   :   "DCProgress"
    /// 
    /// Purpose :   a bit overkill, but this static class allows the DC.IO functions to report their progress remotely
    /// without making the loading functions incredibly bloated. With this, we can report anywhere in any load function
    /// and not have to pass the progress up the chain, through a reference object, or with a obtuse wrapper.
    /// 
    /// This is a headless port of the original DCProgress: instead of writing directly to a DCBackgroundWorker (a
    /// WinForms-thread-bound type), it raises plain events and asks a Func&lt;bool&gt; whether the operation should be
    /// canceled. Hosts (CLI, GUI) subscribe to these events/assign CancelRequested however suits their environment;
    /// the property names (value/maximum/name/canceled) are unchanged so parser/exporter call sites did not need to change.
    /// </summary>
    public static class DCProgress
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static string m_szName;
        static int m_nValue = 0;
        static int m_nMaximum = 1;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Events.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Raised whenever the progress value changes.
        /// </summary>
        public static event Action<int> ValueChanged;

        /// <summary>
        /// Raised whenever the progress maximum changes.
        /// </summary>
        public static event Action<int> MaximumChanged;

        /// <summary>
        /// Raised whenever the progress phase name changes.
        /// </summary>
        public static event Action<string> NameChanged;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Properties.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Gets/Sets a callback that determines whether the current operation should be canceled. Defaults to "never canceled".
        /// </summary>
        public static Func<bool> CancelRequested { get; set; } = () => false;

        /// <summary>
        /// Gets/Sets the value of the progress bar.
        /// </summary>
        public static int value
        {
            get { return m_nValue; }
            set
            {
                m_nValue = value;
                ValueChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Gets/Sets the maximum of the progress bar.
        /// </summary>
        public static int maximum
        {
            get { return m_nMaximum; }
            set
            {
                m_nMaximum = value;
                MaximumChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Gets/Sets the name of the current phase of the operation.
        /// </summary>
        public static string name
        {
            get { return m_szName; }
            set
            {
                m_szName = value;
                NameChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Gets whether or not the current operation has been canceled.
        /// </summary>
        public static bool canceled
        {
            get { return CancelRequested(); }
        }
    }
}
