using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Otter {
    /// <summary>
    /// A Component that can manage a set of Bone Components that can move Entities around.  Sort of like a system
    /// similar to Spine or Spriter, but moving actual Entities around instead of just textures.
    /// </summary>
    public class Skeleton : Component {

        #region Public Fields

        /// <summary>
        /// Determines if the Skeleton should render debug displays for each Bone.
        /// </summary>
        public bool RenderBones;

        #endregion Public Fields

        #region Private Fields

        List<Bone> bones = new List<Bone>();
        Dictionary<string, Bone> bonesByString = new Dictionary<string, Bone>();
        Dictionary<Bone, string> stringByBones = new Dictionary<Bone, string>();

        #endregion Private Fields

        #region Public Properties

        /// <summary>
        /// The base bone (the first bone added to the Skeleton.)
        /// </summary>
        public Bone Base {
            get {
                return bones.First();
            }
        }

        #endregion Public Properties

        #region Public Indexers

        /// <summary>
        /// Get a Bone by enum name out of the Skeleton.
        /// </summary>
        /// <param name="name">The name of the bone to retrieve.</param>
        /// <returns>The Bone with that name.</returns>
        public Bone this[Enum name] {
            get {
                return GetBone(name);
            }
        }

        /// <summary>
        /// Get a Bone by enum name out of the Skeleton.
        /// </summary>
        /// <param name="name">The name of the bone to retrieve.</param>
        /// <returns>The Bone with that name.</returns>
        public Bone this[string name] {
            get {
                return GetBone(name);
            }
        }

        #endregion Public Indexers

        #region Public Methods

        /// <summary>
        /// Adds every Bone in the Skeleton's Entity to the Scene that the Skeleton's Entity is in.
        /// </summary>
        public void AddAllBonesToScene() {
            if (!Entity.IsInScene) return;
            foreach (var b in bones) {
                if (b.Entity == null) {
                    Entity.Scene.Add(b.Entity);
                    continue;
                }
                if (!b.Entity.IsInScene) {
                    Entity.Scene.Add(b.Entity);
                }
            }
        }

        /// <summary>
        /// Add a Bone to the Skeleton.
        /// </summary>
        /// <param name="bone">The Bone to add.</param>
        /// <returns>The added Bone.</returns>
        public Bone AddBone(Bone bone) {
            bones.Add(bone);
            bone.Skeleton = this;
            return bone;
        }

        /// <summary>
        /// Add a Bone to the Skeleton.
        /// </summary>
        /// <param name="parentName">The name of the Bone to parent this bone to.</param>
        /// <param name="name">The name of the Bone being added.</param>
        /// <param name="bone">The Bone being added.</param>
        /// <returns>The added Bone.</returns>
        public Bone AddBone(string parentName, string name, Bone bone = null) {
            bone = bone == null ? new Bone() : bone;
            var parent = bonesByString[parentName];
            parent.AddBone(bone);
            AddBone(name, bone);
            return bone;
        }

        /// <summary>
        /// Add a Bone to the Skeleton.
        /// </summary>
        /// <param name="parentName">The name of the Bone to parent this bone to.</param>
        /// <param name="name">The name of the Bone being added.</param>
        /// <param name="bone">The Bone being added.</param>
        /// <returns>The added Bone.</returns>
        public Bone AddBone(Enum parentName, Enum name, Bone bone = null) {
            return AddBone(Util.EnumValueToString(parentName), Util.EnumValueToString(name), bone);
        }

        /// <summary>
        /// Add a Bone to the Skeleton.
        /// </summary>
        /// <param name="parent">The parent Bone.</param>
        /// <param name="bone">The Bone being added.</param>
        /// <returns>The added Bone.</returns>
        public Bone AddBone(Bone parent, Bone bone = null) {
            bone = bone == null ? new Bone() : bone;
            parent.AddBone(bone);
            AddBone(bone);
            return bone;
        }

        /// <summary>
        /// Add a Bone to the Skeleton.
        /// </summary>
        /// <param name="name">The name of the Bone being added.</param>
        /// <param name="bone">The Bone being added.</param>
        /// <returns>The added Bone.</returns>
        public Bone AddBone(string name, Bone bone = null) {
            bone = bone == null ? new Bone() : bone;
            bonesByString.Add(name, bone);
            stringByBones.Add(bone, name); // Inverted dictionary for cross look up or something I dunno lol
            AddBone(bone);
            bone.Name = name;
            return bone;
        }

        /// <summary>
        /// Add a Bone to the Skeleton.
        /// </summary>
        /// <param name="id">The id of the bone being added.</param>
        /// <param name="bone">The Bone being added.</param>
        /// <returns>The added Bone.</returns>
        public Bone AddBone(Enum id, Bone bone = null) {
            return AddBone(Util.EnumValueToString(id), bone);
        }

        /// <summary>
        /// Retrieve a Bone by its string name.
        /// </summary>
        /// <param name="name">The name of the Bone.</param>
        /// <returns>The Bone with that name.</returns>
        public Bone GetBone(string name) {
            return bonesByString[name];
        }

        /// <summary>
        /// Retrieve a Bone by its Enum name.
        /// </summary>
        /// <param name="name">The name of the Bone.</param>
        /// <returns>The Bone with that name.</returns>
        public Bone GetBone(Enum name) {
            return GetBone(Util.EnumValueToString(name));
        }

        /// <summary>
        /// Retrieve a Bone by its int id.
        /// </summary>
        /// <param name="id">The id of the Bone.</param>
        /// <returns>The Bone with that id.</returns>
        public Bone GetBone(int id) {
            return bones[id];
        }

        /// <summary>
        /// Retrieve a Bone by its Entity.
        /// </summary>
        /// <param name="e">The Entity that the desired Bone belongs to.</param>
        /// <returns>The Bone with that Entity.</returns>
        public Bone GetBone(Entity e) {
            foreach (var b in bones) {
                if (b.Entity == e) return b;
            }
            return null;
        }

        /// <summary>
        /// A list of all the Bones contained in this Skeleton.
        /// </summary>
        /// <returns>A list of Bones.</returns>
        public List<Bone> GetBones() {
            return bones.ToList<Bone>();
        }

        /// <summary>
        /// Load a Skeleton from XML data.
        /// </summary>
        /// <param name="xml">The XML data to parse.</param>
        public void LoadXml(string xml) {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            var xmlSkeleton = xmlDoc.SelectSingleNode("//skeleton");

            ParseXml(xmlSkeleton.ChildNodes);
        }

        /// <summary>
        /// Remove all the Entities controlled by the Bones in this Skeleton from the Scene.
        /// </summary>
        public void RemoveAllBonesFromScene() {
            if (!Entity.IsInScene) return;
            foreach (var b in bones) {
                if (b.Entity.IsInScene) {
                    Entity.Scene.Remove(b.Entity);
                }
            }
        }

        /// <summary>
        /// Remove a Bone from the Skeleton.
        /// </summary>
        /// <param name="name">The name of the Bone to remove.</param>
        /// <param name="bone">The Bone to return if no Bone was removed.</param>
        /// <returns>The removed Bone, or the Bone passed into the method if no Bone was removed.</returns>
        public Bone RemoveBone(string name, Bone bone = null) {
            bone = bone == null ? new Bone() : bone;
            if (bonesByString.ContainsKey(name)) {
                stringByBones.Remove(bonesByString[name]);
                bone = bonesByString[name];
                bonesByString.Remove(name);
                bones.RemoveIfContains(bone);
                bone.Skeleton = null;
            }

            return bone;
        }

        /// <summary>
        /// Remove a Bone from the Skeleton.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Bone RemoveBone(Enum id) {
            return RemoveBone(Util.EnumValueToString(id));
        }

        /// <summary>
        /// Remove a Bone from the Skeleton.
        /// </summary>
        /// <param name="bone"></param>
        /// <returns></returns>
        public Bone RemoveBone(Bone bone) {
            if (stringByBones.ContainsKey(bone)) {
                bonesByString.Remove(stringByBones[bone]);
                stringByBones.Remove(bone);
            }
            bones.RemoveIfContains(bone);
            bone.Skeleton = null;

            return bone;
        }

        public override void Render() {
            base.Render();

            if (RenderBones) {
                foreach (var b in bones) {
                    b.Render();
                }
            }
        }

        public override void Update() {
            base.Update();

            if (Base != null) {
                Base.LocalX = Entity.X;
                Base.LocalY = Entity.Y;
                Base.UpdateTransforms();
            }
        }

        #endregion Public Methods

        #region Private Methods

        void ParseXml(XmlNodeList nodes, string parent = "") {
            foreach (XmlNode node in nodes) {
                var bone = new Bone(
                    node.AttributeFloat("x", 0),
                    node.AttributeFloat("y", 0),
                    node.AttributeFloat("rotation", 0),
                    node.AttributeFloat("scaleX", 1),
                    node.AttributeFloat("scaleY", 1)
                    );

                bone.FlipGraphicY = node.AttributeBool("flipGraphicY", false);
                bone.FlipGraphicX = node.AttributeBool("flipGraphicX", false);
                bone.LocalFlipX = node.AttributeBool("flipX", false);
                bone.LocalFlipY = node.AttributeBool("flipY", false);

                var boneName = node.AttributeString("name", "base");

                var boneEntityType = Util.GetTypesFromAllAssemblies<Entity>(node.AttributeString("entity")).FirstOrDefault();
                var boneEntity = (Entity)Activator.CreateInstance(boneEntityType, 0, 0);
                bone.SetEntity(boneEntity);

                if (parent == "") {
                    AddBone(boneName, bone);
                }
                else {
                    AddBone(parent, boneName, bone);
                }

                if (node.ChildNodes.Count > 0) {
                    ParseXml(node.ChildNodes, boneName);
                }
            }
        }

        #endregion Private Methods
    }
}
