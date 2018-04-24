#!/bin/bash
trap "trap - SIGTERM && kill -- -$$" SIGINT SIGTERM EXIT

# Watch changes for app-main
tsc --watch &

# Watch changes for app-renderer and invoke electron
cd app
node dev.js