using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator
{
    public enum MeshSlice { A, B };
    public MeshSlice meshSlice;
    
    //Data = Triangles, Vertices, UVs, Normals

    //Slice A Data
    private static List<int> sliceATriangles;
    private static List<Vector3> sliceAVertices;
    private static List<Vector2> sliceAUVs;
    private static List<Vector3> sliceANormals;
    //Slice B Data
    private static List<int> sliceBTriangles;
    private static List<Vector3> sliceBVertices;
    private static List<Vector2> sliceBUVs;
    private static List<Vector3> sliceBNormals;

    static Plane slicePlane;
    static List<Vector3> pointsAlongPlane;
    static Mesh originalMesh;

    static bool isHollow;

    static Mesh[] meshes = new Mesh[2];

    public static Mesh[] GetMeshes(Plane plane, Mesh mesh, bool hollow)
    {
        sliceATriangles = new List<int>();
        sliceAVertices  = new List<Vector3>();
        sliceAUVs       = new List<Vector2>();
        sliceANormals   = new List<Vector3>();

        sliceBTriangles = new List<int>();
        sliceBVertices  = new List<Vector3>();
        sliceBUVs       = new List<Vector2>();
        sliceBNormals   = new List<Vector3>();

        pointsAlongPlane = new List<Vector3>();
        slicePlane      = plane;
        originalMesh    = mesh;

        isHollow = hollow;

        CreateMeshes();
        return meshes;
    }

    static void CreateMeshes()
    {
        int[] meshTriangles = originalMesh.triangles;
        Vector3[] meshVerts = originalMesh.vertices;
        Vector3[] meshNormals = originalMesh.normals;
        Vector2[] meshUvs = originalMesh.uv;

        for(int i = 0; i < meshTriangles.Length; i += 3)
        {
            Vector3 vert1 = meshVerts[meshTriangles[i]];
            int vert1Index = Array.IndexOf(meshVerts, vert1);
            Vector2 uv1 = meshUvs[vert1Index];
            Vector3 normal1 = meshNormals[vert1Index];
            bool vert1Side = slicePlane.GetSide(vert1);

            Vector3 vert2 = meshVerts[meshTriangles[i + 1]];
            int vert2Index = Array.IndexOf(meshVerts, vert2);
            Vector2 uv2 = meshUvs[vert2Index];
            Vector3 normal2 = meshNormals[vert2Index];
            bool vert2Side = slicePlane.GetSide(vert2);

            Vector3 vert3 = meshVerts[meshTriangles[i + 2]];
            int vert3Index = Array.IndexOf(meshVerts, vert3);
            Vector2 uv3 = meshUvs[vert3Index];
            Vector3 normal3 = meshNormals[vert3Index];
            bool vert3Side = slicePlane.GetSide(vert3);

            //Same side vertices
            if (vert1Side == vert2Side && vert2Side == vert3Side)
            {
                MeshSlice side = (vert1Side) ? MeshSlice.A : MeshSlice.B;
                AddTNUSideCheck(side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
            }
            else
            {
                Vector3 intersection1;
                Vector3 intersection2;

                Vector2 intersection1Uv;
                Vector2 intersection2Uv;

                MeshSlice side1 = (vert1Side) ? MeshSlice.A : MeshSlice.B;
                MeshSlice side2 = (vert1Side) ? MeshSlice.B : MeshSlice.A;

                if (vert1Side == vert2Side)
                {                       
                    intersection1 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection1Uv);
                    intersection2 = GetRayPlaneIntersectionPointAndUv(vert3, uv3, vert1, uv1, out intersection2Uv);

                    AddTNUSideCheck(side1, vert1, null, uv1, vert2, null, uv2, intersection1, null, intersection1Uv, false, false);
                    AddTNUSideCheck(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, false, false);

                    AddTNUSideCheck(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, false, false);

                }
                else if (vert1Side == vert3Side)
                {                       
                    intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                    intersection2 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection2Uv);

                    AddTNUSideCheck(side1, vert1, null, uv1, intersection1, null, intersection1Uv, vert3, null, uv3, false, false);
                    AddTNUSideCheck(side1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, vert3, null, uv3, false, false);

                    AddTNUSideCheck(side2, intersection1, null, intersection1Uv, vert2, null, uv2, intersection2, null, intersection2Uv, false, false);
                }
                else
                {
                    intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                    intersection2 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert3, uv3, out intersection2Uv);

                    AddTNUSideCheck(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, false, false);

                    AddTNUSideCheck(side2, intersection1, null, intersection1Uv, vert2, null, uv2, vert3, null, uv3, false, false);
                    AddTNUSideCheck(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, false, false);
                }

                pointsAlongPlane.Add(intersection1);
                pointsAlongPlane.Add(intersection2);
            }
        }

        if (!isHollow)
        {
            JoinPointsAlongPlane();
        }

        SetMeshData();
    }

    static void AddTNUSideCheck(MeshSlice side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
    {
        if (side == MeshSlice.A)
        {
            AddTrianglesNormalsAndUVs(ref sliceAVertices, ref sliceATriangles, ref sliceANormals, ref sliceAUVs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
        }
        else
        {
            AddTrianglesNormalsAndUVs(ref sliceBVertices, ref sliceBTriangles, ref sliceBNormals, ref sliceBUVs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
        }
    }

    static void AddTrianglesNormalsAndUVs(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
    {
        if (addFirst)
        {
            ShiftTriangleIndexes(ref triangles);
        }

        //Triangle Index 1
        int tri1Index = vertices.IndexOf(vertex1);

        if (tri1Index > -1 && shareVertices)
        {
            triangles.Add(tri1Index);
        }
        else
        {
            if (normal1 == null)
            {
                normal1 = ComputeNormal(vertex1, vertex2, vertex3);
            }

            int? i = null;
            if (addFirst)
            {
                i = 0;
            }

            AddVertNormalUV(ref vertices, ref normals, ref uvs, ref triangles, vertex1, (Vector3)normal1, uv1, i);
        }

        //Triangle Index 2
        int tri2Index = vertices.IndexOf(vertex2);

        if (tri2Index > -1 && shareVertices)
        {
            triangles.Add(tri2Index);
        }
        else
        {
            if (normal2 == null)
            {
                normal2 = ComputeNormal(vertex2, vertex3, vertex1);
            }

            int? i = null;

            if (addFirst)
            {
                i = 1;
            }

            AddVertNormalUV(ref vertices, ref normals, ref uvs, ref triangles, vertex2, (Vector3)normal2, uv2, i);
        }

        //Triangle Index 3
        int tri3Index = vertices.IndexOf(vertex3);

        if (tri3Index > -1 && shareVertices)
        {
            triangles.Add(tri3Index);
        }
        else
        {
            if (normal3 == null)
            {
                normal3 = ComputeNormal(vertex3, vertex1, vertex2);
            }

            int? i = null;
            if (addFirst)
            {
                i = 2;
            }

            AddVertNormalUV(ref vertices, ref normals, ref uvs, ref triangles, vertex3, (Vector3)normal3, uv3, i);
        }
    }

    static void ShiftTriangleIndexes(ref List<int> triangles)
    {
        for (int j = 0; j < triangles.Count; j += 3)
        {
            triangles[j] += +3;
            triangles[j + 1] += 3;
            triangles[j + 2] += 3;
        }
    }

    static Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
    {
        Vector3 side1 = vertex2 - vertex1;
        Vector3 side2 = vertex3 - vertex1;

        Vector3 normal = Vector3.Cross(side1, side2);

        return normal;
    }

    static void AddVertNormalUV(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
    {
        if (index != null)
        {
            int i = (int)index;
            vertices.Insert(i, vertex);
            uvs.Insert(i, uv);
            normals.Insert(i, normal);
            triangles.Insert(i, i);
        }
        else
        {
            vertices.Add(vertex);
            normals.Add(normal);
            uvs.Add(uv);
            triangles.Add(vertices.IndexOf(vertex));
        }
    }

    static Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
    {
        float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection);
        uv = InterpolateUvs(vertex1Uv, vertex2Uv, distance);
        return pointOfIntersection;
    }

    static float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
    {
        Ray ray = new Ray(vertex1, (vertex2 - vertex1));
        slicePlane.Raycast(ray, out float distance);
        pointOfintersection = ray.GetPoint(distance);
        return distance;
    }

    static Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
    {
        Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
        return uv;
    }

    static void JoinPointsAlongPlane()
    {
        Vector3 halfway = GetHalfwayPoint(out float distance);

        for (int i = 0; i < pointsAlongPlane.Count; i += 2)
        {
            Vector3 firstVertex;
            Vector3 secondVertex;

            firstVertex = pointsAlongPlane[i];
            secondVertex = pointsAlongPlane[i + 1];

            Vector3 normal3 = ComputeNormal(halfway, secondVertex, firstVertex);
            normal3.Normalize();

            var direction = Vector3.Dot(normal3, slicePlane.normal);

            if (direction > 0)
            {
                AddTNUSideCheck(MeshSlice.A, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                AddTNUSideCheck(MeshSlice.B, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
            }
            else
            {
                AddTNUSideCheck(MeshSlice.A, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                AddTNUSideCheck(MeshSlice.B, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
            }
        }
    }

    static Vector3 GetHalfwayPoint(out float distance)
    {
        if (pointsAlongPlane.Count > 0)
        {
            Vector3 firstPoint = pointsAlongPlane[0];
            Vector3 furthestPoint = Vector3.zero;
            distance = 0f;

            foreach (Vector3 point in pointsAlongPlane)
            {
                float currentDistance = 0f;
                currentDistance = Vector3.Distance(firstPoint, point);

                if (currentDistance > distance)
                {
                    distance = currentDistance;
                    furthestPoint = point;
                }
            }

            return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
        }
        else
        {
            distance = 0;
            return Vector3.zero;
        }
    }

    static void SetMeshData()
    {
        meshes[0] = new Mesh();
        meshes[0].vertices = sliceAVertices.ToArray();
        meshes[0].triangles = sliceATriangles.ToArray();
        meshes[0].normals = sliceANormals.ToArray();
        meshes[0].uv = sliceAUVs.ToArray();

        meshes[1] = new Mesh();
        meshes[1].vertices = sliceBVertices.ToArray();
        meshes[1].triangles = sliceBTriangles.ToArray();
        meshes[1].normals = sliceBNormals.ToArray();
        meshes[1].uv = sliceBUVs.ToArray();
    }
}
