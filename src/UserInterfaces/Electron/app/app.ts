import {ipcRenderer} from 'electron';

import $ = require("jquery");

import Program from './lib/Program';
import Browser from './lib/Browser';
import TemplateLoader from './TemplateLoader';

var browser:Browser = new Browser();

function setup(){
	ipcRenderer.on("procedure", (event:any, arg:any) => {
		$(".reko-procedures").html(arg);
		browser.update();
	});

	ipcRenderer.on("project", (event:any, arg:any) => {
		var proj = JSON.parse(arg);

		var node = $(".reko-browser");

		proj.programs.forEach((prog:any) => {
			node.append(new Program(prog.name, prog.procedures).render());
		});


		browser.update();
	});

	$("#btn-test").click(function(e){
		ipcRenderer.send("decompile");
	});

	//$TODO: Precompile Templates
	var tpl = TemplateLoader.LoadTemplate("main");
	console.log(tpl);
	$("body").append(tpl({
		message: "Testing Handlebars"
	}));
}

$(document).ready(function(e){
	setup();
});