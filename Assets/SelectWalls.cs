using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

public class SelectWalls : MonoBehaviour
{
    [MenuItem("Tools/ProBuilder/Select Walls Faces")]
    static void Select()
    {
        List<ProBuilderMesh> meshes = MeshSelection.deep.ToList();

        foreach (ProBuilderMesh mesh in meshes)
        {
            List<Face> faces = new List<Face>();
            foreach (Face face in mesh.faces)
            {
                if(GetFaceNormal(mesh, face).y==0)
                    faces.Add(face);
            }
            mesh.SetSelectedFaces(faces);
        }
    }


    public static Vector3 GetFaceNormal(ProBuilderMesh mesh, Face face)
    {
        Vector3 normal;
        Vertex[] vertices;

        vertices = mesh.GetVertices(face.distinctIndexes);

        Vector3 edgeLine1, edgeLine2;
        edgeLine1 = vertices[0].position - vertices[1].position;
        edgeLine2 = vertices[1].position - vertices[2].position;

        normal = Vector3.Cross(edgeLine1, edgeLine2);

        return normal;

    }

}
