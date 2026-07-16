using DC.Analysis.Classification;
using DC.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DC.Types.TriangleStrip;

namespace DC.Analysis
{
    /// <summary>
    /// Class   :   "ModelAnalyzer"
    /// 
    /// Purpose :   computes a ModelAnalysis purely from the original, in-memory DC.Types.Model produced by
    /// MDS.Load (bScanOnly: false) - never from an exported .obj/.smd file. Read-only: does not modify the model.
    /// </summary>
    public static class ModelAnalyzer
    {
        static readonly ModelClassifier s_tClassifier = ModelClassifier.CreateDefault();

        /// <summary>
        /// Analyzes a single model, returning its full statistics and classification.
        /// </summary>
        public static ModelAnalysis Analyze(Model tModel)
        {
            if (tModel == null)
                throw new ArgumentNullException(nameof(tModel));

            ModelAnalysis tAnalysis = new ModelAnalysis
            {
                ModelName = string.IsNullOrEmpty(tModel.Name) ? Path.GetFileNameWithoutExtension(tModel.filePath) : tModel.Name,
                FilePath = tModel.filePath,
                MeshCount = tModel.Meshes?.Length ?? 0,
                BoneCount = tModel.Bones?.Length ?? tModel.BoneCount,
            };

            AnalyzeMeshes(tModel, tAnalysis);

            tAnalysis.Bones = AnalyzeBones(tModel.Bones);
            tAnalysis.HierarchyDepth = tAnalysis.Bones.HierarchyDepth;
            tAnalysis.MaxHierarchyDepth = tAnalysis.Bones.MaxDepth;

            tAnalysis.Weights = AnalyzeWeights(tModel.Weights);
            tAnalysis.WeightHeaderCount = tAnalysis.Weights.HeaderCount;
            tAnalysis.WeightEntryCount = tAnalysis.Weights.WeightCount;
            tAnalysis.UniqueMeshBones = tAnalysis.Weights.UniqueMeshBoneIndexes;
            tAnalysis.UniqueInfluenceBones = tAnalysis.Weights.UniqueBoneIndexes;

            AnalyzeSiblingFiles(tModel, tAnalysis);

            tAnalysis.AvgVerticesPerMesh = tAnalysis.MeshCount > 0 ? (double)tAnalysis.VertexCount / tAnalysis.MeshCount : 0.0;
            tAnalysis.AvgVerticesPerBone = tAnalysis.BoneCount > 0 ? (double)tAnalysis.VertexCount / tAnalysis.BoneCount : 0.0;

            s_tClassifier.Classify(tAnalysis);

            return tAnalysis;
        }

        static void AnalyzeMeshes(Model tModel, ModelAnalysis tAnalysis)
        {
            if (tModel.Meshes == null)
                return;

            foreach (Mesh tMesh in tModel.Meshes)
            {
                tAnalysis.VertexCount += tMesh.VertexCount;
                tAnalysis.NormalCount += tMesh.NormalCount;
                tAnalysis.UVCount += tMesh.UVCount;
                tAnalysis.MaterialCount += tMesh.MaterialCount;
                tAnalysis.FaceCount += CountFaces(tMesh);
            }
        }

        /// <summary>
        /// Estimates the triangle count of a mesh from its triangle strips, using the same OpenGL primitive
        /// semantics the format itself is based on (see TriangleStrip.PrimitiveStyle).
        /// </summary>
        static int CountFaces(Mesh tMesh)
        {
            if (tMesh.Polygons == null)
                return 0;

            int nFaces = 0;
            foreach (Polygon tPolygon in tMesh.Polygons)
            {
                if (tPolygon.Strips == null)
                    continue;

                foreach (TriangleStrip tStrip in tPolygon.Strips)
                {
                    switch (tStrip.Style)
                    {
                        case PrimitiveStyle.GL_TRIANGLES:
                        case PrimitiveStyle.DC2_COLLISION_TRIANGLES:
                            nFaces += tStrip.IndexCount / 3;
                            break;
                        case PrimitiveStyle.GL_TRIANGLE_STRIP:
                        case PrimitiveStyle.GL_TRIANGLE_FAN:
                            nFaces += Math.Max(0, tStrip.IndexCount - 2);
                            break;
                        default:
                            //Points/lines/quads/etc don't contribute triangle faces.
                            break;
                    }
                }
            }
            return nFaces;
        }

