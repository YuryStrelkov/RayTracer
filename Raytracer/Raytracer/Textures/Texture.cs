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
        public int Channels { get; private set; }

        public int ChannelSize { get; private set; }

        public int PixelByteSize { get { return Channels * ChannelSize; } }

        public PixFormat(int channels, int channelSize)
        {
            Channels = channels;
            ChannelSize = channelSize;
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

        public void ToBitmap(ref Bitmap bm)
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

                int stride = picData.Stride;

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

                int stride = picData.Stride;

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

            Format = new PixFormat(3, sizeof(byte));
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

                    Stride = bmData.Stride;

                    Pixels = new byte[Stride * Rows];

                    unsafe
                    {
                        byte* pntr = (byte*)bmData.Scan0.ToPointer();

                        Parallel.For(0, Rows, (row) =>
                        {
                            int index = row * Stride;

                            for (int col = 0; col < Coloms; col++)
                            {
                                Pixels[index + col * 3]     = *pntr;
                                Pixels[index + col * 3 + 1] = *(pntr + 1);
                                Pixels[index + col * 3 + 2] = *(pntr + 2);
                            }
                        });
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occured while texture : " + src + " creation...");

                Pixels = new byte[4];

                Pixels[0] = (255);

                Stride = 4;

                Coloms = 1;

                Rows = 1;
            }
        }

        public Texture(int w, int h)
        {
            Format = new PixFormat(3, sizeof(byte));

            int stride = w * Format.Channels* Format.ChannelSize;

            stride = stride + stride % 4;

            Pixels = new byte[stride * h];

            SyncObj = new object();

            Coloms = w;

            Rows = h;
        }

        public Texture(int w, int h, PixFormat format)
        {
            Format = format;

            int stride = w * Format.Channels * Format.ChannelSize;

            stride = stride + stride % 4;

            Pixels = new byte[stride * h];

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
