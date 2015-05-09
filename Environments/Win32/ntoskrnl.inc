typedef unsigned long ULONG;
typedef unsigned long NTSTATUS;
typedef int BOOLEAN;
typedef int DEVICE_TYPE;

typedef struct _EPROCESS {
} EPROCESS, *PEPROCESS;
typedef struct _KTHREAD
{
} KTHREAD, *PKTHREAD;
typedef wchar_t * PUNICODE_STRING;
typedef struct _DEVICE_OBJECT 
{
} DEVICE_OBJECT,*PDEVICE_OBJECT;
typedef struct _DRIVER_OBJECT
{
} DRIVER_OBJECT, *PDRIVER_OBJECT;

ULONG __cdecl DbgPrint(char * format, ...);
PEPROCESS  IoGetCurrentProcess(void);
PKTHREAD KeGetCurrentThread(void);
NTSTATUS IoCreateDevice(
  PDRIVER_OBJECT  DriverObject,
  ULONG           DeviceExtensionSize,
  PUNICODE_STRING DeviceName,
  DEVICE_TYPE     DeviceType,
  ULONG           DeviceCharacteristics,
  BOOLEAN         Exclusive,
  PDEVICE_OBJECT  *DeviceObject
);
NTSTATUS IoCreateSymbolicLink(
  PUNICODE_STRING SymbolicLinkName,
  PUNICODE_STRING DeviceName
);

