using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Raytracer.Tree;

namespace Raytracer.Textures
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct PixelColor
    {
        public byte R;
        public byte G;
        public byte B;

        public void Mix(PixelColor oter)
        {
            R *= oter.R;
            G *= oter.G;
            B *= oter.B;
        }

        public void Add(PixelColor oter)
        {
            R += oter.R;
            G += oter.G;
            B += oter.B;
        }

        public PixelColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    };

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct PixFormat
    {   
        /// <summary>
        /// Number of channels
        /// </summary>
        public int Channels { get; private set; }
  
        /// <summary>
        /// Bits per pixel channel
        /// </summary>
        public int BPPChannel { get; private set; }
  
        /// <summary>
        /// Pixel size in bits
        /// </summary>
        public int PixelBitsSize { get; private set; }

        public PixFormat(int channels, int bpp)
        {
            Channels = channels;
            BPPChannel = bpp;
            PixelBitsSize = channels * bpp;
        }
    }

    [Serializable]
    public class Texture
    {
        private object SyncObj;

        public byte[] Pixels { get; private set; }

        public int Stride { get; private set; }

        public int Coloms { get; private set; }

        public int Rows { get; private set; }

        private int CalcStride(int w,int bytesPerPix)
        {
            int strtide = w * bytesPerPix;

            if (strtide % 4 != 0)
            {
                strtide = strtide + 4 - strtide % 4;
            }
            return strtide;
        }

        public void ToBitmap( Bitmap bm)
        {
            if (bm.Width != Coloms)
            {
                return;
            }

            if (bm.Height != Rows)
            {
                return;
            }
            lock (SyncObj)
            {
                BitmapData picData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, bm.PixelFormat);

                IntPtr pixel = picData.Scan0;

                Marshal.Copy(Pixels, 0, pixel, Pixels.Length);

                bm.UnlockBits(picData);
            }
        }

        public Bitmap ToBitmap()
        {
            lock (SyncObj)
            {
                Bitmap bm = new Bitmap(Rows, Coloms, PixelFormat.Format24bppRgb);

                BitmapData picData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, bm.PixelFormat);

                IntPtr pixel = picData.Scan0;

                Marshal.Copy(Pixels, 0, pixel, Pixels.Length);

                bm.UnlockBits(picData);

                return bm;
            }

        }

        public PixFormat Format { get; private set; }

        public PixelColor ColorAt(float px, float py)
        {
            px = Math.Abs(px % 1);

            py = Math.Abs(py % 1);

            return ColorAt((int)(px * (Coloms - 1)), (int)(py * (Rows - 1)));
        }

        private PixelColor ColorAt(int px, int py)
        {
            int pixIndex = py * Stride + px;

            return new PixelColor(Pixels[pixIndex], Pixels[pixIndex + 1], Pixels[pixIndex + 2]);
        }

        public void ClearTexture()
        {
            Pixels.ChangeEach(x => 0);
        }

        public void SetPixel(float px, float py, PixelColor color)
        {
            SetPixel(px, py, color.R, color.G, color.B);
        }

        public void SetPixel(float px, float py, float r, float g, float b)
        {
            SetPixel(px, py,  (byte)(255 * r), (byte)(255 * g), (byte)(255 * b));
        }

        public void SetPixel(float px, float py, byte r, byte g, byte b )
        {
            px = Math.Abs(px % 1);

            py = Math.Abs(py % 1);

            int idx = (int)(py * (Rows - 1)) * Stride + (int)(px * (Coloms - 1));

            lock (SyncObj)
            {
                Pixels[idx] = r;
                Pixels[idx + 1] = g;
                Pixels[idx + 2] = b;
            }

        }

        public Texture(float r, float g, float b)
        {
            SyncObj = new object();

            Pixels = new byte[4];

            Pixels[0] = (byte)(255 * r);

            Pixels[1] = (byte)(255 * g);

            Pixels[2] = (byte)(255 * b);

            Stride = 4;

            Coloms = 1;

            Rows = 1;

            Format = new PixFormat(3, 8);
        }

        private PixFormat GetPixelFormat(PixelFormat pf)
        {
            if (PixelFormat.Format16bppArgb1555 == pf)
            {
                return new PixFormat(4,4);
            }

            if (PixelFormat.Format16bppRgb555 == pf)
            {
                return new PixFormat(4, 4);
            }

            if (PixelFormat.Format16bppGrayScale == pf)
            {
                return new PixFormat(4, 4);
            }

            if (PixelFormat.Format16bppRgb565 == pf)
            {
                return new PixFormat(4, 4);
            }

            if (PixelFormat.Format24bppRgb == pf)
            {
                return new PixFormat(3, 8);
            }

            if (PixelFormat.Format32bppArgb == pf)
            {
                return new PixFormat(4, 8);
            }

            if (PixelFormat.Format32bppRgb == pf)
            {
                return new PixFormat(4, 8);
            }

            if (PixelFormat.Format48bppRgb == pf)
            {
                return new PixFormat(3, 16);
            }

            if (PixelFormat.Format4bppIndexed == pf)
            {
                return new PixFormat(1, 4);
            }

            if (PixelFormat.Format64bppArgb == pf)
            {
                return new PixFormat(4, 16);
            }

            if (PixelFormat.Format8bppIndexed == pf)
            {
                return new PixFormat(1, 8);
            }
            
            if (PixelFormat.Format1bppIndexed == pf)
            {
                return new PixFormat(1, 1);
            }

            return new PixFormat(3, 8);

        }

        public void LoadTexture(string src)
        {
             try
            {
                using (Bitmap bm = new Bitmap(src))
                {
                    BitmapData bmData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadWrite, bm.PixelFormat);

                    Coloms = bm.Width;

                    Rows = bm.Height;

                    Format = GetPixelFormat(bm.PixelFormat);

                    Stride = CalcStride(Coloms, 3);

                    Pixels = new byte[Stride * Rows];

                    unsafe
                    {
                        byte* pntr = (byte*)bmData.Scan0.ToPointer();

                        if (bm.Palette.Entries.Length != 0)
                        {
                            Color[] colors = bm.Palette.Entries;

                            Parallel.For(0, Rows, (row) =>
                            {
                                    int paletteInex = 0;

                                    int index =  row * Stride;
                                
                                    int ptrIdx = row * bmData.Stride;

                                        for (int col = 0; col < Coloms; col++)
                                        {
                                             paletteInex = pntr[ptrIdx++] >> (Format.PixelBitsSize % 8);

                                             Pixels[index++] = colors[paletteInex].B;

                                             Pixels[index++] = colors[paletteInex].G;

                                             Pixels[index++] = colors[paletteInex].R;
                                        }
                              });

                            bm.UnlockBits(bmData);

                            return;
                        }

                        Parallel.For(0, Rows, (row) =>
                       {
                            int index = row * Stride;

                            for (int col = 0; col < Coloms; col++)
                            {

                                Pixels[index] = *(pntr + index);

                                index++;

                                Pixels[index] = *(pntr + index);

                                index++;

                                Pixels[index] = *(pntr + index);

                                index++;

                           }
                       });

                        bm.UnlockBits(bmData);
                    }

                }
            }

            catch (Exception e)
            {
                Console.WriteLine("Error occured while texture : " + src + " creation...");

                Pixels = new byte[4];

                Pixels[2] = (255);

                Stride = 4;

                Coloms = 1;

                Rows = 1;
            }
        }

        public Texture(int w, int h)
        {
            Format = new PixFormat(3, sizeof(byte)*8);
            
            Stride = CalcStride(w, 3);

            Pixels = new byte[Stride * h];

            SyncObj = new object();

            Coloms = w;

            Rows = h;
        }

        public Texture(int w, int h, PixFormat format)
        {
            Format = format;

            Stride = CalcStride(w, 3);

            Pixels = new byte[Stride * h];

            SyncObj = new object();

            Coloms = w;

            Rows = h;
        }

        public Texture(string src)
        {
            Pixels = new byte[1];

            Stride = 0;

            Coloms = 0;

            Rows = 0;

            SyncObj = new object();

            Format = new PixFormat(3, sizeof(byte));

            LoadTexture(src);
        }
    }
}
