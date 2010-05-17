var s = WScript.Arguments.Item(0);
var lastDot = s.lastIndexOf('.');
if (lastDot != -1)
   s = s.substring(0, lastDot);

var fso = new ActiveXObject("Scripting.FileSystemObject");
var file = fso.GetFile(s + ".txt");
file.Copy(s + ".exp");

