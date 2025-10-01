using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Location : EntityObject
    {
        [Required, MaxLength(255)]
        public string Street { get; set; } = string.Empty;
        
        [Required, MaxLength(10)]
        public string HouseNumber { get; set; } = string.Empty;
        
        [Required, MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty;
        
        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;
        
        [MaxLength(10)]
        public string? Floor { get; set; }

        // Navigation Properties
        public List<Caregiver> Caregivers { get; set; } = new List<Caregiver>();
        public List<Patient> Patients { get; set; } = new List<Patient>();

        public override string ToString()
        {
            return $"{Street} {HouseNumber}, {PostalCode} {City}";
        }
    }
}
