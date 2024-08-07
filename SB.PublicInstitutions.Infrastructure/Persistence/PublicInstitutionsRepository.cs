using Newtonsoft.Json;
using SB.PublicInstitutions.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.PublicInstitutions.Infrastructure.Models;
using SB.PublicInstitutions.Infrastructure.Utils;

public sealed class PublicInstitutionsRepository(ILogger<PublicInstitutionsRepository> logger, IOptions<DatabasePaths> options) : IPublicInstitutionsRepository
{
    private readonly string filePath = options.Value.PublicInstitutions;

    public async Task<PublicInstitution> Create(PublicInstitution institution)
    {
        try
        {
            var newInstitution = institution;
            newInstitution.CreationDate = DateTime.Now;
            newInstitution.Id = Guid.NewGuid();

            var line = JsonConvert.SerializeObject(newInstitution);
            await File.AppendAllLinesAsync(filePath, new[] { line });
            return newInstitution;

        } catch (Exception ex)
        {
            logger.LogError(ex, "Error creating an Institution");
            throw new Exception("Error creating an Institution", ex);
        }
        
    }

    public async Task<bool> Delete(Guid id)
    {
        try
        {
            var institutions = await FileUtils.GetDeserializedItems<PublicInstitution>(filePath);

            var institution = institutions.FirstOrDefault(i => i.Id == id);
            if (institution == null)
            {
                return false;
            }

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

    public async Task<IEnumerable<PublicInstitution>> GetAll()
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
            var institutions = await FileUtils.GetDeserializedItems<PublicInstitution>(filePath);
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
            var institutions = await FileUtils.GetDeserializedItems<PublicInstitution>(filePath);
            return institutions.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting Institution by Name");
            throw new Exception("Error getting Institution by Name", ex);
        }
    }

    public async Task<PublicInstitution> Update(Guid id, PublicInstitution data)
    {
        try
        {
            var institutions = await FileUtils.GetDeserializedItems<PublicInstitution>(filePath);
            var institution = institutions.FirstOrDefault(i => i.Id == id);
            if (institution == null)
            {
                return null;
            }

            institution = CopyValuesIntoModel(institution, data);

            await FileUtils.WriteAll(institutions, filePath);
            return institution;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating Institution");
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
