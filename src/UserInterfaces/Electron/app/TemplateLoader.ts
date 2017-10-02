import handlebars = require("handlebars");
import * as fs from 'fs';
import * as path from 'path';	

export default class TemplateLoader {
	public static LoadTemplate(name:string){
		var tplPath = path.resolve(__dirname + `/../views/${name}.tpl`);
		var data = fs.readFileSync(tplPath).toString();
		return handlebars.compile(data);
	}
}