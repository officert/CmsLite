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

#Database Tasks

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

#NuGet Tasks

task Nuget-Pack {
	#this link show an example for packing multiple NuGet spec files : https://github.com/hibernating-rhinos/rhino-esb/blob/master/default.ps1
	
	#create new directory
	New-Item $nuget_tempdir\$nuget_packageName\lib\net35 -type directory -force
	#copy CmsLite.Core.dll to that dir
	Copy-Item $source_dir\cmslite.core\bin\CmsLite.Core.dll $nuget_tempdir\$nuget_packageName\lib\net35 -force
	#copy cms.nuspec file to that dir, two levels higher though
	Copy-Item $nuget_dir\cms.nuspec $nuget_tempdir\$nuget_packageName -force
	
	cd $nuget_tempdir\$nuget_packageName
	
	& $tools_dir\nuget\nuget.exe pack $nuget_tempdir\$nuget_packageName\cms.nuspec
}

#CmsLite Tasks

task CopyDirs {
	
}