using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace treeNote
{
    // A class for handling mouse input in a graphics application.
    class MouseHandler
    {
        bool clicked;        // Flag to track if the mouse button is clicked.
        Node? mouseNode;     // The node currently under the mouse cursor.
        Node? grapNode;      // The node being dragged (if any).
        Node? selectedNode;  // The currently selected node.
        Vector2 grapStart;   // The starting position of a drag operation.
        Vector2 grapOffset;  // The offset for dragging a node.
        public Vector2 offset; // The current offset applied to the nodes.
        Vector2 lastMouse;   // The last recorded mouse position.
        FileHandler fh;      // A file handler for managing node data.

        // Constructor for the MouseHandler class.
        public MouseHandler()
        {
            clicked = false;
            offset = new Vector2(0, 0);
            grapStart = new Vector2(0, 0);
            grapOffset = new Vector2(0, 0);
            lastMouse = new Vector2(0, 0);
            fh = new FileHandler();
        }

        // Update method to handle mouse input and interaction with nodes.
        public void update(bool m1, bool ctrl, Vector2 mousePos, Node masterNode)
        {
            // Find the node under the mouse cursor.
            mouseNode = onNode(mousePos - offset, masterNode);

            // Reset the visit status of nodes in the tree.
            masterNode.unvisit();

            if (m1)
            {
                if (!clicked)
                {
                    clicked = true;
                    grapStart = mousePos;

                    // Disable thumbnail drawing for all nodes and unvisit them.
                    masterNode.applyToChilds(x => x.drawThumbnail = false);
                    masterNode.unvisit();

                    if (ctrl)
                    {
                        // Create a new child node at the mouse position.
                        Node to = mouseNode == null ? masterNode : mouseNode;
                        Node newNode = to.createChild(mousePos - offset);

                        // Switch to the newly created node as the dragged node.
                        switchGrapped(newNode, masterNode);
                        grapOffset *= 0; // Reset the drag offset.
                    }
                    else
                    {
                        if (mouseNode != null)
                        {
                            // Switch to the clicked node as the dragged node and calculate the offset.
                            switchGrapped(mouseNode, masterNode);
                            grapOffset = mouseNode.pos - mousePos + offset;
                        }
                    }
                }
                else
                {
                    if (grapNode == null)
                        offset += mousePos - lastMouse;
                    else
                        grapNode.applyToChildsAndParent(x => x.move((x.pos - grapNode.pos) + mousePos - offset + grapOffset));
                }
            }
            else
            {
                grapNode = null;
                clicked = false;

                if (mouseNode != null)
                {
                    // If the mouse node is not null, set it to draw its thumbnail.
                    if (!mouseNode.drawThumbnail)
                        mouseNode.needReload = true;
                    mouseNode.drawThumbnail = true;
                }
                else
                {
                    // Disable thumbnail drawing for all nodes and unvisit them.
                    masterNode.applyToChilds(x => x.drawThumbnail = false);
                    masterNode.unvisit();
                }
            }

            lastMouse = mousePos;
        }

        // Recursive function to find the node under the mouse cursor.
        Node? onNode(Vector2 mousePos, Node node)
        {
            if ((mousePos - node.pos).Length() < node.radius)
            {
                return node;
            }

            Node? onChildren;
            foreach (var n in node.childs)
            {
                onChildren = onNode(mousePos, n);
                if (onChildren != null)
                    return onChildren;
            }
            return null;
        }

        // Switch the currently dragged node and update its selected status.
        void switchGrapped(Node switchTo, Node masterNode)
        {
            grapNode = switchTo;
            Node? selectedNode = null;

            // Reset the selected status for all nodes in the tree.
            masterNode.applyToChilds(x => { if (x.selected) { selectedNode = x; x.selected = false; } });
            masterNode.unvisit();

            // Set the selected status for the newly selected node.
            switchTo.selected = true;

            // Get the new name for the node from the FileHandler.
            string newName = fh.getCurrentFileName();

            if (selectedNode != null && newName != selectedNode.name && newName != "none")
                selectedNode.name = newName;

            // Notify the FileHandler of the node switch.
            fh.changedToFile(switchTo.name);

            // Update the selected node reference.
            selectedNode = switchTo;
        }
    }
}

