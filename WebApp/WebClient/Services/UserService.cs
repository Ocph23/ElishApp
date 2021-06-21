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
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace WebClient.Services
{

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDbContext ApplicationDbContext { get; }

        public UserService(IOptions<AppSettings> appSettings, ApplicationDbContext dbcontext, IHttpContextAccessor httpContextAccessor)
        {
            _context = dbcontext;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticateResponse> Authenticate(UserLogin model)
        {
            try
            {
                var password = GeneratePasswordHash(model.Password);
                var user = (from u in _context.User.Include(x=>x.Roles).ThenInclude(x=>x.Role).Where(x => (x.UserName == model.UserName || x.Email == model.UserName)
                          && x.PasswordHash == password)
                            select u).FirstOrDefault();
                if (user == null)
                    throw new SystemException($"Your Account {model.UserName} Not Found !");

                if (!user.Activated)
                    throw new SystemException($"Your Account {model.UserName} Not Active !");



                var token = await GenerateJwtToken(user);
                return new AuthenticateResponse(user, token);
            }
            catch (System.Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<User> FindUserById(int id)
        {
            var user = _context.User.Where(x => x.Id == id).Include(x=>x.Roles).ThenInclude(x=>x.Role).FirstOrDefault();
            return await Task.FromResult(user);
        }
        public async Task<User> FindUserByUserName(string username)
        {
            var user = _context.User.Where(x => x.UserName == username).Include(x=>x.Roles).FirstOrDefault();
            return await Task.FromResult(user);
        }

        public async Task<User> FindUserByEmail(string email)
        {
            var user = _context.User.Where(x => x.Email == email).Include(x => x.Roles).FirstOrDefault();
            return await Task.FromResult(user);
        }

        public async Task<string> AuthenticateUSerProvider(User user)
        {
            try
            {
                var token = await GenerateJwtToken(user);
                if (string.IsNullOrEmpty(token))
                    throw new SystemException("You Not Have Access");
                return token;

            }
            catch (System.Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        private Task<string> GenerateJwtToken(User user)
        {
            // generate token that is valid for 7 days

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var roles = "";
            foreach (var item in user.Roles)
            {
                    roles += $"{item.Role.Name}, ";
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("name", user.UserName),
                    new Claim("roles", roles)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Task.FromResult(tokenHandler.WriteToken(token));
        }

        public async Task<string> GenerateToken(User user)
        {
            return await GenerateJwtToken(user);
        }

        public async Task<User> Register(RegisterModel model)
        {
            try
            {
                User user = new User { Email = model.Email, Activated=true, UserName = model.UserName, PasswordHash = GeneratePasswordHash(model.Password) };
                if(user.Roles !=null && user.Roles.Count > 0)
                {
                    var roles = from a in model.Roles select new UserRole { Role=a, User=user };
                    user.Roles = roles.ToList();
                }
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                return await Task.FromResult(user);
            }
            catch (Exception ex)
            {
                throw new MySqlServiceException(ex);
            }
        }

        public async Task<Customer> RegisterCustomer(Customer model)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {

                if (string.IsNullOrEmpty(model.Email))
                {
                    string userName = GeneratePasswordHash($"Password{DateTime.Now.ToLongDateString()}");
                    model.Email = userName[0..5];
                }

                User user = new User { Email = model.Email, UserName = model.Email, PasswordHash = GeneratePasswordHash(model.Email), Activated=true };
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                var role = _context.Role.Where(x => x.Name == "customer").FirstOrDefault();
                _context.Userrole.Add(new UserRole { Role = role, User = user});
                model.User= user;
                _context.Customer.Add(model);
                await _context.SaveChangesAsync();
                trans.Commit();
                return model;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new MySqlServiceException(ex);
            }
        }

        public async Task<Karyawan> RegisterKaryawan(Karyawan model)
        {
            var trans = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(model.Email))
                {
                    string userName = GeneratePasswordHash($"Password{DateTime.Now.ToLongDateString()}");
                    model.Email = userName[0..5];
                }

                User user = new User { Email = model.Email, UserName = model.Email, PasswordHash = GeneratePasswordHash(model.Email), Activated=true };
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                model.User= user;
                var role = _context.Role.Where(x => x.Name == "Sales").AsNoTracking().FirstOrDefault();
                _context.Userrole.Add(new UserRole { Role= role, User= user});
                _context.Karyawan.Add(model);
                await _context.SaveChangesAsync();
                trans.Commit();
                return model;
            }
            catch (Exception ex)
            {
                trans.Rollback();
                throw new MySqlServiceException(ex);
            }
        }

        private static string GeneratePasswordHash(string password)
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
                var role = _context.Role.Where(x => x.Name == roleName).FirstOrDefault();
                if (role != null)
                {
                    _context.Userrole.Add(new UserRole { Role= role, User= user});
                    _context.SaveChanges();
                }
                return Task.FromResult(0);
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }


        [ApiAuthorize(Roles="Administrator")]
        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                var users = _context.User.Include(x => x.Roles);
                return await Task.FromResult(users.AsEnumerable());
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public async Task<object> Profile()
        {
            var userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                var user = await FindUserByUserName(userName);
                if(user != null)
                {
                    var role = user.Roles.FirstOrDefault();

                    if (role.Role.Name == "Administrator" || role.Role.Name == "Sales")
                    {
                        return _context.Karyawan.Where(x => x.User.Id == user.Id).FirstOrDefault();
                    }

                    if (role.Role.Name == "Customer")
                    {
                        return _context.Customer.Where(x => x.User.Id == user.Id).Include(x=>x.Karyawan).FirstOrDefault();
                    }
                }
            }
                throw new UnauthorizedAccessException("You Are Profile Not Found !");
        }
    }
}