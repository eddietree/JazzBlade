using UnityEngine;
using System.Collections;

public class BladeTrail : MonoBehaviour 
{
    const int TrailNumFrames = 64;

    struct Frame
    {
        public Vector3 posTop;
        public Vector3 posBot;
        public Quaternion quat;
    };

    Frame[] frames;
    int numFrames;
    int currFrameIndex;

    private Vector3[] newVertices;
    private Vector2[] newUV;
    private int[] newTriangles;

	void Start () 
    {
        InitFrames();
        InitMesh();
	}

    void InitFrames()
    {
        frames = new Frame[TrailNumFrames];
        numFrames = 0;
        currFrameIndex = 0;
    }

    void InitMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // verts
        int numVertsPerFrame = 2;
        int numVertsTotal = numVertsPerFrame * TrailNumFrames;

        // indices
        int numSegs = TrailNumFrames - 1;
        int numIndicesPerSeg = 6;
        int numIndicesTotal = numIndicesPerSeg * numSegs;

        // init arrays
        newVertices = new Vector3[numVertsTotal];
        newUV = new Vector2[numVertsTotal];
        newTriangles = new int[numIndicesTotal];
        
        // zero out data
        for (int i = 0; i < numVertsTotal; ++i)
        {
            newVertices[i] = Vector3.zero;

            // TODO - maybe can precalc uvs?
            newUV[i] = Vector3.zero;
        }

        // garbage data
        newVertices[0] = new Vector3(0.0f, 0.0f, 1.0f);
        newVertices[1] = new Vector3(1.0f, 0.0f, 0.0f);
        newVertices[2] = new Vector3(1.0f, 1.0f, 1.0f);
        newVertices[3] = new Vector3(0.0f, 1.0f, 0.0f);
        

        // indices
        for (int i = 0; i < numSegs; ++i )
        {
            int indexOffset = i * numIndicesPerSeg;
            int vertOffset = numVertsPerFrame * i;

            newTriangles[indexOffset + 0] = vertOffset+0;
            newTriangles[indexOffset + 1] = vertOffset+1;
            newTriangles[indexOffset + 2] = vertOffset+3;

            newTriangles[indexOffset + 3] = vertOffset + 0;
            newTriangles[indexOffset + 4] = vertOffset + 2;
            newTriangles[indexOffset + 5] = vertOffset + 3;
        }

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        mesh.RecalculateNormals();
    }
	
	void Update () 
    {
        UpdateFrame();
        UpdateVerts();
	}

    void UpdateVerts()
    {
        // TODO

        // TODO: calc normals in real-time
    }

    void UpdateFrame()
    {
        var boxCollider = GetComponent<BoxCollider>();
        var boxPosCenter = boxCollider.transform.position;
        var boxHeight = boxCollider.size.y;
        var boxUp = boxCollider.transform.up;

        // check pos?
        var boxPosTop = boxPosCenter + boxUp * boxHeight * 0.5f;
        var boxPosBot = boxPosCenter - boxUp * boxHeight * 0.5f;

        frames[currFrameIndex].posTop = boxPosTop;
        frames[currFrameIndex].posBot = boxPosBot;
        frames[currFrameIndex].quat = boxCollider.transform.rotation;

        numFrames = Mathf.Min(numFrames + 1, TrailNumFrames);
        currFrameIndex = (currFrameIndex + 1) % TrailNumFrames;
    }
}
