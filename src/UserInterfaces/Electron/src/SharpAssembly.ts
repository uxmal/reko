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

	public static InvokeAsync(func: any, args: any) : Promise<any>{
		return new Promise((resolve, reject) => {
			func(args, function(error:any, result:any){
				if(error)
					return reject(error);
				resolve(result);
			});
		});
	}
}