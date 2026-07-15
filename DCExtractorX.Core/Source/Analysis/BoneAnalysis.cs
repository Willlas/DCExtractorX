namespace DC.Analysis
{
    /// <summary>
    /// Class   :   "BoneAnalysis"
    /// 
    /// Purpose :   statistics about a model's bone hierarchy, computed from the Bone[] array's parent/children
    /// links that MDS.Load already establishes (Bone.parent, Bone.children).
    /// </summary>
    public sealed class BoneAnalysis
    {
        public int BoneCount;
        public int HierarchyDepth;
        public int MaxDepth;

        /// <summary>
        /// The number of direct children for each bone, indexed the same as the model's Bone[] array.
        /// </summary>
        public int[] ChildrenPerBone = new int[0];
        public double AvgChildren;
        public int MaxChildren;
    }
}
