using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Core.App_Start;
using CmsLite.Core.Cms.Configuration;
using CmsLite.Core.Cms.Razor;
using CmsLite.Core.Ioc;
using CmsLite.Interfaces.Content;
using Ninject;

namespace CmsLite.Core.Cms
{
    public class CmsApplication : HttpApplication
    {
        private readonly IKernel _kernel;
        private readonly Assembly _callingAssembly;
        private IFileManager _mvcFileManager;

        private DirectoryInfo _projectAdminAreaDir;             //where the static files we need to copy are located
        private DirectoryInfo _callingProjectAdminAreaDir;      //the admin area location of the project using the cms, where we need to copy the file to

        public CmsApplication()
        {
            _kernel = new StandardKernel();
            _callingAssembly = Assembly.GetCallingAssembly();
        }

        public void CmsInit()
        {
            IocConfig.Configure(_kernel);

            ControllerBuilder.Current.SetControllerFactory(new IocControllerFactory(_kernel));                                  //setup ninject as the default MVC controller factory

            //process and validate controllers, actions, and models
            _mvcFileManager = _kernel.Get<IFileManager>();

            _mvcFileManager.ProcessMvcFiles(_callingAssembly);

            AutoMapperConfiguration.Configure();

            SetupViewEngine();

            _projectAdminAreaDir = GetProjectAdminAreaDir();

            _callingProjectAdminAreaDir = GetCallingProjectAdminAreaDir();

            if (_callingProjectAdminAreaDir.Exists)
            {
                Directory.Delete(_callingProjectAdminAreaDir.FullName, true);   //recursive delete all file in dir
                _callingProjectAdminAreaDir.Create();
            }

            RouteConfig.RegisterRoutes(RouteTable.Routes);      //TODO: need to think more about what routes to add to a user's MVC project

            CopyViews();
            CopyScripts();
            CopyStylesheets();
        }

        private static void SetupViewEngine()
        {
            ViewEngines.Engines.Add(new EmbeddedResourceViewEngine());
        }

        private static DirectoryInfo GetProjectAdminAreaDir()
        {
            var currentProjectName = Path.GetFileName(Assembly.GetExecutingAssembly().Location).Replace(".dll", "");

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var baseparentDir = new DirectoryInfo(baseDir).Parent;

            var projectDir = baseparentDir.GetDirectories().FirstOrDefault(x => x.Name == currentProjectName);
            var areasDir = projectDir.GetDirectories().FirstOrDefault(x => x.Name == "Areas");
            var adminDir = areasDir.GetDirectories().FirstOrDefault(x => x.Name == "Admin");

            return adminDir;
        }

        private static DirectoryInfo GetCallingProjectAdminAreaDir()
        {
            var rootPath = HttpContext.Current.Server.MapPath("~/Areas/Admin/");

            return new DirectoryInfo(rootPath);
        }

        private void CopyViews()
        {
            var viewDir = _projectAdminAreaDir.GetDirectories().FirstOrDefault(x => x.Name == "Views");
            var callingProjectViewDir = new DirectoryInfo(_callingProjectAdminAreaDir.FullName + "Views");

            DirectoryCopy(viewDir.FullName, callingProjectViewDir.FullName, true);
        }

        private void CopyScripts()
        {
            var viewDir = _projectAdminAreaDir.GetDirectories().FirstOrDefault(x => x.Name == "Scripts");
            var callingProjectViewDir = new DirectoryInfo(_callingProjectAdminAreaDir.FullName + "Scripts");

            DirectoryCopy(viewDir.FullName, callingProjectViewDir.FullName, true);
        }

        private void CopyStylesheets()
        {
            var viewDir = _projectAdminAreaDir.GetDirectories().FirstOrDefault(x => x.Name == "Content");
            var callingProjectViewDir = new DirectoryInfo(_callingProjectAdminAreaDir.FullName + "Content");

            DirectoryCopy(viewDir.FullName, callingProjectViewDir.FullName, true);
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}
