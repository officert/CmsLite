properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
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

task CreateTestUser {
	Write-Host Database name is : $databaseName -ForegroundColor "Green"
}