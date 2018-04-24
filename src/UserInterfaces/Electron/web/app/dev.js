const webpack = require('webpack');
const child = require("child_process");
const path = require("path");

const config = Object.create(require('./webpack.config.js'));

function showSummary(stats) {
    console.error('[webpack]', stats.toString({
        colors: true,
        hash: false,
        timings: false,
        chunks: false,
        chunkModules: false,
        modules: false,
        children: true,
        version: true,
        cached: false,
        cachedAssets: false,
        reasons: false,
        source: false,
        errorDetails: false
    }));
}

config.watch = true;
config.cache = true;
config.bail = false;


var electron_started = false;

function run_shell(cmd, dir, ...args){
    return child.spawn(cmd, args, {
        shell: true,
        env: process.env,
        cwd: dir,
        stdio:[0,1,2]
    });
}

webpack(config, function(error, stats) {
	if (error) {
		console.error('[webpack]', error);
	}

	showSummary(stats);

	if(!electron_started){
		// start electron from parent directory
		run_shell(
			path.resolve(__dirname, "../node_modules/.bin/electron"),
			path.resolve(__dirname, "../"),
			"generated/main.js"
		);
		electron_started = true;
	}
});