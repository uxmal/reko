typedef struct _XDisplay Display;

Display * XOpenDisplay(char * display_name);
int XCloseDisplay(Display * display_name);
void XtFree(void *);
void * XtMalloc(size_t size);
