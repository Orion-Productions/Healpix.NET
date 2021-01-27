using System;
using System.Collections;
using System.Collections.Generic;

namespace Healpix
{

    /*
    *  This file is ported from Healpix Java.
    */

    /** An angular position on the unit sphere.

        @copyright 2011 Max-Planck-Society
        @author Martin Reinecke */
    public class Pointing
    {
        /** Colatitude in radians (0 is North Pole; Pi is South Pole) */
        public double theta;

        /** Longitude in radians */
        public double phi;

        /** Default constructor */
        public Pointing() {}

        public Pointing(Pointing ptg)
            { this.theta = ptg.theta; this.phi = ptg.phi; }

        /** Simple constructor initializing both values.
            @param theta in radians [0,Pi]
            @param phi in radians [0,2*Pi] */
        public Pointing(double theta, double phi)
            { this.theta = theta; this.phi = phi; }

        /** Conversion from {@link Vec3} */
        public Pointing(Vec3 vec)
        {
            theta = FastMath.atan2(Math.Sqrt(vec.x*vec.x+vec.y*vec.y),vec.z);
            phi = FastMath.atan2 (vec.y,vec.x);
            if (phi<0) phi += 2*Math.PI;
            if (phi>=2*Math.PI) phi -= 2*Math.PI;
        }

        /** Conversion from {@link Zphi} */
        public Pointing (Zphi zphi)
        {
            double xy=Math.Sqrt((1-zphi.z)*(1+zphi.z));
            theta = FastMath.atan2(xy,zphi.z); phi=zphi.phi;
        }
        // for some reason, the alternative below is much slower...
        //{ theta=FastMath.acos(zphi.z); phi=zphi.phi; }

        /** Normalize theta range */
        public void normalizeTheta()
        {
            theta=HealpixUtils.fmodulo(theta,2*Math.PI);
            if (theta>Math.PI)
            {
                phi+=Math.PI;
                theta=2*Math.PI-theta;
            }
        }

        /** Normalize theta and phi ranges */
        public void normalize()
        {
            normalizeTheta();
            phi=HealpixUtils.fmodulo(phi,2*Math.PI);
        }

        public override String ToString()
        {
            return "ptg(" + theta + "," + phi + ")";
        }

        public override bool Equals(Object o)
        {
            if (this==o) return true;
            if ((o==null) || (GetType()!=o.GetType())) return false;
            Pointing pointing = (Pointing) o;
            if (pointing.phi != phi) return false;
            if (pointing.theta != theta) return false;
            return true;
        }

        public override int GetHashCode()
        {
            int result = theta.GetHashCode();
            result = 31 * result + phi.GetHashCode();
            return result;
        }
    }
}
