using webapi.Models;

namespace webapi.Services;

public class TestService
{
    public TestService()
    {
        
    }

    public IEnumerable<User> GetAll()
    {
        throw new NotImplementedException();
    }

    public User? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public User? Create(User newPizza)
    {
        throw new NotImplementedException();
    }

    public void AddTopping(int PizzaId, int ToppingId)
    {
        throw new NotImplementedException();
    }

    public void UpdateSauce(int PizzaId, int SauceId)
    {
        throw new NotImplementedException();
    }

    public void DeleteById(int id)
    {
        throw new NotImplementedException();
    }
}