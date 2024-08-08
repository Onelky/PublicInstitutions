using Microsoft.Extensions.Logging;
using SB.PublicInstitutions.Domain.Entities;
using SB.PublicInstitutions.Domain.Exceptions;
using Shared;

namespace SB.PublicInstitutions.Service
{
    public sealed class PublicInstitutionsService(IPublicInstitutionsRepository repository, ILogger<PublicInstitutionsService> logger) : IPublicInstitutionsService
    {

        public async Task<PublicInstitution>Create(PublicInstitutionCreateDto institution)
        {
            var mappedInstitution = CopyValuesIntoModel(new PublicInstitution(), institution);
            var result = await repository.Create(mappedInstitution);
            logger.LogInformation("Created Public Institution successfully");
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
            logger.LogInformation($"Deleted Institution {id} sucessfully");
            return result;
        }

        public async Task<IEnumerable<PublicInstitution>> GetAll()
        {
            var institutions = await repository.GetAll();
            logger.LogInformation("Get All Public Institutions");
            return institutions;
        }


        public async Task<PublicInstitution> Update(Guid id, PublicInstitutionUpdateDto update)
        {   
            var result = await repository.Update(id, CopyValuesIntoModel(new PublicInstitution(), update));
            logger.LogInformation("Update Public Institution");
            return result;
        }

        
        private TOriginal CopyValuesIntoModel<TOriginal, TUpdate>(TOriginal originalObj, TUpdate updatedObj)
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
