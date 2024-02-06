using KavifxApp.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KavifxApp.Controllers
{
    public class AccountController : Controller
    {
        HttpClient client;
        IConfiguration config;
        private readonly ILogger<HomeController> _logger;
        public AccountController(IHttpClientFactory factory, IConfiguration cfg, ILogger<HomeController> logger)
        {
            client = factory.CreateClient();
            config = cfg;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            string Token = string.Empty;
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;
            LoginDTO inpDTO = new LoginDTO()
            {
                Email = login.Email,
                Password = login.Password
            };
            
            var ReqContent = JsonContent.Create(inpDTO);
            var response = await client.PostAsync(BaseUrl + "Account/Login", ReqContent);
            if (response.IsSuccessStatusCode)
            {
                Token = await response.Content.ReadAsStringAsync();
                if (Token != null && Token.Length != 0)
                {
                    var responseRoles = await client.GetAsync(BaseUrl + "UserRole");
                    if(responseRoles.IsSuccessStatusCode)
                    {
                        string resp = await responseRoles.Content.ReadAsStringAsync();
                        if (resp != null && resp.Length != 0)
                        {
                            List<UserRoleDTO> users = JsonConvert.DeserializeObject<List<UserRoleDTO>>(resp);
                            foreach(var user in users) 
                            {
                                if (user.Email == login.Email)
                                {
                                    var claims = new List<Claim>
                                    { 
                                        new Claim(ClaimTypes.Email, user.Email),
                                        new Claim(ClaimTypes.Role, user.RoleName),
                                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                                    };

                                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var principal = new ClaimsPrincipal(identity);
                                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                                    HttpContext.Session.SetString("JWTToken", Token);
                                    return RedirectToAction("Index", "Dashboard");
                                }
                            }
                        }
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserDTO request)
        {
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;
            UserDTO inpDTO = new UserDTO()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword
            };
            var ReqContent = JsonContent.Create(inpDTO);
            var response = await client.PostAsync(BaseUrl + "Account/Register", ReqContent);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }      
    }
}
