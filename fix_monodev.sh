#!/bin/bash
cd "$(dirname "$0")"
find . -type f -iname "*.csproj" -print0 | while IFS= read -r -d '' proj; do
	sed 's/%24/\$/g;s/%29/\)/g;s/%28/\(/g' -i "$proj"
	echo "Done $proj"
done
