using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

struct Vertex
{
    public float x;
    public float y;
    public float z;
    public float u;
    public float v;
    public float r;
    public float g;
    public float b;
}

class Mapping
{
    public string name;
    public int index;

    public Mapping(string name)
    {
        this.name = name;
    }
}

namespace RenderDocCSV2OBJ
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = args[0].Substring(0, args[0].LastIndexOf('\\'));
            string file = args[0].Substring(args[0].LastIndexOf('\\') + 1, args[0].Length - args[0].LastIndexOf('\\') - 1);
            string filename = file.Substring(0, file.IndexOf('.'));
            TextFieldParser parser = new TextFieldParser(args[0]);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            
            List<Vertex> vertexList = new List<Vertex>();
            List<Mapping> mappings = null;
            
            Vertex lastVertex = new Vertex();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();

                if (mappings == null)
                {
                    mappings = new List<Mapping>(fields.Length);

                    for (int i = 0; i < fields.Length; i++)
                    {
                        mappings.Add(new Mapping(fields[i]));
                    }
                }

                if (string.Compare(fields[2], "Restart") != 0)
                {
                    Vertex v = new Vertex();
                    int x_index = GetIndex(mappings, "gl_Position.x");
                    if (x_index == -1)
                    { 
                        x_index = GetIndex(mappings, "SV_Position.x");
                    }

                    float.TryParse(fields[x_index], out v.x);

                    int y_index = GetIndex(mappings, "gl_Position.y");
                    if (y_index == -1)
                    {
                        y_index = GetIndex(mappings, "SV_Position.y");
                    }

                    float.TryParse(fields[y_index], out v.y);

                    int z_index = GetIndex(mappings, "gl_Position.z");
                    if (z_index == -1)
                    {
                        z_index = GetIndex(mappings, "SV_Position.z");
                    }

                    float.TryParse(fields[z_index], out v.z);

                    //float.TryParse(fields[GetIndex(mappings, "gl_Position.y")], out v.y);
                    //float.TryParse(fields[GetIndex(mappings, "gl_Position.z")], out v.z);

                    int u_index = GetIndex(mappings, "texcoord0.x");
                    if (u_index == -1)
                    {
                        u_index = GetIndex(mappings, "TEXCOORD.x");
                    }

                    float.TryParse(fields[u_index], out v.u);

                    //float.TryParse(fields[GetIndex(mappings, "texcoord0.x")], out v.u);

                    int v_index = GetIndex(mappings, "texcoord0.y");
                    if (v_index == -1)
                    {
                        v_index = GetIndex(mappings, "TEXCOORD.y");
                    }

                    float.TryParse(fields[v_index], out v.v);

                    //float.TryParse(fields[GetIndex(mappings, "texcoord0.y")], out v.v);

                    lastVertex.x = v.x;
                    lastVertex.y = v.y;
                    lastVertex.z = v.z;
                    //lastVertex.x = ComputeXPrime(v.x, width, zNear) / ComputeWPrime(v.z);
                    //lastVertex.y = ComputeYPrime(v.y, height, zNear) / ComputeWPrime(v.z);
                    //lastVertex.z = ComputeZPrime(v.z, width, zNear, zFar) / ComputeWPrime(v.z);

                    lastVertex.u = v.u;
                    lastVertex.v = v.v;
                    lastVertex.r = v.r;
                    lastVertex.g = v.g;
                    lastVertex.b = v.b;

                    vertexList.Add(lastVertex);
                }
            }

            string[] vertexStrings = new string[vertexList.Count-1];
            string[] vertexUVCoords = new string[vertexList.Count-1];
            string[] triangleStrings = new string[(vertexList.Count-1) / 3];

            for(int i = 0; i < vertexStrings.Length; i++)
            {
                vertexStrings[i] = "v " + vertexList[i+1].x + " " + vertexList[i+1].y + " " + vertexList[i+1].z
                
#if VERTEX_COLOR
                + " " + vertexList[i+1].r + " " + vertexList[i+1].g + " " + vertexList[i+1].b;
#else
                ;
#endif
            }

            for (int i = 0; i < vertexUVCoords.Length; i++)
            {
                vertexUVCoords[i] = "vt " + vertexList[i+1].u + " " + vertexList[i+1].v;
            }

            for (int i = 0; i < triangleStrings.Length; i++)
            {
                int i1 = (i * 3 + 1);
                int i2 = (i * 3 + 2);
                int i3 = (i * 3 + 3);
                triangleStrings[i] = "f " + i1 + "/" + i1 + " " + i2 + "/" + i2 + " " + i3 + "/" + i3;
            }

            string allStringsAttached = "";

            for (int i = 0; i < vertexStrings.Length; i++)
            {
                allStringsAttached += vertexStrings[i] + '\n';
            }

            allStringsAttached += '\n';

            for (int i = 0; i < vertexUVCoords.Length; i++)
            {
                allStringsAttached += vertexUVCoords[i] + '\n';
            }

            allStringsAttached += '\n';

            for (int i = 0; i < triangleStrings.Length; i++)
            {
                allStringsAttached += triangleStrings[i] + '\n';
            }

            string[] path = args[0].Split(new char[] {'\\'});
            System.IO.File.WriteAllText(directory + "\\" + filename + ".obj", allStringsAttached);
        }

        static int GetIndex(List<Mapping> mappings, string fieldName)
        {
            for (int i = 0; i < mappings.Count; i++)
            {
                if (fieldName == mappings[i].name)
                    return i;
            }

            return -1;
        }

        static float ComputeXPrime(float x, float width, float near)
        {
            return (2.0f * near / width) * x;
        }

        static float ComputeYPrime(float y, float height, float near)
        {
            return (2.0f * near / height) * y;
        }

        static float ComputeZPrime(float z, float width, float near, float far)
        {
            return ((far / (far - near)) * z) + ((near * far / (near - far)) * width);
        }

        static float ComputeWPrime(float z)
        {
            return z;
        }
    }
}
