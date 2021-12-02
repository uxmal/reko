This subject tests Reko's ability to decompile file images located inside 
nested archives. The file `hello.tar` is a tarball, containing among other
file `hello.ar`, which is a Unix archive file. That file, in turn, contains
two ELF binaries, `hello_O1` and `hello_O3`
