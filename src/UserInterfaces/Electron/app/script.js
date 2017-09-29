const {ipcRenderer} = require('electron')


function setup(){
	ipcRenderer.on("procedure", (event, arg) => {
		alert("Incoming transmission");
		$(".reko-procedures").html(arg);
	});

	$("#btn-test").click(function(e){
		ipcRenderer.send("decompile");
	});
}

$(document).ready(function(e){
	setup();
});