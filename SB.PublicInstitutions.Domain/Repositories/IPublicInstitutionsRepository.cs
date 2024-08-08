using SB.PublicInstitutions.Domain.Entities;

public interface IPublicInstitutionsRepository
{
    Task<List<PublicInstitution>> GetAll();

    Task<PublicInstitution> GetById(Guid id);

    Task<PublicInstitution> GetByName(string name);

    Task<PublicInstitution> Create(PublicInstitution institution);

    Task<PublicInstitution> Update(Guid id, PublicInstitution institution);

    Task<bool> Delete(Guid id);
}