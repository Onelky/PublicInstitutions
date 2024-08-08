using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SB.PublicInstitutions.Domain.Exceptions
{
    public sealed class DuplicatedInstitutionName : BadRequestException
    {
        public DuplicatedInstitutionName(string name)
            : base($"Institution name is already registered")
        {
        }
    }
}

