using System;
using Microsoft.AspNetCore.Http;
using WHLSite.Common.Models;
using WHLSite.ViewModels;

namespace WHLSite.Extensions;

public static class ModelHelpers
{
    public static FaqViewModel ToViewModel(this FaqConfig faq)
    {
        if (faq == null) return null;

        return new FaqViewModel()
        {
            Active = faq.Active,
            CategoryName = faq.CategoryName,
            CreatedBy = faq.CreatedBy,
            CreatedDate = faq.CreatedDate,
            DisplayOrder = faq.DisplayOrder,
            FaqId = faq.FaqId,
            ModifiedBy = faq.ModifiedBy,
            ModifiedDate = faq.ModifiedDate,
            Text = faq.Text,
            Title = faq.Title,
            Url = faq.Url,
            Url1 = faq.Url1,
            Url2 = faq.Url2,
            Url3 = faq.Url3,
            Url4 = faq.Url4,
            Url5 = faq.Url5,
            Url6 = faq.Url6,
            Url7 = faq.Url7,
            Url8 = faq.Url8,
            Url9 = faq.Url9
        };
    }

    public static QuoteViewModel ToViewModel(this QuoteConfig quote)
    {
        if (quote == null) return null;

        return new QuoteViewModel()
        {
            Active = quote.Active,
            CreatedBy = quote.CreatedBy,
            CreatedDate = quote.CreatedDate,
            DisplayOnHomePageInd = quote.DisplayOnHomePageInd,
            ModifiedBy = quote.ModifiedBy,
            ModifiedDate = quote.ModifiedDate,
            Text = quote.Text,
            QuoteId = quote.QuoteId
        };
    }

    public static ResourceViewModel ToViewModel(this ResourceConfig resource)
    {
        if (resource == null) return null;

        return new ResourceViewModel()
        {
            Active = resource.Active,
            CreatedBy = resource.CreatedBy,
            CreatedDate = resource.CreatedDate,
            DisplayOrder = resource.DisplayOrder,
            ResourceId = resource.ResourceId,
            ModifiedBy = resource.ModifiedBy,
            ModifiedDate = resource.ModifiedDate,
            Text = resource.Text,
            Title = resource.Title,
            Url = resource.Url
        };
    }

