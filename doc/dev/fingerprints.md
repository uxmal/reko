# Fingerprint support in Reko

## Definitions

### Fingerprints

A **procedure** can be characterized by its **fingerprint**. Fingerprints
describe a procedure or data that can be recognized by simple textual means
A fingerprint has a **subject** which describes whether it can act on raw
bytes or on disassembled instructions. It has a collection of **features**.


A sketch of a fingerprint is:
```C#
interface IFingerPrint {
    FingerPrintSubject Subject {get;}
    ISet<IFeature> Features {get;}
}
```

### Features
A **feature** is a property that can be
computed from the instructions that constitute the procedure. Some possible
viable features are:
- a hash of all the mnemonics of the procedure.
- a hash of the graph structure of the procedure.
- the size of the procedure in storage units.
- A hash of small one-byte constants makes sense on some platforms (x86) because 
small constants will not be targets of relocations.


## Fingerprint libraries
Fingerprints can be collected in a **fingerprint library**, which is another
kind of metadata that Reko can consume. A fingerprint library consists of 
a set of records which each have the following structure:
```
    - architecture string
    - arch options
    - platform string
    - platform options
    - Fingerprint
        - feature[]
            - featureID
            - value
    - Object attributes
        - Type (code /data)
        - Name
        - Label(s)
        - Comments
        - Signature
        - Characteristics
```
Fingerprint libraries can be in some places:
- The Reko installation directory
- The user's ~/.config/Reko/fingerprints folder
- Referred to by the project file

Fingerprint libraries have type references so they must be loaded after type libraries.

A fingerprint library could theoretically refer to other formats than reko's own. We
therefore define `IFingerprintLibrary` to abstract away the difference.

## Fingerprint generator
A fingerprint generator generates a fingerprint. It either takes a blob of bytes
or a scanned procedure, depending on the fingerprint subject. It accepts a 
**fingerprint strategy** which details what features to look for and what
attributes to extract.

## Fingerprint service
The **fingerprint service** provides high-level services. 

## Object libraries 
An **object library** is a new subtype of `ILoadedObject` which has the
following members:
```C#
public class IObjectLibrary : ILoadedObject {
    LibraryFragment[] Fragments;

    IFormatter GetFormatter();
}
```

## Actions
The user can compute the fingerprints of a whole binary by selection 
`Action > Fingerprint` on a scanned library.
The user can browse a fingerprint library. The library shows all
fingerprints, grouped. User can Edit it to rename/remove entries.
