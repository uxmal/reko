import * as electron from 'electron';
import {app, dialog, BrowserWindow} from 'electron';

import SharpAssembly from "./SharpAssembly";

var util = require('util');

var mainWindow: Electron.BrowserWindow | null = null;

app.on('window-all-closed', function() {
	if (process.platform != 'darwin') {
		app.quit();
	}
});

app.on('ready', function() {
	mainWindow = new BrowserWindow({
		width: 800,
		height: 600
	});

	mainWindow.loadURL('file://' + __dirname + '/../app/index.html');

	mainWindow.on('closed', function() {
		mainWindow = null;
	});
});


var rekoUi:SharpAssembly = new SharpAssembly("generated/assemblies/Reko.Gui.Electron.Adapter.dll");

/*
var Decompile = rekoUi.getFunction("ElectronDecompilerDriver", "Decompile");

Decompile("C:/dec/Aberaham.exe", true, function(error:any, result:any){
	if(error){
		dialog.showErrorBox("C# Exception", error);
	}
	console.log(util.inspect(error, true, 99, false));
});
*/

var hello = rekoUi.getFunction("Reko.Gui.Electron.Adapter.ElectronDecompilerDriver", "Hello");
hello();