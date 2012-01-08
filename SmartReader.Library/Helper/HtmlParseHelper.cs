using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;
using System.Linq;

namespace SmartReader.Library.Helper
{
    public class HtmlParseHelper
    {
        public static IEnumerable<HtmlNode> Skip(IEnumerable<HtmlNode> nodes, int count )
        {
            if (nodes.Count() < Math.Abs(count)) return null;

            if (count > 0 )
            {
                return nodes.Skip(count);
            }
            else
            {
                var x = nodes.Reverse().Skip(Math.Abs(count));
                x = x.Reverse();
                return x;
            }
        }

        public static IEnumerable<HtmlNode> SubArray(IEnumerable<HtmlNode> nodes, int cutFromHead, int cutFromTail)
        {
            var count = nodes.Count();

            if (count < cutFromHead || cutFromHead > cutFromTail ) return null;

            var temp = nodes;
            temp = temp.Skip(cutFromHead);
            
            temp = temp.Reverse().Skip(cutFromTail);
            temp = temp.Reverse();
            return temp;
        }

        public static HtmlNode GetSingleDirectChildByType(HtmlNode node, string type )
        {
            if (node == null) return null;

            return GetSingleDirectChildByTypeAndIndex(node, type, 0);
        }

        public static HtmlNode GetSingleDirectChildByTypeAndIndex(HtmlNode node, string type, int index)
        {
            if (node == null) return null;

            var childs =
                node.DescendantNodes().Where(
                    n => n.Name.ToLower(CultureInfo.InvariantCulture) == type.ToLower(CultureInfo.InvariantCulture));


            int count = childs.Count();
            if (count == 0) return null;
            return childs.Skip(index).First();
        }

        public static HtmlNode  GetSingleChildByTypeChain (HtmlNode node, string [] typeList)
        {
            return typeList.Aggregate(node, GetSingleDirectChildByType);
        }

        public static IEnumerable<HtmlNode> GetDirectChildrenByType(HtmlNode node, string type)
        {
            if (node == null) return null;

            var childs =
                node.DescendantNodes().Where(
                    n => n.Name.ToLower(CultureInfo.InvariantCulture) == type.ToLower(CultureInfo.InvariantCulture));

            return childs;
        }

        public static HtmlNode FindElementById (HtmlNode node, string id)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Id.ToLower() == id.ToLower() )
                {
                    return child;
                }
                else
                {
                    var result = FindElementById(child, id);

                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        public static HtmlNode FindElementByClass(HtmlNode node, string className)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Attributes["class"] != null 
                    && child.Attributes["class"].Value.ToLower() == className.ToLower())
                {
                    return child;
                }
                else
                {
                    var result = FindElementById(child, className);

                    if (result != null)
                        return result;
                }
            }
            return null;
        }


        public static void RemoveNodeByType(HtmlNode node, string type,  List<HtmlNode> nodesToBeDeleted)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Name.ToLower() == type)
                {
                    nodesToBeDeleted.Add(child);
                }
                else
                {
                    RemoveNodeByType(child,type, nodesToBeDeleted);
                }
            }
        }


        public static void GetAllTextElement (HtmlNode node, List<HtmlNode> textNodeParents )
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.NodeType == HtmlNodeType.Text)
                {
                    if ( !String.IsNullOrWhiteSpace(child.InnerText) && child.InnerText.Trim().Length > 1)
                    {
                       if (!textNodeParents.Contains(child))
                            textNodeParents.Add(child);
                    }
                }

                GetAllTextElement(child, textNodeParents);
            }
        }

        public static void GetAllHyperlinkElementWithFilter(HtmlNode node, List<HtmlNode> hyperLinkNode)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Name.ToLower() == "a" 
                    && child.Attributes["target"] == null 
                    && child.Attributes["onclick"] == null
                    && child.InnerText.Trim().Length > 1
                    && !child.InnerText.Contains("最新章节")
                   )
                {
                    var href = child.Attributes["href"];
                    var hrefValue = href == null ? String.Empty : href.Value.ToLower();
                    if ( href != null
                        && !String.IsNullOrEmpty(hrefValue)
                        && !hrefValue.EndsWith("/") 
                        && !hrefValue.Contains("xiazai")
                        && !hrefValue.Contains("shuye")
                        && !hrefValue.Contains("javascript")
                        && !hrefValue.Contains("php")
                        && !hrefValue.Contains("list")
                        && !hrefValue.Contains("index")
                        && !hrefValue.Contains("mailto")
                       )
                    {
                        if (!hyperLinkNode.Contains(child))
                            hyperLinkNode.Add(child);
                    }
                    continue;
                }
                GetAllHyperlinkElementWithFilter(child, hyperLinkNode);
            }
        }

        public static void GetAllImageElementWithFilter(HtmlNode node, List<HtmlNode> imageNodes)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Name.ToLower() == "img")
                {
                    if (child.Attributes["src"] != null)
                    {
                        if (!imageNodes.Contains(child))
                            imageNodes.Add(child);
                    }
                    continue;
                }
                GetAllImageElementWithFilter(child, imageNodes);
            }
        }


        public static void RemoveNodeByClassName(HtmlNode node, string className, List<HtmlNode> nodesToBeDeleted)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Attributes["class"] == null ) continue;

                if (child.Attributes["class"].Value.ToLower() == className)
                {
                    nodesToBeDeleted.Add(child);
                }
                else
                {
                    RemoveNodeByType(child, className, nodesToBeDeleted);
                }
            }
        }

        public static void RemoveNodeById(HtmlNode node, string Id, List<HtmlNode> nodesToBeDeleted)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Id.ToLower() == Id.ToLower())
                {
                    nodesToBeDeleted.Add(child);
                    return;
                }
                else
                {
                    RemoveNodeByType(child, Id, nodesToBeDeleted);
                }
            }
        }


        /// <summary>
        /// <div style='display:none'>
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nodesToBeDeleted"></param>
        public static void RemoveNonDisplayNode(HtmlNode node , List<HtmlNode> nodesToBeDeleted)
        {
            foreach (var child in node.DescendantNodes())
            {
                if (child.Attributes["style"]!= null &&  child.Attributes["style"].Value.ToLower().Contains("display:none"))
                {
                    nodesToBeDeleted.Add(child);
                }
                else
                {
                    RemoveNonDisplayNode(child , nodesToBeDeleted);
                }
            }
        }
    }
}
