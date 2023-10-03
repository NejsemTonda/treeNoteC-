using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

// This namespace declaration is incorrect; remove the semicolon.
namespace treeNote
{
    // Class for aligning UI elements.
    class UIAligner
    {
        List<Node> nodePile; // List to store nodes.
        List<Vector2> rememberedPositions; // List to remember node positions for restoration.
        bool nodesOutOfplace; // Flag to track if nodes are out of place.

        // Constructor for UIAligner class.
        public UIAligner()
        {
            nodePile = new List<Node>();
            rememberedPositions = new List<Vector2>();
            nodesOutOfplace = false;
        }

        // Add a node to the nodePile.
        public void dump(Node n)
        {
            if (!nodePile.Contains(n)) nodePile.Add(n);
        }

        // Align UI elements.
        public void align()
        {
            Node? mouseNode = null; // Node under the mouse cursor (if any).

            // Loop through each node in the nodePile.
            foreach (Node n1 in nodePile)
            {
                if (n1.drawThumbnail) mouseNode = n1;

                // Compare each pair of nodes for collision.
                foreach (Node n2 in nodePile)
                {
                    if (n1 == n2) continue;

                    Vector2 diff = n2.pos - n1.pos;

                    // Resolve collision if nodes overlap.
                    if (diff.Length() < n1.radius + n2.radius)
                    {
                        double overLapSize = n1.radius + n2.radius - diff.Length();
                        n1.move(n1.pos - diff * (float)overLapSize);
                        n2.move(n1.pos + diff * (float)overLapSize);
                    }
                }
            }

            // Restore node positions if the mouse is not over any node and nodes are out of place.
            if (mouseNode == null && nodesOutOfplace)
            {
                int index = 0;
                foreach (Node n in nodePile)
                {
                    n.move(rememberedPositions[index]);
                    ++index;
                }
                nodesOutOfplace = false;
            }

            // Check if the mouse is over a node and handle overlapping nodes.
            if (mouseNode != null && !nodesOutOfplace)
            {
                nodesOutOfplace = true;
                rememberedPositions = new List<Vector2>(new Vector2[nodePile.Count + 1]);
                int index = 0;
                Rectangle thumbnailRect = mouseNode.getThumbnailRect();

                // Loop through nodes to check for overlap and resolve it.
                foreach (Node n in nodePile)
                {
                    rememberedPositions[index] = n.desPos;
                    ++index;

                    // Check if the current node overlaps with the mouse node's thumbnail.
                    if (isOverlapping(thumbnailRect, n))
                    {
                        Console.WriteLine("Need to be Moved");
                        n.move(n.pos + resolveOverlap(n, thumbnailRect));
                    }
                }
            }
        }

        // Check if a spherical node overlaps with a rectangular area.
        bool isOverlapping(Rectangle rect, Node n)
        {
            // Calculate the closest point on the rectangle to the sphere's center.
            float closestX = Math.Max(rect.Left, Math.Min(n.pos.X, rect.Right));
            float closestY = Math.Max(rect.Top, Math.Min(n.pos.Y, rect.Bottom));

            // Calculate the squared distance between the closest point and the sphere's center.
            float distanceSquared = (n.pos.X - closestX) * (n.pos.X - closestX)
                                    + (n.pos.Y - closestY) * (n.pos.Y - closestY);

            // Compare squared distance with the squared radius of the sphere to check for overlap.
            return distanceSquared <= n.radius * n.radius;
        }

        // Resolve overlap between a node and a rectangular area.
        Vector2 resolveOverlap(Node n, Rectangle rect)
        {
            float distToLeft = rect.Left - n.pos.X - 1.5f * (float)n.radius;
            float distToRight = rect.Right - n.pos.X + 1.5f * (float)n.radius;
            float distToTop = rect.Top - n.pos.Y - 1.5f * (float)n.radius;
            float distToBottom = rect.Bottom - n.pos.Y + 1.5f * (float)n.radius;

            // Determine the direction to move the node to resolve overlap.
            if (Math.Abs(distToLeft) <= Math.Min(distToRight, Math.Min(Math.Abs(distToTop), distToBottom)))
            {
				//Left is closest, node should move to the left
                return new Vector2(distToLeft, 0);
            }
            if (distToRight <= Math.Min(Math.Abs(distToLeft), Math.Min(Math.Abs(distToTop), distToBottom)))
            {
				//Right is closest, node should move to the right
                return new Vector2(distToRight, 0);
            }
            if (Math.Abs(distToTop) <= Math.Min(distToRight, Math.Min(Math.Abs(distToLeft), distToBottom)))
            {
				//Top is closest, node should move to the top
                return new Vector2(0, distToTop);
            }
            if (distToBottom <= Math.Min(distToRight, Math.Min(Math.Abs(distToTop), Math.Abs(distToLeft))))
            {
				//Bottom is closest, node should move to the bottom
                return new Vector2(0, distToBottom);
            }

            // This should never happen.
            return new Vector2(0, 0);
        }
    }
}

