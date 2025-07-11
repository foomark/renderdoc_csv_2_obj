This is a program to convert "a limited number" of CSV (Colon Seperated Values) into OBJ from RenderDoc.

RenderDoc does not have a way of viewing models with textures. So this is the only way of doing so.

There are some limitations.

- Supports OpenGL only.
- It works mostly with triangle lists and a limited number of triangle strips.
- The program supports gl_Position or SV_Position, and texcoord0 or TEXCOORD.

** Note, you can change gl_Position, SV_Position, etc.. if its named differently in the shader program.

How to use

1. Open an OpenGL capture in RenderDoc.
2. Click on the "Mesh Viewer" tab.
3. Click the save icon ("disk icon") and "Export to CSV".
4. Once you have your CSV file, in the Command Prompt, use "RenderDocCSV2OBJ [geometry_exported_file.csv]"

Depending on how big the file, it can take sometime to do the conversion.
