## How to build the Electron UI for Reko

## Dependencies

- Visual Studio Code
- NodeJS with npm

## Installation

1. Open reko-decompiler.sln, set **WindowsDecompiler** as starting project, **Debug** as configuration (platform doesn't matter, edge-js supports both architectures). then **build** the solution
2. run `getAssemblies.bat` in this folder
3. `cd` in this folder, then run `npm install`

## Development notes
Run `dev.sh` to edit files on the fly without having to rebuild every time

To refresh the page (and see changes) press `CTRL-SHIFT-I` while focused on the electron window to open the dev tools, then click on the development tools to gain focus and press `F5`

Note that if you edit webpack.config.js you'll need to restart `dev.sh`


vetur is recommended to edit .vue files on Visual Studio Code

## Development issues

### [vetur(visual studio code extension for vuejs) doesn't support typescript 2.7](https://github.com/vuejs/vetur/issues/682)
Vetur doesn't support typescript 2.7 and you will see red underlines for new features like *definite assignment assertions*
```typescript
foo!: number;
```

**WORKAROUND**: Update the extension manually until a new version is released

```batch
npm install -g yarn
cd /d %USERPROFILE%\.vscode\extensions\octref.vetur-0.11.7
yarn upgrade typescript@latest
cd .\server
yarn upgrade typescript@latest
```

-------------------

### [edge-js doesn't handle EDGE_USE_CORECLR properly](https://github.com/agracio/electron-edge-js/issues/9)
EdgeJS is now maintained by agracio in his fork.

EdgeJS can target Mono, CoreCLR and .NET

You can use the environment variable `EDGE_USE_CORECLR` to control wether or not .NET Core should be preferred, but there are 2 flaws

- If you compile edge-js from source, .NET Core will always be preferred
- If you set `EDGE_USE_CORECLR` to false, .NET Core will be used anyways

**WORKAROUND**: Don't use `EDGE_USE_CORECLR`, **make sure it's not in your environment variables** and make sure to remove the `build` folder under the edge-js node modules folder if you recompile it (make sure it doesn't exist anyways)

This is due to the code mishandling environment variables. The moment `EDGE_USE_CORECLR` is set it becomes a string (environment variables are strings only). As such, normal boolean comparison operators now act on strings.

```javascript
if(process.env.EDGE_USE_CORECLR) //always true if the string is not empty
if(process.env.EDGE_USE_CORECLR == 1) //string is converted to integer before comparison
```

edge-js is improperly using the former check

-------------------

### node-gyp ignores --msvs_version switch from command line if npmrc specifies otherwise

If you're going to recompile edge-js you might stumble across this error

```
cannot convert argument 1 from 'nullptr' to 'nullptr &&'
```

If you're on Windows this is probably due to node-gyp picking up a toolchain version that is older than Visual Studio 2017.

You wouldn't get this problem if you run node-gyp by hand. But if node-gyp is invoked as part of prepare.js (by npm install) then npmrc takes precedence (likely a bug, the user's command line option should take precedence)

**WORKAROUND**: Locate npmrc. On Windows it's in %APPDATA%\npm\etc

Open the file in a text editor and delete msvs_version

-------------------

### [handlebars-loader doesn't support Webpack 4](https://github.com/pcardune/handlebars-loader/issues/155)

This is a low priority issue, but if we'll ever have to upgrade to Webpack 4 we should keep an eye on it
