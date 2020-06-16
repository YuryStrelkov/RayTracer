using OpenTK;
using System;

namespace Raytracer.Tree
{
    public enum SplitPlane
    {
        XY = 0,
        XZ = 1,
        YZ = 2,
        NONE = -1
    }

    public enum BoxAxis
    {
        X = 0,
        Y = 1,
        Z = 2,
        NONE = -1
    }
    
    public struct Ray
    {
        public Vector4 Origin { get; set; }

        public Vector4 Direction { get; set; }
        /// <summary>
        /// Если пересекаем ограничивающий пар-пид, то это расстояние до первой точки пересечения
        /// </summary>
        public float RayMaxLength { get; set; }
        /// <summary>
        /// Если пересекаем ограничивающий пар-пид, то это расстояние до второй точки пересечения
        /// </summary>
        public float RayMinLength { get; set; }

        public Ray(Vector4 orign, Vector4 direction)
        {
            Origin = orign;
            Direction = direction;
            RayMaxLength = 0;
            RayMinLength = 0;
        }
    }

    public struct BBox
    {
        public Vector3 BBmin { get; private set; }

        public Vector3 BBmax { get; private set; }

        public BoxAxis SplitAxis { get; private set; }

        public float SplitCoord { get; private set; }

        public Vector3 BBSize { get { return new Vector3(BBmax.X - BBmin.X, BBmax.Y - BBmin.Y, BBmax.Z - BBmin.Z); } }

        public Vector3 BBSArea { get {return ComputeArea();} }

        private Vector3 ComputeArea()
        {
            Vector3 size = BBSize;

            return new Vector3(size.X * size.Y,
                               size.X * size.Z,
                               size.Y * size.X);
        }

        public bool RayBBoxIntersection(ref Ray ray)
        {
            float lo = (BBmin.X - ray.Origin.X) / ray.Direction.X;

            float hi = (BBmax.X - ray.Origin.X) / ray.Direction.X;

            ray.RayMinLength = Math.Min(lo, hi);

            ray.RayMaxLength = Math.Max(lo, hi);

            float lo1 = (BBmin.Y - ray.Origin.Y) / ray.Direction.Y;

            float hi1 = (BBmax.Y - ray.Origin.Y) / ray.Direction.Y;

            ray.RayMinLength = Math.Max(ray.RayMinLength, Math.Min(lo1, hi1));

            ray.RayMaxLength = Math.Min(ray.RayMaxLength, Math.Max(lo1, hi1));

            float lo2 = (BBmin.Z - ray.Origin.Z) / ray.Direction.Z;

            float hi2 = (BBmax.Z - ray.Origin.Z) / ray.Direction.Z;

            ray.RayMinLength = Math.Max(ray.RayMinLength, Math.Min(lo2, hi2));

            ray.RayMaxLength = Math.Min(ray.RayMaxLength, Math.Max(lo2, hi2));

            return (ray.RayMinLength <= ray.RayMaxLength) && (ray.RayMaxLength > 0.0f);

        }

        public BoxAxis GetLongestAxis()
        {
            Vector3 size = BBSize;

            BoxAxis s = BoxAxis.X;

            if ((size.Y > size.X))
            {
                s = BoxAxis.Y;
            }

            if ((size.Z > size.Y))
            {
                s = BoxAxis.Z;
            }

            return s;

        }

        public BBox(Vector3 minB, Vector3 maxB)
        {
            BBmin = minB;
            BBmax = maxB;
            SplitAxis = BoxAxis.NONE;
            SplitCoord = 0;
        }
    }
    
    public class KDTree : BTree<int[]>
    {
        public static readonly int MAX_BASKET_CAP = 8;

        private struct KDNode
        {
            BBox bbox;
        }

        public void BuildTree()
        {

        }

        public int FindNearest()
        {
            return 0;
        } 
    }
}
