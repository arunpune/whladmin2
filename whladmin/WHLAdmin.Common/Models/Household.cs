using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class Household : UserProfile
{
    public long HouseholdId { get; set; }
    public bool AddressInd { get; set; }
    public string PhysicalStreetLine1 { get; set; }
    public string PhysicalStreetLine2 { get; set; }
    public string PhysicalStreetLine3 { get; set; }
    public string PhysicalCity { get; set; }
    public string PhysicalStateCd { get; set; }
    public string PhysicalZipCode { get; set; }
    public string PhysicalCounty { get; set; }
    public string PhysicalCountyDescription { get; set; }
    public bool DifferentMailingAddressInd { get; set; }
    public string MailingStreetLine1 { get; set; }
    public string MailingStreetLine2 { get; set; }
    public string MailingStreetLine3 { get; set; }
    public string MailingCity { get; set; }
    public string MailingStateCd { get; set; }
    public string MailingZipCode { get; set; }
    public string MailingCounty { get; set; }
    public string MailingCountyDescription { get; set; }
    public bool VoucherInd { get; set; }
    public string VoucherCds { get; set; }
    public string VoucherOther { get; set; }
    public string VoucherAdminName { get; set; }
    public bool LiveInAideInd { get; set; }
    public int HouseholdSize { get; set; }
    public long HouseholdIncomeAmt { get; set; }
    public long HouseholdAssetAmt { get; set; }
    public long HouseholdRealEstateAmt { get; set; }
}

[ExcludeFromCodeCoverage]
public class HouseholdMember : UserProfile
{
    public long MemberId { get; set; }
    public long HouseholdId { get; set; }
    public string Username { get; set; }

    public string RelationTypeCd { get; set; }
    public string RelationTypeDescription { get; set; }
    public string RelationTypeOther { get; set; }
}

[ExcludeFromCodeCoverage]
public class HouseholdAccount : ModelBase
{
    public long AccountId { get; set; }
    public long HouseholdId { get; set; }
    public string Username { get; set; }
    public string InstitutionName { get; set; }
    public string AccountNumber { get; set; }
    public string AccountTypeCd { get; set; }
    public string AccountTypeDescription { get; set; }
    public string AccountTypeOther { get; set; }
    public long AccountValueAmt { get; set; }
    public long PrimaryHolderMemberId { get; set; }
    public string PrimaryHolderMemberName { get; set; }
}

[ExcludeFromCodeCoverage]
public class HouseholdMemberAsset : ModelBase
{
    public long HouseholdId { get; set; }
    public long MemberId { get; set; }
    public string AssetTypeCd { get; set; }
    public string AssetTypeDescription { get; set; }
    public string AssetTypeOther { get; set; }
    public long AssetValueAmt { get; set; }
}

[ExcludeFromCodeCoverage]
public class HouseholdMemberIncome
{
    public long HouseholdId { get; set; }
    public long MemberId { get; set; }
    public string IncomeTypeCd { get; set; }
    public string IncomeTypeDescription { get; set; }
    public string IncomeTypeOther { get; set; }
    public long IncomeValueAmt { get; set; }
}