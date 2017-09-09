int frobulate(int a1){
	return a1 * a1 / 1337;	
}

int bazulate(int a1, int a2){
	return (a1 + a2) / frobulate(a1) / frobulate(a2);
}


int switcheroo(int arg)
{
 switch (arg)
 {
 case 0:
 case 1:
 case 2:
  frobulate(arg);
  break;
 case 4:
  frobulate (arg - 3);
  break;
 case 6:
  bazulate(arg, arg);
 default:
  bazulate(0, 0);
 }
 return arg + 1;
}


int main(int argc, char *argv[]){
	switcheroo(argc);
	return 0;
}
