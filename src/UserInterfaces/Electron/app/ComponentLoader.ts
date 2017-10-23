const vueify = require("vueify");
import * as path from 'path';

export default class ComponentLoader {
	private static Components:Map<string, any> = new Map<string, any>();

	public static LoadComponent(name:string){
		var cached = ComponentLoader.Components.get(name);
		if(typeof(cached) !== 'undefined')
			return cached;

		var componentPath = path.resolve(__dirname + `/../components/${name}.vue`);
		var component = vueify(componentPath, {
			_flags: {
				debug: true
			}
		});
		console.log(component);
		ComponentLoader.Components.set(name, component);

		return component;
	}
}