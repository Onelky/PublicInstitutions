using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.PublicInstitutions.Domain.Exceptions
{
    public sealed class InstitutionNameExists : BadRequestException
    {
        public InstitutionNameExists(string name)
            : base($"Institution with name {name} already exists")
        {
        }
    }
}

