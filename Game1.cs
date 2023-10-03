using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace treeNote
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseHandler handler;
        Node masterNode;
        Vector2 mid;
        SpriteFont font;
        UIAligner uia;

        public Game1()
        {
            // Initialize the game and set preferred screen resolution to match the current display mode.
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            Content.RootDirectory = "Content";

            // Make the mouse cursor visible.
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Create the master node for the project and initialize other necessary variables.
            masterNode = Initializer.createMasterNode();

            handler = new MouseHandler();
            uia = new UIAligner();
            mid = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            font = Content.Load<SpriteFont>("font");

            // Load project data, refresh thumbnails, and unvisit nodes.
            masterNode.applyToChilds(x => Convertor.convert(Config.notesDir + x.name + Config.ext));
            masterNode.unvisit();
            masterNode.applyToChilds(x => x.reloadThumbnail(graphics.GraphicsDevice));
            masterNode.unvisit();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Initialize the sprite batch for drawing.
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // Check for exit command (Escape key or Back button on gamepad).
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Check if the screen resolution has changed and apply changes if needed.
            if (graphics.PreferredBackBufferWidth != graphics.GraphicsDevice.Viewport.Width
                || graphics.PreferredBackBufferHeight != graphics.GraphicsDevice.Viewport.Height)
            {
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.Viewport.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.Viewport.Height;
                graphics.ApplyChanges();
            }
            mid = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);

            // Update mouse and keyboard input, align UI elements, and update nodes.
            var mouseState = Mouse.GetState();
            var kbState = Keyboard.GetState();
            handler.update(
                (mouseState.LeftButton == ButtonState.Pressed),
                kbState.IsKeyDown(Keys.LeftControl),
                new Vector2(mouseState.X, mouseState.Y) - mid,
                masterNode
            );

            masterNode.unvisit();
            masterNode.applyToChilds(x => uia.dump(x));
            uia.align();
            masterNode.unvisit();

            masterNode.applyToChilds(x => x.update());
            masterNode.unvisit();

            masterNode.applyToChildsAndParent(x => FileHandler.checkForDelete(x));
            masterNode.unvisit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen with the specified background color.
            GraphicsDevice.Clear(Config.background);

            spriteBatch.Begin();
            // Reload thumbnails and draw nodes with their respective information.
            masterNode.applyToChilds(x => x.reloadThumbnail(graphics.GraphicsDevice));
            masterNode.unvisit();
            masterNode.applyToChilds(x => x.draw(spriteBatch, mid + handler.offset, font));
            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected override void OnExiting(Object sender, EventArgs args)
        {
            // Perform cleanup and save project data on exit.
            Initializer.onExit(masterNode);
            base.OnExiting(sender, args);
        }
    }
}

