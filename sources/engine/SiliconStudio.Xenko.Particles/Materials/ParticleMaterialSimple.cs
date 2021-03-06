﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Annotations;
using SiliconStudio.Core.Mathematics;
using SiliconStudio.Xenko.Graphics;
using SiliconStudio.Xenko.Particles.Sorters;
using SiliconStudio.Xenko.Particles.VertexLayouts;
using SiliconStudio.Xenko.Rendering;

namespace SiliconStudio.Xenko.Particles.Materials
{
    public enum ParticleMaterialCulling : byte
    {
        CullNone = 0,
        CullBack = 1,
        CullFront = 2
    }

    /// <summary>
    /// Simple base for most of the particle material classes which uses additive-alpha blending, face culling and setups the color vertex stream
    /// </summary>
    [DataContract("ParticleMaterialSimple")]
    public abstract class ParticleMaterialSimple : ParticleMaterial
    {
        /// <summary>
        /// Shows if the particles should be rendered as alhpa-blended, additive or something in-between (lerp between the two methods)
        /// </summary>
        /// <userdoc>
        /// Defines if the particles should be rendered as alpha-blended (0), additive (1) or something in-between (any value between 0 and 1)
        /// </userdoc>
        [DataMember(20)]
        [DataMemberRange(0, 1, 0.001, 0.1)]
        [Display("Alpha-Add")]
        public float AlphaAdditive { get; set; } = 0f;

        /// <summary>
        /// Allows the particle shape to be back- or front-face culled.
        /// </summary>
        /// <userdoc>
        /// The default option is no culling, other possible options are back-face culling and front-face culling. Culling both faces at the same time is not an option.
        /// </userdoc>
        [DataMember(40)]
        [Display("Culling")]
        public ParticleMaterialCulling FaceCulling { get; set; } = ParticleMaterialCulling.CullNone;

        /// <summary>
        /// Indicates if this material requires a color field in the vertex stream
        /// </summary>
        protected bool HasColorField { get; private set; } = false;

        /// <inheritdoc />
        public override void PrepareForDraw(ParticleVertexBuilder vertexBuilder, ParticleSorter sorter)
        {
            base.PrepareForDraw(vertexBuilder, sorter);

            // Probe if the particles have a color field and if we need to support it
            var colorField = sorter.GetField(ParticleFields.Color);
            if (colorField.IsValid() != HasColorField)
            {
                VertexLayoutHasChanged = true;
                HasColorField = colorField.IsValid();
            }
        }

        /// <inheritdoc />
        public override void Setup(GraphicsDevice graphicsDevice, RenderContext context, Matrix viewMatrix, Matrix projMatrix, Color4 color)
        {
            base.Setup(graphicsDevice, context, viewMatrix, projMatrix, color);

            // Setup graphics device - culling, blend states and depth testing

            if (FaceCulling == ParticleMaterialCulling.CullNone) graphicsDevice.SetRasterizerState(graphicsDevice.RasterizerStates.CullNone);
            if (FaceCulling == ParticleMaterialCulling.CullBack) graphicsDevice.SetRasterizerState(graphicsDevice.RasterizerStates.CullBack);
            if (FaceCulling == ParticleMaterialCulling.CullFront) graphicsDevice.SetRasterizerState(graphicsDevice.RasterizerStates.CullFront);

            graphicsDevice.SetBlendState(graphicsDevice.BlendStates.AlphaBlend);

            graphicsDevice.SetDepthStencilState(graphicsDevice.DepthStencilStates.DepthRead);

            ///////////////
            // This should be CB0 - view/proj matrices don't change per material
            SetParameter(ParticleBaseKeys.MatrixTransform, viewMatrix * projMatrix);

            // Scale up the color intensity - might depend on the eye adaptation later
            SetParameter(ParticleBaseKeys.ColorScale, color);

            SetParameter(ParticleBaseKeys.ColorIsSRgb, graphicsDevice.ColorSpace == ColorSpace.Linear);

            // This is correct. We invert the value here to reduce calculations on the shader side later
            SetParameter(ParticleBaseKeys.AlphaAdditive, 1f - AlphaAdditive);
        }

        /// <inheritdoc />
        public override unsafe void PatchVertexBuffer(ParticleVertexBuilder vertexBuilder, Vector3 invViewX, Vector3 invViewY, ParticleSorter sorter)
        {
            // If you want, you can integrate the base builder here and not call it. It should result in slight speed up
            base.PatchVertexBuffer(vertexBuilder, invViewX, invViewY, sorter);

            var colorField = sorter.GetField(ParticleFields.Color);
            if (!colorField.IsValid())
                return;

            var colAttribute = vertexBuilder.GetAccessor(VertexAttributes.Color);
            if (colAttribute.Size <= 0)
                return;

            foreach (var particle in sorter)
            {
                // Set the vertex color attribute to the particle's color field
                var color = (uint)(*(Color4*)particle[colorField]).ToRgba();
                vertexBuilder.SetAttributePerParticle(colAttribute, (IntPtr)(&color));

                vertexBuilder.NextParticle();
            }

            vertexBuilder.RestartBuffer();
        }

    }
}
