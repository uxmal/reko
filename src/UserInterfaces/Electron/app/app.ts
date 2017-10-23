import {ipcRenderer} from 'electron';

import $ = require("jquery");

import Browser from './lib/Browser';
import Handlebars = require("handlebars");
//import TemplateLoader from './TemplateLoader';
//import ComponentLoader from './ComponentLoader';

import Vue from 'vue';
import App from './components/app.vue';
import Test from './components/test.vue';

const Element = require("element-ui");
Vue.use(Element);

//const bootstrap_css = require("bootstrap.min.css");
//const element_default = require("element-default.css");

//const delims = ["<%", "%>"];

var browser:Browser = new Browser();

function renderProcedure(data:string){
	$("#reko-procedure")
	.html(data);
}

function setup(){
	// Render a procedure
	ipcRenderer.on("procedure", (event:any, arg:any) => {
		renderProcedure(arg);
		browser.update();
	});

	// Render a project
	ipcRenderer.on("project", (event:any, arg:any) => {
		var proj = JSON.parse(arg);

		var node = $(".reko-browser");

		const tpl = require("./views/main.tpl");
		$("#reko-browser")
			.find(".reko-procedure-list")
			.html(tpl(proj));

		browser.update();
	});

	ipcRenderer.on("searchResults", (event:any, arg:object) => {
		const tpl = require("./views/searchResults.tpl");
		
		$("#search-results").html(tpl(arg));
	});

	ipcRenderer.on("reko-message", (event:any, arg:object) => {
		const tpl = require("./views/logEntry.tpl");

		$("#reko-messages > tbody").append(tpl(arg));
	});

	$("#btn-test").click(function(e){
		ipcRenderer.send("decompile");
	});

	$("#getSearchBtn").click(function(e){
		ipcRenderer.send("getSearchResults", "testing");
	});
}

$(document).ready(function(e){
	Vue.component("reko-app", App);
	new Vue({
	  el: '#app',
	  template: "<reko-app />"
	});

	const index = require("./views/index.tpl");
	$("body").append(index({}));
	setup();
});