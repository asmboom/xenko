﻿// Copyright (c) 2014-2016 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Extensions;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Graphics.GeometricPrimitives;
using SiliconStudio.Xenko.Rendering;

namespace SiliconStudio.Xenko.Physics
{
    public class Box2DColliderShape : ColliderShape
    {
        private static MeshDraw cachedDebugPrimitive;

        /// <summary>
        /// Initializes a new instance of the <see cref="Box2DColliderShape"/> class.
        /// </summary>
        /// <param name="size">The half extents.</param>
        public Box2DColliderShape(Vector2 size)
        {
            Type = ColliderShapeTypes.Box;
            Is2D = true;

            InternalShape = new BulletSharp.Box2DShape(size/2) { LocalScaling = new Vector3(1, 1, 0) };

            DebugPrimitiveMatrix = Matrix.Scaling(new Vector3(size.X, size.Y, 0f) * 1.01f);
        }

        public override MeshDraw CreateDebugPrimitive(GraphicsDevice device)
        {
            return cachedDebugPrimitive ?? (cachedDebugPrimitive = GeometricPrimitive.Cube.New(device).ToMeshDraw());
        }
    }
}
