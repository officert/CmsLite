namespace CmsLite.Interfaces.Authentication
{
    public interface IAuthentication
    {
        void SignIn(string userName, bool createPersistantCookie);
        void SignOut();
        string CurrentUserName();
    }
}
