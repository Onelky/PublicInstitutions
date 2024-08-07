using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SB.PublicInstitutions.Domain.Entities;
using Serilog;
using Shared;

internal sealed class PublicInstitutionsRepository(ILogger logger) : IPublicInstitutionsRepository
{
    private readonly string _filePath = "public_institutions.txt";

    public async Task<PublicInstitution> Create(PublicInstitution institution)
    {
        try
        {
            var newInstitution = new PublicInstitution { Id = Guid.NewGuid() };
            var line = JsonConvert.SerializeObject(institution);
            await File.AppendAllLinesAsync(_filePath, new[] { line });
            return newInstitution;

        } catch (Exception ex)
        {
            logger.Error(ex, "Error creating an Institution");
            throw new Exception("Error creating an Institution", ex);
        }
        
    }

    public async Task<bool> Delete(Guid id)
    {
        try
        {
            var institutions = await GetAllInternal();
            var institution = institutions.FirstOrDefault(i => i.Id == id);
            if (institution == null)
            {
                return false;
            }

            institutions.Remove(institution);
            await WriteAllInstitutions(institutions);
            return true;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error deleting an Institution");
            throw new Exception("Error deleting an Institution", ex);
        }
    }

    public async Task<IEnumerable<PublicInstitution>> GetAll()
    {
        try
        {
            var institutions = await GetAllInternal();
            return institutions;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error getting all Institutions");
            throw new Exception("Error getting all Institutions", ex);
        }
    }

    public async Task<PublicInstitution> GetById(Guid id)
    {
        try
        {
            var institutions = await GetAllInternal();
            return institutions.FirstOrDefault(i => i.Id == id);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error getting Institution by Id");
            throw new Exception("Error getting Institution by Id", ex);
        }
    }

    public async Task<PublicInstitution> GetByName(string name)
    {
        try
        {
            var institutions = await GetAllInternal();
            return institutions.FirstOrDefault(i => i.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error getting Institution by Name");
            throw new Exception("Error getting Institution by Name", ex);
        }
    }

    public async Task<PublicInstitution> Update(Guid id, PublicInstitution data)
    {
        try
        {
            var institutions = await GetAllInternal();
            var institution = institutions.FirstOrDefault(i => i.Id == id);
            if (institution == null)
            {
                return null;
            }

            institution = CopyValuesIntoModel(institution, data);

            await WriteAllInstitutions(institutions);
            return institution;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error updating Institution");
            throw new Exception("Error updating Institution", ex);
        }
    }

    private async Task<List<PublicInstitution>> GetAllInternal()
    {
        if (!File.Exists(_filePath))
        {
            return new List<PublicInstitution>();
        }

        var lines = await File.ReadAllLinesAsync(_filePath);
        return lines.Select(line => JsonConvert.DeserializeObject<PublicInstitution>(line)).ToList();
    }

    private async Task WriteAllInstitutions(IEnumerable<PublicInstitution> institutions)
    {
        var lines = institutions.Select(JsonConvert.SerializeObject);
        await File.WriteAllLinesAsync(_filePath, lines);
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
                logger.Error(ex, $"Error updating property {property.Name}: {ex.Message}");
            }
        }

        return originalObj;
    }
}
