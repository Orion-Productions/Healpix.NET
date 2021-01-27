
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Healpix
{

    /*
    *  This file is ported from Healpix Java.
    */

    /** Class for compressing/uncompressing monotonous integer sequences.
        Implementation inspired by Moffat and Stuiver 2000: "Binary Interpolative
        Coding for Effective Index Compression", Information Retrieval 3, 25

        @copyright 2014 Max-Planck-Society
        @author Martin Reinecke */
    public class Compressor
    {
        //FIXME: move into separate class
        private class obitstream
        {
            private long bitpos;
            private int curval;
            private MemoryStream strm;

            public obitstream()
            {
                bitpos=0;
                curval=0;
                strm=new MemoryStream();
            }
            private void put_internal (long val, int bits)
            {
                int bitsleft = 8-(int)(bitpos&7);
                if (bits<=bitsleft) // wbits==bits
                {
                    curval |= ((int)(val&((1L<<bits)-1))<<(bitsleft-bits));
                    bitpos+=bits;
                    byte[] arr = BitConverter.GetBytes(curval);
                    if ((bitpos&7)==0) { strm.Write(arr, 0, arr.Length); curval=0; }
                }
                else // wbits==bitsleft
                {
                    curval |= (int)(((long)((ulong)val>>(bits-bitsleft)))&((1<<bitsleft)-1));
                    bitpos+=bitsleft;
                    byte[] arr = BitConverter.GetBytes(curval);
                    if ((bitpos&7)==0) { strm.Write(arr, 0, arr.Length); curval=0; }
                    put_internal(val,bits-bitsleft);
                }
            }

            public void put (long val, int bits)
            {
                if (bits==0) return;
                HealpixUtils.check(bits<=63,"too many bits");
                HealpixUtils.check(val-(val&((1L<<bits)-1))==0,
                    "value has too many bits");
                put_internal(val,bits);
            }

            public byte[] getData()
            {
                if ((bitpos&7)!=0) { byte[] arr = BitConverter.GetBytes(curval); strm.Write(arr, 0, arr.Length); }
                return strm.ToArray();
            }
        }

        //FIXME: move into separate class
        private class ibitstream
        {
            private long bitpos;
            private int curval;
            private byte[] data;

            public ibitstream(byte[] data_in)
            {
                HealpixUtils.check(data_in.Length>0,"empty input array");
                bitpos=0;
                data= (byte[]) data_in.Clone();
            }
            private long get_internal (long val, int bits)
            {
                int bitsleft = 8-(int)(bitpos&7);
                if (bitsleft==8)
                    {curval=data[(int)((ulong)bitpos>>3)]; if (curval<0) curval+=256;}
                if (bits<=bitsleft)
                {
                    val |= ((int)((uint)curval>>(bitsleft-bits))&((1L<<bits)-1));
                    bitpos+=bits;
                }
                else
                {
                    val |= (curval&((1L<<bitsleft)-1))<<(bits-bitsleft);
                    bitpos+=bitsleft;
                    val=get_internal(val,bits-bitsleft);
                }
                return val;
            }
            public long get (int bits)
            {
                if (bits==0) return 0L;
                HealpixUtils.check(bits<=63,"too many bits");
                HealpixUtils.check((bitpos+bits)<=8*(long)data.Length,
                    "reading past end of stream");
                long res=0L;
                return get_internal(res,bits);
            }
        }

        private static void interpol_encode2 (long[] data, int l, int r,
            obitstream obs, int shift)
        {
            if (r-l<=1) return;
            int m=l+(r-l)/2;

            long nval = (long)((ulong)(data[r]-data[l])>>shift) - (r-l) + 1;
            if (nval<=1) return;

            int nb = 1+HealpixUtils.ilog2(nval-1);
            long val = (long)((ulong)data[m]>>shift)-((long)((ulong)data[l]>>shift)+(m-l));
            long nshort=(1L<<nb)-nval;
            if (val<nshort)
            obs.put(val,nb-1);
            else
            obs.put(val+nshort,nb);
            interpol_encode2(data,l,m,obs,shift);
            interpol_encode2(data,m,r,obs,shift);
        }

        /** Return a byte array representing the compressed sequence
            [data[begin]; data[end-1]] */
        public static byte[] interpol_encode (long[] data, int begin, int end)
        {
            obitstream obs=new obitstream();
            if (begin>=end) // empty range
            { obs.put(0,8); return obs.getData(); }
            HealpixUtils.check(data[begin]>=0,"numbers must be nonnegative");
            long combo=data[begin];
            for (int i=begin+1; i<end; ++i)
            {
                HealpixUtils.check(data[i-1]<data[i],"numbers not strictly increasing");
                combo|=data[i];
            }
            //determine allowable right shift
            int shift = (combo!=0? 64-Convert.ToString(combo,2).Length : 64);

            long maxnum= (long)((ulong)data[end-1]>>shift);
            if (end-begin>maxnum) maxnum = end-begin;
            int maxbits=1+HealpixUtils.ilog2(maxnum);
            obs.put(maxbits,8);
            obs.put(shift,8);
            obs.put(end-begin,maxbits);
            obs.put((long)((ulong)data[begin]>>shift),maxbits);
            if (end-begin==1) return obs.getData(); // a single data entry
            obs.put((long)((ulong)data[end-1]>>shift),maxbits);
            interpol_encode2(data,begin,end-1,obs,shift);
            return obs.getData();
        }

        private static void interpol_decode2 (long[] data, int l, int r,
            ibitstream ibs, int shift)
        {
            if (r-l<=1) return;
            int m=l+(r-l)/2;

            long nval = (long)((ulong)(data[r]-data[l])>>shift) - (r-l) + 1;
            long val=0;

            if (nval>1)
            {
                int nb = 1+HealpixUtils.ilog2(nval-1);
                long nshort=(1L<<nb)-nval;
                val=ibs.get(nb-1);
                if (val>=nshort)
                    val=(val<<1)+ ibs.get(1) - nshort;
            }
            data[m]=data[l]+(((m-l)+val)<<shift);

            interpol_decode2(data,l,m,ibs,shift);
            interpol_decode2(data,m,r,ibs,shift);
        }

        /** Return an array containing the number sequence decompressed from data. */
        public static long[] interpol_decode (byte[] data)
        {
            ibitstream ibs=new ibitstream(data);
            int maxbits=(int)ibs.get(8);
            if (maxbits==0) return new long[0];
            int shift=(int)ibs.get(8);
            long[] v=new long[(int)ibs.get(maxbits)];
            v[0]=ibs.get(maxbits)<<shift;
            if (v.Length==1) return v;
            v[v.Length-1]=ibs.get(maxbits)<<shift;
            interpol_decode2(v,0,v.Length-1,ibs,shift);
            return v;
        }
    }
}
