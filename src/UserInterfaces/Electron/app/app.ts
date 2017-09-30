import {ipcRenderer} from 'electron';
import $ = require("jquery");


function setup(){
	ipcRenderer.on("procedure", (event:any, arg:any) => {
		$(".reko-procedures").html(arg);
	});

	$("#btn-test").click(function(e){
		ipcRenderer.send("decompile");
	});
}

$(document).ready(function(e){
	setup();
});