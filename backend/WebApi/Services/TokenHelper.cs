using System.Security.Claims;

namespace WebApi.Services;

public static class TokenHelper
{
    /// <summary>
    /// Extracts the patient_id from the JWT token claims
    /// </summary>
    public static int? GetPatientId(ClaimsPrincipal user)
    {
        var patientIdClaim = user.FindFirst("patient_id")?.Value;
        
        if (string.IsNullOrEmpty(patientIdClaim))
        {
            return null;
        }

        if (int.TryParse(patientIdClaim, out var patientId))
        {
            return patientId;
        }

        return null;
    }

    /// <summary>
    /// Extracts the caregiver_id from the JWT token claims
    /// </summary>
    public static int? GetCaregiverId(ClaimsPrincipal user)
    {
        var caregiverIdClaim = user.FindFirst("caregiver_id")?.Value;
        
        if (string.IsNullOrEmpty(caregiverIdClaim))
        {
            return null;
        }

        if (int.TryParse(caregiverIdClaim, out var caregiverId))
        {
            return caregiverId;
        }

        return null;
    }

    /// <summary>
    /// Extracts the location_id from the JWT token claims
    /// </summary>
    public static int? GetLocationId(ClaimsPrincipal user)
    {
        var locationIdClaim = user.FindFirst("location_id")?.Value;
        
        if (string.IsNullOrEmpty(locationIdClaim))
        {
            return null;
        }

        if (int.TryParse(locationIdClaim, out var locationId))
        {
            return locationId;
        }

        return null;
    }

    /// <summary>
    /// Gets all roles from the JWT token
    /// </summary>
    public static IEnumerable<string> GetRoles(ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    /// <summary>
    /// Checks if user has a specific role
    /// </summary>
    public static bool HasRole(ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    /// <summary>
    /// Gets the username from the token
    /// </summary>
    public static string? GetUsername(ClaimsPrincipal user)
    {
        return user.FindFirst("preferred_username")?.Value 
               ?? user.FindFirst(ClaimTypes.Name)?.Value;
    }

    /// <summary>
    /// Gets the email from the token
    /// </summary>
    public static string? GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst("email")?.Value 
               ?? user.FindFirst(ClaimTypes.Email)?.Value;
    }
}
