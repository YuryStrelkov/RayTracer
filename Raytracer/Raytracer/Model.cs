using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Raytracer
{
    public static class VericesAttribytes
    {
        public static int V_POSITION = 0;
        public static int V_UVS = 1;
        public static int V_NORMAL = 2;
        public static int V_TANGENT = 4;
        public static int V_BITANGENT = 8;
        public static int V_BONES = 16;
        public static int V_BONES_WEIGHTS = 32;
        public static int V_COLOR_RGB = 64;
        public static int V_COLOR_RGBA = 128;
    }

    [Serializable]

    [StructLayout(LayoutKind.Sequential)]

    public struct AttrAndSize
    {
        public int attrType;

        public int attrLength;

        public AttrAndSize(int t, int l)
        {
            attrType = t;
            attrLength = l;
        }

    }

    [Serializable]

    [StructLayout(LayoutKind.Sequential)]

    public class Model
    {
        byte AtribbytesMask;

        private int VertexDataSize;

        public Dictionary<int, AttrAndSize> Attribytes { get;}

        float[] data;

        public int VericesCount { get { return data.Length / VertexDataSize; }} 

        public void MarkBufferAttributes(int VericesAttribytesMap)
       {
            if ((AtribbytesMask == VericesAttribytesMap) && (Attribytes.Count != 0))
            {
                return;
            }

            if ((VericesAttribytesMap & VericesAttribytes.V_POSITION) == VericesAttribytes.V_POSITION)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_POSITION, 3));
                AtribbytesMask |= (byte)VericesAttribytes.V_POSITION;
                VertexDataSize += 3;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_UVS) == VericesAttribytes.V_UVS)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_UVS, 2));
                AtribbytesMask |= (byte)VericesAttribytes.V_UVS;
                VertexDataSize += 2;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_NORMAL) == VericesAttribytes.V_NORMAL)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_NORMAL, 3));
                AtribbytesMask |= (byte)VericesAttribytes.V_NORMAL;
                VertexDataSize += 3;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_TANGENT) == VericesAttribytes.V_TANGENT)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_TANGENT, 3));
                AtribbytesMask |= (byte)VericesAttribytes.V_TANGENT;
                VertexDataSize += 3;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_BITANGENT) == VericesAttribytes.V_BITANGENT)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_BITANGENT, 3));
                AtribbytesMask |= (byte)VericesAttribytes.V_BITANGENT;
                VertexDataSize += 3;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_BONES) == VericesAttribytes.V_BONES)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_BONES, 4));
                AtribbytesMask |= (byte)VericesAttribytes.V_BONES;
                VertexDataSize += 4;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_BONES_WEIGHTS) == VericesAttribytes.V_BONES_WEIGHTS)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_BONES_WEIGHTS, 4));
                AtribbytesMask |= (byte)VericesAttribytes.V_BONES_WEIGHTS;
                VertexDataSize += 4;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_COLOR_RGB) == VericesAttribytes.V_COLOR_RGB)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_COLOR_RGB, 3));
                AtribbytesMask |= (byte)VericesAttribytes.V_COLOR_RGB;
                VertexDataSize += 3;
            }
            if ((VericesAttribytesMap & (byte)VericesAttribytes.V_COLOR_RGBA) == VericesAttribytes.V_COLOR_RGBA)
            {
                Attribytes.Add(Attribytes.Count, new AttrAndSize(VericesAttribytes.V_COLOR_RGBA, 4));
                AtribbytesMask |= (byte)VericesAttribytes.V_COLOR_RGBA;
                VertexDataSize += 4;
            }
        }

        private void AppendVertexData( float[] src, int vertexIndex)
        {
            int shift = vertexIndex;

            if ((AtribbytesMask & VericesAttribytes.V_POSITION) == VericesAttribytes.V_POSITION)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_UVS) == VericesAttribytes.V_UVS)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_NORMAL) == VericesAttribytes.V_NORMAL)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_TANGENT) == VericesAttribytes.V_TANGENT)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_BITANGENT) == VericesAttribytes.V_BITANGENT)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_BONES) == VericesAttribytes.V_BONES)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_BONES_WEIGHTS) == VericesAttribytes.V_BONES_WEIGHTS)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_COLOR_RGB) == VericesAttribytes.V_COLOR_RGB)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
            if ((AtribbytesMask & (byte)VericesAttribytes.V_COLOR_RGBA) == VericesAttribytes.V_COLOR_RGBA)
            {
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
                data[shift] = src[shift]; shift += 1;
            }
        }

        public void LoadData( float[] vdata, int[] idata)
        {
            ///Распараллелить 
             data = new float[vdata.Length];

            Parallel.For(0, idata.Length, (i) =>
            {
                AppendVertexData(vdata, idata[i] *     VertexDataSize);
                AppendVertexData(vdata, idata[i + 1] * VertexDataSize);
                AppendVertexData(vdata, idata[i + 2] * VertexDataSize);
            });
        }

        public Model(int VericesAttribytesMap)
        {
            data = new float[0];

            Attribytes = new Dictionary<int, AttrAndSize>();

            MarkBufferAttributes(VericesAttribytesMap);
        }

        public Model(float[] vdata, int[] idata, int VericesAttribytesMap)
        {
            Attribytes = new Dictionary<int, AttrAndSize>();

            MarkBufferAttributes(VericesAttribytesMap);

            LoadData(vdata, idata);
        }

    }
}
