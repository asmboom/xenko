﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using SiliconStudio.Xenko.Graphics;

namespace SiliconStudio.Xenko.Rendering
{
    /// <summary>
    /// Used to detect parameters change for dynamic effect.
    /// </summary>
    internal class DynamicEffectParameterUpdaterDefinition : ParameterUpdaterDefinition
    {
        internal object[] SortedCompilationValues;

        internal int[] SortedCounters;

        internal int[] SortedLevels;

        internal ParameterCollection Parameters;

        public DynamicEffectParameterUpdaterDefinition(Effect effect, ParameterCollection usedParameters)
        {
            Initialize(effect, usedParameters);
        }

        private void Initialize(Effect effect, ParameterCollection usedParameters)
        {
            if (effect == null) throw new ArgumentNullException("effect");

            // TODO: Should we ignore various compiler keys such as CompilerParameters.GraphicsPlatformKey, CompilerParameters.GraphicsProfileKey and CompilerParameters.DebugKey?
            //       That was done previously in Effect.CompilerParameters
            // TODO: Should we clone usedParameters? Or somehow make sure it is immutable? (for now it uses the one straight from EffectCompiler, which might not be a good idea...)
            Parameters = usedParameters;
            var parameters = usedParameters;

            var internalValues = parameters.InternalValues;
            SortedKeys = new ParameterKey[internalValues.Count];
            SortedKeyHashes = new ulong[internalValues.Count];
            SortedCompilationValues = new object[internalValues.Count];
            SortedCounters = new int[internalValues.Count];

            for (int i = 0; i < internalValues.Count; ++i)
            {
                var internalValue = internalValues[i];

                SortedKeys[i] = internalValue.Key;
                SortedKeyHashes[i] = internalValue.Key.HashCode;
                SortedCompilationValues[i] = internalValue.Value.Object;
                SortedCounters[i] = internalValue.Value.Counter;
            }

            var keyMapping = new Dictionary<ParameterKey, int>();
            for (int i = 0; i < SortedKeys.Length; i++)
                keyMapping.Add(SortedKeys[i], i);
            Parameters.SetKeyMapping(keyMapping);
        }

        public void UpdatedUsedParameters(Effect effect, ParameterCollection parameters)
        {
            // Try to update counters only if possible
            var internalValues = parameters.InternalValues;
            bool parameterKeyMatches = SortedCounters.Length == internalValues.Count;

            if (parameterKeyMatches)
            {
                for (int i = 0; i < internalValues.Count; ++i)
                {
                    if (SortedKeys[i] != internalValues[i].Key)
                    {
                        parameterKeyMatches = false;
                        break;
                    }
                    SortedCounters[i] = internalValues[i].Value.Counter;
                }
            }

            // Somehow, the used parameters changed, we need a full reset
            if (!parameterKeyMatches)
            {
                Initialize(effect, parameters);
            }
        }
    }
}