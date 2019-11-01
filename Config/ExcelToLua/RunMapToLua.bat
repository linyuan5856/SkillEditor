if exist temp_file (
    rmdir /s /q temp_file
)

mkdir temp_file
mkdir temp_file\excel

if exist lua_config (
    rmdir /s /q lua_config
)
mkdir lua_config

python MapToLua.py %*

set cur=%cd%
cd ../../../server/starve-game/config2/


xcopy /y %cur%\lua_config\*.*  %cd%\

pause