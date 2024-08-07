using SB.PublicInstitutions.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IPublicInstitutionsRepository
{
    Task<IEnumerable<PublicInstitution>> GetAll();

    Task<PublicInstitution> GetById(Guid id);

    Task<PublicInstitution> GetByName(string name);

    Task<PublicInstitution> Create(PublicInstitution institution);

    Task<PublicInstitution> Update(Guid id, PublicInstitution institution);

    Task<bool> Delete(Guid id);
}