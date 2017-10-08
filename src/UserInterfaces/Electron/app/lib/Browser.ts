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
			.data("address");

		var programName = proc
			.parent()
			.find(".program-name")
			.text();

		ipcRenderer.send("getProcedure", {
			program: programName,
			address: addr
		});
	}
}