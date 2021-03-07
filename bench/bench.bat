set a=0
:loop
echo open %a% test program
start mine-tank.exe 10.148.124.63 1234		::server ip and port
set /a a+=1
if %a% lss 20 goto :loop			::test 20 times