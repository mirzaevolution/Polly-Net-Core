using System.Collections.Generic;
using System.Threading.Tasks;
using TodoListApp.Models;

namespace TodoListApp.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoViewModel>> GetTodos();
    }
}