﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using System;
using SiliconStudio.Core;
using SiliconStudio.Core.Mathematics;

namespace SiliconStudio.Xenko.Particles.Initializers
{
    [DataContract("Initial3DRotationSeed")]
    [Display("Initial 3D Rotation by seed")]
    public class Initial3DRotationSeed : ParticleInitializer
    {
        public Initial3DRotationSeed()
        {
            RequiredFields.Add(ParticleFields.Quaternion);
            RequiredFields.Add(ParticleFields.RandomSeed);

            InheritLocation = InheritLocation.Rotation;
        }

        public unsafe override void Initialize(ParticlePool pool, int startIdx, int endIdx, int maxCapacity)
        {
            if (!pool.FieldExists(ParticleFields.Quaternion) || !pool.FieldExists(ParticleFields.RandomSeed))
                return;

            var rotField = pool.GetField(ParticleFields.Quaternion);
            var rndField = pool.GetField(ParticleFields.RandomSeed);

            var i = startIdx;
            while (i != endIdx)
            {
                var particle = pool.FromIndex(i);
                var randSeed = particle.Get(rndField);

                var randomRotation = Quaternion.Slerp(RotationQuaternionMin, RotationQuaternionMax, randSeed.GetFloat(RandomOffset.Offset1A + SeedOffset));
                
                // Results in errors ("small" quaternion) when interpolation -90 to +90 degree rotations, but they are not rotated anyway
                //randomRotation.Normalize();
            
                (*((Quaternion*)particle[rotField])) = randomRotation * WorldRotation;

                i = (i + 1) % maxCapacity;
            }
        }

        [DataMember(8)]
        [Display("Seed offset")]
        public UInt32 SeedOffset { get; set; } = 0;

        [DataMember(30)]
        [Display("Rotation min")]
        public Quaternion RotationQuaternionMin { get; set; } = new Quaternion(0, 0, 0, 1);

        [DataMember(40)]
        [Display("Rotation max")]
        public Quaternion RotationQuaternionMax { get; set; } = new Quaternion(0, 0, 0, 1);
        
    }
}
