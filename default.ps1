properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  #Directories
  $base_dir = resolve-path .
  $source_dir = "$base_dir\src"
  $database_dir = "$base_dir\database"
  $tools_dir = "$base_dir\tools"
  $nuget_dir = "$base_dir\nuget"
  $build_dir = "$base_dir\build"
  #Project settings
  $projectName = 'CmsLite'
  #Database properties
  $databaseServer = '.'
  $databaseName = 'cmslite_local'
  #NuGet settings
  $nuget_tempdir = "$base_dir\nuget-temp"
  $nuget_packageName = 'CmsLiteCore'
  $nuget_version = '6.0'
}

task default -depends Test

task Test -depends Compile, Clean { 
  $testMessage
}

task Compile -depends Clean { 
  $compileMessage
}

task Clean { 
  $cleanMessage
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}

# ------------------------------------ Database Tasks ------------------------------- 

task CreateUsers {
	cd $tools_dir\sqlcmd\
	#& $tools_dir\sqlcmd\sqlcmd.exe -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteTestUsers.sql
	#& $tools_dir\sqlcmd\sqlcmd.exe -E -S $databaseServer -d $databaseName -i $database_dir\scripts\LoadTestUsers.sql

	& sqlcmd -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteTestUsers.sql
	& sqlcmd -E -S $databaseServer -d $databaseName -i $database_dir\scripts\LoadTestUsers.sql
}

task DeleteData {
	cd $tools_dir\sqlcmd\
	#& $tools_dir\sqlcmd\sqlcmd.exe -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteData.sql
	& sqlcmd -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteData.sql
}
task DeleteIntData {
	cd $tools_dir\sqlcmd\
	#& $tools_dir\sqlcmd\sqlcmd.exe -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteData.sql
	& sqlcmd -E -S $databaseServer -d "cmslite_integration" -i $database_dir\scripts\DeleteData.sql
}

# ------------------------------------ NuGet Tasks ---------------------------------- 

task Nuget-Pack {
	#this link show an example for packing multiple NuGet spec files : https://github.com/hibernating-rhinos/rhino-esb/blob/master/default.ps1
	
	#create new directory
	New-Item $nuget_tempdir\$nuget_packageName\lib\net35 -type directory -force
	#copy CmsLite.Core.dll to that dir
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Core.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Data.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Domains.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Interfaces.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Services.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Utilities.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Resources.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	#copy admin content to content dir
	Copy-Item $source_dir\cmslite.core\Areas\Admin\Content $nuget_tempdir\$nuget_packageName\content\Areas\Admin\Content -recurse -force
	Copy-Item $source_dir\cmslite.core\Areas\Admin\Views $nuget_tempdir\$nuget_packageName\content\Areas\Admin\Views -recurse -force
	Copy-Item $source_dir\cmslite.core\Areas\Admin\Scripts $nuget_tempdir\$nuget_packageName\content\Areas\Admin\Scripts -recurse -force
	#copy cms.nuspec file to that dir, two levels higher though
	Copy-Item $nuget_dir\cms.nuspec $nuget_tempdir\$nuget_packageName -force
	
	cd $nuget_tempdir\$nuget_packageName
	
	& $tools_dir\nuget\nuget.exe pack $nuget_tempdir\$nuget_packageName\cms.nuspec
}

# ------------------------------------ Tasks ---------------------------------------- 

task CopyAllMvc -depends CopyViews, CopyScripts, CopyContent {
}

task CopyViews {
	Copy-Item $source_dir\CmsLite.Core\Areas\Admin\Views $source_dir\CmsLite.TestApp\Areas\Admin -force -recurse
}

task CopyScripts {
	Copy-Item $source_dir\CmsLite.Core\Areas\Admin\Scripts $source_dir\CmsLite.TestApp\Areas\Admin -force -recurse
}

task CopyContent {
	Copy-Item $source_dir\CmsLite.Core\Areas\Admin\Content $source_dir\CmsLite.TestApp\Areas\Admin -force -recurse
}

task DeleteAllMvc {
	Remove-Item $source_dir\CmsLite.TestApp\Areas\Admin -force -recurse
}

# ------------------------------------ Build Tasks ---------------------------------- 

task CreateAdminZip {
	New-Item $build_dir\build-artifacts -type directory -force

	#Copy-Item $source_dir\cmslite.core\Areas\Admin $build_dir\build-artifacts\Admin -recurse -force

	ZipFiles $build_dir\build-artifacts\Admin.zip $source_dir\cmslite.core\Areas\Admin
}

task CompileJs {
	$outputdir = "$build_dir\javascript"

	$sourcedir = "$source_dir\CmsLite.Core\Areas\Admin\Scripts\source\hello.js"

	New-Item $outputdir -type directory -force

	$java = "$tools_dir\java"

	Write-Host "Output dir = $outputdir" -ForegroundColor Yellow
	Write-Host "Source dir = $sourcedir" -ForegroundColor Yellow
	
	cd C:\Users\tofficer\Code\cmslite\default\src\CmsLite.Core\Areas\Admin\Scripts\Source
	#& $tools_dir\java\java.exe -jar $tools_dir\googleclosurecompiler\compiler.jar —js hello.js —js_output_file C:\Users\tofficer\Code\cmslite\default\build\javascript\foobar.min.js

	& $tools_dir\java\java.exe -jar $tools_dir\googleclosurecompiler\compiler.jar -help
}

# ------------------------------------ Functions ------------------------------------ 

function ZipFiles( $zipfilename, $sourcedir )
{
    [System.Reflection.Assembly]::LoadFrom("$tools_dir\zip-v1.9\Release\Ionic.Zip.dll");

    $zipfile = new-object Ionic.Zip.ZipFile
    $e = $zipfile.AddDirectory($sourcedir, ".")
    $zipfile.Save($zipfilename)
    $zipfile.Dispose()
}