namespace CmsLite.Services.Helpers
{
    public static class  CmsUrlHelper
    {
        public static string FormatUrlName(string name)
        {
            return name.Replace(" ", "").ToLower();
        }
    }
}
