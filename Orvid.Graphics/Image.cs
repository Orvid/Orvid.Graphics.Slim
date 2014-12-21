//#define DebugDraw // If this is uncommented, all triangles will have a set-color border drawn. This is useful to debug polygon drawing code.
//#define SqrtWorks // Only uncomment this when the square-root works 
//#define DebugImageDraw // Draws a blue box around any image that is drawn.

using System;
using System.Collections.Generic;
using System.Text;

namespace Orvid.Graphics
{
	/// <summary>
	/// This class is used to describe an image.
	/// </summary>
	public class Image : IDisposable
	{
		/// <summary>
		/// The raw data in the image.
		/// </summary>
		public Pixel[] Data;
		/// <summary>
		/// The height of the image.
		/// </summary>
		public int Height
		{
			get { return local_size.Y; }
		}
		/// <summary>
		/// The width of the image.
		/// </summary>
		public int Width
		{
			get { return local_size.X; }
		}

		private Vec2 local_size = Vec2.Zero;
		/// <summary>
		/// The size of the image.
		/// </summary>
		public Vec2 Size
		{
			get { return local_size; }
		}

		/// <summary>
		/// Creates a new image with the specified height and width.
		/// </summary>
		/// <param name="height">The height of the new image.</param>
		/// <param name="width">The width of the new image.</param>
		public Image(int width, int height)
		{
			this.local_size.X = width;
			this.local_size.Y = height;
			this.Data = new Pixel[(Height + 1) * (Width + 1)];
			//string str = "";
		}

		public Image(Vec2 v)
			: this(v.X, v.Y)
		{
		}

		/// <summary>
		/// The default Destructor.
		/// </summary>
		~Image()
		{
			this.Data = null;
		}

		public void Dispose()
		{
			this.Data = null;
			System.GC.Collect();
		}

		#region Clear
		public virtual void Clear(Pixel color)
		{
			for (uint i = 0; i < Data.Length; i++)
			{
				Data[i] = color;
			}
		}
		#endregion

		#region SubImage
		/// <summary>
		/// Gets a sub-image of this image,
		/// from the specified location,
		/// of the specified size.
		/// </summary>
		/// <param name="loc">The location of the image to pull.</param>
		/// <param name="size">The size of the image to pull.</param>
		/// <returns>The sub-image obtained.</returns>
		public Image SubImage(Vec2 loc, Vec2 size)
		{
			Image i = new Image(size);
			for (int y = loc.Y; y < (loc.Y + size.Y); y++)
			{
				for (int x = loc.X; x < (loc.X + size.X); x++)
				{
					i.SetPixel((uint)(x - loc.X), (uint)(y - loc.Y), this.GetPixel((uint)x, (uint)y));
				}
			}
			return i;
		}
		#endregion

		/// <summary>
		/// Get's the pixel a the specified location.
		/// </summary>
		/// <param name="width">The width position of the pixel.</param>
		/// <param name="height">The height position of the pixel.</param>
		/// <returns>The Pixel at the specified position.</returns>
		public virtual Pixel GetPixel(uint x, uint y)
		{
			if (/*x > 0 &&*/ x < Width /*&& y > 0 */&& y < Height)
				return Data[(y * Width) + x];
			else
				return new Pixel(true);
		}

		//public List<Vec2> Modified = new List<Vec2>();

		/// <summary>
		/// Set's the pixel at the specified location, 
		/// to the specified pixel.
		/// </summary>
		/// <param name="width">The width position of the pixel.</param>
		/// <param name="height">The height position of the pixel.</param>
		/// <param name="p">The pixel to set to.</param>
		public virtual void SetPixel(uint x, uint y, Pixel p)
		{
			//Modified.Add(new Vec2(width, height));
			if (p.A != 255)
			{
				if ((p.A == 1) && (p.R + p.G + p.B == 0))
				{
					// This allows us to empty pixels.
					//throw new Exception();
					Data[((y * Width) + x)] = p;
				}
				else if (p.A != 0)
				{
					double r1 = ((double)p.A / 255);
					Pixel cur = Data[((y * Width) + x)];
					double r2 = 1.0d - r1;

					Data[((y * Width) + x)] = new Pixel(
						(byte)((p.R * r1) + (cur.R * r2)),
						(byte)((p.G * r1) + (cur.G * r2)),
						(byte)((p.B * r1) + (cur.B * r2)),
						255
						);
				}
				// else nothing gets drawn.
			}
			else
			{
				Data[((y * Width) + x)] = p;
			}
		}

		#region Operators
		public static explicit operator System.Drawing.Bitmap(Image i)
		{
			System.Drawing.Bitmap b = new System.Drawing.Bitmap(i.Width, i.Height);
			System.Drawing.Imaging.BitmapData bd = b.LockBits(new System.Drawing.Rectangle(0, 0, i.Width, i.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			int bytes = Math.Abs(bd.Stride) * bd.Height;
			byte[] rgbValues = new byte[bytes];
			System.Runtime.InteropServices.Marshal.Copy(bd.Scan0, rgbValues, 0, bytes);
			uint len = (uint)(i.Height * i.Width);
			for (uint d = 0, ind = 0; d < len; d++)
			{
				rgbValues[ind] = i.Data[d].B;
				ind++;
				rgbValues[ind] = i.Data[d].G;
				ind++;
				rgbValues[ind] = i.Data[d].R;
				ind++;
				rgbValues[ind] = i.Data[d].A;
				ind++;
			}
			System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, bd.Scan0, bytes);
			rgbValues = null;
			b.UnlockBits(bd);
			return b;
		}

		public static explicit operator Image(System.Drawing.Bitmap b)
		{
			Image i = new Image(b.Width, b.Height);
			for (uint x = 0; x < b.Width; x++)
			{
				for (uint y = 0; y < b.Height; y++)
				{
					i.SetPixel(x, y, b.GetPixel((int)x, (int)y));
				}
			}
			return i;
		}
		#endregion
	}
}
