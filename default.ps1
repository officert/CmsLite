properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  $base_dir = resolve-path .
  $database_dir = "$base_dir\database"
  $tools_dir= "$base_dir\tools"
  #Project settings
  $projectName = 'CmsLite'
  #Database properties
  $databaseServer = '.'
  $databaseName = 'cmslite_local'
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
	exec { sqlcmd -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteTestUsers.sql }
	exec { sqlcmd -E -S $databaseServer -d $databaseName -i $database_dir\scripts\LoadTestUsers.sql }
}

task DeleteData {
	cd $tools_dir\sqlcmd\
	exec { sqlcmd -E -S $databaseServer -d $databaseName -i $database_dir\scripts\DeleteData.sql }
}