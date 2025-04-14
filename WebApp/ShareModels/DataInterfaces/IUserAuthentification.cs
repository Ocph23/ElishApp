using System.Threading.Tasks;

namespace ShareModels
{
    public interface IUserAuthentification
    {
        Task<AuthenticateResponse> Authenticate(UserLogin model);
        Task<object> Profile();
    }








}
