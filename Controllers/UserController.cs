using KavifxApp.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace KavifxApp.Controllers
{
    public class UserController : Controller
    {
        HttpClient client;
        IConfiguration config;
        string BaseUrl = "";
        public UserController(IHttpClientFactory factory, IConfiguration cfg)
        {
            client = factory.CreateClient();
            config = cfg;
            BaseUrl = config.GetSection("ApiUrl:Dev").Value;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetUsersAsync()
        {
            byte[] TokenBytes = null;
            if (HttpContext.Session.TryGetValue("JWTToken", out TokenBytes))
            {
                string Token = Encoding.UTF8.GetString(TokenBytes);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
                var response = await client.GetAsync(BaseUrl + "User");
                if (response.IsSuccessStatusCode)
                {
                    string JsonString = await response.Content.ReadAsStringAsync();
                    var UserList = JsonConvert.DeserializeObject<List<UserDTO>>(JsonString);

                    return Json(UserList, JsonRequestBehavior.AllowGet);
                }
            }

        }
    }
}
