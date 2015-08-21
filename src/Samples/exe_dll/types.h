#ifdef DEFEXPORTS
#define DRIVERAPI __declspec(dllexport)
#else
#define DRIVERAPI __declspec(dllimport)
#endif
class SampleClass
{
public:
	SampleClass(char * s) { data = s; }
	int DRIVERAPI atoi();
	int DRIVERAPI __cdecl cdecl_method();
	int DRIVERAPI __stdcall stdcall_method();
private:
	char * data;
};
