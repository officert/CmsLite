using CmsLite.Interfaces.Authentication;

namespace CmsLite.Core.Areas.Admin.Models
{
    public class AdminModelFactory : IAdminModelFactory
    {
        private readonly ICmsLiteHttpContext _httpContext;

        public AdminModelFactory(ICmsLiteHttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public T Create<T>() where T : AdminContextModel, new()
        {
            var viewModel = new T();
            Initialize(viewModel);
            return viewModel;
        }

        public void Initialize<T>(T model) where T : AdminContextModel, new()
        {
            model.LoggedInUser = _httpContext.CurrentUser.Identity.Name;
        }
    }
}