using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DepartmentManager.Server.Models;

namespace DepartmentManager.Server.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        [Required, StringLength(100)]
        public string CityName { get; set; }

        // Navigation property
        public List<Affiliation> Affiliations { get; set; } = new List<Affiliation>();
        public List<Member> Members { get; set; } = new List<Member>();
    }

}