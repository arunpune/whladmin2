using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class AmiConfig : ModelBase
{
    public int EffectiveDate { get; set; }
    public int EffectiveYear { get; set; }
    public long IncomeAmt { get; set; } 
    public int Hh1 { get; set; }
    public int Hh2 { get; set; }
    public int Hh3 { get; set; }
    public int Hh4 { get; set; }
    public int Hh5 { get; set; }
    public int Hh6 { get; set; }
    public int Hh7 { get; set; }
    public int Hh8 { get; set; }
    public int Hh9 { get; set; }
    public int Hh10 { get; set; }
}