
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class EmfluenceViewModel : ErrorViewModel
    {
        public EmfluenceSettings Settings { get; set; }

        [JsonPropertyName("success")]
        public int Success { get; set; }

        [JsonIgnore]
        public string Status { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceResponseViewModel<T> : EmfluenceViewModel where T : class
    {
        [JsonPropertyName("data")]
        public T Data { get; set; }

        [JsonPropertyName("errors")]
        public string[] Errors { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceListResponseViewModel<T> : EmfluenceViewModel where T : class
    {
        [JsonPropertyName("data")]
        public EmfluenceDataListViewModel<T> Data { get; set; }

        [JsonPropertyName("errors")]
        public string[] Errors { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceDataListViewModel<T> where T : class
    {
        [JsonPropertyName("records")]
        public IEnumerable<T> Records { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceContactLookupViewModel : EmfluenceViewModel
    {
        [JsonPropertyName("contactID")]
        public long ContactId { get; set; }

        [JsonPropertyName("email")]
        public string EmailAddress { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceContactViewModel : EmfluenceContactLookupViewModel
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("suppressed")]
        public int Suppressed { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceGroupLookupViewModel : EmfluenceViewModel
    {
        [JsonPropertyName("groupID")]
        public long GroupId { get; set; }

        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public class EmfluenceGroupViewModel : EmfluenceGroupLookupViewModel
    {
        [JsonPropertyName("activeMembers")]
        public int ActiveMembers { get; set; }

        [JsonPropertyName("totalMembers")]
        public int TotalMembers { get; set; }

        [JsonPropertyName("status")]
        public string GroupStatus { get; set; }

        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }
    }
}