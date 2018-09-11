typedef int GLenum;
typedef unsigned int GLuint;
typedef float GLfloat;
typedef int GLint;
typedef int GLsizei;
typedef unsigned char GLubyte;
typedef void * Display ;
typedef struct _XVisualInfo { } XVisualInfo ;




void glBegin(GLenum);
void glBindTexture(GLenum target, GLuint texture);
void glBitmap(	GLsizei width,
 	GLsizei height,
 	GLfloat xorig,
 	GLfloat yorig,
 	GLfloat xmove,
 	GLfloat ymove,
 	const GLubyte * bitmap);
// glBlendFunc
// glClear
// glClearColor
// glColor3f
// glColor3fv
// glColorMaterial
// glCullFace
// glDisable
// glDrawBuffer
void glEnable(GLenum cap);
void glEnd(void);
// glFinish
// glFlush
// glFogf
// glFogfv
// glFogi
// glGenTextures
// glGetError
// glGetIntegerv
// glGetString
// glHint
// glLightf
// glLightfv
// glLightModelfv
// glLightModeli
// glLoadIdentity
// glMaterialf
// glMaterialfv
// glMatrixMode
// glNormal3f
// glOrtho
// glPixelStorei
// glPolygonMode
// glPopAttrib
// glPopMatrix
// glPushAttrib
// glPushMatrix
// glRasterPos2f
// glReadBuffer
// glRotatef
// glScalef
// glShadeModel
// glTexCoord2f
// glTexEnvf
// glTexParameteri
// glTranslatef
void glVertex3f(GLfloat x, GLfloat y, GLfloat z);
void glViewport(GLint x, GLint y, GLsizei width, GLsizei height);
 
// gluBuild2DMipmaps
// gluCylinder
// gluDisk
// gluErrorString
// gluNewQuadric
// gluPerspective
// gluQuadricCallback
// gluQuadricDrawStyle
// gluQuadricNormals
// gluSphere

// glutBitmapCharacter
// glutCreateWindow
// glutDisplayFunc
// glutFullScreen
// glutGet
// glutIdleFunc
// glutInit
// glutInitDisplayMode
// glutKeyboardFunc
// glutMainLoop
// glutPostRedisplay
// glutReshapeFunc
// glutSolidCube
// glutSolidSphere
// glutSpecialFunc
// glutSwapBuffers

// glXChooseVisual
// glXCreateContext
// glXCreateContextWithConfigSGIX
// glXDestroyContext
int glXGetConfig(Display * dpy, XVisualInfo * vis, int attrib,int * value);
// glXIsDirect
// glXMakeCurrent
// glXQueryExtension
// glXSwapBuffers
// glXWaitX
