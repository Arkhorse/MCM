using System.Globalization;
using System.Text.Json.Serialization;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace MCM.UI
{
    /// <summary>
    /// Defines the base definition for any mod that uses MCM
    /// This is also used as the base gameObject for the purposes of making it work
    /// </summary>
    [RegisterTypeInIl2Cpp(false)]
    public class MCMMod : MonoBehaviour
    {
        public MCMMod() { }
        public MCMMod(IntPtr pointer) : base(pointer) { }

        /// <summary>
        /// The human readable name used in the MCM
        /// </summary>
        [JsonInclude]
        public string? Name { get; set; }

        /// <summary>
        /// The name of the author
        /// </summary>
        [JsonInclude]
        public string? Author { get; set; }

        /// <summary>
        /// List of previous authors, used in the credits section of the MCM for the mod
        /// </summary>
        [JsonInclude]
        public List<string>? PreviousAuthors { get; set; }

        /// <summary>
        /// The current version of the mod
        /// </summary>
        [JsonInclude]
        public Version? CurrentVersion { get; set; }

        public class RemoteData
        {
            public class RemoteDataContainer
            {

            }

            MCMMod? AttachedMod { get; set; }
            /// <summary>
            /// Use this to set the fact that the mod has remote data
            /// </summary>
            bool UseRemoteData { get; set; } = false;
            /// <summary>
            /// This is the version set via a remote json. Used to check if the mod is updated
            /// </summary>
            Version? RemoteVersion { get; set; }

            /// <summary>
            /// The absolute static link to the remote json file. If this is not set but UseRemoteData is set to true, this will throw an error
            /// </summary>
            string? RemoteDataURI { get; set; }

            public RemoteData(MCMMod? mod, bool useRemoteData, Version? remoteVersion, string? remoteDataURI)
            {
                AttachedMod     = mod;
                UseRemoteData   = useRemoteData;
                RemoteVersion   = remoteVersion;
                RemoteDataURI   = remoteDataURI;
            }

            public bool Enabled()
            {
                return UseRemoteData;
            }

            public bool Verify()
            {
                if (UseRemoteData)
                {
                    if (string.IsNullOrEmpty(RemoteDataURI))
                    {
                        throw new RemoteDataException($"Remote data is enabled but there is no remote URI. Affected mod: {AttachedMod?.Name}");
                    }
                }

                return true;
            }
        }

        public void Awake()
        {

        }
    }

    //TODO: Move to dedicated file
    /// <summary>
    /// Exception to throw if the remote data is not correct
    /// </summary>
    public class RemoteDataException : Exception
    {
        /// <summary>
        /// Default method
        /// </summary>
        public RemoteDataException() { }
        /// <summary>
        /// Method with custom message
        /// </summary>
        /// <param name="message"></param>
        public RemoteDataException(string message) : base(message) { }
        /// <summary>
        /// method with custom message and inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public RemoteDataException(string message, Exception exception) : base(message, exception) { }
    }
}

/* DOCUMENTATION

*/
