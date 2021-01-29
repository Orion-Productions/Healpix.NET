# Healpix.NET
A port of the HEALPix core to C# based on the [Healpix Java](https://healpix.jpl.nasa.gov/html/java/index.html) implementation

## NOTE
Most functions have not been tested in their C# form:

The appropriate C# function and logical equivalents have been replaced, but it is possible that behavior will not be identical. Please submit an issue if this is the case.

So far, tested functions are:

- HealpixBase.pix2ZPhi
   - all functions called by pix2ZPhi
- HealpixBase.boundaries / HealpixBase.boundariesZPhi
   - all functions called by boundaries / boundariesZPhi
