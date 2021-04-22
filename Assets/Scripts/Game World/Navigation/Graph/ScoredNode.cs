using System;
using System.Text;
using C5;

namespace GameBrains.AI {
    public class ScoredNode : IEquatable<ScoredNode>, IShowable, IFormattable {
        public readonly Edge edgeFromParent;
        public readonly float f;
        public readonly float g;
        public readonly Node node;
        public readonly ScoredNode parentScoredNode;

        [Tested]
        public ScoredNode(Node node, float f, float g, Edge edgeFromParent, ScoredNode parentScoredNode){
            this.node = node;
            this.f = f;
            this.g = g;
            this.edgeFromParent = edgeFromParent;
            this.parentScoredNode = parentScoredNode;
        }

        [Tested]
        public bool Equals(ScoredNode other){
            return (node == null ? other.node == null : node.Equals(other.node)) &&
                   f.Equals(other.f) &&
                   g.Equals(other.g) &&
                   (edgeFromParent == null
                       ? other.edgeFromParent == null
                       : edgeFromParent.Equals(other.edgeFromParent)) &&
                   (parentScoredNode == null
                       ? other.parentScoredNode == null
                       : parentScoredNode.Equals(other.parentScoredNode));
        }

        public bool Show(StringBuilder stringbuilder, ref int rest, IFormatProvider formatProvider){
            var flag = true;
            stringbuilder.Append("(");
            rest -= 2;

            try{
                if (flag = !Showing.Show(node, stringbuilder, ref rest, formatProvider)){
                    return false;
                }

                stringbuilder.Append(", ");
                rest -= 2;

                if (flag = !Showing.Show(f, stringbuilder, ref rest, formatProvider)){
                    return false;
                }

                stringbuilder.Append(", ");
                rest -= 2;

                if (flag = !Showing.Show(g, stringbuilder, ref rest, formatProvider)){
                    return false;
                }

                stringbuilder.Append(", ");
                rest -= 2;

                if (flag = !Showing.Show(edgeFromParent, stringbuilder, ref rest, formatProvider)){
                    return false;
                }

                stringbuilder.Append(", ");
                rest -= 2;

                if (flag = !Showing.Show(parentScoredNode, stringbuilder, ref rest, formatProvider)){
                    return false;
                }
            }
            finally{
                if (flag){
                    stringbuilder.Append("...");
                    rest -= 3;
                }

                stringbuilder.Append(")");
            }

            return true;
        }

        public string ToString(string format, IFormatProvider formatProvider){
            return Showing.ShowString(this, format, formatProvider);
        }

        [Tested]
        public override bool Equals(object obj){
            return obj is ScoredNode && Equals((ScoredNode) obj);
        }

        [Tested]
        public static bool operator ==(ScoredNode record1, ScoredNode record2){
            return record1.Equals(record2);
        }

        [Tested]
        public static bool operator !=(ScoredNode record1, ScoredNode record2){
            return !record1.Equals(record2);
        }

        [Tested]
        public override int GetHashCode(){
            var num = node == null ? 0 : node.GetHashCode();
            num = num * 0x5e8d1 + f.GetHashCode();
            num = num * 0x5e8d1 + g.GetHashCode();
            num = parentScoredNode == null ? 0 : parentScoredNode.GetHashCode();
            return num * 0x5e8d1 + (edgeFromParent == null ? 0 : edgeFromParent.GetHashCode());
        }

        public override string ToString(){
            return string.Format("({0}, {1}, {2}, {3}, {4})", node, f, g, edgeFromParent, parentScoredNode);
        }
    }
}