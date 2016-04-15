#include <iostream>
#include <string>
#include <vector>
#include <map>
#include "cmdline.h"
#include "esa.hxx"

using namespace std;

int readFile(const char* fn, vector<int>& T){
  FILE* fp = fopen(fn, "rb");
  if (fp == NULL){
    cerr << "cannot open " << fn << endl;
    return -1;
  }

  if (fseek(fp, 0, SEEK_END) != 0){
    cerr << "cannot fseek " << fn << endl;
    fclose(fp);
    return -1;
  }
  int n = ftell(fp);
  rewind(fp);
  if (n < 0){
    cerr << "cannot ftell " << fn << endl;
    fclose(fp);
    return -1;
  }
  T.resize(n);
  if (fread(&T[0], sizeof(unsigned char), (size_t)n, fp) != (size_t) n){
    cerr << "fread error " << fn << endl;
    fclose(fp);
    return -1;
  }

  fclose(fp);
  return 0;
}

int getID(const string& str, map<string, int>& word2id){
  map<string, int>::const_iterator it = word2id.find(str);
  if (it == word2id.end()){
    int newID = (int)word2id.size();
    word2id[str] = newID;
    return newID;
  } else {
    return it->second;
  }
}

void printSnipet(const vector<int>& T, const int beg, const int len, const vector<string>& id2word){
  for (int i = 0; i < len; ++i){
    int c = T[beg + i];
    if (id2word.size() > 0){
      cout << id2word[c] << " ";
    } else {
      cout << (isspace((char)c) ? '_' : (char)c);
    }
  }
}

int main(int argc, char* argv[]){
  cmdline::parser p;
  p.add("word", 'w', "word type");

  if (!p.parse(argc, argv)){
    cerr << p.error() << endl
	 << p.usage() << endl;
    return -1;
  }

  if (p.rest().size() > 0){
    cerr << p.usage() << endl;
    return -1;
  }

  vector<int> T;

  bool isWord = p.exist("word");
  map<string, int> word2id;
  istreambuf_iterator<char> isit(cin);
  istreambuf_iterator<char> end;

  size_t origLen = 0;
  if (isWord){
    string word;
    while (isit != end){
      char c = *isit++;
      if (!isspace(c)){
	word += c;
      } else if (word.size() > 0){
	T.push_back(getID(word, word2id));
	word = "";
      }
      ++origLen;
    }
    if (word.size() > 0){
      T.push_back(getID(word, word2id));
    }
  } else {
    while (isit != end){
      T.push_back((unsigned char)(*isit++));
    }
    origLen = T.size();
  }

  vector<string> id2word(word2id.size());
  for (map<string, int>::const_iterator it = word2id.begin();
       it != word2id.end(); ++it){
    id2word[it->second] = it->first;
  }

  vector<int> SA(T.size());
  vector<int> L (T.size());
  vector<int> R (T.size());
  vector<int> D (T.size());

  int k = (isWord) ? (int)id2word.size() : 0x100;
  if (isWord){
    cerr << "origN:" << origLen << endl;
  }
  cerr << "    n:" << T.size() << endl;
  cerr << "alpha:" << k        << endl;

  int nodeNum = 0;
  if (esaxx(T.begin(), SA.begin(), 
	    L.begin(), R.begin(), D.begin(), 
	    (int)T.size(), k, nodeNum) == -1){
    return -1;
  }
  cerr << " node:" << nodeNum << endl;

  for (int i = 0; i < nodeNum; ++i){
    cout << i << "\t" << R[i] - L[i] << "\t"  << D[i] << "\t";
    printSnipet(T, SA[L[i]], D[i], id2word);
    cout << endl;
  }

  return 0;
}
