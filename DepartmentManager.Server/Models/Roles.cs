using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DepartmentManager.Server.Models;

namespace DepartmentManager.Server.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        // Foreign Key → Affiliation
        [Required]
        public int AffiliationId { get; set; }
        [ForeignKey("AffiliationId")]
        public Affiliation Affiliation { get; set; }

        // Level (int or enum depending on usage)
        public int Level { get; set; }

        // Navigation property
        public List<Member> Members { get; set; }
    }
}
