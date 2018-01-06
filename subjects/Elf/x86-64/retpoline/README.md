== retpoline.elf ==
As part of the fallout of the [Meltdown](https://en.wikipedia.org/wiki/Meltdown_(security_vulnerability)]) and 
[Spectre](https://en.wikipedia.org/wiki/Spectre_(security_vulnerability) compiler vendors are trying to mitigate
the risk by implementing what is known as "retpolines". This binary has an example of such a retpoline. The hope
is that Reko can identify such constructs and clean them up.
