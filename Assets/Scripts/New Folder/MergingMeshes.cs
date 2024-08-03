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
using static UnityEngine.GridBrushBase;

class MergingMeshes : MonoBehaviour
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
    [MenuItem("Tools/ProBuilder/Count selected faces")]

    static void CountSelectedFaces()
    {
        List<ProBuilderMesh> meshes = MeshSelection.deep.ToList();
        int crt = 0;
        foreach (ProBuilderMesh mesh in meshes)
        {
            crt+=mesh.selectedFaceCount;
        }
        print(crt);
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


        //adding all meshes except the first one to "blocks" so we can later retreive them if needed.
        //first mesh is wont be deleted so no need to add it.
        meshes.RemoveAt(0);
        foreach (ProBuilderMesh mesh in meshes)
        {
            blocks.Add(mesh.gameObject);
        } 
        meshes.Add(Map);
       
        List<ProBuilderMesh> res= CombineMeshes.Combine(meshes, Map);


        SharedVertex[] sharedVertex = SharedVertex.GetSharedVerticesWithPositions(Map.VerticesInWorldSpace());

        List<SimpleTuple<Face, Face>> touchingFacesIndexes;
        List<Face> touchingFaces = new List<Face>();

        foreach (SharedVertex list in sharedVertex) //merges all shared vertex
        {
            Map.MergeVertices(list.ToArray(), true);
        }

        List<SimpleTuple<Face, Face>> faceInsideFace = null;
        touchingFacesIndexes = FindTouchingFaces(Map,out faceInsideFace);

        foreach (SimpleTuple<Face, Face> i in touchingFacesIndexes)
        {
            if (i.item1 != null)
                touchingFaces.Add(i.item1);
            if (i.item2 != null)
                touchingFaces.Add(i.item2);
        }
        Map.SetSelectedFaces(touchingFaces);

        touchingFaces = MergeElements.MergePairs(Map, touchingFacesIndexes, true);


        foreach(SimpleTuple<Face,Face> simpleTuple in faceInsideFace)
        {
            print("hru");
            Map.AppendVerticesToFace(simpleTuple.item1, Map.GetVertices(simpleTuple.item2.distinctIndexes.ToList()).Select(x => x.position).ToArray(), false);
            
            touchingFaces.Add(simpleTuple.item2);
            
        }        
        MergeElements.MergePairs(Map, faceInsideFace, true);



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
    private static List<SimpleTuple<Face, Face>> FindTouchingFaces(ProBuilderMesh mesh, out List<SimpleTuple<Face, Face>> FaceInsideFace)
    {
        FaceInsideFace = new List<SimpleTuple<Face, Face>>();
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
                    if (crt == vertex.Count)
                    {
                        res.Add(new SimpleTuple<Face, Face>(mesh.faces[i], mesh.faces[j]));
                    }
                    else
                    {
                        SimpleTuple<Face, Face> faceInsideFace = WhichPolyInsideTheOther(mesh, new SimpleTuple<Face, Face>(mesh.faces[i], mesh.faces[j]));
                        if (faceInsideFace.item1 != null)
                        {
                            FaceInsideFace.Add(faceInsideFace);
                            print("bro theres some1");
                        }
                    }

                    crt = 0;
                }

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


    public static SimpleTuple<Face,Face> WhichPolyInsideTheOther(ProBuilderMesh mesh, SimpleTuple<Face,Face> simpleTuple)
    {
        print("in new func");

        SimpleTuple<Face, Face> res=new SimpleTuple<Face, Face>();
        res.item1 = null;

        Face face1 = simpleTuple.item1;
        Face face2 = simpleTuple.item2;
        //the theory is: if a point is inside a polygon, then any ray coming out of it has an odd number of intersections with the polyon.
        //so i will be doing this to all vertices of the small polygon that i think will be inside to know if it is really inside the big polygon.

        //equation of line goes like this:
        //P = P0 + t * V
        //for a polygon edge, t is supposed to be clamped between 0 - 1, if higher or lower, the line doesnt intersect with the polygon edge.
        //for the ray coming from the point, t (or namely 's') has to be more or equal to 0.
        //we do two functions, one for the ray and one for the edge, and make them equal to get intersection points.
        //now we divide the one equation into 3 of all 3 dimintions xyz.
        //i do some simplification on paper and heres the result:
        //  t = (Vrx * ΔPrey - Vry * ΔPerx) / (Vey * Vrx - Vex * Vry).
        //and we can get s by:
        //  s = (ΔPerx + t * Vex) / Vrx.

        bool cool = IsFace2InFace1(mesh, face1, face2);

        
        if(cool)
        {
            res.item1= face1;
            res.item2= face2;
        }
        else
        {
            cool = IsFace2InFace1(mesh, face2, face1);
            if(cool)
            {
                res.item1 = face2;
                res.item2 = face1;
            }
        }

        return res;
    }

    public static bool IsFace2InFace1(ProBuilderMesh mesh, Face face1, Face face2)
    {
        foreach (int i in face2.distinctIndexes)//face2 is the baby face. face1 is big face ooga booga
        {
            int crt = 0;
            Vector3 p = mesh.VerticesInWorldSpace()[i];
            Vector3 Vr = p - (mesh.VerticesInWorldSpace()[face1.edges[0].a]+ mesh.VerticesInWorldSpace()[face1.edges[0].b])/2; //direction to mid of 1st edge

            foreach (Edge edge in face1.edges)
            {
                Vector3 e1 = mesh.VerticesInWorldSpace()[edge.a], e2 = mesh.VerticesInWorldSpace()[edge.b];// vertices of an edge in the polygon
                Vector3 Ve = e2 - e1;//direction



                // if one edge is the same as the point, we get two intersections,
                // which makes the program think that the small polygon isnt actually inside, but it is just touching the edge.
                if (p.Equals(e1) || p.Equals(e2))
                {
                    break;
                }

                float t, s; //intersection parms

                if (Vr.x == 0)
                {
                    float ΔPerz = (e1.z - p.z);
                    t = (Vr.z * (p.y - e1.y) + Vr.y * ΔPerz) / (Ve.y * Vr.z - Ve.z * Vr.y);
                    if (t >= 0 && t < 1)
                    {
                        s = (ΔPerz + t * Ve.z) / Vr.z;
                        print("t: " + t + ", s: " + s);
                        if (s >= 0)//I like
                        {
                            crt++;
                            print("shit we got some ");
                        }
                    }
                }
                else if (Vr.y == 0)
                {
                    float ΔPerx = (e1.x - p.x);
                    t = (Vr.x * (p.z - e1.z) + Vr.z * ΔPerx) / (Ve.z * Vr.x - Ve.x * Vr.z);
                    if (t >= 0 && t < 1)
                    {
                        s = (ΔPerx + t * Ve.x) / Vr.x;
                        print("t: " + t + ", s: " + s);
                        if (s >= 0)//I like
                        {
                            crt++;
                            print("shit we got some ");
                        }
                    }

                }
                else
                {
                    float ΔPerx = (e1.x - p.x);
                    t = (Vr.x * (p.y - e1.y) + Vr.y * ΔPerx) / (Ve.y * Vr.x - Ve.x * Vr.y);
                    if (t >= 0 && t < 1)
                    {
                        s = (ΔPerx + t * Ve.x) / Vr.x;
                        print("t: " + t + ", s: " + s);
                        if (s >= 0)//I like
                        {
                            crt++;
                            print("shit we got some ");
                        }
                    }
                }

            }
            print(crt);
            if (crt % 2 == 0)
                return false;
        }
        return true;
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

