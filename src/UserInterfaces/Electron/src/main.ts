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

	mainWindow.loadURL('file://' + __dirname + '/../app/index.html');

	mainWindow.webContents.on("did-finish-load", function(){
		afterInit();
	});

	mainWindow.on('closed', function() {
		//mainWindow = null;
	});
});


var rekoUi:SharpAssembly = new SharpAssembly("generated/assemblies/Reko.Gui.Electron.Adapter.dll");
var rootType:string = "Reko.Gui.Electron.Adapter.ElectronDecompilerDriver";
var reko:any;

/*
ipcMain.on("getProcedure", (event:any, args: any) => {
	var proc = args.program + ":" + args.address;
	reko.RenderProcedure(proc, (error:any, result:any) => {
		if(!error)
			renderProcedure(result);
	});
});
*/

ipcMain.on("getSearchResults", (event:any, args:any) => {
	console.log("Received");
	reko.ShowSearchResult({
		command: "",
		ipcChannel: "",
	}, (error:any, result:object) => {
		mainWindow.webContents.send("searchResults", result);
	})
});

/*ipcMain.on("getTemplate", (event:Electron.IpcMessageEvent, arg:string) => {
	event.returnValue = TemplateLoader.LoadTemplate(arg);
})*/

function afterInit(){
	var resolve = require('path').resolve;
	var createReko = rekoUi.getFunction(rootType, "CreateReko");
	
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
		return SharpAssembly.InvokeAsync(reko.RenderProjectJson, {});
	}).then((result) => {
		console.log("Post");
		mainWindow.webContents.send("project", result);
	}).catch((error:any) => {
		dialog.showErrorBox("Something blew up", error);
		throw new Error("Something blew up");
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