using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace treeNote
{
    // A static class for drawing various primitives in XNA.

    public static class Primitives
    {
        // A 1x1 white pixel texture to be used for drawing lines and shapes.
        private static Texture2D pixel;

        // A cache to store precalculated circles for efficient drawing.
        private static readonly Dictionary<String, List<Vector2>> circleCache = new Dictionary<string, List<Vector2>>();

        // Helper method to create a 1x1 white pixel texture.
        private static void createThePixel(SpriteBatch spriteBatch)
        {
            pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            pixel.SetData(new[] { Color.White });
        }

        // Draw a line from a starting point with a given length, angle, color, and thickness.
        public static void drawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness)
        {
            if (pixel == null)
            {
                createThePixel(spriteBatch);
            }

            // Stretch the pixel between the two vectors to draw the line.
            spriteBatch.Draw(pixel,
                             point,
                             null,
                             color,
                             angle,
                             Vector2.Zero,
                             new Vector2(length, thickness),
                             SpriteEffects.None,
                             0);
        }

        // Draw a line between two points with a specified color and thickness.
        public static void drawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // Calculate the distance between the two points.
            float distance = Vector2.Distance(point1, point2);

            // Calculate the angle between the two points.
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            // Call the previous method to draw the line.
            drawLine(spriteBatch, point1, distance, angle, color, thickness);
        }

        // Helper method to draw a series of connected points with specified color and thickness.
        private static void drawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness)
        {
            if (points.Count < 2) return;
            for (int i = 1; i < points.Count; i++)
            {
                drawLine(spriteBatch, points[i - 1] + position, points[i] + position, color, thickness);
            }
        }

        // Helper method to create a circle with a given radius and number of sides.
        private static List<Vector2> createCircle(double radius, int sides)
        {
            // Look for a cached version of this circle to avoid redundant calculations.
            String circleKey = radius + "x" + sides;
            if (circleCache.ContainsKey(circleKey))
            {
                return circleCache[circleKey];
            }

            List<Vector2> vectors = new List<Vector2>();

            const double max = 2.0 * Math.PI;
            double step = max / sides;

            for (double theta = 0.0; theta < max; theta += step)
            {
                vectors.Add(new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta))));
            }

            // Add the first vector again to close the circle.
            vectors.Add(new Vector2((float)(radius * Math.Cos(0)), (float)(radius * Math.Sin(0))));

            // Cache this circle for future use.
            circleCache.Add(circleKey, vectors);

            return vectors;
        }

        // Draw a circle with a specified center, radius, number of sides, color, and thickness.
        public static void drawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness)
        {
            drawPoints(spriteBatch, center, createCircle(radius, sides), color, thickness);
        }

        // Draw an arrow between two points with a specified color and thickness.
        public static void drawArrow(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)
        {
            // Calculate the direction from point1 to point2.
            Vector2 direction = point2 - point1;
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            float arrowHeadSize = 10;

            // Draw the main body of the arrow as a line.
            drawLine(spriteBatch, point1, point2, color, thickness);

            // Calculate the positions of the arrowheads.
            Vector2 arrowHead1 = point2 - new Vector2(
                (float)Math.Cos(angle + MathHelper.ToRadians(30)) * arrowHeadSize,
                (float)Math.Sin(angle + MathHelper.ToRadians(30)) * arrowHeadSize
            );
            Vector2 arrowHead2 = point2 - new Vector2(
                (float)Math.Cos(angle - MathHelper.ToRadians(30)) * arrowHeadSize,
                (float)Math.Sin(angle - MathHelper.ToRadians(30)) * arrowHeadSize
            );

            // Draw the two arrowhead lines.
            drawLine(spriteBatch, point2, arrowHead1, color, thickness);
            drawLine(spriteBatch, point2, arrowHead2, color, thickness);
        }
    }
}

