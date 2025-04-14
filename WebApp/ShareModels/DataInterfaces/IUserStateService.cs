using System.Threading.Tasks;

namespace ShareModels
{
    public interface IUserStateService
    {
        AuthenticateResponse User { get; set; }
        Task<bool> Login(UserLogin model);
        Task Logout();
        Task Initialize();
    }








}
