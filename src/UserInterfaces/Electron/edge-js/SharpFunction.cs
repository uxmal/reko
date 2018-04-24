using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Retyped.es6_promise;

namespace ElectronUI.edge_js
{
    public class SharpFunction
    {
        public delegate void EdgeCallback(object error, object result);

        private Action<object, EdgeCallback> func;

        public SharpFunction(Action<object, EdgeCallback> func)
        {
            this.func = func;
        }

        public Promise<object> InvokeAsync(object arg)
        {
            return new Promise<object>((resolve, reject) => {
                func(arg, (error, result) => {
                    if (error != null) {
                        reject(error);
                        return;
                    }

                    resolve(result);
                });
            });
        }
    }
}
