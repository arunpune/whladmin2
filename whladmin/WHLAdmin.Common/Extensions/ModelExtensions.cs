using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;

namespace WHLAdmin.Common.Extensions;

public static class ModelExtensions
{
    public static DataTable ToDataTable(this IEnumerable<ListingUnitHousehold> items, ILogger logger, string correlationId)
    {
        var dataTable = new DataTable();
        
        dataTable.Columns.Add(new DataColumn("HouseholdSize", typeof(int)));
        dataTable.Columns.Add(new DataColumn("MinHouseholdIncomeAmt", typeof(decimal)));
        dataTable.Columns.Add(new DataColumn("MaxHouseholdIncomeAmt", typeof(decimal)));

        if ((items?.Count() ?? 0) > 0)
        {
            foreach (var item in items)
            {
                var dataRow = dataTable.NewRow();
                dataRow["HouseholdSize"] = item.HouseholdSize;
                dataRow["MinHouseholdIncomeAmt"] = item.MinHouseholdIncomeAmt;
                dataRow["MaxHouseholdIncomeAmt"] = item.MaxHouseholdIncomeAmt;
                dataTable.Rows.Add(dataRow);
            }
        }

        if (logger != null)
        {
            logger.LogDebug($"CorrelationID: {correlationId}, Message: Converted {items?.Count() ?? 0} ListingUnitHousehold objects to DataTable.");
            logger.LogDebug(dataTable.ToString());
        }

        return dataTable;
    }

    public static string ToAddressTextSingleLine(this Listing listing)
    {
        if (listing == null) return "";

        var addressText = (listing.StreetLine1 ?? "").Trim();
        if (!string.IsNullOrEmpty((listing.StreetLine2 ?? "").Trim()))
        {
            addressText += ", " + (listing.StreetLine2 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((listing.StreetLine3 ?? "").Trim()))
        {
            addressText += ", " + (listing.StreetLine3 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((listing.City ?? "").Trim()))
        {
            addressText += ", " + (listing.City ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((listing.StateCd ?? "").Trim()))
        {
            addressText += ", " + (listing.StateCd ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((listing.ZipCode ?? "").Trim()))
        {
            addressText += " " + (listing.ZipCode ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((listing.County ?? "").Trim()))
        {
            addressText += " " + (listing.County ?? "").Trim();
        }
        return addressText;
    }

    public static string GetDisplayName(string title, string firstName, string middleName, string lastName, string suffix)
    {
        if (string.IsNullOrEmpty((firstName ?? "").Trim())) return "";

        var nameText = (firstName ?? "").Trim();
        if (!string.IsNullOrEmpty((middleName ?? "").Trim()))
        {
            nameText += " " + (middleName ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((lastName ?? "").Trim()))
        {
            nameText += " " + (lastName ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((suffix ?? "").Trim()))
        {
            nameText += " " + (suffix ?? "").Trim();
        }
        return nameText;
    }

    public static string ToApplicantName(this HousingApplication application)
    {
        if (application == null) return "";
        return GetDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix);
    }

    public static string ToApplicantName(this HousingApplicant applicant)
    {
        if (applicant == null) return "";
        return GetDisplayName(applicant.Title, applicant.FirstName, applicant.MiddleName, applicant.LastName, applicant.Suffix);
    }
}