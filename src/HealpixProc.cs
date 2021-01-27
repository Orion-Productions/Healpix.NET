using System;
using System.Collections;
using System.Collections.Generic;

namespace Healpix
{

    /*
    *  This file is ported from Healpix Java.
    */



    /** Procedural interface to the {@link HealpixBase} functionality.
        This class is intended for users who prefer a procedural (instead of
        object-oriented) interface to the HEALPix functionality. It should make
        transition from packages like PixTools easier.

        @copyright 2012 Max-Planck-Society
        @author Martin Reinecke */
    public abstract class HealpixProc : HealpixBase
    {
        public static HealpixBase[] bn = new HealpixBase[HealpixBase.order_max+1],
                            br = new HealpixBase[HealpixBase.order_max+1];
        public static double[] mpr =new double[HealpixBase.order_max+1],
                        cmpr=new double[HealpixBase.order_max+1],
                        smpr=new double[HealpixBase.order_max+1];

        // static
        // {
        //     try 
        //     {
        //         for (int i=0; i<=HealpixBase.order_max; ++i)
        //         {
        //             bn[i]=new HealpixBase(1L<<i, Scheme.NESTED);
        //             br[i]=new HealpixBase(1L<<i, Scheme.RING);
        //             mpr[i]=bn[i].maxPixrad();
        //             cmpr[i]=FastMath.cos(mpr[i]);
        //             smpr[i]=FastMath.sin(mpr[i]);
        //         }
        //     }
        //     catch (Exception Ex) {/*doesn't happen*/}
        // }

        private HealpixProc() {} // this class should not be instantiated

        public static double maxPixrad (int order)
            { return bn[order].maxPixrad(); }

        public static long ang2pixNest (int order, Pointing ptg)
            { return bn[order].ang2pix(ptg); }
        public static long ang2pixRing (int order, Pointing ptg)
            { return br[order].ang2pix(ptg); }

        public static Pointing pix2angNest (int order, long pix)
            { return bn[order].pix2ang(pix); }
        public static Pointing pix2angRing (int order, long pix)
            { return br[order].pix2ang(pix); }

        public static long vec2pixNest (int order, Vec3 vec)
            { return bn[order].vec2pix(vec); }
        public static long vec2pixRing (int order, Vec3 vec)
            { return br[order].vec2pix(vec); }

        public static Vec3 pix2vecNest (int order, long pix)
            { return bn[order].pix2vec(pix); }
        public static Vec3 pix2vecRing (int order, long pix)
            { return br[order].pix2vec(pix); }

        public static long ring2nest (int order, long pix)
            { return bn[order].ring2nest(pix); }
        public static long nest2ring (int order, long pix)
            { return bn[order].nest2ring(pix); }

        public static long[] neighboursNest (int order, long pix)
            { return bn[order].neighbours(pix); }
        public static long[] neighboursRing (int order, long pix)
            { return br[order].neighbours(pix); }

        public static Vec3[] boundariesNest(int order, long pix, int step)
           
            { return bn[order].boundaries(pix,step); }
        public static Vec3[] boundariesRing(int order, long pix, int step)
           
            { return br[order].boundaries(pix,step); }

        public static RangeSet queryDiscNest(int order, Pointing ptg, double radius)
           
            { return bn[order].queryDisc(ptg,radius); }
        public static RangeSet queryDiscRing(int order, Pointing ptg, double radius)
           
            { return br[order].queryDisc(ptg,radius); }
        public static RangeSet queryDiscInclusiveNest(int order, Pointing ptg,
            double radius, int fact)
            { return bn[order].queryDiscInclusive(ptg,radius,fact); }
        public static RangeSet queryDiscInclusiveRing(int order, Pointing ptg,
            double radius, int fact)
            { return br[order].queryDiscInclusive(ptg,radius,fact); }

        public static RangeSet queryPolygonNest(int order, Pointing[] vertex)
           
            { return bn[order].queryPolygon(vertex); }
        public static RangeSet queryPolygonRing(int order, Pointing[] vertex)
           
            { return br[order].queryPolygon(vertex); }
        public static RangeSet queryPolygonInclusiveNest(int order, Pointing[] vertex,
            int fact)
            { return bn[order].queryPolygonInclusive(vertex,fact); }
        public static RangeSet queryPolygonInclusiveRing(int order, Pointing[] vertex,
            int fact)
            { return br[order].queryPolygonInclusive(vertex,fact); }

        public static RangeSet queryStripNest(int order, double theta1, double theta2,
            bool inclusive)
            { return bn[order].queryStrip(theta1,theta2,inclusive); }
        public static RangeSet queryStripRing(int order, double theta1, double theta2,
            bool inclusive)
            { return br[order].queryStrip(theta1,theta2,inclusive); }
    }
}
