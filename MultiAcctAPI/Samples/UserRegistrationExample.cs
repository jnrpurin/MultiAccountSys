using MultiAcctAPI.Models;
using Swashbuckle.AspNetCore.Filters;

public class UserRegistrationExample : IExamplesProvider<User>
{
    public User GetExamples()
    {
        return new User
        {
            Name = "Ademir",
            Email = "ademir@example.com",
            Password = "Teste@1234",
            Token = ""
        };
    }
}