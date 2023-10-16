using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next(); // We plan to do something AFTER the action, otherwise we would use the ActionExecutingContext as opposed to Delegate.

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return; //Likely unnecessary, we're already checking on the controller to make sure we're authenticated.

            var userId = resultContext.HttpContext.User.GetUserId();
            var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
            var user = await repo.GetUserByIdAsync(int.Parse(userId));
            user.LastActive = DateTime.UtcNow;

            await repo.SaveAllAsync();
        }
    }
}