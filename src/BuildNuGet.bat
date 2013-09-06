
SET BUILD=Release
SET PROJ=CmsLite.Web
SET NUGETTEMPDIR=tmpNuGet

MD %NUGETTEMPDIR%\%PROJ%\lib\net35

COPY %PROJ%\bin\CmsLite.Web.dll %NUGETTEMPDIR%\%PROJ%\lib\net35

COPY cms.nuspec %NUGETTEMPDIR%\%PROJ%

cd %NUGETTEMPDIR%\%PROJ%
nuget pack cms.nuspec

PAUSE