using KavifxApp.Models.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Win32;
using System.Net.Http;

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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            string Token = string.Empty;
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;

            LoginViewModel login = new LoginViewModel()
            {
                Email = model.Email,
                Password = model.Password
            };

            var ReqContent = JsonContent.Create(login);
            var response = await client.PostAsync(BaseUrl + "Account/Login", ReqContent);
            if (response.IsSuccessStatusCode)
            {
                Token = await response.Content.ReadAsStringAsync();
                var roles = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email,model.Email)                    
                };
               
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                HttpContext.Session.SetString("JWTToken", Token);
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel viewModel) 
        {
            string Token = HttpContext.Session.GetString("JWTToken");
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            RegisterViewModel register = new RegisterViewModel
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Password = viewModel.Password,
                ConfirmPassword = viewModel.ConfirmPassword
            };

            var Request = JsonContent.Create(register);
            var response = await client.PostAsync(BaseUrl + "Account/Register", Request);
            if (response.IsSuccessStatusCode)
            {
                var JsonString = await response.Content.ReadAsStringAsync();
                var User = JsonConvert.SerializeObject(JsonString);
                ViewBag.User = User;
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            string Token = HttpContext.Session.GetString("JWTToken");
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;
            if (!string.IsNullOrEmpty(Token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                var response = await client.GetAsync(BaseUrl + "User");
                if (response.IsSuccessStatusCode)
                {
                    var JsonString = await response.Content.ReadAsStringAsync();
                    var UserLst = JsonConvert.DeserializeObject<List<RegisterViewModel>>(JsonString);
                    ViewBag.Ust = UserLst;
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            string Token = HttpContext.Session.GetString("JWTToken");
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;

            var multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent(model.FirstName), "FirstName");
            multipartContent.Add(new StringContent(model.LastName), "LastName");
            multipartContent.Add(new StringContent(model.Email), "Email");
            multipartContent.Add(new StringContent(model.PhoneNumber), "PhoneNumber");
            multipartContent.Add(new StringContent(model.OrganizationName), "OrganizationName");
            multipartContent.Add(new StringContent(model.Location), "Location");
            multipartContent.Add(new StringContent(model.DateOfBirth), "DateOfBirth");
            multipartContent.Add(new StreamContent(model.ProfilePicture.OpenReadStream()), "file", model.ProfilePicture.FileName);

            var response = await client.PostAsync(BaseUrl + "", multipartContent);
            if (response.IsSuccessStatusCode)
            {
                // Success
                return RedirectToAction("Success");
            }
            else
            {
                // Handle error
                return RedirectToAction("Error");
            }
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
    }
}
