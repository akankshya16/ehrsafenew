using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserData;

namespace MedicationData
{
    public class Medication
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(255, ErrorMessage = "Description must be less than 255 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        [StringLength(100, ErrorMessage = "Dosage must be less than 100 characters.")]
        public string Dosage { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        [StringLength(50, ErrorMessage = "Frequency must be less than 50 characters.")]
        public string Frequency { get; set; }

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        public int Duration { get; set; }

        [StringLength(500, ErrorMessage = "Reason must be less than 500 characters.")]
        public string? Reason { get; set; }

        [Required(ErrorMessage = "Date of Issue is required.")]
        public DateTime DateOfIssue { get; set; }

        [StringLength(500, ErrorMessage = "Instructions must be less than 500 characters.")]
        public string? Instructions { get; set; }

        // Relationship: Each Medication is linked to a User
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}
