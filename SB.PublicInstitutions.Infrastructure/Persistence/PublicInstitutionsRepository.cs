using Newtonsoft.Json;
using SB.PublicInstitutions.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.PublicInstitutions.Infrastructure.Models;
using SB.PublicInstitutions.Infrastructure.Utils;
using SB.PublicInstitutions.Domain.Exceptions;

public sealed class PublicInstitutionsRepository(ILogger<PublicInstitutionsRepository> logger, IOptions<DatabasePaths> options) : IPublicInstitutionsRepository
{
    private readonly string filePath = options.Value.PublicInstitutions;

    public async Task<List<PublicInstitution>> GetAll()
    {
        try
        {
            return await FileUtils.GetDeserializedItems<PublicInstitution>(filePath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all Institutions");
            throw new Exception("Error getting all Institutions", ex);
        }
    }

    public async Task<PublicInstitution> GetById(Guid id)
    {
        try
        {
            var institutions = await GetAll();
            return institutions.FirstOrDefault(i => i.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Institution by Id");
            throw new Exception("Error getting Institution by Id", ex);
        }
    }

    public async Task<PublicInstitution> GetByName(string name)
    {
        try
        {
            var institutions = await GetAll();
            return institutions.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Institution by Name");
            throw new Exception("Error getting Institution by Name", ex);
        }
    }


    public async Task<PublicInstitution> Create(PublicInstitution institution)
    {
        
        var existingInstitution = await GetByName(institution.Name);

        if (existingInstitution is not null)
        {
            logger.LogError($"Attempted to create institution with registered name");
            throw new DuplicatedInstitutionName(institution.Name);
        }

        try
        {
            var newInstitution = institution;
            newInstitution.CreationDate = DateTime.Now;
            newInstitution.Id = Guid.NewGuid();

            await FileUtils.AppendLine(newInstitution, filePath);

            return newInstitution;

        } catch (Exception ex)
        {
            logger.LogError(ex, "Error creating an Institution");
            throw new Exception(ex.Message);
        }
        
    }


    public async Task<bool> Delete(Guid id)
    {
        var institutions = await GetAll();
        var institution = institutions.FirstOrDefault(i => i.Id == id);

        if (institution is null)
        {
            logger.LogError("Error finding Institution with ID" + id);
            throw new NotFoundException("Institution does not exist");
        }

        try
        {

            institutions.Remove(institution);
            await FileUtils.WriteAll(institutions, filePath);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting an Institution");
            throw new Exception("Error deleting an Institution", ex);
        }
    }

   
    public async Task<PublicInstitution> Update(Guid id, PublicInstitution data)
    {
        var institutions = await GetAll();
        var savedInstitution = institutions.FirstOrDefault(i => i.Id == id);

        if (savedInstitution is null)
        {
            logger.LogError("Error finding Institution with ID" + id);
            throw new NotFoundException("Institution does not exist");
        }

        var institutionByName = institutions.FirstOrDefault(i => i.Name == data.Name);

        if (institutionByName is not null)
        {
            logger.LogError($"Attempted to change institution's name to a registered one");
            throw new DuplicatedInstitutionName(savedInstitution.Name);
        }

        try
        {
            savedInstitution = CopyValuesIntoModel(savedInstitution, data);

            await FileUtils.WriteAll(institutions, filePath);
            return savedInstitution;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            throw new Exception("Error updating Institution", ex);
        }
    }

    private T CopyValuesIntoModel<T>(T originalObj, T updatedObj) where T : class
    {

        if (originalObj == null || updatedObj == null) return originalObj;

        var originalType = originalObj.GetType();
        var updatedType = updatedObj.GetType();

        foreach (var property in updatedType.GetProperties())
        {
            try
            {
                if (property.Name.Equals("Id")) continue;

                var updatedValue = property.GetValue(updatedObj, null);
                if (updatedValue == null) continue;

                var originalProperty = originalType.GetProperty(property.Name);
                if (originalProperty != null && originalProperty.CanWrite)
                {
                    originalProperty.SetValue(originalObj, updatedValue, null);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error updating property {property.Name}: {ex.Message}");
            }
        }

        return originalObj;
    }
}
