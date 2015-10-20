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

        transform.SetParent(null);
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
        const int numVertsPerFrame = 2;
        const int numVertsTotal = numVertsPerFrame * TrailNumFrames;
        
        // indices
        const int numSegs = TrailNumFrames - 1;
        const int numIndicesPerSeg = 6;
        const int numIndicesTotal = numIndicesPerSeg * numSegs;

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
        const int numVertsPerFrame = 2;

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        for (int i = 0; i < numFrames; ++i )
        {
            // get curr frame
            int iFrame = (currFrameIndex + i) % TrailNumFrames;
            var frame = frames[iFrame];

            // vert offset into VBO
            int vertOffset = iFrame * numVertsPerFrame;

            newVertices[vertOffset + 0] = frame.posBot;
            newVertices[vertOffset + 1] = frame.posTop;
        }


        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        // TODO: calc normals in real-time
    }

    void OnDrawGizmos()
    {
        var boxCollider = GameObject.Find("Collision").GetComponent<BoxCollider>();
        var boxHeight = boxCollider.size.y;
        var boxUp = boxCollider.transform.up;
        var boxPosCenter = boxCollider.transform.position + boxUp * boxHeight * 0.75f;
        

        // draw center
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(boxPosCenter, 0.03f);

        // draw
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(boxPosCenter - boxUp * boxHeight*0.5f, 0.02f);
        Gizmos.DrawSphere(boxPosCenter + boxUp * boxHeight*0.5f, 0.02f);
    }

    void UpdateFrame()
    {
        var boxCollider = GameObject.Find("Collision").GetComponent<BoxCollider>();
        var boxHeight = boxCollider.size.y;
        var boxUp = boxCollider.transform.up;
        var boxPosCenter = boxCollider.transform.position + boxUp * boxHeight * 0.75f;

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
