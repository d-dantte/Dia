using Axis.Luna.Common;

namespace Axis.Dia.Convert.Type
{
    public readonly struct ObjectPath: IDefaultValueProvider<ObjectPath>
    {
        /// <summary>
        /// Current node in the object graph path
        /// </summary>
        public IPathNode CurrentNode { get; }

        /// <summary>
        /// Current depth in the object graph path
        /// </summary>
        public int Depth => CurrentNode.Depth;

        #region DefaultValueProvider
        public bool IsDefault => default(ObjectPath).Equals(this);

        public static ObjectPath Default => default;
        #endregion

        internal ObjectPath(System.Type rootType)
        {
            CurrentNode = IPathNode.OfRoot(rootType);
        }

        private ObjectPath(IPathNode node)
        {
            CurrentNode = node;
        }

        internal ObjectPath Next(
            System.Type type,
            string id)
            => new(IPathNode.Of(type, id, CurrentNode));

        public override string ToString() => ToString(CurrentNode);

        private static string ToString(IPathNode node)
        {
            var prefix = node.Parent is not null
                ? $"{ToString(node.Parent)}/"
                : "";

            return $"{prefix}{node.AsString()}";
        }

        #region nested types

        public interface IPathNode
        {
            /// <summary>
            /// The ID of this node. In object-graph parlance, this is the property/index
            /// used to access the object represented by this node from the parent object.
            /// </summary>
            string Id { get; }

            /// <summary>
            /// The parent of this node, if one exists
            /// </summary>
            IPathNode? Parent { get; }

            /// <summary>
            /// The clr type of the object represented by this path
            /// </summary>
            System.Type Type { get; }

            /// <summary>
            /// Path Depth
            /// </summary>
            int Depth { get; }


            /// <summary>
            /// Gets a string representation of this node. used by the <see cref="ObjectPath"/>
            /// when priting the entire path
            /// </summary>
            /// <returns>The string representation of this node</returns>
            string AsString();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public static IPathNode OfRoot(System.Type type) => new RootNode(type);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="id"></param>
            /// <param name="parent"></param>
            /// <returns></returns>
            public static IPathNode Of(System.Type type, string id, IPathNode parent)
            {
                ArgumentNullException.ThrowIfNull(type);
                ArgumentNullException.ThrowIfNull(id);

                if (int.TryParse(id, out var index))
                    return new IndexNode(type, index, parent);

                else return new PropertyNode(type, id, parent);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="index"></param>
            /// <param name="parent"></param>
            /// <returns></returns>
            public static IPathNode OfIndex(
                System.Type type,
                int index,
                IPathNode parent)
            {
                ArgumentNullException.ThrowIfNull(type);

                return new IndexNode(type, index, parent);
            }
        }

        //public abstract record PathNode
        //{
        //    /// <summary>
        //    /// The ID of this node. In object-graph parlance, this is the property/index
        //    /// used to access the object represented by this node from the parent object.
        //    /// </summary>
        //    public string Id { get; }

        //    /// <summary>
        //    /// The parent of this node, if one exists
        //    /// </summary>
        //    public PathNode? Parent { get; }

        //    /// <summary>
        //    /// The clr type of the object represented by this path
        //    /// </summary>
        //    public System.Type Type { get; }

        //    /// <summary>
        //    /// Path depth
        //    /// </summary>
        //    public int Depth => Parent is null ? 0 : Parent.Depth + 1;


        //    protected PathNode(System.Type type, string id, PathNode? parent)
        //    {
        //        Type = type ?? throw new ArgumentNullException(nameof(type));
        //        Id = id ?? throw new ArgumentNullException(nameof(id));
        //        Parent = parent;
        //    }

        //    /// <summary>
        //    /// Gets a string representation of this node. used by the <see cref="ObjectPath"/>
        //    /// when priting the entire path
        //    /// </summary>
        //    /// <returns>The string representation of this node</returns>
        //    public abstract string AsString();

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="type"></param>
        //    /// <returns></returns>
        //    public static PathNode OfRoot(System.Type type) => new RootNode(type);

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="type"></param>
        //    /// <param name="id"></param>
        //    /// <param name="parent"></param>
        //    /// <returns></returns>
        //    public static PathNode Of(System.Type type, string id, PathNode parent)
        //    {
        //        ArgumentNullException.ThrowIfNull(type);
        //        ArgumentNullException.ThrowIfNull(id);

        //        if (int.TryParse(id, out _))
        //            return new IndexNode(type, id, parent);

        //        else return new PropertyNode(type, id, parent);
        //    }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="type"></param>
        //    /// <param name="index"></param>
        //    /// <param name="parent"></param>
        //    /// <returns></returns>
        //    public PathNode OfIndex(System.Type type, int index,  PathNode parent)
        //    {
        //        ArgumentNullException.ThrowIfNull(type);

        //        return new IndexNode(type, index.ToString(), parent);
        //    }
        //}

        public readonly struct RootNode : IPathNode
        {
            public string Id { get; }

            public IPathNode? Parent => null;

            public System.Type Type { get; }

            public int Depth => Parent is null ? 0 : Parent.Depth + 1;

            internal RootNode(System.Type type)
            {
                ArgumentNullException.ThrowIfNull(type);

                Type = type;
                Id = ".";
            }

            public string AsString() => Id;
        }

        public readonly struct PropertyNode : IPathNode
        {
            public string PropertyName => Id;

            public string Id { get; }

            public IPathNode? Parent { get; }

            public System.Type Type { get; }

            public int Depth => Parent is null ? 0 : Parent.Depth + 1;

            internal PropertyNode(
                System.Type type,
                string id,
                IPathNode? parent)
            {
                ArgumentNullException.ThrowIfNull(type);

                if (string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException($"Invaid id: null/empty/whitespace");

                Type = type;
                Parent = parent;
                Id = id;
            }

            public string AsString() => PropertyName;
        }

        public readonly struct IndexNode: IPathNode
        {
            public int Index { get; }

            public string Id { get; }

            public IPathNode? Parent { get; }

            public System.Type Type { get; }

            public int Depth => Parent is null ? 0 : Parent.Depth + 1;

            internal IndexNode(
                System.Type type,
                int id,
                IPathNode? parent)
            {
                ArgumentNullException.ThrowIfNull(type);

                if (id < 0)
                    throw new IndexOutOfRangeException($"Invalid index: < 0");

                Index = Index;
                Id = Index.ToString();
                Parent = parent;
                Type = type;
            }

            public string AsString() => $"#{Id}";
        }
        #endregion
    }
}
