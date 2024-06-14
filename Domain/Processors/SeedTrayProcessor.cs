using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

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
                    //LATER - Add the code to log the errors
                    Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    return false;
                }
            }
            return false;
        }

        public void DeleteSeedTray(int id)
        {
            _repository.Remove(id);
        }

        public IEnumerable<SeedTray> GetAllSeedTrays()
        {
            return _repository.GetAll();
        }

        public IEnumerable<SeedTray> GetActiveSeedTrays()
        {
            return _repository.GetAll().Where(x => x.Active == true)
                .OrderBy(x => x.Name);
        }

        public void CheckChangeInTheSelection(List<SeedTray> seedTrays)
        {
            var changedSeedTrays = seedTrays.Where(x => x.IsSelected != x.Selected);

            foreach (var st in changedSeedTrays)
            {
                st.Selected = st.IsSelected;
                this.SaveSeedTray(st);
            }
        }
    }
}
