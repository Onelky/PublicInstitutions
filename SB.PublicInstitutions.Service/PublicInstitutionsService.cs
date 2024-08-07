using Microsoft.Extensions.Logging;
using SB.PublicInstitutions.Domain.Entities;
using SB.PublicInstitutions.Domain.Exceptions;
using Shared;

namespace SB.PublicInstitutions.Service
{
    public sealed class PublicInstitutionsService(IPublicInstitutionsRepository repository, ILogger<PublicInstitutionsService> logger) : IPublicInstitutionsService
    {

        public async Task<PublicInstitution>Create(PublicInstitutionDto institution)
        {
            var existingInstitution = await repository.GetByName(institution.Name);

            if (existingInstitution is not null)
            {
                logger.LogError($"Institution with Name {institution.Name} already exists" );
                throw new InstitutionNameExists(institution.Name);
            }

            var mappedInstitution = CopyValuesIntoModel<PublicInstitution, PublicInstitutionDto>(new PublicInstitution(), institution);
            var result = await repository.Create(mappedInstitution);
            logger.LogInformation("Created Public Institution");
            return result;
        }

        public async Task<bool> Delete(Guid id)
        {
            var institution = await repository.GetById(id);

            if (institution is null)
            {
                logger.LogError("Failed at finding Institution with ID" + id);
                throw new NotFoundException("Institution does not exist");
            }
            var result = await repository.Delete(id);
            logger.LogInformation("Deleted Institution with ID" + id);
            return result;
        }

        public async Task<IEnumerable<PublicInstitution>> GetAll()
        {
            logger.LogInformation("Get All Public Institutions");
            var institutions = await repository.GetAll();
            return institutions;
            //var ownersDto = institutions.Adapt<IEnumerable<OwnerDto>>();
            //return ownersDto;

        }


        public async Task<PublicInstitution> Update(Guid id, PublicInstitutionDto update)
        {
            var institution = await repository.GetById(id);

            if (institution is null)
            {
                logger.LogError("Failed at finding Institution with ID" + id);
                throw new NotFoundException("Institution does not exist");
            }

            var result = await repository.Update(id, CopyValuesIntoModel<PublicInstitution, PublicInstitutionDto>(new PublicInstitution(), update));
            logger.LogInformation("Update Public Institution");
            return result;
        }

        
        public static TOriginal CopyValuesIntoModel<TOriginal, TUpdate>(TOriginal originalObj, TUpdate updatedObj)
        {
            if (originalObj == null || updatedObj == null) return originalObj;
            foreach (var property in updatedObj.GetType().GetProperties())
            {
                var updatedValue = property.GetValue(updatedObj, null);
                if (updatedValue == null) continue;
                originalObj.GetType().GetProperty(property.Name)
                    .SetValue(originalObj, updatedValue, null);
            }
            return originalObj;
        }
    }
}
