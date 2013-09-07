REM This script creates one test user in the database
REM Username : user0@test.com
REM Password : password

cd scripts
sqlcmd -E -S . -d cmslite_local -i LoadTestUser.sql

pause