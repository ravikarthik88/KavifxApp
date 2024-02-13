using KavifxApp.Models.DTO;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.Win32;
using System.Net.Http;
using System.Text;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace KavifxApp.Controllers
{
    public class AccountController : Controller
    {
        HttpClient client;
        IConfiguration config;
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment env;

        public AccountController(IHttpClientFactory factory, IConfiguration cfg, ILogger<HomeController> logger,
            IWebHostEnvironment hostEnvironment)
        {
            client = factory.CreateClient();
            config = cfg;
            _logger = logger;
            env = hostEnvironment;
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
            var response = await client.PostAsync(BaseUrl + "Auth/Login", ReqContent);
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
        public async Task<IActionResult> Register(RegisterViewModel viewModel) 
        {
            string Token = HttpContext.Session.GetString("JWTToken");
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            var register = new RegisterViewModel
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                Password = viewModel.Password,
                ConfirmPassword = viewModel.ConfirmPassword
            };
            var Request = JsonContent.Create(register);
            var response = await client.PostAsync(BaseUrl + "Auth/Register", Request);
            if (response.IsSuccessStatusCode)
            {
                var JsonString = await response.Content.ReadAsStringAsync();
                var User = JsonConvert.SerializeObject(JsonString);
                ViewBag.User = User;
            }
            return View();
        }


        [HttpGet]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Profile(UserProfileViewModel model)
        {
            string BaseUrl = config.GetSection("ApiUrl:Dev").Value;
            string filename = Path.GetFileName(model.ProfilePicture.FileName);
            string path = env.WebRootPath + "\\uploads\\Profile_Pictures\\";
            string imagePath = Path.Combine(path, filename);
            using (var client = new HttpClient())
            {
                try
                {
                    using(var memorystream = new MemoryStream())
                    {
                        await model.ProfilePicture.CopyToAsync(memorystream);
                        byte[] imageBytes = memorystream.ToArray();

                        using (var requestContent = new MultipartFormDataContent())
                        {
                            UserProfileViewModel user = new UserProfileViewModel
                            {
                                ProfilePictureUrl = imagePath,
                                Organization_Name = model.Organization_Name,
                                Location = model.Location,
                                PhoneNumber = model.PhoneNumber,
                                DateOfBirth = model.DateOfBirth,
                                UserId = model.UserId
                            };
                            var jsonData = JsonConvert.SerializeObject(user);

                            requestContent.Add(new ByteArrayContent(imageBytes), "file", model.ProfilePicture.FileName);
                            requestContent.Add(new StringContent(jsonData), "jsonData");

                            HttpResponseMessage responseMessage = await client.PostAsync(BaseUrl + "UserProfile/Profile", requestContent);
                            if (responseMessage.IsSuccessStatusCode)
                            {
                                _logger.LogInformation(responseMessage.StatusCode + " " + responseMessage.Content);
                                return RedirectToAction("Index", "Dashboard");

                            }
                            else
                            {
                                Console.WriteLine($"Failed to upload profile picture. Status code: {responseMessage.StatusCode}");
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            };
                return View();
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Home");
        }
    }
}
