var edge = require("edge");

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

	/**
	 * Invokes a C# function and returns a promise
	 * @param func an edge C# function delegate
	 * @param args arguments for the function call
	 */
	public static InvokeAsync(func: any , args: any) : Promise<any>{
		return new Promise((resolve, reject) => {
			func(args, function(error:any, result:any){
				if(error)
					return reject(error);
				resolve(result);
			});
		});
	}
}