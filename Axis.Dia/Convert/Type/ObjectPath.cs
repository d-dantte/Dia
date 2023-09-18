using Axis.Luna.Common;

namespace Axis.Dia.Convert.Type
{
    public readonly struct ObjectPath: IDefaultValueProvider<ObjectPath>
    {
        /// <summary>
        /// Current node in the object graph path
        /// </summary>
        public PathNode CurrentNode { get; }

        /// <summary>
        /// Current depth in the object graph path
        /// </summary>
        public int Depth { get; }

        #region DefaultValueProvider
        public bool IsDefault => default(ObjectPath).Equals(this);

        public static ObjectPath Default => default;
        #endregion

        internal ObjectPath(System.Type rootType)
        {
            CurrentNode = PathNode.OfRoot(rootType);
            Depth = CurrentNode.Depth;
        }

        private ObjectPath(PathNode node)
        {
            CurrentNode = node;
            Depth = node.Depth;
        }

        internal ObjectPath Next(System.Type type, string id)
            => new ObjectPath(PathNode.Of(type, id, CurrentNode));

        public override string ToString() => ToString(CurrentNode);

        private static string ToString(PathNode node)
        {
            var prefix = node.Parent is not null
                ? $"{ToString(node.Parent)}/"
                : "";

            return $"{prefix}{node.AsString()}";
        }

        #region nested types
        public abstract record PathNode
        {
            /// <summary>
            /// The ID of this node. In object-graph parlance, this is the property/index
            /// used to access the object represented by this node from the parent object.
            /// </summary>
            public string Id { get; }

            /// <summary>
            /// The parent of this node, if one exists
            /// </summary>
            public PathNode? Parent { get; }

            /// <summary>
            /// The clr type of the object represented by this path
            /// </summary>
            public System.Type Type { get; }

            /// <summary>
            /// Path depth
            /// </summary>
            public int Depth => Parent is null ? 0 : Parent.Depth + 1;


            protected PathNode(System.Type type, string id, PathNode? parent)
            {
                Type = type ?? throw new ArgumentNullException(nameof(type));
                Id = id ?? throw new ArgumentNullException(nameof(id));
                Parent = parent;
            }

            /// <summary>
            /// Gets a string representation of this node. used by the <see cref="ObjectPath"/>
            /// when priting the entire path
            /// </summary>
            /// <returns>The string representation of this node</returns>
            public abstract string AsString();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public static PathNode OfRoot(System.Type type) => new RootNode(type);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="id"></param>
            /// <param name="parent"></param>
            /// <returns></returns>
            public static PathNode Of(System.Type type, string id, PathNode parent)
            {
                ArgumentNullException.ThrowIfNull(type);
                ArgumentNullException.ThrowIfNull(id);

                if (int.TryParse(id, out _))
                    return new IndexNode(type, id, parent);

                else return new PropertyNode(type, id, parent);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="type"></param>
            /// <param name="index"></param>
            /// <param name="parent"></param>
            /// <returns></returns>
            public PathNode OfIndex(System.Type type, int index,  PathNode parent)
            {
                ArgumentNullException.ThrowIfNull(type);

                return new IndexNode(type, index.ToString(), parent);
            }
        }

        public record RootNode : PathNode
        {
            internal RootNode(System.Type type)
                : base(type, ".", null)
            {
            }

            public override string AsString() => Id;
        }

        public record PropertyNode : PathNode
        {
            public string PropertyName => Id;

            internal PropertyNode(
                System.Type type,
                string id,
                PathNode? parent)
                : base(type, id, parent)
            {
            }

            public override string AsString() => PropertyName;
        }

        public record IndexNode: PathNode
        {
            public int Index => int.Parse(Id); 
            
            internal IndexNode(
                System.Type type,
                string id,
                PathNode? parent)
                : base(type, id, parent)
            {
                if (!int.TryParse(id, out _))
                    throw new ArgumentException($"supplied argument is not a valid integer: {id}");
            }

            public override string AsString() => $"#{Id}";
        }
        #endregion
    }
}
