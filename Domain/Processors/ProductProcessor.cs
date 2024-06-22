using DataAccess.Contracts;
using DataAccess.Repositories;
using Domain.Validators;
using FluentValidation.Results;
using SupportLayer.Models;

namespace Domain.Processors;

public class ProductProcessor
{
    private IProductRepository _repository;
    public string Error { get; set; } = null!;

    public ProductProcessor()
    {
        _repository = new ProductRepository();
    }

    private bool ValidateData(Product model)
    {
        ProductValidator validator = new ProductValidator();
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

    public bool SaveProduct(Product model)
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
                //NEXT - Add the code to log the errors
                Error = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return false;
            }
        }

        return false;
    }

    public void DeleteProduct(int id)
    {
        _repository.Remove(id);
    }

    public IEnumerable<Product> GetAllProducts()
    {
        return _repository.GetAll()
            .OrderBy(x => x.Specie.Name)
            .ThenBy(x => x.Variety);
    }

    public Product GetAProductById(int id)
    {
        Product output = ((ProductRepository)_repository).GetById(id);
        return output;
    }
}
