﻿using System.Text.Json.Serialization;

namespace Roses.SolarAPI.Models.FoxCloud
{
    public partial class SetWorkModeRequest : IFoxRequest
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; }

        [JsonPropertyName("values")]
        public object? Values { get; set; } 

        [JsonIgnore]
        public string RequestUri => SetWorkModeUri;

        public SetWorkModeRequest(string spaKey, string workMode)
        {
            if (string.IsNullOrWhiteSpace(spaKey))
            {
                throw new ArgumentOutOfRangeException(nameof(spaKey), "Please provide a spaKey.");
            }

            if (!SpaKeys.ALL.Any(key => key == spaKey))
            {
                throw new ArgumentOutOfRangeException(nameof(spaKey), "A valid SPA key has not been provided.");
            }

            if (!WorkModes.ALL.Any(mode => mode == workMode))
            {
                throw new ArgumentOutOfRangeException(nameof(workMode), "A valid work mode has not been provided.");
            }

            switch (spaKey)
            {
                case SpaKeys.H108:
                    Values = new Values108() { Mode = workMode };
                    break;
                default:
                    Values = new Values109() { Mode = workMode };
                    break;
            }

            // "h109__02__00"
            Key = $"{spaKey.Trim()}__02__00";
        }

        public void Validate()
        {
            if (Id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(SetWorkModeRequest), "No cloud device ID is provided.");
            }

            if (string.IsNullOrWhiteSpace(Key))
            {
                throw new ArgumentOutOfRangeException(nameof(SetWorkModeRequest), "Key has been changed.");
            }

            if (!WorkModes.ALL.Any(mode => mode == (Values as IValues)?.Mode))
            {
                throw new ArgumentOutOfRangeException(nameof(SetWorkModeRequest), "A valid work mode has not been provided.");
            }
        }

        private const string SetWorkModeUri = "https://www.foxesscloud.com/c/v0/device/setting/set";
    }

    public interface IValues
    {
        string? Mode { get; set; }
    }

    public partial class Values109 : IValues
    {
        [JsonPropertyName("h109__02__00")]
        public string? Mode { get; set; }
    }

    public partial class Values108 : IValues
    {
        [JsonPropertyName("h108__02__00")]
        public string? Mode { get; set; }
    }

    public class WorkModes
    {
        public const string FEED_IN = "Feedin";
        public const string SELF_USE = "SelfUse";
        public const string BACKUP = "Backup";

        public readonly static string[] ALL = new[] { FEED_IN, SELF_USE, BACKUP };
    }

    public class SpaKeys
    {
        public const string H109 = "h109";
        public const string H108 = "h108";

        public const string DEFAULT = H109;
        public readonly static string[] ALL = new[] { H109, H108 };
    }
}