    public static VideoViewModel ToViewModel(this VideoConfig video)
    {
        if (video == null) return null;

        return new VideoViewModel()
        {
            Active = video.Active,
            CreatedBy = video.CreatedBy,
            CreatedDate = video.CreatedDate,
            DisplayOrder = video.DisplayOrder,
            DisplayOnHomePageInd = video.DisplayOnHomePageInd,
            ModifiedBy = video.ModifiedBy,
            ModifiedDate = video.ModifiedDate,
            Text = video.Text,
            Title = video.Title,
            Url = video.Url,
            VideoId = video.VideoId
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

    public static AccountViewModel ToViewModel(this UserAccount account)
    {
        if (account == null) return null;

        return new AccountViewModel()
        {
            ActivationKey = account.ActivationKey,
            ActivationKeyExpiry = account.ActivationKeyExpiry,
            Active = account.Active,
            AltEmailAddress = account.AltEmailAddress,
            AltEmailAddressKey = account.AltEmailAddressKey,
            AltEmailAddressKeyExpiry = account.AltEmailAddressKeyExpiry,
            AltEmailAddressVerifiedInd = account.AltEmailAddressVerifiedInd,
            AltPhoneNumber = account.AltPhoneNumber,
            AltPhoneNumberExtn = account.AltPhoneNumberExtn,
            AltPhoneNumberKey = account.AltPhoneNumberKey,
            AltPhoneNumberKeyExpiry = account.AltPhoneNumberKeyExpiry,
            AltPhoneNumberTypeCd = account.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = account.AltPhoneNumberTypeDescription,
            AltPhoneNumberVerifiedInd = account.AltPhoneNumberVerifiedInd,
            AuthRepEmailAddressInd = account.AuthRepEmailAddressInd,
            CreatedBy = account.CreatedBy,
            CreatedDate = account.CreatedDate,
            DeactivatedBy = account.DeactivatedBy,
            DeactivatedDate = account.DeactivatedDate,
            DisabilityInd = account.DisabilityInd,
            EthnicityCd = account.EthnicityCd,
            EthnicityDescription = account.EthnicityDescription,
            FirstName = account.FirstName,
            GenderCd = account.GenderCd,
            GenderDescription = account.GenderDescription,
            LanguagePreferenceCd = account.LanguagePreferenceCd,
            LanguagePreferenceDescription = account.LanguagePreferenceDescription,
            LanguagePreferenceOther = account.LanguagePreferenceOther,
            LastLoggedIn = account.LastLoggedIn,
            LastName = account.LastName,
            ListingPreferenceCd = account.LanguagePreferenceCd,
            ListingPreferenceDescription = account.ListingPreferenceDescription,
            MiddleName = account.MiddleName,
            ModifiedBy = account.ModifiedBy,
            ModifiedDate = account.ModifiedDate,
            PasswordHash = account.PasswordHash,
            PasswordResetKey = account.PasswordResetKey,
            PasswordResetKeyExpiry = account.PasswordResetKeyExpiry,
            PhoneNumber = account.PhoneNumber,
            PhoneNumberExtn = account.PhoneNumberExtn,
            PhoneNumberKey = account.PhoneNumberKey,
            PhoneNumberKeyExpiry = account.PhoneNumberKeyExpiry,
            PhoneNumberTypeCd = account.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = account.PhoneNumberTypeDescription,
            PhoneNumberVerifiedInd = account.PhoneNumberVerifiedInd,
            Pronouns = account.Pronouns,
            RaceCd = account.RaceCd,
            RaceDescription = account.RaceDescription,
            SmsNotificationsPreferenceInd = account.SmsNotificationsPreferenceInd,
            StudentInd = account.StudentInd,
            Suffix = account.Suffix,
            Title = account.Title,
            Username = account.Username,
            UsernameVerifiedInd = account.UsernameVerifiedInd,
            VeteranInd = account.VeteranInd
        };
    }

    public static UserNotificationViewModel ToViewModel(this UserNotification notification)
    {
        if (notification == null) return null;

        return new UserNotificationViewModel()
        {
            Active = notification.Active,
            Body = notification.Body,
            CreatedBy = notification.CreatedBy,
            CreatedDate = notification.CreatedDate,
            EmailSentInd = notification.EmailSentInd,
            EmailTimestamp = notification.EmailTimestamp,
            ModifiedBy = notification.ModifiedBy,
            ModifiedDate = notification.ModifiedDate,
            NotificationId = notification.NotificationId,
            ReadInd = notification.ReadInd,
            Subject = notification.Subject,
            Username = notification.Username
        };
    }

    public static UserDocumentViewModel ToViewModel(this UserDocument document)
    {
        if (document == null) return null;

        return new UserDocumentViewModel()
        {
            Active = document.Active,
            CreatedBy = document.CreatedBy,
            CreatedDate = document.CreatedDate,
            DocContents = document.DocContents,
            DocId = document.DocId,
            DocName = document.DocName,
            DocTypeCd = document.DocTypeCd,
            DocTypeDescription = document.DocTypeDescription,
            FileName = document.FileName,
            MimeType = document.MimeType,
            ModifiedBy = document.ModifiedBy,
            ModifiedDate = document.ModifiedDate,
            Username = document.Username
        };
    }

    public static ListingViewModel ToViewModel(this Listing listing)
    {
        if (listing == null) return null;

        return new ListingViewModel()
        {
            Active = listing.Active,
            ApplicationStartDate = listing.ApplicationStartDate,
            ApplicationEndDate = listing.ApplicationEndDate,
            City = listing.City,
            County = listing.County,
            CreatedBy = listing.CreatedBy,
            CreatedDate = listing.CreatedDate,
            Description = listing.Description,
            EsriX = listing.EsriX,
            EsriY = listing.EsriY,
            ImageContents = listing.ImageContents,
            ImageIsPrimary = listing.ImageIsPrimary,
            ImageMimeType = listing.ImageMimeType,
            ImageTitle = listing.ImageTitle,
            ListingAgeTypeCd = listing.ListingAgeTypeCd,
            ListingAgeTypeDescription = listing.ListingAgeTypeDescription,
            ListingEndDate = listing.ListingEndDate,
            ListingId = listing.ListingId,
            ListingStartDate = listing.ListingStartDate,
            ListingTypeCd = listing.ListingTypeCd,
            ListingTypeDescription = listing.ResaleInd ? "Resale" : listing.ListingTypeDescription,
            LotteryDate = listing.LotteryDate,
            LotteryEligible = listing.LotteryEligible,
            MapUrl = listing.MapUrl,
            MarketingAgentApplicationLink = listing.MarketingAgentApplicationLink,
            MarketingAgentId = listing.MarketingAgentId,
            MarketingAgentInd = listing.MarketingAgentInd,
            MarketingAgentName = listing.MarketingAgentName,
            MaxHouseholdSize = listing.MaxHouseholdSize,
            MaxHouseholdIncomeAmt = listing.MaxHouseholdIncomeAmt,
            MinHouseholdSize = listing.MinHouseholdSize,
            MinHouseholdIncomeAmt = listing.MinHouseholdIncomeAmt,
            ModifiedBy = listing.ModifiedBy,
            ModifiedDate = listing.ModifiedDate,
            Municipality = listing.Municipality,
            MunicipalityUrl = listing.MunicipalityUrl,
            Name = listing.Name,
            PetsAllowedInd = listing.PetsAllowedInd,
            PetsAllowedText = listing.PetsAllowedText,
            RentIncludesText = listing.RentIncludesText,
            ResaleInd = listing.ResaleInd,
            SchoolDistrict = listing.SchoolDistrict,
            SchoolDistrictUrl = listing.SchoolDistrictUrl,
            StateCd = listing.StateCd,
            StatusCd = listing.StatusCd,
            StatusDescription = listing.StatusDescription,
            StreetLine1 = listing.StreetLine1,
            StreetLine2 = listing.StreetLine2,
            StreetLine3 = listing.StreetLine3,
            WaitlistEligible = listing.WaitlistEligible,
            WaitlistEndDate = listing.WaitlistEndDate,
            WaitlistStartDate = listing.WaitlistStartDate,
            WebsiteUrl = listing.WebsiteUrl,
            ZipCode = listing.ZipCode
        };
    }

    public static PrintableFormViewModel ToPrintableViewModel(this Listing listing)
    {
        if (listing == null) return null;

        return new PrintableFormViewModel
        {
            Active = listing.Active,
            ApplicationStartDate = listing.ApplicationStartDate,
            ApplicationEndDate = listing.ApplicationEndDate,
            City = listing.City,
            County = listing.County,
            CreatedBy = listing.CreatedBy,
            CreatedDate = listing.CreatedDate,
            Description = listing.Description,
            ListingEndDate = listing.ListingEndDate,
            ListingId = listing.ListingId,
            ListingStartDate = listing.ListingStartDate,
            ListingTypeCd = listing.ListingTypeCd,
            ListingTypeDescription = listing.ResaleInd ? "Resale" : listing.ListingTypeDescription,
            LotteryDate = listing.LotteryDate,
            LotteryEligible = listing.LotteryEligible,
            MapUrl = listing.MapUrl,
            MaxHouseholdSize = listing.MaxHouseholdSize,
            MaxHouseholdIncomeAmt = listing.MaxHouseholdIncomeAmt,
            MinHouseholdSize = listing.MinHouseholdSize,
            MinHouseholdIncomeAmt = listing.MinHouseholdIncomeAmt,
            ModifiedBy = listing.ModifiedBy,
            ModifiedDate = listing.ModifiedDate,
            Municipality = listing.Municipality,
            MunicipalityUrl = listing.MunicipalityUrl,
            Name = listing.Name,
            PetsAllowedInd = listing.PetsAllowedInd,
            PetsAllowedText = listing.PetsAllowedText,
            RentIncludesText = listing.RentIncludesText,
            ResaleInd = listing.ResaleInd,
            SchoolDistrict = listing.SchoolDistrict,
            SchoolDistrictUrl = listing.SchoolDistrictUrl,
            StateCd = listing.StateCd,
            StatusCd = listing.StatusCd,
            StatusDescription = listing.StatusDescription,
            StreetLine1 = listing.StreetLine1,
            StreetLine2 = listing.StreetLine2,
            StreetLine3 = listing.StreetLine3,
            WaitlistEligible = listing.WaitlistEligible,
            WaitlistEndDate = listing.WaitlistEndDate,
            WaitlistStartDate = listing.WaitlistStartDate,
            WebsiteUrl = listing.WebsiteUrl,
            ZipCode = listing.ZipCode,
        };
    }

    public static ListingDocumentViewModel ToViewModel(this ListingDocument document)
    {
        if (document == null) return null;

        return new ListingDocumentViewModel()
        {
            Active = document.Active,
            Contents = document.Contents,
            CreatedBy = document.CreatedBy,
            CreatedDate = document.CreatedDate,
            DocumentId = document.DocumentId,
            FileName = document.FileName,
            ListingId = document.ListingId,
            MimeType = document.MimeType,
            ModifiedBy = document.ModifiedBy,
            ModifiedDate = document.ModifiedDate,
            Title = document.Title,
        };
    }

    public static ListingImageViewModel ToViewModel(this ListingImage image)
    {
        if (image == null) return null;

        return new ListingImageViewModel()
        {
            Active = image.Active,
            Contents = image.Contents,
            CreatedBy = image.CreatedBy,
            CreatedDate = image.CreatedDate,
            ImageId = image.ImageId,
            IsPrimary = image.IsPrimary,
            ListingId = image.ListingId,
            MimeType = image.MimeType,
            ModifiedBy = image.ModifiedBy,
            ModifiedDate = image.ModifiedDate,
            ThumbnailContents = image.ThumbnailContents,
            Title = image.Title,
        };
    }

    public static ListingUnitViewModel ToViewModel(this ListingUnit unit)
    {
        if (unit == null) return null;

        return new ListingUnitViewModel()
        {
            Active = unit.Active,
            AreaMedianIncomePct = unit.AreaMedianIncomePct,
            AssetLimitAmt = unit.AssetLimitAmt,
            BathroomCnt = unit.BathroomCnt,
            BathroomCntPart = unit.BathroomCntPart,
            BedroomCnt = unit.BedroomCnt,
            CreatedBy = unit.CreatedBy,
            CreatedDate = unit.CreatedDate,
            EstimatedPriceAmt = unit.EstimatedPriceAmt,
            ListingId = unit.ListingId,
            ModifiedBy = unit.ModifiedBy,
            ModifiedDate = unit.ModifiedDate,
            MonthlyInsuranceAmt = unit.MonthlyInsuranceAmt,
            MonthlyMaintenanceAmt = unit.MonthlyMaintenanceAmt,
            MonthlyRentAmt = unit.MonthlyRentAmt,
            MonthlyTaxesAmt = unit.MonthlyTaxesAmt,
            SquareFootage = unit.SquareFootage,
            SubsidyAmt = unit.SubsidyAmt,
            UnitId = unit.UnitId,
            UnitsAvailableCnt = unit.UnitsAvailableCnt,
            UnitTypeCd = unit.UnitTypeCd,
            UnitTypeDescription = unit.UnitTypeDescription
        };
    }

    public static ListingUnitHouseholdViewModel ToViewModel(this ListingUnitHousehold unitHousehold)
    {
        if (unitHousehold == null) return null;

        return new ListingUnitHouseholdViewModel()
        {
            Active = unitHousehold.Active,
            HouseholdId = unitHousehold.HouseholdId,
            HouseholdSize = unitHousehold.HouseholdSize,
            MaxHouseholdIncomeAmt = unitHousehold.MaxHouseholdIncomeAmt,
            MinHouseholdIncomeAmt = unitHousehold.MinHouseholdIncomeAmt,
            UnitId = unitHousehold.UnitId,
            CreatedBy = unitHousehold.CreatedBy,
            CreatedDate = unitHousehold.CreatedDate,
            ModifiedBy = unitHousehold.ModifiedBy,
            ModifiedDate = unitHousehold.ModifiedDate
        };
    }

    public static AmenityViewModel ToViewModel(this Amenity amenity)
    {
        if (amenity == null) return null;

        return new AmenityViewModel()
        {
            Active = amenity.Active,
            Description = amenity.Description,
            AmenityId = amenity.AmenityId,
            Name = amenity.Name,
            CreatedBy = amenity.CreatedBy,
            CreatedDate = amenity.CreatedDate,
            ModifiedBy = amenity.ModifiedBy,
            ModifiedDate = amenity.ModifiedDate,
            UsageCount = amenity.UsageCount
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

    public static DocumentTypeViewModel ToViewModel(this DocumentType documentType)
    {
        if (documentType == null) return null;

        return new DocumentTypeViewModel()
        {
            Active = documentType.Active,
            Description = documentType.Description,
            DocumentTypeId = documentType.DocumentTypeId,
            Name = documentType.Name,
            CreatedBy = documentType.CreatedBy,
            CreatedDate = documentType.CreatedDate,
            ModifiedBy = documentType.ModifiedBy,
            ModifiedDate = documentType.ModifiedDate,
            UsageCount = documentType.UsageCount
        };
    }

    public static FundingSourceViewModel ToViewModel(this FundingSource fundingSource)
    {
        if (fundingSource == null) return null;

        return new FundingSourceViewModel()
        {
            Active = fundingSource.Active,
            Description = fundingSource.Description,
            FundingSourceId = fundingSource.FundingSourceId,
            Name = fundingSource.Name,
            CreatedBy = fundingSource.CreatedBy,
            CreatedDate = fundingSource.CreatedDate,
            ModifiedBy = fundingSource.ModifiedBy,
            ModifiedDate = fundingSource.ModifiedDate,
            UsageCount = fundingSource.UsageCount
        };
    }

    public static ProfileViewModel ToProfileViewModel(this UserAccount profile)
    {
        if (profile == null) return null;

        return new ProfileViewModel()
        {
            ActivationKey = profile.ActivationKey,
            ActivationKeyExpiry = profile.ActivationKeyExpiry,
            Active = profile.Active,
            AddressInd = profile.AddressInd,
            AltEmailAddress = profile.AltEmailAddress,
            AltEmailAddressKey = profile.AltEmailAddressKey,
            AltEmailAddressKeyExpiry = profile.AltEmailAddressKeyExpiry,
            AltEmailAddressVerifiedInd = profile.AltEmailAddressVerifiedInd,
            AltPhoneNumber = profile.AltPhoneNumber,
            AltPhoneNumberExtn = profile.AltPhoneNumberExtn,
            AltPhoneNumberKey = profile.AltPhoneNumberKey,
            AltPhoneNumberKeyExpiry = profile.AltPhoneNumberKeyExpiry,
            AltPhoneNumberTypeCd = profile.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = profile.AltPhoneNumberTypeDescription,
            AltPhoneNumberVerifiedInd = profile.AltPhoneNumberVerifiedInd,
            AssetValueAmt = profile.AssetValueAmt,
            AuthRepEmailAddressInd = profile.AuthRepEmailAddressInd,
            CountyLivingIn = profile.CountyLivingIn,
            CountyWorkingIn = profile.CountyWorkingIn,
            CreatedBy = profile.CreatedBy,
            CreatedDate = profile.CreatedDate,
            CurrentlyWorkingInWestchesterInd = profile.CurrentlyWorkingInWestchesterInd,
            DateOfBirth = profile.DateOfBirth,
            DeactivatedBy = profile.DeactivatedBy,
            DeactivatedDate = profile.DeactivatedDate,
            DifferentMailingAddressInd = profile.DifferentMailingAddressInd,
            DisabilityInd = profile.DisabilityInd,
            EmailAddress = profile.EmailAddress,
            EthnicityCd = profile.EthnicityCd,
            EthnicityDescription = profile.EthnicityDescription,
            EverLivedInWestchesterInd = profile.EverLivedInWestchesterInd,
            FirstName = profile.FirstName,
            GenderCd = profile.GenderCd,
            GenderDescription = profile.GenderDescription,
            HouseholdSize = profile.HouseholdSize,
            IdIssueDate = profile.IdIssueDate,
            IdTypeCd = profile.IdTypeCd,
            IdTypeDescription = profile.IdTypeDescription,
            IdTypeValue = profile.IdTypeValue,
            IncomeValueAmt = profile.IncomeValueAmt,
            LanguagePreferenceCd = profile.LanguagePreferenceCd,
            LanguagePreferenceDescription = profile.LanguagePreferenceDescription,
            LanguagePreferenceOther = profile.LanguagePreferenceOther,
            Last4SSN = profile.Last4SSN,
            LastLoggedIn = profile.LastLoggedIn,
            LastName = profile.LastName,
            LeadTypeCd = profile.LeadTypeCd,
            LeadTypeDescription = profile.LeadTypeDescription,
            LeadTypeOther = profile.LeadTypeOther,
            ListingPreferenceCd = profile.ListingPreferenceCd,
            ListingPreferenceDescription = profile.ListingPreferenceDescription,
            MailingStreetLine1 = profile.MailingStreetLine1,
            MailingStreetLine2 = profile.MailingStreetLine2,
            MailingStreetLine3 = profile.MailingStreetLine3,
            MailingCity = profile.MailingCity,
            MailingStateCd = profile.MailingStateCd,
            MailingZipCode = profile.MailingZipCode,
            MailingCounty = profile.MailingCounty,
            MailingCountyDescription = profile.MailingCountyDescription,
            MiddleName = profile.MiddleName,
            ModifiedBy = profile.ModifiedBy,
            ModifiedDate = profile.ModifiedDate,
            OwnRealEstateInd = profile.OwnRealEstateInd,
            PasswordHash = profile.PasswordHash,
            PhoneNumber = profile.PhoneNumber,
            PhoneNumberExtn = profile.PhoneNumberExtn,
            PhoneNumberKey = profile.PhoneNumberKey,
            PhoneNumberKeyExpiry = profile.PhoneNumberKeyExpiry,
            PhoneNumberTypeCd = profile.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = profile.PhoneNumberTypeDescription,
            PhoneNumberVerifiedInd = profile.PhoneNumberVerifiedInd,
            PhysicalStreetLine1 = profile.PhysicalStreetLine1,
            PhysicalStreetLine2 = profile.PhysicalStreetLine2,
            PhysicalStreetLine3 = profile.PhysicalStreetLine3,
            PhysicalCity = profile.PhysicalCity,
            PhysicalStateCd = profile.PhysicalStateCd,
            PhysicalZipCode = profile.PhysicalZipCode,
            PhysicalCounty = profile.PhysicalCounty,
            PhysicalCountyDescription = profile.PhysicalCountyDescription,
            Pronouns = profile.Pronouns,
            RaceCd = profile.RaceCd,
            RaceDescription = profile.RaceDescription,
            RealEstateValueAmt = profile.RealEstateValueAmt,
            SmsNotificationsPreferenceInd = profile.SmsNotificationsPreferenceInd,
            StudentInd = profile.StudentInd,
            Suffix = profile.Suffix,
            Title = profile.Title,
            Username = profile.Username,
            UsernameVerifiedInd = profile.UsernameVerifiedInd,
            VeteranInd = profile.VeteranInd
        };
    }

    public static HouseholdViewModel ToViewModel(this Household household)
    {
        if (household == null) return null;

        return new HouseholdViewModel()
        {
            Active = household.Active,
            AddressInd = household.AddressInd,
            CreatedBy = household.CreatedBy,
            CreatedDate = household.CreatedDate,
            DifferentMailingAddressInd = household.DifferentMailingAddressInd,
            HouseholdId = household.HouseholdId,
            HouseholdIncomeAmt = household.HouseholdIncomeAmt,
            HouseholdAssetAmt = household.HouseholdAssetAmt,
            HouseholdRealEstateAmt = household.HouseholdRealEstateAmt,
            HouseholdSize = household.HouseholdSize,
            LiveInAideInd = household.LiveInAideInd,
            MailingStreetLine1 = household.MailingStreetLine1,
            MailingStreetLine2 = household.MailingStreetLine2,
            MailingStreetLine3 = household.MailingStreetLine3,
            MailingCity = household.MailingCity,
            MailingStateCd = household.MailingStateCd,
            MailingZipCode = household.MailingZipCode,
            MailingCounty = household.MailingCounty,
            MailingCountyDescription = household.MailingCountyDescription,
            ModifiedBy = household.ModifiedBy,
            ModifiedDate = household.ModifiedDate,
            PhysicalStreetLine1 = household.PhysicalStreetLine1,
            PhysicalStreetLine2 = household.PhysicalStreetLine2,
            PhysicalStreetLine3 = household.PhysicalStreetLine3,
            PhysicalCity = household.PhysicalCity,
            PhysicalStateCd = household.PhysicalStateCd,
            PhysicalZipCode = household.PhysicalZipCode,
            PhysicalCounty = household.PhysicalCounty,
            PhysicalCountyDescription = household.PhysicalCountyDescription,
            Username = household.Username,
            UsernameVerifiedInd = household.UsernameVerifiedInd,
            VoucherCds = household.VoucherCds,
            VoucherInd = household.VoucherInd,
            VoucherOther = household.VoucherOther,
            VoucherAdminName = household.VoucherAdminName,
        };
    }

    public static HouseholdMemberViewModel ToViewModel(this HouseholdMember member)
    {
        if (member == null) return null;

        return new HouseholdMemberViewModel()
        {
            Active = member.Active,
            AltEmailAddress = member.AltEmailAddress,
            AltPhoneNumber = member.AltPhoneNumber,
            AltPhoneNumberExtn = member.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = member.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = member.AltPhoneNumberTypeDescription,
            AssetValueAmt = member.AssetValueAmt,
            CountyLivingIn = member.CountyLivingIn,
            CountyWorkingIn = member.CountyWorkingIn,
            CreatedBy = member.CreatedBy,
            CreatedDate = member.CreatedDate,
            CurrentlyWorkingInWestchesterInd = member.CurrentlyWorkingInWestchesterInd,
            DateOfBirth = member.DateOfBirth,
            DisabilityInd = member.DisabilityInd,
            EmailAddress = member.EmailAddress,
            EthnicityCd = member.EthnicityCd,
            EthnicityDescription = member.EthnicityDescription,
            EverLivedInWestchesterInd = member.EverLivedInWestchesterInd,
            FirstName = member.FirstName,
            GenderCd = member.GenderCd,
            GenderDescription = member.GenderDescription,
            IdIssueDate = member.IdIssueDate,
            IdTypeCd = member.IdTypeCd,
            IdTypeDescription = member.IdTypeDescription,
            IdTypeValue = member.IdTypeValue,
            IncomeValueAmt = member.IncomeValueAmt,
            Last4SSN = member.Last4SSN,
            LastName = member.LastName,
            MemberId = member.MemberId,
            MiddleName = member.MiddleName,
            ModifiedBy = member.ModifiedBy,
            ModifiedDate = member.ModifiedDate,
            OwnRealEstateInd = member.OwnRealEstateInd,
            PhoneNumber = member.PhoneNumber,
            PhoneNumberExtn = member.PhoneNumberExtn,
            PhoneNumberTypeCd = member.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = member.PhoneNumberTypeDescription,
            Pronouns = member.Pronouns,
            RaceCd = member.RaceCd,
            RaceDescription = member.RaceDescription,
            RealEstateValueAmt = member.RealEstateValueAmt,
            RelationTypeCd = member.RelationTypeCd,
            RelationTypeDescription = member.RelationTypeDescription,
            RelationTypeOther = member.RelationTypeOther,
            StudentInd = member.StudentInd,
            Suffix = member.Suffix,
            Title = member.Title,
            VeteranInd = member.VeteranInd
        };
    }

    public static HouseholdAccountViewModel ToViewModel(this HouseholdAccount account)
    {
        if (account == null) return null;

        return new HouseholdAccountViewModel()
        {
            AccountId = account.AccountId,
            AccountNumber = account.AccountNumber,
            AccountTypeCd = account.AccountTypeCd,
            AccountTypeDescription = account.AccountTypeDescription,
            AccountTypeOther = account.AccountTypeOther,
            AccountValueAmt = account.AccountValueAmt,
            Active = account.Active,
            CreatedBy = account.CreatedBy,
            CreatedDate = account.CreatedDate,
            InstitutionName = account.InstitutionName,
            ModifiedBy = account.ModifiedBy,
            ModifiedDate = account.ModifiedDate,
            PrimaryHolderMemberId = account.PrimaryHolderMemberId,
        };
    }

    public static HousingApplicationViewModel ToViewModel(this HousingApplication application)
    {
        if (application == null) return null;

        return new HousingApplicationViewModel()
        {
            AccessibilityCds = application.AccessibilityCds,
            AccountIds = application.AccountIds,
            Active = application.Active,
            AddressInd = application.AddressInd,
            AltEmailAddress = application.AltEmailAddress,
            AltPhoneNumber = application.AltPhoneNumber,
            AltPhoneNumberExtn = application.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = application.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = application.AltPhoneNumberTypeDescription,
            ApplicationId = application.ApplicationId,
            AssetValueAmt = application.AssetValueAmt,
            CoApplicantInd = application.CoApplicantInd,
            CoApplicantMemberId = application.CoApplicantMemberId,
            CountyLivingIn = application.CountyLivingIn,
            CountyWorkingIn = application.CountyWorkingIn,
            CreatedBy = application.CreatedBy,
            CreatedDate = application.CreatedDate,
            CurrentlyWorkingInWestchesterInd = application.CurrentlyWorkingInWestchesterInd,
            DateOfBirth = application.DateOfBirth,
            DifferentMailingAddressInd = application.DifferentMailingAddressInd,
            DisabilityInd = application.DisabilityInd,
            DisqualificationCd = application.DisqualificationCd,
            DisqualificationDescription = application.DisqualificationDescription,
            DisqualificationOther = application.DisqualificationOther,
            DisqualificationReason = application.DisqualificationReason,
            DisqualifiedInd = application.DisqualifiedInd,
            DuplicateCheckCd = application.DuplicateCheckCd,
            DuplicateCheckResponseDueDate = application.DuplicateCheckResponseDueDate,
            EmailAddress = application.EmailAddress,
            EthnicityCd = application.EthnicityCd,
            EthnicityDescription = application.EthnicityDescription,
            EverLivedInWestchesterInd = application.EverLivedInWestchesterInd,
            FirstName = application.FirstName,
            GenderCd = application.GenderCd,
            GenderDescription = application.GenderDescription,
            IdIssueDate = application.IdIssueDate,
            IdTypeCd = application.IdTypeCd,
            IdTypeDescription = application.IdTypeDescription,
            IdTypeValue = application.IdTypeValue,
            IncomeValueAmt = application.IncomeValueAmt,
            LanguagePreferenceCd = application.LanguagePreferenceCd,
            LanguagePreferenceDescription = application.LanguagePreferenceDescription,
            LanguagePreferenceOther = application.LanguagePreferenceOther,
            Last4SSN = application.Last4SSN,
            LastName = application.LastName,
            LeadTypeCd = application.LeadTypeCd,
            LeadTypeDescription = application.LeadTypeDescription,
            LeadTypeOther = application.LeadTypeOther,
            ListingId = application.ListingId,
            ListingPreferenceCd = application.ListingPreferenceCd,
            ListingPreferenceDescription = application.ListingPreferenceDescription,
            LiveInAideInd = application.LiveInAideInd,
            MailingCity = application.MailingCity,
            MailingCounty = application.MailingCounty,
            MailingStateCd = application.MailingStateCd,
            MailingStreetLine1 = application.MailingStreetLine1,
            MailingStreetLine2 = application.MailingStreetLine2,
            MailingStreetLine3 = application.MailingStreetLine3,
            MailingZipCode = application.MailingZipCode,
            MemberIds = application.MemberIds,
            MiddleName = application.MiddleName,
            ModifiedBy = application.ModifiedBy,
            ModifiedDate = application.ModifiedDate,
            OwnRealEstateInd = application.OwnRealEstateInd,
            PhoneNumber = application.PhoneNumber,
            PhoneNumberExtn = application.PhoneNumberExtn,
            PhoneNumberTypeCd = application.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = application.PhoneNumberTypeDescription,
            PhysicalCity = application.PhysicalCity,
            PhysicalCounty = application.PhysicalCounty,
            PhysicalStateCd = application.PhysicalStateCd,
            PhysicalStreetLine1 = application.PhysicalStreetLine1,
            PhysicalStreetLine2 = application.PhysicalStreetLine2,
            PhysicalStreetLine3 = application.PhysicalStreetLine3,
            PhysicalZipCode = application.PhysicalZipCode,
            Pronouns = application.Pronouns,
            RaceCd = application.RaceCd,
            RaceDescription = application.RaceDescription,
            RealEstateValueAmt = application.RealEstateValueAmt,
            ReceivedDate = application.ReceivedDate,
            SmsNotificationsPreferenceInd = application.SmsNotificationsPreferenceInd,
            StatusCd = application.StatusCd,
            StatusDescription = application.StatusDescription,
            StudentInd = application.StudentInd,
            SubmittedDate = application.SubmittedDate,
            Suffix = application.Suffix,
            Title = application.Title,
            UnitTypeCds = application.UnitTypeCds,
            Username = application.Username,
            VeteranInd = application.VeteranInd,
            VoucherAdminName = application.VoucherAdminName,
            VoucherCds = application.VoucherCds,
            VoucherInd = application.VoucherInd,
            VoucherOther = application.VoucherOther,
            WithdrawnDate = application.WithdrawnDate,

            Name = application.Name,
            StreetLine1 = application.StreetLine1,
            StreetLine2 = application.StreetLine2,
            StreetLine3 = application.StreetLine3,
            City = application.City,
            StateCd = application.StateCd,
            ZipCode = application.ZipCode,
            County = application.County,
            ListingStartDate = application.ListingStartDate,
            ListingEndDate = application.ListingEndDate,
            ApplicationStartDate = application.ApplicationStartDate,
            ApplicationEndDate = application.ApplicationEndDate,
            LotteryDate = application.LotteryDate,
            LotteryEligible = application.LotteryEligible,
            WaitlistEligible = application.WaitlistEligible,
            WaitlistStartDate = application.WaitlistStartDate,
            WaitlistEndDate = application.WaitlistEndDate,
        };
    }

    public static ApplicationDocumentViewModel ToViewModel(this ApplicationDocument document)
    {
        if (document == null) return null;

        return new ApplicationDocumentViewModel()
        {
            Active = document.Active,
            ApplicationId = document.ApplicationId,
            CreatedBy = document.CreatedBy,
            CreatedDate = document.CreatedDate,
            DocContents = document.DocContents,
            DocId = document.DocId,
            DocName = document.DocName,
            DocTypeId = document.DocTypeId,
            DocTypeDescription = document.DocTypeDescription,
            FileName = document.FileName,
            MimeType = document.MimeType,
            ModifiedBy = document.ModifiedBy,
            ModifiedDate = document.ModifiedDate,
            Username = document.Username
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
            ModifiedBy = comment.ModifiedBy,
            ModifiedDate = comment.ModifiedDate,
            Username = comment.Username
        };
    }

    public static string ToDateEditorStringFormat(this DateTime? input, string format = "yyyy-MM-dd")
    {
        return input.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? input.Value.ToString(format) : "";
    }

    public static DateTime? FromDateEditorStringFormat(this string input, string format = "yyyy-MM-dd")
    {
        if (DateTime.TryParse(input, out var date) && date.Date >= new DateTime(1900, 1, 1).Date)
        {
            return date;
        }
        return null;
    }
}