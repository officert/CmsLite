REM This script deletes all data in the database

cd scripts
sqlcmd -E -S .\SQLEXPRESS -d cmslite_local -i DeleteData.sql

pause