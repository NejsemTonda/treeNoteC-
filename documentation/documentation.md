# TreeNote - A Tree Structure Note-taking Application

This document provides an overview and documentation for the TreeNote application code.
This application is written in C# + .NET Core programming language, and it uses the MonoGame framework. Even though MonoGame is a cross-platform module, this application uses some modules that are Unix-only (like Pandoc and Poppler).

## Table of Contents

- [Motivation](#motivation)
- [Usage](#usage)
    - [Dependencies](#dependencies)
    - [How to Run](#how-to-run)
    - [Controls](#controls)
- [Development Documentation](#development-documentation)
    - [Node](#node)
    - [MouseHandler](#mousehandler)
    - [FileHandler](#filehandler)
    - [Config](#config)
    - [Initializer](#initializer)
    - [Converter](#converter)
    - [PrimitivesDrawing](#primitivesdrawing)

## Motivation

I wanted to create an app where I would be comfortable making notes. I didn't like the fact that I don't have any perspective in my notes, and when I write them in one file, I often get lost in them. I really liked the tree-like structure of the notes, like in Obsidian.md. I wanted to use Vim as a text editor because it suits me the most of all text editors. But I didn't find any application that suited me perfectly, so I decided to create my own.

## Usage

### Dependencies

- Pandoc >= 3.1.2 
- Poppler >= 23.08.0
- .NET Core >= 7.0.110
- (dotnet) MonoGame >= 3.8.1

### How to Run

1. Copy all files from the project to the folder in which you wish to take notes.
2. Run this command: `dotnet run --init`
3. Run this command: `dotnet run`
4. Open `edit.md` (or any other file extension) in your favorite text editor.

### Controls

- Click on the node you wish to edit. The currently loaded node should be highlighted with a different color.
- Hover the mouse over the node you wish to see its thumbnail.
- Click and drag on the background to shift all nodes.
- Click and drag on a node to move the node and its children.
- Ctrl+click on the background to create a new node.
- Ctrl+click on a node to create new children of this node.

## Development Documentation 

### Node Class 

The `Node` class represents a node in a tree structure.

#### Properties

- `pos` (Vector2): The position of the node.
- `desPos` (Vector2): The desired position to allow smooth movement.
- `radius` (double): The radius of the node.
- `visited` (bool): Indicates if the node has been visited during depth-first search.
- `selected` (bool): Indicates if the node is selected.
- `name` (string): The name of the node.
- `childs` (List<Node>): A list of child nodes.
- `needReload` (bool): Indicates if the node needs to be reloaded.
- `drawThumbnail` (bool): Determines if a thumbnail should be drawn.
- `thumbnail` (Texture2D?): The texture for the thumbnail (nullable).

#### Constructor

- `public Node(Vector2 Pos, double r, string Name)`: Initializes a new instance of the `Node` class.
    - `Pos` (Vector2): The position of the node.
    - `r` (double): The radius of the node.
    - `Name` (string): The name of the node.

#### Methods

- `public void draw(SpriteBatch sb, Vector2 mid, SpriteFont font)`: Draws the node.
    - `sb` (SpriteBatch): The `SpriteBatch` used for drawing.
    - `mid` (Vector2): The midpoint used for positioning.
    - `font` (SpriteFont): The font used for drawing text.

- `public void move(Vector2 to)`: Moves the node to a new position.
    - `to` (Vector2): The new position.

- `public void update()`: Updates the node's position.

- `public void applyToChilds(Action<Node> foo)`: Applies a function to all child nodes.
    - `foo` (Action<Node>): The function to apply.

- `public void applyToChildsAndParent(Action<Node> foo)`: Applies a function to both child nodes and the parent node.
     `foo` (Action<Node>): The function to apply.

- `public void unvisit()`: Unvisits the node and its child nodes.

- `public Node createChild(Vector2 tpos)`: Creates a child node.
     `tpos` (Vector2): The position of the child node.

- `public void reloadThumbnail(GraphicsDevice graphics)`: Reloads the thumbnail of the node.
     `graphics` (GraphicsDevice): The graphics device used for loading the thumbnail.

### MouseHandler Class 

A class for handling mouse input in a graphics application.

#### Properties

- `clicked` (bool): A flag to track if the mouse button is clicked.
- `mouseNode` (Node?): The node currently under the mouse cursor.
- `grapNode` (Node?): The node being dragged (if any).
- `selectedNode` (Node?): The currently selected node.
- `grapStart` (Vector2): The starting position of a drag operation.
- `grapOffset` (Vector2): The offset for dragging a node.
- `offset` (Vector2): The current offset applied to the nodes.
- `lastMouse` (Vector2): The last recorded mouse position.
- `fh` (FileHandler): A file handler for managing node data.

#### Constructor

- `public MouseHandler()`: Initializes a new instance of the `MouseHandler` class.
    - Initializes the `clicked` flag to `false`.
    - Initializes the `offset` vector to `(0, 0)`.
    - Initializes the `grapStart` vector to `(0, 0)`.
    - Initializes the `grapOffset` vector to `(0, 0)`.
    - Initializes the `lastMouse` vector to `(0, 0)`.
    - Creates a new instance of the `FileHandler` class and assigns it to the `fh` property.

#### Methods

- `public void update(bool m1, bool ctrl, Vector2 mousePos, Node masterNode)`: Updates the mouse input and interaction with nodes.
    - `m1` (bool): A boolean indicating whether the mouse button is pressed.
    - `ctrl` (bool): A boolean indicating whether the Ctrl key is pressed.
    - `mousePos` (Vector2): The current mouse position.
    - `masterNode` (Node): The root node of the tree structure.

- `private Node? onNode(Vector2 mousePos, Node node)`: A recursive function to find the node under the mouse cursor.
    - `mousePos` (Vector2): The current mouse position.
    - `node` (Node): The node to check.

- `private void switchGrapped(Node switchTo, Node masterNode)`: Switches the currently dragged node and updates its selected status.
    - `switchTo` (Node): The node to switch to.
    - `masterNode` (Node): The root node of the tree structure.

### FileHandler Class 

The `FileHandler` class is responsible for handling file operations and managing the currently opened file in the `TreeNote` application.

#### Properties

- `openedFile` (string): Stores the name of the currently opened file.
- `editFile` (string): Stores the name of the file used for editing.

#### Constructor

- `public FileHandler()`
    - Initializes the `openedFile` and `editFile` variables.
    - Attempts to copy the contents of a file located at `Config.notesDir + openedFile` to `editFile`.
    - Handles a `DirectoryNotFoundException` by calling `Initializer.printNoInit()` if the directory is not found.

#### Methods

- `void copyFile(string loc, string dest)`
    - `loc` (string): The source file location.
    - `dest` (string): The destination file location.
    - Copies the contents of one file to another using a `StringBuilder` to efficiently concatenate the source file's contents.

- `public void changedToFile(string file)`
    - `file` (string): The name of the file that has changed.
    - Handles changes made to a file, including creating a new file if it doesn't exist, copying contents from `editFile`, and regenerating a thumbnail.
    - Updates the `openedFile` variable.

- `public string getCurrentFileName()`
    - Retrieves the current file name from `editFile`.
    - Returns the current file name without the "#" character and leading/trailing whitespace.

- `public static void deleteFile(string fileName)`
    - `fileName` (string): The name of the file to be deleted.
    - Deletes the specified file if it exists, or prints a message if the file doesn't exist.

- `public static void checkForDelete(Node node)`
    - `node` (Node): The node to check for children with delete keywords.
    - Checks if the node has any children with names denoting deletion keywords ("Delete," "delete," "del") and removes them if found.

### Config Class 

Configuration class for the TreeNote application.

#### Properties

- `ext` (string) : Default file extension for notes.
- `notesDir` (string) : Default directory for storing notes.
- `cacheDir` (string) : Default directory for caching PDFs and thumbnails.
- `thumbnailSizeX` (int) : Default width for thumbnails.
- `thumbnailSizeY` (int) : Default height for thumbnails, calculated to maintain aspect ratio.
- `defaultNodeSize` (int) : Default size for nodes.
- `childScaler` (double) : Scaling factor for child nodes.
- `defaultLineWidth` (int) : Default line width for arrows.
- `cropThumbnail` (int) : Cropping size for thumbnails.
- `background` (Color) : Default background color.
- `nodes` (Color) : Color for nodes.
- `selected` (Color) : Color for selected nodes.
- `arrows` (Color) : Color for arrows.

#### Constructor

- `Config()`
    - Initializes a new instance of the `Config` class.

#### Methods

- `public static void loadConfig(string file = ".config")`
    - Loads configuration options from a file.
    - `file` (string) : The path to the configuration file. Defaults to ".config".
    
- `public static Color hexStringToColor(string hex)`
    - Converts a hexadecimal string to a Color object.
    - `hex` (string) : The hexadecimal color string.
    - Returns a Color object.

- `public static void dumpDefault(string confFile = ".config")`
    - Creates a default configuration file with explanations.
    - `confFile` (string) : The path to the configuration file. Defaults to ".config".

### Initializer Class 

Class responsible for initializing and managing the application.

#### Properties

- `noInitMessage` (string): Message to display when the application is not initialized properly.

#### Constructor

- `Initializer()`
  - Initializes an instance of the `Initializer` class.

#### Methods

- `public static Node createMasterNode()`
  - Creates the master node of the tree.
  - Returns: `Node` - The master node of the tree.

- `private static Node parseLine(int depth, StreamReader sr)`
  - Parse a line of text to create a node in the tree.
  - Parameters:
    - `depth` (int): The depth of the node in the tree.
    - `sr` (StreamReader): The stream reader used to read the input.
  - Returns: `Node` - The parsed node.

- `public static void onExit(Node masterNode)`
  - Saves the tree structure to the session file when exiting.
  - Parameters:
    - `masterNode` (Node): The master node of the tree.

- `private static void dumpNode(Node n, StreamWriter sw)`
  - Writes node information to the session file.
  - Parameters:
    - `n` (Node): The node to write to the session file.
    - `sw` (StreamWriter): The stream writer used to write the output.

- `public static void init()`
  - Initializes the application by creating necessary files and directories.

- `public static void printNoInit()`
  - Displays an error message when initialization is not successful.

### Converter Class

This class provides functionality for converting files to PDF format and creating thumbnails.

#### Properties

- None

#### Constructor

- `Converter()`
    - Initializes a new instance of the `Converter` class.

#### Methods

- `public static void convert(string fileName)`
    - `fileName` (string): The name of the file to be converted to PDF.
    - Converts the specified file to PDF format and creates a thumbnail.

### Primitives Class

A static class for drawing various primitives in XNA.

#### Properties

- `private static Texture2D pixel`: A 1x1 white pixel texture used for drawing lines and shapes.

#### Constructor

- `Primitives.createThePixel(SpriteBatch spriteBatch)`: Helper method to create the 1x1 white pixel texture.

#### Methods

- `drawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness)`
  - `point`: The starting point of the line.
  - `length`: The length of the line.
  - `angle`: The angle of the line in radians.
  - `color`: The color of the line.
  - `thickness`: The thickness of the line.
  - `void`: Draws a line from the starting point with the specified length, angle, color, and thickness.

- `drawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)`
  - `point1`: The first endpoint of the line.
  - `point2`: The second endpoint of the line.
  - `color`: The color of the line.
  - `thickness`: The thickness of the line.
  - `void`: Draws a line between two points with the specified color and thickness.
  
- `drawPoints(SpriteBatch spriteBatch, Vector2 position, List<Vector2> points, Color color, float thickness)`
  - `position`: The position at which to draw the points.
  - `points`: The list of points to draw.
  - `color`: The color of the points.
  - `thickness`: The thickness of the lines connecting the points.
  - `void`: Draws a series of connected points with the specified color and thickness.
  
- `createCircle(double radius, int sides)`
  - `radius`: The radius of the circle.
  - `sides`: The number of sides (segments) of the circle.
  - `List<Vector2>`: Creates and returns a list of vectors representing the points of the circle.

- `drawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness)`
  - `center`: The center point of the circle.
  - `radius`: The radius of the circle.
  - `sides`: The number of sides (segments) of the circle.
  - `color`: The color of the circle.
  - `thickness`: The thickness of the circle's outline.
  - `void`: Draws a circle with the specified center, radius, number of sides, color, and thickness.

- `drawArrow(SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness)`
  - `point1`: The starting point of the arrow.
  - `point2`: The ending point of the arrow.
  - `color`: The color of the arrow.
  - `thickness`: The thickness of the arrow lines.
  - `void`: Draws an arrow between two points with the specified color and thickness.

