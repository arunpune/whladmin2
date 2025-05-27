using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class Amortization : ModelBase
{
    public decimal Rate { get; set; }
    public decimal RateInterestOnly { get; set; }
    public decimal Rate10Year { get; set; } 
    public decimal Rate15Year { get; set; }
    public decimal Rate20Year { get; set; }
    public decimal Rate25Year { get; set; }
    public decimal Rate30Year { get; set; }
    public decimal Rate40Year { get; set; }
}