struct foo
{
     foo * left;
     foo * right;
     int n;
};

extern foo g_ptr;
void frobulate(foo *);


foo * locate(
  int key, foo *ptr)
{
  if (ptr == 0)
  return 0;
  if (key < ptr->n)
    return locate(key, ptr->left);
  if (key > ptr->n)
    return locate(key, ptr->right);
  else
    return ptr;
}

int irreducible(foo * ptr)
{
  int x = ptr->n;
  if (x > 0)
  {
    x = x * 2;
  irr:
    x <<= 1;
    goto irr2;
  }
  else
  {
irr2:
    x = x ^ 0x1010101;
      if (x & 1)
      goto irr;
  }
  return x;
}

int eval(foo * ptr) {
  if (ptr->n & 1)
    return ptr->n >> 1;
  switch (ptr->n >> 1) {
    case 0: return eval(ptr->left) + eval(ptr->right);
    case 1: return eval(ptr->left)*eval(ptr->right);
    case 2: return eval(ptr->left)-eval(ptr->right);
    case 3: return eval(ptr->left)/eval(ptr->right);
    case 4: return -eval(ptr->left);
    case 5: return eval(ptr->left)/eval(ptr->right);
    }
  return 0;
}

foo * buf[300];
  
int main()
{
  frobulate(&g_ptr);
  foo * ptr = &g_ptr;
  
  foo * fnd = locate(
    32,
    ptr);
  buf[3] = fnd;
}