## How to build the Electron UI for Reko

1. Install Visual Studio Code
2. Install NodeJS, make sure npm and add to PATH are checked
3. Open a command prompt, cd to this folder, and run 
```
npm install -g typescript electron electron-packager node-gyp
``` 
which installs global dependencies. Then run
```
npm install
pushd node_modules\electron-edge
node-gyp clean
node-gyp configure build --target=1.8.1 --dist-url=https://atom.io/download/atom-shell --msvs_version=2015

node-gyp clean
node-gyp configure build --target=1.7.8 --dist-url=https://atom.io/download/atom-shell

popd
cd app && npm install
```

