using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UIElements;

public class CreateMazeRooted : MonoBehaviour
{

    [SerializeField] private GameObject mazeCube;
    public float cubeSize = 10;
    public int squareDimentions = 30;
    public Vector3 startPos = Vector3.zero;
    public int mainPathLength = 30;
    public int rootInfrequency = 3;
    public int rootedRootNum = 1;
    public Vector3 mazeStartPos;
    public Material mainPathMaterial;
    public bool CreatExtraRoots = false;
    public bool ReCreate = false;

    private PositionUsage[,] positionUsages;
    private List<GameObject> mainPath = new List<GameObject>();
    private List<GameObject> allBlocks = new List<GameObject>();
    private int startI = 0;
    private int startJ = 0;
    private Dictionary<string, Vector3> maze = new Dictionary<string, Vector3>();

    private void Start()
    {
        FetchCoords();
        MazeCreation();
    }
    private void Update()
    {
        if(ReCreate)
        {
            ReCreate = false;

            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            mainPath.Clear();
            allBlocks.Clear();
            FetchCoords();

            MazeCreation();
        }
        if(CreatExtraRoots)
        {
            CreatExtraRoots = false;
            CreateRootedRoots(rootedRootNum, rootInfrequency);
        }
    }

    private void FetchCoords()
    {
        positionUsages = new PositionUsage[squareDimentions, squareDimentions];
        Vector3 currentPos = startPos;
        for (int i = 0; i < squareDimentions; i++)
        {
            for (int j = 0; j < squareDimentions; j++)
            {
                positionUsages[i, j] = new PositionUsage(false, currentPos);
                if (positionUsages[i, j].position == mazeStartPos)
                {
                    positionUsages[i,j].used = true;
                    startI = i;
                    startJ = j;
                }
                currentPos += Vector3.right * cubeSize;
            }
            currentPos = (currentPos.z + cubeSize) * Vector3.forward + Vector3.right * startPos.x + Vector3.up * startPos.y;
        }
    }

    private void MazeCreation()
    {
        CreateMainRout();
        ApplyMaterialToPath();
        CreateRootedRoots(rootedRootNum, rootInfrequency);
    }

    private int NeighborsaNumber(int i, int j)
    {
        int num = 0;

        try
        {
            if (positionUsages[i, j - 1].used)
                num++;
        }
        catch { }


        try
        {
            if (positionUsages[i, j + 1].used)
                num++;
        }
        catch { }


        try
        {
            if (positionUsages[i - 1, j].used)
                num++;
        }
        catch { }

        try
        {
            if (positionUsages[i + 1, j].used)
                num++;
        }
        catch { }


        return num;
    }

