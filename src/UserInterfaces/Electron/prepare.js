var child = require("child_process");
var fs = require("fs");

function pr_status(){
    // \bright, \white, <str>, \reset
    console.log('\x1b[1m\x1b[37m%s\x1b[0m', Array.prototype.slice.call(arguments));
}

// Prepare directories
pr_status("[+] Creating directories...");
try {
    fs.mkdirSync("generated");
    fs.mkdirSync("generated/assemblies");
    fs.mkdirSync("app/generated");
} catch(Error){}

function run_shell(cmd, dir, ...args){
    return child.spawnSync(cmd, args, {
        shell: true,
        cwd: dir,
        stdio:[0,1,2]
    });
}

var global_deps = [
    "typescript",
    "electron",
    "electron-packager",
    "node-gyp"
];

pr_status("[+] Installing required global dependencies");
run_shell("npm install -g " + global_deps.join(" "));
pr_status("[+] Rebuilding edge for electron");
run_shell(
    "node-gyp",
    __dirname + "/node_modules/electron-edge/",
    "clean configure build " +
    "--target=1.7.8 --dist-url=https://atom.io/download/atom-shell --msvs_version=2015"
);
pr_status("[+] Installing dependencies for app-render");
run_shell("npm install", __dirname + "/app");