using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Static helper code
public static class TilesHelper
{

    public static GameObject GetNeighbor(GameObject currNode, int direction, List<GameObject> nodesList)
    {
        if (currNode != null)
        {
            int currX = currNode.GetComponent<Node>().x;
            int currY = currNode.GetComponent<Node>().y;
            //Note: this only works for top-left filling grids
            switch (direction)
            {
                case 0:
                    return GetForCoords(currX, currY - 1, nodesList);
                case 1:
                    return GetForCoords(currX + 1, currY, nodesList);
                case 2:
                    return GetForCoords(currX, currY + 1, nodesList);
                case 3:
                    return GetForCoords(currX - 1, currY, nodesList);
            }
        }
        return null;
    }

    static GameObject GetForCoords(int x, int y, List<GameObject> nodesList)
    {
        if (x < 0 || x >= AddButtons.boardWidth || y < 0 || y >= AddButtons.boardHeight)
        {
            return null;
        }

        int index = y * AddButtons.boardWidth + x;

        GameObject theNode = nodesList[index];
        return theNode;
    }

    // Counts the amount of distinct clusters of nodes for a given owner
    public static int CountClusters(int owner, List<GameObject> nodesList)
    {
        int clusterCounter = 0;

        Dictionary<GameObject, string> clustersMap = new Dictionary<GameObject, string>();

        foreach (GameObject current in nodesList)
        {
            if (current.GetComponent<Node>().owner == owner)
            {
                if (!clustersMap.ContainsKey(current))
                {
                    HashSet<GameObject> visited = new HashSet<GameObject>();
                    string foundName = ExploreTargets(current, visited, clustersMap, nodesList);
                    if (foundName == null)
                    {
                        clusterCounter++;
                        string currentClusterName = clusterCounter.ToString();
                        clustersMap.Add(current, currentClusterName);
                        TagTargets(currentClusterName, current, clustersMap, nodesList);
                    }
                }
            }
        }

        return clusterCounter;
    }

    static string ExploreTargets(GameObject current, HashSet<GameObject> visited, Dictionary<GameObject, string> clustersMap, List<GameObject> nodesList)
    {
        if (visited.Contains(current))
        {
            return null;
        }
        else
        {
            visited.Add(current);
        }

        int currentTargetDirection = current.GetComponent<Node>().direction;
        GameObject targetNode = GetNeighbor(current, currentTargetDirection, nodesList);
        if (targetNode != null)
        {
            string potentialCluster = null;
            if (clustersMap.TryGetValue(targetNode, out potentialCluster))
            {
                return potentialCluster;
            }
            else
            {
                return ExploreTargets(targetNode, visited, clustersMap, nodesList);
            }
        }


        return null;
    }

    private static void TagTargets(string clusterName, GameObject current, Dictionary<GameObject, string> clustersMap, List<GameObject> nodesList)
    {
        int currentTargetDirection = current.GetComponent<Node>().direction;
        GameObject targetNode = GetNeighbor(current, currentTargetDirection, nodesList);
        if (targetNode != null)
        {
            if (clustersMap.ContainsKey(targetNode))
            {
                return;
            }
            else
            {
                clustersMap.Add(targetNode, clusterName);
                TagTargets(clusterName, targetNode, clustersMap, nodesList);
            }
        }
    }

    public static int GetClusterSize(GameObject currentNode, List<GameObject> nodesList)
    {
        HashSet<GameObject> knownNodes = new HashSet<GameObject>();
        ExploreDown(currentNode, knownNodes, nodesList);

        return knownNodes.Count;
    }

    static void ExploreDown(GameObject current, HashSet<GameObject> knownNodes, List<GameObject> nodesList)
    {
        if (current == null)
        {
            return;
        }
        else if (knownNodes.Contains(current))
        {
            return;
        }
        knownNodes.Add(current);

        //Check 4 directions
        for (int i = 0; i < 4; i++)
        {
            GameObject neighbor = GetNeighbor(current, i, nodesList);
            if (neighbor != null)
            {
                int neighborTarget = neighbor.GetComponent<Node>().direction;
                if (neighborTarget == (i + 2) % 4)
                {
                    ExploreDown(neighbor, knownNodes, nodesList);
                }
            }
        }

        // Check downstream
        int currentTargetDirection = current.GetComponent<Node>().direction;
        GameObject targetNode = GetNeighbor(current, currentTargetDirection, nodesList);
        ExploreDown(targetNode, knownNodes, nodesList);
    }

    public static int GetSizeOfTargetCluster(GameObject candidateNode, int targetTeam, List<GameObject> nodesList)
    {
        int clusterSize = 0;
        int newDirection = (candidateNode.GetComponent<Node>().direction + 1) % 4;
        GameObject destinationNode = TilesHelper.GetNeighbor(candidateNode, newDirection, nodesList);
        if (destinationNode != null)
        {
            int targetOwner = destinationNode.GetComponent<Node>().owner;
            if (targetOwner == targetTeam)
            {
                clusterSize = TilesHelper.GetClusterSize(destinationNode, nodesList);
            }
        }
        return clusterSize;
    }

    // Returns the nodes that are behind the current one in the chain
    public static HashSet<GameObject> GetNodeBehind(GameObject current, List<GameObject> nodesList)
    {
        int team = current.GetComponent<Node>().owner;
        HashSet<GameObject> nodesBehind = new HashSet<GameObject>();

        GameObject nodeBehind2 = GetNeighbor(current, (current.GetComponent<Node>().direction + 2) % 4, nodesList);
        ValidateNodeBehind(current, nodesList, team, nodesBehind, nodeBehind2);

        GameObject nodeBehind3 = GetNeighbor(current, (current.GetComponent<Node>().direction + 3) % 4, nodesList);
        ValidateNodeBehind(current, nodesList, team, nodesBehind, nodeBehind3);

        return nodesBehind;
    }

    private static void ValidateNodeBehind(GameObject current, List<GameObject> nodesList, int team, HashSet<GameObject> nodesBehind, GameObject nodeBehind)
    {
        if (nodeBehind != null)
        {
            if (nodeBehind.GetComponent<Node>().owner == team)
            {
                GameObject targetOfBehind = GetNeighbor(nodeBehind, nodeBehind.GetComponent<Node>().direction, nodesList);
                if (targetOfBehind == current)
                {
                    nodesBehind.Add(nodeBehind);
                }
            }
        }
    }
}
