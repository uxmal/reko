import {ipcRenderer} from 'electron';

import $ = require("jquery");

import Browser from './lib/Browser';
import TemplateLoader from './TemplateLoader';

var browser:Browser = new Browser();

function setup(){
	// Render a procedure
	ipcRenderer.on("procedure", (event:any, arg:any) => {
		$(".reko-procedures").html(arg);
		browser.update();
	});

	// Render a project
	ipcRenderer.on("project", (event:any, arg:any) => {
		var proj = JSON.parse(arg);

		var node = $(".reko-browser");

		var tpl = TemplateLoader.LoadTemplate("main");
		$("#reko-browser")
			.find(".reko-procedures")
			.html(tpl(proj));

		browser.update();
	});

	ipcRenderer.on("searchResults", (event:any, arg:object) => {
		var tpl = TemplateLoader.LoadTemplate("searchResults");
		
		$("#search-results").html(tpl(arg));
	});

	$("#btn-test").click(function(e){
		ipcRenderer.send("decompile");
	});

	$("#getSearchBtn").click(function(e){
		ipcRenderer.send("getSearchResults", "testing");
	});
}

$(document).ready(function(e){
	setup();
});