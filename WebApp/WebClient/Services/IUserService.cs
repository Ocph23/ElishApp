using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebClient.Models;
using System.Security.Cryptography;
using ShareModels;
using WebClient;

namespace WebClient.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(UserLogin model);
        Task<User> FindUserById(int id);
        Task<string> AuthenticateUSerProvider(User user);
        Task<string> GenerateToken(User user);
        Task<User> Register(RegisterModel user);
        Task AddUserRole(string v, User admin);
        Task<Customer> RegisterCustomer(Customer value);
        Task<Karyawan> RegisterKaryawan(Karyawan value);

        Task<User> FindUserByUserName(string userName);
        Task<User> FindUserByEmail(string email);
    }

    public class UserService : IUserService
    {
        private readonly OcphDbContext _context;

        private readonly AppSettings _appSettings;

        public OcphDbContext OcphDbContext { get; }

        public UserService(IOptions<AppSettings> appSettings, OcphDbContext dbcontext)
        {
            _context = dbcontext;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> Authenticate(UserLogin model)
        {
            try
            {
                var password = GeneratePasswordHash(model.Password);
                var user = (from u in _context.Users.Where(x => (x.UserName == model.UserName || x.Email == model.UserName)
                          && x.PasswordHash == password)
                            select u).FirstOrDefault();
                if (user == null)
                    throw new SystemException($"Your Account {model.UserName} Not Found !");

                if (!user.Activated)
                    throw new SystemException($"Your Account {model.UserName} Not Active !");

                var roles = from ur in _context.UserRoles.Where(x => x.UserId == user.Id)
                            join c in _context.Roles.Select() on ur.RoleId equals c.Id into rGroup
                            from c in rGroup.DefaultIfEmpty()
                            select c;

                user.Roles = roles.ToList();

                var token = await generateJwtToken(user);
                return new AuthenticateResponse(user, token);
            }
            catch (System.Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<User> FindUserById(int id)
        {
            var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
            if (user != null)
            {
                var roles = from ur in _context.UserRoles.Where(x => x.UserId == user.Id)
                            join c in _context.Roles.Select() on ur.RoleId equals c.Id into rGroup
                            from c in rGroup.DefaultIfEmpty()
                            select c;

                user.Roles = roles.ToList();
            }

            return await Task.FromResult(user);
        }
        public async Task<User> FindUserByUserName(string username)
        {
            var user = _context.Users.Where(x => x.UserName == username).FirstOrDefault();
            if (user != null)
            {
                var roles = from ur in _context.UserRoles.Where(x => x.UserId == user.Id)
                            join c in _context.Roles.Select() on ur.RoleId equals c.Id into rGroup
                            from c in rGroup.DefaultIfEmpty()
                            select c;

                user.Roles = roles.ToList();
            }

            return await Task.FromResult(user);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            var user = _context.Users.Where(x => x.Email == email).FirstOrDefault();
            if (user != null)
            {
                var roles = from ur in _context.UserRoles.Where(x => x.UserId == user.Id)
                            join c in _context.Roles.Select() on ur.RoleId equals c.Id into rGroup
                            from c in rGroup.DefaultIfEmpty()
                            select c;

                user.Roles = roles.ToList();
            }

            return await Task.FromResult(user);
        }

        public async Task<string> AuthenticateUSerProvider(User user)
        {
            try
            {
                var token = await generateJwtToken(user);
                if (string.IsNullOrEmpty(token))
                    throw new SystemException("You Not Have Access");
                return token;

            }
            catch (System.Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        private Task<string> generateJwtToken(User user)
        {
            // generate token that is valid for 7 days

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("name", user.UserName),
                    new Claim("roles", user.Roles.Select(x=>x.Name).ToArray().ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }

        public async Task<string> GenerateToken(User user)
        {
            return await generateJwtToken(user);
        }

        public async Task<User> Register(RegisterModel model)
        {
            try
            {
                User user = new User { Roles = model.Roles, Email = model.Email, UserName = model.UserName, PasswordHash = GeneratePasswordHash(model.Password) };
                user.Id = _context.Users.InsertAndGetLastID(user);
                if (user.Id <= 0)
                    throw new SystemException("Register Not Success");

                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                throw new MySqlServiceException(ex);
            }
        }

        public Task<Customer> RegisterCustomer(Customer model)
        {
            var trans = _context.BeginTransaction();
            try
            {
                User user = new User { Email = model.Email, UserName = model.Email, PasswordHash = GeneratePasswordHash(model.Email) };
                user.Id = _context.Users.InsertAndGetLastID(user);
                if (user.Id <= 0)
                    throw new SystemException("Register Not Success !");

                var role = _context.Roles.Where(x => x.Name == "customer").FirstOrDefault();
                _context.UserRoles.Insert(new Userrole { RoleId = role.Id, UserId = user.Id });

                model.UserId = user.Id;

                model.Id = _context.Customers.InsertAndGetLastID(model);
                if (model.Id <= 0)
                    throw new SystemException("Customer Not Created !");

                trans.Commit();
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new MySqlServiceException(ex);
            }
        }

        public Task<Karyawan> RegisterKaryawan(Karyawan model)
        {
            var trans = _context.BeginTransaction();
            try
            {
                User user = new User { Email = model.Email, UserName = model.Email, PasswordHash = GeneratePasswordHash(model.Email) };
                user.Id = _context.Users.InsertAndGetLastID(user);
                if (user.Id <= 0)
                    throw new SystemException("Register Not Success !");

                var role = _context.Roles.Where(x => x.Name == "customer").FirstOrDefault();
                _context.UserRoles.Insert(new Userrole { RoleId = role.Id, UserId = user.Id });

                model.UserId = user.Id;

                model.Id = _context.Karyawans.InsertAndGetLastID(model);
                if (model.Id <= 0)
                    throw new SystemException("Karyawan Not Created !");

                trans.Commit();
                return Task.FromResult(model);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new MySqlServiceException(ex);
            }
        }

        private string GeneratePasswordHash(string password)
        {

            if (string.IsNullOrEmpty(password))
                throw new SystemException("Password Requeired !");

            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(password));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }

        public Task AddUserRole(string roleName, User user)
        {
            try
            {
                var role = _context.Roles.Where(x => x.Name == roleName).FirstOrDefault();
                if (role != null)
                {
                    _context.UserRoles.Insert(new Userrole { RoleId = role.Id, UserId = user.Id });
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


    }
}