#include <stdlib.h>
#include <string.h>
//#include <windows.h>

//#pragma pack(1)
struct 
__attribute__((packed))
unaligned {
	unsigned char tag;
	struct unaligned *next;
};

/*int WinMain(
	HINSTANCE hInstance,
	HINSTANCE hPrevInstance,
	LPSTR lpCmdLine,
	int nShowCmd
){*/
int main(int argc, char *argv[]){
	struct unaligned info;
	struct unaligned *heap;

	memset(&info, 0x00, sizeof(struct unaligned));
	heap = calloc(1, sizeof(struct unaligned));
	memcpy(heap, &info, sizeof(info));

	heap->tag = 12;
	heap->next = NULL;
	
	info.tag = 0x42;
	info.next = heap;
	return 0;
}
