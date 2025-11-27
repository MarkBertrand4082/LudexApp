//Mark Bertrand I made this but we're probably not gonna use it
using LudexApp.Models;
using LudexApp.Repositories.Implementation;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LudexApp.Controllers.APIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public Task<IEnumerable<User?>> Get() => _userRepository.GetUsersAsync();

        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            return Ok(_userRepository.GetUserByIdAsync(id));
        }
    }
}
