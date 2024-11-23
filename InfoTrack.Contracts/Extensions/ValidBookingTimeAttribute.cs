using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace InfoTrack.Contracts.Extensions;

/// <summary>
/// Custom validation attribute to validate booking times.
/// 
/// Ensures that the booking time is in the correct format (HH:mm)
/// and within the specified business hours (9:00 AM to 4:00 PM).
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValidBookingTimeAttribute : ValidationAttribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return new ValidationResult("Booking time is required");
        }

        // Parse the time string into a TimeSpan object
        if (!TimeSpan.TryParseExact(value.ToString(), "hh\\:mm", CultureInfo.InvariantCulture, out TimeSpan timeSpan))
        {
            return new ValidationResult("Invalid time format. Please use HH:mm");
        }

        // Check if the time is within business hours
        if (timeSpan < TimeSpan.FromHours(9) || timeSpan >= TimeSpan.FromHours(16))
        {
            return new ValidationResult("Booking time must be between 9:00 AM and 4:00 PM");
        }

        return ValidationResult.Success;
    }
}
