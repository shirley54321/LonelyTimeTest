#! /bin/sh

BASEDIR=$(dirname "$0")

cd "$BASEDIR"

read -p "請輸入fork專案下來的網址: " githttp
echo "$githttp"

git clone --no-checkout "$githttp" a.tmp

cd a.tmp

mv .git "$BASEDIR"

cd ..

rmdir a.tmp

git reset --hard

read -s -n1 -p "按任意鍵繼續 ... "