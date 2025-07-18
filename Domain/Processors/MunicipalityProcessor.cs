﻿using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using log4net;
using Microsoft.Data.SqlClient;
using SupportLayer;
using SupportLayer.Models;

namespace Domain.Processors;

public class MunicipalityProcessor
{
    private IGenericRepository<Municipality> _repository;
    public string Error { get; set; } = null!;

    public MunicipalityProcessor()
    {
        _repository = new MunicipalityRepository();
    }

    private bool ValidateData(Municipality model)
    {
        MunicipalityValidator validator = new MunicipalityValidator();
        ValidationResult validationResult = validator.Validate(model);

        if (validationResult.IsValid)
        {
            return true;
        }
        else
        {
            Error = validationResult.Errors.First().ErrorMessage;
            return false;
        }
    }

    public bool SaveMunicipality(Municipality model)
    {
        if (ValidateData(model) == true)
        {
            try
            {
                if (model.Id == 0)
                {
                    _repository.Insert(model);
                }
                else
                {
                    _repository.Update(model);
                }
                return true;
            }
            // TODO: Refactor this error handling
            catch (Exception ex) when (ex.InnerException is SqlException sqlEx
                && sqlEx.Number == 2627)
            {
                ILog log = LogHelper.GetLogger();
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(model);
                log.Error("There was an error saving a Municipality record to the DB", ex);
                log4net.GlobalContext.Properties["Model"] = "";

                Error = "El registro que intentaste insertar ya existe.";
                return false;
            }
            catch (Exception ex)
            {
                ILog log = LogHelper.GetLogger();
                log4net.GlobalContext.Properties["Model"] = PropertyFormatter.FormatProperties(model);
                log.Error("There was an error saving a Municipality record to the DB", ex);
                log4net.GlobalContext.Properties["Model"] = "";

                Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return false;
            }
        }

        return false;
    }

    public void DeleteMunicipality(int id)
    {
        _repository.Remove(id);
    }

    public IEnumerable<Municipality> GetAllMunicipalities()
    {
        return _repository.GetAll().OrderBy(x => x.ProvinceName).ThenBy(x => x.Name);
    }
}
