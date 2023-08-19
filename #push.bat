@echo off
set /p var=提交信息：
git add .
git commit -m %var%
git push
pause