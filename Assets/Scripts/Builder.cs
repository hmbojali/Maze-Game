using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.ProBuilder;
using UnityEngine.ProGrids;
using System;
using System.ComponentModel;
using System.Linq;
using UnityEditor.ProBuilder;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject buildingBlock;
    [SerializeField] private float movingSpeed = 3f;
    [SerializeField] private ProBuilderMesh mesh;
    private List<GameObject> blocks = new List<GameObject>();
    private bool destroy = false;

    // Start is called before the first frame update
    void Start()
    {
        float x = Snapping.Snap(transform.position.x, 10);
        float y = Snapping.Snap(transform.position.y, 10);
        float z = Snapping.Snap(transform.position.z, 10);
        transform.position = new Vector3(x + 4.9f, y, z);

        GameObject firstBlock = Instantiate(buildingBlock);
        firstBlock.transform.position = new Vector3(x, 0, z);
        blocks.Add(firstBlock);
    }

    // Update is called once per frame
    void Update()
    {
        bool moving = Moving(movingSpeed);
        if (blocks.Count > 0)
        {
            if (moving)
            {
                if (!destroy)
                    BuildingBlocks();
                else
                    DestroyingBlocks();
            }
            destroy = Input.GetKeyDown(KeyCode.Backspace) ? !destroy : destroy;
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                Merging();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
                NewMap();
        }

        if (mesh != null)
            PrintSelectedFace(mesh);
    }

    public bool Moving(float speed)
    {

        float forward = Input.GetAxis("Vertical") * speed;
        float sideways = Input.GetKeyDown(KeyCode.A) ? -1 : 0;
        sideways += Input.GetKeyDown(KeyCode.D) ? 1 : 0;


        Vector3 movement = (transform.forward * forward) * Time.deltaTime;

        transform.Rotate(Vector3.up * Mathf.Asin(sideways) * Mathf.Rad2Deg, Space.Self);
        transform.position += movement;

        return forward != 0;
    }

    public void BuildingBlocks()
    {
        Vector3 newBlockPos = transform.position;
        newBlockPos.y = -0.5f;
        RaycastHit hitInfo;
        if (Physics.Raycast(newBlockPos, Vector3.up, out hitInfo))
        {
            //print("Hit object name: " + hitInfo.collider.name);
        }
        else
        {
            float x = Snapping.Snap(newBlockPos.x, 10);
            float z = Snapping.Snap(newBlockPos.z, 10);
            newBlockPos = new Vector3(x, 0, z);

            bool flag = false;
            foreach (GameObject block in blocks)
            {
                if (block.transform.position == newBlockPos)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                //print("shyte");
            }
            else
            {
                print(newBlockPos);
                GameObject newBlock = Instantiate(buildingBlock, newBlockPos, transform.rotation, blocks[0].transform);

                for (int i = 0; i < newBlock.transform.childCount; i++)
                {
                    Destroy(newBlock.transform.GetChild(i).gameObject);
                }

                //string data = JsonUtility.ToJson(buildingBlock.GetComponent<MeshRenderer>());
                //JsonUtility.FromJsonOverwrite(data, newBlock.GetComponent<MeshRenderer>());

                blocks.Add(newBlock);
            }

        }
    }

    public void DestroyingBlocks()
    {
        Vector3 newBlockPos = transform.position;
        newBlockPos.y = -0.5f;
        RaycastHit hitInfo;
        print("build in");
        if (Physics.Raycast(newBlockPos, Vector3.up, out hitInfo))
        {
            print("Hit object name: " + hitInfo.collider.name);
            GameObject newBlock = hitInfo.collider.gameObject;

            bool flag = false;
            foreach(GameObject block in blocks)
            {
                if(block.transform.position == newBlock.transform.position)
                {
                    flag = true;
                    break;
                }
            }

            if (flag)
            {
                print(newBlock.transform.position);
                blocks.Remove(newBlock);
                Destroy(newBlock);
            }
            else
            {
                print("not a block");

            }
        }
        else
        {
            print("no destruction");
        }
    }

    private static ProBuilderMesh Merging(bool removeObjects = true)
    {


        List<ProBuilderMesh> meshes = MeshSelection.deep.ToList();
        ProBuilderMesh Map = meshes[0];

        List<GameObject> blocks = new List<GameObject>();
        foreach (ProBuilderMesh mesh in meshes)
        {
            blocks.Add(mesh.gameObject);
        }
        CombineMeshes.Combine(meshes, Map);



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
        print(touchingFaces.Count);
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
            print("outside of bounds");
            print("Exception:       " + ex.Message);
        }


        if (removeObjects)
        {
            blocks.RemoveAt(0);
            foreach (GameObject block in blocks)
            {
                DestroyImmediate(block);
            }
        }
        Map.ToMesh();
        Map.Refresh();

        return Map;
    }

    public void NewMap()
    {
            print("new map attempt");
        if(!(blocks.Count>0))
        {
            float x = Snapping.Snap(transform.position.x, 10);
            float z = Snapping.Snap(transform.position.z, 10);
            GameObject firstBlock = Instantiate(buildingBlock);
            firstBlock.transform.position = new Vector3(x, 0, z);
            blocks.Add(firstBlock);
        }
        else
        {
            print("finish the current map before making a new one!");
        }
    }

    public void PrintSelectedFace(ProBuilderMesh mesh)
    {
        if(mesh.selectedVertices.Count>0)
        print(ToString(mesh.selectedVertices.ToList()));

    }

    public string ToString(List<int> list)
    {
        string str = "";
        foreach(int i in list)
        {
            str += i + " ";
        }
        return str;
    }

    public string ToString(SharedVertex list)
    {
        string str = "";
        foreach (int i in list)
        {
            str += i + " ";
        }
        return str;
    }

    private static List<SimpleTuple<Face, Face>> FindTouchingFaces(ProBuilderMesh mesh)
    {
        List<SimpleTuple<Face, Face>> res = new List<SimpleTuple<Face, Face>>();


        int crt = 0;
        for (int i = 0; i < mesh.faces.Count; i++)
        {
            List<Vector3> vertex = new List<Vector3>();
            foreach (int k in mesh.faces[i].distinctIndexes)
            {
                vertex.Add(mesh.GetVertices(new List<int>() { k })[0].position);
            }
            for (int j = i + 1; j < mesh.faces.Count; j++)
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
                    print("found one!");
                    res.Add(new SimpleTuple<Face, Face>(mesh.faces[i], mesh.faces[j]));
                }
                crt = 0;

            }
        }
        return res;
    }

}
