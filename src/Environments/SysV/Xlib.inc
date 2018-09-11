typedef int Bool;
typedef struct _XDisplay Display;
typedef int Status;
typedef void * Colormap;
typedef void * Window;
typedef void * Atom;
typedef void * Visual;
typedef void * Drawable;
typedef struct _WMHints {
} XWMHints;
typedef struct _XSetWindowAttributes {
} XSetWindowAttributes;
typedef struct _XWindowChanges {
} XWindowChanges;
typedef struct _XStandardColormap {
} XStandardColormap;




void * XtMalloc(size_t size);
Status XAllocColorCells(Display *display, Colormap colormap, Bool contig, unsigned long plane_masks_return[], unsigned int nplanes, unsigned long pixels_return[], unsigned int npixels);
XWMHints *XAllocWMHints(void);
int XChangeProperty(Display *display, Window w, Atom property, Atom type, int format, int mode, unsigned char *data, int nelements);
int XChangeWindowAttributes(Display *display, Window w, unsigned long valuemask, XSetWindowAttributes *attributes);
int XConfigureWindow(Display *display, Window w, unsigned value_mask, XWindowChanges *changes);
int XCloseDisplay(Display * display_name);
Colormap XCreateColormap(Display *display, Window w, Visual *visual, int alloc);
Window XCreateWindow(Display *display, Window parent, int x, int y, unsigned int width, unsigned int height, unsigned int border_width, int depth, unsigned int _class, Visual *visual, unsigned long valuemask, XSetWindowAttributes *attributes);
int XDeleteProperty(Display *display, Window w, Atom property);
int XDestroyWindow(Display *display, Window w);
char *XDisplayName(char *string);
int XEventsQueued(Display *display, int mode);
int XFlush(Display *display);
int XFree(void *data);
int XFreeColormap(Display *display, Colormap colormap);
Status XGetGeometry(Display *display, Drawable d, Window *root_return, int *x_return, int *y_return, unsigned int *width_return, unsigned int *height_return, unsigned int *border_width_return, unsigned int *depth_return);
Status XGetRGBColormaps(Display *display, Window w, XStandardColormap **std_colormap_return, int *count_return, Atom property);
// XGetVisualInfo
// XGetWindowProperty
// XIconifyWindow
// XInternAtom
// XLookupKeysym
// XLookupString
// XMapWindow
// XmuLookupStandardColormap
// XNextEvent
Display * XOpenDisplay(char * display_name);
// XParseGeometry
// XPeekEvent
int XPending(Display *display);
// XRefreshKeyboardMapping
// XResizeWindow
// XSelectInput
// XSetWMColormapWindows
// XSetWMNormalHints
// XSetWMProperties
// XSetWMProtocols
// XSynchronize
// XTranslateCoordinates
void XtFree(void *);
// XUnmapWindow
// XWithdrawWindow
