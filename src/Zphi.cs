using System;
using System.Collections;
using System.Collections.Generic;

namespace Healpix
{

    /*
    *  This file is ported from Healpix Java.
    */


    /** Class for storing a position on the unit sphere as a (z,phi)-tuple.

        @copyright (C) 2011 Max-Planck-Society
        @author Martin Reinecke */
    public class Zphi
    {
        /** Cosine of the colatitude, or z component of unit vector; Range [-1;1]. */
        public double z;

        /** Longitude in radians; Range [0; 2Pi]. */
        public double phi;

        /** Default constructor */
        public Zphi() {}

        /** Creation from individual components */
        public Zphi (double z_, double phi_)
            { z=z_; phi=phi_; }

        /** Conversion from {@link Vec3} */
        public Zphi (Vec3 v)
            { z = v.z/v.length(); phi = FastMath.atan2(v.y,v.x); }

        /** Conversion from {@link Pointing} */
        public Zphi (Pointing ptg)
            { z = FastMath.cos(ptg.theta); phi=ptg.phi; }

        public override String ToString()
        {
            return "zphi(" + z + "," + phi + ")";
        }

        public override bool Equals(Object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            Zphi zphi = (Zphi) o;
            if (zphi.phi != phi) return false;
            if (zphi.z != z) return false;
            return true;
        }

        public override int GetHashCode()
        {
            long temp = z != +0.0d ? BitConverter.DoubleToInt64Bits(z) : 0L;
            int result = (int) (temp ^ (int)((uint)temp >> 32));
            temp = phi != +0.0d ? BitConverter.DoubleToInt64Bits(phi) : 0L;
            result = 31 * result + (int) (temp ^ (int)((uint)temp >> 32));
            return result;
        }
    }
}
