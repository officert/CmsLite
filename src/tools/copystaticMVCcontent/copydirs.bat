REM this is a comment

SET fromRootDir=C:\Users\tofficer\Code\CmsLite\trunk\CmsLite.Web\Areas\Admin\
SET toRootDir=C:\Users\tofficer\Code\CmsLite\trunk\CmsLite.TestApp\Areas\Admin\

REM XCOPY %fromRootDir%Views %toRootDir%Views /S /O /X /E /H /K
XCOPY %fromRootDir%Scripts %toRootDir%Scripts /S /O /X /E /H /K
REM XCOPY %fromRootDir%Content %toRootDir%Content /S /O /X /E /H /K

PAUSE