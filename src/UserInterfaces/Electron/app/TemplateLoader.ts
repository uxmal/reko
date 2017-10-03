import handlebars = require("handlebars");
import * as fs from 'fs';
import * as path from 'path';	

export default class TemplateLoader {
	private static Templates:Map<string, HandlebarsTemplateDelegate<any>> = new Map<string, HandlebarsTemplateDelegate<any>>();

	public static LoadTemplate(name:string){
		var cached = TemplateLoader.Templates.get(name);
		if(typeof(cached) !== 'undefined')
			return cached;

		var tplPath = path.resolve(__dirname + `/../views/${name}.tpl`);
		var data = fs.readFileSync(tplPath).toString();
		var tpl = handlebars.compile(data)
		TemplateLoader.Templates.set(name, tpl);
		
		return tpl;
	}
}