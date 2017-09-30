import $ = require("jquery");
import Procedure from "./Procedure";

export default class Program {

	constructor(private name:string, private procedures:any[]){}

	public render() : any{
		var element = $("<div class='program treenode'>");

		element.append(`<span class='program-name'>${this.name}</span>`);

		this.procedures.forEach(proc => {
			element.append(new Procedure(proc.address, proc.name).render());
		});

		return element;
	}

}