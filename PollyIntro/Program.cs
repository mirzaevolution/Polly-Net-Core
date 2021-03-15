//https://medium.com/@therealjordanlee/retry-circuit-breaker-patterns-in-c-with-polly-9aa24c5fe23a#id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6ImZlZDgwZmVjNTZkYjk5MjMzZDRiNGY2MGZiYWZkYmFlYjkxODZjNzMiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJuYmYiOjE2MTQ1MjU2ODAsImF1ZCI6IjIxNjI5NjAzNTgzNC1rMWs2cWUwNjBzMnRwMmEyamFtNGxqZGNtczAwc3R0Zy5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsInN1YiI6IjExMzkwNjY2Mjk1NzIzNzQyMjYxMSIsImVtYWlsIjoiZ2h1bGFtLnJhc3lpZEBnbWFpbC5jb20iLCJlbWFpbF92ZXJpZmllZCI6dHJ1ZSwiYXpwIjoiMjE2Mjk2MDM1ODM0LWsxazZxZTA2MHMydHAyYTJqYW00bGpkY21zMDBzdHRnLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwibmFtZSI6Ik1pcnphIEdodWxhbSBSYXN5aWQiLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EtL0FPaDE0R2gya2RyV2lqc1RzZS1oSnJMeEdNZFI2cHQzTGZULWoyWUZNUGZ6N2c9czk2LWMiLCJnaXZlbl9uYW1lIjoiTWlyemEgR2h1bGFtIiwiZmFtaWx5X25hbWUiOiJSYXN5aWQiLCJpYXQiOjE2MTQ1MjU5ODAsImV4cCI6MTYxNDUyOTU4MCwianRpIjoiZGFiZTZiYWNkNzgyNzBiMDMwMjdiNGIzZTI0YzA5YjUzYjJkZTE1NCJ9.GxlhPL90UUEcD0hqbgUzyyFyQo6GavRB5C_zJYbd_TDl8x0NNo8Ul0enblVtc_4bSAsd5nfT8oulhD1er7oLjT2i34gQJSSh1pt_NYp3TY7DD13wQlnMXl3bcjC1ZjH42oNB8zgyHX7UkTqnQNZUDeoqS2J-Jhqbjs01S6QntxRf7pCits0rS95y9x-P0Bf73fUWSt3sDN5rg9heQ68P60Lxpx26AB_isFZPM02CdrtOLROVwAI3CXjlIu5vdSeswxfUPszJhjmiG7XyKz5U4iTdQ6PfL1BXEMPu_7rLyyEm2kE43VavQeSWrfz7QK5qeEbI2EX4Rq0ZgAyi_eeZTg

using System;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using Polly.CircuitBreaker;
using System.Threading;

namespace PollyIntro
{
    class Program
    {
        static Task<string> ReverseString(string value, bool noRandom=false)
        {
            Random random = new Random();
            int randomNumber = random.Next(1, 101);
            if (!noRandom && randomNumber % 3 ==0)
                throw new InvalidOperationException("Transient failure occured");
            return Task.FromResult(new string(value.ToCharArray().Reverse().ToArray()));
        }
        static async Task RetryPolicy()
        {
            AsyncRetryPolicy<string> asyncRetryPolicy = Policy<string>
                .Handle<InvalidOperationException>()
                .WaitAndRetryAsync(3, (retryCount) =>
                {
                    Console.WriteLine($"Retry number: {retryCount}");
                    return TimeSpan.FromSeconds(2);
                });
            try
            {

                string data = "Hello World";
                string result = await asyncRetryPolicy.ExecuteAsync(async () => await ReverseString(data));
                Console.WriteLine($"[Retry] Original string: {data}");
                Console.WriteLine($"[Retry] Reversed string: {result}");
            }
            catch(InvalidOperationException ex)
            {
                Console.WriteLine($"Transient failure count exceeds maximum defined retry-count!");
                Console.WriteLine("Error detail:");
                Console.WriteLine(ex.ToString());
            }
        }
        static void CircuitBreakerPolicyWith2ConsecutiveCalls()
        {
            AsyncCircuitBreakerPolicy<string> asyncCircuitBreakerPolicy = Policy<string>
                .Handle<InvalidOperationException>()
                .CircuitBreakerAsync(2,TimeSpan.FromSeconds(10),
                (exception, timespan) =>
                {
                    Console.WriteLine("On Break!");
                },() =>
                {
                    Console.WriteLine("On Reset!");
                });

            string data = "Hello World";

            Console.WriteLine($"[CircuitBreaker] Original string: {data}");
            try
            {
                string result = asyncCircuitBreakerPolicy.ExecuteAsync(async() => await ReverseString(data)).Result;
                Console.WriteLine($"[CircuitBreaker] Reversed string 1: {result}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot handle InvalidOperationException!");
                Console.WriteLine("Error detail:");
                Console.WriteLine(ex.ToString());
            }

            try
            {
                string result2 = asyncCircuitBreakerPolicy.ExecuteAsync(async () => await ReverseString(data,true)).Result;
                Console.WriteLine($"[CircuitBreaker] Reversed string 2: {result2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot handle InvalidOperationException!");
                Console.WriteLine("Error detail:");
                Console.WriteLine(ex.ToString());
            }
        }
        static void CircuitBreakerPolicyWith2ConsecutiveCallsWithBlockingThread()
        {
            AsyncCircuitBreakerPolicy<string> asyncCircuitBreakerPolicy = Policy<string>
                .Handle<InvalidOperationException>()
                .CircuitBreakerAsync(1, TimeSpan.FromSeconds(10), //can handle 1 event only
                (exception, timespan) =>
                {
                    Console.WriteLine("On Break!");
                }, () =>
                {
                    Console.WriteLine("On Reset!");
                });

            string data = "Hello World";

            Console.WriteLine($"[CircuitBreaker] Original string: {data}");
            try
            {
                string result = asyncCircuitBreakerPolicy.ExecuteAsync(async () => await ReverseString(data)).Result;
                Console.WriteLine($"[CircuitBreaker] Reversed string 1: {result}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot handle InvalidOperationException!");
                Console.WriteLine("Error detail:");
                Console.WriteLine(ex.ToString());
            }

            Thread.Sleep(10000); //wait around 10 secs

            try
            {
                string result2 = asyncCircuitBreakerPolicy.ExecuteAsync(async () => await ReverseString(data, true)).Result;
                Console.WriteLine($"[CircuitBreaker] Reversed string 2: {result2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Cannot handle InvalidOperationException!");
                Console.WriteLine("Error detail:");
                Console.WriteLine(ex.ToString());
            }
        }
        static void Main(string[] args)
        {
            //RetryPolicy().Wait();
            CircuitBreakerPolicyWith2ConsecutiveCalls();
            //CircuitBreakerPolicyWith2ConsecutiveCallsWithBlockingThread();
            Console.ReadLine();
        }
    }
}
