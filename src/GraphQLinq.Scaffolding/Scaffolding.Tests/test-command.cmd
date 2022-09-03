REM Run this manually to test the exe file and to view its errors...

set exe=..\Scaffolding\bin\Release\net6\Scaffolding.exe

set uri=https://api.spacex.land/graphql

set o=SpaceXFolderMultiFiles
set f="SingleFile"

set n=SpaceX

set c=SpaceX

set u=""
set p=""

set q=False
set j=True


set hk=""
set hv=""


%exe% %uri% -q=%q% -j=%j% -f %f% -o %o% -c %c% -n %n% -u %u% -p %p% -hk=%hk% -hv=%hv%

pause