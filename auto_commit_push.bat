@echo off
REM Change directory to the project folder
cd /d C:\Users\Austin\Desktop\Lightbound

REM Fetch the latest changes from the remote repository
git fetch origin

REM Check if there are uncommitted changes in the working directory
git status --porcelain > temp_status.txt
if %ERRORLEVEL% neq 0 (
    echo "Failed to get the status of the working directory."
    del temp_status.txt
    pause
    exit /b 1
)

REM If there are uncommitted changes, proceed with adding and committing them
for /f %%i in (temp_status.txt) do set uncommitted_changes=%%i
del temp_status.txt

if defined uncommitted_changes (
    REM Add all changes to the staging area
    git add .

    REM Commit changes with "Auto commit" message
    git commit -m "Auto commit"
) else (
    echo "No changes to commit."
)

REM Check for merge conflicts
git merge FETCH_HEAD --no-commit --no-ff
if %ERRORLEVEL% neq 0 (
    echo "Merge conflicts detected. Please resolve them before pushing."
    git merge --abort
    pause
    exit /b 1
)

REM Push changes to the remote repository on the main branch
git push origin main
if %ERRORLEVEL% neq 0 (
    echo "Failed to push changes to the remote repository."
    pause
    exit /b 1
)

echo "Changes have been successfully pushed to the remote repository."
pause
