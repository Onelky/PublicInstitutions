using SB.PublicInstitutions.Domain.Entities;
using SB.PublicInstitutions.Domain.Exceptions;
using Serilog;
using Shared;

namespace SB.PublicInstitutions.Service
{
    internal sealed class PublicInstitutionsService(IPublicInstitutionsRepository repository, ILogger logger) : IPublicInstitutionsService
    {

        public async Task<PublicInstitution>Create(PublicInstitutionDto institution)
        {
            var existingInstitution = await repository.GetByName(institution.Name);

            if (existingInstitution is not null)
            {
                logger.Error($"Institution with Name {institution.Name} already exists" );
                throw new InstitutionNameExists($"Institution with Name {institution.Name} already exists");
            }

            var mappedInstitution = CopyValuesIntoModel<PublicInstitution, PublicInstitutionDto>(new PublicInstitution(), institution);
            var result = await repository.Create(mappedInstitution);
            logger.Information("Created Public Institution");
            return result;
        }

        public async Task<bool> Delete(Guid id)
        {
            var institution = await repository.GetById(id);

            if (institution is null)
            {
                logger.Error("Failed at finding Institution with ID" + id);
                throw new NotFoundException("Institution does not exist");
            }
            var result = await repository.Delete(id);
            logger.Information("Deleted Institution with ID" + id);
            return result;
        }

        public async Task<IEnumerable<PublicInstitution>> GetAll()
        {
            logger.Information("Get All Public Institutions");
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
                logger.Error("Failed at finding Institution with ID" + id);
                throw new NotFoundException("Institution does not exist");
            }

            var result = await repository.Update(id, CopyValuesIntoModel<PublicInstitution, PublicInstitutionDto>(new PublicInstitution(), update));
            logger.Information("Update Public Institution");
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
