using UnityEngine;
using System.Collections;

public class BladeTrail : MonoBehaviour 
{
    const int TrailNumFrames = 256;

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
        float deltaTexCoordU = 1.0f / (TrailNumFrames - 1);
        for (int iFrame = 0; iFrame < TrailNumFrames; ++iFrame)
        {
            int vertIndex = iFrame * numVertsPerFrame;
            newVertices[vertIndex + 0] = Vector3.zero;
            newVertices[vertIndex + 1] = Vector3.zero;

            // can precalc uvs?
            newUV[vertIndex + 0] = new Vector2( deltaTexCoordU * iFrame, 0.0f );
            newUV[vertIndex + 1] = new Vector2(deltaTexCoordU * iFrame, 1.0f);
        }
        
        // indices
        for (int i = 0; i < numSegs; ++i )
        {
            int indexOffset = i * numIndicesPerSeg;
            int vertOffset = numVertsPerFrame * i;

            newTriangles[indexOffset + 0] = vertOffset+0;
            newTriangles[indexOffset + 1] = vertOffset+1;
            newTriangles[indexOffset + 2] = vertOffset+3;

            newTriangles[indexOffset + 3] = vertOffset + 0;
            newTriangles[indexOffset + 4] = vertOffset + 3;
            newTriangles[indexOffset + 5] = vertOffset + 2;
        }

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        mesh.RecalculateNormals();
        mesh.MarkDynamic();
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

        for (int i = 0; i < numFrames; ++i )
        {
            // get curr frame
            int iFrame = (currFrameIndex - i - 1 + TrailNumFrames) % TrailNumFrames;
            var frame = frames[iFrame];

            // vert offset into VBO
            int vertOffset = i * numVertsPerFrame;

            newVertices[vertOffset + 0] = frame.posBot;
            newVertices[vertOffset + 1] = frame.posTop;
        }

        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;

        // TODO: calc normals in real-time

        mesh.RecalculateBounds();
    }

    void OnDrawGizmos()
    {
        var boxColliderObj = GameObject.Find("SwordCollision");

        if (boxColliderObj == null)
            return;

        var boxCollider = boxColliderObj.GetComponent<BoxCollider>();
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
        var boxColliderObj = GameObject.Find("SwordCollision");

        if (boxColliderObj == null)
            return;

        var boxCollider = boxColliderObj.GetComponent<BoxCollider>();
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
