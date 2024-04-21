@echo off
for /F "tokens=1 delims= " %%A in (names.txt) do echo %%A >> names-corrected.txt %%A