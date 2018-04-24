#!/bin/bash
trap 'jobs -p | xargs kill' EXIT

tsc --watch &
cd app
./node_modules/.bin/webpack --progress --colors --watch
