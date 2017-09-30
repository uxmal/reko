import * as electron from 'electron';
import {app, dialog, BrowserWindow, ipcMain} from 'electron';

import SharpAssembly from "./SharpAssembly";

var util = require('util');

var mainWindow: Electron.BrowserWindow;

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

	mainWindow.loadURL('file://' + __dirname + '/../../app/index.html');

	mainWindow.webContents.on("did-finish-load", function(){
		afterInit();
	});

	mainWindow.on('closed', function() {
		//mainWindow = null;
	});
});


var rekoUi:SharpAssembly = new SharpAssembly("generated/assemblies/Reko.Gui.Electron.Adapter.dll");

var rootType:string = "Reko.Gui.Electron.Adapter.ElectronDecompilerDriver";

function afterInit(){
	var resolve = require('path').resolve;
	var createReko = rekoUi.getFunction(rootType, "CreateReko");
	
	var reko:any;
	
	console.log("$$$ About to decompile");
	SharpAssembly.InvokeAsync(createReko, {
		appConfig: resolve("generated/assemblies/reko.config"),
		fileName: "E:/dec/Aberaham.exe",
		//fileName: "C:/dev/uxmal/reko/zoo/users/smxsmx/abheram/Aberaham.exe",
		notify: function (data:any, callback:any) {
			console.log(JSON.stringify(data));
			//$TODO: display the message in HTML user interface.
			callback(null, true);
		}
	}).then((result) => {
		console.log("$$$ i'm this far");
		reko = result;
		return SharpAssembly.InvokeAsync(reko.Decompile, {});
	}).then((result) => {
		/*dialog.showMessageBox(<Electron.BrowserWindow> mainWindow, {
			message: result
		});*/
		return SharpAssembly.InvokeAsync(reko.RenderProcedure, "Aberaham.exe:fn00011000");
	}).then((result) => {
		console.log("Sending result");
		mainWindow.webContents.send("procedure", result);
	}).catch((error) => {
		dialog.showErrorBox("Something blew up", error);
	});
}

/*
var hello = rekoUi.getFunction(rootType, "Hello");
hello(null, function(error:any, result:any){
	var res2 = result.Reko2(null, true);
	console.log(res2);
	var res3 = result.Reko3(null, true);
	console.log(res3);
});
*/


	
// console.log("$$$ About to render")
// 	renderProcedure(
// 	"Aberaham.exe:fn00011000",
// 	function(error:any, html:any) {
// 		if (!error) {
// 			console.log("The generated HTML");
// 			console.log(html);
// 		 }
// 	});