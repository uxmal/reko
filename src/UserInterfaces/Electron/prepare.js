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
    "node-gyp"
];

pr_status("[+] Installing required global dependencies");
run_shell("npm install -g " + global_deps.join(" "));

var electron_version = child.spawnSync("electron", ["-v"], {
    shell: true,
    cwd: __dirname + "/node_modules/.bin"
}).output.toString().replace(/\r?\n/gm, "").replace(/[,v]/gm, "").trim();

pr_status("[+] Rebuilding edge for electron " + electron_version);
run_shell(
    "node-gyp",
    __dirname + "/node_modules/edge/",
    "clean configure build --verbose " +
    `--target=${electron_version} ` + 
    "--dist-url=https://atom.io/download/atom-shell"
);
pr_status("[+] Installing dependencies for app-render");
run_shell("npm install", __dirname + "/app");
