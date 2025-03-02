using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicationData;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MedicationController : ControllerBase
{
    private readonly MasterDB _context;

    public MedicationController(MasterDB context)
    {
        _context = context;
    }

    // Helper method to get logged-in user ID
    private int? GetLoggedInUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userId, out int parsedUserId) ? parsedUserId : null;
    }

    /// <summary>
    /// Register a new medication for the logged-in user.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> RegisterMedication([FromBody] Medication medication)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetLoggedInUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized." });
        }

        medication.UserId = userId.Value;
        _context.Medications.Add(medication);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Medication registered successfully", medication });
    }

    /// <summary>
    /// Get all medications for the logged-in user with optional filters.
    /// </summary>
    [HttpGet("get")]
    public async Task<IActionResult> GetMedications(
        [FromQuery] DateTime? afterDateOfIssue,
        [FromQuery] string? description,
        [FromQuery] string? frequency,
        [FromQuery] string? reason)
    {
        var userId = GetLoggedInUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized." });
        }

        var query = _context.Medications.Where(m => m.UserId == userId.Value);

        if (afterDateOfIssue.HasValue)
            query = query.Where(m => m.DateOfIssue >= afterDateOfIssue);

        if (!string.IsNullOrEmpty(description))
            query = query.Where(m => m.Description.Contains(description));

        if (!string.IsNullOrEmpty(frequency))
            query = query.Where(m => m.Frequency == frequency);

        if (!string.IsNullOrEmpty(reason))
            query = query.Where(m => m.Reason == reason);

        var medications = await query.ToListAsync();
        if (!medications.Any())
        {
            return NotFound(new { message = "No medications found matching the criteria." });
        }

        return Ok(medications);
    }

    /// <summary>
    /// Update an existing medication.
    /// </summary>
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateMedication(int id, [FromBody] Medication updatedMedication)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userId = GetLoggedInUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized." });
        }

        var existingMedication = await _context.Medications
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId.Value);

        if (existingMedication == null)
        {
            return NotFound(new { message = "Medication not found." });
        }

        
        existingMedication.Description = updatedMedication.Description;
        existingMedication.Dosage = updatedMedication.Dosage;
        existingMedication.Frequency = updatedMedication.Frequency;
        existingMedication.Duration = updatedMedication.Duration;
        existingMedication.Reason = updatedMedication.Reason;
        existingMedication.DateOfIssue = updatedMedication.DateOfIssue;
        existingMedication.Instructions = updatedMedication.Instructions;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Medication updated successfully", medication = existingMedication });
    }

    /// <summary>
    /// Delete a medication.
    /// </summary>
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteMedication(int id)
    {
        var userId = GetLoggedInUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "User not found or unauthorized." });
        }

        var medication = await _context.Medications
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId.Value);

        if (medication == null)
        {
            return NotFound(new { message = "Medication not found." });
        }

        _context.Medications.Remove(medication);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Medication deleted successfully" });
    }
}
