using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TodoListApp.Models;
using TodoListApp.Services;

namespace TodoListApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITodoService _todoService;
        public HomeController(
                ITodoService todoService,
                ILogger<HomeController> logger
            )
        {
            _todoService = todoService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _todoService.GetTodos();
            return View(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
