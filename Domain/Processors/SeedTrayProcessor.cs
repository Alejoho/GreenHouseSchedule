using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Processors
{
    public class SeedTrayProcessor
    {
        private ISeedTrayRepository _repository;
        public string Error { get; set; } = null!;

        public SeedTrayProcessor()
        {
            _repository = new SeedTrayRepository();
        }

        private bool ValidateData(SeedTray model)
        {
            SeedTrayValidator validator = new SeedTrayValidator();
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

        public bool SaveSeedTray(SeedTray model)
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
                catch (Exception ex)
                {
                    //TODO - Add the code to log the errors
                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteSeedTray(SeedTray model)
        {
            //TODO - Create the delete method for seed trays
            throw new NotImplementedException();
        }

        public IEnumerable<SeedTray> GetAllSeedTrays()
        {
            return _repository.GetAll();
        }

        public int GetNumber()
        {            
            int.TryParse(ConfigurationManager.AppSettings["MinimumLimitOfSow"],out int output);
            return output;
        }
    }
}
