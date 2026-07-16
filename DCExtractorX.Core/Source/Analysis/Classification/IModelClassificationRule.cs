namespace DC.Analysis.Classification
{
    /// <summary>
    /// Enumeration :   "ModelClass"
    /// 
    /// Purpose     :   the set of high-level categories a model can be classified as. Intentionally small and
    /// coarse; new values can be added here as new rules are introduced without affecting existing rules.
    /// </summary>
    public enum ModelClass
    {
        Unknown,
        StaticProp,
        AnimatedProp,
        Monster,
        NPC,
        Player,
    }

    /// <summary>
    /// Interface   :   "IModelClassificationRule"
    /// 
    /// Purpose     :   a single, self-contained heuristic that looks at a model's analysis and optionally proposes
    /// a classification with a confidence score. This is the extension point for future classification rules -
    /// add a new class implementing this interface and register it with ModelClassifier, no existing rule or
    /// caller needs to change.
    /// </summary>
    public interface IModelClassificationRule
    {
        /// <summary>
        /// A short, human-readable name for this rule (used for diagnostics/logging only).
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Attempts to classify the model. Returns false if this rule does not apply to the model at all.
        /// </summary>
        /// <param name="tAnalysis">The model's computed analysis.</param>
        /// <param name="eClass">The proposed classification, if this rule applies.</param>
        /// <param name="fScore">A confidence score in the range 0.0-1.0 for the proposed classification.</param>
        bool TryClassify(ModelAnalysis tAnalysis, out ModelClass eClass, out double fScore);
    }
}