        static BoneAnalysis AnalyzeBones(Bone[] tBones)
        {
            BoneAnalysis tResult = new BoneAnalysis();
            if (tBones == null || tBones.Length == 0)
                return tResult;

            tResult.BoneCount = tBones.Length;
            tResult.ChildrenPerBone = new int[tBones.Length];
            for (int i = 0; i < tBones.Length; i++)
                tResult.ChildrenPerBone[i] = tBones[i].children?.Count ?? 0;

            tResult.MaxChildren = tResult.ChildrenPerBone.Length > 0 ? tResult.ChildrenPerBone.Max() : 0;
            tResult.AvgChildren = tResult.ChildrenPerBone.Length > 0 ? tResult.ChildrenPerBone.Average() : 0.0;

            int nMaxDepth = 0;
            for (int i = 0; i < tBones.Length; i++)
                nMaxDepth = Math.Max(nMaxDepth, DepthOf(tBones[i]));

            //The spec calls out both "HierarchyDepth" and "MaxHierarchyDepth" as model stats; both describe the
            //same measurement (the deepest parent chain in the skeleton) since there's only one hierarchy per model.
            tResult.HierarchyDepth = nMaxDepth;
            tResult.MaxDepth = nMaxDepth;

            return tResult;
        }

        /// <summary>
        /// Walks a bone's parent chain to find its depth from the root. Guards against malformed/cyclic parent
        /// data (which would otherwise never occur, but corrupt files are always possible) so analysis can never
        /// infinite-loop on bad input.
        /// </summary>
        static int DepthOf(Bone tBone)
        {
            int nDepth = 0;
            HashSet<Bone> tVisited = new HashSet<Bone>();
            Bone tCurrent = tBone;
            while (tCurrent?.parent != null && tVisited.Add(tCurrent))
            {
                nDepth++;
                tCurrent = tCurrent.parent;
            }
            return nDepth;
        }

        static WGTAnalysis AnalyzeWeights(VertexWeight[] tWeights)
        {
            WGTAnalysis tResult = new WGTAnalysis();
            if (tWeights == null || tWeights.Length == 0)
                return tResult;

            tResult.WeightCount = tWeights.Length;

            HashSet<int> tMeshBones = new HashSet<int>();
            HashSet<int> tBoneIndexes = new HashSet<int>();
            HashSet<(int MeshIndex, int BoneIndex)> tHeaderGroups = new HashSet<(int, int)>();
            Dictionary<(int MeshIndex, int VertexIndex), int> tInfluencesPerVertex = new Dictionary<(int, int), int>();

            foreach (VertexWeight tWeight in tWeights)
            {
                tMeshBones.Add(tWeight.meshIndex);
                tBoneIndexes.Add(tWeight.boneIndex);
                tHeaderGroups.Add((tWeight.meshIndex, tWeight.boneIndex));

                (int, int) tVertexKey = (tWeight.meshIndex, tWeight.vertexIndex);
                tInfluencesPerVertex.TryGetValue(tVertexKey, out int nCount);
                tInfluencesPerVertex[tVertexKey] = nCount + 1;
            }

            tResult.UniqueMeshBoneIndexes = tMeshBones.Count;
            tResult.UniqueBoneIndexes = tBoneIndexes.Count;
            tResult.HeaderCount = tHeaderGroups.Count;

            tResult.MaxInfluencesPerVertex = tInfluencesPerVertex.Count > 0 ? tInfluencesPerVertex.Values.Max() : 0;
            tResult.AvgInfluencesPerVertex = tInfluencesPerVertex.Count > 0 ? tInfluencesPerVertex.Values.Average() : 0.0;

            return tResult;
        }

        /// <summary>
        /// Determines HasWGT/HasMOT/HasBBP the same way MDS.Load itself does (sibling file presence), falling
        /// back to the in-memory data actually being populated in case the model was loaded some other way.
        /// </summary>
        static void AnalyzeSiblingFiles(Model tModel, ModelAnalysis tAnalysis)
        {
            if (string.IsNullOrEmpty(tModel.filePath))
            {
                tAnalysis.HasWGT = tModel.hasWeights;
                tAnalysis.HasMOT = tModel.Animations != null && tModel.Animations.Length > 0;
                tAnalysis.HasBBP = tModel.Bones != null && tModel.Bones.Any(b => b.hasBindPose);
                return;
            }

            string szBBPPath = Path.ChangeExtension(tModel.filePath, ".bbp");
            string szWGTPath = Path.ChangeExtension(tModel.filePath, ".wgt");
            string szMOTPath = Path.ChangeExtension(tModel.filePath, ".mot");

            tAnalysis.HasBBP = File.Exists(szBBPPath) || (tModel.Bones != null && tModel.Bones.Any(b => b.hasBindPose));
            tAnalysis.HasWGT = File.Exists(szWGTPath) || tModel.hasWeights;
            tAnalysis.HasMOT = File.Exists(szMOTPath) || (tModel.Animations != null && tModel.Animations.Length > 0);
        }
    }
}
