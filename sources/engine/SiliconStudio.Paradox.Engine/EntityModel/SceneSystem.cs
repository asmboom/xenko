// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.

using SiliconStudio.Core;
using SiliconStudio.Core.Serialization.Assets;
using SiliconStudio.Paradox.Effects;
using SiliconStudio.Paradox.Engine;
using SiliconStudio.Paradox.Engine.Graphics;
using SiliconStudio.Paradox.Games;

namespace SiliconStudio.Paradox.EntityModel
{
    /// <summary>
    /// The scene system handles the scenes of a game.
    /// </summary>
    public class SceneSystem : GameSystemBase
    {
        private const string DefaultSceneName = "__DefaultScene__"; // TODO: How to determine the default scene?

        private RenderContext renderContext;

        private RenderFrame mainRenderFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSystemBase" /> class.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <remarks>The GameSystem is expecting the following services to be registered: <see cref="IGame" /> and <see cref="IAssetManager" />.</remarks>
        public SceneSystem(IServiceRegistry registry)
            : base(registry)
        {
            registry.AddService(typeof(SceneSystem), this);
            Enabled = true;
            Visible = true;
        }

        /// <summary>
        /// Gets or sets the root scene.
        /// </summary>
        /// <value>The scene.</value>
        /// <exception cref="System.ArgumentNullException">Scene cannot be null</exception>
        public SceneInstance SceneInstance { get; set; }

        protected override void LoadContent()
        {
            var assetManager = Services.GetSafeServiceAs<AssetManager>();

            // TODO: Temp work around for PreviewGame init
            //    // Preload the scene if it exists
            //    if (assetManager.Exists(DefaultSceneName))
            //    {
            //        Scene = assetManager.Load<Scene>(DefaultSceneName);
            //    }

            mainRenderFrame = RenderFrame.FromTexture(GraphicsDevice.BackBuffer, GraphicsDevice.DepthStencilBuffer);

            // Create the drawing context
            renderContext = RenderContext.GetShared(Services);
        }

        public override void Update(GameTime gameTime)
        {
            if (SceneInstance != null)
            {
                SceneInstance.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if (SceneInstance == null)
            {
                return;
            }

            renderContext.Time = gameTime;

            // Draw the scene
            SceneInstance.Draw(renderContext, mainRenderFrame);
        }
    }
}