using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MSIT155.Models;
using MSIT155.Models.DTO;
using MSIT155Site.Models.DTO;
using System.IO;
using System.Linq;
using System.Text;

namespace MSIT155.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public ApiController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(a => a.City).Distinct();
            return Json(cities);
        }
        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(a => a.City==city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }
        public IActionResult Roads(string districts)
        {
            var roads = _context.Addresses.Where(a => a.SiteId == districts).Select(a => a.Road).Distinct();
            return Json(roads);
        }

        public IActionResult Index()
        {
            Thread.Sleep(3000);

            //int x = 10;
            //int y = 0;
            //int z = x / y;
            return Content("Content 你好!!", "text/plain", Encoding.UTF8);
        }

        //public IActionResult Register(string name, int age = 28)
        //public IActionResult Register(UserDTO _user)
        public IActionResult Register(Member _user,IFormFile Avatar)

        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            //return Content($"Hello {_user.Name}, {_user.Age}歲了, 電子郵件是 {_user.Email}", "text/plain", Encoding.UTF8);


            //for FileName
            string fileName = "empty.jpg";
            if (Avatar != null) { fileName =Avatar.FileName; }
            string uploadFile = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            using (var filestream = new FileStream(uploadFile, FileMode.Create))
            {
                Avatar?.CopyTo(filestream);
            }
            _user.FileName = fileName;

            //for FileData
            byte[] fileImg = null;
            using (var memoryStream = new MemoryStream())
            {
                Avatar?.CopyTo(memoryStream);
                fileImg = memoryStream.ToArray();
            }
            _user.FileData = fileImg;
            _context.Members.Add(_user);
            _context.SaveChanges();

            return Content($"{Avatar?.FileName} - {Avatar?.ContentType} - {Avatar?.Length}");

        }
        public IActionResult Avatar(int id = 1)
        {
            Member? member = _context.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                if (img != null)
                {
                    return File(img, "image/jpeg");
                }
            }

            return NotFound();
        }
        public IActionResult CheckAccountAction(UserDTO _user)
        {
            bool check = _context.Members.Any(a => a.Name == _user.Name);
            if (check)
                return Content("帳號已使用");
            else
                return Content("帳號可使用");
        }
        [HttpPost]
        public IActionResult Spots([FromBody] SearchDTO _search)
        {
            //搜尋CategoryId
            var spots = _search.CategoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s => s.CategoryId == _search.CategoryId);

            //KeyWord搜尋
            if (!string.IsNullOrEmpty(_search.Keyword))
            {
                spots = spots.Where(s => s.SpotTitle.Contains(_search.Keyword) || s.SpotDescription.Contains(_search.Keyword));
            }

            //排序方式
            switch (_search.SortBy)
            {
                case "spotTitle":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotTitle) : spots.OrderByDescending(s => s.SpotTitle);
                    break;
                case "categoryId":
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                default:
                    spots = _search.SortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }

            //分頁功能
            //總共幾筆資料
            int spotCount = spots.Count();
            //一頁幾筆資料
            int pagePerNumber = _search.PageSize ?? 9;
            //總共幾頁
            int pageCount = (int)Math.Ceiling((decimal)spotCount / pagePerNumber);
            //讀到第幾頁
            int pageNow = _search.Page ?? 1;
            //第幾頁秀出的資料
            spots = spots.Skip((pageNow - 1) * pagePerNumber).Take(pagePerNumber);

            SpotsPagingDTO spotsPaging= new SpotsPagingDTO();
            spotsPaging.TotalPages = pageCount;
            spotsPaging.TotalCount = spotCount;
            spotsPaging.SpotsResult=spots.ToList();


            return Json(spotsPaging);
        }

        public IActionResult SpotTitle(string keyword)
        {
            var titles = _context.Spots.Where(s=>s.SpotTitle.Contains(keyword)).Select(s=>s.SpotTitle).Take(8);
            return Json(titles);
        }
    }
}
