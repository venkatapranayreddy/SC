using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DepartmentManager.Server.Models
{
    public class Affiliation
    {
        [Key]
        public int AffiliationId { get; set; }

        [Required, StringLength(150)]
        public string Name { get; set; }

        // Foreign Key → City
        [Required]
        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; } = new City();

        // Navigation property
        public List<Role> Roles { get; set; } = new List<Role>();
        public List<Member> Members { get; set; } = new List<Member>();
    }
}
