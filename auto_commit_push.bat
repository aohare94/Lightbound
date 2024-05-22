@echo off
REM Change directory to the project folder
cd /d C:\Users\Austin\Desktop\Lightbound

REM Initialize Git LFS
git lfs install

REM Track large files with Git LFS
git lfs track "*.dll"
git lfs track "*.dylib"
git lfs track "*.so"
git lfs track "*.a"
git lfs track "*.lib"
git lfs track "*.exe"

REM Add all changes, including untracked files
git add .

REM Commit changes with "Auto commit" message
git commit -m "Auto commit"

REM Push changes to the remote repository on the main branch
git push origin main

REM Pause to see the result (optional)
pause
