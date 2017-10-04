import {ipcRenderer} from 'electron';

import $ = require("jquery");

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

		var tpl = TemplateLoader.LoadTemplate("main");
		$("body").append(tpl(proj));

		browser.update();
	});

	$("#btn-test").click(function(e){
		ipcRenderer.send("decompile");
	});
}

$(document).ready(function(e){
	setup();
});