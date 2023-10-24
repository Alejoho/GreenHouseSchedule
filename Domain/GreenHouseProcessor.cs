using DataAccess.Contracts;
using DataAccess.Repositories;
using SupportLayer.Models;

namespace Domain
{
    public class GreenHouseProcessor
    {
        private IGreenHouseRepository repository;
        private List<string> errors;

        public string FirstError
        {
            get
            {
                string error = errors.FirstOrDefault("No hay error");
                errors.Clear();
                return error;
            }
        }

        public GreenHouseProcessor()
        {
            repository = new GreenHouseRepository();
            errors = new List<string>();
        }

        private bool ValidateData(GreenHouse model)
        {
            //TODO - Create the method to validate the model
            //throw new NotImplementedException();
            return true;
        }

        public bool SaveGreenHouse(GreenHouse model)
        {
            if (ValidateData(model) == true)
            {
                if (model.Id == 0)
                {
                    repository.Insert(model);
                }
                else
                {
                    repository.Update(model);
                }
                return true;
            }
            return false;
        }

        public void DeleteGreenHouse(GreenHouse model)
        {
            //TODO - Create the delete method for green houses
            throw new NotImplementedException();
        }

        public IEnumerable<GreenHouse> GetAllGreenHouses()
        {
            //TODO - Create the GetAll method for green houses
            //throw new NotImplementedException();
            return repository.GetAll();
        }
    }
}
