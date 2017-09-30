import $ = require("jquery");
import {ipcRenderer} from 'electron';

export default class Browser {
	constructor(){}

	public update(): any {
		$(".procedure").click(this.onProcedureClick);
	}

	public onProcedureClick(): any {
		var proc = $(this);
		var addr = proc
			.find("a")
			.attr("href");

		var programName = proc
			.parent()
			.text();

		ipcRenderer.send("getProcedure", {
			program: programName,
			address: addr
		});
	}
}