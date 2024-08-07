using SB.PublicInstitutions.Domain.Entities;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IPublicInstitutionsService
{
    Task<IEnumerable<PublicInstitution>> GetAll();

    Task<PublicInstitution> Create(PublicInstitutionDto institutionDto);

    Task<PublicInstitution> Update(Guid id, PublicInstitutionDto institutionDto);

    Task<bool> Delete(Guid id);
}