    private void CreateMainRout()
    {
        GameObject parent = new GameObject();
        parent.transform.SetParent(gameObject.transform);
        parent.name = "Main Path";
        GameObject currCube = Instantiate(mazeCube, mazeStartPos, mazeCube.transform.rotation);
        mainPath.Add(currCube);
        currCube.transform.SetParent(parent.transform);
        int currI = startI, currJ = startJ;

        int direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left

        for (int i = 0; i < mainPathLength - 1; i++)
        {
        Repeat:
            if (direction == 0)
            {
                try
                {
                    print("In try");
                    if (!positionUsages[currI, currJ - 1].used)
                    {
                        print("In IF");
                        print(NeighborsaNumber(currI, currJ - 1));
                        if (NeighborsaNumber(currI, currJ - 1) == 1)
                        {
                            print("creating cube");
                            currCube = Instantiate(mazeCube, positionUsages[currI, currJ - 1].position, mazeCube.transform.rotation);
                            positionUsages[currI, currJ - 1].used = true;
                            mainPath.Add(currCube);
                            currJ--;
                        }
                    }
                }
                catch
                {
                    print("in catch");
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }
            else if (direction == 1)
            {
                try
                {
                    if (!positionUsages[currI + 1, currJ].used)
                    {
                        if (NeighborsaNumber(currI + 1, currJ) == 1)
                        {
                            currCube = Instantiate(mazeCube, positionUsages[currI + 1, currJ].position, mazeCube.transform.rotation);
                            positionUsages[currI + 1, currJ].used = true;
                            mainPath.Add(currCube);
                            currI++;
                        }
                    }
                }
                catch
                {
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }
            else if (direction == 2)
            {
                try
                {
                    if (!positionUsages[currI, currJ + 1].used)
                    {
                        if (NeighborsaNumber(currI, currJ + 1) == 1)
                        {
                            currCube = Instantiate(mazeCube, positionUsages[currI, currJ + 1].position, mazeCube.transform.rotation);
                            positionUsages[currI, currJ + 1].used = true;
                            mainPath.Add(currCube);
                            currJ++;
                        }
                    }
                }
                catch
                {
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }
            else if (direction == 3)
            {
                try
                {
                    if (!positionUsages[currI - 1, currJ].used)
                    {
                        if (NeighborsaNumber(currI - 1, currJ) == 1)
                        {
                            currCube = Instantiate(mazeCube, positionUsages[currI - 1, currJ].position, mazeCube.transform.rotation);
                            positionUsages[currI - 1, currJ].used = true;
                            mainPath.Add(currCube);
                            currI--;
                        }
                    }
                }
                catch
                {
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }


            currCube.transform.SetParent(parent.transform);

            direction = Random.Range(0, 3);//0 is forward, 1 is right, 2 is backward, 3 is left
        }
        allBlocks.AddRange(mainPath);
    }
    private void ApplyMaterialToPath()
    {
        foreach(GameObject block in mainPath)
        {
            block.GetComponent<MeshRenderer>().material = mainPathMaterial;
        }
    }

    private void CreateRoot(int i, int j, int length, Transform Parent)
    {
        GameObject parent = new GameObject();
        parent.transform.SetParent(Parent.transform);

        GameObject currCube;

        int direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left

        for (int k = 0; k < length; k++)
        {
        Repeat:
            if (direction == 0)
            {
                try
                {
                    print("In try");
                    if (!positionUsages[i, j - 1].used)
                    {
                        print("In IF");
                        print(NeighborsaNumber(i, j - 1));
                        if (NeighborsaNumber(i, j - 1) == 1)
                        {
                            print("creating cube");
                            currCube = Instantiate(mazeCube, positionUsages[i, j - 1].position, mazeCube.transform.rotation);
                            allBlocks.Add(currCube);
                            currCube.transform.SetParent(parent.transform);

                            positionUsages[i, j - 1].used = true;
                            j--;
                        }
                    }
                }
                catch
                {
                    print("in catch");
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }
            else if (direction == 1)
            {
                try
                {
                    if (!positionUsages[i + 1, j].used)
                    {
                        if (NeighborsaNumber(i + 1, j) == 1)
                        {
                            currCube = Instantiate(mazeCube, positionUsages[i + 1, j].position, mazeCube.transform.rotation);
                            allBlocks.Add(currCube);
                            currCube.transform.SetParent(parent.transform);

                            positionUsages[i + 1, j].used = true;
                            mainPath.Add(currCube);
                            i++;
                        }
                    }
                }
                catch
                {
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }
            else if (direction == 2)
            {
                try
                {
                    if (!positionUsages[i, j + 1].used)
                    {
                        if (NeighborsaNumber(i, j + 1) == 1)
                        {
                            currCube = Instantiate(mazeCube, positionUsages[i, j + 1].position, mazeCube.transform.rotation);
                            allBlocks.Add(currCube);
                            currCube.transform.SetParent(parent.transform);

                            positionUsages[i, j + 1].used = true;
                            mainPath.Add(currCube);
                            j++;
                        }
                    }
                }
                catch
                {
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }
            else if (direction == 3)
            {
                try
                {
                    if (!positionUsages[i - 1, j].used)
                    {
                        if (NeighborsaNumber(i - 1, j) == 1)
                        {
                            currCube = Instantiate(mazeCube, positionUsages[i - 1, j].position, mazeCube.transform.rotation);
                            allBlocks.Add(currCube);
                            currCube.transform.SetParent(parent.transform);

                            positionUsages[i - 1, j].used = true;
                            mainPath.Add(currCube);
                            i--;
                        }
                    }
                }
                catch
                {
                    direction = Random.Range(0, 3);//0 is up, 1 is right, 2 is down, 3 is left
                    goto Repeat;
                }
            }

            direction = Random.Range(0, 3);//0 is forward, 1 is right, 2 is backward, 3 is left
        }


        parent.name = "Root(" + parent.transform.childCount + ")";
        if(parent.transform.childCount == 0)
            DestroyImmediate(parent);
    }
    private void CreateRoots(int infreqency)
    {
        List<int[]> indexes= new List<int[]>();
        for (int i = 0; i < squareDimentions; i++)
        {
            for (int j = 0; j < squareDimentions; j++)
            {
                if (positionUsages[i, j].used)
                {
                    indexes.Add(new int[2] { i, j });
                }
            }
        }

        foreach(int[] index in indexes)
        {
            int skipper = Random.Range(0, infreqency);
            if (skipper == 0)
            {
                GameObject parent=null;
                foreach(GameObject block in allBlocks)
                {
                    if (block.transform.position == positionUsages[index[0], index[1]].position)
                    { 
                        parent = block;
                        break;
                    }
                }
                CreateRoot(index[0], index[1],
                    Random.Range((squareDimentions - index[0] + squareDimentions - index[1]) / 2, squareDimentions - index[0] + squareDimentions - index[1])
                    , parent.transform);
            }

        }
    }
    private void CreateRootedRoots(int rootNum, int infreqency)
    {
        for (int i = 0; i < rootNum; i++)
        {
            CreateRoots(infreqency);
        }
    }


}
