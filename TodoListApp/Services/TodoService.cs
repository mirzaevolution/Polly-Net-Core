using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TodoListApp.Models;

namespace TodoListApp.Services
{
    public class TodoService: ITodoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TodoService(IHttpClientFactory httpClientFactory)
        {
            this._httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<TodoViewModel>> GetTodos()
        {
            var client = _httpClientFactory.CreateClient("TodoApi");
            var response = await client.GetAsync("/todos");
            List<TodoViewModel> list = new List<TodoViewModel>();
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                list = JsonConvert.DeserializeObject<List<TodoViewModel>>(jsonData);
            }
            return list;
        }
    }
}
