import $ = require("jquery");

export default class Procedure {

	constructor(private address:string, private name:string){}

	public render() : any {
		var addr = $(`<a href='${this.address}'>${this.name}</a>`);

		return $("<div class='procedure treenode'>")
			.append(addr);
	}
}