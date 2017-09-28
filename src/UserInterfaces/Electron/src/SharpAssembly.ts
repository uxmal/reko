var edge = require("electron-edge");

export default class SharpAssembly {
	private assembly:string;

	constructor(assemblyFile: string){
		this.assembly = assemblyFile;
	}

	public getFunction(typeName: string, methodName: string) : any{
		var clrMethod = edge.func({
			assemblyFile: this.assembly,
			typeName: typeName,
			methodName: methodName // This must be Func<object,Task<object>>
		});
		return clrMethod;
	}
}