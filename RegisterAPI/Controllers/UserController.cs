using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RegisterAPI.Infrastructure.Data.Dto;
using RegisterAPI.Infrastructure.Services;
using System.Collections.Immutable;


namespace RegisterAPI.Controllers
{
    [ApiController]
    //[Authorize]
    [Authorize(Policy = "ReadOnlyAccess")]
    [Route("api/[Controller]")]
    public class UserController : Controller
    {

        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public UserController()
        {
        }

        [HttpGet]
        //[Authorize(Roles = "Api.ReadOnly")] for role based access
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers(){

            var users = await  _userService.GetAllUsersAsync();

            return Ok(users);

        }

        [HttpPost]

        public async Task<ActionResult<UserDto>> RegisterUsers(UserRegistrationDto userRegistrationDto ) { 

            bool result = await _userService.RegisterUserAsync(userRegistrationDto);
            if (result)
            {
                return CreatedAtAction(nameof(GetUsers), new { id = userRegistrationDto.Email, userRegistrationDto });

            }
           
            return BadRequest();
          
        
        
        }

        [HttpGet("Fibonacci/{count}")]

        public async Task<ActionResult> GetFibonacciList( int count)
        {

            if (count <0) return BadRequest("COunt cannot be less than 0");

            var fibonacci = GenerateFibonacci(count);
            return Ok(fibonacci);



        }

        [HttpGet("SortArray")]
        public async Task<IActionResult> GetSortedArray(SortArrayRequest request)
        {
            if (request == null) return BadRequest(" arry must be provided");

            var sortedArray = request.IsAscending? request.Array.OrderBy(x => x).ToList(): 
                request.Array.OrderByDescending(x => x).ToList();

            return Ok(sortedArray);
        
        }




        private List<int> GenerateFibonacci(int count) { 
        
            List<int> fibonacci = new List<int> { 0, 1};

            for (int i = 2; i < count; i++) {

                fibonacci .Add( fibonacci[i - 1] + fibonacci[i - 2]);
            }

            return fibonacci.Take(count).ToList();
        }

        private int[] GetArray(int[] A, int K)
        {

                // Implement your solution here
                int n = A.Length;
                int[] B = new int[n];

                for (int i = 0; i < n; i++)

                {
                    B[(i + K) % n] = A[i];

                }
                return B;
            }

        private int GetUnMatched(int[] A)
        {
            int result = 0;
            foreach (int x in A)
            {
                result = result ^ x;
            }
            return result;

        }

        private int GetMissing(int[] A)
        {
            int len = A.Length;
            int result = 0;
            for (int i = 1; i <= len; i++)
            {
                if (Array.IndexOf(A, i) == -1)
                    result = i;

            }
            return result;
        }
        //public int GetPairSolution(int[] A)
        //{
        //    int pairCount = 0;  // Total number of pairs
        //    int zeroCount = 0;  // Running count of zeros

        //    foreach (int num in A)
        //    {
        //        if (num == 0)
        //        {
        //            zeroCount++; // Increment count of zeros
        //        }
        //        else if (num == 1)
        //        {
        //            pairCount += zeroCount; // Add pairs for each zero seen so far
        //            if (pairCount > 1000000000)
        //            {
        //                return -1; // Return -1 if pairs exceed 1 billion
        //            }
        //        }
        //    }

        //    return pairCount;
        //}



    }




    public class SortArrayRequest
        {
            public int[] Array { get; set; }
            public bool IsAscending { get; set; }
        }
    
}
