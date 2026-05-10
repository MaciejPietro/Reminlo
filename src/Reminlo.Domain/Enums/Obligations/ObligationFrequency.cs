namespace Reminlo.Domain.Enums.Obligations;

/// <summary>
/// Represents the frequency interval for recurring obligations.
/// Used in conjunction with a numeric frequency value to determine how often an obligation recurs.
/// </summary>
public enum ObligationFrequency
{
    Hourly,
    Daily,
    Weekly,
    Monthly,
    Yearly
}