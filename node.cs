using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Define the namespace for this code
namespace treeNote
{
    // Define the Node class
    class Node
    {
        // Position of the node
        public Vector2 pos;
        // Desired position to allow smooth movement
        public Vector2 desPos;
        // Radius of the node
        public double radius;
        // Boolean to track if the node has been visited during depth-first search
        public bool visited;
        // Boolean to indicate if the node is selected
        public bool selected;
        // Name of the node
        public string name;
        // List of child nodes
        public List<Node> childs;
        // Boolean to check if node needs to be reloaded
        public bool needReload;
        // Boolean to determine if a thumbnail should be drawn
        public bool drawThumbnail;
        // Texture for the thumbnail
        Texture2D? thumbnail;

        // Constructor for Node class
        public Node(Vector2 Pos, double r, string Name)
        {
            pos = Pos;
            desPos = pos;
            radius = r;
            name = Name;
            childs = new List<Node>();
            selected = false;
            drawThumbnail = false;
            needReload = true;
        }

        // Method to draw the node
        public void draw(SpriteBatch sb, Vector2 mid, SpriteFont font)
        {
            // The "master" node should not be drawn
            if (name == "master") return;
            Vector2 actPos = pos + mid;

            // Draw the circle of the node
            Primitives.drawCircle(sb, actPos, (float)radius, 30, Config.nodes, Config.defaultLineWidth);

            // Draw a selected circle if the node is selected
            if (selected) Primitives.drawCircle(sb, actPos, (float)radius - 1, 30, Config.selected, Config.defaultLineWidth);

            // Draw the text for the node
            Vector2 size = font.MeasureString(name);
            sb.DrawString(font, name, actPos + new Vector2(0, (int)radius + 10) - size / 2, Color.White);

            // Draw arrows to all child nodes
            foreach (Node n in childs)
            {
                Vector2 diff = Vector2.Normalize(n.pos + mid - actPos);
                Primitives.drawArrow(sb, actPos + diff * (float)radius, n.pos + mid - diff * (float)radius, Config.arrows, 1.5f);
            }

            // Draw the thumbnail if it should be drawn and exists
            if (drawThumbnail && thumbnail != null)
            {
                Rectangle thumbnailRect = getThumbnailRect();
                thumbnailRect = new Rectangle(thumbnailRect.X + (int)mid.X,
                                              thumbnailRect.Y + (int)mid.Y,
                                              thumbnailRect.Width,
                                              thumbnailRect.Height);
                sb.Draw(thumbnail, thumbnailRect, Color.White);
            }
        }

        // Method to get the rectangle for the thumbnail
        public Rectangle getThumbnailRect()
        {
            return new Rectangle((int)pos.X - Config.thumbnailSizeX - (int)(radius * 1.5),
                                (int)pos.Y - Config.thumbnailSizeY / 2,
                                Config.thumbnailSizeX,
                                Config.thumbnailSizeY);
        }

        // Method to move the node to a new position
        public void move(Vector2 to)
        {
            desPos = to;
        }

        // Method to update the node's position
        public void update()
        {
            // If the mouse is on this node, don't move it
            if (drawThumbnail) return;
            // Slowly move to the desired location
            if (pos == desPos) return;
            Vector2 dif = desPos - pos;
            if (dif.Length() < 1)
            {
                pos = desPos;
                return;
            }
            pos += dif / 10;
        }

        // Method to apply a function to all child nodes
        public void applyToChilds(Action<Node> foo)
        {
            // Recursive action applicator
            visited = true;
            foreach (Node n in childs)
            {
                if (n.visited) continue;
                n.applyToChildsAndParent(foo);
            }
        }

        // Method to apply a function to both child nodes and parent node
        public void applyToChildsAndParent(Action<Node> foo)
        {
            // Recursive action applicator
            visited = true;
            foo(this);
            applyToChilds(foo);
        }

        // Method to unvisit the node and its child nodes
        public void unvisit()
        {
            visited = false;
            foreach (Node n in childs)
            {
                n.unvisit();
            }
        }

        // Method to create a child node
        public Node createChild(Vector2 tpos)
        {
            string newName = name + "subtopic";
            // Check if the new name already exists and add a number if needed
            if (File.Exists(Config.notesDir + newName + Config.ext))
            {
                int x = 1;
                while (File.Exists(Config.notesDir + newName + x + Config.ext))
                {
                    x += 1;
                }
                newName = name + "subtopic" + x;
            }

            // Make the child node smaller
            double rad = name == "master" ? Config.defaultNodeSize : radius * Config.childScaler;
            Node c = new Node(tpos, rad, newName);
            childs.Add(c);
            return c;
        }

        // Method to reload the thumbnail of the node
        public void reloadThumbnail(GraphicsDevice graphics)
        {
            // Check if the thumbnail needs to be reloaded
            if (!needReload) return;
            needReload = false;
            try
            {
                Console.WriteLine($"Reloading thumbnail for {name}");
                using (FileStream stream = new FileStream(Config.cacheDir + name + "-thumbnail-1.png", FileMode.Open))
                {
                    Texture2D rawPng = Texture2D.FromStream(graphics, stream); // Load the generated thumbnail

                    // Calculate the cropped boundary 
                    Rectangle newBounds = rawPng.Bounds;
                    int resizeBy = Config.cropThumbnail;
                    newBounds.X += resizeBy;
                    newBounds.Y += resizeBy;
                    newBounds.Width -= resizeBy * 2;
                    newBounds.Height -= resizeBy * 2;

                    // Create a new texture of the desired size
                    Texture2D croppedTexture = new Texture2D(graphics, newBounds.Width, newBounds.Height);

                    // Copy the data from the cropped region into a buffer, then into the new texture
                    Color[] data = new Color[newBounds.Width * newBounds.Height];
                    rawPng.GetData(0, newBounds, data, 0, newBounds.Width * newBounds.Height);
                    croppedTexture.SetData(data);
                    thumbnail = croppedTexture;
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine(Config.cacheDir + name + "-thumbnail-1.png" + " was not generated");
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine(Config.cacheDir + name + "-thumbnail-1.png" + " the image format is not supported or was not generated properly");
            }
            catch (DirectoryNotFoundException)
            {
                Initializer.printNoInit();
            }
        }
    }
}

