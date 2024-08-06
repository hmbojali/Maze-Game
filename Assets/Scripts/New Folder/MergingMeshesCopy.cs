using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using UnityEditor;
using UnityEditor.ProBuilder;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using Unity.Mathematics;
using static Unity.Collections.AllocatorManager;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts;
using UnityEditor.SceneManagement;
using System.Runtime.InteropServices.WindowsRuntime;

class MergingMeshesCopy : MonoBehaviour
{

    [MenuItem("Tools/ProBuilder/Merge Meshes (Keep Objects)")]
    static void KeepObjects()
    {
        MergingCouples(false);
    }
    [MenuItem("Tools/ProBuilder/Merge Meshes (Remove Objects)")]
    static void RemoveObjects()
    {
        MergingCouples();
    }


    private static void MergingCouples(bool removeObjects = true)
    {

        List<ProBuilderMesh> meshes = MeshSelection.deep.ToList();
        List<ProBuilderMesh> meshes1 = new List<ProBuilderMesh>();
        foreach (ProBuilderMesh mesh in meshes)
        {
            meshes1.Add(Instantiate(mesh));
        }
        List<ProBuilderMesh> meshes2 = new List<ProBuilderMesh>();



        while (meshes1.Count > 1 || meshes2.Count > 1)
        {
            while (meshes1.Count > 1)
            {
                meshes2.Add(Merging(new List<ProBuilderMesh> { meshes1[0], meshes1[1] }));
                meshes1.RemoveAt(0);
                meshes1.RemoveAt(0);
            }
            while (meshes2.Count > 1)
            {
                meshes1.Add(Merging(new List<ProBuilderMesh> { meshes2[0], meshes2[1] }));
                meshes2.RemoveAt(0);
                meshes2.RemoveAt(0);
            }
        }

        ProBuilderMesh res;
        try
        {
            res = Merging(new List<ProBuilderMesh> { meshes1[0], meshes2[0] });
        }
        catch
        {
            if (meshes1.Count == 0)
                res = meshes2[0];
            else
                res = meshes1[0];
        }





        res.name = "MergedObjects";
        res.ToMesh();
        res.Refresh();

        if (removeObjects)
        {
            foreach (ProBuilderMesh mesh in meshes)
            {
                DestroyImmediate(mesh.gameObject);
            }
        }
        else
        {
            foreach (ProBuilderMesh mesh in meshes)
            {
                mesh.ToMesh();
                mesh.Refresh();
            }
        }

        string sceneName = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.SaveOpenScenes();
        EditorSceneManager.OpenScene(sceneName);
        EditorSceneManager.OpenScene(sceneName);
        EditorSceneManager.OpenScene(sceneName);
    }
    private static ProBuilderMesh Merging(List<ProBuilderMesh> meshes, bool removeObjects = true)
    {
        ProBuilderMesh Map = meshes[0];

        List<GameObject> blocks = new List<GameObject>();



        meshes.RemoveAt(0);
        foreach (ProBuilderMesh mesh in meshes)
        {
            blocks.Add(mesh.gameObject);
        }

        meshes.Add(Map);

        List<ProBuilderMesh> res = CombineMeshes.Combine(meshes, Map);






        SharedVertex[] sharedVertex = SharedVertex.GetSharedVerticesWithPositions(Map.VerticesInWorldSpace());

        List<SimpleTuple<Face, Face>> touchingFacesIndexes = new List<SimpleTuple<Face, Face>>();
        List<Face> touchingFaces = new List<Face>();

        foreach (SharedVertex list in sharedVertex)
        {
            Map.MergeVertices(list.ToArray(), true);
        }


        touchingFacesIndexes = FindTouchingFaces(Map);

        foreach (SimpleTuple<Face, Face> i in touchingFacesIndexes)
        {
            if (i.item1 != null)
                touchingFaces.Add(i.item1);
            if (i.item2 != null)
                touchingFaces.Add(i.item2);
        }
        Map.SetSelectedFaces(touchingFaces);

        touchingFaces = MergeElements.MergePairs(Map, touchingFacesIndexes, true);

        Map.ToMesh();
        Map.Refresh();

        try
        {
            Map.DeleteFaces(touchingFaces);
        }
        catch (Exception ex)
        {
            print("Exception:       " + ex.Message);
        }


        blocks[0].GetComponent<ProBuilderMesh>().ToMesh();
        blocks[0].GetComponent<ProBuilderMesh>().Refresh();

        if (removeObjects)
        {
            foreach (GameObject block in blocks)
            {
                DestroyImmediate(block);
            }
        }
        else
        {
            foreach (GameObject block in blocks)
            {
                block.GetComponent<ProBuilderMesh>().ToMesh();
                block.GetComponent<ProBuilderMesh>().Refresh();
            }
        }
        Map.ToMesh();
        Map.Refresh();

        return Map;
    }
    private static List<SimpleTuple<Face, Face>> FindTouchingFaces(ProBuilderMesh mesh)
    {
        List<SimpleTuple<Face, Face>> res = new List<SimpleTuple<Face, Face>>();
        List<Vector3> facesNormals = new List<Vector3>();

        int crt = 0;
        for (int i = 0; i < mesh.faces.Count; i++)
        {
            facesNormals.Add(GetFaceNormal(mesh, mesh.faces[i]));
        }

        for (int i = 0; i < mesh.faces.Count; i++)
        {
            List<Vector3> vertex = new List<Vector3>();
            foreach (int k in mesh.faces[i].distinctIndexes)//getting the position of each vertice
            {
                vertex.Add(mesh.GetVertices(new List<int>() { k })[0].position);
            }
            for (int j = i + 1; j < mesh.faces.Count; j++)
            {
                if (Vector3.Angle(facesNormals[i], facesNormals[j]) % 180 < 3)
                {
                    foreach (int k in mesh.faces[j].distinctIndexes)
                    {
                        foreach (Vector3 pos in vertex)
                        {
                            if (pos == mesh.GetVertices(new List<int>() { k })[0].position)
                                crt++;
                        }
                    }
                }
                if (crt == vertex.Count)
                {
                    res.Add(new SimpleTuple<Face, Face>(mesh.faces[i], mesh.faces[j]));
                }
                crt = 0;

            }
        }
        return res;
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


    private static List<SimpleTuple<Face, Face>> FindTouchingFaces2(ProBuilderMesh mesh)
    {
        List<SimpleTuple<Face, Face>> res = new List<SimpleTuple<Face, Face>>();

        //fetch all normals of all faces and save them in a vector3 List


        //start the loops like above, but compare normals as a first check. Angle between 2 normals > 175 --> faces are parallel 

        //calculate distance(d) of a plane(face) from origin by doing dot product of a vertice pos and the normal
        //subtract [d * normal.nomalized] from all vertice positions and save them in a Vector3 List which make a new plane with distance = 0

        //now pick a normal to the xz plane( [0,1,0] ) and do the dot product of that normal with any of the vertices positions in the last result
        //this will calculate the new d




        //continue 





        return res;
    }

}

