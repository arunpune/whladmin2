using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using WHLAdmin.Common.Extensions;
using WHLAdmin.Common.Models;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Extensions;

public static class ModelExtensions
{
    public static Dictionary<string, string> ToDictionary(this IEnumerable<CodeDescription> codeDescriptions)
    {
        var optionsList = new Dictionary<string, string>();
        if ((codeDescriptions?.Count() ?? 0) > 0)
        {
            foreach (var codeDescription in codeDescriptions)
            {
                optionsList.Add(codeDescription.Code, codeDescription.Description);
            }
        }
        return optionsList;
    }

    public static Dictionary<string, string> ToDropdownList(this IEnumerable<CodeDescription> codeDescriptions, string defaultKey = "", string defaultValue = "Select One")
    {
        var optionsList = new Dictionary<string, string>
        {
            { defaultKey, defaultValue }
        };
        if ((codeDescriptions?.Count() ?? 0) > 0)
        {
            foreach (var codeDescription in codeDescriptions)
            {
                optionsList.Add(codeDescription.Code, codeDescription.Description);
            }
        }
        return optionsList;
    }

    public static string ToFullName(this HousingApplication item)
    {
        if (item == null) return "";

        var fullName = (item.FirstName ?? "").Trim();
        if (!string.IsNullOrEmpty((item.MiddleName ?? "").Trim()))
        {
            fullName += (fullName.Length > 0 ? " " : "") + (item.MiddleName ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((item.LastName ?? "").Trim()))
        {
            fullName += (fullName.Length > 0 ? " " : "") + (item.LastName ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((item.Suffix ?? "").Trim()))
        {
            fullName += (fullName.Length > 0 ? " " : "") + (item.Suffix ?? "").Trim();
        }
        return fullName;
    }

    public static string ToAddressTextSingleLine(this Listing item)
    {
        if (item == null) return "";

        var addressText = (item.StreetLine1 ?? "").Trim();
        if (!string.IsNullOrEmpty((item.StreetLine2 ?? "").Trim()))
        {
            addressText += ", " + (item.StreetLine2 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((item.StreetLine3 ?? "").Trim()))
        {
            addressText += ", " + (item.StreetLine3 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(item.City))
        {
            addressText += ", " + (item.City ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(item.StateCd))
        {
            addressText += ", " + (item.StateCd ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(item.ZipCode))
        {
            addressText += " " + (item.ZipCode ?? "").Trim();
        }
        return addressText;
    }

    public static string ToAddressTextSingleLine(this HousingApplication model, bool fromMailingAddress = false)
    {
        if (model == null) return "";

        var addressText = "";
        if (fromMailingAddress)
        {
            addressText = (model.MailingStreetLine1 ?? "").Trim();
            if (!string.IsNullOrEmpty((model.MailingStreetLine2 ?? "").Trim()))
            {
                addressText += ", " + (model.MailingStreetLine2 ?? "").Trim();
            }
            if (!string.IsNullOrEmpty((model.MailingStreetLine3 ?? "").Trim()))
            {
                addressText += ", " + (model.MailingStreetLine3 ?? "").Trim();
            }
            if (!string.IsNullOrEmpty(model.MailingCity))
            {
                addressText += ", " + (model.MailingCity ?? "").Trim();
            }
            if (!string.IsNullOrEmpty(model.MailingStateCd))
            {
                addressText += ", " + (model.MailingStateCd ?? "").Trim();
            }
            if (!string.IsNullOrEmpty(model.MailingZipCode))
            {
                addressText += " " + (model.MailingZipCode ?? "").Trim();
            }
            return addressText;
        }

        addressText = (model.PhysicalStreetLine1 ?? "").Trim();
        if (!string.IsNullOrEmpty((model.PhysicalStreetLine2 ?? "").Trim()))
        {
            addressText += ", " + (model.PhysicalStreetLine2 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.PhysicalStreetLine3 ?? "").Trim()))
        {
            addressText += ", " + (model.PhysicalStreetLine3 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(model.PhysicalCity))
        {
            addressText += ", " + (model.PhysicalCity ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(model.PhysicalStateCd))
        {
            addressText += ", " + (model.PhysicalStateCd ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(model.PhysicalZipCode))
        {
            addressText += " " + (model.PhysicalZipCode ?? "").Trim();
        }
        return addressText;
    }

    public static string ToIdTypeValueDescription(this HousingApplication item)
    {
        if (item == null) return "";
        var idTypeDescription = (item.IdTypeDescription ?? "").Trim();
        var idTypeValue = (item.IdTypeValue ?? "").Trim();
        return string.IsNullOrEmpty(idTypeDescription) ? "" : $"{idTypeDescription} - {idTypeValue}".Trim();
    }

    public static string ToPhoneNumberDisplayText(this string item)
    {
        var number = (item ?? "").Trim();
        if (number.Length == 10)
        {
            return $"({number[..3]}) {number.Substring(3, 3)}-{number[6..]}";
        }
        return "";
    }

    public static string ToOtherAndValueText(this string code, string description, string otherValue = null, string defaultValue = "")
    {
        code = (code ?? "").Trim();
        description = (description ?? "").Trim();
        otherValue = (otherValue ?? "").Trim();
        defaultValue = (defaultValue ?? "").Trim();
        if (!string.IsNullOrEmpty(code))
        {
            if (code.Equals("OTHER", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(otherValue))
            {
                return $"Other: {otherValue}";
            }
            return description.Length > 0 ? description : code;
        }
        return defaultValue;
    }

    public static string ToDateTimeDisplayText(this DateTime? dateTime, string dateFormat = null, string timeFormat = null)
    {
        if (dateTime.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue) return "";

        var displayText = "";

        dateFormat = (dateFormat ?? "").Trim();
        if (!string.IsNullOrEmpty(dateFormat))
        {
            displayText += dateTime.Value.ToString(dateFormat);
        }

        timeFormat = (timeFormat ?? "").Trim();
        if (!string.IsNullOrEmpty(timeFormat))
        {
            displayText += (displayText.Length > 0 ? " " : "") + dateTime.Value.ToString(timeFormat);
        }

        return displayText;
    }

    public static string ToDateEditorFormat(this DateTime? date, string format = "yyyy-MM-dd")
    {
        return date.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? date.Value.Date.ToString(format) : "";
    }

    public static string ToTimeEditorFormat(this DateTime? time, string format = "HH:mm:ss")
    {
        return time.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? time.Value.ToString(format) : "";
    }

    public static long ToAmiAmount(this long inputAmt, int inputPercentage, int roundToNearest = 50)
    {
        if (inputAmt > 0 && inputPercentage > 0)
        {
            var amiAmount = Math.Truncate((decimal)inputAmt * (decimal)inputPercentage / 100);
            if (roundToNearest > 0)
            {
                return (long)(Math.Round(amiAmount / roundToNearest, 0) * roundToNearest);
            }
            return (long)Math.Truncate(amiAmount);
        }

        return 0;
    }

    public static AmiConfigViewModel ToViewModel(this AmiConfig item)
    {
        if (item == null) return null;

        var hhPctAmts = new List<AmiHhPctAmt>
        {
            new() { Size = 1, Pct = item.Hh1, Amt = item.IncomeAmt.ToAmiAmount(item.Hh1) },
            new() { Size = 2, Pct = item.Hh2, Amt = item.IncomeAmt.ToAmiAmount(item.Hh2) },
            new() { Size = 3, Pct = item.Hh3, Amt = item.IncomeAmt.ToAmiAmount(item.Hh3) },
            new() { Size = 4, Pct = item.Hh4, Amt = item.IncomeAmt.ToAmiAmount(item.Hh4) },
            new() { Size = 5, Pct = item.Hh5, Amt = item.IncomeAmt.ToAmiAmount(item.Hh5) },
            new() { Size = 6, Pct = item.Hh6, Amt = item.IncomeAmt.ToAmiAmount(item.Hh6) },
            new() { Size = 7, Pct = item.Hh7, Amt = item.IncomeAmt.ToAmiAmount(item.Hh7) },
            new() { Size = 8, Pct = item.Hh8, Amt = item.IncomeAmt.ToAmiAmount(item.Hh8) },
            new() { Size = 9, Pct = item.Hh9, Amt = item.IncomeAmt.ToAmiAmount(item.Hh9) },
            new() { Size = 10, Pct = item.Hh10, Amt = item.IncomeAmt.ToAmiAmount(item.Hh10) }
        };

        return new AmiConfigViewModel()
        {
            Active = item.Active,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            EffectiveDate = item.EffectiveDate,
            EffectiveYear = item.EffectiveYear,
            Hh1 = item.Hh1,
            Hh2 = item.Hh2,
            Hh3 = item.Hh3,
            Hh4 = item.Hh4,
            Hh5 = item.Hh5,
            Hh6 = item.Hh6,
            Hh7 = item.Hh7,
            Hh8 = item.Hh8,
            Hh9 = item.Hh9,
            Hh10 = item.Hh10,
            HhPctAmts = hhPctAmts,
            IncomeAmt = item.IncomeAmt,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate
        };
    }

    public static EditableAmiConfigViewModel ToEditableViewModel(this AmiConfig item)
    {
        if (item == null) return null;

        var hhPctAmts = new List<AmiHhPctAmt>
        {
            new() { Size = 1, Pct = item.Hh1, Amt = item.IncomeAmt.ToAmiAmount(item.Hh1) },
            new() { Size = 2, Pct = item.Hh2, Amt = item.IncomeAmt.ToAmiAmount(item.Hh2) },
            new() { Size = 3, Pct = item.Hh3, Amt = item.IncomeAmt.ToAmiAmount(item.Hh3) },
            new() { Size = 4, Pct = item.Hh4, Amt = item.IncomeAmt.ToAmiAmount(item.Hh4) },
            new() { Size = 5, Pct = item.Hh5, Amt = item.IncomeAmt.ToAmiAmount(item.Hh5) },
            new() { Size = 6, Pct = item.Hh6, Amt = item.IncomeAmt.ToAmiAmount(item.Hh6) },
            new() { Size = 7, Pct = item.Hh7, Amt = item.IncomeAmt.ToAmiAmount(item.Hh7) },
            new() { Size = 8, Pct = item.Hh8, Amt = item.IncomeAmt.ToAmiAmount(item.Hh8) },
            new() { Size = 9, Pct = item.Hh9, Amt = item.IncomeAmt.ToAmiAmount(item.Hh9) },
            new() { Size = 10, Pct = item.Hh10, Amt = item.IncomeAmt.ToAmiAmount(item.Hh10) }
        };

        return new EditableAmiConfigViewModel()
        {
            EffectiveDate = item.EffectiveDate.ToString().Insert(6, "-").Insert(4, "-"),
            EffectiveYear = item.EffectiveYear,
            IncomeAmt = item.IncomeAmt,
            HhPctAmts = hhPctAmts,
            Active = item.Active,
            Mode = "EDIT"
        };
    }

    public static AmortizationViewModel ToViewModel(this Amortization item)
    {
        if (item == null) return null;

        return new AmortizationViewModel()
        {
            Active = item.Active,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            Rate = item.Rate,
            RateInterestOnly = item.RateInterestOnly,
            Rate10Year = item.Rate10Year,
            Rate15Year = item.Rate15Year,
            Rate20Year = item.Rate20Year,
            Rate25Year = item.Rate25Year,
            Rate30Year = item.Rate30Year,
            Rate40Year = item.Rate40Year
        };
    }

    public static EditableAmortizationViewModel ToEditableViewModel(this Amortization item)
    {
        if (item == null) return null;

        return new EditableAmortizationViewModel()
        {
            Rate = item.Rate,
            RateInterestOnly = item.RateInterestOnly,
            Rate10Year = item.Rate10Year,
            Rate15Year = item.Rate15Year,
            Rate20Year = item.Rate20Year,
            Rate25Year = item.Rate25Year,
            Rate30Year = item.Rate30Year,
            Rate40Year = item.Rate40Year,
            Active = item.Active,
            Mode = "EDIT"
        };
    }

    public static AuditViewModel ToViewModel(this AuditEntry item)
    {
        if (item == null) return null;

        return new AuditViewModel()
        {
            Id = item.Id,
            ActionCd = item.ActionCd,
            ActionDescription = item.ActionDescription,
            EntityDescription = item.EntityDescription,
            EntityId = item.EntityId,
            EntityName = item.EntityName,
            EntityTypeCd = item.EntityTypeCd,
            Note = item.Note,
            Timestamp = item.Timestamp,
            Username = item.Username,
        };
    }

    public static NoteViewModel ToViewModel(this Note item)
    {
        if (item == null) return null;

        return new NoteViewModel()
        {
            Id = item.Id,
            EntityDescription = item.EntityDescription,
            EntityId = item.EntityId,
            EntityName = item.EntityName,
            EntityTypeCd = item.EntityTypeCd,
            Note = item.Note,
            Timestamp = item.Timestamp,
            Username = item.Username,
        };
    }

    public static AmenityViewModel ToViewModel(this Amenity item)
    {
        if (item == null) return null;

        return new AmenityViewModel()
        {
            Active = item.Active,
            Description = item.Description,
            AmenityId = item.AmenityId,
            Name = item.Name,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            UsageCount = item.UsageCount
        };
    }

    public static EditableAmenityViewModel ToEditableViewModel(this Amenity item)
    {
        if (item == null) return null;

        return new EditableAmenityViewModel()
        {
            AmenityId = item.AmenityId,
            AmenityName = item.Name,
            AmenityDescription = item.Description,
            Active = item.Active
        };
    }

    public static DocumentTypeViewModel ToViewModel(this DocumentType item)
    {
        if (item == null) return null;

        return new DocumentTypeViewModel()
        {
            Active = item.Active,
            Description = item.Description,
            DocumentTypeId = item.DocumentTypeId,
            Name = item.Name,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            UsageCount = item.UsageCount
        };
    }

    public static EditableDocumentTypeViewModel ToEditableViewModel(this DocumentType item)
    {
        if (item == null) return null;

        return new EditableDocumentTypeViewModel()
        {
            DocumentTypeId = item.DocumentTypeId,
            DocumentTypeName = item.Name,
            DocumentTypeDescription = item.Description,
            Active = item.Active
        };
    }

    public static FaqConfigViewModel ToViewModel(this FaqConfig item)
    {
        if (item == null) return null;

        return new FaqConfigViewModel()
        {
            Active = item.Active,
            CategoryName = item.CategoryName,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DisplayOrder = item.DisplayOrder,
            FaqId = item.FaqId,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            Text = item.Text,
            Title = item.Title,
            Url = item.Url,
            Url1 = item.Url1,
            Url2 = item.Url2,
            Url3 = item.Url3,
            Url4 = item.Url4,
            Url5 = item.Url5,
            Url6 = item.Url6,
            Url7 = item.Url7,
            Url8 = item.Url8,
            Url9 = item.Url9
        };
    }

    public static EditableFaqConfigViewModel ToEditableViewModel(this FaqConfig item)
    {
        if (item == null) return null;

        return new EditableFaqConfigViewModel()
        {
            FaqId = item.FaqId,
            CategoryName = item.CategoryName,
            Title = item.Title,
            Text = item.Text,
            Url = item.Url,
            Url1 = item.Url1,
            Url2 = item.Url2,
            Url3 = item.Url3,
            Url4 = item.Url4,
            Url5 = item.Url5,
            Url6 = item.Url6,
            Url7 = item.Url7,
            Url8 = item.Url8,
            Url9 = item.Url9,
            DisplayOrder = item.DisplayOrder,
            Active = item.Active
        };
    }

    public static FundingSourceViewModel ToViewModel(this FundingSource item)
    {
        if (item == null) return null;

        return new FundingSourceViewModel()
        {
            Active = item.Active,
            Description = item.Description,
            FundingSourceId = item.FundingSourceId,
            Name = item.Name,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            UsageCount = item.UsageCount
        };
    }

    public static EditableFundingSourceViewModel ToEditableViewModel(this FundingSource item)
    {
        if (item == null) return null;

        return new EditableFundingSourceViewModel()
        {
            FundingSourceId = item.FundingSourceId,
            FundingSourceName = item.Name,
            FundingSourceDescription = item.Description,
            Active = item.Active
        };
    }

    public static MarketingAgentViewModel ToViewModel(this MarketingAgent item)
    {
        if (item == null) return null;

        return new MarketingAgentViewModel()
        {
            Active = item.Active,
            AgentId = item.AgentId,
            ContactName = item.ContactName,
            EmailAddress = item.EmailAddress,
            Name = item.Name,
            PhoneNumber = item.PhoneNumber,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            UsageCount = item.UsageCount
        };
    }

    public static EditableMarketingAgentViewModel ToEditableViewModel(this MarketingAgent item)
    {
        if (item == null) return null;

        return new EditableMarketingAgentViewModel()
        {
            AgentId = item.AgentId,
            AgentName = item.Name,
            ContactName = item.ContactName,
            PhoneNumber = item.PhoneNumber,
            EmailAddress = item.EmailAddress,
            Active = item.Active
        };
    }

    public static NotificationConfigViewModel ToViewModel(this NotificationConfig item)
    {
        if (item == null) return null;

        return new NotificationConfigViewModel()
        {
            Active = item.Active,
            CategoryCd = item.CategoryCd,
            CategoryDescription = item.CategoryDescription,
            FrequencyCd = item.FrequencyCd,
            FrequencyDescription = item.FrequencyDescription,
            FrequencyInterval = item.FrequencyInterval,
            NotificationId = item.NotificationId,
            NotificationList = item.NotificationList,
            Text = item.Text,
            Title = item.Title,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
        };
    }

    public static EditableNotificationConfigViewModel ToEditableViewModel(this NotificationConfig item)
    {
        if (item == null) return null;

        return new EditableNotificationConfigViewModel()
        {
            NotificationId = item.NotificationId,
            Title = item.Title,
            Text = item.Text,
            CategoryCd = item.CategoryCd,
            FrequencyInterval = item.FrequencyInterval,
            FrequencyCd = item.FrequencyCd,
            NotificationList = item.NotificationList,
            Active = item.Active
        };
    }

    public static QuoteConfigViewModel ToViewModel(this QuoteConfig item)
    {
        if (item == null) return null;

        return new QuoteConfigViewModel()
        {
            Active = item.Active,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DisplayOnHomePageInd = item.DisplayOnHomePageInd,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            Text = item.Text,
            QuoteId = item.QuoteId
        };
    }

    public static EditableQuoteConfigViewModel ToEditableViewModel(this QuoteConfig item)
    {
        if (item == null) return null;

        return new EditableQuoteConfigViewModel()
        {
            QuoteId = item.QuoteId,
            Text = item.Text,
            DisplayOnHomePageInd = item.DisplayOnHomePageInd,
            Active = item.Active
        };
    }

    public static ResourceConfigViewModel ToViewModel(this ResourceConfig item)
    {
        if (item == null) return null;

        return new ResourceConfigViewModel()
        {
            Active = item.Active,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DisplayOrder = item.DisplayOrder,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            ResourceId = item.ResourceId,
            Text = item.Text,
            Title = item.Title,
            Url = item.Url
        };
    }

    public static EditableResourceConfigViewModel ToEditableViewModel(this ResourceConfig item)
    {
        if (item == null) return null;

        return new EditableResourceConfigViewModel()
        {
            ResourceId = item.ResourceId,
            Title = item.Title,
            Text = item.Text,
            Url = item.Url,
            DisplayOrder = item.DisplayOrder,
            Active = item.Active
        };
    }

    public static UserViewModel ToViewModel(this User item)
    {
        if (item == null) return null;

        return new UserViewModel()
        {
            Active = item.Active,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DisplayName = item.DisplayName,
            EmailAddress = item.EmailAddress,
            LastLoggedIn = item.LastLoggedIn,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            OrganizationCd = item.OrganizationCd,
            OrganizationDescription = item.OrganizationDescription,
            RoleCd = item.RoleCd,
            RoleDescription = item.RoleDescription,
            UserId = item.UserId
        };
    }

    public static EditableUserViewModel ToEditableViewModel(this User item)
    {
        if (item == null) return null;

        return new EditableUserViewModel()
        {
            DisplayName = item.DisplayName,
            EmailAddress = item.EmailAddress,
            OrganizationCd = item.OrganizationCd,
            RoleCd = item.RoleCd,
            UserId = item.UserId
        };
    }

    public static VideoConfigViewModel ToViewModel(this VideoConfig item)
    {
        if (item == null) return null;

        return new VideoConfigViewModel()
        {
            Active = item.Active,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DisplayOrder = item.DisplayOrder,
            DisplayOnHomePageInd = item.DisplayOnHomePageInd,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            Text = item.Text,
            Title = item.Title,
            Url = item.Url,
            VideoId = item.VideoId
        };
    }

    public static EditableVideoConfigViewModel ToEditableViewModel(this VideoConfig item)
    {
        if (item == null) return null;

        return new EditableVideoConfigViewModel()
        {
            VideoId = item.VideoId,
            Title = item.Title,
            Text = item.Text,
            Url = item.Url,
            DisplayOrder = item.DisplayOrder,
            DisplayOnHomePageInd = item.DisplayOnHomePageInd,
            Active = item.Active
        };
    }

    public static HousingApplicationViewModel ToViewModel(this HousingApplication item)
    {
        if (item == null) return null;

        return new HousingApplicationViewModel()
        {
            Active = item.Active,
            ApplicationId = item.ApplicationId,
            ListingId = item.ListingId,

            AddressInd = item.AddressInd,
            AltPhoneNumber = item.AltPhoneNumber,
            AltPhoneNumberExtn = item.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = item.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = item.AltPhoneNumberTypeDescription,
            AssetValueAmt = item.AssetValueAmt,
            CountyLivingIn = item.CountyLivingIn,
            CountyWorkingIn = item.CountyWorkingIn,
            CurrentlyWorkingInWestchesterInd = item.CurrentlyWorkingInWestchesterInd,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DateOfBirth = item.DateOfBirth,
            DifferentMailingAddressInd = item.DifferentMailingAddressInd,
            DisabilityInd = item.DisabilityInd,
            DisqualificationCd = item.DisqualificationCd,
            DisqualificationDescription = item.DisqualificationDescription,
            DisqualificationOther = item.DisqualificationOther,
            DisqualificationReason = item.DisqualificationReason,
            DisqualifiedInd = item.DisqualifiedInd,
            DuplicateCheckCd = item.DuplicateCheckCd,
            DuplicateCheckResponseDueDate = item.DuplicateCheckResponseDueDate,
            EmailAddress = item.EmailAddress,
            EthnicityCd = item.EthnicityCd,
            EthnicityDescription = item.EthnicityDescription,
            EverLivedInWestchesterInd = item.EverLivedInWestchesterInd,
            FirstName = item.FirstName,
            GenderCd = item.GenderCd,
            GenderDescription = item.GenderDescription,
            IdIssueDate = item.IdIssueDate,
            IdTypeCd = item.IdTypeCd,
            IdTypeDescription = item.IdTypeDescription,
            IdTypeValue = item.IdTypeValue,
            IncomeValueAmt = item.IncomeValueAmt,
            Last4SSN = item.Last4SSN,
            LastName = item.LastName,
            LotteryDate = item.LotteryDate,
            LotteryId = item.LotteryId,
            LotteryNumber = item.LotteryNumber,
            MailingCity = item.MailingCity,
            MailingCounty = item.MailingCounty,
            MailingStateCd = item.MailingStateCd,
            MailingStreetLine1 = item.MailingStreetLine1,
            MailingStreetLine2 = item.MailingStreetLine2,
            MailingStreetLine3 = item.MailingStreetLine3,
            MailingZipCode = item.MailingZipCode,
            MiddleName = item.MiddleName,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            OwnRealEstateInd = item.OwnRealEstateInd,
            PhoneNumber = item.PhoneNumber,
            PhoneNumberExtn = item.PhoneNumberExtn,
            PhoneNumberTypeCd = item.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = item.PhoneNumberTypeDescription,
            PhysicalCity = item.PhysicalCity,
            PhysicalCounty = item.PhysicalCounty,
            PhysicalStateCd = item.PhysicalStateCd,
            PhysicalStreetLine1 = item.PhysicalStreetLine1,
            PhysicalStreetLine2 = item.PhysicalStreetLine2,
            PhysicalStreetLine3 = item.PhysicalStreetLine3,
            PhysicalZipCode = item.PhysicalZipCode,
            RaceCd = item.RaceCd,
            RaceDescription = item.RaceDescription,
            RealEstateValueAmt = item.RealEstateValueAmt,
            ReceivedDate = item.ReceivedDate,
            StatusCd = item.StatusCd,
            StatusDescription = item.StatusDescription,
            StudentInd = item.StudentInd,
            SubmissionTypeCd = item.SubmissionTypeCd,
            SubmissionTypeDescription = (item.SubmissionTypeCd ?? "").Trim().Equals("PAPER", StringComparison.CurrentCultureIgnoreCase) ? "Paper" : "Online",
            SubmittedDate = item.SubmittedDate,
            Suffix = item.Suffix,
            Title = item.Title,
            VeteranInd = item.VeteranInd,
            WithdrawnDate = item.WithdrawnDate,

            UnitTypeCds = item.UnitTypeCds,
            CoApplicantMemberId = item.CoApplicantMemberId,
            MemberIds = item.MemberIds,
            AccountIds = item.AccountIds,
            AccessibilityCds = item.AccessibilityCds,

            DisplayName = item.ToFullName(),
            DisplayPhysicalAddress = item.ToAddressTextSingleLine(),
            DisplayMailingAddress = item.ToAddressTextSingleLine(true),
            DisplayIdTypeValue = item.ToIdTypeValueDescription(),
            DisplayPhoneNumber = item.PhoneNumber.ToPhoneNumberDisplayText(),
            DisplayAltPhoneNumber = item.AltPhoneNumber.ToPhoneNumberDisplayText(),
            DisplayLeadType = item.LeadTypeCd.ToOtherAndValueText(item.LeadTypeDescription, item.LeadTypeOther, "Not Specified")
        };
    }

    public static EditableHousingApplicationViewModel ToEditableViewModel(this HousingApplication item)
    {
        if (item == null) return null;

        return new EditableHousingApplicationViewModel()
        {
            ApplicationId = item.ApplicationId,
            ListingId = item.ListingId,

            Title = item.Title,
            FirstName = item.FirstName,
            MiddleName = string.IsNullOrEmpty(item.MiddleName ?? "") ? null : item.MiddleName,
            LastName = item.LastName,
            Suffix = string.IsNullOrEmpty(item.Suffix ?? "") ? null : item.Suffix,
            Last4SSN = item.Last4SSN,
            DateOfBirth = item.DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? item.DateOfBirth.Value.ToString("yyyy-MM-dd") : null,
            IdTypeCd = item.IdTypeCd,
            IdTypeDescription = item.IdTypeDescription,
            IdTypeValue = item.IdTypeValue,
            IdIssueDate = item.IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? item.IdIssueDate.Value.ToString("yyyy-MM-dd") : null,
            GenderCd = item.GenderCd,
            GenderDescription = item.GenderDescription,
            RaceCd = item.RaceCd,
            RaceDescription = item.RaceDescription,
            EthnicityCd = item.EthnicityCd,
            EthnicityDescription = item.EthnicityDescription,
            Pronouns = string.IsNullOrEmpty(item.Pronouns ?? "") ? null : item.Pronouns,
            CountyLivingIn = string.IsNullOrEmpty(item.CountyLivingIn ?? "") ? null : item.CountyLivingIn,
            CountyWorkingIn = string.IsNullOrEmpty(item.CountyWorkingIn ?? "") ? null : item.CountyWorkingIn,
            StudentInd = item.StudentInd,
            DisabilityInd = item.DisabilityInd,
            VeteranInd = item.VeteranInd,
            EverLivedInWestchesterInd = item.EverLivedInWestchesterInd,
            CurrentlyWorkingInWestchesterInd = item.CurrentlyWorkingInWestchesterInd,
            PhoneNumberTypeCd = item.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = item.PhoneNumberTypeDescription,
            PhoneNumber = item.PhoneNumber,
            PhoneNumberExtn = item.PhoneNumberExtn,
            AltPhoneNumberTypeCd = item.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = item.AltPhoneNumberTypeDescription,
            AltPhoneNumber = item.AltPhoneNumber,
            AltPhoneNumberExtn = item.AltPhoneNumberExtn,
            EmailAddress = item.EmailAddress,
            AltEmailAddress = item.AltEmailAddress,
            OwnRealEstateInd = item.OwnRealEstateInd,
            RealEstateValueAmt = item.RealEstateValueAmt,
            AssetValueAmt = item.AssetValueAmt,
            IncomeValueAmt = item.IncomeValueAmt,

            AddressInd = item.AddressInd,
            PhysicalStreetLine1 = item.PhysicalStreetLine1,
            PhysicalStreetLine2 = item.PhysicalStreetLine2,
            PhysicalStreetLine3 = item.PhysicalStreetLine3,
            PhysicalCity = item.PhysicalCity,
            PhysicalStateCd = item.PhysicalStateCd,
            PhysicalZipCode = item.PhysicalZipCode,
            PhysicalCounty = item.PhysicalCounty,
            DifferentMailingAddressInd = item.DifferentMailingAddressInd,
            MailingStreetLine1 = item.MailingStreetLine1,
            MailingStreetLine2 = item.MailingStreetLine2,
            MailingStreetLine3 = item.MailingStreetLine3,
            MailingCity = item.MailingCity,
            MailingStateCd = item.MailingStateCd,
            MailingZipCode = item.MailingZipCode,
            MailingCounty = item.MailingCounty,

            LeadTypeCd = item.LeadTypeCd,
            LeadTypeDescription = item.LeadTypeDescription,
            LeadTypeOther = item.LeadTypeOther,

            StatusCd = item.StatusCd,
            StatusDescription = item.StatusDescription,

            SubmissionTypeCd = item.SubmissionTypeCd,
            SubmissionTypeDescription = item.SubmissionTypeDescription,
            SubmittedDate = item.SubmittedDate,
        };
    }

    public static string SiteUrl(this HttpRequest request)
    {
        if (request == null) return null;

        try
        {
            var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static DuplicateViewModel ToViewModel(this DuplicateApplication item)
    {
        if (item == null) return null;

        return new DuplicateViewModel()
        {
            Count = item.Count,
            DateOfBirth = item.DateOfBirth,
            EmailAddress = item.EmailAddress,
            Last4SSN = item.Last4SSN,
            Name = item.Name,
            PhoneNumber = item.PhoneNumber,
            StreetAddress = item.StreetAddress
        };
    }

    public static LotteryViewModel ToViewModel(this Lottery item)
    {
        if (item == null) return null;

        return new LotteryViewModel()
        {
            ListingId = item.ListingId,
            LotteryId = item.LotteryId,
            ManualInd = item.ManualInd,
            RunBy = item.RunBy,
            RunDate = item.RunDate,
            LotteryStatusCd = item.LotteryStatusCd,
            LotteryStatusDescription = item.LotteryStatusDescription
        };
    }

    public static ListingViewModel ToViewModel(this Listing item)
    {
        if (item == null) return null;

        return new ListingViewModel()
        {
            Active = item.Active,
            ApplicationStartDate = item.ApplicationStartDate,
            ApplicationEndDate = item.ApplicationEndDate,
            City = item.City,
            CompletedOrInitialOccupancyYear = item.CompletedOrInitialOccupancyYear,
            County = item.County,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            Description = item.Description,
            EsriX = item.EsriX,
            EsriY = item.EsriY,
            ListingAgeTypeCd = item.ListingAgeTypeCd,
            ListingAgeTypeDescription = item.ListingAgeTypeDescription,
            ListingEndDate = item.ListingEndDate,
            ListingId = item.ListingId,
            ListingStartDate = item.ListingStartDate,
            ListingTypeCd = item.ListingTypeCd,
            ListingTypeDescription = item.ResaleInd ? "Resale" : item.ListingTypeDescription,
            LotteryDate = item.LotteryDate,
            LotteryEligible = item.LotteryEligible,
            LotteryId = item.LotteryId,
            MapUrl = item.MapUrl,
            MarketingAgentApplicationLink = item.MarketingAgentApplicationLink,
            MarketingAgentId = item.MarketingAgentId,
            MarketingAgentInd = item.MarketingAgentInd,
            MarketingAgentName = item.MarketingAgentName,
            MaxHouseholdSize = item.MaxHouseholdSize,
            MaxHouseholdIncomeAmt = item.MaxHouseholdIncomeAmt,
            MinHouseholdSize = item.MinHouseholdSize,
            MinHouseholdIncomeAmt = item.MinHouseholdIncomeAmt,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            Municipality = item.Municipality,
            MunicipalityUrl = item.MunicipalityUrl,
            Name = item.Name,
            PetsAllowedInd = item.PetsAllowedInd,
            PetsAllowedText = item.PetsAllowedText,
            PublishedVersionNo = item.PublishedVersionNo,
            RentIncludesText = item.RentIncludesText,
            ResaleInd = item.ResaleInd,
            SchoolDistrict = item.SchoolDistrict,
            SchoolDistrictUrl = item.SchoolDistrictUrl,
            StateCd = item.StateCd,
            StatusCd = item.StatusCd,
            StatusDescription = item.StatusDescription,
            StreetLine1 = item.StreetLine1,
            StreetLine2 = item.StreetLine2,
            StreetLine3 = item.StreetLine3,
            TermOfAffordability = item.TermOfAffordability,
            VersionNo = item.VersionNo,
            WaitlistEligible = item.WaitlistEligible,
            WaitlistEndDate = item.WaitlistEndDate,
            WaitlistStartDate = item.WaitlistStartDate,
            WebsiteUrl = item.WebsiteUrl,
            ZipCode = item.ZipCode,
            DisplayAddress = item.ToAddressTextSingleLine()
        };
    }

    public static HousingApplicantViewModel ToViewModel(this HousingApplicant item)
    {
        if (item == null) return null;

        return new HousingApplicantViewModel()
        {
            AccountCount = 0,
            Active = item.Active,
            AltEmailAddress = item.AltEmailAddress,
            AltPhoneNumber = item.AltPhoneNumber,
            AltPhoneNumberExtn = item.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = item.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = item.AltPhoneNumberTypeDescription,
            ApplicantId = item.ApplicantId,
            ApplicationId = item.ApplicationId,
            AssetValueAmt = item.AssetValueAmt,
            CoApplicantInd = item.CoApplicantInd,
            CountyLivingIn = item.CountyLivingIn,
            CountyWorkingIn = item.CountyWorkingIn,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            CurrentlyWorkingInWestchesterInd = item.CurrentlyWorkingInWestchesterInd,
            DateOfBirth = item.DateOfBirth,
            DisabilityInd = item.DisabilityInd,
            DisplayName = item.ToApplicantName(),
            EmailAddress = item.EmailAddress,
            EthnicityCd = item.EthnicityCd,
            EthnicityDescription = item.EthnicityDescription,
            EverLivedInWestchesterInd = item.EverLivedInWestchesterInd,
            FirstName = item.FirstName,
            GenderCd = item.GenderCd,
            GenderDescription = item.GenderDescription,
            IdIssueDate = item.IdIssueDate,
            IdTypeCd = item.IdTypeCd,
            IdTypeDescription = item.IdTypeDescription,
            IdTypeValue = item.IdTypeValue,
            IncomeValueAmt = item.IncomeValueAmt,
            LanguagePreferenceCd = item.LanguagePreferenceCd,
            LanguagePreferenceDescription = item.LanguagePreferenceDescription,
            LanguagePreferenceOther = item.LanguagePreferenceOther,
            Last4SSN = item.Last4SSN,
            LastName = item.LastName,
            ListingPreferenceCd = item.ListingPreferenceCd,
            ListingPreferenceDescription = item.ListingPreferenceDescription,
            MemberId = item.MemberId,
            MiddleName = item.MiddleName,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            OwnRealEstateInd = item.OwnRealEstateInd,
            PhoneNumber = item.PhoneNumber,
            PhoneNumberExtn = item.PhoneNumberExtn,
            PhoneNumberTypeCd = item.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = item.PhoneNumberTypeDescription,
            Pronouns = item.Pronouns,
            RaceCd = item.RaceCd,
            RaceDescription = item.RaceDescription,
            RealEstateValueAmt = item.RealEstateValueAmt,
            RelationTypeCd = item.RelationTypeCd,
            RelationTypeDescription = item.RelationTypeDescription,
            RelationTypeOther = item.RelationTypeOther,
            SmsNotificationsPreferenceInd = item.SmsNotificationsPreferenceInd,
            StudentInd = item.StudentInd,
            Suffix = item.Suffix,
            Title = item.Title,
            Username = item.Username,
            VeteranInd = item.VeteranInd,
        };
    }

    public static HousingApplicantAssetViewModel ToViewModel(this HousingApplicantAsset item)
    {
        if (item == null) return null;

        return new HousingApplicantAssetViewModel()
        {
            AccountId = item.AccountId,
            AccountNumber = item.AccountNumber,
            AccountTypeCd = item.AccountTypeCd,
            AccountTypeDescription = item.AccountTypeDescription,
            AccountTypeOther = item.AccountTypeOther,
            AccountValueAmt = item.AccountValueAmt,
            Active = item.Active,
            ApplicantId = item.ApplicantId,
            ApplicationId = item.ApplicationId,
            CreatedBy = item.CreatedBy,
            CreatedDate = item.CreatedDate,
            DisplayAccountType = item.AccountTypeCd.ToOtherAndValueText(item.AccountTypeDescription, item.AccountTypeOther),
            HouseholdId = item.HouseholdId,
            InstitutionName = item.InstitutionName,
            ModifiedBy = item.ModifiedBy,
            ModifiedDate = item.ModifiedDate,
            PrimaryHolderMemberId = item.PrimaryHolderMemberId,
            PrimaryHolderMemberName = item.PrimaryHolderMemberName
        };
    }

    public static ApplicationCommentViewModel ToViewModel(this ApplicationComment comment)
    {
        if (comment == null) return null;

        return new ApplicationCommentViewModel()
        {
            Active = comment.Active,
            ApplicationId = comment.ApplicationId,
            CommentId = comment.CommentId,
            Comments = comment.Comments,
            CreatedBy = comment.CreatedBy,
            CreatedDate = comment.CreatedDate,
            InternalOnlyInd = comment.InternalOnlyInd,
            ModifiedBy = comment.ModifiedBy,
            ModifiedDate = comment.ModifiedDate,
            Username = comment.Username
        };
    }

    public static string ToCsvFileContents(this DataTable dataTable)
    {
        if (dataTable?.Columns?.Count == 0) return "";

        var sb = new StringBuilder();

        // Headers
        foreach (var col in dataTable.Columns)
        {
            if (col == null)
                sb.Append(",");
            else
                sb.Append("\"" + col.ToString().Replace("\"", "\"\"") + "\",");
        }
        sb.Replace(",", Environment.NewLine, sb.Length - 1, 1);

        // Data
        foreach (DataRow dr in dataTable.Rows)
        {
            foreach (var column in dr.ItemArray)
            {
                if (column == null)
                    sb.Append(",");
                else
                    sb.Append("\"" + column.ToString().Replace("\"", "\"\"") + "\",");
            }
            sb.Replace(",", System.Environment.NewLine, sb.Length - 1, 1);
        }
        return sb.ToString();
    }
}