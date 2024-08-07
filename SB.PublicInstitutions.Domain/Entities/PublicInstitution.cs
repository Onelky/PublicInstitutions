using SB.PublicInstitutions.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SB.PublicInstitutions.Domain.Entities
{
    public class PublicInstitution
    {
        public Guid Id {  get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string Director { get; set; }
        public string? Website { get; set; }
        public GovermentBranch GovermentBranch { get; set; }
        public InstitutionType InstitutionType { get; set; }
        public DateTime CreationDate { get; set; }

    }
}
